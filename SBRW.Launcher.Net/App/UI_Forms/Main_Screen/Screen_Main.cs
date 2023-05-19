using Newtonsoft.Json;
using SBRW.Launcher.RunTime.Auth;
using SBRW.Launcher.RunTime.InsiderKit;
using SBRW.Launcher.RunTime.LauncherCore.APICheckers;
using SBRW.Launcher.RunTime.LauncherCore.Client;
using SBRW.Launcher.RunTime.LauncherCore.Client.Auth;
using SBRW.Launcher.RunTime.LauncherCore.FileReadWrite;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Languages.Visual_Forms;
using SBRW.Launcher.RunTime.LauncherCore.LauncherUpdater;
using SBRW.Launcher.RunTime.LauncherCore.Lists;
using SBRW.Launcher.RunTime.LauncherCore.Lists.JSON;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.ModNet;
using SBRW.Launcher.RunTime.LauncherCore.ModNet.JSON;
using SBRW.Launcher.RunTime.LauncherCore.Support;
using SBRW.Launcher.RunTime.SystemPlatform.Windows;
using SBRW.Launcher.App.UI_Forms.Custom_Server_Screen;
using SBRW.Launcher.App.UI_Forms.SecurityCenter_Screen;
using SBRW.Launcher.App.UI_Forms.Settings_Screen;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Discord.Reference_.List_;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Downloader;
using SBRW.Launcher.Core.Downloader.LZMA;
#if NETFRAMEWORK
using SBRW.Launcher.App.UI_Forms.About_Screen;
#endif
using SBRW.Launcher.Core.Extension.Hash_;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.String_;
using SBRW.Launcher.Core.Extension.Taskbar_;
using SBRW.Launcher.Core.Extension.Time_;
using SBRW.Launcher.Core.Extension.Validation_;
using SBRW.Launcher.Core.Extension.Validation_.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Proxy.Nancy_;
using SBRW.Launcher.Core.Recommended.Process_;
using SBRW.Launcher.Core.Recommended.Time_;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Required.Anti_Cheat;
using SBRW.Launcher.Core.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Net.Cache;
using SBRW.Launcher.Core.Extension.Numbers_;
using SBRW.Launcher.Core.Extension.Api_;
using SBRW.Launcher.App.UI_Forms.Register_Screen;
using SBRW.Launcher.Core.Extra.Reference.System_;
using System.Threading.Tasks;
using SBRW.Launcher.Core.Downloader.Extension_;
using SBRW.Launcher.Core.Downloader.LZMA.Extension_;

namespace SBRW.Launcher.App.UI_Forms.Main_Screen
{
    public partial class Screen_Main : Form
    {
        public static Screen_Main? Screen_Instance { get; set; }
        public static int UI_MODE { get; set; } = 0;
        private static bool LoginEnabled { get; set; }
        private static bool ServerEnabled { get; set; }
        private static bool Builtinserver { get; set; }
        private static bool SkipServerTrigger { get; set; }
        private static bool Playenabled { get; set; }
        private static bool IsDownloading { get; set; } = true;
        private static bool DisableLogout { get; set; }

        public static string GetTempName { get; set; } = Path.GetTempFileName();

        private static int LastSelectedServerId { get; set; }
        public static int NfswPid { get; set; }
        public static Thread? Nfswstarted { get; set; }
        private static bool StillCheckingLastServer { get; set; }

        
        public static Download_LZMA_Data? LZMA_Downloader { get; set; }
        public static Download_Client? Pack_SBRW_Downloader { get; set; }
        public static Download_Extract? Pack_SBRW_Unpacker { get; set; }
        private static Download_Information_ModNet? ModNet_Download_Status { get; set; }
        public static bool Pack_SBRW_Downloader_Unpack_Lock { get; set; }


        private static string JsonGSI { get; set; } = string.Empty;
        private static MemoryStream? ServerRawBanner { get; set; }
        private string LoginWelcomeTime { get; set; } = string.Empty;
        private string LoginToken { get; set; } = string.Empty;
        private string UserId { get; set; } = string.Empty;
        private static int ServerSecondsToShutDown { get; set; }
        
        private static System.Timers.Timer? Live_Action_Timer { get; set; }

        public static string ModNetFileNameInUse { get; set; } = string.Empty;
        public static Queue<Uri> ModFilesDownloadUrls { get; set; } = new Queue<Uri>();
        public static bool IsDownloadingModNetFiles { get; set; }
        public static int CurrentModFileCount { get; set; }
        public static int TotalModFileCount { get; set; }
        public static string Custom_SBRW_Pack { get { return Path.Combine(Locations.LauncherFolder, "GameFiles.sbrwpack"); } }

        private void Server_Ping(string Server_Host_Url, int Ping_Timeout)
        {
            Ping? CheckMate = default;

            try
            {
                if (!(Disposing || IsDisposed))
                {
                    if (!Label_Client_Ping.Equals(string.Empty))
                    {
                        Label_Client_Ping.Text = string.Empty;
                    }
                }

                if(!string.IsNullOrWhiteSpace(Server_Host_Url))
                {
                    Json_List_Server Cached_Server_GSI = Launcher_Value.Launcher_Select_Server_Data;

                    CheckMate = new Ping();
                    CheckMate.PingCompleted += (_sender, _e) =>
                    {
                        if (Cached_Server_GSI.Equals(Launcher_Value.Launcher_Select_Server_Data))
                        {
                            if (_e.Cancelled)
                            {
                                Log.Warning("SERVER PING: Ping Canceled for " + ServerListUpdater.ServerName("Ping"));
                            }
                            else if (_e.Error != null)
                            {
                                Log.Error("SERVER PING: Ping Failed for " + ServerListUpdater.ServerName("Ping") + " -> " + _e.Error.ToString());
                            }
                            else if (_e.Reply != null)
                            {
                                if (_e.Reply.Status == IPStatus.Success && ServerListUpdater.ServerName("Ping") != "Offline Built-In Server")
                                {
                                    if (!(Disposing || IsDisposed))
                                    {
                                        Label_Client_Ping.Text = string.Format("Your Ping to the Server \n{0}".ToUpper(), _e.Reply.RoundtripTime + "ms");
                                    }

                                    Log.Info("SERVER PING: " + _e.Reply.RoundtripTime + "ms for " + ServerListUpdater.ServerName("Ping"));
                                }
                                else
                                {
                                    Log.Warning("SERVER PING: " + ServerListUpdater.ServerName("Ping") + " is " + _e.Reply.Status);
                                }
                            }
                            else
                            {
                                Log.Warning("SERVER PING:  Unable to Ping " + ServerListUpdater.ServerName("Ping"));
                            }

                            if (_e.UserState != null)
                            {
#pragma warning disable CS8602 // Null Safe Check is done Above.
                                (_e.UserState as AutoResetEvent).Set();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                            }
                        }
                    };

                    if (Cached_Server_GSI.Equals(Launcher_Value.Launcher_Select_Server_Data))
                    {
                        CheckMate.SendAsync(Server_Host_Url,
                            (Launcher_Value.Launcher_WebCall_Timeout_Enable && (Launcher_Value.Launcher_WebCall_Timeout() > 0) ?
                                        (Launcher_Value.Launcher_WebCall_Timeout() * 1000) : Ping_Timeout), new byte[1], new PingOptions(30, true), new AutoResetEvent(false));
                    }
                }
            }
            catch (PingException Error)
            {
                LogToFileAddons.OpenLog("Pinging", string.Empty, Error, string.Empty, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Ping", string.Empty, Error, string.Empty, true);
            }
            finally
            {
                if (CheckMate != default)
                {
                    CheckMate.Dispose();
                    CheckMate = null;
                }
            }
        }

        private void ButtonClose_MouseDown(object sender, EventArgs e)
        {
            Button_Close.BackgroundImage = Image_Icon.Close_Click;
        }

        private void ButtonClose_MouseEnter(object sender, EventArgs e)
        {
            Button_Close.BackgroundImage = Image_Icon.Close_Hover;
        }

        private void ButtonClose_MouseLeaveANDMouseUp(object sender, EventArgs e)
        {
            Button_Close.BackgroundImage = Image_Icon.Close;
        }

        private void ButtonSecurityCenter_MouseDown(object sender, EventArgs e)
        {
            Button_Security_Center.BackgroundImage = SecurityCenter.SecurityCenterIcon(1);
        }

        private void ButtonSecurityCenter_MouseEnter(object sender, EventArgs e)
        {
            Button_Security_Center.BackgroundImage = SecurityCenter.SecurityCenterIcon(2);
        }

        private void ButtonSecurityCenter_MouseLeaveANDMouseUp(object sender, EventArgs e)
        {
            Button_Security_Center.BackgroundImage = SecurityCenter.SecurityCenterIcon(0);
        }

        private void ButtonSettings_MouseDown(object sender, EventArgs e)
        {
            Button_Settings.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Good") ? Image_Icon.Gear_Click : Image_Icon.Gear_Warning_Click;
        }

        private void ButtonSettings_MouseEnter(object sender, EventArgs e)
        {
            Button_Settings.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Good") ? Image_Icon.Gear_Hover : Image_Icon.Gear_Warning_Hover;
        }

        private void ButtonSettings_MouseLeaveANDMouseUp(object sender, EventArgs e)
        {
            Button_Settings.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Good") ? Image_Icon.Gear : Image_Icon.Gear_Warning;
        }

        private void Greenbutton_hover_MouseEnter(object sender, EventArgs e)
        {
            Button_Register.BackgroundImage = Image_Button.Green_Hover;
        }

        private void Greenbutton_MouseLeave(object sender, EventArgs e)
        {
            Button_Register.BackgroundImage = Image_Button.Green;
        }

        private void Greenbutton_hover_MouseUp(object sender, EventArgs e)
        {
            Button_Register.BackgroundImage = Image_Button.Green_Hover;
        }

        private void Greenbutton_click_MouseDown(object sender, EventArgs e)
        {
            Button_Register.BackgroundImage = Image_Button.Green_Click;
        }

        private void Email_TextChanged(object sender, EventArgs e)
        {
            Picture_Input_Email.Image = Image_Other.Text_Border_Email;
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            if (Picture_Input_Password.Image != Image_Other.Text_Border_Password)
            {
                Picture_Input_Email.Image = Image_Other.Text_Border_Email;
                Picture_Input_Password.Image = Image_Other.Text_Border_Password;
            }
        }

        private void Graybutton_click_MouseDown(object sender, EventArgs e)
        {
            if (Button_Logout.BackgroundImage != Image_Button.Grey_Click)
            {
                Button_Logout.BackgroundImage = Image_Button.Grey_Click;
            }
        }

        private void Graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            if (Button_Logout.BackgroundImage != Image_Button.Grey_Hover)
            {
                Button_Logout.BackgroundImage = Image_Button.Grey_Hover;
            }
        }

        private void Graybutton_MouseLeave(object sender, EventArgs e)
        {
            if (Button_Logout.BackgroundImage != Image_Button.Grey)
            {
                Button_Logout.BackgroundImage = Image_Button.Grey;
            }
        }

        private void Graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            if (Button_Logout.BackgroundImage != Image_Button.Grey_Hover)
            {
                Button_Logout.BackgroundImage = Image_Button.Grey_Hover;
            }
        }

        private void LoginEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && LoginEnabled)
            {
                LoginButton_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Oem3)
            {
                // Handle key at form level.
                // Do not send event to focused control by returning true.

                if (!this.Disposing || !this.IsDisposed)
                {
                    if (!Button_Console_Submit.Visible)
                    {
                        Button_Console_Submit.Visible = Input_Console.Visible = true;
                    }
                    else
                    {
                        Button_Console_Submit.Visible = Input_Console.Visible = false;
                    }
                }

                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void Loginbuttonenabler(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Input_Email.Text) || string.IsNullOrWhiteSpace(Input_Password.Text))
            {
                LoginEnabled = false;
                Button_Login.BackgroundImage = Image_Button.Grey;
                Button_Login.ForeColor = Color_Text.L_Six;
            }
            else
            {
                LoginEnabled = true;
                Button_Login.BackgroundImage = Image_Button.Grey;
                Button_Login.ForeColor = Color_Text.L_Five;
            }
        }

        private void LoginButton_MouseUp(object sender, EventArgs e)
        {
            if (LoginEnabled || Builtinserver)
            {
                Button_Login.BackgroundImage = Image_Button.Grey_Hover;
            }
            else
            {
                Button_Login.BackgroundImage = Image_Button.Grey;
            }
        }

        private void LoginButton_MouseDown(object sender, EventArgs e)
        {
            if (LoginEnabled || Builtinserver)
            {
                Button_Login.BackgroundImage = Image_Button.Grey_Click;
            }
            else
            {
                Button_Login.BackgroundImage = Image_Button.Grey;
            }
        }

        /*  After Successful Login, Hide Login Forms */
        private void LoggedInFormElements(bool hideElements)
        {
            if (hideElements == true)
            {
                try
                {
                    DateTime currentTime = DateTime.Now;

                    if ((currentTime.Hour >= 5) && (currentTime.Hour < 12))
                    {
                        LoginWelcomeTime = "Good Morning";
                    }
                    else if ((currentTime.Hour >= 12) && (currentTime.Hour < 18))
                    {
                        LoginWelcomeTime = "Good Afternoon";
                    }
                    else if ((currentTime.Hour >= 18) && (currentTime.Hour < 22))
                    {
                        LoginWelcomeTime = "Good Evening";
                    }
                    else
                    {
                        LoginWelcomeTime = "Hello Night Owl";
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LOGIN TIME", string.Empty, Error, string.Empty, true);
                    LoginWelcomeTime = "Pshhh Pshhh";
                }

                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Input_Email.Text).ToUpper();
                if (Picture_Information_Window.Image != Image_Other.Information_Window_Success)
                {
                    Picture_Information_Window.Image = Image_Other.Information_Window_Success;
                }

                if (Picture_Information_Window.Image != Image_Other.Information_Window_Success)
                {
                    Picture_Information_Window.Image = Image_Other.Information_Window_Success;
                }
                Button_Play_OR_Update.ForeColor = Color_Text.L_Five;
                Button_Play_OR_Update.Visible = hideElements;

                if (Button_Logout.BackgroundImage != Image_Button.Grey)
                {
                    Button_Logout.BackgroundImage = Image_Button.Grey;
                }
                Button_Logout.ForeColor = Color_Text.L_Five;
            }

            Panel_Launch.Visible = hideElements;
        }

        private void LoginFormElements(bool hideElements)
        {
            if (hideElements == true)
            {
                Label_Information_Window.Text = "Enter Your Account Information to Log In".ToUpper();
            }

            CheckBox_Remember_Us.Visible = hideElements;
            Button_Login.Visible = hideElements;

            Button_Register.Visible = hideElements;
            Input_Email.Visible = hideElements;
            Input_Password.Visible = hideElements;
            LinkLabel_Forgot_Password.Visible = hideElements;
            Button_Settings.Visible = hideElements;
            Button_Security_Center.Visible = hideElements;

            Button_Custom_Server.Enabled = hideElements;
            ComboBox_Server_List.Enabled = hideElements;

            /* Input Strokes */
            Picture_Input_Email.Visible = hideElements;
            Picture_Input_Password.Visible = hideElements;

            if (Picture_Input_Email.Image != Image_Other.Text_Border_Email)
            {
                Picture_Input_Email.Image = Image_Other.Text_Border_Email;
            }

            if (Picture_Input_Password.Image != Image_Other.Text_Border_Password)
            {
                Picture_Input_Password.Image = Image_Other.Text_Border_Password;
            }

            if (Picture_Information_Window.Image != Image_Other.Information_Window)
            {
                Picture_Information_Window.Image = Image_Other.Information_Window;
            }
        }

        private void LoginButton_Click(object? sender, EventArgs? e)
        {
            if ((LoginEnabled == false || ServerEnabled == false) && Builtinserver == false)
            {
                return;
            }

            if (IsDownloading)
            {
                string TempEmailCache = string.Empty;
                if (!string.IsNullOrWhiteSpace(Input_Email.Text))
                {
                    TempEmailCache = Input_Email.Text;
                    Input_Email.Text = "EMAIL IS HIDDEN";
                }
                MessageBox.Show(this, "Please wait while the GameLauncher is still downloading the game files.", "GameLauncher", MessageBoxButtons.OK);
                if (!string.IsNullOrWhiteSpace(TempEmailCache))
                {
                    Input_Email.Text = TempEmailCache;
                }

                return;
            }

            Tokens.Clear();

            string Email;
            string Password;

            Tokens.IPAddress = Launcher_Value.Launcher_Select_Server_Data.IPAddress;
            Tokens.ServerName = ServerListUpdater.ServerName("Login");

            switch (Authentication.HashType(Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Version ?? string.Empty))
            {
                case AuthHash.H10:
                    Email = Input_Email.Text.ToString();
                    Password = Input_Password.Text.ToString();
                    break;
                case AuthHash.H11:
                    Email = Input_Email.Text.ToString();
                    Password = Hashes.Hash_String(0, Input_Password.Text.ToString()).ToLower();
                    break;
                case AuthHash.H12:
                    Email = Input_Email.Text.ToString();
                    Password = Hashes.Hash_String(1, Input_Password.Text.ToString()).ToLower();
                    break;
                case AuthHash.H13:
                    Email = Input_Email.Text.ToString();
                    Password = Hashes.Hash_String(2, Input_Password.Text.ToString()).ToLower();
                    break;
                case AuthHash.H20:
                    Email = Hashes.Hash_String(0, Input_Email.Text.ToString()).ToLower();
                    Password = Hashes.Hash_String(0, Input_Password.Text.ToString()).ToLower();
                    break;
                case AuthHash.H21:
                    Email = Hashes.Hash_String(1, Input_Email.Text.ToString()).ToLower();
                    Password = Hashes.Hash_String(1, Input_Password.Text.ToString()).ToLower();
                    break;
                case AuthHash.H22:
                    Email = Hashes.Hash_String(2, Input_Email.Text.ToString()).ToLower();
                    Password = Hashes.Hash_String(2, Input_Password.Text.ToString()).ToLower();
                    break;
                default:
                    Log.Error("HASH TYPE: Unknown Hash Standard was Provided");
                    return;
            }

            Authentication.Client("Login", Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Post, Email, Password, String.Empty);

            if (string.IsNullOrWhiteSpace(Tokens.Error))
            {
                try
                {
                    if (!(ComboBox_Server_List.SelectedItem is Json_List_Server server)) return;
                    Save_Account.Live_Data.Saved_Server_Address = server.IPAddress;
                }
                catch { }

                UserId = Tokens.UserId;
                LoginToken = Tokens.LoginToken;
                Launcher_Value.Launcher_Select_Server_Data.IPAddress = Tokens.IPAddress;

                /* Tells the FileAccountSave to Actually Save the Information or Not */
                Save_Account.SaveLoginInformation = CheckBox_Remember_Us.Checked;
                Save_Account.Live_Data.User_Raw_Email = Input_Email.Text.ToString();
                Save_Account.Live_Data.User_Raw_Password = Input_Password.Text.ToString();

                switch (Authentication.HashType(Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Version ?? string.Empty))
                {
                    case AuthHash.H10:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "1.0";
                        Save_Account.Live_Data.User_Hashed_Email = string.Empty;
                        Save_Account.Live_Data.User_Hashed_Password = string.Empty;
                        break;
                    case AuthHash.H11:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "1.1";
                        Save_Account.Live_Data.User_Hashed_Email = string.Empty;
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(0, Input_Password.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H12:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "1.2";
                        Save_Account.Live_Data.User_Hashed_Email = string.Empty;
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(1, Input_Password.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H13:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "1.3";
                        Save_Account.Live_Data.User_Hashed_Email = string.Empty;
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(2, Input_Password.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H20:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "2.0";
                        Save_Account.Live_Data.User_Hashed_Email = Hashes.Hash_String(0, Input_Email.Text.ToString()).ToLower();
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(0, Input_Password.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H21:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "2.1";
                        Save_Account.Live_Data.User_Hashed_Email = Hashes.Hash_String(1, Input_Email.Text.ToString()).ToLower();
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(1, Input_Password.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H22:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "2.2";
                        Save_Account.Live_Data.User_Hashed_Email = Hashes.Hash_String(2, Input_Email.Text.ToString()).ToLower();
                        Save_Account.Live_Data.User_Hashed_Password = Hashes.Hash_String(2, Input_Password.Text.ToString()).ToLower();
                        break;
                    default:
                        Save_Account.Live_Data.Saved_Server_Hash_Version = "Unknown";
                        Save_Account.Live_Data.User_Hashed_Email = string.Empty;
                        Save_Account.Live_Data.User_Hashed_Password = string.Empty;
                        Log.Error("HASH TYPE: Unknown Hash Standard was Provided");
                        return;
                }

                Save_Account.Save();

                if (!string.IsNullOrWhiteSpace(Tokens.Warning))
                {
                    Input_Email.Text = "EMAIL IS HIDDEN";
                    MessageBox.Show(this, Tokens.Warning, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Input_Email.Text = Email;
                }

                LoginFormElements(false);
                LoggedInFormElements(true);
            }
            else
            {
                /* Main Screen Login */
                if (Picture_Input_Email.Image != Image_Other.Text_Border_Email_Error)
                {
                    Picture_Input_Email.Image = Image_Other.Text_Border_Email_Error;
                }
                if (Picture_Input_Password.Image != Image_Other.Text_Border_Password_Error)
                {
                    Picture_Input_Password.Image = Image_Other.Text_Border_Password_Error;
                }
                if (Picture_Information_Window.Image != Image_Other.Information_Window_Error)
                {
                    Picture_Information_Window.Image = Image_Other.Information_Window_Error;
                }
                Input_Email.Text = "EMAIL IS HIDDEN";
                MessageBox.Show(this, Tokens.Error, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Input_Email.Text = Email;
            }
        }

        private void LoginButton_MouseEnter(object sender, EventArgs e)
        {
            Button_Login.BackgroundImage = (LoginEnabled || Builtinserver) ? Image_Button.Grey_Hover : Image_Button.Grey;
            Button_Login.ForeColor = (LoginEnabled || Builtinserver) ? Color_Text.L_Five : Color_Text.L_Six;
        }

        private void LoginButton_MouseLeave(object sender, EventArgs e)
        {
            Button_Login.BackgroundImage = (LoginEnabled || Builtinserver) ? Image_Button.Grey : Image_Button.Grey;
            Button_Login.ForeColor = (LoginEnabled || Builtinserver) ? Color_Text.L_Five : Color_Text.L_Six;
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            if (DisableLogout == true)
            {
                return;
            }

            LoggedInFormElements(false);
            LoginFormElements(true);

            UserId = string.Empty;
            LoginToken = string.Empty;
        }

        private void Update_Popup_Click(object sender, EventArgs e)
        {
            if (LauncherUpdateCheck.UpgradeAvailable)
            {
                /* Reason: The specified executable is not a valid application for this OS platform.
                 * Code: -2147467259
                 * Full Log: System.ComponentModel.Win32Exception (0x80004005): The specified executable is not a valid application for this OS platform.*/
                LauncherUpdateCheck.UpdateStatusResult(true);
            }
        }

        private void Console_Enter(object sender, EventArgs e)
        {
            FunctionEvents.Console_Commands(Input_Console.Text);
            Input_Console.Text = string.Empty;
        }

        private void Console_Quick_Send(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Console_Enter(sender, e);
            }
        }

        /* Register PAGE LAYOUT */
        public void Button_Register_Click(object sender, EventArgs e)
        {
            if (FunctionStatus.AllowRegistration)
            {
                if(Launcher_Value.Launcher_Select_Server_JSON != default)
                {
                    if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Registration_Page))
                    {
#if NETFRAMEWORK
                    Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Registration_Page);
#else
                        Process.Start(new ProcessStartInfo { FileName = Launcher_Value.Launcher_Select_Server_JSON.Server_Registration_Page, UseShellExecute = true });
#endif
                        MessageBox.Show(this, "A browser window has been opened to complete registration on " +
                            ServerListUpdater.ServerName("Register"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (Launcher_Value.Launcher_Select_Server_Data.Name.ToUpper() == "WORLDUNITED OFFICIAL")
                    {
#if NETFRAMEWORK
                    Process.Start("https://signup.worldunited.gg/");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://signup.worldunited.gg/", UseShellExecute = true });
#endif
                        MessageBox.Show(this, "A browser window has been opened to complete registration on " +
                            Launcher_Value.Launcher_Select_Server_Data.Name, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        try
                        {
                            Screen_Register Custom_Instance_Register = new Screen_Register() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true, FormBorderStyle = FormBorderStyle.None };
                            Panel_Register_Screen.Controls.Add(Custom_Instance_Register);
                            Panel_Register_Screen.Visible = true;
                            Custom_Instance_Register.Show();
                            Text = "Register - SBRW Launcher: " + Application.ProductVersion;
                        }
                        catch (Exception Error)
                        {
                            string ErrorMessage = "Register Screen Encountered an Error";
                            LogToFileAddons.OpenLog("SETTINGS Register", ErrorMessage, Error, "Exclamation", false);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, "Loading Server Information. Please try again.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show(this, "Server seems to be Offline.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* SETTINGS PAGE LAYOUT */
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            try
            {
                Screen_Settings Custom_Instance_Settings = new Screen_Settings() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true, FormBorderStyle = FormBorderStyle.None };
                Panel_Form_Screens.Controls.Add(Custom_Instance_Settings);
                Panel_Form_Screens.Visible = true;
                Custom_Instance_Settings.Show();
                Text = "Settings - SBRW Launcher: " + Application.ProductVersion;
            }
            catch (Exception Error)
            {
                string ErrorMessage = "Settings Screen Encountered an Error";
                LogToFileAddons.OpenLog("SETTINGS SCREEN", ErrorMessage, Error, "Exclamation", false);
            }
        }

        private void ButtonSecurityCenter_Click(object sender, EventArgs e)
        {
            try
            {
                Screen_Security_Center Custom_Instance_Settings = new Screen_Security_Center() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true, FormBorderStyle = FormBorderStyle.None };
                Panel_Form_Screens.Visible = true;
                Panel_Form_Screens.Controls.Add(Custom_Instance_Settings);
                Screen_Security_Center.RPCStateCache = "Idle Ready";
                Custom_Instance_Settings.Show();
                Text = "Security Center - SBRW Launcher: " + Application.ProductVersion;
            }
            catch (Exception Error)
            {
                string ErrorMessage = "Security Center Screen Encountered an Error";
                LogToFileAddons.OpenLog("Security Center Panel", ErrorMessage, Error, "Exclamation", false);
            }
        }

        private void ComboBox_Server_List_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void PlayButton_MouseUp(object sender, EventArgs e)
        {
            if (Playenabled == false)
            {
                return;
            }

            if (Button_Play_OR_Update.BackgroundImage != Image_Button.Play_Hover)
            {
                Button_Play_OR_Update.BackgroundImage = Image_Button.Play_Hover;
            }
        }

        private void PlayButton_MouseDown(object sender, EventArgs e)
        {
            if (Playenabled == false)
            {
                return;
            }

            if (Button_Play_OR_Update.BackgroundImage != Image_Button.Play_Click)
            {
                Button_Play_OR_Update.BackgroundImage = Image_Button.Play_Click;
            }
        }

        private void PlayButton_MouseEnter(object sender, EventArgs e)
        {
            if (Playenabled == false)
            {
                return;
            }

            if (Button_Play_OR_Update.BackgroundImage != Image_Button.Play_Hover)
            {
                Button_Play_OR_Update.BackgroundImage = Image_Button.Play_Hover;
            }
        }

        private void PlayButton_MouseLeave(object sender, EventArgs e)
        {
            if (Playenabled == false)
            {
                return;
            }

            if (Button_Play_OR_Update.BackgroundImage != Image_Button.Play)
            {
                Button_Play_OR_Update.BackgroundImage = Image_Button.Play;
            }
        }

        private bool DisablePlayButton(bool Return_Value = false)
        {
            IsDownloading = false;
            Playenabled = false;

            return Return_Value;
        }

        /* Social Panel | Ping or Offline or DEV Servers | */
        private void DisableSocialPanelandClearIt()
        {
            /* Hides Social Panel */
            Panel_Server_Information.Visible = false;
            /* Home */
            if (Picture_Icon_Server_Home.BackgroundImage != Image_Icon.Home_Disabled)
            {
                Picture_Icon_Server_Home.BackgroundImage = Image_Icon.Home_Disabled;
            }
            LinkLabel_Server_Home.Enabled = false;
            /* Discord */
            if (Picture_Icon_Server_Discord.BackgroundImage != Image_Icon.Discord_Disabled)
            {
                Picture_Icon_Server_Discord.BackgroundImage = Image_Icon.Discord_Disabled;
            }
            LinkLabel_Server_Discord.Enabled = false;
            /* Facebook */
            if (Picture_Icon_Server_Facebook.BackgroundImage != Image_Icon.Facebook_Disabled)
            {
                Picture_Icon_Server_Facebook.BackgroundImage = Image_Icon.Facebook_Disabled;
            }
            LinkLabel_Server_Facebook.Enabled = false;
            /* Twitter */
            if (Picture_Icon_Server_Twitter.BackgroundImage != Image_Icon.Twitter_Disabled)
            {
                Picture_Icon_Server_Twitter.BackgroundImage = Image_Icon.Twitter_Disabled;
            }
            LinkLabel_Server_Twitter.Enabled = false;
            /* Scenery */
            Label_Server_Scenery.Text = "But It's Me!";
            /* Restart Timer */
            Label_Server_Force_Restart_Timer.Text = "Game Launcher!";
        }

        private void Game_Bootup(string UserID, string LoginToken)
        {
            if (!(Disposing || IsDisposed))
            {
                if (InformationCache.SelectedServerEnforceProxy)
                {
                    if (!Proxy_Settings.Running())
                    {
                        Proxy_Server.Instance.Start("Start Game");
                    }
                }

                Launcher_Value.Launcher_Proxy = Proxy_Settings.Running();

                Nfswstarted = new Thread(() =>
                {
                    if (Proxy_Settings.Running())
                    {
                        Game_Live_Data(UserID, LoginToken, "http://127.0.0.1:" + Proxy_Settings.Port + "/nfsw/Engine.svc", this);
                    }
                    else
                    {
                        Uri convert = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress);

                        if (convert.Scheme == "http")
                        {
                            Match match = Regex.Match(convert.Host, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
                            if (!match.Success)
                            {
                                Launcher_Value.Launcher_Select_Server_Data.IPAddress =
                                Launcher_Value.Launcher_Select_Server_Data.IPAddress.Replace(convert.Host, FunctionStatus.HostName2IP(convert.Host));
                            }
                        }

                        Game_Live_Data(UserID, LoginToken, Launcher_Value.Launcher_Select_Server_Data.IPAddress, this);
                    }
                })
                { IsBackground = true };

                Nfswstarted.Start();
                Presence_Launcher.Status(28, string.Empty);
            }
        }

        /* Check Serverlist API Status Upon Main Screen load - DavidCarbon */
        private void PingServerListAPIStatus()
        {
            Label_Status_API.Text = "United API:\n - Online";
            Label_Status_API.ForeColor = Color_Text.S_Sucess;
            Label_Status_API_Details.Text = "Connected to API";
            Picture_Icon_API.BackgroundImage = Image_Icon.Plug_Online;

            if (!VisualsAPIChecker.UnitedAPI())
            {
                Label_Status_API.Text = "Carbon API:\n - Online";
                Label_Status_API.ForeColor = Color_Text.S_Sucess;
                Label_Status_API_Details.Text = "Connected to API";
                Picture_Icon_API.BackgroundImage = Image_Icon.Plug_Online;

                if (!VisualsAPIChecker.CarbonAPI())
                {
                    Label_Status_API.Text = "Carbon 2nd API:\n - Online";
                    Label_Status_API.ForeColor = Color_Text.S_Sucess;
                    Label_Status_API_Details.Text = "Connected to API";
                    Picture_Icon_API.BackgroundImage = Image_Icon.Plug_Online;

                    if (!VisualsAPIChecker.CarbonAPITwo())
                    {
                        Label_Status_API.Text = "Cached API:\n - Local";
                        Label_Status_API.ForeColor = Color_Text.S_Warning;
                        Label_Status_API_Details.Text = "Using Local Cache";
                        Picture_Icon_API.BackgroundImage = Image_Icon.Plug_Warning;

                        if (!VisualsAPIChecker.Local_Cached_API())
                        {
                            Label_Status_API.Text = "Connection API:\n - Error";
                            Label_Status_API.ForeColor = Color_Text.S_Error;
                            Label_Status_API_Details.Text = "Launcher is Offline";
                            Picture_Icon_API.BackgroundImage = Image_Icon.Plug_Offline;
                            Log.Api("PINGING API: Failed to Connect to APIs! Quick Hide and Bunker Down! (Ask for help)");
                        }
                        else
                        {
                            Log.Api("PINGING API: Failed to Connect to APIs! Using Local Cache! (Ask for help)");
                        }
                    }
                }
            }
        }

        /* Launch game */
        private void Game_Check_Launch()
        {
            if (!(Disposing || IsDisposed))
            {
                Presence_Launcher.Start(false, Presence_Launcher.ApplicationID());

                try
                {
                    if (UI_MODE != 12)
                    {
                        UI_MODE = 12;
                    }

                    string GameExePath = Path.Combine(Save_Settings.Live_Data.Game_Path, "nfsw.exe");
                    string GameExehash = Hashes.Hash_SHA(GameExePath);
                    if
                      (
                        GameExehash.Equals("7C0D6EE08EB1EDA67D5E5087DDA3762182CDE4AC") ||
                        GameExehash.Equals("DB9287FB7B0CDA237A5C3885DD47A9FFDAEE1C19") ||
                        GameExehash.Equals("E69890D31919DE1649D319956560269DB88B8F22") ||
                        GameExehash.Equals("3CBE3FAAFF00FAD84F78A2AFEA4FFFC78294EEA2")
                      )
                    {
                        Launcher_Value.Game_Server_Name = ServerListUpdater.ServerName("Proxy");
                        Launcher_Value.Game_Server_IP = Launcher_Value.Launcher_Select_Server_Data.IPAddress;

                        Launcher_Value.Game_User_ID = UserId;
                        Launcher_Value.Game_Server_IP_Host = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress).Host;

                        /* REMOVED MODNET FILE COMPLETION */

                        if (UI_MODE != 8)
                        {
                            UI_MODE = 8;
                        }

                        Game_Bootup(UserId, LoginToken);
                    }
                    else if (!File.Exists(GameExePath))
                    {
                        Display_Color_Icons(2);
                        Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                        MessageBox.Show(this, "You do not have the Game Downloaded. Please Verify Game Files installation path.", "GameLauncher",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Button_Login_Logout_Modes(true, true);
                        Display_Color_Icons();
                    }
                    else
                    {
                        Display_Color_Icons(2);
                        Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                        MessageBox.Show(this, "Your NFSW.exe is Modified. Please Verify Game Files.", "GameLauncher",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Button_Login_Logout_Modes(true, true);
                        Display_Color_Icons();
                    }
                }
                catch (Exception Error)
                {
                    Display_Color_Icons(2);
                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    LogToFileAddons.OpenLog("GAME LAUNCH", Error.Message, Error, "Error", false);
                    Button_Login_Logout_Modes(true, true);
                    Display_Color_Icons();
                }
            }
        }

        private void Launcher_Close_Check(Form Live_Form, int Process_Exit_Code, int Process_ID = 0, bool Did_Game_Start = false, MessageBoxIcon Icon_Box_Art = MessageBoxIcon.Asterisk)
        {
            Presence_Launcher.Start();
            /* The process is adorable like a puppy, Kill it (https://youtu.be/mY3sM0jtwaA) */
            Log.Core("LAUNCHER: Killing any left over Processes related to NFSW");

            try
            {
                if (Process_ID != 0)
                {
                    if (!Process.GetProcessById(NfswPid).HasExited)
                    {
                        if (!Process.GetProcessById(NfswPid).CloseMainWindow())
                        {
                            Process.GetProcessById(NfswPid).Kill();
                        }
                    }
                }
            }
            catch { }

            try
            {
                Process[] Its_The_Law = Process.GetProcessesByName("nfsw");
                if (Its_The_Law != null)
                {
                    if (Its_The_Law.Length > 0)
                    {
                        foreach (Process Papers_Please in Its_The_Law)
                        {
                            try
                            {
                                if (!Process.GetProcessById(Papers_Please.Id).HasExited)
                                {
                                    if (!Process.GetProcessById(Papers_Please.Id).CloseMainWindow())
                                    {
                                        Process.GetProcessById(Papers_Please.Id).Kill();
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }

            Launcher_Value.Game_In_Event_Bug = FunctionStatus.LauncherBattlePass = false;

            if (Live_Action_Timer != null)
            {
                if (Live_Action_Timer.Enabled)
                {
                    Live_Action_Timer.Stop();
                }
            }

#if NETFRAMEWORK
            Nfswstarted?.Abort();
#endif
            string Error_Msg = NFSW.ErrorTranslation(Process_Exit_Code);

            if (Did_Game_Start && !string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path) && !FunctionStatus.LauncherBattlePass)
            {
                if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)))
                {
                    ModNetHandler.CleanLinks(Save_Settings.Live_Data.Game_Path);
                }
            }

            Live_Form.SafeEndInvokeAsyncCatch(Live_Form.SafeBeginInvokeActionAsync(Launcher_X_Form =>
            {
                UI_MODE = 14;
                
                DisableLogout = false;

                if (Screen_Instance != null)
                {
                    Label_Information_Window.SafeInvokeAction(() => 
                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper(), this, false);
                    Label_Download_Information.SafeInvokeAction(() =>
                    Label_Download_Information.Text = Error_Msg.ToUpper(), this);
                    Button_Play_OR_Update.SafeInvokeAction(() => Button_Play_OR_Update.Visible = false, this, false);
                }

                if (Did_Game_Start)
                {
                    Presence_Launcher.Status(0, "Game Closed with Error Code: " + Process_Exit_Code.ToString());
                    Log.Error("GAME CRASH [EXIT CODE]: " + Process_Exit_Code.ToString() + " HEX: (0x" + Process_Exit_Code.ToString("X") + ")" + " REASON: " + Error_Msg);
                    
                    if (Screen_Instance != null)
                    {
                        Display_Color_Icons(3);
                    }
                }
                else
                {
                    Presence_Launcher.Status(0, "Game Failed to Launch");
                    Log.Core("LAUNCHER: Game failed to Launch. Forcing User to Login again.");
                }

                if (MessageBox.Show(Screen_Instance, Error_Msg, "GameLauncher", MessageBoxButtons.OK, Icon_Box_Art) == DialogResult.OK)
                {
                    Display_Color_Icons(1);
                }
            }));
        }

        private void Game_Live_Data(string UserID, string LoginToken, string ServerIP, Form Live_Form)
        {
            if (!(Disposing || IsDisposed))
            {
                if (new Process_Start_Game().Initialize(Save_Settings.Live_Data.Game_Path, ServerIP, LoginToken,
                UserID, Launcher_Value.Launcher_Select_Server_Data.ID.ToUpper(), true, "nfsw.exe") != null)
                {
                    /* Request a New Session */
                    Time_Window.Client_Session();
                    Session_Timer.Remaining = Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer != 0 ? Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer : 2 * 60 * 60;
                    FunctionStatus.LauncherBattlePass = Process_Start_Game.Live_Process.EnableRaisingEvents = true;
                    NfswPid = Process_Start_Game.Live_Process.Id;
                    Process_Start_Game.Live_Process.Exited += (Send, It) =>
                    {
                        NfswPid = 0;
                        int exitCode = Process_Start_Game.Live_Process.ExitCode;

                        FunctionStatus.LauncherBattlePass = false;

                        if (Launcher_Value.Game_In_Event_Bug)
                        {
                            if (AC_Core.Status)
                            {
                                exitCode = 2017;
#if NETFRAMEWORK
                            ContextMenu = new ContextMenu();
                            ContextMenu.MenuItems.Add(new MenuItem("Ezekiel was Here - Sent from Mars (C&T)", (b, n) => 
                            {
#if NETFRAMEWORK
                                Process.Start("https://www.youtube.com/watch?v=T-AF81iBCi0");
#else
                                Process.Start("explorer.exe", "https://www.youtube.com/watch?v=T-AF81iBCi0");
#endif
                            }));
                            ContextMenu.MenuItems.Add("-");
                            if (Parent_Screen.Screen_Instance != null)
                            {
                                ContextMenu.MenuItems.Add(new MenuItem("Close Launcher", Parent_Screen.Screen_Instance.Button_Close_Click));
                            }

                            NotifyIcon_Notification.ContextMenu = ContextMenu;
#endif
                            }
                            else
                            {
                                exitCode = 2137;
#if NETFRAMEWORK
                            ContextMenu = new ContextMenu();
                            ContextMenu.MenuItems.Add(new MenuItem("One more Minute", (b, n) => 
                            {
#if NETFRAMEWORK
                                Process.Start("https://youtu.be/HNuOQlt1KEM");
#else
                                Process.Start("explorer.exe", "https://youtu.be/HNuOQlt1KEM");
#endif
                            }));
                            ContextMenu.MenuItems.Add("-");
                            if (Parent_Screen.Screen_Instance != null)
                            {
                                ContextMenu.MenuItems.Add(new MenuItem("Close Launcher", Parent_Screen.Screen_Instance.Button_Close_Click));
                            }

                            NotifyIcon_Notification.ContextMenu = ContextMenu;
#endif
                            }
                        }
                        if (exitCode == 0 && !Launcher_Value.Game_In_Event_Bug && AC_Core.Stop_Check())
                        {
                            Parent_Screen.Screen_Instance?.Button_Close_Click(new object(), new EventArgs());
                        }
                        else if (AC_Core.Stop_Check())
                        {
                            Launcher_Close_Check(Live_Form, exitCode, NfswPid, true, MessageBoxIcon.Error);
                        }
                    };

                    while (Process_Start_Game.Live_Process.MainWindowHandle == IntPtr.Zero && !Process_Start_Game.Live_Process.HasExited)
                    {
                        /* Loop Here until the game Window Appears */
                    }

                    if (!Process_Start_Game.Live_Process.HasExited)
                    {
                        Presence_Launcher.Status(28, string.Empty);

                        /* TIMER HERE */
                        Live_Action_Timer = new System.Timers.Timer();
                        Live_Action_Timer.Elapsed += new System.Timers.ElapsedEventHandler(Time_Window.ClockWork_Planet);
                        Time_Window.Session_Alert += (x, D_Live_Events) =>
                        {
                            if (D_Live_Events != null)
                            {
                                try
                                {
                                    if (NotifyIcon_Notification != default)
                                    {
                                        NotifyIcon_Notification.Visible = D_Live_Events.Valid;
                                        NotifyIcon_Notification.BalloonTipIcon = ToolTipIcon.Info;
                                        NotifyIcon_Notification.BalloonTipTitle = "Force Restart - " + Launcher_Value.Game_Server_Name;
                                        NotifyIcon_Notification.BalloonTipText = "Game will shutdown by " + (D_Live_Events.Session_End_Time ?? DateTime.Now.AddMinutes(5)).ToString("t") + ". Please restart it manually before the launcher does it.";
                                        NotifyIcon_Notification.ShowBalloonTip(TimeSpan.FromMinutes(2).Seconds);
                                        NotifyIcon_Notification.BalloonTipClicked += (x, D_Live_Events) =>
                                        {
                                            return;
                                        };
                                        NotifyIcon_Notification.BalloonTipClosed += (x, D_Live_Events) =>
                                        {
                                            return;
                                        };
                                    }
                                }
                                catch (Exception Error)
                                {
                                    LogToFileAddons.OpenLog("NotifyIcon_Notification Timer", string.Empty, Error, "Error", true);
                                }
                                finally
                                {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                    GC.Collect();
#endif
                                }
                            }
                        };

                        /* 0 = Static Timer, 1 = Dynamic Timer, 2 = No Timer */
                        if (Save_Settings.Live_Data.Launcher_Display_Timer == "1")
                        {
                            Time_Window.Timer_Dynamic = true;
                        }
                        else if (Save_Settings.Live_Data.Launcher_Display_Timer == "2")
                        {
                            /* Notes: This actually does not Display Timers on the Title Window and 'Time_Window.Live_Stream' will be renamed in the future */
                            Time_Window.Timer_None = true;
                        }
                        else
                        {
                            Time_Window.Timer_None = Time_Window.Timer_Dynamic = false;
                        }

                        Live_Action_Timer.Interval = 30000;
                        Live_Action_Timer.Enabled = true;

#if NETFRAMEWORK
                    ContextMenu = new ContextMenu();
                    ContextMenu.MenuItems.Add(new MenuItem("Running Out of Time", (b, n) => 
                    {
#if NETFRAMEWORK
                        Process.Start("https://youtu.be/vq9-bmoI-RI");
#else
                        Process.Start("explorer.exe", "https://youtu.be/vq9-bmoI-RI");
#endif
                    }));
                    ContextMenu.MenuItems.Add("-");
                    if (Parent_Screen.Screen_Instance != null)
                    {
                        ContextMenu.MenuItems.Add(new MenuItem("Close Game and Launcher", Parent_Screen.Screen_Instance.Button_Close_Click));
                    }

                    NotifyIcon_Notification.ContextMenu = ContextMenu;
#endif
                        UI_MODE = 13;
                        Log.Core("LAUNCHER: Game has Fully Launched, Minimized Launcher");
                    }
                    else if (FunctionStatus.LauncherBattlePass)
                    {
                        Launcher_Close_Check(Live_Form, 2020, NfswPid, true, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void Client_DownloadProgressChanged_RELOADED(object _, DownloadProgressChangedEventArgs e)
        {
            if (Screen_Instance != null && !IsDisposed && !Disposing)
            {
                try
                {
                    ModNet_Download_Status = new Download_Information_ModNet()
                    {
                        Download_Percentage = (int)((double)CurrentModFileCount/(double)TotalModFileCount * 100).Clamp(0, 100),
                        File_Name = ModNetFileNameInUse,
                        File_Size_Total = e.TotalBytesToReceive,
                        File_Size_Current = e.BytesReceived,
                        File_Size_Remaining = e.TotalBytesToReceive - e.BytesReceived,
                        Download_Complete = false
                    };

                    if (UI_MODE != 7)
                    {
                        UI_MODE = 7;
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Client_DownloadProgressChanged_RELOADED", string.Empty, Error, string.Empty, true);
                }
            }
        }

        public void DownloadModNetFilesRightNow(string path)
        {
            if (!(Disposing || IsDisposed))
            {
                while (IsDownloadingModNetFiles == false)
                {
                    CurrentModFileCount++;
                    Uri url = ModFilesDownloadUrls.Dequeue();
                    string FileName = url.ToString().Substring(url.ToString().LastIndexOf("/") + 1, (url.ToString().Length - url.ToString().LastIndexOf("/") - 1));

                    ModNetFileNameInUse = FileName;
                    ServicePointManager.FindServicePoint(url).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
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
                        Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged_RELOADED);
                        Client.DownloadFileCompleted += (Live_Object, Live_Final_Results) =>
                        {
#if !DEBUG
                        if (Live_Final_Results.Error != null)
                        {
                            LogToFileAddons.OpenLog("Modnet Server Files", string.Empty, Live_Final_Results.Error, string.Empty, true);
                        }
                        else if (Live_Final_Results.Cancelled)
                        {
                            Log.Core("LAUNCHER: Modnet Server Files Download was Cancelled");
                        }
                        else
                        {
#endif
                            Log.Core("LAUNCHER: Downloaded: " + FileName);
                            IsDownloadingModNetFiles = false;
                            if (!ModFilesDownloadUrls.Any())
                            {
                                ModNet_Download_Status = new Download_Information_ModNet()
                                {
                                    Download_Percentage = 100,
                                    Download_Complete = true
                                };

                                Game_Check_Launch();
                            }
                            else
                            {
                                /* Redownload other file */
                                DownloadModNetFilesRightNow(path);
                            }
#if !DEBUG
                        }
#endif
                        };
                        Client.DownloadFileAsync(url, path + "/" + FileName);
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("Modnet Server Files", string.Empty, Error, string.Empty, true);

                        if (Screen_Instance != null && (!IsDisposed || !Disposing))
                        {
                            Label_Information_Window.SafeInvokeAction(() =>
                            Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper());
                        }
                    }
                    finally
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }

                    IsDownloadingModNetFiles = true;
                }
            }
        }

        private void Display_Color_Icons(int Color_Mode = 0, bool Login_Icon_Color = true)
        {
            try
            {
                if (ProgressBar.Value < 100)
                {
                    if (ProgressBar.InvokeRequired)
                    {
                        ProgressBar.Invoke(new Action(delegate ()
                        {
                            ProgressBar.Value = 100;
                        }));
                    }
                    else
                    {
                        ProgressBar.Value = 100;
                    }
                }

                switch (Color_Mode)
                {
                    /* Checking (Pinging Blue) */
                    case 1:
                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(0))
                        {
                            if (Picture_Bar_Outline.InvokeRequired)
                            {
                                Picture_Bar_Outline.SafeInvokeAction(() => 
                                {
                                    Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Checking_Outline;
                                    Picture_Bar_Outline.Tag = Image_ProgressBar.Checking_Outline.Tag;
                                }, this);
                            }
                            else
                            {
                                Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Checking_Outline;
                                Picture_Bar_Outline.Tag = Image_ProgressBar.Checking_Outline.Tag;
                            }
                        }

                        if (ProgressBar.BackColor != Color_Winform_Other.ProgressBar_Loading_Top)
                        {
                            if (ProgressBar.InvokeRequired)
                            {
                                ProgressBar.Invoke(new Action(delegate ()
                                {
                                    ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Loading_Top;
                                    ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Loading_Bottom;
                                    
                                }));
                            }
                            else
                            {
                                ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Loading_Top;
                                ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Loading_Bottom;
                                
                            }
                        }

                        if (Login_Icon_Color)
                        {
                            if (Picture_Information_Window.Tag == default || !Picture_Information_Window.Tag.Equals(0))
                            {
                                if (Picture_Information_Window.InvokeRequired)
                                {
                                    Picture_Information_Window.SafeInvokeAction(() =>
                                    {
                                        Picture_Information_Window.Image = Image_Other.Information_Window;
                                        Picture_Information_Window.Tag = 0;
                                    }, this);
                                }
                                else
                                {
                                    Picture_Information_Window.Image = Image_Other.Information_Window;
                                    Picture_Information_Window.Tag = 0;
                                }
                            }
                        }
                        break;
                    /* Error (Red) */
                    case 2:
                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(3))
                        {
                            if (Picture_Bar_Outline.InvokeRequired)
                            {
                                Picture_Bar_Outline.SafeInvokeAction(() =>
                                {
                                    Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Error_Outline;
                                    Picture_Bar_Outline.Tag = Image_ProgressBar.Error_Outline.Tag;
                                }, this);
                            }
                            else
                            {
                                Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Error_Outline;
                                Picture_Bar_Outline.Tag = Image_ProgressBar.Error_Outline.Tag;
                            }
                        }

                        if (ProgressBar.BackColor != Color_Winform_Other.ProgressBar_Error_Top)
                        {
                            if (ProgressBar.InvokeRequired)
                            {
                                ProgressBar.Invoke(new Action(delegate ()
                                {
                                    ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Error_Top;
                                    ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Error_Bottom;
                                    
                                }));
                            }
                            else
                            {
                                ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Error_Top;
                                ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Error_Bottom;
                                
                            }
                        }

                        if (Login_Icon_Color)
                        {
                            if (Picture_Information_Window.Tag == default || !Picture_Information_Window.Tag.Equals(3))
                            {
                                if (Picture_Information_Window.InvokeRequired)
                                {
                                    Picture_Information_Window.SafeInvokeAction(() =>
                                    {
                                        Picture_Information_Window.Image = Image_Other.Information_Window_Error;
                                        Picture_Information_Window.Tag = 3;
                                    }, this);
                                }
                                else
                                {
                                    Picture_Information_Window.Image = Image_Other.Information_Window_Error;
                                    Picture_Information_Window.Tag = 3;
                                }
                            }
                        }
                        break;
                    /* Warning (Yellow) */
                    case 3:
                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(2))
                        {
                            if (Picture_Bar_Outline.InvokeRequired)
                            {
                                Picture_Bar_Outline.SafeInvokeAction(() =>
                                {
                                    Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Warning_Outline;
                                    Picture_Bar_Outline.Tag = Image_ProgressBar.Warning_Outline.Tag;
                                }, this);
                            }
                            else
                            {
                                Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Warning_Outline;
                                Picture_Bar_Outline.Tag = Image_ProgressBar.Warning_Outline.Tag;
                            }
                        }

                        if (ProgressBar.BackColor != Color_Winform_Other.ProgressBar_Warning_Top)
                        {
                            if (ProgressBar.InvokeRequired)
                            {
                                ProgressBar.Invoke(new Action(delegate ()
                                {
                                    ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Warning_Top;
                                    ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Warning_Bottom;
                                    
                                }));
                            }
                            else
                            {
                                ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Warning_Top;
                                ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Warning_Bottom;
                                
                            }
                        }

                        if (Login_Icon_Color)
                        {
                            if (Picture_Information_Window.Tag == default || !Picture_Information_Window.Tag.Equals(2))
                            {
                                if (Picture_Information_Window.InvokeRequired)
                                {
                                    Picture_Information_Window.SafeInvokeAction(() =>
                                    {
                                        Picture_Information_Window.Image = Image_Other.Information_Window_Warning;
                                        Picture_Information_Window.Tag = 2;
                                    }, this);
                                }
                                else
                                {
                                    Picture_Information_Window.Image = Image_Other.Information_Window_Warning;
                                    Picture_Information_Window.Tag = 2;
                                }
                            }
                        }
                        break;
                    /* Unknown (Gray) */
                    case 4:
                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(4))
                        {
                            if (Picture_Bar_Outline.InvokeRequired)
                            {
                                Picture_Bar_Outline.SafeInvokeAction(() =>
                                {
                                    Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Preload_Outline;
                                    Picture_Bar_Outline.Tag = Image_ProgressBar.Preload_Outline.Tag;
                                }, this);
                            }
                            else
                            {
                                Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Preload_Outline;
                                Picture_Bar_Outline.Tag = Image_ProgressBar.Preload_Outline.Tag;
                            }
                        }

                        if (ProgressBar.BackColor != Color_Winform_Other.ProgressBar_Unknown_Top)
                        {
                            if (ProgressBar.InvokeRequired)
                            {
                                ProgressBar.Invoke(new Action(delegate ()
                                {
                                    ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Unknown_Top;
                                    ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Unknown_Bottom;
                                    
                                }));
                            }
                            else
                            {
                                ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Unknown_Top;
                                ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Unknown_Bottom;
                                
                            }
                        }

                        if (Login_Icon_Color)
                        {
                            if (Picture_Information_Window.Tag == default || !Picture_Information_Window.Tag.Equals(4))
                            {
                                if (Picture_Information_Window.InvokeRequired)
                                {
                                    Picture_Information_Window.SafeInvokeAction(() =>
                                    {
                                        Picture_Information_Window.Image = Image_Other.Information_Window_Unknown;
                                        Picture_Information_Window.Tag = 4;
                                    }, this);
                                }
                                else
                                {
                                    Picture_Information_Window.Image = Image_Other.Information_Window_Unknown;
                                    Picture_Information_Window.Tag = 4;
                                }
                            }
                        }
                        break;
                    /* Complete (Green) */
                    default:
                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(1))
                        {
                            if (Picture_Bar_Outline.InvokeRequired)
                            {
                                Picture_Bar_Outline.SafeInvokeAction(() =>
                                {
                                    Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Complete_Outline;
                                    Picture_Bar_Outline.Tag = Image_ProgressBar.Complete_Outline.Tag;
                                }, this);
                            }
                            else
                            {
                                Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Complete_Outline;
                                Picture_Bar_Outline.Tag = Image_ProgressBar.Complete_Outline.Tag;
                            }
                        }

                        if (ProgressBar.BackColor != Color_Winform_Other.ProgressBar_Sucess_Top)
                        {
                            if (ProgressBar.InvokeRequired)
                            {
                                ProgressBar.Invoke(new Action(delegate ()
                                {
                                    ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Sucess_Top;
                                    ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Sucess_Bottom;
                                    
                                }));
                            }
                            else
                            {
                                ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Sucess_Top;
                                ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Sucess_Bottom;
                                
                            }
                        }

                        if (Login_Icon_Color)
                        {
                            if (Picture_Information_Window.Tag == default || !Picture_Information_Window.Tag.Equals(1))
                            {
                                if (Picture_Information_Window.InvokeRequired)
                                {
                                    Picture_Information_Window.SafeInvokeAction(() =>
                                    {
                                        Picture_Information_Window.Image = Image_Other.Information_Window_Success;
                                        Picture_Information_Window.Tag = 1;
                                    }, this);
                                }
                                else
                                {
                                    Picture_Information_Window.Image = Image_Other.Information_Window_Success;
                                    Picture_Information_Window.Tag = 1;
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception Error_Live)
            {
                LogToFileAddons.OpenLog("Display_Color_Icons", string.Empty, Error_Live, string.Empty, true);
            }
        }

        /// <summary>
        /// Disable Play Button and Logout Buttons
        /// </summary>
        /// <param name="Enabler_Mode"></param>
        private void Button_Login_Logout_Modes(bool Enabler_Mode = false, bool Disable_Play_Button = false)
        {
            if (Enabler_Mode)
            {
                Button_Play_OR_Update.Visible = Disable_Play_Button ? false : true;
                Button_Logout.Visible = EnablePlayButton(true);
                DisableLogout = false;
            }
            else
            {
                Button_Play_OR_Update.Visible = Button_Logout.Visible = DisablePlayButton();
                DisableLogout = true;
            }
        }

        private async void PlayButton_Click(object sender, EventArgs e)
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
            DriveInfo driveInfo = new DriveInfo(Save_Settings.Live_Data.Game_Path);

            if (!string.Equals(driveInfo.DriveFormat, "NTFS", StringComparison.InvariantCultureIgnoreCase))
            {
                Picture_Information_Window.Image = Image_Other.Information_Window_Error;
                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                MessageBox.Show(this,
                    $"Playing the game on a non-NTFS-formatted drive is not supported.\nDrive '{driveInfo.Name}' is formatted with: {driveInfo.DriveFormat}",
                    "Compatibility",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error)
                    ;
                Picture_Information_Window.Image = Image_Other.Information_Window_Success;
                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Save_Account.Live_Data.User_Raw_Email).ToUpper();
                return;
            }
#endif

            if (Save_Settings.Live_Data.Game_Integrity == "Ignore")
            {
                Display_Color_Icons(3);
            }
            else if (Save_Settings.Live_Data.Game_Integrity != "Good")
            {
                Display_Color_Icons(3);
                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                MessageBox.Show(this, "GameLauncher has detected a GameFiles Integrity Error\nPlease 'Verify GameFiles' in the Settings Screen",
                    "Game Files Integrity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Display_Color_Icons();
                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Save_Account.Live_Data.User_Raw_Email).ToUpper();
                return;
            }

            if (!Redistributable.Error_Free)
            {
                Picture_Information_Window.Image = Image_Other.Information_Window_Error;
                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                MessageBox.Show(this, "GameLauncher has detected that the 2015-2019 (or newer) VC++ Redistributable Package is not installed or may be damaged\n" +
                    "Please manually Install or Repair the Packages for your Operating System",
                    "VC++ Redistributable Package Check", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Picture_Information_Window.Image = Image_Other.Information_Window_Success;
                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Save_Account.Live_Data.User_Raw_Email).ToUpper();
                return;
            }

            if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)))
            {
                try
                {
                    File.Delete(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks));
                }
                catch { }
            }
            
            Button_Login_Logout_Modes();
            Display_Color_Icons(1, false);

            ModNetHandler.FileANDFolder(Save_Settings.Live_Data.Game_Path);
            Log.Core("LAUNCHER: Installing ModNet");
            Label_Download_Information.Text = ("Detecting ModNet Support for " + ServerListUpdater.ServerName("ModNet")).ToUpper();

            if (ModNetHandler.Supported())
            {
                /* Caches (In Order of Excution) */
                string ModulesJSON = string.Empty;
                string ServerModInfo = string.Empty;
                GetModInfo? json2 = null;
                string remoteCarsFile = string.Empty;
                string remoteEventsFile = string.Empty;
                string ServerModListJSON = string.Empty;
                ServerModList? json3 = null;

                try
                {
                    Presence_Launcher.Status(5);
                    /* Get Remote ModNet list to process for checking required ModNet files are present and current */
                    Uri ModNetURI = new Uri(URLs.ModNet + "/launcher-modules/modules.json");
                    ServicePointManager.FindServicePoint(ModNetURI).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                    Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
                    var ModNetJsonURI = new WebClient
                    {
                        Encoding = Encoding.UTF8,
                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                    };
                    if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                    { 
                        ModNetJsonURI = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) }; 
                    }
                    else
                    {
                        ModNetJsonURI.Headers.Add("user-agent", "SBRW Launcher " +
                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    }

                    /* ModNet Cache File Comparison */
                    DateTime Time_Check = DateTime.Now.Date;
                    string Launcher_Data_Folder = Path.Combine("Launcher_Data", "JSON", "ModNet");
                    string Time_Stamp = Path.Combine(Launcher_Data_Folder, "Time_Stamp.txt");
                    string Server_List_Cache = Path.Combine(Launcher_Data_Folder, "Modules.json");
                    bool ModNet_Offline = false;

                    if (File.Exists(Time_Stamp))
                    {
                        try
                        {
                            Time_Check = DateTime.Parse(File.ReadLines(Time_Stamp).First()).Date;
                        }
                        catch
                        {

                        }
                    }

                    try
                    {
                        try
                        {
                            ModulesJSON = ModNetJsonURI.DownloadString(ModNetURI);
                            Label_Download_Information.Text = "JSON: Retrieved ModNet Files Information".ToUpper();
                        }
                        catch
                        {
                            ModNet_Offline = true;
                            if (File.Exists(Server_List_Cache))
                            {
                                Log.Warning("MODNET FILE CACHE: Found");

                                bool Allow_ModNet_Cache = false;
                                if (Time_Check < DateTime.Now.Date)
                                {
                                    Display_Color_Icons(3);
                                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                                    if (MessageBox.Show(this, "Launcher has found a ModNet Cache File.\nHowever, its a day old.\nWould you like to use the old File Cache?",
                                        "SBRW Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                    {
                                        Allow_ModNet_Cache = true;
                                    }
                                    Display_Color_Icons();
                                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Save_Account.Live_Data.User_Raw_Email).ToUpper();
                                }
                                else
                                {
                                    Allow_ModNet_Cache = true;
                                }

                                if (Allow_ModNet_Cache)
                                {
                                    try
                                    {
                                        Log.Warning("MODNET FILE CACHE: Loading");
                                        ModulesJSON = File.ReadAllText(Server_List_Cache);
                                        Log.Warning("MODNET FILE CACHE: Loaded");
                                    }
                                    catch
                                    {
                                        throw;
                                    }
                                }
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                    catch (Exception Error)
                    {
                        Display_Color_Icons(2);
                        Label_Download_Information.Text = ("JSON: Unable to Retrieve ModNet Files Information").ToUpper();
                        Presence_Launcher.Status(8);
                        Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                        string LogMessage = "There was an error with ModNet JSON Retrieval:";
                        LogToFileAddons.OpenLog("MODNET FILES", LogMessage, Error, "Error", false);
                        Button_Login_Logout_Modes(true, true);
                        Display_Color_Icons();
                    }
                    finally
                    {
                        if (ModNetJsonURI != null)
                        {
                            ModNetJsonURI.Dispose();
                        }
                    }

                    if (string.IsNullOrWhiteSpace(ModulesJSON) || !ModulesJSON.Valid_Json())
                    {
                        Display_Color_Icons(2);
                        Label_Download_Information.Text = ("JSON: Invalid ModNet Files Information").ToUpper();
                        Presence_Launcher.Status(8);
                        Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                        Button_Login_Logout_Modes(true, true);
                        Display_Color_Icons();
                        ModulesJSON = string.Empty;
                        return;
                    }
                    else
                    {
                        try
                        {
                            if (!ModNet_Offline)
                            {
                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        if ((Time_Check < DateTime.Now.Date) || !File.Exists(Time_Stamp))
                                        {
                                            if (!Directory.Exists(Launcher_Data_Folder))
                                            {
                                                Directory.CreateDirectory(Launcher_Data_Folder);
                                            }

                                            File.WriteAllText(Server_List_Cache, ModulesJSON);
                                            File.WriteAllText(Time_Stamp, DateTime.Now.ToString());
                                        }
                                    }
                                    catch { }
                                    finally
                                    {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                        GC.Collect();
#endif
                                    }
                                });
                            }

                            Label_Download_Information.Text = ("ModNet: Checking Local Files. This may take awhile.").ToUpper();

                            string[] modules_newlines = ModulesJSON.Split(new string[] { "\n" }, StringSplitOptions.None);
                            foreach (string modules_newline in modules_newlines)
                            {
                                if (modules_newline.Trim().ToStringInvariant() == "{" || modules_newline.Trim().ToStringInvariant() == "}")
                                {
                                    continue;
                                }

                                string trim_modules_newline = modules_newline.Trim().ToStringInvariant();
                                string[] modules_files = trim_modules_newline.Split(new char[] { ':' });

                                string ModNetList = modules_files[0].Replace("\"", "").Trim().ToStringInvariant();
                                string ModNetSHA = modules_files[1].Replace("\"", "").Replace(",", "").Trim().ToLowerInvariant();

                                string ModNetFilePath = Path.Combine(Save_Settings.Live_Data.Game_Path, ModNetList);
                                string ModNetLocalFileHash = string.Empty;

                                await Task.Run(() =>
                                {
                                    ModNetLocalFileHash = Hashes.Hash_SHA256(ModNetFilePath).ToLowerInvariant();
                                });

                                if (!ModNetLocalFileHash.Equals(ModNetSHA) || !File.Exists(ModNetFilePath))
                                {
                                    Label_Download_Information.Text = ("ModNet: Downloading " + ModNetList).ToUpper();

                                    Log.Warning("MODNET CORE: " + ModNetList + " Does not match SHA Hash on File Server -> Online Hash: '" + ModNetSHA + "'");

                                    if (File.Exists(ModNetFilePath))
                                    {
                                        File.Delete(ModNetFilePath);
                                    }

                                    Presence_Launcher.Status(7, ModNetList);

                                    Uri URLCall = new Uri(URLs.ModNet + "/launcher-modules/" + ModNetList);
                                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                    Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
                                    var newModNetFilesDownload = new WebClient
                                    {
                                        Encoding = Encoding.UTF8,
                                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                                    };
                                    if (!Launcher_Value.Launcher_Alternative_Webcalls())
                                    {
                                        newModNetFilesDownload = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) };
                                    }
                                    else
                                    {
                                        newModNetFilesDownload.Headers.Add("user-agent", "SBRW Launcher " +
                                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                    }
                                    newModNetFilesDownload.DownloadFile(URLCall, ModNetFilePath);
                                }
                                else
                                {
                                    Label_Download_Information.Text = ("ModNet: Up to Date " + ModNetList).ToUpper();
                                    Log.Info("MODNET CORE: " + ModNetList + " Is Up to Date!");
                                }
                            }
                        }
                        catch (Exception Error)
                        {
                            Display_Color_Icons(2);
                            Presence_Launcher.Status(8);
                            Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            string LogMessage = "There was an error with ModNet Files Check:";
                            LogToFileAddons.OpenLog("MODNET CORE", LogMessage, Error, "Error", false);
                            Button_Login_Logout_Modes(true, true);
                            Display_Color_Icons();

                            return;
                        }
                        finally
                        {
                            if (!string.IsNullOrWhiteSpace(ModulesJSON))
                            {
                                ModulesJSON = string.Empty;
                            }
                        }

                        Server_Ping(Launcher_Value.Launcher_Select_Server_Data.IPAddress, 5000);

                        Uri newModNetUri = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress + "/Modding/GetModInfo");
                        ServicePointManager.FindServicePoint(newModNetUri).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                    Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
                        var ModInfoJson = new WebClient
                        {
                            Encoding = Encoding.UTF8,
                            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                        };
                        if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                        { 
                            ModInfoJson = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) }; 
                        }
                        else
                        {
                            ModInfoJson.Headers.Add("user-agent", "SBRW Launcher " +
                            Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        }

                        try
                        {
                            ServerModInfo = ModInfoJson.DownloadString(newModNetUri);
                            Label_Download_Information.Text = ("JSON: Retrieved Server Mod Information").ToUpper();
                        }
                        catch (Exception Error)
                        {
                            Label_Download_Information.Text = ("JSON: Unable to Retrieve Server Mod Information").ToUpper();
                            Presence_Launcher.Status(10);
                            Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            string LogMessage = "There was an error with Server Mod Information Retrieval:";
                            LogToFileAddons.OpenLog("SERVER MOD INFO", LogMessage, Error, "Error", false);
                            Button_Login_Logout_Modes(true, true);
                            Display_Color_Icons();
                        }
                        finally
                        {
                            ModInfoJson?.Dispose();
                        }

                        if (string.IsNullOrWhiteSpace(ServerModInfo) || !ServerModInfo.Valid_Json())
                        {
                            Display_Color_Icons(2);
                            Label_Download_Information.Text = ("JSON: Invalid Server Mod Information").ToUpper();
                            Presence_Launcher.Status(10);
                            Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            Button_Login_Logout_Modes(true, true);
                            Display_Color_Icons();
                            ServerModInfo = string.Empty;
                            return;
                        }
                        else
                        {
                            /* get files now */
                            json2 = JsonConvert.DeserializeObject<GetModInfo>(ServerModInfo);
                            ServerModInfo = string.Empty;

                            /* Set and Get for RemoteRPC Files */
#pragma warning disable CS8602 // Null Safe Check Done Before This Section
                            Uri URLCall_A = new Uri(json2.basePath + "/cars.json");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                            ServicePointManager.FindServicePoint(URLCall_A).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                    Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
                            var CarsJson = new WebClient
                            {
                                Encoding = Encoding.UTF8,
                                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                            };
                            if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                            { 
                                CarsJson = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) }; 
                            }
                            else
                            {
                                CarsJson.Headers.Add("user-agent", "SBRW Launcher " +
                                Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            }

                            try
                            {
                                remoteCarsFile = CarsJson.DownloadString(URLCall_A);
                            }
                            catch { }
                            finally
                            {
                                CarsJson?.Dispose();
                            }

                            Uri URLCall_B = new Uri(json2.basePath + "/events.json");
                            ServicePointManager.FindServicePoint(URLCall_B).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                    Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
                            var EventsJson = new WebClient
                            {
                                Encoding = Encoding.UTF8,
                                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                            };
                            if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                            { 
                                EventsJson = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) }; 
                            }
                            else
                            {
                                EventsJson.Headers.Add("user-agent", "SBRW Launcher " +
                                Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            }

                            try
                            {
                                remoteEventsFile = EventsJson.DownloadString(URLCall_B);
                            }
                            catch { }
                            finally
                            {
                                EventsJson?.Dispose();
                            }

                            /* Version 1.3 @metonator - DavidCarbon */
                            if (remoteCarsFile.Valid_Json())
                            {
                                Log.Info("DISCORD: Found RemoteRPC List for cars.json");
                                Cars.List_File = remoteCarsFile;
                                remoteCarsFile = string.Empty;
                            }
                            else
                            {
                                Log.Warning("DISCORD: RemoteRPC list for cars.json does not exist");
                                Cars.List_File = string.Empty;
                            }

                            if (remoteEventsFile.Valid_Json())
                            {
                                Log.Info("DISCORD: Found RemoteRPC List for events.json");
                                SBRW.Launcher.Core.Discord.Reference_.List_.Events.List_File = remoteEventsFile;
                                remoteEventsFile = string.Empty;
                            }
                            else
                            {
                                Log.Warning("DISCORD: RemoteRPC list for events.json does not exist");
                                SBRW.Launcher.Core.Discord.Reference_.List_.Events.List_File = string.Empty;
                            }

                            Log.Core("CORE: Loading Server Mods List");
                            /* Get Server Mod Index */
                            Uri newIndexFile = new Uri(json2.basePath + "/index.json");
                            ServicePointManager.FindServicePoint(newIndexFile).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                    Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
                            var ServerModsList = new WebClient
                            {
                                Encoding = Encoding.UTF8,
                                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                            };
                            if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                            { 
                                ServerModsList = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) }; 
                            }
                            else
                            {
                                ServerModsList.Headers.Add("user-agent", "SBRW Launcher " +
                                Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            }

                            try
                            {
                                Log.Core("CORE: Retrieved Server Mods List");
                                ServerModListJSON = ServerModsList.DownloadString(newIndexFile);
                                Label_Download_Information.Text = ("JSON: Retrieved Server Mod List Information").ToUpper();
                            }
                            catch (Exception Error)
                            {
                                Display_Color_Icons(2);
                                Label_Download_Information.Text = ("JSON: Unable to Retrieve Server Mod List Information").ToUpper();
                                Presence_Launcher.Status(10);
                                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                                string LogMessage = "There was an error with Server Mod List Information Retrieval:";
                                LogToFileAddons.OpenLog("SERVER MOD JSON", LogMessage, Error, "Error", false);
                                Button_Login_Logout_Modes(true, true);
                                Display_Color_Icons();
                            }
                            finally
                            {
                                ServerModsList?.Dispose();
                            }

                            if (string.IsNullOrWhiteSpace(ServerModListJSON) || !ServerModListJSON.Valid_Json())
                            {
                                Display_Color_Icons(2);
                                Label_Download_Information.Text = ("JSON: Invalid Server Mod List Information").ToUpper();
                                Presence_Launcher.Status(10, null);
                                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                                Button_Login_Logout_Modes(true, true);
                                Display_Color_Icons();
                                ServerModListJSON = string.Empty;
                                return;
                            }
                            else
                            {
                                try
                                {
                                    Label_Download_Information.Text = ("Server Mods: Folder & File Check").ToUpper();
                                    json3 = JsonConvert.DeserializeObject<ServerModList>(ServerModListJSON);
                                    ServerModListJSON = string.Empty;
                                    string ModFolderCache = Path.Combine(Save_Settings.Live_Data.Game_Path, "MODS", Hashes.Hash_String(0, json2.serverID).ToLower());
                                    if (!Directory.Exists(ModFolderCache))
                                    {
                                        Directory.CreateDirectory(ModFolderCache);
                                    }

                                    string[] Directory_Cache_List_Files = new string[] { };

                                    await Task.Run(() =>
                                    {
                                        Directory_Cache_List_Files = Directory.GetFiles(ModFolderCache);
                                    });

                                    if (Directory_Cache_List_Files.Length > 0)
                                    {
                                        /* (FILENAME.mods) 
                                     * Checks for any Files that Don't match the Server Index Json and Removes that File  */
                                        foreach (string file in Directory_Cache_List_Files)
                                        {
                                            string name = Path.GetFileName(file);

#pragma warning disable CS8602 // Null Safe Check Done Before This Section
                                            if (json3.entries.All(en => en.Name != name))
                                            {
                                                try
                                                {
                                                    File.Delete(file);
                                                    Log.Core("LAUNCHER: Removed Stale Mod Package: " + file);
                                                }
                                                catch (Exception Error)
                                                {
                                                    LogToFileAddons.OpenLog("SERVER MOD CACHE", string.Empty, Error, string.Empty, true);
                                                }
                                            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                                        }
                                    }
                                    
                                    Label_Download_Information.Text = ("Server Mods: Folder & File Check").ToUpper();
                                    /* (OLD-FILENAME.mods != NEW-FILENAME.mods)
                                     * Checks for the file and if the File Hash does not match it will be added to a list to be downloaded 
                                     * If a file exists and doesn't match a the server provided index json it will be deleted 
                                     * 5/22/2021: If a Server Extracted Mods Directory is present and 
                                     * if a Server Mod File no longer matches it will now delete the folder (.data/SERVER-ID-HASH) - DavidCarbon
                                     */
                                    int ExtractedServerFolderRunTime = 0;

#pragma warning disable CS8602 // Null Safe Check Done Before This Section
                                    foreach (ServerModFileEntry modfile in json3.entries)
                                    {
                                        string ModCachedFile = Path.Combine(ModFolderCache, modfile.Name);
                                        string Mod_File_Hash_Local = string.Empty;

                                        await Task.Run(() =>
                                        {
                                            Mod_File_Hash_Local = Hashes.Hash_SHA(ModCachedFile).ToLowerInvariant();
                                        });
                                        

                                        if (!Mod_File_Hash_Local.Equals(modfile.Checksum))
                                        {
                                            try
                                            {
                                                if (ExtractedServerFolderRunTime == 0)
                                                {
                                                    string ExtractedServerFolder = Path.Combine(Save_Settings.Live_Data.Game_Path, ".data", Hashes.Hash_String(0, json2.serverID).ToLower());
                                                    if (Directory.Exists(ExtractedServerFolder))
                                                    {
                                                        Directory.Delete(ExtractedServerFolder, true);
                                                        Log.Core("LAUNCHER: Removed Extracted Server Mods Folder: .data/" + Hashes.Hash_String(0, json2.serverID).ToLower());
                                                    }

                                                    ExtractedServerFolderRunTime++;
                                                }

                                                if (File.Exists(ModCachedFile))
                                                {
                                                    File.Delete(ModCachedFile);
                                                    Log.Core("LAUNCHER: Removed Old Mod Package: " + modfile.Name);
                                                }
                                            }
                                            catch (Exception Error)
                                            {
                                                LogToFileAddons.OpenLog("SERVER MOD CACHE FILE", string.Empty, Error, string.Empty, true);
                                            }

                                            ModFilesDownloadUrls.Enqueue(new Uri(json2.basePath + "/" + modfile.Name));
                                            TotalModFileCount++;
                                        }
                                        else
                                        {
                                            Label_Download_Information.Text = ("Server Mods: Up to Date " + modfile.Name).ToUpper();
                                        }
                                    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                                    if(!(Disposing || IsDisposed))
                                    {
                                        if (ModFilesDownloadUrls.Count != 0)
                                        {
                                            this.DownloadModNetFilesRightNow(ModFolderCache);
                                            Presence_Launcher.Status(9);
                                        }
                                        else
                                        {
                                            Game_Check_Launch();
                                        }
                                    }
                                }
                                catch (Exception Error)
                                {
                                    Display_Color_Icons(2);
                                    Presence_Launcher.Status(10);
                                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                                    string LogMessage = "There was an error with Server Mods Check:";
                                    LogToFileAddons.OpenLog("SERVER MOD DOWNLOAD", LogMessage, Error, "Error", false);
                                    Button_Login_Logout_Modes(true, true);
                                    Display_Color_Icons();
                                    return;
                                }
                                finally
                                {
                                    if (!string.IsNullOrWhiteSpace(ModulesJSON))
                                    {
                                        ModulesJSON = string.Empty;
                                    }
                                    if (!string.IsNullOrWhiteSpace(ServerModInfo))
                                    {
                                        ServerModInfo = string.Empty;
                                    }
                                    if (json2 != null)
                                    {
                                        json2 = null;
                                    }
                                    if (!string.IsNullOrWhiteSpace(remoteCarsFile))
                                    {
                                        remoteCarsFile = string.Empty;
                                    }
                                    if (!string.IsNullOrWhiteSpace(remoteEventsFile))
                                    {
                                        remoteEventsFile = string.Empty;
                                    }
                                    if (!string.IsNullOrWhiteSpace(ServerModListJSON))
                                    {
                                        ServerModListJSON = string.Empty;
                                    }
                                    if (json3 != null)
                                    {
                                        json3 = null;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception Error)
                {
                    Display_Color_Icons(2);
                    Presence_Launcher.Status(8);
                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    string LogMessage = "There was an error downloading ModNet Files:";
                    LogToFileAddons.OpenLog("MODNET FILES", LogMessage, Error, "Error", false);
                    Button_Login_Logout_Modes(true, true);
                    Display_Color_Icons();
                    return;
                }
                finally
                {
                    if (!string.IsNullOrWhiteSpace(ModulesJSON))
                    {
                        ModulesJSON = string.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(ServerModInfo))
                    {
                        ServerModInfo = string.Empty;
                    }
                    if (json2 != null)
                    {
                        json2 = null;
                    }
                    if (!string.IsNullOrWhiteSpace(remoteCarsFile))
                    {
                        remoteCarsFile = string.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(remoteEventsFile))
                    {
                        remoteEventsFile = string.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(ServerModListJSON))
                    {
                        ServerModListJSON = string.Empty;
                    }
                    if (json3 != null)
                    {
                        json3 = null;
                    }
                }
            }
            else
            {
                Game_Check_Launch();
            }
        }

#region Game Server Information Download
        private void ComboBox_Server_List_SelectedIndexChanged(object sender, EventArgs e)
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
            GC.Collect(); 
#endif
            if (Picture_Input_Email.Image != Image_Other.Text_Border_Email)
            {
                Picture_Input_Email.Image = Image_Other.Text_Border_Email;
            }
            if (Picture_Input_Password.Image != Image_Other.Text_Border_Password)
            {
                Picture_Input_Password.Image = Image_Other.Text_Border_Password;
            }
            /* Disable Certain Functions */
            LoginEnabled = ServerEnabled = FunctionStatus.AllowRegistration = false;
            Launcher_Value.Launcher_Select_Server_JSON = null;
            /* Disable Login & Register Button */
            Button_Login.Enabled = Button_Register.Enabled = false;
            /* Disable Social Panel when switching */
            DisableSocialPanelandClearIt();

            if (!ServerListUpdater.LoadedList && Launcher_Value.Launcher_Select_Server_Data == null)
            {
                Label_Status_Game_Server.Text = "Launcher Offline:\n - Unknown";
                Label_Status_Game_Server.ForeColor = Color_Text.L_Three;
                Label_Status_Game_Server_Data.Text = string.Empty;
                if (Picture_Icon_Server.BackgroundImage != Image_Icon.Server_Unknown)
                {
                    Picture_Icon_Server.BackgroundImage = Image_Icon.Server_Unknown;
                }
                return;
            }

            /* Stops any actions for a Server by comparing Live Instance of the Selected Cache */
            Json_List_Server Cached_Server_GSI = Launcher_Value.Launcher_Select_Server_Data = (Json_List_Server)ComboBox_Server_List.SelectedItem;

            if (Launcher_Value.Launcher_Select_Server_Data.IsSpecial)
            {
                ComboBox_Server_List.SelectedIndex = LastSelectedServerId;
                return;
            }

            if (!SkipServerTrigger) { return; }

            LastSelectedServerId = ComboBox_Server_List.SelectedIndex;

            Label_Status_Game_Server.Text = "Server Status:\n - Pinging";
            Label_Status_Game_Server.ForeColor = Color_Text.L_Two;
            Label_Status_Game_Server_Data.Text = string.Empty;
            if (Picture_Icon_Server.BackgroundImage != Image_Icon.Server_Checking)
            {
                Picture_Icon_Server.BackgroundImage = Image_Icon.Server_Checking;
            }

            Button_Login.ForeColor = Color_Text.L_Six;
            string Banner_Cache_Folder = Path.Combine(Locations.LauncherDataFolder, "Bin", "Server", "Banner", "EyeCatcher");
            string Banner_Cache_File = Path.Combine(Banner_Cache_Folder, Hashes.Hash_String(1, Launcher_Value.Launcher_Select_Server_Data.IPAddress) + ".bin");
            Picture_Server_Banner.Image = Image_Handler.Grayscale(Banner_Cache_File) ?? Image_Other.Server_Banner;
            Picture_Server_Banner.BackColor = Color.Transparent;
            string ImageUrl = string.Empty;
            string numPlayers = string.Empty;
            string numRegistered = string.Empty;

            if (ComboBox_Server_List.GetItemText(ComboBox_Server_List.SelectedItem) == "Offline Built-In Server")
            {
                Builtinserver = true;
                if (Button_Login.BackgroundImage != Image_Button.Grey)
                {
                    Button_Login.BackgroundImage = Image_Button.Grey;
                }
                Button_Login.Text = "Launch".ToUpper();
                Button_Login.ForeColor = Color_Text.L_Five;
                Panel_Server_Information.Visible = false;
            }
            else
            {
                Builtinserver = false;
                if (Button_Login.BackgroundImage != Image_Button.Grey)
                {
                    Button_Login.BackgroundImage = Image_Button.Grey;
                }
                Button_Login.Text = "Login".ToUpper();
                Button_Login.ForeColor = Color_Text.L_Six;
                Panel_Server_Information.Visible = false;
            }

            Uri ServerURI = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress + "/GetServerInformation");
            ServicePointManager.FindServicePoint(ServerURI).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
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

            Client.DownloadStringAsync(ServerURI);

            System.Timers.Timer aTimer = new System.Timers.Timer(Launcher_Value.Launcher_WebCall_Timeout_Enable ? Launcher_Value.Launcher_WebCall_Timeout() * 1000 : 10000);
            aTimer.Elapsed += (x, y) => 
            {
                if (Client != default)
                {
                    Client.CancelAsync();
                }

                try
                {
                    if (aTimer != default)
                    {
                        if (aTimer.Enabled)
                        {
                            aTimer.Stop();
                            aTimer.Dispose();
                        }
                    }
                }
#if (DEBUG || DEBUG_UNIX)
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("aTimer", string.Empty, Error, string.Empty, true);
                }
#else
                catch
                {

                }
#endif
            };
            aTimer.AutoReset = false;
            aTimer.Enabled = true;

            Client.DownloadStringCompleted += (sender2, e2) =>
            {
                try 
                {
                    if(aTimer.Enabled)
                    {
                        aTimer.Stop();
                        aTimer.Dispose();
                    } 
                }
#if (DEBUG || DEBUG_UNIX)
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("aTimer [Download Complete]", string.Empty, Error, string.Empty, true);
                }
#else
                catch
                {

                }
#endif

                bool GSIErrorFree = true;

                if (e2.Cancelled || e2.Error != null)
                {
                    Launcher_Value.Launcher_Select_Server_JSON = null;
                    if (Picture_Icon_Server.BackgroundImage != Image_Icon.Server_Offline)
                    {
                        Picture_Icon_Server.BackgroundImage = Image_Icon.Server_Offline;
                    }
                    Label_Status_Game_Server.Text = "Server Status:\n - Offline ( OFF )";
                    Label_Status_Game_Server.ForeColor = Color_Text.S_Error;
                    Label_Status_Game_Server_Data.Text = (e2.Error != null) ?
                    (e2.Error.Message ?? "Server Seems to be Offline").Encode_UTF8() : "Failed to Connect to Server";

                    if (!InformationCache.ServerStatusBook.ContainsKey(Launcher_Value.Launcher_Select_Server_Data.ID))
                    {
                        InformationCache.ServerStatusBook.Add(Launcher_Value.Launcher_Select_Server_Data.ID, (e2.Error != null) ? 0 : 3);
                    }
                    else
                    {
                        InformationCache.ServerStatusBook[Launcher_Value.Launcher_Select_Server_Data.ID] = (e2.Error != null) ? 0 : 3;
                    }

                    if (e2.Error != null)
                    {
                        LogToFileAddons.OpenLog("JSON GSI", string.Empty, e2.Error, string.Empty, true);
                    }

                    Client?.Dispose();
                }
                else if (Cached_Server_GSI.Equals(Launcher_Value.Launcher_Select_Server_Data))
                {
                    if (ServerListUpdater.ServerName("Ping") == "Offline Built-In Server")
                    {
                        numPlayers = "∞";
                        numRegistered = "∞";
                    }
                    else
                    {
                        try
                        {
                            Launcher_Value.Launcher_Select_Server_JSON = JsonConvert.DeserializeObject<Json_Server_Info>(e2.Result);
                        }
                        catch (Exception Error)
                        {
                            if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                            {
                                try { Log.Error("JSON GSI (Received): " + e2.Result); }
                                catch { Log.Error("JSON GSI (Received): Unable to Get Result"); }
                            }
                            else
                            {
                                Log.Error("JSON GSI: Invalid");
                            }

                            LogToFileAddons.OpenLog("JSON GSI", string.Empty, Error, string.Empty, true);
                            GSIErrorFree = false;
                            Launcher_Value.Launcher_Select_Server_JSON = null;
                        }

                        if (!InformationCache.ServerStatusBook.ContainsKey(Launcher_Value.Launcher_Select_Server_Data.ID))
                        {
                            InformationCache.ServerStatusBook.Add(Launcher_Value.Launcher_Select_Server_Data.ID, (!GSIErrorFree) ? 3 : 1);
                        }
                        else
                        {
                            InformationCache.ServerStatusBook[Launcher_Value.Launcher_Select_Server_Data.ID] = (!GSIErrorFree) ? 3 : 1;
                        }

                        if (GSIErrorFree && (Launcher_Value.Launcher_Select_Server_JSON != null))
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Banner))
                                {
                                    bool ServerBannerResult;

                                    try
                                    {
                                        ServerBannerResult = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Banner, UriKind.Absolute, out Uri? uriResult) &&
                                        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                    }
                                    catch
                                    {
                                        ServerBannerResult = false;
                                    }

                                    ImageUrl = ServerBannerResult ? Launcher_Value.Launcher_Select_Server_JSON.Server_Banner : string.Empty;
                                }
                                else
                                {
                                    ImageUrl = string.Empty;
                                }
                            }
                            catch
                            {

                            }

                            /* Social Panel Core */

                            /* Discord Invite Display */
                            try
                            {
                                bool ServerDiscordLink;
                                try
                                {
                                    ServerDiscordLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord, UriKind.Absolute, out Uri? uriResult) &&
                                                             (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch
                                {
                                    ServerDiscordLink = false;
                                }
                                if (Picture_Icon_Server_Discord.BackgroundImage != (ServerDiscordLink ? Image_Icon.Discord : Image_Icon.Discord_Disabled))
                                {
                                    Picture_Icon_Server_Discord.BackgroundImage = ServerDiscordLink ? Image_Icon.Discord : Image_Icon.Discord_Disabled;
                                }
                                LinkLabel_Server_Discord.Enabled = ServerDiscordLink;
                                LinkLabel_Server_Discord.Text = ServerDiscordLink ? "Discord Invite" : string.Empty;
                            }
                            catch
                            {

                            }

                            /* Homepage Display */
                            try
                            {
                                bool ServerWebsiteLink;
                                try
                                {
                                    ServerWebsiteLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home, UriKind.Absolute, out Uri? uriResult) &&
                                              (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch
                                {
                                    ServerWebsiteLink = false;
                                }
                                if (Picture_Icon_Server_Home.BackgroundImage != (ServerWebsiteLink ? Image_Icon.Home : Image_Icon.Home_Disabled))
                                {
                                    Picture_Icon_Server_Home.BackgroundImage = ServerWebsiteLink ? Image_Icon.Home : Image_Icon.Home_Disabled;
                                }
                                LinkLabel_Server_Home.Enabled = ServerWebsiteLink;
                                LinkLabel_Server_Home.Text = ServerWebsiteLink ? "Home Page" : string.Empty;
                            }
                            catch
                            {

                            }

                            /* Facebook Group Display */
                            try
                            {
                                bool ServerFacebookLink;
                                try
                                {
                                    ServerFacebookLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Facebook, UriKind.Absolute, out Uri? uriResult) &&
                                                         (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch
                                {
                                    ServerFacebookLink = false;
                                }
                                if (Picture_Icon_Server_Facebook.BackgroundImage != (ServerFacebookLink ? Image_Icon.Facebook : Image_Icon.Facebook_Disabled))
                                {
                                    Picture_Icon_Server_Facebook.BackgroundImage = ServerFacebookLink ? Image_Icon.Facebook : Image_Icon.Facebook_Disabled;
                                }
                                LinkLabel_Server_Facebook.Enabled = ServerFacebookLink;
                                LinkLabel_Server_Facebook.Text = ServerFacebookLink ? "Facebook Page" : string.Empty;
                            }
                            catch
                            {

                            }

                            /* Twitter Account Display */
                            try
                            {
                                bool ServerTwitterLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Twitter, UriKind.Absolute, out Uri? uriResult) &&
                                                         (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                if (Picture_Icon_Server_Twitter.BackgroundImage != (ServerTwitterLink ? Image_Icon.Twitter : Image_Icon.Twitter_Disabled))
                                {
                                    Picture_Icon_Server_Twitter.BackgroundImage = ServerTwitterLink ? Image_Icon.Twitter : Image_Icon.Twitter_Disabled;
                                }
                                LinkLabel_Server_Twitter.Enabled = ServerTwitterLink;
                                LinkLabel_Server_Twitter.Text = ServerTwitterLink ? "Twitter Feed" : string.Empty;
                            }
                            catch
                            {

                            }

                            /* Server Set Speedbug Timer Display */
                            try
                            {
                                ServerSecondsToShutDown =
                                (Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer != 0) ? Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer : 7200;
                                Label_Server_Force_Restart_Timer.Text = string.Format(Translations.Database("MainScreen_Text_ServerShutDown") +
                                    " " + Time_Conversion.RelativeTime(ServerSecondsToShutDown));
                            }
                            catch
                            {

                            }

                            try
                            {
                                string SceneryStatus = string.Join("", Launcher_Value.Launcher_Select_Server_JSON.Server_Active_Scenery) switch
                                {
                                    "SCENERY_GROUP_NEWYEARS" => "Scenery: New Years",
                                    "SCENERY_GROUP_VALENTINES" => "Scenery: Valentines",
                                    "SCENERY_GROUP_OKTOBERFEST" => "Scenery: Oktoberfest",
                                    "SCENERY_GROUP_HALLOWEEN" => "Scenery: Halloween",
                                    "SCENERY_GROUP_CHRISTMAS" => "Scenery: Christmas",
                                    _ => "Scenery: Normal",
                                };
                                Label_Server_Scenery.Text = SceneryStatus;
                            }
                            catch
                            {

                            }

                            /* Check Selected Server Address for since nfsw currently requires being proxied for https */
                            if (Launcher_Value.Launcher_Select_Server_Data.IPAddress.StartsWith("https"))
                            {
                                if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                                {
                                    Log.Debug("Server is Https");
                                }
                                /* This is in case it is not listed in GSI at all || is present in GSI and is set to true */
                                if ((!Launcher_Value.Launcher_Select_Server_JSON.Server_Proxy_Forced) || (Launcher_Value.Launcher_Select_Server_JSON.Server_Proxy_Forced != false))
                                {   /* So we can force the Proxy On even if a User has Disabled it */
                                    InformationCache.SelectedServerEnforceProxy = true;
                                    if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                                    {
                                        Log.Debug("Server is Https: Case 1");
                                    }
                                }
                                /* but still allow that nfsw might get patched to do https raw? */
                                else if (Launcher_Value.Launcher_Select_Server_JSON.Server_Proxy_Forced == false)
                                {   /* In which case, respect the GSI set value */
                                    InformationCache.SelectedServerEnforceProxy = false;
                                    if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                                    {
                                        Log.Debug("Server is Https: Case 2 (Never Gets Touched Because of First If Statement)");
                                    }
                                }
                            }
                            /* If it's an HTTP Server, check if Proxy is being requested as Enforced On */
                            else if (Launcher_Value.Launcher_Select_Server_JSON.Server_Proxy_Forced != true)
                            {   /* This is set so that it doesn't try to enforce Proxy On if user switches
                                 * to a server that doesn't have enforceLauncherProxy set or true */
                                InformationCache.SelectedServerEnforceProxy = false;
                                if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                                {
                                    Log.Debug("Server is not Https: Case 3");
                                }
                            }

                            Launcher_Value.Launcher_Server_Crew_Tags = Launcher_Value.Launcher_Select_Server_JSON.Server_Enable_Crew_Tags;

                            if (Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Peak != 0)
                            {
                                numPlayers = string.Format("{0} / {1}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online, Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Peak);
                                numRegistered = string.Format("{0}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Registered);
                            }
                            else if (Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Max != 0)
                            {
                                numPlayers = string.Format("{0} / {1}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online, Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Max);
                                numRegistered = string.Format("{0}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Registered);
                            }
                            else if ((Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Max == 0) || (Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online_Peak == 0))
                            {
                                numPlayers = string.Format("{0}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Online);
                                numRegistered = string.Format("{0}", Launcher_Value.Launcher_Select_Server_JSON.Server_User_Registered);
                            }

                            FunctionStatus.AllowRegistration = true;
                        }
                    }

                    if (!GSIErrorFree)
                    {
                        try
                        {
                            Label_Status_Game_Server.Text = "Server Connection:\n - Unstable";
                            Label_Status_Game_Server.ForeColor = Color_Text.S_Warning;
                            Label_Status_Game_Server_Data.Text = "Received Invalid JSON Game Server Info.";
                            if (Picture_Icon_Server.BackgroundImage != Image_Icon.Server_Warning)
                            {
                                Picture_Icon_Server.BackgroundImage = Image_Icon.Server_Warning;
                            }
                        }
                        catch
                        {
                            /* Sad Noises */
                        }
                    }
                    else
                    {
                        try
                        {
                            Label_Status_Game_Server.Text = "Server Status:\n - Online ( ON )";
                            Label_Status_Game_Server.ForeColor = Color_Text.S_Sucess;
                            if (Picture_Icon_Server.BackgroundImage != Image_Icon.Server_Online)
                            {
                                Picture_Icon_Server.BackgroundImage = Image_Icon.Server_Online;
                            }
                            /* Enable Login & Register Button */
                            LoginEnabled = true;
                            Button_Login.ForeColor = Color_Text.L_Five;
                            Button_Login.Enabled = !IsDownloading;
                            Button_Register.Enabled = true;
                            Launcher_Value.Launcher_Select_Server_Category = ((Json_List_Server)ComboBox_Server_List.SelectedItem).Category ?? string.Empty;

                            if (Launcher_Value.Launcher_Select_Server_Category.ToUpper() == "DEV" ||
                            Launcher_Value.Launcher_Select_Server_Category.ToUpper() == "OFFLINE")
                            {
                                /* Disable Social Panel */
                                DisableSocialPanelandClearIt();
                            }
                            else
                            {
                                /* Enable Social Panel  */
                                Panel_Server_Information.Visible = true;
                            }
                        }
                        catch
                        {
                            /* ¯\_(ツ)_/¯ */
                        }
                        finally
                        {
                            Client?.Dispose();
                        }

                        /* For Thread Safety */
                        try
                        {
                            Label_Status_Game_Server_Data.Text = string.Format("Online: {0}\nRegistered: {1}", numPlayers, numRegistered);
                        }
                        catch
                        {

                        }

                        Server_Ping(ServerURI.Host, 5000);

                        ServerEnabled = true;

                        if (Cached_Server_GSI.Equals(Launcher_Value.Launcher_Select_Server_Data))
                        {
                            try
                            {
                                if (!Directory.Exists(Banner_Cache_Folder)) { Directory.CreateDirectory(Banner_Cache_Folder); }

                                if (!string.IsNullOrWhiteSpace(ImageUrl))
                                {

                                    Uri URICall_A = new Uri(ImageUrl);
                                    ServicePointManager.FindServicePoint(URICall_A).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                        Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
                                    var Client_A = new WebClient
                                    {
                                        Encoding = Encoding.UTF8,
                                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                                    };
                                    if (!Launcher_Value.Launcher_Alternative_Webcalls())
                                    {
                                        Client_A = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) };
                                    }
                                    else
                                    {
                                        Client_A.Headers.Add("user-agent", "SBRW Launcher " +
                                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                    }

                                    Client_A.DownloadDataAsync(URICall_A);
                                    Client_A.DownloadProgressChanged += (Object_A, Events_A) =>
                                    {
                                        if (!Cached_Server_GSI.Equals(Launcher_Value.Launcher_Select_Server_Data))
                                        {
                                            Client_A.CancelAsync();
                                            Log.Info("BANNER: Stopping " + ServerListUpdater.ServerName("Ping") + " Server Banner Download");
                                        }
                                        else if (Events_A.TotalBytesToReceive > 2000000)
                                        {
                                            Client_A.CancelAsync();
                                            Log.Warning("BANNER: Unable to Cache " + ServerListUpdater.ServerName("Ping") + " Server Banner! {Over 2MB?}");
                                        }
                                    };

                                    Client_A.DownloadDataCompleted += (Object_A, Events_A) =>
                                    {
                                        if (Events_A.Cancelled)
                                        {
                                            Client_A?.Dispose();
                                        }
                                        else if (Cached_Server_GSI.Equals(Launcher_Value.Launcher_Select_Server_Data))
                                        {
                                            if (Events_A.Error != null)
                                            {
                                                if (Cached_Server_GSI.Equals(Launcher_Value.Launcher_Select_Server_Data))
                                                {
                                                    /* Load cached banner! */
                                                    if (Picture_Server_Banner.Image != (Image_Handler.Grayscale(Banner_Cache_File) ?? Image_Other.Server_Banner))
                                                    {
                                                        Picture_Server_Banner.Image = Image_Handler.Grayscale(Banner_Cache_File) ?? Image_Other.Server_Banner;
                                                    }
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                                    GC.Collect();
#endif
                                                }

                                                Client_A?.Dispose();
                                            }
                                            else if (Cached_Server_GSI.Equals(Launcher_Value.Launcher_Select_Server_Data) && Events_A.Result != null)
                                            {
                                                try
                                                {
                                                    try
                                                    {
                                                        if (ServerRawBanner != null)
                                                        {
                                                            ServerRawBanner.Close();
                                                            ServerRawBanner.Dispose();
                                                        }
                                                    }
                                                    catch { }

                                                    ServerRawBanner = new MemoryStream(Events_A.Result)
                                                    {
                                                        Position = 0
                                                    };

                                                    if (Picture_Server_Banner.Image != (Image.FromStream(ServerRawBanner) ?? Image_Other.Server_Banner))
                                                    {
                                                        Picture_Server_Banner.Image = Image.FromStream(ServerRawBanner) ?? Image_Other.Server_Banner;
                                                    }

                                                    if (Strings.GetExtension(ImageUrl) == "gif")
                                                    {
                                                        Image.FromStream(ServerRawBanner).Save(Banner_Cache_File);
                                                    }
                                                    else
                                                    {
                                                        File.WriteAllBytes(Banner_Cache_File, ServerRawBanner.ToArray());
                                                    }
                                                }
                                                catch (Exception Error)
                                                {
                                                    LogToFileAddons.OpenLog("Server Banner", string.Empty, Error, string.Empty, true);
                                                    Picture_Server_Banner.BackColor = Color_Winform_Other.Server_Banner_BackColor;
                                                }
                                                finally
                                                {
                                                    Client_A?.Dispose();

#if !(RELEASE_UNIX || DEBUG_UNIX)
                                                    GC.Collect();
#endif
                                                }
                                            }
                                        }
                                    };
                                }
                                else if (File.Exists(Banner_Cache_File) && !(Application.OpenForms[this.Name].IsDisposed || Application.OpenForms[this.Name].Disposing))
                                {
                                    /* Load cached banner! */
                                    if (Picture_Server_Banner.Image != (Image_Handler.Grayscale(Banner_Cache_File) ?? Image_Other.Server_Banner))
                                    {
                                        Picture_Server_Banner.Image = Image_Handler.Grayscale(Banner_Cache_File) ?? Image_Other.Server_Banner;
                                    }
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                    GC.Collect();
#endif
                                }
                                else if (!Application.OpenForms[this.Name].IsDisposed)
                                {
                                    Picture_Server_Banner.BackColor = Color_Winform_Other.Server_Banner_BackColor;
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                    GC.Collect();
#endif
                                }
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("BANNER Cache", string.Empty, Error, string.Empty, true);
                            }
                        }
                    }
                }
                else
                {
                    /* Just ingore this. */
                }

                if (Application.OpenForms[this.Name] != null)
                {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                    GC.Collect(); 
#endif
                }
            };

#if !(RELEASE_UNIX || DEBUG_UNIX)
            GC.Collect(); 
#endif
        }
#endregion

#region Game Files Downloader Components (LZMA [.dat])

        public static void RemoveTracksHighFiles()
        {
            try
            {
                string SpecificTracksHighPath = Path.Combine(Save_Settings.Live_Data.Game_Path, "TracksHigh");
                if (File.Exists(Path.Combine(SpecificTracksHighPath, "STREAML5RA_98.BUN")))
                {
                    Directory.Delete(SpecificTracksHighPath, true);
                }
            }
            catch { }
        }

        public void DownloadTracksFiles()
        {
            if (Screen_Instance != null && !(IsDisposed || Disposing))
            {
                if (UI_MODE != 3)
                {
                    UI_MODE = 3;
                }

                if (Label_Download_Information.InvokeRequired)
                {
                    Label_Download_Information.Invoke(new Action(delegate ()
                    {
                        Label_Download_Information_Support.Text = "Checking Tracks Files...".ToUpper();
                    }));
                }
                else
                {
                    Label_Download_Information_Support.Text = "Checking Tracks Files...".ToUpper();
                }

                string SpecificTracksFilePath = Path.Combine(Save_Settings.Live_Data.Game_Path, "Tracks", "STREAML5RA_98.BUN");
                if (!File.Exists(SpecificTracksFilePath) && (LZMA_Downloader != null))
                {
                    if (Label_Download_Information.InvokeRequired)
                    {
                        Label_Download_Information.Invoke(new Action(delegate ()
                        {
                            Label_Download_Information_Support.Text = "Downloading: Tracks Data".ToUpper();
                        }));
                    }
                    else
                    {
                        Label_Download_Information_Support.Text = "Downloading: Tracks Data".ToUpper();
                    }

                    Log.Info("DOWNLOAD: Getting Tracks Folder");
                    Download_Settings.Alternative_WebCalls = Launcher_Value.Launcher_Alternative_Webcalls();
                    LZMA_Downloader.StartDownload(Save_Settings.Live_Data.Launcher_CDN, "Tracks", Save_Settings.Live_Data.Game_Path, false, false, 615494528);
                }
                else
                {
                    DownloadSpeechFiles();
                }
            }
            else
            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                GC.Collect(); 
#endif
            }
        }

        public void DownloadSpeechFiles()
        {
            if (Screen_Instance != null && !(IsDisposed || Disposing))
            {
                string speechFile = string.Empty;
                int speechSize = 0;

                if (UI_MODE != 3)
                {
                    UI_MODE = 3;
                }

                if (Label_Download_Information.InvokeRequired)
                {
                    Label_Download_Information.Invoke(new Action(delegate ()
                    {
                        Label_Download_Information_Support.Text = "Looking for correct Speech Files...".ToUpper();
                    }));
                }
                else
                {
                    Label_Download_Information_Support.Text = "Looking for correct Speech Files...".ToUpper();
                }

                try
                {
                    speechFile = Translations.Speech_Files(Save_Settings.Live_Data.Launcher_Language);

                    Uri URLCall = new Uri(Save_Settings.Live_Data.Launcher_CDN + "/" + speechFile + "/index.xml");
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
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
                        string response = Client.DownloadString(URLCall);

                        XmlDocument speechFileXml = new XmlDocument();
                        speechFileXml.LoadXml(response);

                        if (speechFileXml != default)
                        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                            XmlNode speechSizeNode = speechFileXml.SelectSingleNode("index/header/compressed");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                            speechSize = Convert.ToInt32(speechSizeNode.InnerText);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        }
                        else
                        {
                            speechFile = Translations.Speech_Files(InformationCache.Lang.ThreeLetterISOLanguageName);
                            speechSize = Translations.Speech_Files_Size();
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        Client?.Dispose();
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Download Speech Files", string.Empty, Error, string.Empty, true);
                    speechFile = Translations.Speech_Files(InformationCache.Lang.ThreeLetterISOLanguageName ?? string.Empty);
                    speechSize = Translations.Speech_Files_Size();
                }

                if (Label_Download_Information.InvokeRequired)
                {
                    Label_Download_Information.Invoke(new Action(delegate ()
                    {
                        Label_Download_Information_Support.Text = string.Format("Checking for {0} Speech Files.", speechFile).ToUpper();
                    }));
                }
                else
                {
                    Label_Download_Information_Support.Text = string.Format("Checking for {0} Speech Files.", speechFile).ToUpper();
                }

                string SoundSpeechPath = Path.Combine(Save_Settings.Live_Data.Game_Path, "Sound", "Speech", "copspeechsth_" + speechFile + ".big");
                if (!File.Exists(SoundSpeechPath) && LZMA_Downloader != null)
                {
                    if (Label_Download_Information.InvokeRequired)
                    {
                        Label_Download_Information.Invoke(new Action(delegate ()
                        {
                            Label_Download_Information_Support.Text = "Downloading: Language Audio".ToUpper();
                        }));
                    }
                    else
                    {
                        Label_Download_Information_Support.Text = "Downloading: Language Audio".ToUpper();
                    }

                    Log.Info("DOWNLOAD: Getting Speech/Audio Files");
                    Download_Settings.Alternative_WebCalls = Launcher_Value.Launcher_Alternative_Webcalls();
                    LZMA_Downloader.StartDownload(Save_Settings.Live_Data.Launcher_CDN, speechFile, Save_Settings.Live_Data.Game_Path, false, false, speechSize);
                }
                else
                {
                    if (Label_Download_Information.InvokeRequired)
                    {
                        Label_Download_Information.Invoke(new Action(delegate ()
                        {
                            Label_Download_Information_Support.Text = string.Empty;
                        }));
                    }
                    else
                    {
                        Label_Download_Information_Support.Text = string.Empty;
                    }

                    OnDownloadFinished();
                    Log.Info("DOWNLOAD: Game Files Download is Complete!");
                }
            }
            else
            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                GC.Collect(); 
#endif
            }
        }

        private void OnDownloadFinished()
        {
            try
            {
                if (LZMA_Downloader != null)
                {
                    if (LZMA_Downloader.Downloading)
                    {
                        LZMA_Downloader.Stop();
                    }
                }

                if (Pack_SBRW_Downloader != null)
                {
                    if (!Pack_SBRW_Downloader.Cancel)
                    {
                        Pack_SBRW_Downloader.Cancel = true;
                    }

                    if (Pack_SBRW_Unpacker != null)
                    {
                        if (!Pack_SBRW_Unpacker.Cancel)
                        {
                            Pack_SBRW_Unpacker.Cancel = true;
                        }
                    }
                }

                Pack_SBRW_Downloader_Unpack_Lock = false;
            }
            catch (Exception Error_Live)
            {
                LogToFileAddons.OpenLog("Progress Bar/Outline ODF", string.Empty, Error_Live, string.Empty, true);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                {
                    string GFX_BootFlow_File_Path = Path.Combine(Save_Settings.Live_Data.Game_Path, "GFX", "BootFlow.gfx");
                    
                    if (Hashes.Hash_SHA(GFX_BootFlow_File_Path) != "97ED41D1A44ACF58AF2613C243BDD88E8C3806EB")
                    {
                        if (File.Exists(GFX_BootFlow_File_Path))
                        {
                            File.Delete(GFX_BootFlow_File_Path);
                        }

                        File.WriteAllBytes(GFX_BootFlow_File_Path, Core.Theme.Conversion_.Embeded_Files.BootFlow_GFX_Bytes());
                    }
                }
            }
            catch (Exception Error_Live)
            {
                LogToFileAddons.OpenLog("BootFlow GFX File", string.Empty, Error_Live, string.Empty, true);
            }

            if (Save_Settings.Live_Data.Game_Integrity == "Unknown")
            {
                Save_Settings.Live_Data.Game_Integrity = "Good";
                Save_Settings.Save();
            }

            Presence_Launcher.Download = false;
            Presence_Launcher.Status(4);

            try
            {
                if (Screen_Instance != null && !(IsDisposed || Disposing))
                {
                    if (UI_MODE != 5)
                    {
                        UI_MODE = 5;
                    }

                    Label_Download_Information.SafeInvokeAction(() => Label_Download_Information.Text = "Ready!".ToUpper(), this);
                    Button_Login.Enabled = LoginEnabled;

                    EnablePlayButton();
                }
                else
                {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                    GC.Collect(); 
#endif
                }
            }
            catch (Exception Error_Live)
            {
                LogToFileAddons.OpenLog("Progress Bar/Outline ODF", string.Empty, Error_Live, string.Empty, true);
            }
        }

        private bool EnablePlayButton(bool Return_Value = false)
        {
            IsDownloading = false;
            Playenabled = true;

            return Return_Value;
        }

        private void OnDownloadFailed(Exception Error)
        {
            try
            {
                LZMA_Downloader?.Stop();

                if (Pack_SBRW_Downloader != null)
                {
                    if (!Pack_SBRW_Downloader.Cancel)
                    {
                        Pack_SBRW_Downloader.Cancel = true;
                    }

                    if (Pack_SBRW_Unpacker != null)
                    {
                        if (!Pack_SBRW_Unpacker.Cancel)
                        {
                            Pack_SBRW_Unpacker.Cancel = true;
                        }
                    }
                }

                Pack_SBRW_Downloader_Unpack_Lock = false;
            }
            catch (Exception Error_Live)
            {
                LogToFileAddons.OpenLog("Progress Bar/Outline ODF", string.Empty, Error_Live, string.Empty, true);
            }

            if (Screen_Instance != null && (!IsDisposed || !Disposing))
            {
                Presence_Launcher.Download = false;
                Presence_Launcher.Status(3);

                try
                {
                    if (UI_MODE != -1)
                    {
                        UI_MODE = -1;
                    }

                    Label_Download_Information.SafeInvokeAction(() => Label_Download_Information.Text = ((Error != null) ? Error.Message : "Download Failed. No Reason Provided").ToUpper(), this);

                    if (Error != null)
                    {
                        string TempEmailCache = string.Empty;
                        if (!string.IsNullOrWhiteSpace(Input_Email.Text))
                        {
                            TempEmailCache = Input_Email.Text;
                            Input_Email.SafeInvokeAction(() => Input_Email.Text = "EMAIL IS HIDDEN", this);
                        }

                        string LogMessage = "CDN Downloader Encountered an Error:";
                        LogToFileAddons.OpenLog("Game Download", LogMessage, Error, "Error", false, Screen_Instance);

                        if (!string.IsNullOrWhiteSpace(TempEmailCache))
                        {
                            Input_Email.SafeInvokeAction(() => Input_Email.Text = TempEmailCache, this);
                        }
                    }
                }
                catch (Exception Error_Live)
                {
                    LogToFileAddons.OpenLog("Progress Bar/Outline ODF", string.Empty, Error_Live, string.Empty, true);
                }

                FunctionStatus.IsVerifyHashDisabled = true;
            }
        }
#endregion

#region Game Files Downloader (SBRW Pack [.pack.sbrw])
        /* potential error is that the Pack_SBRW_Unpacker variable is being assigned a new Download_Extract object every time the 
         * Game_Pack_Downloader method is called, but it's not being disposed of or set to null afterwards. 
         * This could lead to memory leaks if the method is called repeatedly. 
         * @DavidCarbon or @DavidCarbon-SBRW/launcher-team
         */
        private void Game_Pack_Unpacker(string Provided_File_Path)
        {
            if (!Pack_SBRW_Downloader_Unpack_Lock)
            {
                Pack_SBRW_Downloader_Unpack_Lock = true;
                Pack_SBRW_Unpacker = new Download_Extract();
                Pack_SBRW_Unpacker.Internal_Error += (_, U_Live_Events) =>
                {
                    if (U_Live_Events.Recorded_Exception != null && !Disposing && !IsDisposed)
                    {
                        LogToFileAddons.OpenLog("Pack_SBRW_Unpacker", string.Empty, U_Live_Events.Recorded_Exception, string.Empty, true);
                        OnDownloadFailed(U_Live_Events.Recorded_Exception);
                    }
                };
                Pack_SBRW_Unpacker.Live_Progress += (_, U_Live_Events) =>
                {
                    if (U_Live_Events != null && !Disposing && !IsDisposed)
                    {
                        if (UI_MODE != 4)
                        {
                            UI_MODE = 4;
                        }
                    }
                };
                Pack_SBRW_Unpacker.Complete += (_, U_Live_Events) =>
                {
                    if (U_Live_Events != null && !Disposing && !IsDisposed)
                    {
                        Label_Download_Information_Support.SafeInvokeAction(() =>
                        {
                            Label_Download_Information_Support.Visible = false;
                            Label_Download_Information_Support.Text = string.Empty;
                        }, this);

                        IsDownloading = false;
                        OnDownloadFinished();
                        try
                        {
                            if (NotifyIcon_Notification != default)
                            {
                                NotifyIcon_Notification.Visible = true;
                                NotifyIcon_Notification.BalloonTipIcon = ToolTipIcon.Info;
                                NotifyIcon_Notification.BalloonTipTitle = "SBRW Launcher";
                                NotifyIcon_Notification.BalloonTipText = "Your game is now ready to launch!";
                                NotifyIcon_Notification.BalloonTipClicked += (x, D_Live_Events) =>
                                {
                                    return;
                                };
                                NotifyIcon_Notification.BalloonTipClosed += (x, D_Live_Events) =>
                                {
                                    return;
                                };
                                NotifyIcon_Notification.ShowBalloonTip(5000);
                            }
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("NotifyIcon_Notification Unpack", string.Empty, Error, string.Empty, true);
                        }
                    }
                };
                Pack_SBRW_Unpacker.Custom_Unpack(Provided_File_Path, Save_Settings.Live_Data.Game_Path);
            }
        }
        /* That's right the Protype Extractor from 2.1.5.x, now back from the dead - DavidCarbon */
        /// <summary>
        /// SBRW Pack Downloader
        /// </summary>
        private void Game_Pack_Downloader()
        {
            if (Screen_Instance != null)
            {
                if (UI_MODE != 9)
                {
                    UI_MODE = 9;
                }

                long Game_Folder_Size = File_and_Folder_Extention.GetDirectorySize_GameFiles(new DirectoryInfo(Save_Settings.Live_Data.Game_Path));
                /* TODO: Check for other files and Folder Size */
                if ((Game_Folder_Size == -1) &&
                    (MessageBox.Show(this, "Seems like we are unable to determine the Games Folder Size" +
                        "\nDo you have the Game Files Already Downloaded?", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes))
                {
                    Game_Folder_Size = 3296810469;
                }
                else
                {
                    Log.Debug($"Game Folder Size: {Game_Folder_Size}");
                }

                if (UI_MODE != 3)
                {
                    UI_MODE = 3;
                }

                if (!Game_Folder_Size.GameInstall_Found() || !File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, "nfsw.exe")))
                {
                    if (UI_MODE != 10)
                    {
                        UI_MODE = 10;
                    }

                    if (Hashes.Hash_SHA(Save_Settings.Live_Data.Game_Archive_Location) == "88C886B6D131C052365C3D6D14E14F67A4E2C253")
                    {
                        Game_Pack_Unpacker(Save_Settings.Live_Data.Game_Archive_Location);
                    }
                    else
                    {
                        switch (API_Core.StatusCheck(Save_Settings.Live_Data.Launcher_CDN + "/GameFiles.sbrwpack", 10))
                        {
                            case APIStatus.Online:
                                if (UI_MODE != 11)
                                {
                                    UI_MODE = 11;
                                }

                                Pack_SBRW_Downloader = new Download_Client()
                                {
                                    Folder_Path = Save_Settings.Live_Data.Game_Path,
                                    File_Path = Save_Settings.Live_Data.Game_Archive_Location,
                                    File_Name = "GameFiles.sbrwpack",
                                    Web_File_Size = 3862102244,
                                    Web_URL = Save_Settings.Live_Data.Launcher_CDN + "/GameFiles.sbrwpack",
                                    File_Hash = "88C886B6D131C052365C3D6D14E14F67A4E2C253",
                                    File_Removal = true,
                                    Download_Retry_Attempts = 100
                                };
                                /* @DavidCarbon or @Zacam (Translation Strings Required) */
                                Pack_SBRW_Downloader.Internal_Error += (_, D_Live_Events) =>
                                {
                                    if (D_Live_Events.Recorded_Exception != null && !Pack_SBRW_Downloader.Cancel)
                                    {
                                        LogToFileAddons.OpenLog("Pack_SBRW_Downloader", string.Empty, D_Live_Events.Recorded_Exception, string.Empty, true);

                                        if (D_Live_Events.Recorded_Exception is WebException)
                                        {
                                            string Status_Code_Explaination = "Unknown";
                                            bool Allow_Restart = true;
                                            switch (API_Core.StatusCodes(Save_Settings.Live_Data.Launcher_CDN, D_Live_Events.Recorded_Exception as WebException, null))
                                            {
                                                /* SSL Chain Validation Error */
                                                case APIStatus.TrustFailure:
                                                case APIStatus.SecureChannelFailure:
                                                case APIStatus.InvaildSSL:
                                                case APIStatus.SSLFailed:
                                                    Status_Code_Explaination = "Unable to Create a Secure Connection." +
                                                    "\nSSL may be invalid, System has blocked connection, or System is unable to handle TLS 1.2 and higher with C# Apps."
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                                    ;
#else
                                                + "\nCheck if Alternative WebCalls is Enabled to Fix the issue";
#endif
                                                    Allow_Restart = false;
                                                    break;
                                                /* The following Error Codes Means Internal Error Had Occurred */
                                                case APIStatus.ProtocolError:
                                                case APIStatus.UnknownError:
                                                case APIStatus.UnknownStatusCode:
                                                case APIStatus.Unknown:
                                                    Status_Code_Explaination = "Internal Error had occurred." +
                                                        "\nCheck Launcher Log for more Details.";
                                                    break;
                                                /* Unable to reach online server */
                                                case APIStatus.Offline:
                                                case APIStatus.NameResolutionFailure:
                                                case APIStatus.OriginUnreachable:
                                                case APIStatus.ServerUnavailable:
                                                    Status_Code_Explaination = "Unable to Connect to CDN." +
                                                        "\nCheck Launcher Log for more Details.";
                                                    Allow_Restart = false;
                                                    break;
                                                /* Not Found, Don't Retry */
                                                case APIStatus.NotFound:
                                                    Status_Code_Explaination = "File Not Found." +
                                                        "\nAsk for Assistance or Change to another CDN.";
                                                    Allow_Restart = false;
                                                    break;
                                                case APIStatus.Forbidden:
                                                    Status_Code_Explaination = "No Permission to Access this File or Server" +
                                                        "\nCheck Launcher Log for more Details." +
                                                        "\nAsk for Assistance or Change to another CDN.";
                                                    Allow_Restart = false;
                                                    break;
                                                /* Generic Error Type */
                                                default:
                                                    Status_Code_Explaination = "A Generic Error was encountered" +
                                                        "\nCheck Launcher Log for more Details.";
                                                    break;
                                            }

                                            DialogResult User_Prompt_Box = MessageBox.Show(this, Status_Code_Explaination, "GameLauncher",
                                                Allow_Restart ? MessageBoxButtons.RetryCancel : MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            if (User_Prompt_Box == DialogResult.Retry)
                                            {
                                                Game_Pack_Downloader();
                                            }
                                            else
                                            {
                                                OnDownloadFailed(D_Live_Events.Recorded_Exception);
                                            }
                                        }
                                        else
                                        {
                                            OnDownloadFailed(D_Live_Events.Recorded_Exception);
                                        }
                                    }
                                };
                                Pack_SBRW_Downloader.Live_Progress += (_, D_Live_Events) =>
                                {
                                    if (UI_MODE != 1)
                                    {
                                        UI_MODE = 1;
                                    }
                                };
                                Pack_SBRW_Downloader.Complete += (_, D_Live_Events) =>
                                {
                                    if (D_Live_Events.Complete && !(this.Disposing || this.IsDisposed))
                                    {
                                        if (UI_MODE != 2)
                                        {
                                            UI_MODE = 2;
                                        }
                                        /* Unpack Local GameFiles Pack */
                                        Game_Pack_Unpacker(D_Live_Events.Download_Location ?? InformationCache.Default_Game_Archive_Path());
                                    }
                                };
                                /* Main Note: Current Revision File Size (in long) is: 3862102244 */
                                Pack_SBRW_Downloader.Download();

                                break;
                            case APIStatus.Forbidden:
                            case APIStatus.NotFound:
                                if (MessageBox.Show(Screen_Instance, "Game Archive is Not Present on Current Saved CDN." +
                                    "\nWould you like to check for LZMA Support? This would switch to the old LZMA Downloader." +
                                    "\nOtherwise, please switch to another CDN.", "GameLauncher",
                                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Retry)
                                {
                                    switch (API_Core.StatusCheck(Save_Settings.Live_Data.Launcher_CDN + "/en/index.xml", 10))
                                    {
                                        case APIStatus.Online:
                                            Game_Downloaders(true);
                                            break;
                                        case APIStatus.Forbidden:
                                        case APIStatus.NotFound:
                                            OnDownloadFailed(new Exception("Game Archive & LZMA Not Present on Server. Please Choose Another CDN"));
                                            break;
                                        default:
                                            OnDownloadFailed(new Exception("Please Choose Another CDN"));
                                            break;
                                    }
                                }
                                else
                                {
                                    OnDownloadFailed(new Exception("Game Archive Not Present on Server. Please Choose Another CDN"));
                                }
                                break;
                            default:
                                OnDownloadFailed(new Exception("Unable to Connect to CDN. Choose Another CDN or look at Logs for Details"));
                                break;
                        }
                    }
                }
                else
                {
                    OnDownloadFinished();
                }
            }
        }
#endregion

#region Game Downloader Background Thread Support Functions
        public async void Game_Downloaders(bool From_PackDownloader = false)
        {
            await Task.Run(() => 
            {
                if (Screen_Instance != null && (!IsDisposed || !Disposing))
                {
                    try
                    {
                        if (UI_MODE != 3)
                        {
                            UI_MODE = 3;
                        }

                        if (Label_Download_Information.InvokeRequired)
                        {
                            Label_Download_Information.Invoke(new Action(delegate ()
                            {
                                Label_Download_Information.Text = "Loading list of files to download...".ToUpper();
                            }));
                        }
                        else
                        {
                            Label_Download_Information.Text = "Loading list of files to download...".ToUpper();
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("Progress Bar/Outline", string.Empty, Error, string.Empty, true);
                    }
                }

                try
                {
                    if (LZMA_Downloader != null)
                    {
                        if (LZMA_Downloader.Downloading)
                        {
                            LZMA_Downloader.Stop();
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("CDN DOWNLOADER [LZMA]", string.Empty, Error, string.Empty, true);
                }

                try
                {
                    if (Pack_SBRW_Downloader != null)
                    {
                        if (!Pack_SBRW_Downloader.Cancel)
                        {
                            Pack_SBRW_Downloader.Cancel = true;
                        }

                        if (Pack_SBRW_Unpacker != null)
                        {
                            if (!Pack_SBRW_Unpacker.Cancel)
                            {
                                Pack_SBRW_Unpacker.Cancel = true;
                            }
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("CDN DOWNLOADER", string.Empty, Error, string.Empty, true);
                }

                /* Use Local Packed Archive for Install Source - DavidCarbon */
                if (!InformationCache.EnableLZMADownloader() && !From_PackDownloader)
                {
                    try
                    {
                        //@DavidCarbon -> 9-15-2022
                        Game_Pack_Downloader();
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("New Download Game Files", string.Empty, Error, string.Empty, true);
                    }
                }
                else
                {
                    try
                    {
                        LZMA_Downloader = new Download_LZMA_Data(3, 2, 16);
                        
                        LZMA_Downloader.Internal_Error += (_, D_Live_Events) =>
                        {
                            if (D_Live_Events.Recorded_Exception != null && LZMA_Downloader.Downloading)
                            {
                                LogToFileAddons.OpenLog("LZMA_SBRW_Downloader", string.Empty, D_Live_Events.Recorded_Exception, string.Empty, true);
                                OnDownloadFailed(D_Live_Events.Recorded_Exception);
                            }
                        };

                        LZMA_Downloader.Complete += (_, Live_Data) =>
                        {
                            if ((!IsDisposed || !Disposing) && Live_Data.Complete)
                            {
                                Game_Downloaders();
                            }
                        };

                        LZMA_Downloader.Live_Progress += (_, Live_Data) =>
                        {
                            if ((!IsDisposed || !Disposing) && LZMA_Downloader != null)
                            {
                                if (LZMA_Downloader.Downloading)
                                {
                                    if (UI_MODE != 15)
                                    {
                                        UI_MODE = 15;
                                    }
                                }
                                else if (LZMA_Downloader != null)
                                {
                                    if (LZMA_Downloader.Downloading)
                                    {
                                        LZMA_Downloader.Stop();
                                    }
                                }
                            }
                            else if (LZMA_Downloader != null)
                            {
                                if (LZMA_Downloader.Downloading)
                                {
                                    LZMA_Downloader.Stop();
                                }
                            }
                        };

                        if (Screen_Instance != null && (!IsDisposed || !Disposing))
                        {
                            if (Label_Download_Information.InvokeRequired)
                            {
                                Label_Download_Information.Invoke(new Action(delegate ()
                                {
                                    Label_Download_Information.Text = "Checking Core Files...".ToUpper();
                                }));
                            }
                            else
                            {
                                Label_Download_Information.Text = "Checking Core Files...".ToUpper();
                            }

                            string GameExePath = Path.Combine(Save_Settings.Live_Data.Game_Path, "nfsw.exe");

                            if (!File.Exists(GameExePath) && LZMA_Downloader != null)
                            {
                                if (Label_Download_Information.InvokeRequired)
                                {
                                    Label_Download_Information.Invoke(new Action(delegate ()
                                    {
                                        Label_Download_Information_Support.Text = "Downloading: Core GameFiles".ToUpper();
                                    }));
                                }
                                else
                                {
                                    Label_Download_Information_Support.Text = "Downloading: Core GameFiles".ToUpper();
                                }

                                Log.Info("DOWNLOAD: Getting Core Game Files");
#if (RELEASE_UNIX || DEBUG_UNIX)
                                Download_LZMA_Settings.System_Unix = Download_Settings.System_Unix = true;
#endif
                                Download_LZMA_Settings.Alternative_WebCalls = Download_Settings.Alternative_WebCalls = Launcher_Value.Launcher_Alternative_Webcalls();
                                LZMA_Downloader.StartDownload(Save_Settings.Live_Data.Launcher_CDN, string.Empty, Save_Settings.Live_Data.Game_Path, false, false, 1130632198);
                            }
                            else
                            {
                                DownloadTracksFiles();
                            }
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("Classic Download Game Files", string.Empty, Error, string.Empty, true);
                    }
                }
            });
        }
#if (DEBUG_UNIX || RELEASE_UNIX)
        public void Game_Folder_Checks(bool Bypass_Storage_Requirement = false)
#else
        public void Game_Folder_Checks()
#endif
        {
            if (Screen_Instance != null && (!IsDisposed || !Disposing))
            {
                if (UI_MODE != 3)
                {
                    UI_MODE = 3;
                }

                Button_Play_OR_Update.SafeInvokeAction(() =>
                {
                    Button_Play_OR_Update.BackgroundImage = Image_Button.Play;
                    Button_Play_OR_Update.ForeColor = Color_Text.L_Three;
                }, this, false);

                Label_Download_Information.SafeInvokeAction(() =>
                Label_Download_Information.Text = "Checking up all files".ToUpper(), this);

                try
                {
                    Label_Download_Information.SafeInvokeAction(() => 
                    Label_Download_Information.Text = "Checking Drive Format and Space".ToUpper(), this);

                    Format_System_Storage Detected_Drive = System_Storage.Drive_Full_Info(Save_Settings.Live_Data.Game_Path, false, true);

#if !(RELEASE_UNIX || DEBUG_UNIX)
                    if (Detected_Drive.TotalFreeSpace < 8000000000 ||
                        !string.Equals(Detected_Drive.DriveFormat, "NTFS", StringComparison.InvariantCultureIgnoreCase))
#else
                    if (Detected_Drive.TotalFreeSpace < 8000000000 && !Bypass_Storage_Requirement && Save_Settings.Live_Data.Alert_Storage_Space == "0")
#endif
                    {
                        if (UI_MODE != 6)
                        {
                            UI_MODE = 6;
                        }

#if !(RELEASE_UNIX || DEBUG_UNIX)
                        if (!string.Equals(Detected_Drive.DriveFormat, "NTFS", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Label_Download_Information_Support.SafeInvokeAction(() =>
                            Label_Download_Information_Support.Text = ("Playing the game on a non-NTFS-formatted drive is not supported.").ToUpper(), this, false);
                            Label_Download_Information.SafeInvokeAction(() =>
                            Label_Download_Information.Text = ("Drive '" + Detected_Drive.Name + "' is formatted with: " + Detected_Drive.DriveFormat + " Type.").ToUpper(), this);
                        }
                        else
                        {
                            Label_Download_Information.SafeInvokeAction(() =>
                            Label_Download_Information.Text = ("Make sure you have at least 8GB of free space on hard drive.").ToUpperInvariant(), this);
                        }

                        FunctionStatus.IsVerifyHashDisabled = true;
#else
                        Label_Download_Information.SafeInvokeAction(() =>
                        Label_Download_Information.Text = ("Make sure you have at least 8GB of free space on hard drive.").ToUpperInvariant(), this);

                        DialogResult Live_Prompt_Data = new Update_Popup_Screen.Screen_Update_Popup(false).ShowDialog();
                        /*"Click Ignore to Enable Storage Detection Bypass (Unix Builds Only) and Restarts the Downloader" +
                            "Click Retry to temporary bypass the Storage Detection." +
                            "Click Ok, to Close this Message";
                         */
                        switch (Live_Prompt_Data)
                        {
                            case DialogResult.Ignore:
                                Save_Settings.Live_Data.Alert_Storage_Space = "1";
                                Game_Folder_Checks(true);
                                break;
                            case DialogResult.Retry:
                                Game_Folder_Checks(true);
                                break;
                        }
#endif
                    }
                    else if (Save_Settings.Live_Data.Launcher_CDN.StartsWith("http://localhost") || Save_Settings.Live_Data.Launcher_CDN.StartsWith("https://localhost"))
                    {
                        if (UI_MODE != 6)
                        {
                            UI_MODE = 6;
                        }

                        Label_Download_Information_Support.SafeInvokeAction(() => Label_Download_Information_Support.Text = "Failsafe CDN Detected".ToUpperInvariant(), this, false);
                        Label_Download_Information.SafeInvokeAction(() => Label_Download_Information.Text = "Please Choose a CDN from Settings Screen".ToUpperInvariant(), this);
                    }
                    else
                    {                        
                        Game_Downloaders();
                    }
                }
                catch (IOException Error)
                {
                    LogToFileAddons.OpenLog("Game Folder Checks [I.O.E.]", string.Empty, Error, string.Empty, true);
                }
                catch (UnauthorizedAccessException Error)
                {
                    LogToFileAddons.OpenLog("Game Folder Checks [U.A.E.]", string.Empty, Error, string.Empty, true);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Game Folder Checks", string.Empty, Error, string.Empty, true);
                }
            }
        }
#endregion

#region Loads, Set, and Start Certain Functions (CORE)
        private void MainScreen_Load(object sender, EventArgs e)
        {
            Log.Visuals("CORE: Loading Main Screen");
            Application.OpenForms[this.Name].Activate();

            if (!string.IsNullOrWhiteSpace(InsiderInfo.BuildNumber()))
            {
                if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                {
                    Label_Insider_Build_Number.Visible = true;
                    Label_Insider_Build_Number.Text = InsiderInfo.BuildNumber();
                }
                else
                {
                    Label_Insider_Build_Number.Visible = Label_Debug_Language.Visible = false;
                }
            }

            Log.Core("LAUNCHER: NFSW Download Source is now: " + Save_Settings.Live_Data.Launcher_CDN);

            Input_Email.Text = Save_Account.Live_Data.User_Raw_Email;
            Input_Password.Text = Save_Account.Live_Data.User_Raw_Password;

            Log.Core("LAUNCHER: Checking for password");
            LoginEnabled = ServerEnabled = !string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Raw_Password);
            Button_Login.BackgroundImage = Image_Button.Grey;
            Button_Login.ForeColor = LoginEnabled ? Color_Text.L_Five : Color_Text.L_Six;

            if (!string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Raw_Email) &&
                (!string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Hashed_Password) || !string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Raw_Password)))
            {
                Log.Core("LAUNCHER: Restoring last saved email and password");
                CheckBox_Remember_Us.Checked = true;
                Save_Account.SaveLoginInformation = true;
            }

            /* Server Display List */
            ComboBox_Server_List.DisplayMember = "Name";
            Log.Core("LAUNCHER: Setting server list");
            ComboBox_Server_List.DataSource = ServerListUpdater.CleanList;

            /* Display Server List Dialog if Server IP Doesn't Exist */
            if (string.IsNullOrWhiteSpace(Save_Account.Live_Data.Saved_Server_Address))
            {
                Screen_Custom_Server.OpenScreen(false);

                if (SelectedServer.Data != null)
                {
                    Save_Account.Live_Data.Saved_Server_Address = SelectedServer.Data.IPAddress;
                    Save_Account.Save();
                }
                else
                {
                    FunctionStatus.LauncherForceClose = true;
                }
            }

            if (FunctionStatus.LauncherForceClose)
            {
                FunctionStatus.ErrorCloseLauncher("Closing From SelectServer Dialog", false);
            }
            else
            {
                Log.Core("SERVERLIST: Checking...");
                Log.Core("SERVERLIST: Setting first server in list");
                Log.Core("SERVERLIST: Checking if server is set on INI File");
                try
                {
                    if (string.IsNullOrWhiteSpace(Save_Account.Live_Data.Saved_Server_Address))
                    {
                        Log.Warning("SERVERLIST: Failed to find anything... assuming " + ((Json_List_Server)ComboBox_Server_List.SelectedItem).IPAddress);
                        Save_Account.Live_Data.Saved_Server_Address = ((Json_List_Server)ComboBox_Server_List.SelectedItem).IPAddress;
                        Save_Account.Save();
                    }
                }
                catch
                {
                    Log.Error("SERVERLIST: Failed to write anything...");
                    Save_Account.Live_Data.Saved_Server_Address = string.Empty;
                    Save_Account.Save();
                }

                Log.Core("SERVERLIST: Re-Checking if server is set on INI File");
                if (!string.IsNullOrWhiteSpace(Save_Account.Live_Data.Saved_Server_Address))
                {
                    Log.Core("SERVERLIST: Found something!");
                    SkipServerTrigger = true;

                    Log.Core("SERVERLIST: Checking if server exists on our database");

                    try
                    {
                        if (ServerListUpdater.CleanList.Count != 0)
                        {
                            if (ServerListUpdater.CleanList.FindIndex(i => string.Equals(i.IPAddress, Save_Account.Live_Data.Saved_Server_Address)) != 0)
                            {
                                Log.Core("SERVERLIST: Server found! Checking ID");
                                var index = ServerListUpdater.CleanList.FindIndex(i => string.Equals(i.IPAddress, Save_Account.Live_Data.Saved_Server_Address));

                                Log.Core("SERVERLIST: ID is " + index);
                                if (index >= 0)
                                {
                                    Log.Core("SERVERLIST: ID set correctly");
                                    ComboBox_Server_List.SelectedIndex = index;
                                }
                                else
                                {
                                    ComboBox_Server_List.SelectedIndex = 1;
                                }
                            }
                            else
                            {
                                Log.Warning("SERVERLIST: Unable to find anything, assuming default");
                                ComboBox_Server_List.SelectedIndex = 1;
                                Log.Warning("SERVERLIST: Deleting unknown entry");
                                Save_Account.Live_Data.Saved_Server_Address = string.Empty;
                                Save_Account.Save();
                            }

                            Log.Core("SERVERLIST: Triggering server change");
                            if (ComboBox_Server_List.SelectedIndex == 1)
                            {
                                ComboBox_Server_List_SelectedIndexChanged(sender, e);
                            }

                            Log.Completed("SERVERLIST: All done");
                        }
                        else { ComboBox_Server_List_SelectedIndexChanged(sender, e); Log.Completed("SERVERLIST: Empty List. Not Setting Index"); }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("Serverlist", string.Empty, Error, string.Empty, true);
                    }
                }

                Log.Core("LAUNCHER: Re-checking InstallationDirectory: " + Save_Settings.Live_Data.Game_Path);

                string Drive = Path.GetPathRoot(Save_Settings.Live_Data.Game_Path)??string.Empty;
                if (!Directory.Exists(Drive))
                {
                    if (string.IsNullOrWhiteSpace(Drive))
                    {
                        Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                        Save_Settings.Save();
                        string Display_Message = Translations.Database("MainScreen_TextBox_GameFiles_Invalid_Location");
                        Log.Error(string.Format("LAUNCHER: Drive {0} was not found. Your actual installation directory is set to {1} now.",
                            Drive, Locations.GameFilesFailSafePath));

                        string TempEmailCache = string.Empty;
                        if (!string.IsNullOrWhiteSpace(Input_Email.Text))
                        {
                            TempEmailCache = Input_Email.Text;
                            Input_Email.Text = "EMAIL IS HIDDEN";
                        }
                        MessageBox.Show(this, string.Format("Drive {0} was not found. Your actual installation directory is set to {1} now.",
                            Drive, Locations.GameFilesFailSafePath),
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (!string.IsNullOrWhiteSpace(TempEmailCache))
                        {
                            Input_Email.Text = TempEmailCache;
                        }
                    }
                }

                try
                {
                    new LauncherUpdateCheck(Picture_Icon_Version, Label_Status_Launcher, Label_Status_Launcher_Version).ChangeVisualStatus();
                }
                catch
                {
#if RELEASE_UNIX || DEBUG_UNIX
                        if (Picture_Icon_Version.BackgroundImage != Image_Icon.Engine_Good)
                        {
                            Picture_Icon_Version.BackgroundImage = Image_Icon.Engine_Good;
                        }

                        Label_Status_Launcher.ForeColor = Color_Text.S_Sucess;
                        Label_Status_Launcher.Text = "Launcher Status:\n - Linux Build";
                        Label_Status_Launcher_Version.Text = "Version: " + Application.ProductVersion;
#endif
                }

                PingServerListAPIStatus();

                Log.Visuals("CORE: Applyinng ContextMenu");
#if NETFRAMEWORK
                ContextMenu = new ContextMenu();
                try
                {
                    /* Internal Message Reference Time: 01/14/2023 1:31 AM PST */
                    if (DateTime.Now == new DateTime(DateTime.Now.Year, 1, 14) || DateTime.Now == new DateTime(DateTime.Now.Year, 4, 18))
                    {
                        ContextMenu.MenuItems.Add(new MenuItem("RIP M.L. (1925-2023)", (_, E) =>
                        {
#if NETFRAMEWORK
                            Process.Start("https://www.youtube.com/watch?v=wctrwXZkUK0");
#else
                            Process.Start("explorer.exe", "https://www.youtube.com/watch?v=wctrwXZkUK0");
#endif
                        }));
                        ContextMenu.MenuItems.Add("-");
                    }
                }
                catch
                {
                    ContextMenu.MenuItems.Add(new MenuItem("RIP M.L. (1925-2023)", (_, E) =>
                    {
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=rANqc5br7dc");
#else
                        Process.Start("explorer.exe", "https://www.youtube.com/watch?v=rANqc5br7dc");
#endif
                    }));
                    ContextMenu.MenuItems.Add("-");
                }

                try
                {
                    if (DateTime.Now == new DateTime(DateTime.Now.Year, 4, 1))
                    {
                        ContextMenu.MenuItems.Add(new MenuItem("The Mermaid Sisters is Here!", (_, E) =>
                        {
#if NETFRAMEWORK
                            Process.Start("https://www.youtube.com/watch?v=OFjqEexH0Tg");
#else
                            Process.Start("explorer.exe", "https://www.youtube.com/watch?v=OFjqEexH0Tg");
#endif
                        }));
                        ContextMenu.MenuItems.Add("-");
                    }
                }
                catch
                {
                    /* Show it Anyways, lets be real here */
                    ContextMenu.MenuItems.Add(new MenuItem("The Mermaid Sisters is Here!", (_, E) =>
                    {
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=OFjqEexH0Tg");
#else
                        Process.Start("explorer.exe", "https://www.youtube.com/watch?v=OFjqEexH0Tg");
#endif
                    }));
                    ContextMenu.MenuItems.Add("-");
                }

                try
                {
                    if (DateTime.Now == new DateTime(DateTime.Now.Year, 7, 4))
                    {
                        ContextMenu.MenuItems.Add(new MenuItem("Fireworks", (_, E) =>
                            {
#if NETFRAMEWORK
                                Process.Start("https://youtu.be/2m5vQo81Jik");
#else
                                Process.Start("explorer.exe", "https://youtu.be/2m5vQo81Jik");
#endif
                            }));
                        ContextMenu.MenuItems.Add("-");
                    }
                    else if (DateTime.Now == new DateTime(DateTime.Now.Year, 6, 4))
                    {
                        ContextMenu.MenuItems.Add(new MenuItem(
                            ((EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed()) ? 
                            "": "2017/06/04 - ") + "Development Cycle", (_, E) =>
                        {
#if NETFRAMEWORK
                            Process.Start("https://www.youtube.com/watch?v=5hv2p0RtVY0");
#else
                            Process.Start("explorer.exe", "https://www.youtube.com/watch?v=5hv2p0RtVY0");
#endif
                        }));
                        ContextMenu.MenuItems.Add("-");
                    }
                    /* Development Release Year: 2017 */
                    else if (DateTime.Now == new DateTime(DateTime.Now.Year, 6, 18))
                    {
                        ContextMenu.MenuItems.Add(new MenuItem("Happy Birthday Interface 1", (_, E) =>
                        {
#if NETFRAMEWORK
                            Process.Start("https://raw.githubusercontent.com/SoapboxRaceWorld/GameLauncher_NFSW/interface_v1/screenshot.png");
#else
                            Process.Start("explorer.exe", "https://raw.githubusercontent.com/SoapboxRaceWorld/GameLauncher_NFSW/interface_v1/screenshot.png");
#endif
                        }));
                        ContextMenu.MenuItems.Add("-");
                    }
                    /* Development Release Year: 2017 */
                    else if (DateTime.Now == new DateTime(DateTime.Now.Year, 11, 2))
                    {
                        ContextMenu.MenuItems.Add(new MenuItem("Happy Birthday Interface 2!", (_, E) =>
                        {
#if NETFRAMEWORK
                            Process.Start("https://raw.githubusercontent.com/SoapboxRaceWorld/GameLauncher_NFSW/interface_v2/screenshot.png");
#else
                            Process.Start("explorer.exe", "https://raw.githubusercontent.com/SoapboxRaceWorld/GameLauncher_NFSW/interface_v2/screenshot.png");
#endif
                        }));
                        ContextMenu.MenuItems.Add("-");
                    }
                    /* Development Release Year: 2018 */
                    else if (DateTime.Now == new DateTime(DateTime.Now.Year, 11, 8))
                    {
                        ContextMenu.MenuItems.Add(new MenuItem("Happy Birthday Interface 3!", (_, E) =>
                        {
#if NETFRAMEWORK
                            Process.Start("https://raw.githubusercontent.com/SoapboxRaceWorld/GameLauncher_NFSW/Net.Standard/01-Main_Screen.png");
#else
                            Process.Start("explorer.exe", "https://raw.githubusercontent.com/SoapboxRaceWorld/GameLauncher_NFSW/Net.Standard/01-Main_Screen.png");
#endif
                        }));
                        ContextMenu.MenuItems.Add("-");
                    }
                }
                catch
                {
                    /* Forget about it Cuh */
                }

                ContextMenu.MenuItems.Add(new MenuItem("About", (O, K) => { Screen_About.OpenScreen(); }));
                if (LauncherUpdateCheck.UpgradeAvailable)
                {
                    ContextMenu.MenuItems.Add("-");
                    ContextMenu.MenuItems.Add(new MenuItem("Obsolete", (N, O) => 
                    {
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=LutDfASARmE");
#else
                        Process.Start("explorer.exe", "https://www.youtube.com/watch?v=LutDfASARmE");
#endif
                    }));
                }
                ContextMenu.MenuItems.Add("-");
                if (Parent_Screen.Screen_Instance != null)
                {
                    ContextMenu.MenuItems.Add(new MenuItem("Close Launcher", Parent_Screen.Screen_Instance.Button_Close_Click));
                }

                NotifyIcon_Notification.ContextMenu = ContextMenu;
                ContextMenu = null;
#endif
                NotifyIcon_Notification.Icon = FormsIcon.Retrive_Icon();
                NotifyIcon_Notification.Text = "SBRW Launcher";
                NotifyIcon_Notification.Visible = true;

                /* Remove TracksHigh Folder and Files */
                RemoveTracksHighFiles();
            }
        }
#endregion

#region Theme and Functions
        private void Set_Visuals()
        {
            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Icon = FormsIcon.Retrive_Icon();
            Text = "SBRW Launcher: " + Application.ProductVersion;

            /*******************************/
            /* Set Font                     /
            /*******************************/

#if !(RELEASE_UNIX || DEBUG_UNIX)
            float MainFontSize = 9f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = 8f * 96f / CreateGraphics().DpiY;
            float ThirdFontSize = 10f * 96f / CreateGraphics().DpiY;
            float FourthFontSize = 14f * 96f / CreateGraphics().DpiY;
#else
            float MainFontSize = 9f;
            float SecondaryFontSize = 8f;
            float ThirdFontSize = 10f;
            float FourthFontSize = 14f;
#endif
            Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);

            /* Front Screen */
            Label_Insider_Build_Number.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_Select_Server.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Debug_Language.Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            ComboBox_Server_List.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Custom_Server.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            Panel_Server_Information.Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            Label_Information_Window.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Status_Launcher.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Status_Launcher_Version.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Status_Game_Server.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Status_Game_Server_Data.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Status_API.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Status_API_Details.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            ProgressBar.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            /* Social Panel */
            Panel_Server_Information.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            LinkLabel_Server_Home.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            LinkLabel_Server_Discord.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            LinkLabel_Server_Facebook.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            LinkLabel_Server_Twitter.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Server_Scenery.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Server_Force_Restart_Timer.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            /* Log In Panel */
            Input_Email.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Input_Password.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            CheckBox_Remember_Us.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            LinkLabel_Forgot_Password.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_Login.Font = new Font(FormsFont.Primary_Bold(), ThirdFontSize, FontStyle.Bold);
            Button_Register.Font = new Font(FormsFont.Primary_Bold(), ThirdFontSize, FontStyle.Bold);
            Label_Client_Ping.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Logout.Font = new Font(FormsFont.Primary_Bold(), ThirdFontSize, FontStyle.Bold);
            Button_Play_OR_Update.Font = new Font(FormsFont.Primary_Bold(), FourthFontSize, FontStyle.Bold);
            Label_Download_Information.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Download_Information_Support.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            /* Console */
            Button_Console_Submit.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            Input_Console.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            /* Set Background with Transparent Key */
            BackgroundImage = Image_Background.Login;
            TransparencyKey = Color_Screen.BG_Main;

            Picture_Logo.BackgroundImage = Image_Other.Logo;
            Button_Settings.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Bad") ? Image_Icon.Gear_Warning : Image_Icon.Gear;
            Button_Close.BackgroundImage = Image_Icon.Close;
            Button_Security_Center.BackgroundImage = SecurityCenter.SecurityCenterIcon(1);

            Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Preload_Outline;
            ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Unknown_Top;
            ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Unknown_Bottom;
            ProgressBar.OuterRectangleBackColor = Color_Winform_Other.ProgressBar_Background;

            Label_Download_Information.ForeColor = Color_Text.L_Five;
            Label_Download_Information_Support.ForeColor = Color_Text.L_Five;

            Picture_Input_Email.Image = Image_Other.Text_Border_Email;
            Picture_Input_Password.Image = Image_Other.Text_Border_Password;
            Picture_Information_Window.Image = Image_Other.Information_Window;

            Label_Information_Window.ForeColor = Color_Text.L_Five;

            Label_Status_Launcher_Version.ForeColor = Color_Text.L_Five;
            Label_Status_Game_Server.ForeColor = Color_Text.L_Five;
            Label_Status_Game_Server_Data.ForeColor = Color_Text.L_Five;
            Label_Status_API_Details.ForeColor = Color_Text.L_Five;

            Button_Login.ForeColor = Color_Text.L_Five;
            Button_Login.BackgroundImage = Image_Button.Grey;

            Button_Register.ForeColor = Color_Text.L_Seven;
            Button_Register.BackgroundImage = Image_Button.Green;

            CheckBox_Remember_Us.ForeColor = Color_Text.L_Five;

            LinkLabel_Forgot_Password.ActiveLinkColor = Color_Winform_Other.Link_Active;
            LinkLabel_Forgot_Password.LinkColor = Color_Winform_Other.Link;

            Input_Email.BackColor = Color_Winform_Other.Input;
            Input_Email.ForeColor = Color_Text.L_Five;
            Input_Password.BackColor = Color_Winform_Other.Input;
            Input_Password.ForeColor = Color_Text.L_Five;

            Panel_Server_Information.BackgroundImage = Image_Background.Server_Information;

            Label_Server_Force_Restart_Timer.ForeColor = Color_Text.L_Two;
            Label_Server_Scenery.ForeColor = Color_Text.L_Two;

            LinkLabel_Server_Twitter.LinkColor = Color_Text.L_Two;
            LinkLabel_Server_Facebook.LinkColor = Color_Text.L_Two;
            LinkLabel_Server_Discord.LinkColor = Color_Text.L_Two;
            LinkLabel_Server_Home.LinkColor = Color_Text.L_Two;

            LinkLabel_Server_Twitter.ActiveLinkColor = Color_Text.L_Five;
            LinkLabel_Server_Facebook.ActiveLinkColor = Color_Text.L_Five;
            LinkLabel_Server_Discord.ActiveLinkColor = Color_Text.L_Five;
            LinkLabel_Server_Home.ActiveLinkColor = Color_Text.L_Five;

            Label_Insider_Build_Number.ForeColor = Color_Text.L_Five;

            Input_Console.BackColor = Color_Winform_Other.Input;
            Input_Console.ForeColor = Color_Text.L_Five;

            Button_Console_Submit.ForeColor = Color_Winform_Buttons.Green_Fore_Color;
            Button_Console_Submit.BackColor = Color_Winform_Buttons.Green_Back_Color;
            Button_Console_Submit.FlatAppearance.BorderColor = Color_Winform_Buttons.Green_Border_Color;
            Button_Console_Submit.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Green_Mouse_Over_Back_Color;

            /********************************/
            /* Events                        /
            /********************************/

            Label_Status_Launcher.Click += new EventHandler(Update_Popup_Click);
            Label_Status_Launcher_Version.Click += new EventHandler(Update_Popup_Click);

            LinkLabel_Server_Twitter.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.TwitterAccountLink_LinkClicked);
            LinkLabel_Server_Facebook.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.FacebookGroupLink_LinkClicked);
            LinkLabel_Server_Discord.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.DiscordInviteLink_LinkClicked);
            LinkLabel_Server_Home.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.HomePageLink_LinkClicked);

            Button_Select_Server.Click += new EventHandler(FunctionEvents.SelectServerBtn_Click);
            
            Button_Close.MouseEnter += new EventHandler(ButtonClose_MouseEnter);
            Button_Close.MouseLeave += new EventHandler(ButtonClose_MouseLeaveANDMouseUp);
            Button_Close.MouseUp += new MouseEventHandler(ButtonClose_MouseLeaveANDMouseUp);
            Button_Close.MouseDown += new MouseEventHandler(ButtonClose_MouseDown);
            if (Parent_Screen.Screen_Instance != null)
            {
                Button_Close.Click += new EventHandler(Parent_Screen.Screen_Instance.Button_Close_Click);
            }

            Button_Settings.MouseEnter += new EventHandler(ButtonSettings_MouseEnter);
            Button_Settings.MouseLeave += new EventHandler(ButtonSettings_MouseLeaveANDMouseUp);
            Button_Settings.MouseUp += new MouseEventHandler(ButtonSettings_MouseLeaveANDMouseUp);
            Button_Settings.MouseDown += new MouseEventHandler(ButtonSettings_MouseDown);
            Button_Settings.Click += new EventHandler(SettingsButton_Click);

            Button_Security_Center.MouseEnter += new EventHandler(ButtonSecurityCenter_MouseEnter);
            Button_Security_Center.MouseLeave += new EventHandler(ButtonSecurityCenter_MouseLeaveANDMouseUp);
            Button_Security_Center.MouseUp += new MouseEventHandler(ButtonSecurityCenter_MouseLeaveANDMouseUp);
            Button_Security_Center.MouseDown += new MouseEventHandler(ButtonSecurityCenter_MouseDown);
            Button_Security_Center.Click += new EventHandler(ButtonSecurityCenter_Click);

            Button_Login.MouseEnter += new EventHandler(LoginButton_MouseEnter);
            Button_Login.MouseLeave += new EventHandler(LoginButton_MouseLeave);
            Button_Login.MouseUp += new MouseEventHandler(LoginButton_MouseUp);
            Button_Login.MouseDown += new MouseEventHandler(LoginButton_MouseDown);
            Button_Login.Click += new EventHandler(LoginButton_Click);

            Button_Logout.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            Button_Logout.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            Button_Logout.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            Button_Logout.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);
            Button_Logout.Click += new EventHandler(LogoutButton_Click);

            Button_Custom_Server.Click += new EventHandler(FunctionEvents.AddServer_Click);

            Input_Email.KeyUp += new KeyEventHandler(Loginbuttonenabler);
            Input_Email.KeyDown += new KeyEventHandler(LoginEnter);
            Input_Password.KeyUp += new KeyEventHandler(Loginbuttonenabler);
            Input_Password.KeyDown += new KeyEventHandler(LoginEnter);

            ComboBox_Server_List.SelectedIndexChanged += new EventHandler(ComboBox_Server_List_SelectedIndexChanged);
            ComboBox_Server_List.DrawItem += new DrawItemEventHandler(FunctionEvents.ServerList_Menu_DrawItem);
            ComboBox_Server_List.MouseWheel += new MouseEventHandler(ComboBox_Server_List_MouseWheel);

            LinkLabel_Forgot_Password.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.ForgotPassword_LinkClicked);

            if (Parent_Screen.Screen_Instance != null)
            {
                MouseMove += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Move);
                MouseUp += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Up);
                MouseDown += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Down);

                Picture_Logo.MouseMove += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Move);
                Picture_Logo.MouseUp += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Up);
                Picture_Logo.MouseDown += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Down);

                Label_Debug_Language.MouseMove += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Move);
                Label_Debug_Language.MouseUp += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Up);
                Label_Debug_Language.MouseDown += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Down);
            }

            Button_Play_OR_Update.MouseEnter += new EventHandler(PlayButton_MouseEnter);
            Button_Play_OR_Update.MouseLeave += new EventHandler(PlayButton_MouseLeave);
            Button_Play_OR_Update.MouseUp += new MouseEventHandler(PlayButton_MouseUp);
            Button_Play_OR_Update.MouseDown += new MouseEventHandler(PlayButton_MouseDown);
            Button_Play_OR_Update.Click += new EventHandler(PlayButton_Click);

            Button_Register.MouseEnter += new EventHandler(Greenbutton_hover_MouseEnter);
            Button_Register.MouseLeave += new EventHandler(Greenbutton_MouseLeave);
            Button_Register.MouseUp += new MouseEventHandler(Greenbutton_hover_MouseUp);
            Button_Register.MouseDown += new MouseEventHandler(Greenbutton_click_MouseDown);
            Button_Register.Click += new EventHandler(Button_Register_Click);

            Load += new EventHandler(MainScreen_Load);

            Input_Console.KeyDown += new KeyEventHandler(Console_Quick_Send);
            Button_Console_Submit.Click += new EventHandler(Console_Enter);

            KeyPreview = true;

            /********************************/
            /* Enable/Disable Visuals        /
            /********************************/

            Button_Select_Server.Visible = EnableInsiderDeveloper.Allowed();

            /********************************/
            /* Set Hardcoded Text            /
            /********************************/

            Label_Client_Ping.Text = string.Empty;

            /********************************/
            /* Functions                     /
            /********************************/

            Closing += (x, y) =>
            {
                Screen_Instance = null;
            };

            Shown += async (x, y) =>
            {
                Game_Folder_Checks();
                await Task.Run(() =>
                {
                    Presence_Launcher.Update();

                    if (ServerListUpdater.NoCategoryList != null && ServerListUpdater.NoCategoryList.Any())
                    {
                        foreach (Json_List_Server Servers in ServerListUpdater.NoCategoryList)
                        {
                            if (Nfswstarted != null || Parent_Screen.Screen_Instance == null || Screen_Instance == null)
                            {
                                break;
                            }
                            else
                            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                GC.Collect();
#endif

                                try
                                {
                                    while (StillCheckingLastServer) { }
                                    Uri URLCall = new Uri(Servers.IPAddress + "/GetServerInformation");
                                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                    Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
                                    var Client = new WebClient
                                    {
                                        Encoding = Encoding.UTF8,
                                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                                    };
                                    if (!Launcher_Value.Launcher_Alternative_Webcalls())
                                    {
                                        Client = new WebClientWithTimeout
                                        {
                                            Encoding = Encoding.UTF8,
                                            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                                        };
                                    }
                                    else
                                    {
                                        Client.Headers.Add("user-agent", "SBRW Launcher " +
                                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                    }

                                    try
                                    {
                                        JsonGSI = Client.DownloadString(URLCall);
                                        StillCheckingLastServer = true;
                                        bool GSIErrorFree = true;

                                        if (!JsonGSI.Valid_Json())
                                        {
                                            GSIErrorFree = false;
                                            if (EnableInsiderBetaTester.Allowed() || EnableInsiderBetaTester.Allowed())
                                            {
                                                Log.Error("Pinging GSI (Received): " + JsonGSI);
                                            }
                                        }

                                        if (!InformationCache.ServerStatusBook.ContainsKey(Servers.ID))
                                        {
                                            InformationCache.ServerStatusBook.Add(Servers.ID, (!GSIErrorFree) ? 3 : 1);
                                        }
                                    }
                                    catch (Exception Error)
                                    {
                                        LogToFileAddons.OpenLog("Pinging GSI [DownloadString]", string.Empty, Error, string.Empty, true);

                                        if (!InformationCache.ServerStatusBook.ContainsKey(Servers.ID))
                                        {
                                            InformationCache.ServerStatusBook.Add(Servers.ID, 0);
                                        }
                                    }
                                    finally
                                    {
                                        StillCheckingLastServer = false;

                                        Client?.Dispose();
                                    }
                                }
                                catch (Exception Error)
                                {
                                    LogToFileAddons.OpenLog("Pinging GSI [WebClient]", string.Empty, Error, string.Empty, true);
                                }
                                finally
                                {
                                    if (!string.IsNullOrWhiteSpace(JsonGSI))
                                    {
                                        JsonGSI = string.Empty;
                                    }
                                }
                            }
                        }
                    }
                });

#if !(RELEASE_UNIX || DEBUG_UNIX)
                GC.Collect(); 
#endif
            };
        }
#endregion

        public static void Clear_Hide_Screen_Form_Panel(bool From_Registration = false)
        {
            if (Screen_Instance != null)
            {
                if (Screen_Instance.Panel_Form_Screens.Visible)
                {
                    Screen_Instance.Panel_Form_Screens.Controls.Clear();
                    Screen_Instance.Panel_Form_Screens.Visible = false;
                }
                else if (Screen_Instance.Panel_Register_Screen.Visible)
                {
                    Screen_Instance.Panel_Register_Screen.Controls.Clear();
                    Screen_Instance.Panel_Register_Screen.Visible = false;
                }
            }

            if (Parent_Screen.Screen_Instance != null)
            {
                Parent_Screen.Screen_Instance.Text = "SBRW Launcher: " + Application.ProductVersion;
            }
        }

        public Screen_Main()
        {
            InitializeComponent();
            Set_Visuals();
            Screen_Instance = this;
        }

        private void UI_Timer_Tick(object sender, EventArgs e)
        {
            if (!this.Disposing || !this.IsDisposed)
            {
                switch (UI_MODE)
                {
                    /* Download Failed */
                    case -1:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(3))
                        {
                            Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Error_Outline;
                            Picture_Bar_Outline.Tag = Image_ProgressBar.Error_Outline.Tag;
                        }

                        ProgressBar.Value = 100;
                        if (ProgressBar.ID != 3)
                        {
                            ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Error_Top;
                            ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Error_Bottom;
                            ProgressBar.ID = 3;
                        }
                        
                        break;
                    /* Pack Downloader (In-Progress) */
                    case 1:
                        if (Pack_SBRW_Downloader != null)
                        {
                            Download_Information? Cached_Status = Pack_SBRW_Downloader.Download_Status();
                            if ((Cached_Status != null) && !Pack_SBRW_Downloader.Cancel)
                            {
                                if (Cached_Status.Download_Attempts > 0)
                                {
                                    Label_Download_Information_Support.Text = string.Format("Download Attempt {0}: Core Game Files Package", 
                                        Cached_Status.Download_Attempts).ToUpper();
                                }

                                Label_Download_Information.Text = (Time_Conversion.FormatFileSize(Cached_Status.File_Size_Current) + " of " + Time_Conversion.FormatFileSize(Cached_Status.File_Size_Total) +
                                    " (" + Cached_Status.Download_Percentage + "%) - " +
                                    Time_Conversion.EstimateFinishTime(Cached_Status.File_Size_Current, Cached_Status.File_Size_Total, Cached_Status.Start_Time)).ToUpperInvariant();

                                ProgressBar.Value = Cached_Status.Download_Percentage.Clamp(0, 100);

                                if (ProgressBar.ID != 0)
                                {
                                    ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Loading_Top;
                                    ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Loading_Bottom;
                                    ProgressBar.ID = 0;
                                }

                                if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(0))
                                {
                                    Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Checking_Outline;
                                    Picture_Bar_Outline.Tag = Image_ProgressBar.Checking_Outline.Tag;
                                }

                                Presence_Launcher.Status(2, string.Format("Downloaded {0}% of the Game!", Cached_Status.Download_Percentage));
                            }
                        }
                        break;
                    /* Pack Downloader (Progress Complete) */
                    case 2:
                        ProgressBar.Value = 0;

                        if (ProgressBar.ID != 0)
                        {
                            ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Loading_Top;
                            ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Loading_Bottom;
                            ProgressBar.ID = 0;
                        }

                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(0))
                        {
                            Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Checking_Outline;
                            Picture_Bar_Outline.Tag = Image_ProgressBar.Checking_Outline.Tag;
                        }

                        Label_Download_Information.Text = "Checking Package Integrity".ToUpperInvariant();
                        Label_Download_Information_Support.Text = "Downloaded: SBRW Game Files Package".ToUpperInvariant();
                        break;
                    /* Generic Loading */
                    case 3:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        Label_Download_Information.Text = "Loading".ToUpperInvariant();
                        ProgressBar.Value = 0;

                        if (ProgressBar.ID != 0)
                        {
                            ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Loading_Top;
                            ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Loading_Bottom;
                            ProgressBar.ID = 0;
                        }

                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(0))
                        {
                            Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Checking_Outline;
                            Picture_Bar_Outline.Tag = Image_ProgressBar.Checking_Outline.Tag;
                        }
                        break;
                    /* Unpack Archive */
                    case 4:
                        if(Pack_SBRW_Unpacker != null)
                        {
                            Extract_Information? Cached_Status = Pack_SBRW_Unpacker.Extract_Status();
                            if ((Cached_Status != null) && !Pack_SBRW_Unpacker.Cancel)
                            {
                                ProgressBar.Value = Cached_Status.Extract_Percentage.Clamp(0, 100);

                                if (!string.IsNullOrWhiteSpace(Cached_Status.File_Current_Name))
                                {
#pragma warning disable CS8602 // Dereference of a possibly null reference. (.NET 6)
                                    Label_Download_Information.Text = ("Unpacking " + Cached_Status.File_Current_Name.Replace(Pack_SBRW_Unpacker.File_Extension_Replacement, string.Empty)).ToUpperInvariant();
#pragma warning restore CS8602 // Dereference of a possibly null reference. (.NET 6)
                                }

                                Label_Download_Information_Support.Text = Cached_Status.Extract_Percentage + "% [" + Cached_Status.File_Current + " / " + Cached_Status.File_Total + "]".ToUpperInvariant();

                                Presence_Launcher.Status(1, string.Format("Unpacking Game: {0}%", Cached_Status.Extract_Percentage));

                                if (ProgressBar.ID != 0)
                                {
                                    ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Loading_Top;
                                    ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Loading_Bottom;
                                    ProgressBar.ID = 0;
                                }

                                if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(0))
                                {
                                    Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Checking_Outline;
                                    Picture_Bar_Outline.Tag = Image_ProgressBar.Checking_Outline.Tag;
                                }
                            }
                        }
                        break;
                    /* Generic Complete */
                    case 5:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        ProgressBar.Value = 100;

                        if (ProgressBar.ID != 1)
                        {
                            ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Sucess_Top;
                            ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Sucess_Bottom;
                            ProgressBar.ID = 1;
                        }

                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(1))
                        {
                            Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Complete_Outline;
                            Picture_Bar_Outline.Tag = Image_ProgressBar.Complete_Outline.Tag;
                        }
                        break;
                    /* Generic Warning */
                    case 6:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        ProgressBar.Value = 100;

                        if (ProgressBar.ID != 2)
                        {
                            ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Warning_Top;
                            ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Warning_Bottom;
                            ProgressBar.ID = 2;
                        }

                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(2))
                        {
                            Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Warning_Outline;
                            Picture_Bar_Outline.Tag = Image_ProgressBar.Warning_Outline.Tag;
                        }
                        break;
                    /* ModNet Progress (Downloading) */
                    case 7:
                        if (ModNet_Download_Status != null)
                        {
                            if (ModNet_Download_Status.Download_Complete)
                            {
                                if (UI_MODE != 8)
                                {
                                    UI_MODE = 8;
                                }

                                ModNet_Download_Status = null;
                            }
                            else
                            {
                                Label_Download_Information_Support.Text = ("Downloading - [" + CurrentModFileCount + " / " + TotalModFileCount + "] :").ToUpperInvariant();
                                Label_Download_Information.Text = (" Server Mods: " + ModNetFileNameInUse + " - " + Time_Conversion.FormatFileSize(ModNet_Download_Status.File_Size_Current) + " of " + Time_Conversion.FormatFileSize(ModNet_Download_Status.File_Size_Total)).ToUpperInvariant();

                                ProgressBar.Value = ModNet_Download_Status.Download_Percentage.Clamp(0, 100);

                                if (ProgressBar.ID != 0)
                                {
                                    ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Loading_Top;
                                    ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Loading_Bottom;
                                    ProgressBar.ID = 0;
                                }

                                if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(0))
                                {
                                    Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Checking_Outline;
                                    Picture_Bar_Outline.Tag = Image_ProgressBar.Checking_Outline.Tag;
                                }

                                if (ModNet_Download_Status.Download_Percentage >= 100)
                                {
                                    Presence_Launcher.Status(28, string.Empty);
                                }
                                else
                                {
                                    Presence_Launcher.Status(2, string.Format("Downloaded {0}% of Server Game Mods!", ModNet_Download_Status.Download_Percentage));
                                }
                            }
                        }
                        break;
                    /* Loading Game */
                    case 8:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        Presence_Launcher.Status(28, string.Empty);

                        if (Builtinserver)
                        {
                            Label_Download_Information.Text = "Soapbox server launched. Waiting for queries.".ToUpperInvariant();
                        }
                        else
                        {
                            Display_Color_Icons();
                            Label_Download_Information.Text = "Loading game. Launcher will minimize once Game has Loaded".ToUpperInvariant();
                            Label_Download_Information_Support.Text = string.Empty;
                            Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
#if NETFRAMEWORK
                            ContextMenu = new ContextMenu();
                            ContextMenu.MenuItems.Add(new MenuItem("Now Loading!!!", (b, n) => 
                            {
#if NETFRAMEWORK
                                Process.Start("https://www.youtube.com/watch?v=kq3X78ngFAY");
#else
                                Process.Start("explorer.exe", "https://www.youtube.com/watch?v=kq3X78ngFAY");
#endif
                            }));

                            ContextMenu.MenuItems.Add("-");
                            if (Parent_Screen.Screen_Instance != null)
                            {
                                ContextMenu.MenuItems.Add(new MenuItem("Close Game and Launcher", Parent_Screen.Screen_Instance.Button_Close_Click));
                            }
#endif
#if NETFRAMEWORK
                            NotifyIcon_Notification.ContextMenu = ContextMenu;
#endif
                        }
                        break;
                    /* Checking Folder Size (Pack Downloader) */
                    case 9:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        Label_Download_Information.Text = "Calculating Game Folder Size".ToUpperInvariant();
                        ProgressBar.Value = 0;

                        if (ProgressBar.ID != 0)
                        {
                            ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Loading_Top;
                            ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Loading_Bottom;
                            ProgressBar.ID = 0;
                        }

                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(0))
                        {
                            Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Checking_Outline;
                            Picture_Bar_Outline.Tag = Image_ProgressBar.Checking_Outline.Tag;
                        }
                        break;
                    case 10:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        Label_Download_Information_Support.Text = "Checking Game Files Package Hash".ToUpper();
                        ProgressBar.Value = 100;

                        if (ProgressBar.ID != 4)
                        {
                            ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Unknown_Top;
                            ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Unknown_Bottom;
                            ProgressBar.ID = 4;
                        }

                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(4))
                        {
                            Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Preload_Outline;
                            Picture_Bar_Outline.Tag = Image_ProgressBar.Preload_Outline.Tag;
                        }
                        break;
                    case 11:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        Label_Download_Information_Support.Text = "Downloading: Core Game Files Package".ToUpper();
                        ProgressBar.Value = 0;

                        if (ProgressBar.ID != 0)
                        {
                            ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Loading_Top;
                            ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Loading_Bottom;
                            ProgressBar.ID = 0;
                        }

                        if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(0))
                        {
                            Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Checking_Outline;
                            Picture_Bar_Outline.Tag = Image_ProgressBar.Checking_Outline.Tag;
                        }
                        break;
                    case 12:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        Label_Download_Information.Text = "Launcher: Checking NFSW EXE File Hash".ToUpperInvariant();
                        Label_Download_Information_Support.Text = string.Empty;
                        Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                        break;
                    case 13:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        if ((!IsDisposed || !Disposing))
                        {
                            Button_Close.Visible = false;

                            if (Parent_Screen.Screen_Instance != default)
                            {
                                Parent_Screen.Screen_Instance.WindowState = FormWindowState.Minimized;
                                Parent_Screen.Screen_Instance.ShowInTaskbar = false;
                            }
                        }
                        break;
                    case 14:
                        if (UI_MODE != 0)
                        {
                            UI_MODE = 0;
                        }

                        if (!IsDisposed || !Disposing)
                        {
                            Button_Close.Visible = Button_Logout.Visible = EnablePlayButton(true);

                            if (Parent_Screen.Screen_Instance != default)
                            {
                                Parent_Screen.Screen_Instance.WindowState = FormWindowState.Normal;
                                Parent_Screen.Screen_Instance.ShowInTaskbar = true;
                            }
                        }
                        break;
                    /* LZMA Downloader Progress */
                    case 15:
                        if (LZMA_Downloader != null)
                        {
                            Download_Information_LZMA? Cached_Status = LZMA_Downloader.Download_Status();
                            if ((Cached_Status != null) && LZMA_Downloader.Downloading)
                            {
                                Label_Download_Information.Text = (Time_Conversion.FormatFileSize(Cached_Status.File_Size_Current) + " of " + Time_Conversion.FormatFileSize(Cached_Status.File_Size_Total) +
                                    " (" + Cached_Status.Download_Percentage + "%) - " +
                                    Time_Conversion.EstimateFinishTime(Cached_Status.File_Size_Current, Cached_Status.File_Size_Total, Cached_Status.Start_Time)).ToUpperInvariant();

                                ProgressBar.Value = Cached_Status.Download_Percentage.Clamp(0, 100);

                                if (ProgressBar.ID != 0)
                                {
                                    ProgressBar.BackColor = Color_Winform_Other.ProgressBar_Loading_Top;
                                    ProgressBar.ForeColor = Color_Winform_Other.ProgressBar_Loading_Bottom;
                                    ProgressBar.ID = 0;
                                }

                                if (Picture_Bar_Outline.Tag == default || !Picture_Bar_Outline.Tag.Equals(0))
                                {
                                    Picture_Bar_Outline.BackgroundImage = Image_ProgressBar.Checking_Outline;
                                    Picture_Bar_Outline.Tag = Image_ProgressBar.Checking_Outline.Tag;
                                }
                                
                                Presence_Launcher.Status(2, string.Format("Downloaded {0}% of the Game!", Cached_Status.Download_Percentage));
                            }
                        }
                        break;
                }
            }
        }
    }
}
