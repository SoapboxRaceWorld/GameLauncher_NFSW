using GameLauncher.App.Classes;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncherReborn;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFirewallHelper;

namespace GameLauncher
{
    public partial class SplashScreen : Form
    {
        public static int ProgressStatus = 0;

        public SplashScreen()
        {
            InitializeComponent();
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Interval = 1;
            timer1.Start();
            RunCoreEssentials();
        }

        private void RunCoreEssentials()
        {
            if (!DetectLinux.LinuxDetected())
            {
                //Windows Firewall Runner
                if (!string.IsNullOrEmpty(FileSettingsSave.FirewallStatus))
                {
                    string nameOfLauncher = "SBRW - Game Launcher";
                    string localOfLauncher = Assembly.GetEntryAssembly().Location;

                    string nameOfUpdater = "SBRW - Game Launcher Updater";
                    string localOfUpdater = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "GameLauncherUpdater.exe");

                    string groupKeyLauncher = "Game Launcher for Windows";
                    string descriptionLauncher = "Soapbox Race World";

                    bool removeFirewallRule = false;
                    bool firstTimeRun = false;

                    if (FileSettingsSave.FirewallStatus == "Not Excluded")
                    {
                        firstTimeRun = true;
                        FileSettingsSave.FirewallStatus = "Excluded";
                    }
                    else if (FileSettingsSave.FirewallStatus == "Reset")
                    {
                        removeFirewallRule = true;
                        FileSettingsSave.FirewallStatus = "Not Excluded";
                    }

                    FileSettingsSave.SaveSettings();

                    //Inbound & Outbound
                    FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfLauncher, localOfLauncher, groupKeyLauncher, descriptionLauncher, FirewallProtocol.Any);
                    FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfUpdater, localOfUpdater, groupKeyLauncher, descriptionLauncher, FirewallProtocol.Any);

                    //This Removes the Game File Exe From Firewall
                    //To Find the one that Adds the Exe To Firewall -> Search for `OnDownloadFinished()`
                    string CurrentGameFilesExePath = Path.Combine(FileSettingsSave.GameInstallation + "\\nfsw.exe");

                    if (File.Exists(CurrentGameFilesExePath) && removeFirewallRule == true)
                    {
                        string nameOfGame = "SBRW - Game";
                        string localOfGame = CurrentGameFilesExePath;

                        string groupKeyGame = "Need for Speed: World";
                        string descriptionGame = groupKeyGame;

                        //Inbound & Outbound
                        FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfGame, localOfGame, groupKeyGame, descriptionGame, FirewallProtocol.Any);
                    }

                    ProgressStatus++;
                }

                //Windows 7 Fix
                if ((string.IsNullOrEmpty(FileSettingsSave.Win7UpdatePatches) && WindowsProductVersion.GetWindowsNumber() == 6.1) || FileSettingsSave.Win7UpdatePatches == "0")
                {
                    if (Self.GetInstalledHotFix("KB3020369") == false || Self.GetInstalledHotFix("KB3125574") == false)
                    {
                        String messageBoxPopupKB = String.Empty;
                        messageBoxPopupKB = "Hey Windows 7 User, we've detected a potential issue of some missing Updates that are required.\n";
                        messageBoxPopupKB += "We found that these Windows Update packages are showing as not installed:\n\n";

                        if (Self.GetInstalledHotFix("KB3020369") == false) messageBoxPopupKB += "- Update KB3020369\n";
                        if (Self.GetInstalledHotFix("KB3125574") == false) messageBoxPopupKB += "- Update KB3125574\n";

                        messageBoxPopupKB += "\nAditionally, we must add a value to the registry:\n";

                        messageBoxPopupKB += "- HKLM/SYSTEM/CurrentControlSet/Control/SecurityProviders\n/SCHANNEL/Protocols/TLS 1.2/Client\n";
                        messageBoxPopupKB += "- Value: DisabledByDefault -> 0\n\n";

                        messageBoxPopupKB += "Would you like to add those values?";
                        DialogResult replyPatchWin7 = MessageBox.Show(null, messageBoxPopupKB, "GameLauncherReborn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (replyPatchWin7 == DialogResult.Yes)
                        {
                            RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client");
                            key.SetValue("DisabledByDefault", 0x0);

                            MessageBox.Show(null, "Registry option set, Remember that the changes may require a system reboot to take effect", "GameLauncherReborn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show(null, "Roger that, There may be some issues connecting to the servers.", "GameLauncherReborn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        FileSettingsSave.Win7UpdatePatches = "1";
                        FileSettingsSave.SaveSettings();
                    }
                }

                if (!RedistributablePackage.IsInstalled(RedistributablePackageVersion.VC2015to2019x86))
                {
                    var result = MessageBox.Show(
                        "You do not have the 32-bit 2015-2019 VC++ Redistributable Package installed.\n \nThis will install in the Background\n \nThis may restart your computer. \n \nClick OK to install it.",
                        "Compatibility",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.OK)
                    {
                        MessageBox.Show("The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    var wc = new WebClient();
                    wc.DownloadFile("https://aka.ms/vs/16/release/VC_redist.x86.exe", "VC_redist.x86.exe");
                    var proc = Process.Start(new ProcessStartInfo
                    {
                        Verb = "runas",
                        Arguments = "/quiet",
                        FileName = "VC_redist.x86.exe"
                    });

                    if (proc == null)
                    {
                        MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }

                if (Environment.Is64BitOperatingSystem == true)
                {
                    if (!RedistributablePackage.IsInstalled(RedistributablePackageVersion.VC2015to2019x64))
                    {
                        var result = MessageBox.Show(
                            "You do not have the 64-bit 2015-2019 VC++ Redistributable Package installed.\n \nThis will install in the Background\n \nThis may restart your computer. \n \nClick OK to install it.",
                            "Compatibility",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Warning);

                        if (result != DialogResult.OK)
                        {
                            MessageBox.Show("The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }

                        var wc = new WebClient();
                        wc.DownloadFile("https://aka.ms/vs/16/release/VC_redist.x64.exe", "VC_redist.x64.exe");
                        var proc = Process.Start(new ProcessStartInfo
                        {
                            Verb = "runas",
                            Arguments = "/quiet",
                            FileName = "VC_redist.x64.exe"
                        });

                        if (proc == null)
                        {
                            MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                if (!File.Exists("GameLauncherUpdater.exe"))
                {
                    Log.Info("LAUNCHER UPDATER: Starting GameLauncherUpdater downloader");
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                            {
                                if (new FileInfo("GameLauncherUpdater.exe").Length == 0)
                                {
                                    File.Delete("GameLauncherUpdater.exe");
                                }
                            };
                            wc.DownloadFile(new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe"), "GameLauncherUpdater.exe");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("LAUCHER UPDATER: Failed to download updater. " + ex.Message);
                    }

                    ProgressStatus++;
                }
                else if (File.Exists("GameLauncherUpdater.exe"))
                {
                    String GameLauncherUpdaterLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameLauncherUpdater.exe");
                    var LauncherUpdaterBuild = FileVersionInfo.GetVersionInfo(GameLauncherUpdaterLocation);
                    var LauncherUpdaterBuildNumber = LauncherUpdaterBuild.FileVersion;
                    var UpdaterBuildNumberResult = LauncherUpdaterBuildNumber.CompareTo(Program.LatestUpdaterBuildVersion);

                    Log.Build("LAUNCHER UPDATER BUILD: GameLauncherUpdater " + LauncherUpdaterBuildNumber);
                    if (UpdaterBuildNumberResult < 0)
                    {
                        Log.Info("LAUNCHER UPDATER: " + UpdaterBuildNumberResult + " Builds behind latest Updater!");
                    }
                    else
                    {
                        Log.Info("LAUNCHER UPDATER: Latest GameLauncherUpdater!");
                    }

                    if (UpdaterBuildNumberResult < 0)
                    {
                        Log.Info("LAUNCHER UPDATER: Downloading New GameLauncherUpdater.exe");
                        File.Delete("GameLauncherUpdater.exe");

                        try
                        {
                            using (WebClient wc = new WebClient())
                            {
                                wc.DownloadFile(new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe"), "GameLauncherUpdater.exe");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("LAUNCHER UPDATER: Failed to download new updater. " + ex.Message);
                        }
                    }

                    ProgressStatus++;
                }
            }

            VisualsAPIChecker.PingAPIStatus();
            ServerListUpdater.GetList();
            CDNListUpdater.GetList();

            if (VisualsAPIChecker.CarbonAPITwo == false)
            {
                DialogResult restartAppNoApis = MessageBox.Show(null, "There's no internet connection, Launcher might crash \n \nClick Yes to Close Launcher \nor \nClick No Continue", "GameLauncher has Stopped, Failed To Connect To API", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (restartAppNoApis == DialogResult.No)
                {
                    MessageBox.Show("Good Luck... \n No Really \n ...Good Luck", "GameLauncher Will Continue, When It Failed To Connect To API");
                    Log.Warning("PRE-CHECK: User has Bypassed 'No Internet Connection' Check and Will Continue");
                }

                if (restartAppNoApis == DialogResult.Yes)
                {
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }
            }

            Log.Info("PROXY: Starting Proxy");
            ServerProxy.Instance.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (ProgressStatus)
            {
                case 4: case 5:
                case 6: case 7:
                    timer1.Stop();
                    timer1.Enabled = false;

                    this.Hide();

                    Log.Visuals("CORE: Starting MainScreen");
                    new MainScreen().ShowDialog();

                    break;
                default:
                    break;
            }
        }

        public static void SucessfulProgress()
        {
            ProgressStatus++;
        }
    }
}
