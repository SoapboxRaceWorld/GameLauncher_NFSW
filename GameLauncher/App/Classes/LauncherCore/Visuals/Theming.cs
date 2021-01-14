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


        /* Theme Name & Author */

        public static string ThemeName = "Default";

        public static string ThemeAuthor = "Launcher Divison";


        /* Logo */

        public static Bitmap Logo = Properties.Resources.mainbackground;


        /* Main Backgrounds */

        public static Bitmap MainScreen = Properties.Resources.mainbackground;

        public static Bitmap SettingsScreen = Properties.Resources.secondarybackground;

        public static Bitmap SocialPanel = Properties.Resources.socialbg;


        /* MainScreen Icons */

        public static Bitmap UpdateIconError = Properties.Resources.ac_error;

        public static Bitmap UpdateIconSuccess = Properties.Resources.ac_success;

        public static Bitmap UpdateIconUnknown = Properties.Resources.ac_unknown;

        public static Bitmap UpdateIconWarning = Properties.Resources.ac_warning;


        public static Bitmap APIIconChecking = Properties.Resources.api_checking;

        public static Bitmap APIIconError = Properties.Resources.api_error;

        public static Bitmap APIIconSuccess = Properties.Resources.api_success;

        public static Bitmap APIIconUnkown = Properties.Resources.api_unkown;


        public static Bitmap ServerIconChecking = Properties.Resources.server_checking;

        public static Bitmap ServerIconOffline = Properties.Resources.server_offline;

        public static Bitmap AServerIconSuccess = Properties.Resources.server_online;

        public static Bitmap ServerIconUnkown = Properties.Resources.server_unknown;


        public static Bitmap DiscordIcon = Properties.Resources.social_discord;

        public static Bitmap DiscordIconDisabled = Properties.Resources.social_discord_disabled;

        public static Bitmap FacebookIcon = Properties.Resources.social_facebook;

        public static Bitmap FacebookIconDisabled = Properties.Resources.social_facebook_disabled;

        public static Bitmap HomeIcon = Properties.Resources.social_home_page;

        public static Bitmap HomeIconDisabled = Properties.Resources.social_home_page_disabled;

        public static Bitmap TwitterIcon = Properties.Resources.social_twitter;

        public static Bitmap TwitterIconDisabled = Properties.Resources.social_twitter_disabled;


        /* Image Buttons */

        public static Bitmap GrayButton = Properties.Resources.graybutton;

        public static Bitmap GrayButtonClick = Properties.Resources.graybutton_click;

        public static Bitmap GrayButtonHover = Properties.Resources.graybutton_hover;


        public static Bitmap GreenButton = Properties.Resources.graybutton;

        public static Bitmap GreenButtonClick = Properties.Resources.graybutton_click;

        public static Bitmap GreenButtonHover = Properties.Resources.graybutton_hover;


        public static Bitmap CloseButton = Properties.Resources.close;

        public static Bitmap GearButton = Properties.Resources.settingsbtn;


        public static Bitmap PlayButton = Properties.Resources.playbutton;

        public static Bitmap PlayButtonClick = Properties.Resources.playbutton_click;

        public static Bitmap PlayButtonHover = Properties.Resources.playbutton_hover;


        /* Borders for Login Inputs */

        public static Bitmap BorderTicket = Properties.Resources.ticket_text_border;

        public static Bitmap BorderTicketError = Properties.Resources.ticket_error_text_border;

        public static Bitmap BorderEmail = Properties.Resources.email_text_border;

        public static Bitmap BorderEmailError = Properties.Resources.email_error_text_border;

        public static Bitmap BorderPassword = Properties.Resources.password_text_border;

        public static Bitmap BorderPasswordError = Properties.Resources.password_error_text_border;


        /* ProgressBar & Outline */

        public static Bitmap ProgressBarOutline = Properties.Resources.progress_outline;

        public static Bitmap ProgressBarSuccess = Properties.Resources.progress_success;

        public static Bitmap ProgressBarPreload = Properties.Resources.progress_preload;

        public static Bitmap ProgressBarWarning = Properties.Resources.progress_warning;

        public static Bitmap ProgressBarError = Properties.Resources.progress_error;


        /* WinForm Buttons */

        public static Color BlueBackColorButton = Color.FromArgb(22, 29, 38);

        public static Color BlueBorderColorButton = Color.FromArgb(77, 181, 191);

        public static Color BlueForeColorButton = Color.FromArgb(192, 192, 192);

        public static Color BlueMouseOverBackColorButton = Color.FromArgb(44, 58, 76);


        public static Color YellowBackColorButton = Color.FromArgb(22, 29, 38);

        public static Color YellowBorderColorButton = Color.FromArgb(184, 134, 11);

        public static Color YellowForeColorButton = Color.FromArgb(44, 58, 76);

        public static Color YellowMouseOverBackColorButton = Color.FromArgb(44, 58, 76);


        /* Main Text Colors */

        public static Color MainTextForeColor = Color.FromArgb(224, 224, 224);

        public static Color SecondaryTextForeColor = Color.FromArgb(66, 179, 189);

        public static Color ThirdTextForeColor = Color.FromArgb(132, 132, 132);

        public static Color FourthTextForeColor = Color.FromArgb(44, 58, 76);


        /* Read Theme File and Check Values */
        public static void CheckIfThemeExists()
        {
            if (!string.IsNullOrEmpty(ThemeFile.Read("ThemeName")))
            {
                ThemeName = ThemeFile.Read("ThemeName");
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("ThemeAuthor")))
            {
                ThemeAuthor = ThemeFile.Read("ThemeAuthor");
            }

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

            if (!string.IsNullOrEmpty(ThemeFile.Read("APIUnkownIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIUnkownIcon")))
                {
                    APIIconUnkown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIUnkownIcon"));
                }
            }

            /* Social Panel Icons */

            if (!string.IsNullOrEmpty(ThemeFile.Read("DiscordIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("DiscordIcon")))
                {
                    DiscordIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("DiscordIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("DiscordIconDisabled")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("DiscordIconDisabled")))
                {
                    DiscordIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("DiscordIconDisabled"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("FacebookIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("FacebookIcon")))
                {
                    FacebookIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("FacebookIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("FacebookIconDisabled")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("FacebookIconDisabled")))
                {
                    FacebookIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("FacebookIconDisabled"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("HomeIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("HomeIcon")))
                {
                    HomeIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("HomeIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("HomeIconDisabled")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("HomeIconDisabled")))
                {
                    HomeIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("HomeIconDisabled"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("TwitterIcon")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("TwitterIcon")))
                {
                    TwitterIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("TwitterIcon"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("TwitterIconDisabled")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("TwitterIconDisabled")))
                {
                    TwitterIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("TwitterIconDisabled"));
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

            if (!string.IsNullOrEmpty(ThemeFile.Read("PlayButton")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButton")))
                {
                    PlayButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButton"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("PlayButtonClick")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButtonClick")))
                {
                    PlayButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButtonClick"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("PlayButtonHover")))
            {
                if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButtonHover")))
                {
                    PlayButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButtonHover"));
                }
            }

            /* Custom Inputs Borders */

            if (!string.IsNullOrEmpty(ThemeFile.Read("BorderTicket")))
            {
                if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderTicket")))
                {
                    BorderTicket = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderTicket"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("BorderTicketError")))
            {
                if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderTicketError")))
                {
                    BorderTicketError = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderTicketError"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("BorderEmail")))
            {
                if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderEmail")))
                {
                    BorderEmail = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderEmail"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("BorderEmailError")))
            {
                if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderEmailError")))
                {
                    BorderEmailError = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderEmailError"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("BorderPassword")))
            {
                if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderPassword")))
                {
                    BorderPassword = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderPassword"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("BorderPasswordError")))
            {
                if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderPasswordError")))
                {
                    BorderPasswordError = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderPasswordError"));
                }
            }

            /* ProgressBar and Outline */

            if (!string.IsNullOrEmpty(ThemeFile.Read("ProgressBarOutline")))
            {
                if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarOutline")))
                {
                    ProgressBarOutline = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarOutline"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("ProgressBarSuccess")))
            {
                if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarSuccess")))
                {
                    ProgressBarSuccess = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarSuccess"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("ProgressBarPreload")))
            {
                if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarPreload")))
                {
                    ProgressBarPreload = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarPreload"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("ProgressBarWarning")))
            {
                if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarWarning")))
                {
                    ProgressBarWarning = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarWarning"));
                }
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("ProgressBarError")))
            {
                if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarError")))
                {
                    ProgressBarError = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarError"));
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

            /* Text Colors */

            if (!string.IsNullOrEmpty(ThemeFile.Read("MainTextForeColor")))
            {
                MainTextForeColor = ToColor(ThemeFile.Read("MainTextForeColor"));
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("SecondaryTextForeColor")))
            {
                SecondaryTextForeColor = ToColor(ThemeFile.Read("SecondaryTextForeColor"));
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("ThirdTextForeColor")))
            {
                ThirdTextForeColor = ToColor(ThemeFile.Read("ThirdTextForeColor"));
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("FourthTextForeColor")))
            {
                FourthTextForeColor = ToColor(ThemeFile.Read("FourthTextForeColor"));
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
