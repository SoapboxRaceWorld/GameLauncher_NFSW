using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.Core.Cache;
using System;
using System.IO;
using SBRW.Launcher.Core.Required.DLL;

namespace SBRW.Launcher.RunTime.SystemPlatform.Unix
{
    class UnixOS
    {
#if NETFRAMEWORK
        private static int CachedNumPlatform { get; set; } = 2020;
#endif
        private static string CacheUnixOSName { get; set; } = string.Empty;
#if NETFRAMEWORK
        public static int Platform()
        {
            try
            {
                if (CachedNumPlatform == 2020)
                {
                    CachedNumPlatform = (int)Environment.OSVersion.Platform;
                }

                return CachedNumPlatform;
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Unix Platform Detection", string.Empty, Error, string.Empty, true);
                return CachedNumPlatform;
            }
        }

        public static PlatformIDPort ID(int Number)
        {
            return Number switch
            {
                0 => PlatformIDPort.Win32S,
                1 => PlatformIDPort.Win32Windows,
                2 => PlatformIDPort.Win32NT,
                3 => PlatformIDPort.WinCE,
                4 => PlatformIDPort.Unix,
                5 => PlatformIDPort.Xbox,
                6 => PlatformIDPort.MacOSX,
                128 => PlatformIDPort.MonoLegacy,
                _ => PlatformIDPort.Unknown,
            };
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Skip_Core_File"></param>
        /// <returns></returns>
        public static bool AmI()
        {
            try
            {
#if NETFRAMEWORK
            if (Type.GetType("Mono.Runtime") != null || DLL_NTDLL.WineDetected())
            {
                return true;
            }
            else
            {
                switch (ID(Platform()))
                {
                    case PlatformIDPort.Unix:
                    case PlatformIDPort.MonoLegacy:
                    case PlatformIDPort.MacOSX:
                        return true;
                    default:
                        return false;
                }
            }
#else
                return OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD() ||
                (Type.GetType("Mono.Runtime") != null) || DLL_NTDLL.WineDetected();
#endif
            }
            catch
            {
#if NETFRAMEWORK
            if (Type.GetType("Mono.Runtime") != null)
            {
                return true;
            }
            else
            {
                switch (ID(Platform()))
                {
                    case PlatformIDPort.Unix:
                    case PlatformIDPort.MonoLegacy:
                    case PlatformIDPort.MacOSX:
                        return true;
                    default:
                        return false;
                }
            }
#else
                return OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD() ||
                (Type.GetType("Mono.Runtime") != null);
#endif
            }
        }

        public static bool Detected() => File.Exists("SBRW.Launcher.Core.dll") ? Launcher_Value.System_Unix = AmI() : AmI();
#if (RELEASE_UNIX || DEBUG_UNIX)
        private static string PlatformOSName()
        {
            if (!File.Exists(@"/etc/os-release"))
            {
#if NETFRAMEWORK
                if (ID(Platform()) == PlatformIDPort.MacOSX)
                {
                    return "MacOSX";
                }
#else
                if (OperatingSystem.IsMacOS())
                {
                    return "MacOSX";
                }
                else if (OperatingSystem.IsFreeBSD())
                {
                    return "FreeBSD";
                }
                else if (OperatingSystem.IsLinux())
                {
                    return "Linux";
                }
#endif
                else
                {
                    return "Unknown System OS";
                }
            }
            else if (File.Exists(@"/etc/os-release"))
            {
                try
                {
                    using (StreamReader stream = new StreamReader(@"/etc/os-release"))
                    {
                        string Live_String_Data = string.Empty;
                        bool OS_Found = false;

                        while (((Live_String_Data = stream.ReadLine()??string.Empty) != default) && !OS_Found)
                        {
                            if (!string.IsNullOrWhiteSpace(Live_String_Data))
                            {
                                try
                                {
                                    string[] Live_Split_String = Live_String_Data.Split(new[] { '=' }, 2);
                                    if (Live_Split_String[0] == "PRETTY_NAME")
                                    {
                                        string Live_String_Value = Live_Split_String[1];

                                        if (Live_String_Value[0] == '"')
                                        {
                                            Live_String_Value = Live_String_Value.Substring(1);
                                        }

                                        if (Live_String_Value[Live_String_Value.Length - 1] == '"')
                                        {
                                            Live_String_Value = Live_String_Value.Substring(0, Live_String_Value.Length - 1);
                                        }

                                        try
                                        {
                                            return Live_String_Value;
                                        }
                                        finally
                                        {
                                            OS_Found = true;
                                            Live_String_Data = string.Empty;
                                        }
                                    }
                                }
                                catch (Exception Error)
                                {
                                    OS_Found = true;
                                    LogToFileAddons.OpenLog("Platform OS Name", string.Empty, Error, string.Empty, true);
                                }
                            }
                            else
                            {
                                OS_Found = true;
                            }
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Platform OS Release", string.Empty, Error, string.Empty, true);
                }

                return "Linux";
            }
            else if (File.Exists(@"/proc/sys/kernel/ostype"))
            {
                try
                {
                    if (File.ReadAllText(@"/proc/sys/kernel/ostype").StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
                    {
                        /* Note: Android falls into This Detection gets here too */
                        return "Linux";
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Platform OS Type", string.Empty, Error, string.Empty, true);
                }
            }
#if NET5_0_OR_GREATER
            else if (OperatingSystem.IsFreeBSD())
            {
                return "FreeBSD";
            }
            else if (OperatingSystem.IsLinux())
            {
                return "Linux";
            }
#endif
            return "Unknown System OS";
        }

        public static string FullName()
        {
            if (string.IsNullOrWhiteSpace(CacheUnixOSName))
            {
                CacheUnixOSName = PlatformOSName();
            }

            return CacheUnixOSName;
        }
#endif

        /* If Launcher Targets .Net Framework 4.7.1 then the code above is redundent and would recommend the following for Detected Boolean */
        /* https://gist.github.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38#file-unixos-cs */
    }
}
