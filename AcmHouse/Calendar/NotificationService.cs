using AcmHouse.Profile;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace AcmHouse.Calendar
{
    public class NotificationService
    {
        private readonly CalendarViewModel _viewModel;

        public NotificationService(CalendarViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            DispatcherTimer notificationTimer = new DispatcherTimer();
            notificationTimer.Interval = TimeSpan.FromSeconds(60); // Проверяем каждую минуту
            notificationTimer.Tick += CheckNotifications;
            notificationTimer.Start();
        }

        private void CheckNotifications(object sender, EventArgs e)
        {
            foreach (var calendarEvent in _viewModel.Events)
            {
                if (calendarEvent.Notify == 1 && calendarEvent.StartTime <= DateTime.Now.AddMinutes(15) && calendarEvent.StartTime > DateTime.Now)
                {
                    ShowNotification($"Предстоящее событие: {calendarEvent.Title} в {calendarEvent.StartTime}");
                }
            }
        }

        public void ShowNotification(string message)
        {
            MessageBox.Show(message, "Уведомление о событии", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
