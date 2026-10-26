using System;
using AcmHouse.AccommodationInfo;
using AcmHouse.Data;
using AcmHouse.Services;

namespace AcmHouse.ViewModels
{
    /// <summary>What a property card shows: photo, name, type, rooms, area, address and price.</summary>
    public class OfferCardViewModel
    {
        public OfferCardViewModel(AccommodationOffer offer)
        {
            Offer = offer ?? throw new ArgumentNullException(nameof(offer));
        }

        public AccommodationOffer Offer { get; }

        public Accommodation Accommodation => Offer.Accommodation;

        public string Name => Offer.Accommodation.Name;

        public string TypeName => Offer.Accommodation.AccommodationType.ToString();

        public int Rooms => Offer.Accommodation.AmountOfRooms;

        public double Area => Offer.Accommodation.Area;

        public string Address => Offer.Details.Address;

        public double DayPrice => Offer.Accommodation.DayPrice;

        public string ImagePath => AppPaths.GetImagePath(Offer.Accommodation.ImageID);

        public bool Matches(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return true;
            }

            string query = searchText.Trim();
            return Contains(Address, query) || Contains(Name, query);
        }

        private static bool Contains(string text, string query)
        {
            return text != null && text.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
