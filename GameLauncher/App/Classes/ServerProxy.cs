using System;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.Proxy;
using GameLauncherReborn;
using Nancy.Hosting.Self;

namespace GameLauncher.App.Classes
{
    public class ServerProxy : Singleton<ServerProxy>
    {
        private string _serverUrl;
        private string _serverName;
        private NancyHost _host;

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

        public void Start()
        {
            Log.Debug("Setting Proxy under :"+Self.ProxyPort);

            if (_host != null) {
                Log.Error("Proxy is already running!");
                throw new Exception("Server already running!");
            }

            try { 
                _host = new NancyHost(new Uri("http://127.0.0.1:" + Self.ProxyPort), new NancyBootstrapper(), new HostConfiguration {
                    AllowChunkedEncoding = false,
                    UrlReservations = new UrlReservations() { CreateAutomatically = true }
                });

                Log.Debug("Starting Proxy");
                _host.Start();
            } catch(Exception ex) {
                Log.Error(ex.Message);
            }
        }

        public void Stop()
        {
            _host?.Stop();
        }
    }
}
