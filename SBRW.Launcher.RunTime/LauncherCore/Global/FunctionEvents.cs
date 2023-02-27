using SBRW.Launcher.RunTime.InsiderKit;
using SBRW.Launcher.RunTime.LauncherCore.LauncherUpdater;
using SBRW.Launcher.RunTime.LauncherCore.Lists;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.Visuals;
using SBRW.Launcher.App.UI_Forms.Custom_Server_Screen;
using SBRW.Launcher.App.UI_Forms.Main_Screen;
using SBRW.Launcher.App.UI_Forms.Register_Screen;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Validation_;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using SBRW.Launcher.App.UI_Forms.Settings_Screen;
using SBRW.Launcher.Core.Required.System;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Proxy.Nancy_;

namespace SBRW.Launcher.RunTime.LauncherCore.Global
{
    /* This is Used to Visual Events (Draw or Click Events) */
    class FunctionEvents
    {
        /* ServerList Load Checks */
        public static void SelectServerBtn_Click(object sender, EventArgs e)
        {
            Screen_Custom_Server.OpenScreen(false);
        }

        public static void AddServer_Click(object sender, EventArgs e)
        {
            Screen_Custom_Server.OpenScreen(true);
        }

        public static void DiscordInviteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Launcher_Value.Launcher_Select_Server_JSON != null && !string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord))
            {
#if NETFRAMEWORK
                Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord);
#else
                Process.Start("explorer.exe", Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord);
#endif
            }
        }

        public static void HomePageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Launcher_Value.Launcher_Select_Server_JSON != null && !string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home))
            {
#if NETFRAMEWORK
                Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home);
#else
                Process.Start("explorer.exe", Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home);
#endif
            }
        }

        public static void FacebookGroupLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Launcher_Value.Launcher_Select_Server_JSON != null && !string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Facebook))
            {
#if NETFRAMEWORK
                Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Facebook);
#else
                Process.Start("explorer.exe", Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Facebook);
#endif
            }
        }

        public static void TwitterAccountLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Launcher_Value.Launcher_Select_Server_JSON != null && !string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Twitter))
            {
#if NETFRAMEWORK
                Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Twitter);
#else
                Process.Start("explorer.exe", Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Twitter);
#endif
            }  
        }

        public static void ForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Launcher_Value.Launcher_Select_Server_JSON != null)
            {
                if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Account_Recovery_Page))
                {
#if NETFRAMEWORK
                    Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Account_Recovery_Page);
#else
                    Process.Start("explorer.exe", Launcher_Value.Launcher_Select_Server_JSON.Server_Account_Recovery_Page);
#endif
                    MessageBox.Show(null, "A browser window has been opened to complete password recovery on " +
                        Launcher_Value.Launcher_Select_Server_JSON.Server_Name, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string send = Prompt.ShowDialog("Please specify your email address.", "GameLauncher");

                    if (!string.IsNullOrWhiteSpace(send))
                    {
                        if (!send.Valid_Email())
                        {
                            MessageBox.Show(null, "Email Address is not Valid. Please Check and Try Again", "GameLauncher",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            try
                            {
                                Uri resetPasswordUrl = new Uri(Launcher_Value.Launcher_Select_Server_Data.IPAddress + "/RecoveryPassword/forgotPassword");
                                ServicePointManager.FindServicePoint(resetPasswordUrl).ConnectionLeaseTimeout =
                                    (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

                                HttpWebRequest Request = (HttpWebRequest)System.Net.WebRequest.Create(resetPasswordUrl);
                                string postData = "email=" + send;
                                byte[] data = Encoding.ASCII.GetBytes(postData);
                                Request.Method = "POST";
                                Request.ContentType = "application/x-www-form-urlencoded";
                                Request.ContentLength = data.Length;
                                Request.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

                                using (var stream = Request.GetRequestStream())
                                {
                                    stream.Write(data, 0, data.Length);
                                }

                                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                                string ResponseBody = new StreamReader(Response.GetResponseStream()).ReadToEnd();

                                string DisplayMessage;
                                if (!string.IsNullOrWhiteSpace(ResponseBody))
                                {
                                    if (ResponseBody.Contains("ERROR"))
                                    {
                                        if (ResponseBody.ToUpper().Contains("INVALID EMAIL"))
                                        {
                                            DisplayMessage = "If an Account with the Email Exists, a Password Reset will be Sent to Your Inbox.";
                                        }
                                        else if (ResponseBody.ToUpper().Contains("RECOVERY PASSWORD LINK ALREADY SENT"))
                                        {
                                            DisplayMessage = "Recovery Password Reset Link has already been sent. " +
                                                "Please check your Spam Inbox or Try again in 1 Hour";
                                        }
                                        else
                                        {
                                            DisplayMessage = ResponseBody;
                                        }
                                    }
                                    else if (ResponseBody.ToUpper().Contains("RESET PASSWORD SENT TO"))
                                    {
                                        DisplayMessage = "A Password Reset Link will be Sent to Your Inbox.";
                                    }
                                    else
                                    {
                                        DisplayMessage = ResponseBody;
                                    }
                                }
                                else
                                {
                                    DisplayMessage = "The Server received the Forgot Password Request, but has not Accepted your Request.";
                                }

                                MessageBox.Show(null, DisplayMessage, "GameLauncher",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (WebException Error)
                            {
                                LogToFileAddons.OpenLog("REGISTRATION", "Unable to Send Email to Server.", Error, "Error", false);
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("REGISTRATION", "Unable to Send Email.", Error, "Error", false);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(null, "Email Address can not be empty. Please Check and Try Again", "GameLauncher",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        public static void ServerList_Menu_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                string serverListText = string.Empty;
                /* 0 = Offline | 1 = Online | 2 = Checking | 3 = GSI Error */
                int onlineStatus = 2;

                if (sender is ComboBox cb)
                {
                    if (e.Index != -1 && cb.Items != null)
                    {
                        if (cb.Items[e.Index] is Json_List_Server si)
                        {
                            serverListText = si.Name;
                            onlineStatus = InformationCache.ServerStatusBook.ContainsKey(si.ID) ? InformationCache.ServerStatusBook[si.ID] : 2;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(serverListText) && sender != null)
                {
                    Font font = ((ComboBox)sender).Font;
                    Brush backgroundColor;
                    Brush textColor;

                    if (serverListText.StartsWith("<GROUP>"))
                    {
                        font = new Font(font, FontStyle.Bold);
                        e.Graphics.FillRectangle(new SolidBrush(Color_Winform_Other.DropMenu_White), e.Bounds);
                        e.Graphics.DrawString(serverListText.Replace("<GROUP>", string.Empty), font, new SolidBrush(Color_Winform_Other.DropMenu_Black), e.Bounds);
                    }
                    else
                    {
                        font = new Font(font, FontStyle.Regular);
                        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && e.State != DrawItemState.ComboBoxEdit)
                        {
                            backgroundColor = SystemBrushes.Highlight;
                            textColor = SystemBrushes.HighlightText;
                        }
                        else
                        {
                            backgroundColor = onlineStatus switch
                            {
                                1 => new SolidBrush(Color_Winform_Other.DropMenu_Ping_Success),/* ONLINE */
                                2 => new SolidBrush(Color_Winform_Other.DropMenu_Ping_Checking),/* CHECKING */
                                3 => new SolidBrush(Color_Winform_Other.DropMenu_Ping_Warning),/* GSI ERROR */
                                _ => new SolidBrush(Color_Winform_Other.DropMenu_Ping_Error),/* OFFLINE */
                            };
                            textColor = new SolidBrush(Color_Winform_Other.DropMenu_Black);
                        }

                        e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                        e.Graphics.DrawString("    " + serverListText, font, textColor, e.Bounds);
                    }
                }
            }
            catch { }
        }

        public static void Console_Commands(string Live_Commands)
        {
            try
            {
                Live_Commands = Live_Commands.ToLowerInvariant();
                switch (Live_Commands)
                {
                    case "what if...":
                    case "what if":
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=OaWYwk7dysc");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=OaWYwk7dysc/", UseShellExecute = true });
#endif
                        break;
                    case "don't look!":
                    case "dont look":
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=nwqtdwcqrBE");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=nwqtdwcqrBE/", UseShellExecute = true });
#endif
                        break;
                    case "behind the scenes":
#if NETFRAMEWORK
                        Process.Start("https://cdn.discordapp.com/attachments/620401560954077214/987989783022272562/unknown.png");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://cdn.discordapp.com/attachments/620401560954077214/987989783022272562/unknown.png", UseShellExecute = true });
#endif
                        break;
                    case "ezekiel":
                    case "crash the server":
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=T-AF81iBCi0");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=T-AF81iBCi0/", UseShellExecute = true });
#endif
                        break;
                    case "ezekiel extended mix":
                    case "crash the server extended mix":
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=sReIQTvS1kM");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=sReIQTvS1kM/", UseShellExecute = true });
#endif
                        break;
                    case "obsolete":
                        if (LauncherUpdateCheck.UpgradeAvailable)
                        {
#if NETFRAMEWORK
                            Process.Start("https://youtu.be/LutDfASARmE");
#else
                            Process.Start(new ProcessStartInfo { FileName = "https://youtu.be/LutDfASARmE/", UseShellExecute = true });
#endif
                        }
                        break;
                    case "a song long ago":
#if NETFRAMEWORK
                        Process.Start("https://youtu.be/zo_C-dk6Xh4");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://youtu.be/zo_C-dk6Xh4/", UseShellExecute = true });
#endif
                        break;
                    case "keygen":
                    case "keygen 2013":
#if NETFRAMEWORK
                        Process.Start("https://youtu.be/vCMzIE9p07Y");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://youtu.be/vCMzIE9p07Y/", UseShellExecute = true });
#endif
                        break;
                    case "who am i?":
                    case "who am i":
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=6TmlR27izRo");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=6TmlR27izRo/", UseShellExecute = true });
#endif
                        break;
                    case "who am i? the greatest":
                    case "who am i the greatest":
                    case "the greatest":
#if NETFRAMEWORK
                        Process.Start("https://youtu.be/wV_5yHp8bqU");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://youtu.be/wV_5yHp8bqU", UseShellExecute = true });
#endif
                        break;
                    case "straightuphippo":
#if NETFRAMEWORK
                        Process.Start("https://youtu.be/Uc57tO6g--I");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://youtu.be/Uc57tO6g--I/", UseShellExecute = true });
#endif
                        break;
                    case "seeing what's next":
                    case "seeing whats next":
                    case "insider":
                    case "developer":
                    case "beta":
#if NETFRAMEWORK
                        Process.Start("https://youtu.be/F6dVLZIJatk");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://youtu.be/F6dVLZIJatk/", UseShellExecute = true });
#endif
                        break;
                    case "password":
                    case "my password":
                        if ((Screen_Settings.Screen_Instance != default) || (Screen_Main.Screen_Instance != default))
                        {
                            string Text_Display = string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Raw_Password) ? "No Password Found" :
                                "Your Password: " + Save_Account.Live_Data.User_Raw_Password;

                            if (Screen_Settings.Screen_Instance != default)
                            {
                                MessageBox.Show(Screen_Settings.Screen_Instance, Text_Display, "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (Screen_Main.Screen_Instance != default)
                            {
                                MessageBox.Show(Screen_Main.Screen_Instance, Text_Display, "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        break;
                    case "now loading":
                    case "now loading!!!":
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=kq3X78ngFAY");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=kq3X78ngFAY/", UseShellExecute = true });
#endif
                        break;
                    case "update":
                        if (LauncherUpdateCheck.UpgradeAvailable)
                        {
                            LauncherUpdateCheck.UpdateStatusResult(true);
                        }
                        else
                        {
                            LauncherUpdateCheck.Latest(false);
                        }
                        break;
#if DEBUG
                    case "storage":
                        new SBRW.Launcher.App.UI_Forms.Update_Popup_Screen.Screen_Update_Popup(false).ShowDialog();
                        break;
#endif
                    case "build":
                    case "build date":
                        if ((Screen_Settings.Screen_Instance != default) || (Screen_Main.Screen_Instance != default))
                        {
                            if (Screen_Settings.Screen_Instance != default)
                            {
                                MessageBox.Show(Screen_Settings.Screen_Instance, InsiderInfo.BuildNumber(), "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else if (Screen_Main.Screen_Instance != default)
                            {
                                MessageBox.Show(Screen_Main.Screen_Instance, InsiderInfo.BuildNumber(), "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        break;
                    case "opt build beta":
                    case "opt build dev":
                    case "opt build stable":
                        if (Live_Commands.Contains("beta"))
                        {
                            Screen_Settings.Insider_Settings_Lock = true;
                            Save_Settings.Live_Data.Launcher_Insider = "1";
                            EnableInsiderDeveloper.Allowed(false);
                            EnableInsiderBetaTester.Allowed(true);
                        }
                        else if (Live_Commands.Contains("dev"))
                        {
                            Screen_Settings.Insider_Settings_Lock = true;
                            Save_Settings.Live_Data.Launcher_Insider = "2";
                            EnableInsiderDeveloper.Allowed(true);
                            EnableInsiderBetaTester.Allowed(false);
                        }
                        else
                        {
                            Save_Settings.Live_Data.Launcher_Insider = "0";
                            EnableInsiderDeveloper.Allowed(false);
                            EnableInsiderBetaTester.Allowed(false);
                        }
                        break;
                    case "help":
                        if ((Screen_Settings.Screen_Instance != default) || (Screen_Main.Screen_Instance != default))
                        {
                            string LIST_OF_COMMANDS = "Available Commands" +
                                                        "\nMy Password - Displays your raw Password" +
                                                        "\nUpdate - Triggers the Update popup, if an Update is available" +
                                                        "\nBuild Date - Displays Compiled Date";

                            if (Screen_Settings.Screen_Instance != default)
                            {
                                MessageBox.Show(Screen_Settings.Screen_Instance, LIST_OF_COMMANDS, "SBRW Launcher Commands",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (Screen_Main.Screen_Instance != default)
                            {
                                MessageBox.Show(Screen_Main.Screen_Instance, LIST_OF_COMMANDS, "SBRW Launcher Commands",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        break;
                    case "vhs":
                    case "verify hash skip":
                        if (Save_Settings.Live_Data.Game_Integrity != "Good")
                        {
                            if ((Screen_Settings.Screen_Instance != default) || (Screen_Main.Screen_Instance != default))
                            {
                                string Entry_Text = Prompt.ShowDialog("Enter New Game Integrity Status", "SBRW Launcher");
                                if (!string.IsNullOrWhiteSpace(Entry_Text))
                                {
                                    DialogResult WIN_OWNER = DialogResult.OK;

                                    if (Screen_Settings.Screen_Instance != default)
                                    {
                                        WIN_OWNER = MessageBox.Show(Screen_Settings.Screen_Instance, "Confirm the Following Changes:" +
                                        "\n\nGame Integrity OLD: " + Save_Settings.Live_Data.Game_Integrity +
                                        "\nGame Integrity NEW: Good", "SBRW Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                    }
                                    else if (Screen_Main.Screen_Instance != default)
                                    {
                                        WIN_OWNER = MessageBox.Show(Screen_Settings.Screen_Instance, "Confirm the Following Changes:" +
                                        "\n\nGame Integrity OLD: " + Save_Settings.Live_Data.Game_Integrity +
                                        "\nGame Integrity NEW: Good", "SBRW Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                    }

                                    if (WIN_OWNER == DialogResult.Yes)
                                    {
                                        Save_Settings.Live_Data.Game_Integrity = Entry_Text;
                                        Save_Settings.Save();
                                        if (Screen_Main.Screen_Instance != null)
                                        {
                                            Screen_Main.Screen_Instance.Button_Settings.BackgroundImage = Image_Icon.Gear;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "hwid":
                    case "hw!d":
                        if ((Screen_Settings.Screen_Instance != default) || (Screen_Main.Screen_Instance != default))
                        {
                            string Live_ID = FingerPrint.Api_ID(Live_Commands == "hw!d");
                            DialogResult WIN_OWNER = DialogResult.OK;

                            if (Screen_Settings.Screen_Instance != default)
                            {
                                WIN_OWNER = MessageBox.Show(Screen_Settings.Screen_Instance, "ID:" +
                                Live_ID +
                                "\n\nClick Yes to Copy to Clipboard", "SBRW Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            }
                            else if (Screen_Main.Screen_Instance != default)
                            {
                                WIN_OWNER = MessageBox.Show(Screen_Main.Screen_Instance, "ID:" +
                                Live_ID +
                                "\n\nClick Yes to Copy to Clipboard", "SBRW Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            }

                            if (WIN_OWNER == DialogResult.Yes)
                            {
                                Clipboard.SetText(Live_ID);
                            }
                        }
                        break;
                    case "nfsw goes offline":
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=BEX3pd3vHks/");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=BEX3pd3vHks/", UseShellExecute = true });
#endif
                        break;
                    case "proxy domain preview":
                    case "pdp":
                        if (Save_Settings.Live_Data != default)
                        {
                            if (Save_Settings.Live_Data.Launcher_Proxy.Equals("0"))
                            {
                                if ((Screen_Settings.Screen_Instance != default) || (Screen_Main.Screen_Instance != default))
                                {
                                    string Entry_Text = Prompt.ShowDialog("Enter New Domain Name (Does not Save on Relaunch)", "SBRW Launcher");

                                    if (!string.IsNullOrWhiteSpace(Entry_Text))
                                    {
                                        DialogResult WIN_OWNER = DialogResult.OK;

                                        if (Screen_Settings.Screen_Instance != default)
                                        {
                                            WIN_OWNER = MessageBox.Show(Screen_Settings.Screen_Instance, "Confirm the Following Changes:" +
                                            "\n\nOLD Domain: " + Proxy_Settings.Domain +
                                            "\nNEW Domain: " + Entry_Text, "SBRW Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                        }
                                        else if (Screen_Main.Screen_Instance != default)
                                        {
                                            WIN_OWNER = MessageBox.Show(Screen_Main.Screen_Instance, "Confirm the Following Changes:" +
                                            "\n\nOLD Domain: " + Proxy_Settings.Domain +
                                            "\nNEW Domain: " + Entry_Text, "SBRW Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                        }

                                        if (WIN_OWNER == DialogResult.Yes)
                                        {
                                            if (Proxy_Settings.Running())
                                            {
                                                Proxy_Server.Instance.Stop("SBRW.Launcher.Core [Console]");
                                            }

                                            Log.Function("[Console]: Custom Proxy Domain:".ToUpper() + " -> " + (Proxy_Settings.Domain = Entry_Text) + " has been Set");

                                            if (!Proxy_Settings.Running())
                                            {
                                                Proxy_Server.Instance.Start("SBRW.Launcher.Core [Console]");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
#if (DEBUG_UNIX || RELEASE_UNIX)
                    case "alert storage space":
                    case "alert storage":
                    case "ass":
                    case "storage bypass":
                    case "sb":
                        if (!Save_Settings.Live_Data.Alert_Storage_Space.Equals("1"))
                        {
                            Save_Settings.Live_Data.Alert_Storage_Space = "1";
                            Save_Settings.Save();

                            if (Screen_Main.Screen_Instance != default)
                            {
                                Screen_Main.Screen_Instance.Game_Folder_Checks(true);
                            }
                        }
                        break;
#endif
                    case "restart game download":
                    case "restart gd":
                    case "rsgd":
                        if (Screen_Main.Screen_Instance != default)
                        {
                            Screen_Main.Screen_Instance.Game_Folder_Checks();
                        }
                        break;
                    case "delete game files pack":
                    case "delete game pack":
                    case "del gp":
                    case "dgp":
                        try
                        {
                            if (Save_Settings.Live_Data != default)
                            {
                                if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Archive_Location))
                                {
                                    if (File.Exists(Save_Settings.Live_Data.Game_Archive_Location))
                                    {
                                        File.Delete(Save_Settings.Live_Data.Game_Archive_Location);

                                        if (Screen_Settings.Screen_Instance != default)
                                        {
                                            MessageBox.Show(Screen_Settings.Screen_Instance, "Game Files Pack File Removed Successfully",
                                                "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else if (Screen_Main.Screen_Instance != default)
                                        {
                                            MessageBox.Show(Screen_Settings.Screen_Instance, "Game Files Pack File Removed Successfully",
                                                "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("Console Commands [dgp]", string.Empty, Error, string.Empty, false, Screen_Settings.Screen_Instance??default);
                        }
                        break;
                    case "the buggles":
                    case "i need a ride":
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=XUywABgU-Ow");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=XUywABgU-Ow", UseShellExecute = true });
#endif
                        break;
                    case "jun 4 2017":
                    case "aug 22, 2017":
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=5hv2p0RtVY0");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=5hv2p0RtVY0", UseShellExecute = true });
#endif
                        break;
                    case "mermaid sisters":
                    case "the mermaid sisters is here":
                    case "the mermaid sisters is here!":
#if NETFRAMEWORK
                        Process.Start("https://www.youtube.com/watch?v=OFjqEexH0Tg");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=OFjqEexH0Tg", UseShellExecute = true });
#endif
                        break;
                    case "fireworks":
#if NETFRAMEWORK
                        Process.Start("https://youtu.be/2m5vQo81Jik");
#else
                        Process.Start(new ProcessStartInfo { FileName = "https://youtu.be/2m5vQo81Jik", UseShellExecute = true });
#endif
                        break;
                    default:
                        if (!string.IsNullOrWhiteSpace(Live_Commands))
                        {
                            if (Screen_Settings.Screen_Instance != default)
                            {
                                MessageBox.Show(Screen_Settings.Screen_Instance, 
                                    "Command not Found", "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else if (Screen_Main.Screen_Instance != default)
                            {
                                MessageBox.Show(Screen_Main.Screen_Instance,
                                    "Command not Found", "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        break;
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Console Commands", string.Empty, Error, string.Empty, true);
            }
        }
    }
}