using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Threading.Tasks;
using GameLauncher.App;
using GameLauncher.App.Classes.Auth;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Validator.JSON;
using GameLauncher.App.Classes.LauncherCore.Validator.Email;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Downloader;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.Hash;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.SystemPlatform;
using GameLauncher.App.Classes.LauncherCore.LauncherUpdater;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.ModNet.JSON;
using GameLauncher.App.Classes.LauncherCore;
using GameLauncher.App.Classes.LauncherCore.Client.Auth;

namespace GameLauncher
{
    public partial class MainScreen : Form
    {
        private Point _mouseDownPoint = Point.Empty;
        private bool _loginEnabled;
        private bool _serverEnabled;
        private bool _builtinserver;
        private bool _useSavedPassword;
        private bool _skipServerTrigger;
        private bool _playenabled;
        private bool _isDownloading = true;
        private bool _disableLogout = false;

        public static String getTempNa = Path.GetTempFileName();

        private int _lastSelectedServerId;
        private int _nfswPid;
        private Thread _nfswstarted;

        private DateTime _downloadStartTime;
        private Downloader _downloader;

        public string _serverWebsiteLink = "";
        public string _serverFacebookLink = "";
        public string _serverDiscordLink = "";
        public string _serverTwitterLink = "";
        private string _loginWelcomeTime = "";
        private string _loginToken = "";
        private string _userId = "";
        private string _langInfo;

        public static String ModNetFileNameInUse = String.Empty;
        readonly Queue<Uri> modFilesDownloadUrls = new Queue<Uri>();
        bool isDownloadingModNetFiles = false;
        int CurrentModFileCount = 0;
        int TotalModFileCount = 0;

        readonly String filename_pack = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameFiles.sbrwpack");

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

        public MainScreen()
        {
            InitializeComponent();
            SetVisuals();
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            Log.Visuals("CORE: Entering mainScreen_Load");

            FunctionStatus.CenterScreen(this);
            Application.OpenForms["MainScreen"].Activate();
            Log.Core("CORE: Setting Parent Window location");
            InformationCache.ParentScreenLocation = Location;

            if (!string.IsNullOrEmpty(InsiderInfo.BuildNumber()))
            {
                if (EnableInsiderDeveloper.Allowed() == true || EnableInsiderBetaTester.Allowed() == true)
                {
                    InsiderBuildNumberText.Visible = true;
                    InsiderBuildNumberText.Text = InsiderInfo.BuildNumber();
                }
                else
                {
                    InsiderBuildNumberText.Visible = false;
                }
            }

            Log.Core("LAUNCHER: NFSW Download Source is now: " + FileSettingsSave.CDN);

            Log.Visuals("CORE: Applyinng ContextMenu");
            translatedBy.Text = "";
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(new MenuItem("About", FunctionEvents.AboutButton_Click));
            ContextMenu.MenuItems.Add(new MenuItem("Donate", (b, n) => { Process.Start("https://paypal.me/metonator95"); }));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem("Add Server", FunctionEvents.AddServer_Click));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem("Close launcher", CloseBTN_Click));

            Notification.ContextMenu = ContextMenu;
            Notification.Icon = new Icon(Icon, Icon.Width, Icon.Height);
            Notification.Text = "GameLauncher";
            Notification.Visible = true;

            ContextMenu = null;

            MainEmail.Text = FileAccountSave.UserRawEmail;
            MainPassword.Text = FileAccountSave.UserRawPassword;
            if (!string.IsNullOrEmpty(FileAccountSave.UserRawEmail) && !string.IsNullOrEmpty(FileAccountSave.UserHashedPassword))
            {
                Log.Core("LAUNCHER: Restoring last saved email and password");
                RememberMe.Checked = true;
            }

            /* Server Display List */
            ServerPick.DisplayMember = "Name";
            Log.Core("LAUNCHER: Setting server list");
            ServerPick.DataSource = ServerListUpdater.CleanList;

            /* Display Server List Dialog if Server IP Doesn't Exist */
            if (string.IsNullOrEmpty(FileAccountSave.ChoosenGameServer))
            {
                new SelectServer().ShowDialog();

                if (SelectedServer.Data != null)
                {
                    FileAccountSave.ChoosenGameServer = SelectedServer.Data.IpAddress;
                    FileAccountSave.SaveAccount();
                }
                else
                {
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }
            }

            Log.Core("SERVERLIST: Checking...");
            Log.Core("SERVERLIST: Setting first server in list");
            Log.Core("SERVERLIST: Checking if server is set on INI File");
            try
            {
                if (string.IsNullOrEmpty(FileAccountSave.ChoosenGameServer))
                {
                    Log.Warning("SERVERLIST: Failed to find anything... assuming " + ((ServerList)ServerPick.SelectedItem).IpAddress);
                    FileAccountSave.ChoosenGameServer = ((ServerList)ServerPick.SelectedItem).IpAddress;
                    FileAccountSave.SaveAccount();
                }
            }
            catch
            {
                Log.Error("SERVERLIST: Failed to write anything...");
                FileAccountSave.ChoosenGameServer = string.Empty;
                FileAccountSave.SaveAccount();
            }

            Log.Core("SERVERLIST: Re-Checking if server is set on INI File");
            if (!string.IsNullOrEmpty(FileAccountSave.ChoosenGameServer))
            {
                Log.Core("SERVERLIST: Found something!");
                _skipServerTrigger = true;

                Log.Core("SERVERLIST: Checking if server exists on our database");

                if (ServerListUpdater.CleanList.FindIndex(i => string.Equals(i.IpAddress, FileAccountSave.ChoosenGameServer)) != 0)
                {
                    Log.Core("SERVERLIST: Server found! Checking ID");
                    var index = ServerListUpdater.CleanList.FindIndex(i => string.Equals(i.IpAddress, FileAccountSave.ChoosenGameServer));

                    Log.Core("SERVERLIST: ID is " + index);
                    if (index >= 0)
                    {
                        Log.Core("SERVERLIST: ID set correctly");
                        ServerPick.SelectedIndex = index;
                    }
                }
                else
                {
                    Log.Warning("SERVERLIST: Unable to find anything, assuming default");
                    ServerPick.SelectedIndex = 1;
                    Log.Warning("SERVERLIST: Deleting unknown entry");
                    FileAccountSave.ChoosenGameServer = string.Empty;
                    FileAccountSave.SaveAccount();
                }

                Log.Core("SERVERLIST: Triggering server change");
                if (ServerPick.SelectedIndex == 1)
                {
                    ServerPick_SelectedIndexChanged(sender, e);
                }
                Log.Core("SERVERLIST: All done");
            }

            Log.Core("LAUNCHER: Checking for password");
            if (!string.IsNullOrEmpty(FileAccountSave.UserHashedPassword))
            {
                _loginEnabled = true;
                _serverEnabled = true;
                _useSavedPassword = true;
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.ForeColor = Theming.FivithTextForeColor;
            }
            else
            {
                _loginEnabled = false;
                _serverEnabled = false;
                _useSavedPassword = false;
                LoginButton.BackgroundImage = Theming.GrayButton;
                LoginButton.ForeColor = Theming.SixTextForeColor;
            }

            Log.Core("LAUNCHER: Re-checking InstallationDirectory: " + FileSettingsSave.GameInstallation);

            var drive = Path.GetPathRoot(FileSettingsSave.GameInstallation);
            if (!Directory.Exists(drive))
            {
                if (!string.IsNullOrEmpty(drive))
                {
                    var newdir = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                    FileSettingsSave.GameInstallation = newdir;
                    FileSettingsSave.SaveSettings();
                    Log.Error(string.Format("LAUNCHER: Drive {0} was not found. Your actual installation directory is set to {1} now.", drive, newdir));

                    MessageBox.Show(null, string.Format("Drive {0} was not found. Your actual installation directory is set to {1} now.", drive, newdir), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            BeginInvoke((MethodInvoker)delegate
            {
                Log.Core("CORE: 'GetServerInformation' from all Servers in Server List and Download Selected Server Banners");
                LaunchNfsw();
            });

            this.BringToFront();

            if (!DetectLinux.LinuxDetected())
            {
                new LauncherUpdateCheck(LauncherIconStatus, LauncherStatusText, LauncherStatusDesc).ChangeVisualStatus();
            }
            else
            {
                LauncherIconStatus.BackgroundImage = Theming.UpdateIconSuccess;
                LauncherStatusText.ForeColor = Theming.Sucess;
                LauncherStatusText.Text = "Launcher Status:\n - Linux Build";
                LauncherStatusDesc.Text = "Version: v" + Application.ProductVersion;
            }

            /* Load Settings API Connection Status */
            Task.Delay(800);
            PingServerListAPIStatus();

            /* Remove TracksHigh Folder and Files */
            RemoveTracksHighFiles();
        }

        private void CloseBTN_Click(object sender, EventArgs e)
        {
            CloseBTN.BackgroundImage = Theming.CloseButtonClick;

            FileSettingsSave.SaveSettings();
            FileAccountSave.SaveAccount();

            Process[] allOfThem = Process.GetProcessesByName("nfsw");
            foreach (var oneProcess in allOfThem)
            {
                Process.GetProcessById(oneProcess.Id).Kill();
            }

            /* Kill DiscordRPC */
            if (DiscordLauncherPresense.Client != null)
            {
                DiscordLauncherPresense.Stop();
            }

            if (ServerProxy.Host != null)
            {
                ServerProxy.Instance.Stop("Main Screen");
            }
            
            Notification.Dispose();

            var linksPath = Path.Combine(FileSettingsSave.GameInstallation + "\\.links");
            ModNetHandler.CleanLinks(linksPath);

            /* Leave this here. Its to properly close the launcher from Visual Studio (And Close the Launcher a well) */
            try { this.Close(); } catch { }
        }

        private void CloseBTN_MouseEnter(object sender, EventArgs e)
        {
            CloseBTN.BackgroundImage = Theming.CloseButtonHover;
        }

        private void CloseBTN_MouseLeave(object sender, EventArgs e)
        {
            CloseBTN.BackgroundImage = Theming.CloseButton;
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
            if (string.IsNullOrEmpty(MainEmail.Text) || string.IsNullOrEmpty(MainPassword.Text))
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

            _useSavedPassword = false;
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
                MessageBox.Show(null, "Please wait while launcher is still downloading gamefiles.", "GameLauncher", MessageBoxButtons.OK);
                return;
            }

            Tokens.Clear();

            String username = MainEmail.Text.ToString();
            String realpass;

            Tokens.IPAddress = InformationCache.SelectedServerData.IpAddress;
            Tokens.ServerName = InformationCache.SelectedServerData.Name;

            InformationCache.UserAgent = InformationCache.SelectedServerData.ForceUserAgent ?? null;

            if (InformationCache.ModernAuthSupport == false)
            {
                /* ClassicAuth sends password in SHA1 */
                realpass = (_useSavedPassword) ? FileAccountSave.UserHashedPassword : SHA.HashPassword(MainPassword.Text.ToString()).ToLower();
                Authentication.Client("Login", "Non Secure", username, realpass, null);
                //ClassicAuth.Login(username, realpass);
            }
            else
            {
                /* ModernAuth sends passwords in plaintext, but is POST request */
                realpass = (_useSavedPassword) ? FileAccountSave.UserHashedPassword : MainPassword.Text.ToString();
                Authentication.Client("Login", "Secure", username, realpass, null);
                //ModernAuth.Login(username, realpass);
            }

            if (RememberMe.Checked)
            {
                FileAccountSave.UserRawEmail = username;
                FileAccountSave.UserHashedPassword = realpass;
                FileAccountSave.UserRawPassword = MainPassword.Text.ToString();
                FileAccountSave.SaveAccount();
            }
            else
            {
                FileAccountSave.UserRawEmail = String.Empty;
                FileAccountSave.UserHashedPassword = String.Empty;
                FileAccountSave.UserRawPassword = string.Empty;
                FileAccountSave.SaveAccount();
            }

            try
            {
                if (!(ServerPick.SelectedItem is ServerList server)) return;
                FileAccountSave.ChoosenGameServer = server.IpAddress;
            }
            catch { }

            if (String.IsNullOrEmpty(Tokens.Error))
            {
                _userId = Tokens.UserId;
                _loginToken = Tokens.LoginToken;
                InformationCache.SelectedServerData.IpAddress = Tokens.IPAddress;

                if (!String.IsNullOrEmpty(Tokens.Warning))
                {
                    MessageBox.Show(null, Tokens.Warning, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                LoginFormElements(false);
                LoggedInFormElements(true);
            }
            else
            {
                /* Main Screen Login */
                MainEmailBorder.Image = Theming.BorderEmailError;
                MainPasswordBorder.Image = Theming.BorderPasswordError;
                MessageBox.Show(null, Tokens.Error, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MainEmailBorder.Image = Theming.BorderEmail;
            MainPasswordBorder.Image = Theming.BorderPassword;

            InformationCache.SelectedServerData = (ServerList)ServerPick.SelectedItem;
            InformationCache.ModernAuthSupport = false;

            if (InformationCache.SelectedServerData.IsSpecial)
            {
                ServerPick.SelectedIndex = _lastSelectedServerId;
                return;
            }

            if (!_skipServerTrigger) { return; }

            _lastSelectedServerId = ServerPick.SelectedIndex;
            FunctionStatus.AllowRegistration = false;
            _loginEnabled = false;

            ServerStatusText.Text = "Server Status:\n - Pinging";
            ServerStatusText.ForeColor = Theming.SecondaryTextForeColor;
            ServerStatusDesc.Text = "";
            ServerStatusIcon.BackgroundImage = Theming.ServerIconChecking;

            LoginButton.ForeColor = Theming.SixTextForeColor;
            var verticalImageUrl = "";
            VerticalBanner.Image = VerticalBanners.Grayscale(".BannerCache/" + SHA.HashPassword(InformationCache.SelectedServerData.Name) + ".bin");
            VerticalBanner.BackColor = Theming.VerticalBannerBackColor;

            string numPlayers = "";
            string numRegistered = "";

            /* Disable Social Panel when switching */
            DisableSocialPanelandClearIt();

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

            FunctionStatus.TLS();
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");

            var artificialPingStart = Time.GetStamp();
            VerticalBanner.BackColor = Color.Transparent;

            var stringToUri = new Uri(InformationCache.SelectedServerData.IpAddress + "/GetServerInformation");
            client.DownloadStringAsync(stringToUri);

            System.Timers.Timer aTimer = new System.Timers.Timer(10000);
            aTimer.Elapsed += (x, y) => { client.CancelAsync(); };
            aTimer.Enabled = true;

            client.DownloadStringCompleted += (sender2, e2) =>
            {
                aTimer.Enabled = false;

                var artificialPingEnd = Time.GetStamp();

                if (e2.Cancelled)
                {
                    ServerStatusText.Text = "Server Status:\n - Offline ( OFF )";
                    ServerStatusText.ForeColor = Theming.Error;
                    ServerStatusDesc.Text = "Failed to connect to server.";
                    ServerStatusIcon.BackgroundImage = Theming.ServerIconOffline;
                    _serverEnabled = false;
                    FunctionStatus.AllowRegistration = false;
                    /* Disable Login & Register Button */
                    LoginButton.Enabled = false;
                    RegisterText.Enabled = false;
                    /* Disable Social Panel */
                    DisableSocialPanelandClearIt();

                    if (!InformationCache.ServerStatusBook.ContainsKey(InformationCache.SelectedServerData.Id))
                    {
                        InformationCache.ServerStatusBook.Add(InformationCache.SelectedServerData.Id, 2);
                    }
                    else
                    {
                        InformationCache.ServerStatusBook[InformationCache.SelectedServerData.Id] = 2;
                    }
                }
                else if (e2.Error != null)
                {
                    ServerStatusText.Text = "Server Status:\n - Offline ( OFF )";
                    ServerStatusText.ForeColor = Theming.Error;
                    ServerStatusDesc.Text = "Server seems to be offline.";
                    ServerStatusIcon.BackgroundImage = Theming.ServerIconOffline;
                    _serverEnabled = false;
                    FunctionStatus.AllowRegistration = false;
                    /* Disable Login & Register Button */
                    LoginButton.Enabled = false;
                    RegisterText.Enabled = false;
                    /* Disable Social Panel */
                    DisableSocialPanelandClearIt();

                    if (!InformationCache.ServerStatusBook.ContainsKey(InformationCache.SelectedServerData.Id))
                    {
                        InformationCache.ServerStatusBook.Add(InformationCache.SelectedServerData.Id, 0);
                    }
                    else
                    {
                        InformationCache.ServerStatusBook[InformationCache.SelectedServerData.Id] = 0;
                    }
                }
                else
                {
                    if (InformationCache.SelectedServerData.Name == "Offline Built-In Server")
                    {
                        DisableSocialPanelandClearIt();
                        numPlayers = "∞";
                        numRegistered = "∞";
                    }
                    else
                    {
                        if (!InformationCache.ServerStatusBook.ContainsKey(InformationCache.SelectedServerData.Id))
                        {
                            InformationCache.ServerStatusBook.Add(InformationCache.SelectedServerData.Id, 1);
                        }
                        else
                        {
                            InformationCache.ServerStatusBook[InformationCache.SelectedServerData.Id] = 1;
                        }

                        try
                        {
                            /* Enable Social Panel  */
                            ServerInfoPanel.Visible = true;
                        }
                        catch { }

                        String purejson = String.Empty;
                        purejson = e2.Result;
                        InformationCache.SelectedServerJSON = JsonConvert.DeserializeObject<GetServerInformation>(e2.Result);

                        try
                        {
                            if (!string.IsNullOrEmpty(InformationCache.SelectedServerJSON.bannerUrl))
                            {
                                bool result;

                                try
                                {
                                    result = Uri.TryCreate(InformationCache.SelectedServerJSON.bannerUrl, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch
                                {
                                    result = false;
                                }

                                if (result)
                                {
                                    verticalImageUrl = InformationCache.SelectedServerJSON.bannerUrl;
                                }
                                else
                                {
                                    verticalImageUrl = null;
                                }
                            }
                            else
                            {
                                verticalImageUrl = null;
                            }
                        }
                        catch
                        {
                            verticalImageUrl = null;
                        }

                        /* Social Panel Core */

                        /* Discord Invite Display */
                        try
                        {
                            if (string.IsNullOrEmpty(InformationCache.SelectedServerJSON.discordUrl))
                            {
                                DiscordIcon.BackgroundImage = Theming.DiscordIconDisabled;
                                DiscordInviteLink.Enabled = false;
                                _serverDiscordLink = null;
                                DiscordInviteLink.Text = "";
                            }
                            else
                            {
                                DiscordIcon.BackgroundImage = Theming.DiscordIcon;
                                DiscordInviteLink.Enabled = true;
                                _serverDiscordLink = InformationCache.SelectedServerJSON.discordUrl;
                                DiscordInviteLink.Text = "Discord Invite";
                            }
                        }
                        catch { }

                        /* Homepage Display */
                        try
                        {
                            if (string.IsNullOrEmpty(InformationCache.SelectedServerJSON.homePageUrl))
                            {
                                HomePageIcon.BackgroundImage = Theming.HomeIconDisabled;
                                HomePageLink.Enabled = false;
                                _serverWebsiteLink = null;
                                HomePageLink.Text = "";
                            }
                            else
                            {
                                HomePageIcon.BackgroundImage = Theming.HomeIcon;
                                HomePageLink.Enabled = true;
                                _serverWebsiteLink = InformationCache.SelectedServerJSON.homePageUrl;
                                HomePageLink.Text = "Home Page";
                            }
                        }
                        catch { }

                        /* Facebook Group Display */
                        try
                        {
                            if (string.IsNullOrEmpty(InformationCache.SelectedServerJSON.facebookUrl) || InformationCache.SelectedServerJSON.facebookUrl == "Your facebook page url")
                            {
                                FacebookIcon.BackgroundImage = Theming.FacebookIconDisabled;
                                FacebookGroupLink.Enabled = false;
                                _serverFacebookLink = null;
                                FacebookGroupLink.Text = "";
                            }
                            else
                            {
                                FacebookIcon.BackgroundImage = Theming.FacebookIcon;
                                FacebookGroupLink.Enabled = true;
                                _serverFacebookLink = InformationCache.SelectedServerJSON.facebookUrl;
                                FacebookGroupLink.Text = "Facebook Page";
                            }
                        }
                        catch { }

                        /* Twitter Account Display */
                        try
                        {
                            if (string.IsNullOrEmpty(InformationCache.SelectedServerJSON.twitterUrl))
                            {
                                TwitterIcon.BackgroundImage = Theming.TwitterIconDisabled;
                                TwitterAccountLink.Enabled = false;
                                _serverTwitterLink = null;
                                TwitterAccountLink.Text = "";
                            }
                            else
                            {
                                TwitterIcon.BackgroundImage = Theming.TwitterIcon;
                                TwitterAccountLink.Enabled = true;
                                _serverTwitterLink = InformationCache.SelectedServerJSON.twitterUrl;
                                TwitterAccountLink.Text = "Twitter Feed";
                            }
                        }
                        catch { }

                        /* Server Set Speedbug Timer Display */
                        try
                        {
                            int serverSecondsToShutDown = (InformationCache.SelectedServerJSON.secondsToShutDown != 0) ? InformationCache.SelectedServerJSON.secondsToShutDown : 2 * 60 * 60;
                            string serverSecondsToShutDownNamed = string.Format("Gameplay Timer: " + TimeConversions.RelativeTime(serverSecondsToShutDown));

                            this.ServerShutDown.Text = serverSecondsToShutDownNamed;
                        }
                        catch { }

                        try
                        {
                            /* Scenery Group Display */
                            switch (String.Join("", InformationCache.SelectedServerJSON.activatedHolidaySceneryGroups))
                            {
                                case "SCENERY_GROUP_NEWYEARS":
                                    this.SceneryGroupText.Text = "Scenery: New Years";
                                    break;
                                case "SCENERY_GROUP_OKTOBERFEST":
                                    this.SceneryGroupText.Text = "Scenery: OKTOBERFEST";
                                    break;
                                case "SCENERY_GROUP_HALLOWEEN":
                                    this.SceneryGroupText.Text = "Scenery: Halloween";
                                    break;
                                case "SCENERY_GROUP_CHRISTMAS":
                                    this.SceneryGroupText.Text = "Scenery: Christmas";
                                    break;
                                case "SCENERY_GROUP_VALENTINES":
                                    this.SceneryGroupText.Text = "Scenery: Valentines";
                                    break;
                                default:
                                    this.SceneryGroupText.Text = "Scenery: Normal";
                                    break;
                            }
                        }
                        catch { }

                        try
                        {
                            if (InformationCache.SelectedServerJSON.modernAuthSupport == "true")
                            {
                                if (stringToUri.Scheme == "https")
                                {
                                    InformationCache.ModernAuthSupport = true;
                                }
                                else
                                {
                                    InformationCache.ModernAuthSupport = false;
                                }
                            }
                            else
                            {
                                InformationCache.ModernAuthSupport = false;
                            }
                        }
                        catch
                        {
                            InformationCache.ModernAuthSupport = false;
                        }

                        if (InformationCache.SelectedServerJSON.maxOnlinePlayers != 0)
                        {
                            numPlayers = string.Format("{0} / {1}", InformationCache.SelectedServerJSON.onlineNumber, InformationCache.SelectedServerJSON.maxOnlinePlayers.ToString());
                            numRegistered = string.Format("{0}", InformationCache.SelectedServerJSON.numberOfRegistered);
                        }
                        else if (InformationCache.SelectedServerJSON.maxUsersAllowed != 0)
                        {
                            numPlayers = string.Format("{0} / {1}", InformationCache.SelectedServerJSON.onlineNumber, InformationCache.SelectedServerJSON.maxUsersAllowed.ToString());
                            numRegistered = string.Format("{0}", InformationCache.SelectedServerJSON.numberOfRegistered);
                        }
                        else if ((InformationCache.SelectedServerJSON.maxUsersAllowed == 0) || (InformationCache.SelectedServerJSON.maxOnlinePlayers == 0))
                        {
                            numPlayers = string.Format("{0}", InformationCache.SelectedServerJSON.onlineNumber);
                            numRegistered = string.Format("{0}", InformationCache.SelectedServerJSON.numberOfRegistered);
                        }

                        FunctionStatus.AllowRegistration = true;
                    }

                    try
                    {
                        ServerStatusText.Text = "Server Status:\n - Online ( ON )";
                        ServerStatusText.ForeColor = Theming.Sucess;
                        ServerStatusIcon.BackgroundImage = Theming.ServerIconSuccess;
                        _loginEnabled = true;
                        /* Enable Login & Register Button */
                        LoginButton.ForeColor = Theming.FivithTextForeColor;
                        LoginButton.Enabled = true;
                        RegisterText.Enabled = true;
                        InformationCache.SelectedServerCategory = ((ServerList)ServerPick.SelectedItem).Category;
                        InformationCache.RestartTimer = (InformationCache.SelectedServerJSON.secondsToShutDown != 0) ? InformationCache.SelectedServerJSON.secondsToShutDown : 2 * 60 * 60;

                        if (InformationCache.SelectedServerCategory == "DEV")
                        {
                            /* Disable Social Panel */
                            DisableSocialPanelandClearIt();
                        }
                    }
                    catch
                    {
                        //¯\_(ツ)_/¯
                    }

                    if (!DetectLinux.LinuxDetected())
                    {
                        ServerPingStatusText.ForeColor = Theming.FivithTextForeColor;

                        Ping pingSender = new Ping();
                        pingSender.SendAsync(stringToUri.Host, 1000, new byte[1], new PingOptions(64, true), new AutoResetEvent(false));
                        pingSender.PingCompleted += (sender3, e3) => {
                            PingReply reply = e3.Reply;

                            if (reply.Status == IPStatus.Success && InformationCache.SelectedServerData.Name != "Offline Built-In Server")
                            {
                                if (this.ServerPingStatusText.InvokeRequired)
                                {
                                    ServerStatusDesc.Invoke(new Action(delegate () {
                                        ServerPingStatusText.Text = string.Format("Your Ping to the Server \n{0}".ToUpper(), reply.RoundtripTime + "ms");
                                    }));
                                }
                                else
                                {
                                    this.ServerPingStatusText.Text = string.Format("Your Ping to the Server \n{0}".ToUpper(), reply.RoundtripTime + "ms");
                                }
                            }
                            else
                            {
                                this.ServerPingStatusText.Text = string.Format("");
                            }
                        };
                    }
                    else
                    {
                        this.ServerPingStatusText.Text = string.Format("");
                    }

                    /* for thread safety */
                    if (this.ServerStatusDesc.InvokeRequired)
                    {
                        ServerStatusDesc.Invoke(new Action(delegate ()
                        {
                            ServerStatusDesc.Text = string.Format("Online: {0}\nRegistered: {1}", numPlayers, numRegistered);
                        }));
                    }
                    else
                    {
                        this.ServerStatusDesc.Text = string.Format("Online: {0}\nRegistered: {1}", numPlayers, numRegistered);
                    }

                    _serverEnabled = true;

                    if (!Directory.Exists(".BannerCache")) { Directory.CreateDirectory(".BannerCache"); }
                    if (!string.IsNullOrEmpty(verticalImageUrl))
                    {
                        FunctionStatus.TLS();
                        WebClient client2 = new WebClient();
                        client2.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        Uri stringToUri3 = new Uri(verticalImageUrl);
                        client2.DownloadDataAsync(stringToUri3);
                        client2.DownloadProgressChanged += (sender4, e4) =>
                        {
                            if (e4.TotalBytesToReceive > 2000000)
                            {
                                client2.CancelAsync();
                                Log.Warning("Unable to Cache " + InformationCache.SelectedServerData.Name + " Server Banner! {Over 2MB?}");
                            }
                        };

                        client2.DownloadDataCompleted += (sender4, e4) =>
                        {
                            if (e4.Cancelled)
                            {
                                /* Load cached banner! */
                                VerticalBanner.Image = VerticalBanners.Grayscale(".BannerCache/" + SHA.HashPassword(InformationCache.SelectedServerData.Name) + ".bin");
                                VerticalBanner.BackColor = Theming.VerticalBannerBackColor;
                                return;
                            }
                            else if (e4.Error != null)
                            {
                                /* Load cached banner! */
                                VerticalBanner.Image = VerticalBanners.Grayscale(".BannerCache/" + SHA.HashPassword(InformationCache.SelectedServerData.Name) + ".bin");
                                VerticalBanner.BackColor = Theming.VerticalBannerBackColor;
                                return;
                            }
                            else
                            {
                                try
                                {
                                    Image image;
                                    var memoryStream = new MemoryStream(e4.Result);
                                    image = Image.FromStream(memoryStream);

                                    VerticalBanner.Image = image;
                                    VerticalBanner.BackColor = Theming.VerticalBannerBackColor;

                                    if (VerticalBanners.GetFileExtension(verticalImageUrl) != "gif")
                                    {
                                        File.WriteAllBytes(".BannerCache/" + SHA.HashPassword(InformationCache.SelectedServerData.Name) + ".bin", memoryStream.ToArray());
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error("SERVER BANNER: " + ex.Message);
                                    VerticalBanner.Image = null;
                                }
                            }
                        };
                    }
                    else
                    {
                        /* Load cached banner! */
                        VerticalBanner.Image = VerticalBanners.Grayscale(".BannerCache/" + SHA.HashPassword(InformationCache.SelectedServerData.Name) + ".bin");
                        VerticalBanner.BackColor = Theming.VerticalBannerBackColor;
                    }
                }
            };
        }

        /* Main Screen Elements */

        /* Social Panel | Ping or Offline | */
        private void DisableSocialPanelandClearIt()
        {
            /* Hides Social Panel */
            ServerInfoPanel.Visible = false;
            /* Home */
            HomePageIcon.BackgroundImage = Theming.HomeIconDisabled;
            HomePageLink.Enabled = false;
            _serverWebsiteLink = null;
            /* Discord */
            DiscordIcon.BackgroundImage = Theming.DiscordIconDisabled;
            DiscordInviteLink.Enabled = false;
            _serverDiscordLink = null;
            /* Facebook */
            FacebookIcon.BackgroundImage = Theming.FacebookIconDisabled;
            FacebookGroupLink.Enabled = false;
            _serverFacebookLink = null;
            /* Twitter */
            TwitterIcon.BackgroundImage = Theming.TwitterIconDisabled;
            TwitterAccountLink.Enabled = false;
            _serverTwitterLink = null;
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
                DateTime currentTime = DateTime.Now;

                if (currentTime.Hour < 12)
                {
                    _loginWelcomeTime = "Good Morning";
                }
                else if (currentTime.Hour <= 16)
                {
                    _loginWelcomeTime = "Good Afternoon";
                }
                else if (currentTime.Hour <= 20)
                {
                    _loginWelcomeTime = "Good Evening";
                }
                else
                {
                    _loginWelcomeTime = "Good Night";
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

            _userId = String.Empty;
            _loginToken = String.Empty;
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

        /* SETTINGS PAGE LAYOUT */
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            if (InformationCache.CDNListStatus != "Loaded")
            {
                CDNListUpdater.GetList();
            }

            SettingsButton.BackgroundImage = Theming.GearButtonClick;

            if (!(ServerPick.SelectedItem is ServerList server)) return;

            new SettingsScreen(server.IpAddress, server.Name).ShowDialog();
        }

        private void SettingsButton_MouseEnter(object sender, EventArgs e)
        {
            SettingsButton.BackgroundImage = Theming.GearButtonHover;
        }

        private void SettingsButton_MouseLeave(object sender, EventArgs e)
        {
            SettingsButton.BackgroundImage = Theming.GearButton;
        }

        private void StartGame(string UserID, string LoginToken)
        {
            if (InformationCache.SelectedServerData.Name == "Freeroam Sparkserver")
            {
                if (ServerProxy.Host == null)
                {
                    ServerProxy.Instance.Start("Start Game");
                }
                /* Force start proxy and enable it */
                FunctionStatus.DisableProxy = false;
            }

            _nfswstarted = new Thread(() =>
            {
                if (FunctionStatus.DisableProxy == true || ServerProxy.Host == null)
                {
                    Uri convert = new Uri(InformationCache.SelectedServerData.IpAddress);

                    if (convert.Scheme == "http")
                    {
                        Match match = Regex.Match(convert.Host, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
                        if (!match.Success)
                        {
                            InformationCache.SelectedServerData.IpAddress = InformationCache.SelectedServerData.IpAddress.Replace(convert.Host, FunctionStatus.HostName2IP(convert.Host));
                        }
                    }

                    LaunchGame(UserID, LoginToken, InformationCache.SelectedServerData.IpAddress, this);
                }
                else
                {
                    LaunchGame(UserID, LoginToken, "http://127.0.0.1:" + ServerProxy.ProxyPort + "/nfsw/Engine.svc", this);
                }
            })
            { IsBackground = true };

            _nfswstarted.Start();

            DiscordLauncherPresense.Status("In-Game", InformationCache.SelectedServerData.DiscordPresenceKey);
        }

        /* Check Serverlist API Status Upon Main Screen load - DavidCarbon */
        private void PingServerListAPIStatus()
        {
            APIStatusText.Text = "United API:\n - Online";
            APIStatusText.ForeColor = Theming.Sucess;
            APIStatusDesc.Text = "Connected to API";
            APIStatusIcon.BackgroundImage = Theming.APIIconSuccess;

            if (VisualsAPIChecker.UnitedAPI != true)
            {
                APIStatusText.Text = "Carbon API:\n - Online";
                APIStatusText.ForeColor = Theming.Sucess;
                APIStatusDesc.Text = "Connected to API";
                APIStatusIcon.BackgroundImage = Theming.APIIconSuccess;
            }
            else if (VisualsAPIChecker.CarbonAPI != true)
            {
                APIStatusText.Text = "Carbon 2nd API:\n - Online";
                APIStatusText.ForeColor = Theming.Sucess;
                APIStatusDesc.Text = "Connected to API";
                APIStatusIcon.BackgroundImage = Theming.APIIconSuccess;
            }
            else if (VisualsAPIChecker.CarbonAPITwo != true)
            {
                APIStatusText.Text = "WOPL API:\n - Online";
                APIStatusText.ForeColor = Theming.Sucess;
                APIStatusDesc.Text = "Connected to API";
                APIStatusIcon.BackgroundImage = Theming.APIIconSuccess;
            }
            else if (VisualsAPIChecker.WOPLAPI != true)
            {
                APIStatusText.Text = "Connection API:\n - Error";
                APIStatusText.ForeColor = Theming.Error;
                APIStatusDesc.Text = "Failed to Connect to APIs";
                APIStatusIcon.BackgroundImage = Theming.APIIconError;
                Log.Api("PINGING API: Failed to Connect to APIs! Quick Hide and Bunker Down! (Ask for help)");
            }
        }

        private void LaunchGame(string UserID, string LoginToken, string ServerIP, Form x)
        {
            var psi = new ProcessStartInfo();

            if (DetectLinux.LinuxDetected())
            {
                psi.UseShellExecute = false;
            }

            psi.WorkingDirectory = FileSettingsSave.GameInstallation;
            psi.FileName = FileSettingsSave.GameInstallation + "\\nfsw.exe";
            psi.Arguments = InformationCache.SelectedServerData.Id.ToUpper() + " " + ServerIP + " " + LoginToken + " " + UserID;

            var nfswProcess = Process.Start(psi);
            nfswProcess.PriorityClass = ProcessPriorityClass.AboveNormal;

            var processorAffinity = 0;
            for (var i = 0; i < Math.Min(Math.Max(1, Environment.ProcessorCount), 8); i++)
            {
                processorAffinity |= 1 << i;
            }

            nfswProcess.ProcessorAffinity = (IntPtr)processorAffinity;

            AntiCheat.process_id = nfswProcess.Id;

            /* TIMER HERE */
            System.Timers.Timer shutdowntimer = new System.Timers.Timer();
            shutdowntimer.Elapsed += (x2, y2) =>
            {
                int ProcessID = 0;

                if (InformationCache.RestartTimer == 300)
                {
                    Notification.Visible = true;
                    Notification.BalloonTipIcon = ToolTipIcon.Info;
                    Notification.BalloonTipTitle = "Force Restart - " + InformationCache.SelectedServerData.Name;
                    Notification.BalloonTipText = "Game is going to shut down in 5 minutes. Please restart it manually before the launcher does it.";
                    Notification.ShowBalloonTip(5000);
                    Notification.Dispose();
                }

                Process[] allOfThem = Process.GetProcessesByName("nfsw");

                if (InformationCache.RestartTimer <= 0)
                {
                    if (FunctionStatus.CanCloseGame == true)
                    {
                        foreach (var oneProcess in allOfThem)
                        {
                            FunctionStatus.GameKilledBySpeedBugCheck = true;
                            Process.GetProcessById(oneProcess.Id).Kill();
                        }
                    }
                    else
                    {
                        InformationCache.RestartTimer = 0;
                    }
                }

                if (FunctionStatus.DisableProxy == true)
                {
                    if (ProcessID == 0)
                    {
                        ProcessID++;
                        AntiCheat.LocalEnablechecks();
                    }

                    if (FunctionStatus.ExternalToolsWasUsed == true && ProcessID == 1)
                    {
                        ProcessID++;
                        AntiCheat.DisableChecks();
                    }
                }

                /* change title */

                foreach (var oneProcess in allOfThem)
                {
                    long p = oneProcess.MainWindowHandle.ToInt64();
                    TimeSpan t = TimeSpan.FromSeconds(InformationCache.RestartTimer);

                    /* Proper Formatting */
                    List<string> list_of_times = new List<string>();
                    if (t.Days != 0) list_of_times.Add(t.Days + (t.Days != 1 ? " Days" : " Day"));
                    if (t.Hours != 0) list_of_times.Add(t.Hours + (t.Hours != 1 ? " Hours" : " Hour"));
                    if (t.Minutes != 0) list_of_times.Add(t.Minutes + (t.Minutes != 1 ? " Minutes" : " Minute"));
                    if (t.Seconds != 0) list_of_times.Add(t.Seconds + (t.Seconds != 1 ? " Seconds" : " Second"));

                    String secondsToShutDownNamed = String.Empty;
                    if (list_of_times.Count() >= 2)
                    {
                        secondsToShutDownNamed = list_of_times[0] + ", " + list_of_times[1];
                    }
                    else
                    {
                        secondsToShutDownNamed = list_of_times[0];
                    }

                    if (InformationCache.RestartTimer == 0)
                    {
                        secondsToShutDownNamed = "Waiting for event to finish.";
                    }

                    User32.SetWindowText((IntPtr)p, "NEED FOR SPEED™ WORLD | Server: " + InformationCache.SelectedServerData.Name + " | " + DiscordGamePresence.LauncherRPC + " | Force Restart In: " + secondsToShutDownNamed);
                }

                --InformationCache.RestartTimer;
            };

            shutdowntimer.Interval = 1000;
            shutdowntimer.Enabled = true;

            if (nfswProcess != null)
            {
                nfswProcess.EnableRaisingEvents = true;
                _nfswPid = nfswProcess.Id;

                nfswProcess.Exited += (sender2, e2) =>
                {
                    _nfswPid = 0;
                    var exitCode = nfswProcess.ExitCode;

                    if (FunctionStatus.GameKilledBySpeedBugCheck == true)
                    {
                        if (FunctionStatus.ExternalToolsWasUsed == true) exitCode = 2017;
                        else exitCode = 2137;
                    }

                    if (exitCode == 0)
                    {
                        if (AntiCheat.thread != null)
                        {
                            AntiCheat.thread.Abort();
                        }

                        CloseBTN_Click(null, null);
                    }
                    else
                    {
                        if (AntiCheat.thread != null)
                        {
                            AntiCheat.thread.Abort();
                        }

                        x.BeginInvoke(new Action(() =>
                        {
                            x.WindowState = FormWindowState.Normal;
                            x.ShowInTaskbar = true;
                            String errorMsg = "Game Crash with exitcode: " + exitCode.ToString() + " (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073741819) errorMsg = "Game Crash: Access Violation (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073740940) errorMsg = "Game Crash: Heap Corruption (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073740791) errorMsg = "Game Crash: Stack buffer overflow (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -805306369) errorMsg = "Game Crash: Application Hang (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073741515) errorMsg = "Game Crash: Missing dependency files (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073740972) errorMsg = "Game Crash: Debugger crash (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073741676) errorMsg = "Game Crash: Division by Zero (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == 1) errorMsg = "The process nfsw.exe was killed via Task Manager";
                            if (exitCode == 69) errorMsg = "AllocationAssistant encountered an 'Out of Memory' condition";
                            if (exitCode == 2137) errorMsg = "Launcher killed your game to prevent SpeedBugging.";
                            if (exitCode == 2017) errorMsg = "Server replied with Code: " + Tokens.UserId + " (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -3) errorMsg = "The Server was unable to resolve the request";
                            if (exitCode == -4) errorMsg = "Another instance is already executed";
                            if (exitCode == -5) errorMsg = "DirectX Device was not found. Please install GPU Drivers before playing";
                            if (exitCode == -6) errorMsg = "Server was unable to resolve your request";
                            /* ModLoader */
                            if (exitCode == 2) errorMsg = "ModNet: Game was launched with invalid command line parameters.";
                            if (exitCode == 3) errorMsg = "ModNet: .links file should not exist upon startup!";
                            if (exitCode == 4) errorMsg = "ModNet: An Unhandled Error Appeared";
                            PlayProgressText.Text = errorMsg.ToUpper();
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
                            DialogResult restartApp = MessageBox.Show(null, errorMsg + "\nWould you like to restart the launcher?", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (restartApp == DialogResult.Yes)
                            {
                                Application.Restart();
                                Application.ExitThread();
                            }
                            this.CloseBTN_Click(null, null);
                        }));
                    }
                };
            }
        }

        public void DownloadModNetFilesRightNow(string path)
        {
            while (isDownloadingModNetFiles == false)
            {
                CurrentModFileCount++;
                var url = modFilesDownloadUrls.Dequeue();
                string FileName = url.ToString().Substring(url.ToString().LastIndexOf("/") + 1, (url.ToString().Length - url.ToString().LastIndexOf("/") - 1));

                ModNetFileNameInUse = FileName;

                try
                {
                    FunctionStatus.TLS();
                    WebClient client2 = new WebClient();
                    client2.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");

                    client2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged_RELOADED);
                    client2.DownloadFileCompleted += (test, stuff) =>
                    {
                        Log.Core("LAUNCHER: Downloaded: " + FileName);
                        isDownloadingModNetFiles = false;
                        if (modFilesDownloadUrls.Any() == false)
                        {
                            LaunchGame();
                        }
                        else
                        {
                            /* Redownload other file */
                            DownloadModNetFilesRightNow(path);
                        }
                    };
                    client2.DownloadFileAsync(url, path + "/" + FileName);
                }
                catch
                {
                    CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", IsEmailValid.Mask(FileAccountSave.UserRawEmail)).ToUpper();
                }

                isDownloadingModNetFiles = true;
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (FileSettingsSave.GameIntegrity != "Good")
            {
                MessageBox.Show("Launcher had Detected Game Files Integrity Error\nPlease Verify Game Files in Settings Screen", 
                    "Game Files Integrity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DetectLinux.LinuxDetected())
            {
                var installDir = FileSettingsSave.GameInstallation;
                DriveInfo driveInfo = new DriveInfo(installDir);

                if (!string.Equals(driveInfo.DriveFormat, "NTFS", StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show(
                        $"Playing the game on a non-NTFS-formatted drive is not supported.\nDrive '{driveInfo.Name}' is formatted with: {driveInfo.DriveFormat}",
                        "Compatibility",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }

            if (File.Exists(FileSettingsSave.GameInstallation + "\\.links"))
            {
                File.Delete(FileSettingsSave.GameInstallation + "\\.links");
            }

            /* Disable Play Button and Logout Buttons */
            PlayButton.Visible = false;
            LogoutButton.Visible = false;
            _disableLogout = true;
            DisablePlayButton();

            ModNetHandler.ResetModDat(FileSettingsSave.GameInstallation);

            if (Directory.Exists(FileSettingsSave.GameInstallation + "/modules")) Directory.Delete(FileSettingsSave.GameInstallation + "/modules", true);
            if (!Directory.Exists(FileSettingsSave.GameInstallation + "/scripts")) Directory.CreateDirectory(FileSettingsSave.GameInstallation + "/scripts");

            Log.Core("LAUNCHER: Installing ModNet");
            PlayProgressText.Text = ("Detecting ModNet Support for " + InformationCache.SelectedServerData.Name).ToUpper();
            String jsonModNet = ModNetHandler.ModNetSupported(InformationCache.SelectedServerData.IpAddress);

            if (jsonModNet != String.Empty)
            {
                PlayProgressText.Text = "ModNet support detected, setting up...".ToUpper();

                try
                {
                    DiscordLauncherPresense.Status("Download ModNet", null);

                    /* Get Remote ModNet list to process for checking required ModNet files are present and current */
                    FunctionStatus.TLS();
                    String modules = new WebClient().DownloadString(URLs.ModNet + "/launcher-modules/modules.json");

                    try
                    {
                        string[] modules_newlines = modules.Split(new string[] { "\n" }, StringSplitOptions.None);
                        foreach (String modules_newline in modules_newlines)
                        {
                            if (modules_newline.Trim() == "{" || modules_newline.Trim() == "}") continue;

                            String trim_modules_newline = modules_newline.Trim();
                            String[] modules_files = trim_modules_newline.Split(new char[] { ':' });

                            String ModNetList = modules_files[0].Replace("\"", "").Trim();
                            String ModNetSHA = modules_files[1].Replace("\"", "").Replace(",", "").Trim();

                            if (SHATwoFiveSix.HashFile(FileSettingsSave.GameInstallation + "\\" + ModNetList).ToLower() != ModNetSHA || !File.Exists(FileSettingsSave.GameInstallation + "\\" + ModNetList))
                            {
                                PlayProgressText.Text = ("ModNet: Downloading " + ModNetList).ToUpper();

                                Log.Warning("MODNET CORE: " + ModNetList + " Does not match SHA Hash on File Server -> Online Hash: '" + ModNetSHA + "'");

                                if (File.Exists(FileSettingsSave.GameInstallation + "\\" + ModNetList))
                                {
                                    File.Delete(FileSettingsSave.GameInstallation + "\\" + ModNetList);
                                }

                                FunctionStatus.TLS();
                                WebClient newModNetFilesDownload = new WebClient();
                                newModNetFilesDownload.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                newModNetFilesDownload.DownloadFile(URLs.ModNet + "/launcher-modules/" + ModNetList, FileSettingsSave.GameInstallation + "/" + ModNetList);
                            }
                            else
                            {
                                PlayProgressText.Text = ("ModNet: Up to Date " + ModNetList).ToUpper();
                                Log.Debug("MODNET CORE: " + ModNetList + " Is Up to Date!");
                            }

                            Application.DoEvents();
                            PlayProgressText.Text = ("Fetching Server Mods List!").ToUpper();
                        }
                    }
                    catch (Exception error)
                    {
                        Log.Error("MODNET CORE: " + error.Message);
                        CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", IsEmailValid.Mask(FileAccountSave.UserRawEmail)).ToUpper();
                        MessageBox.Show(null, $"There was an error with ModNet Files Check:\n{error.Message}", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    /* get files now */
                    MainJson json2 = JsonConvert.DeserializeObject<MainJson>(jsonModNet);

                    /* Set and Get for RemoteRPC Files */
                    String remoteCarsFile = String.Empty;
                    String remoteEventsFile = String.Empty;
                    try
                    {
                        FunctionStatus.TLS();
                        remoteCarsFile = new WebClient().DownloadString(json2.basePath + "/cars.json");
                    }
                    catch { }

                    try
                    {
                        FunctionStatus.TLS();
                        remoteEventsFile = new WebClient().DownloadString(json2.basePath + "/events.json");
                    }
                    catch { }

                    /* Version 1.3 @metonator - DavidCarbon */
                    if (IsJSONValid.ValidJson(remoteCarsFile) == true)
                    {
                        Log.Info("DISCORD: Found RemoteRPC List for cars.json");
                        CarsList.remoteCarsList = remoteCarsFile;
                    }
                    else
                    {
                        Log.Warning("DISCORD: RemoteRPC List for cars.json does not exist");
                        CarsList.remoteCarsList = String.Empty;
                    }

                    if (IsJSONValid.ValidJson(remoteEventsFile) == true)
                    {
                        Log.Info("DISCORD: Found RemoteRPC List for events.json");
                        EventsList.remoteEventsList = remoteEventsFile;
                    }
                    else
                    {
                        Log.Warning("DISCORD: RemoteRPC List for events.json does not exist");
                        EventsList.remoteEventsList = String.Empty;
                    }

                    /* get new index */
                    Uri newIndexFile = new Uri(json2.basePath + "/index.json");
                    Log.Core("CORE: Loading Server Mods List");
                    FunctionStatus.TLS();
                    String jsonindex = new WebClient().DownloadString(newIndexFile);

                    try
                    {
                        IndexJson json3 = JsonConvert.DeserializeObject<IndexJson>(jsonindex);

                        int CountFilesTotal = 0;
                        CountFilesTotal = json3.entries.Count;

                        String path = Path.Combine(FileSettingsSave.GameInstallation, "MODS", MDFive.HashPassword(json2.serverID).ToLower());
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                        foreach (IndexJsonEntry modfile in json3.entries)
                        {
                            if (SHA.HashFile(path + "/" + modfile.Name).ToLower() != modfile.Checksum)
                            {
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
                            this.DownloadModNetFilesRightNow(path);
                            DiscordLauncherPresense.Status("Download Server Mods", null);
                        }
                        else
                        {
                            LaunchGame();
                        }

                        foreach (var file in Directory.GetFiles(path))
                        {
                            var name = Path.GetFileName(file);

                            if (json3.entries.All(en => en.Name != name))
                            {
                                Log.Core("LAUNCHER: removing package: " + file);
                                try
                                {
                                    File.Delete(file);
                                }
                                catch (Exception ex)
                                {
                                    Log.Error($"Failed to remove {file}: {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        Log.Error("LAUNCHER " + error.Message);
                        CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", IsEmailValid.Mask(FileAccountSave.UserRawEmail)).ToUpper();
                        MessageBox.Show(null, $"There was an error with Server Mods Check:\n{error.Message}", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }
                }
                catch (Exception error)
                {
                    Log.Error("LAUNCHER " + error.Message);
                    CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", IsEmailValid.Mask(FileAccountSave.UserRawEmail)).ToUpper();
                    MessageBox.Show(null, $"There was an error downloading ModNet Files:\n{error.Message}", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
            }
            else
            {
                LaunchGame();
            }
        }

        void Client_DownloadProgressChanged_RELOADED(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                PlayProgressTextTimer.Text = ("Downloading - [" + CurrentModFileCount + " / " + TotalModFileCount + "] :").ToUpper();
                PlayProgressText.Text = (" Server Mods: " + ModNetFileNameInUse + " - " + TimeConversions.FormatFileSize(e.BytesReceived) + " of " + TimeConversions.FormatFileSize(e.TotalBytesToReceive)).ToUpper();

                ExtractingProgress.Value = Convert.ToInt32(Decimal.Divide(e.BytesReceived, e.TotalBytesToReceive) * 100);
                ExtractingProgress.Width = Convert.ToInt32(Decimal.Divide(e.BytesReceived, e.TotalBytesToReceive) * 519);
            });
            PlayProgressTextTimer.Text = "";
        }

        /* Launch game */
        private void LaunchGame()
        {
            if (InformationCache.SelectedServerData.DiscordAppId != null)
            {
                DiscordLauncherPresense.Start("New RPC", InformationCache.SelectedServerData.DiscordAppId);
            }

            try
            {
                if
                  (
                    SHA.HashFile(FileSettingsSave.GameInstallation + "/nfsw.exe") == "7C0D6EE08EB1EDA67D5E5087DDA3762182CDE4AC" ||
                    SHA.HashFile(FileSettingsSave.GameInstallation + "/nfsw.exe") == "DB9287FB7B0CDA237A5C3885DD47A9FFDAEE1C19" ||
                    SHA.HashFile(FileSettingsSave.GameInstallation + "/nfsw.exe") == "E69890D31919DE1649D319956560269DB88B8F22"
                  )
                {
                    ServerProxy.Instance.SetServerUrl(InformationCache.SelectedServerData.IpAddress);
                    ServerProxy.Instance.SetServerName(InformationCache.SelectedServerData.Name);

                    AntiCheat.user_id = _userId;
                    AntiCheat.serverip = new Uri(InformationCache.SelectedServerData.IpAddress).Host;

                    StartGame(_userId, _loginToken);

                    if (_builtinserver)
                    {
                        PlayProgressText.Text = "Soapbox server launched. Waiting for queries.".ToUpper();
                    }
                    else
                    {
                        var secondsToCloseLauncher = 10;

                        ExtractingProgress.Value = 100;
                        ExtractingProgress.Width = 519;

                        while (secondsToCloseLauncher > 0)
                        {
                            PlayProgressTextTimer.Text = "";
                            PlayProgressText.Text = string.Format("Loading game. Launcher will minimize in {0} seconds.", secondsToCloseLauncher).ToUpper();
                            Time.SecondsRemaining(1);
                            secondsToCloseLauncher--;
                        }

                        if (secondsToCloseLauncher == 0)
                        {
                            CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", IsEmailValid.Mask(FileAccountSave.UserRawEmail)).ToUpper();
                        }

                        PlayProgressTextTimer.Text = "";
                        PlayProgressText.Text = "";

                        WindowState = FormWindowState.Minimized;
                        ShowInTaskbar = false;

                        ContextMenu = new ContextMenu();
                        ContextMenu.MenuItems.Add(new MenuItem("Donate", (b, n) => { Process.Start("https://paypal.me/metonator95"); }));
                        ContextMenu.MenuItems.Add("-");
                        ContextMenu.MenuItems.Add(new MenuItem("Close Launcher", (sender2, e2) =>
                        {
                            MessageBox.Show(null, "Please close the game before closing launcher.", "Please close the game before closing launcher.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));

                        Update();
                        Refresh();

                        Notification.ContextMenu = ContextMenu;
                    }
                }
                else
                {
                    MessageBox.Show(null, "Your NFSW.exe is modified. Please re-download the game.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            PlayButton.BackgroundImage = Theming.PlayButton;
            PlayButton.ForeColor = Theming.ThirdTextForeColor;

            PlayProgressText.Text = "Checking up all files".ToUpper();
            PlayProgress.Width = 0;
            ExtractingProgress.Width = 0;

            if (!File.Exists(FileSettingsSave.GameInstallation + "/Sound/Speech/copspeechhdr_" + FileSettingsSave.Lang.ToLower() + ".big"))
            {
                PlayProgressText.Text = "Loading list of files to download...".ToUpper();

                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    if (d.Name == Path.GetPathRoot(FileSettingsSave.GameInstallation))
                    {
                        if (d.TotalFreeSpace < 8589934592)
                        {
                            ExtractingProgress.Value = 100;
                            ExtractingProgress.Width = 519;
                            ExtractingProgress.Image = Theming.ProgressBarWarning;
                            ExtractingProgress.ProgressColor = Theming.ExtractingProgressColor;

                            PlayProgressText.Text = "Please make sure you have at least 8GB free space on hard drive.".ToUpper();

                            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Paused);
                            TaskbarProgress.SetValue(Handle, 100, 100);
                        }
                        else
                        {
                            DownloadCoreFiles();
                        }
                    }
                }
            }
            else
            {
                OnDownloadFinished();
            }
        }

        public void RemoveTracksHighFiles()
        {
            if (File.Exists(FileSettingsSave.GameInstallation + "/TracksHigh/STREAML5RA_98.BUN"))
            {
                Directory.Delete(FileSettingsSave.GameInstallation + "/TracksHigh", true);
            }
        }

        public void DownloadCoreFiles()
        {
            PlayProgressText.Text = "Checking Core Files...".ToUpper();
            PlayProgress.Width = 0;
            ExtractingProgress.Width = 0;

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            /* Use Local Packed Archive for Install Source - DavidCarbon */
            if (File.Exists(filename_pack) && !File.Exists(FileSettingsSave.GameInstallation + "/nfsw.exe"))
            {
                PlayProgressTextTimer.Visible = true;
                PlayProgressText.Text = "Local GameFiles sbrwpack Found In Launcher Folder".ToUpper();
                PlayProgressTextTimer.Text = "Loading".ToUpper();

                /* GameFiles.sbrwpack */
                LocalGameFiles();
            }
            else if (!File.Exists(FileSettingsSave.GameInstallation + "/nfsw.exe"))
            {
                _downloadStartTime = DateTime.Now;
                PlayProgressTextTimer.Text = "Downloading: Core GameFiles".ToUpper();
                Log.Info("DOWNLOAD: Getting Core Game Files");
                _downloader.StartDownload(FileSettingsSave.CDN, "", FileSettingsSave.GameInstallation, false, false, 1130632198);
            }
            else
            {
                DownloadTracksFiles();
            }
        }

        public void DownloadTracksFiles()
        {
            PlayProgressText.Text = "Checking Tracks Files...".ToUpper();
            PlayProgress.Width = 0;
            ExtractingProgress.Width = 0;

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            if (!File.Exists(FileSettingsSave.GameInstallation + "/Tracks/STREAML5RA_98.BUN"))
            {
                _downloadStartTime = DateTime.Now;
                PlayProgressTextTimer.Text = "Downloading: Tracks Data".ToUpper();
                Log.Info("DOWNLOAD: Getting Tracks Folder");
                _downloader.StartDownload(FileSettingsSave.CDN, "Tracks", FileSettingsSave.GameInstallation, false, false, 615494528);
            }
            else
            {
                DownloadSpeechFiles();
            }
        }

        public void DownloadSpeechFiles()
        {
            PlayProgressText.Text = "Looking for correct Speech Files...".ToUpper();
            PlayProgress.Width = 0;
            ExtractingProgress.Width = 0;

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            string speechFile;
            int speechSize;

            if (FileSettingsSave.Lang.ToLower() == FunctionStatus.SpeechFiles())
            {
                speechFile = FileSettingsSave.Lang.ToLower();
            }
            else
            {
                speechFile = FunctionStatus.SpeechFiles();
            }

            try
            {
                WebClientWithTimeout wc = new WebClientWithTimeout();
                var response = wc.DownloadString(FileSettingsSave.CDN + "/" + speechFile + "/index.xml");

                response = response.Substring(3, response.Length - 3);

                var speechFileXml = new XmlDocument();
                speechFileXml.LoadXml(response);
                var speechSizeNode = speechFileXml.SelectSingleNode("index/header/compressed");

                speechSize = Convert.ToInt32(speechSizeNode.InnerText);
                /* Fix this issue - DavidCarbon */
                //_langInfo = SettingsLanguage.GetItemText(SettingsLanguage.SelectedItem).ToUpper();
            }
            catch (Exception)
            {
                speechFile = FunctionStatus.SpeechFiles();
                speechSize = FunctionStatus.SpeechFilesSize();
                _langInfo = FunctionStatus.SpeechFiles();
            }

            PlayProgressText.Text = string.Format("Checking for {0} Speech Files.", _langInfo).ToUpper();

            if (!File.Exists(FileSettingsSave.GameInstallation + "\\Sound\\Speech\\copspeechsth_" + speechFile + ".big"))
            {
                _downloadStartTime = DateTime.Now;
                PlayProgressTextTimer.Text = "Downloading: Language Audio".ToUpper();
                Log.Info("DOWNLOAD: Getting Speech/Audio Files");
                _downloader.StartDownload(FileSettingsSave.CDN, speechFile, FileSettingsSave.GameInstallation, false, false, speechSize);
            }
            else
            {
                OnDownloadFinished();
                PlayProgressTextTimer.Text = "";
                Log.Info("DOWNLOAD: Game Files Download is Complete!");
            }
        }

        /* Check Local GameFiles Hash */
        private async void LocalGameFiles()
        {
            await Task.Delay(5000);
            if (SHA.HashFile("GameFiles.sbrwpack") == "88C886B6D131C052365C3D6D14E14F67A4E2C253")
            {
                TaskbarProgress.SetValue(Handle, 100, 100);
                PlayProgress.Value = 100;
                PlayProgress.Width = 519;

                GoForUnpack(filename_pack);
            }
        }

        /* That's right the Protype Extractor from 2.1.5.x, now back from the dead - DavidCarbon */
        public void GoForUnpack(string filename_pack)
        {
            Thread unpacker = new Thread(() => {
                this.BeginInvoke((MethodInvoker)delegate {
                    using (ZipArchive archive = ZipFile.OpenRead(filename_pack))
                    {
                        int numFiles = archive.Entries.Count;
                        int current = 1;

                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string fullName = entry.FullName;

                            ExtractingProgress.Value = (int)((long)100 * current / numFiles);
                            ExtractingProgress.Width = (int)((long)519 * current / numFiles);

                            TaskbarProgress.SetValue(Handle, (int)(100 * current / numFiles), 100);

                            if (!File.Exists(Path.Combine(FileSettingsSave.GameInstallation, fullName.Replace(".sbrw", String.Empty))))
                            {
                                PlayProgressText.Text = ("Unpacking " + fullName.Replace(".sbrw", String.Empty)).ToUpper();
                                PlayProgressTextTimer.Text = "[" + current + " / " + archive.Entries.Count + "]";


                                if (fullName.Substring(fullName.Length - 1) == "/")
                                {
                                    /* Is a directory, create it! */
                                    string folderName = fullName.Remove(fullName.Length - 1);
                                    if (Directory.Exists(Path.Combine(FileSettingsSave.GameInstallation, folderName)))
                                    {
                                        Directory.Delete(Path.Combine(FileSettingsSave.GameInstallation, folderName), true);
                                    }

                                    Directory.CreateDirectory(Path.Combine(FileSettingsSave.GameInstallation, folderName));
                                }
                                else
                                {
                                    String oldFileName = fullName.Replace(".sbrw", String.Empty);
                                    String[] split = oldFileName.Split('/');

                                    String newFileName = String.Empty;

                                    if (split.Length >= 2)
                                    {
                                        newFileName = Path.Combine(split[split.Length - 2], split[split.Length - 1]);
                                    }
                                    else
                                    {
                                        newFileName = split.Last();
                                    }

                                    String KEY = Regex.Replace(SHA.HashPassword(newFileName), "[^0-9.]", "").Substring(0, 8);
                                    String IV = Regex.Replace(MDFive.HashPassword(newFileName), "[^0-9.]", "").Substring(0, 8);

                                    entry.ExtractToFile(getTempNa, true);

                                    DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider()
                                    {
                                        Key = Encoding.ASCII.GetBytes(KEY),
                                        IV = Encoding.ASCII.GetBytes(IV)
                                    };

                                    FileStream fileStream = new FileStream(Path.Combine(FileSettingsSave.GameInstallation, oldFileName), FileMode.Create);
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
                                PlayProgressText.Text = ("Skipping " + fullName).ToUpper();
                            }

                            string Status = string.Format("Unpacking game: " + (100 * current / numFiles) + "%");
                            DiscordLauncherPresense.Status("Unpack Game Files", Status);

                            Application.DoEvents();

                            if (numFiles == current)
                            {
                                PlayProgressTextTimer.Visible = false;
                                PlayProgressTextTimer.Text = "";

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
            });

            unpacker.Start();
        }

        private void OnDownloadProgress(long downloadLength, long downloadCurrent, long compressedLength, string filename, int skiptime = 0)
        {
            if (downloadCurrent < compressedLength)
            {
                PlayProgressText.Text = String.Format("{0} of {1} ({3}%) — {2}", TimeConversions.FormatFileSize(downloadCurrent), TimeConversions.FormatFileSize(compressedLength), TimeConversions.EstimateFinishTime(downloadCurrent, compressedLength, _downloadStartTime), (int)(100 * downloadCurrent / compressedLength)).ToUpper();
            }

            try
            {
                PlayProgress.Value = (int)(100 * downloadCurrent / compressedLength);
                PlayProgress.Width = (int)(519 * downloadCurrent / compressedLength);

                string Status = string.Format("Downloaded {0}% of the Game!", (int)(100 * downloadCurrent / compressedLength));
                DiscordLauncherPresense.Status("Download Game Files", Status);

                TaskbarProgress.SetValue(Handle, (int)(100 * downloadCurrent / compressedLength), 100);
            }
            catch
            {
                TaskbarProgress.SetValue(Handle, 0, 100);
                PlayProgress.Value = 0;
                PlayProgress.Width = 0;
            }

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);
        }

        private void OnDownloadFinished()
        {
            try
            {
                File.WriteAllBytes(FileSettingsSave.GameInstallation + "/GFX/BootFlow.gfx", ExtractResource.AsByte("GameLauncher.Resources.Bootscreen.BootFlow.gfx"));
            }
            catch { }

            if (FileSettingsSave.GameIntegrity == "Unknown")
            {
                FileSettingsSave.GameIntegrity = "Good";
                FileSettingsSave.SaveSettings();
            }

            /* Windows Firewall Runner */
            if (!string.IsNullOrEmpty(FileSettingsSave.FirewallGameStatus))
            {
                FirewallFunctions.GameFiles();
                FileORFolderPermissionsFunctions.Folders();
            }

            PlayProgressText.Text = "Ready!".ToUpper();
            DiscordLauncherPresense.Status("Idle Ready", null);

            EnablePlayButton();

            ExtractingProgress.Width = 519;

            TaskbarProgress.SetValue(Handle, 100, 100);
            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);
        }

        private void EnablePlayButton()
        {
            _isDownloading = false;
            _playenabled = true;

            ExtractingProgress.Value = 100;
            ExtractingProgress.Width = 519;
        }

        private void DisablePlayButton()
        {
            _isDownloading = false;
            _playenabled = false;

            ExtractingProgress.Value = 100;
            ExtractingProgress.Width = 519;
        }

        private void OnDownloadFailed(Exception error)
        {
            Log.Error("CDN DOWNLOADER: " + error.Message);

            string failureMessage;
            try
            {

                failureMessage = error.Message;
            }
            catch
            {
                failureMessage = "Download failed.";
            }

            DiscordLauncherPresense.Status("Download Game Files Error", null);

            ExtractingProgress.Value = 100;
            ExtractingProgress.Width = 519;
            ExtractingProgress.Image = Theming.ProgressBarError;
            ExtractingProgress.ProgressColor = Theming.Error;

            PlayProgressText.Text = failureMessage.ToUpper();

            TaskbarProgress.SetValue(Handle, 100, 100);
            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Error);

            MessageBox.Show(null, "Failed to download gamefiles. \n\nCDN might be offline. \n\nPlease select a different CDN on Next Screen", "GameLauncher - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            /* CDN Went Offline Screen switch - DavidCarbon */
            SettingsButton_Click(null, null);
        }

        private void OnShowExtract(string filename, long currentCount, long allFilesCount)
        {
            if (PlayProgress.Value == 100)
            {
                PlayProgressText.Text = String.Format("{0} of {1} : ({3}%) — {2}", TimeConversions.FormatFileSize(currentCount), TimeConversions.FormatFileSize(allFilesCount), TimeConversions.EstimateFinishTime(currentCount, allFilesCount, _downloadStartTime), (int)(100 * currentCount / allFilesCount)).ToUpper();
            }

            ExtractingProgress.Value = (int)(100 * currentCount / allFilesCount);
            ExtractingProgress.Width = (int)(519 * currentCount / allFilesCount);
        }

        private void OnShowMessage(string message, string header)
        {
            MessageBox.Show(message, header);
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
            SettingsButton.BackgroundImage = Theming.GearButton;
            CloseBTN.BackgroundImage = Theming.CloseButton;

            ProgressBarOutline.BackgroundImage = Theming.ProgressBarOutline;
            PlayProgress.Image = Theming.ProgressBarPreload;
            ExtractingProgress.Image = Theming.ProgressBarSuccess;

            PlayProgressText.ForeColor = Theming.FivithTextForeColor;
            PlayProgressTextTimer.ForeColor = Theming.FivithTextForeColor;

            MainEmailBorder.Image = Theming.BorderEmail;
            MainPasswordBorder.Image = Theming.BorderPassword;

            CurrentWindowInfo.ForeColor = Theming.FivithTextForeColor;

            LauncherStatusDesc.ForeColor = Theming.FivithTextForeColor;
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

            CloseBTN.MouseEnter += new EventHandler(CloseBTN_MouseEnter);
            CloseBTN.MouseLeave += new EventHandler(CloseBTN_MouseLeave);
            CloseBTN.Click += new EventHandler(CloseBTN_Click);

            SettingsButton.MouseEnter += new EventHandler(SettingsButton_MouseEnter);
            SettingsButton.MouseLeave += new EventHandler(SettingsButton_MouseLeave);
            SettingsButton.Click += new EventHandler(SettingsButton_Click);

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
            ServerPick.DrawItem += new DrawItemEventHandler(FunctionEvents.ComboBox1_DrawItem);

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
            /* Functions                     /
            /********************************/

            /* Run the API Checks to Make Sure it Visually Displayed Correctly */
            if (FunctionStatus.IsVisualAPIsChecked != true)
            {
                VisualsAPIChecker.PingAPIStatus();
            }

            Log.Debug("PROXY: Checking if Proxy Is Disabled from User Settings! It's value is " + FunctionStatus.DisableProxy);

            Shown += (x, y) =>
            {
                new Thread(() =>
                {
                    DiscordLauncherPresense.Update();

                    /* Let's fetch all servers */
                    List<ServerList> allServs = ServerListUpdater.CleanList.FindAll(i => string.Equals(i.IsSpecial, false));
                    allServs.ForEach(delegate (ServerList server) {
                        try
                        {
                            WebClientWithTimeout pingServer = new WebClientWithTimeout();
                            pingServer.DownloadString(server.IpAddress + "/GetServerInformation");

                            if (!InformationCache.ServerStatusBook.ContainsKey(server.Id))
                            {
                                InformationCache.ServerStatusBook.Add(server.Id, 1);
                            }
                        }
                        catch
                        {
                            if (!InformationCache.ServerStatusBook.ContainsKey(server.Id))
                            {
                                InformationCache.ServerStatusBook.Add(server.Id, 0);
                            }
                        }
                    });
                }).Start();
            };

            if (!DetectLinux.LinuxDetected())
            {
                try
                {
                    string CursorFile = Path.GetTempFileName();
                    File.WriteAllBytes(CursorFile, ExtractResource.AsByte("GameLauncher.Resources.Cursors.Cursor.ani"));
                    Cursor mycursor = new Cursor(Cursor.Current.Handle);
                    IntPtr colorcursorhandle = User32.LoadCursorFromFile(CursorFile);
                    mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, null, mycursor, new object[] { colorcursorhandle });
                    Cursor = mycursor;
                    File.Delete(CursorFile);
                }
                catch (Exception error)
                {
                    Log.Error("LAUNCHER: " + error.Message);
                }
            }
        }
    }
    /* Moved 7 Unused Code to Gist */
    /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a8F89a5e5aebce38/raw/00de505302fbf9f8cfea9b163a707d9f8f122552/MainScreen.cs */
}
