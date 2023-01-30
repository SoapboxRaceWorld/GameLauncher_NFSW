using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.SystemPlatform.Unix;
using System;
using System.Management;
using System.Runtime.InteropServices;

namespace SBRW.Launcher.RunTime.SystemPlatform.Windows
{
    class ManagementSearcher
    {
        /* Searches for Installed Windows Updates */
        public static bool GetInstalledHotFix(string identification)
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
            ManagementObjectSearcher? ObjectPath = null;
            ManagementObjectCollection? ObjectCollection = null;

            try
            {
                ObjectPath = new ManagementObjectSearcher("SELECT HotFixID FROM Win32_QuickFixEngineering");
                ObjectCollection = ObjectPath.Get();

                foreach (ManagementBaseObject SearchBase in ObjectCollection)
                {
                    if (SearchBase.Properties["HotFixID"].Value.ToString() == identification)
                    {
                        return true;
                    }
                }
            }
            catch (ManagementException Error)
            {
                LogToFileAddons.OpenLog("Installed KB [M.E.]", string.Empty, Error, string.Empty, true);
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("Installed KB [C.O.M.]", string.Empty, Error, string.Empty, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Installed KB", string.Empty, Error, string.Empty, true);
            }
            finally
            {
                ObjectPath?.Dispose();
                ObjectCollection?.Dispose();
            }

            return false;
#else
            return true;
#endif
        }
    }
}
