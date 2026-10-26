using AcmHouse.SQL_Manager;
using AcmHouse.UsersInfo;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace AcmHouse.LandlordInfo
{
    public class Landlords
    {
        public List<Landlord> landlords;
        public SQLiteManager manager;
        public Landlords()
        {
            manager = new SQLiteManager();
            landlords = new List<Landlord>();
            GetAllUsers();
        }
        public bool AddUser(string name, string surname, string email, string phone, string passportNumber, string identificationNumber)
        {
            manager.OpenConnection();

            string insertQuery = " INSERT INTO Landlords (Name, Surname, Email, Phone, PassportNumber, IdentificationNumber) VALUES (@Name, @Surname, @Email, @Phone, @PassportNumber, @IdentificationNumber);";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Surname", surname);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Phone", phone);
                command.Parameters.AddWithValue("@PassportNumber", passportNumber);
                command.Parameters.AddWithValue("@IdentificationNumber", identificationNumber);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    landlords.Add(new Landlord(name, surname, email, phone, passportNumber, identificationNumber));
                    manager.CloseConnection();
                    return true;
                }
                else
                {
                    manager.CloseConnection();
                    return false;
                }
            }
        }
        public void GetAllUsers()
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM Landlords";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["Name"].ToString();
                        string surname = reader["Surname"].ToString();
                        string email = reader["Email"].ToString();
                        string phone = reader["Phone"].ToString();
                        string passportNumber = reader["PassportNumber"].ToString();
                        string identificationNumber = reader["IdentificationNumber"].ToString();
                        landlords.Add(new Landlord(name, surname, email, phone, passportNumber, identificationNumber));
                    }
                }
            }

            manager.CloseConnection();
        }
    }
}
