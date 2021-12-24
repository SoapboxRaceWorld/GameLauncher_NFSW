using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.Core.Discord.RPC_;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Settings_Screen
{
    public partial class Screen_Settings : Form
    {
        /*******************************/
        /* Global Functions             /
        /*******************************/

        private static bool IsSettingsScreenOpen { get; set; }
        private int LastSelectedCdnId { get; set; }
        private int LastSelectedLanguage { get; set; }
        private bool DisableProxy { get; set; }
        private bool DisableDiscordRPC { get; set; }
        private bool EnableAltWebCalls { get; set; }
        private bool EnableInsiderPreview { get; set; }
        private bool EnableThemeSupport { get; set; }
        private bool EnableStreamSupport { get; set; }
        private bool RestartRequired { get; set; }
        private string NewLauncherPath { get; set; }
        private string NewGameFilesPath { get; set; }
        private string FinalCDNURL { get; set; }
        private static Thread? ThreadChangedCDN { get; set; }
        private static Thread? ThreadSavedCDN { get; set; }
        private static Thread? ThreadChecksums { get; set; }

        public static void OpenScreen()
        {
            if (IsSettingsScreenOpen || Application.OpenForms["Screen_Settings"] != null)
            {
                if (Application.OpenForms["Screen_Settings"] != null) { Application.OpenForms["Screen_Settings"].Activate(); }
            }
            else
            {
                try { new Screen_Settings().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Settings Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Settings Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public Screen_Settings()
        {
            IsSettingsScreenOpen = true;
            InitializeComponent();
            //SetVisuals();
            this.Closing += (x, y) =>
            {
                Presence_Launcher.Status("Idle Ready", null);

                if (ThreadChangedCDN != null)
                {
                    ThreadChangedCDN.Abort();
                    ThreadChangedCDN = null;
                }
                if (ThreadSavedCDN != null)
                {
                    ThreadSavedCDN.Abort();
                    ThreadSavedCDN = null;
                }
                if (ThreadChecksums != null)
                {
                    ThreadChecksums.Abort();
                    ThreadChecksums = null;
                }

                IsSettingsScreenOpen = false;

                /* This is for Mono Support */
                if (ToolTip_Hover.Active)
                {
                    ToolTip_Hover.RemoveAll();
                    ToolTip_Hover.Dispose();
                }

                GC.Collect();
            };

            Presence_Launcher.Status("Settings", null);
        }
    }
}
