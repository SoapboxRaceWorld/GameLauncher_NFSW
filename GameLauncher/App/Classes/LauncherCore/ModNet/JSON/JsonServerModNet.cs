using System;
using System.Collections.Generic;

namespace GameLauncher.App.Classes.LauncherCore.ModNet.JSON
{
    public class MainJson
    {
        public string basePath { get; set; }
        public string serverID { get; set; }
        public List<object> features { get; set; }
    }

    public class IndexJsonEntry
    {
        public string Name { get; set; }
        public string Checksum { get; set; }
    }

    public class IndexJson
    {
        public DateTime built_at { get; set; }
        public List<IndexJsonEntry> entries { get; set; }
    }
}
