using GameLauncher.App.Classes.LauncherCore.Logger;
using Nancy.Hosting.Self;
using System;

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
            try
            {
                if (Running())
                {
                    Log.Warning("PROXY: Local Proxy Server already Running! (" + From + ")");
                }
                else
                {
                    Log.Info("PROXY: Local Proxy Server has Fully Initialized (" + From + ")");

                    HostConfiguration Configs = new HostConfiguration()
                    {
                        AllowChunkedEncoding = false,
                        RewriteLocalhost = false,
                        UrlReservations = new UrlReservations()
                        {
                            CreateAutomatically = true
                        }
                    };

                    Host = new NancyHost(new Uri("http://127.0.0.1:" + ProxyPort), new NancyBootstrapper(), Configs);
                    Host.Start();
                }
            }
            catch (AutomaticUrlReservationCreationFailureException Error)
            {
                LogToFileAddons.OpenLog("PROXY [U.R.]", null, Error, null, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("PROXY", null, Error, null, true);
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