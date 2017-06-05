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

namespace GameLauncher {
    public partial class mainScreen : Form {
        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;
        public int DEBUG = 1;

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
            }

            consoleLog.SelectionColor = consoleLog.ForeColor;
            consoleLog.SelectionFont = new Font(consoleLog.Font, FontStyle.Regular);
            consoleLog.AppendText(e);
            consoleLog.AppendText("\r\n");
            consoleLog.ScrollToCaret();
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST) {
                m.Result = (IntPtr)(HT_CAPTION);
            }
        }

        public mainScreen() {
            MaximizeBox = false;

            InitializeComponent();

            closebtn.MouseEnter += new EventHandler(closebtn_MouseEnter);
            closebtn.MouseLeave += new EventHandler(closebtn_MouseLeave);
            closebtn.Click += new EventHandler(closebtn_Click);

            minimizebtn.MouseEnter += new EventHandler(minimizebtn_MouseEnter);
            minimizebtn.MouseLeave += new EventHandler(minimizebtn_MouseLeave);
            minimizebtn.Click += new EventHandler(minimizebtn_Click);
        }

        private void mainScreen_Load(object sender, EventArgs e) {
            //Console output to textbox
            ConsoleLog("Log initialized", "info");
            ConsoleLog("GameLauncher initialized", "info");

            //Fetch serverlist, and disable if failed to fetch.
            try
            {
                WebClient wc = new WebClientWithTimeout();
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)");
                string serverurl = "https://raw.githubusercontent.com/nilzao/soapbox-race-hill/master/serverlist-v2.txt";
                var response = wc.DownloadString(serverurl);
                ConsoleLog("Fetching " + serverurl, "info");

                if (String.IsNullOrEmpty(response)) {
                    MessageBox.Show("Failed to fetch serverlist:\r\nServerlist is empty.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Windows.Forms.Application.Exit();
                }

                //Time to add servers
                serverPick.DisplayMember = "Text";
                serverPick.ValueMember = "Value";

                List<Object> items = new List<Object>();

                String[] substrings = response.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (var substring in substrings)
                {
                    if (!String.IsNullOrEmpty(substring))
                    {
                        String[] substrings2 = substring.Split(new string[] { ";" }, StringSplitOptions.None);
                        items.Add(new { Text = substrings2[0], Value = substrings2[1] });
                    }
                }
                serverPick.DataSource = items;

            }
            catch (Exception ex) {
                MessageBox.Show("Failed to fetch serverlist:\r\n" + ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }


            serverStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999f, FontStyle.Bold, GraphicsUnit.Point, 0);
            serverStatusImg.Location = new Point(-16, -16);

            //ONLINE: 
            //serverStatusImg.Location = new Point(20, 323);
            //serverStatusImg.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.server_online));
            //serverStatus.ForeColor = Color.FromArgb(181, 255, 33);
            //serverStatus.Text = "This server is currenly up and running.";

            //OFFLINE: 
            //serverStatusImg.Location = new Point(20, 335); 
            //serverStatusImg.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.server_offline));
            //serverStatus.ForeColor = Color.FromArgb(227, 88, 50);
            //serverStatus.Text = "This server is currently down. Thanks for your patience.";
        }

        private void closebtn_Click(object sender, EventArgs e) {
            this.closebtn.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.close_click));
            System.Windows.Forms.Application.Exit();
        }

        private void closebtn_MouseEnter(object sender, EventArgs e) {
            this.closebtn.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.close_hover));
        }

        private void closebtn_MouseLeave(object sender, EventArgs e) {
            this.closebtn.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.close));
        }

        private void minimizebtn_Click(object sender, EventArgs e) {
            this.minimizebtn.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.minimize_click));
            this.WindowState = FormWindowState.Minimized;
        }

        private void minimizebtn_MouseEnter(object sender, EventArgs e) {
            this.minimizebtn.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.minimize_hover));
        }

        private void minimizebtn_MouseLeave(object sender, EventArgs e) {
            this.minimizebtn.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.minimize));
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string serverIP = serverPick.SelectedValue.ToString();
            string username = email.Text.ToString();

            HashAlgorithm algorithm = SHA1.Create();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(password.Text.ToString()))) {
                sb.Append(b.ToString("X2"));
            }
            string encryptedpassword = sb.ToString();

            ConsoleLog("Trying to login into " + serverPick.GetItemText(serverPick.SelectedItem) + " (" + serverIP + ")", "info");

            try
            {
                WebClient wc = new WebClientWithTimeout();
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)");

                string BuildURL = serverIP + "/soapbox-race-core/Engine.svc/User/authenticateUser?email=" + username + "&password=" + encryptedpassword;
                var response = wc.DownloadString(BuildURL);

                ConsoleLog("Looks like its success, but XML Parser is not implemented, yet." + response.Replace("\n", "").Replace("  ", ""), "warning");
            }
            catch (WebException ex) {
                if (ex.Status == WebExceptionStatus.ProtocolError) {
                    HttpWebResponse serverReply = (HttpWebResponse)ex.Response;
                    if ((int)serverReply.StatusCode == 500) {
                        using (StreamReader sr = new StreamReader(serverReply.GetResponseStream())) {
                            var response = sr.ReadToEnd();
                            ConsoleLog("Looks like its success, but XML Parser is not implemented, yet." + response.Replace("\n", "").Replace("  ", ""), "warning");
                        }
                    } else {
                        //Yup, its an error
                        ConsoleLog("Failed to login to server: " + ex.Message, "error");
                    }
                } else {
                    ConsoleLog("Failed to login to server: " + ex.Message, "error");
                }
            }
        }
    }
}
