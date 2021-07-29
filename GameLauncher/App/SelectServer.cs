using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using System.Text;
using GameLauncher.App.Classes.LauncherCore.Validator.JSON;
using System.Net;
using GameLauncher.App.Classes.LauncherCore.Global;

namespace GameLauncher.App
{
    public partial class SelectServer : Form
    {
        private readonly int ID = 1;
        readonly Dictionary<int, GetServerInformation> rememberServerInformationID = new Dictionary<int, GetServerInformation>();
        public GetServerInformation ServerList;
        readonly Dictionary<int, ServerList> data = new Dictionary<int, ServerList>();

        /* Used to ping the Server in ms */
        public Queue<string> servers = new Queue<string>();

        public static string ServerName;
        public static GetServerInformation ServerJsonData;

        public SelectServer()
        {
            InitializeComponent();
            SetVisuals();

            Version.Text = "Version: v" + Application.ProductVersion;

            /* And one for keeping data about server, IP tbh */
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
                    servers.Enqueue(ID + "_|||_" + substring.IPAddress + "_|||_" + substring.Name);

                    ServerListRenderer.Items.Add(new ListViewItem(
                        new[]
                        {
                                ID.ToString(), substring.Name, "", "", "", "", ""
                        }
                    ));

                    data.Add(ID, substring);
                    ID++;
                }
                catch { }
            }

            Thread newList = new Thread(() =>
            {
                //Thread.Sleep(200);
                this.BeginInvoke((MethodInvoker)delegate
                {
                    while (servers.Count != 0)
                    {
                        string QueueContent = servers.Dequeue();
                        string[] QueueContent2 = QueueContent.Split(new string[] { "_|||_" }, StringSplitOptions.None);

                        int serverid = Convert.ToInt32(QueueContent2[0]) - 1;
                        string serverurl = QueueContent2[1] + "/GetServerInformation";
                        ServerName = QueueContent2[2];
                        string ServerJson = null;

                        try
                        {
                            try
                            {
                                FunctionStatus.TLS();
                                Uri URLCall = new Uri(URLs.GitHub_Launcher);
                                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                var Client = new WebClient
                                {
                                    Encoding = Encoding.UTF8
                                };

                                if (!WebCalls.Alternative) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                                else
                                {
                                    Client.Headers.Add("user-agent", "SBRW Launcher " +
                                    Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                }

                                try
                                {
                                    ServerJson = Client.DownloadString(serverurl);
                                }
                                catch { }
                                finally
                                {
                                    if (Client != null)
                                    {
                                        Client.Dispose();
                                    }
                                }
                            }
                            catch { }

                            if (string.IsNullOrWhiteSpace(ServerJson))
                            {
                                ServerListRenderer.Items[serverid].SubItems[1].Text = ServerName;
                                ServerListRenderer.Items[serverid].SubItems[2].Text = "---";
                                ServerListRenderer.Items[serverid].SubItems[3].Text = "---";
                                ServerListRenderer.Items[serverid].SubItems[4].Text = "---";
                                ServerListRenderer.Items[serverid].SubItems[5].Text = "---";
                            }
                            else if (!IsJSONValid.ValidJson(ServerJson))
                            {
                                ServerListRenderer.Items[serverid].SubItems[1].Text = ServerName;
                                ServerListRenderer.Items[serverid].SubItems[2].Text = "-?-";
                                ServerListRenderer.Items[serverid].SubItems[3].Text = "-?-";
                                ServerListRenderer.Items[serverid].SubItems[4].Text = "-?-";
                                ServerListRenderer.Items[serverid].SubItems[5].Text = "-?-";
                            }
                            else
                            {
                                ServerJsonData = JsonConvert.DeserializeObject<GetServerInformation>(ServerJson);

                                ServerListRenderer.Items[serverid].SubItems[1].Text = (!string.IsNullOrWhiteSpace(ServerJsonData.serverName)) ? ServerJsonData.serverName : ServerName;
                                ServerListRenderer.Items[serverid].SubItems[2].Text = ServerListUpdater.CountryName(ServerJsonData.country.ToString());
                                ServerListRenderer.Items[serverid].SubItems[3].Text = ServerJsonData.onlineNumber.ToString();
                                ServerListRenderer.Items[serverid].SubItems[4].Text = ServerJsonData.numberOfRegistered.ToString();

                                Ping CheckMate = null;

                                try
                                {
                                    Uri StringToUri = new Uri(serverurl);
                                    ServicePointManager.FindServicePoint(StringToUri).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                    CheckMate = new Ping();
                                    CheckMate.PingCompleted += (sender3, e3) => {
                                        if (e3.Reply != null)
                                        {
                                            if (e3.Reply.Status == IPStatus.Success && ServerName != "Offline Built-In Server")
                                            {
                                                ServerListRenderer.Items[serverid].SubItems[5].Text = e3.Reply.RoundtripTime + "ms";
                                            }
                                            else
                                            {
                                                ServerListRenderer.Items[serverid].SubItems[5].Text = "!?";
                                            }
                                        }
                                        else
                                        {
                                            ServerListRenderer.Items[serverid].SubItems[5].Text = "N/A";
                                        }

                                        ((AutoResetEvent)e3.UserState).Set();
                                    };
                                    CheckMate.SendAsync(StringToUri.Host, 5000, new byte[1], new PingOptions(30, true), new AutoResetEvent(false));
                                }
                                catch
                                {
                                    ServerListRenderer.Items[serverid].SubItems[5].Text = "?";
                                }
                                finally
                                {
                                    if (CheckMate != null)
                                    {
                                        CheckMate.Dispose();
                                    }
                                }
                            }
                        }
                        catch
                        {
                            ServerListRenderer.Items[serverid].SubItems[1].Text = ServerName;
                            ServerListRenderer.Items[serverid].SubItems[2].Text = "---";
                            ServerListRenderer.Items[serverid].SubItems[3].Text = "---";
                            ServerListRenderer.Items[serverid].SubItems[4].Text = "---";
                            ServerListRenderer.Items[serverid].SubItems[5].Text = "---";
                        }
                        finally
                        {
                            if (ServerJson != null)
                            {
                                ServerJson = null;
                            }
                            if (ServerJsonData != null)
                            {
                                ServerJsonData = null;
                            }
                            if (ServerName != null)
                            {
                                ServerName = null;
                            }
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

            if (DetectLinux.LinuxDetected())
            {
                MainFontSize = 9f;
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
                rememberServerInformationID.TryGetValue(ServerListRenderer.SelectedIndices[0], out ServerList);

                SelectedServer.Data = data[ServerListRenderer.SelectedIndices[0] + 1];

                this.Close();
            }
        }
    }
}
