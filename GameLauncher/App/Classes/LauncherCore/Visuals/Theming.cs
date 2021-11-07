using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Visuals
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
            if (File.Exists(Path.Combine(Locations.LauncherThemeFolder, "Theme.ini")) && (FileSettingsSave.ThemeSupport == "1"))
            {
                try
                {
                    IniFile ThemeFile = new IniFile(Path.Combine(Locations.LauncherThemeFolder, "Theme.ini"));

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ThemeName")))
                    {
                        ThemeName = ThemeFile.Read("ThemeName");
                    }
                    else
                    {
                        ThemeName = "Custom";
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ThemeAuthor")))
                    {
                        ThemeAuthor = ThemeFile.Read("ThemeAuthor");
                    }
                    else
                    {
                        ThemeAuthor = "Unknown - Check File";
                    }

                    /* Logo */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("Logo")))
                    {
                        if (File.Exists(ThemeFolder + "\\Logos\\" + ThemeFile.Read("Logo")))
                        {
                            LogoMain = new Bitmap(ThemeFolder + "\\Logos\\" + ThemeFile.Read("Logo"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("LogoSplashScreen")))
                    {
                        if (File.Exists(ThemeFolder + "\\Logos\\" + ThemeFile.Read("LogoSplashScreen")))
                        {
                            LogoSplash = new Bitmap(ThemeFolder + "\\Logos\\" + ThemeFile.Read("LogoSplashScreen"));
                        }
                    }

                    /* Main Backgrounds */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SettingsScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\SettingsScreen\\" + ThemeFile.Read("SettingsScreenBG")))
                        {
                            SettingsScreen = new Bitmap(ThemeFolder + "\\SettingsScreen\\" + ThemeFile.Read("SettingsScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("MainScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\MainScreen\\" + ThemeFile.Read("MainScreenBG")))
                        {
                            MainScreen = new Bitmap(ThemeFolder + "\\MainScreen\\" + ThemeFile.Read("MainScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SocialPanelBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\MainScreen\\" + ThemeFile.Read("SocialPanelBG")))
                        {
                            SocialPanel = new Bitmap(ThemeFolder + "\\MainScreen\\" + ThemeFile.Read("SocialPanelBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("RegisterScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\RegisterScreen\\" + ThemeFile.Read("RegisterScreenBG")))
                        {
                            RegisterScreen = new Bitmap(ThemeFolder + "\\RegisterScreen\\" + ThemeFile.Read("RegisterScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("USXEScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\USXEScreen\\" + ThemeFile.Read("USXEScreenBG")))
                        {
                            USXEEditor = new Bitmap(ThemeFolder + "\\USXEScreen\\" + ThemeFile.Read("USXEScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SecurityCenterScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\SecurityCenterScreen\\" + ThemeFile.Read("SecurityCenterScreenBG")))
                        {
                            SecurityCenterScreen = new Bitmap(ThemeFolder + "\\SecurityCenterScreen\\" + ThemeFile.Read("SecurityCenterScreenBG"));
                        }
                    }

                    /* MainScreen Icons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("UpdateErrorIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateErrorIcon")))
                        {
                            UpdateIconError = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateErrorIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("UpdateSuccessIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateSuccessIcon")))
                        {
                            UpdateIconSuccess = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateSuccessIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("UpdateUnkownIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateUnkownIcon")))
                        {
                            UpdateIconUnknown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateUnkownIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("UpdateWarningIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateWarningIcon")))
                        {
                            UpdateIconWarning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("UpdateWarningIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("APICheckingIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APICheckingIcon")))
                        {
                            APIIconChecking = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APICheckingIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("APIErrorIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIErrorIcon")))
                        {
                            APIIconError = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIErrorIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("APISuccessIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APISuccessIcon")))
                        {
                            APIIconSuccess = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APISuccessIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("APIUnkownIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIUnkownIcon")))
                        {
                            APIIconUnkown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIUnkownIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("APIWarningIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIWarningIcon")))
                        {
                            APIIconWarning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("APIWarningIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ServerCheckingIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ServerCheckingIcon")))
                        {
                            ServerIconChecking = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ServerCheckingIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ServerOfflineIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ServerOfflineIcon")))
                        {
                            ServerIconOffline = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ServerOfflineIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ServerSuccessIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ServerSuccessIcon")))
                        {
                            ServerIconSuccess = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ServerSuccessIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ServerWarningIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ServerWarningIcon")))
                        {
                            ServerIconWarning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ServerWarningIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ServerUnknownIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ServerUnknownIcon")))
                        {
                            ServerIconUnkown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ServerUnknownIcon"));
                        }
                    }

                    /* Social Panel Icons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DiscordIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("DiscordIcon")))
                        {
                            DiscordIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("DiscordIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DiscordIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("DiscordIconDisabled")))
                        {
                            DiscordIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("DiscordIconDisabled"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("FacebookIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("FacebookIcon")))
                        {
                            FacebookIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("FacebookIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("FacebookIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("FacebookIconDisabled")))
                        {
                            FacebookIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("FacebookIconDisabled"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("HomeIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("HomeIcon")))
                        {
                            HomeIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("HomeIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("HomeIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("HomeIconDisabled")))
                        {
                            HomeIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("HomeIconDisabled"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("TwitterIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("TwitterIcon")))
                        {
                            TwitterIcon = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("TwitterIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("TwitterIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("TwitterIconDisabled")))
                        {
                            TwitterIconDisabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("TwitterIconDisabled"));
                        }
                    }

                    /* Image Buttons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GrayButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButton")))
                        {
                            GrayButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GrayButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButtonClick")))
                        {
                            GrayButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GrayButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButtonHover")))
                        {
                            GrayButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GrayButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GreenButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButton")))
                        {
                            GreenButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GreenButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButtonClick")))
                        {
                            GreenButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GreenButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButtonHover")))
                        {
                            GreenButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GreenButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("CloseButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("CloseButton")))
                        {
                            CloseButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("CloseButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("CloseButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("CloseButtonClick")))
                        {
                            CloseButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("CloseButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("CloseButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("CloseButtonHover")))
                        {
                            CloseButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("CloseButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GearButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButton")))
                        {
                            GearButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GearButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButtonClick")))
                        {
                            GearButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GearButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButtonHover")))
                        {
                            GearButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GearButtonWarning")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButtonWarning")))
                        {
                            GearButtonWarning = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButtonWarning"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GearButtonWarningClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButtonWarningClick")))
                        {
                            GearButtonWarningClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButtonWarningClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GearButtonWarningHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButtonWarningHover")))
                        {
                            GearButtonWarningHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("GearButtonWarningHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("PlayButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButton")))
                        {
                            PlayButton = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("PlayButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButtonClick")))
                        {
                            PlayButtonClick = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("PlayButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButtonHover")))
                        {
                            PlayButtonHover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Read("PlayButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonUnknown")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonUnknown")))
                        {
                            ShieldButtonUnknown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonUnknown"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonUnknownClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonUnknownClick")))
                        {
                            ShieldButtonUnknownClick = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonUnknownClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonUnknownHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonUnknownHover")))
                        {
                            ShieldButtonUnknownHover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonUnknownHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonChecking")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonChecking")))
                        {
                            ShieldButtonChecking = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonChecking"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonCheckingClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonCheckingClick")))
                        {
                            ShieldButtonCheckingClick = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonCheckingClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonCheckingHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonCheckingHover")))
                        {
                            ShieldButtonCheckingHover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonCheckingHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonSuccess")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonSuccess")))
                        {
                            ShieldButtonSuccess = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonSuccess"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonSuccessClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonSuccessClick")))
                        {
                            ShieldButtonSuccessClick = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonSuccessClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonSuccessHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonSuccessHover")))
                        {
                            ShieldButtonSuccessHover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonSuccessHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonWarning")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonWarning")))
                        {
                            ShieldButtonWarning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonWarning"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonWarningClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonWarningClick")))
                        {
                            ShieldButtonWarningClick = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonWarningClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonWarningHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonWarningHover")))
                        {
                            ShieldButtonWarningHover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonWarningHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonError")))
                        {
                            ShieldButtonError = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonError"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonErrorClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonErrorClick")))
                        {
                            ShieldButtonErrorClick = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonErrorClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ShieldButtonErrorHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonErrorHover")))
                        {
                            ShieldButtonErrorHover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("ShieldButtonErrorHover"));
                        }
                    }

                    /* Custom Inputs Borders for MainScreen */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BorderTicket")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderTicket")))
                        {
                            BorderTicket = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderTicket"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BorderTicketError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderTicketError")))
                        {
                            BorderTicketError = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderTicketError"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BorderEmail")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderEmail")))
                        {
                            BorderEmail = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderEmail"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BorderEmailError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderEmailError")))
                        {
                            BorderEmailError = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderEmailError"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BorderPassword")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderPassword")))
                        {
                            BorderPassword = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderPassword"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BorderPasswordError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderPasswordError")))
                        {
                            BorderPasswordError = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Read("BorderPasswordError"));
                        }
                    }

                    /* ProgressBar and Outline */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ProgressBarOutline")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarOutline")))
                        {
                            ProgressBarOutline = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarOutline"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ProgressBarSuccess")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarSuccess")))
                        {
                            ProgressBarSuccess = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarSuccess"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ProgressBarPreload")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarPreload")))
                        {
                            ProgressBarPreload = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarPreload"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ProgressBarWarning")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarWarning")))
                        {
                            ProgressBarWarning = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarWarning"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ProgressBarError")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarError")))
                        {
                            ProgressBarError = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Read("ProgressBarError"));
                        }
                    }

                    /* WinForm Buttons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BlueBackColorButton")))
                    {
                        BlueBackColorButton = ToColor(ThemeFile.Read("BlueBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BlueBorderColorButton")))
                    {
                        BlueBorderColorButton = ToColor(ThemeFile.Read("BlueBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BlueForeColorButton")))
                    {
                        BlueForeColorButton = ToColor(ThemeFile.Read("BlueForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BlueMouseOverBackColorButton")))
                    {
                        BlueMouseOverBackColorButton = ToColor(ThemeFile.Read("BlueMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("YellowBackColorButton")))
                    {
                        YellowBackColorButton = ToColor(ThemeFile.Read("YellowBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("YellowBorderColorButton")))
                    {
                        YellowBorderColorButton = ToColor(ThemeFile.Read("YellowBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("YellowForeColorButton")))
                    {
                        YellowForeColorButton = ToColor(ThemeFile.Read("YellowForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("YellowMouseOverBackColorButton")))
                    {
                        YellowMouseOverBackColorButton = ToColor(ThemeFile.Read("YellowMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("RedBackColorButton")))
                    {
                        RedBackColorButton = ToColor(ThemeFile.Read("RedBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("RedBorderColorButton")))
                    {
                        RedBorderColorButton = ToColor(ThemeFile.Read("RedBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("RedForeColorButton")))
                    {
                        RedForeColorButton = ToColor(ThemeFile.Read("RedForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("RedMouseOverBackColorButton")))
                    {
                        RedMouseOverBackColorButton = ToColor(ThemeFile.Read("RedMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GreenBackColorButton")))
                    {
                        GreenBackColorButton = ToColor(ThemeFile.Read("GreenBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GreenBorderColorButton")))
                    {
                        GreenBorderColorButton = ToColor(ThemeFile.Read("GreenBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GreenForeColorButton")))
                    {
                        GreenForeColorButton = ToColor(ThemeFile.Read("GreenForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GreenMouseOverBackColorButton")))
                    {
                        GreenMouseOverBackColorButton = ToColor(ThemeFile.Read("GreenMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GrayBackColorButton")))
                    {
                        GrayBackColorButton = ToColor(ThemeFile.Read("GrayBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GrayBorderColorButton")))
                    {
                        GrayBorderColorButton = ToColor(ThemeFile.Read("GrayBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GrayForeColorButton")))
                    {
                        GrayForeColorButton = ToColor(ThemeFile.Read("GrayForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("GrayMouseOverBackColorButton")))
                    {
                        GrayMouseOverBackColorButton = ToColor(ThemeFile.Read("GrayMouseOverBackColorButton"));
                    }

                    /* Text Colors */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("MainTextForeColor")))
                    {
                        MainTextForeColor = ToColor(ThemeFile.Read("MainTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SecondaryTextForeColor")))
                    {
                        SecondaryTextForeColor = ToColor(ThemeFile.Read("SecondaryTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ThirdTextForeColor")))
                    {
                        ThirdTextForeColor = ToColor(ThemeFile.Read("ThirdTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("FourthTextForeColor")))
                    {
                        FourthTextForeColor = ToColor(ThemeFile.Read("FourthTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("FivithTextForeColor")))
                    {
                        FivithTextForeColor = ToColor(ThemeFile.Read("FivithTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SucessForeColor")))
                    {
                        Sucess = ToColor(ThemeFile.Read("SucessForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ErrorForeColor")))
                    {
                        Error = ToColor(ThemeFile.Read("ErrorForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("WarningForeColor")))
                    {
                        Warning = ToColor(ThemeFile.Read("WarningForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("BannerBackColor")))
                    {
                        BannerBackColor = ToColor(ThemeFile.Read("BannerBackColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("ExtractingProgressColor")))
                    {
                        ExtractingProgressColor = ToColor(ThemeFile.Read("ExtractingProgressColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("MainScreenTransparencyKey")))
                    {
                        MainScreenTransparencyKey = ToColor(ThemeFile.Read("MainScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SettingsScreenTransparencyKey")))
                    {
                        SettingsScreenTransparencyKey = ToColor(ThemeFile.Read("SettingsScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SplashScreenTransparencyKey")))
                    {
                        SplashScreenTransparencyKey = ToColor(ThemeFile.Read("SplashScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("RegisterScreenTransparencyKey")))
                    {
                        RegisterScreenTransparencyKey = ToColor(ThemeFile.Read("RegisterScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("USXEEScreenTransparencyKey")))
                    {
                        USXEETransparencyKey = ToColor(ThemeFile.Read("USXEEScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SecurityCenterScreenTransparencyKey")))
                    {
                        SecurityCenterScreenTransparencyKey = ToColor(ThemeFile.Read("SecurityCenterScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("MainScreenInputForeColor")))
                    {
                        Input = ToColor(ThemeFile.Read("MainScreenInputForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("MainScreenLinkForeColor")))
                    {
                        Link = ToColor(ThemeFile.Read("MainScreenLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("MainScreenActiveLinkForeColor")))
                    {
                        ActiveLink = ToColor(ThemeFile.Read("MainScreenActiveLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SettingsLinkForeColor")))
                    {
                        SettingsLink = ToColor(ThemeFile.Read("SettingsLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SettingsActiveLinkForeColor")))
                    {
                        SettingsActiveLink = ToColor(ThemeFile.Read("SettingsActiveLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SettingsCheckBoxesForeColor")))
                    {
                        SettingsCheckBoxes = ToColor(ThemeFile.Read("SettingsCheckBoxesForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("SeventhTextForeColorForeColor")))
                    {
                        SeventhTextForeColor = ToColor(ThemeFile.Read("SeventhTextForeColorForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("EighthTextForeColorForeColor")))
                    {
                        EighthTextForeColor = ToColor(ThemeFile.Read("EighthTextForeColorForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("WinFormTextForeColor")))
                    {
                        WinFormTextForeColor = ToColor(ThemeFile.Read("WinFormTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("WinFormSecondaryTextForeColor")))
                    {
                        WinFormSecondaryTextForeColor = ToColor(ThemeFile.Read("WinFormSecondaryTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("WinFormBGForeColor")))
                    {
                        WinFormTBGForeColor = ToColor(ThemeFile.Read("WinFormBGForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("WinFormBGDarkerForeColor")))
                    {
                        WinFormTBGDarkerForeColor = ToColor(ThemeFile.Read("WinFormBGDarkerForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("WinFormSuccessTextForeColor")))
                    {
                        WinFormSuccessTextForeColor = ToColor(ThemeFile.Read("WinFormSuccessTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("WinFormWarningTextForeColor")))
                    {
                        WinFormWarningTextForeColor = ToColor(ThemeFile.Read("WinFormWarningTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("WinFormErrorTextForeColor")))
                    {
                        WinFormErrorTextForeColor = ToColor(ThemeFile.Read("WinFormErrorTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("WinFormGridForeColor")))
                    {
                        WinFormGridForeColor = ToColor(ThemeFile.Read("WinFormGridForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("AboutBGForeColor")))
                    {
                        AboutBGForeColor = ToColor(ThemeFile.Read("AboutBGForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("AboutTextForeColor")))
                    {
                        AboutTextForeColor = ToColor(ThemeFile.Read("AboutTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DropMenuTextForeColor")))
                    {
                        DropMenuTextForeColor = ToColor(ThemeFile.Read("DropMenuTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DropMenuBackgroundForeColor")))
                    {
                        DropMenuBackgroundForeColor = ToColor(ThemeFile.Read("DropMenuBackgroundForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DropMenuTextForeColorCategory")))
                    {
                        DropMenuTextForeColor_Category = ToColor(ThemeFile.Read("DropMenuTextForeColorCategory"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DropMenuBackgroundForeColorCategory")))
                    {
                        DropMenuBackgroundForeColor_Category = ToColor(ThemeFile.Read("DropMenuBackgroundForeColorCategory"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DropMenuPingSuccess")))
                    {
                        DropMenuPingSuccess = ToColor(ThemeFile.Read("DropMenuPingSuccess"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DropMenuPingChecking")))
                    {
                        DropMenuPingChecking = ToColor(ThemeFile.Read("DropMenuPingChecking"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DropMenuPingError")))
                    {
                        DropMenuPingError = ToColor(ThemeFile.Read("DropMenuPingError"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DropMenuPingWarning")))
                    {
                        DropMenuPingWarning = ToColor(ThemeFile.Read("DropMenuPingWarning"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DropMenuBlack")))
                    {
                        DropMenuBlack = ToColor(ThemeFile.Read("DropMenuBlack"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("DropMenuWhite")))
                    {
                        DropMenuWhite = ToColor(ThemeFile.Read("DropMenuWhite"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Read("PrivacyRPCBuild")))
                    {
                        PrivacyRPCBuild = ThemeFile.Read("PrivacyRPCBuild");
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