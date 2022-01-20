using SBRW.Launcher.App.Classes.InsiderKit;
using SBRW.Launcher.App.Classes.LauncherCore.APICheckers;
using SBRW.Launcher.App.Classes.LauncherCore.FileReadWrite;
using SBRW.Launcher.App.Classes.LauncherCore.Global;
using SBRW.Launcher.App.Classes.LauncherCore.Lists;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.App.Classes.LauncherCore.ModNet;
using SBRW.Launcher.App.Classes.LauncherCore.Support;
using SBRW.Launcher.App.Classes.LauncherCore.Visuals;
using SBRW.Launcher.App.Classes.SystemPlatform.Unix;
using SBRW.Launcher.App.UI_Forms.SecurityCenter_Screen;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extension.Api_;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.String_;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Extra.Ini_;
using SBRW.Launcher.Core.Proxy.Nancy_;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Required.System.Windows_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Settings_Screen
{
    public partial class Screen_Settings : Form
    {
        /*******************************/
        /* Global Functions             /
        /*******************************/

        private static bool IsSettingsScreenOpen { get; set; }
        private int LastSelectedCdnId { get; set; }
        private int LastSelectedLanguage { get; set; }
        private bool DisableProxy { get; set; }
        private bool DisableDiscordRPC { get; set; }
        private bool EnableAltWebCalls { get; set; }
        private bool EnableInsiderPreview { get; set; }
        private bool EnableThemeSupport { get; set; }
        private bool EnableStreamSupport { get; set; }
        private bool RestartRequired { get; set; }
        private string NewLauncherPath { get; set; }
        private string NewGameFilesPath { get; set; }
        private string FinalCDNURL { get; set; }
        private static Thread? ThreadChangedCDN { get; set; }
        private static Thread? ThreadSavedCDN { get; set; }
        private static Thread? ThreadChecksums { get; set; }

        public static void OpenScreen()
        {
            if (IsSettingsScreenOpen || Application.OpenForms["Screen_Settings"] != null)
            {
                if (Application.OpenForms["Screen_Settings"] != null) { Application.OpenForms["Screen_Settings"].Activate(); }
            }
            else
            {
                try { new Screen_Settings().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Settings Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Settings Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }
        #region Support Functions
        private void WindowsDefenderGameFilesDirctoryChange()
        {
            if (!UnixOS.Detected())
            {
                /* Check if New Game! Files is not in Banned Folder Locations */
                CheckGameFilesDirectoryPrevention();
                /* Store Old Location for Security Panel to Use Later on */
                Save_Settings.Live_Data.Game_Path_Old = Save_Settings.Live_Data.Game_Path;
                Save_Settings.Live_Data.Firewall_Game = "Not Excluded";
                Save_Settings.Live_Data.Defender_Game = "Not Excluded";
                ButtonsColorSet(Button_Security_Center, 2, true);
            }

            Save_Settings.Live_Data.Game_Path = NewGameFilesPath;

            /* Clean Mods Files from New Dirctory (If it has .links in directory) */
            if (File.Exists(Path.Combine(NewGameFilesPath, Locations.NameModLinks)))
            {
                ModNetHandler.CleanLinks(NewGameFilesPath);
                Log.Completed("CLEANLINKS: Done");
            }

            ButtonsColorSet(Button_Game_Verify_Files, 1, true);
            RestartRequired = true;
        }

        private void CheckGameFilesDirectoryPrevention()
        {
            if (!UnixOS.Detected())
            {
                bool FailSafePathCreation = false;
                switch (FunctionStatus.CheckFolder(NewGameFilesPath))
                {
                    case FolderType.IsSameAsLauncherFolder:
                        FailSafePathCreation = true;
                        Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                        Log.Error("LAUNCHER: Installing NFSW in same location where the GameLauncher resides is NOT allowed.");
                        MessageBox.Show(null, string.Format("Installing NFSW in same location where the GameLauncher resides is NOT allowed." +
                            "\nInstead, we will install it at {0}.", Locations.GameFilesFailSafePath), "GameLauncher",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case FolderType.IsTempFolder:
                        FailSafePathCreation = true;
                        Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                        Log.Error("LAUNCHER: (╯°□°）╯︵ ┻━┻ Installing NFSW in the Temp Folder is NOT allowed!");
                        MessageBox.Show(null, string.Format("(╯°□°）╯︵ ┻━┻\n\nInstalling NFSW in the Temp Folder is NOT allowed!" +
                            "\nInstead, we will install it at {0}.", Locations.GameFilesFailSafePath + "\n\n┬─┬ ノ( ゜-゜ノ)"), "GameLauncher",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case FolderType.IsProgramFilesFolder:
                    case FolderType.IsUsersFolders:
                    case FolderType.IsWindowsFolder:
                        FailSafePathCreation = true;
                        Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                        Log.Error("LAUNCHER: Installing NFSW in a Special Directory is disadvised.");
                        MessageBox.Show(null, string.Format("Installing NFSW in a Special Directory is not recommended or allowed." +
                            "\nInstead, we will install it at {0}.", Locations.GameFilesFailSafePath), "GameLauncher",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
                Save_Settings.Save();

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
                            LogToFileAddons.OpenLog("Launcher", string.Empty, Error, string.Empty, true);
                        }
                    }
                }
            }
        }
        #endregion
        #region Settings
        #region Shown (When Window Is Visually Shown)
        /* CDN Display Playing Game! - DavidCarbon */
        private void PingSavedCDN()
        {
            if (ThreadSavedCDN != null)
            {
                ThreadSavedCDN.Abort();
                ThreadSavedCDN = null;
            }

            if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Launcher_CDN))
            {
                LinkLabel_CDN_Current.LinkColor = Color_Text.L_Two;
                Log.Info("SETTINGS PINGING CDN: Checking Current CDN from Settings.ini");

                ThreadSavedCDN = new Thread(() =>
                {
                    if (!Application.OpenForms[this.Name].IsDisposed)
                    {
                        switch (API_Core.StatusCheck(Save_Settings.Live_Data.Launcher_CDN + "/index.xml", 10))
                        {
                            case APIStatus.Online:
                                LinkLabel_CDN_Current.SafeInvokeAction(() =>
                                {
                                    LinkLabel_CDN_Current.LinkColor = Color_Text.S_Sucess;
                                });
                                Log.UrlCall("SETTINGS PINGING CDN: " + Save_Settings.Live_Data.Launcher_CDN + " Is Online!");
                                break;
                            default:
                                LinkLabel_CDN_Current.SafeInvokeAction(() =>
                                {
                                    LinkLabel_CDN_Current.LinkColor = Color_Text.S_Error;
                                });
                                Log.UrlCall("SETTINGS PINGING CDN: " + Save_Settings.Live_Data.Launcher_CDN + " Is Offline!");
                                break;
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
        #endregion
        #region Loading
        private void Screen_Settings_Load(object sender, EventArgs e)
        {

            /*******************************/
            /* Read Settings.ini            /
            /*******************************/

            if (File.Exists(Save_Settings.Live_Data.Game_Path + "/profwords") || File.Exists(Save_Settings.Live_Data.Game_Path + "/profwords_dis"))
            {
                CheckBox_Word_Filter_Check.Checked = !File.Exists(Save_Settings.Live_Data.Game_Path + "/profwords");
            }
            else
            {
                CheckBox_Word_Filter_Check.Enabled = false;
            }

            Label_Theme_Name.Text = "Theme Name: " + Theming.ThemeName;
            Label_Theme_Author.Text = "Theme Author: " + Theming.ThemeAuthor;

            /*******************************/
            /* Folder Locations             /
            /*******************************/

            NewGameFilesPath = Save_Settings.Live_Data.Game_Path;
            NewLauncherPath = Locations.LauncherFolder;
            DisableProxy = (Save_Settings.Live_Data.Launcher_Proxy == "1");
            DisableDiscordRPC = (Save_Settings.Live_Data.Launcher_Discord_Presence == "1");
            EnableAltWebCalls = (Save_Settings.Live_Data.Launcher_WebClient_Method == "WebClientWithTimeout");
            EnableInsiderPreview = Save_Settings.Live_Data.Launcher_Insider == "1";
            EnableThemeSupport = Save_Settings.Live_Data.Launcher_Theme_Support == "1";
            EnableStreamSupport = Save_Settings.Live_Data.Launcher_Streaming_Support == "1";

            CheckBox_Proxy.Checked = DisableProxy;
            CheckBox_RPC.Checked = DisableDiscordRPC;
            CheckBox_Alt_WebCalls.Checked = EnableAltWebCalls;
            CheckBox_Opt_Insider.Checked = EnableInsiderPreview;
            CheckBox_Theme_Support.Checked = EnableThemeSupport;
            CheckBox_Stream.Checked = EnableStreamSupport;

            /*******************************/
            /* Enable/Disable Visuals       /
            /*******************************/

            if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, "NFSWO_COMMUNICATION_LOG.txt")))
            {
                ButtonsColorSet(Button_Clear_NFSWO_Logs, 2, true);
            }
            else
            {
                ButtonsColorSet(Button_Clear_NFSWO_Logs, 4, false);
            }

            if (Directory.Exists(Save_Settings.Live_Data.Game_Path + "/.data"))
            {
                ButtonsColorSet(Button_Clear_Server_Mods, 2, true);
            }
            else
            {
                ButtonsColorSet(Button_Clear_Server_Mods, 4, false);
            }

            try
            {
                DirectoryInfo CrashLogFilesDirectory = new DirectoryInfo(Save_Settings.Live_Data.Game_Path);

                if (CrashLogFilesDirectory.EnumerateFiles("SBRCrashDump_CL0*.dmp", SearchOption.TopDirectoryOnly).Count() != 0)
                {
                    ButtonsColorSet(Button_Clear_Crash_Logs, 2, true);
                }
                else if (CrashLogFilesDirectory.EnumerateFiles("SBRCrashDump_CL0*.dmp", SearchOption.TopDirectoryOnly).Count() == 0)
                {
                    ButtonsColorSet(Button_Clear_Crash_Logs, 4, false);
                }
                else
                {
                    ButtonsColorSet(Button_Clear_Crash_Logs, 1, false);
                }
            }
            catch (Exception Error)
            {
                ButtonsColorSet(Button_Clear_Crash_Logs, 3, false);
                LogToFileAddons.OpenLog("SettingsScreen [SBRCrashDump_Check]", string.Empty, Error, string.Empty, true);
            }

            try
            {
                DirectoryInfo LauncherLogFilesDirectory = new DirectoryInfo(Log_Location.LogFolder);

                if (LauncherLogFilesDirectory.EnumerateDirectories().Count() != 1)
                {
                    ButtonsColorSet(Button_Launcher_logs, 2, true);
                }
                else
                {
                    ButtonsColorSet(Button_Launcher_logs, 1, false);
                }
            }
            catch (Exception Error)
            {
                ButtonsColorSet(Button_Launcher_logs, 3, false);
                LogToFileAddons.OpenLog("SettingsScreen [Launcher Log Check]", string.Empty, Error, string.Empty, true);
            }

            try
            {
                Log.Info("SETTINGS VERIFYHASH: Checking Characters in URL");
                if (Save_Settings.Live_Data.Launcher_CDN.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    FinalCDNURL = Save_Settings.Live_Data.Launcher_CDN.TrimEnd(charsToTrim);
                    Log.Info("SETTINGS VERIFYHASH: Trimed end of URL -> " + FinalCDNURL);
                }
                else
                {
                    FinalCDNURL = Save_Settings.Live_Data.Launcher_CDN;
                }
            }
            catch (Exception Error)
            {
                FinalCDNURL = Save_Settings.Live_Data.Launcher_CDN;
                LogToFileAddons.OpenLog("SETTINGS CDN URL TRIM", string.Empty, Error, string.Empty, true);
            }

            try
            {
                if (EnableInsiderDeveloper.Allowed())
                {
                    FunctionStatus.DoesCDNSupportVerifyHash = true;
                    ButtonsColorSet(Button_Game_Verify_Files, 4, true);
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
                        if (!Application.OpenForms[this.Name].IsDisposed)
                        {
                            if (!Application.OpenForms[this.Name].Disposing)
                            {
                                ButtonsColorSet(Button_Game_Verify_Files, 0, false);
                                switch (API_Core.StatusCheck(FinalCDNURL + "/unpacked/checksums.dat", 10))
                                {
                                    case APIStatus.Online:
                                        FunctionStatus.DoesCDNSupportVerifyHash = true;
                                        ButtonsColorSet(Button_Game_Verify_Files, (Save_Settings.Live_Data.Game_Integrity != "Good" ? 2 : 0), true);
                                        break;
                                    default:
                                        FunctionStatus.DoesCDNSupportVerifyHash = false;
                                        ButtonsColorSet(Button_Game_Verify_Files, 3, true);
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
                LogToFileAddons.OpenLog("SETTINGS VERIFYHASH", string.Empty, Error, string.Empty, true);
            }

            /********************************/
            /* CDN, APIs, & Restore Last CDN /
            /********************************/

            /* Check If Launcher Failed to Connect to any APIs */
            if (!VisualsAPIChecker.CarbonAPITwo())
            {
                MessageBox.Show(null, "Unable to Connect to any CDN List API. Please check your connection." +
                "\nCDN Dropdown List will not be available on Settings Screen",
                "GameLauncher has Paused, Failed To Connect to any CDN List API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion
        #region ComboxList Setup
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
                        if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Launcher_CDN))
                        {
                            string FinalCDNURL;

                            if (Save_Settings.Live_Data.Launcher_CDN.EndsWith("/"))
                            {
                                char[] charsToTrim = { '/' };
                                FinalCDNURL = Save_Settings.Live_Data.Launcher_CDN.TrimEnd(charsToTrim);
                            }
                            else
                            {
                                FinalCDNURL = Save_Settings.Live_Data.Launcher_CDN;
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
                                    ComboBox_CDN_List.SelectedIndex = index;
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
                                ComboBox_CDN_List.SelectedIndex = 1;
                                Log.Warning("SETTINGS CDNLIST: Unknown entry value is " + FinalCDNURL);
                            }
                        }
                        else
                        {
                            ComboBox_CDN_List.SelectedIndex = 1;
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("SETTINGS CDNLIST", string.Empty, Error, string.Empty, true);
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
                if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Launcher_CDN))
                {
                    string FinalCDNURL = Save_Settings.Live_Data.Launcher_CDN + "/";

                    if (CDNListUpdater.CleanList.FindIndex(i => string.Equals(i.Url, FinalCDNURL)) != 0)
                    {
                        var index = CDNListUpdater.CleanList.FindIndex(i => string.Equals(i.Url, FinalCDNURL));

                        if (index >= 0)
                        {
                            Log.Warning("SETTINGS CDNLIST: Found ID Based on OLD Standard");
                            ComboBox_CDN_List.SelectedIndex = index;
                        }
                        else if (index < 0)
                        {
                            Log.Warning("SETTINGS CDNLIST: Failed to Detect Standard!");
                            ComboBox_CDN_List.SelectedIndex = 1;
                            Log.Warning("SETTINGS CDNLIST: Displaying First CDN in List!");
                        }
                    }
                    else
                    {
                        ComboBox_CDN_List.SelectedIndex = 1;
                    }
                }
                else
                {
                    ComboBox_CDN_List.SelectedIndex = 1;
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("SETTINGS CDNLIST", string.Empty, Error, string.Empty, true);
            }
        }

        private void RememberLastLanguage()
        {
            /* Last Selected CDN */
            Log.Core("SETTINGS LANGLIST: Checking...");
            Log.Core("SETTINGS LANGLIST: Setting first Language in list");
            Log.Core("SETTINGS LANGLIST: Checking if Language is set on INI File");

            try
            {
                if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Launcher_Language))
                {
                    string SavedLang = Save_Settings.Live_Data.Launcher_Language.Trim();

                    Log.Core("SETTINGS LANGLIST: Found something!");
                    Log.Core("SETTINGS LANGLIST: Checking if Language exists on our database");

                    if (LanguageListUpdater.CleanList.FindIndex(i => string.Equals(i.Value_Ini, SavedLang)) != 0)
                    {
                        Log.Core("SETTINGS LANGLIST: Language found! Checking its Value");
                        var index = LanguageListUpdater.CleanList.FindIndex(i => string.Equals(i.Value_Ini, SavedLang));

                        Log.Core("SETTINGS LANGLIST: ID is " + index);
                        if (index >= 0)
                        {
                            Log.Core("SETTINGS LANGLIST: ID set correctly");
                            ComboBox_Language_List.SelectedIndex = index;
                        }
                        else if (index < 0)
                        {
                            ComboBox_Language_List.SelectedIndex = 1;
                        }
                    }
                    else
                    {
                        Log.Warning("SETTINGS LANGLIST: Unable to find anything, assuming default");
                        ComboBox_Language_List.SelectedIndex = 1;
                        Log.Warning("SETTINGS LANGLIST: Unknown entry value is " + SavedLang);
                    }
                    Log.Core("SETTINGS LANGLIST: All done");
                }
                else
                {
                    Log.Warning("SETTINGS LANGLIST: Unable to find anything, assuming default");
                    ComboBox_Language_List.SelectedIndex = 1;
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("SETTINGS LANGLIST", string.Empty, Error, string.Empty, true);
            }
        }
        #endregion
        #region Event Functions
        private void Button_Security_Center_Click(object sender, EventArgs e)
        {
            try
            {
                Screen_Security_Center.OpenScreen("Settings");
            }
            catch (Exception Error)
            {
                string ErrorMessage = "Security Center Screen Encountered an Error";
                LogToFileAddons.OpenLog("Security Center Panel", ErrorMessage, Error, "Exclamation", false);
            }
        }
        /* Settings Verify Hash */
        private void Button_Game_Verify_Files_Click(object sender, EventArgs e)
        {
            if (FunctionStatus.IsVerifyHashDisabled)
            {
                ButtonsColorSet(Button_Game_Verify_Files, 3, true);
                if (!File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, "nfsw.exe")))
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
                ButtonsColorSet(Button_Game_Verify_Files, 3, true);
                MessageBox.Show(null, "The current saved CDN does not support 'Verify GameFiles' Scan" +
                    "\nPlease Choose Another CDN from the list", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ButtonsColorSet(Button_Game_Verify_Files, (Save_Settings.Live_Data.Game_Integrity != "Good" ? 2 : 0), true);
                //VerifyHash.OpenScreen();
            }
        }
        /*******************************/
        /* On Button/Dropdown Functions /
        /*******************************/

        /* Settings Save */
        private void SettingsSave_Click(object sender, EventArgs e)
        {
            Button_Save.Text = "SAVING";
            /* TODO null check */
            if (ComboBox_Language_List.SelectedItem != null && !string.IsNullOrWhiteSpace(((Json_List_Language)ComboBox_Language_List.SelectedItem).Value_Ini))
            {
                Save_Settings.Live_Data.Launcher_Language = ((Json_List_Language)ComboBox_Language_List.SelectedItem).Value_Ini;
                FileGameSettingsData.Language = ((Json_List_Language)ComboBox_Language_List.SelectedItem).Value_XML;

                /* TODO: Inform player about custom languagepack used. */
                if (((Json_List_Language)ComboBox_Language_List.SelectedItem).Category == "Custom")
                {
                    MessageBox.Show(null, "Please Note: If a Server does not provide a Language Pack, it will fallback to English Language Pack instead.",
                        "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (!Directory.Exists(Save_Settings.Live_Data.Game_Path + "/scripts"))
                    {
                        try { Directory.CreateDirectory(Save_Settings.Live_Data.Game_Path + "/scripts"); }
                        catch { }
                    }

                    if (File.Exists(Save_Settings.Live_Data.Game_Path + "/scripts/LangPicker.ini"))
                    {
                        try { File.Delete(Save_Settings.Live_Data.Game_Path + "/scripts/LangPicker.ini"); }
                        catch { }
                    }

                    try
                    {
                        Ini_File LanguagePickerFile = new Ini_File(Save_Settings.Live_Data.Game_Path + "/scripts/LangPicker.ini");
                        LanguagePickerFile.Key_Write("Language", ((Json_List_Language)ComboBox_Language_List.SelectedItem).Value_Ini);
                    }
                    catch { }
                }
                /* Delete Custom Settings.ini for LangPicker.asi module */
                else if (File.Exists(Save_Settings.Live_Data.Game_Path + "/scripts/LangPicker.ini"))
                {
                    try { File.Delete(Save_Settings.Live_Data.Game_Path + "/scripts/LangPicker.ini"); }
                    catch { }
                }
            }

            if (!string.IsNullOrWhiteSpace(NewGameFilesPath))
            {
                if (Product_Version.GetWindowsNumber() >= 10.0 && (Save_Settings.Live_Data.Game_Path != NewGameFilesPath) && !UnixOS.Detected())
                {
                    WindowsDefenderGameFilesDirctoryChange();
                }
                else if (Save_Settings.Live_Data.Game_Path != NewGameFilesPath)
                {
                    if (!UnixOS.Detected())
                    {
                        /* Check if New Game! Files is not in Banned Folder Locations */
                        CheckGameFilesDirectoryPrevention();
                        /* Store Old Location for Security Panel to Use Later on */
                        Save_Settings.Live_Data.Game_Path_Old = Save_Settings.Live_Data.Game_Path;
                        Save_Settings.Live_Data.Firewall_Game = "Not Excluded";
                        ButtonsColorSet(Button_Security_Center, 2, true);
                    }

                    Save_Settings.Live_Data.Game_Path = NewGameFilesPath;

                    /* Clean Mods Files from New Dirctory (If it has .links in directory) */
                    if (File.Exists(Path.Combine(NewGameFilesPath, Locations.NameModLinks)))
                    {
                        ModNetHandler.CleanLinks(NewGameFilesPath);
                        Log.Completed("CLEANLINKS: Done");
                    }

                    ButtonsColorSet(Button_Change_Game_Path, 1, true);
                    RestartRequired = true;
                }
            }

            if (ComboBox_CDN_List.SelectedItem != null && !string.IsNullOrWhiteSpace(((Json_List_CDN)ComboBox_CDN_List.SelectedItem).Url))
            {
                string SelectedCDNFromList = ((Json_List_CDN)ComboBox_CDN_List.SelectedItem).Url;
                string LocalFinalCDNURL;

                if (SelectedCDNFromList.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    LocalFinalCDNURL = SelectedCDNFromList.TrimEnd(charsToTrim);
                }
                else
                {
                    LocalFinalCDNURL = ((Json_List_CDN)ComboBox_CDN_List.SelectedItem).Url;
                }

                if (Save_Settings.Live_Data.Launcher_CDN != LocalFinalCDNURL)
                {
                    Label_CDN_Current.Text = "CHANGED CDN";
                    LinkLabel_CDN_Current.Text = LocalFinalCDNURL;
                    FinalCDNURL = Save_Settings.Live_Data.Launcher_CDN = LocalFinalCDNURL;
                    RestartRequired = true;

                    if (ThreadChecksums != null)
                    {
                        ThreadChecksums.Abort();
                        ThreadChecksums = null;
                    }

                    ThreadChecksums = new Thread(() =>
                    {
                        ButtonsColorSet(Button_Game_Verify_Files, 0, false);

                        switch (API_Core.StatusCheck(FinalCDNURL + "/unpacked/checksums.dat", 10))
                        {
                            case APIStatus.Online:
                                FunctionStatus.DoesCDNSupportVerifyHash = true;
                                ButtonsColorSet(Button_Game_Verify_Files, (Save_Settings.Live_Data.Game_Integrity != "Good" ? 2 : 0), true);
                                break;
                            default:
                                FunctionStatus.DoesCDNSupportVerifyHash = false;
                                ButtonsColorSet(Button_Game_Verify_Files, 3, true);
                                break;
                        }
                    });
                    ThreadChecksums.Start();
                }
            }
            else
            {
                Log.Error("SETTINGS: Selected CDN does not contain a URL, unable to Save Contents");
            }

            if (Save_Settings.Live_Data.Launcher_Proxy != (CheckBox_Proxy.Checked ? "1" : "0"))
            {
                Save_Settings.Live_Data.Launcher_Proxy = CheckBox_Proxy.Checked ? "1" : "0";

                if (Save_Settings.Live_Data.Launcher_Proxy == "1")
                {
                    if (Proxy_Settings.Running())
                    {
                        Proxy_Server.Instance.Stop("Settings Screen");
                    }

                    if (InformationCache.SelectedServerEnforceProxy)
                    {
                        MessageBox.Show(null, ServerListUpdater.ServerName("Settings") + " requires Proxy to be Enabled." +
                            "\nThe launcher will turn on Proxy, even if you have chosen to Disable it",
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (!Proxy_Settings.Running())
                    {
                        Proxy_Server.Instance.Start("Settings Screen");
                    }
                }
            }

            if (Save_Settings.Live_Data.Launcher_Discord_Presence != (CheckBox_RPC.Checked ? "1" : "0"))
            {
                Save_Settings.Live_Data.Launcher_Discord_Presence = CheckBox_RPC.Checked ? "1" : "0";

                if (Save_Settings.Live_Data.Launcher_Discord_Presence == "1")
                {
                    if (Presence_Launcher.Running())
                    {
                        Presence_Launcher.Stop("Close");
                    }
                }
                else
                {
                    if (!Presence_Launcher.Running())
                    {
                        Presence_Launcher.Start("Start Up", null);
                    }
                }
            }

            if (Save_Settings.Live_Data.Launcher_Insider != (CheckBox_Opt_Insider.Checked ? "1" : "0"))
            {
                EnableInsiderBetaTester.Allowed((Save_Settings.Live_Data.Launcher_Insider = CheckBox_Opt_Insider.Checked ? "1" : "0") == "1");
                RestartRequired = true;
            }

            if (Save_Settings.Live_Data.Launcher_Theme_Support != (CheckBox_Theme_Support.Checked ? "1" : "0"))
            {
                Save_Settings.Live_Data.Launcher_Theme_Support = CheckBox_Theme_Support.Checked ? "1" : "0";
                RestartRequired = true;
            }

            if (Save_Settings.Live_Data.Launcher_WebClient_Method != (CheckBox_Alt_WebCalls.Checked ? "WebClientWithTimeout" : "WebClient"))
            {
                Save_Settings.Live_Data.Launcher_WebClient_Method = CheckBox_Alt_WebCalls.Checked ? "WebClientWithTimeout" : "WebClient";
                Launcher_Value.Launcher_Alternative_Webcalls(Save_Settings.Live_Data.Launcher_WebClient_Method == "WebClient");
            }

            if (Save_Settings.Live_Data.Launcher_Streaming_Support != (CheckBox_Stream.Checked ? "1" : "0"))
            {
                Save_Settings.Live_Data.Launcher_Streaming_Support = CheckBox_Stream.Checked ? "1" : "0";
            }

            try
            {
                /* Actually lets check those 2 files */
                if (File.Exists(Save_Settings.Live_Data.Game_Path + "/profwords") && File.Exists(Save_Settings.Live_Data.Game_Path + "/profwords_dis"))
                {
                    File.Delete(Save_Settings.Live_Data.Game_Path + "/profwords_dis");
                }

                /* Delete/Enable profwords filter here */
                if (CheckBox_Word_Filter_Check.Checked)
                {
                    if (File.Exists(Save_Settings.Live_Data.Game_Path + "/profwords"))
                    {
                        File.Move(Save_Settings.Live_Data.Game_Path + "/profwords", Save_Settings.Live_Data.Game_Path + "/profwords_dis");
                    }
                }
                else
                {
                    if (File.Exists(Save_Settings.Live_Data.Game_Path + "/profwords_dis"))
                    {
                        File.Move(Save_Settings.Live_Data.Game_Path + "/profwords_dis", Save_Settings.Live_Data.Game_Path + "/profwords");
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("SETTINGS SAVE [Profwords]", string.Empty, Error, string.Empty, true);
            }

            /* Save Settings */
            Save_Settings.Save();
            FileGameSettings.Save("Suppress", "Language Only");
            Button_Save.Text = "SAVED";

            if (RestartRequired)
            {
                MessageBox.Show(null, "In order to see settings changes, you need to restart the Launcher manually.", "GameLauncher",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /* Settings Cancel */
        private void SettingsCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /* Settings UserSettings XML Editor */
        private void SettingsUEditorButton_Click(object sender, EventArgs e)
        {
            //USXEditor.OpenScreen();
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
                    if (Directory.Exists(Save_Settings.Live_Data.Game_Path + "/.data"))
                    {
                        Directory.Delete(Save_Settings.Live_Data.Game_Path + "/.data", true);
                    }
                    if (Directory.Exists(Save_Settings.Live_Data.Game_Path + "/MODS"))
                    {
                        Directory.Delete(Save_Settings.Live_Data.Game_Path + "/MODS", true);
                    }
                    Log.Warning("LAUNCHER: User Confirmed to Delete Server Mods Cache");
                    ButtonsColorSet(Button_Clear_Server_Mods, 1, false);
                    MessageBox.Show(null, "Deleted Server Mods Cache", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception Error)
                {
                    ButtonsColorSet(Button_Clear_Server_Mods, 3, true);
                    LogToFileAddons.OpenLog("SETTINGS CLEAR", "Unable to Delete Server Mods Cache", Error, "Exclamation", false);
                }
            }
        }

        /* Settings Clear Communication Logs */
        private void SettingsClearCommunicationLogButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Save_Settings.Live_Data.Game_Path + "/NFSWO_COMMUNICATION_LOG.txt"))
                {
                    File.Delete(Save_Settings.Live_Data.Game_Path + "/NFSWO_COMMUNICATION_LOG.txt");
                }
                ButtonsColorSet(Button_Clear_NFSWO_Logs, 1, false);
                MessageBox.Show(null, "Deleted NFSWO Communication Log", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Error)
            {
                ButtonsColorSet(Button_Clear_NFSWO_Logs, 3, true);
                LogToFileAddons.OpenLog("SETTINGS CLEAR", "Unable to Delete NFSWO Communication Log", Error, "Exclamation", false);
            }
        }

        /* Settings Clear Game Crash Logs */
        private void SettingsClearCrashLogsButton_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo CrashLogFilesDirectory = new DirectoryInfo(Save_Settings.Live_Data.Game_Path);

                foreach (FileInfo LocatedFile in CrashLogFilesDirectory.EnumerateFiles("SBRCrashDump_CL0*.dmp", SearchOption.TopDirectoryOnly))
                {
                    LocatedFile.Delete();
                }

                foreach (FileInfo LocatedFile in CrashLogFilesDirectory.EnumerateFiles("SBRCrashDump_CL0*.txt", SearchOption.TopDirectoryOnly))
                {
                    LocatedFile.Delete();
                }

                foreach (FileInfo LocatedFile in CrashLogFilesDirectory.EnumerateFiles("NFSCrashDump_CL0*.dmp", SearchOption.TopDirectoryOnly))
                {
                    LocatedFile.Delete();
                }

                ButtonsColorSet(Button_Clear_Crash_Logs, 1, false);
                MessageBox.Show(null, "Deleted Crash Logs", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Error)
            {
                ButtonsColorSet(Button_Clear_Crash_Logs, 3, true);
                LogToFileAddons.OpenLog("SETTINGS CLEAR", "Unable to Delete Crash Logs", Error, "Exclamation", false);
            }
        }

        /* Settings Clear Old Launcher Logs */
        private void SettingsClearLauncherLogsButton_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo InstallationDirectory = new DirectoryInfo(Log_Location.LogFolder);

                foreach (DirectoryInfo Folder in InstallationDirectory.EnumerateDirectories())
                {
                    if (Directory.Exists(Folder.FullName))
                    {
                        if (Folder.FullName != Log_Location.LogCurrentFolder)
                        {
                            Directory.Delete(Folder.FullName, true);
                        }
                    }
                }

                ButtonsColorSet(Button_Launcher_logs, 1, false);
                MessageBox.Show(null, "Deleted Old Launcher Logs", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Error)
            {
                ButtonsColorSet(Button_Launcher_logs, 3, true);
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
                    NewGameFilesPath = Path.GetDirectoryName(changeGameFilesPath.FileName)??"Invalid Folder Path";
                    Label_Game_Current_Path.Text = "NEW DIRECTORY";
                    LinkLabel_Game_Path.Text = NewGameFilesPath;
                }

                changeGameFilesPath.Dispose();
            }
            else
            {
                FolderBrowserDialog changeGameFilesPath = new FolderBrowserDialog();

                if (changeGameFilesPath.ShowDialog() == DialogResult.OK)
                {
                    NewGameFilesPath = Path.GetFullPath(changeGameFilesPath.SelectedPath);
                    Label_Game_Current_Path.Text = "NEW DIRECTORY";
                    LinkLabel_Game_Path.Text = NewGameFilesPath;
                }
            }
        }

        /* Settings Open Current CDN in Browser */
        private void SettingsCDNCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Launcher_CDN))
            {
                Process.Start(Save_Settings.Live_Data.Launcher_CDN);
            }
        }

        /* Settings Open Current Launcher Path in Explorer */
        private void SettingsLauncherPathCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewLauncherPath))
            {
                Process.Start(NewLauncherPath);
            }
        }

        /* Settings Open Current Game Files Path in Explorer */
        private void SettingsGameFilesCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewGameFilesPath))
            {
                Process.Start(NewGameFilesPath);
            }
        }

        /* Settings Open About Dialog */
        private void SettingsAboutButton_Click(object sender, EventArgs e)
        {
            //About.OpenScreen();
        }

        private void SettingsLauncherVersion_Click(object sender, EventArgs e)
        {
            //new DebugScreen().ShowDialog();
        }

        /* Settings CDN Dropdown Menu Index */
        private void ComboBox_CDN_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ComboBox_CDN_List.SelectedItem != null)
                {
                    if (((Json_List_CDN)ComboBox_CDN_List.SelectedItem).IsSpecial)
                    {
                        ComboBox_CDN_List.SelectedIndex = LastSelectedCdnId;
                    }
                    else if (!string.IsNullOrWhiteSpace(((Json_List_CDN)ComboBox_CDN_List.SelectedItem).Url ?? string.Empty))
                    {
                        LastSelectedCdnId = ComboBox_CDN_List.SelectedIndex;

                        if (ThreadChangedCDN != null)
                        {
                            ThreadChangedCDN.Abort();
                            ThreadChangedCDN = null;
                        }

                        Label_CDN.Text = "CDN: PINGING";
                        Label_CDN.ForeColor = Color_Text.L_Two;
                        Log.Info("SETTINGS PINGING CHANGED CDN: Checking Changed CDN from Drop Down List");

                        ThreadChangedCDN = new Thread(() =>
                        {
                            if (!Application.OpenForms[this.Name].IsDisposed)
                            {
                                switch (API_Core.StatusCheck(((Json_List_CDN)ComboBox_CDN_List.SelectedItem).Url + "/index.xml", 10))
                                {
                                    case APIStatus.Online:
                                        Label_CDN.SafeInvokeAction(() =>
                                        {
                                            Label_CDN.Text = "CDN: ONLINE";
                                            Label_CDN.ForeColor = Color_Text.S_Sucess;
                                        });
                                        Log.UrlCall("SETTINGS PINGING CHANGED CDN: " + ((Json_List_CDN)ComboBox_CDN_List.SelectedItem).Url + " Is Online!");
                                        break;
                                    default:
                                        Label_CDN.SafeInvokeAction(() =>
                                        {
                                            Label_CDN.Text = "CDN: OFFLINE";
                                            Label_CDN.ForeColor = Color_Text.S_Error;
                                        });
                                        Log.UrlCall("SETTINGS PINGING CHANGED CDN: " + ((Json_List_CDN)ComboBox_CDN_List.SelectedItem).Url + " Is Offline!");
                                        break;
                                }
                            }
                        });

                        ThreadChangedCDN.Start();
                    }
                    else
                    {
                        Label_CDN.Text = "CDN:";
                        Label_CDN.ForeColor = Color_Text.L_Five;
                        Log.Error("SETTINGS PINGING CHANGED CDN: '((CDNObject)SettingsCDNPick.SelectedItem).Url)' has an Empty CDN URL");
                    }
                }
                else
                {
                    Label_CDN.Text = "CDN:";
                    Label_CDN.ForeColor = Color_Text.L_Five;
                }
            }
            catch { }
        }

        private void ComboBox_Language_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (bool.TryParse(((Json_List_Language)ComboBox_Language_List.SelectedItem).IsSpecial.ToString(), out bool Result))
                {
                    if (((Json_List_Language)ComboBox_Language_List.SelectedItem).IsSpecial)
                    {
                        ComboBox_Language_List.SelectedIndex = LastSelectedLanguage;
                    }
                    else
                    {
                        LastSelectedLanguage = ComboBox_Language_List.SelectedIndex;
                    }
                }
            }
            catch { }
        }
        #endregion
        #endregion
        #region Theme, Text, and Function Setter
        #region Draw and Regular Events
        /// <summary>
        /// Sets the Category for the CDN Drop Down Menu with its set of Colors
        /// </summary>
        /// <remarks>Dropdown Menu Visual</remarks>
        private void ComboBox_CDN_List_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                string cdnListText = string.Empty;

                if (sender is ComboBox cb)
                {
                    if (e.Index != -1 && cb.Items != null)
                    {
                        if (cb.Items[e.Index] is Json_List_CDN si)
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
                        e.Graphics.FillRectangle(new SolidBrush(Color_Winform_Other.DropMenu_Category_Background_ForeColor), e.Bounds);
                        e.Graphics.DrawString(cdnListText.Replace("<GROUP>", string.Empty), font,
                            new SolidBrush(Color_Winform_Other.DropMenu_Category_Text_ForeColor), e.Bounds);
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
                            backgroundColor = new SolidBrush(Color_Winform_Other.DropMenu_Background_ForeColor);
                            textColor = new SolidBrush(Color_Winform_Other.DropMenu_Text_ForeColor);
                        }

                        e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                        e.Graphics.DrawString("    " + cdnListText, font, textColor, e.Bounds);
                    }
                }
            }
            catch { }
        }
        /// <summary>
        /// Sets the Category for the Language Drop Down Menu with its set of Colors
        /// </summary>
        /// <remarks>Dropdown Menu Visual</remarks>
        private void ComboBox_Language_List_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                string langListText = string.Empty;

                if (sender is ComboBox cb)
                {
                    if (e.Index != -1 && cb.Items != null)
                    {
                        if (cb.Items[e.Index] is Json_List_Language si)
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
                        e.Graphics.FillRectangle(new SolidBrush(Color_Winform_Other.DropMenu_Category_Background_ForeColor), e.Bounds);
                        e.Graphics.DrawString(langListText.Replace("<GROUP>", string.Empty), font,
                            new SolidBrush(Color_Winform_Other.DropMenu_Category_Text_ForeColor), e.Bounds);
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
                            backgroundColor = new SolidBrush(Color_Winform_Other.DropMenu_Background_ForeColor);
                            textColor = new SolidBrush(Color_Winform_Other.DropMenu_Text_ForeColor);
                        }

                        e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                        e.Graphics.DrawString("    " + langListText, font, textColor, e.Bounds);
                    }
                }
            }
            catch { }
        }
        private void DropDownMenu_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }
        #endregion
        #region Buttons and API
        /* DavidCarbon */
        private void PingAPIStatus()
        {
            if (VisualsAPIChecker.UnitedAPI())
            {
                Label_API_Status_One.Text = "[API] United: Online";
                Label_API_Status_One.ForeColor = Color_Text.S_Sucess;
            }
            else
            {
                Label_API_Status_One.ForeColor = Color_Text.S_Warning;
                if (VisualsAPIChecker.UnitedSL && !VisualsAPIChecker.UnitedCDNL) { Label_API_Status_One.Text = "[API] United: Server List Only"; }
                else if (!VisualsAPIChecker.UnitedSL && VisualsAPIChecker.UnitedCDNL) { Label_API_Status_One.Text = "[API] United: CDN List Only"; }
                else
                {
                    Label_API_Status_One.Text = "[API] United: " + Strings.Truncate(APIChecker.StatusStrings(VisualsAPIChecker.UnitedSC), 32);
                    Label_API_Status_One.ForeColor = Color_Text.S_Error;
                }
                Label_API_Status_Two.Visible = true;
            }

            if (VisualsAPIChecker.CarbonAPI())
            {
                Label_API_Status_Two.Text = "[API] Carbon: Online";
                Label_API_Status_Two.ForeColor = Color_Text.S_Sucess;
            }
            else
            {
                Label_API_Status_Two.ForeColor = Color_Text.S_Warning;
                if (VisualsAPIChecker.CarbonSL && !VisualsAPIChecker.CarbonCDNL) { Label_API_Status_Two.Text = "[API] Carbon: Server List Only"; }
                else if (!VisualsAPIChecker.CarbonSL && VisualsAPIChecker.CarbonCDNL) { Label_API_Status_Two.Text = "[API] Carbon: CDN List Only"; }
                else
                {
                    Label_API_Status_Two.Text = "[API] Carbon: " + Strings.Truncate(APIChecker.StatusStrings(VisualsAPIChecker.CarbonSC), 32);
                    Label_API_Status_Two.ForeColor = Color_Text.S_Error;
                }
                Label_API_Status_Three.Visible = true;
            }

            if (VisualsAPIChecker.CarbonAPITwo())
            {
                Label_API_Status_Three.Text = "[API] Carbon (2nd): Online";
                Label_API_Status_Three.ForeColor = Color_Text.S_Sucess;
            }
            else
            {
                Label_API_Status_Three.ForeColor = Color_Text.S_Warning;
                if (VisualsAPIChecker.CarbonTwoSL && !VisualsAPIChecker.CarbonTwoCDNL) { Label_API_Status_Three.Text = "[API] Carbon (2nd): Server List Only"; }
                else if (!VisualsAPIChecker.CarbonTwoSL && VisualsAPIChecker.CarbonTwoCDNL) { Label_API_Status_Three.Text = "[API] Carbon (2nd): CDN List Only"; }
                else
                {
                    Label_API_Status_Three.Text = "[API] Carbon (2nd): " + Strings.Truncate(APIChecker.StatusStrings(VisualsAPIChecker.CarbonTwoSC), 32);
                    Label_API_Status_Three.ForeColor = Color_Text.S_Error;
                }
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
                    Elements.SafeInvokeAction(() =>
                    {
                        Elements.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
                        Elements.BackColor = Color_Winform_Buttons.Blue_Back_Color;
                        Elements.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
                        Elements.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;
                        Elements.Enabled = EnabledORDisabled;
                    });
                    break;
                /* Success Green */
                case 1:
                    Elements.SafeInvokeAction(() =>
                    {
                        Elements.ForeColor = Color_Winform_Buttons.Green_Fore_Color;
                        Elements.BackColor = Color_Winform_Buttons.Green_Back_Color;
                        Elements.FlatAppearance.BorderColor = Color_Winform_Buttons.Green_BorderColorButton;
                        Elements.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Green_Mouse_Over_Back_Color;
                        Elements.Enabled = EnabledORDisabled;
                    });
                    break;
                /* Warning Orange */
                case 2:
                    Elements.SafeInvokeAction(() =>
                    {
                        Elements.ForeColor = Color_Winform_Buttons.Yellow_Fore_Color;
                        Elements.BackColor = Color_Winform_Buttons.Yellow_Back_Color;
                        Elements.FlatAppearance.BorderColor = Color_Winform_Buttons.Yellow_Border_Color;
                        Elements.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Yellow_Mouse_Over_Back_Color;
                        Elements.Enabled = EnabledORDisabled;
                    });
                    break;
                /* Error Red */
                case 3:
                    Elements.SafeInvokeAction(() =>
                    {
                        Elements.ForeColor = Color_Winform_Buttons.Red_Fore_Color;
                        Elements.BackColor = Color_Winform_Buttons.Red_Back_Color;
                        Elements.FlatAppearance.BorderColor = Color_Winform_Buttons.Red_Border_Color;
                        Elements.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Red_Mouse_Over_Back_Color;
                        Elements.Enabled = EnabledORDisabled;
                    });
                    break;
                /* Unknown Gray */
                default:
                    Elements.SafeInvokeAction(() =>
                    {
                        Elements.ForeColor = Color_Winform_Buttons.Gray_Fore_Color;
                        Elements.BackColor = Color_Winform_Buttons.Gray_Back_Color;
                        Elements.FlatAppearance.BorderColor = Color_Winform_Buttons.Gray_Border_Color;
                        Elements.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Gray_Mouse_Over_Back_Color;
                        Elements.Enabled = EnabledORDisabled;
                    });
                    break;
            }
        }
        private void Greenbutton_hover_MouseEnter(object sender, EventArgs e)
        {
            Button_Save.Image = Image_Button.Green_Hover;
        }

        private void Greenbutton_MouseLeave(object sender, EventArgs e)
        {
            Button_Save.Image = Image_Button.Green;
        }

        private void Greenbutton_hover_MouseUp(object sender, EventArgs e)
        {
            Button_Save.Image = Image_Button.Green_Hover;
        }

        private void Greenbutton_click_MouseDown(object sender, EventArgs e)
        {
            Button_Save.Image = Image_Button.Green_Click;
        }

        private void Graybutton_click_MouseDown(object sender, EventArgs e)
        {
            Button_Exit.Image = Image_Button.Green_Click;
        }

        private void Graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            Button_Exit.Image = Image_Button.Green_Hover;
        }

        private void Graybutton_MouseLeave(object sender, EventArgs e)
        {
            Button_Exit.Image = Image_Button.Grey;
        }

        private void Graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            Button_Exit.Image = Image_Button.Grey_Hover;
        }
        #endregion
        /// <summary>
        /// Sets the Button, Image, Text, and Fonts. Enables/Disables Certain Elements of the Screen for Certain Platforms. Also contains functions that act as helper functions
        /// </summary>
        /// <remarks>Settings Screen Visuals</remarks>
        private void Set_Visuals()
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

            BackgroundImage = Image_Background.Settings;
            TransparencyKey = Color_Screen.BG_Settings;

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            LinkLabel_CDN_Current.Text = Save_Settings.Live_Data.Launcher_CDN;
            LinkLabel_Game_Path.Text = Save_Settings.Live_Data.Game_Path;
            LinkLabel_Launcher_Path.Text = AppDomain.CurrentDomain.BaseDirectory;
            Label_Version_Build.Text = "Version: v" + Application.ProductVersion;

            /*******************************/
            /* Set Font                     /
            /*******************************/

            float MainFontSize = UnixOS.Detected() ? 9f : 9f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = UnixOS.Detected() ? 8f : 8f * 96f / CreateGraphics().DpiY;

            Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            Button_Security_Center.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_About.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Game_Files.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Change_Game_Path.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Game_Verify_Files.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_CDN.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            ComboBox_CDN_List.Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            Label_Game_Settings.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            ComboBox_Language_List.Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            Button_Game_User_Settings.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_Clear_Crash_Logs.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            Button_Launcher_logs.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            Button_Clear_NFSWO_Logs.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            Button_Clear_Server_Mods.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            CheckBox_Word_Filter_Check.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            CheckBox_Proxy.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            CheckBox_RPC.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            CheckBox_Alt_WebCalls.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            CheckBox_Opt_Insider.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            CheckBox_Theme_Support.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            CheckBox_Stream.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Game_Current_Path.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            LinkLabel_Game_Path.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_CDN_Current.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            LinkLabel_CDN_Current.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Launcher_Path.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            LinkLabel_Launcher_Path.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_API_Status.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_API_Status_One.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_API_Status_Two.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_API_Status_Three.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_API_Status_Four.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Version_Build.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_Save.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Exit.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Theme_Name.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Theme_Author.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            /* Buttons */
            ButtonsColorSet(Button_Change_Game_Path, 0, true);
            ButtonsColorSet(Button_About, 0, true);
            ButtonsColorSet(Button_Game_Verify_Files, 0, false);
            ButtonsColorSet(Button_Game_User_Settings, 0, true);
            ButtonsColorSet(Button_Clear_Crash_Logs, 0, false);
            ButtonsColorSet(Button_Launcher_logs, 0, true);
            ButtonsColorSet(Button_Clear_NFSWO_Logs, 0, false);
            ButtonsColorSet(Button_Clear_Server_Mods, 0, false);
            ButtonsColorSet(Button_Security_Center, 0, true);

            /* Label Links */
            LinkLabel_Game_Path.LinkColor = Color_Winform_Other.Link_Settings;
            LinkLabel_Game_Path.ActiveLinkColor = Color_Winform_Other.Link_Settings_Active;
            LinkLabel_CDN_Current.LinkColor = Color_Winform_Other.Link_Settings;
            LinkLabel_CDN_Current.ActiveLinkColor = Color_Winform_Other.Link_Settings_Active;
            LinkLabel_Launcher_Path.LinkColor = Color_Winform_Other.Link_Settings;
            LinkLabel_Launcher_Path.ActiveLinkColor = Color_Winform_Other.Link_Settings_Active;

            /* Labels */
            Label_Game_Current_Path.ForeColor = Color_Text.L_Five;
            Label_Game_Current_Path.ForeColor = Color_Text.L_Five;
            Label_CDN_Current.ForeColor = Color_Text.L_Five;
            Label_Launcher_Path.ForeColor = Color_Text.L_Five;
            Label_CDN.ForeColor = Color_Text.L_Five;
            Label_Game_Settings.ForeColor = Color_Text.L_Five;
            Label_API_Status.ForeColor = Color_Text.L_Five;

            /* Check boxes */
            CheckBox_Word_Filter_Check.ForeColor = Color_Winform_Other.CheckBoxes_Settings;
            CheckBox_Proxy.ForeColor = Color_Winform_Other.CheckBoxes_Settings;
            CheckBox_RPC.ForeColor = Color_Winform_Other.CheckBoxes_Settings;
            CheckBox_Alt_WebCalls.ForeColor = Color_Winform_Other.CheckBoxes_Settings;
            CheckBox_Opt_Insider.ForeColor = Color_Winform_Other.CheckBoxes_Settings;
            CheckBox_Theme_Support.ForeColor = Color_Winform_Other.CheckBoxes_Settings;
            CheckBox_Stream.ForeColor = Color_Winform_Other.CheckBoxes_Settings;

            /* Bottom Left */
            Label_Version_Build.ForeColor = Color_Text.L_Five;
            Label_Theme_Name.ForeColor = Color_Text.L_Five;
            Label_Theme_Author.ForeColor = Color_Text.L_Five;

            /* Main Settings Buttons (Save or Cancel) */
            Button_Save.ForeColor = Color_Text.L_Seven;
            Button_Save.Image = Image_Button.Green;
            Button_Exit.Image = Image_Button.Grey;
            Button_Exit.ForeColor = Color_Text.L_One;

            /*******************************/
            /* Load CDN List                /
            /*******************************/

            if (!CDNListUpdater.LoadedList)
            {
                CDNListUpdater.GetList();
            }

            ComboBox_CDN_List.DisplayMember = "Name";
            ComboBox_CDN_List.DataSource = CDNListUpdater.CleanList;

            ComboBox_Language_List.DisplayMember = "Name";
            ComboBox_Language_List.DataSource = LanguageListUpdater.CleanList;


            /********************************/
            /* Events                        /
            /********************************/

            ComboBox_CDN_List.DrawItem += new DrawItemEventHandler(ComboBox_CDN_List_DrawItem);
            ComboBox_CDN_List.SelectedIndexChanged += new EventHandler(ComboBox_CDN_List_SelectedIndexChanged);
            ComboBox_CDN_List.MouseWheel += new MouseEventHandler(DropDownMenu_MouseWheel);
            ComboBox_Language_List.DrawItem += new DrawItemEventHandler(ComboBox_Language_List_DrawItem);
            ComboBox_Language_List.SelectedIndexChanged += new EventHandler(ComboBox_Language_List_SelectedIndexChanged);
            ComboBox_Language_List.MouseWheel += new MouseEventHandler(DropDownMenu_MouseWheel);

            Button_Save.MouseEnter += new EventHandler(Greenbutton_hover_MouseEnter);
            Button_Save.MouseLeave += new EventHandler(Greenbutton_MouseLeave);
            Button_Save.MouseUp += new MouseEventHandler(Greenbutton_hover_MouseUp);
            Button_Save.MouseDown += new MouseEventHandler(Greenbutton_click_MouseDown);

            Button_Exit.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            Button_Exit.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            Button_Exit.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            Button_Exit.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);

            Button_Security_Center.Click += new EventHandler(Button_Security_Center_Click);
            Button_Game_Verify_Files.Click += new EventHandler(Button_Game_Verify_Files_Click);
            Button_Save.Click += new EventHandler(SettingsSave_Click);
            Button_Exit.Click += new EventHandler(SettingsCancel_Click);
            Button_Game_User_Settings.Click += new EventHandler(SettingsUEditorButton_Click);
            Button_Clear_Server_Mods.Click += new EventHandler(SettingsClearServerModCacheButton_Click);
            Button_Clear_NFSWO_Logs.Click += new EventHandler(SettingsClearCommunicationLogButton_Click);
            Button_Clear_Crash_Logs.Click += new EventHandler(SettingsClearCrashLogsButton_Click);
            Button_About.Click += new EventHandler(SettingsAboutButton_Click);
            Label_Version_Build.Click += new EventHandler(SettingsLauncherVersion_Click);
            Button_Change_Game_Path.Click += new EventHandler(SettingsGameFiles_Click);
            Button_Launcher_logs.Click += new EventHandler(SettingsClearLauncherLogsButton_Click);

            Load += new EventHandler(Screen_Settings_Load);

            /********************************/
            /* Load XML (Only one Section)   /
            /********************************/

            FileGameSettings.Read("Language Only");

            /********************************/
            /* Sets Red Buttons/Disables     /
            /********************************/

            if (FunctionStatus.IsVerifyHashDisabled)
            {
                ButtonsColorSet(Button_Game_Verify_Files, 3, true);
            }

            /*******************************/
            /* Set ToolTip Texts            /
            /*******************************/

            ToolTip_Hover.SetToolTip(Button_Change_Game_Path, "Change the location of where the \'nfsw.exe\' that the Launcher will run");
            ToolTip_Hover.SetToolTip(Button_Game_Verify_Files, "Checks and Restores GameFiles back to \"Stock\"");
            ToolTip_Hover.SetToolTip(ComboBox_CDN_List, "Download Location for Fetching the base GameFiles\n" +
                "Can also be a Soruce for VerifyHash to get replacement files");
            ToolTip_Hover.SetToolTip(ComboBox_Language_List, "Controls the In-Game Lanuguage setting\n" +
                "This also includes setting the Default Chat joined In-Game");
            ToolTip_Hover.SetToolTip(Button_Game_User_Settings, "Opens a UserSettings.xml Editor\nAllows in-depth control over Game Settings");

            ToolTip_Hover.SetToolTip(Button_Clear_Crash_Logs, "Removes \"SBRCrashLogs_*\" DMP and TXT files from GameFiles Folder");

            ToolTip_Hover.SetToolTip(Button_Launcher_logs, "Removes all but current session \"LOGS\\\" folders");
            ToolTip_Hover.SetToolTip(Button_Clear_Server_Mods, "Erases all Server Mods from .data/MODS folders");
            ToolTip_Hover.SetToolTip(Button_Security_Center, "Opens a new Panel to review Security Information and Settings");

            ToolTip_Hover.SetToolTip(CheckBox_Word_Filter_Check, "Disables the In-Game Chat \"censor\" or word filter.");
            ToolTip_Hover.SetToolTip(CheckBox_Proxy, "Disables the Launcher Proxy communications hook.\n" +
                "Can not be turned off for httpS Servers.\n" +
                "Will also impact/limit the DiscordRPC functions.");
            ToolTip_Hover.SetToolTip(CheckBox_RPC, "Prevents Launcher from sending Discord Presence information.");

            ToolTip_Hover.SetToolTip(CheckBox_Opt_Insider, "Unchecked: Only Official \"Release\" Builds will prompt Updates\n" +
                "Checked: Insider/Beta Build\'s will be available to the Updater");
            ToolTip_Hover.SetToolTip(CheckBox_Theme_Support, "Enables supporting External Themes for the Launcher");
            ToolTip_Hover.SetToolTip(CheckBox_Stream, "Setting for Recording/Streaming Programs:\n" +
                "Enable \"Native\" capture of the NFSW Game Window\n" +
                "If Checked, this removes the Window Title countdown timer\n" +
                "If Unchecked, you can still capture, but may need special methods");
            ToolTip_Hover.SetToolTip(CheckBox_Alt_WebCalls, "Changes the internal method used by Launcher for Communications\n" +
                "Unchecked: Uses \'standard\' WebClient calls\n" +
                "Checked: Uses WebClientWithTimeout");

            Shown += (x, y) =>
            {
                RememberLastCDN();
                RememberLastLanguage();
                PingSavedCDN();
                PingAPIStatus();
            };
        }
        #endregion
        public Screen_Settings()
        {
            IsSettingsScreenOpen = true;
            InitializeComponent();
            Set_Visuals();
            this.Closing += (x, y) =>
            {
                Presence_Launcher.Status("Idle Ready", null);

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

                IsSettingsScreenOpen = false;

                /* This is for Mono Support */
                if (ToolTip_Hover.Active)
                {
                    ToolTip_Hover.RemoveAll();
                    ToolTip_Hover.Dispose();
                }

                GC.Collect();
            };

            Presence_Launcher.Status("Settings", null);
        }
    }
}
