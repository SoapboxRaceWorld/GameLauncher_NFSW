using System;
using System.Drawing;
using System.IO;

namespace GameLauncher.App.Classes.LauncherCore.Visuals
{
    class Theming
    {
        private static readonly IniFile ThemeFile = new IniFile("Theme.ini");

        private static string ThemeFolder = AppDomain.CurrentDomain.BaseDirectory + "Theme";


        /* Main Backgrounds */

        public static Bitmap MainScreen = Properties.Resources.mainbackground;

        public static Bitmap SettingsScreen = Properties.Resources.secondarybackground;


        /* MainScreen Icons */

        public static Bitmap UpdateIconError = Properties.Resources.ac_error;

        public static Bitmap UpdateIconSuccess = Properties.Resources.ac_success;

        public static Bitmap UpdateIconUnknown = Properties.Resources.ac_unknown;

        public static Bitmap UpdateIconWarning = Properties.Resources.ac_warning;


        public static Bitmap APIIconChecking = Properties.Resources.api_checking;

        public static Bitmap APIIconError = Properties.Resources.api_error;

        public static Bitmap APIIconSuccess = Properties.Resources.api_success;

        public static Bitmap APIIconUnkown = Properties.Resources.api_unkown;


        /* Image Buttons */

        public static Bitmap GrayButton = Properties.Resources.graybutton;

        public static Bitmap GrayButtonClick = Properties.Resources.graybutton_click;

        public static Bitmap GrayButtonHover = Properties.Resources.graybutton_hover;


        public static Bitmap GreenButton = Properties.Resources.graybutton;

        public static Bitmap GreenButtonClick = Properties.Resources.graybutton_click;

        public static Bitmap GreenButtonHover = Properties.Resources.graybutton_hover;


        public static Bitmap CloseButton = Properties.Resources.close;

        public static Bitmap GearButton = Properties.Resources.settingsbtn;


        /* WinForm Buttons */

        /* Read Theme File and Check Values */
        public static void CheckIfThemeExists()
        {
            if (!string.IsNullOrEmpty(ThemeFile.Read("SettingsScreenBG")))
            {
                if (File.Exists(ThemeFolder + "\\Settings\\" + ThemeFile.Read("SettingsScreenBG")))
                {
                    SettingsScreen = new Bitmap(ThemeFolder + "\\Settings\\" + ThemeFile.Read("SettingsScreenBG"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("MainScreenBG")))
            {
                if (File.Exists(ThemeFolder + "\\Settings\\" + ThemeFile.Read("MainScreenBG")))
                {
                    SettingsScreen = new Bitmap(ThemeFolder + "\\Settings\\" + ThemeFile.Read("MainScreenBG"));
                }
            }

        }

        /* Global Events Ex. -> Hover Events */
    }
}
