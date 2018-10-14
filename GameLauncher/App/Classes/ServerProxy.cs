using System;
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
            if (_host != null)
            {
                throw new Exception("Server already running!");
            }

            _host = new NancyHost(new HostConfiguration
            {
                AllowChunkedEncoding = false
            }, new Uri("http://127.0.0.1:6262"));
            _host.Start();
        }

        public void Stop()
        {
            _host?.Stop();
        }
    }
}
