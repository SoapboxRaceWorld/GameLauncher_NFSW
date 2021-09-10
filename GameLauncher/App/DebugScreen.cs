using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Components;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.SystemPlatform;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using GameLauncher.App.Classes.LauncherCore.RPC;

namespace GameLauncher.App
{
    public partial class DebugScreen : Form
    {
        public DebugScreen()
        {
            InitializeComponent();
            ApplyEmbeddedFonts();
        }

        private void ApplyEmbeddedFonts()
        {
            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Text = "Debug - SBRW Launcher: v" + Application.ProductVersion;

            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            float MainFontSize = UnixOS.Detected() ? 8f : 8f * 100f / CreateGraphics().DpiY;
            Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors              /
            /********************************/

            ForeColor = Theming.WinFormSecondaryTextForeColor;
            BackColor = Theming.WinFormTBGForeColor;

            data.ForeColor = Theming.WinFormSecondaryTextForeColor;
            data.GridColor = Theming.WinFormGridForeColor;

            Shown += (x, y) =>
            {
                try
                {
                    Application.OpenForms["DebugScreen"].Activate();
                }
                catch { }
                this.BringToFront();
            };
        }

        public static string SecurityCenter(string caller)
        {
            string virusCheckerName = string.Empty;
            try
            {
                ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM " + caller);
                ManagementObjectCollection data = wmiData.Get();

                foreach (ManagementObject virusChecker in data)
                {
                    virusCheckerName = virusChecker["displayName"].ToString();
                    int status = Convert.ToInt32(virusChecker["productState"]);
                }
            }
            catch (ManagementException Error)
            {
                LogToFileAddons.OpenLog("Debug", null, Error, null, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Debug", null, Error, null, true);
            }

            return virusCheckerName;
        }

        private void DebugScreen_Load(object sender, EventArgs e)
        {
            data.AutoGenerateColumns = true;

            string Antivirus = String.Empty;
            string Firewall = String.Empty;
            string AntiSpyware = String.Empty;

            if (!UnixOS.Detected())
            {
                try
                {
                    Antivirus = (String.IsNullOrWhiteSpace(SecurityCenter("AntiVirusProduct"))) ? "---" : SecurityCenter("AntiVirusProduct");
                    Firewall = (String.IsNullOrWhiteSpace(SecurityCenter("FirewallProduct"))) ? "Built-In" : SecurityCenter("FirewallProduct");
                    AntiSpyware = (String.IsNullOrWhiteSpace(SecurityCenter("AntiSpywareProduct"))) ? "---" : SecurityCenter("AntiSpywareProduct");
                }
                catch
                {
                    Antivirus = "Unknown";
                    Firewall = "Unknown";
                    AntiSpyware = "Unknown";
                }
            }

            string UpdateSkip = "";

            if (FileSettingsSave.IgnoreVersion == Application.ProductVersion || FileSettingsSave.IgnoreVersion == String.Empty)
            {
                    UpdateSkip = "False";
            }
            else
            {
                UpdateSkip = FileSettingsSave.IgnoreVersion;
            }

            long memKb = 0;
            ulong lpFreeBytesAvailable = 0;
            List<string> GPUs = new List<string>();
            string Win32_Processor = "";
            if (!UnixOS.Detected())
            {
                try
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

                    Kernel32.GetDiskFreeSpaceEx(FileSettingsSave.GameInstallation, 
                        out lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Debug", null, Error, null, true);
                }
            }

            var Win32_VideoController = string.Join(" | ", GPUs);

            var settings = new List<ListType>
            {
                new ListType{ Name = "InstallationDirectory", Value = FileSettingsSave.GameInstallation},
                new ListType{ Name = "Launcher Version", Value = Application.ProductVersion},
                new ListType{ Name = "Credentials Saved", Value = (!String.IsNullOrWhiteSpace(FileAccountSave.UserHashedPassword)) ? "True" : "False"},
                new ListType{ Name = "Language", Value =  FileSettingsSave.Lang},
                new ListType{ Name = "Skipping Update", Value = UpdateSkip},
                new ListType{ Name = "Proxy Enabled", Value = ServerProxy.Running().ToString()},
                new ListType{ Name = "RPC Enabled", Value = DiscordLauncherPresence.Running().ToString()},
                new ListType{ Name = "Firewall Rule - Launcher", Value =  FileSettingsSave.FirewallLauncherStatus},
                new ListType{ Name = "Firewall Rule - Game", Value =  FileSettingsSave.FirewallGameStatus},
                new ListType{ Name = "", Value = "" },
                new ListType{ Name = "Server Name", Value = ServerListUpdater.ServerName("Debug")},
                new ListType{ Name = "Server Address", Value = InformationCache.SelectedServerData.IPAddress},
                new ListType{ Name = "CDN Address", Value = FileSettingsSave.CDN},
                new ListType{ Name = "ProxyPort", Value = ServerProxy.ProxyPort.ToString()},
                new ListType{ Name = "", Value = "" },
            };

            if (!UnixOS.Detected())
            {
                settings.AddRange(new[]
                {
                    new ListType{ Name = "Antivirus", Value = Antivirus },
                    new ListType{ Name = "Firewall Application", Value = Firewall },
                    new ListType{ Name = "AntiSpyware", Value = AntiSpyware },
                    new ListType{ Name = "", Value = "" },
                    new ListType{ Name = "CPU", Value = Win32_Processor },
                    new ListType{ Name = "GPU", Value = Win32_VideoController},
                    new ListType{ Name = "RAM", Value = (memKb / 1024).ToString() + "MB" },
                    new ListType{ Name = "Disk Space Left", Value = FormatFileSize(lpFreeBytesAvailable) },
                    new ListType{ Name = "", Value = ""}
                });
            }
            settings.AddRange(new[]
            {
                new ListType{ Name = "HWID", Value = HardwareID.FingerPrint.Level_One_Value()},
                new ListType{ Name = "Operating System", Value = (UnixOS.Detected())? UnixOS.FullName() : Environment.OSVersion.VersionString},
                new ListType{ Name = "Environment Version", Value = Environment.OSVersion.Version.ToString() },
                new ListType{ Name = "Screen Resolution", Value = Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height }
            });

            data.DataSource = settings;

            DataGridViewCellStyle style = new DataGridViewCellStyle {
                Font = new Font(data.Font, FontStyle.Regular)
            };
            data.Columns[0].DefaultCellStyle = style;

            data.Columns[0].Width += 50;

            int size_x = 512;
            int size_y = 640;

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
