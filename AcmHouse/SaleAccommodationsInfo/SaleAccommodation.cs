using AcmHouse.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.SaleAccommodationsInfo
{
    public class SaleAccommodation
    {
        private static int lastID = 0;
        public int ID { get; set; }
        public int SellerID {get;set;}

        private string name;
        private AccommodationType accommodationType;
        private string adress;
        private string flatNumber;
        private int amountOfRooms;
        private double area;
        private double price;
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
        public string Adress
        {
            get { return adress; }
            set { adress = value; }
        }
        public string FlatNumber
        {
            get { return flatNumber; }
            set { flatNumber = value; }
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
        public double Price
        {
            get { return price; }
            set { price = value; }
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

        public SaleAccommodation(int sellerID, string name, AccommodationType accommodationType, string adress, string flatNumber, int amountOfRooms, double area, double price, int imageID, int rentalType)
        {
            ID = ++lastID;
            SellerID = sellerID;
            Name = name;
            AccommodationType = accommodationType;
            Adress = adress;
            FlatNumber = flatNumber;
            AmountOfRooms = amountOfRooms;
            Area = area;
            Price = price;
            ImageID = imageID;
            RentalType = rentalType;
        }
        public SaleAccommodation(int id, int sellerID, string name, AccommodationType accommodationType, string adress, string flatNumber, int amountOfRooms, double area, double price, int imageID, int rentalType)
        {
            ID = id;
            SellerID = sellerID;
            Name = name;
            AccommodationType = accommodationType;
            Adress = adress;
            FlatNumber = flatNumber;
            AmountOfRooms = amountOfRooms;
            Area = area;
            Price = price;
            ImageID = imageID;
            RentalType = rentalType;
        }
    }
}
