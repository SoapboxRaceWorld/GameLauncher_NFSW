using Newtonsoft.Json;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Lists;
using SBRW.Launcher.RunTime.LauncherCore.Lists.JSON;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.Support;
using SBRW.Launcher.RunTime.SystemPlatform.Unix;
using SBRW.Launcher.App.UI_Forms.Custom_Server_Add_Screen;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Validation_.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Custom_Server_Screen
{
    public partial class Screen_Custom_Server : Form
    {
        private static bool IsSelectServerOpen { get; set; }
        private static bool CustomServersOnly { get; set; }
        private static int ID { get; set; } = 1;
        private static readonly Dictionary<int, Json_List_Server> ServerListBook = new Dictionary<int, Json_List_Server>();

        /* Used to ping the Server in ms */
        public static Queue<string> ServersToPing { get; set; } = new Queue<string>();

        public static void OpenScreen(bool CSO)
        {
            if (IsSelectServerOpen || Application.OpenForms["Screen_Custom_Server"] != null)
            {
                Application.OpenForms["Screen_Custom_Server"]?.Activate();
            }
            else
            {
                try
                {
                    CustomServersOnly = CSO;
                    new Screen_Custom_Server().ShowDialog();
                }
                catch (Exception Error)
                {
                    string ErrorMessage = "Select Server Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Select Server", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        private void Screen_Custom_Server_Shown(object sender, EventArgs e) 
        {
            if (e != null)
            {
                Application.OpenForms[this.Name].Activate();
                this.BringToFront();

                ListView_Server_List.Columns.Add("");
                ListView_Server_List.Columns[0].Width = 1;

                ListView_Server_List.Columns.Add("Name");
                ListView_Server_List.Columns[1].Width = 210;

                ListView_Server_List.Columns.Add("Country");
                ListView_Server_List.Columns[2].Width = 100;
                ListView_Server_List.Columns[2].TextAlign = HorizontalAlignment.Center;

                ListView_Server_List.Columns.Add("Online");
                ListView_Server_List.Columns[3].Width = 75;
                ListView_Server_List.Columns[3].TextAlign = HorizontalAlignment.Center;

                ListView_Server_List.Columns.Add("Registered");
                ListView_Server_List.Columns[4].Width = 85;
                ListView_Server_List.Columns[4].TextAlign = HorizontalAlignment.Center;

                ListView_Server_List.Columns.Add("Ping");
                ListView_Server_List.Columns[5].Width = 60;
                ListView_Server_List.Columns[5].TextAlign = HorizontalAlignment.Center;

                foreach (Json_List_Server substring in CustomServersOnly ? ServerListUpdater.NoCategoryList_CSO : ServerListUpdater.NoCategoryList)
                {
                    try
                    {
                        ServersToPing.Enqueue(ID + "_|||_" + substring.IPAddress + "_|||_" + substring.Name);

                        ListView_Server_List.Items.Add(new ListViewItem(
                            new[]
                            {
                                ID.ToString(), substring.Name, "", "", "", "", ""
                            }
                        ));

                        ServerListBook.Add(ID, substring);
                        ID++;
                    }
                    catch { }
                    finally
                    {
                        #if !(RELEASE_UNIX || DEBUG_UNIX) 
                        GC.Collect(); 
                        #endif
                    }
                }

                while ((ServersToPing.Count != 0) && IsSelectServerOpen)
                {
                    string QueueContent = ServersToPing.Dequeue();
                    string[] Queue_Local_Server_Info = QueueContent.Split(new string[] { "_|||_" }, StringSplitOptions.None);

                    int serverid = Convert.ToInt32(Queue_Local_Server_Info[0]) - 1;
                    string serverurl = Queue_Local_Server_Info[1] + "/GetServerInformation";
                    Json_Server_Info? ServerJsonData = null;

                    try
                    {
                        Uri URLCall = new Uri(serverurl);
                        ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                        var Client = new WebClient
                        {
                            Encoding = Encoding.UTF8,
                            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                        };
                        if (!Launcher_Value.Launcher_Alternative_Webcalls())
                        {
                            Client = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) };
                        }
                        else
                        {
                            Client.Headers.Add("user-agent", "SBRW Launcher " +
                            Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        }

                        try
                        {

                            Client.DownloadStringAsync(URLCall);
                            System.Timers.Timer aTimer = new System.Timers.Timer(10000);
                            aTimer.Elapsed += (x, y) =>
                            {
                                Client.CancelAsync();
                                try
                                {
                                    aTimer.Stop();
                                    aTimer.Dispose();
                                }
                                catch
                                {

                                }
                                finally
                                {
                                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                                    GC.Collect(); 
                                    #endif
                                }
                            };
                            aTimer.AutoReset = false;
                            aTimer.Enabled = true;
                            Client.DownloadStringCompleted += (sender2, e2) =>
                            {
                                try
                                {
                                    aTimer.Stop();
                                    aTimer.Dispose();
                                }
                                catch
                                {

                                }
                                finally
                                {
                                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                                    GC.Collect(); 
                                    #endif
                                }

                                if (e2.Cancelled || e2.Error != null || !IsSelectServerOpen)
                                {
                                    if (e2.Error != null)
                                    {
                                        LogToFileAddons.OpenLog("JSON GSI", string.Empty, e2.Error, string.Empty, true);
                                    }
                                    else if (IsSelectServerOpen)
                                    {
                                        ListView_Server_List.SafeInvokeAction(() =>
                                        {
                                            ListView_Server_List.Items[serverid].SubItems[1].Text = Queue_Local_Server_Info[2];
                                            ListView_Server_List.Items[serverid].SubItems[2].Text = "---";
                                            ListView_Server_List.Items[serverid].SubItems[3].Text = "---";
                                            ListView_Server_List.Items[serverid].SubItems[4].Text = "---";
                                            ListView_Server_List.Items[serverid].SubItems[5].Text = "---";
                                        }, this);
                                    }

                                    Client?.Dispose();
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(e2.Result) && IsSelectServerOpen)
                                    {
                                        ListView_Server_List.SafeInvokeAction(() =>
                                        {
                                            ListView_Server_List.Items[serverid].SubItems[1].Text = Queue_Local_Server_Info[2];
                                            ListView_Server_List.Items[serverid].SubItems[2].Text = "---";
                                            ListView_Server_List.Items[serverid].SubItems[3].Text = "---";
                                            ListView_Server_List.Items[serverid].SubItems[4].Text = "---";
                                            ListView_Server_List.Items[serverid].SubItems[5].Text = "---";
                                        }, this);
                                    }
                                    else if (!e2.Result.Valid_Json() && IsSelectServerOpen)
                                    {
                                        ListView_Server_List.SafeInvokeAction(() =>
                                        {
                                            ListView_Server_List.Items[serverid].SubItems[1].Text = Queue_Local_Server_Info[2];
                                            ListView_Server_List.Items[serverid].SubItems[2].Text = "-?-";
                                            ListView_Server_List.Items[serverid].SubItems[3].Text = "-?-";
                                            ListView_Server_List.Items[serverid].SubItems[4].Text = "-?-";
                                            ListView_Server_List.Items[serverid].SubItems[5].Text = "-?-";
                                        }, this);
                                    }
                                    else if (IsSelectServerOpen)
                                    {
                                        try
                                        {
                                            ServerJsonData = JsonConvert.DeserializeObject<Json_Server_Info>(e2.Result);
                                        }
                                        catch
                                        {
                                            ServerJsonData = null;
                                        }
                                        finally
                                        {
                                            #if !(RELEASE_UNIX || DEBUG_UNIX) 
                                            GC.Collect(); 
                                            #endif
                                        }

                                        if (ServerJsonData != null)
                                        {
                                            ListView_Server_List.SafeInvokeAction(() =>
                                            {
                                                ListView_Server_List.Items[serverid].SubItems[1].Text = (!string.IsNullOrWhiteSpace(ServerJsonData.Server_Name)) ?
                                                    ServerJsonData.Server_Name : Queue_Local_Server_Info[2];
                                                ListView_Server_List.Items[serverid].SubItems[2].Text = string.IsNullOrWhiteSpace(ServerJsonData.Server_Allowed_Countries) ?
                                                ServerListUpdater.CountryName(ServerJsonData.Server_Location) : ServerListUpdater.CountryName(ServerJsonData.Server_Allowed_Countries);
                                                ListView_Server_List.Items[serverid].SubItems[3].Text = ServerJsonData.Server_User_Online.ToString();
                                                ListView_Server_List.Items[serverid].SubItems[4].Text = ServerJsonData.Server_User_Registered.ToString();
                                            }, this);

                                            Ping? CheckMate = null;

                                            try
                                            {
                                                Uri StringToUri = new Uri(serverurl);
                                                ServicePointManager.FindServicePoint(StringToUri).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                                CheckMate = new Ping();
                                                CheckMate.PingCompleted += (sender3, e3) =>
                                                {
                                                    if (e3.Reply != null)
                                                    {
                                                        if (e3.Reply.Status == IPStatus.Success && Queue_Local_Server_Info[2] != "Offline Built-In Server")
                                                        {
                                                            ListView_Server_List.SafeInvokeAction(() =>
                                                            {
                                                                ListView_Server_List.Items[serverid].SubItems[5].Text = e3.Reply.RoundtripTime + "ms";
                                                            }, this);
                                                        }
                                                        else
                                                        {
                                                            ListView_Server_List.SafeInvokeAction(() =>
                                                            {
                                                                ListView_Server_List.Items[serverid].SubItems[5].Text = "!?";
                                                            }, this);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ListView_Server_List.SafeInvokeAction(() =>
                                                        {
                                                            ListView_Server_List.Items[serverid].SubItems[5].Text = "N/A";
                                                        }, this);
                                                    }

                                                    if (e3.UserState != null)
                                                    {
                                                        ((AutoResetEvent)e3.UserState).Set();
                                                    }
                                                };
                                                CheckMate.SendAsync(StringToUri.Host, 5000, new byte[1], new PingOptions(30, true), new AutoResetEvent(false));
                                            }
                                            catch
                                            {
                                                ListView_Server_List.SafeInvokeAction(() =>
                                                {
                                                    ListView_Server_List.Items[serverid].SubItems[5].Text = "?";
                                                }, this);
                                            }
                                            finally
                                            {
                                                CheckMate?.Dispose();

                                                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                                                GC.Collect(); 
                                                #endif
                                            }
                                        }
                                        else
                                        {
                                            ListView_Server_List.SafeInvokeAction(() =>
                                            {
                                                ListView_Server_List.Items[serverid].SubItems[1].Text = Queue_Local_Server_Info[2];
                                                ListView_Server_List.Items[serverid].SubItems[2].Text = "NULL";
                                                ListView_Server_List.Items[serverid].SubItems[3].Text = "NULL";
                                                ListView_Server_List.Items[serverid].SubItems[4].Text = "NULL";
                                                ListView_Server_List.Items[serverid].SubItems[5].Text = "NULL";
                                            }, this);
                                        }

                                        Client?.Dispose();
                                    }
                                }
                            };
                        }
                        catch
                        {
                            if (IsSelectServerOpen)
                            {
                                ListView_Server_List.SafeInvokeAction(() =>
                                {
                                    ListView_Server_List.Items[serverid].SubItems[1].Text = Queue_Local_Server_Info[2];
                                    ListView_Server_List.Items[serverid].SubItems[2].Text = "I-E";
                                    ListView_Server_List.Items[serverid].SubItems[3].Text = "I-E";
                                    ListView_Server_List.Items[serverid].SubItems[4].Text = "I-E";
                                    ListView_Server_List.Items[serverid].SubItems[5].Text = "I-E";
                                }, this);
                            }
                        }
                    }
                    catch
                    {
                        if (IsSelectServerOpen)
                        {
                            ListView_Server_List.SafeInvokeAction(() =>
                            {
                                ListView_Server_List.Items[serverid].SubItems[1].Text = Queue_Local_Server_Info[2];
                                ListView_Server_List.Items[serverid].SubItems[2].Text = "I-E";
                                ListView_Server_List.Items[serverid].SubItems[3].Text = "I-E";
                                ListView_Server_List.Items[serverid].SubItems[4].Text = "I-E";
                                ListView_Server_List.Items[serverid].SubItems[5].Text = "I-E";
                            }, this);
                        }
                    }
                    finally
                    {
                        #if !(RELEASE_UNIX || DEBUG_UNIX) 
                        GC.Collect(); 
                        #endif
                    }

                    Application.DoEvents();
                }

                Label_Loading.SafeInvokeAction(() =>
                {
                    Label_Loading.Text = string.Empty;
                }, this);
            }
        }

        #region Theme and Core Runtime Functions
        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/
#if !(RELEASE_UNIX || DEBUG_UNIX)
            float MainFontSize = 9f * 96f / CreateGraphics().DpiY;
#else
            float MainFontSize = 9f;
#endif

            Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            ListView_Server_List.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Loading.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_Server_Add.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Server_Remove.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Server_Select.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Close.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Version.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            ForeColor = Color_Winform.Text_Fore_Color;
            BackColor = Color_Winform.BG_Fore_Color;

            Label_Loading.ForeColor = Color_Winform.Warning_Text_Fore_Color;
            Label_Version.ForeColor = Color_Winform.Text_Fore_Color;

            ListView_Server_List.ForeColor = Color_Winform.Secondary_Text_Fore_Color;
            ListView_Server_List.BackColor = Color_Winform.BG_Darker_Fore_Color;

            Button_Server_Add.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Server_Add.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Server_Add.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Server_Add.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            Button_Server_Remove.ForeColor = CustomServersOnly ? Color_Winform_Buttons.Blue_Fore_Color : Color_Winform_Buttons.Gray_Fore_Color;
            Button_Server_Remove.BackColor = CustomServersOnly ? Color_Winform_Buttons.Blue_Back_Color : Color_Winform_Buttons.Gray_Back_Color;
            Button_Server_Remove.FlatAppearance.BorderColor = CustomServersOnly ? Color_Winform_Buttons.Blue_Border_Color : Color_Winform_Buttons.Green_Border_Color;
            Button_Server_Remove.FlatAppearance.MouseOverBackColor = CustomServersOnly ? Color_Winform_Buttons.Blue_Mouse_Over_Back_Color : Color_Winform_Buttons.Gray_Mouse_Over_Back_Color;

            Button_Server_Select.ForeColor = !CustomServersOnly ? Color_Winform_Buttons.Blue_Fore_Color : Color_Winform_Buttons.Gray_Fore_Color;
            Button_Server_Select.BackColor = !CustomServersOnly ? Color_Winform_Buttons.Blue_Back_Color : Color_Winform_Buttons.Gray_Back_Color;
            Button_Server_Select.FlatAppearance.BorderColor = !CustomServersOnly ? Color_Winform_Buttons.Blue_Border_Color : Color_Winform_Buttons.Gray_Border_Color;
            Button_Server_Select.FlatAppearance.MouseOverBackColor = !CustomServersOnly ? Color_Winform_Buttons.Blue_Mouse_Over_Back_Color : Color_Winform_Buttons.Gray_Mouse_Over_Back_Color;

            Button_Close.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Close.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Close.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Close.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            /********************************/
            /* Functions                     /
            /********************************/

            Icon = FormsIcon.Retrive_Icon();
            Text = (CustomServersOnly ? "Saved Custom Servers" : "Please Select a Server") + " - SBRW Launcher";

            ListView_Server_List.AllowColumnReorder = false;
            ListView_Server_List.ColumnWidthChanging += (handler, args) =>
            {
                args.Cancel = true;
                args.NewWidth = ListView_Server_List.Columns[args.ColumnIndex].Width;
            };

            ListView_Server_List.DoubleClick += new EventHandler((handler, args) =>
            {
                if (!CustomServersOnly)
                {
                    SelectedGameServerToRemember();
                }
            });
            Button_Server_Select.Click += new EventHandler(Button_Server_Select_Click);
            Button_Server_Remove.Click += new EventHandler(Button_Server_Remove_Click);
            Button_Server_Add.Click += new EventHandler(Button_Server_Add_Click);
            Button_Close.Click += new EventHandler(Button_Close_Click);

            Label_Version.Text = "Version: " + Application.ProductVersion;

            /* And one for keeping data about server, IP tbh */
            ListView_Server_List.View = View.Details;
            ListView_Server_List.FullRowSelect = true;

            Shown += new EventHandler(Screen_Custom_Server_Shown);
        }
#endregion

#region Button Functions
        private void Button_Server_Add_Click(object sender, EventArgs e)
        {
            Screen_Custom_Server_Add.OpenScreen();
        }

        private void Button_Server_Select_Click(object sender, EventArgs e)
        {
            if (!CustomServersOnly)
            {
                SelectedGameServerToRemember();
            }
            else
            {
                MessageBox.Show(null, "Click on a Server to Remove it from Your Custom Saved List",
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Button_Server_Remove_Click(object sender, EventArgs e)
        {
            if (CustomServersOnly)
            {
                SelectedGameServerToRemove();
            }
            else
            {
                MessageBox.Show(null, "This feature will Unlocked After This Screen",
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Button_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SelectedGameServerToRemember()
        {
            if (ListView_Server_List.SelectedItems.Count == 1)
            {
                SelectedServer.Data = ServerListBook[ListView_Server_List.SelectedIndices[0] + 1];
                this.Close();
            }
        }

        private void SelectedGameServerToRemove()
        {
            if (ListView_Server_List.SelectedItems.Count == 1)
            {
                try
                {
                    if (ServerListUpdater.NoCategoryList_CSO.Count > 0)
                    {
                        if (MessageBox.Show(null, "Confirm to Remove " + ServerListBook[ListView_Server_List.SelectedIndices[0] + 1].Name + " from Saved Custom Servers",
                            "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            ServerListUpdater.NoCategoryList_CSO.RemoveAt(ListView_Server_List.SelectedIndices[0]);
                            ListView_Server_List.Items.Remove(ListView_Server_List.SelectedItems[0]);
                            File.WriteAllText(Locations.LauncherCustomServers, JsonConvert.SerializeObject(ServerListUpdater.NoCategoryList_CSO));
                            Application.DoEvents();
                            #if !(RELEASE_UNIX || DEBUG_UNIX) 
                            GC.Collect();
                            #endif
                        }
                    }
                }
                catch (Exception Error)
                {
                    string LogMessage = "Failed to Remove Selected Server:";
                    LogToFileAddons.OpenLog("Remove Server", LogMessage, Error, string.Empty, false);
                }
            }
        }
#endregion

        public Screen_Custom_Server()
        {
            IsSelectServerOpen = true;
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, y) =>
            {
                ID = 1;
                ServerListBook.Clear();
                ServersToPing.Clear();
                CustomServersOnly = IsSelectServerOpen = false;
                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            };
        }
    }
}
