namespace AcmHouse.Services
{
    /// <summary>Shows messages to the user. Lets view models stay free of MessageBox calls.</summary>
    public interface IMessageService
    {
        void ShowInfo(string message, string title = "");

        void ShowError(string message, string title = "");
    }
}
