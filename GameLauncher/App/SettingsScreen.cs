using GameLauncher.App.Classes;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.Resources;
using GameLauncherReborn;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WindowsFirewallHelper;

namespace GameLauncher.App
{
    public partial class SettingsScreen : Form
    {
        /*******************************/
        /* Global Functions             /
        /*******************************/

        public string _userSettings = Environment.GetEnvironmentVariable("AppData") + "/Need for Speed World/Settings/UserSettings.xml";

        private int _lastSelectedCdnId;
        private bool _disableProxy;
        private bool _disableDiscordRPC;
        private bool _restartRequired;
        private string _newLauncherPath;
        private string _newGameFilesPath;

        public string ServerIP = String.Empty;
        public string ServerName = String.Empty;

        public SettingsScreen(string serverIP, string serverName)
        {
            ServerIP = serverIP;
            ServerName = serverName;

            InitializeComponent();
            SetVisuals();
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Initial position & Icon  /
            /*******************************/

            this.StartPosition = FormStartPosition.CenterParent;

            /*******************************/
            /* Set Background Image         /
            /*******************************/

            BackgroundImage = Theming.SettingsScreen;

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            SettingsCDNCurrent.Text = FileSettingsSave.CDN;
            SettingsGameFilesCurrent.Text = FileSettingsSave.GameInstallation;
            SettingsLauncherPathCurrent.Text = AppDomain.CurrentDomain.BaseDirectory;
            SettingsLauncherVersion.Text = "Version: v" + Application.ProductVersion;

            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            var MainFontSize = 9f * 100f / CreateGraphics().DpiY;
            var SecondaryFontSize = 8f * 100f / CreateGraphics().DpiY;

            if (DetectLinux.LinuxDetected())
            {
                MainFontSize = 9f;
                SecondaryFontSize = 8f;
            }

            SettingsAboutButton.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsGamePathText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsGameFiles.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsCDNText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsCDNPick.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            SettingsLanguageText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsLanguage.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            SettingsClearCrashLogsButton.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            SettingsClearCommunicationLogButton.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            SettingsClearServerModCacheButton.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            SettingsWordFilterCheck.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsProxyCheckbox.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsDiscordRPCCheckbox.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsGameFilesCurrentText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsGameFilesCurrent.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsCDNCurrentText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsCDNCurrent.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsLauncherPathText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsLauncherPathCurrent.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsNetworkText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsMainSrvText.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsMainCDNText.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsBkupSrvText.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsBkupCDNText.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsVFilesButton.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsLauncherVersion.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsSave.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsCancel.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ThemeName.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ThemeAuthor.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            /* Buttons */
            SettingsGameFiles.ForeColor = Theming.BlueForeColorButton;
            SettingsGameFiles.BackColor = Theming.BlueBackColorButton;
            SettingsGameFiles.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            SettingsGameFiles.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            SettingsAboutButton.ForeColor = Theming.BlueForeColorButton;
            SettingsAboutButton.BackColor = Theming.BlueBackColorButton;
            SettingsAboutButton.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            SettingsAboutButton.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            SettingsVFilesButton.ForeColor = Theming.YellowForeColorButton;
            SettingsVFilesButton.BackColor = Theming.YellowBackColorButton;
            SettingsVFilesButton.FlatAppearance.BorderColor = Theming.YellowBorderColorButton;
            SettingsVFilesButton.FlatAppearance.MouseOverBackColor = Theming.YellowMouseOverBackColorButton;

            SettingsClearCrashLogsButton.ForeColor = Theming.BlueForeColorButton;
            SettingsClearCrashLogsButton.BackColor = Theming.BlueBackColorButton;
            SettingsClearCrashLogsButton.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            SettingsClearCrashLogsButton.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            SettingsClearCommunicationLogButton.ForeColor = Theming.BlueForeColorButton;
            SettingsClearCommunicationLogButton.BackColor = Theming.BlueBackColorButton;
            SettingsClearCommunicationLogButton.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            SettingsClearCommunicationLogButton.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            SettingsClearServerModCacheButton.ForeColor = Theming.BlueForeColorButton;
            SettingsClearServerModCacheButton.BackColor = Theming.BlueBackColorButton;
            SettingsClearServerModCacheButton.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            SettingsClearServerModCacheButton.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            /* Label Links */
            SettingsGameFilesCurrent.LinkColor = Theming.SettingsLink;
            SettingsGameFilesCurrent.ActiveLinkColor = Theming.SettingsActiveLink;
            SettingsCDNCurrent.LinkColor = Theming.SettingsLink;
            SettingsCDNCurrent.ActiveLinkColor = Theming.SettingsActiveLink;
            SettingsLauncherPathCurrent.LinkColor = Theming.SettingsLink;
            SettingsLauncherPathCurrent.ActiveLinkColor = Theming.SettingsActiveLink;

            /* Labels */
            SettingsGamePathText.ForeColor = Theming.FivithTextForeColor;
            SettingsGameFilesCurrentText.ForeColor = Theming.FivithTextForeColor;
            SettingsCDNCurrentText.ForeColor = Theming.FivithTextForeColor;
            SettingsLauncherPathText.ForeColor = Theming.FivithTextForeColor;
            SettingsCDNText.ForeColor = Theming.FivithTextForeColor;
            SettingsLanguageText.ForeColor = Theming.FivithTextForeColor;
            SettingsNetworkText.ForeColor = Theming.FivithTextForeColor;

            /* Check boxes */
            SettingsWordFilterCheck.ForeColor = Theming.SettingsCheckBoxes;
            SettingsProxyCheckbox.ForeColor = Theming.SettingsCheckBoxes;
            SettingsDiscordRPCCheckbox.ForeColor = Theming.SettingsCheckBoxes;

            /* Bottom Left */
            SettingsLauncherVersion.ForeColor = Theming.FivithTextForeColor;
            ThemeName.ForeColor = Theming.FivithTextForeColor;
            ThemeAuthor.ForeColor = Theming.FivithTextForeColor;

            /* Main Settings Buttons (Save or Cancel) */
            SettingsSave.ForeColor = Theming.SeventhTextForeColor;
            SettingsSave.Image = Theming.GreenButton;
            SettingsCancel.Image = Theming.GrayButton;
            SettingsCancel.ForeColor = Theming.MainTextForeColor;

            /********************************/
            /* Events                        /
            /********************************/

            SettingsCDNPick.DrawItem += new DrawItemEventHandler(SettingsCDNPick_DrawItem);

            SettingsSave.MouseEnter += new EventHandler(Greenbutton_hover_MouseEnter);
            SettingsSave.MouseLeave += new EventHandler(Greenbutton_MouseLeave);
            SettingsSave.MouseUp += new MouseEventHandler(Greenbutton_hover_MouseUp);
            SettingsSave.MouseDown += new MouseEventHandler(Greenbutton_click_MouseDown);

            SettingsCancel.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            SettingsCancel.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            SettingsCancel.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            SettingsCancel.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);
        }

        /********************************/
        /* Draw Events                   /
        /********************************/

        private void SettingsCDNPick_DrawItem(object sender, DrawItemEventArgs e)
        {
            var font = (sender as ComboBox).Font;
            Brush backgroundColor;
            Brush textColor;
            Brush customTextColor = new SolidBrush(Theming.CDNMenuTextForeColor);
            Brush customBGColor = new SolidBrush(Theming.CDNMenuBGForeColor);

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

        private void SettingsScreen_Load(object sender, EventArgs e)
        {
            /*******************************/
            /* Read Settings.ini            /
            /*******************************/

            _disableProxy = (FileSettingsSave.Proxy == "1");
            _disableDiscordRPC = (FileSettingsSave.RPC == "1");

            if (File.Exists(FileSettingsSave.GameInstallation + "/profwords") || File.Exists(FileSettingsSave.GameInstallation + "/profwords_dis"))
            {
                try
                {
                    SettingsWordFilterCheck.Checked = !File.Exists(FileSettingsSave.GameInstallation + "/profwords");
                }
                catch
                {
                    SettingsWordFilterCheck.Checked = false;
                }
            }
            else
            {
                SettingsWordFilterCheck.Enabled = false;
            }

            if (File.Exists("Theme.ini"))
            {
                ThemeName.Text = "Theme Name: " + Theming.ThemeName;
                ThemeAuthor.Text = "Theme Author: " + Theming.ThemeAuthor;
            }
            else
            {
                ThemeName.Text = "Theme Name: Default";
                ThemeAuthor.Text = "Theme Author: Launcher - Division";
            }

            /*******************************/
            /* Folder Locations             /
            /*******************************/

            _newGameFilesPath = Path.GetFullPath(FileSettingsSave.GameInstallation);
            _newLauncherPath = AppDomain.CurrentDomain.BaseDirectory;

            SettingsProxyCheckbox.Checked = _disableProxy;
            SettingsDiscordRPCCheckbox.Checked = _disableDiscordRPC;

            SettingsLanguage.DisplayMember = "Text";
            SettingsLanguage.ValueMember = "Value";

            var languages = new[]
            {
                new { Text = "English", Value = "EN" },
                new { Text = "Deutsch", Value = "DE" },
                new { Text = "Español", Value = "ES" },
                new { Text = "Français", Value = "FR" },
                new { Text = "Polski", Value = "PL" },
                new { Text = "Русский", Value = "RU" },
                new { Text = "Português (Brasil)", Value = "PT" },
                new { Text = "繁體中文", Value = "TC" },
                new { Text = "简体中文", Value = "SC" },
                new { Text = "ภาษาไทย", Value = "TH" },
                new { Text = "Türkçe", Value = "TR" },
            };

            SettingsLanguage.DataSource = languages;

            if (!string.IsNullOrEmpty(FileSettingsSave.Lang))
            {
                SettingsLanguage.SelectedValue = FileSettingsSave.Lang;
            }

            /*******************************/
            /* Enable/Disable Visuals       /
            /*******************************/

            if (File.Exists(FileSettingsSave.GameInstallation + "/NFSWO_COMMUNICATION_LOG.txt"))
            {
                SettingsClearCommunicationLogButton.Enabled = true;
            }

            if (Directory.Exists(FileSettingsSave.GameInstallation + "/.data"))
            {
                SettingsClearServerModCacheButton.Enabled = true;
            }

            var crashLogFilesDirectory = new DirectoryInfo(FileSettingsSave.GameInstallation);

            if (crashLogFilesDirectory.EnumerateFiles("SBRCrashDump_CL0*.dmp").Count() != 0)
            {
                SettingsClearCrashLogsButton.Enabled = true;
            }

            try
            {
                string SavedCDN = FileSettingsSave.CDN;
                char[] charsToTrim = { '/' };
                string FinalCDNURL = SavedCDN.TrimEnd(charsToTrim);

                if (EnableInsider.ShouldIBeAnInsider() == true)
                {
                    Log.Info("SETTINGS VERIFYHASH: Checking Characters in URL");
                    Log.Info("SETTINGS VERIFYHASH: Trimed end of URL -> " + FinalCDNURL);
                    Theming.ButtonVerifyHash = true;
                }
                else
                {
                    if (Theming.DisableVerifyHash == true)
                    {
                        Theming.ButtonVerifyHash = false;
                    }
                    else
                    {
                        switch (APIStatusChecker.CheckStatus(FinalCDNURL + "/unpacked/checksums.dat"))
                        {
                            case API.Online:
                                Theming.ButtonVerifyHash = true;
                                break;
                            default:
                                Theming.ButtonVerifyHash = false;
                                break;
                        }
                    }
                }

                SettingsVFilesButton.Enabled = Theming.ButtonVerifyHash;
            }
            catch { }

            /********************************/
            /* CDN, APIs, & Restore Last CDN /
            /********************************/

            SettingsCDNPick.DisplayMember = "Name";
            SettingsCDNPick.DataSource = CDNListUpdater.CleanList;

            RememberLastCDN();
            IsCDNDownGame();
            PingAPIStatus();
        }

        /*******************************/
        /* On Button/Dropdown Functions /
        /*******************************/

        /* Settings Save */
        private void SettingsSave_Click(object sender, EventArgs e)
        {
            //TODO null check
            FileSettingsSave.Lang = SettingsLanguage.SelectedValue.ToString();

            if (WindowsProductVersion.GetWindowsNumber() >= 10.0 && (FileSettingsSave.GameInstallation != _newGameFilesPath) && !DetectLinux.LinuxDetected())
            {
                WindowsDefenderGameFilesDirctoryChange();
            }
            else if (FileSettingsSave.GameInstallation != _newGameFilesPath)
            {
                CheckGameFilesDirectoryPrevention();

                if (!DetectLinux.LinuxDetected())
                {
                    //Remove current Firewall for the Game Files 
                    string CurrentGameFilesExePath = Path.Combine(FileSettingsSave.GameInstallation + "\\nfsw.exe");

                    if (File.Exists(CurrentGameFilesExePath) && FirewallHelper.RuleExist("SBRW - Game") == true)
                    {
                        bool removeFirewallRule = true;
                        bool firstTimeRun = true;

                        string nameOfGame = "SBRW - Game";
                        string localOfGame = CurrentGameFilesExePath;

                        string groupKeyGame = "Need for Speed: World";
                        string descriptionGame = groupKeyGame;

                        //Inbound & Outbound
                        FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfGame, localOfGame, groupKeyGame, descriptionGame, FirewallProtocol.Any);
                    }
                }

                FileSettingsSave.GameInstallation = _newGameFilesPath;

                //Clean Mods Files from New Dirctory (If it has .links in directory)
                var linksPath = Path.Combine(_newGameFilesPath, "\\.links");
                ModNetLinksCleanup.CleanLinks(linksPath);

                _restartRequired = true;
            }

            if (FileSettingsSave.CDN != ((CDNObject)SettingsCDNPick.SelectedItem).Url)
            {
                SettingsCDNCurrentText.Text = "CHANGED CDN";
                SettingsCDNCurrent.Text = ((CDNObject)SettingsCDNPick.SelectedItem).Url;
                FileSettingsSave.CDN = ((CDNObject)SettingsCDNPick.SelectedItem).Url;
                _restartRequired = true;
            }

            String disableProxy = (SettingsProxyCheckbox.Checked == true) ? "1" : "0";
            if (FileSettingsSave.Proxy != disableProxy)
            {
                FileSettingsSave.Proxy = (SettingsProxyCheckbox.Checked == true) ? "1" : "0";
                _restartRequired = true;
            }

            String disableRPC = (SettingsDiscordRPCCheckbox.Checked == true) ? "1" : "0";
            if (FileSettingsSave.RPC != disableRPC)
            {
                FileSettingsSave.RPC = (SettingsDiscordRPCCheckbox.Checked == true) ? "1" : "0";
                _restartRequired = true;
            }

            if (_restartRequired)
            {
                MessageBox.Show(null, "In order to see settings changes, you need to restart launcher manually.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //Actually lets check those 2 files
            if (File.Exists(FileSettingsSave.GameInstallation + "/profwords") && File.Exists(FileSettingsSave.GameInstallation + "/profwords_dis"))
            {
                File.Delete(FileSettingsSave.GameInstallation + "/profwords_dis");
            }

            //Delete/Enable profwords filter here
            if (SettingsWordFilterCheck.Checked)
            {
                if (File.Exists(FileSettingsSave.GameInstallation + "/profwords")) File.Move(FileSettingsSave.GameInstallation + "/profwords", FileSettingsSave.GameInstallation + "/profwords_dis");
            }
            else
            {
                if (File.Exists(FileSettingsSave.GameInstallation + "/profwords_dis")) File.Move(FileSettingsSave.GameInstallation + "/profwords_dis", FileSettingsSave.GameInstallation + "/profwords");
            }

            /* Save Settings */
            FileSettingsSave.SaveSettings();

            var userSettingsXml = new XmlDocument();
            try
            {
                if (File.Exists(_userSettings))
                {
                    try
                    {
                        userSettingsXml.Load(_userSettings);
                        var language = userSettingsXml.SelectSingleNode("Settings/UI/Language");
                        language.InnerText = SettingsLanguage.SelectedValue.ToString();
                    }
                    catch
                    {
                        File.Delete(_userSettings);

                        var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
                        var ui = setting.AppendChild(userSettingsXml.CreateElement("UI"));

                        var persistentValue = setting.AppendChild(userSettingsXml.CreateElement("PersistentValue"));
                        var chat = persistentValue.AppendChild(userSettingsXml.CreateElement("Chat"));
                        chat.InnerXml = "<DefaultChatGroup Type=\"string\">" + Self.currentLanguage + "</DefaultChatGroup>";
                        ui.InnerXml = "<Language Type=\"string\">" + SettingsLanguage.SelectedValue + "</Language>";

                        var directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(_userSettings));
                    }
                }
                else
                {
                    try
                    {
                        var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
                        var ui = setting.AppendChild(userSettingsXml.CreateElement("UI"));

                        var persistentValue = setting.AppendChild(userSettingsXml.CreateElement("PersistentValue"));
                        var chat = persistentValue.AppendChild(userSettingsXml.CreateElement("Chat"));
                        chat.InnerXml = "<DefaultChatGroup Type=\"string\">" + Self.currentLanguage + "</DefaultChatGroup>";
                        ui.InnerXml = "<Language Type=\"string\">" + SettingsLanguage.SelectedValue + "</Language>";

                        var directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(_userSettings));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(null, "There was an error saving your settings to actual file. Restoring default.\n" + ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        File.Delete(_userSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, "There was an error saving your settings to actual file. Restoring default.\n" + ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.Delete(_userSettings);
            }

            /* Save XML Settings */
            userSettingsXml.Save(_userSettings);

            //DialogResult = DialogResult.OK;
            Close();
        }

        /* Settings Cancel */
        private void SettingsCancel_Click(object sender, EventArgs e)
        {
            //DialogResult = DialogResult.Cancel;
            Close();
        }

        /* Settings Verify Hash */
        private void SettingsVFilesButton_Click(object sender, EventArgs e)
        {
            new VerifyHash().ShowDialog();
            SettingsVFilesButton.Enabled = false;
        }

        /* Settings Clear ModNet Cache */
        private void SettingsClearServerModCacheButton_Click(object sender, EventArgs e)
        {
            Directory.Delete(FileSettingsSave.GameInstallation + "/.data", true);
            Directory.Delete(FileSettingsSave.GameInstallation + "/MODS", true);
            Log.Warning("LAUNCHER: User Confirmed to Delete Server Mods Cache");
            MessageBox.Show(null, "Deleted Server Mods Cache", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingsClearServerModCacheButton.Enabled = false;
        }

        /* Settings Clear Communication Logs */
        private void SettingsClearCommunicationLogButton_Click(object sender, EventArgs e)
        {
            File.Delete(FileSettingsSave.GameInstallation + "/NFSWO_COMMUNICATION_LOG.txt");
            MessageBox.Show(null, "Deleted NFSWO Communication Log", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingsClearCommunicationLogButton.Enabled = false;
        }

        /* Settings Clear Game Crash Logs */
        private void SettingsClearCrashLogsButton_Click(object sender, EventArgs e)
        {
            var crashLogFilesDirectory = new DirectoryInfo(FileSettingsSave.GameInstallation);

            foreach (var file in crashLogFilesDirectory.EnumerateFiles("SBRCrashDump_CL0*.dmp"))
            {
                file.Delete();
            }

            foreach (var file in crashLogFilesDirectory.EnumerateFiles("SBRCrashDump_CL0*.txt"))
            {
                file.Delete();
            }

            foreach (var file in crashLogFilesDirectory.EnumerateFiles("NFSCrashDump_CL0*.dmp"))
            {
                file.Delete();
            }

            MessageBox.Show(null, "Deleted Crash Logs", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingsClearCrashLogsButton.Enabled = false;
        }

        /* Settings Change Game Files Location */
        private void SettingsGameFiles_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog changeGameFilesPath = new System.Windows.Forms.OpenFileDialog
            {
                InitialDirectory = "C:\\",
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                Title = "Select the location to Find or Download nfsw.exe",
                FileName = "Select Game Files Folder"
            };
            if (changeGameFilesPath.ShowDialog() == DialogResult.OK)
            {
                _newGameFilesPath = Path.GetDirectoryName(changeGameFilesPath.FileName);
                SettingsGameFilesCurrentText.Text = "NEW DIRECTORY";
                SettingsGameFilesCurrent.Text = _newGameFilesPath;
            }
        }

        /* Settings Open Current CDN in Browser */
        private void SettingsCDNCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(FileSettingsSave.CDN);
        }

        /* Settings Open Current Launcher Path in Explorer */
        private void SettingsLauncherPathCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(_newLauncherPath);
        }

        /* Settings Open Current Game Files Path in Explorer */
        private void SettingsGameFilesCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(_newGameFilesPath);
        }

        /* Settings Open About Dialog */
        private void SettingsAboutButton_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void SettingsLauncherVersion_Click(object sender, EventArgs e)
        {
            new DebugWindow(ServerIP, ServerName).ShowDialog();
        }

        /* Settings CDN Dropdown Menu Index */
        private void SettingsCDNPick_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((CDNObject)SettingsCDNPick.SelectedItem).IsSpecial && ((CDNObject)SettingsCDNPick.SelectedItem).Url == null)
            {
                SettingsCDNPick.SelectedIndex = _lastSelectedCdnId;
                return;
            }
            else if (((CDNObject)SettingsCDNPick.SelectedItem).Url != null)
            {
                IsChangedCDNDown();
            }
            else
            {
                SettingsCDNText.Text = "CDN:";
                SettingsCDNText.ForeColor = Theming.FivithTextForeColor;
            }

            _lastSelectedCdnId = SettingsCDNPick.SelectedIndex;
        }

        /*******************************/
        /* Callback Functions           /
        /*******************************/

        private void IsChangedCDNDown()
        {
            if (!string.IsNullOrEmpty(((CDNObject)SettingsCDNPick.SelectedItem).Url))
            {
                SettingsCDNText.Text = "CDN: PINGING";
                SettingsCDNText.ForeColor = Theming.SecondaryTextForeColor;
                Log.Info("SETTINGS PINGING CHANGED CDN: Checking Changed CDN from Drop Down List");

                switch (APIStatusChecker.CheckStatus(((CDNObject)SettingsCDNPick.SelectedItem).Url + "/index.xml"))
                {
                    case API.Online:
                        SettingsCDNText.Text = "CDN: ONLINE";
                        SettingsCDNText.ForeColor = Theming.Sucess;
                        Log.UrlCall("SETTINGS PINGING CHANGED CDN: " + ((CDNObject)SettingsCDNPick.SelectedItem).Url + " Is Online!");
                        break;
                    default:
                        SettingsCDNText.Text = "CDN: OFFLINE";
                        SettingsCDNText.ForeColor = Theming.Error;
                        Log.UrlCall("SETTINGS PINGING CHANGED CDN: " + ((CDNObject)SettingsCDNPick.SelectedItem).Url + " Is Offline!");
                        break;
                }
            }
            else
            {
                Log.Error("SETTINGS PINGING CHANGED CDN: '((CDNObject)SettingsCDNPick.SelectedItem).Url)' has an Empty CDN URL");
            }
        }

        private void WindowsDefenderGameFilesDirctoryChange()
        {
            //Check if New Game! Files is not in Banned Folder Locations
            CheckGameFilesDirectoryPrevention();

            try
            {
                //Remove current Exclusion and Add new location for Exclusion
                using (PowerShell ps = PowerShell.Create())
                {
                    Log.Warning("WINDOWS DEFENDER: Removing OLD Game Files Directory: " + FileSettingsSave.GameInstallation);
                    ps.AddScript($"Remove-MpPreference -ExclusionPath \"{FileSettingsSave.GameInstallation}\"");
                    Log.Core("WINDOWS DEFENDER: Excluding NEW Game Files Directory: " + _newGameFilesPath);
                    ps.AddScript($"Add-MpPreference -ExclusionPath \"{_newGameFilesPath}\"");
                    var result = ps.Invoke();
                }
            }
            catch (Exception ex)
            {
                Log.Error("WINDOWS DEFENDER: " + ex.Message);
            }

            //Remove current Firewall for the Game Files 
            string CurrentGameFilesExePath = Path.Combine(FileSettingsSave.GameInstallation + "\\nfsw.exe");

            if (File.Exists(CurrentGameFilesExePath) && FirewallHelper.RuleExist("SBRW - Game") == true)
            {
                bool removeFirewallRule = true;
                bool firstTimeRun = true;

                string nameOfGame = "SBRW - Game";
                string localOfGame = CurrentGameFilesExePath;

                string groupKeyGame = "Need for Speed: World";
                string descriptionGame = groupKeyGame;

                //Inbound & Outbound
                FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfGame, localOfGame, groupKeyGame, descriptionGame, FirewallProtocol.Any);
            }

            FileSettingsSave.GameInstallation = _newGameFilesPath;

            //Clean Mods Files from New Dirctory (If it has .links in directory)
            var linksPath = Path.Combine(_newGameFilesPath, "\\.links");
            ModNetLinksCleanup.CleanLinks(linksPath);

            _restartRequired = true;
        }

        private void CheckGameFilesDirectoryPrevention()
        {
            if (!DetectLinux.LinuxDetected())
            {
                switch (Self.CheckFolder(_newGameFilesPath))
                {
                    case FolderType.IsSameAsLauncherFolder:
                        Directory.CreateDirectory("Game Files");
                        Log.Error("LAUNCHER: Installing NFSW in same directory where the launcher resides is NOT recommended.");
                        MessageBox.Show(null, string.Format("Installing NFSW in same directory where the launcher resides is not allowed.\nInstead, we will install it at {0}.", AppDomain.CurrentDomain.BaseDirectory + "Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        break;
                    case FolderType.IsTempFolder:
                        Directory.CreateDirectory("Game Files");
                        Log.Error("LAUNCHER: (╯°□°）╯︵ ┻━┻ Installing NFSW in the Temp Folder is NOT allowed!");
                        MessageBox.Show(null, string.Format("(╯°□°）╯︵ ┻━┻\n\nInstalling NFSW in the Temp Folder is NOT allowed!\nInstead, we will install it at {0}.", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files" + "\n\n┬─┬ ノ( ゜-゜ノ)"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        break;
                    case FolderType.IsProgramFilesFolder:
                    case FolderType.IsUsersFolders:
                    case FolderType.IsWindowsFolder:
                        Directory.CreateDirectory("Game Files");
                        Log.Error("LAUNCHER: Installing NFSW in a Special Directory is disadvised.");
                        MessageBox.Show(null, string.Format("Installing NFSW in a Special Directory is not recommended or allowed.\nInstead, we will install it at {0}.", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        break;
                }
                FileSettingsSave.SaveSettings();
            }
        }

        //DavidCarbon
        private void PingAPIStatus()
        {
            if (VisualsAPIChecker.UnitedAPI != false)
            {
                SettingsMainSrvText.Text = "[API] United: ONLINE";
                SettingsMainSrvText.ForeColor = Theming.Sucess;
            }
            else
            {
                SettingsMainSrvText.Text = "[API] United: ERROR";
                SettingsMainSrvText.ForeColor = Theming.Error;
                SettingsMainCDNText.Visible = true;
            }

            if (VisualsAPIChecker.CarbonAPI != false)
            {
                SettingsMainCDNText.Text = "[API] Carbon: ONLINE";
                SettingsMainCDNText.ForeColor = Theming.Sucess;
            }
            else
            {
                SettingsMainCDNText.Text = "[API] Carbon: ERROR";
                SettingsMainCDNText.ForeColor = Theming.Error;
                SettingsBkupSrvText.Visible = true;
            }

            if (VisualsAPIChecker.CarbonAPITwo != false)
            {
                SettingsBkupSrvText.Text = "[API] Carbon (2nd): ONLINE";
                SettingsBkupSrvText.ForeColor = Theming.Sucess;
            }
            else
            {
                SettingsBkupSrvText.Text = "[API] Carbon (2nd): ERROR";
                SettingsBkupSrvText.ForeColor = Theming.Error;
                SettingsBkupCDNText.Visible = true;
            }

            if (VisualsAPIChecker.WOPLAPI != false)
            {
                SettingsBkupCDNText.Text = "[API] WOPL: ONLINE";
                SettingsBkupCDNText.ForeColor = Theming.Sucess;
            }
            else
            {
                SettingsBkupCDNText.Text = "[API] WOPL: ERROR";
                SettingsBkupCDNText.ForeColor = Theming.Error;
            }
        }

        private void RememberLastCDN()
        {
            /* Last Selected CDN */
            Log.Core("SETTINGS CDNLIST: Checking...");
            Log.Core("SETTINGS CDNLIST: Setting first server in list");
            Log.Core("SETTINGS CDNLIST: Checking if server is set on INI File");

            if (!string.IsNullOrEmpty(FileSettingsSave.CDN))
            {
                string SavedCDN = FileSettingsSave.CDN;
                char[] charsToTrim = { '/' };
                string FinalCDNURL = SavedCDN.TrimEnd(charsToTrim);

                Log.Core("SETTINGS CDNLIST: Found something!");
                Log.Core("SETTINGS CDNLIST: Checking if CDN exists on our database");

                if (CDNListUpdater.CleanList.FindIndex(i => string.Equals(i.Url, FinalCDNURL)) != 0)
                {
                    Log.Core("SETTINGS CDNLIST: CDN found! Checking ID");
                    var index = CDNListUpdater.CleanList.FindIndex(i => string.Equals(i.Url, FinalCDNURL));

                    Log.Core("SETTINGS CDNLIST: ID is " + index);
                    if (index >= 0)
                    {
                        Log.Core("SETTINGS CDNLIST: ID set correctly");
                        SettingsCDNPick.SelectedIndex = index;
                    }
                    else if (index < 0)
                    {
                        Log.Warning("SETTINGS CDNLIST: Checking ID Against OLD Standard");
                        RememberLastCDNOldStandard();
                    }
                }
                else
                {
                    Log.Warning("SETTINGS CDNLIST: Unable to find anything, assuming default");
                    SettingsCDNPick.SelectedIndex = 1;
                    Log.Warning("SETTINGS CDNLIST: Unknown entry value is " + FinalCDNURL);
                }
                Log.Core("SETTINGS CDNLIST: All done");
            }
        }

        /* This is for Main API which still includes a trailing slash - DavidCarbon */
        private void RememberLastCDNOldStandard()
        {
            /* Last Selected CDN */

            if (!string.IsNullOrEmpty(FileSettingsSave.CDN))
            {
                string FinalCDNURL = FileSettingsSave.CDN + "/";

                if (CDNListUpdater.CleanList.FindIndex(i => string.Equals(i.Url, FinalCDNURL)) != 0)
                {
                    var index = CDNListUpdater.CleanList.FindIndex(i => string.Equals(i.Url, FinalCDNURL));

                    if (index >= 0)
                    {
                        Log.Warning("SETTINGS CDNLIST: Found ID Based on OLD Standard");
                        SettingsCDNPick.SelectedIndex = index;
                    }
                    else if (index < 0)
                    {
                        Log.Warning("SETTINGS CDNLIST: Failed to Detect Standard!");
                        SettingsCDNPick.SelectedIndex = 1;
                        Log.Warning("SETTINGS CDNLIST: Displaying First CDN in List!");
                    }
                }
                else
                {
                    SettingsCDNPick.SelectedIndex = 1;
                }
            }
        }

        //CDN Display Playing Game! - DavidCarbon
        private async void IsCDNDownGame()
        {
            if (!string.IsNullOrEmpty(FileSettingsSave.CDN))
            {
                SettingsCDNCurrent.LinkColor = Theming.SecondaryTextForeColor;
                Log.Info("SETTINGS PINGING CDN: Checking Current CDN from Settings.ini");
                await Task.Delay(500);

                switch (APIStatusChecker.CheckStatus(FileSettingsSave.CDN + "/index.xml"))
                {
                    case API.Online:
                        SettingsCDNCurrent.LinkColor = Theming.Sucess;
                        Log.UrlCall("SETTINGS PINGING CDN: " + FileSettingsSave.CDN + " Is Online!");
                        break;
                    default:
                        SettingsCDNCurrent.LinkColor = Theming.Error;
                        Log.UrlCall("SETTINGS PINGING CDN: " + FileSettingsSave.CDN + " Is Offline!");
                        break;
                }
            }
            else
            {
                Log.Error("SETTINGS PINGING CDN: Settings.ini has an Empty CDN URL");
            }
        }

        private void Greenbutton_hover_MouseEnter(object sender, EventArgs e)
        {
            SettingsSave.Image = Theming.GreenButtonHover;
        }

        private void Greenbutton_MouseLeave(object sender, EventArgs e)
        {
            SettingsSave.Image = Theming.GreenButton;
        }

        private void Greenbutton_hover_MouseUp(object sender, EventArgs e)
        {
            SettingsSave.Image = Theming.GreenButtonHover;
        }

        private void Greenbutton_click_MouseDown(object sender, EventArgs e)
        {
            SettingsSave.Image = Theming.GreenButtonClick;
        }

        private void Graybutton_click_MouseDown(object sender, EventArgs e)
        {
            SettingsCancel.Image = Theming.GrayButtonClick;
        }

        private void Graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            SettingsCancel.Image = Theming.GrayButtonHover;
        }

        private void Graybutton_MouseLeave(object sender, EventArgs e)
        {
            SettingsCancel.Image = Theming.GrayButton;
        }

        private void Graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            SettingsCancel.Image = Theming.GrayButtonHover;
        }
    }
}
