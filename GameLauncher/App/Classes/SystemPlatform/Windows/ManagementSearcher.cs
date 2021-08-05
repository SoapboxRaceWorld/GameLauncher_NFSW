using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.IO;
using System.Management;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class SecurityCenter
    {
        public static bool Antivirus() => ManagementSearcher.GetSecurityCenterStatus("AntivirusEnabled");
        public static bool Antispyware() => ManagementSearcher.GetSecurityCenterStatus("AntispywareEnabled");
        public static bool RealTimeProtection() => ManagementSearcher.GetSecurityCenterStatus("RealTimeProtectionEnabled");
    }

    class ManagementSearcher
    {
        /* Checks AntiVirus is running (Windows 10 Only) */
        public static bool GetSecurityCenterStatus(string Query)
        {
            try
            {
                ManagementObjectSearcher Search =
                    new ManagementObjectSearcher(Path.Combine("root", "Microsoft", "Windows", "Defender"),
                    "SELECT * FROM MSFT_MpComputerStatus");

                foreach (ManagementObject queryObj in Search.Get())
                {
                    Log.Debug(Search.Get().ToString());
                    return (bool)queryObj[Query];
                }
            }
            catch (ManagementException Error)
            {
                LogToFileAddons.OpenLog("Security Center", null, Error, null, true);
                return false;
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Security Center", null, Error, null, true);
                return false;
            }

            return false;
        }

        /* Searches for Installed Windows Updates */
        public static bool GetInstalledHotFix(string identification)
        {
            try
            {
                var search = new ManagementObjectSearcher("SELECT HotFixID FROM Win32_QuickFixEngineering");
                var collection = search.Get();

                foreach (ManagementObject quickFix in collection)
                {
                    Console.WriteLine("Updates installed: " + quickFix["HotFixID"].ToString());
                    if (quickFix["HotFixID"].ToString() == identification)
                    {
                        return true;
                    }
                }
            }
            catch (ManagementException Error)
            {
                LogToFileAddons.OpenLog("Installed KB", null, Error, null, true);
                return false;
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Installed KB", null, Error, null, true);
                return false;
            }

            return false;
        }
    }
}
