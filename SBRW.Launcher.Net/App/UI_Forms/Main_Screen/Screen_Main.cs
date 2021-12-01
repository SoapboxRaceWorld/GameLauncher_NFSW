using System;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Main_Screen
{
    public partial class Screen_Main : Form
    {
        private bool Launcher_Restart { get; set; }

        private void Set_Visuals()
        {
            Button_Close.Click += new EventHandler(Button_Close_Click);
        }

        public Screen_Main()
        {
            InitializeComponent();
            Set_Visuals();
        }

        private void ClosingTasks()
        {
            /*
            Save_Settings.Save();
            Save_Account.Save();

            try
            { _downloader.Stop(); }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("CDN DOWNLOADER", null, Error, null, true);
            }

            try
            {
                if (FunctionStatus.LauncherBattlePass)
                {
                    Process.GetProcessById(_nfswPid).Kill();
                }
                else
                {
                    Process[] allOfThem = Process.GetProcessesByName("nfsw");

                    if (allOfThem != null && allOfThem.Any())
                    {
                        foreach (var oneProcess in allOfThem)
                        {
                            Process.GetProcessById(oneProcess.Id).Kill();
                        }
                    }
                }
            }
            catch { }

            if (Presence_Launcher.Running())
            {
                Presence_Launcher.Stop("Close");
            }

            if (Proxy_Settings.Running())
            {
                Proxy_Server.Instance.Stop("Main Screen");
            }

            try { Notification.Dispose(); } catch { }

            if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)) && !FunctionStatus.LauncherBattlePass)
            {
                ModNetHandler.CleanLinks(Save_Settings.Live_Data.Game_Path);
            }
            */
        }

        private void Button_Close_Click(object sender, EventArgs e)
        {
            ClosingTasks();

            /* Leave this here. Its to properly close the launcher from Visual Studio (And Close the Launcher a well) 
             * If the Boolen is true it will restart the Application
             */
            if (Launcher_Restart)
            {
                Application.Restart();
            }
            else
            {
                try { this.Close(); } catch { }
            }
        }
    }
}
