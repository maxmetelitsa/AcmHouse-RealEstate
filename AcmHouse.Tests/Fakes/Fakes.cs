using System;
using System.Collections.Generic;
using System.Linq;
using AcmHouse.AccommodationExtraInfo;
using AcmHouse.AccommodationInfo;
using AcmHouse.Data;
using AcmHouse.Domain;
using AcmHouse.Enums;
using AcmHouse.LandlordInfo;
using AcmHouse.Services;
using AcmHouse.UsersInfo;

namespace AcmHouse.Tests.Fakes
{
    internal static class TestData
    {
        public static AccommodationOffer Offer(int id, string name, string address, double dayPrice = 100, int rentalType = 0)
        {
            var accommodation = new Accommodation(id, name, AccommodationType.Апартаменты, 2, 54.5, dayPrice, id, rentalType);
            return new AccommodationOffer(accommodation, new AccommodationDetails(id, address, "12"));
        }

        public static CurrentUser Employee(bool shiftOpen = true)
        {
            return new CurrentUser(1, "Иван", "Петров", "ivan.petrov@acmhouse.by", "hash", "MP1234567", "3010190A001PB1", shiftOpen);
        }
    }

    internal class FakeAuthService : IAuthService
    {
        public LoginResult Result { get; set; } = LoginResult.Failed;

        public int Calls { get; private set; }

        public LoginResult SignIn(string email, string password)
        {
            Calls++;
            return Result;
        }
    }

    internal class FakeAccommodationRepository : IAccommodationRepository
    {
        private readonly List<AccommodationOffer> offers;

        public FakeAccommodationRepository(params AccommodationOffer[] offers)
        {
            this.offers = offers.ToList();
        }

        public IReadOnlyList<AccommodationOffer> GetOffers(int rentalType)
        {
            return offers.Where(o => o.Accommodation.RentalType == rentalType).ToList();
        }

        public AccommodationOffer GetOffer(int accommodationId)
        {
            return offers.FirstOrDefault(o => o.Accommodation.ID == accommodationId);
        }
    }

    internal class FakeRentalRepository : IRentalRepository
    {
        public bool Available { get; set; } = true;

        public List<Landlord> KnownClients { get; } = new List<Landlord>();

        public List<RentalRecord> SavedRentals { get; } = new List<RentalRecord>();

        public int NextOrderId { get; set; } = 42;

        public bool IsAvailable(int accommodationId, DateTime startDate, DateTime endDate)
        {
            return Available;
        }

        public IReadOnlyList<Landlord> SearchClients(string text)
        {
            return KnownClients
                .Where(c => c.Surname.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0
                         || c.Name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)
                .ToList();
        }

        public int SaveRental(RentalRecord rental)
        {
            SavedRentals.Add(rental);
            return NextOrderId;
        }
    }

    internal class FakeContractService : IContractService
    {
        public int CreatedForOrder { get; private set; } = -1;

        public bool Fail { get; set; }

        public void CreateRentalContracts(int orderId, Accommodation accommodation, AccommodationDetails details,
            CurrentUser employee, RentalForm form, RentalQuote quote)
        {
            if (Fail)
            {
                throw new InvalidOperationException("disk is full");
            }

            CreatedForOrder = orderId;
        }
    }

    internal class FakeMessageService : IMessageService
    {
        public List<string> Infos { get; } = new List<string>();

        public List<string> Errors { get; } = new List<string>();

        public void ShowInfo(string message, string title = "")
        {
            Infos.Add(message);
        }

        public void ShowError(string message, string title = "")
        {
            Errors.Add(message);
        }
    }
}
