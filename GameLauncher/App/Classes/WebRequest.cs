using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO.Compression;
using System.Windows.Forms;

namespace GameLauncherReborn {
    public class WebClientWithTimeout : WebClient {
        protected override WebRequest GetWebRequest(Uri address) {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);

                request.UserAgent = "GameLauncher (+https://github.com/metonator/GameLauncher_NFSW)";
                request.Timeout = 3000;
                return request;
        }
    }
}