using System.Collections.Generic;

namespace GameLauncher.App.Classes.LauncherCore.Lists.JSON
{
    /* http://localhost/Engine.svc/GetServerInformation */
    public class GetServerInformation
    {
        public string messageSrv { get; set; }
        public string homePageUrl { get; set; }
        public string facebookUrl { get; set; }
        public string twitterUrl { get; set; }
        public string discordUrl { get; set; }
        public string serverName { get; set; }
        public string country { get; set; }
        public int? timezone { get; set; }
        public string bannerUrl { get; set; }
        public string adminList { get; set; }
        public string ownerList { get; set; }
        public int numberOfRegistered { get; set; }
        public int secondsToShutDown { get; set; }
        public string allowedCountries { get; set; }
        public string modsUrl { get; set; }
        public List<string> activatedHolidaySceneryGroups { get; set; }
        public List<string> disactivatedHolidaySceneryGroups { get; set; }
        public int onlineNumber { get; set; }
        public string requireTicket { get; set; }
        public string playerCountRewardMultiplier { get; set; }
        public bool happyHourEnabled { get; set; }
        public string happyHourMultipler { get; set; }
        public string serverVersion { get; set; }
        public int maxUsersAllowed { get; set; }
        public int maxOnlinePlayers { get; set; }
        public bool rwacallow { get; set; }
        public bool enforceLauncherProxy { get; set; }
        public string authHash { get; set; }
        public bool modernAuthSupport { get; set; }
        public string webSignupUrl { get; set; }
        public string webPanelUrl { get; set; }
        public string iconUrl { get; set; }
        public string webRecoveryUrl { get; set; }
        public string discordApplicationID { get; set; }
        public string passwordResetUrl { get; set; }
        public string freeroamMapSocket { get; set; }
        public List<string> freeroamServers { get; set; }
    }

    /* Moved "FreeroamObject" to Gist */
    /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/0fc7275667c051cebd2e2b1bdab2dc64bca568f7/JsonScheme.cs */
}
