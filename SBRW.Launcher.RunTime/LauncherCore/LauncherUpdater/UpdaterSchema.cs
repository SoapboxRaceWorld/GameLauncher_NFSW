using Newtonsoft.Json;
using System.Collections.Generic;

namespace SBRW.Launcher.RunTime.LauncherCore.LauncherUpdater
{
    public class GitHubRelease
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("assets")]
        public List<AssetModel> Assets { get; set; } = new List<AssetModel>();

        public class AssetModel
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            [JsonProperty("browser_download_url")]
            public string Url { get; set; } = string.Empty;
        }

        [JsonProperty("prerelease")]
        public bool Pre_Release { get; set; }

        [JsonProperty("zipball_url")]
        public string Url_Zipball { get; set; } = string.Empty;

        [JsonProperty("body")]
        public string Body { get; set; } = string.Empty;
    }
}
