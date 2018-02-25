using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using GameLauncherReborn;
using Newtonsoft.Json;
using SoapBox.JsonScheme;

namespace GameLauncher.App {
    public partial class ShowMap : Form {
        string ServerIP = String.Empty;
        string ServerName = String.Empty;
        string ServerReply = String.Empty;
        int SquareSize = 4;

        public ShowMap(string serverIP, string serverName) {
            ServerIP = serverIP;
            ServerName = serverName;
            InitializeComponent();
        }

        private void picturebox_Paint(object sender, PaintEventArgs e) {
            PictureBox p = sender as PictureBox;
            Graphics gr = e.Graphics;
            gr.ResetTransform();
            gr.FillRectangle(Brushes.Red, 0, 0, SquareSize*2, SquareSize*2);
        }

        private void ShowMap_Load(object sender, EventArgs e) {

            this.Close();
            MessageBox.Show(null, "This function is not yet ready.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            /*WebClient wc = new WebClientWithTimeout();
            string BuildURL = ServerIP + "/GetFreeroamJson";
            ServerReply = wc.DownloadString(BuildURL);

            if(String.IsNullOrEmpty(ServerReply)) {
                this.Close();
                MessageBox.Show(null, "This server does not have freeroam preview.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else {
                ServerReply = ServerReply.Replace("updateMap(", String.Empty);
                ServerReply = ServerReply.Replace(");", String.Empty);

                FreeroamObject json = JsonConvert.DeserializeObject<FreeroamObject>(ServerReply);

                foreach(var player in json.objList) {
                    PictureBox picturebox = new PictureBox();
                    picturebox.Name = "Square_" + player.id;
                    picturebox.Tag = 1;
                    picturebox.Size = new Size(SquareSize, SquareSize);
                    picturebox.Location = new Point(170, 385);
                    this.Controls.Add(picturebox);
                    picturebox.BringToFront();
                    picturebox.Paint += new PaintEventHandler(picturebox_Paint);
                    picturebox.MouseEnter += (sender2, e2) => {
                        string con = ((PictureBox)sender2).Name;
                        ToolTip tt = new ToolTip();
                        Controls[con].Size = new Size(SquareSize*2, SquareSize*2);
                        Controls[con].Location = new Point(Controls[con].Location.X-2, Controls[con].Location.Y - 2);
                        tt.SetToolTip( Controls[con] , con.Replace("Square_", String.Empty) );
                    };
                    picturebox.MouseLeave += (sender3, e3) => {
                        string con = ((PictureBox)sender3).Name;
                        Controls[con].Location = new Point(Controls[con].Location.X + 2, Controls[con].Location.Y + 2);
                        Controls[con].Size = new Size(SquareSize, SquareSize);
                    };
                }

                this.Text = "Freeroam: " + ServerName;
                this.srvInfo.Text = "ServerName: " + ServerName + "\nPlayers on freeroam: " + json.objList.Count();
            }*/
        }
    }
}
