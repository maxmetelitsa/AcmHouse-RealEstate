using System;
using AcmHouse.Tests.Fakes;
using AcmHouse.Data;
using AcmHouse.LandlordInfo;
using AcmHouse.ViewModels;
using Xunit;

namespace AcmHouse.Tests.ViewModels
{
    public class RentAccommodationViewModelTests
    {
        private static readonly DateTime Today = new DateTime(2026, 5, 1, 10, 0, 0);

        private readonly FakeRentalRepository repository = new FakeRentalRepository();
        private readonly FakeContractService contracts = new FakeContractService();
        private readonly FakeMessageService messages = new FakeMessageService();

        private RentAccommodationViewModel CreateViewModel(bool shiftOpen = true, double dayPrice = 100)
        {
            return new RentAccommodationViewModel(
                TestData.Offer(7, "Квартира в центре", "проспект Независимости 10", dayPrice),
                TestData.Employee(shiftOpen),
                repository,
                contracts,
                messages,
                () => Today);
        }

        private static void FillValidForm(RentAccommodationViewModel viewModel)
        {
            viewModel.Surname = "Сидоренко";
            viewModel.Initials = "О.П.";
            viewModel.Email = "client@example.com";
            viewModel.Phone = "+375 29 000-00-01";
            viewModel.PassportNumber = "MP1234567";
            viewModel.IdentificationNumber = "3010190A001PB1";
            viewModel.StartDate = Today.Date.AddDays(1);
            viewModel.EndDate = Today.Date.AddDays(11);
        }

        [Fact]
        public void ChangingDates_RecalculatesDaysAndTotal()
        {
            RentAccommodationViewModel viewModel = CreateViewModel(dayPrice: 150);

            viewModel.StartDate = new DateTime(2026, 5, 2);
            viewModel.EndDate = new DateTime(2026, 5, 6);

            Assert.Equal("4", viewModel.DaysText);
            Assert.Equal("600", viewModel.TotalText);
            Assert.Equal(string.Empty, viewModel.EndDateError);
        }

        [Fact]
        public void TooLongPeriod_ShowsErrorUnderEndDate()
        {
            RentAccommodationViewModel viewModel = CreateViewModel();

            viewModel.StartDate = new DateTime(2026, 5, 2);
            viewModel.EndDate = new DateTime(2026, 7, 2);

            Assert.Equal(string.Empty, viewModel.TotalText);
            Assert.Contains("не должно превышать", viewModel.EndDateError);
        }

        [Fact]
        public void Accept_WithClosedShift_DoesNotSaveAnything()
        {
            RentAccommodationViewModel viewModel = CreateViewModel(shiftOpen: false);
            FillValidForm(viewModel);

            viewModel.Accept();

            Assert.Empty(repository.SavedRentals);
            Assert.Contains("смену", Assert.Single(messages.Infos));
        }

        [Fact]
        public void Accept_WithInvalidForm_ShowsFieldErrorsAndDoesNotSave()
        {
            RentAccommodationViewModel viewModel = CreateViewModel();
            FillValidForm(viewModel);
            viewModel.Email = "client@";
            viewModel.PassportNumber = string.Empty;

            viewModel.Accept();

            Assert.Equal("Введите корректный email", viewModel.EmailError);
            Assert.Equal("Поле должно быть заполнено", viewModel.PassportNumberError);
            Assert.Equal(string.Empty, viewModel.SurnameError);
            Assert.Empty(repository.SavedRentals);
        }

        [Fact]
        public void Accept_WhenDatesAreTaken_ShowsErrorAndDoesNotSave()
        {
            repository.Available = false;
            RentAccommodationViewModel viewModel = CreateViewModel();
            FillValidForm(viewModel);

            viewModel.Accept();

            Assert.Empty(repository.SavedRentals);
            Assert.Contains("уже арендовано", Assert.Single(messages.Errors));
        }

        [Fact]
        public void Accept_WithValidData_SavesRentalCreatesContractsAndCompletes()
        {
            RentAccommodationViewModel viewModel = CreateViewModel(dayPrice: 100);
            FillValidForm(viewModel);
            bool completed = false;
            viewModel.RentalCompleted += () => completed = true;

            viewModel.AcceptCommand.Execute(null);

            RentalRecord saved = Assert.Single(repository.SavedRentals);
            Assert.Equal(1, saved.EmployeeId);
            Assert.Equal(7, saved.AccommodationId);
            Assert.Equal(1000, saved.TotalPrice);
            Assert.Equal(105, saved.EmployeeProfit, 6);
            Assert.Equal("Сидоренко", saved.Client.Surname);
            Assert.Equal(repository.NextOrderId, contracts.CreatedForOrder);
            Assert.Contains("№42", Assert.Single(messages.Infos));
            Assert.True(completed);
        }

        [Fact]
        public void Accept_WhenContractFileFails_KeepsTheRentalAndReportsTheError()
        {
            contracts.Fail = true;
            RentAccommodationViewModel viewModel = CreateViewModel();
            FillValidForm(viewModel);

            viewModel.Accept();

            Assert.Single(repository.SavedRentals);
            Assert.Contains("disk is full", Assert.Single(messages.Errors));
        }

        [Fact]
        public void TypingInClientSearch_ShowsMatchingClients()
        {
            repository.KnownClients.Add(new Landlord(1, "О.П.", "Сидоренко", "s@example.com", "+375", "MP4567890", "4010185D004PB4"));
            repository.KnownClients.Add(new Landlord(2, "Д.А.", "Ермолаев", "e@example.com", "+375", "MP5678901", "3120399E005PB5"));
            RentAccommodationViewModel viewModel = CreateViewModel();

            viewModel.ClientSearchText = "сидор";

            Assert.Equal("Сидоренко", Assert.Single(viewModel.Clients).Surname);
            Assert.True(viewModel.IsClientListOpen);
        }

        [Fact]
        public void SelectingClient_FillsTheFormWithoutStartingANewSearch()
        {
            var client = new Landlord(1, "О.П.", "Сидоренко", "s@example.com", "+375 29 000-00-01", "MP4567890", "4010185D004PB4");
            repository.KnownClients.Add(client);
            RentAccommodationViewModel viewModel = CreateViewModel();
            viewModel.ClientSearchText = "сидор";

            viewModel.SelectedClient = client;

            Assert.Equal("О.П.", viewModel.Initials);
            Assert.Equal("Сидоренко", viewModel.Surname);
            Assert.Equal("MP4567890", viewModel.PassportNumber);
            Assert.Equal("4010185D004PB4", viewModel.IdentificationNumber);
            Assert.False(viewModel.IsClientListOpen);
        }
    }
}
