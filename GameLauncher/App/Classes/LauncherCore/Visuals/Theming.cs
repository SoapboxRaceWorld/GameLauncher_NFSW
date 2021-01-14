using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GameLauncher.App.Classes.LauncherCore.Visuals
{
    class Theming
    {
        private static readonly IniFile ThemeFile = new IniFile("Theme.ini");

        private static string ThemeFolder = AppDomain.CurrentDomain.BaseDirectory + "Theme";

        public static string ThemeName = "Default";

        public static string ThemeAuthor = "Launcher Divison";


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

        public static Color BlueBackColorButton = Color.FromArgb(22, 29, 38);

        public static Color BlueBorderColorButton = Color.FromArgb(77, 181, 191);

        public static Color BlueForeColorButton = Color.FromArgb(192, 192, 192);

        public static Color BlueMouseOverBackColorButton = Color.FromArgb(44, 58, 76);


        public static Color YellowBackColorButton = Color.FromArgb(22, 29, 38);

        public static Color YellowBorderColorButton = Color.FromArgb(184, 134, 11);

        public static Color YellowForeColorButton = Color.FromArgb(44, 58, 76);

        public static Color YellowMouseOverBackColorButton = Color.FromArgb(44, 58, 76);


        /* Read Theme File and Check Values */
        public static void CheckIfThemeExists()
        {
            /* Main Backgrounds */

            if (!string.IsNullOrEmpty(ThemeFile.Read("SettingsScreenBG")))
            {
                if (File.Exists(ThemeFolder + "\\SettingsScreen\\" + ThemeFile.Read("SettingsScreenBG")))
                {
                    SettingsScreen = new Bitmap(ThemeFolder + "\\SettingsScreen\\" + ThemeFile.Read("SettingsScreenBG"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("MainScreenBG")))
            {
                if (File.Exists(ThemeFolder + "\\MainScreen\\" + ThemeFile.Read("MainScreenBG")))
                {
                    SettingsScreen = new Bitmap(ThemeFolder + "\\MainScreen\\" + ThemeFile.Read("MainScreenBG"));
                }
            }

            /* MainScreen Icons */

            if (!string.IsNullOrEmpty(ThemeFile.Read("UpdateErrorIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateErrorIcon")))
                {
                    UpdateIconError = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateErrorIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("UpdateSuccessIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateSuccessIcon")))
                {
                    UpdateIconSuccess = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateSuccessIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("UpdateUnkownIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateUnkownIcon")))
                {
                    UpdateIconUnknown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateUnkownIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("UpdateWarningIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateWarningIcon")))
                {
                    UpdateIconWarning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateWarningIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("APICheckingIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APICheckingIcon")))
                {
                    APIIconChecking = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APICheckingIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("APIErrorIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIErrorIcon")))
                {
                    APIIconError = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIErrorIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("APISuccessIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APISuccessIcon")))
                {
                    APIIconSuccess = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APISuccessIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("APIUnkownIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIUnkownIcon")))
                {
                    APIIconUnkown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIUnkownIcon"));
                }
            }

            /* Image Buttons */

            if (!string.IsNullOrEmpty(ThemeFile.Read("GrayButton")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButton")))
                {
                    GrayButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButton"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("GrayButtonClick")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButtonClick")))
                {
                    GrayButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButtonClick"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("GrayButtonHover")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButtonHover")))
                {
                    GrayButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButtonHover"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("GreenButton")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButton")))
                {
                    GreenButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButton"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("GreenButtonClick")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButtonClick")))
                {
                    GreenButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButtonClick"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("GreenButtonHover")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButtonHover")))
                {
                    GreenButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButtonHover"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("CloseButton")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("CloseButton")))
                {
                    CloseButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("CloseButton"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("GearButton")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButton")))
                {
                    GearButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButton"));
                }
            }

            /* WinForm Buttons */

            if (!string.IsNullOrEmpty(ThemeFile.Read("BlueBackColorButton")))
            {
                BlueBackColorButton = ToColor(ThemeFile.Read("BlueBackColorButton"));
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("BlueBorderColorButton")))
            {
                BlueBorderColorButton = ToColor(ThemeFile.Read("BlueBorderColorButton"));
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("BlueForeColorButton")))
            {
                BlueForeColorButton = ToColor(ThemeFile.Read("BlueForeColorButton"));
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("BlueMouseOverBackColorButton")))
            {
                BlueMouseOverBackColorButton = ToColor(ThemeFile.Read("BlueMouseOverBackColorButton"));
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("YellowBackColorButton")))
            {
                YellowBackColorButton = ToColor(ThemeFile.Read("YellowBackColorButton"));
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("YellowBorderColorButton")))
            {
                YellowBorderColorButton = ToColor(ThemeFile.Read("YellowBorderColorButton"));
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("YellowForeColorButton")))
            {
                YellowForeColorButton = ToColor(ThemeFile.Read("YellowForeColorButton"));
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("YellowMouseOverBackColorButton")))
            {
                YellowMouseOverBackColorButton = ToColor(ThemeFile.Read("YellowMouseOverBackColorButton"));
            }

        }


        /* Convert User Inputed String into a Valid RBG Spectrum Values */

        private static Color ToColor(string color)
        {
            var arrColorFragments = color?.Split(',').Select(sFragment => { int.TryParse(sFragment, out int fragment); return fragment; }).ToArray();

            switch (arrColorFragments?.Length)
            {
                case 3:
                    /* Regular RGB Conversion */
                    return Color.FromArgb(arrColorFragments[0], arrColorFragments[1], arrColorFragments[2]);
                case 4:
                    /* Regular ARGB Conversion */
                    return Color.FromArgb(arrColorFragments[0], arrColorFragments[1], arrColorFragments[2], arrColorFragments[3]);
                default:
                    /* Fail Safe Color */
                    return Color.Silver;
            }
        }

    }
}
