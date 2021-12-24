using SBRW.Launcher.App.Classes.LauncherCore.Global;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.App.Classes.LauncherCore.Support;
using SBRW.Launcher.App.Classes.LauncherCore.Visuals;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Threading;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Splash_Screen
{
    public partial class Screen_Splash : Form
    {
        /* Global Thread for Splash Screen */
        private static Thread SplashScreenThread { get; set; }
        private static bool IsSplashScreenLive { get; set; }
        private static int ProcessID { get; set; }

        private static void StartSplashScreen()
        {
            if (!IsSplashScreenLive)
            {
                new Screen_Splash().ShowDialog();
                IsSplashScreenLive = true;
            }
        }

        public static void ThreadStatus(string ContinueThread)
        {
            try
            {
                if (ContinueThread == "Start" && !IsSplashScreenLive)
                {
                    Log.Info("SPLASH SCREEN: Attempting to Start Thread");
                    SplashScreenThread = new Thread(new ThreadStart(StartSplashScreen));
                    SplashScreenThread.Start();
                    Log.Info("SPLASH SCREEN: Thread has now Started");
                }
                else if (ContinueThread == "Stop" && IsSplashScreenLive)
                {
#if NETFRAMEWORK
                    Log.Info("SPLASH SCREEN: Attempting to Stop Thread");
                    SplashScreenThread.Abort();
                    Log.Completed("SPLASH SCREEN: Thread is now Stopped");
#else

#endif
                }
                else
                {
                    Log.Info("SPLASH SCREEN: Thread has already been Stopped");
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("SPLASH SCREEN", string.Empty, Error, string.Empty, true);
            }
        }

        public Screen_Splash()
        {
            InitializeComponent();

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            BackColor = Color_Screen.BG_Splash;
            TransparencyKey = Color_Screen.BG_Splash;
            BackgroundImage = Image_Other.Logo_Splash;

            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Text = "Splash Screen - SBRW Launcher: v" + Application.ProductVersion;

            this.Closing += (x, y) =>
            {
                IsSplashScreenLive = false;
                GC.Collect();
            };
            Shown += (x, y) =>
            {
                IsSplashScreenLive = true;
            };
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            FunctionStatus.CenterScreen(this);
        }

        private void Clock_Tick(object sender, EventArgs e)
        {
            if (FunctionStatus.LoadingComplete || FunctionStatus.LauncherForceClose)
            {
                Clock.Start();

                try
                {
                    Application.OpenForms[this.Name].Close();
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("SPLASH SCREEN", string.Empty, Error, string.Empty, true);
                    Close();
                }
            }
            else if (ProcessID == 0)
            {
                ProcessID++;
                this.SafeInvokeAction(() => BackgroundImage = Image_Other.Logo_Splash, this);
            }
        }
    }
}
