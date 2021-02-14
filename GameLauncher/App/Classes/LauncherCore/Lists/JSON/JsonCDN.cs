using Newtonsoft.Json;
using System;

namespace GameLauncher.App.Classes.LauncherCore.Lists.JSON
{
    class JsonCDN
    {
        public static string CDNUrl = String.Empty;
        public static string TrackHigh = String.Empty;
    }

    public class CDNObject
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
