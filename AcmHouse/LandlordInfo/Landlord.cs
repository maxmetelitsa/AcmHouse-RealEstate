using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.LandlordInfo
{
    public class Landlord
    {
        private static int lastID = 0;
        private int id;
        private string name;
        private string surname;
        private string email;
        private string phone;
        private string passportNumber;
        private string identificationNumber;
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
        public string Phone
        {
            get { return phone; }
            set { phone = value; }
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
        public Landlord(int id, string name, string surname, string email, string phone, string passportNumber, string identificationNumber)
        {
            ID = id;
            Name = name;
            Surname = surname;
            Email = email;
            Phone = phone;
            PassportNumber = passportNumber;
            IdentificationNumber = identificationNumber;
        }

        public Landlord(string name, string surname, string email, string phone, string passportNumber, string identificationNumber)
        {
            ID = ++lastID;
            Name = name;
            Surname = surname;
            Email = email;
            Phone = phone;
            PassportNumber = passportNumber;
            IdentificationNumber = identificationNumber;
        }
    }
}
