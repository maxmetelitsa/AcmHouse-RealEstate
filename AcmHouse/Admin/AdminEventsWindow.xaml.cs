using AcmHouse.Functions;
using AcmHouse.AccommodationOffers;
using AcmHouse.AccommodationInfo;
using AcmHouse.Enums;
using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data;
using AcmHouse.Rent;
using AcmHouse.Profile;
using System.Globalization;
using AcmHouse.Authorization;
using Colors = System.Windows.Media.Colors;
using AcmHouse.AccommodationExtraInfo;
using AcmHouse.Calendar;
using AcmHouse.UsersInfo;
using System.Collections.ObjectModel;

namespace AcmHouse.Admin
{
    public partial class AdminEventsWindow : Window
    {
        private CalendarViewModel _viewModel;
        private NotificationService _notificationService;
        private UsersInfo.Users users; 
        public SQLiteManager manager;

        public AdminEventsWindow()
        {
            InitializeComponent();

            manager = new SQLiteManager();
            users = new UsersInfo.Users();
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
                SELECT ce.ID, ce.Title, ce.Description, ce.StartTime, ce.EndTime, ce.Notify, u.Name, u.Surname, u.Email
                FROM CalendarEvents ce
                JOIN Users u ON ce.UserID = u.ID";

                using (SQLiteCommand command = new SQLiteCommand(query, manager.Connection))
                {
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
                            string name = reader["Name"].ToString();
                            string surname = reader["Surname"].ToString();
                            string email = reader["Email"].ToString();
                            string userInfo = $"Сотрудник: {surname} {name} - {email}";

                            CalendarEvent calendarEvent = new CalendarEvent(id, title, description, startTime, endTime, notify, userInfo);
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

        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            RentButton.ContextMenu.IsOpen = true;
        }
        private void Residential_Click(object sender, RoutedEventArgs e)
        {
            AdminRentWindow window = new AdminRentWindow(0);
            window.Show();
            this.Close();
        }
        private void Commercial_Click(object sender, RoutedEventArgs e)
        {
            AdminRentWindow window = new AdminRentWindow(1);
            window.Show();
            this.Close();
        }

        private void RentalTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminRentalTracking window = new AdminRentalTracking();
            window.Show();
            this.Hide();
        }

        private void OffersButton_Click(object sender, RoutedEventArgs e)
        {
            OffersButton.ContextMenu.IsOpen = true;
        }
        private void ResidentialOffers_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow window = new AdminWindow();
            window.Show();
            this.Hide();
        }
        private void CommercialOffers_Click(object sender, RoutedEventArgs e)
        {
            AdminCommercialWindow window = new AdminCommercialWindow();
            window.Show();
            this.Hide();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow window = new SignInWindow();
            window.Show();
            this.Hide();
        }
        private void EmployeesTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminEmployeesTracking window = new AdminEmployeesTracking();
            window.Show();
            this.Hide();
        }
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            AdminReportWindow window = new AdminReportWindow();
            window.Show();
            this.Hide();
        }

    }
}

