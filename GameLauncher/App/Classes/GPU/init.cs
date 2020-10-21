using System.Linq;
using System.Management;

namespace GameLauncher.App.Classes.GPU {

    public class GPUHelper
    {
        public static string CardName() {
            string _cardName = (from x in new ManagementObjectSearcher("select * from Win32_VideoController").Get()
                .Cast<ManagementObject>()
                select x.GetPropertyValue("Name")).FirstOrDefault().ToString();
            return _cardName;
        }
        public static string DriverVersion() {
            string _driverVersion = (from x in new ManagementObjectSearcher("select * from Win32_VideoController").Get()
                .Cast<ManagementObject>()
                select x.GetPropertyValue("DriverVersion")).FirstOrDefault().ToString();
            return _driverVersion;
        }
    }

    /* Moved "GPU" Class File to Gist */
    /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/7fea53ad29b233ed35711870c90c78a06b82e090/init.cs */
}
