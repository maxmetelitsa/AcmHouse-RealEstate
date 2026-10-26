using AcmHouse.Tests.Fakes;
using AcmHouse.Enums;
using AcmHouse.ViewModels;
using Xunit;

namespace AcmHouse.Tests.ViewModels
{
    public class SignInViewModelTests
    {
        [Fact]
        public void SignIn_WithEmptyFields_ShowsErrorsAndDoesNotCallTheService()
        {
            var auth = new FakeAuthService();
            var viewModel = new SignInViewModel(auth);

            viewModel.SignIn();

            Assert.Equal("Поле должно быть заполнено", viewModel.EmailError);
            Assert.Equal("Поле должно быть заполнено", viewModel.PasswordError);
            Assert.Equal(0, auth.Calls);
        }

        [Fact]
        public void SignIn_WithMalformedEmail_ShowsFormatError()
        {
            var auth = new FakeAuthService();
            var viewModel = new SignInViewModel(auth) { Email = "admin@", Password = "secret" };

            viewModel.SignIn();

            Assert.Equal("Некорректный формат email", viewModel.EmailError);
            Assert.Equal(0, auth.Calls);
        }

        [Fact]
        public void SignIn_WithWrongCredentials_ShowsErrorAndDoesNotNavigate()
        {
            var auth = new FakeAuthService { Result = LoginResult.Failed };
            var viewModel = new SignInViewModel(auth) { Email = "admin@acmhouse.by", Password = "wrong" };
            bool signedIn = false;
            viewModel.SignedIn += result => signedIn = true;

            viewModel.SignIn();

            Assert.Equal("Неверный email или пароль", viewModel.PasswordError);
            Assert.False(signedIn);
        }

        [Theory]
        [InlineData(LoginResult.Employee)]
        [InlineData(LoginResult.Administrator)]
        public void SignIn_WithValidCredentials_RaisesSignedInWithTheRole(LoginResult role)
        {
            var auth = new FakeAuthService { Result = role };
            var viewModel = new SignInViewModel(auth) { Email = "admin@acmhouse.by", Password = "Admin1234" };
            LoginResult? received = null;
            viewModel.SignedIn += result => received = result;

            viewModel.SignInCommand.Execute(null);

            Assert.Equal(role, received);
            Assert.Equal(string.Empty, viewModel.EmailError);
            Assert.Equal(string.Empty, viewModel.PasswordError);
        }
    }
}
