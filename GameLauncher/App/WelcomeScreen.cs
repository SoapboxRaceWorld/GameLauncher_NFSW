using GameLauncher.App.Classes;
using GameLauncher.App.Classes.Logger;
using GameLauncherReborn;
using GameLauncher.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            ApplyEmbeddedFonts();

            CDNSource.DrawItem += new DrawItemEventHandler(CDNSource_DrawItem);
        }

        private void ApplyEmbeddedFonts()
        {
            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansCondensed = FontWrapper.Instance.GetFontFamily("DejaVuSansCondensed.ttf");
            WelcomeText.Font = new Font(DejaVuSans, 9.75f, FontStyle.Bold);
            DownloadSourceText.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            CDNSource.Font = new Font(DejaVuSans, 8.25f, FontStyle.Bold);
            Save.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            ServerStatusText.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Regular);
            CDNStatusText.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Regular);
            APIErrorButton.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            VersionLabel.Font = new Font(DejaVuSans, 8.25f, FontStyle.Regular);
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
            /* NEW CDN Display List */
            List<CDNObject> finalCDNItems = new List<CDNObject>();

            CDNListUpdater.UpdateCDNList();

            Log.Debug("WELCOME: Setting CDN list");
            finalCDNItems = CDNListUpdater.GetCDNList();

            CDNSource.DisplayMember = "Name";
            CDNSource.DataSource = finalCDNItems;
        }

        private void Save_Click(object sender, EventArgs e) {
            if (((CDNObject)CDNSource.SelectedItem).Url != null)
            {
                CDN.CDNUrl = ((CDNObject)CDNSource.SelectedItem).Url;

                QuitWithoutSaving_Click(sender, e);
            }
            else
            {
                MessageBox.Show(null, "Please Choose a CDN. \n\n(╯°□°）╯︵ ┻━┻", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            APIErrorButton.Visible = hideElements;
        }

        private void SettingsFormElements(bool hideElements = true)
        {
            ShowCDNSources();
            DownloadSourceText.Visible = hideElements;
            CDNSource.Visible = hideElements;
            Save.Visible = hideElements;
        }

        public void CDNSource_DrawItem(object sender, DrawItemEventArgs e)
        {
            var font = (sender as ComboBox).Font;
            Brush backgroundColor;
            Brush textColor;
            Brush customTextColor = new SolidBrush(Color.FromArgb(178, 210, 255));
            Brush customBGColor = new SolidBrush(Color.FromArgb(44, 58, 76));

            var cdnListText = "";

            if (sender is ComboBox cb)
            {
                if (cb.Items[e.Index] is CDNObject si)
                {
                    cdnListText = si.Name;
                }
            }

            if (cdnListText.StartsWith("<GROUP>"))
            {
                font = new Font(font, FontStyle.Bold);
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                e.Graphics.DrawString(cdnListText.Replace("<GROUP>", string.Empty), font, Brushes.Black, e.Bounds);
            }
            else
            {
                font = new Font(font, FontStyle.Bold);
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && e.State != DrawItemState.ComboBoxEdit)
                {
                    backgroundColor = SystemBrushes.Highlight;
                    textColor = SystemBrushes.HighlightText;
                }
                else
                {
                    backgroundColor = customBGColor;
                    textColor = customTextColor;
                }

                e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                e.Graphics.DrawString(cdnListText, font, textColor, e.Bounds);
            }

        }
    }
}
