using SBRW.Launcher.RunTime.LauncherCore.FileReadWrite;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Extra.Ini_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SBRW.Launcher.RunTime.LauncherCore.Visuals
{
    class Theming
    {
        private static string ThemeFolder { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "Theme";


        /* Discord RPC Privacy Build Number */

        public static string PrivacyRPCBuild { get; set; } = Application.ProductVersion;


        /* Theme Name & Author */

        public static string ThemeName { get; set; } = "Default";

        public static string ThemeAuthor { get; set; } = "Launcher - Division";


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
                            Image_Other.Logo = new Bitmap(ThemeFolder + "\\Logos\\" + ThemeFile.Key_Read("Logo"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("LogoSplashScreen")))
                    {
                        if (File.Exists(ThemeFolder + "\\Logos\\" + ThemeFile.Key_Read("LogoSplashScreen")))
                        {
                            Image_Other.Logo_Splash = new Bitmap(ThemeFolder + "\\Logos\\" + ThemeFile.Key_Read("LogoSplashScreen"));
                        }
                    }

                    /* Main Backgrounds */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SettingsScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\SettingsScreen\\" + ThemeFile.Key_Read("SettingsScreenBG")))
                        {
                            Image_Background.Settings = new Bitmap(ThemeFolder + "\\SettingsScreen\\" + ThemeFile.Key_Read("SettingsScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\MainScreen\\" + ThemeFile.Key_Read("MainScreenBG")))
                        {
                            Image_Background.Login = new Bitmap(ThemeFolder + "\\MainScreen\\" + ThemeFile.Key_Read("MainScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SocialPanelBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\MainScreen\\" + ThemeFile.Key_Read("SocialPanelBG")))
                        {
                            Image_Background.Server_Information = new Bitmap(ThemeFolder + "\\MainScreen\\" + ThemeFile.Key_Read("SocialPanelBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RegisterScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\RegisterScreen\\" + ThemeFile.Key_Read("RegisterScreenBG")))
                        {
                            Image_Background.Registration = new Bitmap(ThemeFolder + "\\RegisterScreen\\" + ThemeFile.Key_Read("RegisterScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("USXEScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\USXEScreen\\" + ThemeFile.Key_Read("USXEScreenBG")))
                        {
                            Image_Background.User_XML_Settings = new Bitmap(ThemeFolder + "\\USXEScreen\\" + ThemeFile.Key_Read("USXEScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SecurityCenterScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\SecurityCenterScreen\\" + ThemeFile.Key_Read("SecurityCenterScreenBG")))
                        {
                            Image_Background.Security_Center = new Bitmap(ThemeFolder + "\\SecurityCenterScreen\\" + ThemeFile.Key_Read("SecurityCenterScreenBG"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SplashScreenBG")))
                    {
                        if (File.Exists(ThemeFolder + "\\SplashScreen\\" + ThemeFile.Key_Read("SplashScreenBG")))
                        {
                            Image_Background.Splash = new Bitmap(ThemeFolder + "\\SplashScreen\\" + ThemeFile.Key_Read("SplashScreenBG"));
                        }
                    }

                    /* MainScreen Icons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("UpdateErrorIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateErrorIcon")))
                        {
                            Image_Icon.Engine_Error = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateErrorIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("UpdateSuccessIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateSuccessIcon")))
                        {
                            Image_Icon.Engine_Good = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateSuccessIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("UpdateUnkownIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateUnkownIcon")))
                        {
                            Image_Icon.Engine_Unknown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateUnkownIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("UpdateWarningIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateWarningIcon")))
                        {
                            Image_Icon.Engine_Warning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("UpdateWarningIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("APICheckingIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APICheckingIcon")))
                        {
                            Image_Icon.Plug_Checking = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APICheckingIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("APIErrorIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIErrorIcon")))
                        {
                            Image_Icon.Plug_Offline = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIErrorIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("APISuccessIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APISuccessIcon")))
                        {
                            Image_Icon.Plug_Online = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APISuccessIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("APIUnkownIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIUnkownIcon")))
                        {
                            Image_Icon.Plug_Unknown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIUnkownIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("APIWarningIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIWarningIcon")))
                        {
                            Image_Icon.Plug_Warning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("APIWarningIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ServerCheckingIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerCheckingIcon")))
                        {
                            Image_Icon.Server_Checking = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerCheckingIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ServerOfflineIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerOfflineIcon")))
                        {
                            Image_Icon.Server_Offline = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerOfflineIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ServerSuccessIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerSuccessIcon")))
                        {
                            Image_Icon.Server_Online = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerSuccessIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ServerWarningIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerWarningIcon")))
                        {
                            Image_Icon.Server_Warning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerWarningIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ServerUnknownIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerUnknownIcon")))
                        {
                            Image_Icon.Server_Unknown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ServerUnknownIcon"));
                        }
                    }

                    /* Social Panel Icons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DiscordIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("DiscordIcon")))
                        {
                            Image_Icon.Discord = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("DiscordIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DiscordIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("DiscordIconDisabled")))
                        {
                            Image_Icon.Discord_Disabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("DiscordIconDisabled"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("FacebookIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("FacebookIcon")))
                        {
                            Image_Icon.Facebook = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("FacebookIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("FacebookIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("FacebookIconDisabled")))
                        {
                            Image_Icon.Facebook_Disabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("FacebookIconDisabled"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("HomeIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("HomeIcon")))
                        {
                            Image_Icon.Home = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("HomeIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("HomeIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("HomeIconDisabled")))
                        {
                            Image_Icon.Home_Disabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("HomeIconDisabled"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("TwitterIcon")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("TwitterIcon")))
                        {
                            Image_Icon.Twitter = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("TwitterIcon"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("TwitterIconDisabled")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("TwitterIconDisabled")))
                        {
                            Image_Icon.Twitter_Disabled = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("TwitterIconDisabled"));
                        }
                    }

                    /* Image Buttons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButton")))
                        {
                            Image_Button.Grey = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButtonClick")))
                        {
                            Image_Button.Grey_Click = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButtonHover")))
                        {
                            Image_Button.Grey_Hover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GrayButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButton")))
                        {
                            Image_Button.Green = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButtonClick")))
                        {
                            Image_Button.Green_Click = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButtonHover")))
                        {
                            Image_Button.Green_Hover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GreenButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("CloseButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButton")))
                        {
                            Image_Icon.Close = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("CloseButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButtonClick")))
                        {
                            Image_Icon.Close_Click = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("CloseButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButtonHover")))
                        {
                            Image_Icon.Close_Hover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("CloseButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButton")))
                        {
                            Image_Icon.Gear = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonClick")))
                        {
                            Image_Icon.Gear_Click = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonHover")))
                        {
                            Image_Icon.Gear_Hover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButtonWarning")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarning")))
                        {
                            Image_Icon.Gear_Warning = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarning"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButtonWarningClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarningClick")))
                        {
                            Image_Icon.Gear_Warning_Click = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarningClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GearButtonWarningHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarningHover")))
                        {
                            Image_Icon.Gear_Warning_Hover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("GearButtonWarningHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("PlayButton")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButton")))
                        {
                            Image_Button.Play = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButton"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("PlayButtonClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButtonClick")))
                        {
                            Image_Button.Play_Click = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButtonClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("PlayButtonHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButtonHover")))
                        {
                            Image_Button.Play_Hover = new Bitmap(ThemeFolder + "\\Buttons\\" + ThemeFile.Key_Read("PlayButtonHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonUnknown")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknown")))
                        {
                            Image_Icon.Shield_Unknown = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknown"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonUnknownClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknownClick")))
                        {
                            Image_Icon.Shield_Unknown_Click = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknownClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonUnknownHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknownHover")))
                        {
                            Image_Icon.Shield_Unknown_Hover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonUnknownHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonChecking")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonChecking")))
                        {
                            Image_Icon.Shield_Checking = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonChecking"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonCheckingClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonCheckingClick")))
                        {
                            Image_Icon.Shield_Checking_Click = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonCheckingClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonCheckingHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonCheckingHover")))
                        {
                            Image_Icon.Shield_Checking_Hover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonCheckingHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonSuccess")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccess")))
                        {
                            Image_Icon.Shield_Success = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccess"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonSuccessClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccessClick")))
                        {
                            Image_Icon.Shield_Success_Click = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccessClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonSuccessHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccessHover")))
                        {
                            Image_Icon.Shield_Success_Hover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonSuccessHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonWarning")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarning")))
                        {
                            Image_Icon.Shield_Warning = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarning"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonWarningClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarningClick")))
                        {
                            Image_Icon.Shield_Warning_Click = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarningClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonWarningHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarningHover")))
                        {
                            Image_Icon.Shield_Warning_Hover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonWarningHover"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonError")))
                        {
                            Image_Icon.Shield_Error = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonError"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonErrorClick")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonErrorClick")))
                        {
                            Image_Icon.Shield_Error_Click = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonErrorClick"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ShieldButtonErrorHover")))
                    {
                        if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonErrorHover")))
                        {
                            Image_Icon.Shield_Error_Hover = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Key_Read("ShieldButtonErrorHover"));
                        }
                    }

                    /* Custom Inputs Borders for MainScreen */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderTicket")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderTicket")))
                        {
                            Image_Other.Text_Border_Ticket = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderTicket"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderTicketError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderTicketError")))
                        {
                            Image_Other.Text_Border_Ticket_Error = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderTicketError"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderEmail")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderEmail")))
                        {
                            Image_Other.Text_Border_Email = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderEmail"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderEmailError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderEmailError")))
                        {
                            Image_Other.Text_Border_Email_Error = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderEmailError"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderPassword")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderPassword")))
                        {
                            Image_Other.Text_Border_Password = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderPassword"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BorderPasswordError")))
                    {
                        if (File.Exists(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderPasswordError")))
                        {
                            Image_Other.Text_Border_Password_Error = new Bitmap(ThemeFolder + "\\Inputs\\" + ThemeFile.Key_Read("BorderPasswordError"));
                        }
                    }

                    /* ProgressBar and Outline */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressBarCheckingOutline")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarCheckingOutline")))
                        {
                            Image_ProgressBar.Checking_Outline = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarCheckingOutline"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressBarPreloadOutline")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarPreloadOutline")))
                        {
                            Image_ProgressBar.Preload_Outline = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarPreloadOutline"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressBarWarningOutline")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarWarningOutline")))
                        {
                            Image_ProgressBar.Warning_Outline = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarWarningOutline"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressBarErrorOutline")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarErrorOutline")))
                        {
                            Image_ProgressBar.Error_Outline = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarErrorOutline"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressBarCompleteOutline")))
                    {
                        if (File.Exists(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarCompleteOutline")))
                        {
                            Image_ProgressBar.Complete_Outline = new Bitmap(ThemeFolder + "\\ProgressBar\\" + ThemeFile.Key_Read("ProgressBarCompleteOutline"));
                        }
                    }

                    /* WinForm Buttons */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BlueBackColorButton")))
                    {
                        Color_Winform_Buttons.Blue_Back_Color = Color_Converter.Value(ThemeFile.Key_Read("BlueBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BlueBorderColorButton")))
                    {
                        Color_Winform_Buttons.Blue_Border_Color = Color_Converter.Value(ThemeFile.Key_Read("BlueBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BlueForeColorButton")))
                    {
                        Color_Winform_Buttons.Blue_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("BlueForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BlueMouseOverBackColorButton")))
                    {
                        Color_Winform_Buttons.Blue_Mouse_Over_Back_Color = Color_Converter.Value(ThemeFile.Key_Read("BlueMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("YellowBackColorButton")))
                    {
                        Color_Winform_Buttons.Yellow_Back_Color = Color_Converter.Value(ThemeFile.Key_Read("YellowBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("YellowBorderColorButton")))
                    {
                        Color_Winform_Buttons.Yellow_Border_Color = Color_Converter.Value(ThemeFile.Key_Read("YellowBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("YellowForeColorButton")))
                    {
                        Color_Winform_Buttons.Yellow_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("YellowForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("YellowMouseOverBackColorButton")))
                    {
                        Color_Winform_Buttons.Yellow_Mouse_Over_Back_Color = Color_Converter.Value(ThemeFile.Key_Read("YellowMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RedBackColorButton")))
                    {
                        Color_Winform_Buttons.Red_Back_Color = Color_Converter.Value(ThemeFile.Key_Read("RedBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RedBorderColorButton")))
                    {
                        Color_Winform_Buttons.Red_Border_Color = Color_Converter.Value(ThemeFile.Key_Read("RedBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RedForeColorButton")))
                    {
                        Color_Winform_Buttons.Red_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("RedForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RedMouseOverBackColorButton")))
                    {
                        Color_Winform_Buttons.Red_Mouse_Over_Back_Color = Color_Converter.Value(ThemeFile.Key_Read("RedMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenBackColorButton")))
                    {
                        Color_Winform_Buttons.Green_Back_Color = Color_Converter.Value(ThemeFile.Key_Read("GreenBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenBorderColorButton")))
                    {
                        Color_Winform_Buttons.Green_Border_Color = Color_Converter.Value(ThemeFile.Key_Read("GreenBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenForeColorButton")))
                    {
                        Color_Winform_Buttons.Green_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("GreenForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GreenMouseOverBackColorButton")))
                    {
                        Color_Winform_Buttons.Green_Mouse_Over_Back_Color = Color_Converter.Value(ThemeFile.Key_Read("GreenMouseOverBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayBackColorButton")))
                    {
                        Color_Winform_Buttons.Gray_Back_Color = Color_Converter.Value(ThemeFile.Key_Read("GrayBackColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayBorderColorButton")))
                    {
                        Color_Winform_Buttons.Gray_Border_Color = Color_Converter.Value(ThemeFile.Key_Read("GrayBorderColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayForeColorButton")))
                    {
                        Color_Winform_Buttons.Gray_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("GrayForeColorButton"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("GrayMouseOverBackColorButton")))
                    {
                        Color_Winform_Buttons.Gray_Mouse_Over_Back_Color = Color_Converter.Value(ThemeFile.Key_Read("GrayMouseOverBackColorButton"));
                    }

                    /* Text Colors */

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainTextForeColor")))
                    {
                        Color_Text.L_One = Color_Converter.Value(ThemeFile.Key_Read("MainTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SecondaryTextForeColor")))
                    {
                        Color_Text.L_Two = Color_Converter.Value(ThemeFile.Key_Read("SecondaryTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ThirdTextForeColor")))
                    {
                        Color_Text.L_Three = Color_Converter.Value(ThemeFile.Key_Read("ThirdTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("FourthTextForeColor")))
                    {
                        Color_Text.L_Four = Color_Converter.Value(ThemeFile.Key_Read("FourthTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("FivithTextForeColor")))
                    {
                        Color_Text.L_Five = Color_Converter.Value(ThemeFile.Key_Read("FivithTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SucessForeColor")))
                    {
                        Color_Text.S_Sucess = Color_Converter.Value(ThemeFile.Key_Read("SucessForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ErrorForeColor")))
                    {
                        Color_Text.S_Error = Color_Converter.Value(ThemeFile.Key_Read("ErrorForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WarningForeColor")))
                    {
                        Color_Text.S_Warning = Color_Converter.Value(ThemeFile.Key_Read("WarningForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("BannerBackColor")))
                    {
                        Color_Winform_Other.Server_Banner_BackColor = Color_Converter.Value(ThemeFile.Key_Read("BannerBackColor"));
                    }
                    
                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressColorWarning")))
                    {
                        Color_Winform_Other.ProgressBar_Warning_Top = Color_Converter.Value(ThemeFile.Key_Read("ProgressColorWarning"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressColorWarning")))
                    {
                        Color_Winform_Other.ProgressBar_Warning_Bottom = Color_Converter.Value(ThemeFile.Key_Read("ProgressColorWarningAccent"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressColorError")))
                    {
                        Color_Winform_Other.ProgressBar_Error_Top = Color_Converter.Value(ThemeFile.Key_Read("ProgressColorError"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressColorError")))
                    {
                        Color_Winform_Other.ProgressBar_Error_Bottom = Color_Converter.Value(ThemeFile.Key_Read("ProgressColorErrorAccent"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressColorChecking")))
                    {
                        Color_Winform_Other.ProgressBar_Loading_Top = Color_Converter.Value(ThemeFile.Key_Read("ProgressColorChecking"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressColorChecking")))
                    {
                        Color_Winform_Other.ProgressBar_Loading_Bottom = Color_Converter.Value(ThemeFile.Key_Read("ProgressColorCheckingAccent"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressColorSucess")))
                    {
                        Color_Winform_Other.ProgressBar_Sucess_Top = Color_Converter.Value(ThemeFile.Key_Read("ProgressColorSucess"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressColorSucess")))
                    {
                        Color_Winform_Other.ProgressBar_Sucess_Bottom = Color_Converter.Value(ThemeFile.Key_Read("ProgressColorSucessAccent"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressColorUnknown")))
                    {
                        Color_Winform_Other.ProgressBar_Warning_Top = Color_Converter.Value(ThemeFile.Key_Read("ProgressColorUnknown"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("ProgressColorUnknown")))
                    {
                        Color_Winform_Other.ProgressBar_Warning_Bottom = Color_Converter.Value(ThemeFile.Key_Read("ProgressColorUnknownAccent"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainScreenTransparencyKey")))
                    {
                        Color_Screen.BG_Main = Color_Converter.Value(ThemeFile.Key_Read("MainScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SettingsScreenTransparencyKey")))
                    {
                        Color_Screen.BG_Settings = Color_Converter.Value(ThemeFile.Key_Read("SettingsScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SplashScreenTransparencyKey")))
                    {
                        Color_Screen.BG_Splash = Color_Converter.Value(ThemeFile.Key_Read("SplashScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("RegisterScreenTransparencyKey")))
                    {
                        Color_Screen.BG_Registration = Color_Converter.Value(ThemeFile.Key_Read("RegisterScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("USXEEScreenTransparencyKey")))
                    {
                        Color_Screen.BG_User_XML_Editor = Color_Converter.Value(ThemeFile.Key_Read("USXEEScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SecurityCenterScreenTransparencyKey")))
                    {
                        Color_Screen.BG_Security_Center = Color_Converter.Value(ThemeFile.Key_Read("SecurityCenterScreenTransparencyKey"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainScreenInputForeColor")))
                    {
                        Color_Winform_Other.Input = Color_Converter.Value(ThemeFile.Key_Read("MainScreenInputForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainScreenLinkForeColor")))
                    {
                        Color_Winform_Other.Link = Color_Converter.Value(ThemeFile.Key_Read("MainScreenLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("MainScreenActiveLinkForeColor")))
                    {
                        Color_Winform_Other.Link_Active = Color_Converter.Value(ThemeFile.Key_Read("MainScreenActiveLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SettingsLinkForeColor")))
                    {
                        Color_Winform_Other.Link_Settings = Color_Converter.Value(ThemeFile.Key_Read("SettingsLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SettingsActiveLinkForeColor")))
                    {
                        Color_Winform_Other.Link_Settings_Active = Color_Converter.Value(ThemeFile.Key_Read("SettingsActiveLinkForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SettingsCheckBoxesForeColor")))
                    {
                        Color_Winform_Other.CheckBoxes_Settings = Color_Converter.Value(ThemeFile.Key_Read("SettingsCheckBoxesForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("SeventhTextForeColorForeColor")))
                    {
                        Color_Text.L_Seven = Color_Converter.Value(ThemeFile.Key_Read("SeventhTextForeColorForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("EighthTextForeColorForeColor")))
                    {
                        Color_Text.L_Eight = Color_Converter.Value(ThemeFile.Key_Read("EighthTextForeColorForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormTextForeColor")))
                    {
                        Color_Winform.Text_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("WinFormTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormSecondaryTextForeColor")))
                    {
                        Color_Winform.Secondary_Text_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("WinFormSecondaryTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormBGForeColor")))
                    {
                        Color_Winform.BG_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("WinFormBGForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormBGDarkerForeColor")))
                    {
                        Color_Winform.BG_Darker_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("WinFormBGDarkerForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormSuccessTextForeColor")))
                    {
                        Color_Winform.Success_Text_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("WinFormSuccessTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormWarningTextForeColor")))
                    {
                        Color_Winform.Warning_Text_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("WinFormWarningTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormErrorTextForeColor")))
                    {
                        Color_Winform.Error_Text_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("WinFormErrorTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("WinFormGridForeColor")))
                    {
                        Color_Winform.Grid_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("WinFormGridForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("AboutBGForeColor")))
                    {
                        Color_Winform_About.BG_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("AboutBGForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("AboutTextForeColor")))
                    {
                        Color_Winform_About.Text_Fore_Color = Color_Converter.Value(ThemeFile.Key_Read("AboutTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuTextForeColor")))
                    {
                        Color_Winform_Other.DropMenu_Text_ForeColor = Color_Converter.Value(ThemeFile.Key_Read("DropMenuTextForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuBackgroundForeColor")))
                    {
                        Color_Winform_Other.DropMenu_Background_ForeColor = Color_Converter.Value(ThemeFile.Key_Read("DropMenuBackgroundForeColor"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuTextForeColorCategory")))
                    {
                        Color_Winform_Other.DropMenu_Category_Text_ForeColor = Color_Converter.Value(ThemeFile.Key_Read("DropMenuTextForeColorCategory"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuBackgroundForeColorCategory")))
                    {
                        Color_Winform_Other.DropMenu_Category_Background_ForeColor = Color_Converter.Value(ThemeFile.Key_Read("DropMenuBackgroundForeColorCategory"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuPingSuccess")))
                    {
                        Color_Winform_Other.DropMenu_Ping_Success = Color_Converter.Value(ThemeFile.Key_Read("DropMenuPingSuccess"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuPingChecking")))
                    {
                        Color_Winform_Other.DropMenu_Ping_Checking = Color_Converter.Value(ThemeFile.Key_Read("DropMenuPingChecking"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuPingError")))
                    {
                        Color_Winform_Other.DropMenu_Ping_Error = Color_Converter.Value(ThemeFile.Key_Read("DropMenuPingError"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuPingWarning")))
                    {
                        Color_Winform_Other.DropMenu_Ping_Warning = Color_Converter.Value(ThemeFile.Key_Read("DropMenuPingWarning"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuBlack")))
                    {
                        Color_Winform_Other.DropMenu_Black = Color_Converter.Value(ThemeFile.Key_Read("DropMenuBlack"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("DropMenuWhite")))
                    {
                        Color_Winform_Other.DropMenu_White = Color_Converter.Value(ThemeFile.Key_Read("DropMenuWhite"));
                    }

                    if (!string.IsNullOrWhiteSpace(ThemeFile.Key_Read("PrivacyRPCBuild")))
                    {
                        PrivacyRPCBuild = ThemeFile.Key_Read("PrivacyRPCBuild");
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("THEMING", string.Empty, Error, string.Empty, true);
                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "THEMING", Error.InnerException.Message);
                    }
                }
                finally
                {
                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                    GC.Collect(); 
                    #endif
                }
            }
        }
    }
}