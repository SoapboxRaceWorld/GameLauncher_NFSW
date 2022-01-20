using SBRW.Launcher.App.Classes.SystemPlatform.Unix;
using System;
using System.IO;

namespace SBRW.Launcher.App.Classes.LauncherCore.Global
{
    class Locations
    {
        public static string NameLauncher { get; set; } = AppDomain.CurrentDomain.FriendlyName;
        public static string NameUpdater { get; set; } = "GameLauncherUpdater.exe";
        public static string NameNewServersJSON { get; set; } = "Servers-Custom.json";
        public static string NameOldServersJSON { get; set; } = "servers.json";
        public static string NameLZMA { get; set; } = "LZMA.dll";
        public static string NameModLinks { get; set; } = ".links";

        public static string LauncherFolder { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string LauncherThemeFolder { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Theme");

        public static string LocalAppDataFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string RoamingAppDataFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string RoamingAppDataFolder_Launcher { get; set; } = Path.Combine(RoamingAppDataFolder, "Soapbox Race World", "Launcher");

        public static string LauncherCustomServers { get; set; } = UnixOS.Detected() ? NameNewServersJSON : Path.Combine(RoamingAppDataFolder_Launcher, NameNewServersJSON);

        public static string UserSettingsFolder { get; set; } = Path.Combine(RoamingAppDataFolder, "Need for Speed World", "Settings");
        public static string UserSettingsXML { get; set; } = Path.Combine(UserSettingsFolder, "UserSettings.xml");

        public static string GameFilesFailSafePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Files");
    }
}
