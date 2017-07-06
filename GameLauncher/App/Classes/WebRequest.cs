using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO.Compression;

namespace GameLauncher {
    public class WebClientWithTimeout : WebClient {
        protected override WebRequest GetWebRequest(Uri address) {
            var wr = base.GetWebRequest(address);
            wr.Timeout = 1000;
            return wr;
        }
    }
}