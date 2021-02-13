using GameLauncher.App.Classes;
using GameLauncher.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Visuals;

namespace GameLauncher.App
{
    public partial class WelcomeScreen : Form
    {
        private bool StatusCheck = false;

        public WelcomeScreen()
        {
            InitializeComponent();
            SetVisuals();
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            VersionLabel.Text = "Version: v" + Application.ProductVersion;

            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            var MainFontSize = 9f * 100f / CreateGraphics().DpiY;
            var SecondaryFontSize = 8f * 100f / CreateGraphics().DpiY;
            var ThirdFontSize = 10f * 100f / CreateGraphics().DpiY;
            var FourthFontSize = 14f * 100f / CreateGraphics().DpiY;

            if (DetectLinux.LinuxDetected())
            {
                MainFontSize = 9f;
                SecondaryFontSize = 8f;
                ThirdFontSize = 10f;
                FourthFontSize = 14f;
            }

            WelcomeText.Font = new Font(DejaVuSansBold, ThirdFontSize, FontStyle.Bold);
            DownloadSourceText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            CDNSource.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            Save.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ListStatusText.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            APIErrorButton.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            VersionLabel.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            BackColor = Theming.WinFormTBGForeColor;
            ForeColor = Theming.WinFormTextForeColor;

            ListStatusText.ForeColor = Theming.WinFormTextForeColor;
            WelcomeText.ForeColor = Theming.WinFormSecondaryTextForeColor;

            DownloadSourceText.ForeColor = Theming.WinFormTextForeColor;

            APIErrorButton.ForeColor = Theming.BlueForeColorButton;
            APIErrorButton.BackColor = Theming.BlueBackColorButton;
            APIErrorButton.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            APIErrorButton.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            Save.ForeColor = Theming.BlueForeColorButton;
            Save.BackColor = Theming.BlueBackColorButton;
            Save.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            Save.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            VersionLabel.ForeColor = Theming.WinFormTextForeColor;

            /********************************/
            /* Events                        /
            /********************************/

            CDNSource.DrawItem += new DrawItemEventHandler(CDNSource_DrawItem);
            CDNSource.SelectedIndexChanged += new EventHandler(CDNSource_SelectedIndexChanged);
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
            
            if (VisualsAPIChecker.UnitedAPI != true)
            {
                if (VisualsAPIChecker.CarbonAPI == true)
                {
                    ListStatusText.Text = "Carbon List - Online";
                    StatusCheck = true;
                }
            }

            if (VisualsAPIChecker.CarbonAPI != true)
            {
                if (VisualsAPIChecker.CarbonAPITwo == true)
                {
                    ListStatusText.Text = "Carbon 2nd List - Online";
                    StatusCheck = true;
                }
            }

            if (VisualsAPIChecker.CarbonAPITwo != true)
            {
                if (VisualsAPIChecker.WOPLAPI == true)
                {
                    ListStatusText.Text = "WOPL List - Online";
                    StatusCheck = true;
                }
            }

            if (VisualsAPIChecker.WOPLAPI != true)
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
            Brush customTextColor = new SolidBrush(Theming.CDNMenuTextForeColor);
            Brush customBGColor = new SolidBrush(Theming.CDNMenuBGForeColor);
            Brush cat_customTextColor = new SolidBrush(Theming.CDNMenuTextForeColor_Category);
            Brush cat_customBGColor = new SolidBrush(Theming.CDNMenuBGForeColor_Category);

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
                e.Graphics.FillRectangle(cat_customBGColor, e.Bounds);
                e.Graphics.DrawString(cdnListText.Replace("<GROUP>", string.Empty), font, cat_customTextColor, e.Bounds);
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
