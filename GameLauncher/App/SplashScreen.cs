using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using System;
using GameLauncher.App.Classes.Logger;

namespace GameLauncher.App.Classes
{
    public partial class SplashScreen : Form
    {
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
            if (InformationCache.ServerListStatus == "Loaded" || FunctionStatus.LoadingComplete || FunctionStatus.LauncherForceClose)
            {
                Clock.Start();

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
