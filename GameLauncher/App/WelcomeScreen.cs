using GameLauncher.App.Classes;
using GameLauncherReborn;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.App
{
    public partial class WelcomeScreen : Form {
        private readonly IniFile _settingFile = new IniFile("Settings.ini");
        List<CDNObject> CDNList = new List<CDNObject>();
        private bool CDNStatusCheck = false;
        private bool ServerStatusCheck = false;

        public WelcomeScreen() {
            InitializeComponent();
        }

        //Check Serverlist API Status Upon Main Screen load - DavidCarbon
        private async void PingServerListStatus()
        {
            HttpWebRequest requestMainServerListAPI = (HttpWebRequest)HttpWebRequest.Create(Self.mainserver + "/serverlist.json");
            requestMainServerListAPI.AllowAutoRedirect = false;
            requestMainServerListAPI.Method = "HEAD";
            requestMainServerListAPI.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            try
            {
                HttpWebResponse mainServerListResponseAPI = (HttpWebResponse)requestMainServerListAPI.GetResponse();
                mainServerListResponseAPI.Close();
                ServerStatusText.Text = "Main Server List - Online";
                ServerStatusCheck = true;
            }
            catch (WebException)
            {
                ServerStatusText.Text = "Main Server List - Offline";

                await Task.Delay(500);
                ServerStatusText.Text = "Checking Backup API - Pinging";

                await Task.Delay(1000);
                try
                {
                    //Check Using Backup API
                    HttpWebRequest requestBkupServerListAPI = (HttpWebRequest)HttpWebRequest.Create(Self.staticapiserver + "/serverlist.json");
                    requestBkupServerListAPI.AllowAutoRedirect = false;
                    requestBkupServerListAPI.Method = "HEAD";
                    requestBkupServerListAPI.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                    try
                    {
                        HttpWebResponse bkupServerListResponseAPI = (HttpWebResponse)requestBkupServerListAPI.GetResponse();
                        bkupServerListResponseAPI.Close();
                        ServerStatusText.Text = "Backup Server List - Online";
                        ServerStatusCheck = true;
                    }
                    catch (WebException)
                    {
                        ServerStatusText.Text = "Server Lists Connection - Error";
                    }
                }
                catch { }
            }
        }

        private async void PingCDNListStatus()
        {
            HttpWebRequest requestMainServerListAPI = (HttpWebRequest)HttpWebRequest.Create(Self.mainserver + "/cdn_list.json");
            requestMainServerListAPI.AllowAutoRedirect = false;
            requestMainServerListAPI.Method = "HEAD";
            requestMainServerListAPI.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            try
            {
                HttpWebResponse mainServerListResponseAPI = (HttpWebResponse)requestMainServerListAPI.GetResponse();
                mainServerListResponseAPI.Close();
                CDNStatusText.Text = "Main CDN List - Online";
                CDNStatusCheck = true;
            }
            catch (WebException)
            {
                CDNStatusText.Text = "Main CDN List - Offline";

                await Task.Delay(500);
                CDNStatusText.Text = "Checking Backup API - Pinging";

                await Task.Delay(1000);
                try
                {
                    //Check Using Backup API
                    HttpWebRequest requestBkupServerListAPI = (HttpWebRequest)HttpWebRequest.Create(Self.staticapiserver + "/cdn_list.json");
                    requestBkupServerListAPI.AllowAutoRedirect = false;
                    requestBkupServerListAPI.Method = "HEAD";
                    requestBkupServerListAPI.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                    try
                    {
                        HttpWebResponse bkupServerListResponseAPI = (HttpWebResponse)requestBkupServerListAPI.GetResponse();
                        bkupServerListResponseAPI.Close();
                        CDNStatusText.Text = "Backup CDN List - Online";
                        CDNStatusCheck = true;
                    }
                    catch (WebException)
                    {
                        CDNStatusText.Text = "CDN Lists Connection - Error";
                    }
                }
                catch { }
            }
        }

        private void WelcomeScreen_Load(object sender, EventArgs e) {
            SettingsFormElements(false);
            APIErrorFormElements(false);
            PingServerListStatus();
            PingCDNListStatus();
            CheckFinalAPIStatus();
        }

        private void ShowCDNSources()
        {
            //CDN
            String _slresponse = String.Empty;
            try
            {
                WebClientWithTimeout wc = new WebClientWithTimeout();
                try
                {
                    _slresponse = wc.DownloadString(Self.CDNUrlList);
                }
                catch
                {
                    _slresponse = wc.DownloadString(Self.CDNUrlStaticList);
                }
            }
            catch
            {
                _slresponse = JsonConvert.SerializeObject(new[] {
                    new CDNObject {
                        Name = "[CF] WorldUnited.gg Mirror",
                        Url = "http://cdn.worldunited.gg/gamefiles/packed/"
                    },
                    new CDNObject {
                        Name = "[CF] DavidCarbon Mirror",
                        Url = "http://g-sbrw.davidcarbon.download/"
                    }
                });
            }

            CDNList = JsonConvert.DeserializeObject<List<CDNObject>>(_slresponse);

            CDNSource.DisplayMember = "name";
            CDNSource.DataSource = CDNList;
        }

        private void Save_Click(object sender, EventArgs e) {
            CDN.CDNUrl = ((CDNObject)CDNSource.SelectedItem).Url;

            QuitWithoutSaving_Click(sender, e);
        }

        private void QuitWithoutSaving_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void APIErrorButton_Click(object sender, EventArgs e)
        {
            APIErrorFormElements(false);
            SettingsFormElements();
            WelcomeText.Text = "Howdy! Looks like it's the first time this launcher is started. Please specify where you want to download all required game files";
        }

        private async void CheckFinalAPIStatus()
        {
            await Task.Delay(2000);
            if (ServerStatusCheck == true && CDNStatusCheck == false)
            {
                WelcomeText.Text = "Looks like the Game Launcher failed to Reach CDN APIs. Clicking 'Manual Bypass' will allow you to continue with the Error";
                APIErrorFormElements();
            }

            await Task.Delay(2000);
            if (ServerStatusCheck == false && CDNStatusCheck == true)
            {
                WelcomeText.Text = "Looks like the Game Launcher failed to Reach Server Lists APIs. Clicking 'Manual Bypass' will allow you to continue with the Error";
                APIErrorFormElements();
            }

            await Task.Delay(1000);
            if (ServerStatusCheck == false && CDNStatusCheck == false)
            {
                WelcomeText.Text = "Looks like the Game Launcher failed to Reach our APIs. Clicking 'Manual Bypass' will allow you to continue with the Error";
                APIErrorFormElements();
            }

            await Task.Delay(1000);
            if (ServerStatusCheck == true && CDNStatusCheck == true)
            {
                APIErrorFormElements(false);
                SettingsFormElements(true);
                WelcomeText.Text = "Howdy! Looks like it's the first time this launcher is started. Please specify where you want to download all required game files";
            }
        }

        private void APIErrorFormElements(bool hideElements = true)
        {
            apiErrorButton.Visible = hideElements;
        }

        private void SettingsFormElements(bool hideElements = true)
        {
            ShowCDNSources();
            downloadSourceText.Visible = hideElements;
            CDNSource.Visible = hideElements;
            Save.Visible = hideElements;
        }
    }
}
