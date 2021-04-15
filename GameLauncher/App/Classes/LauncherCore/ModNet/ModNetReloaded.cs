using System;
using System.Net;
using System.Collections.Generic;

namespace GameLauncher.App.Classes.LauncherCore.ModNet
{
    public class MainJson
    {
        public string basePath { get; set; }
        public string serverID { get; set; }
        public List<object> features { get; set; }
    }

    public class IndexJsonEntry
    {
        public string Name { get; set; }
        public string Checksum { get; set; }
    }

    public class IndexJson
    {
        public DateTime built_at { get; set; }
        public List<IndexJsonEntry> entries { get; set; }
    }

    class ModNetReloaded
    {
        public static string ModNetSupported(string _serverIp)
        {
            try
            {
                Uri newModNetUri = new Uri(_serverIp + "/Modding/GetModInfo");
                WebClient x = new WebClient();
                return x.DownloadString(newModNetUri);
            }
            catch(Exception)
            {
                return String.Empty;
            }
        }
    }
}
