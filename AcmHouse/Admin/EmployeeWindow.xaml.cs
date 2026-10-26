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
using AcmHouse.UsersInfo;
using AcmHouse.Enums;
using System.Data.SQLite;
using AcmHouse.SQL_Manager;
using AcmHouse.Authorization;
using AcmHouse.AccommodationInfo;
using AcmHouse.OrdersInfo;
using AcmHouse.UserOrders;
using System.Diagnostics;
using AcmHouse.Functions;
using System.IO;

namespace AcmHouse.Admin
{
    /// <summary>
    /// Логика взаимодействия для TestMainWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : System.Windows.Window
    {
        public SQLiteManager manager;
        public OrdersManager ordersManager;
        public UserOrderManager userOrdersManager;
        int ID;
        List<UserOrder> userOrders;

        public EmployeeWindow(int id)
        {
            manager = new SQLiteManager();
            ordersManager = new OrdersManager();
            userOrdersManager = new UserOrderManager();
            ID = id;
            userOrders = userOrdersManager.userOrdersList.GetUserOrders(ID);
            InitializeComponent();
            ShowInfo();
            ShowOpenedShiftsInDataGrid(openedShiftsGrid);
            ShowClosedShiftsInDataGrid(closedShiftsGrid);
            ShowUserProfit();
        }
        private void ShowInfo()
        {

            manager.OpenConnection();

            string selectQuery = $"SELECT * FROM Users WHERE ID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", ID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nameFromDB = reader["Name"].ToString();
                        string surnameFromDB = reader["Surname"].ToString();
                        string emailFromDB = reader["Email"].ToString();
                        string passportNumberFromDB = reader["PassportNumber"].ToString();
                        string identificationNumberFromDB = reader["IdentificationNumber"].ToString();

                        NameTextBox.Text = nameFromDB;
                        SurnameTextBox.Text = surnameFromDB;
                        EmailTextBox.Text = emailFromDB;
                        PassportNumberTextBox.Text = passportNumberFromDB;
                        IdentificationNumberTextBox.Text = identificationNumberFromDB;

                    }
                }
            }

            ShowUserOrders();
            manager.CloseConnection();
        }

        public void ShowOpenedShiftsInDataGrid(DataGrid openedShiftsGrid)
        {
            manager.OpenConnection();

            string selectQuery = $"SELECT Time AS [Дата открытия смены] FROM [OpenedShift] WHERE UserID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", ID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    dataTable.Load(reader);

                    openedShiftsGrid.ItemsSource = dataTable.DefaultView;
                }
            }

            manager.CloseConnection();
        }
        public void ShowClosedShiftsInDataGrid(DataGrid closedShiftsGrid)
        {
            manager.OpenConnection();

            string selectQuery = $"SELECT Time AS [Дата закрытия смены] FROM [ClosedShift] WHERE UserID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", ID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    dataTable.Load(reader);

                    closedShiftsGrid.ItemsSource = dataTable.DefaultView;
                }
            }

            manager.CloseConnection();
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

                string userQuery = "SELECT TotalPrice FROM [Order] WHERE ID = @OrderID";
                double totalPrice = 0;

                using (SQLiteCommand totalPriceCommand = new SQLiteCommand(userQuery, userOrdersManager.manager.Connection))
                {
                    totalPriceCommand.Parameters.AddWithValue("@OrderID", userOrder.OrderID);
                    userOrdersManager.manager.OpenConnection();
                    var totalPriceResult = totalPriceCommand.ExecuteScalar();
                    if (totalPriceResult != null)
                    {
                        totalPrice = Convert.ToDouble(totalPriceResult);
                    }
                    userOrdersManager.manager.CloseConnection();
                }

                userOrder.BookingText = $"Договор был заключен на сумму {totalPrice} руб. с прибылью в {profit} руб.";
            }

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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
            this.Hide();
        }

        private void RentalsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = RentalsListBox.SelectedItem as UserOrder;

            if (selectedItem != null)
            {
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string orderName = $"user_order{selectedItem.ID}.docx";
                string filePath = System.IO.Path.Combine(projectDirectory, "UserRentals", orderName);

                try
                {
                    // Получить информацию о каталоге
                    string directoryPath = System.IO.Path.GetDirectoryName(filePath);
                    System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(directoryPath);

                    // Обновить содержимое каталога
                    directoryInfo.Refresh();

                    // Открыть документ
                    Process.Start(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при открытии файла: " + ex.Message);
                }
            }
        }
        public void ShowUserProfit()
        {
            double totalProfit = 0;
            manager.OpenConnection();

            string select = "SELECT * FROM OrdersProfit WHERE UserID = @UserID";

            using (SQLiteCommand selectCommand = new SQLiteCommand(select, manager.Connection))
            {
                selectCommand.Parameters.AddWithValue("@UserID", ID);

                using (SQLiteDataReader selectReader = selectCommand.ExecuteReader())
                {
                    while (selectReader.Read())
                    {
                        double profit = Convert.ToDouble(selectReader["Profit"]);
                        totalProfit += profit;
                    }
                }
            }

            manager.CloseConnection();

            DeductionsTextBox.Text = totalProfit.ToString() + " руб.";
        }
        public bool CheckAbilityToWork()
        {
            DateTime lastClosedShift = getLastClosedShift();

            if (lastClosedShift == DateTime.MinValue)
            {
                return true;
            }
            else
            {
                TimeSpan timeDifference = DateTime.Now - lastClosedShift;

                if (timeDifference.TotalHours >= 12)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public DateTime getLastClosedShift()
        {
            DateTime lastClosedShift = DateTime.MinValue;

            manager.OpenConnection();

            string selectClosedShiftQuery = "SELECT * FROM ClosedShift WHERE UserID = @UserID ORDER BY Time DESC LIMIT 1";

            using (SQLiteCommand closedShiftCommand = new SQLiteCommand(selectClosedShiftQuery, manager.Connection))
            {
                closedShiftCommand.Parameters.AddWithValue("@UserID", ID);

                using (SQLiteDataReader closedShiftReader = closedShiftCommand.ExecuteReader())
                {
                    if (closedShiftReader.Read())
                    {
                        lastClosedShift = Convert.ToDateTime(closedShiftReader["Time"]);
                    }
                }
            }

            manager.CloseConnection();

            return lastClosedShift;
        }

    }
}