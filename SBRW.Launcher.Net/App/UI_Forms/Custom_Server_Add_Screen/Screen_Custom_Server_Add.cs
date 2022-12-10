using Newtonsoft.Json;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.ModNet.JSON;
using SBRW.Launcher.RunTime.LauncherCore.Support;
using SBRW.Launcher.RunTime.SystemPlatform.Unix;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.String_;
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
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SBRW.Launcher.App.UI_Forms.Custom_Server_Add_Screen
{
    public partial class Screen_Custom_Server_Add : Form
    {
        private static bool IsAddServerOpen { get; set; }

        public void DrawErrorAroundTextBox(TextBox x)
        {
            if (ActiveForm != null)
            {
                if (!ActiveForm.IsDisposed)
                {
                    x.BorderStyle = BorderStyle.Fixed3D;
                    Pen p = new Pen(Color_Winform.Error_Text_Fore_Color);
                    Graphics g = this.CreateGraphics();
                    int variance = 1;
                    g.DrawRectangle(p, new Rectangle(x.Location.X - variance, x.Location.Y - variance, x.Width + variance, x.Height + variance));
                }
            }
        }

        private async void Button_Add_Click(object sender, EventArgs e)
        {
            Label_Alert.Visible = false;
            this.Refresh();
            if (string.IsNullOrWhiteSpace(TextBox_Server_Address.Text) || string.IsNullOrWhiteSpace(Textbox_Server_Name.Text.Encode_UTF8()))
            {
                if (string.IsNullOrWhiteSpace(TextBox_Server_Address.Text))
                {
                    Label_Alert.Text = "Fix Empty IP";
                    DrawErrorAroundTextBox(TextBox_Server_Address);
                }
                else
                {
                    Label_Alert.Text = "Fix Empty Name";
                    DrawErrorAroundTextBox(Textbox_Server_Name);
                }

                Label_Alert.Visible = true;
                return;
            }

            if (Label_Alert.Visible)
            {
                Label_Alert.Visible = false;
            }

            bool CorrectFormat = Uri.TryCreate(TextBox_Server_Address.Text, UriKind.Absolute, out Uri? Result) && (Result.Scheme == Uri.UriSchemeHttp || Result.Scheme == Uri.UriSchemeHttps);

            string FormattedURL;
            if (!CorrectFormat || Result == null)
            {
                DrawErrorAroundTextBox(TextBox_Server_Address);
                return;
            }
            else
            {
                FormattedURL = Result.ToString();
            }

            ButtonControls(false);
            await Task.Run(() =>
            {
                try
                {
                    string ServerInfomationJSON = string.Empty;
                    try
                    {
                        Uri StringToUri = new Uri(FormattedURL + "/GetServerInformation");
                        ServicePointManager.FindServicePoint(StringToUri).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                        Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
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
                            ServerInfomationJSON = Client.DownloadString(StringToUri);
                        }
                        catch (Exception Error)
                        {
                            string LogMessage = "Add Server Check Encountered an Error:";
                            LogToFileAddons.OpenLog("Add Server", LogMessage, Error, string.Empty, false);
                        }
                        finally
                        {
                            Client?.Dispose();
                        }
                    }
                    catch (Exception Error)
                    {
                        string LogMessage = "Add Server Check Encountered an Error:";
                        LogToFileAddons.OpenLog("Add Server", LogMessage, Error, string.Empty, false);
                    }

                    if (string.IsNullOrWhiteSpace(ServerInfomationJSON))
                    {
                        ButtonControls(true);
                        return;
                    }
                    else if (!ServerInfomationJSON.Valid_Json())
                    {
                        Label_Alert.Text = "Unstable Connection";
                        DrawErrorAroundTextBox(TextBox_Server_Address);
                        Label_Alert.Visible = true;
                        ButtonControls(true);
                        ServerInfomationJSON = string.Empty;
                        return;
                    }
                    else
                    {
                        Json_Server_Info? ServerInformationData = null;

                        try
                        {
                            ServerInformationData = JsonConvert.DeserializeObject<Json_Server_Info>(ServerInfomationJSON);
                        }
                        catch (Exception Error)
                        {
                            string LogMessage = "Add Server Get Information Encountered an Error:";
                            LogToFileAddons.OpenLog("Add Server", LogMessage, Error, string.Empty, false);
                        }

                        if (ServerInformationData == null)
                        {
                            ButtonControls(true);
                            ServerInfomationJSON = string.Empty;
                            return;
                        }
                        else
                        {
                            string ServerID = string.Empty;
                            Uri newModNetUri = new Uri(FormattedURL + "/Modding/GetModInfo");
                            ServicePointManager.FindServicePoint(newModNetUri).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                        Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
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
                                GetModInfo? ServerGetInfo = JsonConvert.DeserializeObject<GetModInfo>(Client.DownloadString(newModNetUri));
                                ServerID = (ServerGetInfo != null) ? string.IsNullOrWhiteSpace(ServerGetInfo.serverID) ?
                                    Result.Host : ServerGetInfo.serverID : Result.Host;
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("Add Server", string.Empty, Error, string.Empty, true);
                                ServerID = Result.Host;
                            }
                            finally
                            {
                                Client?.Dispose();
                            }

                            try
                            {
                                StreamReader sr = new StreamReader(Locations.LauncherCustomServers);
                                string oldcontent = sr.ReadToEnd();
                                sr.Close();

                                if (string.IsNullOrWhiteSpace(oldcontent))
                                {
                                    oldcontent = "[]";
                                }

                                var Servers = JsonConvert.DeserializeObject<List<Json_List_Server>>(oldcontent);

                                if (Servers != null)
                                {
                                    Servers.Add(new Json_List_Server
                                    {
                                        Name = Textbox_Server_Name.Text.Encode_UTF8(),
                                        IPAddress = FormattedURL,
                                        IsSpecial = false,
                                        ID = ServerID,
                                        Category = string.IsNullOrWhiteSpace(TextBox_Server_Category.Text.Encode_UTF8()) ? "Custom" : TextBox_Server_Category.Text.Encode_UTF8()
                                    });

                                    File.WriteAllText(Locations.LauncherCustomServers, JsonConvert.SerializeObject(Servers));

                                    MessageBox.Show(null, "The New server will be added on the next start of the Launcher.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            catch (Exception Error)
                            {
                                string LogMessage = "Failed to Add New Server:";
                                LogToFileAddons.OpenLog("Add Server", LogMessage, Error, string.Empty, false);
                                ButtonControls(true);
                                return;
                            }
                            finally
                            {
                                if (!string.IsNullOrWhiteSpace(ServerID))
                                {
                                    ServerID = string.Empty;
                                }

                                if (!string.IsNullOrWhiteSpace(ServerInfomationJSON))
                                {
                                    ServerInfomationJSON = string.Empty;
                                }
                            }

                            Button_Cancel_Click(sender, e);
                        }
                    }
                }
                catch
                {
                    DrawErrorAroundTextBox(TextBox_Server_Address);
                    ButtonControls(true);
                }
            });
        }

        private void ButtonControls(bool Enable)
        {
            if (ActiveForm != null)
            {
                if (!ActiveForm.IsDisposed)
                {
                    Button_Cancel.Enabled = Enable;
                    Button_Add.Enabled = Enable;
                    TextBox_Server_Address.Enabled = Enable;
                    Textbox_Server_Name.Enabled = Enable;
                }
            }
        }

        private void Button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void TextBox_Server_Category_ShowPlaceHolderText(object sender, EventArgs e)
        {
            if (ActiveForm != null)
            {
                if (!ActiveForm.IsDisposed)
                {
                    if (string.IsNullOrWhiteSpace(TextBox_Server_Category.Text.Encode_UTF8()))
                    {
                        TextBox_Server_Category.Text = "Custom";
                    }
                }
            }
        }

        public void TextBox_Server_Category_RemovePlaceHolderText(object sender, EventArgs e)
        {
            if (ActiveForm != null)
            {
                if (!ActiveForm.IsDisposed)
                {
                    if (TextBox_Server_Category.Text == "Custom")
                    {
                        TextBox_Server_Category.Text = string.Empty;
                    }
                }
            }
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Icon = FormsIcon.Retrive_Icon();
            Text = "Add Server - SBRW Launcher";

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            Label_Version.Text = "Version: " + Application.ProductVersion;

            /*******************************/
            /* Set Font                     /
            /*******************************/

#if !(RELEASE_UNIX || DEBUG_UNIX)
            float MainFontSize = 9f * 96f / CreateGraphics().DpiY;
#else
            float MainFontSize = 9f;
#endif
            Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);

            Button_Add.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Cancel.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Server_Name.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Textbox_Server_Name.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Server_Address.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            TextBox_Server_Address.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Server_Category.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            TextBox_Server_Category.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Alert.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Version.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors              /
            /********************************/

            BackColor = Color_Winform.BG_Fore_Color;
            ForeColor = Color_Winform.Text_Fore_Color;

            Textbox_Server_Name.BackColor = Color_Winform.BG_Darker_Fore_Color;
            Textbox_Server_Name.ForeColor = Color_Winform.Secondary_Text_Fore_Color;

            TextBox_Server_Address.BackColor = Color_Winform.BG_Darker_Fore_Color;
            TextBox_Server_Address.ForeColor = Color_Winform.Secondary_Text_Fore_Color;

            TextBox_Server_Category.BackColor = Color_Winform.BG_Darker_Fore_Color;
            TextBox_Server_Category.ForeColor = Color_Winform.Secondary_Text_Fore_Color;

            Label_Alert.ForeColor = Color_Winform.Warning_Text_Fore_Color;

            Label_Server_Name.ForeColor = Color_Winform.Text_Fore_Color;
            Label_Server_Address.ForeColor = Color_Winform.Text_Fore_Color;
            Label_Server_Category.ForeColor = Color_Winform.Text_Fore_Color;
            Label_Version.ForeColor = Color_Winform.Text_Fore_Color;

            Button_Cancel.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Cancel.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Cancel.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Cancel.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            Button_Add.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Add.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Add.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Add.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            /********************************/
            /* Events                        /
            /********************************/

            TextBox_Server_Category.GotFocus += new EventHandler(TextBox_Server_Category_RemovePlaceHolderText);
            TextBox_Server_Category.LostFocus += new EventHandler(TextBox_Server_Category_ShowPlaceHolderText);
            Button_Cancel.Click += new EventHandler(Button_Cancel_Click);
            Button_Add.Click += new EventHandler(Button_Add_Click);

            Shown += (x, y) =>
            {
                Application.OpenForms[this.Name].Activate();
                this.BringToFront();
            };
        }

        /// <summary>Opens the Form. If the Form is already Open it will Give it Foucus</summary>
        public static void OpenScreen()
        {
            if (IsAddServerOpen || Application.OpenForms["Screen_Custom_Server_Add"] != null)
            {
                Application.OpenForms["Screen_Custom_Server_Add"]?.Activate();
            }
            else
            {
                try { new Screen_Custom_Server_Add().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Add Server Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Add Server Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public Screen_Custom_Server_Add()
        {
            IsAddServerOpen = true;
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, CloseForm) =>
            {
                IsAddServerOpen = false;
                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            };
        }
    }
}
