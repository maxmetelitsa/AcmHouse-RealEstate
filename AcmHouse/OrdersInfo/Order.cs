using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.OrdersInfo
{
    public class Order
    {
        private static int lastID = 0;
        public int ID { get; }
        private int userID;
        private int accommodationID;
        private DateTime startDate;
        private DateTime endDate;
        private double totalPrice;
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }
        public int AccommodationID
        {
            get { return accommodationID; }
            set { accommodationID = value; }
        }
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }
        public double TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }

        public Order(int userID, int accommodationID, DateTime startDate, DateTime endDate, double totalPrice)
        {
            ID = lastID;
            UserID = userID;
            AccommodationID = accommodationID;
            StartDate = startDate;
            EndDate = endDate;
            TotalPrice = totalPrice;
        }
    }
}
