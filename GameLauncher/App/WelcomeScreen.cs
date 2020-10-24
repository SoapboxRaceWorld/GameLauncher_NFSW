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
            //CDN
            String _slresponse = String.Empty;
            try {
                WebClientWithTimeout wc = new WebClientWithTimeout();
                try {
                    _slresponse = wc.DownloadString(Self.CDNUrlList);
                } catch {
                    _slresponse = wc.DownloadString(Self.CDNUrlStaticList);
                }
            } catch {
                _slresponse = JsonConvert.SerializeObject(new[] {
                    new CDNObject {
                        name = "[CF] WorldUnited.gg Mirror",
                        url = "http://cdn.worldunited.gg/gamefiles/packed/"
                    },
                    new CDNObject {
                        name = "[CF] DavidCarbon Mirror",
                        url = "http://g-sbrw.davidcarbon.download/"
                    }
                });
            }

            CDNList = JsonConvert.DeserializeObject<List<CDNObject>>(_slresponse);

        }

        private void Save_Click(object sender, EventArgs e) {
            CDN.TrackHigh = "1";
            CDN.CDNUrl = ((CDNObject)CDNSource.SelectedItem).url;

            QuitWithoutSaving_Click(sender, e);
        }

        private void QuitWithoutSaving_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
