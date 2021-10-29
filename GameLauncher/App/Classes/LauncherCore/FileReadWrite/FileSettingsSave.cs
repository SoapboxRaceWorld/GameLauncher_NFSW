using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using SBRWCore.Classes.Launcher;
using SBRWCore.Classes.References;
using SBRWCore.Classes.Required;
using SBRWCore.Classes.System;
using System;

namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    class FileSettingsSave
    {
        /// <summary>Settings Format Information In Live Memory</summary>
        public static Format_Settings Live_Data = new Format_Settings();
        ///<value>Settings File Information on Disk</value>s
        private static IniFile SettingFile;
        ///<summary>Launcher Streaming Support [Saved Live Value]</summary>
        ///<remarks>Allows Video Capture Natively</remarks>
        ///<returns>True or False</returns>
        public static bool LiveStreamingSupport() => Live_Data.Launcher_Streaming_Support == "1";
        /// <summary>Creates all the NullSafe Values for Settings.ini</summary>
        public static void NullSafe()
        {
            SettingFile = new IniFile(Locations.Launcher_Settings);

            /* Pervent Removal of Login Info Before Main Screen (Temporary Boolean) */
            FileAccountSave.SaveLoginInformation = true;

            /* Migrate old Key Entries */
            if (SettingFile.KeyExists("Server"))
            {
                FileAccountSave.Live_Data.Saved_Server_Address = SettingFile.Read("Server");
                SettingFile.DeleteKey("Server");
                FileAccountSave.Save();
            }

            if (SettingFile.KeyExists("AccountEmail"))
            {
                FileAccountSave.Live_Data.User_Raw_Email = SettingFile.Read("AccountEmail");
                SettingFile.DeleteKey("AccountEmail");
                FileAccountSave.Save();
            }

            if (SettingFile.KeyExists("Password"))
            {
                FileAccountSave.Live_Data.User_Hashed_Password = SettingFile.Read("Password");
                SettingFile.DeleteKey("Password");
                FileAccountSave.Save();
            }

            /* Reset This Value as its now Safe to Do So */
            FileAccountSave.SaveLoginInformation = false;

            if (SettingFile.KeyExists("Firewall"))
            {
                Live_Data.Firewall_Game = Live_Data.Firewall_Launcher = SettingFile.Read("Firewall");
                SettingFile.DeleteKey("Firewall");
            }

            if (SettingFile.KeyExists("WindowsDefender"))
            {
                Live_Data.Defender_Game = Live_Data.Defender_Launcher = SettingFile.Read("WindowsDefender");
                SettingFile.DeleteKey("WindowsDefender");
            }

            /* Check if any Entries are missing */

            if (UnixOS.Detected() && !SettingFile.KeyExists("InstallationDirectory"))
            {
                SettingFile.Write("InstallationDirectory", "GameFiles");
            }
            else if (!SettingFile.KeyExists("InstallationDirectory"))
            {
                SettingFile.Write("InstallationDirectory", Live_Data.Game_Path);
            }
            else
            {
                Live_Data.Game_Path = SettingFile.Read("InstallationDirectory");
            }

            if (UnixOS.Detected() && SettingFile.KeyExists("OldInstallationDirectory"))
            {
                SettingFile.DeleteKey("OldInstallationDirectory");
            }
            else if (!UnixOS.Detected() && !SettingFile.KeyExists("OldInstallationDirectory"))
            {
                SettingFile.Write("OldInstallationDirectory", Live_Data.Game_Path_Old);
            }
            else
            {
                Live_Data.Game_Path_Old = SettingFile.Read("OldInstallationDirectory");
            }

            if (!SettingFile.KeyExists("CDN") || string.IsNullOrWhiteSpace(SettingFile.Read("CDN")))
            {
                SettingFile.Write("CDN", Live_Data.Launcher_CDN = "http://localhost");
            }
            else
            {
                if (SettingFile.Read("CDN").EndsWith("/"))
                {
                    SettingFile.Write("CDN", Live_Data.Launcher_CDN = SettingFile.Read("CDN").TrimEnd('/'));
                }
                else
                {
                    Live_Data.Launcher_CDN = SettingFile.Read("CDN");
                }
            }

            if (!SettingFile.KeyExists("Language") || string.IsNullOrWhiteSpace(SettingFile.Read("Language")))
            {
                SettingFile.Write("Language", Live_Data.Launcher_Language = "EN");
            }
            else
            {
                Live_Data.Launcher_Language = SettingFile.Read("Language");
            }

            if (!SettingFile.KeyExists("DisableProxy") || string.IsNullOrWhiteSpace(SettingFile.Read("DisableProxy")))
            {
                SettingFile.Write("DisableProxy", Live_Data.Launcher_Proxy = "0");
            }
            else if ((SettingFile.Read("DisableProxy") == "0") || (SettingFile.Read("DisableProxy") == "1"))
            {
                Live_Data.Launcher_Proxy = SettingFile.Read("DisableProxy");
            }
            else
            {
                SettingFile.Write("DisableProxy", Live_Data.Launcher_Proxy = "0");
            }

            if (!SettingFile.KeyExists("DisableRPC") || string.IsNullOrWhiteSpace(SettingFile.Read("DisableRPC")))
            {
                SettingFile.Write("DisableRPC", Live_Data.Launcher_Discord_Presense = "0");
            }
            else if ((SettingFile.Read("DisableRPC") == "0") || (SettingFile.Read("DisableRPC") == "1"))
            {
                Live_Data.Launcher_Discord_Presense = SettingFile.Read("DisableRPC");
            }
            else
            {
                SettingFile.Write("DisableRPC", Live_Data.Launcher_Discord_Presense = "0");
            }

            if (!SettingFile.KeyExists("IgnoreUpdateVersion") || string.IsNullOrWhiteSpace(SettingFile.Read("IgnoreUpdateVersion")))
            {
                SettingFile.Write("IgnoreUpdateVersion", Live_Data.Update_Version_Skip);
            }
            else
            {
                Live_Data.Update_Version_Skip = SettingFile.Read("IgnoreUpdateVersion");
            }

            if (UnixOS.Detected() && SettingFile.KeyExists("FilePermission"))
            {
                SettingFile.DeleteKey("FilePermission");
            }
            else if (!UnixOS.Detected() && (!SettingFile.KeyExists("FilePermission") || string.IsNullOrWhiteSpace(SettingFile.Read("FilePermission"))))
            {
                SettingFile.Write("FilePermission", Live_Data.Write_Permissions = "Unknown");
            }
            else
            {
                Live_Data.Write_Permissions = SettingFile.Read("FilePermission");
            }

            if (!SettingFile.KeyExists("GameIntegrity") || string.IsNullOrWhiteSpace(SettingFile.Read("FilePermission")))
            {
                SettingFile.Write("GameIntegrity", Live_Data.Game_Integrity = "Unknown");
            }
            else
            {
                Live_Data.Game_Integrity = SettingFile.Read("GameIntegrity");
            }

            if (!SettingFile.KeyExists("ProxyPort"))
            {
                SettingFile.Write("ProxyPort", Live_Data.Launcher_Proxy_Port);
            }
            else
            {
                Live_Data.Launcher_Proxy_Port = SettingFile.Read("ProxyPort");
            }

            if (!SettingFile.KeyExists("WebCallMethod") || string.IsNullOrWhiteSpace(SettingFile.Read("WebCallMethod")))
            {
                SettingFile.Write("WebCallMethod", Live_Data.Launcher_WebClient_Method = "WebClient");
            }
            else if (SettingFile.Read("WebCallMethod") == "WebClient" || SettingFile.Read("WebCallMethod") == "WebClientWithTimeout")
            {
                Live_Data.Launcher_WebClient_Method = SettingFile.Read("WebCallMethod");
            }
            else
            {
                Live_Data.Launcher_WebClient_Method = "WebClient";
            }

            if (!SettingFile.KeyExists("ThemeSupport") || string.IsNullOrWhiteSpace(SettingFile.Read("ThemeSupport")))
            {
                SettingFile.Write("ThemeSupport", Live_Data.Launcher_Theme_Support = "0");
            }
            else if ((SettingFile.Read("ThemeSupport") == "0") || (SettingFile.Read("ThemeSupport") == "1"))
            {
                Live_Data.Launcher_Theme_Support = SettingFile.Read("ThemeSupport");
            }
            else
            {
                SettingFile.Write("ThemeSupport", Live_Data.Launcher_Theme_Support = "0");
            }

            if (!SettingFile.KeyExists("StreamingSupport") || string.IsNullOrWhiteSpace(SettingFile.Read("StreamingSupport")))
            {
                SettingFile.Write("StreamingSupport", Live_Data.Launcher_Streaming_Support = "0");
            }
            else if ((SettingFile.Read("StreamingSupport") == "0") || (SettingFile.Read("StreamingSupport") == "1"))
            {
                Live_Data.Launcher_Streaming_Support = SettingFile.Read("StreamingSupport");
            }
            else
            {
                SettingFile.Write("StreamingSupport", Live_Data.Launcher_Streaming_Support = "0");
            }

            if (!SettingFile.KeyExists("Insider") || string.IsNullOrWhiteSpace(SettingFile.Read("Insider")))
            {
                SettingFile.Write("Insider", Live_Data.Launcher_Insider = "0");
            }
            else if (!EnableInsiderBetaTester.Allowed() && (SettingFile.Read("Insider") == "0") || (SettingFile.Read("Insider") == "1"))
            {
                Live_Data.Launcher_Insider = SettingFile.Read("StreamingSupport");
                Log.Core("Insider Status: ".ToUpper() + "Opted Into the Beta Preview -> " + 
                    EnableInsiderBetaTester.Allowed(Live_Data.Launcher_Insider == "1"));
            }
            else
            {
                SettingFile.Write("Insider", Live_Data.Launcher_Insider = "0");
            }

            if (!UnixOS.Detected())
            {
                if (!SettingFile.KeyExists("FirewallLauncher") || string.IsNullOrWhiteSpace(SettingFile.Read("FirewallLauncher")))
                {
                    SettingFile.Write("FirewallLauncher", Live_Data.Firewall_Launcher = "Unknown");
                }
                else
                {
                    Live_Data.Firewall_Launcher = SettingFile.Read("FirewallLauncher");
                }

                if (!SettingFile.KeyExists("FirewallGame") || string.IsNullOrWhiteSpace(SettingFile.Read("FirewallGame")))
                {
                    SettingFile.Write("FirewallGame", Live_Data.Firewall_Game = "Unknown");
                }
                else
                {
                    Live_Data.Firewall_Game = SettingFile.Read("FirewallGame");
                }

                if (WindowsProductVersion.GetWindowsNumber() >= 10.0)
                {
                    if (!SettingFile.KeyExists("DefenderLauncher") || string.IsNullOrWhiteSpace(SettingFile.Read("DefenderLauncher")))
                    {
                        SettingFile.Write("DefenderLauncher", Live_Data.Defender_Launcher = "Unknown");
                    }
                    else
                    {
                        Live_Data.Defender_Launcher = SettingFile.Read("DefenderLauncher");
                    }

                    if (!SettingFile.KeyExists("DefenderGame") || string.IsNullOrWhiteSpace(SettingFile.Read("DefenderGame")))
                    {
                        SettingFile.Write("DefenderGame", Live_Data.Defender_Game = "Unknown");
                    }
                    else
                    {
                        Live_Data.Defender_Game = SettingFile.Read("DefenderGame");
                    }
                }
                else if (WindowsProductVersion.GetWindowsNumber() < 10.0)
                {
                    if (SettingFile.KeyExists("DefenderLauncher") || !string.IsNullOrWhiteSpace(SettingFile.Read("DefenderLauncher")))
                    {
                        SettingFile.DeleteKey("DefenderLauncher");
                    }

                    if (SettingFile.KeyExists("DefenderGame") || !string.IsNullOrWhiteSpace(SettingFile.Read("DefenderGame")))
                    {
                        SettingFile.DeleteKey("DefenderGame");
                    }
                }

                if (WindowsProductVersion.GetWindowsNumber() == 6.1 && !SettingFile.KeyExists("PatchesApplied"))
                {
                    SettingFile.Write("PatchesApplied", Live_Data.Win_7_Patches);
                }
                else if (WindowsProductVersion.GetWindowsNumber() == 6.1 && SettingFile.KeyExists("PatchesApplied"))
                {
                    Live_Data.Win_7_Patches = SettingFile.Read("PatchesApplied");
                }
                else if ((UnixOS.Detected() || WindowsProductVersion.GetWindowsNumber() != 6.1) && SettingFile.KeyExists("PatchesApplied"))
                {
                    SettingFile.DeleteKey("PatchesApplied");
                }
            }

            /* Key Entries to Convert into Boolens */

            /** Proxy Port Number **/
            bool UsingCustomProxyPort = false;

            if (!string.IsNullOrWhiteSpace(Live_Data.Launcher_Proxy_Port))
            {
                bool isNumeric = int.TryParse(Live_Data.Launcher_Proxy_Port, out int Port);

                if (isNumeric)
                {
                    if (Port > 0)
                    {
                        ServerProxy.ProxyPort = Port;
                        UsingCustomProxyPort = true;
                        Log.Info("SETTINGS FILE: Custom Proxy Port -> " + Port);
                    }
                }
            }

            if (!UsingCustomProxyPort)
            {
                bool isNumeric = int.TryParse(DateTime.Now.Year.ToString(), out int Port);

                if (isNumeric)
                {
                    ServerProxy.ProxyPort = new Random().Next(2017, Port);
                }
                else
                {
                    ServerProxy.ProxyPort = new Random().Next(2017, 2021);
                }

                Log.Info("SETTINGS FILE: Random Generated Default Port -> " + ServerProxy.ProxyPort);
            }

            Live_Cache.Launcher_Alternative_Webcalls(Live_Data.Launcher_WebClient_Method == "WebClient");

            /* Key Entries to Remove (No Longer Needed) */

            if (SettingFile.KeyExists("LauncherPosX"))
            {
                SettingFile.DeleteKey("LauncherPosX");
            }

            if (SettingFile.KeyExists("LauncherPosY"))
            {
                SettingFile.DeleteKey("LauncherPosY");
            }

            if (SettingFile.KeyExists("DisableVerifyHash"))
            {
                SettingFile.DeleteKey("DisableVerifyHash");
            }

            if (SettingFile.KeyExists("TracksHigh"))
            {
                SettingFile.DeleteKey("TracksHigh");
            }

            if (SettingFile.KeyExists("ModNetDisabled"))
            {
                SettingFile.DeleteKey("ModNetDisabled");
            }

            if (SettingFile.KeyExists("ModNetZip"))
            {
                SettingFile.DeleteKey("ModNetZip");
            }

            SettingFile = new IniFile(Locations.Launcher_Settings);
        }
        /// <summary>Saves all Current Values</summary>
        public static void SaveSettings()
        {
            SettingFile = new IniFile(Locations.Launcher_Settings);

            if (SettingFile.Read("CDN") != Live_Data.Launcher_CDN)
            {
                if (Live_Data.Launcher_CDN.EndsWith("/"))
                {
                    SettingFile.Write("CDN", Live_Data.Launcher_CDN.TrimEnd('/'));
                }
                else
                {
                    SettingFile.Write("CDN", Live_Data.Launcher_CDN);
                }
            }

            if (SettingFile.Read("Language") != Live_Data.Launcher_Language)
            {
                SettingFile.Write("Language", Live_Data.Launcher_Language);
            }

            if (SettingFile.Read("DisableProxy") != Live_Data.Launcher_Proxy)
            {
                SettingFile.Write("DisableProxy", Live_Data.Launcher_Proxy);
            }

            if (SettingFile.Read("DisableRPC") != Live_Data.Launcher_Discord_Presense)
            {
                SettingFile.Write("DisableRPC", Live_Data.Launcher_Discord_Presense);
            }

            if (SettingFile.Read("InstallationDirectory") != Live_Data.Game_Path)
            {
                SettingFile.Write("InstallationDirectory", Live_Data.Game_Path);
            }

            if (!UnixOS.Detected() && SettingFile.Read("OldInstallationDirectory") != Live_Data.Game_Path_Old)
            {
                SettingFile.Write("OldInstallationDirectory", Live_Data.Game_Path_Old);
            }

            if (SettingFile.Read("IgnoreUpdateVersion") != Live_Data.Update_Version_Skip)
            {
                SettingFile.Write("IgnoreUpdateVersion", Live_Data.Update_Version_Skip);
            }

            if (SettingFile.Read("GameIntegrity") != Live_Data.Game_Integrity)
            {
                SettingFile.Write("GameIntegrity", Live_Data.Game_Integrity);
            }

            if (SettingFile.Read("WebCallMethod") != Live_Data.Launcher_WebClient_Method)
            {
                SettingFile.Write("WebCallMethod", Live_Data.Launcher_WebClient_Method);
            }

            if (SettingFile.Read("ThemeSupport") != Live_Data.Launcher_Theme_Support)
            {
                SettingFile.Write("ThemeSupport", Live_Data.Launcher_Theme_Support);
            }

            if (SettingFile.Read("StreamingSupport") != Live_Data.Launcher_Streaming_Support)
            {
                SettingFile.Write("StreamingSupport", Live_Data.Launcher_Streaming_Support);
            }

            if (SettingFile.Read("Insider") != Live_Data.Launcher_Insider)
            {
                SettingFile.Write("Insider", Live_Data.Launcher_Insider);
            }

            if (!UnixOS.Detected())
            {
                if (SettingFile.Read("FilePermission") != Live_Data.Write_Permissions)
                {
                    SettingFile.Write("FilePermission", Live_Data.Write_Permissions);
                }

                if (SettingFile.Read("FirewallLauncher") != Live_Data.Firewall_Launcher)
                {
                    SettingFile.Write("FirewallLauncher", Live_Data.Firewall_Launcher);
                }

                if (SettingFile.Read("FirewallGame") != Live_Data.Firewall_Game)
                {
                    SettingFile.Write("FirewallGame", Live_Data.Firewall_Game);
                }

                if (WindowsProductVersion.GetWindowsNumber() >= 10.0)
                {
                    if (SettingFile.Read("DefenderLauncher") != Live_Data.Defender_Launcher)
                    {
                        SettingFile.Write("DefenderLauncher", Live_Data.Defender_Launcher);
                    }

                    if (SettingFile.Read("DefenderGame") != Live_Data.Defender_Game)
                    {
                        SettingFile.Write("DefenderGame", Live_Data.Defender_Game);
                    }
                }

                if ((SettingFile.Read("PatchesApplied") != Live_Data.Win_7_Patches) && WindowsProductVersion.GetWindowsNumber() == 6.1)
                {
                    SettingFile.Write("PatchesApplied", Live_Data.Win_7_Patches);
                }
            }

            SettingFile = new IniFile(Locations.Launcher_Settings);
        }
    }
}