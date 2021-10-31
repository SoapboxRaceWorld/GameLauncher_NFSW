using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GameLauncher.App.UI_Forms.Welcome_Screen
{
    public partial class WelcomeScreen : Form
    {
        private static bool IsWelcomeScreenOpen = false;
        private bool StatusCheck = false;

        public static void OpenScreen()
        {
            if (IsWelcomeScreenOpen || Application.OpenForms["WelcomeScreen"] != null)
            {
                if (Application.OpenForms["WelcomeScreen"] != null) { Application.OpenForms["WelcomeScreen"].Activate(); }
            }
            else
            {
                try { new WelcomeScreen().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Welcome Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Welcome Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public WelcomeScreen()
        {
            IsWelcomeScreenOpen = true;
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, CloseForm) =>
            {
                IsWelcomeScreenOpen = false;
                GC.Collect();
            };
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Load CDN List                /
            /*******************************/

            Log.Checking("API: Test #3");
            /* Check If Launcher Failed to Connect to any APIs */
            if (!VisualsAPIChecker.CarbonAPITwo())
            {
                MessageBox.Show(null, "Unable to Connect to any CDN List API. Please check your connection." +
                "\n\nCDN Dropdown List will not be available on Welcome Screen",
                "GameLauncher has Paused, Failed To Connect to any CDN List API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Log.Checking("API: Test #3 Done");

            /*******************************/
            /* Load CDN List                /
            /*******************************/

            if (!CDNListUpdater.LoadedList)
            {
                CDNListUpdater.GetList();
            }

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            VersionLabel.Text = "Version: v" + Application.ProductVersion;

            if (UnixOS.Detected())
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

            if (UnixOS.Detected())
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

            Shown += (x, y) =>
            {
                Application.OpenForms["WelcomeScreen"].Activate();
                this.BringToFront();
            };
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
            ListStatusText.Text = "United List - Online";

            if (!VisualsAPIChecker.UnitedAPI())
            {
                ListStatusText.Text = "Carbon List - Online";

                if (!VisualsAPIChecker.CarbonAPI())
                {
                    ListStatusText.Text = "Carbon 2nd List - Online";

                    if (!VisualsAPIChecker.CarbonAPITwo())
                    {
                        ListStatusText.Text = "API Lists Connection - Error";
                        StatusCheck = true;
                    }
                }
            }

            if (StatusCheck)
            {
                WelcomeText.Text = "Looks like the Game Launcher failed to Reach our APIs.\n" +
                    "Clicking 'Manual Bypass' will allow you to continue with the Error";
                APIErrorFormElements();
            }
            else
            {
                APIErrorFormElements(false);
                SettingsFormElements(true);
                WelcomeText.Text = "Howdy!\n" +
                    "Looks like this is the first time this launcher has been started.\n" +
                    "Please select from the options below in order to continue this setup.";
            }
        }

        private void WelcomeScreen_Load(object sender, EventArgs e)
        {
            SettingsFormElements(false);
            APIErrorFormElements(false);
            CheckListStatus();

            /********************************/
            /* Load XML (Only one Section)   /
            /********************************/

            FileGameSettings.Read("Language Only");
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(((LangObject)GameLangSource.SelectedItem).INI_Value))
            {
                FileSettingsSave.Lang = ((LangObject)GameLangSource.SelectedItem).INI_Value;
                FileGameSettingsData.Language = ((LangObject)GameLangSource.SelectedItem).XML_Value;
            }

            try
            {
                if (((LangObject)GameLangSource.SelectedItem).Category == "Custom")
                {
                    /* Create Custom Settings.ini for LangPicker.asi module */
                    if (!Directory.Exists(FileSettingsSave.GameInstallation + "/scripts"))
                    {
                        Directory.CreateDirectory(FileSettingsSave.GameInstallation + "/scripts");
                    }

                    IniFile LanguagePickerFile = new IniFile(FileSettingsSave.GameInstallation + "/scripts/LangPicker.ini");
                    LanguagePickerFile.Write("Language", ((LangObject)GameLangSource.SelectedItem).INI_Value);
                    MessageBox.Show(null, "Please Note: If a Server does not provide a Language Pack, it will fallback to English instead.",
                        "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (File.Exists(FileSettingsSave.GameInstallation + "/scripts/LangPicker.ini"))
                    {
                        File.Delete(FileSettingsSave.GameInstallation + "/scripts/LangPicker.ini");
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WELCOME SCREEN", null, Error, null, true);
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
            WelcomeText.Text = "Howdy!\n" +
                "Looks like this is the first time this launcher has been started.\n" +
                "Please select from the options below in order to continue this setup.";
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
                string cdnListText = string.Empty;

                if (sender is ComboBox cb)
                {
                    if (e.Index != -1 && cb.Items != null)
                    {
                        if (cb.Items[e.Index] is CDNList si)
                        {
                            cdnListText = si.Name;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(cdnListText))
                {
                    Font font = (sender as ComboBox).Font;
                    Brush backgroundColor;
                    Brush textColor;

                    if (cdnListText.StartsWith("<GROUP>"))
                    {
                        font = new Font(font, FontStyle.Bold);
                        e.Graphics.FillRectangle(new SolidBrush(Theming.DropMenuBackgroundForeColor_Category), e.Bounds);
                        e.Graphics.DrawString(cdnListText.Replace("<GROUP>", string.Empty), font,
                            new SolidBrush(Theming.DropMenuTextForeColor_Category), e.Bounds);
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
                            backgroundColor = new SolidBrush(Theming.DropMenuBackgroundForeColor);
                            textColor = new SolidBrush(Theming.DropMenuTextForeColor);
                        }

                        e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                        e.Graphics.DrawString(cdnListText, font, textColor, e.Bounds);
                    }
                }
            }
            catch { }
        }

        private void GameLangSource_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                string langListText = string.Empty;

                if (sender is ComboBox cb)
                {
                    if (e.Index != -1 && cb.Items != null)
                    {
                        if (cb.Items[e.Index] is LangObject si)
                        {
                            langListText = si.Name;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(langListText))
                {
                    Font font = (sender as ComboBox).Font;
                    Brush backgroundColor;
                    Brush textColor;

                    if (langListText.StartsWith("<GROUP>"))
                    {
                        font = new Font(font, FontStyle.Bold);
                        e.Graphics.FillRectangle(new SolidBrush(Theming.DropMenuBackgroundForeColor_Category), e.Bounds);
                        e.Graphics.DrawString(langListText.Replace("<GROUP>", string.Empty), font,
                            new SolidBrush(Theming.DropMenuTextForeColor_Category), e.Bounds);
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
                            backgroundColor = new SolidBrush(Theming.DropMenuBackgroundForeColor);
                            textColor = new SolidBrush(Theming.DropMenuTextForeColor);
                        }

                        e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                        e.Graphics.DrawString("    " + langListText, font, textColor, e.Bounds);
                    }
                }
            }
            catch { }
        }
    }
}