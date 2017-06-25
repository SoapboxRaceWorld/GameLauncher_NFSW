using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using GameLauncher.Properties;
using SlimDX.DirectInput;
using GameLauncher.Resources;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.Threading;
using System.Net.NetworkInformation;

namespace GameLauncher {
    public partial class mainScreen : Form {
        Point mouseDownPoint = Point.Empty;
        bool loginEnabled;
        bool serverEnabled;
        bool builtinserver = false;

        private void mainScreen_MouseDown(object sender, MouseEventArgs e) {
            mouseDownPoint = new Point(e.X, e.Y);
        }

        private void mainScreen_MouseUp(object sender, MouseEventArgs e) {
            mouseDownPoint = Point.Empty;
        }

        private void mainScreen_MouseMove(object sender, MouseEventArgs e) {
            if (mouseDownPoint.IsEmpty) { return; }
            Form f = sender as Form;
            f.Location = new Point(f.Location.X + (e.X - mouseDownPoint.X), f.Location.Y + (e.Y - mouseDownPoint.Y));
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
            consoleLog.ScrollToCaret();
        }

        public mainScreen() {
            InitializeComponent();
            ApplyEmbeddedFonts();

            MaximizeBox = false;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            //First of all, we need to check if files exists
            String[] files = { "SlimDX.dll", "Microsoft.WindowsAPICodePack.dll", "Microsoft.WindowsAPICodePack.Shell.dll" };
            foreach(string file in files) {
                if (!File.Exists(file)) {
                    MessageBox.Show(null, "Cannot find " + file + " - Exiting", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(1);
                }
            }

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

            email.KeyUp += new KeyEventHandler(loginbuttonenabler);
            password.KeyUp += new KeyEventHandler(loginbuttonenabler);

            email.PreviewKeyDown += new PreviewKeyDownEventHandler(loginEnter);
            password.PreviewKeyDown += new PreviewKeyDownEventHandler(loginEnter);

            serverPick.TextChanged += new EventHandler(serverPick_TextChanged);
            forgotPassword.LinkClicked += new LinkLabelLinkClickedEventHandler(forgotPassword_LinkClicked);
            githubLink.LinkClicked += new LinkLabelLinkClickedEventHandler(githubLink_LinkClicked);

            this.MouseDown += new MouseEventHandler(this.mainScreen_MouseDown);
            this.MouseMove += new MouseEventHandler(this.mainScreen_MouseMove);
            this.MouseUp += new MouseEventHandler(this.mainScreen_MouseUp);

            //Command-line Arguments
            string[] args = Environment.GetCommandLineArgs();


            //Somewhere here we will setup the game installation directory
            if (String.IsNullOrEmpty(Settings.Default.InstallationDirectory)) {
                var openFolder = new CommonOpenFileDialog();
                openFolder.InitialDirectory = "";
                openFolder.IsFolderPicker = true;
                openFolder.Title = "GameLauncher: Please pick up a directory to install NFSW.";
                var result = openFolder.ShowDialog();

                if (result == CommonFileDialogResult.Ok) {
                    Settings.Default.InstallationDirectory = openFolder.FileName;
                    Settings.Default.Save();
                } else if(result == CommonFileDialogResult.Cancel) {
                    System.Environment.Exit(1);
                }
            }

            if (!Directory.Exists(Settings.Default.InstallationDirectory + "/nfsw")) {
                Directory.CreateDirectory(Settings.Default.InstallationDirectory + "/nfsw");
                Directory.CreateDirectory(Settings.Default.InstallationDirectory + "/nfsw/Cache");
                Directory.CreateDirectory(Settings.Default.InstallationDirectory + "/nfsw/Data");
                Directory.CreateDirectory(Settings.Default.InstallationDirectory + "/nfsw/Data/Modules");
                File.Create(Settings.Default.InstallationDirectory + "/nfsw/Cache/keep.this");
                File.Create(Settings.Default.InstallationDirectory + "/nfsw/Data/put.your.nfsw.exe.here");
                Process.Start(@"" + Settings.Default.InstallationDirectory + "/nfsw/Data/");
            }

            registerText.Text = "DON'T HAVE AN ACCOUNT?\nCLICK HERE TO CREATE ONE NOW...";

        }

        private void mainScreen_Load(object sender, EventArgs e) {
            //Console output to textbox
            ConsoleLog("Log initialized", "info");
            ConsoleLog("GameLauncher initialized", "info");
            ConsoleLog("Installation directory: " + Settings.Default.InstallationDirectory, "info");

            //Silly way to detect mono
            int SysVersion = (int)Environment.OSVersion.Platform;
            bool mono = (SysVersion == 4 || SysVersion == 6 || SysVersion == 128);

            //Silly way to detect wine
            bool wine;
            try {
                RegistryKey regKey = Registry.CurrentUser;
                RegistryKey rkTest = regKey.OpenSubKey(@"Software\Wine");

                if(String.IsNullOrEmpty(rkTest.ToString())) {
                    wine = false;
                } else {
                    wine = true;
                }
            } catch {
                wine = false;
            }

            //Console log with warning
            if (mono == true) {
                ConsoleLog("Detected OS: Linux using Mono - Note that game might not launch.", "warning");
            } else if (wine == true) {
                ConsoleLog("Detected OS: Linux using Wine - Note that game might not launch.", "warning");
            }

            //Detect controller (if any)
            var directInput = new DirectInput();
            var controllerName = "";

            foreach (var deviceInstance in directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly)) {
                controllerName = deviceInstance.ProductName;
                if (controllerName == "Wireless Controller") {
                    /* @TODO@ Detection of 3rd party gamepad emulation like DS4Windows or InputMapper */
                    ConsoleLog("Found a controller. However, this controller might not work without a valid controller emulation, like DS4Windows or InputMapper", "warning");
                }
            }

            //Detect modules inside gamefolder
            //modules/MODULENAME..dll vs. modules/MODULENAME.asi

            email.Text = Settings.Default.email.ToString();
            if (Settings.Default.rememberme == 1) {
                rememberMe.Checked = true;
            }

            //Fetch serverlist, and disable if failed to fetch.
            var response = "";
            try {
                WebClient wc = new WebClientWithTimeout();
                wc.Headers.Add("user-agent", "GameLauncher (+https://github.com/metonator/GameLauncher_NFSW)");

                string serverurl = "http://nfsw.metonator.ct8.pl/serverlist.txt";
                response = wc.DownloadString(serverurl);
                ConsoleLog("Fetching " + serverurl, "info");
            } catch (Exception ex) {
                ConsoleLog("Failed to fetch serverlist. " + ex.Message, "error");
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
            serverStatusImg.Location = new Point(-16, -16);
            loginEnabled = false;
            serverEnabled = false;
            this.loginButton.Image = Properties.Resources.button_disable;
            this.loginButton.ForeColor = Color.FromArgb(128, 128, 128);
            RegisterFormHideElements();
        }

        private void closebtn_Click(object sender, EventArgs e) {
            this.closebtn.BackgroundImage = Properties.Resources.close_click;
            if(!Directory.Exists("logs")) {
                Directory.CreateDirectory("logs");
            }

            long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000000;
            string timestamp = ticks.ToString();
            consoleLog.SaveFile("logs/" + timestamp + ".log", RichTextBoxStreamType.PlainText);

            Application.Exit();
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

        private void loginEnter(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyCode == Keys.Return && loginEnabled == true) {
                loginButton_Click(sender, e);
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

            string serverIP = serverPick.SelectedValue.ToString();
            string serverName = serverPick.GetItemText(serverPick.SelectedItem);
            string username = email.Text.ToString();
            string encryptedpassword = "";
            string serverLoginResponse = "";

            HashAlgorithm algorithm = SHA1.Create();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(password.Text.ToString()))) {
                sb.Append(b.ToString("X2"));
            }

            encryptedpassword = sb.ToString();

            if (rememberMe.Checked) {
                Settings.Default.email = username;
                Settings.Default.rememberme = 1;
            } else {
                Settings.Default.email = "";
                Settings.Default.rememberme = 0;
            }

            Settings.Default.Save();

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
            SBRW_XML.LoadXml(serverLoginResponse);
            XmlNodeList nodes = null;

            String Description = "";
            String LoginToken = "";
            String UserId = "";

            if (serverName == "World Revival") {
                try {
                    if(SBRW_XML.SelectSingleNode("EngineExceptionTrans") != null) {
                        nodes = SBRW_XML.SelectNodes("EngineExceptionTrans");
                    } else {
                        nodes = SBRW_XML.SelectNodes("LoginData");
                    }
                } catch(Exception) {
                    nodes = SBRW_XML.SelectNodes("LoginData");
                }
            }
            else {
                nodes = SBRW_XML.SelectNodes("LoginStatusVO");
            }

            foreach (XmlNode childrenNode in nodes) {
                if (serverName == "World Revival") {
                    try {
                        UserId = childrenNode["UserId"].InnerText;
                        LoginToken = childrenNode["LoginToken"].InnerText;
                    } catch {
                        Description = "LOGIN ERROR";
                    }
                } else {
                    UserId = childrenNode["UserId"].InnerText;
                    LoginToken = childrenNode["LoginToken"].InnerText;
                    Description = childrenNode["Description"].InnerText;
                }

                if (Description == "LOGIN ERROR") {
                    ConsoleLog("Invalid username or password.", "error");
                } else {
                    try {
                        string filename = Settings.Default.InstallationDirectory.ToString() + "\\nfsw\\Data\\nfsw.exe";
                        ConsoleLog("Logged in. Starting game (" + filename + ").", "success");
                        String cParams = "US " + serverIP + " " + LoginToken + " " + UserId;
                        var proc = Process.Start(filename, cParams);
                        proc.EnableRaisingEvents = true;
                        proc.Exited += (sender2, e2) => {
                            Application.Exit();
                        };

                        if (builtinserver == true) {
                            ConsoleLog("SoapBox Built-In Initialized, waiting for queries", "success");
                        } else {
                            ConsoleLog("Closing myself in 5 seconds.", "warning");
                            Thread.Sleep(5000);
                            Application.Exit();
                        }
                    } catch (Exception) {
                        ConsoleLog("Logged in. But i cannot find NFSW executable file. Are you sure you've copied all files?", "error");
                    }
                }
            }
        }

        private void loginButton_MouseEnter(object sender, EventArgs e) {
            if (loginEnabled == true || builtinserver == true) {
                this.loginButton.Image = Properties.Resources.button_hover;
            } else {
                this.loginButton.Image = Properties.Resources.button_disable;
            }
        }

        private void loginButton_MouseLeave(object sender, EventArgs e) {
            if (loginEnabled == true || builtinserver == true) {
                this.loginButton.Image = Properties.Resources.button_enable;
            } else {
                this.loginButton.Image = Properties.Resources.button_disable;
            }
        }

        private void clearConsole_Click(object sender, EventArgs e) {
            consoleLog.ForeColor = Color.Gray;
            consoleLog.Text = "Console cleaned.";
        }

        private void serverPick_TextChanged(object sender, EventArgs e) {
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
            } else {
                builtinserver = false;
                this.loginButton.Image = Properties.Resources.button_disable;
                this.loginButton.Text = "LOG IN";
            }

            var client = new WebClient();
            Uri StringToUri = new Uri(serverIP + "/OnlineUsers/getOnline");
            client.DownloadStringAsync(StringToUri);
            client.DownloadStringCompleted += (sender2, e2) => {
                if (e2.Error != null) {
                    serverStatusImg.Location = new Point(20, 335);
                    serverStatusImg.BackgroundImage = Properties.Resources.server_offline;
                    serverStatus.ForeColor = Color.FromArgb(227, 88, 50);
                    serverStatus.Text = "This server is currently down. Thanks for your patience.";
                    serverStatus.Location = new Point(44, 329);
                    serverEnabled = false;
                } else {
                    serverStatusImg.Location = new Point(20, 323);
                    serverStatusImg.BackgroundImage = Properties.Resources.server_online;
                    serverStatus.ForeColor = Color.FromArgb(181, 255, 33);
                    serverStatus.Text = "This server is currenly up and running.";
                    serverStatus.Location = new Point(44, 322);

                    if (serverName == "Offline Built-In Server") {
                        numPlayers = "1337";
                    } else if(serverName == "World Revival") {
                        //JSON... and Dedicated API... c'mon WorldRevival...
                        Uri StringToUri2 = new Uri("http://world-revival.fr/api/GetStatus.php");
                        var reply = client.DownloadString(StringToUri2);
                        String[] substrings = reply.Split(new string[] { "\"" }, StringSplitOptions.None);
                        numPlayers = substrings[9];
                    } else {
                        numPlayers = e2.Result;
                    }

                    onlineCount.Text = "Players on server: " + numPlayers;
                    serverEnabled = true;
                }
            };

            Ping pingSender = new Ping();
            pingSender.SendAsync(StringToUri.Host, 1000, new byte[1], new PingOptions(64, true), new AutoResetEvent(false));
            pingSender.PingCompleted += (sender2, e2) => {
                PingReply reply = e2.Reply;

                if (reply.Status == IPStatus.Success && serverName != "Offline Built-In Server") {
                    ConsoleLog("This PC <---> " + serverName + ": " + reply.RoundtripTime + "ms", "ping");
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

            currentWindowInfo.Font = new Font(fontFamily3, 12.75f, FontStyle.Italic);
            rememberMe.Font = new Font(fontFamily, 9f, FontStyle.Bold);
            loginButton.Font = new Font(fontFamily2, 15f, FontStyle.Bold | FontStyle.Italic);
            registerButton.Font = new Font(fontFamily2, 15f, FontStyle.Bold | FontStyle.Italic);
            serverStatus.Font = new Font(fontFamily, 9.749999f, FontStyle.Bold);
            onlineCount.Font = new Font(fontFamily, 9.749999f, FontStyle.Bold);
            registerText.Font = new Font(fontFamily, 9.749999f, FontStyle.Bold);
            emailLabel.Font = new Font(fontFamily4, 11f);
            passwordLabel.Font = new Font(fontFamily4, 11f);
            troubleLabel.Font = new Font(fontFamily, 9.749999f, FontStyle.Bold);
            githubLink.Font = new Font(fontFamily, 9.749999f, FontStyle.Bold);
            forgotPassword.Font = new Font(fontFamily, 9f);
            selectServerLabel.Font = new System.Drawing.Font(fontFamily, 9.749999f, FontStyle.Bold);
        }

        private void registerText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if(serverPick.GetItemText(serverPick.SelectedItem) == "World Revival") {
                ConsoleLog("Redirecting into Registration page", "info");
                Process.Start("http://world-revival.fr/user/register");
            } else {
                this.BackgroundImage = Properties.Resources.settingsbg;
                this.currentWindowInfo.Text = "REGISTER ON " + serverPick.GetItemText(serverPick.SelectedItem).ToUpper();
                LoginFormHideElements();
                RegisterFormShowElements();
            }
        }

        private void forgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (serverPick.GetItemText(serverPick.SelectedItem) == "World Revival") {
                ConsoleLog("Redirecting into Reset Password page", "info");
                Process.Start("http://world-revival.fr/user/reset");
            } else {
                MessageBox.Show(null, "This server does not have that ability, yet.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void githubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ConsoleLog("Redirecting into GitHub Issue page", "info");
            Process.Start("https://github.com/metonator/GameLauncher_NFSW/issues");
        }

        private void LoginFormHideElements()
        {
            this.rememberMe.Hide();
            this.loginButton.Hide();
            this.serverStatus.Hide();
            this.onlineCount.Hide();
            this.registerText.Hide();
            this.serverPick.Hide();
            this.serverStatusImg.Hide();
            this.consoleLog.Hide();
            this.clearConsole.Hide();
            this.email.Hide();
            this.password.Hide();
            this.emailLabel.Hide();
            this.passwordLabel.Hide();
            this.troubleLabel.Hide();
            this.githubLink.Hide();
            this.forgotPassword.Hide();
            this.selectServerLabel.Hide();
        }

        private void LoginFormShowElements() {
            this.rememberMe.Show();
            this.loginButton.Show();
            this.serverStatus.Show();
            this.onlineCount.Show();
            this.registerText.Show();
            this.serverPick.Show();
            this.serverStatusImg.Show();
            this.consoleLog.Show();
            this.clearConsole.Show();
            this.email.Show();
            this.password.Show();
            this.emailLabel.Show();
            this.passwordLabel.Show();
            this.troubleLabel.Show();
            this.githubLink.Show();
            this.forgotPassword.Show();
            this.selectServerLabel.Show();
        }

        /* 
         * REGISTER PAGE LAYOUT 
         * Because why should i close Form1 and create/open Form2 if it will look a bit more responsive...
         */

        private void RegisterFormHideElements() {
            this.registerButton.Hide();
        }

        private void RegisterFormShowElements() {
            this.registerButton.Show();
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
            this.BackgroundImage = Properties.Resources.loginbg;
            RegisterFormHideElements();
            LoginFormShowElements();
        }

        /*
         * SETTINGS PAGE LAYOUT
         * A random description
         */

        private void settingsButton_Click(object sender, EventArgs e) {
            this.settingsButton.BackgroundImage = Properties.Resources.settingsbtn_click;
            ConsoleLog("Settings comming soon.", "error");
        }

        private void settingsButton_MouseEnter(object sender, EventArgs e) {
            this.settingsButton.BackgroundImage = Properties.Resources.settingsbtn_hover;
        }

        private void settingsButton_MouseLeave(object sender, EventArgs e) {
            this.settingsButton.BackgroundImage = Properties.Resources.settingsbtn;
        }
    }
}