using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Visuals;
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
            new SelectServer().ShowDialog();
        }

        public static void AddServer_Click(object sender, EventArgs e)
        {
            new AddServer().Show();
        }

        public static void AboutButton_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        public static void RegisterText_LinkClicked(object sender, EventArgs e)
        {
            if (FunctionStatus.AllowRegistration)
            {
                if (!string.IsNullOrEmpty(InformationCache.SelectedServerJSON.webSignupUrl))
                {
                    Process.Start(InformationCache.SelectedServerJSON.webSignupUrl);
                    MessageBox.Show(null, "A browser window has been opened to complete registration on " + InformationCache.SelectedServerJSON.serverName, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (InformationCache.SelectedServerData.Name.ToUpper() == "WORLDUNITED OFFICIAL")
                {
                    Process.Start("https://signup.worldunited.gg/?discordid=" + DiscordLauncherPresense.UserID);
                    MessageBox.Show(null, "A browser window has been opened to complete registration on " + InformationCache.SelectedServerData.Name, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    new RegisterScreen().ShowDialog();
                }
            }
            else
            {
                MessageBox.Show(null, "Server seems to be offline.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void DiscordInviteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InformationCache.SelectedServerJSON.discordUrl))
                Process.Start(InformationCache.SelectedServerJSON.discordUrl);
        }

        public static void HomePageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InformationCache.SelectedServerJSON.homePageUrl))
                Process.Start(InformationCache.SelectedServerJSON.homePageUrl);
        }

        public static void FacebookGroupLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InformationCache.SelectedServerJSON.facebookUrl))
                Process.Start(InformationCache.SelectedServerJSON.facebookUrl);
        }

        public static void TwitterAccountLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InformationCache.SelectedServerJSON.facebookUrl))
                Process.Start(InformationCache.SelectedServerJSON.twitterUrl);
        }

        public static void ForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InformationCache.SelectedServerJSON.webRecoveryUrl))
            {
                Process.Start(InformationCache.SelectedServerJSON.webRecoveryUrl);
                MessageBox.Show(null, "A browser window has been opened to complete password recovery on " +
                    InformationCache.SelectedServerJSON.serverName, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                string send = Prompt.ShowDialog("Please specify your email address.", "GameLauncher");

                if (send != String.Empty)
                {
                    String responseString;
                    try
                    {
                        Uri resetPasswordUrl = new Uri(InformationCache.SelectedServerData.IpAddress + "/RecoveryPassword/forgotPassword");

                        var request = (HttpWebRequest)System.Net.WebRequest.Create(resetPasswordUrl);
                        var postData = "email=" + send;
                        var data = Encoding.ASCII.GetBytes(postData);
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = data.Length;

                        using (var stream = request.GetRequestStream())
                        {
                            stream.Write(data, 0, data.Length);
                        }

                        var response = (HttpWebResponse)request.GetResponse();
                        responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }
                    catch
                    {
                        responseString = "Failed to send email!";
                    }

                    MessageBox.Show(null, responseString, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public static void ComboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var font = (sender as ComboBox).Font;
            Brush backgroundColor;
            Brush textColor;

            var serverListText = "";
            int onlineStatus = 2; /* 0 = offline | 1 = online | 2 = checking */

            if (sender is ComboBox cb)
            {
                if (cb.Items[e.Index] is ServerList si)
                {
                    serverListText = si.Name;
                    onlineStatus = InformationCache.ServerStatusBook.ContainsKey(si.Id) ? InformationCache.ServerStatusBook[si.Id] : 2;
                }
            }

            if (serverListText.StartsWith("<GROUP>"))
            {
                font = new Font(font, FontStyle.Bold);
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                e.Graphics.DrawString(serverListText.Replace("<GROUP>", string.Empty), font, Brushes.Black, e.Bounds);
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
                    if (onlineStatus == 2)
                    {
                        /* CHECKING */
                        backgroundColor = Brushes.Khaki;
                    }
                    else if (onlineStatus == 1)
                    {
                        /* ONLINE */
                        backgroundColor = Brushes.PaleGreen;
                    }
                    else
                    {
                        /* OFFLINE */
                        backgroundColor = Brushes.LightCoral;
                    }

                    textColor = Brushes.Black;
                }

                e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                e.Graphics.DrawString("    " + serverListText, font, textColor, e.Bounds);
            }
        }
    }
}