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
using GameLauncher.Resources;
using GameLauncher.App.Classes;
using Newtonsoft.Json;
using SoapBox.JsonScheme;
using GameLauncher.App.Classes.Events;
using GameLauncherReborn;
using Microsoft.Win32;
using GameLauncher.App;
using GameLauncher.HashPassword;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Security;
using GameLauncher.App.Classes.Logger;
using System.IO.Compression;
using GameLauncher.App.Classes.Auth;
using DiscordRPC;
using DiscordRPC.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Management;
using GameLauncher.App.Classes.ModNetReloaded;
using GameLauncher.App.Classes.HashPassword;
using System.Security;
using GameLauncher.App.Classes.RPC;
using GameLauncher.App.Classes.GPU;
using CommandLine;
using System.Runtime.CompilerServices;
using GameLauncher.Properties;
//using System.Windows;

namespace GameLauncher {
    public sealed partial class MainScreen : Form {
        private Point _mouseDownPoint = Point.Empty;
        private bool _loginEnabled;
        private bool _serverEnabled;
        private bool _builtinserver;
        private bool _useSavedPassword;
        private bool _skipServerTrigger;
        private bool _ticketRequired;
        private bool _windowMoved;
        private bool _playenabled;
        private bool _loggedIn;
        private bool _restartRequired;
        private bool _allowRegistration;
        private bool _isDownloading = true;
        private bool _modernAuthSupport = false;
        private bool _gameKilledBySpeedBugCheck = false;
        private bool _disableLogout = false;

        public static String getTempNa = Path.GetTempFileName();

        //private bool _disableChecks;
        private bool _disableProxy;
        private bool _disableDiscordRPC;

        private int _lastSelectedServerId;
        private int _nfswPid;
        private Thread _nfswstarted;


        private DateTime _downloadStartTime;
        private readonly Downloader _downloader;

        private string _loginWelcomeTime = "";
        private string _loginToken = "";
        private string _userId = "";
        private string _serverIp = "";
        private string _langInfo;
        private string _newGameFilesPath;
        private readonly float _dpiDefaultScale = 96f;

        private readonly RichPresence _presence = new RichPresence();

        private readonly Pen _colorOffline = new Pen(Color.FromArgb(128, 0, 0));
        private readonly Pen _colorOnline = new Pen(Color.FromArgb(0, 128, 0));
        private readonly Pen _colorLoading = new Pen(Color.FromArgb(0, 0, 0));
        private readonly Pen _colorIssues = new Pen(Color.FromArgb(255, 145, 0));

        private readonly IniFile _settingFile = new IniFile("Settings.ini");
        private readonly string _userSettings = Environment.GetEnvironmentVariable("AppData") + "/Need for Speed World/Settings/UserSettings.xml";
        private string _presenceImageKey;
        private string _NFSW_Installation_Source;
        private string _realServername;
        private string _realServernameBanner;
        private string _OS;

        public static String ModNetFileNameInUse = String.Empty;
        Queue<Uri> modFilesDownloadUrls = new Queue<Uri>();
        bool isDownloadingModNetFiles = false;
        int CurrentModFileCount = 0;
        int TotalModFileCount = 0;

        int CountFiles = 0;
        int CountFilesTotal = 0;

        private Point _startPoint = new Point(38, 144);
        private Point _endPoint = new Point(562, 144);

        ServerInfo _serverInfo = null;
        GetServerInformation json = new GetServerInformation();
        String purejson = String.Empty;

        public static DiscordRpcClient discordRpcClient;
        private Random rnd;

        List<ServerInfo> finalItems = new List<ServerInfo>();
        Dictionary<string, int> serverStatusDictionary = new Dictionary<string, int>();

        String filename_pack = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameFiles.sbrwpack");

        //UltimateLauncherFunction: SelectServer
        private static ServerInfo _ServerList;
        public static ServerInfo ServerName {
            get { return _ServerList; }
            set { _ServerList = value; }
        }

        private static Random random = new Random();

        public static string RandomString(int length) {
			const string chars = "qwertyuiopasdfghjklzxcvbnm1234567890_";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

        private void moveWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Y <= 90) _mouseDownPoint = new Point(e.X, e.Y);
        }

        private void moveWindow_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDownPoint = Point.Empty;
            Opacity = 1;
        }

        private void moveWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDownPoint.IsEmpty) { return; }
            var f = this as Form;
            f.Location = new Point(f.Location.X + (e.X - _mouseDownPoint.X), f.Location.Y + (e.Y - _mouseDownPoint.Y));
            _windowMoved = true;
            Opacity = 0.9;
        }

        public MainScreen() {

            ParseUri uri = new ParseUri(Environment.GetCommandLineArgs());

            if (uri.IsDiscordPresent()) {
                Notification.Visible = true;
                Notification.BalloonTipIcon = ToolTipIcon.Info;
                Notification.BalloonTipTitle = "GameLauncherReborn";
                Notification.BalloonTipText = "Discord features are not yet completed.";
                Notification.ShowBalloonTip(5000);
                Notification.Dispose();
            }

            Log.Debug("CORE: Entered mainScreen");

            rnd = new Random(Environment.TickCount);

            discordRpcClient = new DiscordRpcClient(Self.DiscordRPCID);

            discordRpcClient.OnReady += (sender, e) => {
                Log.Debug("DISCORD: Discord ready. Detected user: " + e.User.Username + ". Discord version: " + e.Version);
                Self.discordid = e.User.ID.ToString();
            };

            discordRpcClient.OnError += (sender, e) => {
                Log.Error($"DISCORD: Discord Error\n{e.Message}");
            };

            discordRpcClient.Initialize();

            
            Log.Debug("CORE: Setting SSL Protocol");
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            if (DetectLinux.LinuxDetected()) {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }

            Log.Debug("LAUNCHER: Detecting OS");
            if (DetectLinux.LinuxDetected()) {
                _OS = DetectLinux.Distro();
                Log.Debug("SYSTEM: Detected OS: " + _OS);
            } else {
                _OS = (string)Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("productName");
                Log.Debug("SYSTEM: Detected OS: " + _OS);
                if (Environment.Is64BitOperatingSystem == true) {
                    Log.Debug("SYSTEM: OS Type: 64 Bit");
                }
                Log.Debug("SYSTEM: OS Details: " + Environment.OSVersion);
                Log.Debug("SYSTEM: Video Card: " + GPUHelper.CardName());
                Log.Debug("SYSTEM: Driver Version: " + GPUHelper.DriverVersion());
            }

            _downloader = new Downloader(this, 3, 2, 16) {
                ProgressUpdated = new ProgressUpdated(OnDownloadProgress),
                DownloadFinished = new DownloadFinished(DownloadTracksFiles),
                DownloadFailed = new DownloadFailed(OnDownloadFailed),
                ShowMessage = new ShowMessage(OnShowMessage),
				ShowExtract = new ShowExtract(OnShowExtract)
            };

            Log.Debug("CORE: InitializeComponent");
            InitializeComponent();

            Log.Debug("CORE: Applying Fonts");
            ApplyEmbeddedFonts();

            _disableProxy = (_settingFile.KeyExists("DisableProxy") && _settingFile.Read("DisableProxy") == "1") ? true : false;
            _disableDiscordRPC = (_settingFile.KeyExists("DisableRPC") && _settingFile.Read("DisableRPC") == "1") ? true : false;
            Log.Debug("PROXY: Checking if Proxy Is Disabled from User Settings! It's value is " + _disableProxy);

            Self.centerScreen(this);

            Log.Debug("CORE: Disabling MaximizeBox");
            MaximizeBox = false;
            Log.Debug("CORE: Setting Styles");
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            Log.Debug("CORE: Applying EventHandlers");
            closebtn.MouseEnter += new EventHandler(closebtn_MouseEnter);
            closebtn.MouseLeave += new EventHandler(closebtn_MouseLeave);
            closebtn.Click += new EventHandler(closebtn_Click);

            settingsButton.MouseEnter += new EventHandler(settingsButton_MouseEnter);
            settingsButton.MouseLeave += new EventHandler(settingsButton_MouseLeave);
            settingsButton.Click += new EventHandler(settingsButton_Click);

            loginButton.MouseEnter += new EventHandler(loginButton_MouseEnter);
            loginButton.MouseLeave += new EventHandler(loginButton_MouseLeave);
            loginButton.MouseUp += new MouseEventHandler(loginButton_MouseUp);
            loginButton.MouseDown += new MouseEventHandler(loginButton_MouseDown);
            loginButton.Click += new EventHandler(loginButton_Click);

            registerButton.MouseEnter += greenbutton_hover_MouseEnter;
            registerButton.MouseLeave += greenbutton_MouseLeave;
            registerButton.MouseUp += greenbutton_hover_MouseUp;
            registerButton.MouseDown += greenbutton_click_MouseDown;
            registerButton.Click += registerButton_Click;

            registerCancel.MouseEnter += new EventHandler(graybutton_hover_MouseEnter);
            registerCancel.MouseLeave += new EventHandler(graybutton_MouseLeave);
            registerCancel.MouseUp += new MouseEventHandler(graybutton_hover_MouseUp);
            registerCancel.MouseDown += new MouseEventHandler(graybutton_click_MouseDown);
            registerCancel.Click += new EventHandler(registerCancel_Click);

            logoutButton.MouseEnter += new EventHandler(graybutton_hover_MouseEnter);
            logoutButton.MouseLeave += new EventHandler(graybutton_MouseLeave);
            logoutButton.MouseUp += new MouseEventHandler(graybutton_hover_MouseUp);
            logoutButton.MouseDown += new MouseEventHandler(graybutton_click_MouseDown);
            logoutButton.Click += new EventHandler(logoutButton_Click);

            settingsSave.MouseEnter += new EventHandler(greenbutton_hover_MouseEnter);
            settingsSave.MouseLeave += new EventHandler(greenbutton_MouseLeave);
            settingsSave.MouseUp += new MouseEventHandler(greenbutton_hover_MouseUp);
            settingsSave.MouseDown += new MouseEventHandler(greenbutton_click_MouseDown);
            settingsSave.Click += new EventHandler(settingsSave_Click);

            settingsCancel.MouseEnter += new EventHandler(graybutton_hover_MouseEnter);
            settingsCancel.MouseLeave += new EventHandler(graybutton_MouseLeave);
            settingsCancel.MouseUp += new MouseEventHandler(graybutton_hover_MouseUp);
            settingsCancel.MouseDown += new MouseEventHandler(graybutton_click_MouseDown);
            settingsCancel.Click += new EventHandler(settingsCancel_Click);

            settingsGameFiles.Click += new EventHandler(settingsGameFiles_Click);
            settingsGameFilesCurrent.Click += new EventHandler(settingsGameFilesCurrent_Click);

            addServer.Click += new EventHandler(addServer_Click);
            launcherStatusDesc.Click += new EventHandler(OpenDebugWindow);

            email.KeyUp += new KeyEventHandler(Loginbuttonenabler);
            email.KeyDown += new KeyEventHandler(LoginEnter);
            password.KeyUp += new KeyEventHandler(Loginbuttonenabler);
            password.KeyDown += new KeyEventHandler(LoginEnter);

            serverPick.SelectedIndexChanged += new EventHandler(serverPick_SelectedIndexChanged);
            serverPick.DrawItem += new DrawItemEventHandler(comboBox1_DrawItem);

            forgotPassword.LinkClicked += new LinkLabelLinkClickedEventHandler(forgotPassword_LinkClicked);

            MouseMove += new MouseEventHandler(moveWindow_MouseMove);
            MouseUp += new MouseEventHandler(moveWindow_MouseUp);
            MouseDown += new MouseEventHandler(moveWindow_MouseDown);

            logo.MouseEnter += new EventHandler(logo_MouseEnter);
            logo.MouseLeave += new EventHandler(logo_MouseLeave);
            logo.MouseMove += new MouseEventHandler(moveWindow_MouseMove);
            logo.MouseUp += new MouseEventHandler(moveWindow_MouseUp);
            logo.MouseDown += new MouseEventHandler(moveWindow_MouseDown);

            playButton.MouseEnter += new EventHandler(playButton_MouseEnter);
            playButton.MouseLeave += new EventHandler(playButton_MouseLeave);
            playButton.MouseUp += new MouseEventHandler(playButton_MouseUp);
            playButton.MouseDown += new MouseEventHandler(playButton_MouseDown);
            playButton.Click += new EventHandler(playButton_Click);

            registerText.MouseEnter += new EventHandler(greenbutton_hover_MouseEnter);
            registerText.MouseLeave += new EventHandler(greenbutton_MouseLeave);
            registerText.MouseUp += new MouseEventHandler(greenbutton_hover_MouseUp);
            registerText.MouseDown += new MouseEventHandler(greenbutton_click_MouseDown);
            registerText.Click += new EventHandler(registerText_LinkClicked);

            this.Load += new EventHandler(mainScreen_Load);

            this.Shown += (x,y) => {
                if(UriScheme.ForceGame == true) {
                    playButton_Click(x, y);
                }

                new Thread(() => {
                    discordRpcClient.Invoke();

                    //Let's fetch all servers
                    List<ServerInfo> allServs = finalItems.FindAll(i => string.Equals(i.IsSpecial, false));
                    allServs.ForEach(delegate(ServerInfo server) {
                        try { 
                            WebClientWithTimeout pingServer = new WebClientWithTimeout();
                            pingServer.DownloadString(server.IpAddress + "/GetServerInformation");

                            if(!serverStatusDictionary.ContainsKey(server.Id))
                                serverStatusDictionary.Add(server.Id, 1);
                        } catch {
                            if (!serverStatusDictionary.ContainsKey(server.Id))
                                serverStatusDictionary.Add(server.Id, 0);
                        }
                    });
                }).Start();
            };


            Log.Debug("CORE: Checking permissions");
            if (!Self.hasWriteAccessToFolder(Directory.GetCurrentDirectory())) {
                Log.Error("CORE: Check Permission Failed.");
                MessageBox.Show(null, "Failed to write the test file. Make sure you're running the launcher with administrative privileges.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Log.Debug("LAUNCHER: Checking InstallationDirectory: " + _settingFile.Read("InstallationDirectory"));
            if (string.IsNullOrEmpty(_settingFile.Read("InstallationDirectory"))) {
                Log.Debug("LAUNCHER: First run!");

                try { 
                    Form welcome = new WelcomeScreen();
                    DialogResult welcomereply = welcome.ShowDialog();

                    if(welcomereply != DialogResult.OK) {
                        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                    } else {
                        _settingFile.Write("CDN", CDN.CDNUrl);
                        _settingFile.Write("TracksHigh", CDN.TrackHigh);

                        _NFSW_Installation_Source = CDN.CDNUrl;
                    }
                } catch {
                    _settingFile.Write("CDN", "http://cdn.worldunited.gg/gamefiles/packed/");
                    _settingFile.Write("TracksHigh", "1");
                    _NFSW_Installation_Source = "http://cdn.worldunited.gg/gamefiles/packed/";
                }

                var fbd = new CommonOpenFileDialog {
                    EnsurePathExists = true,
                    EnsureFileExists = false,
                    AllowNonFileSystemItems = false,
                    Title = "Select The Folder With NFS World Instalation",
                    IsFolderPicker = true
                };

                if (fbd.ShowDialog() == CommonFileDialogResult.Ok) {
                    if (!Self.hasWriteAccessToFolder(fbd.FileName)) {
                        Log.Error("LAUNCHER: Not enough permissions. Exiting.");
                        MessageBox.Show(null, "You don't have enough permission to select this path as installation folder. Please select another directory.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Environment.Exit(Environment.ExitCode);
                    }

                    if (fbd.DefaultFileName == Environment.CurrentDirectory) {
                        Directory.CreateDirectory("GameFiles");
                        Log.Debug("LAUNCHER: Installing NFSW in same directory where the launcher resides is disadvised.");
                        MessageBox.Show(null, string.Format("Installing NFSW in same directory where the launcher resides is disadvised. Instead, we will install it on {0}.", Environment.CurrentDirectory + "\\GameFiles"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _settingFile.Write("InstallationDirectory", Environment.CurrentDirectory + "\\GameFiles");
                    } else {
                        Log.Debug("LAUNCHER: Directory Set: " + fbd.FileName);
                        _settingFile.Write("InstallationDirectory", fbd.FileName);
                    }
                } else {
                    Log.Debug("LAUNCHER: Exiting");
                    Environment.Exit(Environment.ExitCode);
                }
                fbd.Dispose();
            }

            if (!DetectLinux.LinuxDetected()) {
                Log.Debug("CORE: Setting cursor.");
                string temporaryFile = Path.GetTempFileName();
                File.WriteAllBytes(temporaryFile, ExtractResource.AsByte("GameLauncher.SoapBoxModules.cursor.ani"));
                Cursor mycursor = new Cursor(Cursor.Current.Handle);
                IntPtr colorcursorhandle = User32.LoadCursorFromFile(temporaryFile);
                mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, null, mycursor, new object[] { colorcursorhandle });
                Cursor = mycursor;
                File.Delete(temporaryFile);
            }

            Log.Debug("CORE: Doing magic with imageServerName");
            var pos = PointToScreen(imageServerName.Location);
            pos = verticalBanner.PointToClient(pos);
            imageServerName.Parent = verticalBanner;
            imageServerName.Location = pos;
            imageServerName.BackColor = Color.Transparent;

            Log.Debug("CORE: Setting ServerStatusBar");
            ServerStatusBar(_colorLoading, _startPoint, _endPoint);

            Log.Debug("CORE: Loading ModManager Cache");
            ModManager.LoadModCache();
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e) {
            var font = (sender as ComboBox).Font;
            Brush backgroundColor;
            Brush textColor;

            var serverListText = "";
            int onlineStatus = 2; //0 = offline | 1 = online | 2 = checking

            if (sender is ComboBox cb) {
                if (cb.Items[e.Index] is ServerInfo si) {
                    serverListText = si.Name;
                    onlineStatus = serverStatusDictionary.ContainsKey(si.Id) ? serverStatusDictionary[si.Id] : 2;
                }
            }

            if (serverListText.StartsWith("<GROUP>")) {
                font = new Font(font, FontStyle.Bold);
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                e.Graphics.DrawString(serverListText.Replace("<GROUP>", string.Empty), font, Brushes.Black, e.Bounds);
            } else {
                font = new Font(font, FontStyle.Regular);
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && e.State != DrawItemState.ComboBoxEdit) {
                    backgroundColor = SystemBrushes.Highlight;
                    textColor = SystemBrushes.HighlightText;
                } else {
                    if(onlineStatus == 2) {
                        //CHECKING
                        backgroundColor = Brushes.Khaki;
                    } else if(onlineStatus == 1) {
                        //ONLINE
                        backgroundColor = Brushes.PaleGreen;
                    } else {
                        //OFFLINE
                        backgroundColor = Brushes.LightCoral;
                    }

                    textColor = SystemBrushes.WindowText;
                }

                e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                e.Graphics.DrawString("    " + serverListText, font, textColor, e.Bounds);
            }
        }

        private void mainScreen_Load(object sender, EventArgs e) {
            Log.Debug("CORE: Entering mainScreen_Load");

            Log.Debug("LAUNCHER: Updating server list");
            ServerListUpdater.UpdateList();

            Log.Debug("CORE: Setting WindowName");
            Text = "GameLauncherReborn v" + Application.ProductVersion;

            Log.Debug("CORE: Checking window location");
            if (Location.X >= Screen.PrimaryScreen.Bounds.Width || Location.Y >= Screen.PrimaryScreen.Bounds.Height || Location.X <= 0 || Location.Y <= 0) {
                Log.Debug("CORE: Window location restored to centerScreen");

                Self.centerScreen(this);
                _windowMoved = true;
            }

            _NFSW_Installation_Source = !string.IsNullOrEmpty(_settingFile.Read("CDN")) ? _settingFile.Read("CDN") : "http://cdn.worldunited.gg/gamefiles/packed/";
            Log.Debug("LAUNCHER: NFSW Download Source is now: " + _NFSW_Installation_Source);

            Log.Debug("CORE: Applyinng ContextMenu");
            translatedBy.Text = "";
            ContextMenu = new ContextMenu();

            ContextMenu.MenuItems.Add(new MenuItem("Donate", (b,n) => { Process.Start("http://paypal.me/metonator95"); }));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem("Settings", settingsButton_Click));
            ContextMenu.MenuItems.Add(new MenuItem("Add Server", addServer_Click));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem("Close launcher", closebtn_Click));

            Notification.ContextMenu = ContextMenu;
            Notification.Icon = new Icon(Icon, Icon.Width, Icon.Height);
            Notification.Text = "GameLauncher";
            Notification.Visible = true;

            ContextMenu = null;

            email.Text = _settingFile.Read("AccountEmail");
            password.Text = Properties.Settings.Default.PasswordDecoded;
            if (!string.IsNullOrEmpty(_settingFile.Read("AccountEmail")) && !string.IsNullOrEmpty(_settingFile.Read("Password"))) {
                Log.Debug("LAUNCHER: Restoring last saved email and password");
                rememberMe.Checked = true;
            }

            serverPick.DisplayMember = "Name";

            Log.Debug("LAUNCHER: Setting server list");
            finalItems = ServerListUpdater.GetList();
            serverPick.DataSource = finalItems;

            //ForceSelectServer
            if (string.IsNullOrEmpty(_settingFile.Read("Server"))) {
                //SelectServerBtn_Click(null, null);
                new SelectServer().ShowDialog();

                if (ServerName != null)  {
                    this.SelectServerBtn.Text = "[...] " + ServerName.Name;
                    _settingFile.Write("Server", ServerName.IpAddress);
                } else {
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }
            } //else {
                Log.Debug("SERVERLIST: Checking...");
                Log.Debug("SERVERLIST: Setting first server in list");
                Log.Debug("SERVERLIST: Checking if server is set on INI File");
                try { 
                    if (string.IsNullOrEmpty(_settingFile.Read("Server"))) {
                        Log.Debug("SERVERLIST: Failed to find anything... assuming " + ((ServerInfo)serverPick.SelectedItem).IpAddress);
                        _settingFile.Write("Server", ((ServerInfo)serverPick.SelectedItem).IpAddress);
                    }
                } catch {
                    Log.Debug("SERVERLIST: Failed to write anything...");
                    _settingFile.Write("Server", "");
                }

                Log.Debug("SERVERLIST: Re-Checking if server is set on INI File");
                if (_settingFile.KeyExists("Server")) {
                    Log.Debug("SERVERLIST: Found something!");
                    _skipServerTrigger = true;

                    Log.Debug("SERVERLIST: Checking if server exists on our database");

                    if ( finalItems.FindIndex(i => string.Equals(i.IpAddress, _settingFile.Read("Server"))) != 0 /*_slresponse.Contains(_settingFile.Read("Server"))*/) {
                        Log.Debug("SERVERLIST: Server found! Checking ID");
                        var index = finalItems.FindIndex(i => string.Equals(i.IpAddress, _settingFile.Read("Server")));

                        Log.Debug("SERVERLIST: ID is " + index);
                        if (index >= 0) {
                            Log.Debug("SERVERLIST: ID set correctly");
                            serverPick.SelectedIndex = index;
                        }
					} else {
                        Log.Debug("SERVERLIST: Unable to find anything, assuming default");
                        serverPick.SelectedIndex = 1;
                        Log.Debug("SERVERLIST: Deleting unknown entry");
                        _settingFile.DeleteKey("Server");
                    }

                    Log.Debug("SERVERLIST: Triggering server change");
                    if (serverPick.SelectedIndex == 1) {
                        serverPick_SelectedIndexChanged(sender, e);
                    }
                    Log.Debug("SERVERLIST: All done");
                }
            //}

            Log.Debug("LAUNCHER: Checking for password");
            if (_settingFile.KeyExists("Password"))
            {
                _loginEnabled = true;
                _serverEnabled = true;
                _useSavedPassword = true;
                loginButton.Image = Properties.Resources.graybutton;
                loginButton.ForeColor = Color.White;
            }
            else
            {
                _loginEnabled = false;
                _serverEnabled = false;
                _useSavedPassword = false;
                loginButton.Image = Properties.Resources.graybutton;
                loginButton.ForeColor = Color.Gray;
            }

            Log.Debug("LAUNCHER: Setting game language");

            settingsLanguage.DisplayMember = "Text";
            settingsLanguage.ValueMember = "Value";

            var languages = new[] {
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

            settingsLanguage.DataSource = languages;

            if (_settingFile.KeyExists("Language"))
            {
                settingsLanguage.SelectedValue = _settingFile.Read("Language");
            }

            Task.Run(() => {
                String _slresponse2 = string.Empty;
                try {
                    WebClientWithTimeout wc = new WebClientWithTimeout();
                    try {
                        _slresponse2 = wc.DownloadString(Self.CDNUrlList);
                    } catch {
                        _slresponse2 = wc.DownloadString(Self.CDNUrlStaticList);
                    }
                } catch(Exception error) {
                    MessageBox.Show(error.Message, "An error occurred while loading CDN List");
                    _slresponse2 = JsonConvert.SerializeObject(new[] {
                        new CDNObject { name = "[CF] WorldUnited.gg Mirror", url = "http://cdn.worldunited.gg/gamefiles/packed/" }
                    });
                }

                List<CDNObject> CDNList = JsonConvert.DeserializeObject<List<CDNObject>>(_slresponse2);

                settingsCDNPick.Invoke(new Action(() => 
                {
                    settingsCDNPick.DisplayMember = "name";
                    settingsCDNPick.DataSource = CDNList;
                }));
            });

            Log.Debug("LAUNCHER: Re-checking InstallationDirectory: " + _settingFile.Read("InstallationDirectory"));

            var drive = Path.GetPathRoot(_settingFile.Read("InstallationDirectory"));
            if (!Directory.Exists(drive)) {
                if (!string.IsNullOrEmpty(drive)) {
                    var newdir = Directory.GetCurrentDirectory() + "\\GameFiles";
                    _settingFile.Write("InstallationDirectory", newdir);
                    Log.Debug(string.Format("LAUNCHER: Drive {0} was not found. Your actual installation directory is set to {1} now.", drive, newdir));

                    MessageBox.Show(null, string.Format("Drive {0} was not found. Your actual installation directory is set to {1} now.", drive, newdir), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //vfilesCheck.Checked = _disableChecks;
            settingsProxyCheckbox.Checked = _disableProxy;
            settingsDiscordRPCCheckbox.Checked = _disableDiscordRPC;

            Log.Debug("CORE: Hiding RegisterFormElements"); RegisterFormElements(false);
            Log.Debug("CORE: Hiding SettingsFormElements"); SettingsFormElements(false);
            Log.Debug("CORE: Hiding LoggedInFormElements"); LoggedInFormElements(false);

            Log.Debug("CORE: Showing LoginFormElements"); LoginFormElements(true);

            Log.Debug("CORE: Setting Registry Options");
            try {
                var gameInstallDirValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "GameInstallDir", RegistryValueKind.String);
                if (gameInstallDirValue == null || gameInstallDirValue.ToString() != Path.GetFullPath(_settingFile.Read("InstallationDirectory"))) {
                    try {
                        Registry.SetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "GameInstallDir", Path.GetFullPath(_settingFile.Read("InstallationDirectory")));
                        Registry.SetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "LaunchInstallDir", Path.GetFullPath(Application.ExecutablePath));
                    } catch(Exception ex) {
                        Log.Error(ex.Message);
                    }
                }
            } catch(Exception ex) {
                Log.Error(ex.Message);
            }

            Log.Debug("LAUNCHER: Setting configurations");
            _newGameFilesPath = Path.GetFullPath(_settingFile.Read("InstallationDirectory"));
            settingsGameFilesCurrentText.Text = "CURRENT DIRECTORY";
            settingsGameFilesCurrent.Text = _newGameFilesPath;
            //DavidCarbon
            settingsCDNCurrent.Text = _settingFile.Read("CDN");



            Log.Debug("DISCORD: Initializing DiscordRPC");
            Log.Debug("DISCORD: Checking if Discord RPC is Disabled from User Settings! It's value is " + _disableDiscordRPC);

            _presence.State = _OS;
            _presence.Details = "In-Launcher: " + Application.ProductVersion;
            _presence.Assets = new Assets
            {
                LargeImageText = "SBRW",
                LargeImageKey = "nfsw"
            };
            if(discordRpcClient != null) discordRpcClient.SetPresence(_presence);

            BeginInvoke((MethodInvoker)delegate {
                Log.Debug("CORE: 'GetServerInformation' from all Servers in Server List and Download Selected Server Banners");
                LaunchNfsw();
            });

            this.BringToFront();

            if(!DetectLinux.LinuxDetected()) {
                Log.Debug("LAUNCHER: Checking for update: " + Self.mainserver + "/update.php?version=" + Application.ProductVersion);
                new LauncherUpdateCheck(launcherIconStatus, launcherStatusText, launcherStatusDesc).checkAvailability();
            } else {
                launcherIconStatus.Image = Properties.Resources.ac_success;
                launcherStatusText.ForeColor = Color.FromArgb(0x9fc120);
                launcherStatusText.Text = "Launcher Status - Linux Fix";
                launcherStatusDesc.Text = "APLHA STAGE. VERSION " + Application.ProductVersion;
            }

            Self.gamedir = _settingFile.Read("InstallationDirectory");

            if(File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords") || File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords_dis")) { 
                try { 
                    settingsWordFilterCheck.Checked = File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords") ? false : true;
                } catch {
                    settingsWordFilterCheck.Checked = false;
                }
            } else {
                settingsWordFilterCheck.Enabled = false;
            }

            /* Load Settings API Connection Status */
            Task.Delay(800);
            PingServerListAPIStatus();
        }

        private void closebtn_Click(object sender, EventArgs e) {
            closebtn.BackgroundImage = Properties.Resources.close_click;

		    try {
                if (!(serverPick.SelectedItem is ServerInfo server)) return;
                _settingFile.Write("Server", server.IpAddress); 
            } catch {
                    
            }

            if (_windowMoved)
            {
                _settingFile.Write("LauncherPosX", Location.X.ToString());
                _settingFile.Write("LauncherPosY", Location.Y.ToString());
            }

            try { 
                _settingFile.Write("InstallationDirectory", Path.GetFullPath(_settingFile.Read("InstallationDirectory")));
            } catch {
                _settingFile.Write("InstallationDirectory", _settingFile.Read("InstallationDirectory"));
            }

            //DavidCarbon
            //This Saves the update the was skipped or to remind the user at next launch
            if (Settings.Default.IgnoreUpdateVersion != String.Empty)
            {
                _settingFile.Write("IgnoreUpdateVersion", Settings.Default.IgnoreUpdateVersion);
                Log.Debug("IGNOREUPDATEVERSION: Skipping Update " + Settings.Default.IgnoreUpdateVersion + " !");
            }
            else
            {
                if (_settingFile.Read("IgnoreUpdateVersion") != String.Empty)
                {
                    if (_settingFile.Read("IgnoreUpdateVersion") == Application.ProductVersion)
                    {
                        _settingFile.Write("IgnoreUpdateVersion", String.Empty);
                        Log.Debug("IGNOREUPDATEVERSION: Cleared OLD IgnoreUpdateVersion Build Number. You're now on the Latest Game Launcher!");
                    }
                    else
                    {
                        Log.Debug("IGNOREUPDATEVERSION: Manually Skipping Update " + _settingFile.Read("IgnoreUpdateVersion") + " !");
                    }
                }
                else
                {
                    Log.Debug("IGNOREUPDATEVERSION: Latest Game Launcher!");
                }
            }

            Process[] allOfThem = Process.GetProcessesByName("nfsw");
            foreach (var oneProcess in allOfThem) {
                Process.GetProcessById(oneProcess.Id).Kill();
            }

            //Kill DiscordRPC
            if(discordRpcClient != null) {
                discordRpcClient.Dispose();
            }

            ServerProxy.Instance.Stop();
            Notification.Dispose();

            //Leave this here. Its to properly close the launcher from Visual Studio (And Close the Launcher a well)
            this.Close();
        }

        private void addServer_Click(object sender, EventArgs e)
        {
             new AddServer().Show();
        }

        private void OpenDebugWindow(object sender, EventArgs e)
        {
            if (!(serverPick.SelectedItem is ServerInfo server)) return;

            var form = new DebugWindow(server.IpAddress, server.Name);
            form.Show();
        }

        private void closebtn_MouseEnter(object sender, EventArgs e)
        {
            closebtn.BackgroundImage = Properties.Resources.close_hover;
        }

        private void closebtn_MouseLeave(object sender, EventArgs e)
        {
            closebtn.BackgroundImage = Properties.Resources.close;
        }

        private void LoginEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && _loginEnabled)
            {
                loginButton_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }

        private void Loginbuttonenabler(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(email.Text) || string.IsNullOrEmpty(password.Text))
            {
                _loginEnabled = false;
                loginButton.Image = Properties.Resources.graybutton;
                loginButton.ForeColor = Color.Gray;
            }
            else
            {
                _loginEnabled = true;
                loginButton.Image = Properties.Resources.graybutton;
                loginButton.ForeColor = Color.White;
            }

            _useSavedPassword = false;
        }

        private void loginButton_MouseUp(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                loginButton.Image = Properties.Resources.graybutton_hover;
            }
            else
            {
                loginButton.Image = Properties.Resources.graybutton;
            }
        }

        private void loginButton_MouseDown(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                loginButton.Image = Properties.Resources.graybutton_click;
            }
            else
            {
                loginButton.Image = Properties.Resources.graybutton;
            }
        }

        private void loginButton_Click(object sender, EventArgs e) {
            if ((_loginEnabled == false || _serverEnabled == false) && _builtinserver == false) {
                return;
            }

            if (_isDownloading) {
                MessageBox.Show(null, "Please wait while launcher is still downloading gamefiles.", "GameLauncher", MessageBoxButtons.OK);
                return;
            }

            Tokens.Clear();

            String username = email.Text.ToString();
            String pass = password.Text.ToString();
            String realpass;

            Tokens.IPAddress = _serverInfo.IpAddress;
            Tokens.ServerName = _serverInfo.Name;

            Self.userAgent = (_serverInfo.forceUserAgent == null) ? null : _serverInfo.forceUserAgent;

            if (_modernAuthSupport == false) {
                //ClassicAuth sends password in SHA1
                realpass = (_useSavedPassword) ? _settingFile.Read("Password") : SHA.HashPassword(password.Text.ToString()).ToLower();
                ClassicAuth.Login(username, realpass);
            } else {
                //ModernAuth sends passwords in plaintext, but is POST request
                realpass = (_useSavedPassword) ? _settingFile.Read("Password") : password.Text.ToString();
                ModernAuth.Login(username, realpass);
            }

            if (rememberMe.Checked) {
                _settingFile.Write("AccountEmail", username);
                _settingFile.Write("Password", realpass);
                Properties.Settings.Default.PasswordDecoded = password.Text.ToString();
            } else {
                _settingFile.DeleteKey("AccountEmail");
                _settingFile.DeleteKey("Password");
                Properties.Settings.Default.PasswordDecoded = String.Empty;
            }

            Properties.Settings.Default.Save();

            if (String.IsNullOrEmpty(Tokens.Error)) {
                _loggedIn = true;
                _userId = Tokens.UserId;
                _loginToken = Tokens.LoginToken;
                _serverIp = Tokens.IPAddress;

                if(!String.IsNullOrEmpty(Tokens.Warning)) {
                    MessageBox.Show(null, Tokens.Warning, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                BackgroundImage = Properties.Resources.loggedbg;
                LoginFormElements(false);
                LoggedInFormElements(true);
            } else {
                MessageBox.Show(null, Tokens.Error, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loginButton_MouseEnter(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                loginButton.Image = Properties.Resources.graybutton_hover;
                loginButton.ForeColor = Color.White;
            }
            else
            {
                loginButton.Image = Properties.Resources.graybutton;
                loginButton.ForeColor = Color.Gray;
            }
        }

        private void loginButton_MouseLeave(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                loginButton.Image = Properties.Resources.graybutton;
                loginButton.ForeColor = Color.White;
            }
            else
            {
                loginButton.Image = Properties.Resources.graybutton;
                loginButton.ForeColor = Color.Gray;
            }
        }

        private void serverPick_SelectedIndexChanged(object sender, EventArgs e) {
            ServerStatusBar(_colorLoading, _startPoint, _endPoint);

            _serverInfo = (ServerInfo)serverPick.SelectedItem;
            _realServername = _serverInfo.Name;
            _realServernameBanner = _serverInfo.Name;
            _modernAuthSupport = false;

            if (_serverInfo.IsSpecial) {
                serverPick.SelectedIndex = _lastSelectedServerId;
                return;
            }

            if (!_skipServerTrigger) { return; }

            _lastSelectedServerId = serverPick.SelectedIndex;
            _allowRegistration = false;
            _loginEnabled = false;

            ServerStatusText.Text = "Server Status - Pinging";
            ServerStatusText.ForeColor = Color.FromArgb(66, 179, 189);
            ServerStatusDesc.Text = "";
            ServerStatusIcon.Image = Properties.Resources.server_checking;

            loginButton.ForeColor = Color.Gray;
            var verticalImageUrl = "";
            verticalBanner.Image = null;
            verticalBanner.BackColor = Color.Transparent;

            var serverIp = _serverInfo.IpAddress;
            string numPlayers;

            imageServerName.Text = ((ServerInfo)serverPick.SelectedItem).Name;

            if (serverPick.GetItemText(serverPick.SelectedItem) == "Offline Built-In Server") {
                _builtinserver = true;
                loginButton.Image = Properties.Resources.graybutton;
                loginButton.Text = "Launch".ToUpper();
                loginButton.ForeColor = Color.White;
            } else {
                _builtinserver = false;
                loginButton.Image = Properties.Resources.graybutton;
                loginButton.Text = "Login".ToUpper();
                loginButton.ForeColor = Color.Gray;
            }

            WebClientWithTimeout client = new WebClientWithTimeout();
            var artificialPingStart = Self.getTimestamp();
            verticalBanner.BackColor = Color.Transparent;

            var stringToUri = new Uri(serverIp + "/GetServerInformation");
            client.DownloadStringAsync(stringToUri);

            System.Timers.Timer aTimer = new System.Timers.Timer(10000);
            aTimer.Elapsed += (x, y) => { client.CancelAsync(); };
            aTimer.Enabled = true;

            client.DownloadStringCompleted += (sender2, e2) => {
                aTimer.Enabled = false;

                var artificialPingEnd = Self.getTimestamp();

                if(e2.Cancelled) {
                    ServerStatusBar(_colorOffline, _startPoint, _endPoint);

                    ServerStatusText.Text = "Server Status - Offline ( OFF )";
                    ServerStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                    ServerStatusDesc.Text = "Failed to connect to server.";
                    ServerStatusIcon.Image = Properties.Resources.server_offline;
                    _serverEnabled = false;
                    _allowRegistration = false;

                    if(!serverStatusDictionary.ContainsKey(_serverInfo.Id)) {
                        serverStatusDictionary.Add(_serverInfo.Id, 2);
                    } else { 
                        serverStatusDictionary[_serverInfo.Id] = 2; 
                    }
                } else if (e2.Error != null) {
                    ServerStatusBar(_colorOffline, _startPoint, _endPoint);

                    ServerStatusText.Text = "Server Status - Offline ( OFF )";
                    ServerStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                    ServerStatusDesc.Text = "Server seems to be offline.";
                    ServerStatusIcon.Image = Properties.Resources.server_offline;
                    _serverEnabled = false;
                    _allowRegistration = false;

                    if (!serverStatusDictionary.ContainsKey(_serverInfo.Id)) {
                        serverStatusDictionary.Add(_serverInfo.Id, 0);
                    } else {
                        serverStatusDictionary[_serverInfo.Id] = 0;
                    }
                } else {
                    if (_realServername == "Offline Built-In Server") {
                        numPlayers = "∞";
                    } else {
                        if (!serverStatusDictionary.ContainsKey(_serverInfo.Id)) {
                            serverStatusDictionary.Add(_serverInfo.Id, 1);
                        } else {
                            serverStatusDictionary[_serverInfo.Id] = 1;
                        }

                        purejson = e2.Result;
                        json = JsonConvert.DeserializeObject<GetServerInformation>(e2.Result);
                        Self.rememberjson = e2.Result;
                        try {
                            _realServernameBanner = json.serverName;
                            if (!string.IsNullOrEmpty(json.bannerUrl)) {
                                bool result;

                                try {
                                    result = Uri.TryCreate(json.bannerUrl, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                } catch {
                                    result = false;
                                }

                                if (result) {
                                    verticalImageUrl = json.bannerUrl;
                                } else {
                                    verticalImageUrl = null;
                                }
                            } else {
                                verticalImageUrl = null;
                            }
                        } catch {
                            verticalImageUrl = null;
                        }

                        try {
                            if (string.IsNullOrEmpty(json.requireTicket)) {
                                _ticketRequired = true;
                            } else if (json.requireTicket == "true") {
                                _ticketRequired = true;
                            } else {
                                _ticketRequired = false;
                            }
                        } catch {
                            _ticketRequired = false;
                        }

                        try {
                            if (string.IsNullOrEmpty(json.modernAuthSupport)) {
                                _modernAuthSupport = false;
                            } else if (json.modernAuthSupport == "true") {
                                if(stringToUri.Scheme == "https") {
                                    _modernAuthSupport = true;
                                } else {
                                    _modernAuthSupport = false;
                                }
                            } else {
                                _modernAuthSupport = false;
                            }
                        } catch {
                            _modernAuthSupport = false;
                        }

                        if (json.maxUsersAllowed == 0) {
                            numPlayers = string.Format("{0}/{1}", json.onlineNumber, json.numberOfRegistered);
                        } else {
                            numPlayers = string.Format("{0}/{1}", json.onlineNumber, json.maxUsersAllowed.ToString());
                        }

                        _allowRegistration = true;

                        ServerStatusBar(_colorOnline, _startPoint, _endPoint);
                    }

                    try { 
                        ServerStatusText.Text = "Server Status - Online ( ON )";
                        ServerStatusText.ForeColor = Color.FromArgb(159, 193, 32);
                        ServerStatusIcon.Image = Properties.Resources.server_online;
                        loginButton.ForeColor = Color.White;
                        _loginEnabled = true;
                    }
                    catch {
                        //¯\_(ツ)_/¯
                    }

                    //for thread safety
                    if (this.ServerStatusDesc.InvokeRequired)
                    {
                        ServerStatusDesc.Invoke(new Action(delegate ()
                        {
                            ServerStatusDesc.Text = string.Format("Players in Game - {0}", numPlayers);
                        }));
                    }
                    else
                    {
                        this.ServerStatusDesc.Text = string.Format("Players in Game - {0}", numPlayers);
                    }

                    _serverEnabled = true;

                    if (!string.IsNullOrEmpty(verticalImageUrl)) {
                        WebClientWithTimeout client2 = new WebClientWithTimeout();
                        Uri stringToUri3 = new Uri(verticalImageUrl);
                        client2.DownloadDataAsync(stringToUri3);
                        client2.DownloadProgressChanged += (sender4, e4) => {
                            if (e4.TotalBytesToReceive > 2000000) {
                                client2.CancelAsync();
                            }
                        };

                        client2.DownloadDataCompleted += (sender4, e4) => {
                            if (e4.Cancelled) {
                                return;
                            } else if (e4.Error != null) {
                                return;
                            } else {
                                try {
                                    if(UriScheme.ForceGame != true) {
                                        Image image;
                                        var memoryStream = new MemoryStream(e4.Result);
                                        image = Image.FromStream(memoryStream);
                                        verticalBanner.Image = image;
                                        verticalBanner.BackColor = Color.Black;

                                        imageServerName.Text = String.Empty; //_realServernameBanner;
                                    } else {
                                        imageServerName.Text = "WebLogin";
                                        verticalBanner.Image = null;
                                        verticalBanner.BackColor = Color.Black;
                                    }
                                } catch(Exception ex) {
                                    Console.WriteLine(ex.Message);
                                    verticalBanner.Image = null;
                                }
                            }
                        };
                    }
                }
            };
        }

        private void ApplyEmbeddedFonts() {
            FontFamily AkrobatSemiBold = FontWrapper.Instance.GetFontFamily("Akrobat-SemiBold.ttf");
            FontFamily AkrobatRegular = FontWrapper.Instance.GetFontFamily("Akrobat-Regular.ttf");

            /* Front Screen */
            launcherStatusText.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            launcherStatusDesc.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            ServerStatusText.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            ServerStatusDesc.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            APIStatusText.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            APIStatusDesc.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            playProgressText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            playProgressTextTimer.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            email.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            loginButton.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            password.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            rememberMe.Font = new Font(AkrobatSemiBold, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            forgotPassword.Font = new Font(AkrobatSemiBold, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            playButton.Font = new Font(AkrobatSemiBold, 15f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            currentWindowInfo.Font = new Font(AkrobatSemiBold, 11f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            imageServerName.Font = new Font(AkrobatSemiBold, 25f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            logoutButton.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            /* Settings */
            settingsLanguageText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            //settingsQualityText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsNetworkText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsMainSrvText.Font = new Font(AkrobatSemiBold, 10.5f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            settingsMainCDNText.Font = new Font(AkrobatSemiBold, 10.5f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            settingsBkupSrvText.Font = new Font(AkrobatSemiBold, 10.5f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            settingsBkupCDNText.Font = new Font(AkrobatSemiBold, 10.5f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            settingsCDNCurrentText.Font = new Font(AkrobatSemiBold, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsCDNCurrent.Font = new Font(AkrobatSemiBold, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            settingsCDNText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsCancel.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsSave.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsGamePathText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsVFilesButton.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsProxyCheckbox.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsDiscordRPCCheckbox.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsWordFilterCheck.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsGameFilesCurrent.Font = new Font(AkrobatSemiBold, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            settingsGameFilesCurrentText.Font = new Font(AkrobatSemiBold, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            /* Registering Screen */
            registerAgree.Font = new Font(AkrobatSemiBold, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerCancel.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerEmail.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            registerPassword.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            registerConfirmPassword.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            registerTicket.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            registerButton.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerButton.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
        }

        private void registerText_LinkClicked(object sender, EventArgs e)
        {
            registerButton.Image = Properties.Resources.greenbutton_click;
            if (_allowRegistration) {
                if(!string.IsNullOrEmpty(json.webSignupUrl)) {
                    Process.Start(json.webSignupUrl);
                    MessageBox.Show(null, "A browser window has been opened to complete registration on " + json.serverName, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if(_realServername == "WorldUnited Official" || _realServername == "WorldUnited OFFICIAL") {
                    Process.Start("https://signup.worldunited.gg/?discordid=" + Self.discordid);
                    MessageBox.Show(null, "A browser window has been opened to complete registration on WorldUnited OFFICIAL", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                BackgroundImage = (_ticketRequired) ? Properties.Resources.register_ticket : Properties.Resources.register_noticket;
                currentWindowInfo.Text = "REGISTER ON \n" + _realServername.ToUpper();
                LoginFormElements(false);
                RegisterFormElements(true);
            } else {
                MessageBox.Show(null, "Server seems to be offline.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void forgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (!string.IsNullOrEmpty(json.webRecoveryUrl)) {
                Process.Start(json.webRecoveryUrl);
                MessageBox.Show(null, "A browser window has been opened to complete password recovery on " + json.serverName, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string send = Prompt.ShowDialog("Please specify your email address.", "GameLauncher");

            if(send != String.Empty) {
                String responseString;
                try { 
                    Uri resetPasswordUrl = new Uri(_serverInfo.IpAddress + "/RecoveryPassword/forgotPassword");

                    var request = (HttpWebRequest)System.Net.WebRequest.Create(resetPasswordUrl);
                    var postData = "email="+send;
                    var data = Encoding.ASCII.GetBytes(postData);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream()) {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                } catch {
                    responseString = "Failed to send email!";
                }

                MessageBox.Show(null, responseString, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        /*  After Successful Login, Hide Login Forms */
        private void LoggedInFormElements(bool hideElements)
        {
            if (hideElements)
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
                currentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n {0}", email.Text).ToUpper();
            }

            logoutButton.Visible = hideElements;
            playProgress.Visible = hideElements;
            extractingProgress.Visible = hideElements;
            playProgressText.Visible = hideElements;
            playProgressTextTimer.Visible = hideElements;
            playButton.Visible = hideElements;
            settingsButton.Visible = hideElements;
            verticalBanner.Visible = hideElements;
            ServerStatusText.Visible = hideElements;
            ServerStatusIcon.Visible = hideElements;
            ServerStatusDesc.Visible = hideElements;
            launcherIconStatus.Visible = hideElements;
            launcherStatusDesc.Visible = hideElements;
            launcherStatusText.Visible = hideElements;
            //allowedCountriesLabel.Visible = hideElements;
            APIStatusText.Visible = hideElements;
            APIStatusDesc.Visible = hideElements;
            APIStatusIcon.Visible = hideElements;
        }

        private void LoginFormElements(bool hideElements = false)
        {
            if (hideElements)
            {
                currentWindowInfo.Text = "Enter your account information to Log In".ToUpper();
            }

            rememberMe.Visible = hideElements;
            loginButton.Visible = hideElements;
            ServerStatusText.Visible = hideElements;
            ServerStatusIcon.Visible = hideElements;
            ServerStatusDesc.Visible = hideElements;
            launcherIconStatus.Visible = hideElements;
            launcherStatusDesc.Visible = hideElements;
            launcherStatusText.Visible = hideElements;

            APIStatusText.Visible = hideElements;
            APIStatusDesc.Visible = hideElements;
            APIStatusIcon.Visible = hideElements;

            registerText.Visible = hideElements;
            serverPick.Visible = hideElements;
            email.Visible = hideElements;
            password.Visible = hideElements;
            forgotPassword.Visible = hideElements;
            settingsButton.Visible = hideElements;
            verticalBanner.Visible = hideElements;
            playProgressText.Visible = hideElements;
            playProgressTextTimer.Visible = hideElements;
            playProgress.Visible = hideElements;
            extractingProgress.Visible = hideElements;
            addServer.Visible = hideElements;
            //allowedCountriesLabel.Visible = hideElements;
            serverPick.Enabled = true;
        }

        private void RegisterFormElements(bool hideElements = true) {
            registerButton.Visible = hideElements;
            registerEmail.Visible = hideElements;
            registerPassword.Visible = hideElements;
            registerConfirmPassword.Visible = hideElements;
            registerAgree.Visible = hideElements;
            registerCancel.Visible = hideElements;
            registerTicket.Visible = (_ticketRequired) ? hideElements : false;

            verticalBanner.Visible = hideElements;
            extractingProgress.Visible = hideElements;
            playProgress.Visible = hideElements;
            playProgressText.Visible = hideElements;

            ServerStatusText.Visible = hideElements;
            ServerStatusIcon.Visible = hideElements;
            ServerStatusDesc.Visible = hideElements;
            launcherIconStatus.Visible = hideElements;
            launcherStatusDesc.Visible = hideElements;
            launcherStatusText.Visible = hideElements;

            APIStatusText.Visible = hideElements;
            APIStatusDesc.Visible = hideElements;
            APIStatusIcon.Visible = hideElements;

            addServer.Visible = hideElements;
            serverPick.Visible = hideElements;
            serverPick.Enabled = false;

            // Reset fields
            registerEmail.Text = "";
            registerPassword.Text = "";
            registerConfirmPassword.Text = "";
            registerAgree.Checked = false;
        }

        private void logoutButton_Click(object sender, EventArgs e) {
            if(_disableLogout == true) {
                return;
            }
            BackgroundImage = Properties.Resources.loginbg;
            _loggedIn = false;
            LoggedInFormElements(false);
            LoginFormElements(true);

            _userId = String.Empty;
            _loginToken = String.Empty;
        }

        private void greenbutton_hover_MouseEnter(object sender, EventArgs e)
        {
            settingsSave.Image = Properties.Resources.greenbutton_hover;
            registerText.Image = Properties.Resources.greenbutton_hover;
            registerButton.Image = Properties.Resources.greenbutton_hover;
        }

        private void greenbutton_MouseLeave(object sender, EventArgs e)
        {
            settingsSave.Image = Properties.Resources.greenbutton;
            registerText.Image = Properties.Resources.greenbutton;
            registerButton.Image = Properties.Resources.greenbutton;
        }

        private void greenbutton_hover_MouseUp(object sender, EventArgs e)
        {
            settingsSave.Image = Properties.Resources.greenbutton_hover;
            registerText.Image = Properties.Resources.greenbutton_hover;
            registerButton.Image = Properties.Resources.greenbutton_hover;
        }

        private void greenbutton_click_MouseDown(object sender, EventArgs e)
        {
            settingsSave.Image = Properties.Resources.greenbutton_click;
            registerText.Image = Properties.Resources.greenbutton_click;
            registerButton.Image = Properties.Resources.greenbutton_click;
        }

        private void registerCancel_Click(object sender, EventArgs e)
        {
            BackgroundImage = Properties.Resources.loginbg;
            currentWindowInfo.Text = "Enter your account information to Log In:".ToUpper();
            RegisterFormElements(false);
            LoginFormElements(true);
        }

        private void graybutton_click_MouseDown(object sender, EventArgs e)
        {
            settingsCancel.Image = Properties.Resources.graybutton_click;
            logoutButton.Image = Properties.Resources.graybutton_click;
            registerCancel.Image = Properties.Resources.graybutton_click;
        }

        private void graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            settingsCancel.Image = Properties.Resources.graybutton_hover;
            logoutButton.Image = Properties.Resources.graybutton_hover;
            registerCancel.Image = Properties.Resources.graybutton_hover;
        }

        private void graybutton_MouseLeave(object sender, EventArgs e)
        {
            settingsCancel.Image = Properties.Resources.graybutton;
            logoutButton.Image = Properties.Resources.graybutton;
            registerCancel.Image = Properties.Resources.graybutton;
        }

        private void graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            settingsCancel.Image = Properties.Resources.graybutton_hover;
            logoutButton.Image = Properties.Resources.graybutton_hover;
            registerCancel.Image = Properties.Resources.graybutton_hover;
        }

        public void DrawErrorAroundTextBox(TextBox x)
        {
            x.BorderStyle = BorderStyle.FixedSingle;
            var p = new Pen(Color.Red);
            var g = CreateGraphics();
            var variance = 1;
            g.DrawRectangle(p, new Rectangle(x.Location.X - variance, x.Location.Y - variance, x.Width + variance, x.Height + variance));
        }

        private void registerButton_Click(object sender, EventArgs e) {
            Refresh();

            List<string> registerErrors = new List<string>(); 

            if (string.IsNullOrEmpty(registerEmail.Text)) {
                registerErrors.Add("Please enter your e-mail.");
            } else if (Self.validateEmail(registerEmail.Text) == false) {
                registerErrors.Add("Please enter a valid e-mail address.");
            }

            if (string.IsNullOrEmpty(registerTicket.Text) && _ticketRequired) {
                registerErrors.Add("Please enter your ticket.");
            }

            if (string.IsNullOrEmpty(registerPassword.Text)) {
                registerErrors.Add("Please enter your password.");
            }

            if (string.IsNullOrEmpty(registerConfirmPassword.Text)) {
                registerErrors.Add("Please confirm your password.");
            }

            if (registerConfirmPassword.Text != registerPassword.Text) {
                registerErrors.Add("Passwords don't match.");
            }

            if (!registerAgree.Checked) {
                registerErrors.Add("You have not agreed to the Terms of Service.");
            }

            if (registerErrors.Count == 0) {
                bool allowReg = false;

                try {
                    WebClientWithTimeout breachCheck = new WebClientWithTimeout();
                    String checkPassword = SHA.HashPassword(registerPassword.Text.ToString()).ToUpper();

                    var regex = new Regex(@"([0-9A-Z]{5})([0-9A-Z]{35})").Split(checkPassword);

                    String range = regex[1];
                    String verify = regex[2];
                    String serverReply = breachCheck.DownloadString("https://api.pwnedpasswords.com/range/"+range);

                    string[] hashes = serverReply.Split('\n');
                    foreach (string hash in hashes) {
                        var splitChecks = hash.Split(':');
                        if(splitChecks[0] == verify) {
                            var passwordCheckReply = MessageBox.Show(null, "Password used for registration has been breached " + Convert.ToInt32(splitChecks[1])+ " times, you should consider using different one.\r\nAlternatively you can use unsafe password anyway. Use it?", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if(passwordCheckReply == DialogResult.Yes) {
                                allowReg = true;
                            } else {
                                allowReg = false;
                            }
                        } else {
                            allowReg = true;
                        }
                    }
                } catch {
                    allowReg = true;
                }

                if(allowReg == true) {
                    Tokens.Clear();

                    String username = registerEmail.Text.ToString();
                    String realpass;
                    String token = (_ticketRequired) ? registerTicket.Text : null;

                    Tokens.IPAddress = _serverInfo.IpAddress;
                    Tokens.ServerName = _serverInfo.Name;

                    if (_modernAuthSupport == false) {
                        realpass = SHA.HashPassword(registerPassword.Text.ToString()).ToLower();
                        ClassicAuth.Register(username, realpass, token);
                    } else {
                        realpass = registerPassword.Text.ToString();
                        ModernAuth.Register(username, realpass, token);
                    }

                    if (!String.IsNullOrEmpty(Tokens.Success)) {
                        _loggedIn = true;
                        _userId = Tokens.UserId;
                        _loginToken = Tokens.LoginToken;
                        _serverIp = Tokens.IPAddress;

                        MessageBox.Show(null, Tokens.Success, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        BackgroundImage = Properties.Resources.loginbg;

                        RegisterFormElements(false);
                        LoginFormElements(true);

                        _loggedIn = true;
                    } else {
                        MessageBox.Show(null, Tokens.Error, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                } else {
                    var message = "There were some errors while registering, please fix them:\n\n";

                    foreach (var error in registerErrors) {
                        message += "• " + error + "\n";
                    }

                    MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /*
         * SETTINGS PAGE LAYOUT
         */

        private void settingsButton_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            settingsButton.BackgroundImage = Properties.Resources.settingsbtn_click;
            BackgroundImage = Properties.Resources.secondarybackground;
            SettingsFormElements(true);
            RegisterFormElements(false);
            LoggedInFormElements(false);
            LoginFormElements(false);
            PingAPIStatus();
        }

        private void settingsButton_MouseEnter(object sender, EventArgs e) {
            settingsButton.BackgroundImage = Properties.Resources.settingsbtn_hover;
        }

        private void settingsButton_MouseLeave(object sender, EventArgs e) {
            settingsButton.BackgroundImage = Properties.Resources.settingsbtn;
        }

        private void logo_MouseLeave(object sender, EventArgs e)
        {
            logo.Image = Properties.Resources.logo;
        }

        private void logo_MouseEnter(object sender, EventArgs e)
        {
            logo.Image = Properties.Resources.logo_hover;
        }

        private void settingsCancel_Click(object sender, EventArgs e)
        {
            SettingsFormElements(false);

            if (_loggedIn)
            {
                BackgroundImage = Properties.Resources.loggedbg;
                LoginFormElements();
                LoggedInFormElements(true);
            }
            else
            {
                BackgroundImage = Properties.Resources.loginbg;
                LoggedInFormElements(false);
                LoginFormElements(true);
            }

            //Reset Connection Status Labels - DavidCarbon
            settingsMainSrvText.Text = "Main Server List API: PINGING";
            settingsMainSrvText.ForeColor = Color.FromArgb(66, 179, 189);
            settingsMainCDNText.Text = "Main CDN List API: PINGING";
            settingsMainCDNText.ForeColor = Color.FromArgb(66, 179, 189);
            settingsBkupSrvText.Text = "Backup Server List API: PINGING";
            settingsBkupSrvText.ForeColor = Color.FromArgb(66, 179, 189);
            settingsBkupCDNText.Text = "Backup CDN List API: PINGING";
            settingsBkupCDNText.ForeColor = Color.FromArgb(66, 179, 189);
        }

        private void settingsSave_Click(object sender, EventArgs e) {

            //TODO null check
            _settingFile.Write("Language", settingsLanguage.SelectedValue.ToString());
            _settingFile.Write("CDN", ((CDNObject)settingsCDNPick.SelectedItem).url);

            var userSettingsXml = new XmlDocument();

            try { 
                if (File.Exists(_userSettings)) {
                    try  {
                        userSettingsXml.Load(_userSettings);
                        var language = userSettingsXml.SelectSingleNode("Settings/UI/Language");
                        language.InnerText = settingsLanguage.SelectedValue.ToString();
                    } catch {
                        File.Delete(_userSettings);

                        var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
                        var ui = setting.AppendChild(userSettingsXml.CreateElement("UI"));

                        var persistentValue = setting.AppendChild(userSettingsXml.CreateElement("PersistentValue"));
                        var chat = persistentValue.AppendChild(userSettingsXml.CreateElement("Chat"));
                        chat.InnerXml = "<DefaultChatGroup Type=\"string\">" + settingsLanguage.SelectedValue + "</DefaultChatGroup>";
                        ui.InnerXml = "<Language Type=\"string\">" + settingsLanguage.SelectedValue + "</Language>";

                        var directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(_userSettings));
                    }
                } else {
                    try {
                        var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
                        var ui = setting.AppendChild(userSettingsXml.CreateElement("UI"));

                        var persistentValue = setting.AppendChild(userSettingsXml.CreateElement("PersistentValue"));
                        var chat = persistentValue.AppendChild(userSettingsXml.CreateElement("Chat"));
                        chat.InnerXml = "<DefaultChatGroup Type=\"string\">" + settingsLanguage.SelectedValue + "</DefaultChatGroup>";
                        ui.InnerXml = "<Language Type=\"string\">" + settingsLanguage.SelectedValue + "</Language>";

                        var directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(_userSettings));
                    } catch (Exception ex) {
                        MessageBox.Show(null, "There was an error saving your settings to actual file. Restoring default.\n" + ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        File.Delete(_userSettings);
                    }
                }
            } catch(Exception ex) {
                MessageBox.Show(null, "There was an error saving your settings to actual file. Restoring default.\n" + ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.Delete(_userSettings);
            }

            userSettingsXml.Save(_userSettings);

            if (_settingFile.Read("InstallationDirectory") != _newGameFilesPath) {
                _settingFile.Write("InstallationDirectory", _newGameFilesPath);
                _restartRequired = true;
            }

            String disableProxy = (settingsProxyCheckbox.Checked == true) ? "1" : "0";
            if (_settingFile.Read("DisableProxy") != disableProxy) {
                _settingFile.Write("DisableProxy", (settingsProxyCheckbox.Checked == true) ? "1" : "0");
                _restartRequired = true;
            }

            String disableRPC = (settingsDiscordRPCCheckbox.Checked == true) ? "1" : "0";
            if (_settingFile.Read("DisableRPC") != disableRPC)
            {
                _settingFile.Write("DisableRPC", (settingsDiscordRPCCheckbox.Checked == true) ? "1" : "0");
                _restartRequired = true;
            }


            if (_restartRequired) {
                MessageBox.Show(null, "In order to see settings changes, you need to restart launcher manually.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //Actually lets check those 2 files
            if(File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords") && File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords_dis")) {
                File.Delete(_settingFile.Read("InstallationDirectory") + "/profwords_dis");
            }

            //Delete/Enable profwords filter here
            if (settingsWordFilterCheck.Checked) {
                if (File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords")) File.Move(_settingFile.Read("InstallationDirectory") + "/profwords", _settingFile.Read("InstallationDirectory") + "/profwords_dis");
            } else {
                if (File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords_dis")) File.Move(_settingFile.Read("InstallationDirectory") + "/profwords_dis", _settingFile.Read("InstallationDirectory") + "/profwords");
            }

            SettingsFormElements(false);

            if (_loggedIn) {
                BackgroundImage = Properties.Resources.loggedbg;
                LoginFormElements();
                LoggedInFormElements(true);
            } else {
                BackgroundImage = Properties.Resources.loginbg;
                LoggedInFormElements(false);
                LoginFormElements(true);
            }

            //Reset Connection Status Labels - DavidCarbon
            settingsMainSrvText.Text = "Main Server List API: PINGING";
            settingsMainSrvText.ForeColor = Color.FromArgb(66, 179, 189);
            settingsMainCDNText.Text = "Main CDN List API: PINGING";
            settingsMainCDNText.ForeColor = Color.FromArgb(66, 179, 189);
            settingsBkupSrvText.Text = "Backup Server List API: PINGING";
            settingsBkupSrvText.ForeColor = Color.FromArgb(66, 179, 189);
            settingsBkupCDNText.Text = "Backup CDN List API: PINGING";
            settingsBkupCDNText.ForeColor = Color.FromArgb(66, 179, 189);
        }

        private void settingsGameFiles_Click(object sender, EventArgs e)
        {
            var fbd2 = new FolderBrowserDialog();
            var result2 = fbd2.ShowDialog();

            if (result2 == DialogResult.OK)
            {
                _newGameFilesPath = Path.GetFullPath(fbd2.SelectedPath);
                settingsGameFilesCurrentText.Text = "NEW DIRECTORY";
                settingsGameFilesCurrent.Text = _newGameFilesPath;
            }
        }

        private void settingsGameFilesCurrent_Click(object sender, EventArgs e) {
            Process.Start(_newGameFilesPath);
        }

        private void SettingsFormElements(bool hideElements = true) {
            if (hideElements) {
                currentWindowInfo.Text = "";
            }

            settingsCancel.Visible = hideElements;
            settingsSave.Visible = hideElements;
            settingsLanguage.Visible = hideElements;
            settingsLanguageText.Visible = hideElements;
            settingsCDNPick.Visible = hideElements;
            settingsCDNText.Visible = hideElements;
            settingsGameFiles.Visible = hideElements;
            settingsGameFilesCurrentText.Visible = hideElements;
            settingsGameFilesCurrent.Visible = hideElements;
            settingsGamePathText.Visible = hideElements;
            settingsWordFilterCheck.Visible = hideElements;
            //settingsVFilesButton.Visible = hideElements;
            settingsProxyCheckbox.Visible = hideElements;
            settingsDiscordRPCCheckbox.Visible = hideElements;
            //Connection Status - DavidCarbon
            settingsNetworkText.Visible = hideElements;
            settingsMainSrvText.Visible = hideElements;
            settingsMainCDNText.Visible = hideElements;
            settingsBkupSrvText.Visible = hideElements;
            settingsBkupCDNText.Visible = hideElements;
            settingsCDNCurrentText.Visible = hideElements;
            settingsCDNCurrent.Visible = hideElements;

        }

        private void StartGame(string userId, string loginToken) {

            if(UriScheme.ServerIP != String.Empty) {
                _serverIp = UriScheme.ServerIP;
            }

            if(_realServername == "Freeroam Sparkserver") {
                //Force proxy enabled.
                Log.Info("LAUNCHER: Forcing Proxified connection for FRSS");
                _disableProxy = false;
            }

            _nfswstarted = new Thread(() => {
                if(_disableProxy == true) {
                    if (_disableDiscordRPC == true)
                    {
                        discordRpcClient.Dispose();
                        discordRpcClient = null;
                    }

                    Uri convert = new Uri(_serverIp);

                    if(convert.Scheme == "http") {
                        Match match = Regex.Match(convert.Host, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
                        if (!match.Success) {
                            _serverIp = _serverIp.Replace(convert.Host, Self.HostName2IP(convert.Host));
                        }
                    }

                    LaunchGame(userId, loginToken, _serverIp, this);
                } else {
                    if (_disableDiscordRPC == true)
                    {
                        discordRpcClient.Dispose();
                        discordRpcClient = null;
                    }
                    LaunchGame(userId, loginToken, "http://127.0.0.1:" + Self.ProxyPort + "/nfsw/Engine.svc", this);
                }
            }) { IsBackground = true };

            _nfswstarted.Start();

            if (_disableDiscordRPC == false)
            {
                _presenceImageKey = _serverInfo.DiscordPresenceKey;
                _presence.State = _realServername;
                _presence.Details = "In-Game";
                _presence.Assets = new Assets
                {
                    LargeImageText = "Need for Speed: World",
                    LargeImageKey = "nfsw",
                    SmallImageText = _realServername,
                    SmallImageKey = _presenceImageKey
                };

                if (discordRpcClient != null) discordRpcClient.SetPresence(_presence);
            }

        }

        //DavidCarbon
        private async void PingAPIStatus ()
        {
            Log.Debug("SETTINGS PINGING API: Checking APIs");
            await Task.Delay(500);
            HttpWebRequest requestMainServerList = (HttpWebRequest)HttpWebRequest.Create(Self.mainserver + "/serverlist.json");
            requestMainServerList.AllowAutoRedirect = false; // Find out if this site is up and don't follow a redirector
            requestMainServerList.Method = "HEAD";
            requestMainServerList.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            try
            {
                HttpWebResponse mainServerListResponse = (HttpWebResponse)requestMainServerList.GetResponse();
                mainServerListResponse.Close();
                settingsMainSrvText.Text = "Main Server List API: ONLINE";
                settingsMainSrvText.ForeColor = Color.FromArgb(159, 193, 32);
                Log.Debug("SETTINGS PINGING API: Main Server List Online");
                //Do something with response.Headers to find out information about the request
            }
            catch (WebException)
            {
                settingsMainSrvText.Text = "Main Server List API: ERROR";
                settingsMainSrvText.ForeColor = Color.FromArgb(254, 0, 0);
                Log.Debug("SETTINGS PINGING API: Main Server List Failed to Connect");
                //Set flag if there was a timeout or some other issues
            }

            await Task.Delay(1000);
            HttpWebRequest requestBkupServerList = (HttpWebRequest)HttpWebRequest.Create(Self.staticapiserver + "/serverlist.json");
            requestBkupServerList.AllowAutoRedirect = false;
            requestBkupServerList.Method = "HEAD";
            requestBkupServerList.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            try
            {
                HttpWebResponse bkupServerListResponse = (HttpWebResponse)requestBkupServerList.GetResponse();
                bkupServerListResponse.Close();
                settingsBkupSrvText.Text = "Backup Server List API: ONLINE";
                settingsBkupSrvText.ForeColor = Color.FromArgb(159, 193, 32);
                Log.Debug("SETTINGS PINGING API: Backup Server List Online");
            }
            catch (WebException)
            {
                settingsBkupSrvText.Text = "Backup Server List API: ERROR";
                settingsBkupSrvText.ForeColor = Color.FromArgb(254, 0, 0);
                Log.Debug("SETTINGS PINGING API: Backup Server List failed to Connect");
            }

            await Task.Delay(1500);
            HttpWebRequest requestMainCDNList = (HttpWebRequest)HttpWebRequest.Create(Self.mainserver + "/cdn_list.json");
            requestMainCDNList.AllowAutoRedirect = false;
            requestMainCDNList.Method = "HEAD";
            requestMainCDNList.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            try
            {
                HttpWebResponse mainCDNListResponse = (HttpWebResponse)requestMainCDNList.GetResponse();
                mainCDNListResponse.Close();
                settingsMainCDNText.Text = "Main CDN List API: ONLINE";
                settingsMainCDNText.ForeColor = Color.FromArgb(159, 193, 32);
                Log.Debug("SETTINGS PINGING API: Main CDN List Online");
            }
            catch (WebException)
            {
                settingsMainCDNText.Text = "Main CDN List API: ERROR";
                settingsMainCDNText.ForeColor = Color.FromArgb(254, 0, 0);
                Log.Debug("SETTINGS PINGING API: Main CDN List failed to Connect");
            }

            await Task.Delay(2000);
            HttpWebRequest requestBkupCDNList = (HttpWebRequest)HttpWebRequest.Create(Self.staticapiserver + "/cdn_list.json");
            requestBkupCDNList.AllowAutoRedirect = false;
            requestBkupCDNList.Method = "HEAD";
            requestBkupCDNList.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            try
            {
                HttpWebResponse bkupCDNListResponse = (HttpWebResponse)requestBkupCDNList.GetResponse();
                bkupCDNListResponse.Close();
                settingsBkupCDNText.Text = "Backup CDN List API: ONLINE";
                settingsBkupCDNText.ForeColor = Color.FromArgb(159, 193, 32);
                Log.Debug("SETTINGS PINGING API: Backup CDN List Online");
            }
            catch (WebException)
            {
                settingsBkupCDNText.Text = "Backup CDN List API: ERROR";
                settingsBkupCDNText.ForeColor = Color.FromArgb(254, 0, 0);
                Log.Debug("SETTINGS PINGING API: Backup CDN List failed to Connect");
            }

        }

        //Check Serverlist API Status Upon Main Screen load - DavidCarbon
        private async void PingServerListAPIStatus()
        {
            Log.Debug("PINGING API: Checking API Status");
            HttpWebRequest requestMainServerListAPI = (HttpWebRequest)HttpWebRequest.Create(Self.mainserver + "/serverlist.json");
            requestMainServerListAPI.AllowAutoRedirect = false;
            requestMainServerListAPI.Method = "HEAD";
            requestMainServerListAPI.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            try
            {
                HttpWebResponse mainServerListResponseAPI = (HttpWebResponse)requestMainServerListAPI.GetResponse();
                mainServerListResponseAPI.Close();
                APIStatusText.Text = "Main API - Online";
                APIStatusText.ForeColor = Color.FromArgb(159, 193, 32);
                APIStatusDesc.Text = "Connected to Main API";
                APIStatusIcon.Image = Properties.Resources.api_success;
                Log.Debug("PINGING API: Main Server has responded. Its Online!");
            }
            catch (WebException)
            {
                APIStatusText.Text = "Main API - Offline";
                APIStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                APIStatusDesc.Text = "Checking to Backup API";
                APIStatusIcon.Image = Properties.Resources.api_error;
                Log.Debug("PINGING API: Main Server has responded. Its Offline! Checking Backup...");

                await Task.Delay(1500);
                APIStatusText.Text = "Backup API - Pinging";
                APIStatusText.ForeColor = Color.FromArgb(66, 179, 189);
                APIStatusIcon.Image = Properties.Resources.api_checking;
                
                await Task.Delay(1500);
                try
                {
                    //Check Using Backup API
                    HttpWebRequest requestBkupServerListAPI = (HttpWebRequest)HttpWebRequest.Create(Self.staticapiserver + "/serverlist.json");
                    requestBkupServerListAPI.AllowAutoRedirect = false;
                    requestBkupServerListAPI.Method = "HEAD";
                    requestBkupServerListAPI.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                    try
                    {
                        HttpWebResponse bkupServerListResponseAPI = (HttpWebResponse)requestBkupServerListAPI.GetResponse();
                        bkupServerListResponseAPI.Close();
                        APIStatusText.Text = "Backup API - Online";
                        APIStatusText.ForeColor = Color.FromArgb(159, 193, 32);
                        APIStatusDesc.Text = "Connected to Backup API";
                        APIStatusIcon.Image = Properties.Resources.api_success;
                        Log.Debug("PINGING API: Backup Server has responded. Its Online!");
                    }
                    catch (WebException)
                    {
                        APIStatusText.Text = "Connection API - Error";
                        APIStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                        APIStatusDesc.Text = "Failed to Connect to APIs";
                        APIStatusIcon.Image = Properties.Resources.api_error;
                        Log.Debug("PINGING API: Failed to Connect to APIs! Quick Hide and Bunker Down! (Ask for help)");
                    }
                }
                catch { }
            }
        }

        private void LaunchGame(string userId, string loginToken, string serverIp, Form x) {
            var oldfilename = _settingFile.Read("InstallationDirectory") + "/nfsw.exe";

            var args = _serverInfo.Id.ToUpper() + " " + serverIp + " " + loginToken + " " + userId;
            var psi = new ProcessStartInfo();

            if(DetectLinux.LinuxDetected()) { 
                psi.UseShellExecute = false;
            }

            psi.WorkingDirectory = _settingFile.Read("InstallationDirectory");
            psi.FileName = oldfilename;
            psi.Arguments = args;

            var nfswProcess = Process.Start(psi);
            nfswProcess.PriorityClass = ProcessPriorityClass.AboveNormal;

            var processorAffinity = 0;
            for (var i = 0; i < Math.Min(Math.Max(1, Environment.ProcessorCount), 8); i++)
            {
                processorAffinity |= 1 << i;
            }

            nfswProcess.ProcessorAffinity = (IntPtr)processorAffinity;

            AntiCheat.process_id = nfswProcess.Id;

            //TIMER HERE
            int secondsToShutDown = (json.secondsToShutDown != 0) ? json.secondsToShutDown : 2*60*60;
                System.Timers.Timer shutdowntimer = new System.Timers.Timer();
                shutdowntimer.Elapsed += (x2, y2) => {
                    if(secondsToShutDown == 300) {
                        Notification.Visible = true;
                        Notification.BalloonTipIcon = ToolTipIcon.Info;
                        Notification.BalloonTipTitle = "SpeedBug Fix - " + _realServername;
                        Notification.BalloonTipText = "Game is going to shut down in 5 minutes. Please restart it manually before the launcher does it.";
                        Notification.ShowBalloonTip(5000);
                        Notification.Dispose();
                    }

                    Process[] allOfThem = Process.GetProcessesByName("nfsw");

                    if (secondsToShutDown <= 0) {
                        if (Self.CanDisableGame == true) {
                            foreach (var oneProcess in allOfThem) {
                                _gameKilledBySpeedBugCheck = true;
                                Process.GetProcessById(oneProcess.Id).Kill();
                            }
                        } else {
                            secondsToShutDown = 0;
                        }
                    }

                    //change title

                    foreach (var oneProcess in allOfThem) {
                        //if (oneProcess.ProcessName == "nfsw") {
                            long p = oneProcess.MainWindowHandle.ToInt64();
                            TimeSpan t = TimeSpan.FromSeconds(secondsToShutDown);
                            string secondsToShutDownNamed = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);

                            if (secondsToShutDown == 0) {
                                secondsToShutDownNamed = "Waiting for event to finish.";
                            }

                            User32.SetWindowText((IntPtr)p, "NEED FOR SPEED™ WORLD | Server: " + _realServername + " | Time Remaining: " + secondsToShutDownNamed);
                        //}
                    }

                    --secondsToShutDown;
                };

                shutdowntimer.Interval = 1000;
                shutdowntimer.Enabled = true;
            

                if (nfswProcess != null) {
                    nfswProcess.EnableRaisingEvents = true;
                    _nfswPid = nfswProcess.Id;

                    nfswProcess.Exited += (sender2, e2) => {
                        _nfswPid = 0;
                        var exitCode = nfswProcess.ExitCode;

                        if (_gameKilledBySpeedBugCheck == true) exitCode = 2137;

                        if (exitCode == 0) {
                            closebtn_Click(null, null);
                        } else {
                            x.BeginInvoke(new Action(() => {
                                x.WindowState = FormWindowState.Normal;
                                x.Opacity = 1;
                                x.ShowInTaskbar = true;

                                String errorMsg = "Game Crash with exitcode: " + exitCode.ToString() + " (0x" + exitCode.ToString("X") + ")";
                                if (exitCode == -1073741819)    errorMsg = "Game Crash: Access Violation (0x" + exitCode.ToString("X") + ")";
                                if (exitCode == -1073740940)    errorMsg = "Game Crash: Heap Corruption (0x" + exitCode.ToString("X") + ")";
                                if (exitCode == -1073740791)    errorMsg = "Game Crash: Stack buffer overflow (0x" + exitCode.ToString("X") + ")";
                                if (exitCode == -805306369)     errorMsg = "Game Crash: Application Hang (0x" + exitCode.ToString("X") + ")";
                                if (exitCode == -1073741515)    errorMsg = "Game Crash: Missing dependency files (0x" + exitCode.ToString("X") + ")";
                                if (exitCode == -1073740972)    errorMsg = "Game Crash: Debugger crash (0x" + exitCode.ToString("X") + ")";
                                if (exitCode == -1073741676)    errorMsg = "Game Crash: Division by Zero (0x" + exitCode.ToString("X") + ")";

                                if (exitCode == 1)              errorMsg = "The process nfsw.exe was killed via Task Manager";
                                if (exitCode == 2137)           errorMsg = "Launcher killed your game to prevent SpeedBugging.";

                                if (exitCode == -3)             errorMsg = "The Server was unable to resolve the request";
                                if (exitCode == -4)             errorMsg = "Another instance is already executed";
                                if (exitCode == -5)             errorMsg = "DirectX Device was not found. Please install GPU Drivers before playing";
                                if (exitCode == -6)             errorMsg = "Server was unable to resolve your request";

                                //ModLoader
                                if (exitCode == 2)              errorMsg = "ModNet: Game was launched with invalid command line parameters.";
                                if (exitCode == 3)              errorMsg = "ModNet: .links file should not exist upon startup!";
                                if (exitCode == 4)              errorMsg = "ModNet: An Unhandled Error Appeared";

                                playProgressText.Text = errorMsg.ToUpper();
                                playProgress.Value = 100;
                                playProgress.ForeColor = Color.Red;

                                if (_nfswPid != 0) {
                                    try {
                                        Process.GetProcessById(_nfswPid).Kill();
                                    } catch { /* ignored */ }
                                }

                                _nfswstarted.Abort();

                                DialogResult restartApp = MessageBox.Show(null, errorMsg + "\nWould you like to restart the launcher?", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if(restartApp == DialogResult.Yes) {
                                    Properties.Settings.Default.IsRestarting = true;
                                    Properties.Settings.Default.Save();
                                    Application.Restart();
                                    Application.ExitThread();
                                }
                                this.closebtn_Click(null, null);
                            }));
                        }
                    };
                }
            //}
        }

        public void DownloadModNetFilesRightNow(string path)
        {
            while (isDownloadingModNetFiles == false)
            {
                CurrentModFileCount++;
                var url = modFilesDownloadUrls.Dequeue();
                string FileName = url.ToString().Substring(url.ToString().LastIndexOf("/") + 1, (url.ToString().Length - url.ToString().LastIndexOf("/") - 1));

                ModNetFileNameInUse = FileName;

                WebClientWithTimeout client2 = new WebClientWithTimeout();

                client2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged_RELOADED);
                client2.DownloadFileCompleted += (test, stuff) => {
                    Log.Debug("LAUNCHER: Downloaded: " + FileName);
                    isDownloadingModNetFiles = false;
                    if (modFilesDownloadUrls.Any() == false)
                    {
                        LaunchGame();
                    }
                    else
                    {
                        //Redownload other file
                        DownloadModNetFilesRightNow(path);
                    }
                };
                client2.DownloadFileAsync(url, path + "/" + FileName);
                isDownloadingModNetFiles = true;
            }
        }

        private void playButton_Click(object sender, EventArgs e) {
            if(UriScheme.ForceGame != true) { 
                if (_loggedIn == false) {
                    if(_useSavedPassword == false) return;
                    loginButton_Click(sender, e);
                }

                if (_playenabled == false) {
                    return;
                }
            } else {
                //set background black
                imageServerName.Text = "WebLogin";
                verticalBanner.Image = null;

                _userId = UriScheme.UserID;
                _loginToken = UriScheme.LoginToken;
                _serverIp = UriScheme.ServerIP;
            }

            _disableLogout = true;

            DisablePlayButton();

            if (!DetectLinux.LinuxDetected())
            {
                var installDir = _settingFile.Read("InstallationDirectory");
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

            var linksPath = Path.Combine(_settingFile.Read("InstallationDirectory"), ".links");
            if (File.Exists(linksPath))
            {
                CleanLinks(linksPath);
            }

            ModManager.ResetModDat(_settingFile.Read("InstallationDirectory"));

            if (!Directory.Exists(_settingFile.Read("InstallationDirectory") + "/modules")) Directory.CreateDirectory(_settingFile.Read("InstallationDirectory") + "/modules");
            if (!Directory.Exists(_settingFile.Read("InstallationDirectory") + "/scripts")) Directory.CreateDirectory(_settingFile.Read("InstallationDirectory") + "/scripts");
            String[] GlobalFiles            = new string[] { "dinput8.dll", "global.ini" };
            String[] ModNetReloadedFiles = new string[]
            {
                "7z.dll", 
                "fmt.dll", 
                "libcurl.dll", 
                "zlib1.dll", 
                "ModLoader.asi"
            };
            String[] ModNetLegacyFiles = new string[] { 
                "modules/udpcrc.soapbox.module", 
                "modules/udpcrypt1.soapbox.module", 
                "modules/udpcrypt2.soapbox.module", 
                "modules/xmppsubject.soapbox.module",
                "scripts/global.ini", 
                "lightfx.dll", 
                "ModManager.asi", 
                "global.ini",
            };

            String[] RemoveAllFiles = GlobalFiles.Concat(ModNetReloadedFiles).Concat(ModNetLegacyFiles).Concat(new[]
            {
                "PocoFoundation.dll",
                "PocoNet.dll",
            }).ToArray();

            foreach (string file in RemoveAllFiles) {
                if(File.Exists(Path.Combine(_settingFile.Read("InstallationDirectory"), file))) { 
                    try {
                        File.Delete(Path.Combine(_settingFile.Read("InstallationDirectory"), file));
                    } catch(Exception ex) {
                        MessageBox.Show($"File {file} cannot be deleted.\n{ex.Message}", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            playButton.BackgroundImage = Properties.Resources.playbutton;

            Log.Debug("LAUNCHER: Installing ModNet");
            playProgressText.Text = ("Detecting ModNetSupport for " + _realServernameBanner).ToUpper();
            String jsonModNet = ModNetReloaded.ModNetSupported(_serverIp);

            if (jsonModNet != String.Empty) {
                playProgressText.Text = "ModNetReloaded support detected, setting up...".ToUpper();

                try {
                    string[] newFiles = GlobalFiles.Concat(ModNetReloadedFiles).ToArray();
                    WebClientWithTimeout newModNetFilesDownload = new WebClientWithTimeout();
                    foreach (string file in newFiles) {
                        playProgressText.Text = ("Fetching ModNetReloaded Files: " + file).ToUpper();
                        Application.DoEvents();

                        newModNetFilesDownload.DownloadFile("https://cdn.soapboxrace.world/modules-v2/" + file, _settingFile.Read("InstallationDirectory") + "/" + file);
                    }

                    //get files now
                    MainJson json2 = JsonConvert.DeserializeObject<MainJson>(jsonModNet);


                    //metonator was here!
                    try
                    {
                        //Cached content goes here
                        EventList.remoteEvent = new WebClientWithTimeout().DownloadString(json2.basePath + "/events.json");
                        CarList.remoteCarList = new WebClientWithTimeout().DownloadString(json2.basePath + "/cars.json");
                    }
                    catch { }

                    //get new index
                    Uri newIndexFile = new Uri(json2.basePath + "/index.json");
                    String jsonindex = new WebClientWithTimeout().DownloadString(newIndexFile);

                    IndexJson json3 = JsonConvert.DeserializeObject<IndexJson>(jsonindex);

                    CountFilesTotal = json3.entries.Count;

                    String path = Path.Combine(_settingFile.Read("InstallationDirectory"), "MODS", MDFive.HashPassword(json2.serverID).ToLower());
                    if(!Directory.Exists(path)) Directory.CreateDirectory(path);

                    foreach (IndexJsonEntry modfile in json3.entries) {
                        if (SHA.HashFile(path + "/" + modfile.Name).ToLower() != modfile.Checksum) {
                            modFilesDownloadUrls.Enqueue(new Uri(json2.basePath + "/" + modfile.Name));
                            TotalModFileCount++;
                        }
                    }

                    if (modFilesDownloadUrls.Count != 0) {
                        this.DownloadModNetFilesRightNow(path);
                    } else {
                        LaunchGame();
                    }

                    foreach (var file in Directory.GetFiles(path)) {
                        var name = Path.GetFileName(file);

                        if (json3.entries.All(en => en.Name != name)) {
                            Log.Debug("LAUNCHER: removing package: " + file);
                            try { 
                                File.Delete(file);
                            } catch(Exception ex) {
                                Log.Error($"Failed to remove {file}: {ex.Message}");
                            }
                        }
                    }
                } catch(Exception ex) {
                    Log.Debug("LAUNCHER " + ex.Message);
                    MessageBox.Show(null, $"There was an error downloading ModNet Files:\n{ex.Message}", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                //Rofl
                LaunchGame();
            }        
        }

        private static readonly object LinkCleanerLock = new object();

        private void CleanLinks(string linksPath)
        {
            lock (LinkCleanerLock)
            {
                if (File.Exists(linksPath))
                {
                    string dir = _settingFile.Read("InstallationDirectory");
                    foreach (var readLine in File.ReadLines(linksPath))
                    {
                        var parts = readLine.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length != 2)
                        {
                            continue;
                        }

                        string loc = parts[0];
                        int type = int.Parse(parts[1]);
                        string realLoc = Path.Combine(dir, loc);
                        if (type == 0)
                        {
                            if (!File.Exists(realLoc))
                            {
                                throw new Exception(".links file includes nonexistent file: " + realLoc);
                            }

                            string origPath = realLoc + ".orig";

                            if (!File.Exists(origPath)) {
                                File.Delete(realLoc);
                                continue;
                            }

                            File.Delete(realLoc);
                            File.Move(origPath, realLoc);
                        }
                        else
                        {
                            if (!Directory.Exists(realLoc))
                            {
                                throw new Exception(".links file includes nonexistent directory: " + realLoc);
                            }
                            Directory.Delete(realLoc, true);
                        }
                    }

                    File.Delete(linksPath);
                }
            }
        }

        void client_DownloadProgressChanged_LEGACY(object sender, DownloadProgressChangedEventArgs e) {
            this.BeginInvoke((MethodInvoker)delegate {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                playProgressText.Text = ("Downloaded " + FormatFileSize(e.BytesReceived) + " of " + FormatFileSize(e.TotalBytesToReceive)).ToUpper();

                extractingProgress.Value = Convert.ToInt32(Decimal.Divide(CountFiles, CountFilesTotal) * 100);
                extractingProgress.Width = Convert.ToInt32(Decimal.Divide(CountFiles, CountFilesTotal) * 519);
            });
        }

        void client_DownloadProgressChanged_RELOADED(object sender, DownloadProgressChangedEventArgs e) {
            this.BeginInvoke((MethodInvoker)delegate {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                playProgressText.Text = ("["+CurrentModFileCount+" / "+TotalModFileCount+"] Downloading " + ModNetFileNameInUse + ": " + FormatFileSize(e.BytesReceived) + " of " + FormatFileSize(e.TotalBytesToReceive)).ToUpper();

                extractingProgress.Value = Convert.ToInt32(Decimal.Divide(e.BytesReceived, e.TotalBytesToReceive) * 100);
                extractingProgress.Width = Convert.ToInt32(Decimal.Divide(e.BytesReceived, e.TotalBytesToReceive) * 519);
            });
        }

        //Launch game
        public void LaunchGame() {
            if (_serverInfo.DiscordAppId != null) {
                discordRpcClient.Dispose();
                discordRpcClient = null;
                discordRpcClient = new DiscordRpcClient(_serverInfo.DiscordAppId);
                discordRpcClient.Initialize();
            }

            if ((_disableDiscordRPC == false) && ((ServerInfo)serverPick.SelectedItem).Category == "DEV") {
                discordRpcClient.Dispose();
                discordRpcClient = null;
            }

            try {
                if (
                    SHA.HashFile(_settingFile.Read("InstallationDirectory") + "/nfsw.exe") == "7C0D6EE08EB1EDA67D5E5087DDA3762182CDE4AC" ||
                    SHA.HashFile(_settingFile.Read("InstallationDirectory") + "/nfsw.exe") == "DB9287FB7B0CDA237A5C3885DD47A9FFDAEE1C19" ||
                    SHA.HashFile(_settingFile.Read("InstallationDirectory") + "/nfsw.exe") == "E69890D31919DE1649D319956560269DB88B8F22"
                ) {
                    ServerProxy.Instance.SetServerUrl(_serverIp);
                    ServerProxy.Instance.SetServerName(_realServername);

                    AntiCheat.user_id = _userId;
                    AntiCheat.serverip = new Uri(_serverIp).Host;

                    StartGame(_userId, _loginToken);

                    if (_builtinserver) {
                        playProgressText.Text = "Soapbox server launched. Waiting for queries.".ToUpper();
                    } else {
                        var secondsToCloseLauncher = 10;

                        extractingProgress.Value = 100;
                        extractingProgress.Width = 519;

                        while (secondsToCloseLauncher > 0) {
                            playProgressText.Text = string.Format("Loading game. Launcher will minimize in {0} seconds.", secondsToCloseLauncher).ToUpper(); //"LOADING GAME. LAUNCHER WILL MINIMIZE ITSELF IN " + secondsToCloseLauncher + " SECONDS";
                            Delay.WaitSeconds(1);
                            secondsToCloseLauncher--;
                        }

                        playProgressText.Text = "";

                        WindowState = FormWindowState.Minimized;
                        ShowInTaskbar = false;

                        ContextMenu = new ContextMenu();
                        ContextMenu.MenuItems.Add(new MenuItem("Donate", (b, n) => { Process.Start("http://paypal.me/metonator95"); }));
                        ContextMenu.MenuItems.Add("-");
                        ContextMenu.MenuItems.Add(new MenuItem("Add Server", addServer_Click));
                        ContextMenu.MenuItems.Add("-");
                        ContextMenu.MenuItems.Add(new MenuItem("Close Launcher", (sender2, e2) =>
                        {
                            MessageBox.Show(null, "Please close the game before closing launcher.", "Please close the game before closing launcher.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));

                        Update();
                        Refresh();

                        Notification.ContextMenu = ContextMenu;

                        Self.MapZoneRPC = "GameLauncherReborn v" + Application.ProductVersion;
                    }
                } else {
                    MessageBox.Show(null, "Your NFSW.exe is modified. Please re-download the game.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } catch (Exception ex) {
                MessageBox.Show(null, ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void playButton_MouseUp(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            playButton.BackgroundImage = Properties.Resources.playbutton_hover;
        }

        private void playButton_MouseDown(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            playButton.BackgroundImage = Properties.Resources.playbutton_click;
        }

        private void playButton_MouseEnter(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            playButton.BackgroundImage = Properties.Resources.playbutton_hover;
        }

        private void playButton_MouseLeave(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            playButton.BackgroundImage = Properties.Resources.playbutton;
        }

        private void LaunchNfsw()
        {
            playButton.BackgroundImage = Properties.Resources.playbutton;
            playButton.ForeColor = Color.Gray;

            playProgressText.Text = "Checking up all files".ToUpper();
            playProgress.Width = 0;
            extractingProgress.Width = 0;

            string speechFile;

            try
            {
                speechFile = string.IsNullOrEmpty(_settingFile.Read("Language")) ? "en" : _settingFile.Read("Language").ToLower();
            }
            catch (Exception)
            {
                speechFile = "en";
            }

            if (!File.Exists(_settingFile.Read("InstallationDirectory") + "/Sound/Speech/copspeechhdr_" + speechFile + ".big")) {
                playProgressText.Text = "Loading list of files to download...".ToUpper();

                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives) {
                    if (d.Name == Path.GetPathRoot(_settingFile.Read("InstallationDirectory"))) {
                        if (d.TotalFreeSpace <= 10000000000)  {

                            extractingProgress.Value = 100;
                            extractingProgress.Width = 519;
                            extractingProgress.Image = Properties.Resources.warningprogress;
                            extractingProgress.ProgressColor = Color.Orange;

                            playProgressText.Text = "Please make sure you have at least 10GB free space on hard drive.".ToUpper();

                            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Paused);
                            TaskbarProgress.SetValue(Handle, 100, 100);
                        }
                        else {
                            DownloadCoreFiles();
                        }
                    }
                }
            } else {
				OnDownloadFinished();
			}
		}

        public void DownloadCoreFiles()
        {
            playProgressText.Text = "Checking core files...".ToUpper();
            playProgress.Width = 0;
            extractingProgress.Width = 0;

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            //Guess who is Back - DavidCarbon
            if (File.Exists(filename_pack))
            {
                playProgressTextTimer.Visible = true;
                playProgressText.Text = "Local GameFiles sbrwpack Found In Launcher Folder".ToUpper();
                playProgressTextTimer.Text = "Loading".ToUpper() ;

                //GameFiles.sbrwpack
                localGameFiles();
            }
            else if (!File.Exists(_settingFile.Read("InstallationDirectory") + "/nfsw.exe"))
            {
                _downloadStartTime = DateTime.Now;
                _downloader.StartDownload(_NFSW_Installation_Source, "", _settingFile.Read("InstallationDirectory"), false, false, 1130632198);
            }
            else
            {
                DownloadTracksFiles();
            }
        }

        public void DownloadTracksFiles()
        {
            playProgressText.Text = "Checking track files...".ToUpper();
            playProgress.Width = 0;
            extractingProgress.Width = 0;

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            if (!File.Exists(_settingFile.Read("InstallationDirectory") + "/TracksHigh/STREAML5RA_98.BUN"))
            {
                _downloadStartTime = DateTime.Now;
                _downloader.StartDownload(_NFSW_Installation_Source, "TracksHigh", _settingFile.Read("InstallationDirectory"), false, false, 278397707);
            }
            else
            {
                DownloadSpeechFiles();
            }
        }

        public void DownloadSpeechFiles()
        {
            playProgressText.Text = "Looking for correct speech files...".ToUpper();
            playProgress.Width = 0;
            extractingProgress.Width = 0;

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            string speechFile;
            ulong speechSize;

            try
            {
                if (string.IsNullOrEmpty(_settingFile.Read("Language")))
                {
                    speechFile = "en";
                    speechSize = 141805935;
                    _langInfo = "ENGLISH";
                }
                else
                {
                    WebClientWithTimeout wc = new WebClientWithTimeout();
                    var response = wc.DownloadString(_NFSW_Installation_Source + "/" + _settingFile.Read("Language").ToLower() + "/index.xml");

                    response = response.Substring(3, response.Length - 3);

                    var speechFileXml = new XmlDocument();
                    speechFileXml.LoadXml(response);
                    var speechSizeNode = speechFileXml.SelectSingleNode("index/header/compressed");

                    speechFile = _settingFile.Read("Language").ToLower();
                    speechSize = Convert.ToUInt64(speechSizeNode.InnerText);
                    _langInfo = settingsLanguage.GetItemText(settingsLanguage.SelectedItem).ToUpper();
                }
            }
            catch (Exception)
            {
                speechFile = "en";
                speechSize = 141805935;
                _langInfo = "ENGLISH";
            }

            playProgressText.Text = string.Format("Checking for {0} speech files.", _langInfo).ToUpper();

            if (!File.Exists(_settingFile.Read("InstallationDirectory") + "\\Sound\\Speech\\copspeechsth_" + speechFile + ".big"))
            {
                _downloadStartTime = DateTime.Now;
                _downloader.StartDownload(_NFSW_Installation_Source, speechFile, _settingFile.Read("InstallationDirectory"), false, false, speechSize);
            }
            else
            {
                DownloadTracksHighFiles();
            }
        }

        public void DownloadTracksHighFiles()
        {
            playProgressText.Text = "Checking track (high) files.".ToUpper();
            playProgress.Width = 0;
            extractingProgress.Width = 0;

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            if (_settingFile.Read("TracksHigh") == "1" && !File.Exists(_settingFile.Read("InstallationDirectory") + "\\Tracks\\STREAML5RA_98.BUN"))
            {
                _downloadStartTime = DateTime.Now;
                _downloader.StartDownload(_NFSW_Installation_Source, "Tracks", _settingFile.Read("InstallationDirectory"), false, false, 615494528);
            }
            else
            {
                OnDownloadFinished();
            }
        }

        public bool DownloadMods(string serverKey)
        {
            try
            {
                playProgress.Width = 1;
                ModManager.Download(ModManager.GetMods(serverKey), _settingFile.Read("InstallationDirectory"), serverKey, playProgressText, extractingProgress);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + e.StackTrace);
                ModManager.ResetModDat(_settingFile.Read("InstallationDirectory"));
                return false;
            }
        }

        //Check Local GameFiles Hash
        private async void localGameFiles()
        {
            await Task.Delay(5000);
            if (SHA.HashFile("GameFiles.sbrwpack") == "B42E00939DC656C14BF5A05644080AD015522C8C")
            {
                TaskbarProgress.SetValue(Handle, 100, 100);
                playProgress.Value = 100;
                playProgress.Width = 519;

                GoForUnpack(filename_pack);
            }
        }

        //That's right the Protype Extractor from 2.1.5.x, now back from the dead - DavidCarbon
        public void GoForUnpack(string filename_pack)
        {
            //Thread.Sleep(1);

            Thread unpacker = new Thread(() => {
                this.BeginInvoke((MethodInvoker)delegate {
                    using (ZipArchive archive = ZipFile.OpenRead(filename_pack))
                    {
                        int numFiles = archive.Entries.Count;
                        int current = 1;

                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string fullName = entry.FullName;

                            extractingProgress.Value = (int)((long)100 * current / numFiles);
                            extractingProgress.Width = (int)((long)519 * current / numFiles);

                            TaskbarProgress.SetValue(Handle, (int)(100 * current / numFiles), 100);

                            if (!File.Exists(Path.Combine(_settingFile.Read("InstallationDirectory"), fullName.Replace(".sbrw", String.Empty))))
                            {
                                playProgressText.Text = ("Unpacking " + fullName.Replace(".sbrw", String.Empty)).ToUpper();
                                playProgressTextTimer.Text = "[" + current + " / " + archive.Entries.Count + "]";


                                if (fullName.Substring(fullName.Length - 1) == "/")
                                {
                                    //Is a directory, create it!
                                    string folderName = fullName.Remove(fullName.Length - 1);
                                    if (Directory.Exists(Path.Combine(_settingFile.Read("InstallationDirectory"), folderName)))
                                    {
                                        Directory.Delete(Path.Combine(_settingFile.Read("InstallationDirectory"), folderName), true);
                                    }

                                    Directory.CreateDirectory(Path.Combine(_settingFile.Read("InstallationDirectory"), folderName));
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

                                    FileStream fileStream = new FileStream(Path.Combine(_settingFile.Read("InstallationDirectory"), oldFileName), FileMode.Create);
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
                                playProgressText.Text = ("Skipping " + fullName).ToUpper();
                            }

                            _presence.State = "Unpacking game: " + (100 * current / numFiles) + "%";
                            discordRpcClient.SetPresence(_presence);

                            Application.DoEvents();

                            if (numFiles == current)
                            {
                                playProgressTextTimer.Visible = false;
                                playProgressTextTimer.Text = "";

                                _isDownloading = false;
                                OnDownloadFinished();

                                Notification.Visible = true;
                                Notification.BalloonTipIcon = ToolTipIcon.Info;
                                Notification.BalloonTipTitle = "GameLauncherReborn";
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

        private string FormatFileSize(long byteCount, bool si = true) {
            int unit = si ? 1000 : 1024;
            if (byteCount < unit) return byteCount + " B";
            int exp = (int)(Math.Log(byteCount) / Math.Log(unit));
            String pre = (si ? "kMGTPE" : "KMGTPE")[exp - 1] + (si ? "" : "i");
            return String.Format("{0}{1}B", Convert.ToDecimal(byteCount / Math.Pow(unit, exp)).ToString("0.##"), pre);
        }

        private string EstimateFinishTime(long current, long total) {
            try { 
                var num = current / (double)total;
                if (num < 0.00185484899838312) {
                    return "Calculating";
                }

                var now = DateTime.Now - _downloadStartTime;
                var timeSpan = TimeSpan.FromTicks((long)(now.Ticks / num)) - now;

                int rHours = Convert.ToInt32(timeSpan.Hours.ToString()) + 1;
                int rMinutes = Convert.ToInt32(timeSpan.Minutes.ToString()) + 1;
                int rSeconds = Convert.ToInt32(timeSpan.Seconds.ToString()) + 1;

                if (rHours > 1) return rHours.ToString() + " hours remaining";
                if (rMinutes > 1) return rMinutes.ToString() + " minutes remaining";
                if (rSeconds > 1) return rSeconds.ToString() + " seconds remaining";

                return "Just now";
            } catch {
                return "N/A";
            }
        }

        private void OnDownloadProgress(long downloadLength, long downloadCurrent, long compressedLength, string filename, int skiptime = 0)
        {
            if (downloadCurrent < compressedLength) {
                playProgressText.Text = String.Format("Downloading — {0} of {1} ({3}%) — {2}", FormatFileSize(downloadCurrent), FormatFileSize(compressedLength), EstimateFinishTime(downloadCurrent, compressedLength), (int)(100 * downloadCurrent / compressedLength)).ToUpper();
            }

            try {
                playProgress.Value = (int)(100 * downloadCurrent / compressedLength);
                playProgress.Width = (int)(519 * downloadCurrent / compressedLength);

                TaskbarProgress.SetValue(Handle, (int)(100 * downloadCurrent / compressedLength), 100);
            } catch {
                TaskbarProgress.SetValue(Handle, 0, 100);
                playProgress.Value = 0;
                playProgress.Width = 0;
            }

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);
        }

        private void OnDownloadFinished() {
            try {
                File.WriteAllBytes(_settingFile.Read("InstallationDirectory") + "/GFX/BootFlow.gfx", ExtractResource.AsByte("GameLauncher.SoapBoxModules.BootFlow.gfx"));
            } catch {
                // ignored
            }

            playProgressText.Text = "Ready!".ToUpper();
            _presence.State = "Ready!";
            if(discordRpcClient != null) discordRpcClient.SetPresence(_presence);

            EnablePlayButton();

            extractingProgress.Width = 519;

            TaskbarProgress.SetValue(Handle, 100, 100);
            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);

        }

        private void EnablePlayButton() {
            _isDownloading = false;
            _playenabled = true;

            extractingProgress.Value = 100;
            extractingProgress.Width = 519;

            playButton.BackgroundImage = Properties.Resources.playbutton;
            playButton.ForeColor = Color.White;
        }

        private void DisablePlayButton() {
            _isDownloading = false;
            _playenabled = false;

            playButton.Visible = false;
            logoutButton.Visible = false;

            extractingProgress.Value = 100;
            extractingProgress.Width = 519;

            playButton.BackgroundImage = Properties.Resources.graybutton;
            playButton.ForeColor = Color.White;
        }

        private void OnDownloadFailed(Exception ex)
        {
            string failureMessage;
            MessageBox.Show(null, "Failed to download gamefiles. Possible cause is that CDN went offline. Please select other CDN from Settings", "GameLauncher - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            try {
                failureMessage = ex.Message;
            } catch {
                failureMessage = "Download failed.";
            }

            extractingProgress.Value = 100;
            extractingProgress.Width = 519;
            extractingProgress.Image = Properties.Resources.errorprogress;
            extractingProgress.ProgressColor = Color.FromArgb(254,0,0);

            playProgressText.Text = failureMessage.ToUpper();

            TaskbarProgress.SetValue(Handle, 100, 100);
            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Error);
        }

		private void OnShowExtract(string filename, long currentCount, long allFilesCount) {
            if(playProgress.Value == 100)
                playProgressText.Text = String.Format("Extracting — {0} of {1} ({3}%) — {2}", FormatFileSize(currentCount), FormatFileSize(allFilesCount), EstimateFinishTime(currentCount, allFilesCount), (int)(100 * currentCount / allFilesCount)).ToUpper();
            
            extractingProgress.Value = (int)(100 * currentCount / allFilesCount);
            extractingProgress.Width = (int)(519 * currentCount / allFilesCount);
        }

        private void OnShowMessage(string message, string header)
        {
            MessageBox.Show(message, header);
        }

        public void ServerStatusBar(Pen color, Point startPoint, Point endPoint, int Thickness = 2) {
            Graphics _formGraphics = CreateGraphics();
            
            for (int x = 0; x <= Thickness; x++) {
                _formGraphics.DrawLine(color, new Point(startPoint.X, startPoint.Y-x), new Point(endPoint.X, endPoint.Y-x));
            }

            _formGraphics.Dispose();
        }

        //VerifyHash
        private void vfilesButton_Click(object sender, EventArgs e)
        {
            //In Development (Zacam got this)
        }

        private void SelectServerBtn_Click(object sender, EventArgs e) {
            new SelectServer().ShowDialog();

            if(ServerName != null) {
                this.SelectServerBtn.Text = "[...] " + ServerName.Name;

                var index = finalItems.FindIndex(i => string.Equals(i.IpAddress, ServerName.IpAddress));
                serverPick.SelectedIndex = index;
            }
        }
    }
    /* Moved 7 Unused Code to Gist */
    /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/00de505302fbf9f8cfea9b163a707d9f8f122552/MainScreen.cs */
}
