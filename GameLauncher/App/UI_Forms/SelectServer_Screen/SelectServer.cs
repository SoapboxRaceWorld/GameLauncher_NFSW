using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Validator.JSON;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using GameLauncher.App.UI_Forms.AddServer_Screen;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GameLauncher.App.UI_Forms.SelectServer_Screen
{
    public partial class SelectServer : Form
    {
        private static bool IsSelectServerOpen = false;
        private static bool CustomServersOnly = false;
        private static int ID = 1;
        private static readonly Dictionary<int, ServerList> ServerListBook = new Dictionary<int, ServerList>();

        /* Used to ping the Server in ms */
        public static Queue<string> ServersToPing = new Queue<string>();

        public static string ServerName;
        public static GetServerInformation ServerJsonData;

        public static void OpenScreen(bool CSO)
        {
            if (IsSelectServerOpen || Application.OpenForms["SelectServer"] != null)
            {
                if (Application.OpenForms["SelectServer"] != null) { Application.OpenForms["SelectServer"].Activate(); }
            }
            else
            {
                try
                {
                    CustomServersOnly = CSO;
                    new SelectServer().ShowDialog();
                }
                catch (Exception Error)
                {
                    string ErrorMessage = "Select Server Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Select Server", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public SelectServer()
        {
            IsSelectServerOpen = true;
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, y) =>
            {
                ID = 1;
                ServerListBook.Clear();
                ServersToPing.Clear();
                ServerName = null;
                ServerJsonData = null;
                CustomServersOnly = false;
                GC.Collect();
            };
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            var MainFontSize = 9f * 96f / CreateGraphics().DpiY;

            if (UnixOS.Detected())
            {
                MainFontSize = 9f;
            }

            Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ServerListRenderer.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            Loading.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            BtnAddServer.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            BtnRemoveServer.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
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

            BtnRemoveServer.ForeColor = CustomServersOnly ? Theming.BlueForeColorButton : Theming.GrayForeColorButton;
            BtnRemoveServer.BackColor = CustomServersOnly ? Theming.BlueBackColorButton : Theming.GrayBackColorButton;
            BtnRemoveServer.FlatAppearance.BorderColor = CustomServersOnly ? Theming.BlueBorderColorButton : Theming.GrayBorderColorButton;
            BtnRemoveServer.FlatAppearance.MouseOverBackColor = CustomServersOnly ? Theming.BlueMouseOverBackColorButton : Theming.GrayMouseOverBackColorButton;

            BtnSelectServer.ForeColor = !CustomServersOnly ? Theming.BlueForeColorButton : Theming.GrayForeColorButton;
            BtnSelectServer.BackColor = !CustomServersOnly ? Theming.BlueBackColorButton : Theming.GrayBackColorButton;
            BtnSelectServer.FlatAppearance.BorderColor = !CustomServersOnly ? Theming.BlueBorderColorButton : Theming.GrayBorderColorButton;
            BtnSelectServer.FlatAppearance.MouseOverBackColor = !CustomServersOnly ? Theming.BlueMouseOverBackColorButton : Theming.GrayMouseOverBackColorButton;

            BtnClose.ForeColor = Theming.BlueForeColorButton;
            BtnClose.BackColor = Theming.BlueBackColorButton;
            BtnClose.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            BtnClose.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            /********************************/
            /* Functions                     /
            /********************************/

            Name = (CustomServersOnly ? "Saved Custom Servers" : "Please Select a Server") + " - SBRW Launcher";
            ServerListRenderer.AllowColumnReorder = false;
            ServerListRenderer.ColumnWidthChanging += (handler, args) =>
            {
                args.Cancel = true;
                args.NewWidth = ServerListRenderer.Columns[args.ColumnIndex].Width;
            };
            ServerListRenderer.DoubleClick += new EventHandler((handler, args) =>
            {
                if (!CustomServersOnly)
                {
                    SelectedGameServerToRemember();
                }
            });
            BtnSelectServer.Click += new EventHandler(BtnSelectServer_Click);
            BtnRemoveServer.Click += new EventHandler(BtnRemoveServer_Click);

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

            foreach (ServerList substring in CustomServersOnly ? ServerListUpdater.NoCategoryList_CSO : ServerListUpdater.NoCategoryList)
            {
                try
                {
                    ServersToPing.Enqueue(ID + "_|||_" + substring.IPAddress + "_|||_" + substring.Name);

                    ServerListRenderer.Items.Add(new ListViewItem(
                        new[]
                        {
                                ID.ToString(), substring.Name, "", "", "", "", ""
                        }
                    ));

                    ServerListBook.Add(ID, substring);
                    ID++;
                }
                catch { }
            }

            Shown += (x, y) =>
            {
                Application.OpenForms[this.Name].Activate();
                this.BringToFront();

                new Thread(() =>
                {
                    while (ServersToPing.Count != 0)
                    {
                        string QueueContent = ServersToPing.Dequeue();
                        string[] QueueContent2 = QueueContent.Split(new string[] { "_|||_" }, StringSplitOptions.None);

                        int serverid = Convert.ToInt32(QueueContent2[0]) - 1;
                        string serverurl = QueueContent2[1] + "/GetServerInformation";
                        ServerName = QueueContent2[2];
                        string ServerJson = null;

                        try
                        {
                            try
                            {
                                Uri URLCall = new Uri(serverurl);
                                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                var Client = new WebClient
                                {
                                    Encoding = Encoding.UTF8
                                };

                                if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
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
                                ServerListRenderer.SafeInvokeAction(() =>
                                {
                                    ServerListRenderer.Items[serverid].SubItems[1].Text = ServerName;
                                    ServerListRenderer.Items[serverid].SubItems[2].Text = "---";
                                    ServerListRenderer.Items[serverid].SubItems[3].Text = "---";
                                    ServerListRenderer.Items[serverid].SubItems[4].Text = "---";
                                    ServerListRenderer.Items[serverid].SubItems[5].Text = "---";
                                }, this);
                            }
                            else if (!IsJSONValid.ValidJson(ServerJson))
                            {
                                ServerListRenderer.SafeInvokeAction(() =>
                                {
                                    ServerListRenderer.Items[serverid].SubItems[1].Text = ServerName;
                                    ServerListRenderer.Items[serverid].SubItems[2].Text = "-?-";
                                    ServerListRenderer.Items[serverid].SubItems[3].Text = "-?-";
                                    ServerListRenderer.Items[serverid].SubItems[4].Text = "-?-";
                                    ServerListRenderer.Items[serverid].SubItems[5].Text = "-?-";
                                }, this);
                            }
                            else
                            {
                                ServerJsonData = JsonConvert.DeserializeObject<GetServerInformation>(ServerJson);

                                ServerListRenderer.SafeInvokeAction(() =>
                                {
                                    ServerListRenderer.Items[serverid].SubItems[1].Text = (!string.IsNullOrWhiteSpace(ServerJsonData.serverName)) ?
                                        ServerJsonData.serverName : ServerName;
                                    ServerListRenderer.Items[serverid].SubItems[2].Text = ServerListUpdater.CountryName(ServerJsonData.country.ToString());
                                    ServerListRenderer.Items[serverid].SubItems[3].Text = ServerJsonData.onlineNumber.ToString();
                                    ServerListRenderer.Items[serverid].SubItems[4].Text = ServerJsonData.numberOfRegistered.ToString();
                                }, this);

                                Ping CheckMate = null;

                                try
                                {
                                    Uri StringToUri = new Uri(serverurl);
                                    ServicePointManager.FindServicePoint(StringToUri).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                    CheckMate = new Ping();
                                    CheckMate.PingCompleted += (sender3, e3) =>
                                    {
                                        if (e3.Reply != null)
                                        {
                                            if (e3.Reply.Status == IPStatus.Success && ServerName != "Offline Built-In Server")
                                            {
                                                ServerListRenderer.SafeInvokeAction(() =>
                                                {
                                                    ServerListRenderer.Items[serverid].SubItems[5].Text = e3.Reply.RoundtripTime + "ms";
                                                }, this);
                                            }
                                            else
                                            {
                                                ServerListRenderer.SafeInvokeAction(() =>
                                                {
                                                    ServerListRenderer.Items[serverid].SubItems[5].Text = "!?";
                                                }, this);
                                            }
                                        }
                                        else
                                        {
                                            ServerListRenderer.SafeInvokeAction(() =>
                                            {
                                                ServerListRenderer.Items[serverid].SubItems[5].Text = "N/A";
                                            }, this);
                                        }

                                        ((AutoResetEvent)e3.UserState).Set();
                                    };
                                    CheckMate.SendAsync(StringToUri.Host, 5000, new byte[1], new PingOptions(30, true), new AutoResetEvent(false));
                                }
                                catch
                                {
                                    ServerListRenderer.SafeInvokeAction(() =>
                                    {
                                        ServerListRenderer.Items[serverid].SubItems[5].Text = "?";
                                    }, this);
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
                            ServerListRenderer.SafeInvokeAction(() =>
                            {
                                ServerListRenderer.Items[serverid].SubItems[1].Text = ServerName;
                                ServerListRenderer.Items[serverid].SubItems[2].Text = "---";
                                ServerListRenderer.Items[serverid].SubItems[3].Text = "---";
                                ServerListRenderer.Items[serverid].SubItems[4].Text = "---";
                                ServerListRenderer.Items[serverid].SubItems[5].Text = "---";
                            }, this);
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

                            GC.Collect();
                        }

                        Application.DoEvents();
                    }

                    Loading.SafeInvokeAction(() =>
                    {
                        Loading.Text = string.Empty;
                    }, this);
                }).Start();
            };
        }

        private void BtnAddServer_Click(object sender, EventArgs e)
        {
            AddServer.OpenScreen();
        }

        private void BtnSelectServer_Click(object sender, EventArgs e)
        {
            if (!CustomServersOnly)
            {
                SelectedGameServerToRemember();
            }
        }

        private void BtnRemoveServer_Click(object sender, EventArgs e)
        {
            SelectedGameServerToRemove();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SelectedGameServerToRemember()
        {
            if (ServerListRenderer.SelectedItems.Count == 1)
            {
                SelectedServer.Data = ServerListBook[ServerListRenderer.SelectedIndices[0] + 1];
                this.Close();
            }
        }

        private void SelectedGameServerToRemove()
        {
            if (ServerListRenderer.SelectedItems.Count == 1)
            {
                try
                {
                    if (ServerListUpdater.NoCategoryList_CSO.Count > 0)
                    {
                        if (MessageBox.Show(null, "Confirm to Remove " + ServerListBook[ServerListRenderer.SelectedIndices[0] + 1].Name + " from Saved Custom Servers",
                            "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            ServerListUpdater.NoCategoryList_CSO.RemoveAt(ServerListRenderer.SelectedIndices[0]);
                            ServerListRenderer.Items.Remove(ServerListRenderer.SelectedItems[0]);
                            File.WriteAllText(Locations.LauncherCustomServers, JsonConvert.SerializeObject(ServerListUpdater.NoCategoryList_CSO));
                            Application.DoEvents();
                            GC.Collect();
                        }
                    }
                }
                catch (Exception Error)
                {
                    string LogMessage = "Failed to Remove Selected Server:";
                    LogToFileAddons.OpenLog("Add Server", LogMessage, Error, null, false);
                }
            }
        }
    }
}
