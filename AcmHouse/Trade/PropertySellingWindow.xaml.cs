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
using AcmHouse.LandlordInfo;
using AcmHouse.OrdersInfo;
using AcmHouse.UserOrders;
using AcmHouse.UsersInfo;
using AcmHouse.SellersInfo;
using AcmHouse.Contracts;
using AcmHouse.SaleAccommodationsInfo;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;

namespace AcmHouse.Trade
{
    public partial class PropertySellingWindow : Window
    {
        private AccommodationManager accommodationManager;
        private OrdersManager ordersManager;
        private AccommodationDetailsManager accommodationDetailsManager;
        private SaleAccommodationManager saleAccommodationManager;
        private UserOrderManager userOrdersManager;
        private SQLiteManager manager = new SQLiteManager();
        private Landlords landlords;
        private Sellers sellers;
        private static int ordersCounter = 0;
        public CurrentUser currentUserData = CurrentUserManager.CurrentUser;
        private int rentalTypeCheck;
        public ObservableCollection<Landlord> Clients { get; set; } = new ObservableCollection<Landlord>();


        public PropertySellingWindow(int rentalTypeCheck)
        {
            InitializeComponent();
            InitializeManagers();
            this.rentalTypeCheck = rentalTypeCheck;
            Rename();

            sellers.GetAllSellers(manager);
            landlords.GetAllUsers();

            SetTypeComboBoxItems();
            ClientComboBox.ItemsSource = Clients;
        }

        private void InitializeManagers()
        {
            accommodationManager = new AccommodationManager();
            accommodationDetailsManager = new AccommodationDetailsManager();
            saleAccommodationManager = new SaleAccommodationManager();
            ordersManager = new OrdersManager();
            userOrdersManager = new UserOrderManager();
            sellers = new Sellers();
            landlords = new Landlords();
        }

        private void SetTypeComboBoxItems()
        {
            List<AccommodationType> types;

            if (rentalTypeCheck == 0)
            {
                types = new List<AccommodationType> { AccommodationType.Апартаменты, AccommodationType.Дом, AccommodationType.Вилла, AccommodationType.Коттедж };
                TypeComboBox.ItemsSource = types;
                TypeComboBox.SelectedIndex = 0;
            }
            else if(rentalTypeCheck == 1)
            {
                types = new List<AccommodationType> { AccommodationType.Офис, AccommodationType.Склад };
                TypeComboBox.ItemsSource = types;
                TypeComboBox.SelectedIndex = 0;
            }
        }

        private void PutForSaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!currentUserData.Shift)
            {
                MessageBox.Show("Откройте рабочую смену в профиле, прежде чем начать совершение сделок.", "Уведомление о необходимости открытия смены");
                return;
            }

            if (ValidateFields())
            {
                ProcessSale();
            }
        }

        private void ProcessSale()
        {
            string offerName = NameTextBox.Text;
            AccommodationType selectedType;
            int rooms;
            double area;
            string adress = AdressTextBox.Text;
            string flatNumber = FlatNumberTextBox.Text;
            int imageID;
            double price;
            DateTime startDate = StartDatePicker.Value ?? DateTime.Now;
            DateTime endDate = EndDatePicker.Value ?? DateTime.Now;

            Seller seller = CreateSellerFromInput();
            SellContract sellContract = new SellContract(seller);

            GetAmountOfOrders();

            if (!string.IsNullOrWhiteSpace(offerName) && !string.IsNullOrWhiteSpace(adress) && !string.IsNullOrWhiteSpace(flatNumber)
                && Enum.TryParse(TypeComboBox.SelectedValue?.ToString(), out selectedType))
            {
                if (int.TryParse(RoomsTextBox.Text, out rooms) && double.TryParse(AreaTextBox.Text, out area) && int.TryParse(ImageIDTextBox.Text, out imageID) && double.TryParse(HourPriceTextBox.Text, out price))
                {
                    string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    string orderName = $"rental_order{++ordersCounter}.docx";
                    string companyContractsPath = System.IO.Path.Combine(projectDirectory, "Rentals", orderName);

                    string userOrderName = $"user_order{ordersCounter}.docx";
                    string personContractsPath = System.IO.Path.Combine(projectDirectory, "UserRentals", userOrderName);

                    sellers.AddSeller(manager, seller.Initials, seller.Surname, seller.Email, seller.Phone, seller.PassportNumber, seller.IdentificationNumber);
                    seller = sellers.GetLastSeller(manager);

                    int lastID = Functions.Functions.GetLastOfAllAccommodationsID();

                    SaleAccommodation saleAccommodation = new SaleAccommodation(++lastID, seller.ID, offerName, selectedType, adress, flatNumber, rooms, area, price, imageID, rentalTypeCheck);
                    saleAccommodationManager.AddAccommodation(saleAccommodation, seller);

                    sellContract.GenerateRealEstateContract(companyContractsPath, seller, currentUserData, saleAccommodation, ordersCounter, startDate, endDate);
                    sellContract.GenerateRealEstateContract(personContractsPath, seller, currentUserData, saleAccommodation, ordersCounter, startDate, endDate);

                    EnsureDirectoriesExist(companyContractsPath, personContractsPath);

                    ordersManager.AddOrder(currentUserData.ID, saleAccommodation.ID, startDate, endDate, price);
                    userOrdersManager.userOrdersList.AddUserOrder(ordersCounter, currentUserData.ID, ordersCounter);

                    int lastOrderID = ordersManager.GetLastOrderID();
                    ordersManager.AddExtraInfo(lastOrderID, DateTime.Now);

                    double profit = 0;
                    AddUserProfit(manager, currentUserData, ordersCounter, profit);

                    MessageBox.Show("Договор успешно заключен");
                    ProfileWindow window = new ProfileWindow();
                    window.Show();
                    this.Hide();
                }
            }
        }

        private Seller CreateSellerFromInput()
        {
            string initials = InitialsTextBox.Text;
            string surname = SurnameTextBox.Text;
            string email = EmailTextBox.Text;
            string phone = PhoneTextBox.Text;
            string passportNumber = PassportNumberTextBox.Text;
            string identificationNumber = IdentificationTextBox.Text;

            return new Seller(initials, surname, email, phone, passportNumber, identificationNumber);
        }

        private static void EnsureDirectoriesExist(params string[] paths)
        {
            foreach (var path in paths)
            {
                string directoryPath = System.IO.Path.GetDirectoryName(path);
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(directoryPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
            }
        }

        public static void AddUserProfit(SQLiteManager manager, CurrentUser currentUserData, int orderID, double profit)
        {
            try
            {
                manager.OpenConnection();

                string insertQuery = "INSERT INTO [OrdersProfit] (OrderID, UserID, Profit) VALUES (@OrderID, @UserID, @Profit);";

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
                {
                    command.Parameters.AddWithValue("@OrderID", orderID);
                    command.Parameters.AddWithValue("@UserID", currentUserData.ID);
                    command.Parameters.Add(new SQLiteParameter("@Profit", DbType.Double) { Value = profit });

                    command.ExecuteNonQuery();
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

        public void GetAmountOfOrders()
        {
            try
            {
                ordersManager.manager.OpenConnection();

                string selectQuery = "SELECT COUNT(*) FROM [Order]";
                int rowCount;

                using (SQLiteCommand command = new SQLiteCommand(selectQuery, ordersManager.manager.Connection))
                {
                    rowCount = Convert.ToInt32(command.ExecuteScalar());
                }

                ordersCounter = rowCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ordersManager.manager.CloseConnection();
            }
        }

        private bool ValidateFields()
        {
            string surname = SurnameTextBox.Text;
            string initials = InitialsTextBox.Text;
            string email = EmailTextBox.Text;
            string phone = PhoneTextBox.Text;
            string passportNumber = PassportNumberTextBox.Text;
            string identificationNumber = IdentificationTextBox.Text;
            string hourPriceText = HourPriceTextBox.Text;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            bool isValid = true;

            // Regular expressions for validation
            Regex nameRegex = new Regex("^[a-zA-Zа-яА-Я]+$");
            Regex phoneRegex = new Regex(@"^\+?[0-9\s\-]+$");
            Regex passportNumberRegex = new Regex(@"^[a-zA-Z0-9]+$");
            Regex identificationNumberRegex = new Regex(@"^[a-zA-Z0-9]+$");

            // Validate name
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                NameNotify.Foreground = new SolidColorBrush(Colors.Red);
                NameNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (!nameRegex.IsMatch(NameTextBox.Text))
            {
                NameNotify.Foreground = new SolidColorBrush(Colors.Red);
                NameNotify.Content = "Имя должно содержать только буквы";
                isValid = false;
            }
            else
            {
                NameNotify.Content = "";
            }

            // Validate type
            if (TypeComboBox.SelectedItem == null)
            {
                TypeNotify.Foreground = new SolidColorBrush(Colors.Red);
                TypeNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                TypeNotify.Content = "";
            }

            // Validate rooms
            if (string.IsNullOrWhiteSpace(RoomsTextBox.Text))
            {
                RoomsNotify.Foreground = new SolidColorBrush(Colors.Red);
                RoomsNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (!int.TryParse(RoomsTextBox.Text, out int rooms) || rooms <= 0)
            {
                RoomsNotify.Foreground = new SolidColorBrush(Colors.Red);
                RoomsNotify.Content = "Введите корректное положительное число";
                isValid = false;
            }
            else
            {
                RoomsNotify.Content = "";
            }

            // Validate area
            if (string.IsNullOrWhiteSpace(AreaTextBox.Text))
            {
                AreaNotify.Foreground = new SolidColorBrush(Colors.Red);
                AreaNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (!double.TryParse(AreaTextBox.Text, out double area) || area <= 0)
            {
                AreaNotify.Foreground = new SolidColorBrush(Colors.Red);
                AreaNotify.Content = "Введите корректное положительное число";
                isValid = false;
            }
            else
            {
                AreaNotify.Content = "";
            }

            // Validate address
            if (string.IsNullOrWhiteSpace(AdressTextBox.Text))
            {
                AddressNotify.Foreground = new SolidColorBrush(Colors.Red);
                AddressNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                AddressNotify.Content = "";
            }

            // Validate flat number
            if (string.IsNullOrWhiteSpace(FlatNumberTextBox.Text))
            {
                FlatNumberNotify.Foreground = new SolidColorBrush(Colors.Red);
                FlatNumberNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                FlatNumberNotify.Content = "";
            }

            // Validate image ID
            if (string.IsNullOrWhiteSpace(ImageIDTextBox.Text))
            {
                ImageNotify.Foreground = new SolidColorBrush(Colors.Red);
                ImageNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                ImageNotify.Content = "";
            }

            // Validate hour price
            if (string.IsNullOrWhiteSpace(HourPriceTextBox.Text))
            {
                HourPriceNotify.Foreground = new SolidColorBrush(Colors.Red);
                HourPriceNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                HourPriceNotify.Content = "";
            }

            // Validate surname
            if (string.IsNullOrWhiteSpace(surname))
            {
                SurnameNotify.Foreground = new SolidColorBrush(Colors.Red);
                SurnameNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (!nameRegex.IsMatch(surname))
            {
                SurnameNotify.Foreground = new SolidColorBrush(Colors.Red);
                SurnameNotify.Content = "Фамилия должна содержать только буквы";
                isValid = false;
            }
            else
            {
                SurnameNotify.Content = "";
            }

            // Validate initials
            if (string.IsNullOrWhiteSpace(initials))
            {
                NameNotify2.Foreground = new SolidColorBrush(Colors.Red);
                NameNotify2.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (!nameRegex.IsMatch(initials))
            {
                NameNotify2.Foreground = new SolidColorBrush(Colors.Red);
                NameNotify2.Content = "Инициалы должны содержать только буквы";
                isValid = false;
            }
            else
            {
                NameNotify2.Content = "";
            }

            // Validate email
            if (string.IsNullOrWhiteSpace(email))
            {
                EmailNotify.Foreground = new SolidColorBrush(Colors.Red);
                EmailNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (!IsValidEmail(email))
            {
                EmailNotify.Foreground = new SolidColorBrush(Colors.Red);
                EmailNotify.Content = "Введите корректный email";
                isValid = false;
            }
            else
            {
                EmailNotify.Content = "";
            }

            // Validate phone number
            if (string.IsNullOrWhiteSpace(phone))
            {
                PhoneNotify.Foreground = new SolidColorBrush(Colors.Red);
                PhoneNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (!phoneRegex.IsMatch(phone))
            {
                PhoneNotify.Foreground = new SolidColorBrush(Colors.Red);
                PhoneNotify.Content = "Введите корректный номер телефона";
                isValid = false;
            }
            else
            {
                PhoneNotify.Content = "";
            }

            // Validate passport number
            if (string.IsNullOrWhiteSpace(passportNumber))
            {
                PassportNumberNotify.Foreground = new SolidColorBrush(Colors.Red);
                PassportNumberNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (!passportNumberRegex.IsMatch(passportNumber))
            {
                PassportNumberNotify.Foreground = new SolidColorBrush(Colors.Red);
                PassportNumberNotify.Content = "Введите корректный номер паспорта";
                isValid = false;
            }
            else
            {
                PassportNumberNotify.Content = "";
            }

            if (string.IsNullOrWhiteSpace(identificationNumber))
            {
                IdentificationNotify.Foreground = new SolidColorBrush(Colors.Red);
                IdentificationNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (identificationNumber.Length < 14)
            {
                IdentificationNotify.Foreground = new SolidColorBrush(Colors.Red);
                IdentificationNotify.Content = "Укажите полный номер";
                isValid = false;
            }
            else if (!identificationNumberRegex.IsMatch(identificationNumber))
            {
                IdentificationNotify.Foreground = new SolidColorBrush(Colors.Red);
                IdentificationNotify.Content = "Введите корректный идентификационный номер";
                isValid = false;
            }
            else
            {
                IdentificationNotify.Content = "";
            }

            // Validate start date
            if (string.IsNullOrWhiteSpace(StartDatePicker.Text))
            {
                StartDateNotify.Foreground = new SolidColorBrush(Colors.Red);
                StartDateNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (!DateTime.TryParse(StartDatePicker.Text, out startDate) || startDate < DateTime.Now.Date)
            {
                StartDateNotify.Foreground = new SolidColorBrush(Colors.Red);
                StartDateNotify.Content = "Введите корректную дату начала аренды";
                isValid = false;
            }
            else
            {
                StartDateNotify.Content = "";
            }

            // Validate end date
            if (string.IsNullOrWhiteSpace(EndDatePicker.Text))
            {
                EndDateNotify.Foreground = new SolidColorBrush(Colors.Red);
                EndDateNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else if (!DateTime.TryParse(EndDatePicker.Text, out endDate))
            {
                EndDateNotify.Foreground = new SolidColorBrush(Colors.Red);
                EndDateNotify.Content = "Введите корректную дату окончания аренды";
                isValid = false;
            }
            else if (endDate <= startDate)
            {
                EndDateNotify.Foreground = new SolidColorBrush(Colors.Red);
                EndDateNotify.Content = "Дата окончания аренды должна быть позже даты начала";
                isValid = false;
            }
            else
            {
                EndDateNotify.Content = "";
            }

            return isValid;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public void Rename()
        {
            if(rentalTypeCheck == 1)
            {
                MainLabel.Content = "Продажа коммерческой недвижимости";
                RoomsLabel.Content = "Помещений";
                FlatNumberLabel.Content = "Номер офиса";
            }
        }

        private void ClearFields()
        {
            NameTextBox.Clear();
            TypeComboBox.SelectedItem = null;
            RoomsTextBox.Clear();
            AreaTextBox.Clear();
            AdressTextBox.Clear();
            FlatNumberTextBox.Clear();
            ImageIDTextBox.Clear();
            HourPriceTextBox.Clear();
        }

        private void ClearNotifies()
        {
            NameNotify.Content = "";
            TypeNotify.Content = "";
            RoomsNotify.Content = "";
            AreaNotify.Content = "";
            AddressNotify.Content = "";
            FlatNumberNotify.Content = "";
            ImageNotify.Content = "";
            HourPriceNotify.Content = "";
        }
        private void ClientComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.Template.FindName("PART_EditableTextBox", ClientComboBox) is System.Windows.Controls.TextBox editableTextBox)
            {
                editableTextBox.TextChanged += ClientComboBox_TextChanged;
            }
        }

        private void ClientComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ClientComboBox.Text;

            if (string.IsNullOrEmpty(searchText))
            {
                Clients.Clear();
                return;
            }

            Clients.Clear();
            var matchedClients = SearchLandlords(searchText);
            foreach (var client in matchedClients)
            {
                Clients.Add(client);
            }

            ClientComboBox.IsDropDownOpen = Clients.Any();
        }

        private ObservableCollection<Landlord> SearchLandlords(string searchText)
        {
            var matchedLandlords = new ObservableCollection<Landlord>();

            string query = "SELECT ID, Name, Surname, Email, Phone, PassportNumber, IdentificationNumber FROM Landlords WHERE Name LIKE @searchText OR Surname LIKE @searchText";
            using (var connection = new SQLiteConnection(landlords.manager.Connection))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchText", $"%{searchText}%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var landlord = new Landlord(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetString(3),
                                reader.GetString(4),
                                reader.GetString(5),
                                reader.GetString(6)
                            );
                            matchedLandlords.Add(landlord);
                        }

                    }
                }
            }

            return matchedLandlords;
        }

        private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientComboBox.SelectedItem is Landlord selectedClient)
            {
                InitialsTextBox.Text = selectedClient.Name;
                SurnameTextBox.Text = selectedClient.Surname;
                EmailTextBox.Text = selectedClient.Email;
                PhoneTextBox.Text = selectedClient.Phone;
                PassportNumberTextBox.Text = selectedClient.PassportNumber;
                IdentificationTextBox.Text = selectedClient.IdentificationNumber;
                Clients.Clear();
            }
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            bool? userClickedOK = openFileDialog.ShowDialog();

            if (userClickedOK == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(filePath);

                string imageNumber = ExtractNumberFromFileName(fileName);

                ImageIDTextBox.Text = imageNumber;
            }
        }

        private string ExtractNumberFromFileName(string fileName)
        {
            Match match = Regex.Match(fileName, @"\d+");
            return match.Value;
        }

        private void ACClearFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            ClearNotifies();
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rentalTypeCheck == 0)
            {
                List<string> types = new List<string> { "Апартаменты", "Дом", "Вилла", "Коттедж" };
                TypeComboBox.ItemsSource = types;
            }
            else if (rentalTypeCheck == 1)
            {
                List<string> types = new List<string> { "Офис", "Склад"};
                TypeComboBox.ItemsSource = types;
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
            var window = new AccommodationOffersWindow(0);
            window.Show();
            this.Hide();
        }

        private void CommercialOffers_Click(object sender, RoutedEventArgs e)
        {
            var window = new AccommodationOffersWindow(1);
            window.Show();
            this.Hide();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ProfileWindow();
            window.Show();
            this.Close();
        }

        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            RentButton.ContextMenu.IsOpen = true;
        }

        private void Residential_Click(object sender, RoutedEventArgs e)
        {
            var window = new RentWindow(0);
            window.Show();
            this.Close();
        }

        private void Commercial_Click(object sender, RoutedEventArgs e)
        {
            var window = new RentWindow(1);
            window.Show();
            this.Close();
        }

        private void EventsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new EventsWindow();
            window.Show();
            this.Close();
        }

        private void TradeButton_Click(object sender, RoutedEventArgs e)
        {
            TradeButton.ContextMenu.IsOpen = true;
        }

        private void BuyResidentialProperty_Click(object sender, RoutedEventArgs e)
        {
            var window = new SaleOffersWindow(0);
            window.Show();
            this.Close();
        }

        private void BuyCommercialProperty_Click(object sender, RoutedEventArgs e)
        {
            var window = new SaleOffersWindow(1);
            window.Show();
            this.Close();
        }

        private void SellResidentialProperty_Click(object sender, RoutedEventArgs e)
        {
            var window = new PropertySellingWindow(0);
            window.Show();
            this.Close();
        }

        private void SellCommercialProperty_Click(object sender, RoutedEventArgs e)
        {
            var window = new PropertySellingWindow(1);
            window.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var window = new SignInWindow();
            window.Show();
            this.Hide();
        }
    }
}

