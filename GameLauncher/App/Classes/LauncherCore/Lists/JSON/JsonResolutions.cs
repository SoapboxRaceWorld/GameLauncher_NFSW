using Newtonsoft.Json;

namespace GameLauncher.App.Classes.LauncherCore.Lists.JSON
{
    public class JsonResolutions
    {
        [JsonProperty("resolution")]
        public string Resolution { get; set; }

        [JsonProperty("dmPelsWidth")]
        public string Width { get; set; }

        [JsonProperty("dmPelsHeight")]
        public string Height { get; set; }
    }
}
