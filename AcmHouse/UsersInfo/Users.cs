using AcmHouse.Security;
using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace AcmHouse.UsersInfo
{
    public class Users
    {
        public List<User> users;
        public SQLiteManager manager = new SQLiteManager();
        public Users()
        {
            users = new List<User>();
            GetAllUsers(manager);
        }
        public bool AddUser(SQLiteManager manager, string name, string surname, string email, string password, string passportNumber, string identificationNumber)
        {
            string passwordHash = PasswordHasher.Hash(password);

            manager.OpenConnection();

            string insertQuery = " INSERT INTO Users (Name, Surname, Email, Password, PassportNumber, IdentificationNumber, Shift) VALUES (@Name, @Surname, @Email, @Password, @PassportNumber, @IdentificationNumber, @Shift);";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Surname", surname);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", passwordHash);
                command.Parameters.AddWithValue("@PassportNumber", passportNumber);
                command.Parameters.AddWithValue("@IdentificationNumber", identificationNumber);
                command.Parameters.AddWithValue("@Shift", false);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    users.Add(new User(name, surname, email, passwordHash, passportNumber, identificationNumber, false));
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

        public void RemoveUser(User user, int ID)
        {
            manager.OpenConnection();

            string deleteQuery = "DELETE FROM [Users] WHERE ID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", ID);
                command.ExecuteNonQuery();
            }

            manager.CloseConnection();
            users.Remove(user);
        }


        public void GetAllUsers(SQLiteManager manager)
        {
            users.Clear();
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM Users";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["Name"].ToString();
                        string surname = reader["Surname"].ToString();
                        string email = reader["Email"].ToString();
                        string password = reader["Password"].ToString();
                        string passportNumber = reader["PassportNumber"].ToString();
                        string identificationNumber = reader["IdentificationNumber"].ToString();
                        int shiftInt = Convert.ToInt32(reader["Shift"]); // Получаем значение из базы данных

                        bool shift = (shiftInt != 0); // Преобразуем int в bool


                        users.Add(new User(name, surname, email, password, passportNumber, identificationNumber, shift));
                    }
                }
            }

            manager.CloseConnection();
        }

    }
}
