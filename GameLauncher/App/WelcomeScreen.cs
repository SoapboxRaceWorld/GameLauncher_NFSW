using GameLauncher.App.Classes;
using GameLauncherReborn;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.App {
    public partial class WelcomeScreen : Form {
        private readonly IniFile _settingFile = new IniFile("Settings.ini");
        List<CDNObject> CDNList = new List<CDNObject>();

        public WelcomeScreen() {
            InitializeComponent();


        }

        private void WelcomeScreen_Load(object sender, EventArgs e) {

        }

        private void Save_Click(object sender, EventArgs e) {

        }

        private void QuitWithoutSaving_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
