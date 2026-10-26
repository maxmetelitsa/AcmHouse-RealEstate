using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.SellersInfo
{
    public class Seller
    {
        public int ID { get; set; }
        public string Initials { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PassportNumber { get; set; }
        public string IdentificationNumber { get; set; }

        public Seller(string initials, string surname, string email, string phone, string passportNumber, string identificationNumber)
        {
            Initials = initials;
            Surname = surname;
            Email = email;
            Phone = phone;
            PassportNumber = passportNumber;
            IdentificationNumber = identificationNumber;
        }

        public Seller(int id, string initials, string surname, string email, string phone, string passportNumber, string identificationNumber)
        {
            ID = id;
            Initials = initials;
            Surname = surname;
            Email = email;
            Phone = phone;
            PassportNumber = passportNumber;
            IdentificationNumber = identificationNumber;
        }
    }
}
