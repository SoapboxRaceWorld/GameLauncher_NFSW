using System.Linq;
using System.Management;

namespace GameLauncher.App.Classes.GPU {
    class GPU {
        public GPU() {
            
        }
        virtual public string CardName() {
            return "";
        }
        virtual public string DriverVersion() {
            return "";
        }
    }

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
}
