using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using System;
using GameLauncher.App.Classes.Logger;

namespace GameLauncher.App.Classes
{
    public partial class SplashScreen : Form
    {
        public static int i = 0;
        public SplashScreen()
        {
            InitializeComponent();

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            BackColor = Theming.SplashScreenTransparencyKey;
            TransparencyKey = Theming.SplashScreenTransparencyKey;
            BackgroundImage = Theming.LogoSplash;

            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Text = "Splash Screen - SBRW Launcher: v" + Application.ProductVersion;
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            FunctionStatus.CenterScreen(this);
        }

        private void Clock_Tick(object sender, EventArgs e)
        {
            i++;

            if (InformationCache.ServerListStatus == "Loaded" || FunctionStatus.LoadingComplete == true)
            {
                Clock.Start();
                Application.OpenForms["SplashScreen"].Close();
            }
            else if (i >= 300)
            {
                Log.Error("SPLASH SCREEN: Launcher might have hanged? 5 Mintinues had passed so I'm closing the form");

                try
                {
                    Application.OpenForms["SplashScreen"].Close();
                }
                catch (Exception error)
                {
                    Log.Error("SPLASH SCREEN: " + error.Message);
                    Close();
                }
            }
        }
    }
}
