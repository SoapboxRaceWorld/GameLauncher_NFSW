using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO.Compression;

namespace GameLauncher
{
    public class WebClientWithTimeout : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest wr = base.GetWebRequest(address) as HttpWebRequest;
            wr.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            wr.Timeout = 3000;

            return wr;
        }
    }
}
