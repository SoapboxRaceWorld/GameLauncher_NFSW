using System;
using System.Text.RegularExpressions;

namespace GameLauncher.App.Classes.LauncherCore.Validator.Email
{
    class IsEmailValid
    {
        public static bool Validate(string email)
        {
            String EmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
               + "@"
               + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

            return Regex.IsMatch(email, EmailPattern);
        }

        public static string Mask(string email)
        {
            string Pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";

            return Regex.Replace(email, Pattern, m => new string('*', m.Length));
        }
    }
}
