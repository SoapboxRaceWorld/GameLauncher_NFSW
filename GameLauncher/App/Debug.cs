using GameLauncher.App.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;
using GameLauncher;
using GameLauncher.App;
using System.Runtime.InteropServices;
using GameLauncherReborn;

namespace GameLauncher.App {
    public partial class DebugWindow : Form {
        public DebugWindow() {
            InitializeComponent();
        }

        public static string AntivirusInstalled(string caller = "AntiVirusProduct") {
            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM " + caller);
            ManagementObjectCollection data = wmiData.Get();

            string virusCheckerName = "";
            int status = 0;

            foreach (ManagementObject virusChecker in data)
            {
                virusCheckerName = virusChecker["displayName"].ToString();
                status = Convert.ToInt32(virusChecker["productState"]);
            }

            return virusCheckerName;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        private void DebugWindow_Load(object sender, EventArgs e) {

            data.AutoGenerateColumns = true;

            IniFile SettingFile = new IniFile("Settings.ini");
            string UserSettings = Environment.ExpandEnvironmentVariables("%AppData%\\Need for Speed World\\Settings\\UserSettings.xml");

            string TracksHigh = (SettingFile.Read("TracksHigh") == "1") ? "True" : "False";
            string Password = (!String.IsNullOrEmpty(SettingFile.Read("Password"))) ? "True" : "False";
            string SkipUpdate = (SettingFile.Read("SkipUpdate") == "1") ? "True" : "False";
            string Antivirus = (String.IsNullOrEmpty(AntivirusInstalled())) ? "---" : AntivirusInstalled();
            string Firewall = (String.IsNullOrEmpty(AntivirusInstalled("FirewallProduct"))) ? "---" : AntivirusInstalled("FirewallProduct");
            string AntiSpyware = (String.IsNullOrEmpty(AntivirusInstalled("AntiSpywareProduct"))) ? "---" : AntivirusInstalled("AntiSpywareProduct");
            string LauncherPosition = "";

            var Win32_OperatingSystem = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                select x.GetPropertyValue("Caption")).FirstOrDefault();

            string OS = (DetectLinux.LinuxDetected()) ? "Linux" : Win32_OperatingSystem.ToString();

            var Win32_Processor = (from x in new ManagementObjectSearcher("SELECT Name FROM Win32_Processor").Get().Cast<ManagementObject>()
                select x.GetPropertyValue("Name")).FirstOrDefault();

            List<string> GPUs = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
            string graphicsCard = string.Empty;
            foreach (ManagementObject mo in searcher.Get()) {
                foreach (PropertyData property in mo.Properties) {
                    GPUs.Add(property.Value.ToString());
                }
            }

            var Win32_VideoController = string.Join(" | ", GPUs);

            long memKb;
            GetPhysicallyInstalledSystemMemory(out memKb);

            if(SettingFile.Read("LauncherPosX") + "x" + SettingFile.Read("LauncherPosY") == "x") {
                LauncherPosition = "Windows Default Position";
            } else {
                LauncherPosition = SettingFile.Read("LauncherPosX") + "x" + SettingFile.Read("LauncherPosY");
            }

            Kernel32.GetDiskFreeSpaceEx(SettingFile.Read("InstallationDirectory"), out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

            var settings = new[] {
                new { Text = "InstallationDirectory", Value = SettingFile.Read("InstallationDirectory")},
                new { Text = "Server", Value =  SettingFile.Read("Server")},
                new { Text = "Credentials Saved", Value = Password},
                new { Text = "Language", Value =  SettingFile.Read("Language")},
                new { Text = "TracksHigh", Value = TracksHigh},
                new { Text = "UILanguage", Value =  SettingFile.Read("UILanguage")},
                new { Text = "SkipUpdate", Value = SkipUpdate},
                new { Text = "LauncherPos", Value = LauncherPosition},

                new { Text = "", Value = "" },
                
                new { Text = "Antivirus", Value = Antivirus },
                new { Text = "Firewall", Value = Firewall },
                new { Text = "AntiSpyware", Value = AntiSpyware },

                new { Text = "", Value = "" },

                new { Text = "Operating System", Value = OS },
                new { Text = "CPU", Value = Win32_Processor.ToString() },
                new { Text = "GPU", Value = Win32_VideoController.ToString() },
                new { Text = "RAM", Value = (memKb / 1024).ToString() + "MB" },
                new { Text = "Screen Resolution", Value = Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height },
                new { Text = "Disk Space Left", Value = FormatFileSize(lpFreeBytesAvailable).ToString() },
            };

            data.DataSource = settings;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Font = new Font(data.Font, FontStyle.Bold);
            data.Columns[0].DefaultCellStyle = style;

            data.Columns[0].Width += 50; 

            int size_x = data.Columns[0].Width + data.Columns[1].Width + 7;
            int size_y = 450;

            data.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Size = new Size(size_x, size_y);
        }

        public string FormatFileSize(ulong byteCount) {
            double[] numArray = new double[] { 1073741824, 1048576, 1024, 0 };
            string[] strArrays = new string[] { "GB", "MB", "KB", "Bytes" };
            for (int i = 0; i < (int)numArray.Length; i++) {
                if ((double)byteCount >= numArray[i]) {
                    return string.Concat(string.Format("{0:0.00}", (double)byteCount / numArray[i]), strArrays[i]);
                }
            }

            return "0 Bytes";
        }
    }
}
