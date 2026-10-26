using System;

namespace AcmHouse.UserOrders
{
    public class UserOrder
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public int RentalContractNumber { get; set; }
        public string BookingText { get; set; }

        public UserOrder(int orderID, int userID, int rentalContractNumber)
        {
            ID = orderID;
            OrderID = orderID;
            UserID = userID;
            RentalContractNumber = rentalContractNumber;
        }
    }
}
