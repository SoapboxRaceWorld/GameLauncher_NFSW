using Newtonsoft.Json;

namespace GameLauncher.App.Classes.LauncherCore.Lists.JSON
{
    /* http://localhost/cdn_list.json */
    public class CDNList
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonIgnore]
        public bool IsSpecial { get; set; }
    }
}
