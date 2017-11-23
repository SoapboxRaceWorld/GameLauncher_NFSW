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
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net.NetworkInformation;
using GameLauncher.Resources;
using GameLauncher.App.Classes;
using Newtonsoft.Json;
using SoapBox.JsonScheme;
using GameLauncher.App.Classes.Events;
using GameLauncherReborn;
using Microsoft.Win32;

namespace GameLauncher {
    public partial class mainScreen : Form {
        Point mouseDownPoint = Point.Empty;
        bool loginEnabled;
        bool serverEnabled;
        bool builtinserver = false;
        bool useSavedPassword;
        bool skipServerTrigger = false;
        bool ticketRequired;
        bool serverlistloaded = false;
        bool windowMoved = false;
        bool playenabled = false;
        bool loggedIn = false;

        DateTime DownloadStartTime;

        String LoginToken = "";
        String UserId = "";
        String serverIP = "";
        String serverCacheKey = "18051995"; // Try to guess what this means for me :)
        String langInfo;
        String UILanguage;
        float DPIDefaultScale = 96f;

        Graphics formGraphics;
        Pen ColorOffline = new Pen(Color.FromArgb(128, 0, 0));
        Pen ColorOnline = new Pen(Color.FromArgb(0, 128, 0));
        Pen ColorLoading = new Pen(Color.FromArgb(0, 0, 0));

        DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();
        
        IniFile SettingFile = new IniFile("Settings.ini");
        string UserSettings = Environment.ExpandEnvironmentVariables("%AppData%\\Need for Speed World\\Settings\\UserSettings.xml");

        protected override void OnPaint(PaintEventArgs e) {
            Pen p = new Pen(Color.FromArgb(4, 9, 16));
            e.Graphics.DrawRectangle(p, new Rectangle(new Point(0, 0), new Size(this.Size.Width - 1, this.Size.Height - 1)));
            e.Graphics.DrawRectangle(p, new Rectangle(new Point(2, 2), new Size(this.Size.Width - 5, this.Size.Height - 5)));
        }

        private void moveWindow_MouseDown(object sender, MouseEventArgs e) {
            mouseDownPoint = new Point(e.X, e.Y);
        }

        private void moveWindow_MouseUp(object sender, MouseEventArgs e) {
            mouseDownPoint = Point.Empty;
            //this.Refresh();
            this.Opacity = 1;
        }

        private void moveWindow_MouseMove(object sender, MouseEventArgs e) {
            if (mouseDownPoint.IsEmpty) { return; }
            Form f = this as Form;
            f.Location = new Point(f.Location.X + (e.X - mouseDownPoint.X), f.Location.Y + (e.Y - mouseDownPoint.Y));
            windowMoved = true;
            this.Opacity = 0.9;
        }

        public void ConsoleLog(string e, string type) {
            if (type == "warning") {

            } else if (type == "info") {

            } else if (type == "error") {
                MessageBox.Show(null, e, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (type == "success") {

            } else if (type == "ping") {

            }
        }

        public mainScreen() {
            if (Environment.OSVersion.Version.Major <= 5) {
                if(DetectLinux.WineDetected() == false) { 
                    //MessageBox.Show(null, "Sadly, the red background cannot be fixed on Windows XP and lower, sorry...", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } else {
                Font = new Font(Font.Name, 8.25f * DPIDefaultScale / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            }

            InitializeComponent();

            if (SettingFile.KeyExists("UIlanguage")) {
                UILanguage = SettingFile.Read("UIlanguage");
            } else {
                UILanguage = "English";
            }

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            //if (DetectLinux.LinuxDetected() == false) {
                ApplyEmbeddedFonts();
            //}

            if(SettingFile.KeyExists("LauncherPosX") || SettingFile.KeyExists("LauncherPosY")) {
                StartPosition = FormStartPosition.Manual;
                int PosX = Int32.Parse(SettingFile.Read("LauncherPosX"));
                int PosY = Int32.Parse(SettingFile.Read("LauncherPosY"));
                Location = new Point(PosX, PosY);
            } else {
                Self.centerScreen(this);
            }

            MaximizeBox = false;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            closebtn.MouseEnter += new EventHandler(closebtn_MouseEnter);
            closebtn.MouseLeave += new EventHandler(closebtn_MouseLeave);
            closebtn.Click += new EventHandler(closebtn_Click);

            settingsButton.MouseEnter += new EventHandler(settingsButton_MouseEnter);
            settingsButton.MouseLeave += new EventHandler(settingsButton_MouseLeave);
            settingsButton.Click += new EventHandler(settingsButton_Click);

            minimizebtn.MouseEnter += new EventHandler(minimizebtn_MouseEnter);
            minimizebtn.MouseLeave += new EventHandler(minimizebtn_MouseLeave);
            minimizebtn.Click += new EventHandler(minimizebtn_Click);

            loginButton.MouseEnter += new EventHandler(loginButton_MouseEnter);
            loginButton.MouseLeave += new EventHandler(loginButton_MouseLeave);
            loginButton.Click += new EventHandler(loginButton_Click);
            loginButton.MouseUp += new MouseEventHandler(loginButton_MouseUp);
            loginButton.MouseDown += new MouseEventHandler(loginButton_MouseDown);

            registerButton.MouseEnter += new EventHandler(registerButton_MouseEnter);
            registerButton.MouseLeave += new EventHandler(registerButton_MouseLeave);
            registerButton.MouseUp += new MouseEventHandler(registerButton_MouseUp);
            registerButton.MouseDown += new MouseEventHandler(registerButton_MouseDown);
            registerButton.Click += new EventHandler(registerButton_Click);

            registerCancel.Click += new EventHandler(registerCancel_Click);
            registerCancel.MouseEnter += new EventHandler(registerCancel_MouseEnter);
            registerCancel.MouseLeave += new EventHandler(registerCancel_MouseLeave);
            registerCancel.MouseUp += new MouseEventHandler(registerCancel_MouseUp);
            registerCancel.MouseDown += new MouseEventHandler(registerCancel_MouseDown);

            logoutButton.Click += new EventHandler(logoutButton_Click);
            logoutButton.MouseEnter += new EventHandler(logoutButton_MouseEnter);
            logoutButton.MouseLeave += new EventHandler(logoutButton_MouseLeave);
            logoutButton.MouseUp += new MouseEventHandler(logoutButton_MouseUp);
            logoutButton.MouseDown += new MouseEventHandler(logoutButton_MouseDown);

            settingsSave.MouseEnter += new EventHandler(settingsSave_MouseEnter);
            settingsSave.MouseLeave += new EventHandler(settingsSave_MouseLeave);
            settingsSave.MouseUp += new MouseEventHandler(settingsSave_MouseUp);
            settingsSave.MouseDown += new MouseEventHandler(settingsSave_MouseDown);
            settingsSave.Click += new EventHandler(settingsSave_Click);

            email.KeyUp += new KeyEventHandler(loginbuttonenabler);
            email.KeyDown += new KeyEventHandler(loginEnter);
            password.KeyUp += new KeyEventHandler(loginbuttonenabler);
            password.KeyDown += new KeyEventHandler(loginEnter);

            serverPick.TextChanged += new EventHandler(serverPick_TextChanged);

            forgotPassword.LinkClicked += new LinkLabelLinkClickedEventHandler(forgotPassword_LinkClicked);

            moveWindow.MouseDown += new MouseEventHandler(moveWindow_MouseDown);
            moveWindow.MouseMove += new MouseEventHandler(moveWindow_MouseMove);
            moveWindow.MouseUp += new MouseEventHandler(moveWindow_MouseUp);

            playButton.MouseEnter += new EventHandler(playButton_MouseEnter);
            playButton.MouseLeave += new EventHandler(playButton_MouseLeave);
            playButton.Click += new EventHandler(playButton_Click);
            playButton.MouseUp += new MouseEventHandler(playButton_MouseUp);
            playButton.MouseDown += new MouseEventHandler(playButton_MouseDown);

            registerText.Click += new EventHandler(registerText_LinkClicked);

            //Simple check if we have enough permission to write file and remove them
            try {
                string file = Directory.GetCurrentDirectory() + "\\test.txt";
                File.WriteAllText(file, "test");
                File.Delete(file);
            } catch {
                MessageBox.Show(null, Language.getLangString("ERROR_NOPERMISSION", UILanguage), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Somewhere here we will setup the game installation directory
            if (String.IsNullOrEmpty(SettingFile.Read("InstallationDirectory"))) {
                    MessageBox.Show(null, Language.getLangString("INSTALL_INFO", UILanguage), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    var fbd = new FolderBrowserDialog();
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK) {
                        SettingFile.Write("InstallationDirectory", fbd.SelectedPath);
                    } else {
                        Environment.Exit(Environment.ExitCode);
                    }
            }

            //Replace cursor
            if (File.Exists(SettingFile.Read("InstallationDirectory") + "\\Media\\Cursors\\default.cur")) {
                Cursor mycursor = new Cursor(Cursor.Current.Handle);
                IntPtr colorcursorhandle = User32.LoadCursorFromFile(SettingFile.Read("InstallationDirectory") + "\\Media\\Cursors\\default.cur");
                mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, null, mycursor, new object[] { colorcursorhandle });
                this.Cursor = mycursor;
            }

            var pos = this.PointToScreen(imageServerName.Location);
            pos = verticalBanner.PointToClient(pos);
            imageServerName.Parent = verticalBanner;
            imageServerName.Location = pos;
            imageServerName.BackColor = Color.Transparent;

            var pos2 = this.PointToScreen(onlineCount.Location);
            pos2 = verticalBanner.PointToClient(pos2);
            onlineCount.Parent = verticalBanner;
            onlineCount.Location = pos2;
            onlineCount.BackColor = Color.Transparent;

            formGraphics = this.CreateGraphics();
            formGraphics.DrawRectangle(ColorLoading, new Rectangle(new Point(30, 125), new Size(372, 274)));
            formGraphics.DrawRectangle(ColorLoading, new Rectangle(new Point(29, 124), new Size(374, 276)));
            formGraphics.Dispose();
        }

        private void mainScreen_Load(object sender, EventArgs e) {
            launcherVersion.Text = "v" + Application.ProductVersion;

            if (!SettingFile.KeyExists("SkipUpdate")) {
                Updater.checkForUpdate(sender, e);
            }

            var UIlanguages = Language.getLanguages();
            settingsUILang.DisplayMember = "Text";
            settingsUILang.ValueMember = "Value";
            settingsUILang.DataSource = UIlanguages;

            if (SettingFile.KeyExists("UILanguage")) {
                settingsUILang.SelectedValue = SettingFile.Read("UILanguage");
                UILanguage = SettingFile.Read("UILanguage");
            } else {
                settingsUILang.SelectedValue = "English";
                UILanguage = "English";
            }

            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(new MenuItem(Language.getLangString("CONTEXT_CHECKUPDATE", UILanguage), Updater.checkForUpdate));
            ContextMenu.MenuItems.Add(new MenuItem(Language.getLangString("CONTEXT_ABOUT", UILanguage), About.showAbout));
            ContextMenu.MenuItems.Add(new MenuItem(Language.getLangString("CONTEXT_SETTINGS", UILanguage), settingsButton_Click));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem(Language.getLangString("CONTEXT_CLOSE", UILanguage), closebtn_Click));

            Notification.ContextMenu = ContextMenu;
            Notification.Icon = new Icon(Icon, Icon.Width, Icon.Height);
            Notification.Text = "GameLauncher";
            Notification.Visible = true;

            this.ContextMenu = null;

            email.Text = SettingFile.Read("AccountEmail");
            if (!String.IsNullOrEmpty(SettingFile.Read("AccountEmail")) && !String.IsNullOrEmpty(SettingFile.Read("Password"))) {
                rememberMe.Checked = true;
            }

            //Fetch serverlist, and disable if failed to fetch.
            var response = "";

            try {
                WebClient wc = new WebClientWithTimeout();

                string serverurl = "http://nfsw.metonator.ct8.pl/serverlist.txt";
                response += wc.DownloadString(serverurl);

                serverlistloaded = true;

                try {
                    FileStream fileStream = new FileStream("ServerCache", FileMode.Create);

                    DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider() {
                        Key = Encoding.ASCII.GetBytes(serverCacheKey),
                        IV = Encoding.ASCII.GetBytes(serverCacheKey)
                    };

                    CryptoStream cryptoStream = new CryptoStream(fileStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
                    StreamWriter streamWriter = new StreamWriter(cryptoStream);
                    streamWriter.Write(response);
                    streamWriter.Close();
                } catch { }
            } catch {
                if(File.Exists("ServerCache")) {
                    FileStream fileStream = new FileStream("ServerCache", FileMode.Open);

                    DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider() {
                        Key = Encoding.ASCII.GetBytes(serverCacheKey),
                        IV = Encoding.ASCII.GetBytes(serverCacheKey)
                    };

                    CryptoStream cryptoStream = new CryptoStream(fileStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read);
                    StreamReader streamReader = new StreamReader(cryptoStream);
                    response = streamReader.ReadToEnd();

                    serverlistloaded = true;
                } else { }
            }

            if (!File.Exists("servers.txt")) {
                File.Create("servers.txt");
            } else {
                response += File.ReadAllText("servers.txt");
            }

            //Time to add servers
            serverPick.DisplayMember = "Text";
            serverPick.ValueMember = "Value";

            List<Object> items = new List<Object>();

            String[] substrings = response.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var substring in substrings) {
                if (!String.IsNullOrEmpty(substring)) {
                    String[] substrings2 = substring.Split(new string[] { ";" }, StringSplitOptions.None);
                    items.Add(new { Text = substrings2[0], Value = substrings2[1] });
                }
            }

            serverPick.DataSource = items;

            //Silliest way to prevent doublecall of TextChanged event...
            if(serverlistloaded == true) {
                try {
                    serverPick.SelectedIndex = 0;
                } catch { }

                if (!SettingFile.KeyExists("Server")) {
                    SettingFile.Write("Server", serverPick.SelectedValue.ToString());
                }

                if(SettingFile.KeyExists("Server")) {
                    skipServerTrigger = true;

                    if(response.Contains(SettingFile.Read("Server"))) { 
                        serverPick.SelectedValue = SettingFile.Read("Server");
                    }

                    //I don't know other way to fix this call...
                    if(serverPick.SelectedIndex == 0) {
                        serverPick_TextChanged(sender, e);
                    }
                }
            }

            if (SettingFile.KeyExists("Password")) {
                loginEnabled = true;
                serverEnabled = true;
                useSavedPassword = true;
                this.loginButton.Image = Properties.Resources.smallbutton_enabled;
                this.loginButton.ForeColor = Color.White;
            } else {
                loginEnabled = false;
                serverEnabled = false;
                useSavedPassword = false;
                this.loginButton.Image = Properties.Resources.smallbutton_disabled;
                this.loginButton.ForeColor = Color.Gray;
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

            if(SettingFile.KeyExists("Language")) {
                settingsLanguage.SelectedValue = SettingFile.Read("Language");
            }

            //Add downloadable quality to settingLanguage
            settingsQuality.DisplayMember = "Text";
            settingsQuality.ValueMember = "Value";

            var quality = new[] {
                new { Text = "Standard", Value = "0" },
                new { Text = "Maximum", Value = "1" },
            };

            settingsQuality.DataSource = quality;

            if(SettingFile.KeyExists("TracksHigh")) {
                settingsQuality.SelectedValue = SettingFile.Read("TracksHigh");
            }

            string drive = Path.GetPathRoot(SettingFile.Read("InstallationDirectory"));
            if (!Directory.Exists(drive)) {
                string newdir = Directory.GetCurrentDirectory() + "\\GameFiles";
                SettingFile.Write("InstallationDirectory", newdir);
                MessageBox.Show(null, String.Format(Language.getLangString("ERROR_404DRIVE", UILanguage), drive, newdir), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Soapbox Modules (without them Freeroam might fail)
            try {
                Directory.CreateDirectory(SettingFile.Read("InstallationDirectory"));
                if(!File.Exists(SettingFile.Read("InstallationDirectory") + "/lightfx.dll")) {
                    File.WriteAllBytes(SettingFile.Read("InstallationDirectory") + "/lightfx.dll", ExtractResource.AsByte("GameLauncher.SoapBoxModules.lightfx.dll"));
                    Directory.CreateDirectory(SettingFile.Read("InstallationDirectory") + "/modules");
                    File.WriteAllText(SettingFile.Read("InstallationDirectory") + "/modules/udpcrc.soapbox.module", ExtractResource.AsString("GameLauncher.SoapBoxModules.udpcrc.soapbox.module"));
                    File.WriteAllText(SettingFile.Read("InstallationDirectory") + "/modules/udpcrypt1.soapbox.module", ExtractResource.AsString("GameLauncher.SoapBoxModules.udpcrypt1.soapbox.module"));
                    File.WriteAllText(SettingFile.Read("InstallationDirectory") + "/modules/udpcrypt2.soapbox.module",  ExtractResource.AsString("GameLauncher.SoapBoxModules.udpcrypt2.soapbox.module"));
                    File.WriteAllText(SettingFile.Read("InstallationDirectory") + "/modules/xmppsubject.soapbox.module",  ExtractResource.AsString("GameLauncher.SoapBoxModules.xmppsubject.soapbox.module"));
                }
            } catch(Exception ex) {
                MessageBox.Show(null, ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                closebtn_Click(null, null);
            }

            if(!File.Exists("discord-rpc.dll")) {
                File.WriteAllBytes("discord-rpc.dll", ExtractResource.AsByte("GameLauncher.Discord.discord-rpc.dll"));
            }

            //Hide other windows
            RegisterFormElements(false);
            SettingsFormElements(false);
            LoggedInFormElements(false);

            //Reshow missing elements
            LoginFormElements(true);

            //Command-line Arguments
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2) {
                MessageBox.Show(Language.getLangString("MAIN_UPDATED", UILanguage));
            }

            //Possible fix for "MAXIMUM" texture (untested, but worth adding that refference)
            try {
                String GameInstallDirValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "GameInstallDir", RegistryValueKind.String).ToString();
                if (GameInstallDirValue != Path.GetFullPath(SettingFile.Read("InstallationDirectory"))) {
                    try {
                        Registry.SetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "GameInstallDir", Path.GetFullPath(SettingFile.Read("InstallationDirectory")));
                        Registry.SetValue("HKEY_LOCAL_MACHINE\\software\\Electronic Arts\\Need For Speed World", "LaunchInstallDir", Path.GetFullPath(Application.ExecutablePath));
                    } catch { }
                }
            } catch { }

            //Somewhere here translations?
            if(String.IsNullOrEmpty(SettingFile.Read("UILanguage"))) {
                UILanguage = "English";
            } else {
                UILanguage = SettingFile.Read("UILanguage");
            }

            setTranslations(UILanguage);

            this.BeginInvoke((MethodInvoker)delegate {
                launchNFSW();
            });
        }

        private void closebtn_Click(object sender, EventArgs e) {
            this.closebtn.BackgroundImage = Properties.Resources.close_click;

            Notification.Visible = false;

            long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000000;
            string timestamp = ticks.ToString();

            if(serverlistloaded == true) {
                SettingFile.Write("Server", serverPick.SelectedValue.ToString());
            }

            if(windowMoved) {
                SettingFile.Write("LauncherPosX", this.Location.X.ToString());
                SettingFile.Write("LauncherPosY", this.Location.Y.ToString());
            }

            //Fix InstallationDirectory
            SettingFile.Write("InstallationDirectory", Path.GetFullPath(SettingFile.Read("InstallationDirectory")));

            //Discord unload?
            DiscordRpc.Shutdown();

            //Dirty way to terminate application (sometimes Application.Exit() didn't really quitted, was still running in background)
            Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
        }

        private void closebtn_MouseEnter(object sender, EventArgs e) {
            this.closebtn.BackgroundImage = Properties.Resources.close_hover;
        }

        private void closebtn_MouseLeave(object sender, EventArgs e) {
            this.closebtn.BackgroundImage = Properties.Resources.close;
        }

        private void minimizebtn_Click(object sender, EventArgs e) {
            this.minimizebtn.BackgroundImage = Properties.Resources.minimize_click;
            this.WindowState = FormWindowState.Minimized;
        }

        private void minimizebtn_MouseEnter(object sender, EventArgs e) {
            this.minimizebtn.BackgroundImage = Properties.Resources.minimize_hover;
        }

        private void minimizebtn_MouseLeave(object sender, EventArgs e) {
            this.minimizebtn.BackgroundImage = Properties.Resources.minimize;
        }

        private void loginEnter(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Return && loginEnabled == true) {
                loginButton_Click(null, null);
                e.SuppressKeyPress = true;
            } else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V) {
                e.SuppressKeyPress = true;
            }
        }

        private void loginbuttonenabler(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(email.Text) || String.IsNullOrEmpty(password.Text)) {
                loginEnabled = false;
                this.loginButton.Image = Properties.Resources.smallbutton_disabled;
                this.loginButton.ForeColor = Color.Gray;
            }
            else {
                loginEnabled = true;
                this.loginButton.Image = Properties.Resources.smallbutton_enabled;
                this.loginButton.ForeColor = Color.White;
            }

            useSavedPassword = false;
        }

        private void loginButton_MouseUp(object sender, EventArgs e) {
            if (loginEnabled == true || builtinserver == true) {
                this.loginButton.Image = Properties.Resources.smallbutton_hover;
            } else {
                this.loginButton.Image = Properties.Resources.smallbutton_disabled;
            }
        }

        private void loginButton_MouseDown(object sender, EventArgs e) {
            if (loginEnabled == true || builtinserver == true) {
                this.loginButton.Image = Properties.Resources.smallbutton_click;
            } else {
                this.loginButton.Image = Properties.Resources.smallbutton_disabled;
            }
        }

        private void loginButton_Click(object sender, EventArgs e) {
            if((loginEnabled == false || serverEnabled == false) && builtinserver == false) {
                return;
            }

            serverIP = serverPick.SelectedValue.ToString();
            string serverName = serverPick.GetItemText(serverPick.SelectedItem);
            string username = email.Text.ToString();
            string encryptedpassword = "";
            string serverLoginResponse = "";

            HashAlgorithm algorithm = SHA1.Create();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(password.Text.ToString()))) {
                sb.Append(b.ToString("X2"));
            }

            if (useSavedPassword) {
                encryptedpassword = SettingFile.Read("Password");
            } else {
                encryptedpassword = sb.ToString();
            }

            if (rememberMe.Checked) {
                SettingFile.Write("AccountEmail", username);
                SettingFile.Write("Password", encryptedpassword.ToLower());
            } else {
                SettingFile.DeleteKey("AccountEmail");
                SettingFile.DeleteKey("Password");
            }

            try {
                WebClient wc = new WebClientWithTimeout();
                wc.Headers.Add("user-agent", "GameLauncher (+https://github.com/metonator/GameLauncher_NFSW)");

                string BuildURL = serverIP + "/User/authenticateUser?email=" + username + "&password=" + encryptedpassword.ToLower();

                serverLoginResponse = wc.DownloadString(BuildURL);
            } catch (WebException ex) {
                if (ex.Status == WebExceptionStatus.ProtocolError) {
                    HttpWebResponse serverReply = (HttpWebResponse)ex.Response;
                    if ((int)serverReply.StatusCode == 500) {
                        using (StreamReader sr = new StreamReader(serverReply.GetResponseStream())) {
                            serverLoginResponse = sr.ReadToEnd();
                        }
                    } else {
                        serverLoginResponse = ex.Message;
                    }
                } else {
                    serverLoginResponse = ex.Message;
                }
            }

            try {
                XmlDocument SBRW_XML = new XmlDocument();

                if (builtinserver == false) {
                    SBRW_XML.LoadXml(serverLoginResponse);
                } else {
                    SBRW_XML.LoadXml("<LoginStatusVO><UserId>1</UserId><LoginToken>aaaaaaaa-aaaa-aaaa-aaaaaaaa</LoginToken><Description/></LoginStatusVO>");
                }

                XmlNode DescriptionNode;
                XmlNode LoginTokenNode;
                XmlNode UserIdNode;

                DescriptionNode = SBRW_XML.SelectSingleNode("LoginStatusVO/Description");
                LoginTokenNode = SBRW_XML.SelectSingleNode("LoginStatusVO/LoginToken");
                UserIdNode = SBRW_XML.SelectSingleNode("LoginStatusVO/UserId");

                if(String.IsNullOrEmpty(DescriptionNode.InnerText)) {
                    UserId = UserIdNode.InnerText;
                    LoginToken = LoginTokenNode.InnerText;

                    loggedIn = true;

                    this.BackgroundImage = Properties.Resources.loggedbg;
                    LoginFormElements(false);
                    LoggedInFormElements(true);

                    this.welcomeBack.Text = String.Format(Language.getLangString("MAIN_WELCOMEBACK", UILanguage), username).ToUpper();
                } else {
                     ConsoleLog(Language.getLangString("ERROR_INVALIDCREDS", UILanguage), "error");
                }
            } catch {
                ConsoleLog(Language.getLangString("ERROR_SERVEROFFLINE", UILanguage), "error");
            }
        }

        private void loginButton_MouseEnter(object sender, EventArgs e) {
            if (loginEnabled == true || builtinserver == true) {
                this.loginButton.Image = Properties.Resources.smallbutton_hover;
                this.loginButton.ForeColor = Color.White;
            } else {
                this.loginButton.Image = Properties.Resources.smallbutton_disabled;
                this.loginButton.ForeColor = Color.Gray;
            }
        }

        private void loginButton_MouseLeave(object sender, EventArgs e) {
            if (loginEnabled == true || builtinserver == true) {
                this.loginButton.Image = Properties.Resources.smallbutton_enabled;
                this.loginButton.ForeColor = Color.White;
            } else {
                this.loginButton.Image = Properties.Resources.smallbutton_disabled;
                this.loginButton.ForeColor = Color.Gray;
            }
        }

        private void serverPick_TextChanged(object sender, EventArgs e) {
            if (!skipServerTrigger) { return; }

            formGraphics = this.CreateGraphics();
            formGraphics.DrawRectangle(ColorLoading, new Rectangle(new Point(30, 125), new Size(372, 274)));
            formGraphics.DrawRectangle(ColorLoading, new Rectangle(new Point(29, 124), new Size(374, 276)));
            formGraphics.Dispose();

            imageServerName.Text = serverPick.GetItemText(serverPick.SelectedItem);

            loginEnabled = false;

            this.loginButton.ForeColor = Color.Gray;
            this.password.Text = "";
            string verticalImageUrl = "";
            verticalBanner.Image = null;
            verticalBanner.BackColor = Color.Transparent;

            string serverIP = serverPick.SelectedValue.ToString();
            string numPlayers;
            string serverName = serverPick.GetItemText(serverPick.SelectedItem);

            var WordsArray = serverName.Split();
            string richPresenceIconID = WordsArray[0] + WordsArray[1];
            richPresenceIconID = richPresenceIconID.ToLower();

            onlineCount.Text = "";

            if (serverPick.GetItemText(serverPick.SelectedItem) == "Offline Built-In Server") {
                builtinserver = true;
                this.loginButton.Image = Properties.Resources.smallbutton_enabled;
                this.loginButton.Text = Language.getLangString("MAIN_LAUNCH", UILanguage).ToUpper();
                this.loginButton.ForeColor = Color.White;
            } else {
                builtinserver = false;
                this.loginButton.Image = Properties.Resources.smallbutton_disabled;
                this.loginButton.Text = Language.getLangString("MAIN_LOGIN", UILanguage).ToUpper();
                this.loginButton.ForeColor = Color.Gray;
            }

            var client = new WebClientWithTimeout();
            client.Headers.Add("user-agent", "GameLauncher (+https://github.com/metonator/GameLauncher_NFSW)");

            Uri StringToUri = new Uri(serverIP + "/GetServerInformation");
            client.CancelAsync();
            client.DownloadStringAsync(StringToUri);
            client.DownloadStringCompleted += (sender2, e2) => {
                if (e2.Cancelled) {
                    client.CancelAsync();
                    return;
                } else if (e2.Error != null) {
                    if (File.Exists("discord-rpc.dll")) {
                        DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();
                        DiscordRpc.Initialize("378322260655603713", ref handlers, true, "");

                        presence.details = serverPick.GetItemText(serverPick.SelectedItem);
                        presence.state = "OFFLINE";
                        presence.largeImageText = "SBRW";
                        presence.largeImageKey = "icon2";
                        presence.partySize = 0;
                        presence.partyMax = 0;
                        presence.instance = true;
                        DiscordRpc.UpdatePresence(ref presence);
                    }

                    formGraphics = this.CreateGraphics();
                    formGraphics.DrawRectangle(ColorOffline, new Rectangle(new Point(30, 125), new Size(372, 274)));
                    formGraphics.DrawRectangle(ColorOffline, new Rectangle(new Point(29, 124), new Size(374, 276)));
                    formGraphics.Dispose();

                    onlineCount.Text = "";
                    serverEnabled = false;
                } else {

                    if (serverName == "Offline Built-In Server") {
                        numPlayers = "∞";
                    } else {
                        if (Environment.OSVersion.Version.Major <= 5) {
                            ticketRequired = true;
                            verticalImageUrl = null;
                            numPlayers = Language.getLangString("MAIN_UNKNOWN", UILanguage);
                        } else {
                            GetServerInformation json = JsonConvert.DeserializeObject<GetServerInformation>(e2.Result);
                            if (!String.IsNullOrEmpty(json.bannerUrl)) {
                                Uri uriResult;
                                bool result;


                                try {
                                    result = Uri.TryCreate(json.bannerUrl, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
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

                            if(String.IsNullOrEmpty(json.requireTicket)) {
                                ticketRequired = true;
                            } else if(json.requireTicket == "true") {
                                ticketRequired = true;
                            } else {
                                ticketRequired = false;
                            }

                            numPlayers = String.Format(Language.getLangString("MAIN_PLAYERSOUTOF", UILanguage), json.onlineNumber, json.numberOfRegistered);

                            if (File.Exists("discord-rpc.dll")) {
                                DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();
                                DiscordRpc.Initialize("378322260655603713", ref handlers, true, "");

                                presence.details = serverName;
                                presence.state = "ONLINE";
                                presence.largeImageText = serverName;
                                presence.largeImageKey = richPresenceIconID;
                                presence.partySize = json.onlineNumber;
                                presence.partyMax = json.numberOfRegistered;
                                presence.partyId = "SBRW";
                                presence.matchSecret = "SBRW2";
                                presence.joinSecret = "SBRW3";
                                presence.spectateSecret = "SBRW4";
                                presence.instance = true;
                                DiscordRpc.UpdatePresence(ref presence);
                            }

                            formGraphics = this.CreateGraphics();
                            formGraphics.DrawRectangle(ColorOnline, new Rectangle(new Point(30, 125), new Size(372, 274)));
                            formGraphics.DrawRectangle(ColorOnline, new Rectangle(new Point(29, 124), new Size(374, 276)));
                            formGraphics.Dispose();
                        }
                    }

                    onlineCount.Text = String.Format(Language.getLangString("MAIN_PLAYERSONSERVER", UILanguage), numPlayers); // "Players on server: " + numPlayers;
                    serverEnabled = true;

                    if (!String.IsNullOrEmpty(verticalImageUrl)) {
                        var client2 = new WebClientWithTimeout();
                        Uri StringToUri3 = new Uri(verticalImageUrl);
                        client2.DownloadDataAsync(StringToUri3);
                        client2.DownloadDataCompleted += (sender4, e4) => {
                            if (e4.Cancelled) {
                                client2.CancelAsync();
                                return;
                            } else if (e4.Error != null) {
                                //What?
                            } else {
                                try {
                                    Image image;
                                    MemoryStream memoryStream = new MemoryStream(e4.Result);
                                    image = Image.FromStream(memoryStream);
                                    verticalBanner.Image = image;
                                } catch {
                                    verticalBanner.Image = null;
                                }
                            }
                        };
                    }

                    onlineCount.Text += ". ";

                    if(DetectLinux.WineDetected() == false) { 
                    Ping pingSender = new Ping();
                        pingSender.SendAsync(StringToUri.Host, 1000, new byte[1], new PingOptions(64, true), new AutoResetEvent(false));
                        pingSender.PingCompleted += (sender3, e3) => {
                            PingReply reply = e3.Reply;

                            if (reply.Status == IPStatus.Success && serverName != "Offline Built-In Server") {
                                onlineCount.Text += String.Format(Language.getLangString("MAIN_PINGSUCCESS", UILanguage), reply.RoundtripTime);
                            } else {
                                onlineCount.Text += Language.getLangString("MAIN_PINGFAILED", UILanguage);
                            }
                        };
                    } else {
                        onlineCount.Text += Language.getLangString("MAIN_PINGDISABLED", UILanguage);
                    }
                }
            };
        }

        private void ApplyEmbeddedFonts() {
            FontFamily fontFamily1 = FontWrapper.Instance.GetFontFamily("Montserrat-Regular.ttf");
            FontFamily fontFamily2 = FontWrapper.Instance.GetFontFamily("Montserrat-Bold.ttf");

            //Implement them to elements
            email.Font = new Font(fontFamily1, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            loginButton.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            password.Font = new Font(fontFamily1, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            rememberMe.Font = new Font(fontFamily1, 8f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            emailLabel.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            passwordLabel.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            launcherVersion.Font = new Font(fontFamily1, 8f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            forgotPassword.Font = new Font(fontFamily2, 9f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            onlineCount.Font = new Font(fontFamily1, 8f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            playProgressText.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            playProgressTime.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            playButton.Font = new Font(fontFamily2, 15f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            currentWindowInfo.Font = new Font(fontFamily2, 11.35f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            imageServerName.Font = new Font(fontFamily2, 25f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            registerEmailText.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerPasswordText.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerConfirmPasswordText.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerTicketText.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerAgree.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerButton.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerCancel.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            settingsLanguageText.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsQualityText.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsSave.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsLanguageDesc.Font = new Font(fontFamily1, 8f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            settingsQualityDesc.Font = new Font(fontFamily1, 8f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            settingsUILangDesc.Font = new Font(fontFamily1, 8f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            settingsUILangText.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

            logoutButton.Font = new Font(fontFamily2, 10f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);

        }

        private void registerText_LinkClicked(object sender, EventArgs e) {
            this.BackgroundImage = Properties.Resources.secondarybackground;
            this.currentWindowInfo.Text = String.Format(Language.getLangString("MAIN_INFORMATIONREG", UILanguage), serverPick.GetItemText(serverPick.SelectedItem)).ToUpper();
            LoginFormElements(false);
            RegisterFormElements(true);
        }

        private void githubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ConsoleLog("Redirecting into GitHub Issue page", "info");
            Process.Start("https://discord.gg/JqN2nMY");
        }

        private void forgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(serverPick.SelectedValue.ToString().Replace("Engine.svc", "") + "forgotPasswd.jsp");
        }

        private void LoggedInFormElements(bool hideElements) {
            if (hideElements == true) {
                this.currentWindowInfo.Text = Language.getLangString("MAIN_INFORMATION", UILanguage).ToUpper();
                this.currentWindowInfo.Location = new Point(479, 140);
                this.currentWindowInfo.Size = new Size(222, 46);
            }

            this.logoutButton.Visible = hideElements;
            this.playProgress.Visible = hideElements;
            this.playProgressText.Visible = hideElements;
            this.playProgressTime.Visible = hideElements;
            this.playButton.Visible = hideElements;
            this.settingsButton.Visible = hideElements;
            this.verticalBanner.Visible = hideElements;
            this.onlineCount.Visible = hideElements;
            this.welcomeBack.Visible = hideElements;
        }

        private void LoginFormElements(bool hideElements = false) {
            if(hideElements == true) {
                this.currentWindowInfo.Text = Language.getLangString("MAIN_INFORMATION", UILanguage).ToUpper();
                this.currentWindowInfo.Location = new Point(479, 140);
                this.currentWindowInfo.Size = new Size(222, 46);
            }

            this.rememberMe.Visible = hideElements;
            this.loginButton.Visible = hideElements;
            this.onlineCount.Visible = hideElements;
            this.registerText.Visible = hideElements;
            this.serverPick.Visible = hideElements;
            this.email.Visible = hideElements;
            this.password.Visible = hideElements;
            this.emailLabel.Visible = hideElements;
            this.passwordLabel.Visible = hideElements;
            this.forgotPassword.Visible = hideElements;
            this.settingsButton.Visible = hideElements;
            this.verticalBanner.Visible = hideElements;
            this.playProgressTime.Visible = hideElements;
            this.playProgressText.Visible = hideElements;
            this.playProgress.Visible = hideElements;
            this.playButton.Visible = hideElements;
        }

        /*
         * REGISTER PAGE LAYOUT
         * Because why should i close Form1 and create/open Form2 if it will look a bit more responsive...
         */

        public bool validateEmail(string email) {
            String theEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                   + "@"
                                   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

            return Regex.IsMatch(email, theEmailPattern);
        }

        private void RegisterFormElements(bool hideElements = true) {
            if(hideElements == true) {
                this.currentWindowInfo.Location = new Point(53, 150);
                this.currentWindowInfo.Size = new Size(700, 46);
            }

            this.registerButton.Visible = hideElements;
            this.registerEmail.Visible = hideElements;
            this.registerEmailText.Visible = hideElements;
            this.registerPassword.Visible = hideElements;
            this.registerPasswordText.Visible = hideElements;
            this.registerConfirmPassword.Visible = hideElements;
            this.registerConfirmPasswordText.Visible = hideElements;
            this.registerAgree.Visible = hideElements;
            this.registerCancel.Visible = hideElements;

            if(ticketRequired) {
                this.registerTicket.Visible = hideElements;
                this.registerTicketText.Visible = hideElements;
            } else {
                this.registerTicket.Visible = false;
                this.registerTicketText.Visible = false;
            }
        }

        private void logoutButton_Click(object sender, EventArgs e) {
            DialogResult reply = MessageBox.Show(null, String.Format(Language.getLangString("MAIN_LOGOUTCONFIRM", UILanguage), serverPick.GetItemText(serverPick.SelectedItem)), "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (reply == DialogResult.Yes) {
                this.BackgroundImage = Properties.Resources.loginbg;
                loggedIn = false;
                LoggedInFormElements(false);
                LoginFormElements(true);
            }
        }

        private void logoutButton_MouseDown(object sender, EventArgs e) {
            this.logoutButton.Image = Properties.Resources.smallbutton_click;
        }

        private void logoutButton_MouseEnter(object sender, EventArgs e) {
            this.logoutButton.Image = Properties.Resources.smallbutton_hover;
        }

        private void logoutButton_MouseLeave(object sender, EventArgs e) {
            this.logoutButton.Image = Properties.Resources.smallbutton_enabled;
        }

        private void logoutButton_MouseUp(object sender, EventArgs e) {
            this.logoutButton.Image = Properties.Resources.smallbutton_hover;
        }

        private void registerButton_MouseEnter(object sender, EventArgs e) {
            this.registerButton.Image = Properties.Resources.smallbutton_hover;
        }

        private void registerButton_MouseLeave(object sender, EventArgs e) {
            this.registerButton.Image = Properties.Resources.smallbutton_enabled;
        }

        private void registerButton_MouseUp(object sender, EventArgs e) {
            this.registerButton.Image = Properties.Resources.smallbutton_hover;
        }

        private void registerButton_MouseDown(object sender, EventArgs e) {
            this.registerButton.Image = Properties.Resources.smallbutton_click;
        }

        private void registerCancel_Click(object sender, EventArgs e) {
            this.BackgroundImage = Properties.Resources.loginbg;
            this.currentWindowInfo.Text = Language.getLangString("MAIN_INFORMATION", UILanguage).ToUpper();
            RegisterFormElements(false);
            LoginFormElements(true);
        }

        private void registerCancel_MouseDown(object sender, EventArgs e) {
            this.registerCancel.Image = Properties.Resources.cancelbutton_click;
        }

        private void registerCancel_MouseEnter(object sender, EventArgs e) {
            this.registerCancel.Image = Properties.Resources.cancelbutton_hover;
        }

        private void registerCancel_MouseLeave(object sender, EventArgs e) {
            this.registerCancel.Image = Properties.Resources.cancelbutton_enabled;
        }

        private void registerCancel_MouseUp(object sender, EventArgs e) {
            this.registerCancel.Image = Properties.Resources.cancelbutton_hover;
        }

        private void registerButton_Click(object sender, EventArgs e) {
            bool registerSuccess = true;

            if(String.IsNullOrEmpty(registerEmail.Text)) {
                ConsoleLog(Language.getLangString("ERROR_REGISTER_MAILEMPTY", UILanguage), "error");
                registerSuccess = false;
            } else if(validateEmail(registerEmail.Text) == false) {
                ConsoleLog(Language.getLangString("ERROR_REGISTER_MAILCORRECT", UILanguage), "error");
                registerSuccess = false;
            }

            if(String.IsNullOrEmpty(registerTicket.Text) && ticketRequired == true) {
                ConsoleLog(Language.getLangString("ERROR_REGISTER_TICKET", UILanguage), "error");
                registerSuccess = false;
            }

            if(String.IsNullOrEmpty(registerPassword.Text)) {
                ConsoleLog(Language.getLangString("ERROR_REGISTER_PASSWORDEMPTY", UILanguage), "error");
                registerSuccess = false;
            }

            if(String.IsNullOrEmpty(registerConfirmPassword.Text)) {
                ConsoleLog(Language.getLangString("ERROR_REGISTER_PASSWORDVERIFY", UILanguage), "error");
                registerSuccess = false;
            }

            if(registerConfirmPassword.Text != registerPassword.Text) {
                ConsoleLog(Language.getLangString("ERROR_REGISTER_PASSWORDMISMATCH", UILanguage), "error");
            }

            if(!registerAgree.Checked) {
                ConsoleLog(Language.getLangString("ERROR_REGISTER_TOS", UILanguage), "error");
                registerSuccess = false;
            }

            if(registerSuccess == true) {
                serverIP = serverPick.SelectedValue.ToString();
                string serverName = serverPick.GetItemText(serverPick.SelectedItem);
                string encryptedpassword = "";
                string serverLoginResponse = "";
                string BuildURL;

                HashAlgorithm algorithm = SHA1.Create();
                StringBuilder sb = new StringBuilder();
                foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(registerPassword.Text.ToString()))) {
                    sb.Append(b.ToString("X2"));
                }

                encryptedpassword = sb.ToString();

                try {
                    WebClient wc = new WebClientWithTimeout();

                    if(ticketRequired) {
                        BuildURL = serverIP + "/User/createUser?email=" + registerEmail.Text + "&password=" + encryptedpassword.ToLower() + "&inviteTicket=" + registerTicket.Text;
                    } else {
                        BuildURL = serverIP + "/User/createUser?email=" + registerEmail.Text + "&password=" + encryptedpassword.ToLower();
                    }

                    serverLoginResponse = wc.DownloadString(BuildURL);
                } catch (WebException ex) {
                    if (ex.Status == WebExceptionStatus.ProtocolError) {
                        HttpWebResponse serverReply = (HttpWebResponse)ex.Response;
                        if ((int)serverReply.StatusCode == 500) {
                            using (StreamReader sr = new StreamReader(serverReply.GetResponseStream())) {
                                serverLoginResponse = sr.ReadToEnd();
                            }
                        } else {
                            serverLoginResponse = ex.Message;
                        }
                    } else {
                        serverLoginResponse = ex.Message;
                    }
                }

                try {
                    XmlDocument SBRW_XML = new XmlDocument();
                    SBRW_XML.LoadXml(serverLoginResponse);

                    XmlNode DescriptionNode;
                    XmlNode LoginTokenNode;
                    XmlNode UserIdNode;

                    DescriptionNode = SBRW_XML.SelectSingleNode("LoginStatusVO/Description");
                    LoginTokenNode = SBRW_XML.SelectSingleNode("LoginStatusVO/LoginToken");
                    UserIdNode = SBRW_XML.SelectSingleNode("LoginStatusVO/UserId");

                    if(String.IsNullOrEmpty(DescriptionNode.InnerText)) {
                        MessageBox.Show(null, String.Format(Language.getLangString("MAIN_REGISTERSUCCESS", UILanguage), serverName), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        UserId = UserIdNode.InnerText;
                        LoginToken = LoginTokenNode.InnerText;

                        this.BackgroundImage = Properties.Resources.loginbg;

                        RegisterFormElements(false);
                        LoginFormElements(true);

                        this.currentWindowInfo.Location = new Point(479, 140);
                        this.currentWindowInfo.Size = new Size(222, 46);

                        loggedIn = true;
                    } else {
                         ConsoleLog(DescriptionNode.InnerText, "error");
                    }
                } catch {
                    MessageBox.Show(Language.getLangString("ERROR_SERVEROFFLINE", UILanguage));
                }
            }
        }

        /*
         * SETTINGS PAGE LAYOUT
         */

        private void settingsButton_Click(object sender, EventArgs e) {
            if(WindowState == FormWindowState.Minimized) { 
                WindowState = FormWindowState.Normal; 
            }

            this.settingsButton.BackgroundImage = Properties.Resources.settingsbtn_click;
            this.BackgroundImage = Properties.Resources.secondarybackground;
            SettingsFormElements(true);
            RegisterFormElements(false);
            LoggedInFormElements(false);
            LoginFormElements(false);
        }

        private void settingsButton_MouseEnter(object sender, EventArgs e) {
            this.settingsButton.BackgroundImage = Properties.Resources.settingsbtn_hover;
        }

        private void settingsButton_MouseLeave(object sender, EventArgs e) {
            this.settingsButton.BackgroundImage = Properties.Resources.settingsbtn;
        }

        private void settingsSave_MouseEnter(object sender, EventArgs e) {
            this.settingsSave.Image = Properties.Resources.smallbutton_hover;
        }

        private void settingsSave_MouseLeave(object sender, EventArgs e) {
            this.settingsSave.Image = Properties.Resources.smallbutton_enabled;
        }

        private void settingsSave_MouseUp(object sender, EventArgs e) {
            this.settingsSave.Image = Properties.Resources.smallbutton_hover;
        }

        private void settingsSave_MouseDown(object sender, EventArgs e) {
            this.settingsSave.Image = Properties.Resources.smallbutton_click;
        }

        private void settingsSave_Click(object sender, EventArgs e) {
            SettingFile.Write("Language", settingsLanguage.SelectedValue.ToString());
            SettingFile.Write("UILanguage", settingsUILang.SelectedValue.ToString());
            SettingFile.Write("TracksHigh", settingsQuality.SelectedValue.ToString());

            XmlDocument UserSettingsXML = new XmlDocument();
            if(File.Exists(UserSettings)) {
                try {
                    //File has been found, lets change Language setting
                    UserSettingsXML.Load(UserSettings);
                    XmlNode Language = UserSettingsXML.SelectSingleNode("Settings/UI/Language");
                    Language.InnerText = settingsLanguage.SelectedValue.ToString();
                } catch {
                    //XML is Corrupted... let's delete it and create new one
                    File.Delete(UserSettings);

                    XmlNode Setting = UserSettingsXML.AppendChild(UserSettingsXML.CreateElement("Settings"));
                    XmlNode PersistentValue = Setting.AppendChild(UserSettingsXML.CreateElement("PersistentValue"));
                    XmlNode Chat = PersistentValue.AppendChild(UserSettingsXML.CreateElement("Chat"));
                    XmlNode UI = Setting.AppendChild(UserSettingsXML.CreateElement("UI"));

                    Chat.InnerXml   = "<DefaultChatGroup Type=\"string\">" + settingsLanguage.SelectedValue.ToString() +"</DefaultChatGroup>";
                    UI.InnerXml     = "<Language Type=\"string\">" + settingsLanguage.SelectedValue.ToString() + "</Language>";

			        DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(UserSettings));
                }
            } else {
                //There's no file like that, let's create it
                XmlNode Setting = UserSettingsXML.AppendChild(UserSettingsXML.CreateElement("Settings"));
                XmlNode PersistentValue = Setting.AppendChild(UserSettingsXML.CreateElement("PersistentValue"));
                XmlNode Chat = PersistentValue.AppendChild(UserSettingsXML.CreateElement("Chat"));
                XmlNode UI = Setting.AppendChild(UserSettingsXML.CreateElement("UI"));

                Chat.InnerXml   = "<DefaultChatGroup Type=\"string\">" + settingsLanguage.SelectedValue.ToString() +"</DefaultChatGroup>";
                UI.InnerXml     = "<Language Type=\"string\">" + settingsLanguage.SelectedValue.ToString() + "</Language>";

			    DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(UserSettings));
            }

            UserSettingsXML.Save(UserSettings);

            SettingsFormElements(false);

            this.Update();
            this.Refresh();

            if(loggedIn) {
                this.BackgroundImage = Properties.Resources.loggedbg;
                LoginFormElements(false);
                LoggedInFormElements(true);
            } else {
                this.BackgroundImage = Properties.Resources.loginbg;
                LoggedInFormElements(false);
                LoginFormElements(true);
            }
        }

        private void SettingsFormElements(bool hideElements = true) {
            if (hideElements == true) {
                this.currentWindowInfo.Text = Language.getLangString("MAIN_INFORMATIONSETTINGS", UILanguage).ToUpper();
                this.currentWindowInfo.Location = new Point(53, 150);
                this.currentWindowInfo.Size = new Size(700, 46);
            }

            this.settingsSave.Visible = hideElements;
            this.settingsLanguage.Visible = hideElements;
            this.settingsLanguageText.Visible = hideElements;
            this.settingsLanguageDesc.Visible = hideElements;
            this.settingsQuality.Visible = hideElements;
            this.settingsQualityText.Visible = hideElements;
            this.settingsQualityDesc.Visible = hideElements;
            this.settingsUILang.Visible = hideElements;
            this.settingsUILangText.Visible = hideElements;
            this.settingsUILangDesc.Visible = hideElements;
        }

        private void LaunchGame(string UserId, string LoginToken, string ServerIP) {
            string filename = SettingFile.Read("InstallationDirectory") + "\\nfsw.exe";
            String cParams = "US " + ServerIP + " " + LoginToken + " " + UserId;
            var proc = Process.Start(filename, cParams);
            proc.EnableRaisingEvents = true;

            proc.Exited += (sender2, e2) => {
                this.WindowState = FormWindowState.Normal;

                closebtn_Click(sender2, e2);
            };
        }

        private void playButton_Click(object sender, EventArgs e) {
            loginButton_Click(sender, e);

            if (loggedIn == false) {
                //MessageBox.Show("Please log in first!");
            } else {

                if (playenabled == false) {
                    return;
                }

                //Relogin here
                string serverLoginResponse;
                string encryptedpassword;
                HashAlgorithm algorithm = SHA1.Create();
                StringBuilder sb = new StringBuilder();
                foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(password.Text.ToString()))) {
                    sb.Append(b.ToString("X2"));
                }

                if (useSavedPassword) {
                    encryptedpassword = SettingFile.Read("Password");
                } else {
                    encryptedpassword = sb.ToString();
                }

                try {
                    WebClient wc = new WebClientWithTimeout();

                    string BuildURL = serverIP + "/User/authenticateUser?email=" + email.Text.ToString() + "&password=" + encryptedpassword.ToLower();

                    serverLoginResponse = wc.DownloadString(BuildURL);
                } catch (WebException ex) {
                    if (ex.Status == WebExceptionStatus.ProtocolError) {
                        HttpWebResponse serverReply = (HttpWebResponse)ex.Response;
                        if ((int)serverReply.StatusCode == 500) {
                            using (StreamReader sr = new StreamReader(serverReply.GetResponseStream())) {
                                serverLoginResponse = sr.ReadToEnd();
                            }
                        } else {
                            serverLoginResponse = ex.Message;
                        }
                    } else {
                        serverLoginResponse = ex.Message;
                    }
                }

                try {
                    XmlDocument SBRW_XML = new XmlDocument();

                    if (builtinserver == false) {
                        SBRW_XML.LoadXml(serverLoginResponse);
                    } else {
                        SBRW_XML.LoadXml("<LoginStatusVO><UserId>1</UserId><LoginToken>aaaaaaaa-aaaa-aaaa-aaaaaaaa</LoginToken><Description/></LoginStatusVO>");
                    }

                    XmlNode DescriptionNode;
                    XmlNode LoginTokenNode;
                    XmlNode UserIdNode;

                    try {
                        DescriptionNode = SBRW_XML.SelectSingleNode("LoginStatusVO/Description");
                        LoginTokenNode = SBRW_XML.SelectSingleNode("LoginStatusVO/LoginToken");
                        UserIdNode = SBRW_XML.SelectSingleNode("LoginStatusVO/UserId");

                        if (String.IsNullOrEmpty(DescriptionNode.InnerText)) {
                            UserId = UserIdNode.InnerText;
                            LoginToken = LoginTokenNode.InnerText;
                        }
                    } catch {
                        MessageBox.Show(Language.getLangString("ERROR_SERVERDOWN", UILanguage));
                    }
                } catch {
                    MessageBox.Show(Language.getLangString("ERROR_SERVERDOWN", UILanguage));
                }

                this.playButton.BackgroundImage = Properties.Resources.largebutton_enabled;

                try {
                    LaunchGame(UserId, LoginToken, serverIP);
                } catch {
                    MessageBox.Show(Language.getLangString("ERROR_404NFSW", UILanguage));
                }

                if (builtinserver == true) {
                    this.playProgressText.Text = Language.getLangString("MAIN_BUILTINSERVERINIT", UILanguage).ToUpper();
                } else {
                    int secondsToCloseLauncher = 5;

                    while (secondsToCloseLauncher > 0) {
                        this.playProgressText.Text = String.Format(Language.getLangString("MAIN_LOADINGGAME", UILanguage), secondsToCloseLauncher).ToUpper(); //"LOADING GAME. LAUNCHER WILL MINIMIZE ITSELF IN " + secondsToCloseLauncher + " SECONDS";
                        Delay.WaitSeconds(1);
                        secondsToCloseLauncher--;
                    }

                    this.WindowState = FormWindowState.Minimized;
                    this.ShowInTaskbar = false;
                    this.Opacity = 0;

                    ContextMenu = new ContextMenu();
                    ContextMenu.MenuItems.Add(new MenuItem(Language.getLangString("CONTEXT_CHECKUPDATE", UILanguage), Updater.checkForUpdate));
                    ContextMenu.MenuItems.Add(new MenuItem(Language.getLangString("CONTEXT_ABOUT", UILanguage), About.showAbout));
                    ContextMenu.MenuItems.Add("-");
                    ContextMenu.MenuItems.Add(new MenuItem(Language.getLangString("CONTEXT_CLOSE", UILanguage), minimizebtn_Click));

                    this.Text = "NEED FOR SPEED™ WORLD";
                    this.Update();
                    this.Refresh();

                    Notification.ContextMenu = ContextMenu;
                }
            }
        }

        private void playButton_MouseUp(object sender, EventArgs e) {
            if (playenabled == false) {
                return;
            }

            this.playButton.BackgroundImage = Properties.Resources.largebutton_hover;
        }

        private void playButton_MouseDown(object sender, EventArgs e) {
            if (playenabled == false) {
                return;
            }

            this.playButton.BackgroundImage = Properties.Resources.largebutton_click;
        }

        private void playButton_MouseEnter(object sender, EventArgs e) {
            if (playenabled == false) {
                return;
            }

            this.playButton.BackgroundImage = Properties.Resources.largebutton_hover;
        }

        private void playButton_MouseLeave(object sender, EventArgs e) {
            if (playenabled == false) {
                return;
            }

            this.playButton.BackgroundImage = Properties.Resources.largebutton_enabled;
        }

        private void launchNFSW() {
            this.playButton.BackgroundImage = Properties.Resources.largebutton_disabled;
            this.playButton.ForeColor = Color.Gray;

            this.playProgressText.Text = Language.getLangString("MAIN_DOWNLOADER_CHECKINGFILES", UILanguage).ToUpper();
            this.playProgressTime.Text = "";
            Delay.WaitSeconds(2);

            string speechFile;

            try {
                if(String.IsNullOrEmpty(SettingFile.Read("Language"))) {
                    speechFile = "en";
                } else {
                    WebClient wc = new WebClientWithTimeout();
                    string response = wc.DownloadString("http://static.cdn.ea.com/blackbox/u/f/NFSWO/1614b/client/" + SettingFile.Read("Language").ToLower() + "/index.xml");
                    speechFile = SettingFile.Read("Language").ToLower();
                }
            } catch (Exception) {
                speechFile = "en";
            }

            

            if (!File.Exists(SettingFile.Read("InstallationDirectory") + "\\Sound\\Speech\\copspeechhdr_" + speechFile + ".big")) {
                this.playProgressText.Text = Language.getLangString("MAIN_DOWNLOADER_LOADINGFILELIST", UILanguage).ToUpper();
                DownloadCoreFiles();
            } else {
                OnDownloadFinished();
            }
        }

        public void DownloadCoreFiles() {
            this.playProgressText.Text = Language.getLangString("MAIN_DOWNLOADER_CHECKINGCORE", UILanguage).ToUpper();
            this.playProgressTime.Text = "";

            if (!File.Exists(SettingFile.Read("InstallationDirectory") + "\\nfsw.exe")) {
                DownloadStartTime = DateTime.Now;

                Downloader downloader = new Downloader(this, 3, 2, 16) {
                    ProgressUpdated = new ProgressUpdated(this.OnDownloadProgress),
                    DownloadFinished = new DownloadFinished(this.DownloadTracksFiles),
                    DownloadFailed = new DownloadFailed(this.OnDownloadFailed),
                    ShowMessage = new ShowMessage(this.OnShowMessage)
                };

                downloader.StartDownload("http://static.cdn.ea.com/blackbox/u/f/NFSWO/1614b/client", "", SettingFile.Read("InstallationDirectory"), false, false, 1130632198);
            } else {
                DownloadTracksFiles();
            }
        }

        public void DownloadTracksFiles() {
            this.playProgressText.Text = Language.getLangString("MAIN_DOWNLOADER_CHECKINGTRACKS", UILanguage).ToUpper();
            this.playProgressTime.Text = "";

            if (!File.Exists(SettingFile.Read("InstallationDirectory") + "\\Tracks\\STREAML5RA_98.BUN")) {
                DownloadStartTime = DateTime.Now;

                Downloader downloader = new Downloader(this, 3, 2, 16) {
                    ProgressUpdated = new ProgressUpdated(this.OnDownloadProgress),
                    DownloadFinished = new DownloadFinished(this.DownloadSpeechFiles),
                    DownloadFailed = new DownloadFailed(this.OnDownloadFailed),
                    ShowMessage = new ShowMessage(this.OnShowMessage)
                };

                downloader.StartDownload("http://static.cdn.ea.com/blackbox/u/f/NFSWO/1614b/client", "Tracks", SettingFile.Read("InstallationDirectory"), false, false, 615494528);
            } else {
                DownloadSpeechFiles();
            }
        }

        public void DownloadSpeechFiles() {
            this.playProgressText.Text = Language.getLangString("MAIN_DOWNLOADER_LOOKINGSPEECH", UILanguage).ToUpper();
            this.playProgressTime.Text = "";

            string speechFile;
            ulong speechSize;

            try {
                if (String.IsNullOrEmpty(SettingFile.Read("Language"))) {
                    speechFile = "en";
                    speechSize = 141805935;
                    langInfo = "ENGLISH";
                } else {
                    WebClient wc = new WebClientWithTimeout();
                    string response = wc.DownloadString("http://static.cdn.ea.com/blackbox/u/f/NFSWO/1614b/client/" + SettingFile.Read("Language").ToLower() + "/index.xml");

                    response = response.Substring(3, response.Length - 3);

                    XmlDocument SpeechFileXML = new XmlDocument();
                    SpeechFileXML.LoadXml(response);
                    XmlNode speechSizeNode = SpeechFileXML.SelectSingleNode("index/header/compressed");

                    speechFile = SettingFile.Read("Language").ToLower();
                    speechSize = Convert.ToUInt64(speechSizeNode.InnerText);
                    langInfo = settingsLanguage.GetItemText(settingsLanguage.SelectedItem).ToUpper();
                }
            } catch(Exception) {
                speechFile = "en";
                speechSize = 141805935;
                langInfo = "ENGLISH";
            }
            
            this.playProgressText.Text = String.Format(Language.getLangString("MAIN_DOWNLOADER_CHECKINGSPEECH", UILanguage), langInfo).ToUpper();

            if (!File.Exists(SettingFile.Read("InstallationDirectory") + "\\Sound\\Speech\\copspeechsth_" + speechFile + ".big")) {
                DownloadStartTime = DateTime.Now;

                Downloader downloader = new Downloader(this, 3, 2, 16) {
                    ProgressUpdated = new ProgressUpdated(this.OnDownloadProgress),
                    DownloadFinished = new DownloadFinished(this.DownloadTracksHighFiles),
                    DownloadFailed = new DownloadFailed(this.OnDownloadFailed),
                    ShowMessage = new ShowMessage(this.OnShowMessage)
                };

                downloader.StartDownload("http://static.cdn.ea.com/blackbox/u/f/NFSWO/1614b/client", speechFile, SettingFile.Read("InstallationDirectory"), false, false, speechSize);
            } else {
                DownloadTracksHighFiles();
            }
        }

        public void DownloadTracksHighFiles() {
            this.playProgressText.Text = Language.getLangString("MAIN_DOWNLOADER_CHECKINGTRACKSHIGH", UILanguage).ToUpper();
            this.playProgressTime.Text = "";

            if (SettingFile.Read("TracksHigh") == "1" && !File.Exists(SettingFile.Read("InstallationDirectory") + "\\TracksHigh\\STREAML5RA_98.BUN")) {
                DownloadStartTime = DateTime.Now;

                Downloader downloader = new Downloader(this, 3, 2, 16) {
                    ProgressUpdated = new ProgressUpdated(this.OnDownloadProgress),
                    DownloadFinished = new DownloadFinished(this.OnDownloadFinished),
                    DownloadFailed = new DownloadFailed(this.OnDownloadFailed),
                    ShowMessage = new ShowMessage(this.OnShowMessage)
                };

                downloader.StartDownload("http://static.cdn.ea.com/blackbox/u/f/NFSWO/1614b/client", "TracksHigh", SettingFile.Read("InstallationDirectory"), false, false, 278397707);
            } else {
                OnDownloadFinished();
            }
        }

        //EA Downloader compatibility (sorry EA)
        private string FormatFileSize(long byteCount) {
            double[] numArray = new double[] { 1073741824, 1048576, 1024, 0 };
            string[] strArrays = new string[] { "GB", "MB", "KB", "Bytes" };
            for (int i = 0; i < (int)numArray.Length; i++) {
                if ((double)byteCount >= numArray[i]) {
                    return string.Concat(string.Format("{0:0.00}", (double)byteCount / numArray[i]), strArrays[i]);
                }
            }

            return "0 Bytes";
        }

        private string EstimateFinishTime(long current, long total) {
            double num = (double)current / (double)total;
            if (num < 0.0500000007450581) {
                return "";
            }
            TimeSpan now = DateTime.Now - this.DownloadStartTime;
            TimeSpan timeSpan = TimeSpan.FromTicks((long)((double)now.Ticks / num)) - now;
            object hours = timeSpan.Hours;
            string str = timeSpan.Minutes.ToString("D02");
            int seconds = timeSpan.Seconds;
            return string.Format("{0}:{1}:{2}", hours, str, seconds.ToString("D02"));
        }

        private void OnDownloadProgress(long downloadLength, long downloadCurrent, long compressedLength, string filename) {
            if (downloadCurrent < compressedLength) {
                int width = this.playProgressText.Width;
                string file = filename.Replace(SettingFile.Read("InstallationDirectory") + "/", "").ToUpper();
                this.playProgressText.Text = string.Format(Language.getLangString("MAIN_DOWNLOADING", UILanguage).ToUpper(), this.FormatFileSize(downloadCurrent), this.FormatFileSize(compressedLength), file);
                this.playProgressTime.Text = this.EstimateFinishTime(downloadCurrent, compressedLength);
            }

            this.playProgress.Value = (int)((long)100 * downloadCurrent / compressedLength);
        }
        private void OnDownloadFinished() {
            playenabled = true;
            this.playProgress.Value = 100;
            this.playButton.BackgroundImage = Properties.Resources.largebutton_enabled;
            this.playButton.ForeColor = Color.White;
            this.playProgressText.Text = Language.getLangString("MAIN_DOWNLOADCOMPLETED", UILanguage).ToUpper();
            this.playProgressTime.Text = "";
        }

        private void OnDownloadFailed(Exception ex) {
            this.playProgress.Value = 100;
            this.playProgressText.Text = String.Format(Language.getLangString("MAIN_DOWNLOADFAILED", UILanguage), ex.Message).ToUpper();
            this.playProgressTime.Text = "";
            this.playProgress.ProgressColor = Color.Red;
        }

        private void OnShowMessage(string message, string header) {
            MessageBox.Show(message, header);
        }

        public void setTranslations(string LangID) {
            emailLabel.Text                         = Language.getLangString("MAIN_EMAIL", LangID).ToUpper();
            passwordLabel.Text                      = Language.getLangString("MAIN_PASSWORD", LangID).ToUpper();
            rememberMe.Text                         = Language.getLangString("MAIN_REMEMBERME", LangID).ToUpper();
            forgotPassword.Text                     = Language.getLangString("MAIN_FORGOTPASS", LangID).ToUpper();
            playButton.Text                         = Language.getLangString("MAIN_PLAY", LangID).ToUpper();

            registerEmailText.Text                  = Language.getLangString("MAIN_REGISTEREMAIL", LangID).ToUpper();
            registerPasswordText.Text               = Language.getLangString("MAIN_REGISTERPASS", LangID).ToUpper();
            registerConfirmPasswordText.Text        = Language.getLangString("MAIN_REGISTERCONFIRMPASS", LangID).ToUpper();
            registerTicketText.Text                 = Language.getLangString("MAIN_REGISTERTICKET", LangID).ToUpper();
            registerAgree.Text                      = Language.getLangString("MAIN_REGISTERTOS", LangID).ToUpper();
            registerButton.Text                     = Language.getLangString("MAIN_REGISTER", LangID).ToUpper();
            registerCancel.Text                     = Language.getLangString("MAIN_CANCEL", LangID).ToUpper();

            settingsLanguageText.Text               = Language.getLangString("MAIN_SETTINGSLANG", LangID).ToUpper();
            settingsQualityText.Text                = Language.getLangString("MAIN_SETTINGSQUALITY", LangID).ToUpper();
            settingsSave.Text                       = Language.getLangString("MAIN_SAVE", LangID).ToUpper();
            settingsLanguageDesc.Text               = Language.getLangString("MAIN_SETTINGSDESCLANG", LangID);
            settingsQualityDesc.Text                = Language.getLangString("MAIN_SETTINGSDESCQUALITY", LangID);
            settingsUILangText.Text                 = Language.getLangString("MAIN_SETTINGSUILANG", LangID).ToUpper();
            settingsUILangDesc.Text                 = Language.getLangString("MAIN_SETTINGSUIDESC", LangID);

            logoutButton.Text                       = Language.getLangString("MAIN_LOGOUT", LangID).ToUpper();
        }
    }
}