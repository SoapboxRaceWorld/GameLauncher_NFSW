using Flurl;
using Flurl.Http;
using Flurl.Http.Content;
using GameLauncher.App.Classes.RPC;
using GameLauncherReborn;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Extensions;
using Nancy.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameLauncher.App.Classes.Logger;
using Url = Flurl.Url;

namespace GameLauncher.App.Classes.Proxy
{

    public class Helper
    {
        public static int session = 0;
        public static String personaid = String.Empty;
    }

    /* Moved "ProxyHandler : IApplicationStartup" to Gist */
    /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/d7f8b9a506333f4c3439a28afb43b9b9fd3db868/ProxyHandler.cs */
}
