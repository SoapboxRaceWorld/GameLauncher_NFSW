﻿using Flurl;
using GameLauncher.App.Classes;
using GameLauncher.App.Classes.Logger;
using GameLauncher.HashPassword;
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
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.App {
    public partial class SelectServer : Form {
        private int ID = 1;
        Dictionary<int, GetServerInformation> rememberServerInformationID = new Dictionary<int, GetServerInformation>();
        private GetServerInformation ServerInfo;

        public Queue<string> servers = new Queue<string>();

        private readonly IniFile _settingFile = new IniFile("Settings.ini");

        public SelectServer(String windowName = "") {
            InitializeComponent();

            if(windowName != "") this.Text = windowName;

            //And one for keeping data about server, IP tbh
            Dictionary<int, ServerInfo> data = new Dictionary<int, ServerInfo>();

            ServerListRenderer.View = View.Details;
            ServerListRenderer.FullRowSelect = true;

            ServerListRenderer.Columns.Add("");
            ServerListRenderer.Columns[0].Width = 1;

            ServerListRenderer.Columns.Add("Name");
            ServerListRenderer.Columns[1].Width = 220;

            ServerListRenderer.Columns.Add("Country");
            ServerListRenderer.Columns[2].Width = 80;

            ServerListRenderer.Columns.Add("Players Online");
            ServerListRenderer.Columns[3].Width = 80;
            ServerListRenderer.Columns[3].TextAlign = HorizontalAlignment.Right;

            ServerListRenderer.Columns.Add("Registered Players");
            ServerListRenderer.Columns[4].Width = 100;
            ServerListRenderer.Columns[4].TextAlign = HorizontalAlignment.Right;

            ServerListRenderer.Columns.Add("Ping");
            ServerListRenderer.Columns[5].Width = 55;
            ServerListRenderer.Columns[5].TextAlign = HorizontalAlignment.Right;

            //Actually accept JSON instead of old format//
            List<ServerInfo> serverInfos = new List<ServerInfo>();

            //foreach (var serverListURL in Self.serverlisturl) {
                try {
                    var wc = new WebClientWithTimeout();
                    var response = wc.DownloadString(Self.serverlisturl[0]);

                    try {
                        serverInfos.AddRange(JsonConvert.DeserializeObject<List<ServerInfo>>(response));
                    } catch (Exception error) {
                        Log.Error("Error occurred while deserializing server list from [" + Self.serverlisturl[0] + "]: " + error.Message);
                    }
                } catch (Exception error) {
                    Log.Error("Error occurred while loading server list from [" + Self.serverlisturl[0] + "]: " + error.Message);
                }
            //}

            if (File.Exists("servers.json")) {
                var fileItems = JsonConvert.DeserializeObject<List<ServerInfo>>(File.ReadAllText("servers.json"));

                if (fileItems.Count > 0) {
                    fileItems.Select(si => {
                        si.DistributionUrl = "";
                        si.DiscordPresenceKey = "";
                        si.Id = SHA.HashPassword($"{si.Name}:{si.Id}:{si.IpAddress}");
                        si.IsSpecial = false;
                        si.Category = "CUSTOM";

                        return si;
                    }).ToList().ForEach(si => serverInfos.Add(si));
                }
            }

            List<ServerInfo> newFinalItems = new List<ServerInfo>();
            foreach (ServerInfo xServ in serverInfos)
            {
                if (newFinalItems.FindIndex(i => string.Equals(i.Name, xServ.Name)) == -1)
                {
                    newFinalItems.Add(xServ);
                }
            }


            foreach (var substring in newFinalItems) {
                try {
                        servers.Enqueue(ID + "_|||_" + substring.IpAddress + "_|||_" + substring.Name);

                        ServerListRenderer.Items.Add(new ListViewItem(
                            new[] {
                                ID.ToString(), substring.Name, "", "", "", "", ""
                            }
                        ));

                        data.Add(ID, substring);
                    ID++;
                } catch {

                }
            }

            Thread newList = new Thread(() => {
                Thread.Sleep(200);
                this.BeginInvoke((MethodInvoker)delegate {
                    while(servers.Count != 0) {
                        string QueueContent = servers.Dequeue();
                        string[] QueueContent2 = QueueContent.Split(new string[] { "_|||_" }, StringSplitOptions.None);

                        int serverid = Convert.ToInt32(QueueContent2[0])-1;
                        string serverurl = QueueContent2[1] + "/GetServerInformation";
                        string servername = QueueContent2[2];

                        try {

                            WebClientWithTimeout getdata = new WebClientWithTimeout();
                            getdata.Timeout(1000);

                            GetServerInformation content = JsonConvert.DeserializeObject<GetServerInformation>(getdata.DownloadString(serverurl));

                            if (content == null) {
                                ServerListRenderer.Items[serverid].SubItems[1].Text = servername;
                                ServerListRenderer.Items[serverid].SubItems[2].Text = "N/A";
                                ServerListRenderer.Items[serverid].SubItems[3].Text = "N/A";
                                ServerListRenderer.Items[serverid].SubItems[4].Text = "---";
                            } else {
                                ServerListRenderer.Items[serverid].SubItems[1].Text = servername;
                                ServerListRenderer.Items[serverid].SubItems[2].Text = Self.CountryName(content.country.ToString());
                                ServerListRenderer.Items[serverid].SubItems[3].Text = content.onlineNumber.ToString();
                                ServerListRenderer.Items[serverid].SubItems[4].Text = content.numberOfRegistered.ToString();

                                //PING
                                if(!DetectLinux.LinuxDetected()) {
                                    Ping pingSender = new Ping();
                                    Uri StringToUri = new Uri(serverurl);
                                    pingSender.SendAsync(StringToUri.Host, 1000, new byte[1], new PingOptions(64, true), new AutoResetEvent(false));
                                    pingSender.PingCompleted += (sender3, e3) => {
                                        PingReply reply = e3.Reply;

                                        if (reply.Status == IPStatus.Success && servername != "Offline Built-In Server") {
                                            ServerListRenderer.Items[serverid].SubItems[5].Text = reply.RoundtripTime + "ms";
                                        } else {
                                            ServerListRenderer.Items[serverid].SubItems[5].Text = "---";
                                        }
                                    };
                                } else {
                                    ServerListRenderer.Items[serverid].SubItems[5].Text = "---";
                                }
                            }
                        } catch {
                            ServerListRenderer.Items[serverid].SubItems[1].Text = servername;
                            ServerListRenderer.Items[serverid].SubItems[3].Text = "N/A";
                            ServerListRenderer.Items[serverid].SubItems[4].Text = "N/A";
                            ServerListRenderer.Items[serverid].SubItems[5].Text = "---";
                        }


                        if (servers.Count == 0) {
                            loading.Text = "";
                        }

                        Application.DoEvents();
                    }
                });
            });
            newList.IsBackground = true;
            newList.Start();

            ServerListRenderer.AllowColumnReorder = false;
            ServerListRenderer.ColumnWidthChanging += (handler, args) => {
                args.Cancel = true;
                args.NewWidth = ServerListRenderer.Columns[args.ColumnIndex].Width;
            };

            ServerListRenderer.DoubleClick += new EventHandler((handler, args) => {
                if (ServerListRenderer.SelectedItems.Count == 1) {
                    rememberServerInformationID.TryGetValue(ServerListRenderer.SelectedIndices[0], out ServerInfo);

                    MainScreen.ServerName = data[ServerListRenderer.SelectedIndices[0]+1];

                    this.Close();
                }
            });
        }

        private void btnAddServer_Click(object sender, EventArgs e)
        {
            new AddServer().Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
