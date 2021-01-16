using GameLauncher.App.Classes.LauncherCore.Visuals;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GameLauncher.App.Classes
{
    public partial class SplashScreen : Form
    {
        private static readonly IniFile ThemeFile = new IniFile("Theme.ini");

        private static string ThemeFolder = AppDomain.CurrentDomain.BaseDirectory + "Theme";

        /* Logo */

        public static Bitmap Logo = Properties.Resources.logo;

        public SplashScreen()
        {
            InitializeComponent();

            /* Logo */

            if (!string.IsNullOrEmpty(ThemeFile.Read("Logo")))
            {
                if (File.Exists(ThemeFolder + "\\Icons\\" + ThemeFile.Read("Logo")))
                {
                    BackgroundImage = new Bitmap(ThemeFolder + "\\Icons\\" + ThemeFile.Read("Logo"));
                }
            }
            else
            {
                BackgroundImage = Theming.Logo;
            }

            if (!string.IsNullOrEmpty(ThemeFile.Read("SplashScreenTransparencyKey")))
            {
                TransparencyKey = Theming.ToColor(ThemeFile.Read("SplashScreenTransparencyKey"));
            }
            else
            {
                TransparencyKey = Theming.SplashScreenTransparencyKey;
            }
        }
    }
}
