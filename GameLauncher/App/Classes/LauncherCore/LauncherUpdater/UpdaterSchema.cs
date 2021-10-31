using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameLauncher.App.Classes.LauncherCore.LauncherUpdater
{
    public class GitHubRelease
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("assets")]
        public List<AssetModel> Assets { get; set; }

        public class AssetModel
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("browser_download_url")]
            public string Url { get; set; }
        }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
