using Newtonsoft.Json;

namespace GameLauncher.App.Classes.LauncherCore.Lists.JSON
{
    /* http://localhost/serverlist.json */
    public class ServerList
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ip_address")]
        public string IPAddress { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("discord_application_id")]
        public string DiscordAppID { get; set; }

        [JsonIgnore]
        public bool IsSpecial { get; set; }
    }

    class SelectedServer
    {
        public static ServerList List;
        public static ServerList Data
        {
            get { return List; }
            set { List = value; }
        }
    }
}
