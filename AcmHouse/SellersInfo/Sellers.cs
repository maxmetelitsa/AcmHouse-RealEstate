using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace AcmHouse.SellersInfo
{
    public class Sellers
    {
        public List<Seller> sellers;
        public SQLiteManager manager = new SQLiteManager();
        public Sellers()
        {
            sellers = new List<Seller>();
            GetAllSellers(manager);
        }

        public void AddSeller(SQLiteManager manager, string initials, string surname, string email, string phone, string passportNumber, string identificationNumber)
        {
            string insertQuery = "INSERT INTO Sellers (Initials, Surname, Email, Phone, PassportNumber, IdentificationNumber) VALUES (@Initials, @Surname, @Email, @Phone, @PassportNumber, @IdentificationNumber);";

            try
            {
                manager.OpenConnection();
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
                {
                    command.Parameters.AddWithValue("@Initials", initials);
                    command.Parameters.AddWithValue("@Surname", surname);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@PassportNumber", passportNumber);
                    command.Parameters.AddWithValue("@IdentificationNumber", identificationNumber);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding seller: {ex.Message}");
            }
            finally
            {
                manager.CloseConnection();
            }
        }



        public Seller GetLastSeller(SQLiteManager manager)
        {
            string selectQuery = "SELECT * FROM Sellers ORDER BY ID DESC LIMIT 1;";

            try
            {
                manager.OpenConnection();
                using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Seller(
                                reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.GetString(reader.GetOrdinal("Initials")),
                                reader.GetString(reader.GetOrdinal("Surname")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                reader.GetString(reader.GetOrdinal("Phone")),
                                reader.GetString(reader.GetOrdinal("PassportNumber")),
                                reader.GetString(reader.GetOrdinal("IdentificationNumber"))
                            );
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching last seller: {ex.Message}");
                return null;
            }
            finally
            {
                manager.CloseConnection();
            }
        }
        public void GetAllSellers(SQLiteManager manager)
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM Sellers";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string initials = reader["Initials"].ToString();
                        string surname = reader["Surname"].ToString();
                        string email = reader["Email"].ToString();
                        string phone = reader["Phone"].ToString();
                        string passportNumber = reader["PassportNumber"].ToString();
                        string identificationNumber = reader["IdentificationNumber"].ToString();

                        sellers.Add(new Seller(initials, surname, email, phone, passportNumber, identificationNumber));
                    }
                }
            }

            manager.CloseConnection();
        }

    }
}
