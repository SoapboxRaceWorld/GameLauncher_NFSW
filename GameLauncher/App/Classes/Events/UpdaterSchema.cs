using Newtonsoft.Json;

namespace GameLauncher.App.Classes.Events
{
    public class Update
    {
        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }
    }

    public class CheckVersion
    {
        [JsonProperty("client_version")]
        public string ClientVersion { get; set; }

        [JsonProperty("latest_version")]
        public string LatestVersion { get; set; }

        [JsonProperty("update_exists")]
        public bool UpdateExists { get; set; }
        
        [JsonProperty("update")]
        public Update Update { get; set; }
    }

    public class UpdateCheckResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("payload")]
        public CheckVersion Payload { get; set; }
    }
}
