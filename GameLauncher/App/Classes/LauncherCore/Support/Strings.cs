using System;
using System.Text;

namespace GameLauncher.App.Classes.LauncherCore.Support
{
    class Strings
    {
        public static string Encode(string String_Text)
        {
            if (string.IsNullOrWhiteSpace(String_Text))
            {
                return string.Empty;
            }
            else
            {
                return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(String_Text));
            }
        }

        public static string Truncate(string String_Text, int Max_Text_Length)
        {
            return string.IsNullOrWhiteSpace(String_Text) ?
                string.Empty : String_Text.Substring(0, Math.Max(0, Math.Min(String_Text.Length, Max_Text_Length)));
        }
    }
}
