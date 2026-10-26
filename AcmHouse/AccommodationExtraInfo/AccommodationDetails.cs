using AcmHouse.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.AccommodationExtraInfo
{
    public class AccommodationDetails
    {
        private static int lastID = 0;
        public int ID { get; set; }

        private int accommodationID;
        private string address;
        private string flatNumber;

        public int AccommodationID
        {
            get { return accommodationID; }
            set { accommodationID = value; }
        }
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public string FlatNumber
        {
            get { return flatNumber; }
            set { flatNumber = value; }
        }

        public AccommodationDetails(int accommodationID, string address, string flatNumber)
        {
            ID = ++lastID;
            AccommodationID = accommodationID;
            Address = address;
            FlatNumber = flatNumber;
        }
    }
}
