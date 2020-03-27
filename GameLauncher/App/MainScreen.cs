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
using static MeTonaTOR.MessageBox;

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

        private bool _disableChecks;

        private int _lastSelectedServerId;
        private int _nfswPid;
        private Thread _nfswstarted;


        private DateTime _downloadStartTime;
        private readonly Downloader _downloader;

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

        //VerifyHash
        string[][] scannedHashes;
        public int filesToScan;
        public int badFiles;
        public int totalFilesScanned;
        public int redownloadedCount;
        public List<string> invalidFileList = new List<string>();

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

            Log.Debug("Entered mainScreen");

            rnd = new Random(Environment.TickCount);

            discordRpcClient = new DiscordRpcClient(Self.DiscordRPCID, pipe: 0) {
                Logger = new DiscordRPC.Logging.ConsoleLogger(DiscordRPC.Logging.LogLevel.Trace, true)
            };

            discordRpcClient.OnReady += (sender, e) => {
                Log.Debug("Discord ready. Detected user: " + e.User.Username + ". Discord version: " + e.Version);
                Self.discordid = e.User.ID.ToString();
            };

            discordRpcClient.OnError += (sender, e) =>
            {
                MeTonaTOR.MessageBox.Show($"Discord Error\n{e.Message}", e.Code.ToString(), _MessageBoxButtons.OK, _MessageBoxIcon.Error);
            };

            discordRpcClient.Initialize();

            
            Log.Debug("Setting SSL Protocol");
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            
            if (DetectLinux.LinuxDetected()) {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }

            Log.Debug("Detecting OS");
            if (DetectLinux.LinuxDetected()) {
                _OS = DetectLinux.Distro();
            } else {
                _OS = Environment.OSVersion.VersionString;
            }

            Log.Debug("Detected OS: " + _OS);
            _downloader = new Downloader(this, 3, 2, 16) {
                ProgressUpdated = new ProgressUpdated(OnDownloadProgress),
                DownloadFinished = new DownloadFinished(DownloadTracksFiles),
                DownloadFailed = new DownloadFailed(OnDownloadFailed),
                ShowMessage = new ShowMessage(OnShowMessage),
				ShowExtract = new ShowExtract(OnShowExtract)
            };

            Log.Debug("InitializeComponent");
            InitializeComponent();

            Log.Debug("Applying Fonts");
            ApplyEmbeddedFonts();

            _disableChecks = (_settingFile.KeyExists("DisableVerifyHash") && _settingFile.Read("DisableVerifyHash") == "1") ? true : false;

            Log.Debug("Setting launcher location");
            if (_settingFile.KeyExists("LauncherPosX") || _settingFile.KeyExists("LauncherPosY")) {
                StartPosition = FormStartPosition.Manual;
                var posX = int.Parse(_settingFile.Read("LauncherPosX"));
                var posY = int.Parse(_settingFile.Read("LauncherPosY"));
                Location = new Point(posX, posY);
                Log.Debug("Launcher Location: " + posX + "x" + posY);
            } else {
                Log.Debug("Launcher Location: CenterScreen");
                Self.centerScreen(this);
            }

            Log.Debug("Disabling MaximizeBox");
            MaximizeBox = false;
            Log.Debug("Setting Styles");
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            Log.Debug("Applying EventHandlers");
            closebtn.MouseEnter += new EventHandler(closebtn_MouseEnter);
            closebtn.MouseLeave += new EventHandler(closebtn_MouseLeave);
            closebtn.Click += new EventHandler(closebtn_Click);

            settingsButton.MouseEnter += new EventHandler(settingsButton_MouseEnter);
            settingsButton.MouseLeave += new EventHandler(settingsButton_MouseLeave);
            settingsButton.Click += new EventHandler(settingsButton_Click);

            loginButton.MouseEnter += new EventHandler(loginButton_MouseEnter);
            loginButton.MouseLeave += new EventHandler(loginButton_MouseLeave);
            loginButton.Click += new EventHandler(loginButton_Click);
            loginButton.MouseUp += new MouseEventHandler(loginButton_MouseUp);
            loginButton.MouseDown += new MouseEventHandler(loginButton_MouseDown);

            registerButton.MouseEnter += registerButton_MouseEnter;
            registerButton.MouseLeave += registerButton_MouseLeave;
            registerButton.MouseUp += registerButton_MouseUp;
            registerButton.MouseDown += registerButton_MouseDown;
            registerButton.Click += registerButton_Click;

            registerCancel.Click += registerCancel_Click;
            registerCancel.MouseEnter += registerCancel_MouseEnter;
            registerCancel.MouseLeave += registerCancel_MouseLeave;
            registerCancel.MouseUp += registerCancel_MouseUp;
            registerCancel.MouseDown += registerCancel_MouseDown;

            logoutButton.Click += logoutButton_Click;
            logoutButton.MouseEnter += logoutButton_MouseEnter;
            logoutButton.MouseLeave += logoutButton_MouseLeave;
            logoutButton.MouseUp += logoutButton_MouseUp;
            logoutButton.MouseDown += logoutButton_MouseDown;

            settingsSave.MouseEnter += new EventHandler(settingsSave_MouseEnter);
            settingsSave.MouseLeave += new EventHandler(settingsSave_MouseLeave);
            settingsSave.MouseUp += new MouseEventHandler(settingsSave_MouseUp);
            settingsSave.MouseDown += new MouseEventHandler(settingsSave_MouseDown);
            settingsSave.Click += new EventHandler(settingsSave_Click);

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

            MouseDown += new MouseEventHandler(moveWindow_MouseDown);
            MouseMove += new MouseEventHandler(moveWindow_MouseMove);
            MouseUp += new MouseEventHandler(moveWindow_MouseUp);

            logo.MouseDown += new MouseEventHandler(moveWindow_MouseDown);
            logo.MouseMove += new MouseEventHandler(moveWindow_MouseMove);
            logo.MouseUp += new MouseEventHandler(moveWindow_MouseUp);

            playButton.MouseEnter += new EventHandler(playButton_MouseEnter);
            playButton.MouseLeave += new EventHandler(playButton_MouseLeave);
            playButton.Click += new EventHandler(playButton_Click);
            playButton.MouseUp += new MouseEventHandler(playButton_MouseUp);
            playButton.MouseDown += new MouseEventHandler(playButton_MouseDown);

            registerText.Click += new EventHandler(registerText_LinkClicked);

            this.Load += new EventHandler(mainScreen_Load);
            this.Shown += (x,y) => {
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


            Log.Debug("Checking permissions");
            if (!Self.hasWriteAccessToFolder(Directory.GetCurrentDirectory())) {
                Log.Error("Check Permission Failed.");
                MeTonaTOR.MessageBox.Show(null, "Failed to write the test file. Make sure you're running the launcher with administrative privileges.", "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Error);
            }

            Log.Debug("Checking InstallationDirectory");
            if (string.IsNullOrEmpty(_settingFile.Read("InstallationDirectory"))) {
                Log.Debug("First run!");

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

                var fbd = new CommonOpenFileDialog
                {
                    EnsurePathExists = true,
                    EnsureFileExists = false,
                    AllowNonFileSystemItems = false,
                    Title = "Select The Folder With NFS World Instalation",
                    IsFolderPicker = true
                };

                if (fbd.ShowDialog() == CommonFileDialogResult.Ok) {
                    if (!Self.hasWriteAccessToFolder(fbd.FileName)) {
                        Log.Error("Not enough permissions. Exiting.");
                        MeTonaTOR.MessageBox.Show(null, "You don't have enough permission to select this path as installation folder. Please select another directory.", "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Information);
                        Environment.Exit(Environment.ExitCode);
                    }

                    if (fbd.DefaultFileName == Environment.CurrentDirectory) {
                        Directory.CreateDirectory("GameFiles");
                        Log.Debug("Installing NFSW in same directory where the launcher resides is disadvised.");
                        MeTonaTOR.MessageBox.Show(null, string.Format("Installing NFSW in same directory where the launcher resides is disadvised. Instead, we will install it on {0}.", Environment.CurrentDirectory + "\\GameFiles"), "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Information);
                        _settingFile.Write("InstallationDirectory", Environment.CurrentDirectory + "\\GameFiles");
                    } else {
                        Log.Debug("Directory Set: " + fbd.FileName);
                        _settingFile.Write("InstallationDirectory", fbd.FileName);
                    }
                } else {
                    Log.Debug("Exiting");
                    Environment.Exit(Environment.ExitCode);
                }
                fbd.Dispose();
            }

            /*if (!DetectLinux.UnixDetected()) {
                Log.Debug("Setting cursor.");
                string temporaryFile = Path.GetTempFileName();
                File.WriteAllBytes(temporaryFile, ExtractResource.AsByte("GameLauncher.SoapBoxModules.cursor.ani"));
                Cursor mycursor = new Cursor(Cursor.Current.Handle);
                IntPtr colorcursorhandle = User32.LoadCursorFromFile(temporaryFile);
                mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, null, mycursor, new object[] { colorcursorhandle });
                Cursor = mycursor;
                File.Delete(temporaryFile);
            }*/

            Log.Debug("Doing magic with imageServerName");
            var pos = PointToScreen(imageServerName.Location);
            pos = verticalBanner.PointToClient(pos);
            imageServerName.Parent = verticalBanner;
            imageServerName.Location = pos;
            imageServerName.BackColor = Color.Transparent;

            Log.Debug("Setting ServerStatusBar");
            ServerStatusBar(_colorLoading, _startPoint, _endPoint);

            Log.Debug("Loading ModManager Cache");
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
            Log.Debug("Entering mainScreen_Load");

            Log.Debug("Updating server list");
            ServerListUpdater.UpdateList();

            //INFO: this is here because this dll is necessary for downloading game files and I want to make it async.
            if (!File.Exists("LZMA.dll")) {
                Log.Debug("Starting LZMA downloader");
                try {
                    playProgressText.Text = "Downloading LZMA.dll...";
                    using (WebClient wc = new WebClient()) {
                        wc.DownloadFileAsync(new Uri(Self.mainserver + "/files/LZMA.dll"), "LZMA.dll");
                    }
                } catch (Exception ex) {
                    Log.Debug("Failed to download LZMA. " + ex.Message);
                }
            }

            Log.Debug("Setting WindowName");
            Text = "GameLauncherReborn v" + Application.ProductVersion;

            Log.Debug("Checking window location");
            if (Location.X >= Screen.PrimaryScreen.Bounds.Width || Location.Y >= Screen.PrimaryScreen.Bounds.Height || Location.X <= 0 || Location.Y <= 0) {
                Log.Debug("Window location restored to centerScreen");

                Self.centerScreen(this);
                _windowMoved = true;
            }

            _NFSW_Installation_Source = !string.IsNullOrEmpty(_settingFile.Read("CDN")) ? _settingFile.Read("CDN") : "http://cdn.worldunited.gg/gamefiles/packed/";
            Log.Debug("_NFSW_Installation_Source is now " + _NFSW_Installation_Source);

            Log.Debug("Applyinng ContextMenu");
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
            if (!string.IsNullOrEmpty(_settingFile.Read("AccountEmail")) && !string.IsNullOrEmpty(_settingFile.Read("Password"))) {
                Log.Debug("Restoring last saved email and password");
                rememberMe.Checked = true;
            }

            serverPick.DisplayMember = "Name";

            Log.Debug("Setting server list");
            finalItems = ServerListUpdater.GetList();
            serverPick.DataSource = finalItems;

            Log.Debug("SERVERLIST: Checking...");
                Log.Debug("SERVERLIST: Setting first server in list");
                try {
                    serverPick.SelectedIndex = 1;
                    Log.Debug("SERVERLIST: Selected 1");
                } catch (Exception ex) {
                    Log.Debug("SERVERLIST: " + ex.Message);
                }

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
            

            Log.Debug("Checking for password");
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

            Log.Debug("Setting game language");

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
            Log.Debug("Setting texture quality");

            settingsQuality.DisplayMember = "Text";
            settingsQuality.ValueMember = "Value";

            var quality = new[] {
                new { Text = "Standard", Value = "0" },
                new { Text = "Maximum", Value = "1" },
            };

            settingsQuality.DataSource = quality;

            Task.Run(() => {
                String _slresponse2 = string.Empty;
                try {
                    WebClientWithTimeout wc = new WebClientWithTimeout();
                    _slresponse2 = wc.DownloadString(Self.CDNUrlList);
                } catch(Exception error) {
                    MeTonaTOR.MessageBox.Show(error.Message, "An error occurred while loading CDN List");
                    _slresponse2 = JsonConvert.SerializeObject(new[] {
                        new CDNObject { name = "[CF] WorldUnited.gg Mirror", url = "http://cdn.worldunited.gg/gamefiles/packed/" }
                    });
                }

                List<CDNObject> CDNList = JsonConvert.DeserializeObject<List<CDNObject>>(_slresponse2);

                cdnPick.Invoke(new Action(() => 
                {
                    cdnPick.DisplayMember = "name";
                    cdnPick.DataSource = CDNList;
                }));
            });

            if (_settingFile.KeyExists("TracksHigh"))
            {
                string selectedTracksHigh = _settingFile.Read("TracksHigh");
                if(!string.IsNullOrEmpty(selectedTracksHigh))
                {
                    settingsQuality.SelectedValue = selectedTracksHigh;
                } else
                {
                    settingsQuality.SelectedIndex = 0;
                }
            } else
            {
                settingsQuality.SelectedIndex = 0;
            }

            Log.Debug("Re-checking InstallationDirectory");

            var drive = Path.GetPathRoot(_settingFile.Read("InstallationDirectory"));
            if (!Directory.Exists(drive)) {
                if (!string.IsNullOrEmpty(drive)) {
                    var newdir = Directory.GetCurrentDirectory() + "\\GameFiles";
                    _settingFile.Write("InstallationDirectory", newdir);
                    Log.Debug(string.Format("Drive {0} was not found. Your actual installation directory is set to {1} now.", drive, newdir));

                    MeTonaTOR.MessageBox.Show(null, string.Format("Drive {0} was not found. Your actual installation directory is set to {1} now.", drive, newdir), "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Error);
                }
            }

            vfilesCheck.Checked = _disableChecks;

            Log.Debug("Hiding RegisterFormElements"); RegisterFormElements(false);
            Log.Debug("Hiding SettingsFormElements"); SettingsFormElements(false);
            Log.Debug("Hiding LoggedInFormElements"); LoggedInFormElements(false);

            Log.Debug("Showing LoginFormElements"); LoginFormElements(true);

            Log.Debug("Setting Registry Options");
            try {
                var gameInstallDirValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "GameInstallDir", RegistryValueKind.String).ToString();
                if (gameInstallDirValue != Path.GetFullPath(_settingFile.Read("InstallationDirectory"))) {
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

            Log.Debug("Setting configurations");
            _newGameFilesPath = Path.GetFullPath(_settingFile.Read("InstallationDirectory"));
            settingsGameFilesCurrent.Text = "CURRENT DIRECTORY: " + _newGameFilesPath;

            Log.Debug("Initializing DiscordRPC");

            _presence.State = _OS;
            _presence.Details = "In-Launcher: " + Application.ProductVersion;
            _presence.Assets = new Assets
            {
                LargeImageText = "SBRW",
                LargeImageKey = "nfsw"
            };
            discordRpcClient.SetPresence(_presence);

            BeginInvoke((MethodInvoker)delegate {
                Log.Debug("Initialize Downloading Process");
                LaunchNfsw();
            });

            this.BringToFront();

            if(!DetectLinux.LinuxDetected()) {
                Log.Debug("Checking for update");
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
                    wordFilterCheck.Checked = File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords") ? false : true;
                } catch {
                    wordFilterCheck.Checked = false;
                }
            } else {
                wordFilterCheck.Enabled = false;
            }
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

            Process[] allOfThem = Process.GetProcessesByName("nfsw");
            foreach (var oneProcess in allOfThem) {
                Process.GetProcessById(oneProcess.Id).Kill();
            }

            //Kill DiscordRPC
            discordRpcClient.Dispose();

            ServerProxy.Instance.Stop();

            File.WriteAllLines("invalidfiles.dat", invalidFileList);

            Notification.Dispose();

            var linksPath = Path.Combine(_settingFile.Read("InstallationDirectory"), ".links");
            if (File.Exists(linksPath))
            {
                CleanLinks(linksPath);
            }

            Process[] allOfThem2 = Process.GetProcessesByName("GameLauncher");
            foreach (var oneProcess in allOfThem2) {
                Process.GetProcessById(oneProcess.Id).Kill();
            }

            Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
        }

        private void addServer_Click(object sender, EventArgs e)
        {
            Form x = new AddServer();
            x.Show();
        }

        private void OpenDebugWindow(object sender, EventArgs e)
        {
            if (!(serverPick.SelectedItem is ServerInfo server)) return;

            var form = new DebugWindow(server.IpAddress, server.Name);
            form.Show();
        }

        private void OpenMapHandler(object sender, EventArgs e)
        {
            if (!(serverPick.SelectedItem is ServerInfo server)) return;

            var form = new ShowMap(server.IpAddress, _realServername);

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
                MeTonaTOR.MessageBox.Show(null, "Please wait while launcher is still downloading gamefiles.", "GameLauncher", _MessageBoxButtons.OK);
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
            } else {
                _settingFile.DeleteKey("AccountEmail");
                _settingFile.DeleteKey("Password");
            }

            if (String.IsNullOrEmpty(Tokens.Error)) {
                _loggedIn = true;
                _userId = Tokens.UserId;
                _loginToken = Tokens.LoginToken;
                _serverIp = Tokens.IPAddress;

                if(!String.IsNullOrEmpty(Tokens.Warning)) {
                    MeTonaTOR.MessageBox.Show(null, Tokens.Warning, "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Warning);
                }

                BackgroundImage = Properties.Resources.loggedbg;
                LoginFormElements(false);
                LoggedInFormElements(true);
            } else {
                MeTonaTOR.MessageBox.Show(null, Tokens.Error, "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Error);
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
            imageServerName.Text = _serverInfo.Name;
            _loginEnabled = false;

            ServerStatusText.Text = "Server Status - Pinging";
            ServerStatusText.ForeColor = Color.FromArgb(66, 179, 189);
            ServerStatusDesc.Text = "";

            loginButton.ForeColor = Color.Gray;
            password.Text = "";
            var verticalImageUrl = "";
            verticalBanner.Image = null;
            verticalBanner.BackColor = Color.Transparent;

            var serverIp = _serverInfo.IpAddress;
            string numPlayers;

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

                        /*if (!string.IsNullOrEmpty(json.allowedCountries)) {
                            var countries = new List<object>();
                            var splitted = json.allowedCountries.Split(';');

                            foreach (var splitter in splitted)
                            {
                                countries.Add(Self.CountryName(splitter));
                            }

                            var allowed = string.Join(", ", countries);

                            allowedCountriesLabel.Text = string.Format("Warning, this server only accepts players from: {0}", allowed);
                        } else {
                            allowedCountriesLabel.Text = "";
                        }*/

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
                    } catch {
                        //¯\_(ツ)_/¯
                    }

                    //for thread safety
                    if (this.ServerStatusDesc.InvokeRequired)
                    {
                        ServerStatusDesc.Invoke(new Action(delegate ()
                        {
                            ServerStatusDesc.Text = string.Format("players in game - {0}", numPlayers);
                        }));
                    }
                    else
                    {
                        this.ServerStatusDesc.Text = string.Format("players in game - {0}", numPlayers);
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
                                    Image image;
                                    var memoryStream = new MemoryStream(e4.Result);
                                    image = Image.FromStream(memoryStream);
                                    verticalBanner.Image = image;
                                    verticalBanner.BackColor = Color.Black;

                                    imageServerName.Text = _realServernameBanner;
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
            Log.Debug("Getting AirportCyr");            FontFamily AirportCyr = FontWrapper.Instance.GetFontFamily("Airport-Cyr.ttf");
            Log.Debug("Getting AkrobatSemiBold");       FontFamily AkrobatSemiBold = FontWrapper.Instance.GetFontFamily("Akrobat-SemiBold.ttf");
            Log.Debug("Getting AkrobatRegular");        FontFamily AkrobatRegular = FontWrapper.Instance.GetFontFamily("Akrobat-Regular.ttf");
                
            Log.Debug("Applying AkrobatRegular mainScreen to launcherStatusText");          launcherStatusText.Font = new Font(AkrobatRegular, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            Log.Debug("Applying AirportCyr mainScreen to launcherStatusText");              launcherStatusDesc.Font = new Font(AirportCyr, 7f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);

            Log.Debug("Applying AkrobatRegular mainScreen to ServerStatusText");            ServerStatusText.Font = new Font(AkrobatRegular, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            Log.Debug("Applying AirportCyr mainScreen to ServerStatusDesc");                ServerStatusDesc.Font = new Font(AirportCyr, 7f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            Log.Debug("Applying AkrobatSemiBold mainScreen to playProgressText");           playProgressText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            Log.Debug("Applying AkrobatRegular mainScreen to email");                       email.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            Log.Debug("Applying AkrobatSemiBold mainScreen to loginButton");                loginButton.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatRegular mainScreen to password");                    password.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            Log.Debug("Applying AkrobatSemiBold mainScreen to rememberMe");                 rememberMe.Font = new Font(AkrobatSemiBold, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to forgotPassword");             forgotPassword.Font = new Font(AkrobatSemiBold, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to playProgressText");           playProgressText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to playButton");                 playButton.Font = new Font(AkrobatSemiBold, 15f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to currentWindowInfo");          currentWindowInfo.Font = new Font(AkrobatSemiBold, 11f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to imageServerName");            imageServerName.Font = new Font(AkrobatSemiBold, 25f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            Log.Debug("Applying AkrobatSemiBold mainScreen to registerAgree");              registerAgree.Font = new Font(AkrobatSemiBold, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to registerCancel");             registerCancel.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            Log.Debug("Applying AkrobatSemiBold mainScreen to settingsLanguageText");       settingsLanguageText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to settingsQualityText");        settingsQualityText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to settingsSave");               settingsSave.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            Log.Debug("Applying AkrobatSemiBold mainScreen to settingsGamePathText");       settingsGamePathText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to wordFilterCheck");            wordFilterCheck.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to vFilesCheck");                vfilesCheck.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to settingsGameFilesCurrent");   settingsGameFilesCurrent.Font = new Font(AkrobatSemiBold, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            Log.Debug("Applying AkrobatSemiBold mainScreen to logoutButton");               logoutButton.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            Log.Debug("Applying AkrobatRegular mainScreen to registerEmail");               registerEmail.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            Log.Debug("Applying AkrobatRegular mainScreen to registerPassword");            registerPassword.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            Log.Debug("Applying AkrobatRegular mainScreen to registerConfirmPassword");     registerConfirmPassword.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            Log.Debug("Applying AkrobatRegular mainScreen to registerTicket");              registerTicket.Font = new Font(AkrobatRegular, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);

            Log.Debug("Applying AkrobatSemiBold mainScreen to registerButton");             registerButton.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to registerButton");             registerButton.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            Log.Debug("Applying AkrobatSemiBold mainScreen to registerText");               registerText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            Log.Debug("Applying cdnText mainScreen to settingsGamePathText");               cdnText.Font = new Font(AkrobatSemiBold, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
        }

        private void registerText_LinkClicked(object sender, EventArgs e)
        {
            if (_allowRegistration) {
                BackgroundImage = (_ticketRequired) ? Properties.Resources.register_ticket : Properties.Resources.register_noticket;
                currentWindowInfo.Text = "REGISTER ON " + _realServername.ToUpper() + ":";
                LoginFormElements(false);
                RegisterFormElements(true);
            } else {
                MeTonaTOR.MessageBox.Show(null, "Server seems to be offline.", "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Error);
            }
        }

        private void forgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
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

                MeTonaTOR.MessageBox.Show(null, responseString, "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Information);
            }

        }

        private void LoggedInFormElements(bool hideElements)
        {
            if (hideElements)
            {
                currentWindowInfo.Text = string.Format("Welcome back, {0}!", email.Text).ToUpper();
            }

            logoutButton.Visible = hideElements;
            playProgress.Visible = hideElements;
            extractingProgress.Visible = hideElements;
            playProgressText.Visible = hideElements;
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
        }

        private void LoginFormElements(bool hideElements = false)
        {
            if (hideElements)
            {
                currentWindowInfo.Text = "Enter your account information to Log In:".ToUpper();
            }

            rememberMe.Visible = hideElements;
            loginButton.Visible = hideElements;
            ServerStatusText.Visible = hideElements;
            ServerStatusIcon.Visible = hideElements;
            ServerStatusDesc.Visible = hideElements;
            launcherIconStatus.Visible = hideElements;
            launcherStatusDesc.Visible = hideElements;
            launcherStatusText.Visible = hideElements;
            registerText.Visible = hideElements;
            serverPick.Visible = hideElements;
            email.Visible = hideElements;
            password.Visible = hideElements;
            forgotPassword.Visible = hideElements;
            settingsButton.Visible = hideElements;
            verticalBanner.Visible = hideElements;
            playProgressText.Visible = hideElements;
            playProgress.Visible = hideElements;
            extractingProgress.Visible = hideElements;
            addServer.Visible = hideElements;
            //allowedCountriesLabel.Visible = hideElements;
            serverPick.Enabled = true;
            randomServer.Visible = hideElements;
            randomServer.Enabled = true;
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

            addServer.Visible = hideElements;
            serverPick.Visible = hideElements;
            serverPick.Enabled = false;

            randomServer.Visible = hideElements;
            randomServer.Enabled = false;

            // Reset fields
            registerEmail.Text = "";
            registerPassword.Text = "";
            registerConfirmPassword.Text = "";
            registerAgree.Checked = false;
        }

        private void logoutButton_Click(object sender, EventArgs e) {
            var reply = MeTonaTOR.MessageBox.Show(null, string.Format("Are you sure you want to log out from {0}?", serverPick.GetItemText(serverPick.SelectedItem)), "GameLauncher", _MessageBoxButtons.YesNo, _MessageBoxIcon.Warning);
            if (reply == TaskDialogResult.Yes) {
                BackgroundImage = Properties.Resources.loginbg;
                _loggedIn = false;
                LoggedInFormElements(false);
                LoginFormElements(true);

                _userId = String.Empty;
                _loginToken = String.Empty;
            }
        }

        private void logoutButton_MouseDown(object sender, EventArgs e)
        {
            logoutButton.Image = Properties.Resources.graybutton_click;
        }

        private void logoutButton_MouseEnter(object sender, EventArgs e)
        {
            logoutButton.Image = Properties.Resources.graybutton_hover;
        }

        private void logoutButton_MouseLeave(object sender, EventArgs e)
        {
            logoutButton.Image = Properties.Resources.graybutton;
        }

        private void logoutButton_MouseUp(object sender, EventArgs e)
        {
            logoutButton.Image = Properties.Resources.graybutton_hover;
        }

        private void registerButton_MouseEnter(object sender, EventArgs e)
        {
            registerButton.Image = Properties.Resources.greenbutton_hover;
        }

        private void registerButton_MouseLeave(object sender, EventArgs e)
        {
            registerButton.Image = Properties.Resources.greenbutton;
        }

        private void registerButton_MouseUp(object sender, EventArgs e)
        {
            registerButton.Image = Properties.Resources.greenbutton_hover;
        }

        private void registerButton_MouseDown(object sender, EventArgs e)
        {
            registerButton.Image = Properties.Resources.greenbutton_click;
        }

        private void registerCancel_Click(object sender, EventArgs e)
        {
            BackgroundImage = Properties.Resources.loginbg;
            currentWindowInfo.Text = "Enter your account information to Log In:".ToUpper();
            RegisterFormElements(false);
            LoginFormElements(true);
        }

        private void registerCancel_MouseDown(object sender, EventArgs e)
        {
            registerCancel.Image = Properties.Resources.graybutton_click;
        }

        private void registerCancel_MouseEnter(object sender, EventArgs e)
        {
            registerCancel.Image = Properties.Resources.graybutton_hover;
        }

        private void registerCancel_MouseLeave(object sender, EventArgs e)
        {
            registerCancel.Image = Properties.Resources.graybutton;
        }

        private void registerCancel_MouseUp(object sender, EventArgs e)
        {
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
                            var passwordCheckReply = MeTonaTOR.MessageBox.Show(null, "Password used for registration has been breached " + Convert.ToInt32(splitChecks[1])+ " times, you should consider using different one.\r\nAlternatively you can use unsafe password anyway. Use it?", "GameLauncher", _MessageBoxButtons.YesNo, _MessageBoxIcon.Warning);
                            if(passwordCheckReply == TaskDialogResult.Yes) {
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

                        MeTonaTOR.MessageBox.Show(null, Tokens.Success, "GameLauncher", "Registration:", _MessageBoxButtons.OK, _MessageBoxIcon.Warning);

                        BackgroundImage = Properties.Resources.loginbg;

                        RegisterFormElements(false);
                        LoginFormElements(true);

                        _loggedIn = true;
                    } else {
                        MeTonaTOR.MessageBox.Show(null, Tokens.Error, "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Error);
                    }


                } else {
                    var message = "There were some errors while registering, please fix them:\n\n";

                    foreach (var error in registerErrors) {
                        message += "• " + error + "\n";
                    }

                    MeTonaTOR.MessageBox.Show(null, message, "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Error);
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
        }

        private void settingsButton_MouseEnter(object sender, EventArgs e) {
            settingsButton.BackgroundImage = Properties.Resources.settingsbtn_hover;
        }

        private void settingsButton_MouseLeave(object sender, EventArgs e) {
            settingsButton.BackgroundImage = Properties.Resources.settingsbtn;
        }

        private void settingsSave_MouseEnter(object sender, EventArgs e) {
            settingsSave.Image = Properties.Resources.greenbutton_hover;
        }

        private void settingsSave_MouseLeave(object sender, EventArgs e) {
            settingsSave.Image = Properties.Resources.greenbutton;
        }

        private void settingsSave_MouseUp(object sender, EventArgs e) {
            settingsSave.Image = Properties.Resources.greenbutton_hover;
        }

        private void settingsSave_MouseDown(object sender, EventArgs e) {
            settingsSave.Image = Properties.Resources.greenbutton_click;
        }

        private void settingsSave_Click(object sender, EventArgs e) {

            //TODO null check
            _settingFile.Write("Language", settingsLanguage.SelectedValue.ToString());
            _settingFile.Write("TracksHigh", settingsQuality.SelectedValue.ToString());
            _settingFile.Write("CDN", ((CDNObject)cdnPick.SelectedItem).url);

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

                        //Disable chat for linux users.
                        if(!DetectLinux.LinuxDetected()) {
                            var persistentValue = setting.AppendChild(userSettingsXml.CreateElement("PersistentValue"));
                            var chat = persistentValue.AppendChild(userSettingsXml.CreateElement("Chat"));
                            chat.InnerXml = "<DefaultChatGroup Type=\"string\">" + settingsLanguage.SelectedValue + "</DefaultChatGroup>";
                        }

                        ui.InnerXml = "<Language Type=\"string\">" + settingsLanguage.SelectedValue + "</Language>";

                        var directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(_userSettings));
                    }
                } else {
                    try {
                        var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
                        var ui = setting.AppendChild(userSettingsXml.CreateElement("UI"));

                        //Disable chat for linux users.
                        if (!DetectLinux.LinuxDetected()) {
                            var persistentValue = setting.AppendChild(userSettingsXml.CreateElement("PersistentValue"));
                            var chat = persistentValue.AppendChild(userSettingsXml.CreateElement("Chat"));
                            chat.InnerXml = "<DefaultChatGroup Type=\"string\">" + settingsLanguage.SelectedValue + "</DefaultChatGroup>";
                        }

                        ui.InnerXml = "<Language Type=\"string\">" + settingsLanguage.SelectedValue + "</Language>";

                        var directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(_userSettings));
                    } catch (Exception ex) {
                        MeTonaTOR.MessageBox.Show(null, "There was an error saving your settings to actual file. Restoring default.\n" + ex.Message, "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Error);
                        File.Delete(_userSettings);
                    }
                }
            } catch(Exception ex) {
                MeTonaTOR.MessageBox.Show(null, "There was an error saving your settings to actual file. Restoring default.\n" + ex.Message, "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Error);
                File.Delete(_userSettings);
            }

            userSettingsXml.Save(_userSettings);

            if (_settingFile.Read("InstallationDirectory") != _newGameFilesPath) {
                _settingFile.Write("InstallationDirectory", _newGameFilesPath);
                _restartRequired = true;
            }

            String disableCheck = (vfilesCheck.Checked == true) ? "1" : "0";

            if (_settingFile.Read("DisableVerifyHash") != disableCheck) {
                _settingFile.Write("DisableVerifyHash", (vfilesCheck.Checked == true) ? "1" : "0");
                _restartRequired = true;
            }


            if (_restartRequired) {
                MeTonaTOR.MessageBox.Show(null, "In order to see settings changes, you need to restart launcher manually.", "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Information);
            }

            //Delete/Enable profwords filter here
            if (wordFilterCheck.Checked) {
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
        }

        private void settingsGameFiles_Click(object sender, EventArgs e)
        {
            var fbd2 = new FolderBrowserDialog();
            var result2 = fbd2.ShowDialog();

            if (result2 == DialogResult.OK)
            {
                _newGameFilesPath = Path.GetFullPath(fbd2.SelectedPath);
                settingsGameFilesCurrent.Text = "NEW DIRECTORY: " + _newGameFilesPath;
            }
        }

        private void settingsGameFilesCurrent_Click(object sender, EventArgs e) {
            Process.Start(_newGameFilesPath);
        }

        private void SettingsFormElements(bool hideElements = true) {
            if (hideElements) {
                currentWindowInfo.Text = "";
            }

            settingsSave.Visible = hideElements;
            settingsLanguage.Visible = hideElements;
            settingsLanguageText.Visible = hideElements;
            settingsQuality.Visible = hideElements;
            settingsQualityText.Visible = hideElements;
            cdnPick.Visible = hideElements;
            cdnText.Visible = hideElements;
            settingsGameFiles.Visible = hideElements;
            settingsGameFilesCurrent.Visible = hideElements;
            settingsGamePathText.Visible = hideElements;
            wordFilterCheck.Visible = hideElements;
            vfilesCheck.Visible = hideElements;
        }

        private void StartGame(string userId, string loginToken) {
            _nfswstarted = new Thread(() => {
                LaunchGame(userId, loginToken, "http://127.0.0.1:" + Self.ProxyPort + "/nfsw/Engine.svc", this);
            }) { IsBackground = true };

            _nfswstarted.Start();

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
            discordRpcClient.SetPresence(_presence);
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

            //if(!DetectLinux.LinuxDetected()) { 
                if (Environment.ProcessorCount >= 4) {
                    nfswProcess.ProcessorAffinity = (IntPtr)0x000F;
                }

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

                                if (exitCode == 1)              errorMsg = "You just killed nfsw.exe via Task Manager";
                                if (exitCode == 2137)           errorMsg = "Launcher killed your game to prevent SpeedBugging.";

                                if (exitCode == -3)             errorMsg = "Server were unable to resolve your request";
                                if (exitCode == -4)             errorMsg = "Another instance is already executed";
                                if (exitCode == -5)             errorMsg = "DirectX Device was not found. Please install GPU Drivers before playing";
                                if (exitCode == -6)             errorMsg = "Server was unable to resolve your request";

                                playProgressText.Text = errorMsg.ToUpper();
                                playProgress.Value = 100;
                                playProgress.ForeColor = Color.Red;

                                if (_nfswPid != 0) {
                                    try {
                                        Process.GetProcessById(_nfswPid).Kill();
                                    } catch { /* ignored */ }
                                }

                                _nfswstarted.Abort();

                                MeTonaTOR.MessageBox.Show(null, errorMsg, "GameLauncher", "An error occurred with NFSW Executable", _MessageBoxButtons.OK, _MessageBoxIcon.Warning);
                                this.closebtn_Click(null, null);
                            }));
                        }
                    };
                }
            //}
        }

        private void playButton_Click(object sender, EventArgs e) {
            if (_loggedIn == false) {
                if(_useSavedPassword == false) return;
                loginButton_Click(sender, e);
            }

            if (_playenabled == false) {
                return;
            }

            if (!DetectLinux.LinuxDetected())
            {
                if (!RedistributablePackage.IsInstalled(RedistributablePackageVersion.VC2015to2019x86))
                {
                    var result = MessageBox.Show(
                        "You do not have the 2015-2019 VC++ Redistributable Package installed. Click OK to install it.",
                        "Compatibility",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.OK)
                    {
                        MessageBox.Show("The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    var wc = new WebClientWithTimeout();
                    wc.DownloadFile("https://aka.ms/vs/16/release/VC_redist.x86.exe", "VC_redist.x86.exe");
                    var proc = Process.Start(new ProcessStartInfo
                    {
                        Verb = "runas",
                        FileName = "VC_redist.x86.exe"
                    });

                    if (proc == null)
                    {
                        MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    proc.EnableRaisingEvents = true;
                    proc.WaitForExit();
                    if (proc.ExitCode != 0)
                    {
                        MessageBox.Show("Package installation failed. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }

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

            //if (DetectLinux.LinuxDetected()) {
            //Temporarely disable any modnet for linux.
            //LaunchGame();
            //return;
            //}

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
                    } catch {
                        MeTonaTOR.MessageBox.Show($"File {file} cannot be deleted.", "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Warning);
                    }
                }
            }

            playButton.BackgroundImage = Properties.Resources.playbutton;

            Log.Debug("Installing ModNet");
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

                        if(DetectLinux.LinuxDetected()) {
                            newModNetFilesDownload.DownloadFile("http://cdn.soapboxrace.world/modules-v2/" + file, _settingFile.Read("InstallationDirectory") + "/" + file);
                        } else {
                            newModNetFilesDownload.DownloadFile("https://cdn.soapboxrace.world/modules-v2/" + file, _settingFile.Read("InstallationDirectory") + "/" + file);
                        }
                    }

                    try  {
                        if (DetectLinux.LinuxDetected()) {
                            newModNetFilesDownload.DownloadFile("http://cdn.mtntr.pl/legacy_modnet/global.ini", _settingFile.Read("InstallationDirectory") + "/global.ini");
                        } else {
                            newModNetFilesDownload.DownloadFile("https://cdn.mtntr.pl/legacy_modnet/global.ini", _settingFile.Read("InstallationDirectory") + "/global.ini");
                        }
                    } catch { }

                    //get files now
                    MainJson json2 = JsonConvert.DeserializeObject<MainJson>(jsonModNet);

                    //get new index
                    if(DetectLinux.LinuxDetected()) json2.basePath = json2.basePath.Replace("https://", "http://");
                    Uri newIndexFile = new Uri(json2.basePath + "/index.json");
                    String jsonindex = new WebClientWithTimeout().DownloadString(newIndexFile);

                    IndexJson json3 = JsonConvert.DeserializeObject<IndexJson>(jsonindex);

                    CountFilesTotal = json3.entries.Count;

                    String path = Path.Combine(_settingFile.Read("InstallationDirectory"), "MODS", MDFive.HashPassword(json2.serverID).ToLower());
                    if(!Directory.Exists(path)) Directory.CreateDirectory(path);

                    foreach (IndexJsonEntry modfile in json3.entries) {
                        if (SHA.HashFile(path + "/" + modfile.Name).ToLower() != modfile.Checksum) {
                            WebClientWithTimeout client2 = new WebClientWithTimeout();
                            client2.DownloadFileAsync(new Uri(json2.basePath + "/" + modfile.Name), path + "/" + modfile.Name);

                            client2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                            client2.DownloadFileCompleted += (test, stuff) => { 
                                //if(SHA.HashFile(path + "/" + modfile.Name).ToLower() == modfile.Checksum) {
                                    CountFiles++;

                                    if(CountFiles == CountFilesTotal) {
                                        LaunchGame();
                                    }
                                //} else {
                                //    File.Delete(path + "/" + modfile.Name);
                                //    Console.WriteLine(modfile.Name + " must be removed.");
                                //    playButton_Click(sender, e);
                                //}
                            };
                        } else {
                            CountFiles++;

                            if (CountFiles == CountFilesTotal) {
                                LaunchGame();
                            }
                        }
                    }
                } catch(Exception ex) {
                    MeTonaTOR.MessageBox.Show(ex.Message);
                }
            } else {
                string[] newFiles = GlobalFiles.Concat(ModNetLegacyFiles).ToArray();
                WebClientWithTimeout newModNetFilesDownload = new WebClientWithTimeout();
                foreach (string file in newFiles) {
                    playProgressText.Text = ("Fetching ModNetLegacy Files: " + file).ToUpper();
                    Application.DoEvents();
                    if (DetectLinux.LinuxDetected()) {
                        newModNetFilesDownload.DownloadFile("http://cdn.mtntr.pl/legacy_modnet/" + file, _settingFile.Read("InstallationDirectory") + "/" + file);
                    } else {
                        newModNetFilesDownload.DownloadFile("https://cdn.mtntr.pl/legacy_modnet/" + file, _settingFile.Read("InstallationDirectory") + "/" + file);
                    }
                }

                if (json.modsUrl != null) {
                    playProgressText.Text = "Electron support detected, checking mods...".ToUpper();

                    if (DetectLinux.LinuxDetected()) json.modsUrl = json.modsUrl.Replace("https://", "http://");
                    Uri newIndexFile = new Uri(json.modsUrl + "/index.json");
                    String jsonindex = new WebClientWithTimeout().DownloadString(newIndexFile);
                    List<ElectronIndex> json3 = JsonConvert.DeserializeObject<List<ElectronIndex>>(jsonindex);

                    CountFilesTotal = json3.Count;

                    String electronpath = (new Uri(_serverIp).Host).Replace(".", "-");
                    String path = Path.Combine(_settingFile.Read("InstallationDirectory"), "MODS", electronpath);
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    File.WriteAllText(path + ".json", jsonindex);

                    //Lets save modmanager.dat file first.

                    using (var fs = new FileStream(Path.Combine(_settingFile.Read("InstallationDirectory"), "ModManager.dat"), FileMode.Create))
                    using (var bw = new BinaryWriter(fs)) {
                        bw.Write(json3.Count);

                        foreach (ElectronIndex file in json3) {
                            var originalPath = Path.Combine(_settingFile.Read("InstallationDirectory"), file.file).Replace("/", "\\").ToUpper();
                            var modPath = Path.Combine(path, file.file).Replace("/", "\\").ToUpper();

                            bw.Write(originalPath.Length);
                            bw.Write(originalPath.ToCharArray());
                            bw.Write(modPath.Length);
                            bw.Write(modPath.ToCharArray());
                        }
                    }

                    foreach (ElectronIndex modfile in json3) {
                        String directorycreate = Path.GetDirectoryName(path + "/" + modfile.file);
                        Directory.CreateDirectory(directorycreate);

                        if (ElectronModNet.calculateHash(path + "/" + modfile.file) != modfile.hash) {
                            WebClientWithTimeout client2 = new WebClientWithTimeout();
                            client2.DownloadFileAsync(new Uri(json.modsUrl + "/" + modfile.file), path + "/" + modfile.file);

                            client2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                            client2.DownloadFileCompleted += (test, stuff) => {
                                if (ElectronModNet.calculateHash(path + "/" + modfile.file) == modfile.hash) {
                                    CountFiles++;

                                    if (CountFiles == CountFilesTotal) {
                                        LaunchGame();
                                    }
                                } else {
                                    File.Delete(path + "/" + modfile.file);
                                    playButton_Click(sender, e);
                                }
                            };
                        } else {
                            CountFiles++;

                            if (CountFiles == CountFilesTotal) {
                                LaunchGame();
                            }
                        }
                    }

                } else if(json.rwacallow == true) {
                    playProgressText.Text = "RWAC support detected, checking mods...".ToUpper();

                    //First lets assume new path for RWAC Mods
                    String rwacpath = MDFive.HashPassword(new Uri(_serverIp).Host);
                    if (DetectLinux.LinuxDetected()) json.homePageUrl = json.homePageUrl.Replace("https://", "http://");

                    String path = Path.Combine(_settingFile.Read("InstallationDirectory"), "MODS", rwacpath);

                    //Then, lets fetch its XML File
                    Uri rwac_wev2 = new Uri(json.homePageUrl + "/rwac/fileschecker_sbrw.xml");
                    String getcontent = new WebClient().DownloadString(rwac_wev2);

                    playProgressText.Text = "Got files for RWAC... downloading...".ToUpper();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(getcontent);
                    var nodes = xmlDoc.SelectNodes("rwac/files/file");

                    CountFilesTotal = nodes.Count;

                    //ModManager.dat
                    using (var fs = new FileStream(Path.Combine(_settingFile.Read("InstallationDirectory"), "ModManager.dat"), FileMode.Create))
                    using (var bw = new BinaryWriter(fs)) {
                        bw.Write(nodes.Count);

                        foreach (XmlNode files in nodes) {
                            string realfilepath = Path.Combine(files.Attributes["path"].Value, files.Attributes["name"].Value);

                            var originalPath = Path.Combine(_settingFile.Read("InstallationDirectory"), realfilepath).Replace("/", "\\").ToUpper();
                            var modPath = Path.Combine(path, realfilepath).Replace("/", "\\").ToUpper();

                            bw.Write(originalPath.Length);
                            bw.Write(originalPath.ToCharArray());
                            bw.Write(modPath.Length);
                            bw.Write(modPath.ToCharArray());
                        }
                    }

                        foreach (XmlNode files in nodes) {
                            //if(!(files.Attributes["name"].Value).Contains(".dll") && !(files.Attributes["name"].Value).Contains(".exe")) {
                                string realfilepath = Path.Combine(files.Attributes["path"].Value, files.Attributes["name"].Value);
                                String directorycreate = Path.GetDirectoryName(path + "/" + realfilepath);

                                Directory.CreateDirectory(directorycreate);
                                if(files.Attributes["download"].Value != String.Empty) { 
                                    if (MDFive.HashFile(path + "/" + realfilepath).ToLower() != files.InnerText) {
                                        WebClientWithTimeout client2 = new WebClientWithTimeout();
                                        client2.DownloadFileAsync(new Uri(files.Attributes["download"].Value), path + "/" + realfilepath);

                                        client2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                        client2.DownloadFileCompleted += (test, stuff) => {
                                            if (MDFive.HashFile(path + "/" + realfilepath).ToLower() == files.InnerText) {
                                                CountFiles++;

                                                if (CountFiles == CountFilesTotal) {
                                                    LaunchGame(); 
                                                }
                                            } else {
                                                String xddd = "Corrupted file found: " + realfilepath;
                                                xddd += "\nGot: " + MDFive.HashFile(path + "/" + realfilepath);
                                                xddd += "\nExpected: " + files.InnerText;

                                                MeTonaTOR.MessageBox.Show(xddd);
                                                File.Delete(path + "/" + realfilepath);
                                                playButton_Click(sender, e);
                                            }
                                        };
                                    } else {
                                        CountFiles++;

                                        if (CountFiles == CountFilesTotal) {
                                            LaunchGame();
                                        }
                                    }
                                } else {
                                    CountFiles++;

                                    if (CountFiles == CountFilesTotal) {
                                        LaunchGame();
                                    }
                                }
                            //} else {
                            //    CountFiles++;

                            //    if (CountFiles == CountFilesTotal) {
                            //        LaunchGame();
                            //    }
                            //}
                    }
                } else { 
                    playProgressText.Text = "LegacyModNet support detected, checking mods...".ToUpper();

                    if (_serverInfo.DistributionUrl != "" && _serverInfo.Id != "nfsw") {
                        DownloadMods(_serverInfo.Id);
                    }

                    LaunchGame();
                }
            }         
        }

        private void CleanLinks(string linksPath)
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

                    if (!File.Exists(origPath))
                    {
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

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            this.BeginInvoke((MethodInvoker)delegate {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                playProgressText.Text = ("Downloaded " + FormatFileSize(e.BytesReceived) + " of " + FormatFileSize(e.TotalBytesToReceive)).ToUpper();

                extractingProgress.Value = Convert.ToInt32(Decimal.Divide(CountFiles, CountFilesTotal) * 100);
                extractingProgress.Width = Convert.ToInt32(Decimal.Divide(CountFiles, CountFilesTotal) * 519);
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
                        var secondsToCloseLauncher = 5;

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
                            MeTonaTOR.MessageBox.Show(null, "Please close the game before closing launcher.", "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Information);
                        }));

                        Update();
                        Refresh();

                        Notification.ContextMenu = ContextMenu;
                    }
                } else {
                    MeTonaTOR.MessageBox.Show(null, "Your NFSW.exe is modified. Please re-download the game.", "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Error);
                }
            } catch (Exception ex) {
                MeTonaTOR.MessageBox.Show(null, ex.Message, "GameLauncher", _MessageBoxButtons.OK, _MessageBoxIcon.Error);
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
                        if (d.TotalFreeSpace <= 4000000000)  {

                            extractingProgress.Value = 100;
                            extractingProgress.Width = 519;
                            extractingProgress.Image = Properties.Resources.warningprogress;
                            extractingProgress.ProgressColor = Color.Orange;

                            playProgressText.Text = "Please make sure you have at least 4GB free space on hard drive.".ToUpper();

                            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Paused);
                            TaskbarProgress.SetValue(Handle, 100, 100);
                        } else {
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

            if (!File.Exists(_settingFile.Read("InstallationDirectory") + "/nfsw.exe"))
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
                MeTonaTOR.MessageBox.Show(e.Message + e.StackTrace);
                ModManager.ResetModDat(_settingFile.Read("InstallationDirectory"));
                return false;
            }
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

            if (_disableChecks != true) { 
                if(File.Exists("invalidfiles.dat")) {
                    playProgressText.Text = "RE-DOWNLOADING INVALID FILES".ToUpper();

                    string[] files = File.ReadAllLines("invalidfiles.dat");

                    foreach(string text in files) {
                        try { 
                            string text2 = _settingFile.Read("InstallationDirectory") + text;

                            string address = "http://cdn.mtntr.pl/gamefiles/unpacked" + text.Replace("\\", "/");

                            if (File.Exists(text2 + ".vhbak")) {
                                File.Delete(text2 + ".vhbak");
                            }
                            File.Move(text2, text2 + ".vhbak");
                            new WebClient().DownloadFile(address, text2);
                            Application.DoEvents();
                        } catch { }
                    }                
                }
            } else {
                playProgressText.Text = "Download Completed".ToUpper();
            }

            EnablePlayButton();

            extractingProgress.Width = 519;

            TaskbarProgress.SetValue(Handle, 100, 100);
            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);

            if(_disableChecks != true) {
                    playProgressText.Text = "Validating files on background.".ToUpper();

                    //Threaded CheckFiles
                    var thread = new Thread(() => {
                    String[] getFilesToCheck = null;
                    try { 
                        getFilesToCheck = new WebClientWithTimeout().DownloadString("http://cdn.mtntr.pl/gamefiles/checksums.dat").Split('\n');

                        scannedHashes = new string[getFilesToCheck.Length][];
                        for (var i = 0; i < getFilesToCheck.Length; i++) {
                            scannedHashes[i] = getFilesToCheck[i].Split(' ');
                        }

                        filesToScan = scannedHashes.Length;
                        totalFilesScanned = 0;
                        redownloadedCount = 0;

                        Directory.EnumerateFiles(_settingFile.Read("InstallationDirectory"), "*.*", SearchOption.AllDirectories).AsParallel().ForAll((file) => {
                            for (var i = 0; i < scannedHashes.Length; i++) {
                                if (scannedHashes[i][1].Trim() == file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim()) {
                                    if (scannedHashes[i][0].Trim() != SHA.HashFile(file).Trim()) {
                                        invalidFileList.Add(file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim());

                                        Notification.Visible = true;
                                        Notification.BalloonTipIcon = ToolTipIcon.Info;
                                        Notification.BalloonTipTitle = "GameLauncherReborn";
                                        Notification.BalloonTipText = "Invalid file found: [GAMEDIR]" + file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim();
                                        Notification.ShowBalloonTip(5000);
                                        Notification.Dispose();
                                    }
                                }
                            }

                            totalFilesScanned++;
                        });

                        playProgressText.Text = "Download Completed.".ToUpper();

                    } catch (Exception) { }
                }){ IsBackground = true };

                thread.Start();
            } else {
                playProgressText.Text = "Download Completed.".ToUpper();
            }
            //End CheckFiles
        }

        private void EnablePlayButton() {
            _isDownloading = false;
            _playenabled = true;

            extractingProgress.Value = 100;
            extractingProgress.Width = 519;

            playButton.BackgroundImage = Properties.Resources.playbutton;
            playButton.ForeColor = Color.White;
        }

        private void OnDownloadFailed(Exception ex)
        {
            string failureMessage;
            MeTonaTOR.MessageBox.Show(null, "Failed to download gamefiles. Possible cause is that CDN went offline. Please select other CDN from Settings", "GameLauncher - Error", _MessageBoxButtons.OK, _MessageBoxIcon.Error);

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
            MeTonaTOR.MessageBox.Show(message, header);
        }

        public void ServerStatusBar(Pen color, Point startPoint, Point endPoint, int Thickness = 2) {
            Graphics _formGraphics = CreateGraphics();
            
            for (int x = 0; x <= Thickness; x++) {
                _formGraphics.DrawLine(color, new Point(startPoint.X, startPoint.Y-x), new Point(endPoint.X, endPoint.Y-x));
            }

            _formGraphics.Dispose();
        }

        int rememberit;
        private void randomServer_Click(object sender, EventArgs e) {
            try {
                int total = (finalItems.Count)-(finalItems.FindAll(i => string.Equals(i.IsSpecial, true)).Count); //Prevent summing GROUPS
                int randomizer = random.Next(total);

                if (finalItems[total].IsSpecial == true) //Prevent picking GROUP as random server
                    randomizer = random.Next(total);

                if (rememberit == randomizer) //Prevent picking same ID as current one
                    randomizer = random.Next(total);

                serverPick.SelectedIndex = randomizer;
                rememberit = randomizer;
            } catch {
                serverPick.SelectedIndex = 2;
            }
        }

        private void srvinfo_Click(object sender, EventArgs e) {
            //new SrvInfo().Show();
        }
    }
}
