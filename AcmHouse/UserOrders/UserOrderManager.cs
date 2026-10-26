using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.UserOrders
{
    public class UserOrderManager
    {
        public UserOrders userOrdersList;
        public SQLiteManager manager = new SQLiteManager();
        public UserOrderManager()
        {
            userOrdersList = new UserOrders();
        }
    }
}
