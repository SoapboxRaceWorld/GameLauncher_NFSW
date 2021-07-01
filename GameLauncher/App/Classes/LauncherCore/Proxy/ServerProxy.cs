using System;
using GameLauncher.App.Classes.Logger;
using Nancy.Hosting.Self;

namespace GameLauncher.App.Classes.LauncherCore.Proxy
{
    public class ServerProxy : Singleton<ServerProxy>
    {
        public static int ProxyPort;

        private string _serverUrl;
        private string _serverName;
        public static NancyHost Host;

        /// <summary>
        /// Boolean Value on Launcher Proxy if its Running
        /// </summary>
        /// <returns>True or False</returns>
        public static bool Running() => Host != null;

        public string GetServerUrl() => _serverUrl;
        public void SetServerUrl(string serverUrl)
        {
            _serverUrl = serverUrl;
        }

        public string GetServerName() => _serverName;
        public void SetServerName(string serverName)
        {
            _serverName = serverName;
        }

        public void Start(string From)
        {
            if (Running())
            {
                Log.Warning("PROXY: Local Proxy Server Already Running! (" + From + ")");
            }
            else
            {
                Log.Info("PROXY: Local Proxy Server has Fully Initialized (" + From + ")");

                var hostConfigs = new HostConfiguration()
                {
                    UrlReservations = new UrlReservations()
                    {
                        CreateAutomatically = true,
                    },
                    AllowChunkedEncoding = false
                };

                Host = new NancyHost(new Uri("http://127.0.0.1:" + ProxyPort), new NancyBootstrapper(), hostConfigs);
                Host.Start();
            }
        }

        public void Stop(string From)
        {
            if (Running())
            {
                Log.Info("PROXY: Local Proxy Server has Shutdown (" + From + ")");
                Host.Stop();
                Host.Dispose();
                Host = null;
            }
            else
            {
                Log.Warning("PROXY: Local Proxy Server is already Shutdown (" + From + ")");
            }
        }
    }
}