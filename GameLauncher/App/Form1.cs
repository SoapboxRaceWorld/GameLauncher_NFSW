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
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using SoapBox.JsonScheme;
using GameLauncher.App.Classes.Events;
using GameLauncherReborn;

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

        DateTime DownloadStartTime;

        String LoginToken = "";
        String UserId = "";
        String serverIP = "";
        String serverCacheKey = "18051995"; // Try to guess what this means for me :)
        String langInfo;
        float DPIDefaultScale = 96f;

        IniFile SettingFile = new IniFile("Settings.ini");
        string UserSettings = Environment.ExpandEnvironmentVariables("%AppData%\\Need for Speed World\\Settings\\UserSettings.xml");

        private void moveWindow_MouseDown(object sender, MouseEventArgs e) {
            mouseDownPoint = new Point(e.X, e.Y);
        }

        private void moveWindow_MouseUp(object sender, MouseEventArgs e) {
            mouseDownPoint = Point.Empty;
            this.Refresh();
            this.Opacity = 1;
        }

        private void moveWindow_MouseMove(object sender, MouseEventArgs e) {
            if (mouseDownPoint.IsEmpty) { return; }
            Form f = this as Form;
            f.Location = new Point(f.Location.X + (e.X - mouseDownPoint.X), f.Location.Y + (e.Y - mouseDownPoint.Y));
            windowMoved = true;
            this.Opacity = 0.7;
        }

        public void ConsoleLog(string e, string type) {
            consoleLog.SelectionStart = consoleLog.TextLength;
            consoleLog.SelectionLength = 0;
            consoleLog.SelectionFont = new Font(consoleLog.Font, FontStyle.Bold);

            consoleLog.SelectionColor = Color.Gray;
            consoleLog.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] ");

            if (type == "warning") {
                consoleLog.SelectionColor = Color.Yellow;
                consoleLog.AppendText("[WARN] ");
            } else if (type == "info") {
                consoleLog.SelectionColor = Color.Cyan;
                consoleLog.AppendText("[INFO] ");
            } else if (type == "error") {
                consoleLog.SelectionColor = Color.Red;
                consoleLog.AppendText("[ERROR] ");
            } else if (type == "success") {
                consoleLog.SelectionColor = Color.Lime;
                consoleLog.AppendText("[SUCCESS] ");
            } else if (type == "ping") {
                consoleLog.SelectionColor = Color.DarkOrange;
                consoleLog.AppendText("[PING] ");
            }

            consoleLog.SelectionColor = consoleLog.ForeColor;
            consoleLog.SelectionFont = new Font(consoleLog.Font, FontStyle.Regular);
            consoleLog.AppendText(e);
            consoleLog.AppendText("\r\n");

            if(DetectLinux.WineDetected() == false) {
                consoleLog.ScrollToCaret();
            }
        }

        public mainScreen() {
            if (Environment.OSVersion.Version.Major <= 5) {
                if(DetectLinux.WineDetected() == false) { 
                    MessageBox.Show(null, "Sadly, the red background cannot be fixed on Windows XP and lower, sorry...", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } else {
                Font = new Font(Font.Name, 8.25f * DPIDefaultScale / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            }

            InitializeComponent();

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
            githubLink.LinkClicked += new LinkLabelLinkClickedEventHandler(githubLink_LinkClicked);

            moveWindow.MouseDown += new MouseEventHandler(moveWindow_MouseDown);
            moveWindow.MouseMove += new MouseEventHandler(moveWindow_MouseMove);
            moveWindow.MouseUp += new MouseEventHandler(moveWindow_MouseUp);

            playButton.MouseEnter += new EventHandler(playButton_MouseEnter);
            playButton.MouseLeave += new EventHandler(playButton_MouseLeave);
            playButton.Click += new EventHandler(playButton_Click);
            playButton.MouseUp += new MouseEventHandler(playButton_MouseUp);
            playButton.MouseDown += new MouseEventHandler(playButton_MouseDown);

            //Simple check if we have enough permission to write file and remove them
            try {
                string file = Directory.GetCurrentDirectory() + "\\test.txt";
                File.WriteAllText(file, "test");
                File.Delete(file);
            } catch {
                Self.runAsAdmin();
                Environment.Exit(Environment.ExitCode);
            }

            //Somewhere here we will setup the game installation directory
            if (String.IsNullOrEmpty(SettingFile.Read("InstallationDirectory"))) {
                if (Environment.OSVersion.Version.Major <= 5) {
                    MessageBox.Show(null, "Click OK to select folder with NFSW.exe", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    var fbd = new FolderBrowserDialog();
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK) {
                        SettingFile.Write("InstallationDirectory", fbd.SelectedPath);
                    } else {
                        Environment.Exit(Environment.ExitCode);
                    }
                } else {
                    CommonOpenFileDialog openFolder = new CommonOpenFileDialog();
                    openFolder.InitialDirectory = "";
                    openFolder.IsFolderPicker = true;
                    openFolder.Title = "GameLauncher: Please pick up a directory where NFSW is located or has to be installed.";
                    CommonFileDialogResult result = openFolder.ShowDialog();

                    if (result == CommonFileDialogResult.Ok) {
                        SettingFile.Write("InstallationDirectory", openFolder.FileName);
                    } else {
                        Environment.Exit(Environment.ExitCode);
                    }
                }
            }

            //Replace cursor
            if (File.Exists(SettingFile.Read("InstallationDirectory") + "\\Media\\Cursors\\default.cur")) {
                Cursor mycursor = new Cursor(Cursor.Current.Handle);
                IntPtr colorcursorhandle = User32.LoadCursorFromFile(SettingFile.Read("InstallationDirectory") + "\\Media\\Cursors\\default.cur");
                mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, null, mycursor, new object[] { colorcursorhandle });
                this.Cursor = mycursor;
            }

            //Skinning ability
            if(!Directory.Exists("Themes")) {
                Directory.CreateDirectory("Themes");
                File.WriteAllText("Themes/ReadMe.txt", "Put your GLS Files in this directory.");
            }

            registerText.Text = "DON'T HAVE AN ACCOUNT?\nCLICK HERE TO CREATE ONE NOW...";
        }

        private void mainScreen_Load(object sender, EventArgs e) {
            Updater.checkForUpdate(sender, e);

            //Console output to textbox

            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(new MenuItem("&Check for updates", Updater.checkForUpdate));
            ContextMenu.MenuItems.Add(new MenuItem("&About", About.showAbout));
            ContextMenu.MenuItems.Add(new MenuItem("&Settings", settingsButton_Click));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem("&Close", closebtn_Click));

            Notification.ContextMenu = ContextMenu;
            Notification.Icon = new Icon(Icon, Icon.Width, Icon.Height);
            Notification.Text = "GameLauncher";
            Notification.Visible = true;

            ConsoleLog("Log initialized", "info");
            ConsoleLog("GameLauncher initialized", "info");
            ConsoleLog("Installation directory: " + Path.GetFullPath(SettingFile.Read("InstallationDirectory")), "info");

            email.Text = SettingFile.Read("AccountEmail");
            if (!String.IsNullOrEmpty(SettingFile.Read("AccountEmail")) && !String.IsNullOrEmpty(SettingFile.Read("Password"))) {
                rememberMe.Checked = true;
            }

            //Fetch serverlist, and disable if failed to fetch.
            var response = "";
            try {
                WebClient wc = new WebClientWithTimeout();

                string serverurl = "http://nfsw.metonator.ct8.pl/serverlist.txt";
                response = wc.DownloadString(serverurl);
                ConsoleLog("Fetching " + serverurl, "info");

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
                    ConsoleLog("Successfully created ServerCache", "success");
                } catch (Exception ex) {
                    ConsoleLog("Failed to create cached serverlist. " + ex.Message, "error");
                }
            } catch (Exception ex) {
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
                    ConsoleLog("Fetched Serverlist from Cache", "warning");
                } else {
                    ConsoleLog("Failed to fetch serverlist. " + ex.Message, "error");
                }
            }

            //Time to add servers
            serverPick.DisplayMember = "Text";
            serverPick.ValueMember = "Value";

            List<Object> items = new List<Object>();
            response += "Offline Built-In Server;http://localhost:7331/nfsw/Engine.svc";

            String[] substrings = response.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var substring in substrings) {
                if (!String.IsNullOrEmpty(substring)) {
                    String[] substrings2 = substring.Split(new string[] { ";" }, StringSplitOptions.None);
                    items.Add(new { Text = substrings2[0], Value = substrings2[1] });
                }
            }

            serverPick.DataSource = items;
            serverPick.SelectedIndex = 0;

            //Silliest way to prevent doublecall of TextChanged event...
            if(serverlistloaded == true) {
                if(!SettingFile.KeyExists("Server")) {
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

            serverStatusImg.Location = new Point(-16, -16);

            if (SettingFile.KeyExists("Password")) {
                loginEnabled = true;
                serverEnabled = true;
                useSavedPassword = true;
                this.loginButton.Image = Properties.Resources.button_enable;
                this.loginButton.ForeColor = Color.White;
                ConsoleLog("Password recovered from Settings.ini file.", "success");
            } else {
                loginEnabled = false;
                serverEnabled = false;
                useSavedPassword = false;
                this.loginButton.Image = Properties.Resources.button_disable;
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

            //Detect UserSettings
            if(File.Exists(UserSettings)) {
                ConsoleLog("Found Game Config under " + UserSettings + " file.", "success");
            }

            //Soapbox Modules (without them Freeroam might fail)
            Directory.CreateDirectory(SettingFile.Read("InstallationDirectory"));
            if(!File.Exists(SettingFile.Read("InstallationDirectory") + "/lightfx.dll")) {
                File.WriteAllBytes(SettingFile.Read("InstallationDirectory") + "/lightfx.dll", ExtractResource.AsByte("GameLauncher.SoapBoxModules.lightfx.dll"));
                Directory.CreateDirectory(SettingFile.Read("InstallationDirectory") + "/modules");
                File.WriteAllText(SettingFile.Read("InstallationDirectory") + "/modules/udpcrc.soapbox.module", ExtractResource.AsString("GameLauncher.SoapBoxModules.udpcrc.soapbox.module"));
                File.WriteAllText(SettingFile.Read("InstallationDirectory") + "/modules/udpcrypt1.soapbox.module", ExtractResource.AsString("GameLauncher.SoapBoxModules.udpcrypt1.soapbox.module"));
                File.WriteAllText(SettingFile.Read("InstallationDirectory") + "/modules/udpcrypt2.soapbox.module",  ExtractResource.AsString("GameLauncher.SoapBoxModules.udpcrypt2.soapbox.module"));
                File.WriteAllText(SettingFile.Read("InstallationDirectory") + "/modules/xmppsubject.soapbox.module",  ExtractResource.AsString("GameLauncher.SoapBoxModules.xmppsubject.soapbox.module"));
            }

            //Trigger login button for offline
            if(builtinserver == true) {
                this.loginButton.ForeColor = Color.White;
                this.loginButton.Image = Properties.Resources.button_enable;
            }

            //Hide other windows
            RegisterFormElements(false);
            SettingsFormElements(false);
            DownloadFormElements(false);

            //Command-line Arguments
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2) {
                MessageBox.Show("Your launcher has been updated.");
            }
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

            try {
                if (!Directory.Exists("Logs")) {
                    Directory.CreateDirectory("Logs");
                }
                consoleLog.SaveFile("Logs/" + timestamp + ".log", RichTextBoxStreamType.PlainText);
            }
            catch { }

            //Fix InstallationDirectory
            SettingFile.Write("InstallationDirectory", Path.GetFullPath(SettingFile.Read("InstallationDirectory")));

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
                this.loginButton.Image = Properties.Resources.button_disable;
                this.loginButton.ForeColor = Color.Gray;
            }
            else {
                loginEnabled = true;
                this.loginButton.Image = Properties.Resources.button_enable;
                this.loginButton.ForeColor = Color.White;
            }

            useSavedPassword = false;
        }

        private void loginButton_MouseUp(object sender, EventArgs e) {
            if (loginEnabled == true || builtinserver == true) {
                this.loginButton.Image = Properties.Resources.button_hover;
            } else {
                this.loginButton.Image = Properties.Resources.button_disable;
            }
        }

        private void loginButton_MouseDown(object sender, EventArgs e) {
            if (loginEnabled == true || builtinserver == true) {
                this.loginButton.Image = Properties.Resources.button_click;
            } else {
                this.loginButton.Image = Properties.Resources.button_disable;
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

            ConsoleLog("Trying to login into " + serverPick.GetItemText(serverPick.SelectedItem) + " (" + serverIP + ")", "info");

            if(builtinserver == true) {
                MessageBox.Show(null, "Careful: This built-in server is in alpha! Use it at your own risk.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            XmlDocument SBRW_XML = new XmlDocument();
            try {
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

                    this.BackgroundImage = Properties.Resources.playbg;
                    this.currentWindowInfo.Visible = false;

                    if (builtinserver == false) {
                        playLoggedInAs.Text = "LOGGED IN AS " + email.Text.ToUpper();
                    } else {
                        playLoggedInAs.Text = "LOGGED IN AS LOCALHOST";
                    }

                    LoginFormElements(false);
                    DownloadFormElements(true);

                    launchNFSW();
                } else {
                     ConsoleLog("Invalid username or password.", "error");
                }
            } catch {
                ConsoleLog("Failed to get token from server, probably is offline.", "error");
            }
        }

        private void loginButton_MouseEnter(object sender, EventArgs e) {
            if (loginEnabled == true || builtinserver == true) {
                this.loginButton.Image = Properties.Resources.button_hover;
                this.loginButton.ForeColor = Color.White;
            } else {
                this.loginButton.Image = Properties.Resources.button_disable;
                this.loginButton.ForeColor = Color.Gray;
            }
        }

        private void loginButton_MouseLeave(object sender, EventArgs e) {
            if (loginEnabled == true || builtinserver == true) {
                this.loginButton.Image = Properties.Resources.button_enable;
                this.loginButton.ForeColor = Color.White;
            } else {
                this.loginButton.Image = Properties.Resources.button_disable;
                this.loginButton.ForeColor = Color.Gray;
            }
        }

        private void serverPick_TextChanged(object sender, EventArgs e) {
            if (!skipServerTrigger) { return; }

            loginEnabled = false;

            this.loginButton.ForeColor = Color.Gray;
            this.password.Text = "";
            string verticalImageUrl = "";
            verticalBanner.Image = null;
            verticalBanner.BackColor = Color.Transparent;

            string serverIP = serverPick.SelectedValue.ToString();
            string numPlayers;
            string serverName = serverPick.GetItemText(serverPick.SelectedItem);

            serverStatusImg.Location = new Point(-16, -16);
            serverStatus.ForeColor = Color.White;
            serverStatus.Text = "Retrieving server status...";
            serverStatus.Location = new Point(44, 329);
            onlineCount.Text = "";

            if (serverPick.GetItemText(serverPick.SelectedItem) == "Offline Built-In Server") {
                builtinserver = true;
                this.loginButton.Image = Properties.Resources.button_enable;
                this.loginButton.Text = "LAUNCH";
                this.loginButton.ForeColor = Color.White;
            } else {
                builtinserver = false;
                this.loginButton.Image = Properties.Resources.button_disable;
                this.loginButton.Text = "LOG IN";
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
                    serverStatusImg.Location = new Point(20, 335);
                    serverStatusImg.BackgroundImage = Properties.Resources.server_offline;
                    serverStatus.ForeColor = Color.FromArgb(227, 88, 50);
                    serverStatus.Text = "This server is currently down. Thanks for your patience.";
                    serverStatus.Location = new Point(44, 329);
                    onlineCount.Text = "";
                    serverEnabled = false;
                } else {
                    serverStatusImg.Location = new Point(20, 323);
                    serverStatusImg.BackgroundImage = Properties.Resources.server_online;
                    serverStatus.ForeColor = Color.FromArgb(181, 255, 33);
                    serverStatus.Text = "This server is currently up and running.";
                    serverStatus.Location = new Point(44, 322);

                    if (serverName == "Offline Built-In Server") {
                        numPlayers = "∞";
                    } else {
                        if (Environment.OSVersion.Version.Major <= 5) {
                            ticketRequired = true;
                            verticalImageUrl = null;
                            numPlayers = "Unknown";
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

                            numPlayers = json.onlineNumber + " out of " + json.numberOfRegistered;
                        }
                    }

                    onlineCount.Text = "Players on server: " + numPlayers;
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
                                    verticalBanner.BackColor = Color.Black;
                                } catch {
                                    verticalBanner.Image = null;
                                }
                            }
                        };
                    }

                    if(DetectLinux.WineDetected() == false) { 
                        Ping pingSender = new Ping();
                        pingSender.SendAsync(StringToUri.Host, 1000, new byte[1], new PingOptions(64, true), new AutoResetEvent(false));
                        pingSender.PingCompleted += (sender3, e3) => {
                            PingReply reply = e3.Reply;

                            if (reply.Status == IPStatus.Success && serverName != "Offline Built-In Server") {
                                onlineCount.Text += ". Server ping is " + reply.RoundtripTime + "ms";
                            } else {
                                onlineCount.Text += ". Server doesn't allow pinging.";
                            }
                        };
                    } else {
                        onlineCount.Text += ". Ping is disabled on non-Windows platform. ";
                    }
                }
            };
        }

        private void ApplyEmbeddedFonts() {
            FontFamily fontFamily = FontWrapper.Instance.GetFontFamily("Font_MyriadProSemiCondBold.ttf");
            FontFamily fontFamily2 = FontWrapper.Instance.GetFontFamily("Font_Register.ttf");
            FontFamily fontFamily3 = FontWrapper.Instance.GetFontFamily("Font_RegisterBoldItalic.ttf");
            FontFamily fontFamily4 = FontWrapper.Instance.GetFontFamily("Font_RegisterDemiBold.ttf");
            FontFamily fontFamily5 = FontWrapper.Instance.GetFontFamily("Font_RegisterBold.ttf");
            FontFamily fontFamily6 = FontWrapper.Instance.GetFontFamily("Font_MyriadProSemiCond.ttf");

            currentWindowInfo.Font = new Font(fontFamily3, 12.75f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Italic);
            rememberMe.Font = new Font(fontFamily, 9f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            loginButton.Font = new Font(fontFamily2, 15f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold | FontStyle.Italic);
            registerButton.Font = new Font(fontFamily2, 15f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold | FontStyle.Italic);
            settingsSave.Font = new Font(fontFamily2, 15f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold | FontStyle.Italic);
            serverStatus.Font = new Font(fontFamily, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            onlineCount.Font = new Font(fontFamily, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerText.Font = new Font(fontFamily, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            emailLabel.Font = new Font(fontFamily4, 11f * DPIDefaultScale / CreateGraphics().DpiX);
            passwordLabel.Font = new Font(fontFamily4, 11f * DPIDefaultScale / CreateGraphics().DpiX);
            troubleLabel.Font = new Font(fontFamily, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            githubLink.Font = new Font(fontFamily, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            forgotPassword.Font = new Font(fontFamily, 9f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            selectServerLabel.Font = new Font(fontFamily, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsLanguageText.Font = new Font(fontFamily, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsLanguageDesc.Font = new Font(fontFamily, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsQualityText.Font = new Font(fontFamily, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            settingsQualityDesc.Font = new Font(fontFamily, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            registerEmailText.Font = new Font(fontFamily4, 11f * DPIDefaultScale / CreateGraphics().DpiX);
            registerPasswordText.Font = new Font(fontFamily4, 11f * DPIDefaultScale / CreateGraphics().DpiX);
            registerConfirmPasswordText.Font = new Font(fontFamily4, 11f * DPIDefaultScale / CreateGraphics().DpiX);
            registerTicketText.Font = new Font(fontFamily4, 11f * DPIDefaultScale / CreateGraphics().DpiX);
            registerAgree.Font = new Font(fontFamily2, 9.749999f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold | FontStyle.Italic);
            playButton.Font = new Font(fontFamily2, 15f * DPIDefaultScale / CreateGraphics().DpiX, FontStyle.Bold | FontStyle.Italic);
        }

        private void registerText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            this.BackgroundImage = Properties.Resources.registerbg;
            this.currentWindowInfo.Text = "REGISTER ON " + serverPick.GetItemText(serverPick.SelectedItem).ToUpper() + ":";
            LoginFormElements(false);
            RegisterFormElements(true);
        }

        private void githubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ConsoleLog("Redirecting into GitHub Issue page", "info");
            Process.Start("https://github.com/metonator/GameLauncher_NFSW/issues");
        }

        private void forgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(serverPick.SelectedValue.ToString().Replace("Engine.svc", "") + "forgotPasswd.jsp");
        }

        private void LoginFormElements(bool hideElements = false) {
            this.rememberMe.Visible = hideElements;
            this.loginButton.Visible = hideElements;
            this.serverStatus.Visible = hideElements;
            this.onlineCount.Visible = hideElements;
            this.registerText.Visible = hideElements;
            this.serverPick.Visible = hideElements;
            this.serverStatusImg.Visible = hideElements;
            this.consoleLog.Visible = hideElements;
            this.email.Visible = hideElements;
            this.password.Visible = hideElements;
            this.emailLabel.Visible = hideElements;
            this.passwordLabel.Visible = hideElements;
            this.troubleLabel.Visible = hideElements;
            this.githubLink.Visible = hideElements;
            this.forgotPassword.Visible = hideElements;
            this.selectServerLabel.Visible = hideElements;
            this.settingsButton.Visible = hideElements;
            this.verticalBanner.Visible = hideElements;
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
            this.registerButton.Visible = hideElements;
            this.registerEmail.Visible = hideElements;
            this.registerEmailText.Visible = hideElements;
            this.registerPassword.Visible = hideElements;
            this.registerPasswordText.Visible = hideElements;
            this.registerConfirmPassword.Visible = hideElements;
            this.registerConfirmPasswordText.Visible = hideElements;
            this.registerAgree.Visible = hideElements;

            //Restore some loginform elements
            this.consoleLog.Visible = true;
            this.verticalBanner.Visible = true;

            if(ticketRequired) {
                this.registerTicket.Visible = hideElements;
                this.registerTicketText.Visible = hideElements;
            } else {
                this.registerTicket.Visible = false;
                this.registerTicketText.Visible = false;
            }
        }

        private void registerButton_MouseEnter(object sender, EventArgs e) {
            this.registerButton.Image = Properties.Resources.button_hover;
        }

        private void registerButton_MouseLeave(object sender, EventArgs e) {
            this.registerButton.Image = Properties.Resources.button_enable;
        }

        private void registerButton_MouseUp(object sender, EventArgs e) {
            this.registerButton.Image = Properties.Resources.button_hover;
        }

        private void registerButton_MouseDown(object sender, EventArgs e) {
            this.registerButton.Image = Properties.Resources.button_click;
        }

        private void registerButton_Click(object sender, EventArgs e) {
            bool registerSuccess = true;
            ConsoleLog("Registering... Please wait", "info");

            if(String.IsNullOrEmpty(registerEmail.Text)) {
                ConsoleLog("Please enter your email", "error");
                registerSuccess = false;
            } else if(validateEmail(registerEmail.Text) == false) {
                ConsoleLog("Please enter your correct email", "error");
                registerSuccess = false;
            }

            if(String.IsNullOrEmpty(registerTicket.Text) && ticketRequired == true) {
                ConsoleLog("Please enter your ticket", "error");
                registerSuccess = false;
            }

            if(String.IsNullOrEmpty(registerPassword.Text)) {
                ConsoleLog("Please enter your password", "error");
                registerSuccess = false;
            }

            if(String.IsNullOrEmpty(registerConfirmPassword.Text)) {
                ConsoleLog("Please confirm your password", "error");
                registerSuccess = false;
            }

            if(registerConfirmPassword.Text != registerPassword.Text) {
                ConsoleLog("Passwords doesn't match", "error");
            }

            if(!registerAgree.Checked) {
                ConsoleLog("You have not agreed to the Terms of Service", "error");
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

                XmlDocument SBRW_XML = new XmlDocument();
                SBRW_XML.LoadXml(serverLoginResponse);

                XmlNode DescriptionNode;
                XmlNode LoginTokenNode;
                XmlNode UserIdNode;

                DescriptionNode = SBRW_XML.SelectSingleNode("LoginStatusVO/Description");
                LoginTokenNode = SBRW_XML.SelectSingleNode("LoginStatusVO/LoginToken");
                UserIdNode = SBRW_XML.SelectSingleNode("LoginStatusVO/UserId");

                if(String.IsNullOrEmpty(DescriptionNode.InnerText)) {
                    UserId = UserIdNode.InnerText;
                    LoginToken = LoginTokenNode.InnerText;

                    this.BackgroundImage = Properties.Resources.playbg;
                    this.currentWindowInfo.Visible = false;

                    playLoggedInAs.Text = "REGISTERED IN AS " + registerEmail.Text.ToUpper();
                    RegisterFormElements(false);
                    DownloadFormElements(true);

                    this.consoleLog.Hide();

                    launchNFSW();
                } else {
                     ConsoleLog(DescriptionNode.InnerText, "error");
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
            this.BackgroundImage = Properties.Resources.settingsbg;
            this.currentWindowInfo.Text = "PLEASE SELECT YOUR GAME SETTINGS:";
            SettingsFormElements(true);
            LoginFormElements(false);
            DownloadFormElements(false);
        }

        private void settingsButton_MouseEnter(object sender, EventArgs e) {
            this.settingsButton.BackgroundImage = Properties.Resources.settingsbtn_hover;
        }

        private void settingsButton_MouseLeave(object sender, EventArgs e) {
            this.settingsButton.BackgroundImage = Properties.Resources.settingsbtn;
        }

        private void settingsSave_MouseEnter(object sender, EventArgs e) {
            this.settingsSave.Image = Properties.Resources.button_hover;
        }

        private void settingsSave_MouseLeave(object sender, EventArgs e) {
            this.settingsSave.Image = Properties.Resources.button_enable;
        }

        private void settingsSave_MouseUp(object sender, EventArgs e) {
            this.settingsSave.Image = Properties.Resources.button_hover;
        }

        private void settingsSave_MouseDown(object sender, EventArgs e) {
            this.settingsSave.Image = Properties.Resources.button_click;
        }

        private void settingsSave_Click(object sender, EventArgs e) {
            SettingFile.Write("Language", settingsLanguage.SelectedValue.ToString());
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

            this.BackgroundImage = Properties.Resources.loginbg;
            this.currentWindowInfo.Text = "ENTER YOUR ACCOUNT INFORMATION TO LOG IN:";
            SettingsFormElements(false);
            LoginFormElements(true);
        }

        private void SettingsFormElements(bool hideElements = true) {
            this.settingsSave.Visible = hideElements;
            this.settingsLanguage.Visible = hideElements;
            this.settingsLanguageText.Visible = hideElements;
            this.settingsLanguageDesc.Visible = hideElements;
            this.settingsQuality.Visible = hideElements;
            this.settingsQualityText.Visible = hideElements;
            this.settingsQualityDesc.Visible = hideElements;
        }

        /*
         * DOWNLOAD PAGE LAYOUT
         */

        private void DownloadFormElements(bool hideElements = true) {
            this.playLoggedInAs.Visible = hideElements;
            this.playProgress.Visible = hideElements;
            this.playProgressText.Visible = hideElements;
            this.playButton.Visible = hideElements;
            this.playProgressTime.Visible = hideElements;
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
            if(playenabled == false) {
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
                MessageBox.Show("Failed to update token, server is probably offline.");
            }

            this.playButton.Image = Properties.Resources.playButton_enable;

            try {
                LaunchGame(UserId, LoginToken, serverIP);
            } catch {
                MessageBox.Show("Failed to launch game. Cannot find NFSW.exe");
            }

            if (builtinserver == true) {
                this.playProgressText.Text = "SOAPBOX SERVER LAUNCHED. WAITING FOR QUERIES";
            } else {
                int secondsToCloseLauncher = 5;

                while(secondsToCloseLauncher > 0) {
                    this.playProgressText.Text = "LOADING GAME. LAUNCHER WILL MINIMIZE ITSELF IN " + secondsToCloseLauncher + " SECONDS";
                    Delay.WaitSeconds(1);
                    secondsToCloseLauncher--;
                }

                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Opacity = 0;

                ContextMenu = new ContextMenu();
                ContextMenu.MenuItems.Add(new MenuItem("&AntiCheat v0 NOTEVENALPHA"));
                ContextMenu.MenuItems.Add("-");
                ContextMenu.MenuItems.Add(new MenuItem("&Check for updates", Updater.checkForUpdate));
                ContextMenu.MenuItems.Add(new MenuItem("&About", About.showAbout));
                ContextMenu.MenuItems.Add("-");
                ContextMenu.MenuItems.Add(new MenuItem("&Close", minimizebtn_Click));

                this.Text = "NEED FOR SPEED™ WORLD";
                this.Update();
                this.Refresh();

                Notification.ContextMenu = ContextMenu;
            }
        }

        private void playButton_MouseUp(object sender, EventArgs e) {
            if (playenabled == false) {
                return;
            }

            this.playButton.Image = Properties.Resources.playButton_hover;
        }

        private void playButton_MouseDown(object sender, EventArgs e) {
            if (playenabled == false) {
                return;
            }

            this.playButton.Image = Properties.Resources.playButton_click;
        }

        private void playButton_MouseEnter(object sender, EventArgs e) {
            if (playenabled == false) {
                return;
            }

            this.playButton.Image = Properties.Resources.playButton_hover;
        }

        private void playButton_MouseLeave(object sender, EventArgs e) {
            if (playenabled == false) {
                return;
            }

            this.playButton.Image = Properties.Resources.playButton_enable;
        }

        private void launchNFSW() {
            this.playButton.Image = Properties.Resources.playButton_disable;
            this.playButton.ForeColor = Color.Gray;

            this.playProgressText.Text = "PLEASE WAIT...";
            this.playProgressTime.Text = "";
            Delay.WaitSeconds(1);

            string speechFile;

            try {
                if(String.IsNullOrEmpty(SettingFile.Read("Language"))) {
                    speechFile = "en";
                } else {
                    WebClient wc = new WebClientWithTimeout();
                    string response = wc.DownloadString("http://mirror.nfsw.mtntr.eu/NFSWO/" + SettingFile.Read("Language").ToLower() + "/index.xml");
                    speechFile = SettingFile.Read("Language").ToLower();
                }
            } catch (Exception) {
                speechFile = "en";
            }

            if (!File.Exists(SettingFile.Read("InstallationDirectory") + "\\Sound\\Speech\\copspeechhdr_" + speechFile + ".big")) {
                MessageBox.Show(null, "This downloader is in alpha. Please report every issue you will notice.\nThere's also a known issue about 'ESET Smart Security' cutting downloader from reaching chunks files.\nPlease, disable your antivirus.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.playProgressText.Text = "LOADING FILELIST FOR DOWNLOAD...";
                DownloadCoreFiles();
            } else {
                OnDownloadFinished();
            }
        }

        public void DownloadCoreFiles() {
            this.playProgressText.Text = "CHECKING CORE FILES...";
            this.playProgressTime.Text = "";

            if (!File.Exists(SettingFile.Read("InstallationDirectory") + "\\nfsw.exe")) {
                DownloadStartTime = DateTime.Now;

                Downloader downloader = new Downloader(this, 3, 2, 16) {
                    ProgressUpdated = new ProgressUpdated(this.OnDownloadProgress),
                    DownloadFinished = new DownloadFinished(this.DownloadTracksFiles),
                    DownloadFailed = new DownloadFailed(this.OnDownloadFailed),
                    ShowMessage = new ShowMessage(this.OnShowMessage)
                };

                downloader.StartDownload("http://mirror.nfsw.mtntr.eu/NFSWO", "", SettingFile.Read("InstallationDirectory"), false, false, 1130632198);
            } else {
                DownloadTracksFiles();
            }
        }

        public void DownloadTracksFiles() {
            this.playProgressText.Text = "CHECKING TRACKS FILES...";
            this.playProgressTime.Text = "";

            if (!File.Exists(SettingFile.Read("InstallationDirectory") + "\\Tracks\\STREAML5RA_98.BUN")) {
                DownloadStartTime = DateTime.Now;

                Downloader downloader = new Downloader(this, 3, 2, 16) {
                    ProgressUpdated = new ProgressUpdated(this.OnDownloadProgress),
                    DownloadFinished = new DownloadFinished(this.DownloadSpeechFiles),
                    DownloadFailed = new DownloadFailed(this.OnDownloadFailed),
                    ShowMessage = new ShowMessage(this.OnShowMessage)
                };

                downloader.StartDownload("http://mirror.nfsw.mtntr.eu/NFSWO", "Tracks", SettingFile.Read("InstallationDirectory"), false, false, 615494528);
            } else {
                DownloadSpeechFiles();
            }
        }

        public void DownloadSpeechFiles() {
            this.playProgressText.Text = "LOOKING FOR COMPATIBLE SPEECH FILES...";
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
                    string response = wc.DownloadString("http://mirror.nfsw.mtntr.eu/NFSWO/" + SettingFile.Read("Language").ToLower() + "/index.xml");

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
            
            this.playProgressText.Text = "CHECKING " + langInfo + " SPEECH FILES...";

            if (!File.Exists(SettingFile.Read("InstallationDirectory") + "\\Sound\\Speech\\copspeechsth_" + speechFile + ".big")) {
                DownloadStartTime = DateTime.Now;

                Downloader downloader = new Downloader(this, 3, 2, 16) {
                    ProgressUpdated = new ProgressUpdated(this.OnDownloadProgress),
                    DownloadFinished = new DownloadFinished(this.DownloadTracksHighFiles),
                    DownloadFailed = new DownloadFailed(this.OnDownloadFailed),
                    ShowMessage = new ShowMessage(this.OnShowMessage)
                };

                downloader.StartDownload("http://mirror.nfsw.mtntr.eu/NFSWO", speechFile, SettingFile.Read("InstallationDirectory"), false, false, speechSize);
            } else {
                DownloadTracksHighFiles();
            }
        }

        public void DownloadTracksHighFiles() {
            this.playProgressText.Text = "CHECKING TRACKSHIGH FILES...";
            this.playProgressTime.Text = "";

            if (SettingFile.Read("TracksHigh") == "1" && !File.Exists(SettingFile.Read("InstallationDirectory") + "\\TracksHigh\\STREAML5RA_98.BUN")) {
                DownloadStartTime = DateTime.Now;

                Downloader downloader = new Downloader(this, 3, 2, 16) {
                    ProgressUpdated = new ProgressUpdated(this.OnDownloadProgress),
                    DownloadFinished = new DownloadFinished(this.OnDownloadFinished),
                    DownloadFailed = new DownloadFailed(this.OnDownloadFailed),
                    ShowMessage = new ShowMessage(this.OnShowMessage)
                };

                downloader.StartDownload("http://mirror.nfsw.mtntr.eu/NFSWO", "TracksHigh", SettingFile.Read("InstallationDirectory"), false, false, 278397707);
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
                this.playProgressText.Text = string.Format("DOWNLOADING {2} ({0}/{1})", this.FormatFileSize(downloadCurrent), this.FormatFileSize(compressedLength), file);
                this.playProgressTime.Text = this.EstimateFinishTime(downloadCurrent, compressedLength);
            }

            this.playProgress.Value = (int)((long)100 * downloadCurrent / compressedLength);
        }
        private void OnDownloadFinished() {
            playenabled = true;
            this.playProgress.Value = 100;
            this.playButton.Image = Properties.Resources.playButton_enable;
            this.playButton.ForeColor = Color.White;
            this.playProgressText.Text = "DOWNLOAD COMPLETED";
            this.playProgressTime.Text = "";
        }

        private void OnDownloadFailed(Exception ex) {
            this.playProgress.Value = 0;
            this.playProgressText.Text = "DOWNLOAD FAILED!";
        }

        private void OnShowMessage(string message, string header) {
            MessageBox.Show(message, header);
        }
    }
}
