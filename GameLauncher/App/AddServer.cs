using GameLauncherReborn;
using GameLauncher.Resources;
using Newtonsoft.Json;
using SoapBox.JsonScheme;
using System;
using System.Net;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncher.HashPassword;
using static System.String;

namespace GameLauncher.App
{
    public partial class AddServer : Form {
        public AddServer() {
            InitializeComponent();
            ApplyEmbeddedFonts();
        }

        public void DrawErrorAroundTextBox(TextBox x) {
            x.BorderStyle = BorderStyle.Fixed3D;
            Pen p = new Pen(Color.Red);
            Graphics g = this.CreateGraphics();
            int variance = 1;
            g.DrawRectangle(p, new Rectangle(x.Location.X - variance, x.Location.Y - variance, x.Width + variance, x.Height + variance));
        }

        private void ApplyEmbeddedFonts() {
            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansCondensed = FontWrapper.Instance.GetFontFamily("DejaVuSansCondensed.ttf");
            OkBTN.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            CancelBTN.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            ServerNameLabel.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            ServerName.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            ServerAddress.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            ServerAddressLabel.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            Error.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            Version.Font= new Font(DejaVuSans, 8.25f, FontStyle.Regular);
        }

        private void OkButton_Click(object sender, EventArgs e) {
			if (!File.Exists("servers.json")) {
				File.Create("servers.json");
			}

			bool success = true;
            Error.Visible = false;
            this.Refresh();

            String wellFormattedURL = "";

            if (IsNullOrEmpty(ServerAddress.Text)) {
                DrawErrorAroundTextBox(ServerAddress);
                success = false;
            }

            if (IsNullOrEmpty(ServerName.Text)) {
                DrawErrorAroundTextBox(ServerName);
                success = false;
            }

            Uri uriResult;
            bool result = Uri.TryCreate(ServerAddress.Text, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!result) {
                DrawErrorAroundTextBox(ServerAddress);
                success = false;
            } else {
                wellFormattedURL = uriResult.ToString();
            }

            CancelBTN.Enabled = false;
            OkBTN.Enabled = false;
            ServerAddress.Enabled = false;
            ServerName.Enabled = false;

            try {
                var client = new WebClient();
                Uri StringToUri = new Uri(wellFormattedURL + "/GetServerInformation");
                var serverLoginResponse = client.DownloadString(StringToUri);

                GetServerInformation json = JsonConvert.DeserializeObject<GetServerInformation>(serverLoginResponse);

                if (IsNullOrEmpty(json.ServerName)) {
                    DrawErrorAroundTextBox(ServerAddress);
                    success = false;
                }
            } catch {
                DrawErrorAroundTextBox(ServerAddress);
                success = false;
            }

            CancelBTN.Enabled = true;
            OkBTN.Enabled = true;
            ServerAddress.Enabled = true;
            ServerName.Enabled = true;

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
                        Name = ServerName.Text,
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

                CancelButton_Click(sender, e);
            } else {
                Error.Visible = true;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
