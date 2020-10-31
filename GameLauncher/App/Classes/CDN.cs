using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes {
    class CDN {
        public static string CDNUrl = String.Empty;
        public static string TrackHigh = String.Empty;
    }

    public class CDNObject {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
