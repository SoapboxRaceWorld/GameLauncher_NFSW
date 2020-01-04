using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameLauncher.App.Classes.GPU {
    class GPU {
        public GPU() { 
            
        }

        virtual public string DriverVersion() {
            return "";
        }
    }

    public class GPUHelper {
        public static GPUManufacturer getManufacturer() {
            String man = (from x in new ManagementObjectSearcher("select * from Win32_VideoController").Get().Cast<ManagementObject>()
                  select x.GetPropertyValue("AdapterCompatibility")).FirstOrDefault().ToString();
            
            switch(man) {
                case "NVIDIA":
                    return GPUManufacturer.NVIDIA;
                case "AMD":
                    return GPUManufacturer.AMD;
                case "INTEL":
                    return GPUManufacturer.INTEL;
                default:
                    return GPUManufacturer.UNKNOWN;
            }
        }

        public enum GPUManufacturer {
            NVIDIA,
            AMD, 
            INTEL, 
            UNKNOWN
        }
    }
}
