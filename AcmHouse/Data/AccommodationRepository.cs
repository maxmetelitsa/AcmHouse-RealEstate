using System;
using System.Collections.Generic;
using System.Data.SQLite;
using AcmHouse.AccommodationExtraInfo;
using AcmHouse.AccommodationInfo;
using AcmHouse.Enums;
using AcmHouse.SQL_Manager;

namespace AcmHouse.Data
{
    /// <summary>A property for rent together with its address.</summary>
    public class AccommodationOffer
    {
        public AccommodationOffer(Accommodation accommodation, AccommodationDetails details)
        {
            Accommodation = accommodation ?? throw new ArgumentNullException(nameof(accommodation));
            Details = details ?? throw new ArgumentNullException(nameof(details));
        }

        public Accommodation Accommodation { get; }

        public AccommodationDetails Details { get; }
    }

    public interface IAccommodationRepository
    {
        /// <summary>Offers of one rental type (0 = residential, 1 = commercial) that have an address.</summary>
        IReadOnlyList<AccommodationOffer> GetOffers(int rentalType);

        /// <summary>The offer with the given ID, or null if it does not exist or has no address.</summary>
        AccommodationOffer GetOffer(int accommodationId);
    }

    public class AccommodationRepository : IAccommodationRepository
    {
        private const string SelectOffers = @"
            SELECT a.ID, a.Name, a.AccommodationType, a.AmountOfRooms, a.Area, a.DayPrice, a.ImageID, a.RentalType,
                   d.Adress, d.FlatNumber
            FROM Accommodations a
            JOIN AccommodationDetails d
              ON d.ID = (SELECT MIN(ID) FROM AccommodationDetails WHERE AccommodationID = a.ID)";

        public IReadOnlyList<AccommodationOffer> GetOffers(int rentalType)
        {
            return Query(SelectOffers + " WHERE a.RentalType = @Value ORDER BY a.ID", rentalType);
        }

        public AccommodationOffer GetOffer(int accommodationId)
        {
            IReadOnlyList<AccommodationOffer> offers = Query(SelectOffers + " WHERE a.ID = @Value", accommodationId);
            return offers.Count > 0 ? offers[0] : null;
        }

        private static IReadOnlyList<AccommodationOffer> Query(string query, int value)
        {
            var offers = new List<AccommodationOffer>();

            using (SQLiteConnection connection = SQLiteManager.CreateOpenConnection())
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Value", value);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["ID"]);

                        var accommodation = new Accommodation(
                            id,
                            reader["Name"].ToString(),
                            (AccommodationType)Convert.ToInt32(reader["AccommodationType"]),
                            Convert.ToInt32(reader["AmountOfRooms"]),
                            Convert.ToDouble(reader["Area"]),
                            Convert.ToDouble(reader["DayPrice"]),
                            Convert.ToInt32(reader["ImageID"]),
                            Convert.ToInt32(reader["RentalType"]));

                        var details = new AccommodationDetails(id, reader["Adress"].ToString(), reader["FlatNumber"].ToString());

                        offers.Add(new AccommodationOffer(accommodation, details));
                    }
                }
            }

            return offers;
        }
    }
}
