using System.Collections.Generic;

namespace GameLauncherUpdater.App
{
    class ReleaseModel
    {
        public string tag_name { get; set; }

        public string name { get; set; }

        public List<AssetModel> assets { get; set; }

        public class AssetModel
        {
            public string name { get; set; }

            public string browser_download_url { get; set; }
        }
    }
}
