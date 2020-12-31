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
    public partial class WelcomeScreen : Form
    {
        private readonly IniFile _settingFile = new IniFile("Settings.ini");
        List<CDNObject> CDNList = new List<CDNObject>();
        private bool CDNStatusCheck = false;
        private bool ServerStatusCheck = false;

        public WelcomeScreen()
        {
            InitializeComponent();
            ApplyEmbeddedFonts();

            CDNSource.DrawItem += new DrawItemEventHandler(CDNSource_DrawItem);
            CDNSource.SelectedIndexChanged += new EventHandler(CDNSource_SelectedIndexChanged);
            VersionLabel.Text = "Version: v" + Application.ProductVersion;
        }

        private void ApplyEmbeddedFonts()
        {
            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");
            WelcomeText.Font = new Font(DejaVuSansBold, 10f, FontStyle.Bold);
            DownloadSourceText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            CDNSource.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            Save.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            ServerStatusText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            CDNStatusText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            APIErrorButton.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            VersionLabel.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
        }

        private void CDNSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((CDNObject)CDNSource.SelectedItem).IsSpecial)
            {
                CDNSource.SelectedIndex = 1;
                return;
            }
        }

        //Check Serverlist API Status Upon Main Screen load - DavidCarbon
        private void PingServerListStatus()
        {
            bool WUGGAPIOffline = false;
            bool DCAPIOffline = false;
            bool DC2APIOffline = false;
            bool AllAPIsOffline = false;

            switch (APIStatusChecker.CheckStatus(Self.mainserver + "/serverlist.json"))
            {
                case API.Online:
                    ServerStatusText.Text = "United Server List - Online";
                    ServerStatusCheck = true;
                    break;
                default:
                    WUGGAPIOffline = true;
                    break;
            }

            if (WUGGAPIOffline == true && DCAPIOffline == false && DC2APIOffline == false && AllAPIsOffline == false)
            {
                switch (APIStatusChecker.CheckStatus(Self.staticapiserver + "/serverlist.json"))
                {
                    case API.Online:
                        ServerStatusText.Text = "Carbon Server List - Online";
                        ServerStatusCheck = true;
                        break;
                    default:
                        DCAPIOffline = true;
                        break;
                }
            }

            if (WUGGAPIOffline == true && DCAPIOffline == true && DC2APIOffline == false && AllAPIsOffline == false)
            {
                switch (APIStatusChecker.CheckStatus(Self.secondstaticapiserver + "/serverlist.json"))
                {
                    case API.Online:
                        ServerStatusText.Text = "Carbon 2nd Server List - Online";
                        ServerStatusCheck = true;
                        break;
                    default:
                        DC2APIOffline = true;
                        break;
                }
            }

            if (WUGGAPIOffline == true && DCAPIOffline == true && DC2APIOffline == true && AllAPIsOffline == false)
            {
                switch (APIStatusChecker.CheckStatus(Self.woplserver + "/serverlist.json"))
                {
                    case API.Online:
                        ServerStatusText.Text = "WOPL Server List - Online";
                        ServerStatusCheck = true;
                        break;
                    default:
                        AllAPIsOffline = true;
                        break;
                }
            }

            if (AllAPIsOffline == true)
            {
                ServerStatusText.Text = "Server Lists Connection - Error";
            }
        }

        private void PingCDNListStatus()
        {
            bool WUGGAPIOffline = false;
            bool DCAPIOffline = false;
            bool DC2APIOffline = false;
            bool AllAPIsOffline = false;

            switch (APIStatusChecker.CheckStatus(Self.mainserver + "/cdn_list.json"))
            {
                case API.Online:
                    CDNStatusText.Text = "United CDN List - Online";
                    CDNStatusCheck = true;
                    break;
                default:
                    WUGGAPIOffline = true;
                    break;
            }

            if (WUGGAPIOffline == true && DCAPIOffline == false && DC2APIOffline == false && AllAPIsOffline == false)
            {
                switch (APIStatusChecker.CheckStatus(Self.staticapiserver + "/cdn_list.json"))
                {
                    case API.Online:
                        CDNStatusText.Text = "Carbon CDN List - Online";
                        CDNStatusCheck = true;
                        break;
                    default:
                        DCAPIOffline = true;
                        break;
                }
            }

            if (WUGGAPIOffline == true && DCAPIOffline == true && DC2APIOffline == false && AllAPIsOffline == false)
            {
                switch (APIStatusChecker.CheckStatus(Self.secondstaticapiserver + "/cdn_list.json"))
                {
                    case API.Online:
                        CDNStatusText.Text = "Carbon 2nd Server List - Online";
                        CDNStatusCheck = true;
                        break;
                    default:
                        DC2APIOffline = true;
                        break;
                }
            }

            if (WUGGAPIOffline == true && DCAPIOffline == true && DC2APIOffline == true && AllAPIsOffline == false)
            {
                switch (APIStatusChecker.CheckStatus(Self.woplserver + "/cdn_list.json"))
                {
                    case API.Online:
                        CDNStatusText.Text = "WOPL Server List - Online";
                        CDNStatusCheck = true;
                        break;
                    default:
                        AllAPIsOffline = true;
                        break;
                }
            }

            if (AllAPIsOffline == true)
            {
                CDNStatusText.Text = "CDN Lists Connection - Error";
            }
        }

        private void WelcomeScreen_Load(object sender, EventArgs e)
        {
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

            Log.Info("WELCOME: Setting CDN list");
            finalCDNItems = CDNListUpdater.GetCDNList();

            CDNSource.DisplayMember = "Name";
            CDNSource.DataSource = finalCDNItems;
        }

        private void Save_Click(object sender, EventArgs e)
        {
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

        private void QuitWithoutSaving_Click(object sender, EventArgs e)
        {
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
