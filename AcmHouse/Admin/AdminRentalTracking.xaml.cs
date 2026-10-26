using AcmHouse.Functions;
using AcmHouse.AccommodationOffers;
using AcmHouse.AccommodationInfo;
using AcmHouse.Enums;
using AcmHouse.OrdersInfo;
using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AcmHouse.UsersInfo;
using AcmHouse.Authorization;
using AcmHouse.Admin;
using AcmHouse.UserOrders;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;

namespace AcmHouse.Admin
{
    public partial class AdminRentalTracking : Window
    {
        private OrdersManager ordersManager;
        public UserOrderManager userOrdersManager;
        List<UserOrder> userOrders;
        public SQLiteManager manager;

        public AdminRentalTracking()
        {
            InitializeComponent();
            ordersManager = new OrdersManager();
            userOrdersManager = new UserOrderManager();
            userOrders = userOrdersManager.userOrdersList.GetAllUserOrders();
            ordersGrid.AutoGeneratingColumn += ordersGrid_AutoGeneratingColumn;
            ShowOrdersInDataGrid(ordersGrid, ordersManager);
            ShowUserOrders();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            ordersManager.GetAllOrders();
            userOrders.Clear();
            userOrders = userOrdersManager.userOrdersList.GetAllUserOrders();
            ShowOrdersInDataGrid(ordersGrid, ordersManager);
            ShowUserOrders();
        }

        public void ShowOrdersInDataGrid(DataGrid ordersGrid, OrdersManager ordersManager)
        {
            ordersManager.manager.OpenConnection();

            string selectQuery = "SELECT o.ID, a.Name AS [Предложение], u.Name || ' ' || u.Surname AS [Сотрудник], " +
                     "o.StartDate AS [Дата начала], o.EndDate AS [Дата окончания], o.TotalPrice AS [Итоговая стоимость] " +
                     "FROM [Order] o " +
                     "JOIN Users u ON o.UserID = u.ID " +
                     "JOIN Accommodations a ON o.AccommodationID = a.ID";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, ordersManager.manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    dataTable.Load(reader);

                    ordersGrid.ItemsSource = dataTable.DefaultView;
                }
            }

            ordersManager.manager.CloseConnection();
        }

        public void ShowUserOrders()
        {
            foreach (UserOrder userOrder in userOrders)
            {
                string profitQuery = "SELECT Profit FROM OrdersProfit WHERE OrderID = @OrderID";
                double profit = 0;

                using (SQLiteCommand profitCommand = new SQLiteCommand(profitQuery, userOrdersManager.manager.Connection))
                {
                    profitCommand.Parameters.AddWithValue("@OrderID", userOrder.OrderID);
                    userOrdersManager.manager.OpenConnection();
                    var profitResult = profitCommand.ExecuteScalar();
                    if (profitResult != null)
                    {
                        profit = Convert.ToDouble(profitResult);
                    }
                    userOrdersManager.manager.CloseConnection();
                }

                string userQuery = "SELECT Name, Surname FROM Users WHERE ID = @UserID";
                string userName = string.Empty;
                string userSurname = string.Empty;

                using (SQLiteCommand userCommand = new SQLiteCommand(userQuery, userOrdersManager.manager.Connection))
                {
                    userCommand.Parameters.AddWithValue("@UserID", userOrder.UserID);
                    userOrdersManager.manager.OpenConnection();
                    using (SQLiteDataReader userReader = userCommand.ExecuteReader())
                    {
                        if (userReader.Read())
                        {
                            userName = userReader["Name"].ToString();
                            userSurname = userReader["Surname"].ToString();
                        }
                    }
                    userOrdersManager.manager.CloseConnection();
                }

                userOrder.BookingText = $"Договор заключил {userSurname} {userName} с прибылью в {profit} руб.";
            }

            RentalsListBox.ItemsSource = null; 
            RentalsListBox.ItemsSource = userOrders; 
        }

        public static T FindDescendantByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            if (parent == null)
                return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T namedChild && (namedChild as FrameworkElement)?.Name == name)
                {
                    foundChild = namedChild;
                    break;
                }

                foundChild = FindDescendantByName<T>(child, name);
                if (foundChild != null)
                    break;
            }

            return foundChild;
        }

        private void RentalsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = RentalsListBox.SelectedItem as UserOrder;

            if (selectedItem != null)
            {
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string orderName = $"rental_order{selectedItem.ID}.docx";
                string filePath = System.IO.Path.Combine(projectDirectory, "Rentals", orderName);

                try
                {
                    string directoryPath = System.IO.Path.GetDirectoryName(filePath);
                    System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(directoryPath);

                    directoryInfo.Refresh();

                    Process.Start(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при открытии файла: " + ex.Message);
                }
            }
        }

        private void OrderDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ordersGrid.SelectedItem != null)
            {
                DataRowView rowView = ordersGrid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    Int64 ID = Convert.ToInt64(rowView["ID"]);
                    Int64 userID = Convert.ToInt64(rowView["Сотрудник"]);
                    Int64 accommodationID = Convert.ToInt64(rowView["Предложение"]);

                    string startDateText = rowView["Дата начала"].ToString();
                    string endDateText = rowView["Дата окончания"].ToString();

                    DateTime startDate;
                    DateTime endDate;

                    if (DateTime.TryParse(startDateText, out startDate) && DateTime.TryParse(endDateText, out endDate))
                    {
                        double totalPrice = Convert.ToDouble(rowView["Итоговая стоимость"]);

                        OrdersInfo.Order selectedOrder = new OrdersInfo.Order((int)userID, (int)accommodationID, startDate, endDate, totalPrice);

                        ordersManager.RemoveOrder(selectedOrder, (int)ID);

                        ShowOrdersInDataGrid(ordersGrid, ordersManager);
                    }
                    else
                    {
                        MessageBox.Show("Invalid date format.");
                    }
                }
            }
            else
            {
                MessageBox.Show("No order selected.");
            }
        }

        private void ordersGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ID")
            {
                e.Cancel = true;
            }
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

        private void EventsTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminEventsWindow window = new AdminEventsWindow();
            window.Show();
            this.Hide();
        }

        private void OffersTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow window = new AdminWindow();
            window.Show();
            this.Hide();
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ToggleMaximizeOnDoubleClick(e);
        }

        private void OrderClearTableButton_Click(object sender, RoutedEventArgs e)
        {
            ordersManager.ClearOrdersTable();
            ShowOrdersInDataGrid(ordersGrid, ordersManager);
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
