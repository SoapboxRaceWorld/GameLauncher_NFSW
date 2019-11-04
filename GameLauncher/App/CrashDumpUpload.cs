using GameLauncher.App.Classes;
using GameLauncherReborn;
using Nancy.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
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

                //GPU List
                List<string> GPUs = new List<string>();
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                string graphicsCard = string.Empty;
                foreach (ManagementObject mo in searcher.Get())
                {
                    foreach (PropertyData property in mo.Properties)
                    {
                        GPUs.Add(property.Value.ToString());
                    }
                }
                var GPU = string.Join(" | ", GPUs);

                //CPU
                string CPU = (from x in new ManagementObjectSearcher("SELECT Name FROM Win32_Processor").Get().Cast<ManagementObject>() select x.GetPropertyValue("Name")).FirstOrDefault().ToString();

                //RAM
                Kernel32.GetPhysicallyInstalledSystemMemory(out long RAM);

                //OS
                string OS = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>() select x.GetPropertyValue("Caption")).FirstOrDefault().ToString();

                NameValueCollection postData = new NameValueCollection();
                postData.Add("ram", (RAM/1024) + "MB");
                postData.Add("os", OS);
                postData.Add("cpu", CPU);
                postData.Add("gpu", GPU);

                string get_values = String.Join("&", postData.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(postData[a])));

                dumpupload.UploadProgressChanged += (x, y) => uploadProgress(x, y, progressBar1);
                dumpupload.UploadFileCompleted += (x, y) => uploadCompleted(x, y, fullpathdump);
                dumpupload.UploadFileAsync(new Uri("http://dumps.worldunited.gg/upload.php?" + get_values), "POST", fullpathdump);
            } 
        }

        private static void uploadProgress(object sender, UploadProgressChangedEventArgs e, ProgressBar progress) {
            try { 
            progress.Value = (int)e.ProgressPercentage;
            }
            catch
            {

            }
        }

        private static void uploadCompleted(object sender, UploadFileCompletedEventArgs e, string file) {
            File.Delete(file);
            Application.Exit();
        }
    }
}
