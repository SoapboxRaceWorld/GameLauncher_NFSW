using System;
using System.Management;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class ManagementSearcher
    {
        /* Checks AntiVirus is running (Windows 10 Only) */
        public static bool SecurityCenter(string Query)
        {
            bool ServiceStatus = false;

            try
            {
                ManagementObjectSearcher Search =
                    new ManagementObjectSearcher("root\\Microsoft\\Windows\\Defender",
                    "SELECT * FROM MSFT_MpComputerStatus");

                foreach (ManagementObject queryObj in Search.Get())
                {
                    ServiceStatus = (bool)queryObj[Query];
                }
            }
            catch
            {
                ServiceStatus = false;
            }

            return ServiceStatus;
        }

        /* Searches for Installed Windows Updates */
        public static bool GetInstalledHotFix(string identification)
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

            return false;
        }
    }
}
