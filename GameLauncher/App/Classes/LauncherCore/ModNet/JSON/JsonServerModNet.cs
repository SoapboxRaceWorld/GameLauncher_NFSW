using System;
using System.Collections.Generic;

namespace GameLauncher.App.Classes.LauncherCore.ModNet.JSON
{
    public class GetModInfo
    {
        public string basePath { get; set; }
        public string serverID { get; set; }
        public List<object> features { get; set; }
    }

    public class ServerModFileEntry
    {
        public string Name { get; set; }
        public string Checksum { get; set; }
    }

    public class ServerModList
    {
        public DateTime built_at { get; set; }
        public List<ServerModFileEntry> entries { get; set; }
    }
}
