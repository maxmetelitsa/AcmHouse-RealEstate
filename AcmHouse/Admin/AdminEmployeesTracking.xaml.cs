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
using Users = AcmHouse.UsersInfo.Users;

namespace AcmHouse.Admin
{
    public partial class AdminEmployeesTracking : Window
    {
        SQLiteManager manager = new SQLiteManager();
        Users usersList;

        public AdminEmployeesTracking()
        {
            InitializeComponent();
            employeesGrid.AutoGeneratingColumn += employeesGrid_AutoGeneratingColumn;
            usersList = new Users();
            usersList.GetAllUsers(manager);
            ShowEmployeesInDataGrid(employeesGrid, usersList);
        }


        public void ShowEmployeesInDataGrid(DataGrid employeesGrid, Users usersList)
        {
            manager.OpenConnection();

            string selectQuery = "SELECT ID, Name AS [Имя], Surname AS [Фамилия], Email AS [Почта], PassportNumber AS [Номер паспорта], " +
                                 "IdentificationNumber AS [Идентификационный номер], Shift AS [Смена] FROM [Users]";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    dataTable.Load(reader);

                    dataTable.Columns.Add("Статус Смены", typeof(string));

                    foreach (System.Data.DataRow row in dataTable.Rows)
                    {
                        Int64 shiftValue = Convert.ToInt64(row["Смена"]);
                        row["Статус Смены"] = shiftValue == 0 ? "Закрыта" : "Открыта";
                    }

                    dataTable.Columns.Remove("Смена");

                    employeesGrid.ItemsSource = dataTable.DefaultView;
                }
            }

            manager.CloseConnection();
        }



        private void EmployeeDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (employeesGrid.SelectedItem != null)
            {
                DataRowView rowView = employeesGrid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    Int64 ID = Convert.ToInt64(rowView["ID"]);
                    string name = rowView["Имя"].ToString();
                    string surname = rowView["Фамилия"].ToString();
                    string email = rowView["Почта"].ToString();
                    string password = rowView["Пароль"].ToString();
                    string passportNumber = rowView["Номер паспорта"].ToString();
                    string identificationNumber = rowView["Идентификационный номер"].ToString();

                    string shiftString = rowView["Статус Смены"].ToString();
                    bool shift = shiftString == "Открыта";

                    User selectedUser = new User(name, surname, email, password, passportNumber, identificationNumber, shift);

                    usersList.RemoveUser(selectedUser, (int)ID);

                    ShowEmployeesInDataGrid(employeesGrid, usersList);
                }
            }
            else
            {
                MessageBox.Show("No order selected.");
            }
        }

        private void LookButton_Click(object sender, RoutedEventArgs e)
        {
            if (employeesGrid.SelectedItem != null)
            {
                DataRowView rowView = employeesGrid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    Int64 ID = Convert.ToInt64(rowView["ID"]);
                    EmployeeWindow window = new EmployeeWindow((int)ID);
                    window.Show();
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника");
            }
        }


        private void employeesGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ID")
            {
                e.Cancel = true;
            }
            if (e.PropertyName == "Password")
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
        private void RentalTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminRentalTracking window = new AdminRentalTracking();
            window.Show();
            this.Hide();
        }

        private void EventsTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminEventsWindow window = new AdminEventsWindow();
            window.Show();
            this.Hide();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ToggleMaximizeOnDoubleClick(e);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow window = new SignInWindow();
            window.Show();
            this.Hide();
        }

        private void EmployeeAddButton_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow window = new SignUpWindow();
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
