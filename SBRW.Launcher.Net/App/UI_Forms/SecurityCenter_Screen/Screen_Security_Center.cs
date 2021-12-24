using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extra.File_;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.SecurityCenter_Screen
{
    public partial class Screen_Security_Center : Form
    {
        ///<summary>Prevents a Duplicate Window from Happening</summary>
        private static bool IsSecurityCenterOpen { get; set; }
        ///<summary>Windows 10: Caches Old Game Path in the event of the user does Firewall First</summary>
        private static string CacheOldGameLocation { get; set; } = Save_Settings.Live_Data.Game_Path_Old;
        ///<summary>Disable Button: Firewall Rules API</summary>
        private static bool DisableButtonFRAPI { get; set; }
        ///<summary>Disable Button: Firewall Rules Check</summary>
        private static bool DisableButtonFRC { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Add All</summary>
        private static bool DisableButtonFRAA { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Add Launcher</summary>
        private static bool DisableButtonFRAL { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Add Game</summary>
        private static bool DisableButtonFRAG { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Remove All</summary>
        private static bool DisableButtonFRRA { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Remove Launcher</summary>
        private static bool DisableButtonFRRL { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Remove Game</summary>
        private static bool DisableButtonFRRG { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion API</summary>
        private static bool DisableButtonDRAPI { get; set; }
        ///<summary>Disable Button: Defender Exclusion Check</summary>
        private static bool DisableButtonDRC { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Add All</summary>
        private static bool DisableButtonDRAA { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Add Launcher</summary>
        private static bool DisableButtonDRAL { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Add Game</summary>
        private static bool DisableButtonDRAG { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Remove All</summary>
        private static bool DisableButtonDRRA { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Remove Launcher</summary>
        private static bool DisableButtonDRRL { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Remove Game</summary>
        private static bool DisableButtonDRRG { get; set; } = true;
        ///<summary>Disable Button: Permission Check</summary>
        private static bool DisableButtonPRC { get; set; }
        ///<summary>Disable Button: Permission Set</summary>
        private static bool DisableButtonPRAA { get; set; } = true;
        ///<summary>RPC: Which State to do once Form Closes</summary>
        private static string RPCStateCache { get; set; }

        public static void OpenScreen(string RPCState)
        {
            if (IsSecurityCenterOpen || Application.OpenForms["SecurityCenterScreen"] != null)
            {
                if (Application.OpenForms["SecurityCenterScreen"] != null) { Application.OpenForms["SecurityCenterScreen"].Activate(); }
            }
            else
            {
                RPCStateCache = RPCState;
                try { new Screen_Security_Center().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Security Center Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Security Center Panel", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public Screen_Security_Center()
        {
            IsSecurityCenterOpen = true;
            InitializeComponent();
            //SetVisuals();
            this.Closing += (x, y) =>
            {
                Presence_Launcher.Status(RPCStateCache, null);
                IsSecurityCenterOpen = DisableButtonFRAPI = DisableButtonDRAPI = DisableButtonDRAPI = DisableButtonPRC = false;
                RPCStateCache = string.Empty;
                GC.Collect();
            };

            Presence_Launcher.Status("Security Center", null);
        }
    }
}
