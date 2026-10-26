using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcmHouse.Data;
using AcmHouse.Domain;
using AcmHouse.LandlordInfo;
using AcmHouse.Mvvm;
using AcmHouse.Services;
using AcmHouse.UsersInfo;

namespace AcmHouse.ViewModels
{
    /// <summary>Rental form: client data, rental period, price calculation and signing the contract.</summary>
    public class RentAccommodationViewModel : ViewModelBase
    {
        private readonly CurrentUser employee;
        private readonly IRentalRepository repository;
        private readonly IContractService contractService;
        private readonly IMessageService messages;
        private readonly Func<DateTime> now;

        private string surname = string.Empty;
        private string initials = string.Empty;
        private string email = string.Empty;
        private string phone = string.Empty;
        private string passportNumber = string.Empty;
        private string identificationNumber = string.Empty;
        private DateTime? startDate;
        private DateTime? endDate;
        private RentalQuote quote;

        private string clientSearchText = string.Empty;
        private Landlord selectedClient;
        private bool isClientListOpen;
        private bool isFillingFromClient;

        private Dictionary<string, string> errors = new Dictionary<string, string>();

        public RentAccommodationViewModel(
            AccommodationOffer offer,
            CurrentUser employee,
            IRentalRepository repository,
            IContractService contractService,
            IMessageService messages,
            Func<DateTime> now = null)
        {
            Card = new OfferCardViewModel(offer);
            this.employee = employee ?? throw new ArgumentNullException(nameof(employee));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.contractService = contractService ?? throw new ArgumentNullException(nameof(contractService));
            this.messages = messages ?? throw new ArgumentNullException(nameof(messages));
            this.now = now ?? (() => DateTime.Now);

            AcceptCommand = new RelayCommand(Accept);
        }

        /// <summary>Raised after the contract has been signed; the view navigates to the profile.</summary>
        public event Action RentalCompleted;

        public OfferCardViewModel Card { get; }

        public RelayCommand AcceptCommand { get; }

        public ObservableCollection<Landlord> Clients { get; } = new ObservableCollection<Landlord>();

        // ---- Client fields ----

        public string Surname { get => surname; set => SetProperty(ref surname, value); }

        public string Initials { get => initials; set => SetProperty(ref initials, value); }

        public string Email { get => email; set => SetProperty(ref email, value); }

        public string Phone { get => phone; set => SetProperty(ref phone, value); }

        public string PassportNumber { get => passportNumber; set => SetProperty(ref passportNumber, value); }

        public string IdentificationNumber { get => identificationNumber; set => SetProperty(ref identificationNumber, value); }

        // ---- Rental period and price ----

        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                if (SetProperty(ref startDate, value))
                {
                    RecalculatePrice();
                }
            }
        }

        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                if (SetProperty(ref endDate, value))
                {
                    RecalculatePrice();
                }
            }
        }

        public string DaysText => quote != null && quote.IsValid ? quote.Days.ToString() : string.Empty;

        public string TotalText => quote != null && quote.IsValid ? quote.Total.ToString("0.##") : string.Empty;

        // ---- Validation messages shown under the fields ----

        public string SurnameError => GetError(ClientValidator.Surname);
        public string InitialsError => GetError(ClientValidator.Initials);
        public string EmailError => GetError(ClientValidator.Email);
        public string PhoneError => GetError(ClientValidator.Phone);
        public string PassportNumberError => GetError(ClientValidator.PassportNumber);
        public string IdentificationNumberError => GetError(ClientValidator.IdentificationNumber);
        public string StartDateError => GetError(ClientValidator.StartDate);

        /// <summary>End date error, or why the selected period cannot be rented.</summary>
        public string EndDateError
        {
            get
            {
                string error = GetError(ClientValidator.EndDate);
                if (error.Length == 0 && quote != null && !quote.IsValid)
                {
                    return quote.Error;
                }

                return error;
            }
        }

        // ---- Client search (editable combo box) ----

        public string ClientSearchText
        {
            get => clientSearchText;
            set
            {
                // The combo box writes the selected client's surname back into the text;
                // that must not start a new search.
                bool isSelectedClientText = selectedClient != null && value == selectedClient.Surname;

                if (SetProperty(ref clientSearchText, value) && !isFillingFromClient && !isSelectedClientText)
                {
                    SearchClients(value);
                }
            }
        }

        public bool IsClientListOpen
        {
            get => isClientListOpen;
            set => SetProperty(ref isClientListOpen, value);
        }

        public Landlord SelectedClient
        {
            get => selectedClient;
            set
            {
                if (SetProperty(ref selectedClient, value) && value != null)
                {
                    FillFromClient(value);
                }
            }
        }

        public void Accept()
        {
            if (!employee.Shift)
            {
                messages.ShowInfo("Откройте рабочую смену в профиле, прежде чем начать совершение сделок.",
                    "Смена не открыта");
                return;
            }

            RentalForm form = CreateForm();
            errors = ClientValidator.Validate(form, now());
            RaiseErrorsChanged();
            if (errors.Count > 0)
            {
                return;
            }

            RentalQuote rentalQuote = RentalCalculator.Calculate(form.StartDate.Value, form.EndDate.Value, Card.DayPrice);
            if (!rentalQuote.IsValid)
            {
                messages.ShowError(rentalQuote.Error, "Невозможно совершить аренду");
                return;
            }

            if (!repository.IsAvailable(Card.Accommodation.ID, form.StartDate.Value, form.EndDate.Value))
            {
                messages.ShowError("Это предложение уже арендовано на выбранные даты.", "Невозможно совершить аренду");
                return;
            }

            int orderId = repository.SaveRental(new RentalRecord
            {
                EmployeeId = employee.ID,
                AccommodationId = Card.Accommodation.ID,
                StartDate = form.StartDate.Value,
                EndDate = form.EndDate.Value,
                TotalPrice = rentalQuote.Total,
                EmployeeProfit = RentalCalculator.CalculateAgentProfit(rentalQuote.Total),
                SignedOn = now(),
                Client = new Landlord(form.Initials, form.Surname, form.Email, form.Phone, form.PassportNumber, form.IdentificationNumber)
            });

            try
            {
                contractService.CreateRentalContracts(orderId, Card.Accommodation, Card.Offer.Details, employee, form, rentalQuote);
            }
            catch (Exception ex)
            {
                messages.ShowError($"Договор №{orderId} сохранён, но файл договора создать не удалось: {ex.Message}", "Ошибка");
                RentalCompleted?.Invoke();
                return;
            }

            messages.ShowInfo($"Договор №{orderId} успешно заключен", "Аренда");
            RentalCompleted?.Invoke();
        }

        private RentalForm CreateForm()
        {
            return new RentalForm
            {
                Surname = Surname?.Trim(),
                Initials = Initials?.Trim(),
                Email = Email?.Trim(),
                Phone = Phone?.Trim(),
                PassportNumber = PassportNumber?.Trim(),
                IdentificationNumber = IdentificationNumber?.Trim(),
                StartDate = StartDate,
                EndDate = EndDate
            };
        }

        private void RecalculatePrice()
        {
            quote = StartDate.HasValue && EndDate.HasValue
                ? RentalCalculator.Calculate(StartDate.Value, EndDate.Value, Card.DayPrice)
                : null;

            OnPropertyChanged(nameof(DaysText));
            OnPropertyChanged(nameof(TotalText));
            OnPropertyChanged(nameof(EndDateError));
        }

        private void SearchClients(string text)
        {
            Clients.Clear();

            if (string.IsNullOrWhiteSpace(text))
            {
                IsClientListOpen = false;
                return;
            }

            foreach (Landlord client in repository.SearchClients(text.Trim()))
            {
                Clients.Add(client);
            }

            IsClientListOpen = Clients.Count > 0;
        }

        private void FillFromClient(Landlord client)
        {
            isFillingFromClient = true;
            try
            {
                Initials = client.Name;
                Surname = client.Surname;
                Email = client.Email;
                Phone = client.Phone;
                PassportNumber = client.PassportNumber;
                IdentificationNumber = client.IdentificationNumber;
                ClientSearchText = client.Surname;
                IsClientListOpen = false;
            }
            finally
            {
                isFillingFromClient = false;
            }
        }

        private string GetError(string field)
        {
            return errors.TryGetValue(field, out string error) ? error : string.Empty;
        }

        private void RaiseErrorsChanged()
        {
            OnPropertyChanged(nameof(SurnameError));
            OnPropertyChanged(nameof(InitialsError));
            OnPropertyChanged(nameof(EmailError));
            OnPropertyChanged(nameof(PhoneError));
            OnPropertyChanged(nameof(PassportNumberError));
            OnPropertyChanged(nameof(IdentificationNumberError));
            OnPropertyChanged(nameof(StartDateError));
            OnPropertyChanged(nameof(EndDateError));
        }
    }
}
