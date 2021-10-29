using GameLauncher.App.Classes.SystemPlatform.Unix;
using System;
using System.IO;

namespace GameLauncher.App.Classes.LauncherCore.Global
{
    class Locations
    {
        public static readonly string NameLauncher = AppDomain.CurrentDomain.FriendlyName;
        public static readonly string NameUpdater = "GameLauncherUpdater.exe";
        public static readonly string NameNewServersJSON = "Servers-Custom.json";
        public static readonly string NameOldServersJSON = "servers.json";
        public static readonly string NameLZMA = "LZMA.dll";
        public static readonly string NameAccountIni = "Account.ini";
        public static readonly string NameSettingsIni = "Settings.ini";
        public static readonly string NameModLinks = ".links";

        public static readonly string LauncherFolder = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string Launcher_Settings = UnixOS.Detected() ? NameSettingsIni : Path.Combine(LauncherFolder, NameSettingsIni);
        public static readonly string LauncherThemeFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Theme");

        public static readonly string LocalAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static readonly string RoamingAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string RoamingAppDataFolder_Launcher = Path.Combine(RoamingAppDataFolder, "Soapbox Race World", "Launcher");
        public static readonly string RoamingAppDataFolder_Launcher_Account = UnixOS.Detected() ? NameAccountIni : Path.Combine(RoamingAppDataFolder_Launcher, NameAccountIni);

        public static readonly string LauncherCustomServers = UnixOS.Detected() ? NameNewServersJSON : Path.Combine(RoamingAppDataFolder_Launcher, NameNewServersJSON);

        public static readonly string UserSettingsFolder = Path.Combine(RoamingAppDataFolder, "Need for Speed World", "Settings");
        public static readonly string UserSettingsXML = Path.Combine(UserSettingsFolder, "UserSettings.xml");

        public static readonly string GameFilesFailSafePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Files");
    }
}
