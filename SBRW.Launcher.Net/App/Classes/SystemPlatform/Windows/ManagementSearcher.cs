using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.App.Classes.SystemPlatform.Unix;
using System;
using System.Management;
using System.Runtime.InteropServices;

namespace SBRW.Launcher.App.Classes.SystemPlatform.Windows
{
    class ManagementSearcher
    {
        /* Searches for Installed Windows Updates */
        public static bool GetInstalledHotFix(string identification)
        {
            if (!UnixOS.Detected())
            {
                ManagementObjectSearcher ObjectPath = null;
                ManagementObjectCollection ObjectCollection = null;

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
                    LogToFileAddons.OpenLog("Installed KB [M.E.]", String.Empty, Error, String.Empty, true);
                }
                catch (COMException Error)
                {
                    LogToFileAddons.OpenLog("Installed KB [C.O.M.]", String.Empty, Error, String.Empty, true);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Installed KB", String.Empty, Error, String.Empty, true);
                }
                finally
                {
                    if (ObjectPath != null) { ObjectPath.Dispose(); }
                    if (ObjectCollection != null) { ObjectCollection.Dispose(); }
                }
            }

            return false;
        }
    }
}
