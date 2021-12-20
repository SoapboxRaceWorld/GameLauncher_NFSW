using SBRW.Launcher.App.Classes.LauncherCore.FileReadWrite;
using SBRW.Launcher.App.Classes.LauncherCore.Global;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Extra.Ini_;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SBRW.Launcher.App.Classes.LauncherCore.Visuals
{
    class Theming
    {
        private static readonly string ThemeFolder = AppDomain.CurrentDomain.BaseDirectory + "Theme";


        /* Discord RPC Privacy Build Number */

        public static string PrivacyRPCBuild = Application.ProductVersion;


        /* Theme Name & Author */

        public static string ThemeName = "Default";

        public static string ThemeAuthor = "Launcher - Division";


        /* Logo */

        public static Bitmap LogoMain = Properties.Resources.logo_main;

        public static Bitmap LogoSplash = Properties.Resources.logo_splash;


        /* Main Backgrounds */

        public static Bitmap MainScreen = Properties.Resources.Background_Login;

        public static Color MainScreenTransparencyKey = Color.FromArgb(255, 0, 255);

        public static Bitmap SettingsScreen = Properties.Resources.Background_Settings;

        public static Color SettingsScreenTransparencyKey = Color.FromArgb(255, 0, 255);

        public static Color SplashScreenTransparencyKey = Color.FromArgb(0, 0, 25);

        public static Bitmap SocialPanel = Properties.Resources.Background_Social;

        public static Bitmap RegisterScreen = Properties.Resources.Background_Register;

        public static Color RegisterScreenTransparencyKey = Color.FromArgb(255, 0, 255);

        public static Bitmap USXEEditor = Properties.Resources.Background_USXE;

        public static Color USXEETransparencyKey = Color.FromArgb(255, 0, 255);

        public static Bitmap SecurityCenterScreen = Properties.Resources.Background_Settings;

        public static Color SecurityCenterScreenTransparencyKey = Color.FromArgb(255, 0, 255);

        /* MainScreen Icons */

        public static Bitmap UpdateIconError = Properties.Resources.icon_engine_error;

        public static Bitmap UpdateIconSuccess = Properties.Resources.icon_engine_success;

        public static Bitmap UpdateIconUnknown = Properties.Resources.icon_engine_unknown;

        public static Bitmap UpdateIconWarning = Properties.Resources.icon_engine_warning;


        public static Bitmap APIIconChecking = Properties.Resources.icon_api_checking;

        public static Bitmap APIIconError = Properties.Resources.icon_api_offline;

        public static Bitmap APIIconSuccess = Properties.Resources.icon_api_online;

        public static Bitmap APIIconWarning = Properties.Resources.icon_api_warning;

        public static Bitmap APIIconUnkown = Properties.Resources.icon_api_unknown;


        public static Bitmap ServerIconChecking = Properties.Resources.icon_game_server_checking;

        public static Bitmap ServerIconOffline = Properties.Resources.icon_game_server_offline;

        public static Bitmap ServerIconSuccess = Properties.Resources.icon_game_server_online;

        public static Bitmap ServerIconWarning = Properties.Resources.icon_game_server_warning;

        public static Bitmap ServerIconUnkown = Properties.Resources.icon_game_server_unknown;


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


        public static Bitmap GreenButton = Properties.Resources.greenbutton;

        public static Bitmap GreenButtonClick = Properties.Resources.greenbutton_click;

        public static Bitmap GreenButtonHover = Properties.Resources.greenbutton_hover;


        public static Bitmap CloseButton = Properties.Resources.icon_close;

        public static Bitmap CloseButtonClick = Properties.Resources.icon_close_click;

        public static Bitmap CloseButtonHover = Properties.Resources.icon_close_hover;


        public static Bitmap GearButton = Properties.Resources.icon_gear;

        public static Bitmap GearButtonClick = Properties.Resources.icon_gear_click;

        public static Bitmap GearButtonHover = Properties.Resources.icon_gear_hover;


        /// <summary>RGB: 230, 159, 0</summary>
        /// <remarks>HEX: E69F00</remarks>
        public static Bitmap GearButtonWarning = Properties.Resources.icon_gear_warning;

        /// <summary>RGB: 230, 129, 0</summary>
        /// <remarks>HEX: E68100</remarks>
        public static Bitmap GearButtonWarningClick = Properties.Resources.icon_gear_warning_click;

        /// <summary>RGB: 230, 99, 0</summary>
        /// <remarks>HEX: E66300</remarks>
        public static Bitmap GearButtonWarningHover = Properties.Resources.icon_gear_warning_hover;


        /// <summary>RGB: 93, 93, 93</summary>
        /// <remarks>HEX: 5D5D5D</remarks>
        public static Bitmap ShieldButtonUnknown = Properties.Resources.icon_shield_unknown;

        /// <summary>RGB: 66, 66, 66</summary>
        /// <remarks>HEX: 424242</remarks>
        public static Bitmap ShieldButtonUnknownClick = Properties.Resources.icon_shield_unknown_click;

        /// <summary>RGB: 132, 132, 132</summary>
        /// <remarks>HEX: 848484</remarks>
        public static Bitmap ShieldButtonUnknownHover = Properties.Resources.icon_shield_unknown_hover;

        /// <summary>RGB: 66, 179, 189</summary>
        /// <remarks>HEX: 42B3BD</remarks>
        public static Bitmap ShieldButtonChecking = Properties.Resources.icon_shield_checking;

        /// <summary>RGB: 66, 159, 189</summary>
        /// <remarks>HEX: 429FBD</remarks>
        public static Bitmap ShieldButtonCheckingClick = Properties.Resources.icon_shield_checking_click;

        /// <summary>RGB: 66, 129, 189</summary>
        /// <remarks>HEX: 4281BD</remarks>
        public static Bitmap ShieldButtonCheckingHover = Properties.Resources.icon_shield_checking_hover;

        /// <summary>RGB: 159, 193, 32</summary>
        /// <remarks>HEX: 9FC120</remarks>
        public static Bitmap ShieldButtonSuccess = Properties.Resources.icon_shield_success;

        /// <summary>RGB: 127, 156, 29</summary>
        /// <remarks>HEX: 7F9C1D</remarks>
        public static Bitmap ShieldButtonSuccessClick = Properties.Resources.icon_shield_success_click;

        /// <summary>RGB: 104, 255, 0</summary>
        /// <remarks>HEX: 68FF08</remarks>
        public static Bitmap ShieldButtonSuccessHover = Properties.Resources.icon_shield_success_hover;

        /// <summary>RGB: 230, 159, 0</summary>
        /// <remarks>HEX: E69F00</remarks>
        public static Bitmap ShieldButtonWarning = Properties.Resources.icon_shield_warning;

        /// <summary>RGB: 230, 129, 0</summary>
        /// <remarks>HEX: E68100</remarks>
        public static Bitmap ShieldButtonWarningClick = Properties.Resources.icon_shield_warning_click;

        /// <summary>RGB: 230, 99, 0</summary>
        /// <remarks>HEX: E66300</remarks>
        public static Bitmap ShieldButtonWarningHover = Properties.Resources.icon_shield_warning_hover;

        /// <summary>RGB: 254, 0, 0</summary>
        /// <remarks>HEX: FE0000</remarks>
        public static Bitmap ShieldButtonError = Properties.Resources.icon_shield_error;

        /// <summary>RGB: 224, 0, 0</summary>
        /// <remarks>HEX: E00000</remarks>
        public static Bitmap ShieldButtonErrorClick = Properties.Resources.icon_shield_error_click;

        /// <summary>RGB: 194, 0, 0</summary>
        /// <remarks>HEX: C20000</remarks>
        public static Bitmap ShieldButtonErrorHover = Properties.Resources.icon_shield_error_hover;


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

        public static Color Input = Color.FromArgb(22, 32, 42);

        public static Color Link = Color.FromArgb(224, 200, 0);

        public static Color ActiveLink = Color.FromArgb(255, 228, 0);


        /* ProgressBar & Outline */

        public static Bitmap ProgressBarOutline = Properties.Resources.progress_outline;

        public static Bitmap ProgressBarSuccess = Properties.Resources.progress_success;

        public static Bitmap ProgressBarPreload = Properties.Resources.progress_preload;

        public static Bitmap ProgressBarWarning = Properties.Resources.progress_warning;

        public static Bitmap ProgressBarError = Properties.Resources.progress_error;

        public static Color ExtractingProgressColor = Color.FromArgb(255, 165, 0);


        /* WinForm Buttons */

        public static Color BlueBackColorButton = Color.FromArgb(22, 29, 38);

        public static Color BlueBorderColorButton = Color.FromArgb(77, 181, 191);

        public static Color BlueForeColorButton = Color.FromArgb(192, 192, 192);

        public static Color BlueMouseOverBackColorButton = Color.FromArgb(44, 58, 76);


        public static Color YellowBackColorButton = Color.FromArgb(22, 29, 38);

        public static Color YellowBorderColorButton = Color.FromArgb(184, 134, 11);

        public static Color YellowForeColorButton = Color.FromArgb(224, 200, 0);

        public static Color YellowMouseOverBackColorButton = Color.FromArgb(44, 58, 76);


        public static Color RedBackColorButton = Color.FromArgb(22, 29, 38);

        public static Color RedBorderColorButton = Color.FromArgb(168, 0, 0);

        public static Color RedForeColorButton = Color.FromArgb(210, 4, 45);

        public static Color RedMouseOverBackColorButton = Color.FromArgb(44, 58, 76);


        public static Color GreenBackColorButton = Color.FromArgb(22, 29, 38);

        public static Color GreenBorderColorButton = Color.FromArgb(159, 193, 32);

        public static Color GreenForeColorButton = Color.FromArgb(119, 145, 22);

        public static Color GreenMouseOverBackColorButton = Color.FromArgb(44, 58, 76);


        public static Color GrayBackColorButton = Color.FromArgb(22, 29, 38);

        public static Color GrayBorderColorButton = Color.FromArgb(128, 128, 128);

        public static Color GrayForeColorButton = Color.FromArgb(100, 100, 100);

        public static Color GrayMouseOverBackColorButton = Color.FromArgb(44, 58, 76);


        /* Main Text Colors */

        ///<summary>Silver</summary>
        public static Color MainTextForeColor = Color.FromArgb(224, 224, 224);

        ///<summary>Pinging Blue</summary>
        public static Color SecondaryTextForeColor = Color.FromArgb(66, 179, 189);

        ///<summary>Gray</summary>
        public static Color ThirdTextForeColor = Color.FromArgb(132, 132, 132);

        ///<summary>Dark Blue</summary>
        public static Color FourthTextForeColor = Color.FromArgb(44, 58, 76);

        ///<summary>White</summary>
        public static Color FivithTextForeColor = Color.FromArgb(255, 255, 255);

        ///<summary>Grey</summary>
        ///<remarks>Slightly Darker</remarks>
        public static Color SixTextForeColor = Color.FromArgb(128, 128, 128);

        ///<summary>Successful Green</summary>
        public static Color SeventhTextForeColor = Color.FromArgb(159, 193, 32);

        ///<summary>Orange</summary>
        ///<remarks>Color Bind</remarks>
        public static Color EighthTextForeColor = Color.FromArgb(230, 159, 0);


        /* WinForm (Screens) Text and Background Colors */

        public static Color WinFormTextForeColor = Color.FromArgb(224, 224, 224);

        public static Color WinFormSecondaryTextForeColor = Color.FromArgb(178, 210, 255);

        public static Color WinFormTBGForeColor = Color.FromArgb(29, 36, 45);

        public static Color WinFormTBGDarkerForeColor = Color.FromArgb(22, 29, 38);

        public static Color WinFormSuccessTextForeColor = Color.FromArgb(0, 192, 0);

        public static Color WinFormWarningTextForeColor = Color.FromArgb(224, 200, 0);

        public static Color WinFormErrorTextForeColor = Color.FromArgb(224, 0, 0);

        public static Color WinFormGridForeColor = Color.FromArgb(77, 181, 191);


        /* WinForm (About) Unique Text and Background Colors */

        public static Color AboutBGForeColor = Color.FromArgb(15, 21, 28);

        public static Color AboutTextForeColor = Color.FromArgb(178, 210, 255);


        /* Drop Down Menu */

        public static Color DropMenuTextForeColor = Color.FromArgb(178, 210, 255);

        public static Color DropMenuBackgroundForeColor = Color.FromArgb(44, 58, 76);

        public static Color DropMenuTextForeColor_Category = Color.FromArgb(150, 194, 255);

        public static Color DropMenuBackgroundForeColor_Category = Color.FromArgb(31, 41, 54);


        /* Drop Down Menu - Colors */

        /// <summary>Color: PaleGreen</summary>
        /// <remarks>
        /// RGB: 152, 251, 152 <code></code>
        /// HEX: 98FB98
        /// </remarks>
        public static Color DropMenuPingSuccess = Color.FromArgb(152, 251, 152);

        /// <summary>Color: Khaki</summary>
        /// <remarks>
        /// RGB: 240, 230, 140 <code></code>
        /// HEX: F0E68C
        /// </remarks>
        public static Color DropMenuPingChecking = Color.FromArgb(240, 230, 140);

        /// <summary>Color: Khaki</summary>
        /// <remarks>
        /// RGB: 240, 128, 128 <code></code>
        /// HEX: F08080
        /// </remarks>
        public static Color DropMenuPingError = Color.FromArgb(240, 128, 128);

        /// <summary>
        /// RGB: 230, 159, 0 <code></code>
        /// HEX: E69F00</summary>
        /// <remarks>Used as an Alert Color, but is currently used for Invalid GSI JSON</remarks>
        public static Color DropMenuPingWarning = Color.FromArgb(230, 159, 0);

        /// <summary>Color: Black</summary>
        /// <remarks>Default Color Text</remarks>
        public static Color DropMenuBlack = Color.FromArgb(0, 0, 0);

        /// <summary>Color: White</summary>
        /// <remarks>Default Color Background</remarks>
        public static Color DropMenuWhite = Color.FromArgb(255, 255, 255);


        /* Status Colors */

        /* Successful Green*/
        public static Color Sucess = Color.FromArgb(159, 193, 32);

        /* Red */
        public static Color Error = Color.FromArgb(254, 0, 0);

        /* Orange [Color Bind] */
        public static Color Warning = Color.FromArgb(230, 159, 0);


        /* Server Banner */

        public static Color BannerBackColor = Color.FromArgb(0, 0, 0);


        /* Settings Label Links */

        /* LawnGreen */
        public static Color SettingsLink = Color.FromArgb(124, 252, 0);

        /* Silver */
        public static Color SettingsActiveLink = Color.FromArgb(224, 224, 224);

        /* Dark Goldenrod */
        public static Color SettingsCheckBoxes = Color.FromArgb(184, 134, 11);


        /* Read Theme File and Check Values */
        public static void CheckIfThemeExists()
        {
            if (File.Exists(Path.Combine(Locations.LauncherThemeFolder, "Theme.ini")) &&
                (Save_Settings.Live_Data.Launcher_Theme_Support == "1"))
            {
                try
                {
                    Ini_File ThemeFile = new Ini_File(Path.Combine(Locations.LauncherThemeFolder, "Theme.ini"));

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ThemeName")))
                    {
                        ThemeName = ThemeFile.Key_Read("ThemeName");
                    }
                    else
                    {
                       ThemeName = "Custom";
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ThemeAuthor")))
                    {
                        ThemeAuthor = ThemeFile.Key_Read("ThemeAuthor");
                    }
                    else
                    {
                        ThemeAuthor = "Unknown - Check File";
                    }

                    /* Logo */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("Logo")))
                    {
                        if (File.Exists(ThemeFolder + "\\Logos\\" + ThemeFile.Key_Read("Logo")))
                        {
                            LogoMain = new Bitmap(ThemeFolder + "\\Logos\\" + ThemeFile.Key_Read("Logo"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("LogoSplashScreen")))
                    {
                        if (File.Exists(ThemeFolder + "\\Logos\\" + ThemeFile.Key_Read("LogoSplashScreen")))
                        {
                            LogoSplash = new Bitmap(ThemeFolder + "\\Logos\\" + ThemeFile.Key_Read("LogoSplashScreen"));
                        }
                    }

                    /* Main Backgrounds */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SettingsScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\SettingsScreen\\" + ThemeFile.Key_Read("SettingsScreenBG")))
                        {
                            SettingsScreen = new Bitmap(ThemeFolder + "\\SettingsScreen\\" + ThemeFile.Key_Read("SettingsScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\MainScreen\\" + ThemeFile.Key_Read("MainScreenBG")))
                        {
                            MainScreen = new Bitmap(ThemeFolder + "\\MainScreen\\" + ThemeFile.Key_Read("MainScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SocialPanelBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\MainScreen\\" + ThemeFile.Key_Read("SocialPanelBG")))
                        {
                            SocialPanel = new Bitmap(ThemeFolder + "\\MainScreen\\" + ThemeFile.Key_Read("SocialPanelBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RegisterScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\RegisterScreen\\" + ThemeFile.Key_Read("RegisterScreenBG")))
                        {
                            RegisterScreen = new Bitmap(ThemeFolder + "\\RegisterScreen\\" + ThemeFile.Key_Read("RegisterScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("USXEScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\USXEScreen\\" + ThemeFile.Key_Read("USXEScreenBG")))
                        {
                            USXEEditor = new Bitmap(ThemeFolder + "\\USXEScreen\\" + ThemeFile.Key_Read("USXEScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SecurityCenterScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\SecurityCenterScreen\\" + ThemeFile.Key_Read("SecurityCenterScreenBG")))
                        {
                            SecurityCenterScreen = new Bitmap(ThemeFolder + "\\SecurityCenterScreen\\" + ThemeFile.Key_Read("SecurityCenterScreenBG"));
                        }
                    }

                    /* MainScreen Icons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("UpdateErrorIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateErrorIcon")))
                        {
                            UpdateIconError = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateErrorIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("UpdateSuccessIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateSuccessIcon")))
                        {
                            UpdateIconSuccess = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateSuccessIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("UpdateUnkownIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateUnkownIcon")))
                        {
                            UpdateIconUnknown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateUnkownIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("UpdateWarningIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateWarningIcon")))
                        {
                            UpdateIconWarning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateWarningIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("APICheckingIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APICheckingIcon")))
                        {
                            APIIconChecking = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APICheckingIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("APIErrorIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIErrorIcon")))
                        {
                            APIIconError = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIErrorIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("APISuccessIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APISuccessIcon")))
                        {
                            APIIconSuccess = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APISuccessIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("APIUnkownIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIUnkownIcon")))
                        {
                            APIIconUnkown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIUnkownIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("APIWarningIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIWarningIcon")))
                        {
                            APIIconWarning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIWarningIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ServerCheckingIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerCheckingIcon")))
                        {
                            ServerIconChecking = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerCheckingIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ServerOfflineIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerOfflineIcon")))
                        {
                            ServerIconOffline = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerOfflineIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ServerSuccessIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerSuccessIcon")))
                        {
                            ServerIconSuccess = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerSuccessIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ServerWarningIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerWarningIcon")))
                        {
                            ServerIconWarning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerWarningIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ServerUnknownIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerUnknownIcon")))
                        {
                            ServerIconUnkown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerUnknownIcon"));
                        }
                    }

                    /* Social Panel Icons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DiscordIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("DiscordIcon")))
                        {
                            DiscordIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("DiscordIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DiscordIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("DiscordIconDisabled")))
                        {
                            DiscordIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("DiscordIconDisabled"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("FacebookIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("FacebookIcon")))
                        {
                            FacebookIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("FacebookIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("FacebookIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("FacebookIconDisabled")))
                        {
                            FacebookIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("FacebookIconDisabled"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("HomeIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("HomeIcon")))
                        {
                            HomeIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("HomeIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("HomeIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("HomeIconDisabled")))
                        {
                            HomeIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("HomeIconDisabled"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("TwitterIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("TwitterIcon")))
                        {
                            TwitterIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("TwitterIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("TwitterIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("TwitterIconDisabled")))
                        {
                            TwitterIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("TwitterIconDisabled"));
                        }
                    }

                    /* Image Buttons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButton")))
                        {
                            GrayButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButtonClick")))
                        {
                            GrayButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButtonHover")))
                        {
                            GrayButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButton")))
                        {
                            GreenButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButtonClick")))
                        {
                            GreenButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButtonHover")))
                        {
                            GreenButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("CloseButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButton")))
                        {
                            CloseButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("CloseButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButtonClick")))
                        {
                            CloseButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("CloseButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButtonHover")))
                        {
                            CloseButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButton")))
                        {
                            GearButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonClick")))
                        {
                            GearButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonHover")))
                        {
                            GearButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButtonWarning")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarning")))
                        {
                            GearButtonWarning = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarning"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButtonWarningClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarningClick")))
                        {
                            GearButtonWarningClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarningClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButtonWarningHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarningHover")))
                        {
                            GearButtonWarningHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarningHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("PlayButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButton")))
                        {
                            PlayButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("PlayButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButtonClick")))
                        {
                            PlayButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("PlayButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButtonHover")))
                        {
                            PlayButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonUnknown")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknown")))
                        {
                            ShieldButtonUnknown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknown"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonUnknownClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknownClick")))
                        {
                            ShieldButtonUnknownClick = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknownClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonUnknownHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknownHover")))
                        {
                            ShieldButtonUnknownHover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknownHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonChecking")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonChecking")))
                        {
                            ShieldButtonChecking = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonChecking"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonCheckingClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonCheckingClick")))
                        {
                            ShieldButtonCheckingClick = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonCheckingClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonCheckingHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonCheckingHover")))
                        {
                            ShieldButtonCheckingHover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonCheckingHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonSuccess")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccess")))
                        {
                            ShieldButtonSuccess = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccess"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonSuccessClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccessClick")))
                        {
                            ShieldButtonSuccessClick = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccessClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonSuccessHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccessHover")))
                        {
                            ShieldButtonSuccessHover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccessHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonWarning")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarning")))
                        {
                            ShieldButtonWarning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarning"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonWarningClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarningClick")))
                        {
                            ShieldButtonWarningClick = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarningClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonWarningHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarningHover")))
                        {
                            ShieldButtonWarningHover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarningHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonError")))
                        {
                            ShieldButtonError = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonError"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonErrorClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonErrorClick")))
                        {
                            ShieldButtonErrorClick = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonErrorClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonErrorHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonErrorHover")))
                        {
                            ShieldButtonErrorHover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonErrorHover"));
                        }
                    }

                    /* Custom Inputs Borders for MainScreen */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderTicket")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderTicket")))
                        {
                            BorderTicket = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderTicket"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderTicketError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderTicketError")))
                        {
                            BorderTicketError = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderTicketError"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderEmail")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderEmail")))
                        {
                            BorderEmail = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderEmail"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderEmailError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderEmailError")))
                        {
                            BorderEmailError = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderEmailError"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderPassword")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderPassword")))
                        {
                            BorderPassword = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderPassword"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderPasswordError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderPasswordError")))
                        {
                            BorderPasswordError = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderPasswordError"));
                        }
                    }

                    /* ProgressBar and Outline */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressBarOutline")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarOutline")))
                        {
                            ProgressBarOutline = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarOutline"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressBarSuccess")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarSuccess")))
                        {
                            ProgressBarSuccess = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarSuccess"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressBarPreload")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarPreload")))
                        {
                            ProgressBarPreload = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarPreload"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressBarWarning")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarWarning")))
                        {
                            ProgressBarWarning = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarWarning"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressBarError")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarError")))
                        {
                            ProgressBarError = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarError"));
                        }
                    }

                    /* WinForm Buttons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BlueBackColorButton")))
                    {
                        BlueBackColorButton = ToColor(ThemeFile.Key_Read("BlueBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BlueBorderColorButton")))
                    {
                        BlueBorderColorButton = ToColor(ThemeFile.Key_Read("BlueBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BlueForeColorButton")))
                    {
                        BlueForeColorButton = ToColor(ThemeFile.Key_Read("BlueForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BlueMouseOverBackColorButton")))
                    {
                        BlueMouseOverBackColorButton = ToColor(ThemeFile.Key_Read("BlueMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("YellowBackColorButton")))
                    {
                        YellowBackColorButton = ToColor(ThemeFile.Key_Read("YellowBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("YellowBorderColorButton")))
                    {
                        YellowBorderColorButton = ToColor(ThemeFile.Key_Read("YellowBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("YellowForeColorButton")))
                    {
                        YellowForeColorButton = ToColor(ThemeFile.Key_Read("YellowForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("YellowMouseOverBackColorButton")))
                    {
                        YellowMouseOverBackColorButton = ToColor(ThemeFile.Key_Read("YellowMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RedBackColorButton")))
                    {
                        RedBackColorButton = ToColor(ThemeFile.Key_Read("RedBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RedBorderColorButton")))
                    {
                        RedBorderColorButton = ToColor(ThemeFile.Key_Read("RedBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RedForeColorButton")))
                    {
                        RedForeColorButton = ToColor(ThemeFile.Key_Read("RedForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RedMouseOverBackColorButton")))
                    {
                        RedMouseOverBackColorButton = ToColor(ThemeFile.Key_Read("RedMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenBackColorButton")))
                    {
                        GreenBackColorButton = ToColor(ThemeFile.Key_Read("GreenBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenBorderColorButton")))
                    {
                        GreenBorderColorButton = ToColor(ThemeFile.Key_Read("GreenBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenForeColorButton")))
                    {
                        GreenForeColorButton = ToColor(ThemeFile.Key_Read("GreenForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenMouseOverBackColorButton")))
                    {
                        GreenMouseOverBackColorButton = ToColor(ThemeFile.Key_Read("GreenMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayBackColorButton")))
                    {
                        GrayBackColorButton = ToColor(ThemeFile.Key_Read("GrayBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayBorderColorButton")))
                    {
                        GrayBorderColorButton = ToColor(ThemeFile.Key_Read("GrayBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayForeColorButton")))
                    {
                        GrayForeColorButton = ToColor(ThemeFile.Key_Read("GrayForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayMouseOverBackColorButton")))
                    {
                        GrayMouseOverBackColorButton = ToColor(ThemeFile.Key_Read("GrayMouseOverBackColorButton"));
                    }

                    /* Text Colors */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainTextForeColor")))
                    {
                        MainTextForeColor = ToColor(ThemeFile.Key_Read("MainTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SecondaryTextForeColor")))
                    {
                        SecondaryTextForeColor = ToColor(ThemeFile.Key_Read("SecondaryTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ThirdTextForeColor")))
                    {
                        ThirdTextForeColor = ToColor(ThemeFile.Key_Read("ThirdTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("FourthTextForeColor")))
                    {
                        FourthTextForeColor = ToColor(ThemeFile.Key_Read("FourthTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("FivithTextForeColor")))
                    {
                        FivithTextForeColor = ToColor(ThemeFile.Key_Read("FivithTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SucessForeColor")))
                    {
                        Sucess = ToColor(ThemeFile.Key_Read("SucessForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ErrorForeColor")))
                    {
                        Error = ToColor(ThemeFile.Key_Read("ErrorForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WarningForeColor")))
                    {
                        Warning = ToColor(ThemeFile.Key_Read("WarningForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BannerBackColor")))
                    {
                        BannerBackColor = ToColor(ThemeFile.Key_Read("BannerBackColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ExtractingProgressColor")))
                    {
                        ExtractingProgressColor = ToColor(ThemeFile.Key_Read("ExtractingProgressColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainScreenTransparencyKey")))
                    {
                        MainScreenTransparencyKey = ToColor(ThemeFile.Key_Read("MainScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SettingsScreenTransparencyKey")))
                    {
                        SettingsScreenTransparencyKey = ToColor(ThemeFile.Key_Read("SettingsScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SplashScreenTransparencyKey")))
                    {
                        SplashScreenTransparencyKey = ToColor(ThemeFile.Key_Read("SplashScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RegisterScreenTransparencyKey")))
                    {
                        RegisterScreenTransparencyKey = ToColor(ThemeFile.Key_Read("RegisterScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("USXEEScreenTransparencyKey")))
                    {
                        USXEETransparencyKey = ToColor(ThemeFile.Key_Read("USXEEScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SecurityCenterScreenTransparencyKey")))
                    {
                        SecurityCenterScreenTransparencyKey = ToColor(ThemeFile.Key_Read("SecurityCenterScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainScreenInputForeColor")))
                    {
                        Input = ToColor(ThemeFile.Key_Read("MainScreenInputForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainScreenLinkForeColor")))
                    {
                        Link = ToColor(ThemeFile.Key_Read("MainScreenLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainScreenActiveLinkForeColor")))
                    {
                        ActiveLink = ToColor(ThemeFile.Key_Read("MainScreenActiveLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SettingsLinkForeColor")))
                    {
                        SettingsLink = ToColor(ThemeFile.Key_Read("SettingsLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SettingsActiveLinkForeColor")))
                    {
                        SettingsActiveLink = ToColor(ThemeFile.Key_Read("SettingsActiveLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SettingsCheckBoxesForeColor")))
                    {
                        SettingsCheckBoxes = ToColor(ThemeFile.Key_Read("SettingsCheckBoxesForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SeventhTextForeColorForeColor")))
                    {
                        SeventhTextForeColor = ToColor(ThemeFile.Key_Read("SeventhTextForeColorForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("EighthTextForeColorForeColor")))
                    {
                        EighthTextForeColor = ToColor(ThemeFile.Key_Read("EighthTextForeColorForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormTextForeColor")))
                    {
                        WinFormTextForeColor = ToColor(ThemeFile.Key_Read("WinFormTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormSecondaryTextForeColor")))
                    {
                        WinFormSecondaryTextForeColor = ToColor(ThemeFile.Key_Read("WinFormSecondaryTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormBGForeColor")))
                    {
                        WinFormTBGForeColor = ToColor(ThemeFile.Key_Read("WinFormBGForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormBGDarkerForeColor")))
                    {
                        WinFormTBGDarkerForeColor = ToColor(ThemeFile.Key_Read("WinFormBGDarkerForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormSuccessTextForeColor")))
                    {
                        WinFormSuccessTextForeColor = ToColor(ThemeFile.Key_Read("WinFormSuccessTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormWarningTextForeColor")))
                    {
                        WinFormWarningTextForeColor = ToColor(ThemeFile.Key_Read("WinFormWarningTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormErrorTextForeColor")))
                    {
                        WinFormErrorTextForeColor = ToColor(ThemeFile.Key_Read("WinFormErrorTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormGridForeColor")))
                    {
                        WinFormGridForeColor = ToColor(ThemeFile.Key_Read("WinFormGridForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("AboutBGForeColor")))
                    {
                        AboutBGForeColor = ToColor(ThemeFile.Key_Read("AboutBGForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("AboutTextForeColor")))
                    {
                        AboutTextForeColor = ToColor(ThemeFile.Key_Read("AboutTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuTextForeColor")))
                    {
                        DropMenuTextForeColor = ToColor(ThemeFile.Key_Read("DropMenuTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuBackgroundForeColor")))
                    {
                        DropMenuBackgroundForeColor = ToColor(ThemeFile.Key_Read("DropMenuBackgroundForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuTextForeColorCategory")))
                    {
                        DropMenuTextForeColor_Category = ToColor(ThemeFile.Key_Read("DropMenuTextForeColorCategory"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuBackgroundForeColorCategory")))
                    {
                        DropMenuBackgroundForeColor_Category = ToColor(ThemeFile.Key_Read("DropMenuBackgroundForeColorCategory"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuPingSuccess")))
                    {
                        DropMenuPingSuccess = ToColor(ThemeFile.Key_Read("DropMenuPingSuccess"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuPingChecking")))
                    {
                        DropMenuPingChecking = ToColor(ThemeFile.Key_Read("DropMenuPingChecking"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuPingError")))
                    {
                        DropMenuPingError = ToColor(ThemeFile.Key_Read("DropMenuPingError"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuPingWarning")))
                    {
                        DropMenuPingWarning = ToColor(ThemeFile.Key_Read("DropMenuPingWarning"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuBlack")))
                    {
                        DropMenuBlack = ToColor(ThemeFile.Key_Read("DropMenuBlack"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuWhite")))
                    {
                        DropMenuWhite = ToColor(ThemeFile.Key_Read("DropMenuWhite"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("PrivacyRPCBuild")))
                    {
                        PrivacyRPCBuild = ThemeFile.Key_Read("PrivacyRPCBuild");
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("THEMING", null, Error, null, true);
                }
            }
        }

        /* Convert User Inputed String into a Valid RBG Spectrum Values */

        public static Color ToColor(string color)
        {
            try
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
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("THEMING", null, Error, null, true);
                return Color.Silver;
            }
        }

    }
}