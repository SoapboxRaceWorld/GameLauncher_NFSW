using GameLauncher.App.Classes.SystemPlatform.Windows;

namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    class FileSettingsSave
    {
        public static IniFile settingFile = new IniFile("Settings.ini");

        public static string CDN = settingFile.Read("CDN");

        public static string Lang = settingFile.Read("Language");

        public static string Proxy = settingFile.Read("DisableProxy");

        public static string RPC = settingFile.Read("DisableRPC");

        public static string GameInstallation = settingFile.Read("InstallationDirectory");

        public static string IgnoreVersion = settingFile.Read("IgnoreUpdateVersion");

        public static string FirewallStatus = settingFile.Read("Firewall");

        public static string WindowsDefenderStatus = settingFile.Read("WindowsDefender");

        public static void NullSafeSettings()
        {
            if (!settingFile.KeyExists("CDN") || string.IsNullOrEmpty(settingFile.Read("CDN")))
            {
                settingFile.Write("CDN", "http://localhost");
            }

            if (!settingFile.KeyExists("Language") || string.IsNullOrEmpty(settingFile.Read("Language")))
            {
                settingFile.Write("Language", "en");
            }

            if (!settingFile.KeyExists("DisableProxy") || string.IsNullOrEmpty(settingFile.Read("DisableProxy")))
            {
                settingFile.Write("DisableProxy", "0");
            }

            if (!settingFile.KeyExists("DisableRPC") || string.IsNullOrEmpty(settingFile.Read("DisableRPC")))
            {
                settingFile.Write("DisableRPC", "0");
            }

            if (!settingFile.KeyExists("IgnoreUpdateVersion"))
            {
                settingFile.Write("IgnoreUpdateVersion", string.Empty);
            }
            
            if (!DetectLinux.LinuxDetected())
            {
                if (!settingFile.KeyExists("Firewall") || string.IsNullOrEmpty(settingFile.Read("Firewall")))
                {
                    settingFile.Write("Firewall", "Not Excluded");
                }

                if (WindowsProductVersion.GetWindowsNumber() >= 10.0)
                {
                    if (!settingFile.KeyExists("WindowsDefender") || string.IsNullOrEmpty(settingFile.Read("WindowsDefender")))
                    {
                        settingFile.Write("WindowsDefender", "Not Excluded");
                    }
                }
            }

            settingFile = new IniFile("Settings.ini");
        }

        public static void SaveSettings()
        {
            if (settingFile.Read("CDN") != CDN)
            {
                settingFile.Write("CDN", CDN);
            }

            if (settingFile.Read("Language") != Lang)
            {
                settingFile.Write("Language", Lang);
            }

            if (settingFile.Read("DisableProxy") != Proxy)
            {
                settingFile.Write("DisableProxy", Proxy);
            }

            if (settingFile.Read("DisableRPC") != RPC)
            {
                settingFile.Write("DisableRPC", RPC);
            }

            if (settingFile.Read("InstallationDirectory") != GameInstallation)
            {
                settingFile.Write("InstallationDirectory", GameInstallation);
            }

            if (settingFile.Read("IgnoreUpdateVersion") != IgnoreVersion)
            {
                settingFile.Write("IgnoreUpdateVersion", IgnoreVersion);
            }

            if (!DetectLinux.LinuxDetected())
            {
                if (settingFile.Read("Firewall") != FirewallStatus)
                {
                    settingFile.Write("Firewall", FirewallStatus);
                }

                if (WindowsProductVersion.GetWindowsNumber() >= 10.0)
                {
                    if (settingFile.Read("WindowsDefender") != WindowsDefenderStatus)
                    {
                        settingFile.Write("WindowsDefender", WindowsDefenderStatus);
                    }
                }
            }

            settingFile = new IniFile("Settings.ini");
        }
    }
}
