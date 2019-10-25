using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.App {
    public partial class CrashDumpUpload : Form {
        public CrashDumpUpload() {
            InitializeComponent();
        }

        private void CrashDumpUpload_Load(object sender, EventArgs e) {
            string fullpathdump = String.Empty;

            try {
                var nfsw_location = new DirectoryInfo(Self.gamedir);
                var file = (nfsw_location.GetFiles("NFSCrashDump_CL*.dmp").OrderByDescending(f => f.LastWriteTime).First()).Name;
                fullpathdump = Path.Combine(nfsw_location.FullName, file);
            }
            catch { fullpathdump = String.Empty; }

            if (fullpathdump != String.Empty) {
                WebClient dumpupload = new WebClient();

                dumpupload.UploadProgressChanged += (x, y) => uploadProgress(x, y, progressBar1);
                dumpupload.UploadFileCompleted += (x, y) => uploadCompleted(x, y, fullpathdump);
                dumpupload.UploadFileAsync(new Uri("http://launcher.worldunited.gg/dumps.php"), "POST", fullpathdump);
            } else {
                Self.Restart();
            }
        }

        private static void uploadProgress(object sender, UploadProgressChangedEventArgs e, ProgressBar progress) {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;

            progress.Value = (int)percentage;
        }

        private static void uploadCompleted(object sender, UploadFileCompletedEventArgs e, string file) {
            File.Delete(file);
            Self.Restart();
        }
    }
}
