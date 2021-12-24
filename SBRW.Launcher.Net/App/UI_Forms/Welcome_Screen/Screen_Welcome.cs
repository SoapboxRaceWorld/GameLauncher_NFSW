using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using System;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Welcome_Screen
{
    public partial class Screen_Welcome : Form
    {
        private static bool IsWelcomeScreenOpen { get; set; }
        private bool StatusCheck { get; set; }

        public static void OpenScreen()
        {
            if (IsWelcomeScreenOpen || Application.OpenForms["Screen_Welcome"] != null)
            {
                if (Application.OpenForms["Screen_Welcome"] != null) { Application.OpenForms["Screen_Welcome"].Activate(); }
            }
            else
            {
                try { new Screen_Welcome().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Welcome Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Welcome Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public Screen_Welcome()
        {
            IsWelcomeScreenOpen = true;
            InitializeComponent();
            //SetVisuals();
            this.Closing += (x, CloseForm) =>
            {
                IsWelcomeScreenOpen = false;
                GC.Collect();
            };
        }
    }
}
