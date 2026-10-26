using AcmHouse.Functions;
using System.Collections.Generic;
using System.Data.SQLite;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using AcmHouse.Authorization;
using AcmHouse.Report;
using AcmHouse.SQL_Manager;

namespace AcmHouse.Admin
{
    public partial class AdminReportWindow : Window
    {
        public SQLiteManager manager;

        public AdminReportWindow()
        {
            InitializeComponent();

            manager = new SQLiteManager();
            LoadReportData();
        }

        private void LoadReportData()
        {
            List<CompanyReport> reports = GetReportData(StartDatePicker.Value, EndDatePicker.Value);
            ReportDataGrid.ItemsSource = reports;
        }

        private List<CompanyReport> GetReportData(DateTime? startDate, DateTime? endDate)
        {
            List<CompanyReport> reports = new List<CompanyReport>();
            double companySumProfit = 0;

            manager.OpenConnection();

            string query = @"
            SELECT 
                od.Date AS Date,
                od.OrderID AS OrderID,
                o.TotalPrice AS TotalPrice,
                p.Profit AS EmployeeProfit,
                o.UserID AS UserID,
                u.Name || ' ' || u.Surname AS EmployeeName
            FROM OrderDetails od
            JOIN [Order] o ON od.OrderID = o.ID
            JOIN OrdersProfit p ON od.OrderID = p.OrderID
            JOIN Users u ON o.UserID = u.ID
            WHERE od.Date >= @startDate AND od.Date <= @endDate
            ORDER BY od.Date";

            using (SQLiteCommand command = new SQLiteCommand(query, manager.Connection))
            {
                command.Parameters.AddWithValue("@startDate", startDate?.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@endDate", endDate?.ToString("yyyy-MM-dd"));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        double employeeProfit = reader.GetDouble(reader.GetOrdinal("EmployeeProfit"));
                        double companyProfit = employeeProfit * 0.40;

                        companySumProfit += companyProfit;

                        reports.Add(new CompanyReport
                        {
                            Date = reader.GetString(reader.GetOrdinal("Date")),
                            OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                            TotalPrice = reader.GetDouble(reader.GetOrdinal("TotalPrice")),
                            EmployeeProfit = employeeProfit,
                            CompanyProfit = companyProfit,
                            UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                            EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName"))
                        });
                    }
                }
            }

            CompanyProfitTextBox.Text = companySumProfit.ToString();

            manager.CloseConnection();

            return reports;
        }

        private void StartDatePicker_SelectedDateChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LoadReportData();
        }

        private void EndDatePicker_SelectedDateChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LoadReportData();
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
