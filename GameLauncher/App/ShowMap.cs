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
            int max = Convert.ToInt32(99999);
            gr.FillRectangle(Brushes.Red, 0, 0, SquareSize*2, SquareSize*2);
        }

        private void ShowMap_Load(object sender, EventArgs e) {
            WebClient wc = new WebClientWithTimeout();
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
                    /* 
                     * MAX X VALUE JSON: 15600 (-3600 / 12000)
                     * MAX X VALUE MAP: 1024
                     * DIV X: 15,24
                     * 
                     * MAX Y VALUE JSON: 7750 (-1900 / 5850)
                     * MAX Y VALUE MAP: 562
                     * DIV Y: 13,79
                     * 
                     */

                    int new_x = Convert.ToInt32( (player.x + 3600) / (15600 / this.Width) );
                    int new_y = Convert.ToInt32( (player.y + 1900) / (7750 / this.Height) );

                    string x = "";
                    x += "PLAYER: " + player.id + "\n";
                    x += "Original place: " + new Size(player.x, player.y).ToString() + "\n";
                    x += "Recalculated place: " + new Size(new_x, new_y).ToString() + "\n";
                    x += "Window Size: " + this.Size.ToString() + "\n";

                    MessageBox.Show(x);

                    PictureBox picturebox = new PictureBox();
                    picturebox.Name = "Square_" + player.id;
                    picturebox.Tag = 1;
                    picturebox.Size = new Size(SquareSize, SquareSize);
                    picturebox.Location = new Point(170, 385);
                    //picturebox.Location = new Point(new_x, new_y);
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
            }
        }
    }
}
