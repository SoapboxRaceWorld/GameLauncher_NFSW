using GameLauncher.App.Classes;
using GameLauncherReborn;
using Newtonsoft.Json;
using SoapBox.JsonScheme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.App {
    public partial class SelectServer : Form {
        private int ID = 1;
        Dictionary<int, GetServerInformation> rememberServerInformationID = new Dictionary<int, GetServerInformation>();
        private GetServerInformation ServerInfo;

        private readonly IniFile _settingFile = new IniFile("Settings.ini");

        public SelectServer() {
            InitializeComponent();

            //And one for keeping data about server, IP tbh
            Dictionary<int, ServerInfo> data = new Dictionary<int, ServerInfo>();

            ServerListRenderer.View = View.Details;
            ServerListRenderer.FullRowSelect = true;

            ServerListRenderer.Columns.Add("ID");
            ServerListRenderer.Columns[0].Width = 25;

            ServerListRenderer.Columns.Add("Name");
            ServerListRenderer.Columns[1].Width = 320;

            ServerListRenderer.Columns.Add("Players Online");
            ServerListRenderer.Columns[2].Width = 80;
            ServerListRenderer.Columns[2].TextAlign = HorizontalAlignment.Right;

            ServerListRenderer.Columns.Add("Registered Players");
            ServerListRenderer.Columns[3].Width = 100;
            ServerListRenderer.Columns[3].TextAlign = HorizontalAlignment.Right;

            //Actually accept JSON instead of old format//
            List<ServerInfo> serverInfos = new List<ServerInfo>();

            foreach (var serverListURL in Self.serverlisturl) {
                try {
                    var wc = new WebClientWithTimeout();
                    var response = wc.DownloadString(serverListURL);

                    try {
                        serverInfos.AddRange(JsonConvert.DeserializeObject<List<ServerInfo>>(response));
                    } catch (Exception error) {
                        throw new Exception("Error occurred while deserializing server list from [" + serverListURL + "]: " + error.Message, error);
                    }
                } catch (Exception error) {
                    throw new Exception("Error occurred while loading server list from [" + serverListURL + "]: " + error.Message, error);
                }
            }

            foreach (var substring in serverInfos) {
                try {
                    GetServerInformation content = JsonConvert.DeserializeObject<GetServerInformation>(new WebClientWithTimeout().DownloadString(substring.IpAddress + "/GetServerInformation"));
                    
                    Console.Write(content);

                    if (content != null) {
                        ServerListRenderer.Items.Add(new ListViewItem(
                            new[] {
                                ID.ToString(),
                                substring.Name,
                                content.onlineNumber.ToString(),
                                content.numberOfRegistered.ToString(),
                            }
                        ));

                        data.Add(ID, substring);
                    }

                    rememberServerInformationID.Add(ID, content);
                    ID++;
                } catch {

                }
            }

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
    }
}
