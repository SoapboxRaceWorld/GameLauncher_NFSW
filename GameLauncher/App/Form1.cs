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
using ShadowDemo;
using Security;

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
        private bool _serverlistloaded;
        private bool _windowMoved;
        private bool _playenabled;
        private bool _loggedIn;
        private bool _restartRequired;
        private bool _allowRegistration;
        //private bool _isIndex = false;
        private bool _isDownloading = true;

        private int _lastSelectedServerId;
        private int _nfswPid;
        private Thread _nfswstarted;
        private string _passwordHash;
        private string _slresponse = "";
        private readonly string _discordrpccode = "427355155537723393";

        private int _errorcode;

        private DateTime _downloadStartTime;
        private readonly Downloader _downloader;

        private string _loginToken = "";
        private string _userId = "";
        private string _serverIp = "";
        private readonly string _serverCacheKey = "02032019"; // Try to guess that now :)
        private string _langInfo;
        private string _uiLanguage;
        private string _newGameFilesPath;
        private readonly float _dpiDefaultScale = 96f;

        private readonly DiscordRpc.RichPresence _presence = new DiscordRpc.RichPresence();

        private readonly Pen _colorOffline = new Pen(Color.FromArgb(128, 0, 0));
        private readonly Pen _colorOnline = new Pen(Color.FromArgb(0, 128, 0));
        private readonly Pen _colorLoading = new Pen(Color.FromArgb(0, 0, 0));
        private readonly Pen _colorIssues = new Pen(Color.FromArgb(255, 145, 0));

        private readonly IniFile _settingFile = new IniFile("Settings.ini");
        private readonly string _userSettings = WineManager.GetUserSettingsPath();
        private string _presenceImageKey;
        private string _NFSW_Installation_Source;
        private string _blacklistedXML;
        private string _realServername;
        private string _OS;

        private Point _startPoint = new Point(38, 144);
        private Point _endPoint = new Point(562, 144);

        Form _splashscreen;

        private static Random random = new Random();
		public static string RandomString(int length) {
			const string chars = "qwertyuiopasdfghjklzxcvbnm1234567890_";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

        protected override void OnPaint(PaintEventArgs e){
            //var p = new Pen(Color.FromArgb(10, 17, 25));
            //e.Graphics.DrawRectangle(p, new Rectangle(new Point(0, 0), new Size(Size.Width - 1, Size.Height - 1)));
            //e.Graphics.DrawRectangle(p, new Rectangle(new Point(2, 2), new Size(Size.Width - 5, Size.Height - 5)));
        }

        private void moveWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Y <= 90) _mouseDownPoint = new Point(e.X, e.Y);
        }

        private void moveWindow_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDownPoint = Point.Empty;
            //this.Refresh();
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

        public void Shake()
        {
            for (var i = 0; i < 5; i++)
            {
                Left += 10;
                Thread.Sleep(40);
                Left -= 10;
                Thread.Sleep(40);
            }
        }

        public void ConsoleLog(string e, string type)
        {
            switch (type)
            {
                case "warning":
                    break;
                case "info":
                    break;
                case "error":
                    Shake();
                    MessageBox.Show(null, e, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "success":
                    break;
                case "ping":
                    break;
            }
        }

        public MainScreen(Form splashscreen)
        {
            _splashscreen = splashscreen;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (DetectLinux.UnixDetected()) {
                _OS = DetectLinux.Distro();
            } else {
                _OS = (string)Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("productName");
            }

            _downloader = new Downloader(this, 3, 2, 16)
            {
                //DOWNLOADFUNCTIONS
                ProgressUpdated = new ProgressUpdated(OnDownloadProgress),
                DownloadFinished = new DownloadFinished(DownloadTracksFiles),
                DownloadFailed = new DownloadFailed(OnDownloadFailed),
                ShowMessage = new ShowMessage(OnShowMessage),
				ShowExtract = new ShowExtract(OnShowExtract)
            };

            if (Environment.OSVersion.Version.Major > 5)
            {
                Font = new Font(Font.Name, 8.25f * _dpiDefaultScale / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            }

            InitializeComponent();
            if (!DetectLinux.UnixDetected())
            {
                ApplyEmbeddedFonts();
            }

            if (_settingFile.KeyExists("LauncherPosX") || _settingFile.KeyExists("LauncherPosY"))
            {
                StartPosition = FormStartPosition.Manual;
                var posX = int.Parse(_settingFile.Read("LauncherPosX"));
                var posY = int.Parse(_settingFile.Read("LauncherPosY"));
                Location = new Point(posX, posY);
            }
            else
            {
                Self.centerScreen(this);
            }

            MaximizeBox = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            closebtn.MouseEnter += new EventHandler(closebtn_MouseEnter);
            closebtn.MouseLeave += new EventHandler(closebtn_MouseLeave);
            closebtn.Click += new EventHandler(closebtn_Click);

            settingsButton.MouseEnter += new EventHandler(settingsButton_MouseEnter);
            settingsButton.MouseLeave += new EventHandler(settingsButton_MouseLeave);
            settingsButton.Click += new EventHandler(settingsButton_Click);

            //minimizebtn.MouseEnter += new EventHandler(minimizebtn_MouseEnter);
            //minimizebtn.MouseLeave += new EventHandler(minimizebtn_MouseLeave);
            //minimizebtn.Click += new EventHandler(minimizebtn_Click);

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
            launcherVersion.Click += new EventHandler(OpenDebugWindow);
            showmap.Click += new EventHandler(OpenMapHandler);

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

            //Simple check if we have enough permission to write file and remove them
            if (!Self.hasWriteAccessToFolder(Directory.GetCurrentDirectory()))
            {
                MessageBox.Show(null, "Failed to write the test file. Make sure you're running the launcher with administrative privileges.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Somewhere here we will setup the game installation directory
            if (string.IsNullOrEmpty(_settingFile.Read("InstallationDirectory")))
            {
                MessageBox.Show(null, "Howdy! Looks like it's the first time this launcher is started. Please press OK and specify where you want to download all required game files (or select your actual installation).", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var fbd = new FolderBrowserDialog();
                var result = fbd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (!Self.hasWriteAccessToFolder(fbd.SelectedPath))
                    {
                        MessageBox.Show(null, "You don't have enough permission to select this path as installation folder. Please select another directory.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Environment.Exit(Environment.ExitCode);
                    }

                    if (fbd.SelectedPath == Environment.CurrentDirectory)
                    {
                        Directory.CreateDirectory("GameFiles");
                        MessageBox.Show(null, string.Format("Installing NFSW in same directory where the launcher resides is disadvised. Instead, we will install it on {0}.", Environment.CurrentDirectory + "\\GameFiles"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _settingFile.Write("InstallationDirectory", Environment.CurrentDirectory + "\\GameFiles");
                    }
                    else
                    {
                        _settingFile.Write("InstallationDirectory", fbd.SelectedPath);
                    }
                }
                else
                {
                    Environment.Exit(Environment.ExitCode);
                }
            }

            //Replace cursor
            if (File.Exists(_settingFile.Read("InstallationDirectory") + "\\Media\\Cursors\\default.cur"))
            {
                var mycursor = new Cursor(Cursor.Current.Handle);
                var colorcursorhandle = User32.LoadCursorFromFile(_settingFile.Read("InstallationDirectory") + "\\Media\\Cursors\\default.cur");
                mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, null, mycursor, new object[] { colorcursorhandle });
                Cursor = mycursor;
            }

            var pos = PointToScreen(imageServerName.Location);
            pos = verticalBanner.PointToClient(pos);
            imageServerName.Parent = verticalBanner;
            imageServerName.Location = pos;
            imageServerName.BackColor = Color.Transparent;

            ServerStatusBar(_colorLoading, _startPoint, _endPoint);

            if (Self.CheckForInternetConnection() == false && !DetectLinux.WineDetected())
            {
                MessageBox.Show(null, "Failed to connect to internet. Please check if your firewall is not blocking launcher.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ModManager.LoadModCache();
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var font = (sender as ComboBox).Font;
            var backgroundColor = Brushes.White;
            var textColor = Brushes.Black;

            var serverListText = "";

            if (sender is ComboBox cb)
            {
                if (cb.Items[e.Index] is ServerInfo si)
                {
                    serverListText = si.Name;
                }
            }

            e.Graphics.FillRectangle(backgroundColor, e.Bounds);

            if (serverListText.StartsWith("<GROUP>"))
            {
                font = new Font(font, FontStyle.Bold);
                e.Graphics.DrawString(serverListText.Replace("<GROUP>", string.Empty), font, textColor, e.Bounds);
            }
            else
            {
                font = new Font(font, FontStyle.Regular);
                e.Graphics.DrawString("    " + serverListText, font, textColor, e.Bounds);
            }
        }

        private void mainScreen_Load(object sender, EventArgs e)
        {
            var f = new Dropshadow(this) {
                ShadowBlur = 40,
                ShadowSpread = -28,
                ShadowColor = Color.Black
            };

            f.RefreshShadow();

            Text = "GameLauncherReborn v" + Application.ProductVersion;

            if (Location.X >= Screen.PrimaryScreen.Bounds.Width || Location.Y >= Screen.PrimaryScreen.Bounds.Height || Location.X <= 0 || Location.Y <= 0)
            {
                Self.centerScreen(this);
                _windowMoved = true;
            }
            _NFSW_Installation_Source = _settingFile.KeyExists("CDN") ? _settingFile.Read("CDN") : "http://static.cdn.ea.com/blackbox/u/f/NFSWO/1614b/client";

            launcherVersion.Text = "v" + Application.ProductVersion + "build-" + WebClientWithTimeout.createHash(AppDomain.CurrentDomain.FriendlyName).Substring(0, 10);
            translatedBy.Text = ""; //Empty

            if (!_settingFile.KeyExists("SkipUpdate"))
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    Updater.CheckForUpdate(sender, e);
                }
                else
                {
                    _settingFile.Write("SkipUpdate", "1");
                }
            }



            ContextMenu = new ContextMenu();

            if (Environment.OSVersion.Version.Major >= 6)
            {
                ContextMenu.MenuItems.Add(new MenuItem("Check for updates.", Updater.CheckForUpdate));
            }

            ContextMenu.MenuItems.Add(new MenuItem("About", About.showAbout));
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
            if (!string.IsNullOrEmpty(_settingFile.Read("AccountEmail")) && !string.IsNullOrEmpty(_settingFile.Read("Password")))
            {
                rememberMe.Checked = true;
            }

            try
            {
                WebClient wc = new WebClientWithTimeout();

                var serverurl = Self.serverlisturl;
                _slresponse += wc.DownloadString(serverurl);

                _serverlistloaded = true;

                try
                {
                    var fileStream = new FileStream("ServerCache.json", FileMode.Create);

                    var dEsCryptoServiceProvider = new DESCryptoServiceProvider()
                    {
                        Key = Encoding.ASCII.GetBytes(_serverCacheKey),
                        IV = Encoding.ASCII.GetBytes(_serverCacheKey)
                    };

                    var cryptoStream = new CryptoStream(fileStream, dEsCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
                    var streamWriter = new StreamWriter(cryptoStream);
                    streamWriter.Write(_slresponse);
                    streamWriter.Close();
                }
                catch
                {
                    // ignored
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                if (File.Exists("ServerCache.json"))
                {
                    var fileStream = new FileStream("ServerCache.json", FileMode.Open);

                    var dEsCryptoServiceProvider = new DESCryptoServiceProvider()
                    {
                        Key = Encoding.ASCII.GetBytes(_serverCacheKey),
                        IV = Encoding.ASCII.GetBytes(_serverCacheKey)
                    };

                    var cryptoStream = new CryptoStream(fileStream, dEsCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read);
                    var streamReader = new StreamReader(cryptoStream);
                    _slresponse = streamReader.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(_slresponse))
                    {
                        _slresponse = "[]";
                    }

                    _serverlistloaded = true;
                }
                else
                {
                    _slresponse = JsonConvert.SerializeObject(new[]
                    {
                        new ServerInfo
                        {
                            Category = "OFFLINE",
                            Name = "Offline Built-In Server",
                            IpAddress = "http://localhost:4416/sbrw/Engine.svc",
                            Id = "__offlinebuiltin__"
                        }
                    });
                }
            }


            serverPick.DisplayMember = "Name";

            var resItems = JsonConvert.DeserializeObject<List<ServerInfo>>(_slresponse);

            var finalItems = new List<ServerInfo>();

            foreach (var serverItemGroup in resItems.GroupBy(s => s.Category))
            {
                finalItems.Add(new ServerInfo
                {
                    Id = $"__category-{serverItemGroup.Key}__",
                    Name = $"<GROUP>{serverItemGroup.Key} Servers",
                    IsSpecial = true
                });

                finalItems.AddRange(serverItemGroup.ToList());
            }


            finalItems.Add(
                new ServerInfo {
                    Category = "NightRiderz",
                    Name = "NightRiderz",
                    IpAddress = "http://149.202.87.33:8680/soapbox-race-core/Engine.svc",
                    Id = "nr",
                    DiscordPresenceKey = "nightriderzserver",
                }
            );

            //List<Object> items = new List<Object>();
            //String[] substrings = slresponse.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            //foreach (var substring in substrings)
            //{
            //    if (!String.IsNullOrEmpty(substring))
            //    {
            //        String[] substrings2 = substring.Split(new string[] { ";" }, StringSplitOptions.None);
            //        items.Add(new { Text = substrings2[0], Value = substrings2[1] });
            //        serverKeyMap[substrings2[1]] = substrings2[2];
            //    }
            //}

            if (File.Exists("servers.json"))
            {
                var fileItems = JsonConvert.DeserializeObject<List<ServerInfo>>(File.ReadAllText("servers.json")) ?? new List<ServerInfo>();

                if (fileItems.Count > 0)
                {
                    finalItems.Add(new ServerInfo
                    {
                        Id = "__category-CUSTOMCUSTOM__",
                        Name = "<GROUP>Custom Servers",
                        IsSpecial = true
                    });

                    finalItems.AddRange(fileItems.Select(si =>
                    {
                        si.DistributionUrl = "";
                        si.DiscordPresenceKey = "";
                        si.Id = SHA.HashPassword($"{si.Name}:{si.Id}:{si.IpAddress}");
                        si.IsSpecial = false;
                        si.Category = "CUSTOMCUSTOM";

                        return si;
                    }));
                }
                //try
                //{
                //    items.Add(new { Text = "<GROUP>Custom Servers", Value = "" });
                //    response2 += File.ReadAllText("servers.txt");

                //    String[] substrings_custom = response2.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                //    foreach (var substring in substrings_custom)
                //    {
                //        if (!String.IsNullOrEmpty(substring))
                //        {
                //            String[] substrings2 = substring.Split(new string[] { ";" }, StringSplitOptions.None);
                //            items.Add(new { Text = substrings2[0], Value = substrings2[1] });
                //        }
                //    }
                //}
                //catch
                //{

                //}
            }

            if (File.Exists("libOfflineServer.dll"))
            {
                finalItems.Add(new ServerInfo
                {
                    Id = "__category-OFFLINEOFFLINE__",
                    Name = "<GROUP>Offline Server",
                    IsSpecial = true
                });

                finalItems.Add(new ServerInfo
                {
                    Name = "Offline Built-In Server",
                    Category = "OFFLINEOFFLINE",
                    DiscordPresenceKey = "",
                    IsSpecial = false,
                    DistributionUrl = "",
                    IpAddress = "http://localhost:4416/sbrw/Engine.svc",
                    Id = "OFFLINE"
                });
                //items.Add(new { Text = "<GROUP>Offline Server", Value = "" });
                //items.Add(new { Text = "Offline Built-In Server", Value = "http://localhost:4416/sbrw/Engine.svc" });
            }

            serverPick.DataSource = finalItems;


            //Silliest way to prevent doublecall of TextChanged event...
            if (_serverlistloaded)
            {
                try
                {
                    serverPick.SelectedIndex = 1;
                }
                catch
                {
                    // ignored
                }

                if (!_settingFile.KeyExists("Server"))
                {
                    _settingFile.Write("Server", ((ServerInfo)serverPick.SelectedItem).Id);
                }

                if (_settingFile.KeyExists("Server"))
                {
                    _skipServerTrigger = true;

                    if (_slresponse.Contains(_settingFile.Read("Server")))
                    {

						var index =
                            finalItems.FindIndex(i => string.Equals(i.IpAddress, _settingFile.Read("Server")));

						if (index >= 0)
                        {
                            serverPick.SelectedIndex = index;
                        }
					}
                    else
                    {
                        serverPick.SelectedIndex = 1;
                        _settingFile.DeleteKey("Server");
                    }

                    //I don't know other way to fix this call...
                    if (serverPick.SelectedIndex == 1)
                    {
                        serverPick_SelectedIndexChanged(sender, e);
                    }
                }
            }

            if (_settingFile.KeyExists("Password"))
            {
                _loginEnabled = true;
                _serverEnabled = true;
                _useSavedPassword = true;
                loginButton.Image = Properties.Resources.smallbutton_enabled;
                loginButton.ForeColor = Color.White;
            }
            else
            {
                _loginEnabled = false;
                _serverEnabled = false;
                _useSavedPassword = false;
                loginButton.Image = Properties.Resources.smallbutton_disabled;
                loginButton.ForeColor = Color.Gray;
            }

            //Add downloadable languages to settingLanguage
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

            //Add downloadable quality to settingLanguage
            settingsQuality.DisplayMember = "Text";
            settingsQuality.ValueMember = "Value";

            var quality = new[] {
                new { Text = "Standard", Value = "0" },
                new { Text = "Maximum", Value = "1" },
            };

            settingsQuality.DataSource = quality;

            if (_settingFile.KeyExists("TracksHigh"))
            {
                settingsQuality.SelectedValue = _settingFile.Read("TracksHigh");
            }

            var drive = Path.GetPathRoot(_settingFile.Read("InstallationDirectory"));
            if (!Directory.Exists(drive))
            {
                if (!string.IsNullOrEmpty(drive))
                {
                    var newdir = Directory.GetCurrentDirectory() + "\\GameFiles";
                    _settingFile.Write("InstallationDirectory", newdir);
                    MessageBox.Show(null, string.Format("Drive {0} was not found. Your actual installation directory is set to {1} now.", drive, newdir), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //Soapbox Modules (without them Freeroam might fail)
            try
            {
                Directory.CreateDirectory(_settingFile.Read("InstallationDirectory"));
                if (!File.Exists(_settingFile.Read("InstallationDirectory") + "/lightfx.dll"))
                {
                    //File.WriteAllBytes(_settingFile.Read("InstallationDirectory") + "/lightfx.dll", ExtractResource.AsByte("GameLauncher.SoapBoxModules.lightfx.dll"));
                    //Instead of extracting this file, we gonna download it from web, coz why not.
                    try
                    {
                        File.WriteAllBytes(_settingFile.Read("InstallationDirectory") + "/lightfx.dll", new WebClientWithTimeout().DownloadData("http://launcher.soapboxrace.world/lightfx.dll"));
                    }
                    catch
                    {
                        ConsoleLog("Failed to fetch 'lightfx.dll' module. Freeroam might not work correctly.", "error");
                    }

                    Directory.CreateDirectory(_settingFile.Read("InstallationDirectory") + "/modules");
                    File.WriteAllText(_settingFile.Read("InstallationDirectory") + "/modules/udpcrc.soapbox.module", ExtractResource.AsString("GameLauncher.SoapBoxModules.udpcrc.soapbox.module"));
                    File.WriteAllText(_settingFile.Read("InstallationDirectory") + "/modules/udpcrypt1.soapbox.module", ExtractResource.AsString("GameLauncher.SoapBoxModules.udpcrypt1.soapbox.module"));
                    File.WriteAllText(_settingFile.Read("InstallationDirectory") + "/modules/udpcrypt2.soapbox.module", ExtractResource.AsString("GameLauncher.SoapBoxModules.udpcrypt2.soapbox.module"));
                    File.WriteAllText(_settingFile.Read("InstallationDirectory") + "/modules/xmppsubject.soapbox.module", ExtractResource.AsString("GameLauncher.SoapBoxModules.xmppsubject.soapbox.module"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                closebtn_Click(null, null);
            }

            //ModNet
            try
            {
                Directory.CreateDirectory(_settingFile.Read("InstallationDirectory"));
                if (!File.Exists(_settingFile.Read("InstallationDirectory") + "/dinput8.dll"))
                {
                    File.WriteAllBytes(_settingFile.Read("InstallationDirectory") + "/dinput8.dll",
                        ExtractResource.AsByte("GameLauncher.SoapBoxModules.dinput8.dll"));
                    Directory.CreateDirectory(_settingFile.Read("InstallationDirectory") + "/scripts");
                    File.WriteAllText(_settingFile.Read("InstallationDirectory") + "/scripts/global.ini",
                        ExtractResource.AsString("GameLauncher.SoapBoxModules.global.ini"));
                    File.WriteAllBytes(_settingFile.Read("InstallationDirectory") + "/ModManager.asi",
                        ExtractResource.AsByte("GameLauncher.SoapBoxModules.ModManager.dll"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show(null, ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                closebtn_Click(null, null);
            }

            //Hide other windows
            RegisterFormElements(false);
            SettingsFormElements(false);
            LoggedInFormElements(false);

            //Reshow missing elements
            LoginFormElements(true);

            try {
                var gameInstallDirValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "GameInstallDir", RegistryValueKind.String).ToString();
                if (gameInstallDirValue != Path.GetFullPath(_settingFile.Read("InstallationDirectory"))) {
                    try
                    {
                        Registry.SetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "GameInstallDir", Path.GetFullPath(_settingFile.Read("InstallationDirectory")));
                        Registry.SetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "LaunchInstallDir", Path.GetFullPath(Application.ExecutablePath));
                    }
                    catch { }
                }
            }
            catch { }

            //Somewhere here translations?
            if (string.IsNullOrEmpty(_settingFile.Read("UILanguage")))
            {
                _uiLanguage = "Default";
            }
            else
            {
                _uiLanguage = _settingFile.Read("UILanguage");
            }

            _newGameFilesPath = Path.GetFullPath(_settingFile.Read("InstallationDirectory"));
            settingsGameFilesCurrent.Text = "CURRENT DIRECTORY: " + _newGameFilesPath;

            SetTranslations(_uiLanguage);

            var handlers = new DiscordRpc.EventHandlers();
            DiscordRpc.Initialize(_discordrpccode, ref handlers, true, "");
            _presence.state = _OS;
            _presence.details = "In-Launcher: " + Application.ProductVersion;
            _presence.largeImageText = "SBRW";
            _presence.largeImageKey = "nfsw";
            _presence.instance = true;
            DiscordRpc.UpdatePresence(_presence);

            try {
                //TODO HERE
                //Download new BlackListedServers.dat via LaunchPad/JetPack and use that instead of static file.

                var fileStream = new FileStream("BlackListedServers.dat", FileMode.Open);

                var dEsCryptoServiceProvider = new DESCryptoServiceProvider() {
                    Key = Encoding.ASCII.GetBytes("2137JPJP"),
                    IV = Encoding.ASCII.GetBytes("NRSUCKXD")
                };

                var cryptoStream = new CryptoStream(fileStream, dEsCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read);
                var streamReader = new StreamReader(cryptoStream);
                _blacklistedXML = streamReader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(_slresponse)) {
                    //MessageBox.Show(null, "Unable to load important files, please extract all files from ZIP.", "GameLauncher", MessageBoxButtons.OK);
                    //Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }
            } catch {
                //MessageBox.Show(null, "Manipulation detected. Please re-download and extract launcher files.", "GameLauncher", MessageBoxButtons.OK);
                //Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
            }

            BeginInvoke((MethodInvoker)delegate
            {
                LaunchNfsw();
            });

            if(_splashscreen != null)
                _splashscreen.Hide();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            closebtn.BackgroundImage = Properties.Resources.close_click;

            if (_serverlistloaded)
            {
				try { _settingFile.Write("Server", ((ServerInfo)serverPick.SelectedItem).IpAddress); } catch { }
            }

            if (_windowMoved)
            {
                _settingFile.Write("LauncherPosX", Location.X.ToString());
                _settingFile.Write("LauncherPosY", Location.Y.ToString());
            }

            //Fix InstallationDirectory
            _settingFile.Write("InstallationDirectory", Path.GetFullPath(_settingFile.Read("InstallationDirectory")));

            //Kill NFSW.exe aswell
            //Process[] allOfThem = Process.GetProcessesByName("nfsw");
            //foreach (var oneProcess in allOfThem) {
            //    Process.GetProcessById(oneProcess.Id).Kill();
            //}

            if (_nfswPid != 0)
            {
                try
                {
                    Process.GetProcessById(_nfswPid).Kill();
                }
                catch
                {
                    // ignored
                }
            }

            //Kill DiscordRPC
            DiscordRpc.Shutdown();

            ServerProxy.Instance.Stop();

            //Dirty way to terminate application (sometimes Application.Exit() didn't really quitted, was still running in background)
            if (DetectLinux.WineDetected())
            {
                Close();
                _downloader.Stop();
                Application.Exit();
                Application.ExitThread();
                Environment.Exit(Environment.ExitCode);
            }
            else
            {
                Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
            }
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
            //Form y = new DebugWindow(serverPick.SelectedValue.ToString(), serverPick.GetItemText(serverPick.SelectedItem));
            //y.Show();
        }

        private void OpenMapHandler(object sender, EventArgs e)
        {
            if (!(serverPick.SelectedItem is ServerInfo server)) return;

            var form = new ShowMap(server.IpAddress, _realServername);

            form.Show();
            //Form z = new ShowMap(serverPick.SelectedValue.ToString(), serverPick.GetItemText(serverPick.SelectedItem));
            //z.Show();
        }

        private void closebtn_MouseEnter(object sender, EventArgs e)
        {
            closebtn.BackgroundImage = Properties.Resources.close_hover;
        }

        private void closebtn_MouseLeave(object sender, EventArgs e)
        {
            closebtn.BackgroundImage = Properties.Resources.close;
        }

        /*private void minimizebtn_Click(object sender, EventArgs e)
        {
            //minimizebtn.BackgroundImage = Properties.Resources.minimize_click;
            WindowState = FormWindowState.Minimized;
        }

        private void minimizebtn_MouseEnter(object sender, EventArgs e)
        {
            //minimizebtn.BackgroundImage = Properties.Resources.minimize_hover;
        }

        private void minimizebtn_MouseLeave(object sender, EventArgs e)
        {
            //minimizebtn.BackgroundImage = Properties.Resources.minimize;
        }*/

        private void LoginEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && _loginEnabled)
            {
                loginButton_Click(null, null);
                e.SuppressKeyPress = true;
            }/* else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V) {
                e.SuppressKeyPress = true;
            }*/
        }

        private void Loginbuttonenabler(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(email.Text) || string.IsNullOrEmpty(password.Text))
            {
                _loginEnabled = false;
                loginButton.Image = Properties.Resources.smallbutton_disabled;
                loginButton.ForeColor = Color.Gray;
            }
            else
            {
                _loginEnabled = true;
                loginButton.Image = Properties.Resources.smallbutton_enabled;
                loginButton.ForeColor = Color.White;
            }

            _useSavedPassword = false;
        }

        private void loginButton_MouseUp(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                loginButton.Image = Properties.Resources.smallbutton_hover;
            }
            else
            {
                loginButton.Image = Properties.Resources.smallbutton_disabled;
            }
        }

        private void loginButton_MouseDown(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                loginButton.Image = Properties.Resources.smallbutton_click;
            }
            else
            {
                loginButton.Image = Properties.Resources.smallbutton_disabled;
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
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

            var serverInfo = (ServerInfo)serverPick.SelectedItem;

            _serverIp = serverInfo.IpAddress;
            var serverName = serverInfo.Name;
            var username = email.Text.ToString();
            var encryptedpassword = "";
            var serverLoginResponse = "";

            if (_useSavedPassword)
            {
                encryptedpassword = _settingFile.Read("Password");
            }
            else
            {
                if (_passwordHash == "BCRYPT")
                {
                    encryptedpassword = BCrypt.HashPassword(password.Text.ToString());
                }
                else
                {
                    encryptedpassword = SHA.HashPassword(password.Text.ToString());
                }
            }

            if (rememberMe.Checked)
            {
                _settingFile.Write("AccountEmail", username);
                _settingFile.Write("Password", encryptedpassword.ToLower());
            }
            else
            {
                _settingFile.DeleteKey("AccountEmail");
                _settingFile.DeleteKey("Password");
            }

            if (FingerPrint.GetHash(username) == "E490FAA128F417472F21958AA12E321BCAC1420D") {
                var msgBoxInfo = string.Format("You got banned on {0}.", serverName) + "\n";
                msgBoxInfo += "Reason: Born in Teleorman\nBanned forever.";
                ConsoleLog(msgBoxInfo, "error");
                return;
            }

            try
            {
                WebClient wc = new WebClientWithTimeout();
                var buildUrl = _serverIp + "/User/authenticateUser?email=" + username + "&password=" + encryptedpassword.ToLower();
                serverLoginResponse = wc.DownloadString(buildUrl);
            }
            catch (WebException ex)
            {
                var serverReply = (HttpWebResponse)ex.Response;

                if (serverReply == null)
                {
                    _errorcode = 500;
                    serverLoginResponse = "<LoginStatusVO><UserId/><LoginToken/><Description>Failed to get reply from server. Please retry.</Description></LoginStatusVO>";
                }
                else
                {
                    using (var sr = new StreamReader(serverReply.GetResponseStream()))
                    {
                        _errorcode = (int)serverReply.StatusCode;
                        serverLoginResponse = sr.ReadToEnd();
                    }
                }
            }

            if (string.IsNullOrEmpty(serverLoginResponse))
            {
                ConsoleLog("Server seems to be offline.", "error");
            }
            else
            {
                try
                {
                    var sbrwXml = new XmlDocument();

                    if (_builtinserver == false)
                    {
                        sbrwXml.LoadXml(serverLoginResponse);
                    }
                    else
                    {
                        sbrwXml.LoadXml("<LoginStatusVO><UserId>1</UserId><LoginToken>aaaaaaaa-aaaa-aaaa-aaaaaaaa</LoginToken><Description/></LoginStatusVO>");
                    }

                    XmlNode extraNode;
                    XmlNode loginTokenNode;
                    XmlNode userIdNode;
                    var msgBoxInfo = "";

                    loginTokenNode = sbrwXml.SelectSingleNode("LoginStatusVO/LoginToken");
                    userIdNode = sbrwXml.SelectSingleNode("LoginStatusVO/UserId");

                    if (sbrwXml.SelectSingleNode("LoginStatusVO/Ban") == null)
                    {
                        if (sbrwXml.SelectSingleNode("LoginStatusVO/Description") == null)
                        {
                            extraNode = sbrwXml.SelectSingleNode("html/body");
                        }
                        else
                        {
                            extraNode = sbrwXml.SelectSingleNode("LoginStatusVO/Description");
                        }
                    }
                    else
                    {
                        extraNode = sbrwXml.SelectSingleNode("LoginStatusVO/Ban");
                    }

                    if (!string.IsNullOrEmpty(extraNode.InnerText))
                    {
                        if (extraNode.SelectSingleNode("Reason") != null)
                        {
                            msgBoxInfo = string.Format("You got banned on {0}.", serverName) + "\n";
                            msgBoxInfo += string.Format("Reason: {0}", extraNode.SelectSingleNode("Reason").InnerText);

                            if (extraNode.SelectSingleNode("Expires") != null)
                            {
                                msgBoxInfo += "\n" + string.Format("Ban expires {0}", extraNode.SelectSingleNode("Expires").InnerText);
                            }
                            else
                            {
                                msgBoxInfo += "\n" + "Banned forever.";
                            }
                        }
                        else
                        {
                            if (extraNode.InnerText == "Please use MeTonaTOR's launcher. Or, are you tampering?")
                            {
                                msgBoxInfo = "Launcher tampering detected. Please use original build.";
                            }
                            else
                            {
                                if (sbrwXml.SelectSingleNode("html/body") == null)
                                {
                                    if (extraNode.InnerText == "LOGIN ERROR")
                                    {
                                        msgBoxInfo = "Invalid e-mail or password.";
                                    }
                                    else
                                    {
                                        msgBoxInfo = extraNode.InnerText;
                                    }
                                }
                                else
                                {
                                    msgBoxInfo = "ERROR " + _errorcode + ": " + extraNode.InnerText;
                                }
                            }
                        }

                        ConsoleLog(msgBoxInfo, "error");
                    }
                    else
                    {
                        _userId = userIdNode.InnerText;
                        _loginToken = loginTokenNode.InnerText;

                        _loggedIn = true;

                        BackgroundImage = Properties.Resources.loggedbg;
                        LoginFormElements(false);
                        LoggedInFormElements(true);

                        welcomeBack.Text = string.Format("Welcome back, {0}!", username).ToUpper();
                    }
                }
                catch
                {
                    ConsoleLog("Server seems to be offline.", "error");
                }
            }
        }

        private void loginButton_MouseEnter(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                loginButton.Image = Properties.Resources.smallbutton_hover;
                loginButton.ForeColor = Color.White;
            }
            else
            {
                loginButton.Image = Properties.Resources.smallbutton_disabled;
                loginButton.ForeColor = Color.Gray;
            }
        }

        private void loginButton_MouseLeave(object sender, EventArgs e)
        {
            if (_loginEnabled || _builtinserver)
            {
                loginButton.Image = Properties.Resources.smallbutton_enabled;
                loginButton.ForeColor = Color.White;
            }
            else
            {
                loginButton.Image = Properties.Resources.smallbutton_disabled;
                loginButton.ForeColor = Color.Gray;
            }
        }

        private void serverPick_SelectedIndexChanged(object sender, EventArgs e)
        {
            var serverInfo = (ServerInfo)serverPick.SelectedItem;
            _realServername = serverInfo.Name; //Its not real, but it solves some issues.

            if (serverInfo.IsSpecial)
            {
                serverPick.SelectedIndex = _lastSelectedServerId;
                return;
            }

            if (!_skipServerTrigger) { return; }

            _lastSelectedServerId = serverPick.SelectedIndex;

            _allowRegistration = false;

            ServerStatusBar(_colorLoading, _startPoint, _endPoint);

            imageServerName.Text = serverInfo.Name;

            _loginEnabled = false;

            loginButton.ForeColor = Color.Gray;
            password.Text = "";
            var verticalImageUrl = "";
            verticalBanner.Image = null;
            verticalBanner.BackColor = Color.Transparent;

            var serverIp = ((ServerInfo)serverPick.SelectedItem).IpAddress;
            string numPlayers;
            var serverName = ((ServerInfo)serverPick.SelectedItem).Name;

            var wordsArray = serverName.Split();
            var richPresenceIconId = ((wordsArray.Length == 1) ? wordsArray[0] : wordsArray[0] + wordsArray[1]).ToLower();

            onlineCount.Text = "";

            if (serverPick.GetItemText(serverPick.SelectedItem) == "Offline Built-In Server")
            {
                _builtinserver = true;
                loginButton.Image = Properties.Resources.smallbutton_enabled;
                loginButton.Text = "Launch".ToUpper();
                loginButton.ForeColor = Color.White;
            }
            else
            {
                _builtinserver = false;
                loginButton.Image = Properties.Resources.smallbutton_disabled;
                loginButton.Text = "Login".ToUpper();
                loginButton.ForeColor = Color.Gray;
            }

            var client = new WebClientWithTimeout();

            //serverPick.Enabled = false;

            var artificialPingStart = Self.getTimestamp();

            allowedCountriesLabel.Text = "";
            verticalBanner.BackColor = Color.Transparent;

            var stringToUri = new Uri(serverIp + "/GetServerInformation");
            client.DownloadStringAsync(stringToUri);
            client.DownloadStringCompleted += (sender2, e2) =>
            {
                //serverPick.Enabled = true;


                var artificialPingEnd = Self.getTimestamp();

                if (e2.Error != null)
                {
                    ServerStatusBar(_colorOffline, _startPoint, _endPoint);

                    onlineCount.Text = "Server seems to be offline.";
                    _serverEnabled = false;
                    _allowRegistration = false;
                }
                else
                {
                    if (serverName == "Offline Built-In Server")
                    {
                        numPlayers = "∞";
                    }
                    else
                    {
                        var json = JsonConvert.DeserializeObject<GetServerInformation>(e2.Result);
                        try
                        {
                            _realServername = json.serverName;
                            imageServerName.Text = json.serverName;
                            if (!string.IsNullOrEmpty(json.bannerUrl))
                            {
                                Uri uriResult;
                                bool result;

                                try
                                {
                                    result = Uri.TryCreate(json.bannerUrl, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch
                                {
                                    result = false;
                                }

                                if (result)
                                {
                                    verticalImageUrl = json.bannerUrl;
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

                        try
                        {
                            if (string.IsNullOrEmpty(json.requireTicket))
                            {
                                _ticketRequired = true;
                            }
                            else if (json.requireTicket == "true")
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

                        if (!string.IsNullOrEmpty(json.allowedCountries))
                        {
                            var countries = new List<object>();
                            var splitted = json.allowedCountries.Split(';');

                            foreach (var splitter in splitted)
                            {
                                countries.Add(Self.CountryName(splitter));
                            }

                            var allowed = string.Join(", ", countries);

                            allowedCountriesLabel.Text = string.Format("Warning, this server only accepts players from: {0}", allowed);
                        }
                        else
                        {
                            allowedCountriesLabel.Text = "";
                        }

                        if (json.maxUsersAllowed == 0)
                        {
                            numPlayers = string.Format("{0} out of {1}", json.onlineNumber, json.numberOfRegistered);
                        }
                        else
                        {
                            numPlayers = string.Format("{0} out of {1}", json.onlineNumber, json.maxUsersAllowed.ToString());
                        }

                        _allowRegistration = true;

                        try
                        {
                            if (json.passwordHashing == "BCRYPT")
                            {
                                _passwordHash = "BCRYPT";
                            }
                            else
                            {
                                _passwordHash = "SHA1";
                            }
                        }
                        catch
                        {
                            _passwordHash = "SHA1";
                        }

                        ServerStatusBar(_colorOnline, _startPoint, _endPoint);
                    }

                    onlineCount.Text = string.Format("Players on server: {0}", numPlayers); // "Players on server: " + numPlayers;
                    _serverEnabled = true;

                    if (!string.IsNullOrEmpty(verticalImageUrl))
                    {
                        var client2 = new WebClientWithTimeout();
                        var stringToUri3 = new Uri(verticalImageUrl);
                        client2.DownloadDataAsync(stringToUri3);
                        client2.DownloadDataCompleted += (sender4, e4) =>
                        {
                            if (e4.Cancelled)
                            {
                                client2.CancelAsync();
                                return;
                            }
                            else if (e4.Error != null)
                            {
                                //What?
                            }
                            else
                            {
                                try
                                {
                                    Image image;
                                    var memoryStream = new MemoryStream(e4.Result);
                                    image = Image.FromStream(memoryStream);
                                    verticalBanner.Image = image;
                                    verticalBanner.BackColor = Color.Black;
                                }
                                catch
                                {
                                    verticalBanner.Image = null;
                                }
                            }
                        };
                    }

                    onlineCount.Text += ". ";

                    if (!DetectLinux.WineDetected() && !DetectLinux.UnixDetected())
                    {
                        var pingSender = new Ping();
                        pingSender.SendAsync(stringToUri.Host, 1000, new byte[1], new PingOptions(64, true), new AutoResetEvent(false));
                        pingSender.PingCompleted += (sender3, e3) =>
                        {
                            var reply = e3.Reply;

                            if (reply.Status == IPStatus.Success && serverName != "Offline Built-In Server")
                            {
                                onlineCount.Text += string.Format("Server ping is {0}ms.", reply.RoundtripTime);
                            }
                            else
                            {
                                var hostEntry = Dns.GetHostEntry(stringToUri.Host);

                                if (hostEntry.AddressList.Length > 0)
                                {
                                    var ip = hostEntry.AddressList[0];

                                    var pingSender2 = new Ping();
                                    pingSender2.SendAsync(ip.ToString(), 1000, new byte[1], new PingOptions(64, true), new AutoResetEvent(false));

                                    pingSender2.PingCompleted += (sender4, e4) =>
                                    {
                                        var reply2 = e4.Reply;

                                        if (reply.Status == IPStatus.Success && serverName != "Offline Built-In Server")
                                        {
                                            onlineCount.Text += string.Format("Server ping is {0}ms.", reply.RoundtripTime);
                                        }
                                        else
                                        {
                                            ServerStatusBar(_colorIssues, _startPoint, _endPoint);

                                            onlineCount.Text += string.Format("Server ping is {0}ms.", (artificialPingEnd - artificialPingStart).ToString());
                                            onlineCount.Text += " (HTTP)";
                                        }
                                    };
                                }
                                else
                                {
                                    ServerStatusBar(_colorIssues, _startPoint, _endPoint);
                                    onlineCount.Text += "Server doesn't allow pinging.";
                                }
                            }
                        };
                    }
                    else
                    {
                        ServerStatusBar(_colorIssues, _startPoint, _endPoint);
                        onlineCount.Text += "Ping is disabled on non-Windows platform.";
                    }
                }
            };
        }

        private void ApplyEmbeddedFonts()
        {
            var fontFamily1 = FontWrapper.Instance.GetFontFamily("Montserrat-Regular.ttf");
            var fontFamily2 = FontWrapper.Instance.GetFontFamily("Montserrat-Bold.ttf");

            //Implement them to elements
            email.Font = new Font(fontFamily1, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            loginButton.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            password.Font = new Font(fontFamily1, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            rememberMe.Font = new Font(fontFamily1, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            emailLabel.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            passwordLabel.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            launcherVersion.Font = new Font(fontFamily1, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            forgotPassword.Font = new Font(fontFamily2, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            onlineCount.Font = new Font(fontFamily1, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            playProgressText.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            playButton.Font = new Font(fontFamily2, 15f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            currentWindowInfo.Font = new Font(fontFamily2, 11.35f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            imageServerName.Font = new Font(fontFamily2, 25f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            registerEmailText.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerPasswordText.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerConfirmPasswordText.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerTicketText.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerAgree.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerButton.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerCancel.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            settingsLanguageText.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsQualityText.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsSave.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsLanguageDesc.Font = new Font(fontFamily1, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            settingsQualityDesc.Font = new Font(fontFamily1, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);

            settingsGamePathText.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsGameFilesCurrent.Font = new Font(fontFamily2, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            logoutButton.Font = new Font(fontFamily2, 10f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            allowedCountriesLabel.Font = new Font(fontFamily1, 8f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
        }

        private void registerText_LinkClicked(object sender, EventArgs e)
        {
            if (_allowRegistration)
            {
                BackgroundImage = Properties.Resources.secondarybackground;
                currentWindowInfo.Text = string.Format("Register on {0}", serverPick.GetItemText(serverPick.SelectedItem)).ToUpper();
                LoginFormElements(false);
                RegisterFormElements(true);
            }
            else
            {
                MessageBox.Show("Server seems to be offline.");
            }
        }

        private void githubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ConsoleLog("Redirecting into GitHub Issue page", "info");
            Process.Start("https://discord.gg/JqN2nMY");
        }

        private void forgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var serverInfo = (ServerInfo)serverPick.SelectedItem;
            Process.Start(serverInfo.IpAddress.ToString().Replace("Engine.svc", "") + "forgotPasswd.jsp");
        }

        private void moreLanguages_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/metonator/GameLauncher_NFSW-translations/tree/master/Languages");
        }

        private void LoggedInFormElements(bool hideElements)
        {
            if (hideElements)
            {
                currentWindowInfo.Text = "Enter your account information to Log In:".ToUpper();
            }

            logoutButton.Visible = hideElements;
            playProgress.Visible = hideElements;
            extractingProgress.Visible = hideElements;
            playProgressText.Visible = hideElements;
            playButton.Visible = hideElements;
            settingsButton.Visible = hideElements;
            verticalBanner.Visible = hideElements;
            onlineCount.Visible = hideElements;
            welcomeBack.Visible = hideElements;
            allowedCountriesLabel.Visible = hideElements;
        }

        private void LoginFormElements(bool hideElements = false)
        {
            if (hideElements)
            {
                currentWindowInfo.Text = "Enter your account information to Log In:".ToUpper();
            }

            rememberMe.Visible = hideElements;
            loginButton.Visible = hideElements;
            onlineCount.Visible = hideElements;
            registerText.Visible = hideElements;
            serverPick.Visible = hideElements;
            email.Visible = hideElements;
            password.Visible = hideElements;
            emailLabel.Visible = hideElements;
            passwordLabel.Visible = hideElements;
            forgotPassword.Visible = hideElements;
            settingsButton.Visible = hideElements;
            verticalBanner.Visible = hideElements;
            playProgressText.Visible = hideElements;
            playProgress.Visible = hideElements;
            extractingProgress.Visible = hideElements;
            playButton.Visible = hideElements;
            addServer.Visible = hideElements;
            allowedCountriesLabel.Visible = hideElements;
            showmap.Visible = hideElements;
        }

        /*
         * REGISTER PAGE LAYOUT
         * Because why should i close Form1 and create/open Form2 if it will look a bit more responsive...
         */

        private void RegisterFormElements(bool hideElements = true)
        {
            registerButton.Visible = hideElements;
            registerEmail.Visible = hideElements;
            registerEmailText.Visible = hideElements;
            registerPassword.Visible = hideElements;
            registerPasswordText.Visible = hideElements;
            registerConfirmPassword.Visible = hideElements;
            registerConfirmPasswordText.Visible = hideElements;
            registerAgree.Visible = hideElements;
            registerCancel.Visible = hideElements;

            if (_ticketRequired)
            {
                registerTicket.Visible = hideElements;
                registerTicketText.Visible = hideElements;
            }
            else
            {
                registerTicket.Visible = false;
                registerTicketText.Visible = false;
            }

            errorConfirm.Visible = hideElements;
            errorEmail.Visible = hideElements;
            errorPassword.Visible = hideElements;
            errorTicket.Visible = hideElements;
            errorTOS.Visible = hideElements;

            // Reset fields
            registerEmail.Text = "";
            registerPassword.Text = "";
            registerConfirmPassword.Text = "";
            registerAgree.Checked = false;
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            var reply = MessageBox.Show(null, string.Format("Are you sure you want to log out from {0}?", serverPick.GetItemText(serverPick.SelectedItem)), "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (reply == DialogResult.Yes)
            {
                BackgroundImage = Properties.Resources.loginbg;
                _loggedIn = false;
                LoggedInFormElements(false);
                LoginFormElements(true);
            }
        }

        private void logoutButton_MouseDown(object sender, EventArgs e)
        {
            logoutButton.Image = Properties.Resources.smallbutton_click;
        }

        private void logoutButton_MouseEnter(object sender, EventArgs e)
        {
            logoutButton.Image = Properties.Resources.smallbutton_hover;
        }

        private void logoutButton_MouseLeave(object sender, EventArgs e)
        {
            logoutButton.Image = Properties.Resources.smallbutton_enabled;
        }

        private void logoutButton_MouseUp(object sender, EventArgs e)
        {
            logoutButton.Image = Properties.Resources.smallbutton_hover;
        }

        private void registerButton_MouseEnter(object sender, EventArgs e)
        {
            registerButton.Image = Properties.Resources.smallbutton_hover;
        }

        private void registerButton_MouseLeave(object sender, EventArgs e)
        {
            registerButton.Image = Properties.Resources.smallbutton_enabled;
        }

        private void registerButton_MouseUp(object sender, EventArgs e)
        {
            registerButton.Image = Properties.Resources.smallbutton_hover;
        }

        private void registerButton_MouseDown(object sender, EventArgs e)
        {
            registerButton.Image = Properties.Resources.smallbutton_click;
        }

        private void registerCancel_Click(object sender, EventArgs e)
        {
            errorEmail.Text = ""; errorPassword.Text = ""; errorConfirm.Text = ""; errorTicket.Text = ""; errorTOS.Text = ""; registerAgree.ForeColor = Color.White;
            BackgroundImage = Properties.Resources.loginbg;
            currentWindowInfo.Text = "Enter your account information to Log In:".ToUpper();
            RegisterFormElements(false);
            LoginFormElements(true);
        }

        private void registerCancel_MouseDown(object sender, EventArgs e)
        {
            registerCancel.Image = Properties.Resources.cancelbutton_click;
        }

        private void registerCancel_MouseEnter(object sender, EventArgs e)
        {
            registerCancel.Image = Properties.Resources.cancelbutton_hover;
        }

        private void registerCancel_MouseLeave(object sender, EventArgs e)
        {
            registerCancel.Image = Properties.Resources.cancelbutton_enabled;
        }

        private void registerCancel_MouseUp(object sender, EventArgs e)
        {
            registerCancel.Image = Properties.Resources.cancelbutton_hover;
        }

        public void DrawErrorAroundTextBox(TextBox x)
        {
            x.BorderStyle = BorderStyle.FixedSingle;
            var p = new Pen(Color.Red);
            var g = CreateGraphics();
            var variance = 1;
            g.DrawRectangle(p, new Rectangle(x.Location.X - variance, x.Location.Y - variance, x.Width + variance, x.Height + variance));
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            var registerSuccess = true;
            var passwordfield = false;
            Refresh();

            if (string.IsNullOrEmpty(registerEmail.Text))
            {
                DrawErrorAroundTextBox(registerEmail);
                errorEmail.Text = "Please enter your e-mail.".ToUpper();
                registerSuccess = false;
            }
            else if (Self.validateEmail(registerEmail.Text) == false)
            {
                DrawErrorAroundTextBox(registerEmail);
                errorEmail.Text = "Please enter a valid e-mail address.".ToUpper();
                registerSuccess = false;
            }
            else
            {
                errorEmail.Text = "";
            }

            if (string.IsNullOrEmpty(registerTicket.Text) && _ticketRequired)
            {
                DrawErrorAroundTextBox(registerTicket);
                errorTicket.Text = "Please enter your ticket.".ToUpper();
                registerSuccess = false;
            }
            else
            {
                errorTicket.Text = "";
            }

            if (string.IsNullOrEmpty(registerPassword.Text))
            {
                DrawErrorAroundTextBox(registerPassword);
                errorPassword.Text = "Please enter your password.".ToUpper();
                registerSuccess = false;
                passwordfield = true;
            }
            else
            {
                errorPassword.Text = "";
            }

            if (string.IsNullOrEmpty(registerConfirmPassword.Text))
            {
                DrawErrorAroundTextBox(registerConfirmPassword);
                errorConfirm.Text = "Please confirm your password.".ToUpper();
                registerSuccess = false;
            }
            else
            {
                errorConfirm.Text = "";
            }

            if (registerConfirmPassword.Text != registerPassword.Text)
            {
                DrawErrorAroundTextBox(registerConfirmPassword);
                DrawErrorAroundTextBox(registerPassword);
                errorPassword.Text = "Passwords don't match.".ToUpper();
                registerSuccess = false;
            }
            else
            {
                if (passwordfield == false)
                {
                    errorPassword.Text = "";
                }
            }

            if (!registerAgree.Checked)
            {
                registerAgree.ForeColor = Color.Red;
                errorTOS.Text = "You have not agreed to the Terms of Service.".ToUpper();
                registerSuccess = false;
            }
            else
            {
                registerAgree.ForeColor = Color.White;
                errorTOS.Text = "";
            }

            if (registerSuccess)
            {
				if (!(serverPick.SelectedItem is ServerInfo server)) return;

				_serverIp = server.IpAddress;
                var serverName = _realServername;
                var encryptedpassword = "";
                var serverLoginResponse = "";
                string buildUrl;

                if (_passwordHash == "BCRYPT")
                {
                    encryptedpassword = BCrypt.HashPassword(registerPassword.Text.ToString());
                }
                else
                {
                    encryptedpassword = SHA.HashPassword(registerPassword.Text.ToString());
                }

                try
                {
                    WebClient wc = new WebClientWithTimeout();

                    if (_ticketRequired)
                    {
                        buildUrl = _serverIp + "/User/createUser?email=" + registerEmail.Text + "&password=" + encryptedpassword.ToLower() + "&inviteTicket=" + registerTicket.Text;
                    }
                    else
                    {
                        buildUrl = _serverIp + "/User/createUser?email=" + registerEmail.Text + "&password=" + encryptedpassword.ToLower();
                    }

					Console.WriteLine(buildUrl);

                    serverLoginResponse = wc.DownloadString(buildUrl);
                }
                catch (WebException ex)
                {
                    var serverReply = (HttpWebResponse)ex.Response;
                    if (serverReply == null)
                    {
                        _errorcode = 500;
                        serverLoginResponse = "<LoginStatusVO><UserId/><LoginToken/><Description>Failed to get reply from server. Please retry.</Description></LoginStatusVO>";
                    }
                    else
                    {
                        using (var sr = new StreamReader(serverReply.GetResponseStream()))
                        {
                            _errorcode = (int)serverReply.StatusCode;
                            serverLoginResponse = sr.ReadToEnd();
                        }
                    }
                }

                try
                {
                    var sbrwXml = new XmlDocument();
                    sbrwXml.LoadXml(serverLoginResponse);

                    XmlNode extraNode;
                    XmlNode loginTokenNode;
                    XmlNode userIdNode;
                    var msgBoxInfo = "";

                    try
                    {
                        loginTokenNode = sbrwXml.SelectSingleNode("LoginStatusVO/LoginToken");
                        userIdNode = sbrwXml.SelectSingleNode("LoginStatusVO/UserId");

                        if (sbrwXml.SelectSingleNode("LoginStatusVO/Ban") == null)
                        {
                            if (sbrwXml.SelectSingleNode("LoginStatusVO/Description") == null)
                            {
                                extraNode = sbrwXml.SelectSingleNode("html/body");
                            }
                            else
                            {
                                extraNode = sbrwXml.SelectSingleNode("LoginStatusVO/Description");
                            }
                        }
                        else
                        {
                            extraNode = sbrwXml.SelectSingleNode("LoginStatusVO/Ban");
                        }

                        if (string.IsNullOrEmpty(extraNode.InnerText) || extraNode.InnerText == "SERVER FULL")
                        {
                            if (extraNode.InnerText == "SERVER FULL")
                            {
                                MessageBox.Show(null, string.Format("Successfully registered on {0}. However, server is actually full, therefore you cannot play it right now.", serverName), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show(null, string.Format("Successfully registered on {0}. You can log in now.", serverName), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            _userId = userIdNode.InnerText;
                            _loginToken = loginTokenNode.InnerText;

                            BackgroundImage = Properties.Resources.loginbg;

                            RegisterFormElements(false);
                            LoginFormElements(true);

                            _loggedIn = true;
                        }
                        else
                        {
                            if (extraNode.SelectSingleNode("Reason") != null)
                            {
                                msgBoxInfo = string.Format("You got banned on {0}.", serverPick.GetItemText(serverPick.SelectedItem)) + "\n";
                                msgBoxInfo += string.Format("Reason: {0}", extraNode.SelectSingleNode("Reason").InnerText);

                                if (extraNode.SelectSingleNode("Expires") != null)
                                {
                                    msgBoxInfo += "\n" + string.Format("Ban expires {0}", extraNode.SelectSingleNode("Expires").InnerText);
                                }
                                else
                                {
                                    msgBoxInfo += "\n" + "Banned forever.";
                                }

                                ConsoleLog(msgBoxInfo, "error");
                            }
                            else
                            {
                                if (extraNode.InnerText == "Please use MeTonaTOR's launcher. Or, are you tampering?")
                                {
                                    msgBoxInfo = "Launcher tampering detected. Please use original build.";
                                    ConsoleLog(msgBoxInfo, "error");
                                }
                                else
                                {
                                    if (sbrwXml.SelectSingleNode("html/body") == null)
                                    {
                                        DrawErrorAroundTextBox(registerEmail);
                                        errorEmail.Text = extraNode.InnerText.ToUpper();
                                        Shake();
                                    }
                                    else
                                    {
                                        msgBoxInfo = "ERROR " + _errorcode + ": " + extraNode.InnerText;
                                        ConsoleLog(msgBoxInfo, "error");
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Server seems to be offline.");
                    }
                }
                catch
                {
                    MessageBox.Show("Server seems to be offline.");
                }
            }
            else
            {
                Shake();
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

        private void settingsButton_MouseEnter(object sender, EventArgs e)
        {
            settingsButton.BackgroundImage = Properties.Resources.settingsbtn_hover;
        }

        private void settingsButton_MouseLeave(object sender, EventArgs e)
        {
            settingsButton.BackgroundImage = Properties.Resources.settingsbtn;
        }

        private void settingsSave_MouseEnter(object sender, EventArgs e)
        {
            settingsSave.Image = Properties.Resources.smallbutton_hover;
        }

        private void settingsSave_MouseLeave(object sender, EventArgs e)
        {
            settingsSave.Image = Properties.Resources.smallbutton_enabled;
        }

        private void settingsSave_MouseUp(object sender, EventArgs e)
        {
            settingsSave.Image = Properties.Resources.smallbutton_hover;
        }

        private void settingsSave_MouseDown(object sender, EventArgs e)
        {
            settingsSave.Image = Properties.Resources.smallbutton_click;
        }

        private void settingsSave_Click(object sender, EventArgs e)
        {
            _settingFile.Write("Language", settingsLanguage.SelectedValue.ToString());
            _settingFile.Write("TracksHigh", settingsQuality.SelectedValue.ToString());

            var userSettingsXml = new XmlDocument();
            if (File.Exists(_userSettings))
            {
                try
                {
                    //File has been found, lets change Language setting
                    userSettingsXml.Load(_userSettings);
                    var language = userSettingsXml.SelectSingleNode("Settings/UI/Language");
                    language.InnerText = settingsLanguage.SelectedValue.ToString();
                }
                catch
                {
                    //XML is Corrupted... let's delete it and create new one
                    File.Delete(_userSettings);

                    var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
                    var persistentValue = setting.AppendChild(userSettingsXml.CreateElement("PersistentValue"));
                    var chat = persistentValue.AppendChild(userSettingsXml.CreateElement("Chat"));
                    var ui = setting.AppendChild(userSettingsXml.CreateElement("UI"));

                    chat.InnerXml = "<DefaultChatGroup Type=\"string\">" + settingsLanguage.SelectedValue + "</DefaultChatGroup>";
                    ui.InnerXml = "<Language Type=\"string\">" + settingsLanguage.SelectedValue + "</Language>";

                    var directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(_userSettings));
                }
            }
            else
            {
                //There's no file like that, let's create it
                var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
                var persistentValue = setting.AppendChild(userSettingsXml.CreateElement("PersistentValue"));
                var chat = persistentValue.AppendChild(userSettingsXml.CreateElement("Chat"));
                var ui = setting.AppendChild(userSettingsXml.CreateElement("UI"));

                chat.InnerXml = "<DefaultChatGroup Type=\"string\">" + settingsLanguage.SelectedValue.ToString() + "</DefaultChatGroup>";
                ui.InnerXml = "<Language Type=\"string\">" + settingsLanguage.SelectedValue.ToString() + "</Language>";

                var directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(_userSettings));
            }

            userSettingsXml.Save(_userSettings);

            if (_settingFile.Read("InstallationDirectory") != _newGameFilesPath) {
                _settingFile.Write("InstallationDirectory", _newGameFilesPath);
                _restartRequired = true;
            }

            if (_restartRequired) {
                MessageBox.Show(null, "In order to see settings changes, you need to restart launcher manually.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void settingsGameFilesCurrent_Click(object sender, EventArgs e)
        {
            Process.Start(_newGameFilesPath);
        }

        private void SettingsFormElements(bool hideElements = true)
        {
            if (hideElements)
            {
                currentWindowInfo.Text = "Please select your game settings:".ToUpper();
            }

            settingsSave.Visible = hideElements;
            settingsLanguage.Visible = hideElements;
            settingsLanguageText.Visible = hideElements;
            settingsLanguageDesc.Visible = hideElements;
            settingsQuality.Visible = hideElements;
            settingsQualityText.Visible = hideElements;
            settingsQualityDesc.Visible = hideElements;
            settingsGameFiles.Visible = hideElements;
            settingsGameFilesCurrent.Visible = hideElements;
            settingsGamePathText.Visible = hideElements;
            inputeditor.Visible = hideElements;
        }

        private void StartGame(string userId, string loginToken, string serverIp, Form x)
        {
            _nfswstarted = new Thread(() => LaunchGame(userId, loginToken, "http://127.0.0.1:" + Self.ProxyPort + "/nfsw/Engine.svc", this));

            _nfswstarted.IsBackground = true;
            _nfswstarted.Start();

            //var serverName = serverPick.GetItemText(serverPick.SelectedItem);
            var selectedServer = (ServerInfo) serverPick.SelectedItem;
            //_presenceLargeImageKey = Self.getDiscordRPCImageIDFromServerName(serverName, _slresponse);

            _presenceImageKey = selectedServer.DiscordPresenceKey;

            var handlers = new DiscordRpc.EventHandlers();
            DiscordRpc.Initialize(_discordrpccode, ref handlers, true, "");
            _presence.state = _realServername;
            _presence.details = "Loading game...";
            _presence.largeImageText = "Need for Speed: World";
            _presence.largeImageKey = "nfsw";
            _presence.smallImageText = _realServername;
            _presence.smallImageKey = _presenceImageKey;
            _presence.instance = true;
            DiscordRpc.UpdatePresence(_presence);
        }

        private void LaunchGameLegacy(string userId, string loginToken, string serverIp, Form x)
        {
			var oldfilename = _settingFile.Read("InstallationDirectory") + "\\nfsw.exe";
			var cParams = "SBRW " + serverIp + " " + loginToken + " " + userId + " -legacyLaunch";

			var proc = Process.Start(oldfilename, cParams);
            proc.EnableRaisingEvents = true;

            _nfswPid = proc.Id;

            proc.Exited += (sender2, e2) => {
                _nfswPid = 0;
                closebtn_Click(sender2, e2);
            };
        }

        private void LaunchGame(string userId, string loginToken, string serverIp, Form x)
        {
			var oldfilename = _settingFile.Read("InstallationDirectory") + "/nfsw.exe";

			var args = "SBRW " + serverIp + " " + loginToken + " " + userId + " -advancedLaunch";
            var psi = new ProcessStartInfo();
            //psi.UseShellExecute = false;
            
            if (!DetectLinux.UnixDetected()) {
                psi.FileName = oldfilename;
                psi.Arguments = args;
            } else {
                WineManager.InitWinePrefix();
                psi.EnvironmentVariables.Add("WINEDEBUG", "-d3d_shader,-d3d");
                psi.EnvironmentVariables.Add("WINEPREFIX", WineManager.GetWinePrefix());
                var wine = WineManager.GetWineDirectory();

                if (Directory.Exists(wine)) {
                    Console.WriteLine("Embedded wine found");
                    psi.EnvironmentVariables.Add("WINEVERPATH", wine);
                    psi.EnvironmentVariables.Add("WINESERVER", wine + "/bin/wineserver");
                    psi.EnvironmentVariables.Add("WINELOADER", wine + "/bin/wine");
                    psi.EnvironmentVariables.Add("WINEDLLPATH", wine + "/lib/wine/fakedlls");
                    psi.EnvironmentVariables.Add("LD_LIBRARY_PATH", wine + "/lib");
                    psi.FileName = wine + "/bin/wine";
                } else {
                    psi.FileName = "wine";
                }

                psi.Arguments = oldfilename + " " + args;
            }

            var nfswProcess = Process.Start(psi);
            if (nfswProcess != null) {
                nfswProcess.EnableRaisingEvents = true;
                _nfswPid = nfswProcess.Id;

                nfswProcess.Exited += (sender2, e2) => {
                    _nfswPid = 0;
                    var exitCode = nfswProcess.ExitCode;

                    if (exitCode == 0) {
                        closebtn_Click(null, null);
                    } else {
                        x.BeginInvoke(new Action(() => {
                            x.WindowState = FormWindowState.Normal;
                            x.Opacity = 1;
                            x.ShowInTaskbar = true;

                            String errorMsg = "Game Crash with exitcode: " + exitCode.ToString() + " (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073741819) errorMsg = "Game Crash: Access Violation (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073740940) errorMsg = "Game Crash: Heap Corruption (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073740791) errorMsg = "Game Crash: Stack buffer overflow (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -805306369) errorMsg = "Game Crash: Application Hang (0x" + exitCode.ToString("X") + ")";

                            playProgressText.Text = errorMsg.ToUpper();
                            playProgress.Value = 100;
                            playProgress.ForeColor = Color.Red;

                            if (_nfswPid != 0) {
                                try {
                                    Process.GetProcessById(_nfswPid).Kill();
                                } catch { /* ignored */ }
                            }

                            _nfswstarted.Abort();

                            var errorReply = MessageBox.Show(null,
                                errorMsg + "\nWould you like to restart the game?",
                                "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (errorReply == DialogResult.No) {
                                closebtn_Click(null, null);
                            } else {
                                Application.Restart();
                            }
                        }));
                    }
                };
            }
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            try {
                var LoadedBlackListXML = new XmlDocument();
                LoadedBlackListXML.LoadXml(_blacklistedXML);

                XmlNode BlackListNode = LoadedBlackListXML.SelectSingleNode("BlackList");
                XmlNodeList BlackList = BlackListNode.SelectNodes("Entry");
                foreach (XmlNode node in BlackList) {
                    var serverName = node.SelectSingleNode("Match");
                    var banReason = node.SelectSingleNode("Reason");

                    if(Regex.Match(_realServername, serverName.InnerText, RegexOptions.IgnoreCase).Success) {
                        MessageBox.Show(null, "This server has been banned by community votes. The final reason as of SBRW Team is:\n\n" + banReason.InnerText + "\n\nPlease select another server to play on.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            } catch(Exception ex) { Console.WriteLine(ex.Message); }

            if (_loggedIn == false)
            {
                if(_useSavedPassword == false) return;
                loginButton_Click(sender, e);
            }

            if (_playenabled == false)
            {
                return;
            }

            playButton.BackgroundImage = Properties.Resources.largebutton_enabled;

            var serverInfo = (ServerInfo)serverPick.SelectedItem;

            if (serverInfo.DistributionUrl != "" && serverInfo.Id != "nfsw")
            {
                DownloadMods(serverInfo.Id);
            }
            else
            {
                ModManager.ResetModDat(_settingFile.Read("InstallationDirectory"));
            }

            try
            {
                if (WebClientWithTimeout.createHash(_settingFile.Read("InstallationDirectory") + "/nfsw.exe") == "7C0D6EE08EB1EDA67D5E5087DDA3762182CDE4AC")
                {
                    ServerProxy.Instance.SetServerUrl(_serverIp);
                    ServerProxy.Instance.SetServerName(_realServername);

                    StartGame(_userId, _loginToken, _serverIp, this);

                    if (_builtinserver)
                    {
                        playProgressText.Text = "Soapbox server launched. Waiting for queries.".ToUpper();
                    }
                    else if (!DetectLinux.UnixDetected())
                    {
                        var secondsToCloseLauncher = 5;

                        while (secondsToCloseLauncher > 0)
                        {
                            playProgressText.Text = string.Format("Loading game. Launcher will minimize in {0} seconds.", secondsToCloseLauncher).ToUpper(); //"LOADING GAME. LAUNCHER WILL MINIMIZE ITSELF IN " + secondsToCloseLauncher + " SECONDS";
                            Delay.WaitSeconds(1);
                            secondsToCloseLauncher--;
                        }

                        playProgressText.Text = "You shall not see it!!!11!!".ToUpper();

                        WindowState = FormWindowState.Minimized;
                        ShowInTaskbar = false;

                        ContextMenu = new ContextMenu();

                        if (Environment.OSVersion.Version.Major >= 6)
                        {
                            ContextMenu.MenuItems.Add(new MenuItem("Check for updates.", Updater.CheckForUpdate));
                        }

                        ContextMenu.MenuItems.Add(new MenuItem("About", About.showAbout));
                        ContextMenu.MenuItems.Add(new MenuItem("Add Server", addServer_Click));
                        ContextMenu.MenuItems.Add("-");
                        ContextMenu.MenuItems.Add(new MenuItem("Close Launcher", (sender2, e2) =>
                        {
                            MessageBox.Show(null, "Please close the game before closing launcher.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));

                        Text = "NEED FOR SPEED™ WORLD";
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
                MessageBox.Show(null, "Failed to find NFSW.exe. Make sure you have \"Need for Speed™: World\" installed on your PC." + "\n\n" + ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void playButton_MouseUp(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            playButton.BackgroundImage = Properties.Resources.largebutton_hover;
        }

        private void playButton_MouseDown(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            playButton.BackgroundImage = Properties.Resources.largebutton_click;
        }

        private void playButton_MouseEnter(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            playButton.BackgroundImage = Properties.Resources.largebutton_hover;
        }

        private void playButton_MouseLeave(object sender, EventArgs e)
        {
            if (_playenabled == false)
            {
                return;
            }

            playButton.BackgroundImage = Properties.Resources.largebutton_enabled;
        }

        private void LaunchNfsw()
        {
            playButton.BackgroundImage = Properties.Resources.largebutton_disabled;
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

            if (!File.Exists(_settingFile.Read("InstallationDirectory") + "/Sound/Speech/copspeechhdr_" + speechFile + ".big"))
            {
                playProgressText.Text = "Loading list of files to download...".ToUpper();

                Kernel32.GetDiskFreeSpaceEx(_settingFile.Read("InstallationDirectory"), out var lpFreeBytesAvailable, out _, out _);
                if (lpFreeBytesAvailable <= 4000000000)
                {

                    extractingProgress.Value = 100;
                    extractingProgress.Width = 519;
                    extractingProgress.Image = Properties.Resources.warningprogress;
                    extractingProgress.ProgressColor = Color.Orange;

                    playProgressText.Text = "Please make sure you have at least 4GB free space on hard drive.".ToUpper();

                    TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Paused);
                    TaskbarProgress.SetValue(Handle, 100, 100);
                }
                else
                {
                    DownloadCoreFiles();
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
                    WebClient wc = new WebClientWithTimeout();
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
                ModManager.Download(ModManager.GetMods(serverKey), _settingFile.Read("InstallationDirectory"), serverKey, playProgressText, playProgress);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ModManager.ResetModDat(_settingFile.Read("InstallationDirectory"));
                return false;
            }
        }

        private string FormatFileSize(long byteCount)
        {
            var numArray = new double[] { 1000000000, 1000000, 1000, 0 };
            var strArrays = new[] { "GB", "MB", "KB", "Bytes" };
            for (var i = 0; i < numArray.Length; i++)
            {
                if (byteCount >= numArray[i])
                {
                    return string.Concat($"{byteCount / numArray[i]:0.00} ", strArrays[i]);
                }
            }

            return "0 Bytes";
        }

        private string EstimateFinishTime(long current, long total)
        {
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

        private void WineDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                OnDownloadProgress(e.TotalBytesToReceive, e.BytesReceived, e.TotalBytesToReceive + 1, "wine.tar.gz", 1);
            });
        }

        private void WineDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                if (File.Exists("wine.tar.gz") && !Directory.Exists("wine"))
                {
                    var thread = new Thread(() =>
                    {
                        Directory.CreateDirectory("wine");
                        playProgressText.Text = "EXTRACTING WINE";
                        if (DetectLinux.MacOSDetected())
                        {
                            Process.Start("tar", "xf wine.tar.gz -C wine --strip-components=1")?.WaitForExit();
                        }
                        else
                        {
                            Process.Start("tar", "xf wine.tar.gz -C wine")?.WaitForExit();
                        }
                        EnablePlayButton();
                    })
                    { IsBackground = true };

                    thread.Start();
                    return;
                }
            });
        }

        private void OnDownloadFinished()
        {
            try {
                File.WriteAllBytes(_settingFile.Read("InstallationDirectory") + "/GFX/BootFlow.gfx", ExtractResource.AsByte("GameLauncher.SoapBoxModules.BootFlow.gfx"));
            } catch {
                // ignored
            }

            if (DetectLinux.UnixDetected()) {
                if (WineManager.NeedEmbeddedWine() && !File.Exists("wine.tar.gz") && !Directory.Exists("wine")) {
                    var wineDownload = new WebClientWithTimeout();

                    wineDownload.DownloadProgressChanged += WineDownloadProgressChanged;
                    wineDownload.DownloadFileCompleted += WineDownloadCompleted;
                    if (DetectLinux.MacOSDetected()) {
                        wineDownload.DownloadFileAsync(new Uri("http://launcher.soapboxrace.world/winebuild/wine_macos.tar.gz"), "wine.tar.gz");
                    } else {
                        wineDownload.DownloadFileAsync(new Uri("http://launcher.soapboxrace.world/winebuild/wine_linux.tar.gz"), "wine.tar.gz");
                    }
                }
            }

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

            playButton.BackgroundImage = Properties.Resources.largebutton_enabled;
            playButton.ForeColor = Color.White;
            playProgressText.Text = "Download completed.".ToUpper();
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

        public void SetTranslations(string langId) {

        }

        public void ServerStatusBar(Pen color, Point startPoint, Point endPoint, int Thickness = 2) {
            Graphics _formGraphics = CreateGraphics();
            
            for (int x = 0; x <= Thickness; x++) {
                _formGraphics.DrawLine(color, new Point(startPoint.X, startPoint.Y-x), new Point(endPoint.X, endPoint.Y-x));
            }

            _formGraphics.Dispose();
        }
    }
}
