using AcmHouse.AccommodationInfo;
using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace AcmHouse.AccommodationExtraInfo
{
    public class AccommodationDetailsManager
    {
        public SQLiteManager manager;
        public List<AccommodationDetails> accommodationDetailsList;

        public AccommodationDetailsManager()
        {
            manager = new SQLiteManager();
            accommodationDetailsList = new List<AccommodationDetails>();
            GetAccommodationDetails();
        }

        public void AddAccommodationDetails(AccommodationDetails accommodationDetails)
        {
            manager.OpenConnection();

            string insertQuery = "INSERT INTO AccommodationDetails (AccommodationID, Adress, FlatNumber) " +
                                 "VALUES (@AccommodationID, @Adress, @FlatNumber)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@AccommodationID", accommodationDetails.AccommodationID);
                command.Parameters.AddWithValue("@Adress", accommodationDetails.Address);
                command.Parameters.AddWithValue("@FlatNumber", accommodationDetails.FlatNumber);

                command.ExecuteNonQuery();
            }
            manager.CloseConnection();
        }

        public void GetAccommodationDetails()
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM AccommodationDetails";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int acID = Convert.ToInt32(reader["AccommodationID"]);
                        string acAddress = reader["Adress"].ToString();
                        string acFlat = reader["FlatNumber"].ToString();

                        AccommodationDetails accommodationDetails = new AccommodationDetails(acID, acAddress, acFlat);
                        accommodationDetailsList.Add(accommodationDetails);
                    }
                }
            }
            manager.CloseConnection();
        }
        public int GetLastAccommodationID()
        {
            manager.OpenConnection();

            string selectQuery = "SELECT MAX(ID) FROM Accommodations";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                int lastAccommodationID = Convert.ToInt32(command.ExecuteScalar());
                manager.CloseConnection();
                return lastAccommodationID;
            }
        }
    }
}
