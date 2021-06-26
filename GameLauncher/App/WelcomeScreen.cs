using System;
using System.Drawing;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using System.IO;

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
            /* Load CDN List                /
            /*******************************/

            VisualsAPIChecker.PingAPIStatus("CDN List", "Welcome");

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            VersionLabel.Text = "Version: v" + Application.ProductVersion;

            if (DetectLinux.LinuxDetected())
            {
                ButtonSave.Text = "Save Settings and Game Language";
            }

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
            Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            WelcomeText.Font = new Font(DejaVuSansBold, ThirdFontSize, FontStyle.Bold);
            DownloadSourceText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            GameLangText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            GameLangSource.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            CDNSource.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ButtonSave.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ListStatusText.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ButtonAPIError.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            VersionLabel.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            BackColor = Theming.WinFormTBGForeColor;
            ForeColor = Theming.WinFormTextForeColor;

            ListStatusText.ForeColor = Theming.WinFormTextForeColor;
            WelcomeText.ForeColor = Theming.WinFormSecondaryTextForeColor;

            DownloadSourceText.ForeColor = Theming.WinFormTextForeColor;
            GameLangSource.ForeColor = Theming.WinFormTextForeColor;

            ButtonAPIError.ForeColor = Theming.BlueForeColorButton;
            ButtonAPIError.BackColor = Theming.BlueBackColorButton;
            ButtonAPIError.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            ButtonAPIError.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            ButtonSave.ForeColor = Theming.BlueForeColorButton;
            ButtonSave.BackColor = Theming.BlueBackColorButton;
            ButtonSave.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            ButtonSave.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            VersionLabel.ForeColor = Theming.WinFormTextForeColor;

            /********************************/
            /* Events                        /
            /********************************/

            CDNSource.DrawItem += new DrawItemEventHandler(CDNSource_DrawItem);
            CDNSource.SelectedIndexChanged += new EventHandler(CDNSource_SelectedIndexChanged);

            GameLangSource.DrawItem += new DrawItemEventHandler(GameLangSource_DrawItem);
            GameLangSource.SelectedIndexChanged += new EventHandler(GameLangSource_SelectedIndexChanged);
        }

        private void CDNSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (((CDNList)CDNSource.SelectedItem).IsSpecial)
                {
                    CDNSource.SelectedIndex = 1;
                    return;
                }
            }
            catch { }
        }

        private void GameLangSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (((LangObject)GameLangSource.SelectedItem).IsSpecial)
                {
                    GameLangSource.SelectedIndex = 1;
                    return;
                }
            }
            catch { }
        }

        private void CheckListStatus()
        {
            if (!VisualsAPIChecker.UnitedAPI)
            {
                ListStatusText.Text = "United List - Online";
                StatusCheck = true;
            }
            
            if (!VisualsAPIChecker.UnitedAPI)
            {
                if (VisualsAPIChecker.CarbonAPI)
                {
                    ListStatusText.Text = "Carbon List - Online";
                    StatusCheck = true;
                }
            }

            if (!VisualsAPIChecker.CarbonAPI)
            {
                if (VisualsAPIChecker.CarbonAPITwo)
                {
                    ListStatusText.Text = "Carbon 2nd List - Online";
                    StatusCheck = true;
                }
            }

            if (!VisualsAPIChecker.CarbonAPITwo)
            {
                if (VisualsAPIChecker.WOPLAPI)
                {
                    ListStatusText.Text = "WOPL List - Online";
                    StatusCheck = true;
                }
            }

            if (!VisualsAPIChecker.WOPLAPI)
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
            if (!string.IsNullOrWhiteSpace(((LangObject)GameLangSource.SelectedItem).INI_Value))
            {
                FileSettingsSave.Lang = ((LangObject)GameLangSource.SelectedItem).INI_Value;
                FileGameSettingsData.Language = ((LangObject)GameLangSource.SelectedItem).XML_Value;
            }

            if (((LangObject)GameLangSource.SelectedItem).Category == "Custom")
            {
                /* Create Custom Settings.ini for LangPicker.asi module */
                if (!Directory.Exists(FileSettingsSave.GameInstallation + "/scripts"))
                {
                    Directory.CreateDirectory(FileSettingsSave.GameInstallation + "/scripts");
                }

                IniFile LanguagePickerFile = new IniFile(FileSettingsSave.GameInstallation + "/scripts/LangPicker.ini");
                LanguagePickerFile.Write("Language", ((LangObject)GameLangSource.SelectedItem).INI_Value);
                MessageBox.Show(null, "Please Note: If a Server does not Provide Language Pack, it will Fallback to English instead.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (File.Exists(FileSettingsSave.GameInstallation + "/scripts/LangPicker.ini"))
                {
                    File.Delete(FileSettingsSave.GameInstallation + "/scripts/LangPicker.ini");
                }
            }

            if (!string.IsNullOrWhiteSpace(((CDNList)CDNSource.SelectedItem).Url))
            {
                string ChoosenCDN = ((CDNList)CDNSource.SelectedItem).Url;
                string FinalCDNURL;

                if (ChoosenCDN.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    FinalCDNURL = ChoosenCDN.TrimEnd(charsToTrim);
                }
                else
                {
                    FinalCDNURL = ((CDNList)CDNSource.SelectedItem).Url;
                }

                SelectedCDN.CDNUrl = FinalCDNURL;

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
            ButtonAPIError.Visible = hideElements;
        }

        private void SettingsFormElements(bool hideElements = true)
        {
            CDNSource.DisplayMember = "Name";
            CDNSource.DataSource = CDNListUpdater.CleanList;

            GameLangSource.DisplayMember = "Name";
            GameLangSource.DataSource = LanguageListUpdater.CleanList;

            GameLangText.Visible = hideElements;
            GameLangSource.Visible = hideElements;
            DownloadSourceText.Visible = hideElements;
            CDNSource.Visible = hideElements;
            ButtonSave.Visible = hideElements;
        }

        public void CDNSource_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
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
                    if (cb.Items[e.Index] is CDNList si)
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
            catch { }
        }

        private void GameLangSource_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                var font = (sender as ComboBox).Font;
                Brush backgroundColor;
                Brush textColor;
                Brush customTextColor = new SolidBrush(Theming.CDNMenuTextForeColor);
                Brush customBGColor = new SolidBrush(Theming.CDNMenuBGForeColor);
                Brush cat_customTextColor = new SolidBrush(Theming.CDNMenuTextForeColor_Category);
                Brush cat_customBGColor = new SolidBrush(Theming.CDNMenuBGForeColor_Category);

                var langListText = "";

                if (sender is ComboBox cb)
                {
                    if (cb.Items[e.Index] is LangObject si)
                    {
                        langListText = si.Name;
                    }
                }

                if (langListText.StartsWith("<GROUP>"))
                {
                    font = new Font(font, FontStyle.Bold);
                    e.Graphics.FillRectangle(cat_customBGColor, e.Bounds);
                    e.Graphics.DrawString(langListText.Replace("<GROUP>", string.Empty), font, cat_customTextColor, e.Bounds);
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
                    e.Graphics.DrawString("    " + langListText, font, textColor, e.Bounds);
                }
            }
            catch { }
        }
    }
}