using GameLauncherReborn;
using Newtonsoft.Json;
using SoapBox.JsonScheme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncher.HashPassword;
using static System.String;

namespace GameLauncher.App {
    public partial class AddServer : Form {
        public AddServer() {
            InitializeComponent();
        }

        public void drawErrorAroundTextBox(TextBox x) {
            x.BorderStyle = BorderStyle.Fixed3D;
            Pen p = new Pen(Color.Red);
            Graphics g = this.CreateGraphics();
            int variance = 1;
            g.DrawRectangle(p, new Rectangle(x.Location.X - variance, x.Location.Y - variance, x.Width + variance, x.Height + variance));
        }

        private void okButton_Click(object sender, EventArgs e) {
			if (!File.Exists("servers.json")) {
				File.Create("servers.json");
			}

			bool success = true;
            error.Visible = false;
            this.Refresh();

            String wellFormattedURL = "";

            if (IsNullOrEmpty(serverAddress.Text)) {
                drawErrorAroundTextBox(serverAddress);
                success = false;
            }

            if (IsNullOrEmpty(serverName.Text)) {
                drawErrorAroundTextBox(serverName);
                success = false;
            }

            Uri uriResult;
            bool result = Uri.TryCreate(serverAddress.Text, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!result) {
                drawErrorAroundTextBox(serverAddress);
                success = false;
            } else {
                wellFormattedURL = uriResult.ToString();
            }

            cancelButton.Enabled = false;
            okButton.Enabled = false;
            serverAddress.Enabled = false;
            serverName.Enabled = false;

            try {
                var client = new WebClientWithTimeout();
                Uri StringToUri = new Uri(wellFormattedURL + "/GetServerInformation");
                var serverLoginResponse = client.DownloadString(StringToUri);

                GetServerInformation json = JsonConvert.DeserializeObject<GetServerInformation>(serverLoginResponse);

                if(IsNullOrEmpty(json.serverName)) {
                    drawErrorAroundTextBox(serverAddress);
                    success = false;
                }
            } catch {
                drawErrorAroundTextBox(serverAddress);
                success = false;
            }

            cancelButton.Enabled = true;
            okButton.Enabled = true;
            serverAddress.Enabled = true;
            serverName.Enabled = true;

            if (success == true) {
                try {
                    StreamReader sr = new StreamReader("servers.json");
                    String oldcontent = sr.ReadToEnd();
                    sr.Close();

                    if (IsNullOrWhiteSpace(oldcontent))
                    {
                        oldcontent = "[]";
                    }

                    var servers = JsonConvert.DeserializeObject<List<ServerInfo>>(oldcontent);

                    servers.Add(new ServerInfo
                    {
                        Name = serverName.Text,
                        IpAddress = wellFormattedURL,
                        IsSpecial = false,
                        Id = SHA.HashPassword(uriResult.Host)
                    });

                    File.WriteAllText("servers.json", JsonConvert.SerializeObject(servers));

                    MessageBox.Show(null, "New server will be added on next start of launcher.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) {
                    MessageBox.Show(null, "Failed to add new server. " + ex.Message, "GameLauncher", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }

                cancelButton_Click(sender, e);
            } else {
                error.Visible = true;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
