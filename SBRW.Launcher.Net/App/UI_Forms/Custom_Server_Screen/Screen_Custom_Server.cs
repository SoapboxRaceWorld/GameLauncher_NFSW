using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Custom_Server_Screen
{
    public partial class Screen_Custom_Server : Form
    {
        private static bool IsSelectServerOpen { get; set; }
        private static bool CustomServersOnly { get; set; }
        private static int ID { get; set; } = 1;
        private static readonly Dictionary<int, Json_List_Server> ServerListBook = new Dictionary<int, Json_List_Server>();

        /* Used to ping the Server in ms */
        public static Queue<string> ServersToPing { get; set; } = new Queue<string>();

        public static string ServerName { get; set; }
        public static Json_Server_Info ServerJsonData { get; set; }

        public static void OpenScreen(bool CSO)
        {
            if (IsSelectServerOpen || Application.OpenForms["Screen_Custom_Server"] != null)
            {
                if (Application.OpenForms["Screen_Custom_Server"] != null) { Application.OpenForms["Screen_Custom_Server"].Activate(); }
            }
            else
            {
                try
                {
                    CustomServersOnly = CSO;
                    new Screen_Custom_Server().ShowDialog();
                }
                catch (Exception Error)
                {
                    string ErrorMessage = "Select Server Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Select Server", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public Screen_Custom_Server()
        {
            IsSelectServerOpen = true;
            InitializeComponent();
            //SetVisuals();
            this.Closing += (x, y) =>
            {
                ID = 1;
                ServerListBook.Clear();
                ServersToPing.Clear();
                ServerName = string.Empty;
                ServerJsonData = null;
                CustomServersOnly = IsSelectServerOpen = false;
                GC.Collect();
            };
        }
    }
}
