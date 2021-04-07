using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using System;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.LauncherCore.LauncherUpdater;
using System.Threading.Tasks;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.SystemPlatform.Components;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using WindowsFirewallHelper;
using System.Reflection;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using System.Management.Automation;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace GameLauncher.App.Classes
{
    public partial class SplashScreen : Form
    {
        public static int i = 0;

        public SplashScreen()
        {
            InitializeComponent();

            BackColor = Theming.SplashScreenTransparencyKey;
            TransparencyKey = Theming.SplashScreenTransparencyKey;
            BackgroundImage = Theming.LogoSplash;
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            FunctionStatus.CenterScreen(this);
        }

        private void Clock_Tick(object sender, EventArgs e)
        {
            if (i == 0)
            {
                Checks();
            }

            i++;
        }

        private void Checks()
        {
            Task.Run(() => CheckAsnyc());
        }

        private async Task CheckAsnyc()
        {
            if (!DetectLinux.LinuxDetected())
            {
                /* Check if User has a compatible .NET Framework Installed */
                const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

                using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
                {
                    /* For now, allow edge case of Windows 8.0 to run .NET 4.6.1 where upgrading to 8.1 is not possible */
                    if (WindowsProductVersion.CachedWindowsNumber == 6.2)
                    {
                        if (ndpKey != null && ndpKey.GetValue("Release") != null && (int)ndpKey.GetValue("Release") >= 394254)
                        {
                            //Do Nothing
                        }
                        else
                        {
                            DialogResult frameworkError = MessageBox.Show(null, "This application requires a minimum version of the .NET Framework:\n" +
                                " .NETFramework, Version=v4.6.1 \n\nDo you want to install this .NET Framework version now?", "GameLauncher.exe - This application could not be started.", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                            if (frameworkError == DialogResult.Yes)
                            {
                                Process.Start("https://dotnet.microsoft.com/download/dotnet-framework/net461");
                            }

                            Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                        }
                    }
                    /* Otherwise, all other OS Versions should have 4.6.2 as a Minimum Version */
                    else if (ndpKey != null && ndpKey.GetValue("Release") != null && (int)ndpKey.GetValue("Release") >= 394802)
                    {
                        //Do Nothing
                    }
                    else
                    {
                        DialogResult frameworkError = MessageBox.Show(null, "This application requires a version equal to or newer than the .NET Framework:\n" +
                            " .NETFramework, Version=v4.6.2 \n\nDo you want to install this .NET Framework version now?", "GameLauncher.exe - This application could not be started.", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                        if (frameworkError == DialogResult.Yes)
                        {
                            Process.Start("https://dotnet.microsoft.com/download/dotnet-framework");
                        }

                        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                    }
                }
            }

            File.Delete("communication.log");
            File.Delete("launcher.log");
            Log.StartLogging();

            if (EnableInsider.ShouldIBeAnInsider() == true)
            {
                Log.Build("INSIDER: GameLauncher " + Application.ProductVersion + "_" + EnableInsider.BuildNumber());
            }
            else
            {
                Log.Build("BUILD: GameLauncher " + Application.ProductVersion);
            }

            Log.Info("LAUNCHER: Detecting OS");
            if (DetectLinux.LinuxDetected())
            {
                InformationCache.OSName = DetectLinux.Distro();
                Log.System("SYSTEM: Detected OS: " + InformationCache.OSName);
            }
            else
            {
                WindowsProductVersion.CachedWindowsNumber = WindowsProductVersion.GetWindowsNumber();
                InformationCache.OSName = WindowsProductVersion.ConvertWindowsNumberToName(WindowsProductVersion.CachedWindowsNumber);

                Log.System("SYSTEM: Detected OS: " + InformationCache.OSName);
                Log.System("SYSTEM: OS Details: " + Environment.OSVersion);
                Log.System("SYSTEM: Video Card: " + HardwareInfo.GPU.CardName());
                Log.System("SYSTEM: Driver Version: " + HardwareInfo.GPU.DriverVersion());
            }

            FileSettingsSave.NullSafeSettings();
            FileAccountSave.NullSafeAccount();

            /* Set Launcher Directory */
            Log.Info("CORE: Setting up current directory: " + Path.GetDirectoryName(Application.ExecutablePath));
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

            if (!DetectLinux.LinuxDetected())
            {
                Log.Info("CORE: Checking current directory");

                switch (FunctionStatus.CheckFolder(Directory.GetCurrentDirectory()))
                {
                    case FolderType.IsTempFolder:
                    case FolderType.IsUsersFolders:
                    case FolderType.IsProgramFilesFolder:
                    case FolderType.IsWindowsFolder:
                    case FolderType.IsRootFolder:
                        String constructMsg = String.Empty;

                        constructMsg += "Using this location for GameLauncher is not allowed.\nThe Launcher folder/directory can NOT be in:\n\n";
                        constructMsg += "• X:\\GameLauncher.exe (Root of Drive, such as C:\\ or D:\\, must be in a folder)\n";
                        constructMsg += "• C:\\Program Files\n";
                        constructMsg += "• C:\\Program Files (x86)\n";
                        constructMsg += "• C:\\Users (Includes 'Desktop', 'Documents', 'Downloads')\n";
                        constructMsg += "• C:\\Windows\n\n";
                        constructMsg += "Instead, move it someplace like:\n";
                        constructMsg += "• 'X:\\Soabox Race World' or 'X:\\SBRW'\n";
                        constructMsg += "(Where 'X:' is a 'Local Disk' location on `My Computer` / `This PC`)\n\n";

                        MessageBox.Show(null, constructMsg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        Environment.Exit(0);
                        break;
                }

                if (!FunctionStatus.HasWriteAccessToFolder(Path.GetDirectoryName(Application.ExecutablePath)))
                {
                    MessageBox.Show("Unable to do a Test Write to Launcher Folder\nPermission Issue");
                }

                if (!FunctionStatus.HasWriteAccessToFolder(FileSettingsSave.GameInstallation) && !string.IsNullOrEmpty(FileSettingsSave.GameInstallation))
                {
                    MessageBox.Show("Unable to do a Test Write to Game Files Folder\nPermission Issue");
                }

                /* Windows Firewall Runner */
                if (!string.IsNullOrEmpty(FileSettingsSave.FirewallLauncherStatus))
                {
                    if (FirewallManager.IsServiceRunning == true && FirewallHelper.FirewallStatus() == true)
                    {
                        string nameOfLauncher = "SBRW - Game Launcher";
                        string localOfLauncher = Assembly.GetEntryAssembly().Location;

                        string nameOfUpdater = "SBRW - Game Launcher Updater";
                        string localOfUpdater = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "GameLauncherUpdater.exe");

                        string groupKeyLauncher = "Game Launcher for Windows";
                        string descriptionLauncher = "Soapbox Race World";

                        bool removeFirewallRule = false;
                        bool firstTimeRun = false;

                        if (FileSettingsSave.FirewallLauncherStatus == "Not Excluded" || FileSettingsSave.FirewallLauncherStatus == "Turned Off" || FileSettingsSave.FirewallLauncherStatus == "Service Stopped" || FileSettingsSave.FirewallLauncherStatus == "Unknown")
                        {
                            firstTimeRun = true;
                            FileSettingsSave.FirewallLauncherStatus = "Excluded";
                        }
                        else if (FileSettingsSave.FirewallLauncherStatus == "Reset")
                        {
                            removeFirewallRule = true;
                            FileSettingsSave.FirewallLauncherStatus = "Not Excluded";
                        }

                        /* Inbound & Outbound */
                        FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfLauncher, localOfLauncher, groupKeyLauncher, descriptionLauncher, FirewallProtocol.Any);
                        FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfUpdater, localOfUpdater, groupKeyLauncher, descriptionLauncher, FirewallProtocol.Any);
                    }
                    else if (FirewallManager.IsServiceRunning == true && FirewallHelper.FirewallStatus() == false)
                    {
                        FileSettingsSave.FirewallLauncherStatus = "Turned Off";
                    }
                    else
                    {
                        FileSettingsSave.FirewallLauncherStatus = "Service Stopped";
                    }

                    FileSettingsSave.SaveSettings();
                }

                /* Windows 7 Fix */
                if (WindowsProductVersion.CachedWindowsNumber == 6.1 && string.IsNullOrEmpty(FileSettingsSave.Win7UpdatePatches))
                {
                    if (ManagementSearcher.GetInstalledHotFix("KB3020369") == false || ManagementSearcher.GetInstalledHotFix("KB3125574") == false)
                    {
                        String messageBoxPopupKB = String.Empty;
                        messageBoxPopupKB = "Hey Windows 7 User, we've detected a potential issue of some missing Updates that are required.\n";
                        messageBoxPopupKB += "We found that these Windows Update packages are showing as not installed:\n\n";

                        if (ManagementSearcher.GetInstalledHotFix("KB3020369") == false) messageBoxPopupKB += "- Update KB3020369\n";
                        if (ManagementSearcher.GetInstalledHotFix("KB3125574") == false) messageBoxPopupKB += "- Update KB3125574\n";

                        messageBoxPopupKB += "\nAditionally, we must add a value to the registry:\n";

                        messageBoxPopupKB += "- HKLM/SYSTEM/CurrentControlSet/Control/SecurityProviders\n/SCHANNEL/Protocols/TLS 1.2/Client\n";
                        messageBoxPopupKB += "- Value: DisabledByDefault -> 0\n\n";

                        messageBoxPopupKB += "Would you like to add those values?";
                        DialogResult replyPatchWin7 = MessageBox.Show(null, messageBoxPopupKB, "SBRW Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (replyPatchWin7 == DialogResult.Yes)
                        {
                            RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client");
                            key.SetValue("DisabledByDefault", 0x0);

                            MessageBox.Show(null, "Registry option set, Remember that the changes may require a system reboot to take effect", "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            FileSettingsSave.Win7UpdatePatches = "1";
                        }
                        else
                        {
                            MessageBox.Show(null, "Roger that, There may be some issues connecting to the servers.", "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            FileSettingsSave.Win7UpdatePatches = "0";
                        }

                        FileSettingsSave.SaveSettings();
                    }
                }

                /* Check if Redistributable Packages are Installed */
                await Redistributable.CheckAsync();
            }

            if (!File.Exists("servers.json"))
            {
                try
                {
                    File.WriteAllText("servers.json", "[]");
                }
                catch { /* ignored */ }
            }

            if (!string.IsNullOrEmpty(FileSettingsSave.GameInstallation))
            {
                var linksPath = Path.Combine(FileSettingsSave.GameInstallation + "\\.links");
                ModNetLinksCleanup.CleanLinks(linksPath);
            }

            if (FileSettingsSave.Proxy == "0")
            {
                Log.Info("PROXY: Starting Proxy (From Startup)");
                ServerProxy.Instance.Start("Splash Screen");
            }

            /* Sets Up Langauge List */
            LanguageListUpdater.GetList();
            /* Check If Updater Exists or Requires an Update */
            await UpdaterExecutable.CheckAsync();
            /* Check Up to Date Certificate Status */
            await CertificateStore.LatestAsync();
            /* Check Latest Launcher Version */
            await LauncherUpdateCheck.Latest();
            /* Check ServerList Status */
            await Task.Run(() => ServerListUpdater.GetList());
            /* Do First Time Run Checks */
            FirstTimeRun();
        }

        public static void FirstTimeRun()
        {
            Log.Core("LAUNCHER: Checking InstallationDirectory: " + FileSettingsSave.GameInstallation);
            if (string.IsNullOrEmpty(FileSettingsSave.GameInstallation))
            {
                Log.Core("LAUNCHER: First run!");

                try
                {
                    Form welcome = new WelcomeScreen();
                    DialogResult welcomereply = welcome.ShowDialog();

                    if (welcomereply != DialogResult.OK)
                    {
                        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                    }
                    else
                    {
                        FileSettingsSave.CDN = SelectedCDN.CDNUrl;
                        FileSettingsSave.SaveSettings();
                    }
                }
                catch
                {
                    Log.Warning("LAUNCHER: CDN Source URL was Empty! Setting a Null Safe URL 'http://localhost'");
                    FileSettingsSave.CDN = "http://localhost";
                    Log.Core("LAUNCHER: Installation Directory was Empty! Creating and Setting Directory at " + AppDomain.CurrentDomain.BaseDirectory + "\\Game Files");
                    FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                    FileSettingsSave.SaveSettings();
                }

                CommonOpenFileDialog fbd = new CommonOpenFileDialog
                {
                    EnsurePathExists = true,
                    EnsureFileExists = false,
                    AllowNonFileSystemItems = false,
                    Title = "Select the location to Find or Download NFS:W",
                    IsFolderPicker = true
                };

                fbd.ShowDialog();

                if (fbd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (!FunctionStatus.HasWriteAccessToFolder(fbd.FileName))
                    {
                        Log.Error("LAUNCHER: Not enough permissions. Exiting.");
                        MessageBox.Show(null, "You don't have enough permission to select this path as installation folder. Please select another directory.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Environment.Exit(Environment.ExitCode);
                    }

                    if (fbd.FileName.Length == 3)
                    {
                        Log.Warning("LAUNCHER: Installing NFSW in root of the harddisk is not allowed.");
                        MessageBox.Show(null, string.Format("Installing NFSW in root of the harddisk is not allowed. " +
                            "Instead, we will install it on {0}.", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        FileSettingsSave.SaveSettings();
                    }
                    else if (fbd.FileName == AppDomain.CurrentDomain.BaseDirectory)
                    {
                        Directory.CreateDirectory("Game Files");
                        Log.Warning("LAUNCHER: Installing NFSW in same directory where the launcher resides is disadvised.");
                        MessageBox.Show(null, string.Format("Installing NFSW in same directory where the launcher resides is disadvised. " +
                            "Instead, we will install it on {0}.", AppDomain.CurrentDomain.BaseDirectory + "\\Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        FileSettingsSave.SaveSettings();
                    }
                    else
                    {
                        Log.Core("LAUNCHER: Directory Set: " + fbd.FileName);
                        FileSettingsSave.GameInstallation = fbd.FileName;
                        FileSettingsSave.SaveSettings();
                    }
                }
                else
                {
                    Log.Core("LAUNCHER: Exiting");
                    Environment.Exit(Environment.ExitCode);
                }

                /*
                Thread FileBrowser = new Thread(() =>
                {

                });

                FileBrowser.SetApartmentState(ApartmentState.STA);
                FileBrowser.Start();
                FileBrowser.Join();
                */
            }

            if (!DetectLinux.LinuxDetected())
            {
                switch (FunctionStatus.CheckFolder(FileSettingsSave.GameInstallation))
                {
                    case FolderType.IsSameAsLauncherFolder:
                        Directory.CreateDirectory("Game Files");
                        Log.Error("LAUNCHER: Installing NFSW in same location where the GameLauncher resides is NOT allowed.");
                        MessageBox.Show(null, string.Format("Installing NFSW in same location where the GameLauncher resides is NOT allowed.\n" +
                            "Instead, we will install it at {0}.", AppDomain.CurrentDomain.BaseDirectory + "Game Files"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        break;

                    case FolderType.IsTempFolder:
                    case FolderType.IsUsersFolders:
                    case FolderType.IsProgramFilesFolder:
                    case FolderType.IsWindowsFolder:
                    case FolderType.IsRootFolder:
                        String constructMsg = String.Empty;
                        Directory.CreateDirectory("Game Files");
                        constructMsg += "Using this location for Game Files is not allowed.\nThe following list are NOT allowed:\n\n";
                        constructMsg += "• X:\\nfsw.exe (Root of Drive, such as C:\\ or D:\\, must be in a folder)\n";
                        constructMsg += "• C:\\Program Files\n";
                        constructMsg += "• C:\\Program Files (x86)\n";
                        constructMsg += "• C:\\Users (Includes 'Desktop', 'Documents', 'Downloads')\n";
                        constructMsg += "• C:\\Windows\n\n";
                        constructMsg += "Instead, we will install the NFSW Game at " + AppDomain.CurrentDomain.BaseDirectory + "\\Game Files\n";

                        MessageBox.Show(null, constructMsg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Log.Error("LAUNCHER: Installing NFSW in a Restricted Location is not allowed.");
                        FileSettingsSave.GameInstallation = AppDomain.CurrentDomain.BaseDirectory + "\\Game Files";
                        break;
                }
                FileSettingsSave.SaveSettings();

                /* Windows Defender (Windows 10) */
                if (WindowsProductVersion.CachedWindowsNumber >= 10.0 && (FileSettingsSave.WindowsDefenderStatus == "Not Excluded" || FileSettingsSave.WindowsDefenderStatus == "Unknown"))
                {
                    Log.Core("WINDOWS DEFENDER: Windows 10 Detected! Running Exclusions for Core Folders");
                    if (ManagementSearcher.SecurityCenter("AntivirusEnabled") == true && ManagementSearcher.SecurityCenter("AntispywareEnabled") == true)
                    {
                        /* Create Windows Defender Exclusion */
                        try
                        {
                            Log.Info("WINDOWS DEFENDER: Excluding Core Folders");
                            /* Add Exclusion to Windows Defender */
                            using (PowerShell ps = PowerShell.Create())
                            {
                                ps.AddScript($"Add-MpPreference -ExclusionPath \"{AppDomain.CurrentDomain.BaseDirectory}\"");
                                ps.AddScript($"Add-MpPreference -ExclusionPath \"{FileSettingsSave.GameInstallation}\"");
                                var result = ps.Invoke();
                            }

                            FileSettingsSave.WindowsDefenderStatus = "Excluded";
                            FileSettingsSave.SaveSettings();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("WINDOWS DEFENDER: " + ex.Message);
                            FileSettingsSave.WindowsDefenderStatus = "Not Excluded";
                            FileSettingsSave.SaveSettings();
                        }
                    }
                    else
                    {
                        FileSettingsSave.WindowsDefenderStatus = "Not Supported";
                        FileSettingsSave.SaveSettings();
                    }
                }
                else if (WindowsProductVersion.CachedWindowsNumber >= 10.0 && !string.IsNullOrEmpty(FileSettingsSave.WindowsDefenderStatus))
                {
                    Log.Core("WINDOWS DEFENDER: Found 'WindowsDefender' key! Its value is " + FileSettingsSave.WindowsDefenderStatus);
                }
            }

            Start();
        }

        private static void Start()
        {
            /* Check If Launcher Failed to Connect to any APIs */
            if (VisualsAPIChecker.WOPLAPI == false)
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

            DiscordLauncherPresense.Start("Start Up", "540651192179752970");

            Log.Visuals("CORE: Starting MainScreen");
            new MainScreen().ShowDialog();
        }
    }
}
