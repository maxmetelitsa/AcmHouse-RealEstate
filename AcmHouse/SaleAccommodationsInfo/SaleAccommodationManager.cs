using AcmHouse.AccommodationInfo;
using AcmHouse.Enums;
using AcmHouse.SellersInfo;
using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace AcmHouse.SaleAccommodationsInfo
{
    public class SaleAccommodationManager
    {
        public SQLiteManager manager;
        public SaleAccommodations accommodationsList;
        public SaleAccommodationManager()
        {
            manager = new SQLiteManager();
            accommodationsList = new SaleAccommodations();
        }
        public void GetAllAccommodations()
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM SaleAccommodations";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Int64 id = reader.GetInt64(reader.GetOrdinal("ID"));
                        Int64 sellerID = reader.GetInt64(reader.GetOrdinal("SellerID"));
                        string name = reader.GetString(reader.GetOrdinal("Name"));
                        AccommodationType type = (AccommodationType)reader.GetInt32(reader.GetOrdinal("AccommodationType"));
                        string adress = reader.GetString(reader.GetOrdinal("Adress"));
                        string flatNumber = reader.GetString(reader.GetOrdinal("FlatNumber"));
                        int amountOfRooms = reader.GetInt32(reader.GetOrdinal("AmountOfRooms"));
                        double area = reader.GetDouble(reader.GetOrdinal("Area"));
                        double price = reader.GetDouble(reader.GetOrdinal("Price"));
                        int imageID = reader.GetInt32(reader.GetOrdinal("ImageID"));
                        int rentalType = reader.GetInt32(reader.GetOrdinal("RentalType"));

                        SaleAccommodation accommodation = new SaleAccommodation((int)id, (int)sellerID, name, type, adress, flatNumber, amountOfRooms, area, price, imageID, rentalType);

                        accommodationsList.saleAccommodations.Add(accommodation);
                    }
                }
            }

            manager.CloseConnection();
        }

        public void AddAccommodation(SaleAccommodation accommodation, Seller seller)
        {
            manager.OpenConnection();

            string insertQuery = "INSERT INTO SaleAccommodations (ID, SellerID, Name, AccommodationType, Adress, FlatNumber, AmountOfRooms, Area, Price, ImageID, RentalType) " +
                                 "VALUES (@ID, @SellerID, @Name, @AccommodationType, @Adress, @FlatNumber, @AmountOfRooms, @Area, @Price, @ImageID, @RentalType)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", accommodation.ID);
                command.Parameters.AddWithValue("@SellerID", seller.ID);
                command.Parameters.AddWithValue("@Name", accommodation.Name);
                command.Parameters.AddWithValue("@AccommodationType", (int)accommodation.AccommodationType);
                command.Parameters.AddWithValue("@Adress", accommodation.Adress);
                command.Parameters.AddWithValue("@FlatNumber", accommodation.FlatNumber);
                command.Parameters.AddWithValue("@AmountOfRooms", accommodation.AmountOfRooms);
                command.Parameters.AddWithValue("@Area", accommodation.Area);
                command.Parameters.AddWithValue("@Price", accommodation.Price);
                command.Parameters.AddWithValue("@ImageID", accommodation.ImageID);
                command.Parameters.AddWithValue("@RentalType", accommodation.RentalType);

                command.ExecuteNonQuery();
            }

            accommodationsList.saleAccommodations.Add(accommodation);

            manager.CloseConnection();
        }

        public void RemoveAccommodation(SaleAccommodation accommodation)
        {
            manager.OpenConnection();

            string selectQuery = "SELECT ID FROM SaleAccommodations WHERE Name = @Name AND AccommodationType = @AccommodationType AND Adress = @Adress AND FlatNumber = @FlatNumber AND AmountOfRooms = @AmountOfRooms AND Area = @Area AND Price = @Price AND ImageID = @ImageID AND RentalType = @RentalType";

            int acId;

            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, manager.Connection))
            {
                selectCommand.Parameters.AddWithValue("@Name", accommodation.Name);
                selectCommand.Parameters.AddWithValue("@AccommodationType", (int)accommodation.AccommodationType);
                selectCommand.Parameters.AddWithValue("@Adress", accommodation.Adress);
                selectCommand.Parameters.AddWithValue("@FlatNumber", accommodation.FlatNumber);
                selectCommand.Parameters.AddWithValue("@AmountOfRooms", accommodation.AmountOfRooms);
                selectCommand.Parameters.AddWithValue("@Area", accommodation.Area);
                selectCommand.Parameters.AddWithValue("@Price", accommodation.Price);
                selectCommand.Parameters.AddWithValue("@ImageID", accommodation.ImageID);
                selectCommand.Parameters.AddWithValue("@RentalType", accommodation.RentalType);

                using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        acId = reader.GetInt32(0);
                    }
                    else
                    {
                        return; 
                    }
                }
            }

            string deleteQuery = "DELETE FROM SaleAccommodations WHERE ID = @ID";
            using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                deleteCommand.Parameters.AddWithValue("@ID", acId);
                deleteCommand.ExecuteNonQuery();
            }

            accommodationsList.saleAccommodations.Remove(accommodation);

            manager.CloseConnection();
        }
    }
}
