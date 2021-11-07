using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.ModNet.JSON;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Validator.JSON;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using static System.String;

namespace GameLauncher.App.UI_Forms.AddServer_Screen
{
    public partial class AddServer : Form
    {
        private static bool IsAddServerOpen = false;
        /// <summary>Opens the Form. If the Form is already Open it will Give it Foucus</summary>
        public static void OpenScreen()
        {
            if (IsAddServerOpen || Application.OpenForms["AddServer"] != null)
            {
                if (Application.OpenForms["AddServer"] != null) { Application.OpenForms["AddServer"].Activate(); }
            }
            else
            {
                try { new AddServer().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Add Server Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Add Server Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public AddServer()
        {
            IsAddServerOpen = true;
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, CloseForm) =>
            {
                IsAddServerOpen = false;
                GC.Collect();
            };
        }

        public void DrawErrorAroundTextBox(TextBox x)
        {
            x.BorderStyle = BorderStyle.Fixed3D;
            Pen p = new Pen(Theming.WinFormErrorTextForeColor);
            Graphics g = this.CreateGraphics();
            int variance = 1;
            g.DrawRectangle(p, new Rectangle(x.Location.X - variance, x.Location.Y - variance, x.Width + variance, x.Height + variance));
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Text = "Add Server - SBRW Launcher: v" + Application.ProductVersion;

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            Version.Text = "Version : v" + Application.ProductVersion;

            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            float MainFontSize = UnixOS.Detected() ? 9f : 9f * 96f / CreateGraphics().DpiY;
            Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);

            OkBTN.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            CancelBTN.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ServerNameLabel.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ServerName.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ServerAddressLabel.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ServerAddress.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ServerCategoryLabel.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ServerCategory.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            Error.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            Version.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors              /
            /********************************/

            BackColor = Theming.WinFormTBGForeColor;
            ForeColor = Theming.WinFormTextForeColor;

            ServerName.BackColor = Theming.WinFormTBGDarkerForeColor;
            ServerName.ForeColor = Theming.WinFormSecondaryTextForeColor;

            ServerAddress.BackColor = Theming.WinFormTBGDarkerForeColor;
            ServerAddress.ForeColor = Theming.WinFormSecondaryTextForeColor;

            ServerCategory.BackColor = Theming.WinFormTBGDarkerForeColor;
            ServerCategory.ForeColor = Theming.WinFormSecondaryTextForeColor;

            Error.ForeColor = Theming.WinFormWarningTextForeColor;

            ServerNameLabel.ForeColor = Theming.WinFormTextForeColor;
            ServerAddressLabel.ForeColor = Theming.WinFormTextForeColor;
            ServerCategoryLabel.ForeColor = Theming.WinFormTextForeColor;
            Version.ForeColor = Theming.WinFormTextForeColor;

            CancelBTN.ForeColor = Theming.BlueForeColorButton;
            CancelBTN.BackColor = Theming.BlueBackColorButton;
            CancelBTN.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            CancelBTN.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            OkBTN.ForeColor = Theming.BlueForeColorButton;
            OkBTN.BackColor = Theming.BlueBackColorButton;
            OkBTN.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            OkBTN.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            /********************************/
            /* Events                        /
            /********************************/

            ServerCategory.GotFocus += new EventHandler(ServerCategory_RemovePlaceHolderText);
            ServerCategory.LostFocus += new EventHandler(ServerCategory_ShowPlaceHolderText);

            Shown += (x, y) =>
            {
                Application.OpenForms["AddServer"].Activate();
                this.BringToFront();
            };
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameOldServersJSON))))
                {
                    File.Move(
                        Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameOldServersJSON)),
                        Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON)));
                }
                else if (!File.Exists(
                    Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON))))
                {
                    try
                    {
                        File.WriteAllText(
                            Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON)), "[]");
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("JSON SERVER FILE", null, Error, null, true);
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("JSON SERVER FILE", null, Error, null, true);
            }

            Error.Visible = false;
            this.Refresh();
            if (IsNullOrWhiteSpace(ServerAddress.Text) || IsNullOrWhiteSpace(Strings.Encode(ServerName.Text)))
            {
                if (IsNullOrWhiteSpace(ServerAddress.Text))
                {
                    Error.Text = "Fix Empty IP";
                    DrawErrorAroundTextBox(ServerAddress);
                }
                else
                {
                    Error.Text = "Fix Empty Name";
                    DrawErrorAroundTextBox(ServerName);
                }

                Error.Visible = true;
                return;
            }

            if (Error.Visible)
            {
                Error.Visible = false;
            }

            bool CorrectFormat = Uri.TryCreate(ServerAddress.Text, UriKind.Absolute, out Uri Result) && (Result.Scheme == Uri.UriSchemeHttp || Result.Scheme == Uri.UriSchemeHttps);

            string FormattedURL;
            if (!CorrectFormat)
            {
                DrawErrorAroundTextBox(ServerAddress);
                return;
            }
            else
            {
                FormattedURL = Result.ToString();
            }

            ButtonControls(false);

            try
            {
                string ServerInfomationJSON = Empty;
                try
                {
                    Uri StringToUri = new Uri(FormattedURL + "/GetServerInformation");
                    ServicePointManager.FindServicePoint(StringToUri).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
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
                        ServerInfomationJSON = Client.DownloadString(StringToUri);
                    }
                    catch (Exception Error)
                    {
                        string LogMessage = "Add Server Check Encountered an Error:";
                        LogToFileAddons.OpenLog("Add Server", LogMessage, Error, null, false);
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
                    string LogMessage = "Add Server Check Encountered an Error:";
                    LogToFileAddons.OpenLog("Add Server", LogMessage, Error, null, false);
                }

                if (IsNullOrWhiteSpace(ServerInfomationJSON))
                {
                    ButtonControls(true);
                    return;
                }
                else if (!IsJSONValid.ValidJson(ServerInfomationJSON))
                {
                    Error.Text = "Unstable Connection";
                    DrawErrorAroundTextBox(ServerAddress);
                    Error.Visible = true;
                    ButtonControls(true);
                    ServerInfomationJSON = null;
                    return;
                }
                else
                {
                    GetServerInformation ServerInformationData = null;

                    try
                    {
                        ServerInformationData = JsonConvert.DeserializeObject<GetServerInformation>(ServerInfomationJSON);
                    }
                    catch (Exception Error)
                    {
                        string LogMessage = "Add Server Get Information Encountered an Error:";
                        LogToFileAddons.OpenLog("Add Server", LogMessage, Error, null, false);
                    }

                    if (ServerInformationData == null)
                    {
                        ButtonControls(true);
                        ServerInfomationJSON = null;
                        return;
                    }
                    else
                    {
                        string ServerID = Empty;
                        Uri newModNetUri = new Uri(FormattedURL + "/Modding/GetModInfo");
                        ServicePointManager.FindServicePoint(newModNetUri).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
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
                            GetModInfo ServerGetInfo = JsonConvert.DeserializeObject<GetModInfo>(Client.DownloadString(newModNetUri));
                            ServerID = IsNullOrWhiteSpace(ServerGetInfo.serverID) ? Result.Host : ServerGetInfo.serverID;
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("Add Server", null, Error, null, true);
                            ServerID = Result.Host;
                        }
                        finally
                        {
                            if (Client != null)
                            {
                                Client.Dispose();
                            }
                        }

                        try
                        {
                            StreamReader sr = new StreamReader(Locations.LauncherCustomServers);
                            String oldcontent = sr.ReadToEnd();
                            sr.Close();

                            if (IsNullOrWhiteSpace(oldcontent))
                            {
                                oldcontent = "[]";
                            }

                            var Servers = JsonConvert.DeserializeObject<List<ServerList>>(oldcontent);

                            Servers.Add(new ServerList
                            {
                                Name = Strings.Encode(ServerName.Text),
                                IPAddress = FormattedURL,
                                IsSpecial = false,
                                ID = ServerID,
                                Category = IsNullOrWhiteSpace(Strings.Encode(ServerCategory.Text)) ? "Custom" : Strings.Encode(ServerCategory.Text)
                            });

                            File.WriteAllText(Locations.LauncherCustomServers, JsonConvert.SerializeObject(Servers));

                            MessageBox.Show(null, "The New server will be added on the next start of the Launcher.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception Error)
                        {
                            string LogMessage = "Failed to Add New Server:";
                            LogToFileAddons.OpenLog("Add Server", LogMessage, Error, null, false);
                            ButtonControls(true);
                            return;
                        }
                        finally
                        {
                            if (ServerID != null)
                            {
                                ServerID = null;
                            }

                            if (ServerInfomationJSON != null)
                            {
                                ServerInfomationJSON = null;
                            }
                        }

                        CancelButton_Click(sender, e);
                    }
                }
            }
            catch
            {
                DrawErrorAroundTextBox(ServerAddress);
                ButtonControls(true);
            }
        }

        private void ButtonControls(bool Enable)
        {
            CancelBTN.Enabled = Enable;
            OkBTN.Enabled = Enable;
            ServerAddress.Enabled = Enable;
            ServerName.Enabled = Enable;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ServerCategory_ShowPlaceHolderText(object sender, EventArgs e)
        {
            if (IsNullOrWhiteSpace(Strings.Encode(ServerCategory.Text)))
            {
                ServerCategory.Text = "Custom";
            }
        }

        public void ServerCategory_RemovePlaceHolderText(object sender, EventArgs e)
        {
            if (ServerCategory.Text == "Custom")
            {
                ServerCategory.Text = Empty;
            }
        }
    }
}
