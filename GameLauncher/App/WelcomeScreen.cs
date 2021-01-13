using GameLauncher.App.Classes;
using GameLauncher.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.APICheckers;

namespace GameLauncher.App
{
    public partial class WelcomeScreen : Form
    {
        private bool StatusCheck = false;

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
            ListStatusText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
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

        private void CheckListStatus()
        {
            if (VisualsAPIChecker.UnitedAPI != false)
            {
                ListStatusText.Text = "United List - Online";
                StatusCheck = true;
            }
            
            if (VisualsAPIChecker.UnitedAPI == false)
            {
                if (VisualsAPIChecker.CarbonAPI == true)
                {
                    ListStatusText.Text = "Carbon List - Online";
                    StatusCheck = true;
                }
            }

            if (VisualsAPIChecker.CarbonAPI != false)
            {
                if (VisualsAPIChecker.CarbonAPITwo == true)
                {
                    ListStatusText.Text = "Carbon 2nd List - Online";
                    StatusCheck = true;
                }
            }

            if (VisualsAPIChecker.CarbonAPITwo != false)
            {
                if (VisualsAPIChecker.WOPLAPI == true)
                {
                    ListStatusText.Text = "WOPL List - Online";
                    StatusCheck = true;
                }
            }

            if (VisualsAPIChecker.WOPLAPI == false)
            {
                ListStatusText.Text = "API Lists Connection - Error";
            }

            if (StatusCheck == false)
            {
                WelcomeText.Text = "Looks like the Game Launcher failed to Reach our APIs. Clicking 'Manual Bypass' will allow you to continue with the Error";
                APIErrorFormElements();
            }
            else
            {
                APIErrorFormElements(false);
                SettingsFormElements(true);
                WelcomeText.Text = "Howdy! Looks like it's the first time this launcher is started. Please specify where you want to download all required game files";
            }
        }

        private void WelcomeScreen_Load(object sender, EventArgs e)
        {
            SettingsFormElements(false);
            APIErrorFormElements(false);
            CheckListStatus();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (((CDNObject)CDNSource.SelectedItem).Url != null)
            {
                string ChoosenCDN = ((CDNObject)CDNSource.SelectedItem).Url;
                char[] charsToTrim = { '/' };
                string FinalCDNURL = ChoosenCDN.TrimEnd(charsToTrim);

                CDN.CDNUrl = FinalCDNURL;

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

        private void APIErrorFormElements(bool hideElements = true)
        {
            APIErrorButton.Visible = hideElements;
        }

        private void SettingsFormElements(bool hideElements = true)
        {
            CDNSource.DisplayMember = "Name";
            CDNSource.DataSource = CDNListUpdater.CleanList;

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
