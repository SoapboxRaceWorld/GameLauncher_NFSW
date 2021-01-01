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
using System.Linq;
using System.Text.RegularExpressions;
using GameLauncher.App.Classes.Logger;
using System.IO.Compression;
using GameLauncher.App.Classes.Auth;
using DiscordRPC;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading.Tasks;
using GameLauncher.App.Classes.ModNetReloaded;
using GameLauncher.App.Classes.HashPassword;
using GameLauncher.App.Classes.RPC;
using GameLauncher.App.Classes.GPU;
using GameLauncher.Properties;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using System.Management.Automation;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using WindowsFirewallHelper;
using GameLauncher.App.Classes.InsiderKit;

namespace GameLauncher
{
    public sealed partial class MainScreen : Form
    {
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
        public bool _disableProxy;
        public bool _disableDiscordRPC;

        private int _lastSelectedServerId;
        private int _lastSelectedCdnId;
        private int _nfswPid;
        private Thread _nfswstarted;

        private DateTime _downloadStartTime;
        private readonly Downloader _downloader;

        private string _serverWebsiteLink = "";
        private string _serverFacebookLink = "";
        private string _serverDiscordLink = "";
        private string _serverTwitterLink = "";
        private string _loginWelcomeTime = "";
        private string _loginToken = "";
        private string _userId = "";
        private string _serverIp = "";
        private string _langInfo;
        private string _newLauncherPath;
        private string _newGameFilesPath;

        private readonly RichPresence _presence = new RichPresence();

        //private readonly Pen _colorOffline = new Pen(Color.FromArgb(128, 0, 0));
        //private readonly Pen _colorOnline = new Pen(Color.FromArgb(0, 128, 0));
        //private readonly Pen _colorLoading = new Pen(Color.FromArgb(0, 0, 0));

        private readonly IniFile _settingFile = new IniFile("Settings.ini");
        private readonly string _userSettings = Environment.GetEnvironmentVariable("AppData") + "/Need for Speed World/Settings/UserSettings.xml";
        private string _presenceImageKey;
        private string _NFSW_Installation_Source;
        private string _realServername;
        private string _realServernameBanner;
        public string _OS;

        public static String ModNetFileNameInUse = String.Empty;
        readonly Queue<Uri> modFilesDownloadUrls = new Queue<Uri>();
        bool isDownloadingModNetFiles = false;
        int CurrentModFileCount = 0;
        int TotalModFileCount = 0;

        //private Point _startPoint = new Point(28, 308);
        //private Point _endPoint = new Point(549, 308);

        ServerInfo _serverInfo = null;
        GetServerInformation json = new GetServerInformation();

        public static DiscordRpcClient discordRpcClient;

        List<ServerInfo> finalItems = new List<ServerInfo>();
        List<CDNObject> finalCDNItems = new List<CDNObject>();
        readonly Dictionary<string, int> serverStatusDictionary = new Dictionary<string, int>();

        readonly String filename_pack = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameFiles.sbrwpack");

        //UltimateLauncherFunction: SelectServer
        private static ServerInfo _ServerList;
        public static ServerInfo ServerName
        {
            get { return _ServerList; }
            set { _ServerList = value; }
        }

        public static Random random = new Random();
        //Log launcherLog = new Log("launcher.log");

        public static string RandomString(int length)
        {
            const string chars = "qwertyuiopasdfghjklzxcvbnm1234567890_";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

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
            _windowMoved = true;
            Opacity = 0.9;
        }

        public MainScreen()
        {
            ParseUri uri = new ParseUri(Environment.GetCommandLineArgs());

            if (uri.IsDiscordPresent()) 
            {
                Notification.Visible = true;
                Notification.BalloonTipIcon = ToolTipIcon.Info;
                Notification.BalloonTipTitle = "GameLauncherReborn";
                Notification.BalloonTipText = "Discord features are not yet completed.";
                Notification.ShowBalloonTip(5000);
                Notification.Dispose();
            }

            Log.Visuals("CORE: Entered mainScreen");

            Random rnd;
            rnd = new Random(Environment.TickCount);

            discordRpcClient = new DiscordRpcClient(Self.DiscordRPCID);

            discordRpcClient.OnReady += (sender, e) =>
            {
                Log.Debug("DISCORD: Discord ready. Detected user: " + e.User.Username + ". Discord version: " + e.Version);
                Self.discordid = e.User.ID.ToString();
            };

            discordRpcClient.OnError += (sender, e) =>
            {
                Log.Error($"DISCORD: Discord Error\n{e.Message}");
            };

            discordRpcClient.Initialize();

            Log.Debug("CORE: Setting SSL Protocol");
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            if (DetectLinux.LinuxDetected()) 
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }

            Log.Info("LAUNCHER: Detecting OS");
            if (DetectLinux.LinuxDetected())
            {
                _OS = DetectLinux.Distro();
                Log.System("SYSTEM: Detected OS: " + _OS);
            } 
            else
            {
                _OS = (string)Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("productName");
                Log.System("SYSTEM: Detected OS: " + _OS);
                if (Environment.Is64BitOperatingSystem == true)
                {
                    Log.Debug("SYSTEM: OS Type: 64 Bit");
                }
                Log.System("SYSTEM: OS Details: " + Environment.OSVersion);
                Log.System("SYSTEM: Video Card: " + GPUHelper.CardName());
                Log.System("SYSTEM: Driver Version: " + GPUHelper.DriverVersion());
            }

            _downloader = new Downloader(this, 3, 2, 16)
            {
                ProgressUpdated = new ProgressUpdated(OnDownloadProgress),
                DownloadFinished = new DownloadFinished(DownloadTracksFiles),
                DownloadFailed = new DownloadFailed(OnDownloadFailed),
                ShowMessage = new ShowMessage(OnShowMessage),
                ShowExtract = new ShowExtract(OnShowExtract)
            };

            Log.Visuals("CORE: InitializeComponent");
            InitializeComponent();

            Log.Visuals("CORE: Applying Fonts");
            ApplyEmbeddedFonts();

            _disableProxy = (_settingFile.KeyExists("DisableProxy") && _settingFile.Read("DisableProxy") == "1");
            _disableDiscordRPC = (_settingFile.KeyExists("DisableRPC") && _settingFile.Read("DisableRPC") == "1");
            Log.Debug("PROXY: Checking if Proxy Is Disabled from User Settings! It's value is " + _disableProxy);

            Self.CenterScreen(this);

            Log.Visuals("CORE: Disabling MaximizeBox");
            MaximizeBox = false;
            Log.Visuals("CORE: Setting Styles");
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            Log.Visuals("CORE: Applying EventHandlers");
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

            RegisterButton.MouseEnter += Greenbutton_hover_MouseEnter;
            RegisterButton.MouseLeave += Greenbutton_MouseLeave;
            RegisterButton.MouseUp += Greenbutton_hover_MouseUp;
            RegisterButton.MouseDown += Greenbutton_click_MouseDown;
            RegisterButton.Click += RegisterButton_Click;

            RegisterCancel.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            RegisterCancel.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            RegisterCancel.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            RegisterCancel.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);
            RegisterCancel.Click += new EventHandler(RegisterCancel_Click);

            LogoutButton.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            LogoutButton.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            LogoutButton.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            LogoutButton.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);
            LogoutButton.Click += new EventHandler(LogoutButton_Click);

            SettingsSave.MouseEnter += new EventHandler(Greenbutton_hover_MouseEnter);
            SettingsSave.MouseLeave += new EventHandler(Greenbutton_MouseLeave);
            SettingsSave.MouseUp += new MouseEventHandler(Greenbutton_hover_MouseUp);
            SettingsSave.MouseDown += new MouseEventHandler(Greenbutton_click_MouseDown);
            SettingsSave.Click += new EventHandler(SettingsSave_Click);

            SettingsCancel.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            SettingsCancel.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            SettingsCancel.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            SettingsCancel.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);
            SettingsCancel.Click += new EventHandler(SettingsCancel_Click);

            SettingsLauncherPathCurrent.Click += new EventHandler(SettingsLauncherPathCurrent_Click);
            SettingsGameFiles.Click += new EventHandler(SettingsGameFiles_Click);
            SettingsGameFilesCurrent.Click += new EventHandler(SettingsGameFilesCurrent_Click);

            AddServer.Click += new EventHandler(AddServer_Click);
            SettingsLauncherVersion.Click += new EventHandler(OpenDebugWindow);

            MainEmail.KeyUp += new KeyEventHandler(Loginbuttonenabler);
            MainEmail.KeyDown += new KeyEventHandler(LoginEnter);
            MainPassword.KeyUp += new KeyEventHandler(Loginbuttonenabler);
            MainPassword.KeyDown += new KeyEventHandler(LoginEnter);

            ServerPick.SelectedIndexChanged += new EventHandler(ServerPick_SelectedIndexChanged);
            ServerPick.DrawItem += new DrawItemEventHandler(ComboBox1_DrawItem);
            SettingsCDNPick.DrawItem += new DrawItemEventHandler(SettingsCDNPick_DrawItem);

            ForgotPassword.LinkClicked += new LinkLabelLinkClickedEventHandler(ForgotPassword_LinkClicked);

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
            RegisterText.Click += new EventHandler(RegisterText_LinkClicked);

            this.Load += new EventHandler(MainScreen_Load);

            this.Shown += (x,y) =>
            {
                if (UriScheme.ForceGame == true)
                {
                    PlayButton_Click(x, y);
                }

                new Thread(() =>
                {
                    discordRpcClient.Invoke();

                    //Let's fetch all servers
                    List<ServerInfo> allServs = finalItems.FindAll(i => string.Equals(i.IsSpecial, false));
                    allServs.ForEach(delegate(ServerInfo server) {
                        try
                        {
                            WebClient pingServer = new WebClient();
                            pingServer.DownloadString(server.IpAddress + "/GetServerInformation");

                            if (!serverStatusDictionary.ContainsKey(server.Id))
                            {
                                serverStatusDictionary.Add(server.Id, 1);
                            }
                        }
                        catch
                        {
                            if (!serverStatusDictionary.ContainsKey(server.Id))
                            {
                                serverStatusDictionary.Add(server.Id, 0);
                            }
                        }
                    });
                }).Start();
            };

            Log.Core("CORE: Checking permissions");
            if (!Self.HasWriteAccessToFolder(Directory.GetCurrentDirectory())) 
            {
                Log.Error("CORE: Check Permission Failed.");
                MessageBox.Show(null, "Failed to write the test file. Make sure you're running the launcher with administrative privileges.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Log.Core("LAUNCHER: Checking InstallationDirectory: " + _settingFile.Read("InstallationDirectory"));
            if (string.IsNullOrEmpty(_settingFile.Read("InstallationDirectory"))) 
            {
                Log.Core("LAUNCHER: First run!");

                try
                {
                    Form welcome = new WelcomeScreen();
                    DialogResult welcomereply = welcome.ShowDialog();

                    if (welcomereply != DialogResult.OK) 
                    {
                        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                    }
                    else
                    {
                        _settingFile.Write("CDN", CDN.CDNUrl);
                        _NFSW_Installation_Source = CDN.CDNUrl;
                    }
                }
                catch
                {
                    Log.Warning("LAUNCHER: CDN Source URL was Empty! Setting a Null Safe URL 'http://localhost'");
                    _settingFile.Write("CDN", "http://localhost");
                    _NFSW_Installation_Source = "http://localhost";
                    Log.Core("LAUNCHER: Installation Directory was Empty! Creating and Setting Directory at " + AppDomain.CurrentDomain.BaseDirectory + "\\Game Files");
                    _settingFile.Write("InstallationDirectory", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files");
                }

                var fbd = new CommonOpenFileDialog 
                {
                    EnsurePathExists = true,
                    EnsureFileExists = false,
                    AllowNonFileSystemItems = false,
                    Title = "Select the location to Find or Download NFS:W",
                    IsFolderPicker = true
                };

                if (fbd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (!Self.HasWriteAccessToFolder(fbd.FileName))
                    {
                        Log.Error("LAUNCHER: Not enough permissions. Exiting.");
                        MessageBox.Show(null, "You don't have enough permission to select this path as installation folder. Please select another directory.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Environment.Exit(Environment.ExitCode);
                    }

                    if (fbd.FileName == AppDomain.CurrentDomain.BaseDirectory)
                    {
                        Directory.CreateDirectory("Game Files");
                        Log.Warning("LAUNCHER: Installing NFSW in same directory where the launcher resides is disadvised.");
                        MessageBox.Show(null, string.Format("Installing NFSW in same directory where the launcher resides is disadvised. Instead, we will install it on {0}.", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _settingFile.Write("InstallationDirectory", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files");
                    }
                    else
                    {
                        Log.Core("LAUNCHER: Directory Set: " + fbd.FileName);
                        _settingFile.Write("InstallationDirectory", fbd.FileName);
                    }
                } 
                else
                {
                    Log.Core("LAUNCHER: Exiting");
                    Environment.Exit(Environment.ExitCode);
                }
                fbd.Dispose();
            }
 
            if (!_settingFile.KeyExists("CDN") || string.IsNullOrEmpty(_settingFile.Read("CDN")))
            {
                _settingFile.Write("CDN", "http://localhost");
                _NFSW_Installation_Source = "http://localhost";
                Log.Warning("LAUNCHER: CDN Source URL was Empty! Setting a Null Safe URL 'http://localhost'");
            }

            if (!DetectLinux.LinuxDetected())
            {
                CheckGameFilesDirectoryPrevention();

                Log.Visuals("CORE: Setting cursor.");
                string temporaryFile = Path.GetTempFileName();
                File.WriteAllBytes(temporaryFile, ExtractResource.AsByte("GameLauncher.SoapBoxModules.cursor.ani"));
                Cursor mycursor = new Cursor(Cursor.Current.Handle);
                IntPtr colorcursorhandle = User32.LoadCursorFromFile(temporaryFile);
                mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, null, mycursor, new object[] { colorcursorhandle });
                Cursor = mycursor;
                File.Delete(temporaryFile);

                //Windows Defender (Windows 10)
                if (WindowsProductVersion.GetWindowsNumber() >= 10.0 && (_settingFile.Read("WindowsDefender") == "Not Excluded"))
                {
                    Log.Core("WINDOWS DEFENDER: Windows 10 Detected! Running Exclusions for Core Folders");
                    WindowsDefenderFirstRun();
                }
                else if (WindowsProductVersion.GetWindowsNumber() >= 10.0 && _settingFile.KeyExists("WindowsDefender"))
                {
                    Log.Core("WINDOWS DEFENDER: Found 'WindowsDefender' key! Its value is " + _settingFile.Read("WindowsDefender"));
                }
            }

            Log.Visuals("CORE: Doing magic with ImageServerName");
            var pos = PointToScreen(ImageServerName.Location);
            pos = VerticalBanner.PointToClient(pos);
            ImageServerName.Parent = VerticalBanner;
            ImageServerName.Location = pos;
            ImageServerName.BackColor = Color.Transparent;

            //Log.Debug("CORE: Setting ServerStatusBar");
            //ServerStatusBar(_colorLoading, _startPoint, _endPoint);

            Log.Core("CORE: Loading ModManager Cache");
            ModManager.LoadModCache();
        }

        private void ComboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var font = (sender as ComboBox).Font;
            Brush backgroundColor;
            Brush textColor;

            var serverListText = "";
            int onlineStatus = 2; //0 = offline | 1 = online | 2 = checking

            if (sender is ComboBox cb)
            {
                if (cb.Items[e.Index] is ServerInfo si)
                {
                    serverListText = si.Name;
                    onlineStatus = serverStatusDictionary.ContainsKey(si.Id) ? serverStatusDictionary[si.Id] : 2;
                }
            }

            if (serverListText.StartsWith("<GROUP>"))
            {
                font = new Font(font, FontStyle.Bold);
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                e.Graphics.DrawString(serverListText.Replace("<GROUP>", string.Empty), font, Brushes.Black, e.Bounds);
            }
            else
            {
                font = new Font(font, FontStyle.Regular);
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && e.State != DrawItemState.ComboBoxEdit) 
                {
                    backgroundColor = SystemBrushes.Highlight;
                    textColor = SystemBrushes.HighlightText;
                }
                else
                {
                    if (onlineStatus == 2)
                    {
                        //CHECKING
                        backgroundColor = Brushes.Khaki;
                    } 
                    else if (onlineStatus == 1)
                    {
                        //ONLINE
                        backgroundColor = Brushes.PaleGreen;
                    }
                    else
                    {
                        //OFFLINE
                        backgroundColor = Brushes.LightCoral;
                    }

                    textColor = Brushes.Black;
                }

                e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                e.Graphics.DrawString("    " + serverListText, font, textColor, e.Bounds);
            }
        }

        private void SettingsCDNPick_DrawItem(object sender, DrawItemEventArgs e)
        {
            var font = (sender as ComboBox).Font;
            Brush backgroundColor;
            Brush textColor;
            Brush customTextColor = new SolidBrush(Color.FromArgb(178, 210, 255));
            Brush customBGColor = new SolidBrush(Color.FromArgb(44, 58, 76));

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

        private void MainScreen_Load(object sender, EventArgs e)
        {
            Log.Visuals("CORE: Entering mainScreen_Load");

            Log.Core("LAUNCHER: Updating server list");
            ServerListUpdater.UpdateList();

            Log.Visuals("CORE: Setting WindowName");
            Text = "GameLauncherReborn v" + Application.ProductVersion;

            Log.Core("CORE: Checking window location");
            if (Location.X >= Screen.PrimaryScreen.Bounds.Width || Location.Y >= Screen.PrimaryScreen.Bounds.Height || Location.X <= 0 || Location.Y <= 0) 
            {
                Log.Core("CORE: Window location restored to centerScreen");

                Self.CenterScreen(this);
                _windowMoved = true;
            }

            if (!string.IsNullOrEmpty(EnableInsider.BuildNumber()))
            {
                InsiderBuildNumberText.Visible = EnableInsider.ShouldIBeAnInsider();
                InsiderBuildNumberText.Text = "Insider Build Date: " + EnableInsider.BuildNumber();
            }

            _NFSW_Installation_Source = !string.IsNullOrEmpty(_settingFile.Read("CDN")) ? _settingFile.Read("CDN") : "http://localhost";
            Log.Core("LAUNCHER: NFSW Download Source is now: " + _NFSW_Installation_Source);

            Log.Visuals("CORE: Applyinng ContextMenu");
            translatedBy.Text = "";
            ContextMenu = new ContextMenu();

            ContextMenu.MenuItems.Add(new MenuItem("Donate", (b,n) => { Process.Start("https://paypal.me/metonator95"); }));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem("Settings", SettingsButton_Click));
            ContextMenu.MenuItems.Add(new MenuItem("Add Server", AddServer_Click));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem("Close launcher", CloseBTN_Click));

            Notification.ContextMenu = ContextMenu;
            Notification.Icon = new Icon(Icon, Icon.Width, Icon.Height);
            Notification.Text = "GameLauncher";
            Notification.Visible = true;

            ContextMenu = null;

            MainEmail.Text = _settingFile.Read("AccountEmail");
            MainPassword.Text = Properties.Settings.Default.PasswordDecoded;
            if (!string.IsNullOrEmpty(_settingFile.Read("AccountEmail")) && !string.IsNullOrEmpty(_settingFile.Read("Password")))
            {
                Log.Core("LAUNCHER: Restoring last saved email and password");
                RememberMe.Checked = true;
            }

            /* Server Display List */
            ServerPick.DisplayMember = "Name";
            Log.Core("LAUNCHER: Setting server list");
            finalItems = ServerListUpdater.GetList();
            ServerPick.DataSource = finalItems;

            //ForceSelectServer
            if (string.IsNullOrEmpty(_settingFile.Read("Server")))
            {
                //SelectServerBtn_Click(null, null);
                new SelectServer().ShowDialog();

                if (ServerName != null)
                {
                    this.SelectServerBtn.Text = "[...] " + ServerName.Name;
                    _settingFile.Write("Server", ServerName.IpAddress);
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
                if (string.IsNullOrEmpty(_settingFile.Read("Server")))
                {
                    Log.Warning("SERVERLIST: Failed to find anything... assuming " + ((ServerInfo)ServerPick.SelectedItem).IpAddress);
                    _settingFile.Write("Server", ((ServerInfo)ServerPick.SelectedItem).IpAddress);
                }
            }
            catch
            {
                Log.Error("SERVERLIST: Failed to write anything...");
                _settingFile.Write("Server", "");
            }

            Log.Core("SERVERLIST: Re-Checking if server is set on INI File");
            if (_settingFile.KeyExists("Server"))
            {
                Log.Core("SERVERLIST: Found something!");
                _skipServerTrigger = true;

                Log.Core("SERVERLIST: Checking if server exists on our database");

                if (finalItems.FindIndex(i => string.Equals(i.IpAddress, _settingFile.Read("Server"))) != 0 /*_slresponse.Contains(_settingFile.Read("Server"))*/)
                {
                    Log.Core("SERVERLIST: Server found! Checking ID");
                    var index = finalItems.FindIndex(i => string.Equals(i.IpAddress, _settingFile.Read("Server")));

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
                    _settingFile.DeleteKey("Server");
                }

                Log.Core("SERVERLIST: Triggering server change");
                if (ServerPick.SelectedIndex == 1)
                {
                    ServerPick_SelectedIndexChanged(sender, e);
                }
                Log.Core("SERVERLIST: All done");
            }

            /* NEW CDN Display List */
            CDNListUpdater.UpdateCDNList();

            Log.Core("LAUNCHER: Setting CDN list");
            finalCDNItems = CDNListUpdater.GetCDNList();

            Task.Run(() =>
            {
                SettingsCDNPick.Invoke(new Action(() =>
                {
                    SettingsCDNPick.DisplayMember = "Name";
                    SettingsCDNPick.DataSource = finalCDNItems;
                }));
            });

            Log.Core("LAUNCHER: Checking for password");
            if (_settingFile.KeyExists("Password"))
            {
                _loginEnabled = true;
                _serverEnabled = true;
                _useSavedPassword = true;
                LoginButton.Image = Properties.Resources.graybutton;
                LoginButton.ForeColor = Color.White;
            }
            else
            {
                _loginEnabled = false;
                _serverEnabled = false;
                _useSavedPassword = false;
                LoginButton.Image = Properties.Resources.graybutton;
                LoginButton.ForeColor = Color.Gray;
            }

            Log.Core("LAUNCHER: Setting game language");

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

            if (_settingFile.KeyExists("Language"))
            {
                SettingsLanguage.SelectedValue = _settingFile.Read("Language");
            }

            Log.Core("LAUNCHER: Re-checking InstallationDirectory: " + _settingFile.Read("InstallationDirectory"));

            var drive = Path.GetPathRoot(_settingFile.Read("InstallationDirectory"));
            if (!Directory.Exists(drive))
            {
                if (!string.IsNullOrEmpty(drive))
                {
                    var newdir = Directory.GetCurrentDirectory() + "\\Game Files";
                    _settingFile.Write("InstallationDirectory", newdir);
                    Log.Error(string.Format("LAUNCHER: Drive {0} was not found. Your actual installation directory is set to {1} now.", drive, newdir));

                    MessageBox.Show(null, string.Format("Drive {0} was not found. Your actual installation directory is set to {1} now.", drive, newdir), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            SettingsProxyCheckbox.Checked = _disableProxy;
            SettingsDiscordRPCCheckbox.Checked = _disableDiscordRPC;

            Log.Visuals("CORE: Hiding RegisterFormElements"); RegisterFormElements(false);
            Log.Visuals("CORE: Hiding SettingsFormElements"); SettingsFormElements(false);
            Log.Visuals("CORE: Hiding LoggedInFormElements"); LoggedInFormElements(false);

            Log.Visuals("CORE: Showing LoginFormElements"); LoginFormElements(true);

            Log.Core("CORE: Setting Registry Options");
            try
            {
                var gameInstallDirValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "GameInstallDir", RegistryValueKind.String);
                if (gameInstallDirValue == null || gameInstallDirValue.ToString() != Path.GetFullPath(_settingFile.Read("InstallationDirectory")))
                {
                    try
                    {
                        Registry.SetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "GameInstallDir", Path.GetFullPath(_settingFile.Read("InstallationDirectory")));
                        Registry.SetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "LaunchInstallDir", Path.GetFullPath(Application.ExecutablePath));
                    }
                    catch(Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
            } 
            catch(Exception ex) 
            {
                Log.Error(ex.Message);
            }

            Log.Core("LAUNCHER: Setting configurations");
            _newGameFilesPath = Path.GetFullPath(_settingFile.Read("InstallationDirectory"));
            SettingsGameFilesCurrentText.Text = "CURRENT DIRECTORY:";
            SettingsGameFilesCurrent.Text = _newGameFilesPath;
            //DavidCarbon
            SettingsCDNCurrent.Text = _settingFile.Read("CDN");
            //Zacam
            _newLauncherPath = Path.GetDirectoryName(Application.ExecutablePath);
            SettingsLauncherPathText.Text = "LAUNCHER FOLDER:";
            SettingsLauncherPathCurrent.Text = _newLauncherPath;

            Log.Core("DISCORD: Initializing DiscordRPC");
            Log.Core("DISCORD: Checking if Discord RPC is Disabled from User Settings! It's value is " + _disableDiscordRPC);

            _presence.State = _OS;
            _presence.Details = "In-Launcher: " + Application.ProductVersion;
            _presence.Assets = new Assets
            {
                LargeImageText = "SBRW",
                LargeImageKey = "nfsw"
            };
            if (discordRpcClient != null) discordRpcClient.SetPresence(_presence);

            BeginInvoke((MethodInvoker)delegate 
            {
                Log.Core("CORE: 'GetServerInformation' from all Servers in Server List and Download Selected Server Banners");
                LaunchNfsw();
            });

            this.BringToFront();

            if (!DetectLinux.LinuxDetected())
            {
                new LauncherUpdateCheck(LauncherIconStatus, LauncherStatusText, LauncherStatusDesc).CheckAvailability();
            }
            else
            {
                LauncherIconStatus.Image = Properties.Resources.ac_success;
                LauncherStatusText.ForeColor = Color.FromArgb(0x9fc120);
                LauncherStatusText.Text = "Launcher Status:\n - Linux Build";
                LauncherStatusDesc.Text = "Version: v" + Application.ProductVersion;
            }
            SettingsLauncherVersion.Text = LauncherStatusDesc.Text;

            Self.gamedir = _settingFile.Read("InstallationDirectory");

            if (File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords") || File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords_dis"))
            {
                try
                {
                    SettingsWordFilterCheck.Checked = !File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords");
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

            /* Load Settings API Connection Status */
            Task.Delay(800);
            PingServerListAPIStatus();

            /* Remove TracksHigh Folder and Files */
            RemoveTracksHighFiles();
        }

        private void CloseBTN_Click(object sender, EventArgs e)
        {
            //CloseBTN.BackgroundImage = Properties.Resources.close_click;

            try
            {
                if (!(ServerPick.SelectedItem is ServerInfo server)) return;
                _settingFile.Write("Server", server.IpAddress); 
            }
            catch
            {
            }

            if (_windowMoved)
            {
                _settingFile.Write("LauncherPosX", Location.X.ToString());
                _settingFile.Write("LauncherPosY", Location.Y.ToString());
            }

            try
            { 
                _settingFile.Write("InstallationDirectory", Path.GetFullPath(_settingFile.Read("InstallationDirectory")));
            }
            catch
            {
                _settingFile.Write("InstallationDirectory", _settingFile.Read("InstallationDirectory"));
            }

            //DavidCarbon
            //This Saves the update the was skipped or to remind the user at next launch
            if (Settings.Default.IgnoreUpdateVersion != String.Empty)
            {
                _settingFile.Write("IgnoreUpdateVersion", Settings.Default.IgnoreUpdateVersion);
                Log.Info("IGNOREUPDATEVERSION: Skipping Update " + Settings.Default.IgnoreUpdateVersion + " !");
            }
            else
            {
                if (_settingFile.Read("IgnoreUpdateVersion") != String.Empty)
                {
                    if (_settingFile.Read("IgnoreUpdateVersion") == Application.ProductVersion)
                    {
                        _settingFile.Write("IgnoreUpdateVersion", String.Empty);
                        Log.Info("IGNOREUPDATEVERSION: Cleared OLD IgnoreUpdateVersion Build Number. You're now on the Latest Game Launcher!");
                    }
                    else
                    {
                        Log.Info("IGNOREUPDATEVERSION: Manually Skipping Update " + _settingFile.Read("IgnoreUpdateVersion") + " !");
                    }
                }
                else
                {
                    Log.Info("IGNOREUPDATEVERSION: Latest Game Launcher!");
                }
            }

            Process[] allOfThem = Process.GetProcessesByName("nfsw");
            foreach (var oneProcess in allOfThem)
            {
                Process.GetProcessById(oneProcess.Id).Kill();
            }

            //Kill DiscordRPC
            if (discordRpcClient != null)
            {
                discordRpcClient.Dispose();
            }

            ServerProxy.Instance.Stop();
            Notification.Dispose();

            var linksPath = Path.Combine(_settingFile.Read("InstallationDirectory"), ".links");
            if (File.Exists(linksPath))
            {
                Log.Core("CLEANLINKS: Cleaning Up Mod Files {Exiting}");
                CleanLinks(linksPath);
            }

            //Leave this here. Its to properly close the launcher from Visual Studio (And Close the Launcher a well)
            try { this.Close(); } catch { }
        }

        private void AddServer_Click(object sender, EventArgs e)
        {
             new AddServer().Show();
        }

        private void OpenDebugWindow(object sender, EventArgs e)
        {
            if (!(ServerPick.SelectedItem is ServerInfo server)) return;

            var form = new DebugWindow(server.IpAddress, server.Name);
            form.Show();
        }

        private void CloseBTN_MouseEnter(object sender, EventArgs e)
        {
            //CloseBTN.BackgroundImage = Properties.Resources.close_hover;
        }

        private void CloseBTN_MouseLeave(object sender, EventArgs e)
        {
            CloseBTN.BackgroundImage = Properties.Resources.close;
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
                LoginButton.Image = Properties.Resources.graybutton;
                LoginButton.ForeColor = Color.Gray;
            }
            else
            {
                _loginEnabled = true;
                LoginButton.Image = Properties.Resources.graybutton;
                LoginButton.ForeColor = Color.White;
            }

            _useSavedPassword = false;
        }

        private void LoginButton_MouseUp(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                LoginButton.Image = Properties.Resources.graybutton_hover;
            }
            else
            {
                LoginButton.Image = Properties.Resources.graybutton;
            }
        }

        private void LoginButton_MouseDown(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                LoginButton.Image = Properties.Resources.graybutton_click;
            }
            else
            {
                LoginButton.Image = Properties.Resources.graybutton;
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
            String pass = MainPassword.Text.ToString();
            String realpass;

            Tokens.IPAddress = _serverInfo.IpAddress;
            Tokens.ServerName = _serverInfo.Name;

            Self.userAgent = _serverInfo.ForceUserAgent ?? null;

            if (_modernAuthSupport == false)
            {
                //ClassicAuth sends password in SHA1
                realpass = (_useSavedPassword) ? _settingFile.Read("Password") : SHA.HashPassword(MainPassword.Text.ToString()).ToLower();
                ClassicAuth.Login(username, realpass);
            } 
            else
            {
                //ModernAuth sends passwords in plaintext, but is POST request
                realpass = (_useSavedPassword) ? _settingFile.Read("Password") : MainPassword.Text.ToString();
                ModernAuth.Login(username, realpass);
            }

            if (RememberMe.Checked) 
            {
                _settingFile.Write("AccountEmail", username);
                _settingFile.Write("Password", realpass);
                Properties.Settings.Default.PasswordDecoded = MainPassword.Text.ToString();
            }
            else
            {
                _settingFile.DeleteKey("AccountEmail");
                _settingFile.DeleteKey("Password");
                Properties.Settings.Default.PasswordDecoded = String.Empty;
            }

            Properties.Settings.Default.Save();

            if (String.IsNullOrEmpty(Tokens.Error))
            {
                _loggedIn = true;
                _userId = Tokens.UserId;
                _loginToken = Tokens.LoginToken;
                _serverIp = Tokens.IPAddress;

                if (!String.IsNullOrEmpty(Tokens.Warning))
                {
                    MessageBox.Show(null, Tokens.Warning, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                LoginFormElements(false);
                LoggedInFormElements(true);
                SettingsButton.Visible = false;
            }
            else
            {
                //Main Screen Login
                MainEmailBorder.Image = Properties.Resources.email_error_text_border;
                MainPasswordBorder.Image = Properties.Resources.password_error_text_border;
                MessageBox.Show(null, Tokens.Error, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginButton_MouseEnter(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                LoginButton.Image = Properties.Resources.graybutton_hover;
                LoginButton.ForeColor = Color.White;
            }
            else
            {
                LoginButton.Image = Properties.Resources.graybutton;
                LoginButton.ForeColor = Color.Gray;
            }
        }

        private void LoginButton_MouseLeave(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                LoginButton.Image = Properties.Resources.graybutton;
                LoginButton.ForeColor = Color.White;
            }
            else
            {
                LoginButton.Image = Properties.Resources.graybutton;
                LoginButton.ForeColor = Color.Gray;
            }
        }

        private void ServerPick_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainEmailBorder.Image = Properties.Resources.email_text_border;
            MainPasswordBorder.Image = Properties.Resources.password_text_border;

            //ServerStatusBar(_colorLoading, _startPoint, _endPoint);

            _serverInfo = (ServerInfo)ServerPick.SelectedItem;
            _realServername = _serverInfo.Name;
            _realServernameBanner = _serverInfo.Name;
            _modernAuthSupport = false;

            if (_serverInfo.IsSpecial)
            {
                ServerPick.SelectedIndex = _lastSelectedServerId;
                return;
            }

            if (!_skipServerTrigger) { return; }

            _lastSelectedServerId = ServerPick.SelectedIndex;
            _allowRegistration = false;
            _loginEnabled = false;

            ServerStatusText.Text = "Server Status:\n - Pinging";
            ServerStatusText.ForeColor = Color.FromArgb(66, 179, 189);
            ServerStatusDesc.Text = "";
            ServerStatusIcon.Image = Properties.Resources.server_checking;

            LoginButton.ForeColor = Color.Gray;
            var verticalImageUrl = "";
            VerticalBanner.Image = GrayscaleMe(".BannerCache/" + SHA.HashPassword(_realServernameBanner) + ".bin");
            VerticalBanner.BackColor = Color.Black;

            var serverIp = _serverInfo.IpAddress;
            string numPlayers;
            string numRegistered;

            //Disable Social Panel when switching
            DisableSocialPanelandClearIt();

            ImageServerName.Text = ((ServerInfo)ServerPick.SelectedItem).Name;

            if (ServerPick.GetItemText(ServerPick.SelectedItem) == "Offline Built-In Server")
            {
                _builtinserver = true;
                LoginButton.Image = Properties.Resources.graybutton;
                LoginButton.Text = "Launch".ToUpper();
                LoginButton.ForeColor = Color.White;
                ServerInfoPanel.Visible = false;
            }
            else
            {
                _builtinserver = false;
                LoginButton.Image = Properties.Resources.graybutton;
                LoginButton.Text = "Login".ToUpper();
                LoginButton.ForeColor = Color.Gray;
                ServerInfoPanel.Visible = false;
            }

            WebClient client = new WebClient();
            var artificialPingStart = Self.GetTimestamp();
            VerticalBanner.BackColor = Color.Transparent;

            var stringToUri = new Uri(serverIp + "/GetServerInformation");
            client.DownloadStringAsync(stringToUri);

            System.Timers.Timer aTimer = new System.Timers.Timer(10000);
            aTimer.Elapsed += (x, y) => { client.CancelAsync(); };
            aTimer.Enabled = true;

            client.DownloadStringCompleted += (sender2, e2) =>
            {
                aTimer.Enabled = false;

                var artificialPingEnd = Self.GetTimestamp();

                if (e2.Cancelled)
                {
                    //ServerStatusBar(_colorOffline, _startPoint, _endPoint);

                    ServerStatusText.Text = "Server Status:\n - Offline ( OFF )";
                    ServerStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                    ServerStatusDesc.Text = "Failed to connect to server.";
                    ServerStatusIcon.Image = Properties.Resources.server_offline;
                    _serverEnabled = false;
                    _allowRegistration = false;
                    //Disable Login & Register Button
                    LoginButton.Enabled = false;
                    RegisterText.Enabled = false;
                    //Disable Social Panel
                    DisableSocialPanelandClearIt();

                    if (!serverStatusDictionary.ContainsKey(_serverInfo.Id)) 
                    {
                        serverStatusDictionary.Add(_serverInfo.Id, 2);
                    }
                    else
                    {
                        serverStatusDictionary[_serverInfo.Id] = 2; 
                    }
                } 
                else if (e2.Error != null)
                {
                    //ServerStatusBar(_colorOffline, _startPoint, _endPoint);

                    ServerStatusText.Text = "Server Status:\n - Offline ( OFF )";
                    ServerStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                    ServerStatusDesc.Text = "Server seems to be offline.";
                    ServerStatusIcon.Image = Properties.Resources.server_offline;
                    _serverEnabled = false;
                    _allowRegistration = false;
                    //Disable Login & Register Button
                    LoginButton.Enabled = false;
                    RegisterText.Enabled = false;
                    //Disable Social Panel
                    DisableSocialPanelandClearIt();

                    if (!serverStatusDictionary.ContainsKey(_serverInfo.Id)) 
                    {
                        serverStatusDictionary.Add(_serverInfo.Id, 0);
                    } 
                    else
                    {
                        serverStatusDictionary[_serverInfo.Id] = 0;
                    }
                } 
                else 
                {
                    if (_realServername == "Offline Built-In Server")
                    {
                        DisableSocialPanelandClearIt();
                        numPlayers = "∞";
                        numRegistered = "∞";
                    }
                    else
                    {
                        if (!serverStatusDictionary.ContainsKey(_serverInfo.Id))
                        {
                            serverStatusDictionary.Add(_serverInfo.Id, 1);
                        }
                        else
                        {
                            serverStatusDictionary[_serverInfo.Id] = 1;
                        }
                        //Enable Social Panel
                        ServerInfoPanel.Visible = true;

                        String purejson = String.Empty;
                        purejson = e2.Result;
                        json = JsonConvert.DeserializeObject<GetServerInformation>(e2.Result);
                        Self.rememberjson = e2.Result;
                        try
                        {
                            if (!string.IsNullOrEmpty(json.BannerUrl)) 
                            {
                                bool result;

                                try
                                {
                                    result = Uri.TryCreate(json.BannerUrl, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch
                                {
                                    result = false;
                                }

                                if (result)
                                {
                                    verticalImageUrl = json.BannerUrl;
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

                        //Discord Invite Display
                        try
                        {
                            if (string.IsNullOrEmpty(json.DiscordUrl))
                            {
                                DiscordIcon.BackgroundImage = Properties.Resources.social_discord_disabled;
                                DiscordInviteLink.Enabled = false;
                                _serverDiscordLink = null;
                                DiscordInviteLink.Text = "";
                            }
                            else
                            {
                                DiscordIcon.BackgroundImage = Properties.Resources.social_discord;
                                DiscordInviteLink.Enabled = true;
                                _serverDiscordLink = json.DiscordUrl;
                                DiscordInviteLink.Text = "Discord Invite";
                            }
                        }
                        catch
                        {
                            DiscordIcon.BackgroundImage = Properties.Resources.social_discord_disabled;
                            DiscordInviteLink.Enabled = false;
                            _serverDiscordLink = null;
                            DiscordInviteLink.Text = "";
                        }

                        //Homepage Display
                        try
                        {
                            if (string.IsNullOrEmpty(json.HomePageUrl))
                            {
                                HomePageIcon.BackgroundImage = Properties.Resources.social_home_page_disabled;
                                HomePageLink.Enabled = false;
                                _serverWebsiteLink = null;
                                HomePageLink.Text = "";
                            }
                            else
                            {
                                HomePageIcon.BackgroundImage = Properties.Resources.social_home_page;
                                HomePageLink.Enabled = true;
                                _serverWebsiteLink = json.HomePageUrl;
                                HomePageLink.Text = "Home Page";
                            }
                        }
                        catch
                        {
                            HomePageIcon.BackgroundImage = Properties.Resources.social_home_page_disabled;
                            HomePageLink.Enabled = false;
                            _serverWebsiteLink = null;
                            HomePageLink.Text = "";
                        }

                        //Facebook Group Display
                        try
                        {
                            if (string.IsNullOrEmpty(json.FacebookUrl) || json.FacebookUrl == "Your facebook page url")
                            {
                                FacebookIcon.BackgroundImage = Properties.Resources.social_facebook_disabled;
                                FacebookGroupLink.Enabled = false;
                                _serverFacebookLink = null;
                                FacebookGroupLink.Text = "";
                            }
                            else
                            {
                                FacebookIcon.BackgroundImage = Properties.Resources.social_facebook;
                                FacebookGroupLink.Enabled = true;
                                _serverFacebookLink = json.FacebookUrl;
                                FacebookGroupLink.Text = "Facebook Page";
                            }
                        }
                        catch
                        {
                            FacebookIcon.BackgroundImage = Properties.Resources.social_facebook_disabled;
                            FacebookGroupLink.Enabled = false;
                            _serverFacebookLink = null;
                            FacebookGroupLink.Text = "";
                        }

                        //Twitter Account Display
                        try
                        {
                            if (string.IsNullOrEmpty(json.TwitterUrl))
                            {
                                TwitterIcon.BackgroundImage = Properties.Resources.social_twitter_disabled;
                                TwitterAccountLink.Enabled = false;
                                _serverTwitterLink = null;
                                TwitterAccountLink.Text = "";
                            }
                            else
                            {
                                TwitterIcon.BackgroundImage = Properties.Resources.social_twitter;
                                TwitterAccountLink.Enabled = true;
                                _serverTwitterLink = json.TwitterUrl;
                                TwitterAccountLink.Text = "Twitter Feed";
                            }
                        }
                        catch
                        {
                            TwitterIcon.BackgroundImage = Properties.Resources.social_twitter_disabled;
                            TwitterAccountLink.Enabled = false;
                            _serverTwitterLink = null;
                            TwitterAccountLink.Text = "";
                        }

                        //Server Set Speedbug Timer Display
                        try
                        {
                            int serverSecondsToShutDown = (json.SecondsToShutDown != 0) ? json.SecondsToShutDown : 2 * 60 * 60;
                            TimeSpan t = TimeSpan.FromSeconds(serverSecondsToShutDown);
                            string serverSecondsToShutDownNamed = string.Format("Gameplay Timer: " + t.Hours + " Hours");

                             this.ServerShutDown.Text = serverSecondsToShutDownNamed;
                        }
                        catch
                        {
                            this.ServerShutDown.Text = "∞ and Beyond";
                        }

                        //Scenery Group Display
                        switch (String.Join("", json.ActivatedHolidaySceneryGroups))
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
                            default:
                                this.SceneryGroupText.Text = "Scenery: Normal";
                                break;
                        }

                        try
                        {
                            if (string.IsNullOrEmpty(json.RequireTicket))
                            {
                                _ticketRequired = true;
                            }
                            else if (json.RequireTicket == "true") 
                            {
                                _ticketRequired = true;
                            }
                            else
                            {
                                _ticketRequired = false;
                            }
                        }
                        catch
                        {
                            _ticketRequired = false;
                        }

                        try
                        {
                            if (string.IsNullOrEmpty(json.ModernAuthSupport))
                            {
                                _modernAuthSupport = false;
                            }
                            else if (json.ModernAuthSupport == "true") 
                            {
                                if (stringToUri.Scheme == "https") 
                                {
                                    _modernAuthSupport = true;
                                }
                                else
                                {
                                    _modernAuthSupport = false;
                                }
                            } 
                            else
                            {
                                _modernAuthSupport = false;
                            }
                        }
                        catch
                        {
                            _modernAuthSupport = false;
                        }

                        if (json.MaxUsersAllowed == 0)
                        {
                            //numPlayers = string.Format("{0} / {1}", json.OnlineNumber, json.NumberOfRegistered);
                            numPlayers = string.Format("{0}", json.OnlineNumber);
                            numRegistered = string.Format("{0}", json.NumberOfRegistered);
                        }
                        else
                        {
                            numPlayers = string.Format("{0} / {1}", json.OnlineNumber, json.MaxUsersAllowed.ToString());
                            numRegistered = string.Format("{0}", json.NumberOfRegistered);
                        }

                        _allowRegistration = true;

                        //ServerStatusBar(_colorOnline, _startPoint, _endPoint);
                    }

                    try
                    {
                        ServerStatusText.Text = "Server Status:\n - Online ( ON )";
                        ServerStatusText.ForeColor = Color.FromArgb(159, 193, 32);
                        ServerStatusIcon.Image = Properties.Resources.server_online;
                        _loginEnabled = true;
                        //Enable Login & Register Button
                        LoginButton.ForeColor = Color.White;
                        LoginButton.Enabled = true;
                        RegisterText.Enabled = true;

                        if (((ServerInfo)ServerPick.SelectedItem).Category == "DEV")
                        {
                            //Disable Social Panel
                            DisableSocialPanelandClearIt();
                        }
                    }
                    catch
                    {
                        //¯\_(ツ)_/¯
                    }

                    if (!DetectLinux.LinuxDetected())
                    {
                        Ping pingSender = new Ping();
                        pingSender.SendAsync(stringToUri.Host, 1000, new byte[1], new PingOptions(64, true), new AutoResetEvent(false));
                        pingSender.PingCompleted += (sender3, e3) => {
                            PingReply reply = e3.Reply;

                            if (reply.Status == IPStatus.Success && _realServername != "Offline Built-In Server")
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

                    //for thread safety
                    if (this.ServerStatusDesc.InvokeRequired)
                    {
                        ServerStatusDesc.Invoke(new Action(delegate ()
                        {
                            ServerStatusDesc.Text = string.Format("Players Online: {0}\nRegistered: {1}", numPlayers, numRegistered);
                        }));
                    }
                    else
                    {
                        this.ServerStatusDesc.Text = string.Format("Players Online: {0}\nRegistered: {1}", numPlayers, numRegistered);
                    }

                    _serverEnabled = true;

                    if (!Directory.Exists(".BannerCache")) { Directory.CreateDirectory(".BannerCache"); }
                    if (!string.IsNullOrEmpty(verticalImageUrl))
                    {

                        WebClient client2 = new WebClient();
                        Uri stringToUri3 = new Uri(verticalImageUrl);
                        client2.DownloadDataAsync(stringToUri3);
                        client2.DownloadProgressChanged += (sender4, e4) => 
                        {
                            if (e4.TotalBytesToReceive > 2000000)
                            {
                                client2.CancelAsync();
                                Log.Warning("Unable to Cache " + _realServername + " Server Banner! {Over 2MB?}");
                            }
                        };

                        client2.DownloadDataCompleted += (sender4, e4) =>
                        {
                            if (e4.Cancelled)
                            {
                                //Load cached banner!
                                VerticalBanner.Image = GrayscaleMe(".BannerCache/" + SHA.HashPassword(_realServernameBanner) + ".bin");
                                VerticalBanner.BackColor = Color.Black;
                                return;
                            }
                            else if (e4.Error != null)
                            {
                                //Load cached banner!
                                VerticalBanner.Image = GrayscaleMe(".BannerCache/" + SHA.HashPassword(_realServernameBanner) + ".bin");
                                VerticalBanner.BackColor = Color.Black;
                                return;
                            } 
                            else
                            {
                                try
                                {
                                    if (UriScheme.ForceGame != true)
                                    {
                                        Image image;
                                        var memoryStream = new MemoryStream(e4.Result);
                                        image = Image.FromStream(memoryStream);

                                        VerticalBanner.Image = image;
                                        VerticalBanner.BackColor = Color.Black;

                                        Console.WriteLine(GetFileExtension(verticalImageUrl));

                                        if (GetFileExtension(verticalImageUrl) != "gif")
                                        {
                                            File.WriteAllBytes(".BannerCache/" + SHA.HashPassword(_realServernameBanner) + ".bin", memoryStream.ToArray());
                                        }

                                        ImageServerName.Text = String.Empty; //_realServernameBanner;
                                    }
                                    else
                                    {
                                        ImageServerName.Text = "WebLogin";
                                        VerticalBanner.Image = null;
                                        VerticalBanner.BackColor = Color.Black;
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Log.Error(ex.Message);
                                    VerticalBanner.Image = null;
                                }
                            }
                        };
                    }
                    else
                    {
                        //Load cached banner!
                        VerticalBanner.Image = GrayscaleMe(".BannerCache/" + SHA.HashPassword(_realServernameBanner) + ".bin");
                        VerticalBanner.BackColor = Color.Black;
                    }
                }
            };
        }

        public string GetFileExtension(String filename)
        {
            return filename.Split('.').Last();
        }

        public Image GrayscaleMe(String filename)
        {
            if (!File.Exists(filename)) return null;

            try
            {
                using (var fs = new FileStream(filename, FileMode.Open)) 
                {
                    var bmp = new Bitmap(fs);
                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
                    IntPtr ptr = bmpData.Scan0;
                    int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                    byte[] rgbValues = new byte[bytes];
                    Marshal.Copy(ptr, rgbValues, 0, bytes);

                    for (int i = 0; i < rgbValues.Length; i += 3)
                    {
                        byte gray = (byte)(rgbValues[i] * .21 + rgbValues[i + 1] * .71 + rgbValues[i + 2] * .071);
                        rgbValues[i] = rgbValues[i + 1] = rgbValues[i + 2] = gray;
                    }

                    Marshal.Copy(rgbValues, 0, ptr, bytes);
                    bmp.UnlockBits(bmpData);
                    return (Bitmap)bmp.Clone();
                }
            }
            catch
            {
                return null;
            }
        }

        /* Font for all Systems */
        private void ApplyEmbeddedFonts()
        {
            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");
            /* Front Screen */
            InsiderBuildNumberText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SelectServerBtn.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            translatedBy.Font = new Font(DejaVuSans, 8f, FontStyle.Regular);
            ServerPick.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            AddServer.Font = new Font(DejaVuSansBold, 8f, FontStyle.Bold);
            ImageServerName.Font = new Font(DejaVuSans, 9.75f, FontStyle.Regular);
            ShowPlayPanel.Font = new Font(DejaVuSans, 8f, FontStyle.Regular);
            CurrentWindowInfo.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            LauncherStatusText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            LauncherStatusDesc.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            ServerStatusText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            ServerStatusDesc.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            APIStatusText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            APIStatusDesc.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            ExtractingProgress.Font  = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            /* Social Panel */
            //ServerInfoPanel.Font = new Font(DejaVuSans, 8f, FontStyle.Regular);
            HomePageLink.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            DiscordInviteLink.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            FacebookGroupLink.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            TwitterAccountLink.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SceneryGroupText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            ServerShutDown.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            /* Settings */
            SettingsPanel.Font = new Font(DejaVuSans, 8.25f, FontStyle.Regular);
            SettingsAboutButton.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            SettingsGamePathText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            SettingsGameFiles.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            SettingsCDNText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            SettingsCDNPick.Font = new Font(DejaVuSans, 8f, FontStyle.Regular);
            SettingsLanguageText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            SettingsLanguage.Font = new Font(DejaVuSans, 8f, FontStyle.Regular);
            SettingsClearCrashLogsButton.Font = new Font(DejaVuSansBold, 8f, FontStyle.Bold);
            SettingsClearCommunicationLogButton.Font = new Font(DejaVuSansBold, 8f, FontStyle.Bold);
            SettingsClearServerModCacheButton.Font = new Font(DejaVuSansBold, 8f, FontStyle.Bold);
            SettingsWordFilterCheck.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsProxyCheckbox.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsDiscordRPCCheckbox.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsGameFilesCurrentText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            SettingsGameFilesCurrent.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsCDNCurrentText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            SettingsCDNCurrent.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsLauncherPathText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            SettingsLauncherPathCurrent.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsNetworkText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            SettingsMainSrvText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsMainCDNText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsBkupSrvText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsBkupCDNText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsVFilesButton.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsLauncherVersion.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SettingsSave.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            SettingsCancel.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            /* Log In Panel */
            MainEmail.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            MainPassword.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            RememberMe.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            ForgotPassword.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            LoginButton.Font = new Font(DejaVuSansBold, 10f, FontStyle.Bold);
            RegisterText.Font = new Font(DejaVuSansBold, 10f, FontStyle.Bold);
            ServerPingStatusText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            LogoutButton.Font = new Font(DejaVuSansBold, 10f, FontStyle.Bold);
            PlayButton.Font = new Font(DejaVuSansBold, 14f, FontStyle.Bold);
            PlayProgress.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            PlayProgressText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            PlayProgressTextTimer.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            /* Registering Panel */
            RegisterPanel.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            RegisterEmail.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            RegisterPassword.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            RegisterConfirmPassword.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            RegisterTicket.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            RegisterAgree.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            RegisterButton.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            RegisterCancel.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
        }

        private void RegisterText_LinkClicked(object sender, EventArgs e)
        {
            RegisterButton.Image = Properties.Resources.greenbutton_click;
            if (_allowRegistration) 
            {
                if (!string.IsNullOrEmpty(json.WebSignupUrl)) 
                {
                    Process.Start(json.WebSignupUrl);
                    MessageBox.Show(null, "A browser window has been opened to complete registration on " + json.ServerName, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (_realServername == "WorldUnited Official" || _realServername == "WorldUnited OFFICIAL")
                {
                    Process.Start("https://signup.worldunited.gg/?discordid=" + Self.discordid);
                    MessageBox.Show(null, "A browser window has been opened to complete registration on WorldUnited OFFICIAL", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CurrentWindowInfo.Text = "REGISTER ON \n" + _realServername.ToUpper();
                LoginFormElements(false);
                RegisterFormElements(true);
            }
            else
            {
                MessageBox.Show(null, "Server seems to be offline.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) 
        {
            if (!string.IsNullOrEmpty(json.WebRecoveryUrl)) 
            {
                Process.Start(json.WebRecoveryUrl);
                MessageBox.Show(null, "A browser window has been opened to complete password recovery on " + json.ServerName, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                string send = Prompt.ShowDialog("Please specify your email address.", "GameLauncher");

                if (send != String.Empty)
                {
                    String responseString;
                    try
                    {
                        Uri resetPasswordUrl = new Uri(_serverInfo.IpAddress + "/RecoveryPassword/ForgotPassword");

                        var request = (HttpWebRequest)System.Net.WebRequest.Create(resetPasswordUrl);
                        var postData = "email=" + send;
                        var data = Encoding.ASCII.GetBytes(postData);
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = data.Length;

                        using (var stream = request.GetRequestStream())
                        {
                            stream.Write(data, 0, data.Length);
                        }

                        var response = (HttpWebResponse)request.GetResponse();
                        responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }
                    catch
                    {
                        responseString = "Failed to send email!";
                    }

                    MessageBox.Show(null, responseString, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /* Main Screen Elements */

        /* Social Panel | Ping or Offline | */
        private void DisableSocialPanelandClearIt()
        {
            //Hides Social Panel
            ServerInfoPanel.Visible = false;
            //Home
            HomePageIcon.BackgroundImage = Properties.Resources.social_home_page_disabled;
            HomePageLink.Enabled = false;
            _serverWebsiteLink = null;
            //Discord
            DiscordIcon.BackgroundImage = Properties.Resources.social_discord_disabled;
            DiscordInviteLink.Enabled = false;
            _serverDiscordLink = null;
            //Facebook
            FacebookIcon.BackgroundImage = Properties.Resources.social_facebook_disabled;
            FacebookGroupLink.Enabled = false;
            _serverFacebookLink = null;
            //Twitter
            TwitterIcon.BackgroundImage = Properties.Resources.social_twitter_disabled;
            TwitterAccountLink.Enabled = false;
            _serverTwitterLink = null;
            //Scenery
            SceneryGroupText.Text = "But It's Me!";
            //Restart Timer
            ServerShutDown.Text = "Game Launcher!";
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
                CurrentWindowInfo.Text = string.Format(_loginWelcomeTime + "\n{0}", MainEmail.Text).ToUpper();
            }

            ServerPingStatusText.Visible = hideElements;
            ShowPlayPanel.Visible = hideElements;
            ExtractingProgress.Visible = hideElements;
            PlayProgressText.Visible = hideElements;
            PlayProgressTextTimer.Visible = hideElements;
            PlayButton.Visible = hideElements;
            SettingsButton.Visible = hideElements;
            VerticalBanner.Visible = hideElements;
            ServerStatusText.Visible = hideElements;
            ServerStatusIcon.Visible = hideElements;
            ServerStatusDesc.Visible = hideElements;
            LauncherIconStatus.Visible = hideElements;
            LauncherStatusDesc.Visible = hideElements;
            LauncherStatusText.Visible = hideElements;
            //allowedCountriesLabel.Visible = hideElements;
            APIStatusText.Visible = hideElements;
            APIStatusDesc.Visible = hideElements;
            APIStatusIcon.Visible = hideElements;
        }

        private void LoginFormElements(bool hideElements = false)
        {
            if (hideElements)
            {
                CurrentWindowInfo.Text = "Enter Your Account Information to Log In".ToUpper();
            }

            RememberMe.Visible = hideElements;
            LoginButton.Visible = hideElements;
            ServerStatusText.Visible = hideElements;
            ServerStatusIcon.Visible = hideElements;
            ServerStatusDesc.Visible = hideElements;
            LauncherIconStatus.Visible = hideElements;
            LauncherStatusDesc.Visible = hideElements;
            LauncherStatusText.Visible = hideElements;

            APIStatusText.Visible = hideElements;
            APIStatusDesc.Visible = hideElements;
            APIStatusIcon.Visible = hideElements;

            RegisterText.Visible = hideElements;
            ServerPick.Visible = hideElements;
            MainEmail.Visible = hideElements;
            MainPassword.Visible = hideElements;
            ForgotPassword.Visible = hideElements;
            SettingsButton.Visible = hideElements;
            VerticalBanner.Visible = hideElements;
            PlayProgressText.Visible = hideElements;
            PlayProgressTextTimer.Visible = hideElements;
            PlayProgress.Visible = hideElements;
            ExtractingProgress.Visible = hideElements;
            AddServer.Visible = hideElements;
            AddServer.Enabled = true;
            //allowedCountriesLabel.Visible = hideElements;
            ServerPick.Enabled = true;

            //Input Strokes
            MainEmailBorder.Visible = hideElements;
            MainEmailBorder.Image = Properties.Resources.email_text_border;
            MainPasswordBorder.Visible = hideElements;
            MainPasswordBorder.Image = Properties.Resources.password_text_border;
        }

        private void RegisterFormElements(bool hideElements = true)
        {
            RegisterPanel.Visible = hideElements;
            RegisterTicket.Visible = _ticketRequired && hideElements;

            VerticalBanner.Visible = hideElements;
            ExtractingProgress.Visible = hideElements;
            PlayProgress.Visible = hideElements;
            PlayProgressText.Visible = hideElements;

            ServerStatusText.Visible = hideElements;
            ServerStatusIcon.Visible = hideElements;
            ServerStatusDesc.Visible = hideElements;
            LauncherIconStatus.Visible = hideElements;
            LauncherStatusDesc.Visible = hideElements;
            LauncherStatusText.Visible = hideElements;

            APIStatusText.Visible = hideElements;
            APIStatusDesc.Visible = hideElements;
            APIStatusIcon.Visible = hideElements;

            AddServer.Visible = hideElements;
            AddServer.Enabled = false;
            ServerPick.Visible = hideElements;
            ServerPick.Enabled = false;

            // Reset fields
            RegisterEmail.Text = "";
            RegisterPassword.Text = "";
            RegisterConfirmPassword.Text = "";
            RegisterAgree.Checked = false;

            //Input Strokes
            RegisterEmailBorder.Visible = hideElements;
            RegisterPasswordBorder.Visible = hideElements;
            RegisterConfirmPasswordBorder.Visible = hideElements;
            RegisterTicketBorder.Visible = _ticketRequired && hideElements;
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            if (_disableLogout == true)
            {
                return;
            }
            BackgroundImage = Properties.Resources.mainbackground;
            _loggedIn = false;
            LoggedInFormElements(false);
            LoginFormElements(true);

            _userId = String.Empty;
            _loginToken = String.Empty;
        }

        private void Greenbutton_hover_MouseEnter(object sender, EventArgs e)
        {
            SettingsSave.Image = Properties.Resources.greenbutton_hover;
            RegisterText.Image = Properties.Resources.greenbutton_hover;
            RegisterButton.Image = Properties.Resources.greenbutton_hover;
        }

        private void Greenbutton_MouseLeave(object sender, EventArgs e)
        {
            SettingsSave.Image = Properties.Resources.greenbutton;
            RegisterText.Image = Properties.Resources.greenbutton;
            RegisterButton.Image = Properties.Resources.greenbutton;
        }

        private void Greenbutton_hover_MouseUp(object sender, EventArgs e)
        {
            SettingsSave.Image = Properties.Resources.greenbutton_hover;
            RegisterText.Image = Properties.Resources.greenbutton_hover;
            RegisterButton.Image = Properties.Resources.greenbutton_hover;
        }

        private void Greenbutton_click_MouseDown(object sender, EventArgs e)
        {
            SettingsSave.Image = Properties.Resources.greenbutton_click;
            RegisterText.Image = Properties.Resources.greenbutton_click;
            RegisterButton.Image = Properties.Resources.greenbutton_click;
        }

        private void RegisterCancel_Click(object sender, EventArgs e)
        {
            BackgroundImage = Properties.Resources.mainbackground;
            CurrentWindowInfo.Text = "Enter your account information to Log In:".ToUpper();
            RegisterFormElements(false);
            LoginFormElements(true);
            ResetRegisterErrorColors();
        }

        private void ResetRegisterErrorColors()
        {
            RegisterAgree.ForeColor = Color.White;
            //Reset Input Stroke Images
            RegisterEmailBorder.Image = Properties.Resources.email_text_border;
            RegisterPasswordBorder.Image = Properties.Resources.password_text_border;
            RegisterConfirmPasswordBorder.Image = Properties.Resources.password_text_border;
            RegisterTicketBorder.Image = Properties.Resources.ticket_text_border;
        }

        private void RegisterAgree_CheckedChanged(object sender, EventArgs e)
        {
            RegisterAgree.ForeColor = Color.White;
        }

        private void RegisterEmail_TextChanged(object sender, EventArgs e)
        {
            RegisterEmailBorder.Image = Properties.Resources.email_text_border;
        }

        private void RegisterTicket_TextChanged(object sender, EventArgs e)
        {
            RegisterTicketBorder.Image = Properties.Resources.ticket_text_border;
        }

        private void RegisterConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            RegisterConfirmPasswordBorder.Image = Properties.Resources.password_text_border;
        }

        private void RegisterPassword_TextChanged(object sender, EventArgs e)
        {
            RegisterPasswordBorder.Image = Properties.Resources.password_text_border;
        }

        private void Email_TextChanged(object sender, EventArgs e)
        {
            MainEmailBorder.Image = Properties.Resources.email_text_border;
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            MainEmailBorder.Image = Properties.Resources.email_text_border;
            MainPasswordBorder.Image = Properties.Resources.password_text_border;
        }

        private void Graybutton_click_MouseDown(object sender, EventArgs e)
        {
            SettingsCancel.Image = Properties.Resources.graybutton_click;
            LogoutButton.Image = Properties.Resources.graybutton_click;
            RegisterCancel.Image = Properties.Resources.graybutton_click;
        }

        private void Graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            SettingsCancel.Image = Properties.Resources.graybutton_hover;
            LogoutButton.Image = Properties.Resources.graybutton_hover;
            RegisterCancel.Image = Properties.Resources.graybutton_hover;
        }

        private void Graybutton_MouseLeave(object sender, EventArgs e)
        {
            SettingsCancel.Image = Properties.Resources.graybutton;
            LogoutButton.Image = Properties.Resources.graybutton;
            RegisterCancel.Image = Properties.Resources.graybutton;
        }

        private void Graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            SettingsCancel.Image = Properties.Resources.graybutton_hover;
            LogoutButton.Image = Properties.Resources.graybutton_hover;
            RegisterCancel.Image = Properties.Resources.graybutton_hover;
        }

        public void DrawErrorAroundTextBox(TextBox x)
        {
            x.BorderStyle = BorderStyle.FixedSingle;
            var p = new Pen(Color.Red);
            var g = CreateGraphics();
            var variance = 1;
            g.DrawRectangle(p, new Rectangle(x.Location.X - variance, x.Location.Y - variance, x.Width + variance, x.Height + variance));
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            Refresh();

            List<string> registerErrors = new List<string>(); 

            if (string.IsNullOrEmpty(RegisterEmail.Text))
            {
                registerErrors.Add("Please enter your e-mail.");
                RegisterEmailBorder.Image = Properties.Resources.email_error_text_border;

            }
            else if (Self.ValidateEmail(RegisterEmail.Text) == false)
            {
                registerErrors.Add("Please enter a valid e-mail address.");
                RegisterEmailBorder.Image = Properties.Resources.email_error_text_border;
            }

            if (string.IsNullOrEmpty(RegisterTicket.Text) && _ticketRequired)
            {
                registerErrors.Add("Please enter your ticket.");
                RegisterTicketBorder.Image = Properties.Resources.ticket_error_text_border;
            }

            if (string.IsNullOrEmpty(RegisterPassword.Text))
            {
                registerErrors.Add("Please enter your password.");
                RegisterPasswordBorder.Image = Properties.Resources.password_error_text_border;
            }

            if (string.IsNullOrEmpty(RegisterConfirmPassword.Text)) 
            {
                registerErrors.Add("Please confirm your password.");
                RegisterConfirmPasswordBorder.Image = Properties.Resources.password_error_text_border;
            }

            if (RegisterConfirmPassword.Text != RegisterPassword.Text)
            {
                registerErrors.Add("Passwords don't match.");
                RegisterPasswordBorder.Visible = true;
                RegisterConfirmPasswordBorder.Image = Properties.Resources.password_error_text_border;
            }

            if (!RegisterAgree.Checked)
            {
                registerErrors.Add("You have not agreed to the Terms of Service.");
                RegisterAgree.ForeColor = Color.FromArgb(254, 0, 0);
            }

            if (registerErrors.Count == 0)
            {
                bool allowReg = false;

                try
                {
                    WebClient breachCheck = new WebClient();
                    String checkPassword = SHA.HashPassword(RegisterPassword.Text.ToString()).ToUpper();

                    var regex = new Regex(@"([0-9A-Z]{5})([0-9A-Z]{35})").Split(checkPassword);

                    String range = regex[1];
                    String verify = regex[2];
                    String serverReply = breachCheck.DownloadString("https://api.pwnedpasswords.com/range/"+range);

                    string[] hashes = serverReply.Split('\n');
                    foreach (string hash in hashes)
                    {
                        var splitChecks = hash.Split(':');
                        if (splitChecks[0] == verify)
                        {
                            var passwordCheckReply = MessageBox.Show(null, "Password used for registration has been breached " + Convert.ToInt32(splitChecks[1])+ " times, you should consider using different one.\r\nAlternatively you can use unsafe password anyway. Use it?", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (passwordCheckReply == DialogResult.Yes)
                            {
                                allowReg = true;
                            }
                            else
                            {
                                allowReg = false;
                            }
                        } 
                        else
                        {
                            allowReg = true;
                        }
                    }
                } 
                catch
                {
                    allowReg = true;
                }

                if (allowReg == true)
                {
                    Tokens.Clear();

                    String username = RegisterEmail.Text.ToString();
                    String realpass;
                    String token = (_ticketRequired) ? RegisterTicket.Text : null;

                    Tokens.IPAddress = _serverInfo.IpAddress;
                    Tokens.ServerName = _serverInfo.Name;

                    if (_modernAuthSupport == false)
                    {
                        realpass = SHA.HashPassword(RegisterPassword.Text.ToString()).ToLower();
                        ClassicAuth.Register(username, realpass, token);
                    }
                    else
                    {
                        realpass = RegisterPassword.Text.ToString();
                        ModernAuth.Register(username, realpass, token);
                    }

                    if (!String.IsNullOrEmpty(Tokens.Success))
                    {
                        _loggedIn = true;
                        _userId = Tokens.UserId;
                        _loginToken = Tokens.LoginToken;
                        _serverIp = Tokens.IPAddress;

                        MessageBox.Show(null, Tokens.Success, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        ResetRegisterErrorColors();

                        BackgroundImage = Properties.Resources.mainbackground;

                        RegisterFormElements(false);
                        LoginFormElements(true);

                        _loggedIn = true;
                    }
                    else
                    {
                        MessageBox.Show(null, Tokens.Error, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var message = "There were some errors while registering, please fix them:\n\n";

                    foreach (var error in registerErrors)
                    {
                        message += "• " + error + "\n";
                    }

                    MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /* SETTINGS PAGE LAYOUT */
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SettingsPanelDisplay();
        }

        private void CDN_Offline_Switch()
        {
            SettingsPanelDisplay();
        }

        private void SettingsPanelDisplay()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            if (EnableInsider.ShouldIBeAnInsider() == true)
            {
                SettingsVFilesButton.Visible = true;
            }

            BackgroundImage = Properties.Resources.secondarybackground;
            SettingsFormElements(true);
            RegisterFormElements(false);
            LoggedInFormElements(false);
            LoginFormElements(false);

            /* Functions Calls */
            RememberLastCDN();
            IsCDNDownGame();
            PingAPIStatus();

            /* Hide Social Panel */
            ServerInfoPanel.Visible = false;

            if (File.Exists(_settingFile.Read("InstallationDirectory") + "/NFSWO_COMMUNICATION_LOG.txt"))
            {
                SettingsClearCommunicationLogButton.Enabled = true;
            }

            if (Directory.Exists(_settingFile.Read("InstallationDirectory") + "/.data"))
            {
                SettingsClearServerModCacheButton.Enabled = true;
            }

            var crashLogFilesDirectory = new DirectoryInfo(_settingFile.Read("InstallationDirectory"));

            foreach (var file in crashLogFilesDirectory.EnumerateFiles("SBRCrashDump_CL0*.dmp"))
            {
                SettingsClearCrashLogsButton.Enabled = true;
            }
        }

        private void SettingsButton_MouseEnter(object sender, EventArgs e)
        {
        }

        private void SettingsButton_MouseLeave(object sender, EventArgs e)
        {
            SettingsButton.BackgroundImage = Properties.Resources.settingsbtn;
        }

        private void SettingsCancel_Click(object sender, EventArgs e)
        {
            SettingsFormElements(false);
            LoggedInFormElements(false);
            LoginFormElements(true);
            BackgroundImage = Properties.Resources.mainbackground;
            //Show Social Panel
            ServerInfoPanel.Visible = true;
        }

        public void ClearColoredPingStatus()
        {
            //Reset Connection Status Labels - DavidCarbon
            SettingsMainSrvText.Text = "[API] United: PINGING";
            SettingsMainSrvText.ForeColor = Color.FromArgb(66, 179, 189);
            SettingsMainCDNText.Text = "[API] Carbon: PINGING";
            SettingsMainCDNText.ForeColor = Color.FromArgb(66, 179, 189);
            SettingsBkupSrvText.Text = "[API] Carbon (2nd): PINGING";
            SettingsBkupSrvText.ForeColor = Color.FromArgb(66, 179, 189);
            SettingsBkupCDNText.Text = "[API] WOPL: PINGING";
            SettingsBkupCDNText.ForeColor = Color.FromArgb(66, 179, 189);
        }

        private void SettingsSave_Click(object sender, EventArgs e)
        {
            //TODO null check
            _settingFile.Write("Language", SettingsLanguage.SelectedValue.ToString());

            if (WindowsProductVersion.GetWindowsNumber() >= 10.0 && (_settingFile.Read("InstallationDirectory") != _newGameFilesPath))
            {
                WindowsDefenderGameFilesDirctoryChange();
            }
            else if (_settingFile.Read("InstallationDirectory") != _newGameFilesPath)
            {
                //Remove current Firewall for the Game Files 
                string CurrentGameFilesExePath = Path.Combine(_settingFile.Read("InstallationDirectory") + "\\nfsw.exe");

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

                _settingFile.Write("InstallationDirectory", _newGameFilesPath);

                if (!DetectLinux.LinuxDetected())
                {
                    CheckGameFilesDirectoryPrevention();
                }

                _restartRequired = true;
                //Clean Mods Files from New Dirctory (If it has .links in directory)
                var linksPath = Path.Combine(_settingFile.Read("InstallationDirectory"), ".links");
                if (File.Exists(linksPath))
                {
                    Log.Core("CLEANLINKS: Cleaning Up Mod Files {Settings}");
                    CleanLinks(linksPath);
                }
            }

            Console.WriteLine(_settingFile.Read("CDN"));
            Console.WriteLine(((CDNObject)SettingsCDNPick.SelectedItem).Url);

            if (_settingFile.Read("CDN") != ((CDNObject)SettingsCDNPick.SelectedItem).Url) {
                SettingsCDNCurrentText.Text = "CHANGED CDN";
                SettingsCDNCurrent.Text = ((CDNObject)SettingsCDNPick.SelectedItem).Url;
                _settingFile.Write("CDN", ((CDNObject)SettingsCDNPick.SelectedItem).Url);
                _restartRequired = true;
            }

            String disableProxy = (SettingsProxyCheckbox.Checked == true) ? "1" : "0";
            if (_settingFile.Read("DisableProxy") != disableProxy) {
                _settingFile.Write("DisableProxy", (SettingsProxyCheckbox.Checked == true) ? "1" : "0");
                _restartRequired = true;
            }

            String disableRPC = (SettingsDiscordRPCCheckbox.Checked == true) ? "1" : "0";
            if (_settingFile.Read("DisableRPC") != disableRPC)
            {
                _settingFile.Write("DisableRPC", (SettingsDiscordRPCCheckbox.Checked == true) ? "1" : "0");
                _restartRequired = true;
            }

            if (_restartRequired)
            {
                MessageBox.Show(null, "In order to see settings changes, you need to restart launcher manually.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //Actually lets check those 2 files
            if (File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords") && File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords_dis"))
            {
                File.Delete(_settingFile.Read("InstallationDirectory") + "/profwords_dis");
            }

            //Delete/Enable profwords filter here
            if (SettingsWordFilterCheck.Checked)
            {
                if (File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords")) File.Move(_settingFile.Read("InstallationDirectory") + "/profwords", _settingFile.Read("InstallationDirectory") + "/profwords_dis");
            }
            else
            {
                if (File.Exists(_settingFile.Read("InstallationDirectory") + "/profwords_dis")) File.Move(_settingFile.Read("InstallationDirectory") + "/profwords_dis", _settingFile.Read("InstallationDirectory") + "/profwords");
            }

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
            catch(Exception ex)
            {
                MessageBox.Show(null, "There was an error saving your settings to actual file. Restoring default.\n" + ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.Delete(_userSettings);
            }
            userSettingsXml.Save(_userSettings);

            SettingsFormElements(false);
            LoggedInFormElements(false);
            LoginFormElements(true);
            BackgroundImage = Properties.Resources.mainbackground;
            //Show Social Panel
            ServerInfoPanel.Visible = true;
        }

        //Changing GameFiles Location from Settings - DavidCarbon & Zacam
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

        private void SettingsLauncherPathCurrent_Click(object sender, EventArgs e)
        {
            Process.Start(_newLauncherPath);
        }

        private void SettingsGameFilesCurrent_Click(object sender, EventArgs e)
        {
            Process.Start(_newGameFilesPath);
        }

        private void SettingsCDNCurrent_LinkClicked(object sender, EventArgs e)
        {
            Process.Start(_settingFile.Read("CDN"));
        }

        private void SettingsFormElements(bool hideElements = true)
        {
            if (hideElements)
            {
                CurrentWindowInfo.Text = "";
            }

            SettingsPanel.Visible = hideElements;
        }

        private void StartGame(string userId, string loginToken)
        {
            if (UriScheme.ServerIP != String.Empty)
            {
                _serverIp = UriScheme.ServerIP;
            }

            if (_realServername == "Freeroam Sparkserver")
            {
                //Force proxy enabled.
                Log.Core("LAUNCHER: Forcing Proxified connection for FRSS");
                _disableProxy = false;
            }

            _nfswstarted = new Thread(() =>
            {
                if (_disableProxy == true)
                {
                    if (_disableDiscordRPC == true)
                    {
                        discordRpcClient.Dispose();
                        discordRpcClient = null;
                    }

                    Uri convert = new Uri(_serverIp);

                    if (convert.Scheme == "http")
                    {
                        Match match = Regex.Match(convert.Host, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
                        if (!match.Success)
                        {
                            _serverIp = _serverIp.Replace(convert.Host, Self.HostName2IP(convert.Host));
                        }
                    }

                    LaunchGame(userId, loginToken, _serverIp, this);
                }
                else
                {
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

                if(discordRpcClient != null) discordRpcClient.SetPresence(_presence);
            }
        }

        //DavidCarbon
        private async void PingAPIStatus ()
        {
            ClearColoredPingStatus();
            Log.Api("SETTINGS PINGING API: Checking APIs");
            await Task.Delay(1000);
            switch (APIStatusChecker.CheckStatus(Self.mainserver + "/serverlist.json"))
            {
                case API.Online:
                    SettingsMainSrvText.Text = "[API] United: ONLINE";
                    SettingsMainSrvText.ForeColor = Color.FromArgb(159, 193, 32);
                    break;
                case API.Offline:
                    SettingsMainSrvText.Text = "[API] United: OFFLINE";
                    SettingsMainSrvText.ForeColor = Color.FromArgb(254, 0, 0);
                    break;
                default:
                    SettingsMainSrvText.Text = "[API] United: ERROR";
                    SettingsMainSrvText.ForeColor = Color.FromArgb(254, 0, 0);
                    break;
            }

            await Task.Delay(1500);
            switch (APIStatusChecker.CheckStatus(Self.staticapiserver + "/serverlist.json"))
            {
                case API.Online:
                    SettingsMainCDNText.Text = "[API] Carbon: ONLINE";
                    SettingsMainCDNText.ForeColor = Color.FromArgb(159, 193, 32);
                    break;
                case API.Offline:
                    SettingsMainCDNText.Text = "[API] Carbon: OFFLINE";
                    SettingsMainCDNText.ForeColor = Color.FromArgb(254, 0, 0);
                    break;
                default:
                    SettingsMainCDNText.Text = "[API] Carbon: ERROR";
                    SettingsMainCDNText.ForeColor = Color.FromArgb(254, 0, 0);
                    break;
            }

            await Task.Delay(2000);
            switch (APIStatusChecker.CheckStatus(Self.secondstaticapiserver + "/serverlist.json"))
            {
                case API.Online:
                    SettingsBkupSrvText.Text = "[API] Carbon (2nd): ONLINE";
                    SettingsBkupSrvText.ForeColor = Color.FromArgb(159, 193, 32);
                    break;
                case API.Offline:
                    SettingsBkupSrvText.Text = "[API] Carbon (2nd): OFFLINE";
                    SettingsBkupSrvText.ForeColor = Color.FromArgb(254, 0, 0);
                    break;
                default:
                    SettingsBkupSrvText.Text = "[API] Carbon (2nd): ERROR";
                    SettingsBkupSrvText.ForeColor = Color.FromArgb(254, 0, 0);
                    break;
            }

            await Task.Delay(2500);
            switch (APIStatusChecker.CheckStatus(Self.woplserver + "/serverlist.json"))
            {
                case API.Online:
                    SettingsBkupCDNText.Text = "[API] WOPL: ONLINE";
                    SettingsBkupCDNText.ForeColor = Color.FromArgb(159, 193, 32);
                    break;
                case API.Offline:
                    SettingsBkupCDNText.Text = "[API] WOPL: OFFLINE";
                    SettingsBkupCDNText.ForeColor = Color.FromArgb(254, 0, 0);
                    break;
                default:
                    SettingsBkupCDNText.Text = "[API] WOPL: ERROR";
                    SettingsBkupCDNText.ForeColor = Color.FromArgb(254, 0, 0);
                    break;
            }
        }

        //Check Serverlist API Status Upon Main Screen load - DavidCarbon
        private async void PingServerListAPIStatus()
        {
            bool WUGGAPIOffline = false;
            bool DCAPIOffline = false;
            bool DC2APIOffline = false;
            bool AllAPIsOffline = false;

            Log.Api("PINGING API: Checking API Status");
            switch (APIStatusChecker.CheckStatus(Self.mainserver + "/serverlist.json"))
            {
                case API.Online:
                    APIStatusText.Text = "United API:\n - Online";
                    APIStatusText.ForeColor = Color.FromArgb(159, 193, 32);
                    APIStatusDesc.Text = "Connected to API";
                    APIStatusIcon.Image = Properties.Resources.api_success;
                    Log.Api("PINGING API: United API has responded. Its Online!");
                    break;
                default:
                    APIStatusText.Text = "United API:\n - Offline";
                    APIStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                    APIStatusDesc.Text = "Checking Carbon API";
                    APIStatusIcon.Image = Properties.Resources.api_error;
                    Log.Api("PINGING API: United API has not responded. Checking Backup...");
                    WUGGAPIOffline = true;
                    break;
            }

            if (WUGGAPIOffline == true && DCAPIOffline == false && DC2APIOffline == false && AllAPIsOffline == false)
            {
                await Task.Delay(500);
                APIStatusText.Text = "Carbon API:\n - Pinging";
                APIStatusText.ForeColor = Color.FromArgb(66, 179, 189);
                APIStatusIcon.Image = Properties.Resources.api_checking;
                await Task.Delay(1000);
                switch (APIStatusChecker.CheckStatus(Self.staticapiserver + "/serverlist.json"))
                {
                    case API.Online:
                        APIStatusText.Text = "Carbon API:\n - Online";
                        APIStatusText.ForeColor = Color.FromArgb(159, 193, 32);
                        APIStatusDesc.Text = "Connected to API";
                        APIStatusIcon.Image = Properties.Resources.api_success;
                        Log.Api("PINGING API: Carbon API has responded. Its Online!");
                        break;
                    default:
                        APIStatusText.Text = "Carbon API:\n - Offline";
                        APIStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                        APIStatusDesc.Text = "Checking Carbon 2nd API";
                        APIStatusIcon.Image = Properties.Resources.api_error;
                        Log.Api("PINGING API: Carbon API has not responded. Checking Backup...");
                        DCAPIOffline = true;
                        break;
                }
            }

            if (WUGGAPIOffline == true && DCAPIOffline == true && DC2APIOffline == false && AllAPIsOffline == false)
            {
                await Task.Delay(500);
                APIStatusText.Text = "Carbon 2nd API:\n - Pinging";
                APIStatusText.ForeColor = Color.FromArgb(66, 179, 189);
                APIStatusIcon.Image = Properties.Resources.api_checking;
                await Task.Delay(1000);
                switch (APIStatusChecker.CheckStatus(Self.secondstaticapiserver + "/serverlist.json"))
                {
                    case API.Online:
                        APIStatusText.Text = "Carbon 2nd API:\n - Online";
                        APIStatusText.ForeColor = Color.FromArgb(159, 193, 32);
                        APIStatusDesc.Text = "Connected to API";
                        APIStatusIcon.Image = Properties.Resources.api_success;
                        Log.Api("PINGING API: Carbon 2nd API has responded. Its Online!");
                        break;
                    default:
                        APIStatusText.Text = "Carbon 2nd API:\n - Offline";
                        APIStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                        APIStatusDesc.Text = "Checking WOPL API";
                        APIStatusIcon.Image = Properties.Resources.api_error;
                        Log.Api("PINGING API: Carbon 2nd API has not responded. Checking Backup...");
                        DC2APIOffline = true;
                        break;
                }
            }

            if (WUGGAPIOffline == true && DCAPIOffline == true && DC2APIOffline == true && AllAPIsOffline == false)
            {
                await Task.Delay(500);
                APIStatusText.Text = "WOPL API:\n - Pinging";
                APIStatusText.ForeColor = Color.FromArgb(66, 179, 189);
                APIStatusIcon.Image = Properties.Resources.api_checking;
                await Task.Delay(1000);
                switch (APIStatusChecker.CheckStatus(Self.woplserver + "/serverlist.json"))
                {
                    case API.Online:
                        APIStatusText.Text = "WOPL API:\n - Online";
                        APIStatusText.ForeColor = Color.FromArgb(159, 193, 32);
                        APIStatusDesc.Text = "Connected to API";
                        APIStatusIcon.Image = Properties.Resources.api_success;
                        Log.Api("PINGING API: WOPL API has responded. Its Online!");
                        break;
                    default:
                        APIStatusText.Text = "WOPL API:\n - Offline";
                        APIStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                        APIStatusDesc.Text = "Yikes!";
                        APIStatusIcon.Image = Properties.Resources.api_error;
                        Log.Api("PINGING API: WOPL API has not responded....");
                        AllAPIsOffline = true;
                        break;
                }
            }

            if (AllAPIsOffline == true)
            {
                await Task.Delay(1000);
                APIStatusText.Text = "Connection API:\n - Error";
                APIStatusText.ForeColor = Color.FromArgb(254, 0, 0);
                APIStatusDesc.Text = "Failed to Connect to APIs";
                APIStatusIcon.Image = Properties.Resources.api_error;
                Log.Api("PINGING API: Failed to Connect to APIs! Quick Hide and Bunker Down! (Ask for help)");
            }
        }

        //CDN Display Playing Game! - DavidCarbon
        private async void IsCDNDownGame()
        {
            if (!string.IsNullOrEmpty(_settingFile.Read("CDN")))
            {
                SettingsCDNCurrent.LinkColor = Color.FromArgb(66, 179, 189);
                Log.Info("SETTINGS PINGING CDN: Checking Current CDN from Settings.ini");
                await Task.Delay(500);

                switch (APIStatusChecker.CheckStatus(_settingFile.Read("CDN") + "/index.xml"))
                {
                    case API.Online:
                        SettingsCDNCurrent.LinkColor = Color.FromArgb(159, 193, 32);
                        Log.UrlCall("SETTINGS PINGING CDN: " + _settingFile.Read("CDN") + " Is Online!");
                        break;
                    default:
                        SettingsCDNCurrent.LinkColor = Color.FromArgb(254, 0, 0);
                        Log.UrlCall("SETTINGS PINGING CDN: " + _settingFile.Read("CDN") + " Is Offline!");
                        break;
                }
            }
            else
            {
                Log.Error("SETTINGS PINGING CDN: Settings.ini has an Empty CDN URL");
            }
        }

        private void IsChangedCDNDown()
        {
            if (!string.IsNullOrEmpty(((CDNObject)SettingsCDNPick.SelectedItem).Url))
            {
                SettingsCDNText.Text = "CDN: PINGING";
                SettingsCDNText.ForeColor = Color.FromArgb(66, 179, 189);
                Log.Info("SETTINGS PINGING CHANGED CDN: Checking Changed CDN from Drop Down List");

                switch (APIStatusChecker.CheckStatus(((CDNObject)SettingsCDNPick.SelectedItem).Url + "/index.xml"))
                {
                    case API.Online:
                        SettingsCDNText.Text = "CDN: ONLINE";
                        SettingsCDNText.ForeColor = Color.FromArgb(159, 193, 32);
                        Log.UrlCall("SETTINGS PINGING CHANGED CDN: " + ((CDNObject)SettingsCDNPick.SelectedItem).Url + " Is Online!");
                        break;
                    default:
                        SettingsCDNText.Text = "CDN: OFFLINE";
                        SettingsCDNText.ForeColor = Color.FromArgb(254, 0, 0);
                        Log.UrlCall("SETTINGS PINGING CHANGED CDN: " + ((CDNObject)SettingsCDNPick.SelectedItem).Url + " Is Offline!");
                        break;
                }
            }
            else
            {
                Log.Error("SETTINGS PINGING CHANGED CDN: '((CDNObject)SettingsCDNPick.SelectedItem).Url)' has an Empty CDN URL");
            }
        }

        private void LaunchGame(string userId, string loginToken, string serverIp, Form x)
        {
            var oldfilename = _settingFile.Read("InstallationDirectory") + "/nfsw.exe";

            var args = _serverInfo.Id.ToUpper() + " " + serverIp + " " + loginToken + " " + userId;
            var psi = new ProcessStartInfo();

            if (DetectLinux.LinuxDetected())
            {
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
            int secondsToShutDown = (json.SecondsToShutDown != 0) ? json.SecondsToShutDown : 2*60*60;
                System.Timers.Timer shutdowntimer = new System.Timers.Timer();
                shutdowntimer.Elapsed += (x2, y2) =>
            {
                if (secondsToShutDown == 300)
                {
                    Notification.Visible = true;
                    Notification.BalloonTipIcon = ToolTipIcon.Info;
                    Notification.BalloonTipTitle = "SpeedBug Fix - " + _realServername;
                    Notification.BalloonTipText = "Game is going to shut down in 5 minutes. Please restart it manually before the launcher does it.";
                    Notification.ShowBalloonTip(5000);
                    Notification.Dispose();
                }

                Process[] allOfThem = Process.GetProcessesByName("nfsw");

                if (secondsToShutDown <= 0)
                {
                    if (Self.CanDisableGame == true) {
                        foreach (var oneProcess in allOfThem)
                        {
                            _gameKilledBySpeedBugCheck = true;
                            Process.GetProcessById(oneProcess.Id).Kill();
                        }
                    }
                    else
                    {
                        secondsToShutDown = 0;
                    }
                }

                //change title

                foreach (var oneProcess in allOfThem)
                {
                    long p = oneProcess.MainWindowHandle.ToInt64();
                    TimeSpan t = TimeSpan.FromSeconds(secondsToShutDown);
                    string secondsToShutDownNamed = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);

                    if (secondsToShutDown == 0)
                    {
                        secondsToShutDownNamed = "Waiting for event to finish.";
                    }

                    User32.SetWindowText((IntPtr)p, "NEED FOR SPEED™ WORLD | Server: " + _realServername + " | Launcher Build: " + ProductVersion + " | Force Restart In: " + secondsToShutDownNamed);
                }

                --secondsToShutDown;
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

                    if (_gameKilledBySpeedBugCheck == true) exitCode = 2137;

                    if (exitCode == 0)
                    {
                        CloseBTN_Click(null, null);
                    }
                    else
                    {
                        x.BeginInvoke(new Action(() =>
                        {
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
                            PlayProgressText.Text = errorMsg.ToUpper();
                            PlayProgress.Value = 100;
                            PlayProgress.ForeColor = Color.Red;
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
                                Properties.Settings.Default.IsRestarting = true;
                                Properties.Settings.Default.Save();
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

                WebClient client2 = new WebClient();

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
                        //Redownload other file
                        DownloadModNetFilesRightNow(path);
                    }
                };
                client2.DownloadFileAsync(url, path + "/" + FileName);
                isDownloadingModNetFiles = true;
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (UriScheme.ForceGame != true)
            {
                if (_loggedIn == false)
                {
                    if (_useSavedPassword == false) return;
                    LoginButton_Click(sender, e);
                }

                if (_playenabled == false) {
                    return;
                }
            }
            else
            {
                //set background black
                ImageServerName.Text = "WebLogin";
                VerticalBanner.Image = null;

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

            ModManager.ResetModDat(_settingFile.Read("InstallationDirectory"));

            if (Directory.Exists(_settingFile.Read("InstallationDirectory") + "/modules")) Directory.Delete(_settingFile.Read("InstallationDirectory") + "/modules", true);
            if (!Directory.Exists(_settingFile.Read("InstallationDirectory") + "/scripts")) Directory.CreateDirectory(_settingFile.Read("InstallationDirectory") + "/scripts");
            String[] ModNetReloadedFiles = new string[]
            {
                "dinput8.dll",
                "global.ini",
                "7z.dll", 
                "fmt.dll", 
                "libcurl.dll", 
                "zlib1.dll", 
                "ModLoader.asi"
            };

            PlayButton.BackgroundImage = Properties.Resources.playbutton;

            Log.Core("LAUNCHER: Installing ModNet");
            PlayProgressText.Text = ("Detecting ModNetSupport for " + _realServernameBanner).ToUpper();
            String jsonModNet = ModNetReloaded.ModNetSupported(_serverIp);

            if (jsonModNet != String.Empty)
            {
                PlayProgressText.Text = "ModNetReloaded support detected, setting up...".ToUpper();

                try
                {
                    string[] newFiles = ModNetReloadedFiles.ToArray();
                    WebClient newModNetFilesDownload = new WebClient();
                    foreach (string file in newFiles)
                    {
                        PlayProgressText.Text = ("Fetching ModNetReloaded Files: " + file).ToUpper();
                        Application.DoEvents();
                        newModNetFilesDownload.DownloadFile(Self.modnetserver + "/modules-v2/" + file, _settingFile.Read("InstallationDirectory") + "/" + file);
                    }

                    //get files now
                    MainJson json2 = JsonConvert.DeserializeObject<MainJson>(jsonModNet);

                    //metonator was here!
                    try
                    {
                        CarsList.remoteCarsList = new WebClient().DownloadString(json2.BasePath + "/cars.json");
                    }
                    catch { }
                    if (CarsList.remoteCarsList != String.Empty) { Log.Info("DISCORD: Found RemoteRPC List for cars.json"); }
                    if (CarsList.remoteCarsList == String.Empty) { Log.Warning("DISCORD: RemoteRPC List for cars.json does not exist"); }

                    try
                    {
                        EventsList.remoteEventsList = new WebClient().DownloadString(json2.BasePath + "/events.json");
                    }
                    catch { }
                    if (EventsList.remoteEventsList != String.Empty) { Log.Info("DISCORD: Found RemoteRPC List for events.json"); }
                    if (EventsList.remoteEventsList == String.Empty) { Log.Warning("DISCORD: RemoteRPC List for events.json does not exist"); }

                    //get new index
                    Uri newIndexFile = new Uri(json2.BasePath + "/index.json");
                    Log.Core("CORE: Loading Server Mods List");
                    String jsonindex = new WebClient().DownloadString(newIndexFile);

                    IndexJson json3 = JsonConvert.DeserializeObject<IndexJson>(jsonindex);

                    int CountFilesTotal = 0;
                    CountFilesTotal = json3.Entries.Count;

                    String path = Path.Combine(_settingFile.Read("InstallationDirectory"), "MODS", MDFive.HashPassword(json2.ServerID).ToLower());
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    foreach (IndexJsonEntry modfile in json3.Entries)
                    {
                        if (SHA.HashFile(path + "/" + modfile.Name).ToLower() != modfile.Checksum) 
                        {
                            modFilesDownloadUrls.Enqueue(new Uri(json2.BasePath + "/" + modfile.Name));
                            TotalModFileCount++;
                        }
                    }

                    if (modFilesDownloadUrls.Count != 0)
                    {
                        this.DownloadModNetFilesRightNow(path);
                        _presence.State = "Downloading Server Mods!";
                        if (discordRpcClient != null) discordRpcClient.SetPresence(_presence);
                    }
                    else
                    {
                        LaunchGame();
                    }

                    foreach (var file in Directory.GetFiles(path))
                    {
                        var name = Path.GetFileName(file);

                        if (json3.Entries.All(en => en.Name != name)) 
                        {
                            Log.Core("LAUNCHER: removing package: " + file);
                            try
                            {
                                File.Delete(file);
                            }
                            catch(Exception ex)
                            {
                                Log.Error($"Failed to remove {file}: {ex.Message}");
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log.Error("LAUNCHER " + ex.Message);
                    MessageBox.Show(null, $"There was an error downloading ModNet Files:\n{ex.Message}", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
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
                    Log.Core("CLEANLINKS: Found Server Mod Files to remove {Process}");
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
            }
        }

        void Client_DownloadProgressChanged_RELOADED(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                PlayProgressText.Text = ("["+CurrentModFileCount+" / "+TotalModFileCount+"] Downloading " + ModNetFileNameInUse + ": " + FormatFileSize(e.BytesReceived) + " of " + FormatFileSize(e.TotalBytesToReceive)).ToUpper();

                ExtractingProgress.Value = Convert.ToInt32(Decimal.Divide(e.BytesReceived, e.TotalBytesToReceive) * 100);
                ExtractingProgress.Width = Convert.ToInt32(Decimal.Divide(e.BytesReceived, e.TotalBytesToReceive) * 519);
            });
        }

        //Launch game
        public void LaunchGame()
        {
            if (_serverInfo.DiscordAppId != null)
            {
                discordRpcClient.Dispose();
                discordRpcClient = null;
                discordRpcClient = new DiscordRpcClient(_serverInfo.DiscordAppId);
                discordRpcClient.Initialize();
            }

            if ((_disableDiscordRPC == false) && ((ServerInfo)ServerPick.SelectedItem).Category == "DEV")
            {
                discordRpcClient.Dispose();
                discordRpcClient = null;
            }

            try
            {
                if
                  (
                    SHA.HashFile(_settingFile.Read("InstallationDirectory") + "/nfsw.exe") == "7C0D6EE08EB1EDA67D5E5087DDA3762182CDE4AC" ||
                    SHA.HashFile(_settingFile.Read("InstallationDirectory") + "/nfsw.exe") == "DB9287FB7B0CDA237A5C3885DD47A9FFDAEE1C19" ||
                    SHA.HashFile(_settingFile.Read("InstallationDirectory") + "/nfsw.exe") == "E69890D31919DE1649D319956560269DB88B8F22"
                  )
                {
                    ServerProxy.Instance.SetServerUrl(_serverIp);
                    ServerProxy.Instance.SetServerName(_realServername);

                    AntiCheat.user_id = _userId;
                    AntiCheat.serverip = new Uri(_serverIp).Host;

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
                            PlayProgressText.Text = string.Format("Loading game. Launcher will minimize in {0} seconds.", secondsToCloseLauncher).ToUpper(); //"LOADING GAME. LAUNCHER WILL MINIMIZE ITSELF IN " + secondsToCloseLauncher + " SECONDS";
                            Delay.WaitSeconds(1);
                            secondsToCloseLauncher--;
                        }

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

            PlayButton.BackgroundImage = Properties.Resources.playbutton_hover;
        }

        private void PlayButton_MouseDown(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            PlayButton.BackgroundImage = Properties.Resources.playbutton_click;
        }

        private void PlayButton_MouseEnter(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            PlayButton.BackgroundImage = Properties.Resources.playbutton_hover;
        }

        private void PlayButton_MouseLeave(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            PlayButton.BackgroundImage = Properties.Resources.playbutton;
        }

        private void LaunchNfsw()
        {
            PlayButton.BackgroundImage = Properties.Resources.playbutton;
            PlayButton.ForeColor = Color.Gray;

            PlayProgressText.Text = "Checking up all files".ToUpper();
            PlayProgress.Width = 0;
            ExtractingProgress.Width = 0;

            string speechFile;

            try
            {
                speechFile = string.IsNullOrEmpty(_settingFile.Read("Language")) ? "en" : _settingFile.Read("Language").ToLower();
            }
            catch (Exception)
            {
                speechFile = "en";
            }

            if (!File.Exists(_settingFile.Read("InstallationDirectory") + "/Sound/Speech/copspeechhdr_" + speechFile + ".big"))
            {
                PlayProgressText.Text = "Loading list of files to download...".ToUpper();

                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    if (d.Name == Path.GetPathRoot(_settingFile.Read("InstallationDirectory")))
                    {
                        if (d.TotalFreeSpace <= 10000000000)
                        {

                            ExtractingProgress.Value = 100;
                            ExtractingProgress.Width = 519;
                            ExtractingProgress.Image = Properties.Resources.progress_warning;
                            ExtractingProgress.ProgressColor = Color.Orange;

                            PlayProgressText.Text = "Please make sure you have at least 10GB free space on hard drive.".ToUpper();

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
            if (File.Exists(_settingFile.Read("InstallationDirectory") + "/TracksHigh/STREAML5RA_98.BUN"))
            {
                Directory.Delete(_settingFile.Read("InstallationDirectory") + "/TracksHigh", true);
            }
        }

        public void DownloadCoreFiles()
        {
            PlayProgressText.Text = "Checking Core Files...".ToUpper();
            PlayProgress.Width = 0;
            ExtractingProgress.Width = 0;

            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);

            //Guess who is Back - DavidCarbon
            if (File.Exists(filename_pack))
            {
                PlayProgressTextTimer.Visible = true;
                PlayProgressText.Text = "Local GameFiles sbrwpack Found In Launcher Folder".ToUpper();
                PlayProgressTextTimer.Text = "Loading".ToUpper() ;

                //GameFiles.sbrwpack
                LocalGameFiles();
            }
            else if (!File.Exists(_settingFile.Read("InstallationDirectory") + "/nfsw.exe"))
            {
                _downloadStartTime = DateTime.Now;
                Log.Info("DOWNLOAD: Getting Core Game Files");
                _downloader.StartDownload(_NFSW_Installation_Source, "", _settingFile.Read("InstallationDirectory"), false, false, 1130632198);
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

            if (!File.Exists(_settingFile.Read("InstallationDirectory") + "/Tracks/STREAML5RA_98.BUN"))
            {
                _downloadStartTime = DateTime.Now;
                Log.Info("DOWNLOAD: Getting Tracks Folder");
                _downloader.StartDownload(_NFSW_Installation_Source, "Tracks", _settingFile.Read("InstallationDirectory"), false, false, 615494528);
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
                    WebClient wc = new WebClient();
                    var response = wc.DownloadString(_NFSW_Installation_Source + "/" + _settingFile.Read("Language").ToLower() + "/index.xml");

                    response = response.Substring(3, response.Length - 3);

                    var speechFileXml = new XmlDocument();
                    speechFileXml.LoadXml(response);
                    var speechSizeNode = speechFileXml.SelectSingleNode("index/header/compressed");

                    speechFile = _settingFile.Read("Language").ToLower();
                    speechSize = Convert.ToInt32(speechSizeNode.InnerText);
                    _langInfo = SettingsLanguage.GetItemText(SettingsLanguage.SelectedItem).ToUpper();
                }
            }
            catch (Exception)
            {
                speechFile = "en";
                speechSize = 141805935;
                _langInfo = "ENGLISH";
            }

            PlayProgressText.Text = string.Format("Checking for {0} Speech Files.", _langInfo).ToUpper();

            if (!File.Exists(_settingFile.Read("InstallationDirectory") + "\\Sound\\Speech\\copspeechsth_" + speechFile + ".big"))
            {
                _downloadStartTime = DateTime.Now;
                Log.Info("DOWNLOAD: Getting Speech/Audio Files");
                _downloader.StartDownload(_NFSW_Installation_Source, speechFile, _settingFile.Read("InstallationDirectory"), false, false, speechSize);
            }
            else
            {
                OnDownloadFinished();
                Log.Info("DOWNLOAD: Game Files Download is Complete!");
            }
        }

        //Check Local GameFiles Hash
        private async void LocalGameFiles()
        {
            await Task.Delay(5000);
            if (SHA.HashFile("GameFiles.sbrwpack") == "B42E00939DC656C14BF5A05644080AD015522C8C")
            {
                TaskbarProgress.SetValue(Handle, 100, 100);
                PlayProgress.Value = 100;
                PlayProgress.Width = 519;

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

                            ExtractingProgress.Value = (int)((long)100 * current / numFiles);
                            ExtractingProgress.Width = (int)((long)519 * current / numFiles);

                            TaskbarProgress.SetValue(Handle, (int)(100 * current / numFiles), 100);

                            if (!File.Exists(Path.Combine(_settingFile.Read("InstallationDirectory"), fullName.Replace(".sbrw", String.Empty))))
                            {
                                PlayProgressText.Text = ("Unpacking " + fullName.Replace(".sbrw", String.Empty)).ToUpper();
                                PlayProgressTextTimer.Text = "[" + current + " / " + archive.Entries.Count + "]";


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
                                PlayProgressText.Text = ("Skipping " + fullName).ToUpper();
                            }

                            _presence.State = "Unpacking game: " + (100 * current / numFiles) + "%";
                            if (discordRpcClient != null) discordRpcClient.SetPresence(_presence);

                            Application.DoEvents();

                            if (numFiles == current)
                            {
                                PlayProgressTextTimer.Visible = false;
                                PlayProgressTextTimer.Text = "";

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

        private string FormatFileSize(long byteCount, bool si = true)
        {
            int unit = si ? 1000 : 1024;
            if (byteCount < unit) return byteCount + " B";
            int exp = (int)(Math.Log(byteCount) / Math.Log(unit));
            String pre = (si ? "kMGTPE" : "KMGTPE")[exp - 1] + (si ? "" : "i");
            return String.Format("{0}{1}B", Convert.ToDecimal(byteCount / Math.Pow(unit, exp)).ToString("0.##"), pre);
        }

        private string EstimateFinishTime(long current, long total)
        {
            try
            {
                var num = current / (double)total;
                if (num < 0.00185484899838312)
                {
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
            }
            catch
            {
                return "N/A";
            }
        }

        private void OnDownloadProgress(long downloadLength, long downloadCurrent, long compressedLength, string filename, int skiptime = 0)
        {
            if (downloadCurrent < compressedLength)
            {
                PlayProgressText.Text = String.Format("Downloading — {0} of {1} ({3}%) — {2}", FormatFileSize(downloadCurrent), FormatFileSize(compressedLength), EstimateFinishTime(downloadCurrent, compressedLength), (int)(100 * downloadCurrent / compressedLength)).ToUpper();
            }

            try
            {
                PlayProgress.Value = (int)(100 * downloadCurrent / compressedLength);
                PlayProgress.Width = (int)(519 * downloadCurrent / compressedLength);

                _presence.State = string.Format("Downloaded {0}% of the Game!", (int)(100 * downloadCurrent / compressedLength));
                if (discordRpcClient != null) discordRpcClient.SetPresence(_presence);

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
                File.WriteAllBytes(_settingFile.Read("InstallationDirectory") + "/GFX/BootFlow.gfx", ExtractResource.AsByte("GameLauncher.SoapBoxModules.BootFlow.gfx"));
            }
            catch
            {
                // ignored
            }

            if (_settingFile.KeyExists("InstallationDirectory"))
            {
                //Remove current Firewall for the Game Files 
                string CurrentGameFilesExePath = Path.Combine(_settingFile.Read("InstallationDirectory") + "\\nfsw.exe");

                if (File.Exists(CurrentGameFilesExePath) && FirewallHelper.RuleExist("SBRW - Game") == false)
                {
                    bool removeFirewallRule = false;
                    bool firstTimeRun = true;

                    string nameOfGame = "SBRW - Game";
                    string localOfGame = CurrentGameFilesExePath;

                    string groupKeyGame = "Need for Speed: World";
                    string descriptionGame = groupKeyGame;

                    //Inbound & Outbound
                    FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfGame, localOfGame, groupKeyGame, descriptionGame, FirewallProtocol.Any);
                }
                else
                {
                    Log.Core("WINDOWS FIREWALL: Already Exlcuded SBRW - Game {Both}");
                }
            }

            PlayProgressText.Text = "Ready!".ToUpper();
            _presence.State = "Ready!";
            if (discordRpcClient != null) discordRpcClient.SetPresence(_presence);

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

            PlayButton.BackgroundImage = Properties.Resources.playbutton;
            PlayButton.ForeColor = Color.White;
        }

        private void DisablePlayButton()
        {
            _isDownloading = false;
            _playenabled = false;

            ShowPlayPanel.Visible = false;

            ExtractingProgress.Value = 100;
            ExtractingProgress.Width = 519;

            PlayButton.BackgroundImage = Properties.Resources.graybutton;
            PlayButton.ForeColor = Color.White;
        }

        private void OnDownloadFailed(Exception ex)
        {
            string failureMessage;
            MessageBox.Show(null, "Failed to download gamefiles. \n\nCDN might be offline. \n\nPlease select a different CDN on Next Screen", "GameLauncher - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //CDN Went Offline Screen switch - DavidCarbon
            CDN_Offline_Switch();

            try
            {
                failureMessage = ex.Message;
            } catch {
                failureMessage = "Download failed.";
            }

            _presence.State = "Failed to Downloaded Game!";
            if (discordRpcClient != null) discordRpcClient.SetPresence(_presence);

            ExtractingProgress.Value = 100;
            ExtractingProgress.Width = 519;
            ExtractingProgress.Image = Properties.Resources.progress_error;
            ExtractingProgress.ProgressColor = Color.FromArgb(254,0,0);

            PlayProgressText.Text = failureMessage.ToUpper();

            TaskbarProgress.SetValue(Handle, 100, 100);
            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Error);
        }

        private void OnShowExtract(string filename, long currentCount, long allFilesCount) {
            if (PlayProgress.Value == 100)
            {
                PlayProgressText.Text = String.Format("Extracting — {0} of {1} ({3}%) — {2}", FormatFileSize(currentCount), FormatFileSize(allFilesCount), EstimateFinishTime(currentCount, allFilesCount), (int)(100 * currentCount / allFilesCount)).ToUpper();
            }

            ExtractingProgress.Value = (int)(100 * currentCount / allFilesCount);
            ExtractingProgress.Width = (int)(519 * currentCount / allFilesCount);
        }

        private void OnShowMessage(string message, string header)
        {
            MessageBox.Show(message, header);
        }

        public void ServerStatusBar(Pen color, Point startPoint, Point endPoint, int Thickness = 2)
        {
            Graphics _formGraphics = CreateGraphics();
            
            for (int x = 0; x <= Thickness; x++)
            {
                _formGraphics.DrawLine(color, new Point(startPoint.X, startPoint.Y-x), new Point(endPoint.X, endPoint.Y-x));
            }

            _formGraphics.Dispose();
        }

        private void SelectServerBtn_Click(object sender, EventArgs e)
        {
            new SelectServer().ShowDialog();

            if (ServerName != null)
            {
                this.SelectServerBtn.Text = "[...] " + ServerName.Name;

                var index = finalItems.FindIndex(i => string.Equals(i.IpAddress, ServerName.IpAddress));
                ServerPick.SelectedIndex = index;
            }
        }

        private void SettingsClearCrashLogsButton_Click(object sender, EventArgs e)
        {
            var crashLogFilesDirectory = new DirectoryInfo(_settingFile.Read("InstallationDirectory"));

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

        private void SettingsClearCommunicationLogButton_Click(object sender, EventArgs e)
        {
            File.Delete(_settingFile.Read("InstallationDirectory") + "/NFSWO_COMMUNICATION_LOG.txt");
            MessageBox.Show(null, "Deleted NFSWO Communication Log", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingsClearCommunicationLogButton.Enabled = false;
        }

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
                SettingsCDNText.ForeColor = Color.FromArgb(224, 224, 224);
            }

            _lastSelectedCdnId = SettingsCDNPick.SelectedIndex;
        }

        private void PatchNotes_Click(object sender, EventArgs e)
        {
            new About().Show();
        }

        private void DiscordInviteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_serverDiscordLink != null)
                Process.Start(_serverDiscordLink);
        }

        private void HomePageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_serverWebsiteLink != null)
                Process.Start(_serverWebsiteLink);
        }

        private void FacebookGroupLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_serverFacebookLink != null)
                Process.Start(_serverFacebookLink);
        }

        private void TwitterAccountLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_serverTwitterLink != null)
                Process.Start(_serverTwitterLink);
        }
        private void WindowsDefenderFirstRun()
        {
            // Create Windows Defender Exclusion
            try
            {
                Log.Info("WINDOWS DEFENDER: Excluding Core Folders");
                //Add Exclusion to Windows Defender
                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddScript($"Add-MpPreference -ExclusionPath \"{AppDomain.CurrentDomain.BaseDirectory}\"");
                    ps.AddScript($"Add-MpPreference -ExclusionPath \"{_settingFile.Read("InstallationDirectory")}\"");
                    var result = ps.Invoke();
                }
                _settingFile.Write("WindowsDefender", "Excluded");
            }
            catch
            {
                Log.Error("WINDOWS DEFENDER: Failed to Exclude Folders");
                _settingFile.Write("WindowsDefender", "Not Excluded");
            }
        }

        private void WindowsDefenderGameFilesDirctoryChange()
        {
            //Remove current Exclusion and Add new location for Exclusion
            using (PowerShell ps = PowerShell.Create())
            {
                Log.Warning("WINDOWS DEFENDER: Removing OLD Game Files Directory: " + _settingFile.Read("InstallationDirectory"));
                ps.AddScript($"Remove-MpPreference -ExclusionPath \"{_settingFile.Read("InstallationDirectory")}\"");
                Log.Core("WINDOWS DEFENDER: Excluding NEW Game Files Directory: " + _newGameFilesPath);
                ps.AddScript($"Add-MpPreference -ExclusionPath \"{_newGameFilesPath}\"");
                var result = ps.Invoke();
            }

            //Remove current Firewall for the Game Files 
            string CurrentGameFilesExePath = Path.Combine(_settingFile.Read("InstallationDirectory") + "\\nfsw.exe");

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

            _settingFile.Write("InstallationDirectory", _newGameFilesPath);

            if (!DetectLinux.LinuxDetected())
            {
                CheckGameFilesDirectoryPrevention();
            }

            _restartRequired = true;
            //Clean Mods Files from New Dirctory (If it has .links in directory)
            var linksPath = Path.Combine(_settingFile.Read("InstallationDirectory"), ".links");
            if (File.Exists(linksPath))
            {
                Log.Core("CLEANLINKS: Cleaning Up Mod Files {Settings}");
                CleanLinks(linksPath);
            }
        }

        private void RememberLastCDN()
        {
            /* Last Selected CDN */
            Log.Core("SETTINGS CDNLIST: Checking...");
            Log.Core("SETTINGS CDNLIST: Setting first server in list");
            Log.Core("SETTINGS CDNLIST: Checking if server is set on INI File");

            if (_settingFile.KeyExists("CDN"))
            {
                Log.Core("SETTINGS CDNLIST: Found something!");
                Log.Core("SETTINGS CDNLIST: Checking if CDN exists on our database");

                if (finalCDNItems.FindIndex(i => string.Equals(i.Url, _settingFile.Read("CDN"))) != 0)
                {
                    Log.Core("SETTINGS CDNLIST: CDN found! Checking ID");
                    var index = finalCDNItems.FindIndex(i => string.Equals(i.Url, _settingFile.Read("CDN")));

                    Log.Core("SETTINGS CDNLIST: ID is " + index);
                    if (index >= 0)
                    {
                        Log.Core("SETTINGS CDNLIST: ID set correctly");
                        SettingsCDNPick.SelectedIndex = index;
                    }
                    else if (index < 0)
                    {
                        Log.Warning("SETTINGS CDNLIST: Old CDN URL Standard Detected!");
                        SettingsCDNPick.SelectedIndex = 1;
                        Log.Warning("SETTINGS CDNLIST: Displaying First CDN in List!");
                    }
                }
                else
                {
                    Log.Warning("SETTINGS CDNLIST: Unable to find anything, assuming default");
                    SettingsCDNPick.SelectedIndex = 1;
                    Log.Warning("SETTINGS CDNLIST: Unknown entry value is " + _settingFile.Read("CDN"));
                }
                Log.Core("SETTINGS CDNLIST: All done");
            }
        }

        private void CheckGameFilesDirectoryPrevention()
        {
            switch (Self.CheckFolder(_settingFile.Read("InstallationDirectory"))) 
            {
                case FolderType.IsSameAsLauncherFolder:
                        Directory.CreateDirectory("Game Files");
                        Log.Error("LAUNCHER: Installing NFSW in same directory where the launcher resides is disadvised.");
                        MessageBox.Show(null, string.Format("Installing NFSW in same directory where the launcher resides is disadvised. Instead, we will install it at {0}.", AppDomain.CurrentDomain.BaseDirectory + "Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _settingFile.Write("InstallationDirectory", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files");
                    break;
                case FolderType.IsTempFolder:
                        Directory.CreateDirectory("Game Files");
                        Log.Error("LAUNCHER: (╯°□°）╯︵ ┻━┻ Installing NFSW in the Temp Folder is disadvised!");
                        MessageBox.Show(null, string.Format("(╯°□°）╯︵ ┻━┻\n\nInstalling NFSW in the Temp Folder is disadvised! Instead, we will install it at {0}.", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files" + "\n\n┬─┬ ノ( ゜-゜ノ)"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _settingFile.Write("InstallationDirectory", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files");
                    break;
                case FolderType.IsProgramFilesFolder:
                case FolderType.IsUsersFolders:
                case FolderType.IsWindowsFolder:
                        Directory.CreateDirectory("Game Files");
                        Log.Error("LAUNCHER: Installing NFSW in a Special Directory is disadvised.");
                        MessageBox.Show(null, string.Format("Installing NFSW in a Special Directory is disadvised. Instead, we will install it at {0}.", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _settingFile.Write("InstallationDirectory", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files");
                    break;
            }
        }

        private void SettingsClearServerModCacheButton_Click(object sender, EventArgs e)
        {
            Directory.Delete(_settingFile.Read("InstallationDirectory") + "/.data", true);
            Directory.Delete(_settingFile.Read("InstallationDirectory") + "/MODS", true);
            Log.Warning("LAUNCHER: User Confirmed to Delete Server Mods Cache");
            MessageBox.Show(null, "Deleted Server Mods Cache", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingsClearServerModCacheButton.Enabled = false;
        }

        private void SettingsVFilesButton_Click(object sender, EventArgs e)
        {
            new VerifyHash().ShowDialog();
        }
    }
    /* Moved 7 Unused Code to Gist */
    /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a8F89a5e5aebce38/raw/00de505302fbf9f8cfea9b163a707d9f8f122552/MainScreen.cs */
}
