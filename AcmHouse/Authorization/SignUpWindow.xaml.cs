using AcmHouse.Functions;
using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static AcmHouse.Functions.Functions;
using AcmHouse.UsersInfo;
using AcmHouse.Admin;

namespace AcmHouse.Authorization
{
    public partial class SignUpWindow : Window
    {
        SQLiteManager manager = new SQLiteManager();
        Users usersList;
        public SignUpWindow()
        {
            usersList = new Users();
            InitializeComponent();
        }


       

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (isCorrectRegistration() == true)
            {
                if (usersList.AddUser(manager, TextBoxName.Text, TextBoxSurname.Text, TextBoxEmail.Text, TextBoxPassword.Text, PassportNumberTextBox.Text, IdentificationTextBox.Text) == true)
                {
                    MessageBox.Show("Сотрудник был успешно зарегистрирован", "Регистрация сотрудника");
                    AdminEmployeesTracking window = new AdminEmployeesTracking();
                    window.Show();
                    this.Close();
                }
            }
        }

        private bool isCorrectRegistration()
        {
            string firstName = TextBoxName.Text;
            string lastName = TextBoxSurname.Text;
            string email = TextBoxEmail.Text;
            string password = TextBoxPassword.Text;
            string confirmPassword = TextBoxPasswordRepeat.Text;
            string passportNumber = PassportNumberTextBox.Text;
            string identificationNumber = IdentificationTextBox.Text;
            bool flag = true;
            string clear = "";

            if (string.IsNullOrEmpty(firstName))
            {
                TextBoxNameNotify.Foreground = new SolidColorBrush(Colors.Red);
                TextBoxNameNotify.Content = "Поле должно быть заполнено";
                flag = false;
            }
            else
            {
                TextBoxNameNotify.Content = clear;
            }

            if (string.IsNullOrEmpty(lastName))
            {
                TextBoxSurnameNotify.Foreground = new SolidColorBrush(Colors.Red);
                TextBoxSurnameNotify.Content = "Поле должно быть заполнено";
                flag = false;
            }
            else
            {
                TextBoxSurnameNotify.Content = clear;
            }

            bool isEmailAlreadyUsed = false;

            foreach (User user in usersList.users)
            {
                if (user.Email == email)
                {
                    TextBoxEmailNotify.Foreground = new SolidColorBrush(Colors.Red);
                    TextBoxEmailNotify.Content = "Данный email уже используется";
                    isEmailAlreadyUsed = true;
                    flag = false;
                    break;
                }
            }

            if (!isEmailAlreadyUsed)
            {
                if (string.IsNullOrEmpty(email))
                {
                    TextBoxEmailNotify.Foreground = new SolidColorBrush(Colors.Red);
                    TextBoxEmailNotify.Content = "Поле должно быть заполнено";
                    flag = false;
                }
                else
                {
                    if (!IsValidEmail(email))
                    {
                        TextBoxEmailNotify.Foreground = new SolidColorBrush(Colors.Red);
                        TextBoxEmailNotify.Content = "Некорректный формат email";
                        flag = false;
                    }
                    else
                    {
                        TextBoxEmailNotify.Content = clear;
                    }
                }
            }

            if (string.IsNullOrEmpty(password))
            {
                TextBoxPasswordNotify.Foreground = new SolidColorBrush(Colors.Red);
                TextBoxPasswordNotify.Content = "Поле должно быть заполнено";
                flag = false;
            }
            else
            {
                TextBoxPasswordNotify.Content = clear;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                TextBoxPasswordRepeatNotify.Foreground = new SolidColorBrush(Colors.Red);
                TextBoxPasswordRepeatNotify.Content = "Поле должно быть заполнено";
                flag = false;
            }
            else if (password != confirmPassword)
            {
                TextBoxPasswordRepeatNotify.Foreground = new SolidColorBrush(Colors.Red);
                TextBoxPasswordRepeatNotify.Content = "Пароли не совпадают";
                flag = false;
            }
            else
            {
                TextBoxPasswordRepeatNotify.Content = clear;
            }

            if (string.IsNullOrEmpty(passportNumber))
            {
                PassportNumberNotify.Foreground = new SolidColorBrush(Colors.Red);
                PassportNumberNotify.Content = "Поле должно быть заполнено";
                flag = false;
            }
            else
            {
                PassportNumberNotify.Content = clear;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(passportNumber, "^[A-Z]{2}[A-Z0-9]{6,}$"))
            {
                PassportNumberNotify.Foreground = new SolidColorBrush(Colors.Red);
                PassportNumberNotify.Content = "Первые 2 символа должны быть заглавными английскими буквами";
                flag = false;
            }
            else { PassportNumberNotify.Content = ""; }

            if (string.IsNullOrEmpty(identificationNumber))
            {
                IdentificationNotify.Foreground = new SolidColorBrush(Colors.Red);
                IdentificationNotify.Content = "Поле должно быть заполнено";
                flag = false;
            }
            else
            {
                IdentificationNotify.Content = clear;
            }

            return flag;
        }


        private void WindowHide_Click(object sender, RoutedEventArgs e)
        {
            this.Minimize();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ToggleMaximizeOnDoubleClick(e);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragWithLeftButton(e);
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

    }
}
