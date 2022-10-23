using SBRW.Launcher.RunTime.LauncherCore.Logger;
using System;
using System.IO;
using System.Linq;
using System.Management;

namespace SBRW.Launcher.RunTime.SystemPlatform.Components
{
    class HardwareInfo
    {
        public class GPU
        {
            public static string CardName()
            {
                try
                {
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    return (from x in new ManagementObjectSearcher("select * from Win32_VideoController").Get()
                    .Cast<ManagementObject>()
                                 select x.GetPropertyValue("Name")).FirstOrDefault().ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8603 // Possible null reference return.
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Hardware Info", string.Empty, Error, string.Empty, true);
                }
                finally
                {
                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                    GC.Collect(); 
                    #endif
                }

                return "Unknown";
            }

            public static string DriverVersion()
            {
                try
                {
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    return (from x in new ManagementObjectSearcher("select * from Win32_VideoController").Get()
                    .Cast<ManagementObject>()
                                      select x.GetPropertyValue("DriverVersion")).FirstOrDefault().ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8603 // Possible null reference return.
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Hardware Info", string.Empty, Error, string.Empty, true);
                }
                finally
                {
                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                    GC.Collect(); 
                    #endif
                }

                return "Unknown";
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
            finally
            {
                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            }
        }
    }
}
