using AcmHouse.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AcmHouse.AccommodationOffers;
using AcmHouse.Rent;
using AcmHouse.Profile;
using AcmHouse.AccommodationInfo;
using AcmHouse.UsersInfo;
using System.Data.SQLite;
using AcmHouse.Enums;
using AcmHouse.OrdersInfo;
using AcmHouse.CompanyInfo;
using AcmHouse.Authorization;
using AcmHouse.UserOrders;
using System.Globalization;
using AcmHouse.AccommodationExtraInfo;
using AcmHouse.LandlordInfo;
using AcmHouse.SQL_Manager;
using System.Data;
using System.Net;
using System.Net.Mail;
using AcmHouse.Trade;
using System.IO;

namespace AcmHouse.AccommodationOffers
{
    public partial class AccommodationInfoWindow : Window
    {
        private AccommodationDetailsManager accommodationDetailsManager;
        Accommodation accommodation;
        SQLiteManager manager = new SQLiteManager();
        CurrentUser currentUserData = CurrentUserManager.CurrentUser;

        public Accommodation Accommodation
        {
            get { return accommodation; }
            set { accommodation = value; }
        }
        public AccommodationInfoWindow(Accommodation accommodation)
        {
            InitializeComponent();
            Accommodation = accommodation;
            accommodationDetailsManager = new AccommodationDetailsManager();
            ShowAccommodation(accommodationInfo);
            ShowRentalSchedule(accommodation);
        }
      
        public void ShowAccommodation(WrapPanel accommodationObject)
        {
            accommodationObject = FindName("accommodationInfo") as WrapPanel;
            AccommodationDetails accommodationDetailsOriginal = null;

            foreach (AccommodationDetails accommodationDetails in accommodationDetailsManager.accommodationDetailsList)
            {
                if (accommodationDetails.AccommodationID == accommodation.ID)
                {
                    accommodationDetailsOriginal = accommodationDetails;
                    break;
                }
            }

            Button accommodationButton = new Button();
            accommodationButton.Tag = Accommodation;
            accommodationButton.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            accommodationButton.BorderThickness = new Thickness(2);
            accommodationButton.Height = 571;
            accommodationButton.Width = 563;
            accommodationButton.Background = new SolidColorBrush(Color.FromRgb(222, 223, 224));

            StackPanel accommodationPanel = new StackPanel();
            accommodationPanel.Orientation = Orientation.Horizontal;
            accommodationPanel.Height = double.NaN;
            accommodationPanel.Width = 563;

            TextBlock accommodationInfo = new TextBlock();
            accommodationInfo.Inlines.Add(new Run("Название: ") { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new Run(Accommodation.Name) { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Тип: ") { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new Run(accommodation.AccommodationType.ToString()) { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Количество комнат: ") { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new Run(Accommodation.AmountOfRooms.ToString()) { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Площадь: ") { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new Run(Accommodation.Area.ToString()) { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new Run(" м") { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new Run("2") { BaselineAlignment = BaselineAlignment.Superscript, FontSize = 10, Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Адрес: ") { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new Run(accommodationDetailsOriginal.Address.ToString()) { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Стоимость за сутки: ") { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new Run(Accommodation.DayPrice.ToString()) { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new Run(" руб.") { Foreground = Brushes.Black });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Foreground = Brushes.White;
            accommodationInfo.FontSize = 20;
            accommodationInfo.Margin = new Thickness(-450, 350, 0, 0);
            accommodationInfo.HorizontalAlignment = HorizontalAlignment.Left;
            accommodationButton.Background = new SolidColorBrush(Color.FromRgb(222, 223, 224));

            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string imageName = $"pic{Accommodation.ImageID}.jpg";
            string imagePath = System.IO.Path.Combine(projectDirectory, "Images", imageName);

            Image image = new Image();
            image.Height = 355;
            image.Width = 450;
            image.Margin = new Thickness(50,0,0,180);
            image.VerticalAlignment = VerticalAlignment.Top;
            image.HorizontalAlignment = HorizontalAlignment.Center;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.EndInit();

            image.Source = bitmap;

            accommodationPanel.Children.Add(image);

            accommodationPanel.Children.Add(accommodationInfo);

            accommodationButton.Content = accommodationPanel;

            accommodationObject.Children.Add(accommodationButton);
        }

        public void ShowRentalSchedule(Accommodation accommodation)
        {
            manager.OpenConnection();

            string selectQuery = "SELECT StartDate AS [Начало аренды], EndDate AS [Окончание аренды] FROM [Order] WHERE AccommodationID = @AccommodationID";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@AccommodationID", Accommodation.ID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    rentalSchedule.ItemsSource = dataTable.DefaultView;
                }
            }

            manager.CloseConnection();
        }

        private void SendObjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientEmailLabel.Visibility == Visibility.Collapsed)
            {
                ClientEmailLabel.Visibility = Visibility.Visible;
                ClientEmailTextBox.Visibility = Visibility.Visible;
                SendEmailButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                SendObjectButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ClientEmailLabel.Visibility = Visibility.Collapsed;
                ClientEmailTextBox.Visibility = Visibility.Collapsed;
                SendEmailButton.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Collapsed;
                SendObjectButton.Visibility = Visibility.Visible;
            }
        }
        private void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {
            string to = ClientEmailTextBox.Text;
            string subject = Accommodation.Name;
            string body = $"<p>Вы получили новое предложение от агенства недвижимости <b>&quot;ПроКвартал&quot;</b>!<br><br>" +
                          $"Сведения о полученном предложении:</p>" +
                          $"<p>Тип жилья: {Accommodation.AccommodationType}<br>" +
                          $"Количество комнат: {Accommodation.AmountOfRooms}<br>" +
                          $"Площадь: {Accommodation.Area}м<sup>2</sup><br>" +
                          $"Стоимость за сутки: {Accommodation.DayPrice}руб.<br><br>" +
                          $"<p>Если у вас возникли вопросы, обращайтесь на электронный адрес риелтора: " +
                          $"<a href='mailto:{currentUserData.Email}'>{currentUserData.Email}</a></p>";
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string imagePath = System.IO.Path.Combine(projectDirectory, "Images", $"pic{Accommodation.ImageID}.jpg");

            try
            {
                SendEmail(to, subject, body, imagePath);
                MessageBox.Show("Объект был успешно выслан на email клиента!", "Сообщение об успешной отпраке email", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"При отправке объекта возникла ошибка: {ex.Message}", "Сообщение о неудачной отпраке email", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ClientEmailTextBox.Clear();
            ClientEmailLabel.Visibility = Visibility.Collapsed;
            ClientEmailTextBox.Visibility = Visibility.Collapsed;
            SendEmailButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            SendObjectButton.Visibility = Visibility.Visible;
        }

        private void SendEmail(string to, string subject, string body, string imagePath)
        {
            // SMTP credentials are read from environment variables so they never end up in the repository.
            string username = Environment.GetEnvironmentVariable("ACMHOUSE_SMTP_USER");
            string password = Environment.GetEnvironmentVariable("ACMHOUSE_SMTP_PASSWORD");
            string smtpServer = Environment.GetEnvironmentVariable("ACMHOUSE_SMTP_HOST") ?? "smtp.mail.ru";
            int smtpPort = 587;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException(
                    "Не настроена почта: задайте переменные окружения ACMHOUSE_SMTP_USER и ACMHOUSE_SMTP_PASSWORD.");
            }

            string from = username;

            var mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(to);
            mail.Subject = subject;

            var htmlView = AlternateView.CreateAlternateViewFromString(body + "<br><img src=cid:imageId>", null, "text/html");

            if (!string.IsNullOrEmpty(imagePath))
            {
                LinkedResource inline = new LinkedResource(imagePath)
                {
                    ContentId = "imageId"
                };
                htmlView.LinkedResources.Add(inline);
            }

            mail.AlternateViews.Add(htmlView);

            using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(username, password);
                smtpClient.EnableSsl = true; 
                smtpClient.Send(mail);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClientEmailTextBox.Clear();
            ClientEmailLabel.Visibility = Visibility.Collapsed;
            ClientEmailTextBox.Visibility = Visibility.Collapsed;
            SendEmailButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            SendObjectButton.Visibility = Visibility.Visible;
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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow window = new SignInWindow();
            window.Show();
            this.Hide();
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
    }
}