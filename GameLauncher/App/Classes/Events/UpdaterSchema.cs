using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLauncher {
    public class Update {
        public bool info { get; set; }
        public string download { get; set; }
    }

    public class CheckVersion {
        public string current_version { get; set; }
        public string github_build { get; set; }
        public Update update { get; set; }
    }
}
