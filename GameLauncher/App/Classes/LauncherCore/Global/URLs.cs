namespace GameLauncher.App.Classes.LauncherCore.Global
{
    class URLs
    {
        public static string Main = "https://api.worldunited.gg";

        public static string File = "https://files.worldunited.gg";

        public static string Static = "https://api-sbrw.davidcarbon.download";

        public static string Static_Alt = "http://api2-sbrw.davidcarbon.download";

        public static string ModNet = "https://cdn.soapboxrace.world";

        public static string GitHub_Launcher_Stable = "https://api.github.com/repos/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest";

        public static string GitHub_Launcher_Beta = "https://api.github.com/repos/SoapboxRaceWorld/GameLauncher_NFSW/releases";

        public static string GitHub_Updater = "https://api.github.com/repos/SoapboxRaceWorld/GameLauncherUpdater/releases/latest";

        public static string[] AntiCheatFD = new string[]
        {
            Main + "/report",
            "https://la-sbrw.davidcarbon.download/report?",
            "https://la2-sbrw.davidcarbon.download/report?"
        };

        public static string[] AntiCheatSD = new string[]
        {
            "https://la-sbrw.davidcarbon.download/report-manual?",
            "https://la2-sbrw.davidcarbon.download/report-manual?"
        };
    }
}
