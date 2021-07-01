using System;

namespace GameLauncher.App.Classes.LauncherCore.Proxy
{
    [Serializable]
    public class ProxyException : Exception
    {
        public ProxyException()
        {
        }

        public ProxyException(string message) : base(message)
        {
        }

        public ProxyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}