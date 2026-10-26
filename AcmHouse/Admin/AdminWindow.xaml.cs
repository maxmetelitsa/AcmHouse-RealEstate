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
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace AcmHouse.Admin
{
    public partial class AdminWindow : Window
    {
        private AccommodationManager accommodationManager;
        private AccommodationDetailsManager accommodationDetailsManager;
        private bool isEditMode = false;
        private int lastEditableAcID;
        public AdminWindow()
        {
            InitializeComponent();

            accommodationManager = new AccommodationManager();
            accommodationDetailsManager = new AccommodationDetailsManager();

            accommodationManager.GetAllAccommodations();
            accommodationDetailsManager.GetAccommodationDetails();
            acGrid.AutoGeneratingColumn += accommodationsGrid_AutoGeneratingColumn;

            ShowAcInDataGrid(acGrid, accommodationManager);

            List<AccommodationType> types = new List<AccommodationType> {
            AccommodationType.Апартаменты,
            AccommodationType.Дом,
            AccommodationType.Вилла,
            AccommodationType.Коттедж
            };

            TypeComboBox.ItemsSource = types;
            TypeComboBox.SelectedValue = null;

            TypeComboBox.SelectionChanged += TypeComboBox_SelectionChanged;
        }


        public void ShowAcInDataGrid(DataGrid acGrid, AccommodationManager accommodationManager)
        {
            accommodationManager.manager.OpenConnection();

            string selectQuery = "SELECT ID, Name AS [Название], " +
            "CASE " +
                "WHEN AccommodationType = 1 THEN 'Апартаменты' " +
                "WHEN AccommodationType = 2 THEN 'Дом' " +
                "WHEN AccommodationType = 3 THEN 'Вилла' " +
                "WHEN AccommodationType = 4 THEN 'Коттедж' " +
            "END AS [Тип жилья], " +
            "AmountOfRooms AS [Количество комнат], " +
            "Area AS [Площадь], " +
            "DayPrice AS [Стоимость за сутки], " +
            "ImageID " +
            "FROM Accommodations WHERE RentalType = 0";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, accommodationManager.manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    acGrid.ItemsSource = dataTable.DefaultView;
                }
            }

            accommodationManager.manager.CloseConnection();
        }

        private void ACAddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            AccommodationType selectedType;
            int rooms;
            double area;
            string address = AdressTextBox.Text;
            string flatNumber = FlatNumberTextBox.Text;
            int imageID;
            double hourPrice;

            if ((ValidateFields() == true))
            {
                selectedType = (AccommodationType)TypeComboBox.SelectedItem;

                if (int.TryParse(RoomsTextBox.Text, out rooms) && double.TryParse(AreaTextBox.Text, out area) && int.TryParse(ImageIDTextBox.Text, out imageID) && double.TryParse(HourPriceTextBox.Text, out hourPrice))
                {
                    int lastID = Functions.Functions.GetLastOfAllAccommodationsID();
                    Accommodation ac = new Accommodation(++lastID, name, selectedType, rooms, area, hourPrice, imageID, 0);
                    accommodationManager.AddAccommodation(ac);

                    int lastAccommodationID = accommodationDetailsManager.GetLastAccommodationID();
                    AccommodationDetails acDetails = new AccommodationDetails(lastAccommodationID, address, flatNumber);

                    accommodationDetailsManager.AddAccommodationDetails(acDetails);

                    ShowAcInDataGrid(acGrid, accommodationManager);
                    ClearFieldsAndNotifies();
                }
            }
        }



        private void ACDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (acGrid.SelectedItem != null)
            {
                var rowView = acGrid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    Int64 ID = (Int64)rowView["ID"];
                    string name = rowView["Название"] as string;
                    AccommodationType selectedType;
                    if (Enum.TryParse(rowView["Тип жилья"] as string, out selectedType))
                    {
                        Int64 rooms = (Int64)rowView["Количество комнат"];
                        double area = Convert.ToDouble(rowView["Площадь"]);
                        double dayPrice = Convert.ToDouble(rowView["Стоимость за сутки"]);
                        Int64 imageID = (Int64)rowView["ImageID"];

                        if (ValidateFields() == true)
                        {
                            Accommodation selectedAc = new Accommodation((int)ID, name, selectedType, (int)rooms, area, dayPrice, (int)imageID, 0);

                            accommodationManager.RemoveAccommodation(selectedAc);
                            ShowAcInDataGrid(acGrid, accommodationManager);
                            return; 
                        }
                    }
                }
            }

            MessageBox.Show("No valid accommodation selected.");
        }

        private void ACEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (acGrid.SelectedItem != null)
            {
                var rowView = acGrid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    Int64 ID = (Int64)rowView["ID"];
                    string name = rowView["Название"] as string;
                    string typeValue = rowView["Тип жилья"] as string;
                    Int64 rooms = (Int64)rowView["Количество комнат"];
                    double area = Convert.ToDouble(rowView["Площадь"]);
                    double dayPrice = Convert.ToDouble(rowView["Стоимость за сутки"]);
                    Int64 imageID = (Int64)rowView["ImageID"];

                    string acAddress = "";
                    string acFlat = "";

                    accommodationDetailsManager.manager.OpenConnection();

                    string selectQuery = "SELECT * FROM AccommodationDetails WHERE AccommodationID = @AccommodationID";

                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, accommodationDetailsManager.manager.Connection))
                    {
                        command.Parameters.AddWithValue("@AccommodationID", ID);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                acAddress = reader["Adress"].ToString();
                                acFlat = reader["FlatNumber"].ToString();
                            }
                        }
                    }

                    AccommodationType selectedAc;
                    if (Enum.TryParse(typeValue, out selectedAc))
                    {
                        NameTextBox.Text = name;
                        TypeComboBox.SelectedItem = selectedAc;
                        RoomsTextBox.Text = rooms.ToString();
                        AreaTextBox.Text = area.ToString();
                        AdressTextBox.Text = acAddress;
                        FlatNumberTextBox.Text = acFlat;
                        ImageIDTextBox.Text = imageID.ToString();
                        HourPriceTextBox.Text = dayPrice.ToString();

                        isEditMode = true;
                        lastEditableAcID = (int)ID;
                    }

                    accommodationDetailsManager.manager.CloseConnection();
                }
            }
            else
            {
                ClearFields();
            }
        }


        private void ACSaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            var rowView = acGrid.SelectedItem as DataRowView;
            if (isEditMode && lastEditableAcID == (int)(Int64)rowView["ID"])
            {
                Int64 ID = 0;
                if (rowView != null)
                {
                    ID = (Int64)rowView["ID"];
                }

                string name = NameTextBox.Text;
                AccommodationType selectedType;
                int rooms;
                double area;
                int imageID;
                double dayPrice;

                if (TypeComboBox.SelectedItem is AccommodationType)
                {
                    selectedType = (AccommodationType)TypeComboBox.SelectedItem;
                    if(ValidateFields() == true)
                    {
                        if (!string.IsNullOrWhiteSpace(name) &&
                        int.TryParse(RoomsTextBox.Text, out rooms) &&
                        double.TryParse(AreaTextBox.Text, out area) &&
                        int.TryParse(ImageIDTextBox.Text, out imageID) &&
                        double.TryParse(HourPriceTextBox.Text, out dayPrice))
                        {
                            string updateQuery = "UPDATE Accommodations SET Name = @Name, AccommodationType = @AccommodationType, AmountOfRooms = @AmountOfRooms, Area = @Area, DayPrice = @DayPrice, ImageID = @ImageID WHERE ID = @ID";

                            accommodationManager.manager.OpenConnection();

                            using (SQLiteCommand command = new SQLiteCommand(updateQuery, accommodationManager.manager.Connection))
                            {
                                command.Parameters.AddWithValue("@Name", name);
                                command.Parameters.AddWithValue("@AccommodationType", (int)selectedType);
                                command.Parameters.AddWithValue("@AmountOfRooms", rooms);
                                command.Parameters.AddWithValue("@Area", area);
                                command.Parameters.AddWithValue("@ImageID", imageID);
                                command.Parameters.AddWithValue("@DayPrice", dayPrice);
                                command.Parameters.AddWithValue("@ID", ID);

                                command.ExecuteNonQuery();
                            }

                            accommodationManager.manager.CloseConnection();
                            isEditMode = false;
                            ClearFields();
                            ShowAcInDataGrid(acGrid, accommodationManager);
                        }
                        else
                        {
                            MessageBox.Show("Неверные значения.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выбран неверный тип либо тип пуст.");
                    }
                }
                    
            }
        }

        private bool ValidateFields()
        {
            bool isValid = true;

            Regex nameRegex = new Regex("^[a-zA-Zа-яА-Я]+$");


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

            return isValid;
        }


        private void ClearFields()
        {
            NameTextBox.Text = null;
            TypeComboBox.SelectedItem = null;
            RoomsTextBox.Text = null;
            AreaTextBox.Text = null;
            AdressTextBox.Text = null;
            FlatNumberTextBox.Text = null;
            ImageIDTextBox.Text = null;
            HourPriceTextBox.Text = null;
        }
        public void ClearNotifies()
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
        public void ClearFieldsAndNotifies()
        {
            NameTextBox.Text = null;
            TypeComboBox.SelectedItem = null;
            RoomsTextBox.Text = null;
            AreaTextBox.Text = null;
            AdressTextBox.Text = null;
            FlatNumberTextBox.Text = null;
            ImageIDTextBox.Text = null;
            HourPriceTextBox.Text = null;


            NameNotify.Content = "";
            TypeNotify.Content = "";
            RoomsNotify.Content = "";
            AreaNotify.Content = "";
            AddressNotify.Content = "";
            FlatNumberNotify.Content = "";
            ImageNotify.Content = "";
            HourPriceNotify.Content = "";
        }
        private void accommodationsGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ID")
            {
                e.Cancel = true;
            }
            if (e.PropertyName == "ImageID")
            {
                e.Cancel = true;
            }
        }
        private void ACClearFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFieldsAndNotifies();
        }

        private void ACClearTableButton_Click(object sender, RoutedEventArgs e)
        {
            accommodationManager.ClearAccomodationsTable();
            ShowAcInDataGrid(acGrid, accommodationManager);
        }
        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeComboBox.ItemsSource == null)
            {
                List<AccommodationType> types = new List<AccommodationType> {
                AccommodationType.Апартаменты,
                AccommodationType.Дом,
                AccommodationType.Вилла,
                AccommodationType.Коттедж
                };
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

        private void RentalTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminRentalTracking window = new AdminRentalTracking();
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

        private void EventsTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminEventsWindow window = new AdminEventsWindow();
            window.Show();
            this.Hide();
        }
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            AdminReportWindow window = new AdminReportWindow();
            window.Show();
            this.Hide();
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
    }
}
