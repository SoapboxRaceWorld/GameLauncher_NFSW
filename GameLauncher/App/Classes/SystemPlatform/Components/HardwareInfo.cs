using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.IO;
using System.Linq;
using System.Management;

namespace GameLauncher.App.Classes.SystemPlatform.Components
{
    class HardwareInfo
    {
        public class CPU
        {
            public static string CPUName()
            {
                string _cpuName = "Unknown";
                try
                {
                    _cpuName = (from x in new ManagementObjectSearcher("SELECT Name FROM Win32_Processor").Get().Cast<ManagementObject>()
                                select x.GetPropertyValue("Name")).FirstOrDefault().ToString();
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Hardware Info", null, Error, null, true);
                }
                return _cpuName;
            }
        }

        public class RAM
        {
            public static string SysMem()
            {
                long memKb = 0;
                try
                {
                    Kernel32.GetPhysicallyInstalledSystemMemory(out memKb);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Hardware Info", null, Error, null, true);
                }
                return (memKb / 1024).ToString();
            }
        }

        public class GPU
        {
            public static string CardName()
            {
                string _cardName = "Unknown";
                try
                {
                    _cardName = (from x in new ManagementObjectSearcher("select * from Win32_VideoController").Get()
                    .Cast<ManagementObject>()
                                 select x.GetPropertyValue("Name")).FirstOrDefault().ToString();
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Hardware Info", null, Error, null, true);
                }

                return _cardName;
            }

            public static string DriverVersion()
            {
                string _driverVersion = "Unknown";
                try
                {
                    _driverVersion = (from x in new ManagementObjectSearcher("select * from Win32_VideoController").Get()
                    .Cast<ManagementObject>()
                                      select x.GetPropertyValue("DriverVersion")).FirstOrDefault().ToString();
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Hardware Info", null, Error, null, true);
                }

                return _driverVersion;
            }
        }

        /* Moved "GPU" Class File to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/7fea53ad29b233ed35711870c90c78a06b82e090/init.cs */

        public static bool CheckArchitectureFile(string fileName)
        {
            try
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
            catch
            {
                return false;
            }
        }
    }
}
