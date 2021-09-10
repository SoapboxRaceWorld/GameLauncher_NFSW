using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.Text.RegularExpressions;

namespace GameLauncher.App.Classes.LauncherCore.Validator.Email
{
    class IsEmailValid
    {
        public static bool Validate(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            else
            {
                try
                {
                    String EmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@"
                    + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

                    return Regex.IsMatch(email, EmailPattern);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Validate", null, Error, null, true);
                    return false;
                }
            }
        }

        public static string Mask(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return email;
            }
            else
            {
                try
                {
                    string Pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";

                    return Regex.Replace(email, Pattern, m => new string('*', m.Length));
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Mask", null, Error, null, true);
                    return "EMAIL IS HIDDEN";
                }
            }
        }
    }
}
