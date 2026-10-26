using AcmHouse.Enums;
using AcmHouse.Functions;
using System.Windows;
using System.Windows.Input;
using AcmHouse.Services;
using AcmHouse.ViewModels;
using AcmHouse.Admin;
using AcmHouse.AccommodationOffers;

namespace AcmHouse.Authorization
{
    public partial class SignInWindow : Window
    {
        private readonly SignInViewModel viewModel;

        public SignInWindow()
        {
            InitializeComponent();

            viewModel = new SignInViewModel(new AuthService());
            viewModel.SignedIn += OnSignedIn;
            DataContext = viewModel;
        }

        private void OnSignedIn(LoginResult result)
        {
            if (result == LoginResult.Administrator)
            {
                new AdminWindow().Show();
            }
            else
            {
                new AccommodationOffersWindow(0).Show();
            }

            Close();
        }

        // PasswordBox.Password cannot be bound (by design, for security), so it is passed on here.
        private void TextBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.Password = TextBoxPassword.Password;
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
            this.ToggleMaximize();
        }

        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.ExitApplication();
        }
    }
}