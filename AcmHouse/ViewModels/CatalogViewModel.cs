using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AcmHouse.Data;
using AcmHouse.Mvvm;

namespace AcmHouse.ViewModels
{
    /// <summary>Catalog of residential (0) or commercial (1) offers with search by address or name.</summary>
    public class CatalogViewModel : ViewModelBase
    {
        private readonly List<OfferCardViewModel> allOffers;
        private string searchText = string.Empty;

        public CatalogViewModel(int rentalType, IAccommodationRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            RentalType = rentalType;
            allOffers = repository.GetOffers(rentalType).Select(offer => new OfferCardViewModel(offer)).ToList();
            Offers = new ObservableCollection<OfferCardViewModel>(allOffers);
        }

        public int RentalType { get; }

        public ObservableCollection<OfferCardViewModel> Offers { get; }

        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    ApplyFilter();
                }
            }
        }

        private void ApplyFilter()
        {
            Offers.Clear();
            foreach (OfferCardViewModel offer in allOffers.Where(o => o.Matches(SearchText)))
            {
                Offers.Add(offer);
            }
        }
    }
}
