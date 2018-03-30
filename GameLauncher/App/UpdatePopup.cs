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

namespace GameLauncher.App {
    public partial class UpdatePopup : Form {
        IniFile SettingFile = new IniFile("Settings.ini");
        String GitHubAPI = null;

        public UpdatePopup(String x) {
            InitializeComponent();
            GitHubAPI = x;
            CancelButton.Click += new EventHandler(CancelButton_Clicked);
        }

        public void CancelButton_Clicked(object x, EventArgs d) {
            if(SkipCheckBox.Checked) {
                SettingFile.Write("IgnoreUpdateVersion", GitHubAPI);
            }

            this.Close();
        }

        private void UpdatePopup_Load(object sender, EventArgs e) {
            changeLogURL.Url = new Uri("https://launcher.soapboxrace.world/changelog/?version=" + Application.ProductVersion);
        }
    }
}
