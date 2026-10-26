using System;
using AcmHouse.Domain;
using AcmHouse.Enums;
using AcmHouse.Mvvm;
using AcmHouse.Services;

namespace AcmHouse.ViewModels
{
    public class SignInViewModel : ViewModelBase
    {
        private const string Required = "Поле должно быть заполнено";

        private readonly IAuthService authService;

        private string email = string.Empty;
        private string password = string.Empty;
        private string emailError = string.Empty;
        private string passwordError = string.Empty;

        public SignInViewModel(IAuthService authService)
        {
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            SignInCommand = new RelayCommand(SignIn);
        }

        /// <summary>Raised after a successful sign-in; the view opens the matching window.</summary>
        public event Action<LoginResult> SignedIn;

        public RelayCommand SignInCommand { get; }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public string EmailError
        {
            get => emailError;
            private set => SetProperty(ref emailError, value);
        }

        public string PasswordError
        {
            get => passwordError;
            private set => SetProperty(ref passwordError, value);
        }

        public void SignIn()
        {
            if (!ValidateInput())
            {
                return;
            }

            LoginResult result = authService.SignIn(Email, Password);
            if (result == LoginResult.Failed)
            {
                PasswordError = "Неверный email или пароль";
                return;
            }

            SignedIn?.Invoke(result);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(Email))
            {
                EmailError = Required;
            }
            else if (!EmailValidator.IsValid(Email))
            {
                EmailError = "Некорректный формат email";
            }
            else
            {
                EmailError = string.Empty;
            }

            PasswordError = string.IsNullOrEmpty(Password) ? Required : string.Empty;

            return EmailError.Length == 0 && PasswordError.Length == 0;
        }
    }
}
