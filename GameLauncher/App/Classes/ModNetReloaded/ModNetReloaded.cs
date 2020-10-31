using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.ModNetReloaded {
    public class MainJson {
        public string BasePath { get; set; }
        public string ServerID { get; set; }
        public List<object> Features { get; set; }
    }

    public class IndexJsonEntry
    {
        public string Name { get; set; }
        public string Checksum { get; set; }
    }

    public class IndexJson {
        public DateTime Built_At { get; set; }
        public List<IndexJsonEntry> Entries { get; set; }
    }

    class ModNetReloaded {
        public static string ModNetSupported(string _serverIp) {
            try {
                Uri newModNetUri = new Uri(_serverIp + "/Modding/GetModInfo");
                WebClientWithTimeout x = new WebClientWithTimeout();
                return x.DownloadString(newModNetUri);
            } catch(Exception) {
                return String.Empty;
            }
        }


    }
}
