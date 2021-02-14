using System;
using System.IO;
using System.Linq;
using System.Management;

namespace GameLauncher.App.Classes.SystemPlatform.Components
{
    class HardwareInfo
    {
        public class GPU
        {
            public static string CardName()
            {
                string _cardName = (from x in new ManagementObjectSearcher("select * from Win32_VideoController").Get()
                    .Cast<ManagementObject>()
                                    select x.GetPropertyValue("Name")).FirstOrDefault().ToString();
                return _cardName;
            }

            public static string DriverVersion()
            {
                string _driverVersion = (from x in new ManagementObjectSearcher("select * from Win32_VideoController").Get()
                    .Cast<ManagementObject>()
                                         select x.GetPropertyValue("DriverVersion")).FirstOrDefault().ToString();
                return _driverVersion;
            }
        }

        /* Moved "GPU" Class File to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/7fea53ad29b233ed35711870c90c78a06b82e090/init.cs */

        public static bool CheckArchitectureFile(string fileName)
        {
            const int PE_POINTER_OFFSET = 60;
            const int MACHINE_OFFSET = 4;
            byte[] data = new byte[4096];

            using (Stream s = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                s.Read(data, 0, 4096);
            }

            int PE_HEADER_ADDR = BitConverter.ToInt32(data, PE_POINTER_OFFSET);
            int machineUint = BitConverter.ToUInt16(data, PE_HEADER_ADDR + MACHINE_OFFSET);
            return machineUint == 0x014c;
        }
    }
}
