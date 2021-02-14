using Newtonsoft.Json;

namespace GameLauncher.App.Classes.LauncherCore.Lists.JSON
{
    public class LangObject
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("xml_value")]
        public string XML_Value { get; set; }

        [JsonProperty("ini_value")]
        public string INI_Value { get; set; }

        [JsonIgnore]
        public bool IsSpecial { get; set; }
    }
}
