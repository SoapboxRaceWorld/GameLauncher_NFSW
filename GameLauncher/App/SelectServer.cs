using GameLauncher.App.Classes;
using GameLauncherReborn;
using GameLauncher.Resources;
using Newtonsoft.Json;
using SoapBox.JsonScheme;
using System;
using System.Net;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.Visuals;

namespace GameLauncher.App
{
    public partial class SelectServer : Form
    {
        private readonly int ID = 1;
        readonly Dictionary<int, GetServerInformation> rememberServerInformationID = new Dictionary<int, GetServerInformation>();
        public GetServerInformation ServerInfo;
        readonly Dictionary<int, ServerInfo> data = new Dictionary<int, ServerInfo>();

        //Used to ping the Server in ms
        public Queue<string> servers = new Queue<string>();

        public SelectServer()
        {
            InitializeComponent();
            SetVisuals();

            Version.Text = "Version: v" + Application.ProductVersion;

            //And one for keeping data about server, IP tbh
            ServerListRenderer.View = View.Details;
            ServerListRenderer.FullRowSelect = true;

            ServerListRenderer.Columns.Add("");
            ServerListRenderer.Columns[0].Width = 1;

            ServerListRenderer.Columns.Add("Name");
            ServerListRenderer.Columns[1].Width = 210;

            ServerListRenderer.Columns.Add("Country");
            ServerListRenderer.Columns[2].Width = 100;
            ServerListRenderer.Columns[2].TextAlign = HorizontalAlignment.Center;

            ServerListRenderer.Columns.Add("Online");
            ServerListRenderer.Columns[3].Width = 75;
            ServerListRenderer.Columns[3].TextAlign = HorizontalAlignment.Center;

            ServerListRenderer.Columns.Add("Registered");
            ServerListRenderer.Columns[4].Width = 85;
            ServerListRenderer.Columns[4].TextAlign = HorizontalAlignment.Center;

            ServerListRenderer.Columns.Add("Ping");
            ServerListRenderer.Columns[5].Width = 60;
            ServerListRenderer.Columns[5].TextAlign = HorizontalAlignment.Center;

            foreach (var substring in ServerListUpdater.NoCategoryList)
            {
                try
                {
                    servers.Enqueue(ID + "_|||_" + substring.IpAddress + "_|||_" + substring.Name);

                    ServerListRenderer.Items.Add(new ListViewItem(
                        new[]
                        {
                                ID.ToString(), substring.Name, "", "", "", "", ""
                        }
                    ));

                    data.Add(ID, substring);
                    ID++;
                }
                catch
                {

                }
            }

            Thread newList = new Thread(() =>
            {
                Thread.Sleep(200);
                this.BeginInvoke((MethodInvoker)delegate
                {
                    while (servers.Count != 0)
                    {
                        string QueueContent = servers.Dequeue();
                        string[] QueueContent2 = QueueContent.Split(new string[] { "_|||_" }, StringSplitOptions.None);

                        int serverid = Convert.ToInt32(QueueContent2[0]) - 1;
                        string serverurl = QueueContent2[1] + "/GetServerInformation";
                        string servername = QueueContent2[2];

                        try
                        {
                            WebClient getdata = new WebClient();
                            GetServerInformation content = JsonConvert.DeserializeObject<GetServerInformation>(getdata.DownloadString(serverurl));

                            if (content == null)
                            {
                                ServerListRenderer.Items[serverid].SubItems[1].Text = servername;
                                ServerListRenderer.Items[serverid].SubItems[2].Text = "---";
                                ServerListRenderer.Items[serverid].SubItems[3].Text = "---";
                                ServerListRenderer.Items[serverid].SubItems[4].Text = "---";
                            }
                            else
                            {
                                ServerListRenderer.Items[serverid].SubItems[1].Text = servername;
                                ServerListRenderer.Items[serverid].SubItems[2].Text = Self.CountryName(content.Country.ToString());
                                ServerListRenderer.Items[serverid].SubItems[3].Text = content.OnlineNumber.ToString();
                                ServerListRenderer.Items[serverid].SubItems[4].Text = content.NumberOfRegistered.ToString();

                                //PING
                                if (!DetectLinux.LinuxDetected())
                                {
                                    Ping pingSender = new Ping();
                                    Uri StringToUri = new Uri(serverurl);
                                    pingSender.SendAsync(StringToUri.Host, 1000, new byte[1], new PingOptions(64, true), new AutoResetEvent(false));
                                    pingSender.PingCompleted += (sender3, e3) => {
                                        PingReply reply = e3.Reply;

                                        if (reply.Status == IPStatus.Success && servername != "Offline Built-In Server")
                                        {
                                            ServerListRenderer.Items[serverid].SubItems[5].Text = reply.RoundtripTime + "ms";
                                        }
                                        else
                                        {
                                            ServerListRenderer.Items[serverid].SubItems[5].Text = "---";
                                        }
                                    };
                                }
                                else
                                {
                                    ServerListRenderer.Items[serverid].SubItems[5].Text = "N/A";
                                }
                            }
                        }
                        catch
                        {
                            ServerListRenderer.Items[serverid].SubItems[1].Text = servername;
                            ServerListRenderer.Items[serverid].SubItems[2].Text = "---";
                            ServerListRenderer.Items[serverid].SubItems[3].Text = "---";
                            ServerListRenderer.Items[serverid].SubItems[4].Text = "---";
                            ServerListRenderer.Items[serverid].SubItems[5].Text = "---";
                        }

                        if (servers.Count == 0)
                        {
                            Loading.Text = "";
                        }

                        Application.DoEvents();
                    }
                });
            }) { IsBackground = true };
            newList.Start();

            ServerListRenderer.AllowColumnReorder = false;
            ServerListRenderer.ColumnWidthChanging += (handler, args) =>
            {
                args.Cancel = true;
                args.NewWidth = ServerListRenderer.Columns[args.ColumnIndex].Width;
            };

            ServerListRenderer.DoubleClick += new EventHandler((handler, args) =>
            {
                SelectedGameServerToRemember();
            });
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            var MainFontSize = 9f * 100f / CreateGraphics().DpiY;
            //var SecondaryFontSize = 8f * 100f / CreateGraphics().DpiY;
            //var ThirdFontSize = 10f * 100f / CreateGraphics().DpiY;
            //var FourthFontSize = 14f * 100f / CreateGraphics().DpiY;

            if (DetectLinux.LinuxDetected())
            {
                MainFontSize = 9f;
                //SecondaryFontSize = 8f;
                //ThirdFontSize = 10f;
                //FourthFontSize = 14f;
            }
            Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ServerListRenderer.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            Loading.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            BtnAddServer.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            BtnSelectServer.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            BtnClose.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            Version.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            ForeColor = Theming.WinFormTextForeColor;
            BackColor = Theming.WinFormTBGForeColor;

            Loading.ForeColor = Theming.WinFormWarningTextForeColor;
            Version.ForeColor = Theming.WinFormTextForeColor;

            ServerListRenderer.ForeColor = Theming.WinFormSecondaryTextForeColor;

            BtnAddServer.ForeColor = Theming.BlueForeColorButton;
            BtnAddServer.BackColor = Theming.BlueBackColorButton;
            BtnAddServer.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            BtnAddServer.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            BtnSelectServer.ForeColor = Theming.BlueForeColorButton;
            BtnSelectServer.BackColor = Theming.BlueBackColorButton;
            BtnSelectServer.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            BtnSelectServer.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            BtnClose.ForeColor = Theming.BlueForeColorButton;
            BtnClose.BackColor = Theming.BlueBackColorButton;
            BtnClose.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            BtnClose.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;
        }

        private void BtnAddServer_Click(object sender, EventArgs e)
        {
            new AddServer().Show();
        }

        private void BtnSelectServer_Click(object sender, EventArgs e)
        {
            SelectedGameServerToRemember();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SelectedGameServerToRemember()
        {
            if (ServerListRenderer.SelectedItems.Count == 1)
            {
                rememberServerInformationID.TryGetValue(ServerListRenderer.SelectedIndices[0], out ServerInfo);

                MainScreen.ServerName = data[ServerListRenderer.SelectedIndices[0] + 1];

                this.Close();
            }
        }
    }
}
