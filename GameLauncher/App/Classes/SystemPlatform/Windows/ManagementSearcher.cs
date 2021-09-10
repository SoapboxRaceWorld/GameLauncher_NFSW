using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using System;
using System.Management;
using System.Runtime.InteropServices;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
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
                    LogToFileAddons.OpenLog("Installed KB [M.E.]", null, Error, null, true);
                }
                catch (COMException Error)
                {
                    LogToFileAddons.OpenLog("Installed KB [C.O.M.]", null, Error, null, true);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Installed KB", null, Error, null, true);
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
