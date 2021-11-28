using GameLauncher.App.Classes.Auth;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Client;
using GameLauncher.App.Classes.LauncherCore.Client.Auth;
using GameLauncher.App.Classes.LauncherCore.Downloader;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Languages.Visual_Forms;
using GameLauncher.App.Classes.LauncherCore.LauncherUpdater;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.LauncherCore.ModNet.JSON;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.App.UI_Forms.About_Screen;
using GameLauncher.App.UI_Forms.SecurityCenter_Screen;
using GameLauncher.App.UI_Forms.SelectServer_Screen;
using GameLauncher.App.UI_Forms.Settings_Screen;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Bitmap_;
using SBRW.Launcher.Core.Extension.Hash_;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.String_;
using SBRW.Launcher.Core.Extension.Time_;
using SBRW.Launcher.Core.Extension.Validation_;
using SBRW.Launcher.Core.Extension.Validation_.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Required.Anti_Cheat;
using SBRW.Launcher.Core.Discord.Reference_.List_;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Proxy.Nancy_;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using SBRW.Launcher.Core.Recommended.Process_;
using SBRW.Launcher.Core.Recommended.Time_;

namespace GameLauncher.App.UI_Forms.Main_Screen
{
    public partial class MainScreen : Form
    {
        private Point _mouseDownPoint { get; set; } = Point.Empty;
        private bool _loginEnabled { get; set; }
        private bool _serverEnabled { get; set; }
        private bool _builtinserver { get; set; }
        private bool _skipServerTrigger { get; set; }
        private bool _playenabled { get; set; }
        private bool _isDownloading { get; set; } = true;
        private bool _disableLogout { get; set; }
        private bool _restartLauncher { get; set; }

        public static string getTempNa { get; set; } = Path.GetTempFileName();

        private static int _lastSelectedServerId { get; set; }
        private static int _nfswPid { get; set; }
        private static Thread _nfswstarted { get; set; }
        private static bool StillCheckingLastServer { get; set; }
        private static bool ServerChangeTriggered { get; set; }

        private static DateTime _downloadStartTime { get; set; }
        private static Downloader _downloader { get; set; }

        private static string JsonGSI { get; set; }
        private static MemoryStream _serverRawBanner { get; set; }
        private string _loginWelcomeTime { get; set; }
        private string _loginToken { get; set; }
        private string _userId { get; set; }
        private static int serverSecondsToShutDown { get; set; }

        public static string ModNetFileNameInUse { get; set; }
        public static readonly Queue<Uri> modFilesDownloadUrls = new Queue<Uri>();
        public static bool isDownloadingModNetFiles { get; set; }
        public static int CurrentModFileCount { get; set; }
        public static int TotalModFileCount { get; set; }

        public static readonly string filename_pack = Path.Combine(Locations.LauncherFolder, "GameFiles.sbrwpack");

        private void MoveWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Y <= 90) _mouseDownPoint = new Point(e.X, e.Y);
        }

        private void MoveWindow_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDownPoint = Point.Empty;
            Opacity = 1;
        }

        private void MoveWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDownPoint.IsEmpty) { return; }
            var f = this as Form;
            f.Location = new Point(f.Location.X + (e.X - _mouseDownPoint.X), f.Location.Y + (e.Y - _mouseDownPoint.Y));
            InformationCache.ParentScreenLocation = new Point(f.Location.X + (e.X - _mouseDownPoint.X), f.Location.Y + (e.Y - _mouseDownPoint.Y));
            Opacity = 0.9;
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            Log.Visuals("CORE: Loading Main Screen");
            FunctionStatus.CenterScreen(this);
            Application.OpenForms[this.Name].Activate();
            Log.Core("CORE: Setting Parent Window location");
            InformationCache.ParentScreenLocation = Location;

            if (!string.IsNullOrWhiteSpace(InsiderInfo.BuildNumber()))
            {
                if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                {
                    InsiderBuildNumberText.Visible = true;
                    InsiderBuildNumberText.Text = InsiderInfo.BuildNumber();
                }
                else
                {
                    InsiderBuildNumberText.Visible = translatedBy.Visible = false;
                }
            }

            Log.Core("LAUNCHER: NFSW Download Source is now: " + Save_Settings.Live_Data.Launcher_CDN);

            MainEmail.Text = Save_Account.Live_Data.User_Raw_Email;
            MainPassword.Text = Save_Account.Live_Data.User_Raw_Password;

            Log.Core("LAUNCHER: Checking for password");
            if (!string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Raw_Password))
            {
                _loginEnabled = true;
                _serverEnabled = true;
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.ForeColor = Theming.FivithTextForeColor;
            }
            else
            {
                _loginEnabled = false;
                _serverEnabled = false;
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.ForeColor = Theming.SixTextForeColor;
            }

            if (!string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Raw_Email) &&
                (!string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Hashed_Password) || !string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Raw_Password)))
            {
                Log.Core("LAUNCHER: Restoring last saved email and password");
                RememberMe.Checked = true;
                Save_Account.SaveLoginInformation = true;
            }

            /* Server Display List */
            ServerPick.DisplayMember = "Name";
            Log.Core("LAUNCHER: Setting server list");
            ServerPick.DataSource = ServerListUpdater.CleanList;

            /* Display Server List Dialog if Server IP Doesn't Exist */
            if (string.IsNullOrWhiteSpace(Save_Account.Live_Data.Saved_Server_Address))
            {
                SelectServer.OpenScreen(false);

                if (SelectedServer.Data != null)
                {
                    Save_Account.Live_Data.Saved_Server_Address = SelectedServer.Data.IPAddress;
                    Save_Account.Save();
                }
                else
                {
                    FunctionStatus.LauncherForceClose = true;
                }
            }

            if (FunctionStatus.LauncherForceClose)
            {
                FunctionStatus.ErrorCloseLauncher("Closing From SelectServer Dialog", false);
            }
            else
            {
                Log.Core("SERVERLIST: Checking...");
                Log.Core("SERVERLIST: Setting first server in list");
                Log.Core("SERVERLIST: Checking if server is set on INI File");
                try
                {
                    if (string.IsNullOrWhiteSpace(Save_Account.Live_Data.Saved_Server_Address))
                    {
                        Log.Warning("SERVERLIST: Failed to find anything... assuming " + ((Json_List_Server)ServerPick.SelectedItem).IPAddress);
                        Save_Account.Live_Data.Saved_Server_Address = ((Json_List_Server)ServerPick.SelectedItem).IPAddress;
                        Save_Account.Save();
                    }
                }
                catch
                {
                    Log.Error("SERVERLIST: Failed to write anything...");
                    Save_Account.Live_Data.Saved_Server_Address = string.Empty;
                    Save_Account.Save();
                }

                Log.Core("SERVERLIST: Re-Checking if server is set on INI File");
                if (!string.IsNullOrWhiteSpace(Save_Account.Live_Data.Saved_Server_Address))
                {
                    Log.Core("SERVERLIST: Found something!");
                    _skipServerTrigger = true;

                    Log.Core("SERVERLIST: Checking if server exists on our database");

                    try
                    {
                        if (ServerListUpdater.CleanList.Count != 0)
                        {
                            if (ServerListUpdater.CleanList.FindIndex(i => string.Equals(i.IPAddress, Save_Account.Live_Data.Saved_Server_Address)) != 0)
                            {
                                Log.Core("SERVERLIST: Server found! Checking ID");
                                var index = ServerListUpdater.CleanList.FindIndex(i => string.Equals(i.IPAddress, Save_Account.Live_Data.Saved_Server_Address));

                                Log.Core("SERVERLIST: ID is " + index);
                                if (index >= 0)
                                {
                                    Log.Core("SERVERLIST: ID set correctly");
                                    ServerPick.SelectedIndex = index;
                                }
                                else
                                {
                                    ServerPick.SelectedIndex = 1;
                                }
                            }
                            else
                            {
                                Log.Warning("SERVERLIST: Unable to find anything, assuming default");
                                ServerPick.SelectedIndex = 1;
                                Log.Warning("SERVERLIST: Deleting unknown entry");
                                Save_Account.Live_Data.Saved_Server_Address = string.Empty;
                                Save_Account.Save();
                            }

                            Log.Core("SERVERLIST: Triggering server change");
                            if (ServerPick.SelectedIndex == 1)
                            {
                                ServerPick_SelectedIndexChanged(sender, e);
                            }

                            Log.Completed("SERVERLIST: All done");
                        }
                        else { ServerPick_SelectedIndexChanged(sender, e); Log.Completed("SERVERLIST: Empty List. Not Setting Index"); }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("Serverlist", null, Error, null, true);
                    }
                }

                Log.Core("LAUNCHER: Re-checking InstallationDirectory: " + Save_Settings.Live_Data.Game_Path);

                string Drive = Path.GetPathRoot(Save_Settings.Live_Data.Game_Path);
                if (!Directory.Exists(Drive))
                {
                    if (!string.IsNullOrWhiteSpace(Drive))
                    {
                        Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                        Save_Settings.Save();
                        string Display_Message = Translations.Database("MainScreen_TextBox_GameFiles_Invalid_Location");
                        Log.Error(string.Format("LAUNCHER: Drive {0} was not found. Your actual installation directory is set to {1} now.",
                            Drive, Locations.GameFilesFailSafePath));

                        string TempEmailCache = string.Empty;
                        if (!string.IsNullOrWhiteSpace(MainEmail.Text))
                        {
                            TempEmailCache = MainEmail.Text;
                            MainEmail.Text = "EMAIL IS HIDDEN";
                        }
                        MessageBox.Show(null, string.Format("Drive {0} was not found. Your actual installation directory is set to {1} now.",
                            Drive, Locations.GameFilesFailSafePath),
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (!string.IsNullOrWhiteSpace(TempEmailCache))
                        {
                            MainEmail.Text = TempEmailCache;
                        }
                    }
                }

                this.SafeEndInvokeAsyncCatch(this.SafeBeginInvokeActionAsync(Launcher_X_Form =>
                {
                    Log.Core("CORE: 'GetServerInformation' from all Servers in Server List and Download Selected Server Banners");
                    LaunchNfsw();
                }));
                this.BringToFront();

                try
                {
                    new LauncherUpdateCheck(LauncherIconStatus, LauncherStatusText, LauncherStatusDesc).ChangeVisualStatus();
                }
                catch
                {
                    if (UnixOS.Detected())
                    {
                        LauncherIconStatus.BackgroundImage = Theming.UpdateIconSuccess;
                        LauncherStatusText.ForeColor = Theming.Sucess;
                        LauncherStatusText.Text = "Launcher Status:\n - Linux Build";
                        LauncherStatusDesc.Text = "Version: v" + Application.ProductVersion;
                    }
                }

                PingServerListAPIStatus();

                Log.Visuals("CORE: Applyinng ContextMenu");
                ContextMenu = new ContextMenu();
                ContextMenu.MenuItems.Add(new MenuItem("About", (O, K) => { About.OpenScreen(); }));
                if (LauncherUpdateCheck.UpgradeAvailable)
                {
                    ContextMenu.MenuItems.Add("-");
                    ContextMenu.MenuItems.Add(new MenuItem("Obsolete", (N, O) => { Process.Start("https://www.youtube.com/watch?v=LutDfASARmE"); }));
                }
                ContextMenu.MenuItems.Add("-");
                ContextMenu.MenuItems.Add(new MenuItem("Close Launcher", CloseBTN_Click));

                Notification.ContextMenu = ContextMenu;
                Notification.Icon = new Icon(Icon, Icon.Width, Icon.Height);
                Notification.Text = "SBRW Launcher";
                Notification.Visible = true;

                ContextMenu = null;

                /* Remove TracksHigh Folder and Files */
                RemoveTracksHighFiles();
            }
        }

        private void ClosingTasks()
        {
            Save_Settings.Save();
            Save_Account.Save();

            try
            { _downloader.Stop(); }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("CDN DOWNLOADER", null, Error, null, true);
            }

            try
            {
                if (FunctionStatus.LauncherBattlePass)
                {
                    Process.GetProcessById(_nfswPid).Kill();
                }
                else
                {
                    Process[] allOfThem = Process.GetProcessesByName("nfsw");

                    if (allOfThem != null && allOfThem.Any())
                    {
                        foreach (var oneProcess in allOfThem)
                        {
                            Process.GetProcessById(oneProcess.Id).Kill();
                        }
                    }
                }
            }
            catch { }

            if (Presence_Launcher.Running())
            {
                Presence_Launcher.Stop("Close");
            }

            if (Proxy_Settings.Running())
            {
                Proxy_Server.Instance.Stop("Main Screen");
            }

            try { Notification.Dispose(); } catch { }

            if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)) && !FunctionStatus.LauncherBattlePass)
            {
                ModNetHandler.CleanLinks(Save_Settings.Live_Data.Game_Path);
            }
        }

        private void CloseBTN_Click(object sender, EventArgs e)
        {
            ClosingTasks();

            /* Leave this here. Its to properly close the launcher from Visual Studio (And Close the Launcher a well) 
             * If the Boolen is true it will restart the Application
             */
            if (_restartLauncher)
            {
                Application.Restart();
            }
            else
            {
                try { this.Close(); } catch { }
            }
        }

        private void ButtonClose_MouseDown(object sender, EventArgs e)
        {
            CloseBTN.BackgroundImage = Theming.CloseButtonClick;
        }

        private void ButtonClose_MouseEnter(object sender, EventArgs e)
        {
            CloseBTN.BackgroundImage = Theming.CloseButtonHover;
        }

        private void ButtonClose_MouseLeaveANDMouseUp(object sender, EventArgs e)
        {
            CloseBTN.BackgroundImage = Theming.CloseButton;
        }

        private void ButtonSecurityCenter_MouseDown(object sender, EventArgs e)
        {
            ButtonSecurityCenter.BackgroundImage = SecurityCenter.SecurityCenterIcon(1);
        }

        private void ButtonSecurityCenter_MouseEnter(object sender, EventArgs e)
        {
            ButtonSecurityCenter.BackgroundImage = SecurityCenter.SecurityCenterIcon(2);
        }

        private void ButtonSecurityCenter_MouseLeaveANDMouseUp(object sender, EventArgs e)
        {
            ButtonSecurityCenter.BackgroundImage = SecurityCenter.SecurityCenterIcon(0);
        }

        private void ButtonSettings_MouseDown(object sender, EventArgs e)
        {
            SettingsButton.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Bad") ? Theming.GearButtonWarningClick : Theming.GearButtonClick;
        }

        private void ButtonSettings_MouseEnter(object sender, EventArgs e)
        {
            SettingsButton.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Bad") ? Theming.GearButtonWarningHover : Theming.GearButtonHover;
        }

        private void ButtonSettings_MouseLeaveANDMouseUp(object sender, EventArgs e)
        {
            SettingsButton.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Bad") ? Theming.GearButtonWarning : Theming.GearButton;
        }

        private void LoginEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && _loginEnabled)
            {
                LoginButton_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }

        private void Loginbuttonenabler(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MainEmail.Text) || string.IsNullOrWhiteSpace(MainPassword.Text))
            {
                _loginEnabled = false;
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.ForeColor = Theming.SixTextForeColor;
            }
            else
            {
                _loginEnabled = true;
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.ForeColor = Theming.FivithTextForeColor;
            }
        }

        private void LoginButton_MouseUp(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                LoginButton.BackgroundImage = Theming.GrayButtonHover;
            }
            else
            {
                LoginButton.BackgroundImage = Theming.GrayButton;
            }
        }

        private void LoginButton_MouseDown(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                LoginButton.BackgroundImage = Theming.GrayButtonClick;
            }
            else
            {
                LoginButton.BackgroundImage = Theming.GrayButton;
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if ((_loginEnabled == false || _serverEnabled == false) && _builtinserver == false)
            {
                return;
            }

            if (_isDownloading)
            {
                string TempEmailCache = string.Empty;
                if (!string.IsNullOrWhiteSpace(MainEmail.Text))
                {
                    TempEmailCache = MainEmail.Text;
                    MainEmail.Text = "EMAIL IS HIDDEN";
                }
                MessageBox.Show(null, "Please wait while the GameLauncher is still downloading the game files.", "GameLauncher", MessageBoxButtons.OK);
                if (!string.IsNullOrWhiteSpace(TempEmailCache))
                {
                    MainEmail.Text = TempEmailCache;
                }

                return;
            }

            Tokens.Clear();

            string Email;
            string Password;

            Tokens.IPAddress = Launcher_Value.Launcher_Select_Server_Data.IPAddress;
            Tokens.ServerName = ServerListUpdater.ServerName("Login");

            switch (Authentication.HashType(Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Version ?? string.Empty))
            {
                case AuthHash.H10:
                    Email = MainEmail.Text.ToString();
                    Password = MainPassword.Text.ToString();
                    break;
                case AuthHash.H11:
                    Email = MainEmail.Text.ToString();
                    Password = Hashes.Hash_String(0, MainPassword.Text.ToString()).ToLower();
                    break;
                case AuthHash.H12:
                    Email = MainEmail.Text.ToString();
                    Password = Hashes.Hash_String(1, MainPassword.Text.ToString()).ToLower();
                    break;
                case AuthHash.H13:
                    Email = MainEmail.Text.ToString();
                    Password = Hashes.Hash_String(2, MainPassword.Text.ToString()).ToLower();
                    break;
                case AuthHash.H20:
                    Email = Hashes.Hash_String(0, MainEmail.Text.ToString()).ToLower();
                    Password = Hashes.Hash_String(0, MainPassword.Text.ToString()).ToLower();
                    break;
                case AuthHash.H21:
                    Email = Hashes.Hash_String(1, MainEmail.Text.ToString()).ToLower();
                    Password = Hashes.Hash_String(1, MainPassword.Text.ToString()).ToLower();
                    break;
                case AuthHash.H22:
                    Email = Hashes.Hash_String(2, MainEmail.Text.ToString()).ToLower();
                    Password = Hashes.Hash_String(2, MainPassword.Text.ToString()).ToLower();
                    break;
                default:
                    Log.Error("HASH TYPE: Unknown Hash Standard was Provided");
                    return;
            }

            Authentication.Client("Login", Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Post, Email, Password, null);

            if (string.IsNullOrWhiteSpace(Tokens.Error))
            {
                try
                {
                    if (!(ServerPick.SelectedItem is Json_List_Server server)) return;
                    Save_Account.Live_Data.Saved_Server_Address = server.IPAddress;
                }
                catch { }

                _userId = Tokens.UserId;
                _loginToken = Tokens.LoginToken;
                Launcher_Value.Launcher_Select_Server_Data.IPAddress = Tokens.IPAddress;

                /* Tells the FileAccountSave to Actually Save the Information or Not */
                Save_Account.SaveLoginInformation = RememberMe.Checked;
                Save_Account.Live_Data.User_Raw_Email = MainEmail.Text.ToString();
                Save_Account.Live_Data.User_Raw_Password = MainPassword.Text.ToString();

                switch (Authentication.HashType(Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Version ?? string.Empty))
                {
                    case AuthHash.H10:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "1.0";
                        Save_Account.Live_Data.User_Hashed_Email = string.Empty;
                        Save_Account.Live_Data.User_Hashed_Password = string.Empty;
                        break;
                    case AuthHash.H11:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "1.1";
                        Save_Account.Live_Data.User_Hashed_Email = string.Empty;
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(0, MainPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H12:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "1.2";
                        Save_Account.Live_Data.User_Hashed_Email = string.Empty;
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(1, MainPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H13:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "1.3";
                        Save_Account.Live_Data.User_Hashed_Email = string.Empty;
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(2, MainPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H20:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "2.0";
                        Save_Account.Live_Data.User_Hashed_Email = Hashes.Hash_String(0, MainEmail.Text.ToString()).ToLower();
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(0, MainPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H21:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "2.1";
                        Save_Account.Live_Data.User_Hashed_Email = Hashes.Hash_String(1, MainEmail.Text.ToString()).ToLower();
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(1, MainPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H22:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "2.2";
                        Save_Account.Live_Data.User_Hashed_Email = Hashes.Hash_String(2, MainEmail.Text.ToString()).ToLower();
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(2, MainPassword.Text.ToString()).ToLower();
                        break;
                    default:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "Unknown";
                        Save_Account.Live_Data.User_Hashed_Email = string.Empty;
                        Save_Account.Live_Data.User_Hashed_Password = string.Empty;
                        Log.Error("HASH TYPE: Unknown Hash Standard was Provided");
                        return;
                }

                Save_Account.Save();

                if (!string.IsNullOrWhiteSpace(Tokens.Warning))
                {
                    MainEmail.Text = "EMAIL IS HIDDEN";
                    MessageBox.Show(null, Tokens.Warning, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MainEmail.Text = Email;
                }

                LoginFormElements(false);
                LoggedInFormElements(true);
            }
            else
            {
                /* Main Screen Login */
                MainEmailBorder.Image = Theming.BorderEmailError;
                MainPasswordBorder.Image = Theming.BorderPasswordError;
                MainEmail.Text = "EMAIL IS HIDDEN";
                MessageBox.Show(null, Tokens.Error, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MainEmail.Text = Email;
            }
        }

        private void LoginButton_MouseEnter(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                LoginButton.BackgroundImage = Theming.GrayButtonHover;
                LoginButton.ForeColor = Theming.FivithTextForeColor;
            }
            else
            {
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.ForeColor = Theming.SixTextForeColor;
            }
        }

        private void LoginButton_MouseLeave(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.ForeColor = Theming.FivithTextForeColor;
            }
            else
            {
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.ForeColor = Theming.SixTextForeColor;
            }
        }

        private void ServerPick_SelectedIndexChanged(object sender, EventArgs e)
        {
            GC.Collect();
            MainEmailBorder.Image = Theming.BorderEmail;
            MainPasswordBorder.Image = Theming.BorderPassword;
            /* Disable Certain Functions */
            _loginEnabled = false;
            _serverEnabled = false;
            FunctionStatus.AllowRegistration = false;
            Launcher_Value.Launcher_Select_Server_JSON = null;
            /* Disable Login & Register Button */
            LoginButton.Enabled = false;
            RegisterText.Enabled = false;
            /* Disable Social Panel when switching */
            DisableSocialPanelandClearIt();
            /* Stops any actions for a Server */
            ServerChangeTriggered = true;

            if (!ServerListUpdater.LoadedList && Launcher_Value.Launcher_Select_Server_Data == null)
            {
                ServerStatusText.Text = "Launcher Offline:\n - Unknown";
                ServerStatusText.ForeColor = Theming.ThirdTextForeColor;
                ServerStatusDesc.Text = string.Empty;
                ServerStatusIcon.BackgroundImage = Theming.ServerIconUnkown;
                return;
            }

            Launcher_Value.Launcher_Select_Server_Data = (Json_List_Server)ServerPick.SelectedItem;

            if (Launcher_Value.Launcher_Select_Server_Data.IsSpecial)
            {
                ServerPick.SelectedIndex = _lastSelectedServerId;
                return;
            }

            if (!_skipServerTrigger) { return; }

            _lastSelectedServerId = ServerPick.SelectedIndex;

            ServerStatusText.Text = "Server Status:\n - Pinging";
            ServerStatusText.ForeColor = Theming.SecondaryTextForeColor;
            ServerStatusDesc.Text = string.Empty;
            ServerStatusIcon.BackgroundImage = Theming.ServerIconChecking;

            LoginButton.ForeColor = Theming.SixTextForeColor;
            string BannerCache = Path.Combine(".BannerCache", Hashes.Hash_String(1, Launcher_Value.Launcher_Select_Server_Data.IPAddress) + ".bin");
            Banner.Image = Bitmap_Handler.Grayscale(BannerCache);
            Banner.BackColor = Color.Transparent;
            string ImageUrl = string.Empty;
            string numPlayers = string.Empty;
            string numRegistered = string.Empty;

            if (ServerPick.GetItemText(ServerPick.SelectedItem) == "Offline Built-In Server")
            {
                _builtinserver = true;
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.Text = "Launch".ToUpper();
                LoginButton.ForeColor = Theming.FivithTextForeColor;
                ServerInfoPanel.Visible = false;
            }
            else
            {
                _builtinserver = false;
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.Text = "Login".ToUpper();
                LoginButton.ForeColor = Theming.SixTextForeColor;
                ServerInfoPanel.Visible = false;
            }

            Uri ServerURI = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress + "/GetServerInformation");
            ServicePointManager.FindServicePoint(ServerURI).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            var Client = new WebClient
            {
                Encoding = Encoding.UTF8
            };

            if (!Launcher_Value.Launcher_Alternative_Webcalls()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
            else
            {
                Client.Headers.Add("user-agent", "SBRW Launcher " +
                Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
            }

            Client.DownloadStringAsync(ServerURI);

            System.Timers.Timer aTimer = new System.Timers.Timer(10000);
            aTimer.Elapsed += (x, y) => { Client.CancelAsync(); try { aTimer.Dispose(); } catch { } };
            aTimer.AutoReset = false;
            aTimer.Enabled = true;

            Client.DownloadStringCompleted += (sender2, e2) =>
            {
                aTimer.Enabled = false;
                try { aTimer.Dispose(); } catch { }

                bool GSIErrorFree = true;

                if (e2.Cancelled || e2.Error != null)
                {
                    Launcher_Value.Launcher_Select_Server_JSON = null;
                    ServerStatusIcon.BackgroundImage = Theming.ServerIconOffline;
                    ServerStatusText.Text = "Server Status:\n - Offline ( OFF )";
                    ServerStatusText.ForeColor = Theming.Error;
                    ServerStatusDesc.Text = (e2.Error != null) ?
                    Strings.Encode(e2.Error.Message ?? "Server Seems to be Offline") : "Failed to Connect to Server";

                    if (!InformationCache.ServerStatusBook.ContainsKey(Launcher_Value.Launcher_Select_Server_Data.ID))
                    {
                        InformationCache.ServerStatusBook.Add(Launcher_Value.Launcher_Select_Server_Data.ID, (e2.Error != null) ? 0 : 3);
                    }
                    else
                    {
                        InformationCache.ServerStatusBook[Launcher_Value.Launcher_Select_Server_Data.ID] = (e2.Error != null) ? 0 : 3;
                    }

                    if (e2.Error != null)
                    {
                        LogToFileAddons.OpenLog("JSON GSI", null, e2.Error, null, true);
                    }

                    if (Client != null)
                    {
                        Client.Dispose();
                    }
                }
                else
                {
                    if (ServerListUpdater.ServerName("Ping") == "Offline Built-In Server")
                    {
                        numPlayers = "∞";
                        numRegistered = "∞";
                    }
                    else
                    {
                        try
                        {
                            Launcher_Value.Launcher_Select_Server_JSON = JsonConvert.DeserializeObject<Json_Server_Info>(e2.Result);
                        }
                        catch (Exception Error)
                        {
                            if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                            {
                                try { Log.Error("JSON GSI (Received): " + e2.Result); }
                                catch { Log.Error("JSON GSI (Received): Unable to Get Result"); }
                            }
                            else
                            {
                                Log.Error("JSON GSI: Invalid");
                            }

                            LogToFileAddons.OpenLog("JSON GSI", null, Error, null, true);
                            GSIErrorFree = false;
                            Launcher_Value.Launcher_Select_Server_JSON = null;
                        }

                        if (!InformationCache.ServerStatusBook.ContainsKey(Launcher_Value.Launcher_Select_Server_Data.ID))
                        {
                            InformationCache.ServerStatusBook.Add(Launcher_Value.Launcher_Select_Server_Data.ID, (!GSIErrorFree) ? 3 : 1);
                        }
                        else
                        {
                            InformationCache.ServerStatusBook[Launcher_Value.Launcher_Select_Server_Data.ID] = (!GSIErrorFree) ? 3 : 1;
                        }

                        if (GSIErrorFree)
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Banner))
                                {
                                    bool ServerBannerResult;

                                    try
                                    {
                                        ServerBannerResult = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Banner, UriKind.Absolute, out Uri uriResult) &&
                                        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                    }
                                    catch { ServerBannerResult = false; }

                                    ImageUrl = ServerBannerResult ? Launcher_Value.Launcher_Select_Server_JSON.Server_Banner : string.Empty;
                                }
                                else
                                {
                                    ImageUrl = string.Empty;
                                }
                            }
                            catch { }

                            /* Social Panel Core */

                            /* Discord Invite Display */
                            try
                            {
                                bool ServerDiscordLink;
                                try
                                {
                                    ServerDiscordLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord, UriKind.Absolute, out Uri uriResult) &&
                                                             (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch { ServerDiscordLink = false; }
                                DiscordIcon.BackgroundImage = ServerDiscordLink ? Theming.DiscordIcon : Theming.DiscordIconDisabled;
                                DiscordInviteLink.Enabled = ServerDiscordLink;
                                DiscordInviteLink.Text = ServerDiscordLink ? "Discord Invite" : string.Empty;
                            }
                            catch { }

                            /* Homepage Display */
                            try
                            {
                                bool ServerWebsiteLink;
                                try
                                {
                                    ServerWebsiteLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home, UriKind.Absolute, out Uri uriResult) &&
                                              (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch { ServerWebsiteLink = false; }
                                HomePageIcon.BackgroundImage = ServerWebsiteLink ? Theming.HomeIcon : Theming.HomeIconDisabled;
                                HomePageLink.Enabled = ServerWebsiteLink;
                                HomePageLink.Text = ServerWebsiteLink ? "Home Page" : string.Empty;
                            }
                            catch { }

                            /* Facebook Group Display */
                            try
                            {
                                bool ServerFacebookLink;
                                try
                                {
                                    ServerFacebookLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Facebook, UriKind.Absolute, out Uri uriResult) &&
                                                         (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch { ServerFacebookLink = false; }
                                FacebookIcon.BackgroundImage = ServerFacebookLink ? Theming.FacebookIcon : Theming.FacebookIconDisabled;
                                FacebookGroupLink.Enabled = ServerFacebookLink;
                                FacebookGroupLink.Text = ServerFacebookLink ? "Facebook Page" : string.Empty;
                            }
                            catch { }

                            /* Twitter Account Display */
                            try
                            {
                                bool ServerTwitterLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Twitter, UriKind.Absolute, out Uri uriResult) &&
                                                         (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                TwitterIcon.BackgroundImage = ServerTwitterLink ? Theming.TwitterIcon : Theming.TwitterIconDisabled;
                                TwitterAccountLink.Enabled = ServerTwitterLink;
                                TwitterAccountLink.Text = ServerTwitterLink ? "Twitter Feed" : string.Empty;
                            }
                            catch { }

                            /* Server Set Speedbug Timer Display */
                            try
                            {
                                serverSecondsToShutDown =
                                (Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer != 0) ? Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer : 7200;
                                ServerShutDown.Text = string.Format(Translations.Database("MainScreen_Text_ServerShutDown") +
                                    " " + Time_Conversion.RelativeTime(serverSecondsToShutDown));
                            }
                            catch { }

                            try
                            {
                                /* Scenery Group Display */
                                string SceneryStatus;
                                switch (string.Join("", Launcher_Value.Launcher_Select_Server_JSON.Server_Active_Scenery))
                                {
                                    case "SCENERY_GROUP_NEWYEARS":
                                        SceneryStatus = "Scenery: New Years";
                                        break;
                                    case "SCENERY_GROUP_OKTOBERFEST":
                                        SceneryStatus = "Scenery: Oktoberfest";
                                        break;
                                    case "SCENERY_GROUP_HALLOWEEN":
                                        SceneryStatus = "Scenery: Halloween";
                                        break;
                                    case "SCENERY_GROUP_CHRISTMAS":
                                        SceneryStatus = "Scenery: Christmas";
                                        break;
                                    case "SCENERY_GROUP_VALENTINES":
                                        SceneryStatus = "Scenery: Valentines";
                                        break;
                                    default:
                                        SceneryStatus = "Scenery: Normal";
                                        break;
                                }
                                SceneryGroupText.Text = SceneryStatus;
                            }
                            catch { }

                            /* Check Selected Server Address for since nfsw currently requires being proxied for https */
                            if (Launcher_Value.Launcher_Select_Server_Data.IPAddress.StartsWith("https"))
                            {
                                if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                                {
                                    Log.Debug("Server is Https");
                                }
                                /* This is in case it is not listed in GSI at all || is present in GSI and is set to true */
                                if ((!Launcher_Value.Launcher_Select_Server_JSON.Server_Proxy_Forced) || (Launcher_Value.Launcher_Select_Server_JSON.Server_Proxy_Forced != false))
                                {   /* So we can force the Proxy On even if a User has Disabled it */
                                    InformationCache.SelectedServerEnforceProxy = true;
                                    if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                                    {
                                        Log.Debug("Server is Https: Case 1");
                                    }
                                }
                                /* but still allow that nfsw might get patched to do https raw? */
                                else if (Launcher_Value.Launcher_Select_Server_JSON.Server_Proxy_Forced == false)
                                {   /* In which case, respect the GSI set value */
                                    InformationCache.SelectedServerEnforceProxy = false;
                                    if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                                    {
                                        Log.Debug("Server is Https: Case 2 (Never Gets Touched Because of First If Statement)");
                                    }
                                }
                            }
                            /* If it's an HTTP Server, check if Proxy is being requested as Enforced On */
                            else if (Launcher_Value.Launcher_Select_Server_JSON.Server_Proxy_Forced != true)
                            {   /* This is set so that it doesn't try to enforce Proxy On if user switches
                                 * to a server that doesn't have enforceLauncherProxy set or true */
                                InformationCache.SelectedServerEnforceProxy = false;
                                if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                                {
                                    Log.Debug("Server is not Https: Case 3");
                                }
                            }

                            Launcher_Value.Launcher_Server_Crew_Tags = Launcher_Value.Launcher_Select_Server_JSON.Server_Enable_Crew_Tags;

                            if (Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Peak != 0)
                            {
                                numPlayers = string.Format("{0} / {1}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online, Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Peak);
                                numRegistered = string.Format("{0}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Registered);
                            }
                            else if (Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Max != 0)
                            {
                                numPlayers = string.Format("{0} / {1}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online, Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Max);
                                numRegistered = string.Format("{0}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Registered);
                            }
                            else if ((Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Max == 0) || (Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Peak == 0))
                            {
                                numPlayers = string.Format("{0}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online);
                                numRegistered = string.Format("{0}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Registered);
                            }

                            FunctionStatus.AllowRegistration = true;
                        }
                    }

                    if (!GSIErrorFree)
                    {
                        try
                        {
                            ServerStatusText.Text = "Server Connection:\n - Unstable";
                            ServerStatusText.ForeColor = Theming.Warning;
                            ServerStatusDesc.Text = "Recevied Invalid JSON Game Server Info.";
                            ServerStatusIcon.BackgroundImage = Theming.ServerIconWarning;
                        }
                        catch { /* Sad Noises */ }
                    }
                    else
                    {
                        try
                        {
                            ServerStatusText.Text = "Server Status:\n - Online ( ON )";
                            ServerStatusText.ForeColor = Theming.Sucess;
                            ServerStatusIcon.BackgroundImage = Theming.ServerIconSuccess;
                            /* Enable Login & Register Button */
                            _loginEnabled = true;
                            LoginButton.ForeColor = Theming.FivithTextForeColor;
                            LoginButton.Enabled = true;
                            RegisterText.Enabled = true;
                            Launcher_Value.Launcher_Select_Server_Category = ((Json_List_Server)ServerPick.SelectedItem).Category ?? string.Empty;
                            Session_Timer.Remaining = (Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer != 0) ? Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer : 2 * 60 * 60;

                            if (Launcher_Value.Launcher_Select_Server_Category.ToUpper() == "DEV" ||
                            Launcher_Value.Launcher_Select_Server_Category.ToUpper() == "OFFLINE")
                            {
                                /* Disable Social Panel */
                                DisableSocialPanelandClearIt();
                            }
                            else
                            {
                                /* Enable Social Panel  */
                                ServerInfoPanel.Visible = true;
                            }
                        }
                        catch { /* ¯\_(ツ)_/¯ */ }
                        finally
                        {
                            if (Client != null)
                            {
                                Client.Dispose();
                            }
                        }

                        /* For Thread Safety */
                        try
                        {
                            ServerStatusDesc.Text = string.Format("Online: {0}\nRegistered: {1}", numPlayers, numRegistered);
                        }
                        catch { }

                        Ping CheckMate = null;

                        try
                        {
                            ServerPingStatusText.Text = string.Empty;
                            CheckMate = new Ping();
                            CheckMate.PingCompleted += (_sender, _e) =>
                            {
                                if (_e.Cancelled)
                                {
                                    Log.Warning("SERVER PING: Ping Canceled for " + ServerListUpdater.ServerName("Ping"));
                                }
                                else if (_e.Error != null)
                                {
                                    Log.Error("SERVER PING: Ping Failed for " + ServerListUpdater.ServerName("Ping") + " -> " + _e.Error.ToString());
                                }
                                else if (_e.Reply != null)
                                {
                                    if (_e.Reply.Status == IPStatus.Success && ServerListUpdater.ServerName("Ping") != "Offline Built-In Server")
                                    {
                                        ServerPingStatusText.Text = string.Format("Your Ping to the Server \n{0}".ToUpper(), _e.Reply.RoundtripTime + "ms");
                                        Log.Info("SERVER PING: " + _e.Reply.RoundtripTime + "ms for " + ServerListUpdater.ServerName("Ping"));
                                    }
                                    else
                                    {
                                        Log.Warning("SERVER PING: " + ServerListUpdater.ServerName("Ping") + " is " + _e.Reply.Status);
                                    }
                                }
                                else
                                {
                                    Log.Warning("SERVER PING:  Unable to Ping " + ServerListUpdater.ServerName("Ping"));
                                }

                                ((AutoResetEvent)_e.UserState).Set();
                            };

                            CheckMate.SendAsync(ServerURI.Host, 5000, new byte[1], new PingOptions(30, true), new AutoResetEvent(false));
                        }
                        catch (PingException Error)
                        {
                            LogToFileAddons.OpenLog("Pinging", null, Error, null, true);
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("Ping", null, Error, null, true);
                        }
                        finally
                        {
                            if (CheckMate != null)
                            {
                                CheckMate.Dispose();
                            }
                        }

                        _serverEnabled = true;

                        try
                        {
                            if (!Directory.Exists(".BannerCache")) { Directory.CreateDirectory(".BannerCache"); }

                            if (!string.IsNullOrWhiteSpace(ImageUrl))
                            {

                                Uri URICall_A = new Uri(ImageUrl);
                                ServicePointManager.FindServicePoint(URICall_A).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                var Client_A = new WebClient
                                {
                                    Encoding = Encoding.UTF8
                                };

                                if (!Launcher_Value.Launcher_Alternative_Webcalls()) { Client_A = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                                else
                                {
                                    Client_A.Headers.Add("user-agent", "SBRW Launcher " +
                                    Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                }

                                Client_A.DownloadDataAsync(URICall_A);
                                Client_A.DownloadProgressChanged += (Object_A, Events_A) =>
                                {
                                    if (ServerChangeTriggered)
                                    {
                                        Client_A.CancelAsync();
                                        Log.Info("BANNER: Stopping " + ServerListUpdater.ServerName("Ping") + " Server Banner Download");
                                    }
                                    else if (Events_A.TotalBytesToReceive > 2000000)
                                    {
                                        Client_A.CancelAsync();
                                        Log.Warning("BANNER: Unable to Cache " + ServerListUpdater.ServerName("Ping") + " Server Banner! {Over 2MB?}");
                                    }
                                };

                                Client_A.DownloadDataCompleted += (Object_A, Events_A) =>
                                {
                                    if (Events_A.Cancelled)
                                    {
                                        if (Client_A != null)
                                        {
                                            Client_A.Dispose();
                                        }
                                    }
                                    else if (Events_A.Error != null)
                                    {
                                        if (!ServerChangeTriggered)
                                        {
                                            /* Load cached banner! */
                                            Banner.Image = Bitmap_Handler.Grayscale(BannerCache);
                                            GC.Collect();
                                        }

                                        if (Client_A != null)
                                        {
                                            Client_A.Dispose();
                                        }
                                    }
                                    else if (!ServerChangeTriggered && Events_A.Result != null)
                                    {
                                        try
                                        {
                                            try
                                            {
                                                if (_serverRawBanner != null)
                                                {
                                                    _serverRawBanner.Dispose();
                                                    _serverRawBanner.Close();
                                                }
                                            }
                                            catch { }

                                            _serverRawBanner = new MemoryStream(Events_A.Result)
                                            {
                                                Position = 0
                                            };

                                            Banner.Image = Image.FromStream(_serverRawBanner);

                                            if (Strings.GetExtension(ImageUrl) == "gif")
                                            {
                                                Image.FromStream(_serverRawBanner).Save(BannerCache);
                                            }
                                            else
                                            {
                                                File.WriteAllBytes(BannerCache, _serverRawBanner.ToArray());
                                            }
                                        }
                                        catch (Exception Error)
                                        {
                                            LogToFileAddons.OpenLog("Server Banner", null, Error, null, true);
                                            Banner.BackColor = Theming.BannerBackColor;
                                        }
                                        finally
                                        {
                                            if (Client_A != null)
                                            {
                                                Client_A.Dispose();
                                            }

                                            GC.Collect();
                                        }
                                    }
                                };
                            }
                            else if (File.Exists(BannerCache) && !Application.OpenForms[this.Name].IsDisposed)
                            {
                                /* Load cached banner! */
                                Banner.Image = Bitmap_Handler.Grayscale(BannerCache);
                                GC.Collect();
                            }
                            else if (!Application.OpenForms[this.Name].IsDisposed)
                            {
                                Banner.BackColor = Theming.BannerBackColor;
                                GC.Collect();
                            }
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("BANNER Cache", null, Error, null, true);
                        }

                        ServerChangeTriggered = false;
                    }
                }

                if (Application.OpenForms[this.Name] != null)
                {
                    GC.Collect();
                }
            };

            GC.Collect();
        }

        /* Main Screen Elements */

        /* Social Panel | Ping or Offline or DEV Servers | */
        private void DisableSocialPanelandClearIt()
        {
            /* Hides Social Panel */
            ServerInfoPanel.Visible = false;
            /* Home */
            HomePageIcon.BackgroundImage = Theming.HomeIconDisabled;
            HomePageLink.Enabled = false;
            /* Discord */
            DiscordIcon.BackgroundImage = Theming.DiscordIconDisabled;
            DiscordInviteLink.Enabled = false;
            /* Facebook */
            FacebookIcon.BackgroundImage = Theming.FacebookIconDisabled;
            FacebookGroupLink.Enabled = false;
            /* Twitter */
            TwitterIcon.BackgroundImage = Theming.TwitterIconDisabled;
            TwitterAccountLink.Enabled = false;
            /* Scenery */
            SceneryGroupText.Text = "But It's Me!";
            /* Restart Timer */
            ServerShutDown.Text = "Game Launcher!";
        }

        /*  After Successful Login, Hide Login Forms */
        private void LoggedInFormElements(bool hideElements)
        {
            if (hideElements == true)
            {
                try
                {
                    DateTime currentTime = DateTime.Now;

                    if ((currentTime.Hour >= 5) && (currentTime.Hour < 12))
                    {
                        _loginWelcomeTime = "Good Morning";
                    }
                    else if ((currentTime.Hour >= 12) && (currentTime.Hour < 18))
                    {
                        _loginWelcomeTime = "Good Afternoon";
                    }
                    else if ((currentTime.Hour >= 18) && (currentTime.Hour < 22))
                    {
                        _loginWelcomeTime = "Good Evening";
                    }
                    else
                    {
                        _loginWelcomeTime = "Hello Night Owl";
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LOGIN TIME", null, Error, null, true);
                    _loginWelcomeTime = "Pshhh Pshhh";
                }

                CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", MainEmail.Text).ToUpper();

                PlayButton.BackgroundImage = Theming.PlayButton;
                PlayButton.ForeColor = Theming.FivithTextForeColor;

                LogoutButton.BackgroundImage = Theming.GrayButton;
                LogoutButton.ForeColor = Theming.FivithTextForeColor;
            }

            ShowPlayPanel.Visible = hideElements;
        }

        private void LoginFormElements(bool hideElements)
        {
            if (hideElements == true)
            {
                CurrentWindowInfo.Text = "Enter Your Account Information to Log In".ToUpper();
            }

            RememberMe.Visible = hideElements;
            LoginButton.Visible = hideElements;

            RegisterText.Visible = hideElements;
            MainEmail.Visible = hideElements;
            MainPassword.Visible = hideElements;
            ForgotPassword.Visible = hideElements;
            SettingsButton.Visible = hideElements;
            ButtonSecurityCenter.Visible = hideElements;

            AddServer.Enabled = hideElements;
            ServerPick.Enabled = hideElements;

            /* Input Strokes */
            MainEmailBorder.Visible = hideElements;
            MainEmailBorder.Image = Theming.BorderEmail;
            MainPasswordBorder.Visible = hideElements;
            MainPasswordBorder.Image = Theming.BorderPassword;
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            if (_disableLogout == true)
            {
                return;
            }

            LoggedInFormElements(false);
            LoginFormElements(true);

            _userId = string.Empty;
            _loginToken = string.Empty;
        }

        private void Greenbutton_hover_MouseEnter(object sender, EventArgs e)
        {
            RegisterText.BackgroundImage = Theming.GreenButtonHover;
        }

        private void Greenbutton_MouseLeave(object sender, EventArgs e)
        {
            RegisterText.BackgroundImage = Theming.GreenButton;
        }

        private void Greenbutton_hover_MouseUp(object sender, EventArgs e)
        {
            RegisterText.BackgroundImage = Theming.GreenButtonHover;
        }

        private void Greenbutton_click_MouseDown(object sender, EventArgs e)
        {
            RegisterText.BackgroundImage = Theming.GreenButtonClick;
        }

        private void Email_TextChanged(object sender, EventArgs e)
        {
            MainEmailBorder.Image = Theming.BorderEmail;
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            MainEmailBorder.Image = Theming.BorderEmail;
            MainPasswordBorder.Image = Theming.BorderPassword;
        }

        private void Graybutton_click_MouseDown(object sender, EventArgs e)
        {
            LogoutButton.BackgroundImage = Theming.GrayButtonClick;
        }

        private void Graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            LogoutButton.BackgroundImage = Theming.GrayButtonHover;
        }

        private void Graybutton_MouseLeave(object sender, EventArgs e)
        {
            LogoutButton.BackgroundImage = Theming.GrayButton;
        }

        private void Graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            LogoutButton.BackgroundImage = Theming.GrayButtonHover;
        }

        private void ServerPick_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        /* SETTINGS PAGE LAYOUT */
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            try
            {
                SettingsScreen.OpenScreen();
            }
            catch (Exception Error)
            {
                string ErrorMessage = "Settings Screen Encountered an Error";
                LogToFileAddons.OpenLog("SETTINGS SCREEN", ErrorMessage, Error, "Exclamation", false);
            }
        }

        private void ButtonSecurityCenter_Click(object sender, EventArgs e)
        {
            SecurityCenterScreen.OpenScreen("Idle Ready");
        }

        private void StartGame(string UserID, string LoginToken)
        {
            if (InformationCache.SelectedServerEnforceProxy)
            {
                if (!Proxy_Settings.Running())
                {
                    Proxy_Server.Instance.Start("Start Game");
                }
            }

            _nfswstarted = new Thread(() =>
            {
                if (Proxy_Settings.Running())
                {
                    LaunchGame(UserID, LoginToken, "http://127.0.0.1:" + Proxy_Settings.Port + "/nfsw/Engine.svc", this);
                }
                else
                {
                    Uri convert = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress);

                    if (convert.Scheme == "http")
                    {
                        Match match = Regex.Match(convert.Host, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
                        if (!match.Success)
                        {
                            Launcher_Value.Launcher_Select_Server_Data.IPAddress =
                            Launcher_Value.Launcher_Select_Server_Data.IPAddress.Replace(convert.Host, FunctionStatus.HostName2IP(convert.Host));
                        }
                    }

                    LaunchGame(UserID, LoginToken, Launcher_Value.Launcher_Select_Server_Data.IPAddress, this);
                }
            })
            { IsBackground = true };

            _nfswstarted.Start();

            Presence_Launcher.Status("In-Game", null);
        }

        /* Check Serverlist API Status Upon Main Screen load - DavidCarbon */
        private void PingServerListAPIStatus()
        {
            APIStatusText.Text = "United API:\n - Online";
            APIStatusText.ForeColor = Theming.Sucess;
            APIStatusDesc.Text = "Connected to API";
            APIStatusIcon.BackgroundImage = Theming.APIIconSuccess;

            if (!VisualsAPIChecker.UnitedAPI())
            {
                APIStatusText.Text = "Carbon API:\n - Online";
                APIStatusText.ForeColor = Theming.Sucess;
                APIStatusDesc.Text = "Connected to API";
                APIStatusIcon.BackgroundImage = Theming.APIIconSuccess;

                if (!VisualsAPIChecker.CarbonAPI())
                {
                    APIStatusText.Text = "Carbon 2nd API:\n - Online";
                    APIStatusText.ForeColor = Theming.Sucess;
                    APIStatusDesc.Text = "Connected to API";
                    APIStatusIcon.BackgroundImage = Theming.APIIconSuccess;

                    if (!VisualsAPIChecker.CarbonAPITwo())
                    {
                        APIStatusText.Text = "Connection API:\n - Error";
                        APIStatusText.ForeColor = Theming.Error;
                        APIStatusDesc.Text = "Launcher is Offline";
                        APIStatusIcon.BackgroundImage = Theming.APIIconError;
                        Log.Api("PINGING API: Failed to Connect to APIs! Quick Hide and Bunker Down! (Ask for help)");
                    }
                }
            }
        }

        private void LaunchGame(string UserID, string LoginToken, string ServerIP, Form x)
        {
            if (Process_Start_Game.Initialize(Save_Settings.Live_Data.Game_Path, ServerIP, LoginToken, 
                UserID, Launcher_Value.Launcher_Select_Server_Data.ID.ToUpper()) != null)
            {
                FunctionStatus.LauncherBattlePass = Process_Start_Game.Live_Process.EnableRaisingEvents = true;
                _nfswPid = Process_Start_Game.Live_Process.Id;

                /* TIMER HERE */
                System.Timers.Timer shutdowntimer = new System.Timers.Timer();
                shutdowntimer.Elapsed += new System.Timers.ElapsedEventHandler(Time_Window.ClockWork_Planet);
                shutdowntimer.Interval = !Proxy_Settings.Running() ? 30000 : 60000;
                shutdowntimer.Enabled = true;

                CloseBTN.SafeInvokeAction(() =>
                CloseBTN.Visible = false, this);

                this.SafeInvokeAction(() =>
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.ShowInTaskbar = false;
                }, this);

                Process_Start_Game.Live_Process.Exited += (Send, It) =>
                {
                    _nfswPid = 0;
                    int exitCode = Process_Start_Game.Live_Process.ExitCode;

                    FunctionStatus.LauncherBattlePass = false;

                    if (Launcher_Value.Game_In_Event_Bug)
                    {
                        if (AC_Core.Status) exitCode = 2017;
                        else exitCode = 2137;
                    }

                    if (exitCode == 0 && !Launcher_Value.Game_In_Event_Bug && AC_Core.Stop_Check())
                    {
                        CloseBTN_Click(null, null);
                    }
                    else if (AC_Core.Stop_Check())
                    {
                        x.SafeEndInvokeAsyncCatch(x.SafeBeginInvokeActionAsync(Launcher_X_Form =>
                        {
                            x.WindowState = FormWindowState.Normal;
                            x.ShowInTaskbar = true;

                            string Error_Msg = NFSW.ErrorTranslation(exitCode);
                            Log.Error("GAME CRASH [EXIT CODE]: " + exitCode.ToString() + " HEX: (0x" + exitCode.ToString("X") + ")" + " REASON: " + Error_Msg);

                            CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            PlayProgressText.Text = Error_Msg.ToUpper();
                            PlayProgress.Value = 100;
                            PlayProgress.ForeColor = Theming.Error;

                            if (_nfswPid != 0)
                            {
                                try
                                {
                                    Process.GetProcessById(_nfswPid).Kill();
                                }
                                catch { /* ignored */ }
                            }

                            _nfswstarted.Abort();
                            DialogResult restartApp = MessageBox.Show(null, Error_Msg + "\nWould you like to restart the GameLauncher?",
                                "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (restartApp == DialogResult.Yes)
                            {
                                _restartLauncher = true;
                            }
                            this.CloseBTN_Click(null, null);
                        }));
                    }
                };
            }
        }

        private void Client_DownloadProgressChanged_RELOADED(object sender, DownloadProgressChangedEventArgs e)
        {
            PlayProgressTextTimer.SafeInvokeAction(() =>
            PlayProgressTextTimer.Text = ("Downloading - [" + CurrentModFileCount + " / " + TotalModFileCount + "] :").ToUpper(), this);

            if (e.TotalBytesToReceive >= 1)
            {
                PlayProgressText.SafeInvokeAction(() =>
                PlayProgressText.Text = (" Server Mods: " + ModNetFileNameInUse + " - " + Time_Conversion.FormatFileSize(e.BytesReceived) + " of " + Time_Conversion.FormatFileSize(e.TotalBytesToReceive)).ToUpper(), this);

                ExtractingProgress.SafeInvokeAction(() =>
                {
                    ExtractingProgress.Value = Convert.ToInt32(decimal.Divide(e.BytesReceived, e.TotalBytesToReceive) * 100);
                    ExtractingProgress.Width = Convert.ToInt32(decimal.Divide(e.BytesReceived, e.TotalBytesToReceive) * 519);
                }, this);
            }
        }

        public void DownloadModNetFilesRightNow(string path)
        {
            while (isDownloadingModNetFiles == false)
            {
                CurrentModFileCount++;
                Uri url = modFilesDownloadUrls.Dequeue();
                string FileName = url.ToString().Substring(url.ToString().LastIndexOf("/") + 1, (url.ToString().Length - url.ToString().LastIndexOf("/") - 1));

                ModNetFileNameInUse = FileName;

                ServicePointManager.FindServicePoint(url).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                var Client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };

                if (!Launcher_Value.Launcher_Alternative_Webcalls()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                else
                {
                    Client.Headers.Add("user-agent", "SBRW Launcher " +
                    Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                }

                try
                {
                    Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged_RELOADED);
                    Client.DownloadFileCompleted += (test, stuff) =>
                    {
                        Log.Core("LAUNCHER: Downloaded: " + FileName);
                        isDownloadingModNetFiles = false;
                        if (!modFilesDownloadUrls.Any())
                        {
                            LaunchGame();
                        }
                        else
                        {
                            /* Redownload other file */
                            DownloadModNetFilesRightNow(path);
                        }
                    };
                    Client.DownloadFileAsync(url, path + "/" + FileName);
                }
                catch (Exception Error)
                {
                    CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    LogToFileAddons.OpenLog("Modnet Server Files", null, Error, null, true);
                }
                finally
                {
                    if (Client != null)
                    {
                        Client.Dispose();
                    }

                    Application.DoEvents();
                }

                isDownloadingModNetFiles = true;
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (!UnixOS.Detected())
            {
                DriveInfo driveInfo = new DriveInfo(Save_Settings.Live_Data.Game_Path);

                if (!string.Equals(driveInfo.DriveFormat, "NTFS", StringComparison.InvariantCultureIgnoreCase))
                {
                    CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    MessageBox.Show(
                        $"Playing the game on a non-NTFS-formatted drive is not supported.\nDrive '{driveInfo.Name}' is formatted with: {driveInfo.DriveFormat}",
                        "Compatibility",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Save_Account.Live_Data.User_Raw_Email).ToUpper();
                    return;
                }
            }

            if (Save_Settings.Live_Data.Game_Integrity != "Good")
            {
                CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                MessageBox.Show("GameLauncher has detected a GameFiles Integrity Error\nPlease 'Verify GameFiles' in the Settings Screen",
                    "Game Files Integrity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Save_Account.Live_Data.User_Raw_Email).ToUpper();
                return;
            }

            if (!Redistributable.ErrorFree)
            {
                CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                MessageBox.Show("GameLauncher has detected that the 2015-2019 VC++ Redistributable Package is not installed\n" +
                    "Please manually Install the Packages for your Operating System",
                    "VC++ Redistributable Package Check", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Save_Account.Live_Data.User_Raw_Email).ToUpper();
                return;
            }

            if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)))
            {
                try
                {
                    File.Delete(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks));
                }
                catch { }
            }

            /* Disable Play Button and Logout Buttons */
            PlayButton.Visible = false;
            LogoutButton.Visible = false;
            _disableLogout = true;
            DisablePlayButton();

            ModNetHandler.FileANDFolder(Save_Settings.Live_Data.Game_Path);
            Log.Core("LAUNCHER: Installing ModNet");
            PlayProgressText.Text = ("Detecting ModNet Support for " + ServerListUpdater.ServerName("ModNet")).ToUpper();

            if (ModNetHandler.Supported())
            {
                /* Caches (In Order of Excution) */
                string ModulesJSON = string.Empty;
                string ServerModInfo = string.Empty;
                GetModInfo json2 = null;
                string remoteCarsFile = string.Empty;
                string remoteEventsFile = string.Empty;
                string ServerModListJSON = string.Empty;
                ServerModList json3 = null;

                try
                {
                    Presence_Launcher.Status("Checking ModNet", null);
                    /* Get Remote ModNet list to process for checking required ModNet files are present and current */
                    Uri ModNetURI = new Uri(URLs.ModNet + "/launcher-modules/modules.json");
                    ServicePointManager.FindServicePoint(ModNetURI).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var ModNetJsonURI = new WebClient
                    {
                        Encoding = Encoding.UTF8
                    };

                    if (!Launcher_Value.Launcher_Alternative_Webcalls()) { ModNetJsonURI = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                    else
                    {
                        ModNetJsonURI.Headers.Add("user-agent", "SBRW Launcher " +
                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    }

                    try
                    {
                        ModulesJSON = ModNetJsonURI.DownloadString(ModNetURI);
                        PlayProgressText.Text = "JSON: Retrieved ModNet Files Information".ToUpper();
                    }
                    catch (Exception Error)
                    {
                        PlayProgressText.Text = ("JSON: Unable to Retrieve ModNet Files Information").ToUpper();
                        Presence_Launcher.Status("ModNet Files Information Error", null);
                        CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                        string LogMessage = "There was an error with ModNet JSON Retrieval:";
                        LogToFileAddons.OpenLog("MODNET FILES", LogMessage, Error, "Error", false);
                    }
                    finally
                    {
                        if (ModNetJsonURI != null)
                        {
                            ModNetJsonURI.Dispose();
                        }

                        Application.DoEvents();
                    }

                    if (string.IsNullOrWhiteSpace(ModulesJSON) || !Is_Json.Valid(ModulesJSON))
                    {
                        PlayProgressText.Text = ("JSON: Invalid ModNet Files Information").ToUpper();
                        Presence_Launcher.Status("ModNet Files Information Error", null);
                        CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                        ModulesJSON = null;
                        return;
                    }
                    else
                    {
                        try
                        {
                            string[] modules_newlines = ModulesJSON.Split(new string[] { "\n" }, StringSplitOptions.None);
                            foreach (string modules_newline in modules_newlines)
                            {
                                if (modules_newline.Trim() == "{" || modules_newline.Trim() == "}") continue;

                                string trim_modules_newline = modules_newline.Trim();
                                string[] modules_files = trim_modules_newline.Split(new char[] { ':' });

                                string ModNetList = modules_files[0].Replace("\"", "").Trim();
                                string ModNetSHA = modules_files[1].Replace("\"", "").Replace(",", "").Trim();

                                string ModNetFilePath = Path.Combine(Save_Settings.Live_Data.Game_Path, ModNetList);

                                if (Hashes.Hash_SHA256(ModNetFilePath).ToLower() != ModNetSHA || !File.Exists(ModNetFilePath))
                                {
                                    PlayProgressText.Text = ("ModNet: Downloading " + ModNetList).ToUpper();

                                    Log.Warning("MODNET CORE: " + ModNetList + " Does not match SHA Hash on File Server -> Online Hash: '" + ModNetSHA + "'");

                                    if (File.Exists(ModNetFilePath))
                                    {
                                        File.Delete(ModNetFilePath);
                                    }

                                    Presence_Launcher.Status("Download ModNet", ModNetList);

                                    Uri URLCall = new Uri(URLs.ModNet + "/launcher-modules/" + ModNetList);
                                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                    var newModNetFilesDownload = new WebClient
                                    {
                                        Encoding = Encoding.UTF8
                                    };

                                    if (!Launcher_Value.Launcher_Alternative_Webcalls()) { newModNetFilesDownload = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                                    else
                                    {
                                        newModNetFilesDownload.Headers.Add("user-agent", "SBRW Launcher " +
                                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                    }
                                    newModNetFilesDownload.DownloadFile(URLCall, ModNetFilePath);
                                }
                                else
                                {
                                    PlayProgressText.Text = ("ModNet: Up to Date " + ModNetList).ToUpper();
                                    Log.Info("MODNET CORE: " + ModNetList + " Is Up to Date!");
                                }

                                Application.DoEvents();
                            }
                        }
                        catch (Exception Error)
                        {
                            Presence_Launcher.Status("Download ModNet Error", null);
                            CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            string LogMessage = "There was an error with ModNet Files Check:";
                            LogToFileAddons.OpenLog("MODNET CORE", LogMessage, Error, "Error", false);
                            Application.DoEvents();

                            return;
                        }
                        finally
                        {
                            if (ModulesJSON != null)
                            {
                                ModulesJSON = null;
                            }
                        }

                        Uri newModNetUri = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress + "/Modding/GetModInfo");
                        ServicePointManager.FindServicePoint(newModNetUri).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                        var ModInfoJson = new WebClient
                        {
                            Encoding = Encoding.UTF8
                        };

                        if (!Launcher_Value.Launcher_Alternative_Webcalls()) { ModInfoJson = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                        else
                        {
                            ModInfoJson.Headers.Add("user-agent", "SBRW Launcher " +
                            Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        }

                        try
                        {
                            ServerModInfo = ModInfoJson.DownloadString(newModNetUri);
                            PlayProgressText.Text = ("JSON: Retrieved Server Mod Information").ToUpper();
                        }
                        catch (Exception Error)
                        {
                            PlayProgressText.Text = ("JSON: Unable to Retrieve Server Mod Information").ToUpper();
                            Presence_Launcher.Status("Server Mods Get Information Error", null);
                            CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            string LogMessage = "There was an error with Server Mod Information Retrieval:";
                            LogToFileAddons.OpenLog("SERVER MOD INFO", LogMessage, Error, "Error", false);
                        }
                        finally
                        {
                            if (ModInfoJson != null)
                            {
                                ModInfoJson.Dispose();
                            }

                            Application.DoEvents();
                        }

                        if (string.IsNullOrWhiteSpace(ServerModInfo) || !Is_Json.Valid(ServerModInfo))
                        {
                            PlayProgressText.Text = ("JSON: Invalid Server Mod Information").ToUpper();
                            Presence_Launcher.Status("Server Mods Get Information Error", null);
                            CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            ServerModInfo = null;
                            return;
                        }
                        else
                        {
                            /* get files now */
                            json2 = JsonConvert.DeserializeObject<GetModInfo>(ServerModInfo);
                            ServerModInfo = null;

                            /* Set and Get for RemoteRPC Files */
                            Uri URLCall_A = new Uri(json2.basePath + "/cars.json");
                            ServicePointManager.FindServicePoint(URLCall_A).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            var CarsJson = new WebClient
                            {
                                Encoding = Encoding.UTF8
                            };

                            if (!Launcher_Value.Launcher_Alternative_Webcalls()) { CarsJson = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                            else
                            {
                                CarsJson.Headers.Add("user-agent", "SBRW Launcher " +
                                Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            }

                            try
                            {
                                remoteCarsFile = CarsJson.DownloadString(URLCall_A);
                            }
                            catch { }
                            finally
                            {
                                if (CarsJson != null)
                                {
                                    CarsJson.Dispose();
                                }
                            }

                            Uri URLCall_B = new Uri(json2.basePath + "/events.json");
                            ServicePointManager.FindServicePoint(URLCall_B).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            var EventsJson = new WebClient
                            {
                                Encoding = Encoding.UTF8
                            };

                            if (!Launcher_Value.Launcher_Alternative_Webcalls()) { EventsJson = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                            else
                            {
                                EventsJson.Headers.Add("user-agent", "SBRW Launcher " +
                                Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            }

                            try
                            {
                                remoteEventsFile = EventsJson.DownloadString(URLCall_B);
                            }
                            catch { }
                            finally
                            {
                                if (EventsJson != null)
                                {
                                    EventsJson.Dispose();
                                }
                            }

                            /* Version 1.3 @metonator - DavidCarbon */
                            if (Is_Json.Valid(remoteCarsFile))
                            {
                                Log.Info("DISCORD: Found RemoteRPC List for cars.json");
                                Cars.List_File = remoteCarsFile;
                                remoteCarsFile = null;
                            }
                            else
                            {
                                Log.Warning("DISCORD: RemoteRPC list for cars.json does not exist");
                                Cars.List_File = string.Empty;
                            }

                            if (Is_Json.Valid(remoteEventsFile))
                            {
                                Log.Info("DISCORD: Found RemoteRPC List for events.json");
                                SBRW.Launcher.Core.Discord.Reference_.List_.Events.List_File = remoteEventsFile;
                                remoteEventsFile = null;
                            }
                            else
                            {
                                Log.Warning("DISCORD: RemoteRPC list for events.json does not exist");
                                SBRW.Launcher.Core.Discord.Reference_.List_.Events.List_File = string.Empty;
                            }

                            Log.Core("CORE: Loading Server Mods List");
                            /* Get Server Mod Index */
                            Uri newIndexFile = new Uri(json2.basePath + "/index.json");
                            ServicePointManager.FindServicePoint(newIndexFile).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            var ServerModsList = new WebClient
                            {
                                Encoding = Encoding.UTF8
                            };

                            if (!Launcher_Value.Launcher_Alternative_Webcalls()) { ServerModsList = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                            else
                            {
                                ServerModsList.Headers.Add("user-agent", "SBRW Launcher " +
                                Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            }

                            try
                            {
                                Log.Core("CORE: Retrieved Server Mods List");
                                ServerModListJSON = ServerModsList.DownloadString(newIndexFile);
                                PlayProgressText.Text = ("JSON: Retrieved Server Mod List Information").ToUpper();
                            }
                            catch (Exception Error)
                            {
                                PlayProgressText.Text = ("JSON: Unable to Retrieve Server Mod List Information").ToUpper();
                                Presence_Launcher.Status("Server Mods Get Information Error", null);
                                CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                                string LogMessage = "There was an error with Server Mod List Information Retrieval:";
                                LogToFileAddons.OpenLog("SERVER MOD JSON", LogMessage, Error, "Error", false);
                            }
                            finally
                            {
                                if (ServerModsList != null)
                                {
                                    ServerModsList.Dispose();
                                }

                                Application.DoEvents();
                            }

                            if (string.IsNullOrWhiteSpace(ServerModListJSON) || !Is_Json.Valid(ServerModListJSON))
                            {
                                PlayProgressText.Text = ("JSON: Invalid Server Mod List Information").ToUpper();
                                Presence_Launcher.Status("Server Mods Get Information Error", null);
                                CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                                ServerModListJSON = null;
                                return;
                            }
                            else
                            {
                                try
                                {
                                    json3 = JsonConvert.DeserializeObject<ServerModList>(ServerModListJSON);
                                    ServerModListJSON = null;
                                    string ModFolderCache = Path.Combine(Save_Settings.Live_Data.Game_Path, "MODS", Hashes.Hash_String(0, json2.serverID).ToLower());
                                    if (!Directory.Exists(ModFolderCache)) Directory.CreateDirectory(ModFolderCache);

                                    /* (FILENAME.mods) 
                                     * Checks for any Files that Don't match the Server Index Json and Removes that File  */
                                    foreach (string file in Directory.GetFiles(ModFolderCache))
                                    {
                                        string name = Path.GetFileName(file);

                                        if (json3.entries.All(en => en.Name != name))
                                        {
                                            try
                                            {
                                                File.Delete(file);
                                                Log.Core("LAUNCHER: Removed Stale Mod Package: " + file);
                                            }
                                            catch (Exception Error)
                                            {
                                                LogToFileAddons.OpenLog("SERVER MOD CACHE", null, Error, null, true);
                                            }
                                        }
                                    }

                                    /* (OLD-FILENAME.mods != NEW-FILENAME.mods)
                                     * Checks for the file and if the File Hash does not match it will be added to a list to be downloaded 
                                     * If a file exists and doesn't match a the server provided index json it will be deleted 
                                     * 5/22/2021: If a Server Extracted Mods Directory is present and 
                                     * if a Server Mod File no longer matches it will now delete the folder (.data/SERVER-ID-HASH) - DavidCarbon
                                     */
                                    int ExtractedServerFolderRunTime = 0;

                                    foreach (ServerModFileEntry modfile in json3.entries)
                                    {
                                        string ModCachedFile = Path.Combine(ModFolderCache, modfile.Name);
                                        if (Hashes.Hash_SHA(ModCachedFile).ToLower() != modfile.Checksum)
                                        {
                                            try
                                            {
                                                if (ExtractedServerFolderRunTime == 0)
                                                {
                                                    string ExtractedServerFolder = Path.Combine(Save_Settings.Live_Data.Game_Path, ".data", Hashes.Hash_String(0, json2.serverID).ToLower());
                                                    if (Directory.Exists(ExtractedServerFolder))
                                                    {
                                                        Directory.Delete(ExtractedServerFolder, true);
                                                        Log.Core("LAUNCHER: Removed Extracted Server Mods Folder: .data/" + Hashes.Hash_String(0, json2.serverID).ToLower());
                                                    }

                                                    ExtractedServerFolderRunTime++;
                                                }

                                                if (File.Exists(ModCachedFile))
                                                {
                                                    File.Delete(ModCachedFile);
                                                    Log.Core("LAUNCHER: Removed Old Mod Package: " + modfile.Name);
                                                }
                                            }
                                            catch (Exception Error)
                                            {
                                                LogToFileAddons.OpenLog("SERVER MOD CACHE FILE", null, Error, null, true);
                                            }

                                            modFilesDownloadUrls.Enqueue(new Uri(json2.basePath + "/" + modfile.Name));
                                            TotalModFileCount++;
                                        }
                                        else
                                        {
                                            PlayProgressText.Text = ("Server Mods: Up to Date " + modfile.Name).ToUpper();
                                        }

                                        Application.DoEvents();
                                    }

                                    if (modFilesDownloadUrls.Count != 0)
                                    {
                                        this.DownloadModNetFilesRightNow(ModFolderCache);
                                        Presence_Launcher.Status("Download Server Mods", null);
                                    }
                                    else
                                    {
                                        LaunchGame();
                                    }
                                }
                                catch (Exception Error)
                                {
                                    Presence_Launcher.Status("Download Server Mods Error", null);
                                    CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                                    string LogMessage = "There was an error with Server Mods Check:";
                                    LogToFileAddons.OpenLog("SERVER MOD DOWNLOAD", LogMessage, Error, "Error", false);
                                    return;
                                }
                                finally
                                {
                                    if (ModulesJSON != null)
                                    {
                                        ModulesJSON = null;
                                    }
                                    if (ServerModInfo != null)
                                    {
                                        ServerModInfo = null;
                                    }
                                    if (json2 != null)
                                    {
                                        json2 = null;
                                    }
                                    if (remoteCarsFile != null)
                                    {
                                        remoteCarsFile = null;
                                    }
                                    if (remoteEventsFile != null)
                                    {
                                        remoteEventsFile = null;
                                    }
                                    if (ServerModListJSON != null)
                                    {
                                        ServerModListJSON = null;
                                    }
                                    if (json3 != null)
                                    {
                                        json3 = null;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception Error)
                {
                    Presence_Launcher.Status("Download ModNet Error", null);
                    CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    string LogMessage = "There was an error downloading ModNet Files:";
                    LogToFileAddons.OpenLog("MODNET FILES", LogMessage, Error, "Error", false);
                    return;
                }
                finally
                {
                    if (ModulesJSON != null)
                    {
                        ModulesJSON = null;
                    }
                    if (ServerModInfo != null)
                    {
                        ServerModInfo = null;
                    }
                    if (json2 != null)
                    {
                        json2 = null;
                    }
                    if (remoteCarsFile != null)
                    {
                        remoteCarsFile = null;
                    }
                    if (remoteEventsFile != null)
                    {
                        remoteEventsFile = null;
                    }
                    if (ServerModListJSON != null)
                    {
                        ServerModListJSON = null;
                    }
                    if (json3 != null)
                    {
                        json3 = null;
                    }

                    Application.DoEvents();
                }
            }
            else
            {
                LaunchGame();
            }
        }

        /* Launch game */
        private void LaunchGame()
        {
            Presence_Launcher.Start("New RPC", Presence_Launcher.ApplicationID());

            try
            {
                string GameExePath = Path.Combine(Save_Settings.Live_Data.Game_Path, "nfsw.exe");
                if
                  (
                    Hashes.Hash_SHA(GameExePath) == "7C0D6EE08EB1EDA67D5E5087DDA3762182CDE4AC" ||
                    Hashes.Hash_SHA(GameExePath) == "DB9287FB7B0CDA237A5C3885DD47A9FFDAEE1C19" ||
                    Hashes.Hash_SHA(GameExePath) == "E69890D31919DE1649D319956560269DB88B8F22" ||
                    Hashes.Hash_SHA(GameExePath) == "3CBE3FAAFF00FAD84F78A2AFEA4FFFC78294EEA2"
                  )
                {
                    Launcher_Value.Game_Server_Name = ServerListUpdater.ServerName("Proxy");
                    Launcher_Value.Game_Server_IP = Launcher_Value.Launcher_Select_Server_Data.IPAddress;

                    Launcher_Value.Game_User_ID = _userId;
                    Launcher_Value.Game_Server_IP_Host = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress).Host;

                    StartGame(_userId, _loginToken);

                    if (_builtinserver)
                    {
                        PlayProgressText.Text = "Soapbox server launched. Waiting for queries.".ToUpper();
                    }
                    else
                    {
                        Application.DoEvents();

                        ExtractingProgress.Value = 100;
                        ExtractingProgress.Width = 519;

                        PlayProgressTextTimer.Text = string.Empty;
                        PlayProgressText.Text = "Loading game. Launcher will minimize once Game has Loaded".ToUpper();
                        CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();

                        ContextMenu = new ContextMenu();
                        ContextMenu.MenuItems.Add(new MenuItem("Running Out of Time", (b, n) => { Process.Start("https://youtu.be/vq9-bmoI-RI"); }));
                        ContextMenu.MenuItems.Add("-");
                        ContextMenu.MenuItems.Add(new MenuItem("Close Game and Launcher", CloseBTN_Click));

                        Update();
                        Refresh();

                        Notification.Text = "SBRW Launcher";
                        Notification.ContextMenu = ContextMenu;
                    }
                }
                else if (!File.Exists(GameExePath))
                {
                    CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    MessageBox.Show(null, "You do not have the Game Downloaded. Please Verify Game Files installation path.", "GameLauncher",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    MessageBox.Show(null, "Your NFSW.exe is Modified. Please Verify Game Files.", "GameLauncher",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception Error)
            {
                CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                LogToFileAddons.OpenLog("GAME LAUNCH", Error.Message, Error, "Error", false);
            }
        }

        private void PlayButton_MouseUp(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            PlayButton.BackgroundImage = Theming.PlayButtonHover;
        }

        private void PlayButton_MouseDown(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            PlayButton.BackgroundImage = Theming.PlayButtonClick;
        }

        private void PlayButton_MouseEnter(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            PlayButton.BackgroundImage = Theming.PlayButtonHover;
        }

        private void PlayButton_MouseLeave(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            PlayButton.BackgroundImage = Theming.PlayButton;
        }

        private void LaunchNfsw()
        {
            PlayButton.SafeInvokeAction(() =>
            {
                PlayButton.BackgroundImage = Theming.PlayButton;
                PlayButton.ForeColor = Theming.ThirdTextForeColor;
            }, this);

            PlayProgressText.SafeInvokeAction(() =>
            PlayProgressText.Text = "Checking up all files".ToUpper(), this);

            PlayProgress.SafeInvokeAction(() =>
            PlayProgress.Width = 0, this);

            ExtractingProgress.SafeInvokeAction(() =>
            ExtractingProgress.Width = 0, this);

            try
            {
                PlayProgressText.SafeInvokeAction(() =>
                PlayProgressText.Text = "Loading list of files to download...".ToUpper(), this);

                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    if (d.Name == Path.GetPathRoot(Save_Settings.Live_Data.Game_Path))
                    {
                        if (d.TotalFreeSpace < 8589934592 || !string.Equals(d.DriveFormat, "NTFS", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ExtractingProgress.SafeInvokeAction(() =>
                            {
                                ExtractingProgress.Value = 100;
                                ExtractingProgress.Width = 519;
                                ExtractingProgress.Image = Theming.ProgressBarWarning;
                                ExtractingProgress.ProgressColor = Theming.ExtractingProgressColor;
                            }, this);

                            if (!string.Equals(d.DriveFormat, "NTFS", StringComparison.InvariantCultureIgnoreCase))
                            {
                                PlayProgressTextTimer.SafeInvokeAction(() =>
                                PlayProgressTextTimer.Text = ("Playing the game on a non-NTFS-formatted drive is not supported.").ToUpper(), this);
                                PlayProgressText.SafeInvokeAction(() =>
                                PlayProgressText.Text = ("Drive '" + d.Name + "' is formatted with: " + d.DriveFormat + " Type.").ToUpper(), this);
                            }
                            else
                            {
                                PlayProgressText.SafeInvokeAction(() => 
                                PlayProgressText.Text = "Make sure you have at least 8GB of free space on hard drive.".ToUpper(), this);
                            }

                            FunctionStatus.IsVerifyHashDisabled = true;

                            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Paused);
                            TaskbarProgress.SetValue(Handle, 100, 100);
                        }
                        else
                        {
                            DownloadCoreFiles();
                        }

                        break;
                    }
                }
            }
            catch
            {
                DownloadCoreFiles();
            }
        }

        public void RemoveTracksHighFiles()
        {
            try
            {
                string SpecificTracksHighPath = Path.Combine(Save_Settings.Live_Data.Game_Path, "TracksHigh");
                if (File.Exists(Path.Combine(SpecificTracksHighPath, "STREAML5RA_98.BUN")))
                {
                    Directory.Delete(SpecificTracksHighPath, true);
                }
            }
            catch { }
        }

        public void DownloadCoreFiles()
        {
            PlayProgressText.SafeInvokeAction(() =>
            PlayProgressText.Text = "Checking Core Files...".ToUpper(), this);

            PlayProgress.SafeInvokeAction(() =>
            PlayProgress.Width = 0, this);

            ExtractingProgress.SafeInvokeAction(() =>
            ExtractingProgress.Width = 0, this);

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            string GameExePath = Path.Combine(Save_Settings.Live_Data.Game_Path, "nfsw.exe");
            /* Use Local Packed Archive for Install Source - DavidCarbon */
            if (File.Exists(filename_pack) && !File.Exists(GameExePath))
            {
                PlayProgressText.SafeInvokeAction(() =>
                PlayProgressText.Text = "Local GameFiles sbrwpack Found In Launcher Folder".ToUpper(), this);

                PlayProgressTextTimer.SafeInvokeAction(() =>
                PlayProgressTextTimer.Text = "Loading".ToUpper(), this);

                /* GameFiles.sbrwpack */
                LocalGameFiles();
            }
            else if (!File.Exists(GameExePath))
            {
                if (Save_Settings.Live_Data.Launcher_CDN.StartsWith("http://localhost") || Save_Settings.Live_Data.Launcher_CDN.StartsWith("https://localhost"))
                {
                    ExtractingProgress.SafeInvokeAction(() =>
                    {
                        ExtractingProgress.Value = 100;
                        ExtractingProgress.Width = 519;
                        ExtractingProgress.Image = Theming.ProgressBarWarning;
                        ExtractingProgress.ProgressColor = Theming.ExtractingProgressColor;
                    }, this);

                    PlayProgressTextTimer.SafeInvokeAction(() => PlayProgressTextTimer.Text = "Failsafe CDN Detected".ToUpper(), this);
                    PlayProgressText.SafeInvokeAction(() => PlayProgressText.Text = "Please Choose a CDN from Settings Screen".ToUpper(), this);

                    TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Paused);
                    TaskbarProgress.SetValue(Handle, 100, 100);
                }
                else
                {
                    _downloadStartTime = DateTime.Now;
                    PlayProgressTextTimer.SafeInvokeAction(() => PlayProgressTextTimer.Text = "Downloading: Core GameFiles".ToUpper(), this);
                    Log.Info("DOWNLOAD: Getting Core Game Files");
                    _downloader.StartDownload(Save_Settings.Live_Data.Launcher_CDN, string.Empty, Save_Settings.Live_Data.Game_Path, false, false, 1130632198);
                }
            }
            else
            {
                DownloadTracksFiles();
            }
        }

        public void DownloadTracksFiles()
        {
            PlayProgressText.SafeInvokeAction(() =>
            PlayProgressText.Text = "Checking Tracks Files...".ToUpper(), this);

            PlayProgress.SafeInvokeAction(() =>
            PlayProgress.Width = 0, this);

            ExtractingProgress.SafeInvokeAction(() =>
            ExtractingProgress.Width = 0, this);

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            string SpecificTracksFilePath = Path.Combine(Save_Settings.Live_Data.Game_Path, "Tracks", "STREAML5RA_98.BUN");
            if (!File.Exists(SpecificTracksFilePath))
            {
                _downloadStartTime = DateTime.Now;
                PlayProgressTextTimer.SafeInvokeAction(() =>
                PlayProgressTextTimer.Text = "Downloading: Tracks Data".ToUpper(), this);
                Log.Info("DOWNLOAD: Getting Tracks Folder");
                _downloader.StartDownload(Save_Settings.Live_Data.Launcher_CDN, "Tracks", Save_Settings.Live_Data.Game_Path, false, false, 615494528);
            }
            else
            {
                DownloadSpeechFiles();
            }
        }

        public void DownloadSpeechFiles()
        {
            PlayProgressText.SafeInvokeAction(() => PlayProgressText.Text = "Looking for correct Speech Files...".ToUpper(), this);

            PlayProgress.SafeInvokeAction(() => PlayProgress.Width = 0, this);

            ExtractingProgress.SafeInvokeAction(() =>
            ExtractingProgress.Width = 0, this);

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            string speechFile;
            int speechSize;

            try
            {
                speechFile = DownloaderAddons.SpeechFiles(Save_Settings.Live_Data.Launcher_Language);

                Uri URLCall = new Uri(Save_Settings.Live_Data.Launcher_CDN + "/" + speechFile + "/index.xml");
                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                var Client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };

                if (!Launcher_Value.Launcher_Alternative_Webcalls()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                else
                {
                    Client.Headers.Add("user-agent", "SBRW Launcher " +
                    Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                }

                try
                {
                    string response = Client.DownloadString(URLCall);

                    XmlDocument speechFileXml = new XmlDocument();
                    speechFileXml.LoadXml(response);

                    XmlNode speechSizeNode = speechFileXml.SelectSingleNode("index/header/compressed");
                    speechSize = Convert.ToInt32(speechSizeNode.InnerText);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (Client != null)
                    {
                        Client.Dispose();
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Download Speech Files", null, Error, null, true);
                speechFile = DownloaderAddons.SpeechFiles(null);
                speechSize = DownloaderAddons.SpeechFilesSize();
            }

            PlayProgressText.SafeInvokeAction(() =>
            PlayProgressText.Text = string.Format("Checking for {0} Speech Files.", speechFile).ToUpper(), this);

            string SoundSpeechPath = Path.Combine(Save_Settings.Live_Data.Game_Path, "Sound", "Speech", "copspeechsth_" + speechFile + ".big");
            if (!File.Exists(SoundSpeechPath))
            {
                _downloadStartTime = DateTime.Now;
                PlayProgressTextTimer.SafeInvokeAction(() =>
                PlayProgressTextTimer.Text = "Downloading: Language Audio".ToUpper(), this);
                Log.Info("DOWNLOAD: Getting Speech/Audio Files");
                _downloader.StartDownload(Save_Settings.Live_Data.Launcher_CDN, speechFile, Save_Settings.Live_Data.Game_Path, false, false, speechSize);
            }
            else
            {
                OnDownloadFinished();
                PlayProgressTextTimer.SafeInvokeAction(() =>
                PlayProgressTextTimer.Text = string.Empty, this);
                Log.Info("DOWNLOAD: Game Files Download is Complete!");
            }
        }

        /* Check Local GameFiles Hash */
        private async void LocalGameFiles()
        {
            await Task.Delay(5000);
            if (Hashes.Hash_SHA("GameFiles.sbrwpack") == "88C886B6D131C052365C3D6D14E14F67A4E2C253")
            {
                TaskbarProgress.SetValue(Handle, 100, 100);

                PlayProgress.SafeInvokeAction(() =>
                {
                    PlayProgress.Value = 100;
                    PlayProgress.Width = 519;
                }, this);

                GoForUnpack(filename_pack);
            }
        }

        /* That's right the Protype Extractor from 2.1.5.x, now back from the dead - DavidCarbon */
        public void GoForUnpack(string filename_pack)
        {
            Thread unpacker = new Thread(() =>
            {
                if (Application.OpenForms["MainScreen"] != null)
                {
                    if (!Application.OpenForms["MainScreen"].Disposing)
                    {
                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            using (ZipArchive archive = ZipFile.OpenRead(filename_pack))
                            {
                                int numFiles = archive.Entries.Count;
                                int current = 1;

                                foreach (ZipArchiveEntry entry in archive.Entries)
                                {
                                    string fullName = entry.FullName;

                                    ExtractingProgress.SafeInvokeAction(() =>
                                    {
                                        ExtractingProgress.Value = (int)((long)100 * current / numFiles);
                                        ExtractingProgress.Width = (int)((long)519 * current / numFiles);
                                    }, this);

                                    TaskbarProgress.SetValue(Handle, (int)(100 * current / numFiles), 100);

                                    if (!File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, fullName.Replace(".sbrw", string.Empty))))
                                    {
                                        PlayProgressText.SafeInvokeAction(() =>
                                        PlayProgressText.Text = ("Unpacking " + fullName.Replace(".sbrw", string.Empty)).ToUpper(), this);

                                        PlayProgressTextTimer.SafeInvokeAction(() =>
                                        PlayProgressTextTimer.Text = "[" + current + " / " + archive.Entries.Count + "]", this);

                                        if (fullName.Substring(fullName.Length - 1) == "/")
                                        {
                                            /* Is a directory, create it! */
                                            string FolderName = fullName.Remove(fullName.Length - 1);
                                            string GameWithFolderName = Path.Combine(Save_Settings.Live_Data.Game_Path, FolderName);
                                            try
                                            {
                                                if (Directory.Exists(GameWithFolderName))
                                                {
                                                    Directory.Delete(GameWithFolderName, true);
                                                }
                                            }
                                            catch { }

                                            try
                                            {
                                                if (!Directory.Exists(GameWithFolderName))
                                                {
                                                    Directory.CreateDirectory(GameWithFolderName);
                                                }
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            string oldFileName = fullName.Replace(".sbrw", string.Empty);
                                            string[] split = oldFileName.Split('/');

                                            string newFileName = string.Empty;

                                            if (split.Length >= 2)
                                            {
                                                newFileName = Path.Combine(split[split.Length - 2], split[split.Length - 1]);
                                            }
                                            else
                                            {
                                                newFileName = split.Last();
                                            }

                                            string KEY = Regex.Replace(Hashes.Hash_String(1, newFileName), "[^0-9.]", "").Substring(0, 8);
                                            string IV = Regex.Replace(Hashes.Hash_String(0, newFileName), "[^0-9.]", "").Substring(0, 8);

                                            entry.ExtractToFile(getTempNa, true);

                                            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider()
                                            {
                                                Key = Encoding.ASCII.GetBytes(KEY),
                                                IV = Encoding.ASCII.GetBytes(IV)
                                            };

                                            FileStream fileStream = new FileStream(Path.Combine(Save_Settings.Live_Data.Game_Path, oldFileName), FileMode.Create);
                                            CryptoStream cryptoStream = new CryptoStream(fileStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
                                            BinaryWriter binaryFile = new BinaryWriter(cryptoStream);

                                            using (BinaryReader reader = new BinaryReader(File.Open(getTempNa, FileMode.Open)))
                                            {
                                                long numBytes = new FileInfo(getTempNa).Length;
                                                binaryFile.Write(reader.ReadBytes((int)numBytes));
                                                binaryFile.Close();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        PlayProgressText.SafeInvokeAction(() =>
                                        PlayProgressText.Text = ("Skipping " + fullName).ToUpper(), this);
                                    }

                                    string Status = string.Format("Unpacking game: " + (100 * current / numFiles) + "%");
                                    Presence_Launcher.Status("Unpack Game Files", Status);

                                    Application.DoEvents();

                                    if (numFiles == current)
                                    {
                                        PlayProgressTextTimer.SafeInvokeAction(() =>
                                        {
                                            PlayProgressTextTimer.Visible = false;
                                            PlayProgressTextTimer.Text = string.Empty;
                                        }, this);

                                        _isDownloading = false;
                                        OnDownloadFinished();

                                        Notification.Visible = true;
                                        Notification.BalloonTipIcon = ToolTipIcon.Info;
                                        Notification.BalloonTipTitle = "SBRW Launcher";
                                        Notification.BalloonTipText = "Your game is now ready to launch!";
                                        Notification.ShowBalloonTip(5000);
                                        Notification.Dispose();
                                    }

                                    current++;
                                }
                            }
                        });
                    }
                }
            });

            unpacker.Start();
        }

        private void OnDownloadProgress(long downloadLength, long downloadCurrent, long compressedLength, string filename, int skiptime = 0)
        {
            try
            {
                if (downloadCurrent < compressedLength)
                {
                    PlayProgressText.SafeInvokeAction(() =>
                    PlayProgressText.Text = string.Format("{0} of {1} ({3}%) — {2}", Time_Conversion.FormatFileSize(downloadCurrent),
                    Time_Conversion.FormatFileSize(compressedLength), Time_Conversion.EstimateFinishTime(downloadCurrent, compressedLength, 
                    _downloadStartTime), (int)(100 * downloadCurrent / compressedLength)).ToUpper(), this);
                }
            }
            catch { }

            try
            {
                PlayProgress.SafeInvokeAction(() =>
                {
                    PlayProgress.Value = (int)(100 * downloadCurrent / compressedLength);
                    PlayProgress.Width = (int)(519 * downloadCurrent / compressedLength);
                }, this);

                Presence_Launcher.Status("Download Game Files", string.Format("Downloaded {0}% of the Game!", (int)(100 * downloadCurrent / compressedLength)));

                TaskbarProgress.SetValue(Handle, (int)(100 * downloadCurrent / compressedLength), 100);
            }
            catch
            {
                TaskbarProgress.SetValue(Handle, 0, 100);

                PlayProgress.SafeInvokeAction(() =>
                {
                    PlayProgress.Value = 0;
                    PlayProgress.Width = 0;
                }, this);
            }

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);
        }

        private void OnDownloadFinished()
        {
            _downloader.Stop();

            try
            {
                File.WriteAllBytes(Path.Combine(Save_Settings.Live_Data.Game_Path, "GFX", "BootFlow.gfx"),
                    ExtractResource.AsByte("GameLauncher.Resources.Bootscreen.BootFlow.gfx"));
            }
            catch { }

            if (Save_Settings.Live_Data.Game_Integrity == "Unknown")
            {
                Save_Settings.Live_Data.Game_Integrity = "Good";
                Save_Settings.Save();
            }

            Presence_Launcher.Download = false;
            Presence_Launcher.Status("Idle Ready", null);

            PlayProgressText.SafeInvokeAction(() =>
            PlayProgressText.Text = "Ready!".ToUpper(), this);

            EnablePlayButton();

            TaskbarProgress.SetValue(Handle, 100, 100);
            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);
        }

        private void EnablePlayButton()
        {
            _isDownloading = false;
            _playenabled = true;

            ExtractingProgress.SafeInvokeAction(() =>
            {
                ExtractingProgress.Value = 100;
                ExtractingProgress.Width = 519;
            }, this);
        }

        private void DisablePlayButton()
        {
            _isDownloading = false;
            _playenabled = false;

            ExtractingProgress.Value = 100;
            ExtractingProgress.Width = 519;
        }

        private void OnDownloadFailed(Exception Error)
        {
            _downloader.Stop();

            Presence_Launcher.Status("Download Game Files Error", null);

            ExtractingProgress.Value = 100;
            ExtractingProgress.Width = 519;
            ExtractingProgress.Image = Theming.ProgressBarError;
            ExtractingProgress.ProgressColor = Theming.Error;

            PlayProgressText.Text = ((Error != null) ? Error.Message : "Download Failed. No Reason Provided").ToUpper();

            FunctionStatus.IsVerifyHashDisabled = true;

            TaskbarProgress.SetValue(Handle, 100, 100);
            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Error);

            string TempEmailCache = string.Empty;
            if (!string.IsNullOrWhiteSpace(MainEmail.Text))
            {
                TempEmailCache = MainEmail.Text;
                MainEmail.Text = "EMAIL IS HIDDEN";
            }
            string LogMessage = "CDN Downloader Encountered an Error:";
            LogToFileAddons.OpenLog("Game Download", LogMessage, Error, "Error", false);
            if (!string.IsNullOrWhiteSpace(TempEmailCache))
            {
                MainEmail.Text = TempEmailCache;
            }
        }

        private void OnShowExtract(string filename, long currentCount, long allFilesCount)
        {
            try
            {
                if (PlayProgress.Value == 100)
                {
                    PlayProgressText.SafeInvokeAction(() =>
                    PlayProgressText.Text = string.Format("{0} of {1} : ({3}%) — {2}", Time_Conversion.FormatFileSize(currentCount),
                    Time_Conversion.FormatFileSize(allFilesCount), 
                    Time_Conversion.EstimateFinishTime(currentCount, allFilesCount, _downloadStartTime), (int)(100 * currentCount / allFilesCount)).ToUpper(), this);
                }
            }
            catch { }

            try
            {
                ExtractingProgress.SafeInvokeAction(() =>
                {
                    ExtractingProgress.Value = (int)(100 * currentCount / allFilesCount);
                    ExtractingProgress.Width = (int)(519 * currentCount / allFilesCount);
                }, this);
            }
            catch { }
        }

        private void OnShowMessage(string message, string header)
        {
            string TempEmailCache = string.Empty;
            if (!string.IsNullOrWhiteSpace(MainEmail.Text))
            {
                TempEmailCache = MainEmail.Text;
                MainEmail.SafeInvokeAction(() =>
                MainEmail.Text = "EMAIL IS HIDDEN", this);
            }
            MessageBox.Show(message, header);
            if (!string.IsNullOrWhiteSpace(TempEmailCache))
            {
                MainEmail.SafeInvokeAction(() =>
                MainEmail.Text = TempEmailCache, this);
            }
        }

        public MainScreen()
        {
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, y) =>
            {
                ClosingTasks();
            };
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Text = "SBRW Launcher: v" + Application.ProductVersion;

            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            float MainFontSize = UnixOS.Detected() ? 9f : 9f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = UnixOS.Detected() ? 8f : 8f * 96f / CreateGraphics().DpiY;
            float ThirdFontSize = UnixOS.Detected() ? 10f : 10f * 96f / CreateGraphics().DpiY;
            float FourthFontSize = UnixOS.Detected() ? 14f : 14f * 96f / CreateGraphics().DpiY;
            Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);

            /* Front Screen */
            InsiderBuildNumberText.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SelectServerBtn.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            translatedBy.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            ServerPick.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            AddServer.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ShowPlayPanel.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            CurrentWindowInfo.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            LauncherStatusText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            LauncherStatusDesc.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ServerStatusText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ServerStatusDesc.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            APIStatusText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            APIStatusDesc.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ExtractingProgress.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            /* Social Panel */
            ServerInfoPanel.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            HomePageLink.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            DiscordInviteLink.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            FacebookGroupLink.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            TwitterAccountLink.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SceneryGroupText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ServerShutDown.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            /* Log In Panel */
            MainEmail.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            MainPassword.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            RememberMe.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ForgotPassword.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            LoginButton.Font = new Font(DejaVuSansBold, ThirdFontSize, FontStyle.Bold);
            RegisterText.Font = new Font(DejaVuSansBold, ThirdFontSize, FontStyle.Bold);
            ServerPingStatusText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            LogoutButton.Font = new Font(DejaVuSansBold, ThirdFontSize, FontStyle.Bold);
            PlayButton.Font = new Font(DejaVuSansBold, FourthFontSize, FontStyle.Bold);
            PlayProgress.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            PlayProgressText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            PlayProgressTextTimer.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            /* Set Background with Transparent Key */
            BackgroundImage = Theming.MainScreen;
            TransparencyKey = Theming.MainScreenTransparencyKey;

            logo.BackgroundImage = Theming.LogoMain;
            SettingsButton.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Bad") ? Theming.GearButtonWarning : Theming.GearButton;
            CloseBTN.BackgroundImage = Theming.CloseButton;
            ButtonSecurityCenter.BackgroundImage = SecurityCenter.SecurityCenterIcon(1);

            ProgressBarOutline.BackgroundImage = Theming.ProgressBarOutline;
            PlayProgress.Image = Theming.ProgressBarPreload;
            ExtractingProgress.Image = Theming.ProgressBarSuccess;

            PlayProgressText.ForeColor = Theming.FivithTextForeColor;
            PlayProgressTextTimer.ForeColor = Theming.FivithTextForeColor;

            MainEmailBorder.Image = Theming.BorderEmail;
            MainPasswordBorder.Image = Theming.BorderPassword;

            CurrentWindowInfo.ForeColor = Theming.FivithTextForeColor;

            LauncherStatusDesc.ForeColor = Theming.FivithTextForeColor;
            ServerPingStatusText.ForeColor = Theming.FivithTextForeColor;
            ServerStatusDesc.ForeColor = Theming.FivithTextForeColor;
            APIStatusDesc.ForeColor = Theming.FivithTextForeColor;

            LoginButton.ForeColor = Theming.FivithTextForeColor;
            LoginButton.BackgroundImage = Theming.GrayButton;

            RegisterText.ForeColor = Theming.SeventhTextForeColor;
            RegisterText.BackgroundImage = Theming.GreenButton;

            RememberMe.ForeColor = Theming.FivithTextForeColor;

            ForgotPassword.ActiveLinkColor = Theming.ActiveLink;
            ForgotPassword.LinkColor = Theming.Link;

            MainEmail.BackColor = Theming.Input;
            MainEmail.ForeColor = Theming.FivithTextForeColor;
            MainPassword.BackColor = Theming.Input;
            MainPassword.ForeColor = Theming.FivithTextForeColor;

            ServerInfoPanel.BackgroundImage = Theming.SocialPanel;

            ServerShutDown.ForeColor = Theming.SecondaryTextForeColor;
            SceneryGroupText.ForeColor = Theming.SecondaryTextForeColor;

            TwitterAccountLink.LinkColor = Theming.SecondaryTextForeColor;
            FacebookGroupLink.LinkColor = Theming.SecondaryTextForeColor;
            DiscordInviteLink.LinkColor = Theming.SecondaryTextForeColor;
            HomePageLink.LinkColor = Theming.SecondaryTextForeColor;

            TwitterAccountLink.ActiveLinkColor = Theming.FivithTextForeColor;
            FacebookGroupLink.ActiveLinkColor = Theming.FivithTextForeColor;
            DiscordInviteLink.ActiveLinkColor = Theming.FivithTextForeColor;
            HomePageLink.ActiveLinkColor = Theming.FivithTextForeColor;

            InsiderBuildNumberText.ForeColor = Theming.FivithTextForeColor;

            /********************************/
            /* Events                        /
            /********************************/

            TwitterAccountLink.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.TwitterAccountLink_LinkClicked);
            FacebookGroupLink.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.FacebookGroupLink_LinkClicked);
            DiscordInviteLink.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.DiscordInviteLink_LinkClicked);
            HomePageLink.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.HomePageLink_LinkClicked);

            SelectServerBtn.Click += new EventHandler(FunctionEvents.SelectServerBtn_Click);

            CloseBTN.MouseEnter += new EventHandler(ButtonClose_MouseEnter);
            CloseBTN.MouseLeave += new EventHandler(ButtonClose_MouseLeaveANDMouseUp);
            CloseBTN.MouseUp += new MouseEventHandler(ButtonClose_MouseLeaveANDMouseUp);
            CloseBTN.MouseDown += new MouseEventHandler(ButtonClose_MouseDown);
            CloseBTN.Click += new EventHandler(CloseBTN_Click);

            SettingsButton.MouseEnter += new EventHandler(ButtonSettings_MouseEnter);
            SettingsButton.MouseLeave += new EventHandler(ButtonSettings_MouseLeaveANDMouseUp);
            SettingsButton.MouseUp += new MouseEventHandler(ButtonSettings_MouseLeaveANDMouseUp);
            SettingsButton.MouseDown += new MouseEventHandler(ButtonSettings_MouseDown);
            SettingsButton.Click += new EventHandler(SettingsButton_Click);

            ButtonSecurityCenter.MouseEnter += new EventHandler(ButtonSecurityCenter_MouseEnter);
            ButtonSecurityCenter.MouseLeave += new EventHandler(ButtonSecurityCenter_MouseLeaveANDMouseUp);
            ButtonSecurityCenter.MouseUp += new MouseEventHandler(ButtonSecurityCenter_MouseLeaveANDMouseUp);
            ButtonSecurityCenter.MouseDown += new MouseEventHandler(ButtonSecurityCenter_MouseDown);
            ButtonSecurityCenter.Click += new EventHandler(ButtonSecurityCenter_Click);

            LoginButton.MouseEnter += new EventHandler(LoginButton_MouseEnter);
            LoginButton.MouseLeave += new EventHandler(LoginButton_MouseLeave);
            LoginButton.MouseUp += new MouseEventHandler(LoginButton_MouseUp);
            LoginButton.MouseDown += new MouseEventHandler(LoginButton_MouseDown);
            LoginButton.Click += new EventHandler(LoginButton_Click);

            LogoutButton.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            LogoutButton.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            LogoutButton.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            LogoutButton.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);
            LogoutButton.Click += new EventHandler(LogoutButton_Click);

            AddServer.Click += new EventHandler(FunctionEvents.AddServer_Click);

            MainEmail.KeyUp += new KeyEventHandler(Loginbuttonenabler);
            MainEmail.KeyDown += new KeyEventHandler(LoginEnter);
            MainPassword.KeyUp += new KeyEventHandler(Loginbuttonenabler);
            MainPassword.KeyDown += new KeyEventHandler(LoginEnter);

            ServerPick.SelectedIndexChanged += new EventHandler(ServerPick_SelectedIndexChanged);
            ServerPick.DrawItem += new DrawItemEventHandler(FunctionEvents.ServerList_Menu_DrawItem);
            ServerPick.MouseWheel += new MouseEventHandler(ServerPick_MouseWheel);

            ForgotPassword.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.ForgotPassword_LinkClicked);

            MouseMove += new MouseEventHandler(MoveWindow_MouseMove);
            MouseUp += new MouseEventHandler(MoveWindow_MouseUp);
            MouseDown += new MouseEventHandler(MoveWindow_MouseDown);

            logo.MouseMove += new MouseEventHandler(MoveWindow_MouseMove);
            logo.MouseUp += new MouseEventHandler(MoveWindow_MouseUp);
            logo.MouseDown += new MouseEventHandler(MoveWindow_MouseDown);

            PlayButton.MouseEnter += new EventHandler(PlayButton_MouseEnter);
            PlayButton.MouseLeave += new EventHandler(PlayButton_MouseLeave);
            PlayButton.MouseUp += new MouseEventHandler(PlayButton_MouseUp);
            PlayButton.MouseDown += new MouseEventHandler(PlayButton_MouseDown);
            PlayButton.Click += new EventHandler(PlayButton_Click);

            RegisterText.MouseEnter += new EventHandler(Greenbutton_hover_MouseEnter);
            RegisterText.MouseLeave += new EventHandler(Greenbutton_MouseLeave);
            RegisterText.MouseUp += new MouseEventHandler(Greenbutton_hover_MouseUp);
            RegisterText.MouseDown += new MouseEventHandler(Greenbutton_click_MouseDown);
            RegisterText.Click += new EventHandler(FunctionEvents.RegisterText_LinkClicked);

            _downloader = new Downloader(this, 3, 2, 16)
            {
                ProgressUpdated = new ProgressUpdated(OnDownloadProgress),
                DownloadFinished = new DownloadFinished(DownloadTracksFiles),
                DownloadFailed = new DownloadFailed(OnDownloadFailed),
                ShowMessage = new ShowMessage(OnShowMessage),
                ShowExtract = new ShowExtract(OnShowExtract)
            };

            Load += new EventHandler(MainScreen_Load);

            /********************************/
            /* Enable/Disable Visuals        /
            /********************************/

            SelectServerBtn.Visible = EnableInsiderDeveloper.Allowed();

            /********************************/
            /* Set Hardcoded Text            /
            /********************************/

            ServerPingStatusText.Text = string.Empty;

            /********************************/
            /* Functions                     /
            /********************************/

            Shown += (x, y) =>
            {
                new Thread(() =>
                {
                    Presence_Launcher.Update();

                    if (ServerListUpdater.NoCategoryList != null && ServerListUpdater.NoCategoryList.Any())
                    {
                        foreach (Json_List_Server Servers in ServerListUpdater.NoCategoryList)
                        {
                            if (_nfswstarted == null)
                            {
                                GC.Collect();

                                try
                                {
                                    while (StillCheckingLastServer) { }
                                    Uri URLCall = new Uri(Servers.IPAddress + "/GetServerInformation");
                                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                    var Client = new WebClient
                                    {
                                        Encoding = Encoding.UTF8
                                    };

                                    if (!Launcher_Value.Launcher_Alternative_Webcalls()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                                    else
                                    {
                                        Client.Headers.Add("user-agent", "SBRW Launcher " +
                                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                    }

                                    try
                                    {
                                        JsonGSI = Client.DownloadString(URLCall);
                                        StillCheckingLastServer = true;
                                        bool GSIErrorFree = true;

                                        if (!Is_Json.Valid(JsonGSI))
                                        {
                                            GSIErrorFree = false;
                                            if (EnableInsiderBetaTester.Allowed() || EnableInsiderBetaTester.Allowed())
                                            {
                                                Log.Error("Pinging GSI (Received): " + JsonGSI);
                                            }
                                        }

                                        if (!InformationCache.ServerStatusBook.ContainsKey(Servers.ID))
                                        {
                                            InformationCache.ServerStatusBook.Add(Servers.ID, (!GSIErrorFree) ? 3 : 1);
                                        }
                                    }
                                    catch (Exception Error)
                                    {
                                        LogToFileAddons.OpenLog("Pinging GSI [DownloadString]", null, Error, null, true);

                                        if (!InformationCache.ServerStatusBook.ContainsKey(Servers.ID))
                                        {
                                            InformationCache.ServerStatusBook.Add(Servers.ID, 0);
                                        }
                                    }
                                    finally
                                    {
                                        StillCheckingLastServer = false;

                                        if (Client != null)
                                        {
                                            Client.Dispose();
                                        }
                                    }
                                }
                                catch (Exception Error)
                                {
                                    LogToFileAddons.OpenLog("Pinging GSI [WebClient]", null, Error, null, true);
                                }
                                finally
                                {
                                    if (JsonGSI != null)
                                    {
                                        JsonGSI = null;
                                    }
                                }
                            }
                        }
                    }
                }).Start();

                GC.Collect();
            };

            if (!UnixOS.Detected())
            {
                try
                {
                    string CursorFile = (!UnixOS.Detected()) ? Path.GetTempFileName() : Path.Combine(Locations.LauncherFolder, "Cursor.ani");
                    File.WriteAllBytes(CursorFile, ExtractResource.AsByte("GameLauncher.Resources.Cursors.Cursor.ani"));
                    Cursor mycursor = new Cursor(Cursor.Current.Handle);
                    IntPtr colorcursorhandle = User32.LoadCursorFromFile(CursorFile);
                    mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField,
                        null, mycursor, new object[] { colorcursorhandle });
                    Cursor = mycursor;
                    File.Delete(CursorFile);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("CURSOR", null, Error, null, true);
                }
            }
        }
    }
    /* Moved 7 Unused Code to Gist */
    /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a8F89a5e5aebce38/raw/00de505302fbf9f8cfea9b163a707d9f8f122552/MainScreen.cs */
}
