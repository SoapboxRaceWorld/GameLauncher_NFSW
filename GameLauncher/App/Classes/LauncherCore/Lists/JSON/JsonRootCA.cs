using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameLauncher.App.Classes.LauncherCore.Lists.JSON
{
    public class JsonRootCA
    {
        [JsonProperty("CommonName")]
        public string CN { get; set; }

        [JsonProperty("Subject")]
        public string Subject { get; set; }

        [JsonProperty("Issuer")]
        public string Issuer { get; set; }

        [JsonProperty("IDS")]
        public List<IdsModel> Ids { get; set; }

        [JsonProperty("Hashes")]
        public List<HashesModel> Hashes { get; set; }

        [JsonProperty("Valid")]
        public List<DateModel> Valid { get; set; }

        [JsonProperty("File")]
        public List<FileModel> File { get; set; }
    }

    public class FileModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("CER")]
        public string Cer { get; set; }
    }

    public class IdsModel
    {
        [JsonProperty("Fingerprint")]
        public string Fingerprint { get; set; }

        [JsonProperty("Thumbprint")]
        public string Thumbprint { get; set; }

        [JsonProperty("Serial")]
        public string Serial { get; set; }
    }

    public class DateModel
    {
        [JsonProperty("From")]
        public string From { get; set; }

        [JsonProperty("To")]
        public string To { get; set; }
    }

    public class HashesModel
    {
        [JsonProperty("MD5")]
        public string HashMD { get; set; }

        [JsonProperty("SHA1")]
        public string HashSHA { get; set; }

        [JsonProperty("SHA256")]
        public string HashSHATwo { get; set; }
    }
}
