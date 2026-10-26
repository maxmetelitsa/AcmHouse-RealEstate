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
using System.Collections.ObjectModel;
using AcmHouse.Trade;
using AcmHouse.Contracts;
using AcmHouse.SaleAccommodationsInfo;
using AcmHouse.SellersInfo;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using System.IO;

namespace AcmHouse.Trade
{
    public partial class PropertyBuyingWindow : Window
    {
        private OrdersManager ordersManager;
        private UserOrderManager userOrdersManager;
        private SaleAccommodationManager saleAccommodationManager;
        private int rentalTypeCheck = int.MinValue;
        SaleAccommodation accommodation;
        CurrentUser currentUserData = CurrentUserManager.CurrentUser;
        static int ordersCounter = 0;
        SQLiteManager manager = new SQLiteManager();
        Landlords landlords;
        public ObservableCollection<Landlord> Clients { get; set; } = new ObservableCollection<Landlord>();

        public SaleAccommodation Accommodation
        {
            get { return accommodation; }
            set { accommodation = value; }
        }
        public PropertyBuyingWindow(SaleAccommodation accommodation, int rentalTypeCheck)
        {
            InitializeComponent();
            this.rentalTypeCheck = rentalTypeCheck;

            ClientComboBox.ItemsSource = Clients;
            Accommodation = accommodation;
            ordersManager = new OrdersManager();
            userOrdersManager = new UserOrderManager();
            saleAccommodationManager = new SaleAccommodationManager();
            landlords = new Landlords();
            ShowAccommodation(rentalAccommodation);
        }
        public void ShowAccommodation(WrapPanel accommodationObject)
        {
            accommodationObject = FindName("rentalAccommodation") as WrapPanel;

            Button accommodationButton = CreateAccommodationButton();
            StackPanel accommodationPanel = CreateAccommodationPanel();
            TextBlock accommodationInfo = CreateAccommodationInfo();
            Image accommodationImage = CreateAccommodationImage();

            accommodationPanel.Children.Add(accommodationImage);
            accommodationPanel.Children.Add(accommodationInfo);
            accommodationButton.Content = accommodationPanel;

            accommodationObject.Children.Add(accommodationButton);
        }

        private Button CreateAccommodationButton()
        {
            Button accommodationButton = new Button();
            accommodationButton.Tag = Accommodation;
            accommodationButton.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            accommodationButton.BorderThickness = new System.Windows.Thickness(2);
            accommodationButton.Height = 355;
            accommodationButton.Width = 512;
            accommodationButton.Background = new SolidColorBrush(Color.FromRgb(86, 86, 86));
            return accommodationButton;
        }

        private StackPanel CreateAccommodationPanel()
        {
            StackPanel accommodationPanel = new StackPanel();
            accommodationPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            accommodationPanel.Height = 355;
            accommodationPanel.Width = 520;
            return accommodationPanel;
        }

        private TextBlock CreateAccommodationInfo()
        {
            TextBlock accommodationInfo = new TextBlock();
            accommodationInfo.TextWrapping = TextWrapping.Wrap;
            accommodationInfo.Width = 200;
            accommodationInfo.Foreground = Brushes.White;
            accommodationInfo.FontSize = 14;
            accommodationInfo.TextAlignment = TextAlignment.Left;
            accommodationInfo.Margin = new System.Windows.Thickness(30, 80, 10, 10);

            accommodationInfo.Inlines.Add(new Run("Название: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(Accommodation.Name) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Тип: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(Accommodation.AccommodationType.ToString()) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Количество комнат: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(Accommodation.AmountOfRooms.ToString()) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Площадь: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(Accommodation.Area.ToString()) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(" м") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run("2") { BaselineAlignment = BaselineAlignment.Superscript, FontSize = 6, Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Адрес: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(Accommodation.Adress.ToString()) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Номер квартиры: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(Accommodation.FlatNumber.ToString()) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Стоимость: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(Accommodation.Price.ToString()) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(" руб.") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            return accommodationInfo;
        }

        private Image CreateAccommodationImage()
        {
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string imageName = $"pic{accommodation.ImageID}.jpg";
            string imagePath = System.IO.Path.Combine(projectDirectory, "Images", imageName);

            Image image = new Image();
            image.Height = 355;
            image.Width = 250;
            image.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            image.HorizontalAlignment = HorizontalAlignment.Left;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.EndInit();

            image.Source = bitmap;
            return image;
        }

        public void GetAmountOfOrders()
        {
            ordersManager.manager.OpenConnection();

            string selectQuery = "SELECT COUNT(*) FROM [Order]";
            int rowCount = 0;

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, ordersManager.manager.Connection))
            {
                rowCount = Convert.ToInt32(command.ExecuteScalar());
            }

            ordersManager.manager.CloseConnection();

            ordersCounter = rowCount;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            CompanyInfo.Company company = new CompanyInfo.Company();
            string surname = SurnameTextBox.Text;
            string initials = InitialsTextBox.Text;
            string email = EmailTextBox.Text;
            string phone = PhoneTextBox.Text;
            string passportNumber = PassportNumberTextBox.Text;
            string identificationNumber = IdentificationTextBox.Text;
            DateTime startDate = StartDatePicker.Value ?? DateTime.Now;
            DateTime endDate = EndDatePicker.Value ?? DateTime.Now;

            Seller seller = GetSellerByID(accommodation.SellerID);
            Landlord buyer = new Landlord(initials, surname, email, phone, passportNumber, identificationNumber);
            SellContract sellContract = new SellContract(seller);

            GetAmountOfOrders();

            if (currentUserData.Shift == false)
            {
                MessageBox.Show("Откройте рабочую смену в профиле, прежде чем начать совершение сделок.", "Уведомление о необходимости открытия смены");
            }
            else
            {
                if (ValidateFields() == true)
                {
                    string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    string orderName = $"rental_order{++ordersCounter}.docx";
                    string companyContractsPath = System.IO.Path.Combine(projectDirectory, "Rentals", orderName);

                    string userOrderName = $"user_order{ordersCounter}.docx";
                    string personContractsPath = System.IO.Path.Combine(projectDirectory, "UserRentals", userOrderName);

                    sellContract.GenerateSaleContract(companyContractsPath, seller, currentUserData, buyer, Accommodation, ordersCounter, startDate, endDate);
                    sellContract.GenerateSaleContract(personContractsPath, seller, currentUserData, buyer, Accommodation, ordersCounter, startDate, endDate);
                    EnsureDirectoriesExist(companyContractsPath, personContractsPath);

                    ordersManager.AddOrder(currentUserData.ID, Accommodation.ID, startDate, endDate, Accommodation.Price);
                    userOrdersManager.userOrdersList.AddUserOrder(ordersCounter, currentUserData.ID, ordersCounter);

                    saleAccommodationManager.RemoveAccommodation(Accommodation);

                    int lastOrderID = ordersManager.GetLastOrderID();
                    ordersManager.AddExtraInfo(lastOrderID, DateTime.Now);

                    if (isExist(initials, surname, email, phone, passportNumber, identificationNumber) == false)
                    {
                        landlords.AddUser(initials, surname, email, phone, passportNumber, identificationNumber);
                    }

                    double profit = (Accommodation.Price / 100.0) * 2;
                    profit -= (profit / 100.0) * 20;

                    AddUserProfit(manager, currentUserData, ordersCounter, profit);

                    MessageBox.Show("Договор успешно заключен");
                    ProfileWindow window = new ProfileWindow();
                    window.Show();
                    this.Hide();
                }
            }
        }
        public bool isExist(string name, string surname, string email, string phone, string passportNumber, string identificationNumber)
        {
            bool exists = false;

            manager.OpenConnection();

            string selectQuery = "SELECT * FROM Landlords WHERE Name = @Name AND Surname = @Surname AND Email = @Email AND Phone = @Phone AND PassportNumber = @PassportNumber AND IdentificationNumber = @IdentificationNumber";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Surname", surname);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Phone", phone);
                command.Parameters.AddWithValue("@PassportNumber", passportNumber);
                command.Parameters.AddWithValue("@IdentificationNumber", identificationNumber);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        exists = true;
                    }
                }
            }

            manager.CloseConnection();

            return exists;
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

                string insertQuery = " INSERT INTO [OrdersProfit] (OrderID, UserID, Profit) VALUES (@OrderID, @UserID, @Profit);";

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
                {
                    command.Parameters.AddWithValue("@OrderID", orderID);
                    command.Parameters.AddWithValue("@UserID", currentUserData.ID);
                    command.Parameters.Add(new SQLiteParameter("@Profit", DbType.Double)
                    { Value = profit });

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
        public Seller GetSellerByID(int ID)
        {
            string selectQuery = "SELECT * FROM Sellers WHERE ID = @ID";

            try
            {
                manager.OpenConnection();
                using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Seller(
                                reader.GetInt32(reader.GetOrdinal("ID")),
                                reader.GetString(reader.GetOrdinal("Initials")),
                                reader.GetString(reader.GetOrdinal("Surname")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                reader.GetString(reader.GetOrdinal("Phone")),
                                reader.GetString(reader.GetOrdinal("PassportNumber")),
                                reader.GetString(reader.GetOrdinal("IdentificationNumber"))
                            );
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching last seller: {ex.Message}");
                return null;
            }
            finally
            {
                manager.CloseConnection();
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
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            bool isValid = true;

            // Проверка на пустое значение для фамилии
            if (string.IsNullOrWhiteSpace(surname))
            {
                NameNotify.Foreground = new SolidColorBrush(Colors.Red);
                NameNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                NameNotify.Content = "";
            }

            // Проверка на пустое значение для инициалов
            if (string.IsNullOrWhiteSpace(initials))
            {
                SurnameNotify.Foreground = new SolidColorBrush(Colors.Red);
                SurnameNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                SurnameNotify.Content = "";
            }

            if (!IsValidEmail(email))
            {
                EmailNotify.Foreground = new SolidColorBrush(Colors.Red);
                EmailNotify.Content = "Введите корректный email";
                isValid = false;
            }
            else
            {
                EmailNotify.Content = "";
            }
            // Проверка на пустое значение для email
            if (string.IsNullOrWhiteSpace(email))
            {
                EmailNotify.Foreground = new SolidColorBrush(Colors.Red);
                EmailNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                EmailNotify.Content = "";
            }

            // Проверка на пустое значение для телефона
            if (string.IsNullOrWhiteSpace(phone))
            {
                PhoneNotify.Foreground = new SolidColorBrush(Colors.Red);
                PhoneNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                PhoneNotify.Content = "";
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(passportNumber, "^[A-Z]{2}[A-Z0-9]{6,}$"))
            {
                PassportNumberNotify.Foreground = new SolidColorBrush(Colors.Red);
                PassportNumberNotify.Content = "Первые 2 символа должны быть заглавными буквами";
                isValid = false;
            }
            else
            {
                PassportNumberNotify.Content = "";
            }
            // Проверка на пустое значение для номера паспорта и его формат
            if (string.IsNullOrWhiteSpace(passportNumber))
            {
                PassportNumberNotify.Foreground = new SolidColorBrush(Colors.Red);
                PassportNumberNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                PassportNumberNotify.Content = "";
            }

            if (identificationNumber.Length < 14)
            {
                IdentificationNotify.Foreground = new SolidColorBrush(Colors.Red);
                IdentificationNotify.Content = "Укажите полный номер";
                isValid = false;
            }
            else if (identificationNumber.Length == 14 &&
                char.IsUpper(identificationNumber[7]) &&
                char.IsUpper(identificationNumber[11]) &&
                char.IsUpper(identificationNumber[12]))
            {
                IdentificationNotify.Content = "";
            }
            else
            {
                IdentificationNotify.Foreground = new SolidColorBrush(Colors.Red);
                IdentificationNotify.Content = "Некорректный формат номера";
                isValid = false;
            }
            // Проверка на пустое значение для идентификационного номера и его формат
            if (string.IsNullOrWhiteSpace(identificationNumber))
            {
                IdentificationNotify.Foreground = new SolidColorBrush(Colors.Red);
                IdentificationNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                IdentificationNotify.Content = "";
            }

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

            // Проверка на случай, если все поля пустые
            if (string.IsNullOrWhiteSpace(surname) && string.IsNullOrWhiteSpace(initials) &&
                string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone) &&
                string.IsNullOrWhiteSpace(passportNumber) && string.IsNullOrWhiteSpace(identificationNumber) &&
                string.IsNullOrWhiteSpace(StartDatePicker.Text) && string.IsNullOrWhiteSpace(EndDatePicker.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены");
                isValid = false;
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

        private void EndDatePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void StartDatePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }
    }


