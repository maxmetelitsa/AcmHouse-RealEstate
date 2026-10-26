using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AcmHouse.Domain
{
    /// <summary>Data entered in the rental form.</summary>
    public class RentalForm
    {
        public string Surname { get; set; }
        public string Initials { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PassportNumber { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    /// <summary>Validates the rental form and returns an error message per field.</summary>
    public static class ClientValidator
    {
        public const string Surname = nameof(RentalForm.Surname);
        public const string Initials = nameof(RentalForm.Initials);
        public const string Email = nameof(RentalForm.Email);
        public const string Phone = nameof(RentalForm.Phone);
        public const string PassportNumber = nameof(RentalForm.PassportNumber);
        public const string IdentificationNumber = nameof(RentalForm.IdentificationNumber);
        public const string StartDate = nameof(RentalForm.StartDate);
        public const string EndDate = nameof(RentalForm.EndDate);

        private const string Required = "Поле должно быть заполнено";
        private const int IdentificationNumberLength = 14;

        // Belarusian passport: two capital letters followed by digits, e.g. MP1234567.
        private static readonly Regex PassportPattern = new Regex("^[A-Z]{2}[A-Z0-9]{6,}$");

        public static Dictionary<string, string> Validate(RentalForm form, DateTime today)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            var errors = new Dictionary<string, string>();

            RequireText(errors, Surname, form.Surname);
            RequireText(errors, Initials, form.Initials);
            RequireText(errors, Phone, form.Phone);

            if (string.IsNullOrWhiteSpace(form.Email))
            {
                errors[Email] = Required;
            }
            else if (!EmailValidator.IsValid(form.Email))
            {
                errors[Email] = "Введите корректный email";
            }

            if (string.IsNullOrWhiteSpace(form.PassportNumber))
            {
                errors[PassportNumber] = Required;
            }
            else if (!PassportPattern.IsMatch(form.PassportNumber))
            {
                errors[PassportNumber] = "Формат: 2 заглавные буквы и номер, например MP1234567";
            }

            string identificationError = ValidateIdentificationNumber(form.IdentificationNumber);
            if (identificationError != null)
            {
                errors[IdentificationNumber] = identificationError;
            }

            if (form.StartDate == null)
            {
                errors[StartDate] = Required;
            }
            else if (form.StartDate.Value.Date < today.Date)
            {
                errors[StartDate] = "Дата начала не может быть в прошлом";
            }

            if (form.EndDate == null)
            {
                errors[EndDate] = Required;
            }
            else if (form.StartDate != null && form.EndDate.Value <= form.StartDate.Value)
            {
                errors[EndDate] = "Дата окончания аренды должна быть позже даты начала";
            }

            return errors;
        }

        /// <summary>
        /// Belarusian personal identification number: 14 characters with capital letters
        /// at positions 8, 12 and 13, e.g. 3120399A001PB4.
        /// </summary>
        public static string ValidateIdentificationNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Required;
            }

            if (value.Length < IdentificationNumberLength)
            {
                return "Укажите полный номер";
            }

            bool hasValidFormat = value.Length == IdentificationNumberLength
                && char.IsUpper(value[7])
                && char.IsUpper(value[11])
                && char.IsUpper(value[12]);

            return hasValidFormat ? null : "Некорректный формат номера";
        }

        private static void RequireText(Dictionary<string, string> errors, string field, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors[field] = Required;
            }
        }
    }
}
