using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReaderBackend.Utils
{
    public static class CredentialsHelper
    {
        public static string AreValidUserCredentials(string login, string password)
        {
            //here should be list of errors
            StringBuilder error = new StringBuilder();

            if (login is null || login.Length < 4)
                error.Append("Login must at least 4 characters").Append(Environment.NewLine);

            if (!login.All(c => char.IsLetterOrDigit(c)))
                error.Append("Login must contain only letters and digits").Append(Environment.NewLine);

            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,30}$"))
                error.Append("Password must contain at least 8 characters and at least 1 digit").Append(Environment.NewLine);


            return error.ToString();
        }
    }
}