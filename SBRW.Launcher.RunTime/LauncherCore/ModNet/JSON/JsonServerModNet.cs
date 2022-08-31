using System;
using System.Collections.Generic;

namespace SBRW.Launcher.RunTime.LauncherCore.ModNet.JSON
{
    public class GetModInfo
    {
        public string basePath { get; set; } = string.Empty;
        public string serverID { get; set; } = string.Empty;
        public List<object> features { get; set; } = new List<object>();
    }

    public class ServerModFileEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Checksum { get; set; } = string.Empty;
    }

    public class ServerModList
    {
        public DateTime built_at { get; set; }
        public List<ServerModFileEntry> entries { get; set; } = new List<ServerModFileEntry>();
    }
}
