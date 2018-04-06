using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncher.App;
using System.IO;
using System.Diagnostics;

namespace GameLauncher.App {
    public partial class UpdatePopup : Form {
        IniFile SettingFile = new IniFile("Settings.ini");
        String GitHubAPI = null;

        public UpdatePopup(String x) {
            InitializeComponent();
            GitHubAPI = x;
            CancelButton.Click += new EventHandler(CancelButton_Clicked);
            skipButton.Click += new EventHandler(SkipButton_Clicked);
            updateButton.Click += new EventHandler(UpdateButton_Clicked);
        }

        public void CancelButton_Clicked(object x, EventArgs d) {
            this.Close();
        }

        public void UpdateButton_Clicked(object x, EventArgs d) {
            if(File.Exists("GL_Update.exe")) {
                Process.Start(@"GL_Update.exe", Process.GetCurrentProcess().Id.ToString());
            } else {
                Process.Start(@"https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
            }

            this.Close();
        }

        public void SkipButton_Clicked(object x, EventArgs d) {
             SettingFile.Write("IgnoreUpdateVersion", GitHubAPI);
             this.Close();
        }

        private void UpdatePopup_Load(object sender, EventArgs e) {
            changeLogURL.Url = new Uri("https://launcher.soapboxrace.world/changelog/?version=" + Application.ProductVersion);
        }
    }
}
