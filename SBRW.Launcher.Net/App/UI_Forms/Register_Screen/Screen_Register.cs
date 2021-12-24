using SBRW.Launcher.App.Classes.LauncherCore.Lists;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.Core.Discord.RPC_;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Register_Screen
{
    public partial class Screen_Register : Form
    {
        private static bool IsRegisterScreenOpen { get; set; }
        private bool _ticketRequired { get; set; }

        public static void OpenScreen()
        {
            if (IsRegisterScreenOpen || Application.OpenForms["Screen_Register"] != null)
            {
                if (Application.OpenForms["Screen_Register"] != null) { Application.OpenForms["Screen_Register"].Activate(); }
            }
            else
            {
                try { new Screen_Register().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Register Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Register Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public Screen_Register()
        {
            IsRegisterScreenOpen = true;
            InitializeComponent();
            //SetVisuals();
            Presence_Launcher.Status("Register", ServerListUpdater.ServerName("Register"));
            this.Closing += (x, y) =>
            {
                Presence_Launcher.Status("Idle Ready", null);
                IsRegisterScreenOpen = false;
                GC.Collect();
            };
        }
    }
}
