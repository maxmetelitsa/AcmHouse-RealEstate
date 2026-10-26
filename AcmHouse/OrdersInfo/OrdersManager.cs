using AcmHouse.AccommodationInfo;
using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace AcmHouse.OrdersInfo
{
    public class OrdersManager
    {
        public SQLiteManager manager;
        public Orders ordersList;

        public OrdersManager()
        {
            manager = new SQLiteManager();
            ordersList = new Orders();
        }

        public void AddOrder(int userID, int accommodationID, DateTime startDate, DateTime endDate, double totalPrice)
        {
            manager.OpenConnection();

            string insertQuery = "INSERT INTO [Order] (UserID, AccommodationID, StartDate, EndDate, TotalPrice) VALUES (@UserID, @AccommodationID, @StartDate, @EndDate, @TotalPrice)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@AccommodationID", accommodationID);
                command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@TotalPrice", totalPrice);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    ordersList.orders.Add(new Order(userID, accommodationID, startDate, endDate, totalPrice));
                    manager.CloseConnection();
                }
                else
                {
                    manager.CloseConnection();
                }
            }
        }

        public void UpdateAllOrders()
        {
            manager.OpenConnection();

            foreach (Order order in ordersList.orders)
            {
                string updateQuery = $"UPDATE [Order] SET UserID = @UserID, AccommodationID = @AccommodationID, StartDate = @StartDate, EndDate = @EndDate, TotalPrice = @TotalPrice WHERE ID = @ID";

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, manager.Connection))
                {
                    command.Parameters.AddWithValue("@UserID", order.UserID);
                    command.Parameters.AddWithValue("@AccommodationID", order.AccommodationID);
                    command.Parameters.AddWithValue("@StartDate", order.StartDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@EndDate", order.EndDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                    command.Parameters.AddWithValue("@ID", order.ID);

                    command.ExecuteNonQuery();
                }
            }

            manager.CloseConnection();
        }

        public void GetAllOrders()
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM [Order]";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Int64 userID = reader.GetInt64(reader.GetOrdinal("UserID"));
                        Int64 accommodationID = reader.GetInt64(reader.GetOrdinal("AccommodationID"));

                        string startDateText = reader.GetString(reader.GetOrdinal("StartDate"));
                        string endDateText = reader.GetString(reader.GetOrdinal("EndDate"));

                        DateTime startDate = DateTime.Parse(startDateText);
                        DateTime endDate = DateTime.Parse(endDateText);

                        double totalPrice = reader.GetDouble(reader.GetOrdinal("TotalPrice"));

                        ordersList.orders.Add(new Order((int)userID, (int)accommodationID, startDate, endDate, totalPrice));
                    }
                }
            }

            manager.CloseConnection();
        }
        public int GetLastOrderID()
        {
            int lastID = -1;

            manager.OpenConnection();

            string selectQuery = "SELECT MAX(ID) FROM [Order]";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                object result = command.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    lastID = Convert.ToInt32(result);
                }
            }

            manager.CloseConnection();

            return lastID;
        }

        public void AddExtraInfo(int orderID, DateTime date)
        {
            manager.OpenConnection();

            string insertQuery = "INSERT INTO OrderDetails (OrderID, Date) " +
                                 "VALUES (@OrderID, @Date)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@OrderID", orderID);
                command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                command.ExecuteNonQuery();
            }

            manager.CloseConnection();
        }
        public void RemoveOrder(Order order, int ID)
        {
            manager.OpenConnection();

            string deleteQuery = "DELETE FROM [Order] WHERE ID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", ID);
                command.ExecuteNonQuery();
            }

            manager.CloseConnection();
            ordersList.orders.Remove(order);
        }
        public void ClearOrdersTable()
        {
            manager.OpenConnection();

            string deleteQuery = "DELETE FROM [Order]";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                command.ExecuteNonQuery();
            }

            manager.CloseConnection();

            ordersList.orders.Clear();
        }
    }
}
