namespace GameLauncher.App.Classes.LauncherCore.Global
{
    class URLs
    {
        public static string mainserver = "https://api.worldunited.gg";

        public static string fileserver = "https://files.worldunited.gg";

        public static string staticapiserver = "http://api-sbrw.davidcarbon.download";

        public static string secondstaticapiserver = "http://api2-sbrw.davidcarbon.download";

        public static string woplserver = "http://worldonline.pl";

        public static string modnetserver = "http://cdn.soapboxrace.world";


        public static string[] serverlisturl = new string[]
        {
            mainserver + "/serverlist.json",
            staticapiserver + "/serverlist.json",
            secondstaticapiserver + "/serverlist.json",
            woplserver + "/serverlist.json"
        };

        public static string[] cdnlisturl = new string[]
        {
            mainserver + "/cdn_list.json",
            staticapiserver + "/cdn_list.json",
            secondstaticapiserver + "/cdn_list.json",
            woplserver + "/cdn_list.json"
        };

        public static string[] anticheatreporting = new string[]
        {
            mainserver + "/report",
            "http://anticheat.worldonline.pl/report",
            "http://la-sbrw.davidcarbon.download/report?",
            "http://la2-sbrw.davidcarbon.download/report?"
        };
    }
}
