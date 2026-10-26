using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace AcmHouse.UserOrders
{
    public class UserOrders
    {
        private List<UserOrder> userOrders; // Возможно, останется для временного хранения
        private SQLiteManager manager;

        public UserOrders()
        {
            userOrders = new List<UserOrder>();
            manager = new SQLiteManager();
        }

        public void AddUserOrder(int orderID, int userID, int rentalContractNumber)
        {
            manager.OpenConnection();

            string insertQuery = "INSERT INTO UserOrders (OrderID, UserID, RentalСontractNumber) VALUES (@OrderID, @UserID, @RentalСontractNumber)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@OrderID", orderID);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@RentalСontractNumber", rentalContractNumber);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    // Нет необходимости добавлять в локальную коллекцию, так как будем использовать базу данных
                    manager.CloseConnection();
                }
                else
                {
                    manager.CloseConnection();
                    // Обработка ошибки
                }
            }
        }

        public List<UserOrder> GetAllUserOrders()
        {
            List<UserOrder> userOrders = new List<UserOrder>();

            manager.OpenConnection();

            string selectQuery = "SELECT * FROM UserOrders";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Int64 orderID = reader.GetInt64(reader.GetOrdinal("OrderID"));
                        Int64 userID = reader.GetInt64(reader.GetOrdinal("UserID"));
                        Int64 rentalID = reader.GetInt64(reader.GetOrdinal("RentalСontractNumber"));

                        userOrders.Add(new UserOrder((int)orderID, (int)userID, (int)rentalID));
                    }
                }
            }

            manager.CloseConnection();

            return userOrders;
        }

        public List<UserOrder> GetUserOrders(int id)
        {
            List<UserOrder> currentUserOrders = new List<UserOrder>();

            manager.OpenConnection();

            string selectQuery = "SELECT * FROM UserOrders WHERE UserID = @UserID";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@UserID", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Int64 orderID = reader.GetInt64(reader.GetOrdinal("OrderID"));
                        Int64 rentalID = reader.GetInt64(reader.GetOrdinal("RentalСontractNumber"));

                        currentUserOrders.Add(new UserOrder((int)orderID, id, (int)rentalID));
                    }
                }
            }

            manager.CloseConnection();

            return currentUserOrders;
        }
    }
}
