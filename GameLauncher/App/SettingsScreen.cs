using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsFirewallHelper;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using System.Threading;

namespace GameLauncher.App
{
    public partial class SettingsScreen : Form
    {
        /*******************************/
        /* Global Functions             /
        /*******************************/

        private static bool IsSettingsScreenOpen = false;

        private int _lastSelectedCdnId;
        private int _lastSelectedLanguage;
        private bool _disableProxy;
        private bool _disableDiscordRPC;
        private bool _enableAltWebCalls;
        private bool _restartRequired;
        private string _newLauncherPath;
        private string _newGameFilesPath;
        private string FinalCDNURL;
        private static Thread ThreadChangedCDN;
        private static Thread ThreadSavedCDN;
        private static Thread ThreadChecksums;

        public SettingsScreen()
        {
            if (!IsSettingsScreenOpen)
            {
                IsSettingsScreenOpen = false;
                InitializeComponent();
                SetVisuals();
                this.Closing += (x, y) =>
                {
                    DiscordLauncherPresence.Status("Idle Ready", null);

                    if (ThreadChangedCDN != null)
                    {
                        ThreadChangedCDN.Abort();
                        ThreadChangedCDN = null;
                    }
                    if (ThreadSavedCDN != null)
                    {
                        ThreadSavedCDN.Abort();
                        ThreadSavedCDN = null;
                    }
                    if (ThreadChecksums != null)
                    {
                        ThreadChecksums.Abort();
                        ThreadChecksums = null;
                    }
                    if (IsSettingsScreenOpen) { IsSettingsScreenOpen = false; }
                };

                DiscordLauncherPresence.Status("Settings", null);
            }
        }
        /// <summary>
        /// Sets the Color for Buttons
        /// </summary>
        /// <param name="Elements">Button Control Name</param>
        /// <param name="Color">Range 0-3 Sets Colored Button.
        /// <code>"0" Checking Blue</code><code>"1" Success Green</code><code>"2" Warning Orange</code><code>"3" Error Red</code></param>
        /// <param name="EnabledORDisabled">Enables or Disables the Button</param>
        /// <remarks>Range 0-3 Sets Colored Button.
        /// <code>"0" Checking Blue</code><code>"1" Success Green</code><code>"2" Warning Orange</code><code>"3" Error Red</code></remarks>
        private void ButtonsColorSet(Button Elements, int Color, bool EnabledORDisabled)
        {
            switch (Color)
            {
                /* Checking Blue */
                case 0:
                    if (Elements.InvokeRequired)
                    {
                        Elements.Invoke(new Action(delegate ()
                        {
                            Elements.ForeColor = Theming.BlueForeColorButton;
                            Elements.BackColor = Theming.BlueBackColorButton;
                            Elements.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
                            Elements.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;
                            Elements.Enabled = EnabledORDisabled;
                        }));
                    }
                    else
                    {
                        Elements.ForeColor = Theming.BlueForeColorButton;
                        Elements.BackColor = Theming.BlueBackColorButton;
                        Elements.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
                        Elements.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;
                        Elements.Enabled = EnabledORDisabled;
                    }
                    break;
                /* Success Green */
                case 1:
                    if (Elements.InvokeRequired)
                    {
                        Elements.Invoke(new Action(delegate ()
                        {
                            Elements.ForeColor = Theming.GreenForeColorButton;
                            Elements.BackColor = Theming.GreenBackColorButton;
                            Elements.FlatAppearance.BorderColor = Theming.GreenBorderColorButton;
                            Elements.FlatAppearance.MouseOverBackColor = Theming.GreenMouseOverBackColorButton;
                            Elements.Enabled = EnabledORDisabled;
                        }));
                    }
                    else
                    {
                        Elements.ForeColor = Theming.GreenForeColorButton;
                        Elements.BackColor = Theming.GreenBackColorButton;
                        Elements.FlatAppearance.BorderColor = Theming.GreenBorderColorButton;
                        Elements.FlatAppearance.MouseOverBackColor = Theming.GreenMouseOverBackColorButton;
                        Elements.Enabled = EnabledORDisabled;
                    }
                    break;
                /* Warning Orange */
                case 2:
                    if (Elements.InvokeRequired)
                    {
                        Elements.Invoke(new Action(delegate ()
                        {
                            Elements.ForeColor = Theming.YellowForeColorButton;
                            Elements.BackColor = Theming.YellowBackColorButton;
                            Elements.FlatAppearance.BorderColor = Theming.YellowBorderColorButton;
                            Elements.FlatAppearance.MouseOverBackColor = Theming.YellowMouseOverBackColorButton;
                            Elements.Enabled = EnabledORDisabled;
                        }));
                    }
                    else
                    {
                        Elements.ForeColor = Theming.YellowForeColorButton;
                        Elements.BackColor = Theming.YellowBackColorButton;
                        Elements.FlatAppearance.BorderColor = Theming.YellowBorderColorButton;
                        Elements.FlatAppearance.MouseOverBackColor = Theming.YellowMouseOverBackColorButton;
                        Elements.Enabled = EnabledORDisabled;
                    }
                    break;
                /* Error Red */
                case 3:
                    if (Elements.InvokeRequired)
                    {
                        Elements.Invoke(new Action(delegate ()
                        {
                            Elements.ForeColor = Theming.RedForeColorButton;
                            Elements.BackColor = Theming.RedBackColorButton;
                            Elements.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                            Elements.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
                            Elements.Enabled = EnabledORDisabled;
                        }));
                    }
                    else
                    {
                        Elements.ForeColor = Theming.RedForeColorButton;
                        Elements.BackColor = Theming.RedBackColorButton;
                        Elements.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                        Elements.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
                        Elements.Enabled = EnabledORDisabled;
                    }
                    break;
                /* Unknown Gray */
                default:
                    if (Elements.InvokeRequired)
                    {
                        Elements.Invoke(new Action(delegate ()
                        {
                            Elements.ForeColor = Theming.GrayForeColorButton;
                            Elements.BackColor = Theming.GrayBackColorButton;
                            Elements.FlatAppearance.BorderColor = Theming.GrayBorderColorButton;
                            Elements.FlatAppearance.MouseOverBackColor = Theming.GrayMouseOverBackColorButton;
                            Elements.Enabled = EnabledORDisabled;
                        }));
                    }
                    else
                    {
                        Elements.ForeColor = Theming.GrayForeColorButton;
                        Elements.BackColor = Theming.GrayBackColorButton;
                        Elements.FlatAppearance.BorderColor = Theming.GrayBorderColorButton;
                        Elements.FlatAppearance.MouseOverBackColor = Theming.GrayMouseOverBackColorButton;
                        Elements.Enabled = EnabledORDisabled;
                    }
                    break;
            }
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

            float MainFontSize = UnixOS.Detected()? 9f : 9f * 100f / CreateGraphics().DpiY;
            float SecondaryFontSize = UnixOS.Detected()? 8f : 8f * 100f / CreateGraphics().DpiY;

            Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            ButtonSecurityPanel.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
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
            SettingsClearLauncherLogsButton.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            SettingsClearCommunicationLogButton.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            SettingsClearServerModCacheButton.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            SettingsWordFilterCheck.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsProxyCheckbox.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsDiscordRPCCheckbox.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SettingsAltWebCallsheckbox.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
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
            ButtonsColorSet(SettingsGameFiles, 0, true);
            ButtonsColorSet(SettingsAboutButton, 0, true);
            ButtonsColorSet(SettingsVFilesButton, 0, false);
            ButtonsColorSet(SettingsUEditorButton, 0, true);
            ButtonsColorSet(SettingsClearCrashLogsButton, 0, false);
            ButtonsColorSet(SettingsClearLauncherLogsButton, 0, true);
            ButtonsColorSet(SettingsClearCommunicationLogButton, 0, false);
            ButtonsColorSet(SettingsClearServerModCacheButton, 0, false);
            ButtonsColorSet(ButtonSecurityPanel, 0, true);

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
            SettingsAltWebCallsheckbox.ForeColor = Theming.SettingsCheckBoxes;

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

            ButtonSecurityPanel.Click += new EventHandler(ButtonSecurityPanel_Click);
            SettingsVFilesButton.Click += new EventHandler(SettingsVFilesButton_Click);
            SettingsSave.Click += new EventHandler(SettingsSave_Click);
            SettingsCancel.Click += new EventHandler(SettingsCancel_Click);
            SettingsUEditorButton.Click += new EventHandler(SettingsUEditorButton_Click);
            SettingsClearServerModCacheButton.Click += new EventHandler(SettingsClearServerModCacheButton_Click);
            SettingsClearCommunicationLogButton.Click += new EventHandler(SettingsClearCommunicationLogButton_Click);
            SettingsClearCrashLogsButton.Click += new EventHandler(SettingsClearCrashLogsButton_Click);
            SettingsAboutButton.Click += new EventHandler(SettingsAboutButton_Click);
            SettingsLauncherVersion.Click += new EventHandler(SettingsLauncherVersion_Click);
            SettingsGameFiles.Click += new EventHandler(SettingsGameFiles_Click);
            SettingsClearLauncherLogsButton.Click += new EventHandler(SettingsClearLauncherLogsButton_Click);

            /********************************/
            /* Load XML (Only one Section)   /
            /********************************/

            FileGameSettings.Read("Language Only");

            /********************************/
            /* Sets Red Buttons/Disables     /
            /********************************/

            if (FunctionStatus.IsVerifyHashDisabled)
            {
                ButtonsColorSet(SettingsVFilesButton, 3, true);
            }

            /*******************************/
            /* Load CDN List                /
            /*******************************/

            if (!CDNListUpdater.LoadedList)
            {
                CDNListUpdater.GetList();
            }

            SettingsCDNPick.DisplayMember = "Name";
            SettingsCDNPick.DataSource = CDNListUpdater.CleanList;

            SettingsLanguage.DisplayMember = "Name";
            SettingsLanguage.DataSource = LanguageListUpdater.CleanList;

            Shown += (x, y) =>
            {
                RememberLastCDN();
                RememberLastLanguage();
                PingSavedCDN();
                PingAPIStatus();
            };
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

        private void SettingsScreen_Load(object sender, EventArgs e)
        {
            /*******************************/
            /* Read Settings.ini            /
            /*******************************/

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

            _newGameFilesPath = FileSettingsSave.GameInstallation;
            _newLauncherPath = Locations.LauncherFolder;

            _disableProxy = (FileSettingsSave.Proxy == "1");
            _disableDiscordRPC = (FileSettingsSave.RPC == "1");
            _enableAltWebCalls = (FileSettingsSave.WebCallMethod == "WebClientWithTimeout");

            SettingsProxyCheckbox.Checked = _disableProxy;
            SettingsDiscordRPCCheckbox.Checked = _disableDiscordRPC;
            SettingsAltWebCallsheckbox.Checked = _enableAltWebCalls;

            /*******************************/
            /* Enable/Disable Visuals       /
            /*******************************/

            if (File.Exists(Strings.Encode(Path.Combine(FileSettingsSave.GameInstallation, "NFSWO_COMMUNICATION_LOG.txt"))))
            {
                ButtonsColorSet(SettingsClearCommunicationLogButton, 2, true);
            }
            else
            {
                ButtonsColorSet(SettingsClearCommunicationLogButton, 3, false);
            }

            if (Directory.Exists(FileSettingsSave.GameInstallation + "/.data"))
            {

                ButtonsColorSet(SettingsClearServerModCacheButton, 2, true);
            }
            else
            {
                ButtonsColorSet(SettingsClearServerModCacheButton, 3, false);
            }

            var crashLogFilesDirectory = new DirectoryInfo(FileSettingsSave.GameInstallation);

            if (crashLogFilesDirectory.EnumerateFiles("SBRCrashDump_CL0*.dmp").Count() != 0)
            {
                ButtonsColorSet(SettingsClearCrashLogsButton, 2, true);
            }
            else
            {
                ButtonsColorSet(SettingsClearCrashLogsButton, 3, false);
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
                LogToFileAddons.OpenLog("SETTINGS CDN URL TRIM", null, Error, null, true);
            }

            try
            {
                if (EnableInsiderDeveloper.Allowed())
                {
                    FunctionStatus.DoesCDNSupportVerifyHash = true;
                }
                else
                {
                    if (ThreadChecksums != null)
                    {
                        ThreadChecksums.Abort();
                        ThreadChecksums = null;
                    }

                    ThreadChecksums = new Thread(() =>
                    {
                        if (Application.OpenForms["SettingsScreen"] != null)
                        {
                            if (!Application.OpenForms["SettingsScreen"].Disposing)
                            {
                                ButtonsColorSet(SettingsVFilesButton, 0, false);
                                switch (APIChecker.CheckStatus(FinalCDNURL + "/unpacked/checksums.dat", 10))
                                {
                                    case APIStatus.Online:
                                        FunctionStatus.DoesCDNSupportVerifyHash = true;
                                        ButtonsColorSet(SettingsVFilesButton, (FileSettingsSave.GameIntegrity != "Good" ? 2 : 0), true);
                                        break;
                                    default:
                                        FunctionStatus.DoesCDNSupportVerifyHash = false;
                                        ButtonsColorSet(SettingsVFilesButton, 3, true);
                                        break;
                                }
                            }
                        }
                    });

                    ThreadChecksums.Start();
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("SETTINGS VERIFYHASH", null, Error, null, true);
            }

            /********************************/
            /* CDN, APIs, & Restore Last CDN /
            /********************************/

            /* Check If Launcher Failed to Connect to any APIs */
            if (!VisualsAPIChecker.WOPLAPI())
            {
                MessageBox.Show(null, "Unable to Connect to any CDN List API. Please check your connection." +
                "\nCDN Dropdown List will not be available on Settings Screen",
                "GameLauncher has Paused, Failed To Connect to any CDN List API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /*******************************/
        /* On Button/Dropdown Functions /
        /*******************************/

        /* Settings Save */
        private void SettingsSave_Click(object sender, EventArgs e)
        {
            SettingsSave.Text = "Saving";
            /* TODO null check */
            if (!string.IsNullOrWhiteSpace(((LangObject)SettingsLanguage.SelectedItem).INI_Value))
            {
                FileSettingsSave.Lang = ((LangObject)SettingsLanguage.SelectedItem).INI_Value;
                FileGameSettingsData.Language = ((LangObject)SettingsLanguage.SelectedItem).XML_Value;

                /* TODO: Inform player about custom languagepack used. */
                if (((LangObject)SettingsLanguage.SelectedItem).Category == "Custom")
                {
                    MessageBox.Show(null, "Please Note: If a Server does not provide a Language Pack, it will fallback to English Language Pack instead.",
                        "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            if (WindowsProductVersion.GetWindowsNumber() >= 10.0 && (FileSettingsSave.GameInstallation != _newGameFilesPath) && !UnixOS.Detected())
            {
                WindowsDefenderGameFilesDirctoryChange();
            }
            else if (FileSettingsSave.GameInstallation != _newGameFilesPath)
            {
                if (!UnixOS.Detected())
                {
                    /* Check if New Game! Files is not in Banned Folder Locations */
                    CheckGameFilesDirectoryPrevention();
                    /* Store Old Location for Security Panel to Use Later on */
                    FileSettingsSave.GameInstallationOld = FileSettingsSave.GameInstallation;
                    FileSettingsSave.FirewallGameStatus = "Not Excluded";
                    ButtonsColorSet(ButtonSecurityPanel, 2, true);
                }

                FileSettingsSave.GameInstallation = _newGameFilesPath;

                /* Clean Mods Files from New Dirctory (If it has .links in directory) */
                string LinksPath = Strings.Encode(Path.Combine(_newGameFilesPath, ".links"));
                if (File.Exists(LinksPath))
                {
                    ModNetHandler.CleanLinks(LinksPath, _newGameFilesPath);
                    Log.Completed("CLEANLINKS: Done");
                }

                ButtonsColorSet(SettingsGameFiles, 1, true);
                _restartRequired = true;
            }

            if (!string.IsNullOrWhiteSpace(((CDNList)SettingsCDNPick.SelectedItem).Url))
            {
                string SelectedCDNFromList = ((CDNList)SettingsCDNPick.SelectedItem).Url;
                string LocalFinalCDNURL;

                if (SelectedCDNFromList.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    LocalFinalCDNURL = SelectedCDNFromList.TrimEnd(charsToTrim);
                }
                else
                {
                    LocalFinalCDNURL = ((CDNList)SettingsCDNPick.SelectedItem).Url;
                }

                if (FileSettingsSave.CDN != LocalFinalCDNURL)
                {
                    SettingsCDNCurrentText.Text = "CHANGED CDN";
                    SettingsCDNCurrent.Text = LocalFinalCDNURL;
                    FinalCDNURL = FileSettingsSave.CDN = LocalFinalCDNURL;
                    _restartRequired = true;
                }
            }
            else
            {
                Log.Error("SETTINGS: Selected CDN does not contain a URL, unable to Save Contents");
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

                    if (InformationCache.SelectedServerData.IPAddress.StartsWith("https") ||
                        FileAccountSave.ChoosenGameServer.StartsWith("https") || 
                        InformationCache.ModernAuthSecureChannel)
                    {
                        MessageBox.Show(null, ServerListUpdater.ServerName("Settings") + " requires Proxy to be Enabled." +
                            "\nThe launcher will turn on Proxy, even if you have chosen to Disable it", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    if (DiscordLauncherPresence.Running())
                    {
                        DiscordLauncherPresence.Stop("Close");
                    }
                }
                else
                {
                    if (!DiscordLauncherPresence.Running())
                    {
                        DiscordLauncherPresence.Start("Start Up", null);
                    }
                }
            }

            String enableAltWebCalls = (SettingsAltWebCallsheckbox.Checked == true) ? "WebClientWithTimeout" : "WebClient";
            if (FileSettingsSave.WebCallMethod != enableAltWebCalls)
            {
                FileSettingsSave.WebCallMethod = (SettingsAltWebCallsheckbox.Checked == true) ? "WebClientWithTimeout" : "WebClient";
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
                    if (File.Exists(FileSettingsSave.GameInstallation + "/profwords"))
                    {
                        File.Move(FileSettingsSave.GameInstallation + "/profwords", FileSettingsSave.GameInstallation + "/profwords_dis");
                    }
                }
                else
                {
                    if (File.Exists(FileSettingsSave.GameInstallation + "/profwords_dis"))
                    {
                        File.Move(FileSettingsSave.GameInstallation + "/profwords_dis", FileSettingsSave.GameInstallation + "/profwords");
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("SETTINGS SAVE [Profwords]", null, Error, null, true);
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

            if (ThreadChecksums != null)
            {
                ThreadChecksums.Abort();
                ThreadChecksums = null;
            }

            ThreadChecksums = new Thread(() =>
            {
                if (Application.OpenForms["SettingsScreen"] != null)
                {
                    if (!Application.OpenForms["SettingsScreen"].Disposing)
                    {
                        ButtonsColorSet(SettingsVFilesButton, 0, false);

                        switch (APIChecker.CheckStatus(FinalCDNURL + "/unpacked/checksums.dat", 10))
                        {
                            case APIStatus.Online:
                                FunctionStatus.DoesCDNSupportVerifyHash = true;
                                ButtonsColorSet(SettingsVFilesButton, (FileSettingsSave.GameIntegrity != "Good" ? 2 : 0), true);
                                break;
                            default:
                                FunctionStatus.DoesCDNSupportVerifyHash = false;
                                ButtonsColorSet(SettingsVFilesButton, 3, true);
                                break;
                        }
                    }
                }
            });

            ThreadChecksums.Start();

            if (_restartRequired)
            {
                MessageBox.Show(null, "In order to see settings changes, you need to restart the Launcher manually.", "GameLauncher", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            SettingsSave.Text = "Saved";
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
            DialogResult SettingsClearServerModCacheConfirmation = MessageBox.Show(null, "Warning: you are about the Delete Server Mods Cache" +
            "\nBy Deleting the Cache, you will have to re-download the Server Mods Again." +
            "\n\nClick Yes to Delete Mods Cache \nor \nClick No to Keep Mods Cache", "GameLauncher",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (SettingsClearServerModCacheConfirmation == DialogResult.Yes)
            {
                try
                {
                    if (Directory.Exists(FileSettingsSave.GameInstallation + "/.data"))
                    {
                        Directory.Delete(FileSettingsSave.GameInstallation + "/.data", true);
                    }
                    if (Directory.Exists(FileSettingsSave.GameInstallation + "/MODS"))
                    {
                        Directory.Delete(FileSettingsSave.GameInstallation + "/MODS", true);
                    }
                    Log.Warning("LAUNCHER: User Confirmed to Delete Server Mods Cache");
                    ButtonsColorSet(SettingsClearServerModCacheButton, 1, false);
                    MessageBox.Show(null, "Deleted Server Mods Cache", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception Error)
                {
                    ButtonsColorSet(SettingsClearServerModCacheButton, 3, true);
                    LogToFileAddons.OpenLog("SETTINGS CLEAR", "Unable to Delete Server Mods Cache", Error, "Exclamation", false);
                }
            }
        }

        /* Settings Clear Communication Logs */
        private void SettingsClearCommunicationLogButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(FileSettingsSave.GameInstallation + "/NFSWO_COMMUNICATION_LOG.txt"))
                {
                    File.Delete(FileSettingsSave.GameInstallation + "/NFSWO_COMMUNICATION_LOG.txt");
                }
                ButtonsColorSet(SettingsClearCommunicationLogButton, 1, false);
                MessageBox.Show(null, "Deleted NFSWO Communication Log", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Error)
            {
                ButtonsColorSet(SettingsClearCommunicationLogButton, 3, true);
                LogToFileAddons.OpenLog("SETTINGS CLEAR", "Unable to Delete NFSWO Communication Log", Error, "Exclamation", false);
            }
        }

        /* Settings Clear Game Crash Logs */
        private void SettingsClearCrashLogsButton_Click(object sender, EventArgs e)
        {
            try
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

                ButtonsColorSet(SettingsClearCrashLogsButton, 1, false);
                MessageBox.Show(null, "Deleted Crash Logs", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Error)
            {
                ButtonsColorSet(SettingsClearCrashLogsButton, 3, true);
                LogToFileAddons.OpenLog("SETTINGS CLEAR", "Unable to Delete Crash Logs", Error, "Exclamation", false);
            }
        }

        /* Settings Clear Old Launcher Logs */
        private void SettingsClearLauncherLogsButton_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo InstallationDirectory = new DirectoryInfo(Locations.LogFolder);

                foreach (var Folder in InstallationDirectory.EnumerateDirectories())
                {
                    if (Directory.Exists(Folder.FullName))
                    {
                        if (Folder.FullName != Locations.LogCurrentFolder)
                        {
                            Directory.Delete(Folder.FullName, true);
                        }
                    }
                }

                ButtonsColorSet(SettingsClearLauncherLogsButton, 1, false);
                MessageBox.Show(null, "Deleted Old Launcher Logs", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Error)
            {
                ButtonsColorSet(SettingsClearLauncherLogsButton, 3, true);
                LogToFileAddons.OpenLog("SETTINGS CLEAR", "Unable to Delete Old Launcher Logs", Error, "Exclamation", false);
            }
        }

        /* Settings Change Game Files Location */
        private void SettingsGameFiles_Click(object sender, EventArgs e)
        {
            if (!UnixOS.Detected())
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
                    _newGameFilesPath = Strings.Encode(Path.GetDirectoryName(Strings.Encode(changeGameFilesPath.FileName)));
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
                    _newGameFilesPath = Strings.Encode(Path.GetFullPath(Strings.Encode(changeGameFilesPath.SelectedPath)));
                    SettingsGameFilesCurrentText.Text = "NEW DIRECTORY";
                    SettingsGameFilesCurrent.Text = _newGameFilesPath;
                }
            }
        }

        /* Settings Open Current CDN in Browser */
        private void SettingsCDNCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FileSettingsSave.CDN))
            {
                Process.Start(FileSettingsSave.CDN);
            }
        }

        /* Settings Open Current Launcher Path in Explorer */
        private void SettingsLauncherPathCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_newLauncherPath))
            {
                Process.Start(_newLauncherPath);
            }
        }

        /* Settings Open Current Game Files Path in Explorer */
        private void SettingsGameFilesCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_newGameFilesPath))
            {
                Process.Start(_newGameFilesPath);
            }
        }

        /* Settings Open About Dialog */
        private void SettingsAboutButton_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void SettingsLauncherVersion_Click(object sender, EventArgs e)
        {
            new DebugScreen().ShowDialog();
        }

        /* Settings CDN Dropdown Menu Index */
        private void SettingsCDNPick_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
            catch { }
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
            if (ThreadChangedCDN != null)
            {
                ThreadChangedCDN.Abort();
                ThreadChangedCDN = null;
            }

            if (!string.IsNullOrWhiteSpace(((CDNList)SettingsCDNPick.SelectedItem).Url))
            {
                SettingsCDNText.Text = "CDN: PINGING";
                SettingsCDNText.ForeColor = Theming.SecondaryTextForeColor;
                Log.Info("SETTINGS PINGING CHANGED CDN: Checking Changed CDN from Drop Down List");

                ThreadChangedCDN = new Thread(() =>
                {
                    if (Application.OpenForms["SettingsScreen"] != null)
                    {
                        if (!Application.OpenForms["SettingsScreen"].Disposing)
                        {
                            switch (APIChecker.CheckStatus(((CDNList)SettingsCDNPick.SelectedItem).Url + "/index.xml", 10))
                            {
                                case APIStatus.Online:
                                    if (SettingsCDNText.InvokeRequired)
                                    {
                                        SettingsCDNText.Invoke(new Action(delegate ()
                                        {
                                            SettingsCDNText.Text = "CDN: ONLINE";
                                            SettingsCDNText.ForeColor = Theming.Sucess;
                                        }));
                                    }
                                    else
                                    {
                                        SettingsCDNText.Text = "CDN: ONLINE";
                                        SettingsCDNText.ForeColor = Theming.Sucess;
                                    }
                                    Log.UrlCall("SETTINGS PINGING CHANGED CDN: " + ((CDNList)SettingsCDNPick.SelectedItem).Url + " Is Online!");
                                    break;
                                default:
                                    if (SettingsCDNText.InvokeRequired)
                                    {
                                        SettingsCDNText.Invoke(new Action(delegate ()
                                        {
                                            SettingsCDNText.Text = "CDN: OFFLINE";
                                            SettingsCDNText.ForeColor = Theming.Error;
                                        }));
                                    }
                                    else
                                    {
                                        SettingsCDNText.Text = "CDN: OFFLINE";
                                        SettingsCDNText.ForeColor = Theming.Error;
                                    }
                                    Log.UrlCall("SETTINGS PINGING CHANGED CDN: " + ((CDNList)SettingsCDNPick.SelectedItem).Url + " Is Offline!");
                                    break;
                            }
                        }
                    }
                });

                ThreadChangedCDN.Start();
            }
            else
            {
                Log.Error("SETTINGS PINGING CHANGED CDN: '((CDNObject)SettingsCDNPick.SelectedItem).Url)' has an Empty CDN URL");
            }
        }

        private void WindowsDefenderGameFilesDirctoryChange()
        {
            if (!UnixOS.Detected())
            {
                /* Check if New Game! Files is not in Banned Folder Locations */
                CheckGameFilesDirectoryPrevention();
                /* Store Old Location for Security Panel to Use Later on */
                FileSettingsSave.GameInstallationOld = FileSettingsSave.GameInstallation;
                FileSettingsSave.FirewallGameStatus = "Not Excluded";
                FileSettingsSave.DefenderGameStatus = "Not Excluded";
                ButtonsColorSet(ButtonSecurityPanel, 2, true);
            }

            FileSettingsSave.GameInstallation = _newGameFilesPath;

            /* Clean Mods Files from New Dirctory (If it has .links in directory) */
            string LinksPath = Strings.Encode(Path.Combine(_newGameFilesPath, ".links"));
            if (File.Exists(LinksPath))
            {
                ModNetHandler.CleanLinks(LinksPath, _newGameFilesPath);
                Log.Completed("CLEANLINKS: Done");
            }

            ButtonsColorSet(SettingsGameFiles, 1, true);
            _restartRequired = true;
        }

        private void CheckGameFilesDirectoryPrevention()
        {
            if (!UnixOS.Detected())
            {
                bool FailSafePathCreation = false;
                switch (FunctionStatus.CheckFolder(_newGameFilesPath))
                {
                    case FolderType.IsSameAsLauncherFolder:
                        FailSafePathCreation = true;
                        FileSettingsSave.GameInstallation = Locations.GameFilesFailSafePath;
                        Log.Error("LAUNCHER: Installing NFSW in same location where the GameLauncher resides is NOT allowed.");
                        MessageBox.Show(null, string.Format("Installing NFSW in same location where the GameLauncher resides is NOT allowed." +
                            "\nInstead, we will install it at {0}.", Locations.GameFilesFailSafePath), "GameLauncher", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case FolderType.IsTempFolder:
                        FailSafePathCreation = true;
                        FileSettingsSave.GameInstallation = Locations.GameFilesFailSafePath;
                        Log.Error("LAUNCHER: (╯°□°）╯︵ ┻━┻ Installing NFSW in the Temp Folder is NOT allowed!");
                        MessageBox.Show(null, string.Format("(╯°□°）╯︵ ┻━┻\n\nInstalling NFSW in the Temp Folder is NOT allowed!" +
                            "\nInstead, we will install it at {0}.", Locations.GameFilesFailSafePath + "\n\n┬─┬ ノ( ゜-゜ノ)"), "GameLauncher", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case FolderType.IsProgramFilesFolder:
                    case FolderType.IsUsersFolders:
                    case FolderType.IsWindowsFolder:
                        FailSafePathCreation = true;
                        FileSettingsSave.GameInstallation = Locations.GameFilesFailSafePath;
                        Log.Error("LAUNCHER: Installing NFSW in a Special Directory is disadvised.");
                        MessageBox.Show(null, string.Format("Installing NFSW in a Special Directory is not recommended or allowed." +
                            "\nInstead, we will install it at {0}.", Locations.GameFilesFailSafePath), "GameLauncher", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
                FileSettingsSave.SaveSettings();

                if (FailSafePathCreation)
                {
                    if (!Directory.Exists(Locations.GameFilesFailSafePath))
                    {
                        try
                        {
                            Directory.CreateDirectory(Locations.GameFilesFailSafePath);
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("Launcher", null, Error, null, true);
                        }
                    }
                }
            }
        }

        /* DavidCarbon */
        private void PingAPIStatus()
        {
            if (VisualsAPIChecker.UnitedAPI())
            {
                SettingsMainSrvText.Text = "[API] United: ONLINE";
                SettingsMainSrvText.ForeColor = Theming.Sucess;
            }
            else
            {
                SettingsMainSrvText.ForeColor = Theming.Warning;
                if (VisualsAPIChecker.UnitedSL && !VisualsAPIChecker.UnitedCDNL) { SettingsMainSrvText.Text = "[API] United: SERVER LIST ONLY"; }
                else if (!VisualsAPIChecker.UnitedSL && VisualsAPIChecker.UnitedCDNL) { SettingsMainSrvText.Text = "[API] United: CDN LIST ONLY"; }
                else { SettingsMainSrvText.Text = "[API] United: OFFLINE"; SettingsMainSrvText.ForeColor = Theming.Error; }                
                SettingsMainCDNText.Visible = true;
            }

            if (VisualsAPIChecker.CarbonAPI())
            {
                SettingsMainCDNText.Text = "[API] Carbon: ONLINE";
                SettingsMainCDNText.ForeColor = Theming.Sucess;
            }
            else
            {
                SettingsMainCDNText.ForeColor = Theming.Warning;
                if (VisualsAPIChecker.CarbonSL && !VisualsAPIChecker.CarbonCDNL) { SettingsMainCDNText.Text = "[API] Carbon: SERVER LIST ONLY"; }
                else if (!VisualsAPIChecker.CarbonSL && VisualsAPIChecker.CarbonCDNL) { SettingsMainCDNText.Text = "[API] Carbon: CDN LIST ONLY"; }
                else { SettingsMainCDNText.Text = "[API] Carbon: OFFLINE"; SettingsMainCDNText.ForeColor = Theming.Error; }
                SettingsBkupSrvText.Visible = true;
            }

            if (VisualsAPIChecker.CarbonAPITwo())
            {
                SettingsBkupSrvText.Text = "[API] Carbon (2nd): ONLINE";
                SettingsBkupSrvText.ForeColor = Theming.Sucess;
            }
            else
            {
                SettingsBkupSrvText.ForeColor = Theming.Warning;
                if (VisualsAPIChecker.CarbonTwoSL && !VisualsAPIChecker.CarbonTwoCDNL) { SettingsBkupSrvText.Text = "[API] Carbon (2nd): SERVER LIST ONLY"; }
                else if (!VisualsAPIChecker.CarbonTwoSL && VisualsAPIChecker.CarbonTwoCDNL) { SettingsBkupSrvText.Text = "[API] Carbon (2nd): CDN LIST ONLY"; }
                else { SettingsBkupSrvText.Text = "[API] Carbon (2nd): OFFLINE"; SettingsBkupSrvText.ForeColor = Theming.Error; }                
                SettingsBkupCDNText.Visible = true;
            }

            if (VisualsAPIChecker.WOPLAPI())
            {
                SettingsBkupCDNText.Text = "[API] WOPL: ONLINE";
                SettingsBkupCDNText.ForeColor = Theming.Sucess;
            }
            else
            {
                SettingsBkupCDNText.ForeColor = Theming.Warning;
                if (VisualsAPIChecker.WOPLSL && !VisualsAPIChecker.WOPLCDNL) { SettingsBkupCDNText.Text = "[API] WOPL: SERVER LIST ONLY"; }
                else if (!VisualsAPIChecker.WOPLSL && VisualsAPIChecker.WOPLCDNL) { SettingsBkupCDNText.Text = "[API] WOPL: CDN LIST ONLY"; }
                else { SettingsBkupCDNText.Text = "[API] WOPL: OFFLINE"; SettingsBkupCDNText.ForeColor = Theming.Error; }
            }
        }

        private void RememberLastCDN()
        {
            if (CDNListUpdater.CleanList != null)
            {
                if (CDNListUpdater.CleanList.Any())
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
                        LogToFileAddons.OpenLog("SETTINGS CDNLIST", null, Error, null, true);
                    }
                }
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
                LogToFileAddons.OpenLog("SETTINGS CDNLIST", null, Error, null, true);
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
                LogToFileAddons.OpenLog("SETTINGS LANGLIST", null, Error, null, true);
            }
        }

        /* CDN Display Playing Game! - DavidCarbon */
        private void PingSavedCDN()
        {
            if (ThreadSavedCDN != null)
            {
                ThreadSavedCDN.Abort();
                ThreadSavedCDN = null;
            }

            if (!string.IsNullOrWhiteSpace(FileSettingsSave.CDN))
            {
                SettingsCDNCurrent.LinkColor = Theming.SecondaryTextForeColor;
                Log.Info("SETTINGS PINGING CDN: Checking Current CDN from Settings.ini");

                ThreadSavedCDN = new Thread(() =>
                {
                    if (Application.OpenForms["SettingsScreen"] != null)
                    {
                        if (!Application.OpenForms["SettingsScreen"].Disposing)
                        {
                            switch (APIChecker.CheckStatus(FileSettingsSave.CDN + "/index.xml", 10))
                            {
                                case APIStatus.Online:
                                    if (SettingsCDNCurrent.InvokeRequired)
                                    {
                                        SettingsCDNCurrent.Invoke(new Action(delegate ()
                                        {
                                            SettingsCDNCurrent.LinkColor = Theming.Sucess;
                                        }));
                                    }
                                    else { SettingsCDNCurrent.LinkColor = Theming.Sucess; }
                                    Log.UrlCall("SETTINGS PINGING CDN: " + FileSettingsSave.CDN + " Is Online!");
                                    break;
                                default:
                                    if (SettingsCDNCurrent.InvokeRequired)
                                    {
                                        SettingsCDNCurrent.Invoke(new Action(delegate ()
                                        {
                                            SettingsCDNCurrent.LinkColor = Theming.Error;
                                        }));
                                    }
                                    else { SettingsCDNCurrent.LinkColor = Theming.Error; }
                                    Log.UrlCall("SETTINGS PINGING CDN: " + FileSettingsSave.CDN + " Is Offline!");
                                    break;
                            }
                        }
                    }
                });

                ThreadSavedCDN.Start();
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

        private void ButtonSecurityPanel_Click(object sender, EventArgs e)
        {
            try
            {
                new SecurityCenterScreen().ShowDialog();
            }
            catch (Exception Error)
            {
                string ErrorMessage = "Security Center Screen Encountered an Error";
                LogToFileAddons.OpenLog("Security Center Panel", ErrorMessage, Error, "Exclamation", false);
            }
        }

        /* Settings Verify Hash */
        private void SettingsVFilesButton_Click(object sender, EventArgs e)
        {
            if (FunctionStatus.IsVerifyHashDisabled)
            {
                ButtonsColorSet(SettingsVFilesButton, 3, true);
                if (!File.Exists(Path.Combine(FileSettingsSave.GameInstallation, "nfsw.exe")))
                {
                    MessageBox.Show(null, "You need to Download the Game Files first before you can have access to run Verify Hash", 
                        "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show(null, "You have already done a 'Verify GameFiles' Scan" +
                    "\nPlease Restart Launcher to do a new Verify GameFiles Scan", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else if (!FunctionStatus.DoesCDNSupportVerifyHash)
            {
                ButtonsColorSet(SettingsVFilesButton, 3, true);
                MessageBox.Show(null, "The current saved CDN does not support 'Verify GameFiles' Scan" +
                    "\nPlease Choose Another CDN from the list", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ButtonsColorSet(SettingsVFilesButton, (FileSettingsSave.GameIntegrity != "Good" ? 2 : 0), true);
                new VerifyHash().ShowDialog();
            }
        }
    }
}