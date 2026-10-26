using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcmHouse.Enums;

namespace AcmHouse.AccommodationInfo
{
    public class Accommodation
    {
        private static int lastID = 0;
        public int ID { get; set; }

        private string name;
        private AccommodationType accommodationType;
        private int amountOfRooms;
        private double area;
        private double dayPrice;
        private int imageId;
        private int category;
        private int rentalType;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public AccommodationType AccommodationType
        {
            get { return accommodationType; }
            set { accommodationType = value; }
        }
        public int AmountOfRooms
        {
            get { return amountOfRooms; }
            set { amountOfRooms = value; }
        }
        public double Area
        {
            get { return area; }
            set { area = value; }
        }
        public double DayPrice
        {
            get { return dayPrice; }
            set { dayPrice = value; }
        }
        public int ImageID
        {
            get { return imageId; }
            set { imageId = value; }
        }
        public int Category
        {
            get { return category; }
            set { category = value; }
        }
        public int RentalType
        {
            get { return rentalType; }
            set { rentalType = value; }
        }

        public Accommodation(string name, AccommodationType accommodationType, int amountOfRooms, double area, double dayPrice, int imageID, int rentalType)
        {
            ID = ++lastID;
            Name = name;
            AccommodationType = accommodationType;
            AmountOfRooms = amountOfRooms;
            Area = area;
            DayPrice = dayPrice;
            ImageID = imageID;
            RentalType = rentalType;
        }
        public Accommodation(int id, string name, AccommodationType accommodationType, int amountOfRooms, double area, double dayPrice, int imageID, int rentalType)
        {
            ID = id;
            Name = name;
            AccommodationType = accommodationType;
            AmountOfRooms = amountOfRooms;
            Area = area;
            DayPrice = dayPrice;
            ImageID = imageID;
            RentalType = rentalType;
        }
    }
}
