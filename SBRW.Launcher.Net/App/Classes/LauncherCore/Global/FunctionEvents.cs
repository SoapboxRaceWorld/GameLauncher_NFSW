using SBRW.Launcher.App.Classes.InsiderKit;
using SBRW.Launcher.App.Classes.LauncherCore.LauncherUpdater;
using SBRW.Launcher.App.Classes.LauncherCore.Lists;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.App.Classes.LauncherCore.Visuals;
using SBRW.Launcher.App.UI_Forms.Custom_Server_Screen;
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

namespace SBRW.Launcher.App.Classes.LauncherCore.Global
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
                Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Discord);
        }

        public static void HomePageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Launcher_Value.Launcher_Select_Server_JSON != null && !string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home))
                Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Home);
        }

        public static void FacebookGroupLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Launcher_Value.Launcher_Select_Server_JSON != null && !string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Facebook))
                Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Facebook);
        }

        public static void TwitterAccountLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Launcher_Value.Launcher_Select_Server_JSON != null && !string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Twitter))
                Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Social_Twitter);
        }

        public static void ForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Launcher_Value.Launcher_Select_Server_JSON != null)
            {
                if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Account_Recovery_Page))
                {
                    Process.Start(Launcher_Value.Launcher_Select_Server_JSON.Server_Account_Recovery_Page);
                    MessageBox.Show(null, "A browser window has been opened to complete password recovery on " +
                        Launcher_Value.Launcher_Select_Server_JSON.Server_Name, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string send = Prompt.ShowDialog("Please specify your email address.", "GameLauncher");

                    if (!string.IsNullOrWhiteSpace(send))
                    {
                        if (!Is_Email.Valid(send))
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
                Live_Commands = Live_Commands.ToLower();
                switch (Live_Commands)
                {
                    case "what if...":
                    case "what if":
                        Process.Start("https://www.youtube.com/watch?v=OaWYwk7dysc");
                        break;
                    case "don't look!":
                    case "dont look":
                        Process.Start("https://www.youtube.com/watch?v=nwqtdwcqrBE");
                        break;
                    case "behind the scenes":
                        Process.Start("https://cdn.discordapp.com/attachments/620401560954077214/987989783022272562/unknown.png");
                        break;
                    case "ezekiel":
                    case "crash the server":
                        Process.Start("https://www.youtube.com/watch?v=T-AF81iBCi0");
                        break;
                    case "ezekiel extended mix":
                    case "crash the server extended mix":
                        Process.Start("https://www.youtube.com/watch?v=sReIQTvS1kM");
                        break;
                    case "obsolete":
                        if (LauncherUpdateCheck.UpgradeAvailable)
                        {
                            Process.Start("https://youtu.be/LutDfASARmE");
                        }
                        break;
                    case "a song long ago":
                        Process.Start("https://youtu.be/zo_C-dk6Xh4");
                        break;
                    case "keygen":
                    case "keygen 2013":
                        Process.Start("https://youtu.be/vCMzIE9p07Y");
                        break;
                    case "who am i?":
                    case "who am i":
                        Process.Start("https://www.youtube.com/watch?v=6TmlR27izRo");
                        break;
                    case "straightuphippo":
                        Process.Start("https://youtu.be/Uc57tO6g--I");
                        break;
                    case "seeing what's next":
                    case "seeing whats next":
                    case "insider":
                    case "developer":
                    case "beta":
                        Process.Start("https://youtu.be/F6dVLZIJatk");
                        break;
                    case "password":
                    case "my password":
                        string Text_Display = string.IsNullOrWhiteSpace(Save_Account.Live_Data.User_Raw_Password) ? "No Password Found" : 
                            "Your Password: " + Save_Account.Live_Data.User_Raw_Password;
                        MessageBox.Show(null, Text_Display, "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "now loading":
                    case "now loading!!!":
                        Process.Start("https://www.youtube.com/watch?v=kq3X78ngFAY");
                        break;
                    case "update":
                        if (LauncherUpdateCheck.UpgradeAvailable)
                        {
                            LauncherUpdateCheck.UpdateStatusResult(true);
                        }
                        break;
                    case "build":
                    case "build date":
                        MessageBox.Show(null, InsiderInfo.BuildNumber(), "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;
                    case "help":
                        MessageBox.Show(null, "Available Commands" +
                            "\nMy Password - Displays your raw Password" +
                            "\nUpdate - Triggers the Update popup, if an Update is available" +
                            "\nBuild Date - Displays Compiled Date"
                            , "SBRW Launcher Commands", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;
                    default:
                        if (!string.IsNullOrWhiteSpace(Live_Commands))
                        {
                            MessageBox.Show(null, "Command not Found", "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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