using AcmHouse.Functions;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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
using AcmHouse.Profile;
using AcmHouse.SQL_Manager;
using AcmHouse.UsersInfo;
using AcmHouse.AccommodationInfo;
using AcmHouse.AccommodationOffers;
using AcmHouse.Authorization;
using AcmHouse.Enums;
using AcmHouse.AccommodationExtraInfo;
using AcmHouse.Rent;

namespace AcmHouse.Admin
{
    public partial class AdminRentWindow : Window
    {
        private AccommodationManager accommodationManager;
        private AccommodationDetailsManager accommodationDetailsManager;
        private int rentalTypeCheck = int.MinValue;
        public AdminRentWindow(int rentalTypeCheck)
        {
            InitializeComponent();

            accommodationManager = new AccommodationManager();
            accommodationDetailsManager = new AccommodationDetailsManager();
            this.rentalTypeCheck = rentalTypeCheck;

            accommodationManager.GetAllAccommodations();


            ShowAccommodations(accommodationsGrid, accommodationManager);

            List<AccommodationType> types;

            if (rentalTypeCheck == 0)
            {
                types = new List<AccommodationType> { AccommodationType.Апартаменты, AccommodationType.Дом, AccommodationType.Вилла, AccommodationType.Коттедж };
                TypeComboBox.ItemsSource = types;
            }
            else if (rentalTypeCheck == 1)
            {
                UpdateAreaNameLabelContent();
                types = new List<AccommodationType> { AccommodationType.Офис, AccommodationType.Склад };
                TypeComboBox.ItemsSource = types;
            }
            List<string> sortBy = new List<string> { "Дешевые сначала", "Дорогие сначала" };
            PriceComboBox.ItemsSource = sortBy;
            TypeComboBox.SelectedIndex = 0;
            PriceComboBox.SelectedIndex = 0;

            TypeComboBox.SelectedValue = null;
            PriceComboBox.SelectedValue = null;
        }


        private void AccommodationButton_Click(object sender, RoutedEventArgs e)
        {
            Button accommodationButton = (Button)sender;
        }
        public void ShowAccommodations(WrapPanel accommodationsGrid, AccommodationManager accommodationManager)
        {
            accommodationsGrid = FindName("accommodationsGrid") as WrapPanel;
            accommodationsGrid.Children.Clear();

            foreach (Accommodation accommodation in accommodationManager.accommodationsList.accommodations)
            {
                if ((rentalTypeCheck == 0 && accommodation.RentalType == 0) ||
                    (rentalTypeCheck == 1 && accommodation.RentalType == 1))
                {
                    AccommodationDetails accommodationDetailsOriginal = accommodationDetailsManager.accommodationDetailsList
                        .FirstOrDefault(ad => ad.AccommodationID == accommodation.ID);
                    if (accommodationDetailsOriginal == null) continue;

                    CreateAccommodationButton(accommodationsGrid, accommodation, accommodationDetailsOriginal);
                }
            }
        }

        public void ShowSearchedAccommodations(WrapPanel accommodationsGrid, List<Accommodation> accommodations)
        {
            accommodationsGrid = FindName("accommodationsGrid") as WrapPanel;
            accommodationsGrid.Children.Clear();

            foreach (Accommodation accommodation in accommodations)
            {
                if ((rentalTypeCheck == 0 && accommodation.RentalType == 0) ||
                    (rentalTypeCheck == 1 && accommodation.RentalType == 1))
                {
                    AccommodationDetails accommodationDetailsOriginal = accommodationDetailsManager.accommodationDetailsList
                        .FirstOrDefault(ad => ad.AccommodationID == accommodation.ID);
                    if (accommodationDetailsOriginal == null) continue;

                    CreateAccommodationButton(accommodationsGrid, accommodation, accommodationDetailsOriginal);
                }
            }
        }
        private void ApplyChanges()
        {
            AccommodationType selectedType;
            string selectedPrice;
            int selectedRoomsAmount;
            double areaAmount;

            // Определяем, выбран ли тип размещения
            bool isTypeSelected = Enum.TryParse<AccommodationType>(TypeComboBox.SelectedValue?.ToString(), out selectedType);
            // Определяем, выбрана ли цена
            bool isPriceSelected = PriceComboBox.SelectedValue != null && !string.IsNullOrEmpty(PriceComboBox.SelectedValue.ToString());
            // Определяем, введено ли количество комнат
            bool isRoomsAmountSelected = int.TryParse(RoomsAmountTextBox.Text, out selectedRoomsAmount);
            // Определяем, введена ли площадь
            bool selectedArea = double.TryParse(AreaTextBox.Text, out areaAmount);

            // Устанавливаем выбранный тип размещения, если он выбран, иначе None
            selectedType = isTypeSelected ? selectedType : AccommodationType.None;
            selectedPrice = PriceComboBox.SelectedValue?.ToString();

            // Получаем список всех размещений
            List<Accommodation> searchedAccommodations = accommodationManager.accommodationsList.accommodations;

            // Фильтруем по типу размещения, если он выбран
            if (isTypeSelected && selectedType != AccommodationType.None)
            {
                searchedAccommodations = searchedAccommodations.Where(accommodation => accommodation.AccommodationType == selectedType).ToList();
            }

            // Фильтруем по количеству комнат, если оно введено
            if (isRoomsAmountSelected)
            {
                searchedAccommodations = searchedAccommodations.Where(accommodation => accommodation.AmountOfRooms == selectedRoomsAmount).ToList();
            }

            // Фильтруем по площади, если она введена
            if (selectedArea)
            {
                searchedAccommodations = searchedAccommodations.Where(accommodation => accommodation.Area == areaAmount).ToList();
            }

            // Сортируем по цене, если цена выбрана
            if (isPriceSelected)
            {
                if (selectedPrice == "Дешевые сначала")
                {
                    searchedAccommodations = searchedAccommodations.OrderBy(accommodation => accommodation.DayPrice).ToList();
                }
                else
                {
                    searchedAccommodations = searchedAccommodations.OrderByDescending(accommodation => accommodation.DayPrice).ToList();
                }
            }

            // Показываем отфильтрованные и отсортированные размещения
            ShowSearchedAccommodations(accommodationsGrid, searchedAccommodations);
        }

        private void CreateAccommodationButton(WrapPanel accommodationsGrid, Accommodation accommodation, AccommodationDetails accommodationDetails)
        {
            Button accommodationButton = new Button
            {
                Tag = accommodation,
                Style = (Style)Application.Current.Resources["NoHoverButtonStyle"],
                BorderBrush = new SolidColorBrush(Color.FromRgb(255, 250, 250)),
                BorderThickness = new Thickness(4),
                Height = 350,
                Width = 240,
                Background = new SolidColorBrush(Color.FromRgb(86, 86, 86))
            };
            accommodationButton.Click += AccommodationButton_Click;

            StackPanel accommodationPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Height = 350
            };

            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string imageName = $"pic{accommodation.ImageID}.jpg";
            string imagePath = System.IO.Path.Combine(projectDirectory, "Images", imageName);

            Image image = new Image
            {
                Height = 190,
                Width = 170,
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute))
            };

            TextBlock accommodationInfo = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Width = 200,
                Foreground = Brushes.White,
                FontSize = 12.5,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(10, 0, 10, 10)
            };

            accommodationInfo.Inlines.Add(new Run("Название: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(accommodation.Name) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Тип: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(accommodation.AccommodationType.ToString()) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Количество комнат: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(accommodation.AmountOfRooms.ToString()) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Площадь: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(accommodation.Area.ToString()) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(" м") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run("2") { BaselineAlignment = BaselineAlignment.Superscript, FontSize = 6, Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Адрес: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(accommodationDetails.Address) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationInfo.Inlines.Add(new Run("Стоимость за сутки: ") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(accommodation.DayPrice.ToString()) { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new Run(" руб.") { Foreground = Brushes.White });
            accommodationInfo.Inlines.Add(new LineBreak());

            accommodationPanel.Children.Add(image);
            accommodationPanel.Children.Add(accommodationInfo);

            accommodationButton.Content = accommodationPanel;

            accommodationsGrid.Children.Add(accommodationButton);
        }
        private void UpdateAreaNameLabelContent()
        {
            if (rentalTypeCheck == 1)
            {
                LabelLine1.Text = "Количество";
                LabelLine2.Text = "помещений";
            }
            else
            {
                LabelLine1.Text = "Количество";
                LabelLine2.Text = "комнат";
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ClearChanges();
        }

        private void ClearChanges()
        {
            TypeComboBox.SelectedValue = null;
            PriceComboBox.SelectedValue = null;
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
        private void RoomsAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyChanges();
        }
        private void AreaAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyChanges();
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rentalTypeCheck == 0)
            {
                List<AccommodationType> types = new List<AccommodationType> { AccommodationType.Апартаменты, AccommodationType.Дом, AccommodationType.Вилла, AccommodationType.Коттедж };
                TypeComboBox.ItemsSource = types;
            }
            else if (rentalTypeCheck == 1)
            {
                List<AccommodationType> types = new List<AccommodationType> { AccommodationType.Офис, AccommodationType.Склад };
                TypeComboBox.ItemsSource = types;
            }
            ApplyChanges();
        }

        private void PriceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyChanges();
            List<string> sortBy = new List<string> { "Дешевые сначала", "Дорогие сначала" };
            PriceComboBox.ItemsSource = sortBy;
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

        private void EventsTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminEventsWindow window = new AdminEventsWindow();
            window.Show();
            this.Hide();
        }

        private void RentalTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminRentalTracking window = new AdminRentalTracking();
            window.Show();
            this.Hide();
        }

        private void OffersTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow window = new AdminWindow();
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
