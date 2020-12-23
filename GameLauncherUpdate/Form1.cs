using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.IO.Compression;
using System.Threading;
using SimpleJSON;
using System.Web.Script.Serialization;
using GameLauncherUpdater.App;

namespace GameLauncherUpdater
{
    public partial class Form1 : Form {
        string tempNameZip = Path.GetTempFileName();
        string version;
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        public Form1() {
            InitializeComponent();
        }

        public void error(string error) {
            information.Text = error.ToString();
            Delay.WaitSeconds(2);
            Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
        }

        public void success(string success) {
            information.Text = success.ToString();
        }

        public void update() {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length == 2) {
                Process.GetProcessById(Convert.ToInt32(args[1])).Kill();
            }

            if (File.Exists("GameLauncher.exe")) {
                var versionInfo = FileVersionInfo.GetVersionInfo("GameLauncher.exe");
                version = versionInfo.ProductVersion;
            } else {
                version = "0.0.0.0";
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            /*
            var client = new WebClient();
            Uri StringToUri = new Uri("https://api2.worldunited.gg/update.php?version=" + version);
            client.CancelAsync();
            client.DownloadStringAsync(StringToUri);
            client.DownloadStringCompleted += (sender2, e2) => {
                try
                {
                    JSONNode json = JSON.Parse(e2.Result);

                    if (json["payload"]["update_exists"] != false)
                    {
                        Thread thread = new Thread(() => {
                            WebClient client2 = new WebClient();
                            client2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                            client2.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                            client2.DownloadFileAsync(new Uri(json["payload"]["update"]["download_url"]), tempNameZip);
                        });
                        thread.Start();
                    }
                    else
                    {
                        Process.Start(@"GameLauncher.exe");
                        error("Starting GameLauncher.exe");
                    }
                }
                catch (Exception ex)
                {
                    error("Failed to update.\n" + ex.Message);
                }
            };
            */
            var client3 = new WebClient();
            Uri StringToUri2 = new Uri("https://api.github.com/repos/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
            client3.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client3.CancelAsync();
            client3.DownloadStringAsync(StringToUri2);
            client3.DownloadStringCompleted += (sender3, e3) => {
                try
                {
                    /*
                    var gitHubUri = new Uri("https://api.github.com/repos/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");

                    var json = ApiRequest.GetJson(gitHubUri);

                    var jsonObject = Serializer.Deserialize<ReleaseModel>(json);
                    */
                    ReleaseModel json = new JavaScriptSerializer().Deserialize<ReleaseModel>(e3.Result);

                    if (version != json.tag_name)
                    {
                        Thread thread = new Thread(() => {
                            WebClient client4 = new WebClient();
                            client4.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                            client4.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                            client4.DownloadFileAsync(new Uri("http://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases/download/" + json.tag_name + "/Release_" + json.tag_name + ".zip"), tempNameZip);
                        });
                        thread.Start();
                    }
                    else
                    {
                        Process.Start(@"GameLauncher.exe");
                        error("Starting GameLauncher.exe");
                    }
                }
                catch (Exception ex)
                {
                    error("Failed to update.\n" + ex.Message);
                }
            };
        }

        private string FormatFileSize(long byteCount) {
            double[] numArray = new double[] { 1073741824, 1048576, 1024, 0 };
            string[] strArrays = new string[] { "GB", "MB", "KB", "Bytes" };
            for (int i = 0; i < (int)numArray.Length; i++) {
                if ((double)byteCount >= numArray[i]) {
                    return string.Concat(string.Format("{0:0.00}", (double)byteCount / numArray[i]), strArrays[i]);
                }
            }

            return "0 Bytes";
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            this.BeginInvoke((MethodInvoker)delegate {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                information.Text = "Downloaded " + FormatFileSize(e.BytesReceived) + " of " + FormatFileSize(e.TotalBytesToReceive);
                downloadProgress.Style = ProgressBarStyle.Blocks;
                downloadProgress.Value = int.Parse(Math.Truncate(percentage).ToString());
            });
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            this.BeginInvoke((MethodInvoker)delegate {
                downloadProgress.Style = ProgressBarStyle.Marquee;

                string updatePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
                using (ZipArchive archive = ZipFile.OpenRead(tempNameZip)) {
                    int numFiles = archive.Entries.Count;
                    int current = 1;

                    downloadProgress.Style = ProgressBarStyle.Blocks;

                    foreach (ZipArchiveEntry entry in archive.Entries) {
                        string fullName = entry.FullName;

                        if (fullName.Substring(fullName.Length - 1) == "/") {
                            string folderName = fullName.Remove(fullName.Length - 1);
                            if (Directory.Exists(folderName)) {
                                Directory.Delete(folderName, true);
                            }

                            Directory.CreateDirectory(folderName);
                        } else {
                            if (fullName != "GameLauncherUpdater.exe") {
                                if (File.Exists(fullName)) {
                                    File.Delete(fullName);
                                }

                                information.Text = "Extracting: " + fullName;
								try { entry.ExtractToFile(Path.Combine(updatePath, fullName)); } catch { }
                                Delay.WaitMSeconds(200);
                            }
                        }

                        downloadProgress.Value = (int)((long)100 * current / numFiles);
                        current++;
                    }
                }

                Process.Start(@"GameLauncher.exe");
                error("Update completed. Starting GameLauncher.exe");
            });
        }

        private void Form1_Load(object sender, EventArgs e) {
			this.BeginInvoke((MethodInvoker)delegate {
				update();
			});
		}
    }
}
