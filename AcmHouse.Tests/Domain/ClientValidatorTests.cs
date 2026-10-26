using System;
using System.Collections.Generic;
using AcmHouse.Domain;
using Xunit;

namespace AcmHouse.Tests.Domain
{
    public class ClientValidatorTests
    {
        private static readonly DateTime Today = new DateTime(2026, 5, 1);

        private static RentalForm ValidForm()
        {
            return new RentalForm
            {
                Surname = "Сидоренко",
                Initials = "О.П.",
                Email = "client@example.com",
                Phone = "+375 29 000-00-01",
                PassportNumber = "MP1234567",
                IdentificationNumber = "3010190A001PB1",
                StartDate = Today,
                EndDate = Today.AddDays(5)
            };
        }

        [Fact]
        public void Validate_ReturnsNoErrors_ForValidForm()
        {
            Assert.Empty(ClientValidator.Validate(ValidForm(), Today));
        }

        [Fact]
        public void Validate_ReportsEveryEmptyField()
        {
            Dictionary<string, string> errors = ClientValidator.Validate(new RentalForm(), Today);

            Assert.Equal(8, errors.Count);
            Assert.Equal("Поле должно быть заполнено", errors[ClientValidator.Surname]);
            Assert.True(errors.ContainsKey(ClientValidator.StartDate));
            Assert.True(errors.ContainsKey(ClientValidator.EndDate));
        }

        [Fact]
        public void Validate_ReportsInvalidEmail()
        {
            RentalForm form = ValidForm();
            form.Email = "client@";

            Dictionary<string, string> errors = ClientValidator.Validate(form, Today);

            Assert.Equal("Введите корректный email", errors[ClientValidator.Email]);
        }

        [Theory]
        [InlineData("mp1234567")]
        [InlineData("1234567")]
        [InlineData("MP12")]
        public void Validate_ReportsInvalidPassportNumber(string passport)
        {
            RentalForm form = ValidForm();
            form.PassportNumber = passport;

            Assert.True(ClientValidator.Validate(form, Today).ContainsKey(ClientValidator.PassportNumber));
        }

        [Fact]
        public void Validate_ReportsStartDateInThePast()
        {
            RentalForm form = ValidForm();
            form.StartDate = Today.AddDays(-1);

            Assert.True(ClientValidator.Validate(form, Today).ContainsKey(ClientValidator.StartDate));
        }

        [Fact]
        public void Validate_ReportsEndDateNotAfterStartDate()
        {
            RentalForm form = ValidForm();
            form.EndDate = form.StartDate;

            Assert.True(ClientValidator.Validate(form, Today).ContainsKey(ClientValidator.EndDate));
        }

        [Theory]
        [InlineData("3010190A001PB1", null)]
        [InlineData("", "Поле должно быть заполнено")]
        [InlineData("3010190A001", "Укажите полный номер")]
        [InlineData("3010190a001pb1", "Некорректный формат номера")]
        [InlineData("3010190A001PB12", "Некорректный формат номера")]
        public void ValidateIdentificationNumber_ChecksLengthAndCapitalLetters(string value, string expectedError)
        {
            Assert.Equal(expectedError, ClientValidator.ValidateIdentificationNumber(value));
        }

        [Theory]
        [InlineData("user@example.com", true)]
        [InlineData("first.last@mail.by", true)]
        [InlineData("user@", false)]
        [InlineData("not an email", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void EmailValidator_RecognisesValidAddresses(string email, bool expected)
        {
            Assert.Equal(expected, EmailValidator.IsValid(email));
        }
    }
}
