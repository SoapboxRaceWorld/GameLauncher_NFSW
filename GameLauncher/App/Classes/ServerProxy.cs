using System;
using Nancy.Hosting.Self;

namespace GameLauncher.App.Classes
{
    /// <summary>
    /// Class to handle proxying game server requests.
    /// This should allow for the usage of CloudFlare, SSL, HTTP/2, etc.
    /// </summary>
    public class ServerProxy : Singleton<ServerProxy>
    {
        private string _serverUrl;
        private NancyHost _host;

        /// <summary>
        /// Get the current server URL.
        /// </summary>
        /// <returns></returns>
        public string GetServerUrl() => _serverUrl;

        /// <summary>
        /// Set the server URL that the proxy should use.
        /// </summary>
        /// <param name="serverUrl"></param>
        public void SetServerUrl(string serverUrl)
        {
            _serverUrl = serverUrl;
        }

        /// <summary>
        /// Start the proxy.
        /// </summary>
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
