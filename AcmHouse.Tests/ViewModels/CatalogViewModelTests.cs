using System.Linq;
using AcmHouse.Tests.Fakes;
using AcmHouse.ViewModels;
using Xunit;

namespace AcmHouse.Tests.ViewModels
{
    public class CatalogViewModelTests
    {
        private static CatalogViewModel CreateCatalog()
        {
            var repository = new FakeAccommodationRepository(
                TestData.Offer(1, "Квартира в центре", "проспект Независимости 10"),
                TestData.Offer(2, "Уютная квартира", "улица Немига 5"),
                TestData.Offer(3, "Офис на Немиге", "улица Немига 12", rentalType: 1));

            return new CatalogViewModel(0, repository);
        }

        [Fact]
        public void Constructor_LoadsOnlyOffersOfTheRequestedType()
        {
            CatalogViewModel catalog = CreateCatalog();

            Assert.Equal(new[] { 1, 2 }, catalog.Offers.Select(o => o.Accommodation.ID));
        }

        [Fact]
        public void SearchText_FiltersByAddressIgnoringCase()
        {
            CatalogViewModel catalog = CreateCatalog();

            catalog.SearchText = "немига";

            Assert.Equal(2, Assert.Single(catalog.Offers).Accommodation.ID);
        }

        [Fact]
        public void SearchText_AlsoMatchesTheOfferName()
        {
            CatalogViewModel catalog = CreateCatalog();

            catalog.SearchText = "в центре";

            Assert.Equal(1, Assert.Single(catalog.Offers).Accommodation.ID);
        }

        [Fact]
        public void ClearingSearchText_ShowsAllOffersAgain()
        {
            CatalogViewModel catalog = CreateCatalog();
            catalog.SearchText = "немига";

            catalog.SearchText = "  ";

            Assert.Equal(2, catalog.Offers.Count);
        }

        [Fact]
        public void SearchText_WithNoMatches_LeavesCatalogEmpty()
        {
            CatalogViewModel catalog = CreateCatalog();

            catalog.SearchText = "Гродно";

            Assert.Empty(catalog.Offers);
        }
    }
}
