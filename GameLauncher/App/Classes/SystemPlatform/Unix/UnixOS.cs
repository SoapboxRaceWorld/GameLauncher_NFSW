using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.IO;

namespace GameLauncher.App.Classes.SystemPlatform.Unix
{
    class UnixOS
    {
        private static int CachedNumPlatform = 2020;

        private static string CacheUnixOSName = null;

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
                LogToFileAddons.OpenLog("Unix Platform Detection", null, Error, null, true);
                return CachedNumPlatform;
            }
        }

        public static PlatformIDPort ID(int Number)
        {
            switch (Number)
            {
                case 0:
                    return PlatformIDPort.Win32S;
                case 1:
                    return PlatformIDPort.Win32Windows;
                case 2:
                    return PlatformIDPort.Win32NT;
                case 3:
                    return PlatformIDPort.WinCE;
                case 4:
                    return PlatformIDPort.Unix;
                case 5:
                    return PlatformIDPort.Xbox;
                case 6:
                    return PlatformIDPort.MacOSX;
                case 128:
                    return PlatformIDPort.MonoLegacy;
                default:
                    return PlatformIDPort.Unknown;
            }
        }

        public static bool AmI()
        {
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
        }

        public static bool Detected() => AmI();

        private static string PlatformOSName()
        {
            if (!File.Exists("/etc/os-release"))
            {
                if (ID(Platform()) == PlatformIDPort.MacOSX)
                {
                    return "MacOSX";
                }
                else
                {
                    return "UNIX-Like System";
                }
            }
            else
            {
                using (StreamReader stream = new StreamReader("/etc/os-release"))
                {
                    string line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        string[] splits = line.Split(new[] { '=' }, 2);
                        if (splits[0] == "PRETTY_NAME")
                        {
                            string val = splits[1];

                            if (val[0] == '"')
                            {
                                val = val.Substring(1);
                            }

                            if (val[val.Length - 1] == '"')
                            {
                                val = val.Substring(0, val.Length - 1);
                            }

                            return val;
                        }
                    }
                }

                return "Linux";
            }
        }

        public static string FullName()
        {
            if (string.IsNullOrWhiteSpace(CacheUnixOSName))
            {
                CacheUnixOSName = PlatformOSName();
            }

            return CacheUnixOSName;
        }

        /* If Launcher Targets .Net Framework 4.7.1 then the code above is redundent and would recommend the following for Detected Boolean */
        /* https://gist.github.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38#file-unixos-cs */
    }
}
