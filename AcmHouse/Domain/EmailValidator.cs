using System;
using System.Net.Mail;

namespace AcmHouse.Domain
{
    public static class EmailValidator
    {
        public static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var address = new MailAddress(email);
                return address.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
