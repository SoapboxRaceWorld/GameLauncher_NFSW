using GameLauncher.App.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using GameLauncherReborn;

namespace GameLauncher.App
{
    public partial class DebugWindow : Form
    {
        string ServerIP = String.Empty;
        string ServerName = String.Empty;

        public DebugWindow(string serverIP, string serverName)
        {
            ServerIP = serverIP;
            ServerName = serverName;

            InitializeComponent();
        }

        public static string AntivirusInstalled(string caller = "AntiVirusProduct")
        {
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

        private void DebugWindow_Load(object sender, EventArgs e)
        {
            data.AutoGenerateColumns = true;

            IniFile SettingFile = new IniFile("Settings.ini");

            string Password = (!String.IsNullOrEmpty(SettingFile.Read("Password"))) ? "True" : "False";
            string ProxyStatus = (!String.IsNullOrEmpty(SettingFile.Read("DisableProxy"))) ? "False" : "True";
            string RPCStatus = (!String.IsNullOrEmpty(SettingFile.Read("DisableRPC"))) ? "False" : "True";

            string Antivirus = String.Empty;
            string Firewall = String.Empty;
            string AntiSpyware = String.Empty;

            if (!DetectLinux.LinuxDetected())
            {
                try
                {
                    Antivirus = (String.IsNullOrEmpty(AntivirusInstalled())) ? "---" : AntivirusInstalled();
                    Firewall = (String.IsNullOrEmpty(AntivirusInstalled("FirewallProduct"))) ? "---" : AntivirusInstalled("FirewallProduct");
                    AntiSpyware = (String.IsNullOrEmpty(AntivirusInstalled("AntiSpywareProduct"))) ? "---" : AntivirusInstalled("AntiSpywareProduct");
                }
                catch
                {
                    Antivirus = "Unknown";
                    Firewall = "Unknown";
                    AntiSpyware = "Unknown";
                }
            }

            string LauncherPosition = "";
            string OS = "";

            if (DetectLinux.LinuxDetected()) {
                OS = DetectLinux.Distro();
            } else {
                OS = Environment.OSVersion.VersionString;
            }

            if (SettingFile.Read("LauncherPosX") + "x" + SettingFile.Read("LauncherPosY") == "x")
            {
                LauncherPosition = "Windows Default Position";
            }
            else
            {
                LauncherPosition = SettingFile.Read("LauncherPosX") + "x" + SettingFile.Read("LauncherPosY");
            }

            string UpdateSkip = "";

            if (SettingFile.Read("IgnoreUpdateVersion") == Application.ProductVersion || SettingFile.Read("IgnoreUpdateVersion") == String.Empty)
            {
                    UpdateSkip = "False";
            }
            else
            {
                UpdateSkip = SettingFile.Read("IgnoreUpdateVersion");
            }

            long memKb = 0;
            ulong lpFreeBytesAvailable = 0;
            List<string> GPUs = new List<string>();
            string Win32_Processor = "";
            if (!DetectLinux.LinuxDetected())
            {
                Kernel32.GetPhysicallyInstalledSystemMemory(out memKb);

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                string graphicsCard = string.Empty;
                foreach (ManagementObject mo in searcher.Get())
                {
                    foreach (PropertyData property in mo.Properties)
                    {
                        GPUs.Add(property.Value.ToString());
                    }
                }

                Win32_Processor = (from x in new ManagementObjectSearcher("SELECT Name FROM Win32_Processor").Get().Cast<ManagementObject>()
                                   select x.GetPropertyValue("Name")).FirstOrDefault().ToString();

                Kernel32.GetDiskFreeSpaceEx(SettingFile.Read("InstallationDirectory"), out lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
            }

            var Win32_VideoController = string.Join(" | ", GPUs);

            var settings = new List<ListType> {
                new ListType{ Name = "InstallationDirectory", Value = SettingFile.Read("InstallationDirectory")},
                new ListType{ Name = "Credentials Saved", Value = Password},
                new ListType{ Name = "Language", Value =  SettingFile.Read("Language")},
                new ListType{ Name = "LauncherPos", Value = LauncherPosition},
                new ListType{ Name = "Skipping Update", Value = UpdateSkip},
                new ListType{ Name = "Disable Proxy", Value = ProxyStatus},
                new ListType{ Name = "Disable RPC", Value = RPCStatus},
                new ListType{ Name = "", Value = "" },
                new ListType{ Name = "Server Name", Value = ServerName},
                new ListType{ Name = "Server Address", Value = ServerIP},
                new ListType{ Name = "CDN Address", Value = SettingFile.Read("CDN")},
                new ListType{ Name = "ProxyPort", Value = Self.ProxyPort.ToString()},
                new ListType{ Name = "", Value = "" },
            };

            if (!DetectLinux.LinuxDetected())
            {
                settings.AddRange(new[] {
                    new ListType{ Name = "Antivirus", Value = Antivirus },
                    new ListType{ Name = "Firewall", Value = Firewall },
                    new ListType{ Name = "AntiSpyware", Value = AntiSpyware },
                    new ListType{ Name = "", Value = "" },
                    new ListType{ Name = "CPU", Value = Win32_Processor },
                    new ListType{ Name = "GPU", Value = Win32_VideoController},
                    new ListType{ Name = "RAM", Value = (memKb / 1024).ToString() + "MB" },
                    new ListType{ Name = "Disk Space Left", Value = FormatFileSize(lpFreeBytesAvailable) },
                    new ListType{ Name = "", Value = ""}
                });
            }
            settings.AddRange(new[] {
                new ListType{ Name = "HWID", Value = Security.FingerPrint.Value()},
                new ListType{ Name = "Operating System", Value = OS},
                new ListType{ Name = "Environment Version", Value = Environment.OSVersion.Version.ToString() },
                new ListType{ Name = "Screen Resolution", Value = Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height }
            });

            data.DataSource = settings;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Font = new Font(data.Font, FontStyle.Bold);
            data.Columns[0].DefaultCellStyle = style;

            data.Columns[0].Width += 50;

            int size_x = 452;
            int size_y = 580;

            data.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Size = new Size(size_x, size_y);
        }

        public string FormatFileSize(ulong byteCount)
        {
            double[] numArray = new double[] { 1073741824, 1048576, 1024, 0 };
            string[] strArrays = new string[] { "GB", "MB", "KB", "Bytes" };
            for (int i = 0; i < (int)numArray.Length; i++)
            {
                if ((double)byteCount >= numArray[i])
                {
                    return string.Concat(string.Format("{0:0.00}", (double)byteCount / numArray[i]), strArrays[i]);
                }
            }

            return "0 Bytes";
        }

        struct ListType
        {
			public string Name { get; set; }
			public string Value { get; set; }
        }
    }
}
