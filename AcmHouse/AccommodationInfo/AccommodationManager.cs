using AcmHouse.Enums;
using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AcmHouse.Functions;
using System.Data;

namespace AcmHouse.AccommodationInfo
{
    public class AccommodationManager
    {
        public SQLiteManager manager;
        public Accommodations accommodationsList;
        public Accommodations availableAccommodationsList;
        public AccommodationManager()
        {
            manager = new SQLiteManager();
            accommodationsList = new Accommodations();
            availableAccommodationsList = new Accommodations();
        }

        public void GetAllAccommodations()
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM Accommodations";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Int64 id = reader.GetInt64(reader.GetOrdinal("ID"));
                        string name = reader.GetString(reader.GetOrdinal("Name"));
                        AccommodationType type = (AccommodationType)reader.GetInt32(reader.GetOrdinal("AccommodationType"));
                        int amountOfRooms = reader.GetInt32(reader.GetOrdinal("AmountOfRooms"));
                        double area = reader.GetDouble(reader.GetOrdinal("Area"));
                        double dayPrice = reader.GetDouble(reader.GetOrdinal("DayPrice"));
                        int imageID = reader.GetInt32(reader.GetOrdinal("ImageID"));
                        int rentalType = reader.GetInt32(reader.GetOrdinal("RentalType"));

                        Accommodation accommodation = new Accommodation((int)id, name, type, amountOfRooms, area, dayPrice, imageID, rentalType);

                        accommodationsList.accommodations.Add(accommodation);
                    }
                }
            }

            manager.CloseConnection();
        }

        public void AddAccommodation(Accommodation accommodation)
        {
            manager.OpenConnection();

            string insertQuery = "INSERT INTO Accommodations (ID, Name, AccommodationType, AmountOfRooms, Area, DayPrice, ImageID, RentalType) " +
                                 "VALUES (@ID, @Name, @AccommodationType, @AmountOfRooms, @Area, @DayPrice, @ImageID, @RentalType)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", accommodation.ID);
                command.Parameters.AddWithValue("@Name", accommodation.Name);
                command.Parameters.AddWithValue("@AccommodationType", (int)accommodation.AccommodationType);
                command.Parameters.AddWithValue("@AmountOfRooms", accommodation.AmountOfRooms);
                command.Parameters.AddWithValue("@Area", accommodation.Area);
                command.Parameters.AddWithValue("@DayPrice", accommodation.DayPrice);
                command.Parameters.AddWithValue("@ImageID", accommodation.ImageID);
                command.Parameters.AddWithValue("@RentalType", accommodation.RentalType);

                command.ExecuteNonQuery();
            }

            accommodationsList.accommodations.Add(accommodation);

            manager.CloseConnection();
        }
        public void AddCommercialAccommodation(Accommodation accommodation)
        {
            manager.OpenConnection();

            string insertQuery = "INSERT INTO Accommodations (@ID, ID, Name, AccommodationType, AmountOfRooms, Area, DayPrice, ImageID, RentalType) " +
                                 "VALUES (@Name, @AccommodationType, @AmountOfRooms, @Area, @DayPrice, @ImageID, @RentalType)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", accommodation.ID);
                command.Parameters.AddWithValue("@Name", accommodation.Name);
                command.Parameters.AddWithValue("@AccommodationType", (int)accommodation.AccommodationType);
                command.Parameters.AddWithValue("@AmountOfRooms", accommodation.AmountOfRooms);
                command.Parameters.AddWithValue("@Area", accommodation.Area);
                command.Parameters.AddWithValue("@DayPrice", accommodation.DayPrice);
                command.Parameters.AddWithValue("@ImageID", accommodation.ImageID);
                command.Parameters.AddWithValue("@RentalType", accommodation.RentalType);

                command.ExecuteNonQuery();
            }
            
            accommodationsList.accommodations.Add(accommodation);

            manager.CloseConnection();
        }

        public void RemoveAccommodation(Accommodation accommodation)
        {

            manager.OpenConnection();

            string selectQuery = "SELECT ID FROM Accommodations WHERE Name = @Name AND AccommodationType = @AccommodationType AND AmountOfRooms = @AmountOfRooms AND Area = @Area AND DayPrice = @DayPrice AND ImageID = @ImageID AND RentalType = @RentalType";


            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, manager.Connection))
            {
                selectCommand.Parameters.AddWithValue("@Name", accommodation.Name);
                selectCommand.Parameters.AddWithValue("@AccommodationType", (int)accommodation.AccommodationType);
                selectCommand.Parameters.AddWithValue("@AmountOfRooms", accommodation.AmountOfRooms);
                selectCommand.Parameters.AddWithValue("@Area", accommodation.Area);
                selectCommand.Parameters.AddWithValue("@DayPrice", accommodation.DayPrice);
                selectCommand.Parameters.AddWithValue("@ImageID", accommodation.ImageID);
                selectCommand.Parameters.AddWithValue("@RentalType", accommodation.RentalType);
            }

            string deleteQuery = "DELETE FROM Accommodations WHERE ID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", accommodation.ID);

                command.ExecuteNonQuery();
            }

            manager.CloseConnection();
        }
        public void RemoveCommercialAccommodation(Accommodation accommodation)
        {

            manager.OpenConnection();

            string selectQuery = "SELECT ID FROM Accommodations WHERE Name = @Name AND AccommodationType = @AccommodationType AND AmountOfRooms = @AmountOfRooms AND Area = @Area AND DayPrice = @DayPrice AND ImageID = @ImageID AND RentalType = @RentalType";

            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, manager.Connection))
            {
                selectCommand.Parameters.AddWithValue("@Name", accommodation.Name);
                selectCommand.Parameters.AddWithValue("@AccommodationType", (int)accommodation.AccommodationType);
                selectCommand.Parameters.AddWithValue("@AmountOfRooms", accommodation.AmountOfRooms);
                selectCommand.Parameters.AddWithValue("@Area", accommodation.Area);
                selectCommand.Parameters.AddWithValue("@DayPrice", accommodation.DayPrice);
                selectCommand.Parameters.AddWithValue("@ImageID", accommodation.ImageID);
                selectCommand.Parameters.AddWithValue("@RentalType", accommodation.RentalType);

                
            }

            string deleteQuery = "DELETE FROM Accommodations WHERE ID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", accommodation.ID);

                command.ExecuteNonQuery();
            }

            manager.CloseConnection();
        }

        public void ClearAccomodationsTable()
        {
            manager.OpenConnection();

            string deleteQuery = "DELETE FROM Accommodations";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                command.ExecuteNonQuery();
            }

            manager.CloseConnection();

            accommodationsList.accommodations.Clear();
        }

    }

}
