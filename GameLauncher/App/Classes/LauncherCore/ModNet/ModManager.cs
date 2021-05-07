using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using Newtonsoft.Json;

namespace GameLauncher.App.Classes.LauncherCore.ModNet
{
    /* http://localhost/Engine.svc/GetServerInformation */

    public class ModFile
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }
    }

    /* http://localhost/Engine.svc/Modding/GetModInfo */
    public class ModInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public long UpdatedAt { get; set; }

        [JsonProperty("files")]
        public List<ModFile> Files { get; set; }

        [JsonProperty("required_mods")]
        public List<string> RequiredMods { get; set; }
    }

    public static class ModManager
    {
        public static void ResetModDat(string gameDir)
        {
            File.Delete(Path.Combine(gameDir, "ModManager.dat"));
        }
    }
}
