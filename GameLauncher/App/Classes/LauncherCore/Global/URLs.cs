namespace GameLauncher.App.Classes.LauncherCore.Global
{
    class URLs
    {
        public static string Main = "https://api.worldunited.gg";

        public static string File = "https://files.worldunited.gg";

        public static string Static = "https://api-sbrw.davidcarbon.download";

        public static string Static_Alt = "https://api2-sbrw.davidcarbon.download";

        public static string WOPL = "http://worldonline.pl";

        public static string ModNet = "https://cdn.soapboxrace.world";

        public static string GitHub_Launcher = "https://api.github.com/repos/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest";

        public static string GitHub_Updater = "https://api.github.com/repos/SoapboxRaceWorld/GameLauncherUpdater/releases/latest";

        public static string[] ServerList = new string[]
        {
            Main + "/serverlist.json",
            Static + "/serverlist.json",
            Static_Alt + "/serverlist.json",
            WOPL + "/serverlist.json"
        };

        public static string[] CDNList = new string[]
        {
            Main + "/cdn_list.json",
            Static + "/cdn_list.json",
            Static_Alt + "/cdn_list.json",
            WOPL + "/cdn_list.json"
        };

        public static string[] AntiCheatFD = new string[]
        {
            Main + "/report",
            "http://anticheat.worldonline.pl/report",
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
