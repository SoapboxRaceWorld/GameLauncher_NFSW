using System.Management;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class WindowsDefender
    {
        public static bool SecurityCenter(string Query)
        {
            ManagementObjectSearcher Search =
                    new ManagementObjectSearcher("root\\Microsoft\\Windows\\Defender",
                    "SELECT * FROM MSFT_MpComputerStatus");

            bool ServiceStatus = false;

            foreach (ManagementObject queryObj in Search.Get())
            {
                ServiceStatus = (bool)queryObj[Query];
            }

            return ServiceStatus;
        }
    }
}
