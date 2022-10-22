using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.Core.Theme.Conversion_;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace SBRW.Launcher.RunTime.LauncherCore.Support
{
    internal class FormsIcon
    {
        /// <summary>
        /// 
        /// </summary>
        public static Icon? Cached_Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static bool Set_Cached_Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Error_Encountered"></param>
        /// <returns></returns>
        public static Icon? Retrive_Icon(bool Error_Encountered = false)
        {
            if (!Set_Cached_Icon)
            {
                try
                {
                    if (File.Exists(Path.Combine(Locations.LauncherFolder, "SBRW.Icon.ico")))
                    {
                        using var stream = File.OpenRead(Path.Combine(Locations.LauncherFolder, "SBRW.Icon.ico"));
                        Cached_Icon = new Icon(stream);
                        Set_Cached_Icon = true;
                    }
                    else if (Embeded_Files.SBRW_Ico_Bytes().Length > 0)
                    {
                        using MemoryStream Live_Memory_Cache = new MemoryStream(Embeded_Files.SBRW_Ico_Bytes());
                        Cached_Icon = new Icon(Live_Memory_Cache);
                        Set_Cached_Icon = true;
                    }
                    else if (!string.IsNullOrWhiteSpace(Assembly.GetExecutingAssembly().Location) && !Error_Encountered)
                    {
                        if ((Cached_Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location)) == null)
                        {
                            if (!string.IsNullOrWhiteSpace(Path.Combine(Locations.LauncherFolder, Locations.NameLauncher)))
                            {
                                if ((Cached_Icon = Icon.ExtractAssociatedIcon(Path.Combine(Locations.LauncherFolder, Locations.NameLauncher))) == null)
                                {
                                    Retrive_Icon(true);
                                }
                                else
                                {
                                    Set_Cached_Icon = true;
                                }
                            }
                            else
                            {
                                Retrive_Icon(true);
                            }
                        }
                        else
                        {
                            Set_Cached_Icon = true;
                        }
                    }
                    else
                    {
                        Cached_Icon = default;
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Retrive Icon", string.Empty, Error, string.Empty, true);
                }
            }

            return Cached_Icon;
        }
    }
}
