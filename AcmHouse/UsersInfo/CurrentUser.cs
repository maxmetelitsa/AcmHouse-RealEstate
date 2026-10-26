using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace AcmHouse.UsersInfo
{
    public class CurrentUser
    {
        public int id;
        string name;
        string surname;
        string email;
        string password;
        string passportNumber;
        string identificationNumber;
        bool shift;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string PassportNumber
        {
            get { return passportNumber; }
            set { passportNumber = value; }
        }
        public string IdentificationNumber
        {
            get { return identificationNumber; }
            set { identificationNumber = value; }
        }
        public bool Shift
        {
            get { return shift; }
            set { shift = value; }
        }

        public CurrentUser(int id, string name, string surname, string email, string password, string passportNumber, string identificationNumber, bool shift)
        {
            ID = id;
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
            PassportNumber = passportNumber;
            IdentificationNumber = identificationNumber;
            Shift = shift;
        }
        public CurrentUser(string name, string surname, string email, string password, bool shift)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
            Shift = shift;
        }
        public void UpdateUserInfo(SQLiteManager manager)
        {
            manager.OpenConnection();

            CurrentUser currentUser = CurrentUserManager.CurrentUser;
            int userID = currentUser.ID;

            string updateQuery = @"
        UPDATE Users
        SET 
            Name = @Name,
            Surname = @Surname,
            Email = @Email,
            Password = @Password,
            PassportNumber = @PassportNumber,
            IdentificationNumber = @IdentificationNumber,
            Shift = @Shift
        WHERE ID = @UserID";

            using (SQLiteCommand command = new SQLiteCommand(updateQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@Name", currentUser.Name);
                command.Parameters.AddWithValue("@Surname", currentUser.Surname);
                command.Parameters.AddWithValue("@Email", currentUser.Email);
                command.Parameters.AddWithValue("@Password", currentUser.Password);
                command.Parameters.AddWithValue("@PassportNumber", currentUser.PassportNumber);
                command.Parameters.AddWithValue("@IdentificationNumber", currentUser.IdentificationNumber);
                command.Parameters.AddWithValue("@Shift", currentUser.Shift);
                command.Parameters.AddWithValue("@UserID", userID);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Информация о пользователе успешно обновлена.");
                }
                else
                {
                    Console.WriteLine("Не удалось обновить информацию о пользователе.");
                }
            }

            manager.CloseConnection();
        }


    }
}
