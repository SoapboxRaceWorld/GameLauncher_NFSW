using System.Management;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class WindowsDefender
    {
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
    }
}
