using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Validator.Email;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.UI_Forms.Register_Screen;
using GameLauncher.App.UI_Forms.SelectServer_Screen;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Global
{
    /* This is Used to Visual Events (Draw or Click Events) */
    class FunctionEvents
    {
        /* ServerList Load Checks */
        public static void SelectServerBtn_Click(object sender, EventArgs e)
        {
            SelectServer.OpenScreen(false);
        }

        public static void AddServer_Click(object sender, EventArgs e)
        {
            SelectServer.OpenScreen(true);
        }

        public static void RegisterText_LinkClicked(object sender, EventArgs e)
        {
            if (FunctionStatus.AllowRegistration)
            {
                if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.webSignupUrl))
                {
                    Process.Start(InformationCache.SelectedServerJSON.webSignupUrl);
                    MessageBox.Show(null, "A browser window has been opened to complete registration on " +
                        ServerListUpdater.ServerName("Register"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (InformationCache.SelectedServerData.Name.ToUpper() == "WORLDUNITED OFFICIAL")
                {
                    Process.Start("https://signup.worldunited.gg/" + ((!string.IsNullOrWhiteSpace(DiscordLauncherPresence.UserID) &&
                        DiscordLauncherPresence.UserID != "0") ? "?discordid=" + DiscordLauncherPresence.UserID : string.Empty));
                    MessageBox.Show(null, "A browser window has been opened to complete registration on " +
                        InformationCache.SelectedServerData.Name, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    RegisterScreen.OpenScreen();
                }
            }
            else
            {
                MessageBox.Show(null, "Server seems to be Offline.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void DiscordInviteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.discordUrl))
                Process.Start(InformationCache.SelectedServerJSON.discordUrl);
        }

        public static void HomePageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.homePageUrl))
                Process.Start(InformationCache.SelectedServerJSON.homePageUrl);
        }

        public static void FacebookGroupLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.facebookUrl))
                Process.Start(InformationCache.SelectedServerJSON.facebookUrl);
        }

        public static void TwitterAccountLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.facebookUrl))
                Process.Start(InformationCache.SelectedServerJSON.twitterUrl);
        }

        public static void ForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (InformationCache.SelectedServerJSON != null)
            {
                if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.webRecoveryUrl))
                {
                    Process.Start(InformationCache.SelectedServerJSON.webRecoveryUrl);
                    MessageBox.Show(null, "A browser window has been opened to complete password recovery on " +
                        InformationCache.SelectedServerJSON.serverName, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string send = Prompt.ShowDialog("Please specify your email address.", "GameLauncher");

                    if (!string.IsNullOrWhiteSpace(send))
                    {
                        if (!IsEmailValid.Validate(send))
                        {
                            MessageBox.Show(null, "Email Address is not Valid. Please Check and Try Again", "GameLauncher",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            try
                            {
                                Uri resetPasswordUrl = new Uri(InformationCache.SelectedServerData.IPAddress + "/RecoveryPassword/forgotPassword");
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
                        if (cb.Items[e.Index] is ServerList si)
                        {
                            serverListText = si.Name;
                            onlineStatus = InformationCache.ServerStatusBook.ContainsKey(si.ID) ? InformationCache.ServerStatusBook[si.ID] : 2;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(serverListText))
                {
                    Font font = (sender as ComboBox).Font;
                    Brush backgroundColor;
                    Brush textColor;

                    if (serverListText.StartsWith("<GROUP>"))
                    {
                        font = new Font(font, FontStyle.Bold);
                        e.Graphics.FillRectangle(new SolidBrush(Theming.DropMenuWhite), e.Bounds);
                        e.Graphics.DrawString(serverListText.Replace("<GROUP>", string.Empty), font, new SolidBrush(Theming.DropMenuBlack), e.Bounds);
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
                            switch (onlineStatus)
                            {
                                case 1:
                                    /* ONLINE */
                                    backgroundColor = new SolidBrush(Theming.DropMenuPingSuccess);
                                    break;
                                case 2:
                                    /* CHECKING */
                                    backgroundColor = new SolidBrush(Theming.DropMenuPingChecking);
                                    break;
                                case 3:
                                    /* GSI ERROR */
                                    backgroundColor = new SolidBrush(Theming.DropMenuPingWarning);
                                    break;
                                default:
                                    /* OFFLINE */
                                    backgroundColor = new SolidBrush(Theming.DropMenuPingError);
                                    break;
                            }

                            textColor = new SolidBrush(Theming.DropMenuBlack);
                        }

                        e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                        e.Graphics.DrawString("    " + serverListText, font, textColor, e.Bounds);
                    }
                }
            }
            catch { }
        }
    }
}