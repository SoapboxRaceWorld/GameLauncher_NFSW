using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.RPC {
    class OfflineSaveFile {
        public static void SaveContent(string url, string content) {
            string OfflineInstallDir = Path.Combine(Directory.GetCurrentDirectory(), "OfflineBuild", ServerProxy.Instance.GetServerName().Replace(" ", "_"));

            Directory.CreateDirectory(OfflineInstallDir + Path.GetDirectoryName(url));
            File.WriteAllText(OfflineInstallDir + url + ".xml", content);

            //Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "OfflineBuild", Path.GetDirectoryName(url)));
            //File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "OfflineBuild", url) + ".xml", content);
        }
    }
}
