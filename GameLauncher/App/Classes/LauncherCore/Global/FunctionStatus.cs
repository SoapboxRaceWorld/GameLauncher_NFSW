using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Global
{
    /* This is Used to Cache Responses From the Launcher */
    class InformationCache
    {
        /* ServerList Load Checks */
        public static string ServerListStatus = "Unknown";

        /* CDNList Load Checks */
        public static string CDNListStatus = "Unknown";

        /* System Language */
        public static string CurrentLanguage = "EN";

        /* Sets Game Launchers User Agent (If Required) */
        public static string UserAgent;

        /* System OS Name */
        public static string OSName;

        /* Selected Server Category */
        public static string SelectedServerCategory;

        /* Selected Server List Key Information */
        public static ServerList SelectedServerData;

        /* Selected Server JSON (GetServerInformation) */
        public static GetServerInformation SelectedServerJSON = new GetServerInformation();

        /* Holds a collection of Server Status of Servers */
        public static Dictionary<string, int> ServerStatusBook = new Dictionary<string, int>();
    }

    /* This is Used to call Certain Functions (Such as Completion Status or Function Callbacks) */
    class FunctionStatus
    {
        /* Verify Hash Status */
        public static bool IsVerifyHashDisabled = false;

        /* Visual API Status */
        public static bool IsVisualAPIsChecked = false;

        /* Sets Conditional to If its Possible to Close Game */
        public static Boolean CanCloseGame = true;

        /* Detect and Set System Language */
        public static CultureInfo Lang = CultureInfo.CurrentUICulture;

        /* Checks if we have Write Permissions */
        public static bool HasWriteAccessToFolder(string path)
        {
            try
            {
                File.Create(path + "temp.txt").Close();
                File.Delete(path + "temp.txt");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /* Used to Center WinForms Forms (Parent Screen) */
        public static void CenterScreen(Form form)
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2;
        }

        /* Check if Folder Location is Acceptable and Returns a Value
        /* Let's actually make it cleaner and nicer - MeTonaTOR */
        public static FolderType CheckFolder(string FolderName)
        {
            if (FolderName.Contains("C:\\Users") && FolderName.Contains("Temp")) return FolderType.IsTempFolder;
            if (FolderName.Contains("C:\\Users")) return FolderType.IsUsersFolders;
            if (FolderName.Contains("C:\\Program Files")) return FolderType.IsProgramFilesFolder;
            if (FolderName.Contains("C:\\Windows")) return FolderType.IsWindowsFolder;
            if (FolderName.Length == 3) return FolderType.IsRootFolder;
            if (FolderName + "\\" == AppDomain.CurrentDomain.BaseDirectory) return FolderType.IsSameAsLauncherFolder;

            return FolderType.Unknown;
        }

        /* Converts Host Name to a IP (ex. http://localhost -> 192.168.1.69 */
        public static string HostName2IP(string hostname)
        {
            IPHostEntry iphost = Dns.GetHostEntry(hostname);
            IPAddress[] addresses = iphost.AddressList;
            return addresses[0].ToString();
        }

        /* Check System Language and Return Current Lang for Speech Files */
        public static string SpeechFiles()
        {
            string CurrentLang = Lang.ThreeLetterISOLanguageName;

            if (CurrentLang == "eng") return "en";
            else if (CurrentLang == "ger" || CurrentLang == "deu") return "de";
            else if (CurrentLang == "rus") return "ru";
            else if (CurrentLang == "spa") return "es";
            else return "en";
        }

        public static int SpeechFilesSize()
        {
            string CurrentLang = Lang.ThreeLetterISOLanguageName;

            if (CurrentLang == "eng") return 141805935;
            else if (CurrentLang == "ger" || CurrentLang == "deu") return 105948386;
            else if (CurrentLang == "rus") return 121367723;
            else if (CurrentLang == "spa") return 101540466;
            else return 141805935;
        }

        public static void FirstTimeRun()
        {
            Log.Core("LAUNCHER: Checking InstallationDirectory: " + FileSettingsSave.GameInstallation);
            if (string.IsNullOrEmpty(FileSettingsSave.GameInstallation))
            {
                Log.Core("LAUNCHER: First run!");

                try
                {
                    Form welcome = new WelcomeScreen();
                    DialogResult welcomereply = welcome.ShowDialog();

                    if (welcomereply != DialogResult.OK)
                    {
                        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                    }
                    else
                    {
                        FileSettingsSave.CDN = SelectedCDN.CDNUrl;
                        FileSettingsSave.SaveSettings();
                    }
                }
                catch
                {
                    Log.Warning("LAUNCHER: CDN Source URL was Empty! Setting a Null Safe URL 'http://localhost'");
                    FileSettingsSave.CDN = "http://localhost";
                    Log.Core("LAUNCHER: Installation Directory was Empty! Creating and Setting Directory at " + AppDomain.CurrentDomain.BaseDirectory + "\\Game Files");
                    FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                    FileSettingsSave.SaveSettings();
                }

                var fbd = new CommonOpenFileDialog
                {
                    EnsurePathExists = true,
                    EnsureFileExists = false,
                    AllowNonFileSystemItems = false,
                    Title = "Select the location to Find or Download NFS:W",
                    IsFolderPicker = true
                };

                if (fbd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (!FunctionStatus.HasWriteAccessToFolder(fbd.FileName))
                    {
                        Log.Error("LAUNCHER: Not enough permissions. Exiting.");
                        MessageBox.Show(null, "You don't have enough permission to select this path as installation folder. Please select another directory.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Environment.Exit(Environment.ExitCode);
                    }

                    if (fbd.FileName.Length == 3)
                    {
                        Log.Warning("LAUNCHER: Installing NFSW in root of the harddisk is not allowed.");
                        MessageBox.Show(null, string.Format("Installing NFSW in root of the harddisk is not allowed. " +
                            "Instead, we will install it on {0}.", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        FileSettingsSave.SaveSettings();
                    }
                    else if (fbd.FileName == AppDomain.CurrentDomain.BaseDirectory)
                    {
                        Directory.CreateDirectory("Game Files");
                        Log.Warning("LAUNCHER: Installing NFSW in same directory where the launcher resides is disadvised.");
                        MessageBox.Show(null, string.Format("Installing NFSW in same directory where the launcher resides is disadvised. " +
                            "Instead, we will install it on {0}.", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        FileSettingsSave.SaveSettings();
                    }
                    else
                    {
                        Log.Core("LAUNCHER: Directory Set: " + fbd.FileName);
                        FileSettingsSave.GameInstallation = fbd.FileName;
                        FileSettingsSave.SaveSettings();
                    }
                }
                else
                {
                    Log.Core("LAUNCHER: Exiting");
                    Environment.Exit(Environment.ExitCode);
                }
                fbd.Dispose();
            }

            if (!DetectLinux.LinuxDetected())
            {
                switch (FunctionStatus.CheckFolder(FileSettingsSave.GameInstallation))
                {
                    case FolderType.IsSameAsLauncherFolder:
                        Directory.CreateDirectory("Game Files");
                        Log.Error("LAUNCHER: Installing NFSW in same location where the GameLauncher resides is NOT allowed.");
                        MessageBox.Show(null, string.Format("Installing NFSW in same location where the GameLauncher resides is NOT allowed.\n" +
                            "Instead, we will install it at {0}.", AppDomain.CurrentDomain.BaseDirectory + "Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        break;

                    case FolderType.IsTempFolder:
                    case FolderType.IsUsersFolders:
                    case FolderType.IsProgramFilesFolder:
                    case FolderType.IsWindowsFolder:
                    case FolderType.IsRootFolder:
                        String constructMsg = String.Empty;
                        Directory.CreateDirectory("Game Files");
                        constructMsg += "Using this location for Game Files is not allowed.\nThe following list are NOT allowed:\n\n";
                        constructMsg += "• X:\\ (Root of Drive, such as C:\\ or D:\\)\n";
                        constructMsg += "• C:\\Program Files\n";
                        constructMsg += "• C:\\Program Files (x86)\n";
                        constructMsg += "• C:\\Users (Includes 'Desktop', 'Documents', 'Downloads')\n";
                        constructMsg += "• C:\\Windows\n\n";
                        constructMsg += "Instead, we will install the NFSW Game at " + AppDomain.CurrentDomain.BaseDirectory + "\\Game Files\n";

                        MessageBox.Show(null, constructMsg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Log.Error("LAUNCHER: Installing NFSW in a Restricted Location is not allowed.");
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        break;
                }
                FileSettingsSave.SaveSettings();

                /* Windows Defender (Windows 10) */
                if (WindowsProductVersion.CachedWindowsNumber >= 10.0 && (FileSettingsSave.WindowsDefenderStatus == "Not Excluded" || FileSettingsSave.WindowsDefenderStatus == "Unknown"))
                {
                    Log.Core("WINDOWS DEFENDER: Windows 10 Detected! Running Exclusions for Core Folders");
                    if (ManagementSearcher.SecurityCenter("AntivirusEnabled") == true && ManagementSearcher.SecurityCenter("AntispywareEnabled") == true)
                    {
                        /* Create Windows Defender Exclusion */
                        try
                        {
                            Log.Info("WINDOWS DEFENDER: Excluding Core Folders");
                            /* Add Exclusion to Windows Defender */
                            using (PowerShell ps = PowerShell.Create())
                            {
                                ps.AddScript($"Add-MpPreference -ExclusionPath \"{AppDomain.CurrentDomain.BaseDirectory}\"");
                                ps.AddScript($"Add-MpPreference -ExclusionPath \"{FileSettingsSave.GameInstallation}\"");
                                var result = ps.Invoke();
                            }

                            FileSettingsSave.WindowsDefenderStatus = "Excluded";
                            FileSettingsSave.SaveSettings();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("WINDOWS DEFENDER: " + ex.Message);
                            FileSettingsSave.WindowsDefenderStatus = "Not Excluded";
                            FileSettingsSave.SaveSettings();
                        }
                    }
                    else
                    {
                        FileSettingsSave.WindowsDefenderStatus = "Not Supported";
                        FileSettingsSave.SaveSettings();
                    }
                }
                else if (WindowsProductVersion.CachedWindowsNumber >= 10.0 && !string.IsNullOrEmpty(FileSettingsSave.WindowsDefenderStatus))
                {
                    Log.Core("WINDOWS DEFENDER: Found 'WindowsDefender' key! Its value is " + FileSettingsSave.WindowsDefenderStatus);
                }
            }
        }

        /* Moved "runAsAdmin" Code to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/eec2f9f80aa4b350ab98d32383e1ee1f2e1c26fd/Self.cs */
    }

    /* This is Used to Visual Events (Draw or Click Events) */
    class FunctionEvents
    {
        /* ServerList Load Checks */
        public static void SelectServerBtn_Click(object sender, EventArgs e)
        {
            new SelectServer().ShowDialog();
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

        public static void AddServer_Click(object sender, EventArgs e)
        {
            new AddServer().Show();
        }

        public static void AboutButton_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }
    }
}