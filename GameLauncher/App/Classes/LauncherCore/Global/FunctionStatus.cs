using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.LauncherUpdater;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using GameLauncher.App.UI_Forms.Main_Screen;
using GameLauncher.App.UI_Forms.Splash_Screen;
using GameLauncher.App.UI_Forms.Welcome_Screen;
using Microsoft.WindowsAPICodePack.Dialogs;
using SBRW.Launcher.Core.Classes.Extension.Logging_;
using SBRW.Launcher.Core.Classes.Reference.Json_.Newtonsoft_;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Global
{
    /* This is Used to Cache Responses From the Launcher */
    class InformationCache
    {
        /* Detect and Set System Language */
        public static CultureInfo Lang = Thread.CurrentThread.CurrentUICulture;

        /* Parent Screen Cords */
        public static Point ParentScreenLocation;

        /* Selected Server Is Enforcing Proxy */
        public static bool SelectedServerEnforceProxy = false;

        /* Selected Server Category */
        public static string SelectedServerCategory;

        /* Selected Server List Key Information */
        public static Json_List_Server SelectedServerData;

        /* Selected Server JSON (GetServerInformation) */
        public static Json_Server_Info SelectedServerJSON = new Json_Server_Info();

        /* Holds a collection of Server Status of Servers */
        public static Dictionary<string, int> ServerStatusBook = new Dictionary<string, int>();
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
        public static bool CanCloseGame = true;

        /* Sets Conditional if Game was Closed (By Timer) */
        public static bool GameKilledBySpeedBugCheck = false;

        /* Prevents Launcher from bring Closed when Game is Loading */
        public static bool LauncherBattlePass = false;

        /* Checks if we have Write Permissions */
        public static bool HasWriteAccessToFolder(string path)
        {
            try
            {
                Log.Checking("WRITE TEST: Folder Write Test");
                File.Create(Path.Combine(path, "temp.txt")).Close();
                File.Delete(Path.Combine(path, "temp.txt"));
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

        public static void CenterParent(Form Screen)
        {
            Screen.StartPosition = FormStartPosition.Manual;
            Screen.Location = InformationCache.ParentScreenLocation;
        }

        /* Check if Folder Location is Acceptable and Returns a Value
        /* Let's actually make it cleaner and nicer - MeTonaTOR */
        public static FolderType CheckFolder(string FolderName)
        {
            if (FolderName.Contains(Path.Combine("C:", "Users")) && FolderName.Contains("Temp")) { return FolderType.IsTempFolder; }
            else if (FolderName.Contains(Path.Combine("C:", "Users"))) { return FolderType.IsUsersFolders; }
            else if (FolderName.Contains(Path.Combine("C:", "Program Files"))) { return FolderType.IsProgramFilesFolder; }
            else if (FolderName.Contains(Path.Combine("C:", "Windows"))) { return FolderType.IsWindowsFolder; }
            else if (FolderName.Length == 3) { return FolderType.IsRootFolder; }
            else if (Path.Combine(FolderName, "\\") == Locations.LauncherFolder ||
                FolderName == Locations.LauncherFolder) { return FolderType.IsSameAsLauncherFolder; }
            else
            {
                return FolderType.Unknown;
            }
        }

        /* Converts Host Name to a IP (ex. http://localhost -> 192.168.1.69 */
        public static string HostName2IP(string hostname)
        {
            IPHostEntry iphost = Dns.GetHostEntry(hostname);
            IPAddress[] addresses = iphost.AddressList;
            return addresses[0].ToString();
        }

        /// <summary>
        /// Used to Force Close Launcher when Launcher encounters an error during Startup
        /// </summary>
        /// <param name="Notes">Required: Where the Launcher is Closing From</param>
        /// <param name="Boolen">True: Restarts Launcher | False: Closes Launcher</param>
        public static void ErrorCloseLauncher(string Notes, bool Boolen)
        {
            SplashScreen.ThreadStatus("Stop");

            if (DiscordLauncherPresence.Running())
            {
                DiscordLauncherPresence.Stop("Close");
            }

            if (ServerProxy.Running())
            {
                ServerProxy.Instance.Stop("Force Close");
            }

            Log.Warning("LAUNCHER: Exiting (" + Notes + ")");
            if (!string.IsNullOrWhiteSpace(LauncherForceCloseReason))
            {
                DialogResult OpenLogFile = MessageBox.Show(null, "The GameLauncher has ecountered an Error and it must Close. " +
                    "Below is a Summary of the Error:" + "\n" + LauncherForceCloseReason + "\n\n" +
                    LogToFileAddons.OpenLogMessage, "GameLauncher",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (OpenLogFile == DialogResult.Yes)
                {
                    Process.Start(Log_Location.LogCurrentFolder);
                    Process.Start(Log_Location.LogLauncher);
                }
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
            if (!LauncherUpdateCheck.UpdatePopupStoppedSplashScreen)
            {
                LoadingComplete = true;
                SplashScreen.ThreadStatus("Stop");
            }

            if (!string.IsNullOrWhiteSpace(FileSettingsSave.Live_Data.Game_Path))
            {
                Log.Core("LAUNCHER: Checking InstallationDirectory: " + FileSettingsSave.Live_Data.Game_Path);
            }

            Log.Checking("LAUNCHER: Checking Game Installation");
            if (string.IsNullOrWhiteSpace(FileSettingsSave.Live_Data.Game_Path))
            {
                DiscordLauncherPresence.Status("Start Up", "Doing First Time Run");
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
                        FileSettingsSave.Live_Data.Launcher_CDN = SelectedCDN.CDNUrl;
                        FileSettingsSave.SaveSettings();
                    }
                }
                catch
                {
                    Log.Warning("LAUNCHER: CDN Source URL was Empty! Setting a Null Safe URL 'http://localhost'");
                    FileSettingsSave.Live_Data.Launcher_CDN = "http://localhost";
                    Log.Core("LAUNCHER: Installation Directory was Empty! Creating and Setting Directory at " + Locations.GameFilesFailSafePath);
                    FileSettingsSave.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                    FileSettingsSave.SaveSettings();
                }

                if (LauncherForceClose)
                {
                    ErrorCloseLauncher("Closing From Welcome Dialog", false);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(FileSettingsSave.Live_Data.Game_Path))
                    {
                        DiscordLauncherPresence.Status("Start Up", "User Selecting/Inputting Game Files Folder");

                        try
                        {
                            if (!UnixOS.Detected())
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
                                    if (!string.IsNullOrWhiteSpace(FolderDialog.FileName))
                                    {
                                        GameFolderPath = FolderDialog.FileName;
                                    }
                                }

                                FolderDialog.Dispose();

                                if (!string.IsNullOrWhiteSpace(GameFolderPath))
                                {
                                    DiscordLauncherPresence.Status("Start Up", "Verifying Game Files Folder Location");

                                    if (!HasWriteAccessToFolder(GameFolderPath))
                                    {
                                        Log.Error("FOLDER SELECT DIALOG: Not enough permissions. Exiting.");
                                        string ErrorMessage = "You don't have enough permission to select this path as the Installation folder. " +
                                            "Please select another directory.";
                                        MessageBox.Show(null, ErrorMessage, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        LauncherForceClose = true;
                                        LauncherForceCloseReason = ErrorMessage;
                                    }
                                    else
                                    {
                                        if (GameFolderPath.Length == 3)
                                        {
                                            Directory.CreateDirectory("Game Files");
                                            Log.Warning("FOLDER SELECT DIALOG: Installing NFSW in root of the harddisk is not allowed.");
                                            MessageBox.Show(null, string.Format("Installing NFSW in root of the harddisk is not allowed. " +
                                                "Instead, we will install it on {0}.", Locations.GameFilesFailSafePath),
                                                "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            FileSettingsSave.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                                            FileSettingsSave.SaveSettings();
                                            FileGameSettings.Save("Suppress", "Language Only");
                                        }
                                        else if (GameFolderPath == Locations.LauncherFolder)
                                        {
                                            Directory.CreateDirectory("Game Files");
                                            Log.Warning("FOLDER SELECT DIALOG: Installing NFSW in same location where the GameLauncher resides is NOT allowed.");
                                            MessageBox.Show(null, string.Format("Installing NFSW in same location where the GameLauncher resides is NOT allowed.\n " +
                                                "Instead, we will install it on {0}.", Locations.GameFilesFailSafePath),
                                                "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            FileSettingsSave.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                                            FileSettingsSave.SaveSettings();
                                            FileGameSettings.Save("Suppress", "Language Only");
                                        }
                                        else
                                        {
                                            Log.Core("FOLDER SELECT DIALOG: Directory Set: " + GameFolderPath);
                                            FileSettingsSave.Live_Data.Game_Path = GameFolderPath;
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
                                if (string.IsNullOrWhiteSpace(FileSettingsSave.Live_Data.Game_Path))
                                {
                                    try
                                    {
                                        FileSettingsSave.Live_Data.Game_Path = Path.GetFullPath("GameFiles");
                                    }
                                    catch
                                    {
                                        FileSettingsSave.Live_Data.Game_Path = "GameFiles";
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(FileSettingsSave.Live_Data.Game_Path))
                            {
                                if (!Directory.Exists(FileSettingsSave.Live_Data.Game_Path))
                                {
                                    Log.Core("FOLDER SELECT DIALOG: Created Game Files Directory: " + FileSettingsSave.Live_Data.Game_Path);
                                    Directory.CreateDirectory(FileSettingsSave.Live_Data.Game_Path);
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(FileSettingsSave.Live_Data.Game_Path))
                            {
                                Log.Checking("CLEANLINKS: Game Path");
                                if (File.Exists(Path.Combine(FileSettingsSave.Live_Data.Game_Path, Locations.NameModLinks)))
                                {
                                    ModNetHandler.CleanLinks(FileSettingsSave.Live_Data.Game_Path);
                                    Log.Completed("CLEANLINKS: Done");
                                }
                                else
                                {
                                    Log.Completed("CLEANLINKS: Not Present");
                                }
                            }
                        }
                        catch (Exception Error)
                        {
                            LauncherForceClose = true;
                            LauncherForceCloseReason = Error.Message;
                            LogToFileAddons.OpenLog("FOLDER SELECT DIALOG", null, Error, null, true);
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
                if (!UnixOS.Detected())
                {
                    Log.Checking("LAUNCHER: Checking Game Path Location");
                    DiscordLauncherPresence.Status("Start Up", "Checking Game Files Folder Location");

                    switch (CheckFolder(FileSettingsSave.Live_Data.Game_Path))
                    {
                        case FolderType.IsSameAsLauncherFolder:
                            Directory.CreateDirectory("Game Files");
                            Log.Error("LAUNCHER: Installing NFSW in same location where the GameLauncher resides is NOT allowed.");
                            MessageBox.Show(null, string.Format("Installing NFSW in same location where the GameLauncher resides is NOT allowed.\n" +
                                "Instead, we will install it at {0}.", Locations.GameFilesFailSafePath),
                                "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FileSettingsSave.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                            break;
                        case FolderType.IsTempFolder:
                        case FolderType.IsUsersFolders:
                        case FolderType.IsProgramFilesFolder:
                        case FolderType.IsWindowsFolder:
                        case FolderType.IsRootFolder:
                            String constructMsg = String.Empty;
                            constructMsg += "Using this location for Game Files is not allowed.\n\n";
                            constructMsg += "The following locations are also NOT allowed:\n";
                            constructMsg += "• X:\\nfsw.exe (Root of Drive, such as C:\\ or D:\\, must be in a folder)\n";
                            constructMsg += "• C:\\Program Files\n";
                            constructMsg += "• C:\\Program Files (x86)\n";
                            constructMsg += "• C:\\Users (Includes 'Desktop', 'Documents', 'Downloads')\n";
                            constructMsg += "• C:\\Windows\n\n";
                            constructMsg += "Instead, we will install the NFSW Game at " + Locations.GameFilesFailSafePath;
                            try
                            {
                                if (!Directory.Exists(Locations.GameFilesFailSafePath))
                                {
                                    Log.Core("FOLDER SELECT DIALOG: Created Game Files Directory: " + Locations.GameFilesFailSafePath);
                                    Directory.CreateDirectory(Locations.GameFilesFailSafePath);
                                }
                            }
                            catch { }
                            MessageBox.Show(null, constructMsg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Log.Error("LAUNCHER: Installing NFSW in a Restricted Location is not allowed.");
                            FileSettingsSave.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                            break;
                    }
                    FileSettingsSave.SaveSettings();
                    Log.Completed("LAUNCHER: Done Checking Game Path Location");
                }

                /* Check If Launcher Failed to Connect to any APIs */
                if (!VisualsAPIChecker.CarbonAPITwo())
                {
                    DiscordLauncherPresence.Status("Start Up", "Launcher Encountered API Errors");

                    DialogResult restartAppNoApis = MessageBox.Show(null, "There is no internet connection, Launcher might crash." +
                        "\n\nClick Yes to Close GameLauncher" +
                        "\nor" +
                        "\nClick No Continue", "GameLauncher has Stopped, Failed To Connect To API", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

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
                    try
                    {
                        Log.Info("MAINSCREEN: Program Started");
                        Application.Run(new MainScreen());
                    }
                    catch (COMException Error)
                    {
                        LogToFileAddons.OpenLog("Main Screen [Application Run]", "Launcher Encounterd an Error.", Error, "Error", false);
                        ErrorCloseLauncher("Main Screen [Application Run]", false);
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("Main Screen [Application Run]", "Launcher Encounterd an Error.", Error, "Error", false);
                        ErrorCloseLauncher("Main Screen [Application Run]", false);
                    }
                }
            }
        }
        /* Moved "runAsAdmin" Code to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/eec2f9f80aa4b350ab98d32383e1ee1f2e1c26fd/Self.cs */
    }
}