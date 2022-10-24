using System;
using System.IO;

namespace SBRW.Launcher.RunTime.LauncherCore.Global
{
    class Locations
    {
        public static string NameLauncher { get { return AppDomain.CurrentDomain.FriendlyName; } }
        public static string NameUpdater { get { return "GameLauncherUpdater.exe"; } }
        public static string NameNewServersJSON { get { return "Servers-Custom.json"; } }
        public static string NameOldServersJSON { get { return "servers.json"; } }
        public static string NameLZMA { get { return "LZMA.dll"; } }
        public static string NameModLinks { get { return ".links"; } }

        public static string LauncherFolder { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static string LauncherThemeFolder { get { return Path.Combine(LauncherFolder, "Theme"); } }
        public static string LauncherDataFolder { get { return Path.Combine(LauncherFolder, "Launcher_Data"); } }

        public static string LocalAppDataFolder { get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); } }
        public static string RoamingAppDataFolder { get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); } }
        public static string RoamingAppDataFolder_Launcher { get { return Path.Combine(RoamingAppDataFolder, "Soapbox Race World", "Launcher"); } }
#if !(RELEASE_UNIX || DEBUG_UNIX)
        public static string LauncherCustomServers { get { return Path.Combine(RoamingAppDataFolder_Launcher, NameNewServersJSON); } }
#else
        public static string LauncherCustomServers { get { return Path.Combine(LauncherDataFolder); } }
#endif
        public static string UserSettingsFolder { get { return Path.Combine(RoamingAppDataFolder, "Need for Speed World", "Settings"); } }
        public static string UserSettingsXML { get { return Path.Combine(UserSettingsFolder, "UserSettings.xml"); } }

        public static string GameFilesFailSafePath { get { return Path.Combine(LauncherFolder, "Game Files"); } }
    }
}
