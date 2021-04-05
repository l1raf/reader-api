using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace ReaderBackend.Utils
{
    public static class CredentialsHelper
    {
        public static string AreValidUserCredentials(string email, string password)
        {
            string error = null;

            try
            {
                MailAddress m = new MailAddress(email);

                if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,30}$"))
                    error = "Wrong password. Make sure it contains at least 8 characters and at least 1 number.";
            }
            catch (FormatException)
            {
                error = "Wrong email format.";
            }

            return error;
        }
    }
}