using System;
using AcmHouse.Security;
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
using AcmHouse.Trade;
using System.IO;

namespace AcmHouse.Profile
{
    public partial class ProfileWindow : System.Windows.Window
    {
        public SQLiteManager manager;
        public OrdersManager ordersManager;
        public UserOrderManager userOrdersManager;
        public CurrentUser currentUserData = CurrentUserManager.CurrentUser;
        List<UserOrder> userOrders;

        public ProfileWindow()
        {
            manager = new SQLiteManager();
            ordersManager = new OrdersManager();
            userOrdersManager = new UserOrderManager();
            userOrders = userOrdersManager.userOrdersList.GetUserOrders(currentUserData.ID);
            InitializeComponent();
            if (currentUserData.Shift == false)
            {
                OpenShiftDetails();
            }
            else if(currentUserData.Shift == true)
            {
                CloseShiftDetails();
            }
            ShowInfo();
            ShowUserOrders();
            ShowUserProfit();
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ShowUserOrders();
        }

        private void ShowInfo()
        {
            SetReadAbleFields();

            manager.OpenConnection();

            string selectQuery = $"SELECT * FROM Users WHERE ID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", currentUserData.ID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nameFromDB = reader["Name"].ToString();
                        string surnameFromDB = reader["Surname"].ToString();
                        string emailFromDB = reader["Email"].ToString();
                        string passwordFromDB = reader["Password"].ToString();
                        string passportNumberFromDB = reader["PassportNumber"].ToString();
                        string identificationNumberFromDB = reader["IdentificationNumber"].ToString();

                        currentUserData.Name = nameFromDB;
                        currentUserData.Surname = surnameFromDB;
                        currentUserData.Email = emailFromDB;
                        currentUserData.Password = passwordFromDB;
                        currentUserData.PassportNumber = passportNumberFromDB;
                        currentUserData.IdentificationNumber = identificationNumberFromDB;

                        NameTextBox.Text = nameFromDB;
                        SurnameTextBox.Text = surnameFromDB;
                        EmailTextBox.Text = emailFromDB;
                        // Only the password hash is stored, so the fields stay empty
                        // until the user types a new password.
                        PasswordTextBox.Text = string.Empty;
                        PasswordRepeatTextBox.Text = string.Empty;

                    }
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

        /// <summary>Saves the profile. An empty password field keeps the current password.</summary>
        public void UpdateUserInfo(string name, string surname, string email, string password)
        {
            if (PasswordTextBox.Text != PasswordRepeatTextBox.Text)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            if (!ValidateUserFields(name, surname, email))
            {
                return;
            }

            CurrentUser currentUserData = CurrentUserManager.CurrentUser;
            string passwordHash = string.IsNullOrEmpty(password)
                ? currentUserData.Password
                : PasswordHasher.Hash(password);

            manager.OpenConnection();

            string updateQuery = "UPDATE Users SET Name = @Name, Surname = @Surname, Email = @Email, Password = @Password WHERE ID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(updateQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Surname", surname);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", passwordHash);
                command.Parameters.AddWithValue("@ID", currentUserData.ID);

                command.ExecuteNonQuery();
            }

            manager.CloseConnection();

            currentUserData.Name = name;
            currentUserData.Surname = surname;
            currentUserData.Email = email;
            currentUserData.Password = passwordHash;
        }

        private void SaveChangesProfileButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string surname = SurnameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;

            UpdateUserInfo(name, surname, email, password);
            ShowInfo();
        }

        private void ClearFields()
        {
            PasswordTextBox.Text = null;
            PasswordRepeatTextBox.Text = null;
        }
        public bool ValidateUserFields(string name, string surname, string email)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private void SetReadAbleFields()
        {
            NameTextBox.IsReadOnly = true;
            SurnameTextBox.IsReadOnly = true;
            EmailTextBox.IsReadOnly = true;
            PasswordTextBox.IsReadOnly = true;
            PasswordRepeatTextBox.IsReadOnly = true;
        }
        private void SetWriteAbleFields()
        {
            NameTextBox.IsReadOnly = true;
            SurnameTextBox.IsReadOnly = true;
            EmailTextBox.IsReadOnly = true;
            PasswordTextBox.IsReadOnly = false;
            PasswordRepeatTextBox.IsReadOnly = false;
        }
        public void OpenShiftDetails()
        {
            ShiftButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 139, 34));
            ShiftButton.Content = "Открыть смену";
        }
        public void CloseShiftDetails()
        {
            ShiftButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
            ShiftButton.Content = "Закрыть смену";
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void ClearFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }
        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            SetWriteAbleFields();
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
        private void OrderLookButton_Click(object sender, RoutedEventArgs e)
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
                    // Вывод отладочной информации
                    Console.WriteLine($"Попытка открыть файл: {filePath}");

                    if (File.Exists(filePath))
                    {
                        Process.Start(filePath);
                    }
                    else
                    {
                        Console.WriteLine($"Файл не найден по пути: {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при открытии файла: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Не выбран элемент для открытия документа.");
            }
        }


        public void ShowUserProfit()
        {
            double totalProfit = 0;
            manager.OpenConnection();

            string select = "SELECT * FROM OrdersProfit WHERE UserID = @UserID";

            using (SQLiteCommand selectCommand = new SQLiteCommand(select, manager.Connection))
            {
                selectCommand.Parameters.AddWithValue("@UserID", currentUserData.ID);

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
        public void OpenShift(DateTime checkTime)
        {
            try
            {
                manager.OpenConnection();

                string insertQuery = " INSERT INTO [OpenedShift] (UserID, Time) VALUES (@UserID, @Time);";

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
                {
                    command.Parameters.AddWithValue("@UserID", currentUserData.ID);
                    command.Parameters.AddWithValue("@Time", checkTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        manager.CloseConnection();
                    }
                    else
                    {
                        manager.CloseConnection();
                    }
                }

            }
            catch(Exception ex) { Console.WriteLine(ex.Message); }
        }
        public void CloseShift(DateTime checkTime)
        {
            try
            {
                manager.OpenConnection();

                string insertQuery = " INSERT INTO [ClosedShift] (UserID, Time) VALUES (@UserID, @Time);";

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
                {
                    command.Parameters.AddWithValue("@UserID", currentUserData.ID);
                    command.Parameters.AddWithValue("@Time", checkTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        manager.CloseConnection();
                    }
                    else
                    {
                        manager.CloseConnection();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
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
                closedShiftCommand.Parameters.AddWithValue("@UserID", currentUserData.ID);

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


        private void OpenShiftButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime checkTime;
            if (currentUserData.Shift == false && CheckAbilityToWork() == true)
            {
                checkTime = DateTime.Now;
                currentUserData.Shift = true;
                OpenShift(checkTime);
                CloseShiftDetails();
            }
            else if (currentUserData.Shift == true)
            {
                checkTime = DateTime.Now;
                currentUserData.Shift = false;
                CloseShift(checkTime);
                OpenShiftDetails();
            }
            else if (currentUserData.Shift == false && CheckAbilityToWork() == false)
            {
                DateTime lastClosedShift = getLastClosedShift();
                TimeSpan twelveHours = TimeSpan.FromHours(12);
                DateTime timeAfterTwelveHours = lastClosedShift.Add(twelveHours);

                TimeSpan timeDifference = timeAfterTwelveHours - DateTime.Now;
                MessageBox.Show($"До возможности открытия следующей смены осталось: {timeDifference.Hours}ч. {timeDifference.Minutes}мин.");

            }
            currentUserData.UpdateUserInfo(manager);
        }

    }
}