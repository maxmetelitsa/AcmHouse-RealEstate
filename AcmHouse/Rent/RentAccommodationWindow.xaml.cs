using System.Windows;
using System.Windows.Input;
using AcmHouse.AccommodationInfo;
using AcmHouse.AccommodationOffers;
using AcmHouse.Authorization;
using AcmHouse.Data;
using AcmHouse.Functions;
using AcmHouse.Profile;
using AcmHouse.Services;
using AcmHouse.Trade;
using AcmHouse.UsersInfo;
using AcmHouse.ViewModels;

namespace AcmHouse.Rent
{
    public partial class RentAccommodationWindow : Window
    {
        public RentAccommodationWindow(Accommodation accommodation, int rentalType)
        {
            InitializeComponent();

            AccommodationOffer offer = new AccommodationRepository().GetOffer(accommodation.ID);
            if (offer == null)
            {
                MessageBox.Show("Не удалось загрузить данные объекта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Loaded += (sender, e) => Close();
                return;
            }

            var viewModel = new RentAccommodationViewModel(
                offer,
                CurrentUserManager.CurrentUser,
                new RentalRepository(),
                new ContractService(),
                new MessageBoxService());

            viewModel.RentalCompleted += OnRentalCompleted;
            DataContext = viewModel;
        }

        private void OnRentalCompleted()
        {
            new ProfileWindow().Show();
            Hide();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ToggleMaximizeOnDoubleClick(e);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragWithLeftButton(e);
        }

        private void WindowHide_Click(object sender, RoutedEventArgs e)
        {
            this.Minimize();
        }

        private void WindowOpenFull_Click(object sender, RoutedEventArgs e)
        {
            // Maximizing is intentionally disabled for this window.
        }

        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.ExitApplication();
        }

        private void OffersButton_Click(object sender, RoutedEventArgs e)
        {
            OffersButton.ContextMenu.IsOpen = true;
        }
        private void ResidentialOffers_Click(object sender, RoutedEventArgs e)
        {
            AccommodationOffersWindow window = new AccommodationOffersWindow(0);
            window.Show();
            this.Hide();
        }
        private void CommercialOffers_Click(object sender, RoutedEventArgs e)
        {
            AccommodationOffersWindow window = new AccommodationOffersWindow(1);
            window.Show();
            this.Hide();
        }

        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            RentButton.ContextMenu.IsOpen = true;
        }
        private void Residential_Click(object sender, RoutedEventArgs e)
        {
            RentWindow window = new RentWindow(0);
            window.Show();
            this.Close();
        }
        private void Commercial_Click(object sender, RoutedEventArgs e)
        {
            RentWindow window = new RentWindow(1);
            window.Show();
            this.Close();
        }
        private void EventsButton_Click(object sender, RoutedEventArgs e)
        {
            EventsWindow window = new EventsWindow();
            window.Show();
            this.Close();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow window = new ProfileWindow();
            window.Show();
            this.Close();
        }
        private void TradeButton_Click(object sender, RoutedEventArgs e)
        {
            TradeButton.ContextMenu.IsOpen = true;
        }
        private void BuyResidentialProperty_Click(object sender, RoutedEventArgs e)
        {
            SaleOffersWindow window = new SaleOffersWindow(0);
            window.Show();
            this.Close();
        }
        private void BuyCommercialProperty_Click(object sender, RoutedEventArgs e)
        {
            SaleOffersWindow window = new SaleOffersWindow(1);
            window.Show();
            this.Close();
        }
        private void SellResidentialProperty_Click(object sender, RoutedEventArgs e)
        {
            PropertySellingWindow window = new PropertySellingWindow(0);
            window.Show();
            this.Close();
        }
        private void SellCommercialProperty_Click(object sender, RoutedEventArgs e)
        {
            PropertySellingWindow window = new PropertySellingWindow(1);
            window.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow window = new SignInWindow();
            window.Show();
            this.Hide();
        }
    }
}