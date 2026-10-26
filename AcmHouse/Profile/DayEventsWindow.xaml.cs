using AcmHouse.Functions;
using AcmHouse.AccommodationOffers;
using AcmHouse.Authorization;
using AcmHouse.Calendar;
using AcmHouse.Rent;
using AcmHouse.UsersInfo;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
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
using AcmHouse.SQL_Manager;
using System.Windows.Threading;
using System.Windows.Forms;
using AcmHouse.Trade;

namespace AcmHouse.Profile
{
    public partial class DayEventsWindow : Window
    {
        private CalendarViewModel _viewModel;
        private NotificationService _notificationService;

        public CurrentUser currentUserData = CurrentUserManager.CurrentUser;
        public SQLiteManager manager;

        public DayEventsWindow()
        {
            InitializeComponent();

            manager = new SQLiteManager();
            _viewModel = new CalendarViewModel();
            DataContext = _viewModel;
            _notificationService = new NotificationService(_viewModel);
            LoadEventsFromDB();
        }
        private void LoadEventsFromDB()
        {
            try
            {
                manager.OpenConnection();

                string query = @"
            SELECT * 
            FROM [CalendarEvents] 
            WHERE UserID = @UserID 
            AND DATE(StartTime) = DATE('now')
        ";

                using (SQLiteCommand command = new SQLiteCommand(query, manager.Connection))
                {
                    command.Parameters.AddWithValue("@UserID", currentUserData.ID);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["ID"].ToString());
                            string title = reader["Title"].ToString();
                            string description = reader["Description"].ToString();
                            DateTime startTime = DateTime.Parse(reader["StartTime"].ToString());
                            DateTime endTime = DateTime.Parse(reader["EndTime"].ToString());
                            int notify = int.Parse(reader["Notify"].ToString());

                            CalendarEvent calendarEvent = new CalendarEvent(id, title, description, startTime, endTime, notify);
                            _viewModel.AddEvent(calendarEvent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                manager.CloseConnection();
            }
        }

        private void CheckNotifications(object sender, EventArgs e)
        {
            foreach (var calendarEvent in _viewModel.Events)
            {
                if (calendarEvent.Notify == 1 && calendarEvent.StartTime > DateTime.Now && calendarEvent.StartTime <= DateTime.Now.AddHours(1))
                {
                    _notificationService.ShowNotification($"Предстоящее событие: {calendarEvent.Title} в {calendarEvent.StartTime}");

                    var timer = new System.Timers.Timer();
                    timer.Interval = TimeSpan.FromMinutes(15).TotalMilliseconds;
                    timer.Elapsed += (s, ev) =>
                    {
                        if (calendarEvent.StartTime <= DateTime.Now)
                        {
                            _notificationService.ShowNotification($"Предстоящее событие: {calendarEvent.Title} в {calendarEvent.StartTime}");
                        }
                        else
                        {
                            timer.Stop();
                            timer.Dispose();
                        }
                    };
                    timer.Start();
                }
            }
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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow window = new SignInWindow();
            window.Show();
            this.Hide();
        }
    }
}
