using Newtonsoft.Json;
using SBRW.Launcher.App.Classes.Auth;
using SBRW.Launcher.App.Classes.InsiderKit;
using SBRW.Launcher.App.Classes.LauncherCore.APICheckers;
using SBRW.Launcher.App.Classes.LauncherCore.Client;
using SBRW.Launcher.App.Classes.LauncherCore.Client.Auth;
using SBRW.Launcher.App.Classes.LauncherCore.FileReadWrite;
using SBRW.Launcher.App.Classes.LauncherCore.Global;
using SBRW.Launcher.App.Classes.LauncherCore.Languages.Visual_Forms;
using SBRW.Launcher.App.Classes.LauncherCore.LauncherUpdater;
using SBRW.Launcher.App.Classes.LauncherCore.Lists;
using SBRW.Launcher.App.Classes.LauncherCore.Lists.JSON;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.App.Classes.LauncherCore.ModNet;
using SBRW.Launcher.App.Classes.LauncherCore.ModNet.JSON;
using SBRW.Launcher.App.Classes.LauncherCore.Support;
using SBRW.Launcher.App.Classes.SystemPlatform.Unix;
using SBRW.Launcher.App.Classes.SystemPlatform.Windows;
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
using SBRW.Launcher.Core.Extension.Bitmap_;
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
using SBRW.Launcher.Core.Required.DLL.User32_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Net.Cache;

namespace SBRW.Launcher.App.UI_Forms.Main_Screen
{
    public partial class Screen_Main : Form
    {
        public static Screen_Main? Screen_Instance { get; set; }
        public static Panel? Screen_Panel_Forms { get; set; }
        private bool Launcher_Restart { get; set; }
        private Point Mouse_Down_Point { get; set; } = Point.Empty;
        private bool LoginEnabled { get; set; }
        private bool ServerEnabled { get; set; }
        private bool Builtinserver { get; set; }
        private bool SkipServerTrigger { get; set; }
        private bool Playenabled { get; set; }
        private bool IsDownloading { get; set; } = true;
        private bool DisableLogout { get; set; }

        public static string GetTempName { get; set; } = Path.GetTempFileName();

        private static int LastSelectedServerId { get; set; }
        private static int NfswPid { get; set; }
        private static Thread? Nfswstarted { get; set; }
        private static bool StillCheckingLastServer { get; set; }
        private static bool ServerChangeTriggered { get; set; }

        private static DateTime DownloadStartTime { get; set; }
        
        private static Download_LZMA_Data? LZMA_Downloader { get; set; }
        private static Download_Queue? Pack_SBRW_Downloader { get; set; }
        private static Download_Extract? Pack_SBRW_Unpacker { get; set; }
        private static int Pack_SBRW_Downloader_Time_Span { get; set; }


        private static string JsonGSI { get; set; } = string.Empty;
        private static MemoryStream? ServerRawBanner { get; set; }
        private string LoginWelcomeTime { get; set; } = string.Empty;
        private string LoginToken { get; set; } = string.Empty;
        private string UserId { get; set; } = string.Empty;
        private static int ServerSecondsToShutDown { get; set; }
        private static Ping? CheckMate { get; set; }

        public static string ModNetFileNameInUse { get; set; } = string.Empty;
        public static Queue<Uri> ModFilesDownloadUrls { get; set; } = new Queue<Uri>();
        public static bool IsDownloadingModNetFiles { get; set; }
        public static int CurrentModFileCount { get; set; }
        public static int TotalModFileCount { get; set; }
        public static string Custom_SBRW_Pack { get { return Path.Combine(Locations.LauncherFolder, "GameFiles.sbrwpack"); } }
        public static BackgroundWorker? BackgroundWorker_One { get; set; }

        public void Move_Window_Mouse_Down(object sender, MouseEventArgs e)
        {
            if (e.Y <= 90) Mouse_Down_Point = new Point(e.X, e.Y);
        }

        public void Move_Window_Mouse_Up(object sender, MouseEventArgs e)
        {
            Mouse_Down_Point = Point.Empty;
            Opacity = 1;
        }

        public void Move_Window_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (Mouse_Down_Point.IsEmpty) { return; }
            Form Main_Local_Window = this as Form;
            Main_Local_Window.Location = new Point(Main_Local_Window.Location.X + (e.X - Mouse_Down_Point.X), Main_Local_Window.Location.Y + (e.Y - Mouse_Down_Point.Y));
            InformationCache.ParentScreenLocation = new Point(Main_Local_Window.Location.X + (e.X - Mouse_Down_Point.X), Main_Local_Window.Location.Y + (e.Y - Mouse_Down_Point.Y));
            Opacity = 0.9;
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
            Button_Settings.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Bad") ? Image_Icon.Gear_Warning_Click : Image_Icon.Gear_Click;
        }

        private void ButtonSettings_MouseEnter(object sender, EventArgs e)
        {
            Button_Settings.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Bad") ? Image_Icon.Gear_Warning_Hover : Image_Icon.Gear_Hover;
        }

        private void ButtonSettings_MouseLeaveANDMouseUp(object sender, EventArgs e)
        {
            Button_Settings.BackgroundImage = (Save_Settings.Live_Data.Game_Integrity == "Bad") ? Image_Icon.Gear_Warning : Image_Icon.Gear;
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
            Picture_Input_Email.Image = Image_Other.Text_Border_Email;
            Picture_Input_Password.Image = Image_Other.Text_Border_Password;
        }

        private void Graybutton_click_MouseDown(object sender, EventArgs e)
        {
            Button_Logout.BackgroundImage = Image_Button.Grey_Click;
        }

        private void Graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            Button_Logout.BackgroundImage = Image_Button.Grey_Hover;
        }

        private void Graybutton_MouseLeave(object sender, EventArgs e)
        {
            Button_Logout.BackgroundImage = Image_Button.Grey;
        }

        private void Graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            Button_Logout.BackgroundImage = Image_Button.Grey_Hover;
        }

        private void LoginEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && LoginEnabled)
            {
                LoginButton_Click(null, null);
                e.SuppressKeyPress = true;
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
                Picture_Information_Window.Image = Image_Other.Information_Window_Success;

                Button_Play_OR_Update.BackgroundImage = Image_Button.Play;
                Button_Play_OR_Update.ForeColor = Color_Text.L_Five;

                Button_Logout.BackgroundImage = Image_Button.Grey;
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
            Picture_Input_Email.Image = Image_Other.Text_Border_Email;
            Picture_Input_Password.Visible = hideElements;
            Picture_Input_Password.Image = Image_Other.Text_Border_Password;
            Picture_Information_Window.Image = Image_Other.Information_Window;
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
                MessageBox.Show(null, "Please wait while the GameLauncher is still downloading the game files.", "GameLauncher", MessageBoxButtons.OK);
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
                    MessageBox.Show(null, Tokens.Warning, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Input_Email.Text = Email;
                }

                LoginFormElements(false);
                LoggedInFormElements(true);
            }
            else
            {
                /* Main Screen Login */
                Picture_Input_Email.Image = Image_Other.Text_Border_Email_Error;
                Picture_Input_Password.Image = Image_Other.Text_Border_Password_Error;
                Picture_Information_Window.Image = Image_Other.Information_Window_Error;
                Input_Email.Text = "EMAIL IS HIDDEN";
                MessageBox.Show(null, Tokens.Error, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Input_Email.Text = Email;
            }
        }

        private void LoginButton_MouseEnter(object sender, EventArgs e)
        {
            if (LoginEnabled || Builtinserver)
            {
                Button_Login.BackgroundImage = Image_Button.Grey_Hover;
                Button_Login.ForeColor = Color_Text.L_Five;
            }
            else
            {
                Button_Login.BackgroundImage = Image_Button.Grey;
                Button_Login.ForeColor = Color_Text.L_Six;
            }
        }

        private void LoginButton_MouseLeave(object sender, EventArgs e)
        {
            if (LoginEnabled || Builtinserver)
            {
                Button_Login.BackgroundImage = Image_Button.Grey;
                Button_Login.ForeColor = Color_Text.L_Five;
            }
            else
            {
                Button_Login.BackgroundImage = Image_Button.Grey;
                Button_Login.ForeColor = Color_Text.L_Six;
            }
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

        private void ClosingTasks()
        {
            Save_Settings.Save();
            Save_Account.Save();

            try
            {
                if (LZMA_Downloader != null)
                {
                    LZMA_Downloader.Stop();
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("CDN DOWNLOADER [LZMA]", string.Empty, Error, string.Empty, true);
            }

            try
            {
                if (Pack_SBRW_Unpacker != null)
                {
                    Pack_SBRW_Unpacker.Cancel = true;
                }

                if (Pack_SBRW_Downloader != null)
                {
                    Pack_SBRW_Downloader.Cancel = true;
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("CDN DOWNLOADER", string.Empty, Error, string.Empty, true);
            }

            try
            {
                if (BackgroundWorker_One != null)
                {
                    if (BackgroundWorker_One.IsBusy)
                    {
                        BackgroundWorker_One.CancelAsync();
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("BackgroundWorker_One", string.Empty, Error, string.Empty, true);
            }

            try
            {
                if (FunctionStatus.LauncherBattlePass)
                {
                    Process.GetProcessById(NfswPid).Kill();
                }
                else
                {
                    Process[] allOfThem = Process.GetProcessesByName("nfsw");

                    if (allOfThem != null && allOfThem.Any())
                    {
                        foreach (var oneProcess in allOfThem)
                        {
                            Process.GetProcessById(oneProcess.Id).Kill();
                        }
                    }
                }
            }
            catch { }

            if (Presence_Launcher.Running())
            {
                Presence_Launcher.Stop("Close");
            }

            if (Proxy_Settings.Running())
            {
                Proxy_Server.Instance.Stop("Main Screen");
            }

            try { NotifyIcon_Notification.Dispose(); } catch { }

            if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)) && !FunctionStatus.LauncherBattlePass)
            {
                ModNetHandler.CleanLinks(Save_Settings.Live_Data.Game_Path);
            }
        }

        private void Button_Close_Click(object? sender, EventArgs? e)
        {
            ClosingTasks();

            /* Leave this here. Its to properly close the launcher from Visual Studio (And Close the Launcher a well) 
             * If the Boolen is true it will restart the Application
             */
            if (Launcher_Restart)
            {
                Application.Restart();
            }
            else
            {
                try { this.Close(); } catch { }
            }
        }

        /* SETTINGS PAGE LAYOUT */
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            try
            {
                Screen_Settings Custom_Instance_Settings = new Screen_Settings() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true, FormBorderStyle = FormBorderStyle.None };
                Panel_Form_Screens.Visible = true;
                Panel_Form_Screens.Controls.Add(Custom_Instance_Settings);
                Custom_Instance_Settings.Show();
                Text = "Settings - SBRW Launcher: v" + Application.ProductVersion;
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
                Text = "Security Center - SBRW Launcher: v" + Application.ProductVersion;
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

            Button_Play_OR_Update.BackgroundImage = Image_Button.Play_Hover;
        }

        private void PlayButton_MouseDown(object sender, EventArgs e)
        {
            if (Playenabled == false)
            {
                return;
            }

            Button_Play_OR_Update.BackgroundImage = Image_Button.Play_Click;
        }

        private void PlayButton_MouseEnter(object sender, EventArgs e)
        {
            if (Playenabled == false)
            {
                return;
            }

            Button_Play_OR_Update.BackgroundImage = Image_Button.Play_Hover;
        }

        private void PlayButton_MouseLeave(object sender, EventArgs e)
        {
            if (Playenabled == false)
            {
                return;
            }

            Button_Play_OR_Update.BackgroundImage = Image_Button.Play;
        }

        private void DisablePlayButton()
        {
            IsDownloading = false;
            Playenabled = false;

            ProgressBar_Extracting.Value = 100;
            ProgressBar_Extracting.Width = 519;
        }

        /* Social Panel | Ping or Offline or DEV Servers | */
        private void DisableSocialPanelandClearIt()
        {
            /* Hides Social Panel */
            Panel_Server_Information.Visible = false;
            /* Home */
            Picture_Icon_Server_Home.BackgroundImage = Image_Icon.Home_Disabled;
            LinkLabel_Server_Home.Enabled = false;
            /* Discord */
            Picture_Icon_Server_Discord.BackgroundImage = Image_Icon.Discord_Disabled;
            LinkLabel_Server_Discord.Enabled = false;
            /* Facebook */
            Picture_Icon_Server_Facebook.BackgroundImage = Image_Icon.Facebook_Disabled;
            LinkLabel_Server_Facebook.Enabled = false;
            /* Twitter */
            Picture_Icon_Server_Twitter.BackgroundImage = Image_Icon.Twitter_Disabled;
            LinkLabel_Server_Twitter.Enabled = false;
            /* Scenery */
            Label_Server_Scenery.Text = "But It's Me!";
            /* Restart Timer */
            Label_Server_Force_Restart_Timer.Text = "Game Launcher!";
        }

        private void Game_Bootup(string UserID, string LoginToken)
        {
            if (InformationCache.SelectedServerEnforceProxy)
            {
                if (!Proxy_Settings.Running())
                {
                    Proxy_Server.Instance.Start("Start Game");
                }
            }

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

            Presence_Launcher.Status("In-Game", null);
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
                        Label_Status_API.Text = "Connection API:\n - Error";
                        Label_Status_API.ForeColor = Color_Text.S_Error;
                        Label_Status_API_Details.Text = "Launcher is Offline";
                        Picture_Icon_API.BackgroundImage = Image_Icon.Plug_Offline;
                        Log.Api("PINGING API: Failed to Connect to APIs! Quick Hide and Bunker Down! (Ask for help)");
                    }
                }
            }
        }

        /* Launch game */
        private void Game_Check_Launch()
        {
            Presence_Launcher.Start("New RPC", Presence_Launcher.ApplicationID());

            try
            {
                string GameExePath = Path.Combine(Save_Settings.Live_Data.Game_Path, "nfsw.exe");
                if
                  (
                    Hashes.Hash_SHA(GameExePath) == "7C0D6EE08EB1EDA67D5E5087DDA3762182CDE4AC" ||
                    Hashes.Hash_SHA(GameExePath) == "DB9287FB7B0CDA237A5C3885DD47A9FFDAEE1C19" ||
                    Hashes.Hash_SHA(GameExePath) == "E69890D31919DE1649D319956560269DB88B8F22" ||
                    Hashes.Hash_SHA(GameExePath) == "3CBE3FAAFF00FAD84F78A2AFEA4FFFC78294EEA2"
                  )
                {
                    Launcher_Value.Game_Server_Name = ServerListUpdater.ServerName("Proxy");
                    Launcher_Value.Game_Server_IP = Launcher_Value.Launcher_Select_Server_Data.IPAddress;

                    Launcher_Value.Game_User_ID = UserId;
                    Launcher_Value.Game_Server_IP_Host = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress).Host;

                    Game_Bootup(UserId, LoginToken);

                    if (Builtinserver)
                    {
                        Label_Download_Information.Text = "Soapbox server launched. Waiting for queries.".ToUpper();
                    }
                    else
                    {
                        Application.DoEvents();

                        ProgressBar_Extracting.Value = 100;
                        ProgressBar_Extracting.Width = 519;

                        Label_Download_Information_Support.Text = string.Empty;
                        Label_Download_Information.Text = "Loading game. Launcher will minimize once Game has Loaded".ToUpper();
                        Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
#if NETFRAMEWORK

                        ContextMenu = new ContextMenu();
                        ContextMenu.MenuItems.Add(new MenuItem("Running Out of Time", (b, n) => { Process.Start("https://youtu.be/vq9-bmoI-RI"); }));
                        ContextMenu.MenuItems.Add("-");
                        ContextMenu.MenuItems.Add(new MenuItem("Close Game and Launcher", Button_Close_Click));
#endif
                        Update();
                        Refresh();

                        NotifyIcon_Notification.Text = "SBRW Launcher";
#if NETFRAMEWORK
                        NotifyIcon_Notification.ContextMenu = ContextMenu;
#endif
                    }
                }
                else if (!File.Exists(GameExePath))
                {
                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    MessageBox.Show(null, "You do not have the Game Downloaded. Please Verify Game Files installation path.", "GameLauncher",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    MessageBox.Show(null, "Your NFSW.exe is Modified. Please Verify Game Files.", "GameLauncher",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception Error)
            {
                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                LogToFileAddons.OpenLog("GAME LAUNCH", Error.Message, Error, "Error", false);
            }
        }

        private void Game_Live_Data(string UserID, string LoginToken, string ServerIP, Form x)
        {
            if (Process_Start_Game.Initialize(Save_Settings.Live_Data.Game_Path, ServerIP, LoginToken,
                UserID, Launcher_Value.Launcher_Select_Server_Data.ID.ToUpper()) != null)
            {
                FunctionStatus.LauncherBattlePass = Process_Start_Game.Live_Process.EnableRaisingEvents = true;
                NfswPid = Process_Start_Game.Live_Process.Id;

                /* TIMER HERE */
                System.Timers.Timer shutdowntimer = new System.Timers.Timer();
                shutdowntimer.Elapsed += new System.Timers.ElapsedEventHandler(Time_Window.ClockWork_Planet);
                Time_Window.Session_Alert += (x, D_Live_Events) =>
                {
                    if (D_Live_Events != null)
                    {
                        try
                        {
                            NotifyIcon_Notification.Visible = D_Live_Events.Valid;
                            NotifyIcon_Notification.BalloonTipIcon = ToolTipIcon.Info;
                            NotifyIcon_Notification.BalloonTipTitle = "Force Restart - " + Launcher_Value.Game_Server_Name;
                            NotifyIcon_Notification.BalloonTipText = "Game will shutdown by " + (D_Live_Events.Session_End_Time ?? DateTime.Now.AddMinutes(5)).ToString("t") + ". Please restart it manually before the launcher does it.";
                            NotifyIcon_Notification.ShowBalloonTip(TimeSpan.FromMinutes(2).Seconds);
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("NotifyIcon_Notification Timer", string.Empty, Error, "Error", true);
                        }
                        finally
                        {
                            GC.Collect();
                        }
                    }
                };

                shutdowntimer.Interval = !Proxy_Settings.Running() ? 30000 : 60000;
                shutdowntimer.Enabled = true;

                Button_Close.SafeInvokeAction(() =>
                Button_Close.Visible = false, this);

                this.SafeInvokeAction(() =>
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.ShowInTaskbar = false;
                }, this);

                Process_Start_Game.Live_Process.Exited += (Send, It) =>
                {
                    NfswPid = 0;
                    int exitCode = Process_Start_Game.Live_Process.ExitCode;

                    FunctionStatus.LauncherBattlePass = false;

                    if (Launcher_Value.Game_In_Event_Bug)
                    {
                        if (AC_Core.Status) exitCode = 2017;
                        else exitCode = 2137;
                    }

                    if (exitCode == 0 && !Launcher_Value.Game_In_Event_Bug && AC_Core.Stop_Check())
                    {
                        Button_Close_Click(null, null);
                    }
                    else if (AC_Core.Stop_Check())
                    {
                        x.SafeEndInvokeAsyncCatch(x.SafeBeginInvokeActionAsync(Launcher_X_Form =>
                        {
                            x.WindowState = FormWindowState.Normal;
                            x.ShowInTaskbar = true;

                            string Error_Msg = NFSW.ErrorTranslation(exitCode);
                            Log.Error("GAME CRASH [EXIT CODE]: " + exitCode.ToString() + " HEX: (0x" + exitCode.ToString("X") + ")" + " REASON: " + Error_Msg);

                            Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            Label_Download_Information.Text = Error_Msg.ToUpper();
                            ProgressBar_Preload.Value = 100;
                            ProgressBar_Preload.ForeColor = Color_Text.S_Error;

                            if (NfswPid != 0)
                            {
                                try
                                {
                                    Process.GetProcessById(NfswPid).Kill();
                                }
                                catch { /* ignored */ }
                            }

                            if (Nfswstarted != null)
                            {
                                Nfswstarted.Abort();
                            }
                            
                            DialogResult restartApp = MessageBox.Show(null, Error_Msg + "\nWould you like to restart the GameLauncher?",
                                "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (restartApp == DialogResult.Yes)
                            {
                                Launcher_Restart = true;
                            }
                            this.Button_Close_Click(null, null);
                        }));
                    }
                };
            }
        }

        private void Client_DownloadProgressChanged_RELOADED(object sender, DownloadProgressChangedEventArgs e)
        {
            Label_Download_Information_Support.SafeInvokeAction(() =>
            Label_Download_Information_Support.Text = ("Downloading - [" + CurrentModFileCount + " / " + TotalModFileCount + "] :").ToUpper(), this);

            if (e.TotalBytesToReceive >= 1)
            {
                Label_Download_Information.SafeInvokeAction(() =>
                Label_Download_Information.Text = (" Server Mods: " + ModNetFileNameInUse + " - " + Time_Conversion.FormatFileSize(e.BytesReceived) + " of " + Time_Conversion.FormatFileSize(e.TotalBytesToReceive)).ToUpper(), this);

                ProgressBar_Extracting.SafeInvokeAction(() =>
                {
                    ProgressBar_Extracting.Value = Convert.ToInt32(decimal.Divide(e.BytesReceived, e.TotalBytesToReceive) * 100);
                    ProgressBar_Extracting.Width = Convert.ToInt32(decimal.Divide(e.BytesReceived, e.TotalBytesToReceive) * 519);
                }, this);
            }
        }

        public void DownloadModNetFilesRightNow(string path)
        {
            while (IsDownloadingModNetFiles == false)
            {
                CurrentModFileCount++;
                Uri url = ModFilesDownloadUrls.Dequeue();
                string FileName = url.ToString().Substring(url.ToString().LastIndexOf("/") + 1, (url.ToString().Length - url.ToString().LastIndexOf("/") - 1));

                ModNetFileNameInUse = FileName;

#pragma warning disable SYSLIB0014 // Type or member is obsolete
                ServicePointManager.FindServicePoint(url).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                var Client = new WebClient
                {
                    Encoding = Encoding.UTF8,
                    CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                };
#pragma warning restore SYSLIB0014 // Type or member is obsolete


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
                    Client.DownloadFileCompleted += (test, stuff) =>
                    {
                        Log.Core("LAUNCHER: Downloaded: " + FileName);
                        IsDownloadingModNetFiles = false;
                        if (!ModFilesDownloadUrls.Any())
                        {
                            Game_Check_Launch();
                        }
                        else
                        {
                            /* Redownload other file */
                            DownloadModNetFilesRightNow(path);
                        }
                    };
                    Client.DownloadFileAsync(url, path + "/" + FileName);
                }
                catch (Exception Error)
                {
                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    LogToFileAddons.OpenLog("Modnet Server Files", string.Empty, Error, string.Empty, true);
                }
                finally
                {
                    if (Client != null)
                    {
                        Client.Dispose();
                    }

                    Application.DoEvents();
                }

                IsDownloadingModNetFiles = true;
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (!UnixOS.Detected())
            {
                DriveInfo driveInfo = new DriveInfo(Save_Settings.Live_Data.Game_Path);

                if (!string.Equals(driveInfo.DriveFormat, "NTFS", StringComparison.InvariantCultureIgnoreCase))
                {
                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    MessageBox.Show(
                        $"Playing the game on a non-NTFS-formatted drive is not supported.\nDrive '{driveInfo.Name}' is formatted with: {driveInfo.DriveFormat}",
                        "Compatibility",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Save_Account.Live_Data.User_Raw_Email).ToUpper();
                    return;
                }
            }

            if (Save_Settings.Live_Data.Game_Integrity != "Good")
            {
                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                MessageBox.Show("GameLauncher has detected a GameFiles Integrity Error\nPlease 'Verify GameFiles' in the Settings Screen",
                    "Game Files Integrity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Save_Account.Live_Data.User_Raw_Email).ToUpper();
                return;
            }

            if (!Redistributable.Error_Free)
            {
                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                MessageBox.Show("GameLauncher has detected that the 2015-2019 VC++ Redistributable Package is not installed\n" +
                    "Please manually Install the Packages for your Operating System",
                    "VC++ Redistributable Package Check", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            /* Disable Play Button and Logout Buttons */
            Button_Play_OR_Update.Visible = false;
            Button_Logout.Visible = false;
            DisableLogout = true;
            DisablePlayButton();

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
                    Presence_Launcher.Status("Checking ModNet", null);
                    /* Get Remote ModNet list to process for checking required ModNet files are present and current */
                    Uri ModNetURI = new Uri(URLs.ModNet + "/launcher-modules/modules.json");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    ServicePointManager.FindServicePoint(ModNetURI).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var ModNetJsonURI = new WebClient
                    {
                        Encoding = Encoding.UTF8,
                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                    };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

                    if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                    { 
                        ModNetJsonURI = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) }; 
                    }
                    else
                    {
                        ModNetJsonURI.Headers.Add("user-agent", "SBRW Launcher " +
                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    }

                    try
                    {
                        ModulesJSON = ModNetJsonURI.DownloadString(ModNetURI);
                        Label_Download_Information.Text = "JSON: Retrieved ModNet Files Information".ToUpper();
                    }
                    catch (Exception Error)
                    {


                        Label_Download_Information.Text = ("JSON: Unable to Retrieve ModNet Files Information").ToUpper();
                        Presence_Launcher.Status("ModNet Files Information Error", null);
                        Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                        string LogMessage = "There was an error with ModNet JSON Retrieval:";
                        LogToFileAddons.OpenLog("MODNET FILES", LogMessage, Error, "Error", false);
                    }
                    finally
                    {
                        if (ModNetJsonURI != null)
                        {
                            ModNetJsonURI.Dispose();
                        }

                        Application.DoEvents();
                    }

                    if (string.IsNullOrWhiteSpace(ModulesJSON) || !Is_Json.Valid(ModulesJSON))
                    {
                        Label_Download_Information.Text = ("JSON: Invalid ModNet Files Information").ToUpper();
                        Presence_Launcher.Status("ModNet Files Information Error", null);
                        Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                        ModulesJSON = string.Empty;
                        return;
                    }
                    else
                    {
                        try
                        {
                            try
                            {
                                DateTime Time_Check = DateTime.Now.Date;
                                string Launcher_Data_Folder = Path.Combine("Launcher_Data", "JSON", "ModNet");
                                string Time_Stamp = Path.Combine(Launcher_Data_Folder, "Time_Stamp.txt");
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

                                if ((Time_Check < DateTime.Now.Date) || !File.Exists(Time_Stamp))
                                {
                                    if (!Directory.Exists(Launcher_Data_Folder))
                                    {
                                        Directory.CreateDirectory(Launcher_Data_Folder);
                                    }

                                    string Server_List_Cache = Path.Combine(Launcher_Data_Folder, "Modules.json");
                                    File.WriteAllText(Server_List_Cache, ModulesJSON);
                                    File.WriteAllText(Time_Stamp, DateTime.Now.ToString());
                                }
                            }
                            catch { }
                            finally
                            {
                                GC.Collect();
                            }

                            string[] modules_newlines = ModulesJSON.Split(new string[] { "\n" }, StringSplitOptions.None);
                            foreach (string modules_newline in modules_newlines)
                            {
                                if (modules_newline.Trim() == "{" || modules_newline.Trim() == "}") continue;

                                string trim_modules_newline = modules_newline.Trim();
                                string[] modules_files = trim_modules_newline.Split(new char[] { ':' });

                                string ModNetList = modules_files[0].Replace("\"", "").Trim();
                                string ModNetSHA = modules_files[1].Replace("\"", "").Replace(",", "").Trim();

                                string ModNetFilePath = Path.Combine(Save_Settings.Live_Data.Game_Path, ModNetList);

                                if (Hashes.Hash_SHA256(ModNetFilePath).ToLower() != ModNetSHA || !File.Exists(ModNetFilePath))
                                {
                                    Label_Download_Information.Text = ("ModNet: Downloading " + ModNetList).ToUpper();

                                    Log.Warning("MODNET CORE: " + ModNetList + " Does not match SHA Hash on File Server -> Online Hash: '" + ModNetSHA + "'");

                                    if (File.Exists(ModNetFilePath))
                                    {
                                        File.Delete(ModNetFilePath);
                                    }

                                    Presence_Launcher.Status("Download ModNet", ModNetList);

                                    Uri URLCall = new Uri(URLs.ModNet + "/launcher-modules/" + ModNetList);
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                    var newModNetFilesDownload = new WebClient
                                    {
                                        Encoding = Encoding.UTF8,
                                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                                    };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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

                                Application.DoEvents();
                            }
                        }
                        catch (Exception Error)
                        {
                            Presence_Launcher.Status("Download ModNet Error", null);
                            Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            string LogMessage = "There was an error with ModNet Files Check:";
                            LogToFileAddons.OpenLog("MODNET CORE", LogMessage, Error, "Error", false);
                            Application.DoEvents();

                            return;
                        }
                        finally
                        {
                            if (!string.IsNullOrWhiteSpace(ModulesJSON))
                            {
                                ModulesJSON = string.Empty;
                            }
                        }

                        Uri newModNetUri = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress + "/Modding/GetModInfo");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                        ServicePointManager.FindServicePoint(newModNetUri).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                        var ModInfoJson = new WebClient
                        {
                            Encoding = Encoding.UTF8,
                            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                        };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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
                            Presence_Launcher.Status("Server Mods Get Information Error", null);
                            Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            string LogMessage = "There was an error with Server Mod Information Retrieval:";
                            LogToFileAddons.OpenLog("SERVER MOD INFO", LogMessage, Error, "Error", false);
                        }
                        finally
                        {
                            if (ModInfoJson != null)
                            {
                                ModInfoJson.Dispose();
                            }

                            Application.DoEvents();
                        }

                        if (string.IsNullOrWhiteSpace(ServerModInfo) || !Is_Json.Valid(ServerModInfo))
                        {
                            Label_Download_Information.Text = ("JSON: Invalid Server Mod Information").ToUpper();
                            Presence_Launcher.Status("Server Mods Get Information Error", null);
                            Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                            ServerModInfo = string.Empty;
                            return;
                        }
                        else
                        {
                            /* get files now */
                            json2 = JsonConvert.DeserializeObject<GetModInfo>(ServerModInfo);
                            ServerModInfo = string.Empty;

                            /* Set and Get for RemoteRPC Files */
                            Uri URLCall_A = new Uri(json2.basePath + "/cars.json");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                            ServicePointManager.FindServicePoint(URLCall_A).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            var CarsJson = new WebClient
                            {
                                Encoding = Encoding.UTF8,
                                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                            };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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
                                if (CarsJson != null)
                                {
                                    CarsJson.Dispose();
                                }
                            }

                            Uri URLCall_B = new Uri(json2.basePath + "/events.json");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                            ServicePointManager.FindServicePoint(URLCall_B).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            var EventsJson = new WebClient
                            {
                                Encoding = Encoding.UTF8,
                                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                            };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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
                                if (EventsJson != null)
                                {
                                    EventsJson.Dispose();
                                }
                            }

                            /* Version 1.3 @metonator - DavidCarbon */
                            if (Is_Json.Valid(remoteCarsFile))
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

                            if (Is_Json.Valid(remoteEventsFile))
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
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                            ServicePointManager.FindServicePoint(newIndexFile).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            var ServerModsList = new WebClient
                            {
                                Encoding = Encoding.UTF8,
                                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                            };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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
                                Label_Download_Information.Text = ("JSON: Unable to Retrieve Server Mod List Information").ToUpper();
                                Presence_Launcher.Status("Server Mods Get Information Error", null);
                                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                                string LogMessage = "There was an error with Server Mod List Information Retrieval:";
                                LogToFileAddons.OpenLog("SERVER MOD JSON", LogMessage, Error, "Error", false);
                            }
                            finally
                            {
                                if (ServerModsList != null)
                                {
                                    ServerModsList.Dispose();
                                }

                                Application.DoEvents();
                            }

                            if (string.IsNullOrWhiteSpace(ServerModListJSON) || !Is_Json.Valid(ServerModListJSON))
                            {
                                Label_Download_Information.Text = ("JSON: Invalid Server Mod List Information").ToUpper();
                                Presence_Launcher.Status("Server Mods Get Information Error", null);
                                Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                                ServerModListJSON = string.Empty;
                                return;
                            }
                            else
                            {
                                try
                                {
                                    json3 = JsonConvert.DeserializeObject<ServerModList>(ServerModListJSON);
                                    ServerModListJSON = string.Empty;
                                    string ModFolderCache = Path.Combine(Save_Settings.Live_Data.Game_Path, "MODS", Hashes.Hash_String(0, json2.serverID).ToLower());
                                    if (!Directory.Exists(ModFolderCache)) Directory.CreateDirectory(ModFolderCache);

                                    /* (FILENAME.mods) 
                                     * Checks for any Files that Don't match the Server Index Json and Removes that File  */
                                    foreach (string file in Directory.GetFiles(ModFolderCache))
                                    {
                                        string name = Path.GetFileName(file);

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
                                    }

                                    /* (OLD-FILENAME.mods != NEW-FILENAME.mods)
                                     * Checks for the file and if the File Hash does not match it will be added to a list to be downloaded 
                                     * If a file exists and doesn't match a the server provided index json it will be deleted 
                                     * 5/22/2021: If a Server Extracted Mods Directory is present and 
                                     * if a Server Mod File no longer matches it will now delete the folder (.data/SERVER-ID-HASH) - DavidCarbon
                                     */
                                    int ExtractedServerFolderRunTime = 0;

                                    foreach (ServerModFileEntry modfile in json3.entries)
                                    {
                                        string ModCachedFile = Path.Combine(ModFolderCache, modfile.Name);
                                        if (Hashes.Hash_SHA(ModCachedFile).ToLower() != modfile.Checksum)
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

                                        Application.DoEvents();
                                    }

                                    if (ModFilesDownloadUrls.Count != 0)
                                    {
                                        this.DownloadModNetFilesRightNow(ModFolderCache);
                                        Presence_Launcher.Status("Download Server Mods", null);
                                    }
                                    else
                                    {
                                        Game_Check_Launch();
                                    }
                                }
                                catch (Exception Error)
                                {
                                    Presence_Launcher.Status("Download Server Mods Error", null);
                                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                                    string LogMessage = "There was an error with Server Mods Check:";
                                    LogToFileAddons.OpenLog("SERVER MOD DOWNLOAD", LogMessage, Error, "Error", false);
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
                    Presence_Launcher.Status("Download ModNet Error", null);
                    Label_Information_Window.Text = string.Format(LoginWelcomeTime + "\n{0}", Is_Email.Mask(Save_Account.Live_Data.User_Raw_Email)).ToUpper();
                    string LogMessage = "There was an error downloading ModNet Files:";
                    LogToFileAddons.OpenLog("MODNET FILES", LogMessage, Error, "Error", false);
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

                    Application.DoEvents();
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
            GC.Collect();
            Picture_Input_Email.Image = Image_Other.Text_Border_Email;
            Picture_Input_Password.Image = Image_Other.Text_Border_Password;
            /* Disable Certain Functions */
            LoginEnabled = false;
            ServerEnabled = false;
            FunctionStatus.AllowRegistration = false;
            Launcher_Value.Launcher_Select_Server_JSON = null;
            /* Disable Login & Register Button */
            Button_Login.Enabled = Button_Register.Enabled = false;
            /* Disable Social Panel when switching */
            DisableSocialPanelandClearIt();
            /* Stops any actions for a Server */
            ServerChangeTriggered = true;

            if (!ServerListUpdater.LoadedList && Launcher_Value.Launcher_Select_Server_Data == null)
            {
                Label_Status_Game_Server.Text = "Launcher Offline:\n - Unknown";
                Label_Status_Game_Server.ForeColor = Color_Text.L_Three;
                Label_Status_Game_Server_Data.Text = string.Empty;
                Picture_Icon_Server.BackgroundImage = Image_Icon.Server_Unknown;
                return;
            }

            Launcher_Value.Launcher_Select_Server_Data = (Json_List_Server)ComboBox_Server_List.SelectedItem;

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
            Picture_Icon_Server.BackgroundImage = Image_Icon.Server_Checking;

            Button_Login.ForeColor = Color_Text.L_Six;
            string Banner_Cache_Folder = Path.Combine(Locations.LauncherDataFolder, "Bin", "Server", "Banner", "EyeCatcher");
            string Banner_Cache_File = Path.Combine(Banner_Cache_Folder, Hashes.Hash_String(1, Launcher_Value.Launcher_Select_Server_Data.IPAddress) + ".bin");
#if NETFRAMEWORK
            Picture_Server_Banner.Image = Bitmap_Handler.Grayscale(Banner_Cache_File) ??Image_Other.Server_Banner;
#elif NET6_0_OR_GREATER && WINDOWS
            Picture_Server_Banner.Image = Image_Handler.Grayscale(Banner_Cache_File)??Image_Other.Server_Banner;
#endif
            Picture_Server_Banner.BackColor = Color.Transparent;
            string ImageUrl = string.Empty;
            string numPlayers = string.Empty;
            string numRegistered = string.Empty;

            if (ComboBox_Server_List.GetItemText(ComboBox_Server_List.SelectedItem) == "Offline Built-In Server")
            {
                Builtinserver = true;
                Button_Login.BackgroundImage = Image_Button.Grey;
                Button_Login.Text = "Launch".ToUpper();
                Button_Login.ForeColor = Color_Text.L_Five;
                Panel_Server_Information.Visible = false;
            }
            else
            {
                Builtinserver = false;
                Button_Login.BackgroundImage = Image_Button.Grey;
                Button_Login.Text = "Login".ToUpper();
                Button_Login.ForeColor = Color_Text.L_Six;
                Panel_Server_Information.Visible = false;
            }

            Uri ServerURI = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress + "/GetServerInformation");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            ServicePointManager.FindServicePoint(ServerURI).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            var Client = new WebClient
            {
                Encoding = Encoding.UTF8,
                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
            };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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

            System.Timers.Timer aTimer = new System.Timers.Timer(10000);
            aTimer.Elapsed += (x, y) => { Client.CancelAsync(); try { aTimer.Dispose(); } catch { } };
            aTimer.AutoReset = false;
            aTimer.Enabled = true;

            Client.DownloadStringCompleted += (sender2, e2) =>
            {
                aTimer.Enabled = false;
                try { aTimer.Dispose(); } catch { }

                bool GSIErrorFree = true;

                if (e2.Cancelled || e2.Error != null)
                {
                    Launcher_Value.Launcher_Select_Server_JSON = null;
                    Picture_Icon_Server.BackgroundImage = Image_Icon.Server_Offline;
                    Label_Status_Game_Server.Text = "Server Status:\n - Offline ( OFF )";
                    Label_Status_Game_Server.ForeColor = Color_Text.S_Error;
                    Label_Status_Game_Server_Data.Text = (e2.Error != null) ?
                    Strings.Encode(e2.Error.Message ?? "Server Seems to be Offline") : "Failed to Connect to Server";

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
                        LogToFileAddons.OpenLog("JSON GSI", String.Empty, e2.Error, String.Empty, true);
                    }

                    if (Client != null)
                    {
                        Client.Dispose();
                    }
                }
                else
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
                                        ServerBannerResult = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Banner, UriKind.Absolute, out Uri uriResult) &&
                                        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                    }
                                    catch { ServerBannerResult = false; }

                                    ImageUrl = ServerBannerResult ? Launcher_Value.Launcher_Select_Server_JSON.Server_Banner : string.Empty;
                                }
                                else
                                {
                                    ImageUrl = string.Empty;
                                }
                            }
                            catch { }

                            /* Social Panel Core */

                            /* Discord Invite Display */
                            try
                            {
                                bool ServerDiscordLink;
                                try
                                {
                                    ServerDiscordLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord, UriKind.Absolute, out Uri uriResult) &&
                                                             (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch { ServerDiscordLink = false; }
                                Picture_Icon_Server_Discord.BackgroundImage = ServerDiscordLink ? Image_Icon.Discord : Image_Icon.Discord_Disabled;
                                LinkLabel_Server_Discord.Enabled = ServerDiscordLink;
                                LinkLabel_Server_Discord.Text = ServerDiscordLink ? "Discord Invite" : string.Empty;
                            }
                            catch { }

                            /* Homepage Display */
                            try
                            {
                                bool ServerWebsiteLink;
                                try
                                {
                                    ServerWebsiteLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home, UriKind.Absolute, out Uri uriResult) &&
                                              (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch { ServerWebsiteLink = false; }
                                Picture_Icon_Server_Home.BackgroundImage = ServerWebsiteLink ? Image_Icon.Home : Image_Icon.Home_Disabled;
                                LinkLabel_Server_Home.Enabled = ServerWebsiteLink;
                                LinkLabel_Server_Home.Text = ServerWebsiteLink ? "Home Page" : string.Empty;
                            }
                            catch { }

                            /* Facebook Group Display */
                            try
                            {
                                bool ServerFacebookLink;
                                try
                                {
                                    ServerFacebookLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Facebook, UriKind.Absolute, out Uri uriResult) &&
                                                         (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                }
                                catch { ServerFacebookLink = false; }
                                Picture_Icon_Server_Facebook.BackgroundImage = ServerFacebookLink ? Image_Icon.Facebook : Image_Icon.Facebook_Disabled;
                                LinkLabel_Server_Facebook.Enabled = ServerFacebookLink;
                                LinkLabel_Server_Facebook.Text = ServerFacebookLink ? "Facebook Page" : string.Empty;
                            }
                            catch { }

                            /* Twitter Account Display */
                            try
                            {
                                bool ServerTwitterLink = Uri.TryCreate(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Twitter, UriKind.Absolute, out Uri uriResult) &&
                                                         (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                Picture_Icon_Server_Twitter.BackgroundImage = ServerTwitterLink ? Image_Icon.Twitter : Image_Icon.Twitter_Disabled;
                                LinkLabel_Server_Twitter.Enabled = ServerTwitterLink;
                                LinkLabel_Server_Twitter.Text = ServerTwitterLink ? "Twitter Feed" : string.Empty;
                            }
                            catch { }

                            /* Server Set Speedbug Timer Display */
                            try
                            {
                                ServerSecondsToShutDown =
                                (Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer != 0) ? Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer : 7200;
                                Label_Server_Force_Restart_Timer.Text = string.Format(Translations.Database("MainScreen_Text_ServerShutDown") +
                                    " " + Time_Conversion.RelativeTime(ServerSecondsToShutDown));
                            }
                            catch { }

                            try
                            {
                                /* Scenery Group Display */
                                string SceneryStatus;
                                switch (string.Join("", Launcher_Value.Launcher_Select_Server_JSON.Server_Active_Scenery))
                                {
                                    case "SCENERY_GROUP_NEWYEARS":
                                        SceneryStatus = "Scenery: New Years";
                                        break;
                                    case "SCENERY_GROUP_VALENTINES":
                                        SceneryStatus = "Scenery: Valentines";
                                        break;
                                    case "SCENERY_GROUP_OKTOBERFEST":
                                        SceneryStatus = "Scenery: Oktoberfest";
                                        break;
                                    case "SCENERY_GROUP_HALLOWEEN":
                                        SceneryStatus = "Scenery: Halloween";
                                        break;
                                    case "SCENERY_GROUP_CHRISTMAS":
                                        SceneryStatus = "Scenery: Christmas";
                                        break;
                                    default:
                                        SceneryStatus = "Scenery: Normal";
                                        break;
                                }
                                Label_Server_Scenery.Text = SceneryStatus;
                            }
                            catch { }

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
                            Label_Status_Game_Server_Data.Text = "Recevied Invalid JSON Game Server Info.";
                            Picture_Icon_Server.BackgroundImage = Image_Icon.Server_Warning;
                        }
                        catch { /* Sad Noises */ }
                    }
                    else
                    {
                        try
                        {
                            Label_Status_Game_Server.Text = "Server Status:\n - Online ( ON )";
                            Label_Status_Game_Server.ForeColor = Color_Text.S_Sucess;
                            Picture_Icon_Server.BackgroundImage = Image_Icon.Server_Online;
                            /* Enable Login & Register Button */
                            LoginEnabled = true;
                            Button_Login.ForeColor = Color_Text.L_Five;
                            Button_Login.Enabled = true;
                            Button_Register.Enabled = true;
                            Launcher_Value.Launcher_Select_Server_Category = ((Json_List_Server)ComboBox_Server_List.SelectedItem).Category ?? string.Empty;
                            Session_Timer.Remaining = (Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer != 0) ? Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer : 2 * 60 * 60;

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
                        catch { /* ¯\_(ツ)_/¯ */ }
                        finally
                        {
                            if (Client != null)
                            {
                                Client.Dispose();
                            }
                        }

                        /* For Thread Safety */
                        try
                        {
                            Label_Status_Game_Server_Data.Text = string.Format("Online: {0}\nRegistered: {1}", numPlayers, numRegistered);
                        }
                        catch { }

                        try
                        {
                            Label_Client_Ping.Text = string.Empty;
                            CheckMate = new Ping();
                            CheckMate.PingCompleted += (_sender, _e) =>
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
                                        Label_Client_Ping.Text = string.Format("Your Ping to the Server \n{0}".ToUpper(), _e.Reply.RoundtripTime + "ms");
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
                            };

                            CheckMate.SendAsync(ServerURI.Host, 5000, new byte[1], new PingOptions(30, true), new AutoResetEvent(false));
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
                            if (CheckMate != null)
                            {
                                CheckMate.Dispose();
                            }
                        }

                        ServerEnabled = true;

                        try
                        {
                            if (!Directory.Exists(Banner_Cache_Folder)) { Directory.CreateDirectory(Banner_Cache_Folder); }

                            if (!string.IsNullOrWhiteSpace(ImageUrl))
                            {

                                Uri URICall_A = new Uri(ImageUrl);
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                                ServicePointManager.FindServicePoint(URICall_A).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                var Client_A = new WebClient
                                {
                                    Encoding = Encoding.UTF8,
                                    CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                                };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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
                                    if (ServerChangeTriggered)
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
                                        if (Client_A != null)
                                        {
                                            Client_A.Dispose();
                                        }
                                    }
                                    else if (Events_A.Error != null)
                                    {
                                        if (!ServerChangeTriggered)
                                        {
                                            /* Load cached banner! */
#if NETFRAMEWORK
                                            Picture_Server_Banner.Image = Bitmap_Handler.Grayscale(Banner_Cache_File) ??Image_Other.Server_Banner;
#elif NET6_0_OR_GREATER && WINDOWS
                                            Picture_Server_Banner.Image = Image_Handler.Grayscale(Banner_Cache_File) ?? Image_Other.Server_Banner;
#endif
                                            GC.Collect();
                                        }

                                        if (Client_A != null)
                                        {
                                            Client_A.Dispose();
                                        }
                                    }
                                    else if (!ServerChangeTriggered && Events_A.Result != null)
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

                                            Picture_Server_Banner.Image = Image.FromStream(ServerRawBanner) ?? Image_Other.Server_Banner; ;

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
                                            if (Client_A != null)
                                            {
                                                Client_A.Dispose();
                                            }

                                            GC.Collect();
                                        }
                                    }
                                };
                            }
                            else if (File.Exists(Banner_Cache_File) && !Application.OpenForms[this.Name].IsDisposed)
                            {
                                /* Load cached banner! */
#if NETFRAMEWORK
                                Picture_Server_Banner.Image = Bitmap_Handler.Grayscale(Banner_Cache_File) ??Image_Other.Server_Banner;
#elif NET6_0_OR_GREATER && WINDOWS
                                Picture_Server_Banner.Image = Image_Handler.Grayscale(Banner_Cache_File) ?? Image_Other.Server_Banner;
#endif
                                GC.Collect();
                            }
                            else if (!Application.OpenForms[this.Name].IsDisposed)
                            {
                                Picture_Server_Banner.BackColor = Color_Winform_Other.Server_Banner_BackColor;
                                GC.Collect();
                            }
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("BANNER Cache", string.Empty, Error, string.Empty, true);
                        }

                        ServerChangeTriggered = false;
                    }
                }

                if (Application.OpenForms[this.Name] != null)
                {
                    GC.Collect();
                }
            };

            GC.Collect();
        }
        #endregion

        #region Game Files Downloader Components (LZMA [.dat])
        private void LaunchNfsw()
        {
            Button_Play_OR_Update.SafeInvokeAction(() =>
            {
                Button_Play_OR_Update.BackgroundImage = Image_Button.Play;
                Button_Play_OR_Update.ForeColor = Color_Text.L_Three;
            }, this);

            Label_Download_Information.SafeInvokeAction(() =>
            Label_Download_Information.Text = "Checking up all files".ToUpper(), this);

            ProgressBar_Preload.SafeInvokeAction(() =>
            ProgressBar_Preload.Width = 0, this);

            ProgressBar_Extracting.SafeInvokeAction(() =>
            ProgressBar_Extracting.Width = 0, this);

            try
            {
                Label_Download_Information.SafeInvokeAction(() =>
                Label_Download_Information.Text = "Loading list of files to download...".ToUpper(), this);

                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    if (d.Name == Path.GetPathRoot(Save_Settings.Live_Data.Game_Path))
                    {
                        if (d.TotalFreeSpace < 8589934592 || !string.Equals(d.DriveFormat, "NTFS", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ProgressBar_Extracting.SafeInvokeAction(() =>
                            {
                                ProgressBar_Extracting.Value = 100;
                                ProgressBar_Extracting.Width = 519;
                                ProgressBar_Extracting.Image = new Bitmap(Image_ProgressBar.Warning);
                                ProgressBar_Extracting.ProgressColor = Color_Winform_Other.Progress_Color_Extracting;
                            }, this);

                            if (!string.Equals(d.DriveFormat, "NTFS", StringComparison.InvariantCultureIgnoreCase))
                            {
                                Label_Download_Information_Support.SafeInvokeAction(() =>
                                Label_Download_Information_Support.Text = ("Playing the game on a non-NTFS-formatted drive is not supported.").ToUpper(), this);
                                Label_Download_Information.SafeInvokeAction(() =>
                                Label_Download_Information.Text = ("Drive '" + d.Name + "' is formatted with: " + d.DriveFormat + " Type.").ToUpper(), this);
                            }
                            else
                            {
                                Label_Download_Information.SafeInvokeAction(() =>
                                Label_Download_Information.Text = "Make sure you have at least 8GB of free space on hard drive.".ToUpper(), this);
                            }

                            FunctionStatus.IsVerifyHashDisabled = true;

                            if (Screen_Instance != null)
                            {
                                Taskbar_Progress.SetState(Screen_Instance.Handle, Taskbar_Progress.TaskbarStates.Paused);
                                Taskbar_Progress.SetValue(Screen_Instance.Handle, 100, 100);
                            }
                        }
                        else
                        {
                            DownloadCoreFiles();
                        }

                        break;
                    }
                }
            }
            catch
            {
                DownloadCoreFiles();
            }
        }

        public void RemoveTracksHighFiles()
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

        public void DownloadCoreFiles()
        {
            Label_Download_Information.SafeInvokeAction(() =>
            Label_Download_Information.Text = "Checking Core Files...".ToUpper(), this);

            ProgressBar_Preload.SafeInvokeAction(() =>
            ProgressBar_Preload.Width = 0, this);

            ProgressBar_Extracting.SafeInvokeAction(() =>
            ProgressBar_Extracting.Width = 0, this);

            if (Screen_Instance != null)
            {
                Taskbar_Progress.SetState(Screen_Instance.Handle, Taskbar_Progress.TaskbarStates.Indeterminate);
            }

            string GameExePath = Path.Combine(Save_Settings.Live_Data.Game_Path, "nfsw.exe");

            if (!File.Exists(GameExePath))
            {
                if (Save_Settings.Live_Data.Launcher_CDN.StartsWith("http://localhost") || Save_Settings.Live_Data.Launcher_CDN.StartsWith("https://localhost"))
                {
                    ProgressBar_Extracting.SafeInvokeAction(() =>
                    {
                        ProgressBar_Extracting.Value = 100;
                        ProgressBar_Extracting.Width = 519;
                        ProgressBar_Extracting.Image = new Bitmap(Image_ProgressBar.Warning);
                        ProgressBar_Extracting.ProgressColor = Color_Winform_Other.Progress_Color_Extracting;
                    }, this);

                    Label_Download_Information_Support.SafeInvokeAction(() => Label_Download_Information_Support.Text = "Failsafe CDN Detected".ToUpper(), this);
                    Label_Download_Information.SafeInvokeAction(() => Label_Download_Information.Text = "Please Choose a CDN from Settings Screen".ToUpper(), this);

                    if (Screen_Instance != null)
                    {
                        Taskbar_Progress.SetState(Screen_Instance.Handle, Taskbar_Progress.TaskbarStates.Paused);
                        Taskbar_Progress.SetValue(Screen_Instance.Handle, 100, 100);
                    }
                }
                /* Use Local Packed Archive for Install Source - DavidCarbon */
                else if (!InformationCache.EnableLZMADownloader)
                {
                    /* GameFiles.sbrwpack */
                    Game_Downloader();
                }
                else if (LZMA_Downloader != null)
                {
                    DownloadStartTime = DateTime.Now;
                    Label_Download_Information_Support.SafeInvokeAction(() => Label_Download_Information_Support.Text = "Downloading: Core GameFiles".ToUpper(), this);
                    Log.Info("DOWNLOAD: Getting Core Game Files");
                    Download_Settings.System_Unix = UnixOS.Detected();
                    Download_Settings.Alternative_WebCalls = Launcher_Value.Launcher_Alternative_Webcalls();
                    LZMA_Downloader.StartDownload(Save_Settings.Live_Data.Launcher_CDN, string.Empty, Save_Settings.Live_Data.Game_Path, false, false, 1130632198);
                }
                else
                {
                    OnDownloadFinished();
                }
            }
            else if (!InformationCache.EnableLZMADownloader)
            {
                OnDownloadFinished();
            }
            else
            {
                DownloadTracksFiles();
            }
        }

        public void DownloadTracksFiles()
        {
            Label_Download_Information.SafeInvokeAction(() =>
            Label_Download_Information.Text = "Checking Tracks Files...".ToUpper(), this);

            ProgressBar_Preload.SafeInvokeAction(() =>
            ProgressBar_Preload.Width = 0, this);

            ProgressBar_Extracting.SafeInvokeAction(() =>
            ProgressBar_Extracting.Width = 0, this);

            if (Screen_Instance != null)
            {
                Taskbar_Progress.SetState(Screen_Instance.Handle, Taskbar_Progress.TaskbarStates.Indeterminate);
            }

            string SpecificTracksFilePath = Path.Combine(Save_Settings.Live_Data.Game_Path, "Tracks", "STREAML5RA_98.BUN");
            if (!File.Exists(SpecificTracksFilePath) && (LZMA_Downloader != null))
            {
                DownloadStartTime = DateTime.Now;
                Label_Download_Information_Support.SafeInvokeAction(() =>
                Label_Download_Information_Support.Text = "Downloading: Tracks Data".ToUpper(), this);
                Log.Info("DOWNLOAD: Getting Tracks Folder");
                Download_Settings.Alternative_WebCalls = Launcher_Value.Launcher_Alternative_Webcalls();
                LZMA_Downloader.StartDownload(Save_Settings.Live_Data.Launcher_CDN, "Tracks", Save_Settings.Live_Data.Game_Path, false, false, 615494528);
            }
            else
            {
                DownloadSpeechFiles();
            }
        }

        public void DownloadSpeechFiles()
        {
            string speechFile = string.Empty;
            int speechSize = 0;

            if (InformationCache.EnableLZMADownloader)
            {
                Label_Download_Information.SafeInvokeAction(() => Label_Download_Information.Text = "Looking for correct Speech Files...".ToUpper(), this);

                ProgressBar_Preload.SafeInvokeAction(() => ProgressBar_Preload.Width = 0, this);

                ProgressBar_Extracting.SafeInvokeAction(() =>
                ProgressBar_Extracting.Width = 0, this);

                if (Screen_Instance != null)
                {
                    Taskbar_Progress.SetState(Screen_Instance.Handle, Taskbar_Progress.TaskbarStates.Indeterminate);
                }

                try
                {
                    speechFile = Download_LZMA_Support.SpeechFiles(Save_Settings.Live_Data.Launcher_Language);

                    Uri URLCall = new Uri(Save_Settings.Live_Data.Launcher_CDN + "/" + speechFile + "/index.xml");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var Client = new WebClient
                    {
                        Encoding = Encoding.UTF8,
                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                    };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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

                        XmlNode speechSizeNode = speechFileXml.SelectSingleNode("index/header/compressed");
                        speechSize = Convert.ToInt32(speechSizeNode.InnerText);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Download Speech Files", string.Empty, Error, string.Empty, true);
                    speechFile = Download_LZMA_Support.SpeechFiles();
                    speechSize = Download_LZMA_Support.SpeechFilesSize();
                }

                Label_Download_Information.SafeInvokeAction(() =>
                Label_Download_Information.Text = string.Format("Checking for {0} Speech Files.", speechFile).ToUpper(), this);
            }

            string SoundSpeechPath = Path.Combine(Save_Settings.Live_Data.Game_Path, "Sound", "Speech", "copspeechsth_" + speechFile + ".big");
            if (!File.Exists(SoundSpeechPath) && InformationCache.EnableLZMADownloader && LZMA_Downloader != null)
            {
                DownloadStartTime = DateTime.Now;
                Label_Download_Information_Support.SafeInvokeAction(() =>
                Label_Download_Information_Support.Text = "Downloading: Language Audio".ToUpper(), this);
                Log.Info("DOWNLOAD: Getting Speech/Audio Files");
                Download_Settings.Alternative_WebCalls = Launcher_Value.Launcher_Alternative_Webcalls();
                LZMA_Downloader.StartDownload(Save_Settings.Live_Data.Launcher_CDN, speechFile, Save_Settings.Live_Data.Game_Path, false, false, speechSize);
            }
            else
            {
                OnDownloadFinished();
                Label_Download_Information_Support.SafeInvokeAction(() =>
                Label_Download_Information_Support.Text = string.Empty, this);
                Log.Info("DOWNLOAD: Game Files Download is Complete!");
            }
        }

        private void OnDownloadProgress(long downloadLength, long downloadCurrent, long compressedLength, string filename, int skiptime = 0)
        {
            try
            {
                if (downloadCurrent < compressedLength)
                {
                    Label_Download_Information.SafeInvokeAction(() =>
                    Label_Download_Information.Text = string.Format("{0} of {1} ({3}%) — {2}", Time_Conversion.FormatFileSize(downloadCurrent),
                    Time_Conversion.FormatFileSize(compressedLength), Time_Conversion.EstimateFinishTime(downloadCurrent, compressedLength,
                    DownloadStartTime), (int)(100 * downloadCurrent / compressedLength)).ToUpper(), this);
                }
            }
            catch { }

            try
            {
                ProgressBar_Preload.SafeInvokeAction(() =>
                {
                    ProgressBar_Preload.Value = (int)(100 * downloadCurrent / compressedLength);
                    ProgressBar_Preload.Width = (int)(519 * downloadCurrent / compressedLength);
                }, this);

                Presence_Launcher.Status("Download Game Files", string.Format("Downloaded {0}% of the Game!", (int)(100 * downloadCurrent / compressedLength)));

                if (Screen_Instance != null)
                {
                    Taskbar_Progress.SetValue(Screen_Instance.Handle, (int)(100 * downloadCurrent / compressedLength), 100);
                }
            }
            catch
            {
                if (Screen_Instance != null)
                {
                    Taskbar_Progress.SetValue(Screen_Instance.Handle, 0, 100);
                }

                ProgressBar_Preload.SafeInvokeAction(() =>
                {
                    ProgressBar_Preload.Value = 0;
                    ProgressBar_Preload.Width = 0;
                }, this);
            }

            if (Screen_Instance != null)
            {
                Taskbar_Progress.SetState(Screen_Instance.Handle, Taskbar_Progress.TaskbarStates.Normal);
            }
        }

        private void OnDownloadFinished()
        {
            if (LZMA_Downloader != null)
            {
                LZMA_Downloader.Stop();
            }

            try
            {
                File.WriteAllBytes(Path.Combine(Save_Settings.Live_Data.Game_Path, "GFX", "BootFlow.gfx"),
                    ExtractResource.AsByte("GameLauncher.Resources.Bootscreen.BootFlow.gfx"));
            }
            catch { }

            if (Save_Settings.Live_Data.Game_Integrity == "Unknown")
            {
                Save_Settings.Live_Data.Game_Integrity = "Good";
                Save_Settings.Save();
            }

            Presence_Launcher.Download = false;
            Presence_Launcher.Status("Idle Ready", null);

            Label_Download_Information.SafeInvokeAction(() =>
            Label_Download_Information.Text = "Ready!".ToUpper(), this);

            EnablePlayButton();

            if (Screen_Instance != null)
            {
                Taskbar_Progress.SetValue(Screen_Instance.Handle, 100, 100);
                Taskbar_Progress.SetState(Screen_Instance.Handle, Taskbar_Progress.TaskbarStates.Normal);
            }
        }

        private void EnablePlayButton()
        {
            IsDownloading = false;
            Playenabled = true;

            ProgressBar_Extracting.SafeInvokeAction(() =>
            {
                ProgressBar_Extracting.Value = 100;
                ProgressBar_Extracting.Width = 519;
            }, this);
        }

        private void OnDownloadFailed(Exception Error)
        {
            if (LZMA_Downloader != null)
            {
                LZMA_Downloader.Stop();
            }

            Presence_Launcher.Status("Download Game Files Error", null);

            ProgressBar_Extracting.Value = 100;
            ProgressBar_Extracting.Width = 519;
            ProgressBar_Extracting.Image = new Bitmap(Image_ProgressBar.Error);
            ProgressBar_Extracting.ProgressColor = Color_Text.S_Error;

            Label_Download_Information.Text = ((Error != null) ? Error.Message : "Download Failed. No Reason Provided").ToUpper();

            FunctionStatus.IsVerifyHashDisabled = true;

            if (Screen_Instance != null)
            {
                Taskbar_Progress.SetValue(Screen_Instance.Handle, 100, 100);
                Taskbar_Progress.SetState(Screen_Instance.Handle, Taskbar_Progress.TaskbarStates.Error);
            }
            
            if (Error != null)
            {
                string TempEmailCache = string.Empty;
                if (!string.IsNullOrWhiteSpace(Input_Email.Text))
                {
                    TempEmailCache = Input_Email.Text;
                    Input_Email.Text = "EMAIL IS HIDDEN";
                }

                string LogMessage = "CDN Downloader Encountered an Error:";
                LogToFileAddons.OpenLog("Game Download", LogMessage, Error, "Error", false);

                if (!string.IsNullOrWhiteSpace(TempEmailCache))
                {
                    Input_Email.Text = TempEmailCache;
                }
            }
        }

        private void OnShowExtract(string filename, long currentCount, long allFilesCount)
        {
            try
            {
                if (ProgressBar_Preload.Value == 100)
                {
                    Label_Download_Information.SafeInvokeAction(() =>
                    Label_Download_Information.Text = string.Format("{0} of {1} : ({3}%) — {2}", Time_Conversion.FormatFileSize(currentCount),
                    Time_Conversion.FormatFileSize(allFilesCount),
                    Time_Conversion.EstimateFinishTime(currentCount, allFilesCount, DownloadStartTime), (int)(100 * currentCount / allFilesCount)).ToUpper(), this);
                }
            }
            catch { }

            try
            {
                ProgressBar_Extracting.SafeInvokeAction(() =>
                {
                    ProgressBar_Extracting.Value = (int)(100 * currentCount / allFilesCount);
                    ProgressBar_Extracting.Width = (int)(519 * currentCount / allFilesCount);
                }, this);
            }
            catch { }
        }

        private void OnShowMessage(string message, string header)
        {
            string TempEmailCache = string.Empty;
            if (!string.IsNullOrWhiteSpace(Input_Email.Text))
            {
                TempEmailCache = Input_Email.Text;
                Input_Email.SafeInvokeAction(() =>
                Input_Email.Text = "EMAIL IS HIDDEN", this);
            }
            MessageBox.Show(message, header);
            if (!string.IsNullOrWhiteSpace(TempEmailCache))
            {
                Input_Email.SafeInvokeAction(() =>
                Input_Email.Text = TempEmailCache, this);
            }
        }
        #endregion

        #region Game Files Downloader (SBRW Pack [.pack.sbrw])

        /* That's right the Protype Extractor from 2.1.5.x, now back from the dead - DavidCarbon */
        private void Game_Downloader()
        {
            Label_Download_Information_Support.SafeInvokeAction(() =>
            {
                Label_Download_Information_Support.Text = "Downloading: Core Game Files Package".ToUpper();
            }, this);

            Label_Download_Information.SafeInvokeAction(() =>
            Label_Download_Information.Text = "Loading".ToUpper(), this);

            Pack_SBRW_Downloader = new Download_Queue();
            Pack_SBRW_Downloader.Internal_Error += (x, D_Live_Events) =>
            {
                if (D_Live_Events.Recorded_Exception != null && !Pack_SBRW_Downloader.Cancel)
                {
                    LogToFileAddons.OpenLog("Pack_SBRW_Downloader", string.Empty, D_Live_Events.Recorded_Exception, string.Empty, true);
                    OnDownloadFailed(D_Live_Events.Recorded_Exception);
                }
            };
            Pack_SBRW_Downloader.Live_Progress += (x, D_Live_Events) =>
            {
                if (!Pack_SBRW_Downloader.Cancel)
                {
                    ProgressBar_Extracting.SafeInvokeAction(() =>
                    {
                        ProgressBar_Extracting.Value = (int)(100 * D_Live_Events.File_Size_Current / D_Live_Events.File_Size_Total);
                        ProgressBar_Extracting.Width = (int)(519 * D_Live_Events.File_Size_Current / D_Live_Events.File_Size_Total);
                    }, this);

                    TimeSpan Time_Clock = DateTime.Now - D_Live_Events.Start_Time;

                    if (Pack_SBRW_Downloader_Time_Span != Time_Clock.Seconds)
                    {
                        Presence_Launcher.Status("Download Game Files", string.Format("Downloaded {0}% of the Game!", D_Live_Events.Download_Percentage));
                        Pack_SBRW_Downloader_Time_Span = Time_Clock.Seconds;
                    }

                    if (Screen_Instance != null)
                    {
                        Taskbar_Progress.SetValue(Screen_Instance.Handle, (int)(100 * D_Live_Events.File_Size_Current / D_Live_Events.File_Size_Total), 100);
                    }

                    Label_Download_Information.SafeInvokeAction(() =>
                    {
                        Label_Download_Information.Text = (Time_Conversion.FormatFileSize(D_Live_Events.File_Size_Current) + " of " + Time_Conversion.FormatFileSize(D_Live_Events.File_Size_Total) + " - " +
                        Time_Conversion.EstimateFinishTime(D_Live_Events.File_Size_Current, D_Live_Events.File_Size_Total, D_Live_Events.Start_Time)).ToUpper();
                    }, this);
                }
            };
            Pack_SBRW_Downloader.Complete += (x, D_Live_Events) =>
            {
                if (D_Live_Events.Complete)
                {
                    if (Screen_Instance != null)
                    {
                        Taskbar_Progress.SetValue(Screen_Instance.Handle, 100, 100);
                    }

                    ProgressBar_Preload.SafeInvokeAction(() =>
                    {
                        ProgressBar_Preload.Value = 100;
                        ProgressBar_Preload.Width = 519;
                    }, this);

                    Label_Download_Information_Support.SafeInvokeAction(() =>
                    {
                        Label_Download_Information_Support.Text = "Downloaded: Core Game Files Package".ToUpper();
                    }, this);

                    Label_Download_Information.SafeInvokeAction(() =>
                    Label_Download_Information.Text = "Checking Package Integrity".ToUpper(), this);

                    /* Check Local GameFiles Hash */
                    string Last_Known_Location = D_Live_Events.Download_Location ?? Path.Combine(Save_Settings.Live_Data.Game_Path, ".Launcher", "GameFiles.sbrwpack");
                    if (Hashes.Hash_SHA(Last_Known_Location) == "88C886B6D131C052365C3D6D14E14F67A4E2C253")
                    {
                        Pack_SBRW_Unpacker = new Download_Extract();
                        Pack_SBRW_Unpacker.Internal_Error += (x, U_Live_Events) =>
                        {
                            if (U_Live_Events.Recorded_Exception != null)
                            {
                                LogToFileAddons.OpenLog("Pack_SBRW_Unpacker", string.Empty, U_Live_Events.Recorded_Exception, string.Empty, true);
                                OnDownloadFailed(U_Live_Events.Recorded_Exception);
                            }
                        };
                        Pack_SBRW_Unpacker.Live_Progress += (x, U_Live_Events) =>
                        {
                            if (U_Live_Events != null)
                            {
                                ProgressBar_Extracting.SafeInvokeAction(() =>
                                {
                                    ProgressBar_Extracting.Value = U_Live_Events.Extract_Percentage;
                                    ProgressBar_Extracting.Width = (int)(519 * U_Live_Events.File_Current / U_Live_Events.File_Total);
                                }, this);

                                Presence_Launcher.Status("Unpack Game Files", string.Format("Unpacking Game: {0}%", U_Live_Events.Extract_Percentage));

                                if (Screen_Instance != null)
                                {
                                    Taskbar_Progress.SetValue(Screen_Instance.Handle, U_Live_Events.Extract_Percentage, 100);
                                }

                                Label_Download_Information_Support.SafeInvokeAction(() =>
                                Label_Download_Information_Support.Text = "[" + U_Live_Events.File_Current + " / " + U_Live_Events.File_Total + "]", this);

                                if (!string.IsNullOrWhiteSpace(U_Live_Events.File_Current_Name))
                                {
                                    Label_Download_Information.SafeInvokeAction(() =>
                                    Label_Download_Information.Text = ("Unpacking " + U_Live_Events.File_Current_Name.Replace(Pack_SBRW_Unpacker.File_Extension_Replacement, string.Empty)).ToUpper(), this);
                                }
                            }
                        };
                        Pack_SBRW_Unpacker.Complete += (x, U_Live_Events) =>
                        {
                            if (U_Live_Events != null)
                            {
                                Label_Download_Information_Support.SafeInvokeAction(() =>
                                {
                                    Label_Download_Information_Support.Visible = false;
                                    Label_Download_Information_Support.Text = string.Empty;
                                }, this);

                                IsDownloading = false;
                                OnDownloadFinished();

                                NotifyIcon_Notification.Visible = true;
                                NotifyIcon_Notification.BalloonTipIcon = ToolTipIcon.Info;
                                NotifyIcon_Notification.BalloonTipTitle = "SBRW Launcher";
                                NotifyIcon_Notification.BalloonTipText = "Your game is now ready to launch!";
                                NotifyIcon_Notification.ShowBalloonTip(5000);
                            }
                        };
                        Pack_SBRW_Unpacker.Custom_Unpack(D_Live_Events.Download_Location ?? Path.Combine(Save_Settings.Live_Data.Game_Path, ".Launcher", "GameFiles.sbrwpack"), Save_Settings.Live_Data.Game_Path);
                    }
                    else
                    {
                        OnDownloadFailed(new Exception("Game Files Package hash does not Match"));
                    }
                }
            };
            /* Main Note: Current Revision File Size (in long) is: 3862102244 */
            Pack_SBRW_Downloader.Download(Save_Settings.Live_Data.Launcher_CDN, Save_Settings.Live_Data.Game_Path, 3862102244);
        }
        #endregion

        #region Background Workers
        private void BackgroundWorker_One_DoWork(object sender, DoWorkEventArgs e)
        {
            LaunchNfsw();
        }
        #endregion

        #region Loads, Set, and Start Certain Functions (CORE)
        private void MainScreen_Load(object sender, EventArgs e)
        {
            Log.Visuals("CORE: Loading Main Screen");
            FunctionStatus.CenterScreen(this);
            Application.OpenForms[this.Name].Activate();
            Log.Core("CORE: Setting Parent Window location");
            InformationCache.ParentScreenLocation = Location;

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
            if (!string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Raw_Password))
            {
                LoginEnabled = true;
                ServerEnabled = true;
                Button_Login.BackgroundImage = Image_Button.Grey;
                Button_Login.ForeColor = Color_Text.L_Five;
            }
            else
            {
                LoginEnabled = false;
                ServerEnabled = false;
                Button_Login.BackgroundImage = Image_Button.Grey;
                Button_Login.ForeColor = Color_Text.L_Six;
            }

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
                        MessageBox.Show(null, string.Format("Drive {0} was not found. Your actual installation directory is set to {1} now.",
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
                    Log.Core("CORE: 'GetServerInformation' from all Servers in Server List and Download Selected Server Banners");
                    BackgroundWorker_One = new BackgroundWorker
                    {
                        WorkerSupportsCancellation = true,
                        WorkerReportsProgress = true
                    };
                    BackgroundWorker_One.DoWork += new DoWorkEventHandler(BackgroundWorker_One_DoWork);
                    BackgroundWorker_One.RunWorkerAsync();
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("New Download Game Files", string.Empty, Error, string.Empty, true);
                }

                this.BringToFront();

                try
                {
                    new LauncherUpdateCheck(Picture_Icon_Version, Label_Status_Launcher, Label_Status_Launcher_Version).ChangeVisualStatus();
                }
                catch
                {
                    if (UnixOS.Detected())
                    {
                        Picture_Icon_Version.BackgroundImage = Image_Icon.Engine_Good;
                        Label_Status_Launcher.ForeColor = Color_Text.S_Sucess;
                        Label_Status_Launcher.Text = "Launcher Status:\n - Linux Build";
                        Label_Status_Launcher_Version.Text = "Version: v" + Application.ProductVersion;
                    }
                }

                PingServerListAPIStatus();

                Log.Visuals("CORE: Applyinng ContextMenu");
#if NETFRAMEWORK
                ContextMenu = new ContextMenu();
                ContextMenu.MenuItems.Add(new MenuItem("About", (O, K) => { Screen_About.OpenScreen(); }));
                if (LauncherUpdateCheck.UpgradeAvailable)
                {
                    ContextMenu.MenuItems.Add("-");
                    ContextMenu.MenuItems.Add(new MenuItem("Obsolete", (N, O) => { Process.Start("https://www.youtube.com/watch?v=LutDfASARmE"); }));
                }
                ContextMenu.MenuItems.Add("-");
                ContextMenu.MenuItems.Add(new MenuItem("Close Launcher", Button_Close_Click));

                NotifyIcon_Notification.ContextMenu = ContextMenu;
                ContextMenu = null;
#endif
                //NotifyIcon_Notification.Icon = new Icon(Icon, Icon.Width, Icon.Height);
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

            Text = "SBRW Launcher: v" + Application.ProductVersion;

            /*******************************/
            /* Set Font                     /
            /*******************************/

            float MainFontSize = UnixOS.Detected() ? 9f : 9f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = UnixOS.Detected() ? 8f : 8f * 96f / CreateGraphics().DpiY;
            float ThirdFontSize = UnixOS.Detected() ? 10f : 10f * 96f / CreateGraphics().DpiY;
            float FourthFontSize = UnixOS.Detected() ? 14f : 14f * 96f / CreateGraphics().DpiY;
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
            ProgressBar_Extracting.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
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
            ProgressBar_Preload.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Download_Information.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Download_Information_Support.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);

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

            Picture_Bar_Outline.BackgroundImage = Image_Converter.Value(Core.Theme.Properties.Resources.progress_outline);
            ProgressBar_Preload.Image = new Bitmap(Image_ProgressBar.Preload);
            ProgressBar_Extracting.Image = new Bitmap(Image_ProgressBar.Success);

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

            /********************************/
            /* Events                        /
            /********************************/

            LinkLabel_Server_Twitter.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.TwitterAccountLink_LinkClicked);
            LinkLabel_Server_Facebook.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.FacebookGroupLink_LinkClicked);
            LinkLabel_Server_Discord.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.DiscordInviteLink_LinkClicked);
            LinkLabel_Server_Home.LinkClicked += new LinkLabelLinkClickedEventHandler(FunctionEvents.HomePageLink_LinkClicked);

            Button_Select_Server.Click += new EventHandler(FunctionEvents.SelectServerBtn_Click);
            
            Button_Close.MouseEnter += new EventHandler(ButtonClose_MouseEnter);
            Button_Close.MouseLeave += new EventHandler(ButtonClose_MouseLeaveANDMouseUp);
            Button_Close.MouseUp += new MouseEventHandler(ButtonClose_MouseLeaveANDMouseUp);
            Button_Close.MouseDown += new MouseEventHandler(ButtonClose_MouseDown);
            Button_Close.Click += new EventHandler(Button_Close_Click);

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

            MouseMove += new MouseEventHandler(Move_Window_Mouse_Move);
            MouseUp += new MouseEventHandler(Move_Window_Mouse_Up);
            MouseDown += new MouseEventHandler(Move_Window_Mouse_Down);

            Picture_Logo.MouseMove += new MouseEventHandler(Move_Window_Mouse_Move);
            Picture_Logo.MouseUp += new MouseEventHandler(Move_Window_Mouse_Up);
            Picture_Logo.MouseDown += new MouseEventHandler(Move_Window_Mouse_Down);

            Button_Play_OR_Update.MouseEnter += new EventHandler(PlayButton_MouseEnter);
            Button_Play_OR_Update.MouseLeave += new EventHandler(PlayButton_MouseLeave);
            Button_Play_OR_Update.MouseUp += new MouseEventHandler(PlayButton_MouseUp);
            Button_Play_OR_Update.MouseDown += new MouseEventHandler(PlayButton_MouseDown);
            Button_Play_OR_Update.Click += new EventHandler(PlayButton_Click);

            Button_Register.MouseEnter += new EventHandler(Greenbutton_hover_MouseEnter);
            Button_Register.MouseLeave += new EventHandler(Greenbutton_MouseLeave);
            Button_Register.MouseUp += new MouseEventHandler(Greenbutton_hover_MouseUp);
            Button_Register.MouseDown += new MouseEventHandler(Greenbutton_click_MouseDown);
            Button_Register.Click += new EventHandler(FunctionEvents.RegisterText_LinkClicked);

            LZMA_Downloader = new Download_LZMA_Data(this, 3, 2, 16)
            {
                ProgressUpdated = new Download_LZMA_Delegates.Download_LZMA_Progress_Updated(OnDownloadProgress),
                DownloadFinished = new Download_LZMA_Delegates.Download_LZMA_Finished(DownloadTracksFiles),
                DownloadFailed = new Download_LZMA_Delegates.Download_LZMA_Failed(OnDownloadFailed),
                ShowMessage = new Download_LZMA_Delegates.Download_LZMA_Show_Message(OnShowMessage),
                ShowExtract = new Download_LZMA_Delegates.Download_LZMA_Show_Extract(OnShowExtract)
            };

            Load += new EventHandler(MainScreen_Load);
            
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

            Shown += (x, y) =>
            {
                new Thread(() =>
                {
                    Presence_Launcher.Update();

                    if (ServerListUpdater.NoCategoryList != null && ServerListUpdater.NoCategoryList.Any())
                    {
                        foreach (Json_List_Server Servers in ServerListUpdater.NoCategoryList)
                        {
                            if (Nfswstarted == null)
                            {
                                GC.Collect();

                                try
                                {
                                    while (StillCheckingLastServer) { }
                                    Uri URLCall = new Uri(Servers.IPAddress + "/GetServerInformation");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                    var Client = new WebClient
                                    {
                                        Encoding = Encoding.UTF8,
                                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                                    };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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
                                        JsonGSI = Client.DownloadString(URLCall);
                                        StillCheckingLastServer = true;
                                        bool GSIErrorFree = true;

                                        if (!Is_Json.Valid(JsonGSI))
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

                                        if (Client != null)
                                        {
                                            Client.Dispose();
                                        }
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
                }).Start();

                GC.Collect();
            };

            if (!UnixOS.Detected())
            {
                try
                {
                    string CursorFile = (!UnixOS.Detected()) ? Path.GetTempFileName() : Path.Combine(Locations.LauncherFolder, "Cursor.ani");
                    File.WriteAllBytes(CursorFile, ExtractResource.AsByte("Core.Theme.Resources.Cursors.Cursor.ani"));
                    Cursor mycursor = new Cursor(Cursor.Current.Handle);
                    IntPtr colorcursorhandle = DLL_Cursor.LoadCursorFromFile(CursorFile);
                    mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField,
                        null, mycursor, new object[] { colorcursorhandle });
                    Cursor = mycursor;
                    File.Delete(CursorFile);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("CURSOR", string.Empty, Error, string.Empty, true);
                }
            }
        }
#endregion

        public void Clear_Hide_Screen_Form_Panel()
        {
            if (Screen_Panel_Forms != null)
            {
                Screen_Panel_Forms.Controls.Clear();
                Screen_Panel_Forms.Visible = false;
            }
            
            Text = "SBRW Launcher: v" + Application.ProductVersion;
        }

        public Screen_Main()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Set_Visuals();
            this.Closing += (x, y) =>
            {
                ClosingTasks();
            };
            Screen_Instance = this;
            Screen_Panel_Forms = Panel_Form_Screens;
        }
    }
}
