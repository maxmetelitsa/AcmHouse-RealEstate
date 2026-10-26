using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.UsersInfo
{
    public class User
    {
        private static int lastID = 0;
        public int ID { get; }
        private string name;
        private string surname;
        private string email;
        private string password;
        private string passportNumber;
        private string identificationNumber;
        private bool shift;
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

        public User(string name, string surname, string email, string password, string passportNumber, string identificationNumber, bool shift)
        {
            ID = ++lastID;
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
            PassportNumber = passportNumber;
            IdentificationNumber = identificationNumber;
            Shift = shift;
        }
    }
}
