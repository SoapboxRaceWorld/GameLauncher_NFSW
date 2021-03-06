﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFirewallHelper;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using System.Reflection;

namespace GameLauncher.App
{
    public partial class SettingsScreen : Form
    {
        /*******************************/
        /* Global Functions             /
        /*******************************/

        private int _lastSelectedCdnId;
        private int _lastSelectedLanguage;
        private bool _disableProxy;
        private bool _disableDiscordRPC;
        private bool _restartRequired;
        private string _newLauncherPath;
        private string _newGameFilesPath;
        private string FinalCDNURL;
        private readonly bool FirewallEnabled = DetectLinux.LinuxDetected() ? false : FirewallManager.IsServiceRunning;

        public SettingsScreen()
        {
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, y) =>
            {
                DiscordLauncherPresense.Status("Idle Ready", null);
            };

            DiscordLauncherPresense.Status("Settings", null);
        }

        /// <summary>
        /// Sets the Button, Image, Text, and Fonts. Enables/Disables Certain Elements of the Screen for Certain Platforms. Also contains functions that act as helper functions
        /// </summary>
        /// <remarks>Settings Screen Visuals</remarks>
        private void SetVisuals()
        {
            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Text = "Settings - SBRW Launcher: v" + Application.ProductVersion;

            /*******************************/
            /* Set Initial position & Icon  /
            /*******************************/

            FunctionStatus.CenterParent(this);

            /*******************************/
            /* Set Background Image         /
            /*******************************/

            BackgroundImage = Theming.SettingsScreen;
            TransparencyKey = Theming.SettingsScreenTransparencyKey;

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
            Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            ResetFirewallRulesButton.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ResetWindowsDefenderButton.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsAboutButton.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsGamePathText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsGameFiles.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsVFilesButton.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsCDNText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsCDNPick.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            SettingsLanguageText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsLanguage.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            SettingsUEditorButton.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
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

            SettingsUEditorButton.ForeColor = Theming.YellowForeColorButton;
            SettingsUEditorButton.BackColor = Theming.YellowBackColorButton;
            SettingsUEditorButton.FlatAppearance.BorderColor = Theming.YellowBorderColorButton;
            SettingsUEditorButton.FlatAppearance.MouseOverBackColor = Theming.YellowMouseOverBackColorButton;

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

            ResetFirewallRulesButton.ForeColor = Theming.BlueForeColorButton;
            ResetFirewallRulesButton.BackColor = Theming.BlueBackColorButton;
            ResetFirewallRulesButton.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            ResetFirewallRulesButton.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            ResetWindowsDefenderButton.ForeColor = Theming.BlueForeColorButton;
            ResetWindowsDefenderButton.BackColor = Theming.BlueBackColorButton;
            ResetWindowsDefenderButton.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            ResetWindowsDefenderButton.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

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
            SettingsLanguage.DrawItem += new DrawItemEventHandler(SettingsLanguage_DrawItem);

            SettingsSave.MouseEnter += new EventHandler(Greenbutton_hover_MouseEnter);
            SettingsSave.MouseLeave += new EventHandler(Greenbutton_MouseLeave);
            SettingsSave.MouseUp += new MouseEventHandler(Greenbutton_hover_MouseUp);
            SettingsSave.MouseDown += new MouseEventHandler(Greenbutton_click_MouseDown);

            SettingsCancel.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            SettingsCancel.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            SettingsCancel.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            SettingsCancel.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);

            /********************************/
            /* Load XML (Only one Section)   /
            /********************************/

            FileGameSettings.Read("Language Only");

            /********************************/
            /* Sets Red Buttons/Disables     /
            /********************************/

            if (FunctionStatus.IsVerifyHashDisabled)
            {
                SettingsVFilesButton.ForeColor = Theming.RedForeColorButton;
                SettingsVFilesButton.BackColor = Theming.RedBackColorButton;
                SettingsVFilesButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                SettingsVFilesButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            }

            if ((!FirewallHelper.FirewallStatus() && !FirewallEnabled) || 
                (FileSettingsSave.FirewallLauncherStatus != "Excluded" && FileSettingsSave.FirewallGameStatus != "Excluded") || 
                FunctionStatus.IsFirewallResetDisabled || DetectLinux.LinuxDetected())
            {
                FunctionStatus.IsFirewallResetDisabled = true;
                ResetFirewallRulesButton.ForeColor = Theming.RedForeColorButton;
                ResetFirewallRulesButton.BackColor = Theming.RedBackColorButton;
                ResetFirewallRulesButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                ResetFirewallRulesButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            }

            if ((FileSettingsSave.WindowsDefenderStatus != "Excluded") || DetectLinux.LinuxDetected() || 
                FunctionStatus.IsWindowsSecurityResetDisabled || (WindowsProductVersion.CachedWindowsNumber < 10.0) ||
                (!ManagementSearcher.SecurityCenter("AntivirusEnabled") && !ManagementSearcher.SecurityCenter("AntispywareEnabled")))
            {
                FunctionStatus.IsWindowsSecurityResetDisabled = true;
                ResetWindowsDefenderButton.ForeColor = Theming.RedForeColorButton;
                ResetWindowsDefenderButton.BackColor = Theming.RedBackColorButton;
                ResetWindowsDefenderButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                ResetWindowsDefenderButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            }
        }

        /********************************/
        /* Draw Events                   /
        /********************************/

        /// <summary>
        /// Sets the Category for the CDN Drop Down Menu with its set of Colors
        /// </summary>
        /// <remarks>Dropdown Menu Visual</remarks>
        private void SettingsCDNPick_DrawItem(object sender, DrawItemEventArgs e)
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
                    e.Graphics.DrawString("    " + cdnListText, font, textColor, e.Bounds);
                }
            }
            catch { }
        }

        /// <summary>
        /// Sets the Category for the Language Drop Down Menu with its set of Colors
        /// </summary>
        /// <remarks>Dropdown Menu Visual</remarks>
        private void SettingsLanguage_DrawItem(object sender, DrawItemEventArgs e)
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

            /*******************************/
            /* Enable/Disable Visuals       /
            /*******************************/

            if (File.Exists(FileSettingsSave.GameInstallation + "/NFSWO_COMMUNICATION_LOG.txt"))
            {
                SettingsClearCommunicationLogButton.Enabled = true;
            }
            else
            {
                SettingsClearCommunicationLogButton.ForeColor = Theming.RedForeColorButton;
                SettingsClearCommunicationLogButton.BackColor = Theming.RedBackColorButton;
                SettingsClearCommunicationLogButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                SettingsClearCommunicationLogButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            }

            if (Directory.Exists(FileSettingsSave.GameInstallation + "/.data"))
            {
                SettingsClearServerModCacheButton.Enabled = true;
            }
            else
            {
                SettingsClearServerModCacheButton.ForeColor = Theming.RedForeColorButton;
                SettingsClearServerModCacheButton.BackColor = Theming.RedBackColorButton;
                SettingsClearServerModCacheButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                SettingsClearServerModCacheButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            }

            var crashLogFilesDirectory = new DirectoryInfo(FileSettingsSave.GameInstallation);

            if (crashLogFilesDirectory.EnumerateFiles("SBRCrashDump_CL0*.dmp").Count() != 0)
            {
                SettingsClearCrashLogsButton.Enabled = true;
            }
            else
            {
                SettingsClearCrashLogsButton.ForeColor = Theming.RedForeColorButton;
                SettingsClearCrashLogsButton.BackColor = Theming.RedBackColorButton;
                SettingsClearCrashLogsButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                SettingsClearCrashLogsButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            }

            try
            {
                Log.Info("SETTINGS VERIFYHASH: Checking Characters in URL");
                if (FileSettingsSave.CDN.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    FinalCDNURL = FileSettingsSave.CDN.TrimEnd(charsToTrim);
                    Log.Info("SETTINGS VERIFYHASH: Trimed end of URL -> " + FinalCDNURL);
                }
                else
                {
                    FinalCDNURL = FileSettingsSave.CDN;
                }
            }
            catch (Exception Error)
            {
                FinalCDNURL = FileSettingsSave.CDN;
                Log.Error("SETTINGS CDN URL TRIM: " + Error.Message);
            }

            try
            {
                if (EnableInsiderDeveloper.Allowed())
                {
                    FunctionStatus.DoesCDNSupportVerifyHash = true;
                }
                else
                {
                    switch (APIChecker.CheckStatus(FinalCDNURL + "/unpacked/checksums.dat"))
                    {
                        case APIStatus.Online:
                            FunctionStatus.DoesCDNSupportVerifyHash = true;
                            break;
                        default:
                            FunctionStatus.DoesCDNSupportVerifyHash = false;
                            break;
                    }
                }
            }
            catch (Exception Error)
            {
                Log.Error("SETTINGS VERIFYHASH: " + Error.Message);
            }

            /********************************/
            /* CDN, APIs, & Restore Last CDN /
            /********************************/

            VisualsAPIChecker.PingAPIStatus("CDN List", "Settings");

            SettingsCDNPick.DisplayMember = "Name";
            SettingsCDNPick.DataSource = CDNListUpdater.CleanList;

            SettingsLanguage.DisplayMember = "Name";
            SettingsLanguage.DataSource = LanguageListUpdater.CleanList;

            RememberLastCDN();
            RememberLastLanguage();
            IsCDNDownGame();
            PingAPIStatus();
        }

        /*******************************/
        /* On Button/Dropdown Functions /
        /*******************************/

        /* Settings Save */
        private void SettingsSave_Click(object sender, EventArgs e)
        {
            /* TODO null check */
            if (!string.IsNullOrWhiteSpace(((LangObject)SettingsLanguage.SelectedItem).INI_Value))
            {
                FileSettingsSave.Lang = ((LangObject)SettingsLanguage.SelectedItem).INI_Value;
                FileGameSettingsData.Language = ((LangObject)SettingsLanguage.SelectedItem).XML_Value;
            }

            /* TODO: Inform player about custom languagepack used. */
            if (((LangObject)SettingsLanguage.SelectedItem).Category == "Custom") 
            {
                MessageBox.Show(null, "Please Note: If a Server does not Provide Language Pack, it will Fallback to English Language Pack instead.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (WindowsProductVersion.CachedWindowsNumber >= 10.0 && (FileSettingsSave.GameInstallation != _newGameFilesPath) && !DetectLinux.LinuxDetected())
            {
                WindowsDefenderGameFilesDirctoryChange();
            }
            else if (FileSettingsSave.GameInstallation != _newGameFilesPath)
            {
                if (!DetectLinux.LinuxDetected())
                {
                    CheckGameFilesDirectoryPrevention();

                    string GameName = "SBRW - Game";

                    /* Remove current Firewall for the Game Files and Add new one (If Possible) */
                    string OldGamePath = Path.Combine(FileSettingsSave.GameInstallation + "\\nfsw.exe");
                    string NewGamePath = Path.Combine(_newGameFilesPath + "\\nfsw.exe");

                    string groupKeyGame = "Need for Speed: World";
                    string descriptionGame = groupKeyGame;

                    if (File.Exists(OldGamePath) && FirewallHelper.FirewallStatus() == true)
                    {
                        /* Inbound & Outbound */
                        FirewallHelper.DoesRulesExist("Reset", "Path", GameName, OldGamePath, groupKeyGame, descriptionGame, FirewallProtocol.Any);

                        if (File.Exists(NewGamePath))
                        {
                            FirewallHelper.DoesRulesExist("Add-Game", "Path", GameName, NewGamePath, groupKeyGame, descriptionGame, FirewallProtocol.Any);
                        }
                    }
                }

                FileSettingsSave.GameInstallation = _newGameFilesPath;

                /* Clean Mods Files from New Dirctory (If it has .links in directory) */
                var linksPath = Path.Combine(_newGameFilesPath, "\\.links");
                ModNetHandler.CleanLinks(linksPath);

                _restartRequired = true;
            }

            if (!string.IsNullOrWhiteSpace(((CDNList)SettingsCDNPick.SelectedItem).Url))
            {
                string SelectedCDNFromList = ((CDNList)SettingsCDNPick.SelectedItem).Url;
                string FinalCDNURL;

                if (SelectedCDNFromList.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    FinalCDNURL = SelectedCDNFromList.TrimEnd(charsToTrim);
                }
                else
                {
                    FinalCDNURL = ((CDNList)SettingsCDNPick.SelectedItem).Url;
                }

                if (FileSettingsSave.CDN != FinalCDNURL)
                {
                    SettingsCDNCurrentText.Text = "CHANGED CDN";
                    SettingsCDNCurrent.Text = FinalCDNURL;
                    FileSettingsSave.CDN = FinalCDNURL;
                    _restartRequired = true;
                }
            }
            else
            {
                Log.Error("SETTINGS: Selected CDN does not contain a URL, Unable to Save Contents");
            }

            String disableProxy = (SettingsProxyCheckbox.Checked == true) ? "1" : "0";
            if (FileSettingsSave.Proxy != disableProxy)
            {
                FileSettingsSave.Proxy = (SettingsProxyCheckbox.Checked == true) ? "1" : "0";

                if (FileSettingsSave.Proxy == "1")
                {
                    if (ServerProxy.Running())
                    {
                        ServerProxy.Instance.Stop("Settings Screen");
                    }

                    if (FileAccountSave.ChoosenGameServer.StartsWith("https") || InformationCache.ModernAuthSupport)
                    {
                        MessageBox.Show(null, ServerListUpdater.ServerName("Settings") + " requires Proxy to be Enabled." +
                            "\nThe launcher will turn on Proxy even if you have chosen to Disable it", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (!ServerProxy.Running())
                    {
                        ServerProxy.Instance.Start("Settings Screen");
                    }
                }
            }

            String disableRPC = (SettingsDiscordRPCCheckbox.Checked == true) ? "1" : "0";
            if (FileSettingsSave.RPC != disableRPC)
            {
                FileSettingsSave.RPC = (SettingsDiscordRPCCheckbox.Checked == true) ? "1" : "0";

                if (FileSettingsSave.RPC == "1")
                {
                    if (DiscordLauncherPresense.Running())
                    {
                        DiscordLauncherPresense.Stop("Close");
                    }
                }
                else
                {
                    if (!DiscordLauncherPresense.Running())
                    {
                        DiscordLauncherPresense.Start("Start Up", null);
                    }
                }
            }

            /* Actually lets check those 2 files */
            if (File.Exists(FileSettingsSave.GameInstallation + "/profwords") && File.Exists(FileSettingsSave.GameInstallation + "/profwords_dis"))
            {
                File.Delete(FileSettingsSave.GameInstallation + "/profwords_dis");
            }

            /* Delete/Enable profwords filter here */
            try
            {
                if (SettingsWordFilterCheck.Checked)
                {
                    if (File.Exists(FileSettingsSave.GameInstallation + "/profwords")) File.Move(FileSettingsSave.GameInstallation + "/profwords", FileSettingsSave.GameInstallation + "/profwords_dis");
                }
                else
                {
                    if (File.Exists(FileSettingsSave.GameInstallation + "/profwords_dis")) File.Move(FileSettingsSave.GameInstallation + "/profwords_dis", FileSettingsSave.GameInstallation + "/profwords");
                }
            }
            catch (Exception Error)
            {
                Log.Error("SETTINGS SAVE:" + Error.Message);
            }

            /* Create Custom Settings.ini for LangPicker.asi module */
            if (((LangObject)SettingsLanguage.SelectedItem).Category == "Custom") 
            {
                if (!Directory.Exists(FileSettingsSave.GameInstallation + "/scripts")) 
                {
                    try
                    {
                        Directory.CreateDirectory(FileSettingsSave.GameInstallation + "/scripts");
                    }
                    catch {}
                }
                try
                {
                    IniFile LanguagePickerFile = new IniFile(FileSettingsSave.GameInstallation + "/scripts/LangPicker.ini");
                    LanguagePickerFile.Write("Language", ((LangObject)SettingsLanguage.SelectedItem).INI_Value);
                }
                catch {}
            }
            else
            {
                if (File.Exists(FileSettingsSave.GameInstallation + "/scripts/LangPicker.ini")) 
                {
                    try
                    {
                        File.Delete(FileSettingsSave.GameInstallation + "/scripts/LangPicker.ini");
                    }
                    catch{}
                }
            }

            /* Save Settings */
            FileSettingsSave.SaveSettings();
            FileGameSettings.Save("Suppress", "Language Only");

            if (_restartRequired)
            {
                MessageBox.Show(null, "In order to see settings changes, you need to restart launcher manually.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Close();
        }

        /* Settings Cancel */
        private void SettingsCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /* Settings UserSettings XML Editor */
        private void SettingsUEditorButton_Click(object sender, EventArgs e)
        {
            new USXEditor().ShowDialog();
        }

        /* Settings Clear ModNet Cache */
        private void SettingsClearServerModCacheButton_Click(object sender, EventArgs e)
        {
            DialogResult SettingsClearServerModCacheConfirmation = MessageBox.Show(null, "Warning you are about the Delete Server Mods Cache" +
            "\nBy Deleting the Cache, you will have to Redownload the Server Mods Again." +
            "\n\nClick Yes to Delete Mods Cache \nor \nClick No to Keep Mods Cache", "GameLauncher",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (SettingsClearServerModCacheConfirmation == DialogResult.Yes)
            {
                Directory.Delete(FileSettingsSave.GameInstallation + "/.data", true);
                Directory.Delete(FileSettingsSave.GameInstallation + "/MODS", true);
                Log.Warning("LAUNCHER: User Confirmed to Delete Server Mods Cache");
                SettingsClearServerModCacheButton.ForeColor = Theming.RedForeColorButton;
                SettingsClearServerModCacheButton.BackColor = Theming.RedBackColorButton;
                SettingsClearServerModCacheButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                SettingsClearServerModCacheButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
                SettingsClearServerModCacheButton.Enabled = false;
                MessageBox.Show(null, "Deleted Server Mods Cache", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /* Settings Clear Communication Logs */
        private void SettingsClearCommunicationLogButton_Click(object sender, EventArgs e)
        {
            File.Delete(FileSettingsSave.GameInstallation + "/NFSWO_COMMUNICATION_LOG.txt");
            SettingsClearCommunicationLogButton.ForeColor = Theming.RedForeColorButton;
            SettingsClearCommunicationLogButton.BackColor = Theming.RedBackColorButton;
            SettingsClearCommunicationLogButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
            SettingsClearCommunicationLogButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            SettingsClearCommunicationLogButton.Enabled = false;
            MessageBox.Show(null, "Deleted NFSWO Communication Log", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            SettingsClearCrashLogsButton.ForeColor = Theming.RedForeColorButton;
            SettingsClearCrashLogsButton.BackColor = Theming.RedBackColorButton;
            SettingsClearCrashLogsButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
            SettingsClearCrashLogsButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            SettingsClearCrashLogsButton.Enabled = false;
            MessageBox.Show(null, "Deleted Crash Logs", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /* Settings Change Game Files Location */
        private void SettingsGameFiles_Click(object sender, EventArgs e)
        {
            if (!DetectLinux.LinuxDetected())
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

                changeGameFilesPath.Dispose();
            }
            else
            {
                FolderBrowserDialog changeGameFilesPath = new FolderBrowserDialog();

                if (changeGameFilesPath.ShowDialog() == DialogResult.OK)
                {
                    _newGameFilesPath = Path.GetFullPath(changeGameFilesPath.SelectedPath);
                    SettingsGameFilesCurrentText.Text = "NEW DIRECTORY";
                    SettingsGameFilesCurrent.Text = _newGameFilesPath;
                }
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
            new DebugWindow().ShowDialog();
        }

        /* Settings CDN Dropdown Menu Index */
        private void SettingsCDNPick_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((CDNList)SettingsCDNPick.SelectedItem).IsSpecial && ((CDNList)SettingsCDNPick.SelectedItem).Url == null)
            {
                SettingsCDNPick.SelectedIndex = _lastSelectedCdnId;
                return;
            }
            else if (((CDNList)SettingsCDNPick.SelectedItem).Url != null)
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

        private void SettingsLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((LangObject)SettingsLanguage.SelectedItem).IsSpecial) 
            {
                SettingsLanguage.SelectedIndex = _lastSelectedLanguage;
                return;
            }

            _lastSelectedLanguage = SettingsLanguage.SelectedIndex;
        }

        /*******************************/
        /* Callback Functions           /
        /*******************************/

        private void IsChangedCDNDown()
        {
            if (!string.IsNullOrWhiteSpace(((CDNList)SettingsCDNPick.SelectedItem).Url))
            {
                SettingsCDNText.Text = "CDN: PINGING";
                SettingsCDNText.ForeColor = Theming.SecondaryTextForeColor;
                Log.Info("SETTINGS PINGING CHANGED CDN: Checking Changed CDN from Drop Down List");

                switch (APIChecker.CheckStatus(((CDNList)SettingsCDNPick.SelectedItem).Url + "/index.xml"))
                {
                    case APIStatus.Online:
                        SettingsCDNText.Text = "CDN: ONLINE";
                        SettingsCDNText.ForeColor = Theming.Sucess;
                        Log.UrlCall("SETTINGS PINGING CHANGED CDN: " + ((CDNList)SettingsCDNPick.SelectedItem).Url + " Is Online!");
                        break;
                    default:
                        SettingsCDNText.Text = "CDN: OFFLINE";
                        SettingsCDNText.ForeColor = Theming.Error;
                        Log.UrlCall("SETTINGS PINGING CHANGED CDN: " + ((CDNList)SettingsCDNPick.SelectedItem).Url + " Is Offline!");
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
            if (!DetectLinux.LinuxDetected())
            {
                /* Check if New Game! Files is not in Banned Folder Locations */
                CheckGameFilesDirectoryPrevention();

                /* Remove current Exclusion and Add new location for Exclusion */
                FunctionStatus.WindowsDefender("Reset-Game", "Update Reset", FileSettingsSave.GameInstallation, _newGameFilesPath, "Updated Game Files");

                string GameName = "SBRW - Game";

                /* Remove current Firewall for the Game Files and Add new one (If Possible) */
                string OldGamePath = Path.Combine(FileSettingsSave.GameInstallation + "\\nfsw.exe");
                string NewGamePath = Path.Combine(_newGameFilesPath + "\\nfsw.exe");

                string groupKeyGame = "Need for Speed: World";
                string descriptionGame = groupKeyGame;

                if (File.Exists(OldGamePath) && FirewallHelper.FirewallStatus() == true)
                {
                    /* Inbound & Outbound */
                    FirewallHelper.DoesRulesExist("Reset", "Path", GameName, OldGamePath, groupKeyGame, descriptionGame, FirewallProtocol.Any);

                    if (File.Exists(NewGamePath))
                    {
                        FirewallHelper.DoesRulesExist("Add-Game", "Path", GameName, NewGamePath, groupKeyGame, descriptionGame, FirewallProtocol.Any);
                    }
                }
            }

            FileSettingsSave.GameInstallation = _newGameFilesPath;

            /* Clean Mods Files from New Dirctory (If it has .links in directory) */
            var linksPath = Path.Combine(_newGameFilesPath, "\\.links");
            ModNetHandler.CleanLinks(linksPath);

            _restartRequired = true;
        }

        private void CheckGameFilesDirectoryPrevention()
        {
            if (!DetectLinux.LinuxDetected())
            {
                switch (FunctionStatus.CheckFolder(_newGameFilesPath))
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

        /* DavidCarbon */
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

            try
            {
                if (!string.IsNullOrWhiteSpace(FileSettingsSave.CDN))
                {
                    string FinalCDNURL;

                    if (FileSettingsSave.CDN.EndsWith("/"))
                    {
                        char[] charsToTrim = { '/' };
                        FinalCDNURL = FileSettingsSave.CDN.TrimEnd(charsToTrim);
                    }
                    else
                    {
                        FinalCDNURL = FileSettingsSave.CDN;
                    }

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
                }
                else
                {
                    SettingsCDNPick.SelectedIndex = 1;
                }
            }
            catch (Exception Error)
            {
                Log.Error("SETTINGS CDNLIST: " + Error.Message);
            }
        }

        /* This is for Main API which still includes a trailing slash - DavidCarbon */
        private void RememberLastCDNOldStandard()
        {
            /* Last Selected CDN */

            try
            {
                if (!string.IsNullOrWhiteSpace(FileSettingsSave.CDN))
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
                else
                {
                    SettingsCDNPick.SelectedIndex = 1;
                }
            }
            catch (Exception Error)
            {
                Log.Error("SETTINGS CDNLIST: " + Error.Message);
            }
        }

        private void RememberLastLanguage()
        {
            /* Last Selected CDN */
            Log.Core("SETTINGS LANGLIST: Checking...");
            Log.Core("SETTINGS LANGLIST: Setting first server in list");
            Log.Core("SETTINGS LANGLIST: Checking if server is set on INI File");

            try
            {
                if (!string.IsNullOrWhiteSpace(FileSettingsSave.Lang))
                {
                    string SavedLang = FileSettingsSave.Lang.Trim();

                    Log.Core("SETTINGS LANGLIST: Found something!");
                    Log.Core("SETTINGS LANGLIST: Checking if language exists on our database");

                    if (LanguageListUpdater.CleanList.FindIndex(i => string.Equals(i.INI_Value, SavedLang)) != 0)
                    {
                        Log.Core("SETTINGS LANGLIST: Language found! Checking its Value");
                        var index = LanguageListUpdater.CleanList.FindIndex(i => string.Equals(i.INI_Value, SavedLang));

                        Log.Core("SETTINGS LANGLIST: ID is " + index);
                        if (index >= 0)
                        {
                            Log.Core("SETTINGS LANGLIST: ID set correctly");
                            SettingsLanguage.SelectedIndex = index;
                        }
                        else if (index < 0)
                        {
                            SettingsLanguage.SelectedIndex = 1;
                        }
                    }
                    else
                    {
                        Log.Warning("SETTINGS LANGLIST: Unable to find anything, assuming default");
                        SettingsLanguage.SelectedIndex = 1;
                        Log.Warning("SETTINGS LANGLIST: Unknown entry value is " + SavedLang);
                    }
                    Log.Core("SETTINGS LANGLIST: All done");
                }
                else
                {
                    Log.Warning("SETTINGS LANGLIST: Unable to find anything, assuming default");
                    SettingsLanguage.SelectedIndex = 1;
                }
            }
            catch (Exception Error)
            {
                Log.Error("SETTINGS LANGLIST: " + Error.Message);
            }
        }

        /* CDN Display Playing Game! - DavidCarbon */
        private async void IsCDNDownGame()
        {
            if (!string.IsNullOrWhiteSpace(FileSettingsSave.CDN))
            {
                SettingsCDNCurrent.LinkColor = Theming.SecondaryTextForeColor;
                Log.Info("SETTINGS PINGING CDN: Checking Current CDN from Settings.ini");
                await Task.Delay(500);

                switch (APIChecker.CheckStatus(FileSettingsSave.CDN + "/index.xml"))
                {
                    case APIStatus.Online:
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

        private void ResetFirewallRulesButton_Click(object sender, EventArgs e)
        {
            if (FunctionStatus.IsFirewallResetDisabled)
            {
                if (!FirewallHelper.RuleExist("Path", "SBRW - Game", FileSettingsSave.GameInstallation + "\\nfsw.exe") ||
                    !FirewallHelper.RuleExist("Path", "SBRW - Game Launcher", Assembly.GetEntryAssembly().Location))
                {
                    if (DetectLinux.LinuxDetected())
                    {
                        MessageBox.Show(null, "Firewall is Not Supported on Non-Windows Systems", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (!FirewallHelper.FirewallStatus() || !FirewallEnabled)
                    {
                        MessageBox.Show(null, "Firewall Service is Not Running. Please Either Enable or Exclude it Manually", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(null, "You have already Reset Firewall Rules", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (DetectLinux.LinuxDetected())
                {
                    MessageBox.Show(null, "Firewall is Not Supported on Non-Windows Systems", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (!FirewallHelper.FirewallStatus() || !FirewallEnabled)
                {
                    MessageBox.Show(null, "Firewall Service is Not Running. Please Either Enable or Exclude it Manually", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(null, "Hello Developers! How do you do Today? Wait, You're not a Developer? Oh oh...", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ResetFirewallRulesButton.ForeColor = Theming.RedForeColorButton;
                ResetFirewallRulesButton.BackColor = Theming.RedBackColorButton;
                ResetFirewallRulesButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                ResetFirewallRulesButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            }
            else
            {
                DialogResult frameworkError = MessageBox.Show(null, "This will Reset the Firewall Rules that were done for you!" +
                "\n\nClicking Yes and Launcher will Re-create the values Again on Next Launch." +
                "\nClick No and Launcher will Not do any changes", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (frameworkError == DialogResult.Yes)
                {
                    if (!DetectLinux.LinuxDetected())
                    {
                        string GameName = "SBRW - Game";
                        string GamePath = FileSettingsSave.GameInstallation + "\\nfsw.exe";

                        string groupKeyGame = "Need for Speed: World";
                        string descriptionGame = groupKeyGame;

                        /* Inbound & Outbound */
                        FirewallHelper.DoesRulesExist("Reset", "Path", GameName, GamePath, groupKeyGame, descriptionGame, FirewallProtocol.Any);

                        string LauncherName = "SBRW - Game Launcher";
                        string LauncherPath = Assembly.GetEntryAssembly().Location;

                        string UpdaterName = "SBRW - Game Launcher Updater";
                        string UpdaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "GameLauncherUpdater.exe");

                        string groupKeyLauncher = "Game Launcher for Windows";
                        string descriptionLauncher = "Soapbox Race World";

                        /* Inbound & Outbound */
                        FirewallHelper.DoesRulesExist("Reset", "Path", LauncherName, LauncherPath, groupKeyLauncher, descriptionLauncher, FirewallProtocol.Any);
                        FirewallHelper.DoesRulesExist("Reset", "Path", UpdaterName, UpdaterPath, groupKeyLauncher, descriptionLauncher, FirewallProtocol.Any);
                        FunctionStatus.IsFirewallResetDisabled = true;
                        FileSettingsSave.FirewallGameStatus = FileSettingsSave.FirewallLauncherStatus = "Not Excluded";
                    }
                }
            }
        }

        private void ResetWindowsDefenderButton_Click(object sender, EventArgs e)
        {
            if (FunctionStatus.IsWindowsSecurityResetDisabled)
            {
                if (DetectLinux.LinuxDetected())
                {
                    MessageBox.Show(null, "Windows Security (Defender) is Not Supported on Non-Windows Systems", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (WindowsProductVersion.CachedWindowsNumber < 10.0)
                {
                    MessageBox.Show(null, "Windows Security (Defender) does not Exist Before Windows 10." +
                        "\nUnsupported Feature on Current Platform, Sorry.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (!ManagementSearcher.SecurityCenter("AntivirusEnabled") && !ManagementSearcher.SecurityCenter("AntispywareEnabled"))
                {
                    MessageBox.Show(null, "Windows Security (Defender) is Disabled or Turned Off." +
                        "\nThis is not recommended, instead opt to turn it ON, to allow Launcher to create Exclusions", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if ((ManagementSearcher.SecurityCenter("AntivirusEnabled") && ManagementSearcher.SecurityCenter("AntispywareEnabled")) && 
                    FunctionStatus.IsWindowsSecurityResetDisabled)
                {
                    MessageBox.Show(null, "You have already Reset Windows Security (Defender) Exclusions", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(null, "It is Possible that Windows Defender is not Your Default AntiVirus Provider." +
                        "\nPlease Manually Exclude it with the other AntiVirus Program", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ResetWindowsDefenderButton.ForeColor = Theming.RedForeColorButton;
                ResetWindowsDefenderButton.BackColor = Theming.RedBackColorButton;
                ResetWindowsDefenderButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                ResetWindowsDefenderButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            }
            else
            {
                DialogResult frameworkError = MessageBox.Show(null, "This will Reset the Windows Security (Defender) Exclusions that were done for you!" +
                "\n\nClicking Yes and Launcher will Re-create the values Again on Next Launch." +
                "\nClick No and Launcher will Not do any changes", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (frameworkError == DialogResult.Yes)
                {
                    if (!DetectLinux.LinuxDetected())
                    {
                        if (WindowsProductVersion.CachedWindowsNumber >= 10.0)
                        {
                            FunctionStatus.WindowsDefender("Reset-Launcher", "Complete Reset", AppDomain.CurrentDomain.BaseDirectory, FileSettingsSave.GameInstallation, "Removed Game and Launcher");
                            FunctionStatus.IsWindowsSecurityResetDisabled = true;
                        }
                        else
                        {
                            Log.Warning("SETTINGS: A Non-Windows 10 User has Mangaed to Enter this Sector!");
                        }
                    }
                }
            }
        }

        /* Settings Verify Hash */
        private void SettingsVFilesButton_Click(object sender, EventArgs e)
        {
            if (FunctionStatus.IsVerifyHashDisabled)
            {
                MessageBox.Show(null, "You have already did a Verify Game Files Scan" +
                    "\nPlease Restart Launcher to do a new Verify Game Files Scan", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                SettingsVFilesButton.ForeColor = Theming.RedForeColorButton;
                SettingsVFilesButton.BackColor = Theming.RedBackColorButton;
                SettingsVFilesButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                SettingsVFilesButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
            }
            else if (FunctionStatus.DoesCDNSupportVerifyHash == false)
            {
                MessageBox.Show(null, "The current saved CDN does not support Verify Game Files Scan" +
                    "\nPlease Choose Another CDN from the list", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                new VerifyHash().ShowDialog();
            }
        }
    }
}