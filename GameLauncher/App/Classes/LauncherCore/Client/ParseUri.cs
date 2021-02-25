using System;
using System.Linq;

namespace GameLauncher.App.Classes.LauncherCore.Client
{
    class ParseUri
    {
        public String[] Uri;

        public ParseUri(String[] CommandLineUri)
        {
            Uri = CommandLineUri;
        }

        public bool IsDiscordPresent()
        {
            return Uri.Contains("--discord");
        }
    }
}
