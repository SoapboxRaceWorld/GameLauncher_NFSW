using GameLauncherReborn;

namespace GameLauncher.App.Classes.LauncherCore.APICheckers
{
    class VisualsAPIChecker
    {
        public static bool UnitedAPI = true;

        public static bool CarbonAPI = true;

        public static bool CarbonAPITwo = true;

        public static bool WOPLAPI = true;

        public static void PingAPIStatus()
        {
            switch (APIStatusChecker.CheckStatus(Self.mainserver + "/serverlist.json"))
            {
                case API.Online:
                    break;
                default:
                    UnitedAPI = false;
                    break;
            }

            if (UnitedAPI == false)
            {
                switch (APIStatusChecker.CheckStatus(Self.staticapiserver + "/serverlist.json"))
                {
                    case API.Online:
                        break;
                    default:
                        CarbonAPI = false;
                        break;
                }
            }

            if (CarbonAPI == false)
            {
                switch (APIStatusChecker.CheckStatus(Self.secondstaticapiserver + "/serverlist.json"))
                {
                    case API.Online:
                        break;
                    default:
                        CarbonAPITwo = false;
                        break;
                }
            }

            if (CarbonAPITwo == false)
            {
                switch (APIStatusChecker.CheckStatus(Self.woplserver + "/serverlist.json"))
                {
                    case API.Online:
                        break;
                    default:
                        WOPLAPI = false;
                        break;
                }
            }
        }
    }
}
