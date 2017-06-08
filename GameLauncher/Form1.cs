using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using GameLauncher.Properties;
using SlimDX.DirectInput;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using GameLauncher.Resources;

namespace GameLauncher {
    public partial class mainScreen : Form {
        Point mouseDownPoint = Point.Empty;

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
            consoleLog.AppendText("[" + DateTime.Now.ToString() + "] ");

            if (type == "warning") {
                consoleLog.SelectionColor = Color.Yellow;
                consoleLog.AppendText("[WARN] ");
            } else if(type == "info") {
                consoleLog.SelectionColor = Color.Cyan;
                consoleLog.AppendText("[INFO] ");
            } else if(type == "error") {
                consoleLog.SelectionColor = Color.Red;
                consoleLog.AppendText("[ERROR] ");
            } else if(type == "success") {
                consoleLog.SelectionColor = Color.Lime;
                consoleLog.AppendText("[SUCCESS] ");
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

            closebtn.MouseEnter += new EventHandler(closebtn_MouseEnter);
            closebtn.MouseLeave += new EventHandler(closebtn_MouseLeave);
            closebtn.Click += new EventHandler(closebtn_Click);

            minimizebtn.MouseEnter += new EventHandler(minimizebtn_MouseEnter);
            minimizebtn.MouseLeave += new EventHandler(minimizebtn_MouseLeave);
            minimizebtn.Click += new EventHandler(minimizebtn_Click);

            loginButton.MouseEnter += new EventHandler(loginButton_MouseEnter);
            loginButton.MouseLeave += new EventHandler(loginButton_MouseLeave);
            loginButton.Click += new EventHandler(loginButton_Click);
            loginButton.MouseUp += new MouseEventHandler(loginButton_MouseUp);
            loginButton.MouseDown += new MouseEventHandler(loginButton_MouseDown);

            serverPick.TextChanged += new EventHandler(serverPick_TextChanged);

            this.MouseDown += new MouseEventHandler(this.mainScreen_MouseDown);
            this.MouseMove += new MouseEventHandler(this.mainScreen_MouseMove);
            this.MouseUp += new MouseEventHandler(this.mainScreen_MouseUp);
        }

        private void mainScreen_Load(object sender, EventArgs e) {
            //Console output to textbox
            ConsoleLog("Log initialized", "info");
            ConsoleLog("GameLauncher initialized", "info");

            //Detect controller (if any)
            var directInput = new DirectInput();
            var controllerName = "";

            foreach (var deviceInstance in directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly)) {
                controllerName = deviceInstance.ProductName;
                if(controllerName == "Wireless Controller") {
                    /* @TODO@ Detection of 3rd party gamepad emulation like DS4Windows or InputMapper */
                    ConsoleLog("Found a controller. However, this controller might not work without a valid controller emulation, like DS4Windows", "warning");
                } else {
                    ConsoleLog("Found a valid controller: " + controllerName, "success");
                }
            }

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
                ConsoleLog("Failed to fetch serverlist. " + ex.Message , "error");
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
            serverStatus.Font = new Font("Microsoft Sans Serif", 9.749999f, FontStyle.Bold, GraphicsUnit.Point, 0);
            onlineCount.Font = new Font("Microsoft Sans Serif", 9.749999f, FontStyle.Bold, GraphicsUnit.Point, 0);
        }

        private void closebtn_Click(object sender, EventArgs e) {
            this.closebtn.BackgroundImage = Properties.Resources.close_click;
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

        private void loginButton_MouseUp(object sender, EventArgs e) {
            this.loginButton.Image = Properties.Resources.button_hover;
        }

        private void loginButton_MouseDown(object sender, EventArgs e) {
            this.loginButton.Image = Properties.Resources.button_click;
        }

        private void loginButton_Click(object sender, EventArgs e) {
            this.loginButton.Image = Properties.Resources.button_hover;

            string serverIP = serverPick.SelectedValue.ToString();
            string username = email.Text.ToString();
            string encryptedpassword = "";
            string serverLoginResponse = "";

            HashAlgorithm algorithm = SHA1.Create();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(password.Text.ToString()))) {
                sb.Append(b.ToString("X2"));
            }

            encryptedpassword = sb.ToString();
            ConsoleLog("Remember account details is set to " + rememberMe.Checked.ToString(), "info");

            if (rememberMe.Checked) {
                Settings.Default.email = username;
                Settings.Default.rememberme = 1;
            } else {
                Settings.Default.email = "";
                Settings.Default.rememberme = 0;
            }

            Settings.Default.Save();

            ConsoleLog("Trying to login into " + serverPick.GetItemText(serverPick.SelectedItem) + " (" + serverIP + ")", "info");

            try {
                WebClient wc = new WebClientWithTimeout();
                wc.Headers.Add("user-agent", "GameLauncher (+https://github.com/metonator/GameLauncher_NFSW)");

                string BuildURL = serverIP + "User/authenticateUser?email=" + username + "&password=" + encryptedpassword;
                ConsoleLog("Full URL: " + BuildURL, "info");

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

            string consoleXMLOutput = serverLoginResponse.Replace("\r", "").Replace("\n", "").Replace("  ", "").Replace("	", "");
            ConsoleLog("Looks like its success, but XML Parser is not implemented, yet: " + consoleXMLOutput, "warning");
        }

        private void loginButton_MouseEnter(object sender, EventArgs e) {
            this.loginButton.Image = Properties.Resources.button_hover;
        }

        private void loginButton_MouseLeave(object sender, EventArgs e) {
            this.loginButton.Image = Properties.Resources.button_disable;
        }

        private void clearConsole_Click(object sender, EventArgs e) {
            consoleLog.Text = "";
        }

        private void serverPick_TextChanged(object sender, EventArgs e) {
            string serverIP = serverPick.SelectedValue.ToString();

            serverStatusImg.Location = new Point(-16, -16);
            serverStatus.ForeColor = Color.White;
            serverStatus.Text = "Retrieving server status...";
            onlineCount.Text = "";

            var client = new WebClient();
            Uri StringToUri = new Uri(serverIP + "OnlineUsers/getOnline");
            client.DownloadStringAsync(StringToUri);
            client.DownloadStringCompleted += (sender2, e2) => {
                if (e2.Error != null) {
                    serverStatusImg.Location = new Point(20, 335);
                    serverStatusImg.BackgroundImage = Properties.Resources.server_offline;
                    serverStatus.ForeColor = Color.FromArgb(227, 88, 50);
                    serverStatus.Text = "This server is currently down. Thanks for your patience.";
                } else {
                    serverStatusImg.Location = new Point(20, 323);
                    serverStatusImg.BackgroundImage = Properties.Resources.server_online;
                    serverStatus.ForeColor = Color.FromArgb(181, 255, 33);
                    serverStatus.Text = "This server is currenly up and running.";
                    onlineCount.Text = "Players on server: " + e2.Result;
                }
            };
        }

        private void ApplyEmbeddedFonts() {
            FontFamily fontFamily   = FontWrapper.Instance.GetFontFamily("Font_MyriadProSemiCondBold.ttf");
            FontFamily fontFamily2  = FontWrapper.Instance.GetFontFamily("Font_Register.ttf");
            FontFamily fontFamily3  = FontWrapper.Instance.GetFontFamily("Font_RegisterBoldItalic.ttf");
            FontFamily fontFamily4  = FontWrapper.Instance.GetFontFamily("Font_RegisterBold.ttf");

            currentWindowInfo.Font  = new Font(fontFamily3, 12.75f, FontStyle.Italic);
            rememberMe.Font         = new Font(fontFamily, 9f, FontStyle.Bold);
            loginButton.Font        = new Font(fontFamily2, 15f, FontStyle.Bold | FontStyle.Italic);
            serverStatus.Font       = new Font(fontFamily, 9.749999f, FontStyle.Bold);
            onlineCount.Font        = new Font(fontFamily, 9.749999f, FontStyle.Bold);
        }
    }
}
