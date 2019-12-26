using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.ModNetReloaded {
    public class MainJson {
        public string basePath { get; set; }
        public string serverID { get; set; }
        public List<object> features { get; set; }
    }

    public class IndexJsonEntry
    {
        public string Name { get; set; }
        public string Checksum { get; set; }
    }

    public class IndexJson {
        public DateTime built_at { get; set; }
        public List<IndexJsonEntry> entries { get; set; }
    }

    class ModNetReloaded {
        public static string ModNetSupported(string _serverIp) {
            try {
                Uri newModNetUri = new Uri(_serverIp + "/Modding/GetModInfo");
                WebClientWithTimeout x = new WebClientWithTimeout();
                return x.DownloadString(newModNetUri);
            } catch {
                return String.Empty;
            }
        }


    }
}
