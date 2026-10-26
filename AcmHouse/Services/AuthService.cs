using System;
using System.Data.SQLite;
using AcmHouse.Enums;
using AcmHouse.Security;
using AcmHouse.SQL_Manager;
using AcmHouse.UsersInfo;

namespace AcmHouse.Services
{
    public interface IAuthService
    {
        /// <summary>Checks the credentials and, on success, sets CurrentUserManager.CurrentUser.</summary>
        LoginResult SignIn(string email, string password);
    }

    /// <summary>Signs in employees (Users table) and administrators (Administrators table).</summary>
    public class AuthService : IAuthService
    {
        public LoginResult SignIn(string email, string password)
        {
            using (SQLiteConnection connection = SQLiteManager.CreateOpenConnection())
            {
                CurrentUser employee = FindEmployee(connection, email, password);
                if (employee != null)
                {
                    CurrentUserManager.CurrentUser = employee;
                    return LoginResult.Employee;
                }

                CurrentUser administrator = FindAdministrator(connection, email, password);
                if (administrator != null)
                {
                    CurrentUserManager.CurrentUser = administrator;
                    return LoginResult.Administrator;
                }
            }

            return LoginResult.Failed;
        }

        private static CurrentUser FindEmployee(SQLiteConnection connection, string email, string password)
        {
            const string query = "SELECT * FROM Users WHERE Email = @Email LIMIT 1";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    string passwordHash = reader["Password"].ToString();
                    if (!PasswordHasher.Verify(password, passwordHash))
                    {
                        return null;
                    }

                    return new CurrentUser(
                        Convert.ToInt32(reader["ID"]),
                        reader["Name"].ToString(),
                        reader["Surname"].ToString(),
                        reader["Email"].ToString(),
                        passwordHash,
                        reader["PassportNumber"].ToString(),
                        reader["IdentificationNumber"].ToString(),
                        Convert.ToInt32(reader["Shift"]) != 0);
                }
            }
        }

        private static CurrentUser FindAdministrator(SQLiteConnection connection, string email, string password)
        {
            const string query = "SELECT * FROM Administrators WHERE Email = @Email LIMIT 1";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    string passwordHash = reader["PasswordHash"].ToString();
                    if (!PasswordHasher.Verify(password, passwordHash))
                    {
                        return null;
                    }

                    return new CurrentUser(reader["Name"].ToString(), reader["Surname"].ToString(), email, passwordHash, false);
                }
            }
        }
    }
}
