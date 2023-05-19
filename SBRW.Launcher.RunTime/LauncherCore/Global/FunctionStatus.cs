using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Proxy.Nancy_;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using SBRW.Launcher.RunTime.InsiderKit;
using System.Linq;
using System.Net.Sockets;
using SBRW.Launcher.App.UI_Forms;
using SBRW.Launcher.Core.Extra.XML_;
using System.Threading.Tasks;

namespace SBRW.Launcher.RunTime.LauncherCore.Global
{
    /* This is Used to Cache Responses From the Launcher */
    class InformationCache
    {
        /* Detect and Set System Language */
        public static CultureInfo Lang { get; set; } = Thread.CurrentThread.CurrentUICulture;
        /* Parent Screen Cords */
        public static Point ParentScreenLocation { get; set; }
        /* Selected Server Is Enforcing Proxy */
        public static bool SelectedServerEnforceProxy { get; set; }
        /* Holds a collection of Server Status of Servers */
        public static Dictionary<string, int> ServerStatusBook { get; set; } = new Dictionary<string, int>();
        public static bool DisableProxy() => Save_Settings.Live_Data.Launcher_Proxy == "1";
        public static bool DisableDiscordRPC() => Save_Settings.Live_Data.Launcher_Discord_Presence == "1";
        public static bool DisableFrequencyJSONUpdate() => Save_Settings.Live_Data.Launcher_JSON_Frequency_Update_Cache == "1";
        public static bool EnableAltWebCalls() => Save_Settings.Live_Data.Launcher_WebClient_Method == "WebClientWithTimeout";
        public static bool EnableInsiderPreview() => (Save_Settings.Live_Data.Launcher_Insider == "1" || Save_Settings.Live_Data.Launcher_Insider == "2");
        public static bool EnableThemeSupport() => Save_Settings.Live_Data.Launcher_Theme_Support == "1";
        public static bool EnableLZMADownloader() => Save_Settings.Live_Data.Launcher_LZMA_Downloader == "1";
        public static bool EnableProxyDomain() => Save_Settings.Live_Data.Launcher_Proxy_Domain == "1";
        /// <summary>
        /// Default Path Location for Game Files Archive File
        /// </summary>
        /// <remarks>Example: C:\Soapbox Race World\Game Files\.Launcher\Downloads\GameFiles.sbrwpack</remarks>
        public static string Default_Game_Archive_Path() => Path.Combine(Save_Settings.Live_Data.Game_Path, ".Launcher", "Downloads", "GameFiles.sbrwpack");
        /// <summary>
        /// Secondary Path Location for Game Files Archive File
        /// </summary>
        /// <remarks>Example: C:\Soapbox Race World\Launcher\Launcher_Data\Archive\Game Files\GameFiles.sbrwpack</remarks>
        public static string Secondary_Game_Archive_Path() => Path.Combine(Locations.LauncherFolder, "Launcher_Data", "Archive", "GameFiles.sbrwpack");
        /// <summary>
        /// Secondary Path Location for Game Files Archive File
        /// </summary>
        /// <remarks>Example: C:\Soapbox Race World\Launcher\Launcher_Data\Archive\Game Files\GameFiles.sbrwpack</remarks>
        public static string Secondary_Game_Archive_Path_Old() => Path.Combine(Locations.LauncherFolder, "Launcher_Data", "Archive", "Game Files", "GameFiles.sbrwpack");
        /// <summary>
        /// Legacy File path that existed for launchers 2.1.4.X - 2.1.5.X
        /// </summary>
        /// <remarks>Example: C:\Soapbox Race World\Launcher\GameFiles.sbrwpack</remarks>
        public static string Legacy_Game_Archive_Path() => Path.Combine(Locations.LauncherFolder, "GameFiles.sbrwpack");
    }

    /* This is Used to call Certain Functions (Such as Completion Status or Function Callbacks) */
    static class FunctionStatus
    {
        /* Launcher had Encounterd an Error and It Must Close */
        public static bool LauncherForceClose { get; set; }
        /* Launcher had Encounterd an Error and It Reason*/
#pragma warning disable CS8618
        public static string LauncherForceCloseReason { get; set; }
#pragma warning restore CS8618
        /* Updater.cs Sets Conditional on If Launcher had Finished Loading (It Self) */
        public static bool LoadingComplete { get; set; }
        /* Allows Registration Button to be Enabled/Disabled */
        public static bool AllowRegistration { get; set; }
        /* If Verify Hash (.dat) File Exists on Server */
        public static bool DoesCDNSupportVerifyHash { get; set; }
        /* Verify Hash Status */
        public static bool IsVerifyHashDisabled { get; set; }
        /* Visual API Status */
        public static bool IsVisualAPIsChecked { get; set; }
        /* Prevents Launcher from bring Closed when Game is Loading */
        public static bool LauncherBattlePass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Game_Location_Path"></param>
        /// <param name="Check_Mode"></param>
        /// <returns></returns>
        public static bool IsRestrictedGameFolderLocation(this string Game_Location_Path, int Check_Mode)
        {
            return Game_Location_Path.IsRestrictedGameFolderLocation(Check_Mode, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Game_Location_Path"></param>
        /// <param name="Check_Mode"></param>
        /// <param name="Form_Window_Handle"></param>
        /// <returns></returns>
        public static bool IsRestrictedGameFolderLocation(this string Game_Location_Path, int Check_Mode, IWin32Window? Form_Window_Handle)
        {
            if(Check_Mode == 0)
            {
                Presence_Launcher.Status(0, "Verifying Game Files Folder Location");
            }

            Log.Debug(Locations.LauncherFolder);

            int Write_Test = HasWriteAccessToFolder(Game_Location_Path);

            if (Write_Test <= 0)
            {
                if (Write_Test == 0)
                {
                    LogToFileAddons.Parent_Log_Screen(5, "FOLDER FUNCTION CHECK", "Not enough permissions.");
                    string ErrorMessage = "You don't have enough permission to select this path as the Installation folder. " +
                        "Please select another directory by manually setting a new path.";
                    MessageBox.Show(Form_Window_Handle, ErrorMessage, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (Check_Mode == 0)
                    {
                        LauncherForceClose = true;
                        LauncherForceCloseReason = ErrorMessage;
                    }
                }

                return true;
            }
            else
            {
                switch (CheckFolder(Game_Location_Path))
                {
                    case FolderType.IsRootFolder:
                        LogToFileAddons.Parent_Log_Screen(4, "FOLDER FUNCTION CHECK", "Installing NFSW in root of the harddisk is not allowed.");
                        MessageBox.Show(Form_Window_Handle, string.Format("Installing NFSW in root of the harddisk is not allowed. " +
                            "Instead, we will install it on {0}.", Locations.GameFilesFailSafePath),
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                        if (Check_Mode == 0)
                        {
                            Save_Settings.Save();
                            XML_File.Save(1);
                        }

                        return Save_Settings.Live_Data.Game_Path.IsRestrictedGameFolderLocation(Check_Mode, Form_Window_Handle);
                    case FolderType.IsSameAsLauncherFolder:
                        LogToFileAddons.Parent_Log_Screen(4, "FOLDER FUNCTION CHECK", "Installing NFSW in same location where the GameLauncher resides is NOT allowed.");
                        MessageBox.Show(Form_Window_Handle, string.Format("Installing NFSW in same location where the GameLauncher resides is NOT allowed.\n " +
                            "Instead, we will install it on {0}.", Locations.GameFilesFailSafePath),
                            "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                        if (Check_Mode == 0)
                        {
                            Save_Settings.Save();
                            XML_File.Save(1);
                        }

                        return Save_Settings.Live_Data.Game_Path.IsRestrictedGameFolderLocation(Check_Mode, Form_Window_Handle);
                    default:
                        LogToFileAddons.Parent_Log_Screen(11, "FOLDER FUNCTION CHECK", "Directory Set at " + Game_Location_Path);
                        Save_Settings.Live_Data.Game_Path = Game_Location_Path;

                        if (Check_Mode == 0)
                        {
                            Save_Settings.Save();
                            XML_File.Save(1);
                        }

                        if (!Directory.Exists(Save_Settings.Live_Data.Game_Path))
                        {
                            Directory.CreateDirectory(Save_Settings.Live_Data.Game_Path);

                            if (Check_Mode == 0)
                            {
                                LogToFileAddons.Parent_Log_Screen(11, "FOLDER FUNCTION CHECK", "Created Game Files Directory at " + Save_Settings.Live_Data.Game_Path);
                            }
                        }
                        break;
                }
            }

            return false;
        }
        /// <summary>
        /// Checks if we have Write Permissions
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int HasWriteAccessToFolder(string path)
        {
            if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
            {
                Log.Info("WRITE TEST: Folder Path [" + path + "]");
            }

            try
            {
                if (!Directory.Exists(path))
                {
                    Log.Checking("WRITE TEST: Folder [CREATE]");
                    Directory.CreateDirectory(path);
                    Log.Checking("WRITE TEST: Folder [SUCCESS]");
                }
            }
            catch
            {
                return -1;
            }

            try
            {
                Log.Checking("WRITE TEST: File [CREATE & REMOVE]");
                File.Create(Path.Combine(path, "temp.txt")).Close();
                File.Delete(Path.Combine(path, "temp.txt"));
                Log.Checking("WRITE TEST: File [SUCCESS]");
            }
            catch
            {
                return 0;
            }

            return 1;
        }

        /* Used to Center WinForms Forms (Parent Screen) */
        public static void CenterScreen(Form Live_Form)
        {
            Live_Form.StartPosition = FormStartPosition.Manual;
            Live_Form.Top = (Screen.PrimaryScreen.Bounds.Height - Live_Form.Height) / 2;
            Live_Form.Left = (Screen.PrimaryScreen.Bounds.Width - Live_Form.Width) / 2;

            InformationCache.ParentScreenLocation = Live_Form.Location;
        }

        public static void CenterParent(Form Live_Form)
        {
            if (!InformationCache.ParentScreenLocation.IsEmpty)
            {
                Live_Form.StartPosition = FormStartPosition.Manual;
                Live_Form.Location = InformationCache.ParentScreenLocation;
            }
            else
            {
                Live_Form.StartPosition = FormStartPosition.CenterScreen;
            }
        }
#if NETFRAMEWORK
        public static bool Beta_Contains(this string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }
#endif

        /* Check if Folder Location is Acceptable and Returns a Value
        /* Let's actually make it cleaner and nicer - MeTonaTOR */
        public static FolderType CheckFolder(string FolderName)
        {
            if (string.IsNullOrWhiteSpace(FolderName))
            {
                return FolderType.Invalid;
            }
            else
            {
#if NETFRAMEWORK
                if (FolderName.Beta_Contains(@"C:\Users", StringComparison.OrdinalIgnoreCase) && FolderName.Beta_Contains("Temp", StringComparison.OrdinalIgnoreCase))
                {
                    return FolderType.IsTempFolder;
                }
                else if (FolderName.Beta_Contains(@"C:\Users", StringComparison.OrdinalIgnoreCase))
                {
                    return FolderType.IsUsersFolders;
                }
                else if (FolderName.Beta_Contains(@"C:\Program Files", StringComparison.OrdinalIgnoreCase))
                {
                    return FolderType.IsProgramFilesFolder;
                }
                else if (FolderName.Beta_Contains(@"C:\Windows", StringComparison.OrdinalIgnoreCase))
                {
                    return FolderType.IsWindowsFolder;
                }
#else
                if (FolderName.Contains(@"C:\Users", StringComparison.OrdinalIgnoreCase) && FolderName.Contains("Temp", StringComparison.OrdinalIgnoreCase))
                {
                    return FolderType.IsTempFolder;
                }
                else if (FolderName.Contains(@"C:\Users", StringComparison.OrdinalIgnoreCase))
                {
                    return FolderType.IsUsersFolders;
                }
                else if (FolderName.Contains(@"C:\Program Files", StringComparison.OrdinalIgnoreCase))
                {
                    return FolderType.IsProgramFilesFolder;
                }
                else if (FolderName.Contains(@"C:\Windows", StringComparison.OrdinalIgnoreCase))
                {
                    return FolderType.IsWindowsFolder;
                }
#endif
                else if (FolderName.Length == 3) 
                {
                    return FolderType.IsRootFolder;
                }
                else if (Locations.LauncherFolder.Equals(FolderName, StringComparison.OrdinalIgnoreCase) ||
                    Locations.LauncherFolder.Equals(FolderName + @"\", StringComparison.OrdinalIgnoreCase))
                {
                    return FolderType.IsSameAsLauncherFolder;
                }
                else
                {
                    return FolderType.Unknown;
                }
            }
        }

        /* Converts Host Name to a IP (ex. http://localhost -> 192.168.1.69 */
        /// <summary>
        /// Converts Host Name to a IP
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="Beta_Search"></param>
        /// <returns>http://localhost -> 127.0.0.1</returns>
        public static string HostName2IP(string hostname, bool Beta_Search = false)
        {
            try 
            {
                if (!string.IsNullOrWhiteSpace(hostname))
                {
                    if (Beta_Search)
                    {
                        return Dns.GetHostEntry(hostname).AddressList
                        .Where(IPA => IPA.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString()).First().ToString();
                    }
                    else
                    {
                        IPHostEntry iphost = Dns.GetHostEntry(hostname);
                        IPAddress[] addresses = iphost.AddressList;

                        return addresses[0].ToString();
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Host Name to IP", string.Empty, Error, string.Empty);
            }
            finally
            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                GC.Collect(); 
#endif
            }

            return hostname;
        }
        /// <summary>
        /// Used to Force Close Launcher when Launcher encounters an error during Startup
        /// </summary>
        /// <param name="Notes">Required: Where the Launcher is Closing From</param>
        /// <param name="Force_Restart">True: Restarts Launcher | False: Closes Launcher</param>
        /// <param name="No_Log_Prompt"> True to disable the Log Prompt.</param>
        public static void ErrorCloseLauncher(string Notes, bool Force_Restart, IWin32Window? Window_Handle = default, bool No_Log_Prompt = true, bool Display_Close_Only = false)
        {
            if (Presence_Launcher.Running())
            {
                Presence_Launcher.Stop("Close");
            }

            if (Proxy_Settings.Running())
            {
                Proxy_Server.Instance.Stop("Force Close");
            }

            Log.Warning("LAUNCHER: Exiting (" + Notes + ")");
            if (!string.IsNullOrWhiteSpace(LauncherForceCloseReason))
            {
                string Message_Display = !Display_Close_Only ? "The GameLauncher has ecountered an Error and it must Close. " +
                    "Below is a Summary of the Error:" + "\n" + LauncherForceCloseReason + "\n\n" +
                    LogToFileAddons.OpenLogMessage : "Launcher will close. Below is a Summary of the Reason: \n" + LauncherForceCloseReason;

                DialogResult OpenLogFile = MessageBox.Show(Window_Handle, Message_Display, "SBRW Launcher",
                (Display_Close_Only ? MessageBoxButtons.OK : MessageBoxButtons.YesNo), MessageBoxIcon.Information);

                if (OpenLogFile == DialogResult.Yes && No_Log_Prompt)
                {
                    Process.Start(Log_Location.LogCurrentFolder);
                    Process.Start(Log_Location.LogLauncher);
                }
            }

            if (Parent_Screen.Screen_Instance != null)
            {
                Parent_Screen.Launcher_Restart = Force_Restart;
                Parent_Screen.Screen_Instance.Button_Close_Click(new object(), new EventArgs());
            }
            else if (Application.MessageLoop)
            {
                // WinForms Mode
                Application.Exit();
            }
            else
            {
                // If in Console Mode or if Form is Hidden
                Environment.Exit(0);
            }
        }
        /* Moved "runAsAdmin" Code to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/eec2f9f80aa4b350ab98d32383e1ee1f2e1c26fd/Self.cs */
    }
}