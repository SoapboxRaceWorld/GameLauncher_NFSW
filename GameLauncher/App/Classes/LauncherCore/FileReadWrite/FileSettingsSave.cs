using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using System;

namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    class FileSettingsSave
    {
        public static IniFile settingFile = new IniFile("Settings.ini");
        ///<summary>Game Files Path</summary>
        public static string GameInstallation = !string.IsNullOrWhiteSpace(settingFile.Read("InstallationDirectory")) ?
            settingFile.Read("InstallationDirectory") : string.Empty;
        ///<summary>Cache Old Game Files Path</summary>
        ///<remarks>Used for Firewall and Defender Checks</remarks>
        public static string GameInstallationOld = !string.IsNullOrWhiteSpace(settingFile.Read("OldInstallationDirectory")) ?
            settingFile.Read("OldInstallationDirectory") : string.Empty;
        ///<summary>CDN URL [Saved/Choosen]</summary>
        public static string CDN = !string.IsNullOrWhiteSpace(settingFile.Read("CDN")) ? settingFile.Read("CDN") : "http://localhost/";
        ///<summary>Language [Saved/Choosen]</summary>
        public static string Lang = !string.IsNullOrWhiteSpace(settingFile.Read("Language")) ? settingFile.Read("Language") : "EN";
        ///<summary>Launcher Proxy [Saved/Ticked]</summary>
        public static string Proxy = (!string.IsNullOrWhiteSpace(settingFile.Read("DisableProxy"))
                                      && (settingFile.Read("DisableProxy") == "1" || settingFile.Read("DisableProxy") == "0")) ?
            settingFile.Read("DisableProxy") : "0";
        ///<summary>Launcher Discord RPC [Saved/Ticked]</summary>
        public static string RPC = (!string.IsNullOrWhiteSpace(settingFile.Read("DisableRPC"))
                                    && (settingFile.Read("DisableRPC") == "1" || settingFile.Read("DisableRPC") == "0")) ?
            settingFile.Read("DisableRPC") : "0";
        ///<summary>Launcher Version to Ignore [Saved/Clicked]</summary>
        public static string IgnoreVersion = !string.IsNullOrWhiteSpace(settingFile.Read("IgnoreUpdateVersion")) ?
            settingFile.Read("IgnoreUpdateVersion") : string.Empty;
        ///<summary>Firewall Status: Launcher [Saved/Ran]</summary>
        public static string FirewallLauncherStatus = !string.IsNullOrWhiteSpace(settingFile.Read("FirewallLauncher")) ?
            settingFile.Read("FirewallLauncher") : "Unknown";
        ///<summary>Firewall Status: Game [Saved/Ran]</summary>
        public static string FirewallGameStatus = !string.IsNullOrWhiteSpace(settingFile.Read("FirewallGame")) ?
            settingFile.Read("FirewallGame") : "Unknown";
        ///<summary>Defender Status: Launcher [Saved/Ran]</summary>
        public static string DefenderLauncherStatus = !string.IsNullOrWhiteSpace(settingFile.Read("DefenderLauncher")) ?
            settingFile.Read("DefenderLauncher") : "Unknown";
        ///<summary>Defender Status: Game [Saved/Ran]</summary>
        public static string DefenderGameStatus = !string.IsNullOrWhiteSpace(settingFile.Read("DefenderGame")) ?
            settingFile.Read("DefenderGame") : "Unknown";
        ///<summary>Windows 7: Specific KB Updates MessageBox [Saved/Clicked]</summary>
        public static string Win7UpdatePatches = !string.IsNullOrWhiteSpace(settingFile.Read("PatchesApplied")) ?
            settingFile.Read("PatchesApplied") : string.Empty;
        ///<summary>Windows: Folder or File Permission Status [Saved/Ran]</summary>
        public static string FilePermissionStatus = !string.IsNullOrWhiteSpace(settingFile.Read("FilePermission")) ?
            settingFile.Read("FilePermission") : "Not Set";
        ///<summary>Game Files are Corrupt in someway and will need to be Verified [Saved]</summary>
        public static string GameIntegrity = !string.IsNullOrWhiteSpace(settingFile.Read("GameIntegrity")) ?
            settingFile.Read("GameIntegrity") : "Unknown";
        ///<summary>Launcher WebCalls [Saved/Ticked]</summary>
        ///<remarks>Does not affect Launcher Proxy</remarks>
        public static string WebCallMethod = (!string.IsNullOrWhiteSpace(settingFile.Read("WebCallMethod")) &&
            (settingFile.Read("WebCallMethod") == "WebClient" || settingFile.Read("WebCallMethod") == "WebClientWithTimeout")) ?
            settingFile.Read("WebCallMethod") : "WebClient";
        ///<summary>Launcher Theme Support [Saved/Ticked]</summary>
        public static string ThemeSupport = (!string.IsNullOrWhiteSpace(settingFile.Read("ThemeSupport"))
                                    && (settingFile.Read("ThemeSupport") == "1" || settingFile.Read("ThemeSupport") == "0")) ?
            settingFile.Read("ThemeSupport") : "0";
        ///<summary>Launcher Streaming Support [Saved/Ticked]</summary>
        ///<remarks>Allows Video Capture Natively</remarks>
        public static string StreamingSupport = (!string.IsNullOrWhiteSpace(settingFile.Read("StreamingSupport"))
                                    && (settingFile.Read("StreamingSupport") == "1" || settingFile.Read("StreamingSupport") == "0")) ?
            settingFile.Read("StreamingSupport") : "0";
        ///<summary>Launcher Streaming Support [Saved Live Value]</summary>
        ///<remarks>Allows Video Capture Natively</remarks>
        ///<returns>True or False</returns>
        public static bool LiveStreamingSupport() => StreamingSupport == "1";
        ///<summary>Launcher Beta Builds [Saved/Ticked]</summary>
        ///<remarks>User Opt-In to Preview Builds</remarks>
        public static string Insider = (!string.IsNullOrWhiteSpace(settingFile.Read("Insider"))
                                    && (settingFile.Read("Insider") == "1" || settingFile.Read("Insider") == "0")) ?
            settingFile.Read("Insider") : "0";
        /// <summary>Creates all the NullSafe Values for Settings.ini</summary>
        public static void NullSafeSettings()
        {
            /* Pervent Removal of Login Info Before Main Screen (Temporary Boolean) */
            FileAccountSave.SaveLoginInformation = true;

            /* Migrate old Key Entries */
            if (settingFile.KeyExists("Server"))
            {
                FileAccountSave.ChoosenGameServer = settingFile.Read("Server");
                settingFile.DeleteKey("Server");
                FileAccountSave.SaveAccount();
            }

            if (settingFile.KeyExists("AccountEmail"))
            {
                FileAccountSave.UserRawEmail = settingFile.Read("AccountEmail");
                settingFile.DeleteKey("AccountEmail");
                FileAccountSave.SaveAccount();
            }

            if (settingFile.KeyExists("Password"))
            {
                FileAccountSave.UserHashedPassword = settingFile.Read("Password");
                settingFile.DeleteKey("Password");
                FileAccountSave.SaveAccount();
            }

            /* Reset This Value as its now Safe to Do So */
            FileAccountSave.SaveLoginInformation = false;

            if (settingFile.KeyExists("Firewall"))
            {
                FirewallLauncherStatus = settingFile.Read("Firewall");
                FirewallGameStatus = FirewallLauncherStatus;
                settingFile.DeleteKey("Firewall");
            }

            if (settingFile.KeyExists("WindowsDefender"))
            {
                DefenderLauncherStatus = settingFile.Read("WindowsDefender");
                DefenderGameStatus = DefenderLauncherStatus;
                settingFile.DeleteKey("WindowsDefender");
            }

            /* Check if any Entries are missing */

            if (UnixOS.Detected() && !settingFile.KeyExists("InstallationDirectory"))
            {
                settingFile.Write("InstallationDirectory", "GameFiles");
            }
            else if (!settingFile.KeyExists("InstallationDirectory"))
            {
                settingFile.Write("InstallationDirectory", GameInstallation);
            }

            if (UnixOS.Detected() && settingFile.KeyExists("OldInstallationDirectory"))
            {
                settingFile.DeleteKey("OldInstallationDirectory");
            }
            else if (!UnixOS.Detected() && !settingFile.KeyExists("OldInstallationDirectory"))
            {
                settingFile.Write("OldInstallationDirectory", GameInstallationOld);
            }

            if (!settingFile.KeyExists("CDN"))
            {
                settingFile.Write("CDN", CDN);
            }
            else if (settingFile.KeyExists("CDN"))
            {
                if (CDN.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    string FinalCDNURL = CDN.TrimEnd(charsToTrim);

                    settingFile.Write("CDN", FinalCDNURL);
                }
            }

            if (!settingFile.KeyExists("Language"))
            {
                settingFile.Write("Language", Lang);
            }

            if (!settingFile.KeyExists("DisableProxy"))
            {
                settingFile.Write("DisableProxy", Proxy);
            }

            if (!settingFile.KeyExists("DisableRPC"))
            {
                settingFile.Write("DisableRPC", RPC);
            }

            if (!settingFile.KeyExists("IgnoreUpdateVersion"))
            {
                settingFile.Write("IgnoreUpdateVersion", IgnoreVersion);
            }

            if (!settingFile.KeyExists("FilePermission") && !UnixOS.Detected())
            {
                settingFile.Write("FilePermission", FilePermissionStatus);
            }
            else if (settingFile.KeyExists("FilePermission") && UnixOS.Detected())
            {
                settingFile.DeleteKey("FilePermission");
            }

            if (!settingFile.KeyExists("GameIntegrity"))
            {
                settingFile.Write("GameIntegrity", GameIntegrity);
            }

            if (!settingFile.KeyExists("ProxyPort"))
            {
                settingFile.Write("ProxyPort", string.Empty);
            }

            if (!settingFile.KeyExists("WebCallMethod"))
            {
                settingFile.Write("WebCallMethod", WebCallMethod);
            }

            if (!settingFile.KeyExists("ThemeSupport"))
            {
                settingFile.Write("ThemeSupport", ThemeSupport);
            }
            if (!settingFile.KeyExists("StreamingSupport"))
            {
                settingFile.Write("StreamingSupport", StreamingSupport);
            }

            if (!settingFile.KeyExists("Insider"))
            {
                settingFile.Write("Insider", Insider);
            }
            else if (settingFile.KeyExists("Insider") && !EnableInsiderBetaTester.Allowed())
            {
                Log.Core("Insider Status: ".ToUpper() + "Opted Into the Beta Preview -> " + EnableInsiderBetaTester.Allowed(Insider == "1"));
            }

            if (!UnixOS.Detected())
            {
                if (!settingFile.KeyExists("FirewallLauncher"))
                {
                    settingFile.Write("FirewallLauncher", FirewallLauncherStatus);
                }

                if (!settingFile.KeyExists("FirewallGame"))
                {
                    settingFile.Write("FirewallGame", FirewallGameStatus);
                }

                if (WindowsProductVersion.GetWindowsNumber() >= 10.0)
                {
                    if (!settingFile.KeyExists("DefenderLauncher"))
                    {
                        settingFile.Write("DefenderLauncher", DefenderLauncherStatus);
                    }

                    if (!settingFile.KeyExists("DefenderGame"))
                    {
                        settingFile.Write("DefenderGame", DefenderGameStatus);
                    }
                }
                else if (WindowsProductVersion.GetWindowsNumber() < 10.0)
                {
                    if (settingFile.KeyExists("DefenderLauncher") || !string.IsNullOrWhiteSpace(settingFile.Read("DefenderLauncher")))
                    {
                        settingFile.DeleteKey("DefenderLauncher");
                    }

                    if (settingFile.KeyExists("DefenderGame") || !string.IsNullOrWhiteSpace(settingFile.Read("DefenderGame")))
                    {
                        settingFile.DeleteKey("DefenderGame");
                    }
                }

                if (WindowsProductVersion.GetWindowsNumber() == 6.1 && !settingFile.KeyExists("PatchesApplied"))
                {
                    settingFile.Write("PatchesApplied", Win7UpdatePatches);
                }
                else if ((UnixOS.Detected() || WindowsProductVersion.GetWindowsNumber() != 6.1) && settingFile.KeyExists("PatchesApplied"))
                {
                    settingFile.DeleteKey("PatchesApplied");
                }
            }

            /* Key Entries to Convert into Boolens */

            /** Proxy Port Number **/
            bool UsingCustomProxyPort = false;

            if (!string.IsNullOrWhiteSpace(settingFile.Read("ProxyPort")))
            {
                bool isNumeric = int.TryParse(settingFile.Read("ProxyPort"), out int Port);

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

            if (!string.IsNullOrWhiteSpace(WebCallMethod))
            {
                Log.Info("SETTINGS FILE: Choosen WebCall Method -> " + WebCallMethod);
            }

            /* Key Entries to Remove (No Longer Needed) */

            if (settingFile.KeyExists("LauncherPosX"))
            {
                settingFile.DeleteKey("LauncherPosX");
            }

            if (settingFile.KeyExists("LauncherPosY"))
            {
                settingFile.DeleteKey("LauncherPosY");
            }

            if (settingFile.KeyExists("DisableVerifyHash"))
            {
                settingFile.DeleteKey("DisableVerifyHash");
            }

            if (settingFile.KeyExists("TracksHigh"))
            {
                settingFile.DeleteKey("TracksHigh");
            }

            if (settingFile.KeyExists("ModNetDisabled"))
            {
                settingFile.DeleteKey("ModNetDisabled");
            }

            if (settingFile.KeyExists("ModNetZip"))
            {
                settingFile.DeleteKey("ModNetZip");
            }

            settingFile = new IniFile("Settings.ini");
        }
        /// <summary>Saves all Current Values</summary>
        public static void SaveSettings()
        {
            if (settingFile.Read("CDN") != CDN)
            {
                if (CDN.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    string FinalCDNURL = CDN.TrimEnd(charsToTrim);

                    settingFile.Write("CDN", FinalCDNURL);
                }
                else
                {
                    settingFile.Write("CDN", CDN);
                }
            }

            if (settingFile.Read("Language") != Lang)
            {
                settingFile.Write("Language", Lang);
            }

            if (settingFile.Read("DisableProxy") != Proxy)
            {
                settingFile.Write("DisableProxy", Proxy);
            }

            if (settingFile.Read("DisableRPC") != RPC)
            {
                settingFile.Write("DisableRPC", RPC);
            }

            if (settingFile.Read("InstallationDirectory") != GameInstallation)
            {
                settingFile.Write("InstallationDirectory", GameInstallation);
            }

            if (!UnixOS.Detected() && settingFile.Read("OldInstallationDirectory") != GameInstallationOld)
            {
                settingFile.Write("OldInstallationDirectory", GameInstallationOld);
            }

            if (settingFile.Read("IgnoreUpdateVersion") != IgnoreVersion)
            {
                settingFile.Write("IgnoreUpdateVersion", IgnoreVersion);
            }

            if (settingFile.Read("GameIntegrity") != GameIntegrity)
            {
                settingFile.Write("GameIntegrity", GameIntegrity);
            }

            if (settingFile.Read("WebCallMethod") != WebCallMethod)
            {
                settingFile.Write("WebCallMethod", WebCallMethod);
            }

            if (settingFile.Read("ThemeSupport") != ThemeSupport)
            {
                settingFile.Write("ThemeSupport", ThemeSupport);
            }

            if (settingFile.Read("StreamingSupport") != StreamingSupport)
            {
                settingFile.Write("StreamingSupport", StreamingSupport);
            }

            if (settingFile.Read("Insider") != Insider)
            {
                settingFile.Write("Insider", Insider);
            }

            if (!UnixOS.Detected())
            {
                if (settingFile.Read("FilePermission") != FilePermissionStatus)
                {
                    settingFile.Write("FilePermission", FilePermissionStatus);
                }

                if (settingFile.Read("FirewallLauncher") != FirewallLauncherStatus)
                {
                    settingFile.Write("FirewallLauncher", FirewallLauncherStatus);
                }

                if (settingFile.Read("FirewallGame") != FirewallGameStatus)
                {
                    settingFile.Write("FirewallGame", FirewallGameStatus);
                }

                if (WindowsProductVersion.GetWindowsNumber() >= 10.0)
                {
                    if (settingFile.Read("DefenderLauncher") != DefenderLauncherStatus)
                    {
                        settingFile.Write("DefenderLauncher", DefenderLauncherStatus);
                    }

                    if (settingFile.Read("DefenderGame") != DefenderGameStatus)
                    {
                        settingFile.Write("DefenderGame", DefenderGameStatus);
                    }
                }

                if ((settingFile.Read("PatchesApplied") != Win7UpdatePatches) && WindowsProductVersion.GetWindowsNumber() == 6.1)
                {
                    settingFile.Write("PatchesApplied", Win7UpdatePatches);
                }
            }

            settingFile = new IniFile("Settings.ini");
        }
    }
}