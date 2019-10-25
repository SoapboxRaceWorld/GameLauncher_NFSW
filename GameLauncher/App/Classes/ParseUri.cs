using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes {
    class ParseUri {
        String[] Uri;

        public ParseUri(String[] CommandLineUri) {
            Uri = CommandLineUri;
        }

        public bool IsDiscordPresent() {
            return Uri.Contains("--discord");
        }
    }
}
