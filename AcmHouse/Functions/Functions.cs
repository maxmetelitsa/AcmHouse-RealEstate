using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace AcmHouse.Functions
{
    public class Functions
    {
        public static bool IsValidEmail(string email)
        {
            return Domain.EmailValidator.IsValid(email);
        }
        public static int GetLastOfAllAccommodationsID()
        {
            SQLiteManager manager = new SQLiteManager();
            int lastAccommodationID = 0;
            int lastSaleAccommodationID = 0;
            manager.OpenConnection();

            try
            {
                string selectQuery1 = "SELECT MAX(ID) FROM Accommodations";
                using (SQLiteCommand command1 = new SQLiteCommand(selectQuery1, manager.Connection))
                {
                    object result1 = command1.ExecuteScalar();
                    if (result1 != DBNull.Value)
                    {
                        lastAccommodationID = Convert.ToInt32(result1);
                    }
                }

                string selectQuery2 = "SELECT MAX(ID) FROM SaleAccommodations";
                using (SQLiteCommand command2 = new SQLiteCommand(selectQuery2, manager.Connection))
                {
                    object result2 = command2.ExecuteScalar();
                    if (result2 != DBNull.Value)
                    {
                        lastSaleAccommodationID = Convert.ToInt32(result2);
                    }
                }

                int lastID = Math.Max(lastAccommodationID, lastSaleAccommodationID);
                return lastID;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return -1;
            }
            finally
            {
                manager.CloseConnection();
            }
        }
    }
}
