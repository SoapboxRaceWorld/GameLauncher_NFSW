using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Global
{
    /* This is Used to Cache Responses From the Launcher */
    class InformationCache
    {
        /* Parent Screen Cords */
        public static Point ParentScreenLocation;

        /* ServerList Load Checks */
        public static string ServerListStatus = "Unknown";

        /* CDNList Load Checks */
        public static string CDNListStatus = "Unknown";

        /* System Language */
        public static string CurrentLanguage = "EN";

        /* System OS Name */
        public static string OSName;

        /* Selected Server Requires Proxy (Due to being https Only) */
        public static bool ModernAuthSecureChannel = false;

        /* Selected Server ModernAuth Hash Type (Used for Login) */
        public static string ModernAuthHashType = string.Empty;

        /* Selected Server Category */
        public static string SelectedServerCategory;

        /* Selected Server List Key Information */
        public static ServerList SelectedServerData;

        /* Selected Server JSON (GetServerInformation) */
        public static GetServerInformation SelectedServerJSON = new GetServerInformation();

        /* Holds a collection of Server Status of Servers */
        public static Dictionary<string, int> ServerStatusBook = new Dictionary<string, int>();

        /* Selected Server Force Restart Timer */
        public static int RestartTimer;
    }

    /* This is Used to call Certain Functions (Such as Completion Status or Function Callbacks) */
    class FunctionStatus
    {
        /* Launcher had Encounterd an Error and It Must Close */
        public static bool LauncherForceClose = false;

        /* Launcher had Encounterd an Error and It Reason*/
        public static string LauncherForceCloseReason;

        /* Updater.cs Sets Conditional on If Launcher had Finished Loading (It Self) */
        public static bool LoadingComplete = false;

        /* Allows Registration Button to be Enabled/Disabled */
        public static bool AllowRegistration;

        /* If Verify Hash (.dat) File Exists on Server */
        public static bool DoesCDNSupportVerifyHash = false;

        /* Verify Hash Status */
        public static bool IsVerifyHashDisabled = false;

        /* Firewall Status */
        public static bool IsFirewallResetDisabled = false;

        /* Windows Security (Defender) Status */
        public static bool IsWindowsSecurityResetDisabled = false;

        /* Visual API Status */
        public static bool IsVisualAPIsChecked = false;

        /* Sets Conditional to If its Possible to Close Game */
        public static Boolean CanCloseGame = true;

        /* If In-Game OverLays was Used */
        public static bool ExternalToolsWasUsed = false;

        /* Sets Conditional if Game was Closed (By Timer) */
        public static bool GameKilledBySpeedBugCheck = false;

        /* Detect and Set System Language */
        public static CultureInfo Lang = CultureInfo.CurrentUICulture;

        /* Prevents Launcher from bring Closed when Game is Loading */
        public static bool LauncherBattlePass = false;

        /* Checks if we have Write Permissions */
        public static bool HasWriteAccessToFolder(string path)
        {
            try
            {
                File.Create(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(path)) + "temp.txt").Close();
                File.Delete(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(path)) + "temp.txt");
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

        public static void CenterParent(Form form)
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Location = InformationCache.ParentScreenLocation;
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
        public static string SpeechFiles(string Language)
        {
            string CurrentLang = string.IsNullOrWhiteSpace(Language) ? Lang.ThreeLetterISOLanguageName : Language.ToLower();

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

        public static void TLS()
        {
            ServicePointManager.DnsRefreshTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            ServicePointManager.Expect100Continue = true;
            try
            {
                /* TLS 1.3 */
                ServicePointManager.SecurityProtocol |= (SecurityProtocolType)12288 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            }
            catch (NotSupportedException Error)
            {
                Log.Error("SecurityProtocol: Tls13 -> " + Error.Message);

                try
                {
                    /* TLS 1.2 */
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                catch (NotSupportedException ErrorTls12)
                {
                    Log.Error("SecurityProtocol: Tls12 -> " + ErrorTls12.Message);

                    try
                    {
                        /* TLS 1.1 */
                        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    }
                    catch (NotSupportedException ErrorTls11)
                    {
                        Log.Error("SecurityProtocol: Tls11 -> " + ErrorTls11.Message);

                        try
                        {
                            /* TLS 1.0 */
                            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls;
                        }
                        catch (NotSupportedException ErrorTls)
                        {
                            Log.Error("SecurityProtocol: Tls -> " + ErrorTls.Message);
                        }
                    }
                }
            }
            ServicePointManager.ServerCertificateValidationCallback = (Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                bool isOk = true;
                if (sslPolicyErrors != SslPolicyErrors.None)
                {
                    for (int i = 0; i < chain.ChainStatus.Length; i++)
                    {
                        if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                        {
                            continue;
                        }
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 15);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                            break;
                        }
                    }
                }
                return isOk;
            };
        }

        /// <summary>
        /// Reads a key from the Windows Registry.
        /// </summary>
        /// <returns>Registry Key Value or if it Doesn't Exist it Returns a Null Value</returns>
        /// <param name="keyName">Registry Key Entry</param>
        public static string RegistryRead(string keyName)
        {
            string subKey = "SOFTWARE\\Soapbox Race World\\Launcher";

            try
            {
                RegistryKey sk = Registry.LocalMachine.OpenSubKey(subKey, false);
                if (sk == null)
                    return null;
                else
                    return sk.GetValue(keyName).ToString();
            }
            catch (Exception error)
            {
                Log.Error("REGISTRYKEY: READ " + error.Message);
                Log.Error("REGISTRYKEY: READ " + error.InnerException);
                return null;
            }
        }

        /// <summary>
        /// Writes a key to the Windows Registry.
        /// </summary>
        /// <param name="keyName">Registry Key Entry</param>
        /// <param name="value">Inner Value to write to for Key Entry</param>
        public static void RegistryWrite(string keyName, string value)
        {
            string subKey = "SOFTWARE\\Soapbox Race World\\Launcher";

            try
            {
                RegistryKey sk = Registry.LocalMachine.CreateSubKey(subKey, true);
                sk.SetValue(keyName, value);
            }
            catch (Exception error)
            {
                Log.Error("REGISTRYKEY: WRITE " + error.Message);
            }
        }

        /// <summary>
        /// Used to Force Close Launcher when Launcher encounters an error during Startup
        /// </summary>
        /// <param name="Notes">Required: Where the Launcher is Closing From</param>
        /// <param name="Boolen">True: Restarts Launcher | False: Closes Launcher</param>
        public static void ErrorCloseLauncher(string Notes, bool Boolen)
        {
            if (DiscordLauncherPresense.Running())
            {
                DiscordLauncherPresense.Stop("Close");
            }

            if (ServerProxy.Running())
            {
                ServerProxy.Instance.Stop("Force Close");
            }
            
            Log.Warning("LAUNCHER: Exiting (" + Notes + ")");
            if (!string.IsNullOrWhiteSpace(LauncherForceCloseReason))
            {
                MessageBox.Show(null, "Launcher Ecountered an Error and It Must Close. Below is a Summary of the Error" +
                "\n" + LauncherForceCloseReason, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (Boolen)
            {
                Application.Restart();
            }
            else
            {
                Application.Exit();
            }
        }

        public static void FirstTimeRun()
        {
            LoadingComplete = true;

            if (!string.IsNullOrWhiteSpace(FileSettingsSave.GameInstallation))
            {
                Log.Core("LAUNCHER: Checking InstallationDirectory: " + FileSettingsSave.GameInstallation);
            }

            Log.Checking("LAUNCHER: Checking Game Installation");
            if (string.IsNullOrWhiteSpace(FileSettingsSave.GameInstallation))
            {
                DiscordLauncherPresense.Status("Start Up", "Doing First Time Run");
                Log.Core("LAUNCHER: First run!");

                try
                {
                    Form welcome = new WelcomeScreen();
                    DialogResult welcomereply = welcome.ShowDialog();

                    if (welcomereply != DialogResult.OK)
                    {
                        LauncherForceClose = true;
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
                    Log.Core("LAUNCHER: Installation Directory was Empty! Creating and Setting Directory at " + 
                        Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory)) + "\\Game Files");
                    FileSettingsSave.GameInstallation = Encoding.UTF8.GetString(
                        Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory)) + "\\Game Files";
                    FileSettingsSave.SaveSettings();
                }

                if (LauncherForceClose)
                {
                    ErrorCloseLauncher("Closing From Welcome Dialog", false);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(FileSettingsSave.GameInstallation))
                    {
                        DiscordLauncherPresense.Status("Start Up", "User Selecting/Inputting Game Files Folder");

                        try
                        {
                            if (!DetectLinux.LinuxDetected())
                            {
                                string GameFolderPath = string.Empty;

                                CommonOpenFileDialog FolderDialog = new CommonOpenFileDialog
                                {
                                    EnsurePathExists = true,
                                    EnsureFileExists = false,
                                    AllowNonFileSystemItems = false,
                                    Title = "Select the location to Find or Download NFS:W",
                                    IsFolderPicker = true
                                };

                                if (FolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                                {
                                    if (!string.IsNullOrWhiteSpace(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(FolderDialog.FileName))))
                                    {
                                        GameFolderPath = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(FolderDialog.FileName));
                                    }
                                }

                                FolderDialog.Dispose();

                                if (!string.IsNullOrWhiteSpace(GameFolderPath))
                                {
                                    DiscordLauncherPresense.Status("Start Up", "Verifying Game Files Folder Location");

                                    if (!HasWriteAccessToFolder(GameFolderPath))
                                    {
                                        Log.Error("LAUNCHER: Not enough permissions. Exiting.");
                                        string ErrorMessage = "You don't have enough permission to select this path as installation folder. Please select another directory.";
                                        MessageBox.Show(null, ErrorMessage, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        LauncherForceClose = true;
                                        LauncherForceCloseReason = ErrorMessage;
                                    }
                                    else
                                    {
                                        if (GameFolderPath.Length == 3)
                                        {
                                            Directory.CreateDirectory("Game Files");
                                            Log.Warning("LAUNCHER: Installing NFSW in root of the harddisk is not allowed.");
                                            MessageBox.Show(null, string.Format("Installing NFSW in root of the harddisk is not allowed. " +
                                                "Instead, we will install it on {0}.", Encoding.UTF8.GetString(
                                                    Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory))
                                                + "\\Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            FileSettingsSave.GameInstallation = Encoding.UTF8.GetString(
                                                Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory)) 
                                                + "\\Game Files";
                                            FileSettingsSave.SaveSettings();
                                            FileGameSettings.Save("Suppress", "Language Only");
                                        }
                                        else if (GameFolderPath == AppDomain.CurrentDomain.BaseDirectory)
                                        {
                                            Directory.CreateDirectory("Game Files");
                                            Log.Warning("LAUNCHER: Installing NFSW in same directory where the launcher resides is disadvised.");
                                            MessageBox.Show(null, string.Format("Installing NFSW in same directory where the launcher resides is disadvised. " +
                                                "Instead, we will install it on {0}.", Encoding.UTF8.GetString(
                                                    Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory))
                                                + "\\Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            FileSettingsSave.GameInstallation = Encoding.UTF8.GetString(
                                                Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory))
                                                + "\\Game Files";
                                            FileSettingsSave.SaveSettings();
                                            FileGameSettings.Save("Suppress", "Language Only");
                                        }
                                        else
                                        {
                                            Log.Core("LAUNCHER: Directory Set: " + GameFolderPath);
                                            FileSettingsSave.GameInstallation = GameFolderPath;
                                            FileSettingsSave.SaveSettings();
                                            FileGameSettings.Save("Suppress", "Language Only");
                                        }
                                    }
                                }
                                else
                                {
                                    LauncherForceClose = true;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(FileSettingsSave.GameInstallation))
                                {
                                    try
                                    {
                                        FileSettingsSave.GameInstallation = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Path.GetFullPath("GameFiles")));
                                    }
                                    catch
                                    {
                                        FileSettingsSave.GameInstallation = "GameFiles";
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(FileSettingsSave.GameInstallation))
                            {
                                if (!Directory.Exists(FileSettingsSave.GameInstallation))
                                {
                                    Log.Core("LAUNCHER: Created Game Files Directory: " + FileSettingsSave.GameInstallation);
                                    Directory.CreateDirectory(FileSettingsSave.GameInstallation);
                                }
                            }
                        }
                        catch (Exception Error)
                        {
                            LauncherForceClose = true;
                            LauncherForceCloseReason = Error.Message;
                            Log.Error("LAUNCHER: Folder Select Dialog -> " + LauncherForceCloseReason);
                        }
                    }
                }
            }
            Log.Completed("LAUNCHER: Game Installation Path Done");

            if (LauncherForceClose)
            {
                ErrorCloseLauncher("Closing From Folder Dialog", false);
            }
            else
            {
                if (!DetectLinux.LinuxDetected())
                {
                    Log.Checking("LAUNCHER: Checking Game Path Location");
                    DiscordLauncherPresense.Status("Start Up", "Checking Game Files Folder Location");

                    switch (CheckFolder(FileSettingsSave.GameInstallation))
                    {
                        case FolderType.IsSameAsLauncherFolder:
                            Directory.CreateDirectory("Game Files");
                            Log.Error("LAUNCHER: Installing NFSW in same location where the GameLauncher resides is NOT allowed.");
                            MessageBox.Show(null, string.Format("Installing NFSW in same location where the GameLauncher resides is NOT allowed.\n" +
                                "Instead, we will install it at {0}.", 
                                Encoding.UTF8.GetString(
                                    Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory)) + "Game Files"), 
                                "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FileSettingsSave.GameInstallation = Encoding.UTF8.GetString(
                                Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory)) + "\\Game Files";
                            break;

                        case FolderType.IsTempFolder:
                        case FolderType.IsUsersFolders:
                        case FolderType.IsProgramFilesFolder:
                        case FolderType.IsWindowsFolder:
                        case FolderType.IsRootFolder:
                            String constructMsg = String.Empty;
                            Directory.CreateDirectory("Game Files");
                            constructMsg += "Using this location for Game Files is not allowed.\n\n";
                            constructMsg += "The following locations are also NOT allowed:\n";
                            constructMsg += "• X:\\nfsw.exe (Root of Drive, such as C:\\ or D:\\, must be in a folder)\n";
                            constructMsg += "• C:\\Program Files\n";
                            constructMsg += "• C:\\Program Files (x86)\n";
                            constructMsg += "• C:\\Users (Includes 'Desktop', 'Documents', 'Downloads')\n";
                            constructMsg += "• C:\\Windows\n\n";
                            constructMsg += "Instead, we will install the NFSW Game at " + 
                                Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory)) + "\\Game Files\n";

                            MessageBox.Show(null, constructMsg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Log.Error("LAUNCHER: Installing NFSW in a Restricted Location is not allowed.");
                            FileSettingsSave.GameInstallation = Encoding.UTF8.GetString
                                (Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory)) + "\\Game Files";
                            break;
                    }
                    FileSettingsSave.SaveSettings();
                    Log.Completed("LAUNCHER: Done Checking Game Path Location");

                    Log.Checking("LAUNCHER: Windows Defender (If Applicable)");
                    /* Windows Defender (Windows 10) */
                    if (WindowsProductVersion.CachedWindowsNumber >= 10.0 && 
                        (FileSettingsSave.WindowsDefenderStatus == "Not Excluded" || FileSettingsSave.WindowsDefenderStatus == "Unknown"))
                    {
                        Log.Core("WINDOWS DEFENDER: Windows 10 Detected! Running Exclusions for Core Folders");
                        WindowsDefender("Add-Launcher", "Complete Exclude", Encoding.UTF8.GetString(
                            Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory))
                            , FileSettingsSave.GameInstallation, "Launcher");
                    }
                    else if (WindowsProductVersion.CachedWindowsNumber >= 10.0 && !string.IsNullOrWhiteSpace(FileSettingsSave.WindowsDefenderStatus))
                    {
                        Log.Core("WINDOWS DEFENDER: Found 'WindowsDefender' key! Its value is " + FileSettingsSave.WindowsDefenderStatus);
                    }
                    Log.Completed("LAUNCHER: Windows Defender (If Applicable) Done");
                }

                /* Check If Launcher Failed to Connect to any APIs */
                if (!VisualsAPIChecker.WOPLAPI)
                {
                    DiscordLauncherPresense.Status("Start Up", "Launcher Encountered API Errors");

                    DialogResult restartAppNoApis = MessageBox.Show(null, "There's no internet connection, Launcher might crash." +
                        "\n\nClick Yes to Close Launcher" +
                        "\nor" +
                        "\nClick No Continue", "GameLauncher has Stopped, Failed To Connect To API", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (restartAppNoApis == DialogResult.No)
                    {
                        MessageBox.Show("Good Luck... \n No Really \n ...Good Luck", "GameLauncher Will Continue, When It Failed To Connect To API");
                        Log.Warning("PRE-CHECK: User has Bypassed 'No Internet Connection' Check and Will Continue");
                    }

                    if (restartAppNoApis == DialogResult.Yes)
                    {
                        LauncherForceClose = true;
                    }
                }

                if (LauncherForceClose)
                {
                    ErrorCloseLauncher("Closing From API Error", false);
                }
                else
                {
                    Log.Info("MAINSCREEN: Program Started");
                    Application.Run(new MainScreen());
                }
            }
        }

        public static void WindowsDefender(string Type, string Mode, string Path, string SecondPath, string Notes)
        {
            DiscordLauncherPresense.Status("Start Up", "Checking Windows Security (Defender) Exclusions");

            try
            {
                if (ManagementSearcher.SecurityCenter("AntivirusEnabled") == true && ManagementSearcher.SecurityCenter("AntispywareEnabled") == true)
                {
                    /* Create Windows Defender Exclusion */
                    try
                    {
                        if (Type == "Add-Launcher" || Type == "Add-Game")
                        {
                            if (Mode == "Complete Exclude")
                            {
                                /* Add Exclusion to Windows Defender */
                                using (PowerShell ps = PowerShell.Create())
                                {
                                    ps.AddScript($"Add-MpPreference -ExclusionPath \"{Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Path))}\"");
                                    if (Directory.Exists(SecondPath))
                                    {
                                        ps.AddScript($"Add-MpPreference -ExclusionPath \"{Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(SecondPath))}\"");
                                    }
                                    var result = ps.Invoke();
                                }
                            }
                            else
                            {
                                Log.Warning("WINDOWS DEFENDER: Unknown Function Call in 'WindowsDefender' with Add Sub-Section. Please Check Code in Visual Studio");
                            }

                            FileSettingsSave.WindowsDefenderStatus = "Excluded";

                            Log.Info("WINDOWS DEFENDER: Excluded " + Notes + " Folder");
                        }
                        else if (Type == "Reset-Launcher" || Type == "Reset-Game")
                        {
                            if (Mode == "Complete Reset")
                            {
                                /* Add Exclusion to Windows Defender */
                                using (PowerShell ps = PowerShell.Create())
                                {
                                    ps.AddScript($"Remove-MpPreference -ExclusionPath \"{Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Path))}\"");
                                    ps.AddScript($"Remove-MpPreference -ExclusionPath \"{Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(SecondPath))}\"");
                                    var result = ps.Invoke();
                                }

                                FileSettingsSave.WindowsDefenderStatus = "Not Excluded";
                            }
                            else if (Mode == "Update Reset")
                            {
                                /* Remove current Exclusion and Add new location for Exclusion (Game Files Only!) */
                                using (PowerShell ps = PowerShell.Create())
                                {
                                    ps.AddScript($"Remove-MpPreference -ExclusionPath \"{Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Path))}\"");
                                    if (Directory.Exists(SecondPath))
                                    {
                                        ps.AddScript($"Add-MpPreference -ExclusionPath \"{Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(SecondPath))}\"");
                                    }
                                    var result = ps.Invoke();
                                }                                
                            }
                            else
                            {
                                Log.Warning("WINDOWS DEFENDER: Unknown Function Call in 'WindowsDefender' with Reset Sub-Section. Please Check Code in Visual Studio");
                            }

                            Log.Warning("WINDOWS DEFENDER: " + Notes + " Folders");
                        }
                        else
                        {
                            Log.Warning("WINDOWS DEFENDER: Unknown Function Call in 'WindowsDefender'. Please Check Code in Visual Studio");
                        }
                        
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
            catch (Exception ex)
            {
                Log.Error("WINDOWS DEFENDER: " + ex.Message);
                FileSettingsSave.WindowsDefenderStatus = "Not Supported";
                FileSettingsSave.SaveSettings();
            }
        }

        /* Moved "runAsAdmin" Code to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/eec2f9f80aa4b350ab98d32383e1ee1f2e1c26fd/Self.cs */
    }
}