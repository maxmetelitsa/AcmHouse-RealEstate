using System.Windows;

namespace AcmHouse.Services
{
    public class MessageBoxService : IMessageService
    {
        public void ShowInfo(string message, string title = "")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message, string title = "")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
