using Newtonsoft.Json;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Lists;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.Support;
using SBRW.Launcher.RunTime.SystemPlatform.Unix;
using SBRW.Launcher.App.UI_Forms.Custom_Server_Add_Screen;
using SBRW.Launcher.App.UI_Forms.Settings_Screen;
using SBRW.Launcher.App.UI_Forms.Welcome_Screen;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Api_;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SBRW.Launcher.App.UI_Forms.Selection_CDN_Screen
{
    public partial class Screen_CDN_Selection : Form
    {
        private static bool IsSelectServerOpen { get; set; }
        private static int ID { get; set; } = 1;
        private static readonly Dictionary<int, Json_List_CDN> ServerListBook = new Dictionary<int, Json_List_CDN>();
        private static int Screen_Mode_Update { get; set; }
        /* Used to ping the Server in ms */
        public static Queue<string> ServersToPing { get; set; } = new Queue<string>();

        public static void OpenScreen(int Screen_Mode = 0)
        {
            if (IsSelectServerOpen || Application.OpenForms["Screen_CDN_Selection"] != null)
            {
                Application.OpenForms["Screen_CDN_Selection"]?.Activate();
            }
            else
            {
                try
                {
                    Screen_Mode_Update = Screen_Mode;
                    new Screen_CDN_Selection().ShowDialog();
                }
                catch (Exception Error)
                {
                    string ErrorMessage = "Select CDN Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Select CDN", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        private async void Screen_Custom_Server_Shown(object sender, EventArgs e) 
        {
            if (e != null)
            {
                Application.OpenForms[this.Name].Activate();
                this.BringToFront();

                ListView_Server_List.Columns.Add("");
                ListView_Server_List.Columns[0].Width = 1;

                ListView_Server_List.Columns.Add("Name");
                ListView_Server_List.Columns[1].Width = 240;

                ListView_Server_List.Columns.Add("LZMA");
                ListView_Server_List.Columns[2].Width = 60;
                ListView_Server_List.Columns[2].TextAlign = HorizontalAlignment.Center;

                ListView_Server_List.Columns.Add("Verify Hash");
                ListView_Server_List.Columns[3].Width = 85;
                ListView_Server_List.Columns[3].TextAlign = HorizontalAlignment.Center;

                ListView_Server_List.Columns.Add("Game Pack");
                ListView_Server_List.Columns[4].Width = 85;
                ListView_Server_List.Columns[4].TextAlign = HorizontalAlignment.Center;

                ListView_Server_List.Columns.Add("Ping");
                ListView_Server_List.Columns[5].Width = 60;
                ListView_Server_List.Columns[5].TextAlign = HorizontalAlignment.Center;

                foreach (Json_List_CDN substring in CDNListUpdater.CleanList)
                {
                    try
                    {
                        ServersToPing.Enqueue(ID + "_|||_" + substring.Url + "_|||_" + substring.Name);

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

                try
                {
                    await Task.Run(() =>
                    {
                        if (IsSelectServerOpen)
                        {
                            while ((ServersToPing.Count != 0) && IsSelectServerOpen)
                            {
                                if (!IsSelectServerOpen)
                                {
                                    break;
                                }
                                else
                                {
                                    string QueueContent = ServersToPing.Dequeue();
                                    string[] Queue_Local_Server_Info = QueueContent.Split(new string[] { "_|||_" }, StringSplitOptions.None);

                                    int serverid = Convert.ToInt32(Queue_Local_Server_Info[0]) - 1;
                                    string serverurl = Queue_Local_Server_Info[1];

                                    try
                                    {
                                        switch (API_Core.StatusCheck(serverurl + "/en/index.xml", 10))
                                        {
                                            case APIStatus.Online:
                                                ListView_Server_List.Items[serverid].SubItems[2].Text = "Yes";
                                                break;
                                            case APIStatus.Forbidden:
                                            case APIStatus.NotFound:
                                                ListView_Server_List.Items[serverid].SubItems[2].Text = "No";
                                                break;
                                            default:
                                                ListView_Server_List.Items[serverid].SubItems[2].Text = "---";
                                                break;
                                        }

                                        switch (API_Core.StatusCheck(serverurl + "/unpacked/checksums.dat", 10))
                                        {
                                            case APIStatus.Online:
                                                ListView_Server_List.Items[serverid].SubItems[3].Text = "Yes";
                                                break;
                                            case APIStatus.Forbidden:
                                            case APIStatus.NotFound:
                                                ListView_Server_List.Items[serverid].SubItems[3].Text = "No";
                                                break;
                                            default:
                                                ListView_Server_List.Items[serverid].SubItems[3].Text = "---";
                                                break;
                                        }

                                        switch (API_Core.StatusCheck(serverurl + "/GameFiles.sbrwpack", 10))
                                        {
                                            case APIStatus.Online:
                                                ListView_Server_List.Items[serverid].SubItems[4].Text = "Yes";
                                                break;
                                            case APIStatus.Forbidden:
                                            case APIStatus.NotFound:
                                                ListView_Server_List.Items[serverid].SubItems[4].Text = "No";
                                                break;
                                            default:
                                                ListView_Server_List.Items[serverid].SubItems[4].Text = "---";
                                                break;
                                        }

                                        Ping? CheckMate = null;

                                        try
                                        {
                                            Uri StringToUri = new Uri(serverurl);
                                            ServicePointManager.FindServicePoint(StringToUri).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                                    Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
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
                    });
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("New Download Game Files", string.Empty, Error, string.Empty, true);
                }
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

            Button_Server_Remove.ForeColor = Color_Winform_Buttons.Gray_Fore_Color;
            Button_Server_Remove.BackColor = Color_Winform_Buttons.Gray_Back_Color;
            Button_Server_Remove.FlatAppearance.BorderColor = Color_Winform_Buttons.Green_Border_Color;
            Button_Server_Remove.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Gray_Mouse_Over_Back_Color;

            Button_Server_Select.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Server_Select.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Server_Select.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Server_Select.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            Button_Close.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Close.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Close.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Close.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            /********************************/
            /* Functions                     /
            /********************************/

            Icon = FormsIcon.Retrive_Icon();
            Text = "Please Select a CDN - SBRW Launcher";

            ListView_Server_List.AllowColumnReorder = false;
            ListView_Server_List.ColumnWidthChanging += (handler, args) =>
            {
                args.Cancel = true;
                args.NewWidth = ListView_Server_List.Columns[args.ColumnIndex].Width;
            };

            ListView_Server_List.DoubleClick += new EventHandler((handler, args) =>
            {
                SelectedGameServerToRemember();
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
            SelectedGameServerToRemember();
        }

        private void Button_Server_Remove_Click(object sender, EventArgs e)
        {
            SelectedGameServerToRemove();
        }

        private void Button_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SelectedGameServerToRemember()
        {
            if (ListView_Server_List.SelectedItems.Count == 1)
            {
                Json_List_CDN Selected_CDN = ServerListBook[ListView_Server_List.SelectedIndices[0] + 1];
                if (string.IsNullOrWhiteSpace(Selected_CDN.Url))
                {
                    switch (Screen_Mode_Update)
                    {
                        case 1:
                            MessageBox.Show(Screen_Welcome.Screen_Instance, "Selected CDN does not have a Valid URL. Please Choose Another CDN.",
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case 2:
                            MessageBox.Show(Screen_Settings.Screen_Instance, "Selected CDN does not have a Valid URL. Please Choose Another CDN.",
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        default:
                            MessageBox.Show(null, "Selected CDN does not have a Valid URL. Please Choose Another CDN.",
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                    }
                }
                else if (Selected_CDN.Url.StartsWith("https://") || Selected_CDN.Url.StartsWith("http://"))
                {
                    switch (Screen_Mode_Update)
                    {
                        case 1:
                            LogToFileAddons.Parent_Log_Screen(0, "LAUNCHER", "Selected CDN: " + Selected_CDN.Name);
                            Save_Settings.Live_Data.Launcher_CDN = Selected_CDN.Url.EndsWith("/") ? Selected_CDN.Url.TrimEnd('/') : Selected_CDN.Url;
                            Save_Settings.Save();
                            Screen_Welcome.Screen_Instance.Button_Save.Visible = true;
                            break;
                        case 2:
                            Screen_Settings.Screen_Instance.Label_CDN_Current.Text = "NEW SELECTED CDN:";
                            Screen_Settings.Screen_Instance.LinkLabel_CDN_Current.Text = Selected_CDN.Url.EndsWith("/") ? Selected_CDN.Url.TrimEnd('/') : Selected_CDN.Url;
                            Screen_Settings.Screen_Instance.New_Choosen_CDN = Selected_CDN.Url.EndsWith("/") ? Selected_CDN.Url.TrimEnd('/') : Selected_CDN.Url;
                            break;
                        default:
                            Save_Settings.Live_Data.Launcher_CDN = Selected_CDN.Url.EndsWith("/") ? Selected_CDN.Url.TrimEnd('/') : Selected_CDN.Url;
                            break;
                    }

                    this.Close();
                }
                else
                {
                    switch (Screen_Mode_Update)
                    {
                        case 1:
                            MessageBox.Show(Screen_Welcome.Screen_Instance, "Selected CDN does not have a Valid URL. Please Choose Another CDN.",
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case 2:
                            MessageBox.Show(Screen_Settings.Screen_Instance, "Selected CDN does not have a Valid URL. Please Choose Another CDN.",
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        default:
                            MessageBox.Show(null, "Selected CDN does not have a Valid URL. Please Choose Another CDN.",
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                    }
                }
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

        public Screen_CDN_Selection()
        {
            IsSelectServerOpen = true;
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, y) =>
            {
                ID = 1;
                ServerListBook.Clear();
                ServersToPing.Clear();
                IsSelectServerOpen = false;
                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            };
        }
    }
}
