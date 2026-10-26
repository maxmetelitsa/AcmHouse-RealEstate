using System;
using System.Collections.Generic;
using System.Data.SQLite;
using AcmHouse.LandlordInfo;
using AcmHouse.SQL_Manager;

namespace AcmHouse.Data
{
    /// <summary>Everything that has to be stored when a rental contract is signed.</summary>
    public class RentalRecord
    {
        public int EmployeeId { get; set; }
        public int AccommodationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalPrice { get; set; }
        public double EmployeeProfit { get; set; }
        public DateTime SignedOn { get; set; }

        /// <summary>The client; stored in the Landlords table if not there yet.</summary>
        public Landlord Client { get; set; }
    }

    public interface IRentalRepository
    {
        /// <summary>True if the property has no rental overlapping the given period.</summary>
        bool IsAvailable(int accommodationId, DateTime startDate, DateTime endDate);

        IReadOnlyList<Landlord> SearchClients(string text);

        /// <summary>Saves the rental in one transaction and returns the new order (contract) number.</summary>
        int SaveRental(RentalRecord rental);
    }

    public class RentalRepository : IRentalRepository
    {
        private const string DateFormat = "yyyy-MM-dd";

        public bool IsAvailable(int accommodationId, DateTime startDate, DateTime endDate)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM [Order]
                WHERE AccommodationID = @AccommodationID
                  AND StartDate <= @EndDate
                  AND EndDate >= @StartDate";

            using (SQLiteConnection connection = SQLiteManager.CreateOpenConnection())
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AccommodationID", accommodationId);
                command.Parameters.AddWithValue("@StartDate", startDate.ToString(DateFormat));
                command.Parameters.AddWithValue("@EndDate", endDate.ToString(DateFormat));

                return Convert.ToInt64(command.ExecuteScalar()) == 0;
            }
        }

        public IReadOnlyList<Landlord> SearchClients(string text)
        {
            // SQLite's LIKE ignores case only for Latin letters, so Cyrillic names are matched here in C#.
            const string query = @"
                SELECT ID, Name, Surname, Email, Phone, PassportNumber, IdentificationNumber
                FROM Landlords
                ORDER BY Surname";
            const int maxResults = 20;

            var clients = new List<Landlord>();

            using (SQLiteConnection connection = SQLiteManager.CreateOpenConnection())
            using (var command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read() && clients.Count < maxResults)
                    {
                        string name = reader["Name"].ToString();
                        string surname = reader["Surname"].ToString();
                        if (!ContainsIgnoreCase(name, text) && !ContainsIgnoreCase(surname, text))
                        {
                            continue;
                        }

                        clients.Add(new Landlord(
                            Convert.ToInt32(reader["ID"]),
                            name,
                            surname,
                            reader["Email"].ToString(),
                            reader["Phone"].ToString(),
                            reader["PassportNumber"].ToString(),
                            reader["IdentificationNumber"].ToString()));
                    }
                }
            }

            return clients;
        }

        private static bool ContainsIgnoreCase(string value, string text)
        {
            return value.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        public int SaveRental(RentalRecord rental)
        {
            if (rental == null)
            {
                throw new ArgumentNullException(nameof(rental));
            }

            using (SQLiteConnection connection = SQLiteManager.CreateOpenConnection())
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                int orderId = InsertOrder(connection, transaction, rental);

                Execute(connection, transaction,
                    "INSERT INTO UserOrders (OrderID, UserID, RentalСontractNumber) VALUES (@OrderID, @UserID, @OrderID)",
                    ("@OrderID", orderId), ("@UserID", rental.EmployeeId));

                Execute(connection, transaction,
                    "INSERT INTO OrderDetails (OrderID, Date) VALUES (@OrderID, @Date)",
                    ("@OrderID", orderId), ("@Date", rental.SignedOn.ToString(DateFormat)));

                Execute(connection, transaction,
                    "INSERT INTO OrdersProfit (OrderID, UserID, Profit) VALUES (@OrderID, @UserID, @Profit)",
                    ("@OrderID", orderId), ("@UserID", rental.EmployeeId), ("@Profit", rental.EmployeeProfit));

                if (rental.Client != null && !ClientExists(connection, transaction, rental.Client))
                {
                    InsertClient(connection, transaction, rental.Client);
                }

                transaction.Commit();
                return orderId;
            }
        }

        private static int InsertOrder(SQLiteConnection connection, SQLiteTransaction transaction, RentalRecord rental)
        {
            Execute(connection, transaction,
                "INSERT INTO [Order] (UserID, AccommodationID, StartDate, EndDate, TotalPrice) " +
                "VALUES (@UserID, @AccommodationID, @StartDate, @EndDate, @TotalPrice)",
                ("@UserID", rental.EmployeeId),
                ("@AccommodationID", rental.AccommodationId),
                ("@StartDate", rental.StartDate.ToString(DateFormat)),
                ("@EndDate", rental.EndDate.ToString(DateFormat)),
                ("@TotalPrice", rental.TotalPrice));

            using (var command = new SQLiteCommand("SELECT last_insert_rowid()", connection, transaction))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private static bool ClientExists(SQLiteConnection connection, SQLiteTransaction transaction, Landlord client)
        {
            const string query = "SELECT COUNT(*) FROM Landlords WHERE PassportNumber = @PassportNumber AND IdentificationNumber = @IdentificationNumber";

            using (var command = new SQLiteCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@PassportNumber", client.PassportNumber);
                command.Parameters.AddWithValue("@IdentificationNumber", client.IdentificationNumber);
                return Convert.ToInt64(command.ExecuteScalar()) > 0;
            }
        }

        private static void InsertClient(SQLiteConnection connection, SQLiteTransaction transaction, Landlord client)
        {
            Execute(connection, transaction,
                "INSERT INTO Landlords (Name, Surname, Email, Phone, PassportNumber, IdentificationNumber) " +
                "VALUES (@Name, @Surname, @Email, @Phone, @PassportNumber, @IdentificationNumber)",
                ("@Name", client.Name),
                ("@Surname", client.Surname),
                ("@Email", client.Email),
                ("@Phone", client.Phone),
                ("@PassportNumber", client.PassportNumber),
                ("@IdentificationNumber", client.IdentificationNumber));
        }

        private static void Execute(SQLiteConnection connection, SQLiteTransaction transaction, string sql,
            params (string Name, object Value)[] parameters)
        {
            using (var command = new SQLiteCommand(sql, connection, transaction))
            {
                foreach (var (name, value) in parameters)
                {
                    command.Parameters.AddWithValue(name, value);
                }

                command.ExecuteNonQuery();
            }
        }
    }
}
