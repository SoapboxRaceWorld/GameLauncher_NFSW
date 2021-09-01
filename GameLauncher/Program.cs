using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Globalization;
using GameLauncher.App.Classes;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.SystemPlatform.Components;
using GameLauncher.App.Classes.LauncherCore.Client;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using System.Text;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.SystemPlatform.Unix;

namespace GameLauncher
{
    static class Program
    {
        public static bool LauncherMustRestart = false;

        [STAThread]
        static void Main()
        {
            if (Debugger.IsAttached && !NFSW.IsRunning())
            {
                Start();
            }
            else
            {
                if (NFSW.IsRunning())
                {
                    if (NFSW.DetectGameProcess())
                    {
                        MessageBox.Show(null, "An instance of Need for Speed: World is already running", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (NFSW.DetectGameLauncherSimplified())
                    {
                        MessageBox.Show(null, "An instance of SBRW Simplified Launcher is already running", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        MessageBox.Show(null, "An instance of SBRW Launcher is already running", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    FunctionStatus.LauncherForceClose = true;
                }

                if (FunctionStatus.LauncherForceClose)
                {
                    FunctionStatus.ErrorCloseLauncher("User Tried to Launch SBRW Launcher with one Running Already", false);
                }
                else
                {
                    /* INFO: this is here because this dll is necessary for downloading game files and I want to make it async.
                    Updated RedTheKitsune Code so it downloads the file if its missing.
                    It also restarts the launcher if the user click on yes on Prompt. - DavidCarbon */
                    if (!File.Exists("LZMA.dll"))
                    {
                        try
                        {
                            using (WebClientWithTimeout wc = new WebClientWithTimeout())
                            {
                                wc.Encoding = Encoding.UTF8;
                                wc.DownloadFile(new Uri(URLs.File + "/LZMA.dll"), "LZMA.dll");
                            }

                            DialogResult restartApp = MessageBox.Show(null, "Downloaded Missing LZMA.dll File." +
                                "\nPlease Restart Launcher, Thanks!", "GameLauncher Restart Required", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                            FunctionStatus.LauncherForceClose = true;

                            if (restartApp == DialogResult.Yes)
                            {
                                LauncherMustRestart = true;
                            }
                        }
                        catch { }
                    }

                    if (FunctionStatus.LauncherForceClose)
                    {
                        FunctionStatus.ErrorCloseLauncher("Closing From Downloaded Missing LZMA", LauncherMustRestart);
                    }
                    else
                    {
                        var mutex = new Mutex(false, "GameLauncherNFSW-MeTonaTOR");
                        try
                        {
                            if (mutex.WaitOne(0, false))
                            {
                                string[] files =
                                {
                                    "CommandLine.dll - 2.8.0",
                                    "DiscordRPC.dll - 1.0.175.0",
                                    "Flurl.dll - 3.0.2",
                                    "Flurl.Http.dll - 3.2.0",
                                    "INIFileParser.dll - 2.5.2",
                                    "LZMA.dll - 9.10 beta",
                                    "Microsoft.WindowsAPICodePack.dll - 1.1.0.0",
                                    "Microsoft.WindowsAPICodePack.Shell.dll - 1.1.0.0",
                                    "Microsoft.WindowsAPICodePack.ShellExtensions.dll - 1.1.0.0",
                                    "Nancy.dll - 2.0.0",
                                    "Nancy.Hosting.Self.dll - 2.0.0",
                                    "Newtonsoft.Json.dll - 13.0.1",
                                    "System.Runtime.InteropServices.RuntimeInformation.dll - 4.6.24705.01. " +
                                    "Commit Hash: 4d1af962ca0fede10beb01d197367c2f90e92c97",
                                    "System.ValueTuple.dll - 4.6.26515.06 @BuiltBy: dlab-DDVSOWINAGE059 " +
                                    "@Branch: release/2.1 @SrcCode: https://github.com/dotnet/corefx/tree/30ab651fcb4354552bd4891619a0bdd81e0ebdbf",
                                    "WindowsFirewallHelper.dll - 2.1.4.81"
                                };

                                var missingfiles = new List<string>();

                                if (!UnixOS.Detected())
                                {   /* MONO Hates this... */
                                    foreach (var file in files)
                                    {
                                        var splitFileVersion = file.Split(new string[] { " - " }, StringSplitOptions.None);

                                        if (!File.Exists(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(
                                            Directory.GetCurrentDirectory())) + "\\" + splitFileVersion[0]))
                                        {
                                            missingfiles.Add(splitFileVersion[0] + " - Not Found");
                                        }
                                        else
                                        {
                                            try
                                            {
                                                var versionInfo = FileVersionInfo.GetVersionInfo(splitFileVersion[0]);
                                                string[] versionsplit = versionInfo.ProductVersion.Split('+');
                                                string version = versionsplit[0];

                                                if (version == "")
                                                {
                                                    missingfiles.Add(splitFileVersion[0] + " - Invalid File");
                                                }
                                                else
                                                {
                                                    if (!HardwareInfo.CheckArchitectureFile(splitFileVersion[0]))
                                                    {
                                                        missingfiles.Add(splitFileVersion[0] + " - Wrong Architecture");
                                                    }
                                                    else
                                                    {
                                                        if (version != splitFileVersion[1])
                                                        {
                                                            missingfiles.Add(splitFileVersion[0] + " - Invalid Version " +
                                                                "(" + splitFileVersion[1] + " != " + version + ")");
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                missingfiles.Add(splitFileVersion[0] + " - Invalid File");
                                            }
                                        }
                                    }
                                }

                                if (missingfiles.Count != 0)
                                {
                                    var message = "Cannot launch GameLauncher. The following files are invalid:\n\n";

                                    foreach (var file in missingfiles)
                                    {
                                        message += "• " + file + "\n";
                                    }

                                    FunctionStatus.LauncherForceClose = true;
                                    MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                if (FunctionStatus.LauncherForceClose)
                                {
                                    FunctionStatus.ErrorCloseLauncher("Closing From Missing .dll Files Check", LauncherMustRestart);
                                }
                                else
                                {
                                    Start();
                                }
                            }
                            else
                            {
                                MessageBox.Show(null, "An instance of SBRW Launcher is already running", 
                                    "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        finally
                        {
                            mutex.Close();
                            mutex.Dispose();
                        }
                    }
                }
            }
        }

        private static void Start()
        {
            DiscordLauncherPresence.Start("Start Up", null);

            if (!UnixOS.Detected())
            {
                DiscordLauncherPresence.Status("Start Up", "Checking .NET Framework");
                try
                {
                    /* Check if User has a compatible .NET Framework Installed */
                    const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

                    using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
                    {
                        /* For now, allow edge case of Windows 8.0 to run .NET 4.6.1 where upgrading to 8.1 is not possible */
                        if (WindowsProductVersion.GetWindowsNumber() == 6.2)
                        {
                            if (ndpKey != null && ndpKey.GetValue("Release") != null && (int)ndpKey.GetValue("Release") >= 394254)
                            {
                                //Do Nothing
                            }
                            else
                            {
                                DialogResult frameworkError = MessageBox.Show(null, "This application requires a minimum version of the .NET Framework:\n" +
                                    " .NETFramework, Version=v4.6.1 \n\nDo you want to install this .NET Framework version now?",
                                    "GameLauncher.exe - This application could not be started.", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                                if (frameworkError == DialogResult.Yes)
                                {
                                    Process.Start("https://dotnet.microsoft.com/download/dotnet-framework/net461");
                                }

                                FunctionStatus.LauncherForceClose = true;
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
                                " .NETFramework, Version=v4.6.2 \n\nDo you want to install this .NET Framework version now?",
                                "GameLauncher.exe - This application could not be started.", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                            if (frameworkError == DialogResult.Yes)
                            {
                                Process.Start("https://dotnet.microsoft.com/download/dotnet-framework");
                            }

                            FunctionStatus.LauncherForceClose = true;
                        }
                    }
                }
                catch
                {
                    FunctionStatus.LauncherForceClose = true;
                }
            }

            if (FunctionStatus.LauncherForceClose)
            {
                FunctionStatus.ErrorCloseLauncher("Closing From .NET Framework Check", false);
            }
            else
            {
                InformationCache.CurrentLanguage = CultureInfo.CurrentCulture.Name.Split('-')[0].ToUpper();
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(true);

                /* Splash Screen */
                if (!Debugger.IsAttached)
                {
                    /* (Start Process) Sets up Theming */
                    Theming.CheckIfThemeExists();
                }

                LogToFileAddons.RemoveLogs();
                Log.StartLogging();

                Log.Info("CURRENT DATE: " + Time.GetTime("Date"));
                Log.Checking("LAUNCHER MIGRATION: Appdata and/or Roaming Folders");
                /* Deletes Folders that will Crash the Launcher (Cleanup Migration) */
                try
                {
                    if (Directory.Exists(Strings.Encode(Path.Combine(Locations.LocalAppDataFolder, "Soapbox_Race_World"))))
                    {
                        Directory.Delete(Strings.Encode(Path.Combine(Locations.LocalAppDataFolder, "Soapbox_Race_World")), true);
                    }
                    if (Directory.Exists(Strings.Encode(Path.Combine(Locations.RoamingAppDataFolder, "Soapbox_Race_World"))))
                    {
                        Directory.Delete(Strings.Encode(Path.Combine(Locations.RoamingAppDataFolder, "Soapbox_Race_World")), true);
                    }
                    if (Directory.Exists(Strings.Encode(Path.Combine(Locations.LocalAppDataFolder, "SoapBoxRaceWorld"))))
                    {
                        Directory.Delete(Strings.Encode(Path.Combine(Locations.LocalAppDataFolder, "SoapBoxRaceWorld")), true);
                    }
                    if (Directory.Exists(Strings.Encode(Path.Combine(Locations.RoamingAppDataFolder, "SoapBoxRaceWorld"))))
                    {
                        Directory.Delete(Strings.Encode(Path.Combine(Locations.RoamingAppDataFolder, "SoapBoxRaceWorld")), true);
                    }
                    if (Directory.Exists(Strings.Encode(Path.Combine(Locations.LocalAppDataFolder, "WorldUnited.gg"))))
                    {
                        Directory.Delete(Strings.Encode(Path.Combine(Locations.LocalAppDataFolder, "WorldUnited.gg")), true);
                    }
                    if (Directory.Exists(Strings.Encode(Path.Combine(Locations.RoamingAppDataFolder, "WorldUnited.gg"))))
                    {
                        Directory.Delete(Strings.Encode(Path.Combine(Locations.RoamingAppDataFolder, "WorldUnited.gg")), true);
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER MIGRATION", null, Error, null, true);
                }
                Log.Completed("LAUNCHER MIGRATION");

                Log.Checking("LAUNCHER XML: If File Exists or Not");
                DiscordLauncherPresence.Status("Start Up", "Checking if UserSettings XML Exists");
                /* Create Default Configuration Files (if they don't already exist) */
                if (!File.Exists(Locations.UserSettingsXML))
                {
                    try
                    {
                        if (!Directory.Exists(Locations.UserSettingsFolder))
                        {
                            Directory.CreateDirectory(Locations.UserSettingsFolder);
                        }

                        File.WriteAllBytes(Locations.UserSettingsXML, ExtractResource.AsByte("GameLauncher.Resources.UserSettings.UserSettings.xml"));
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("LAUNCHER XML", null, Error, null, true);
                    }
                }
                Log.Completed("LAUNCHER XML");

                string Insider = string.Empty;
                if (EnableInsiderDeveloper.Allowed())
                {
                    Insider = "DEV TEST ";
                }
                else if (EnableInsiderBetaTester.Allowed())
                {
                    Insider = "BETA TEST ";
                }

                Log.Build(Insider + "BUILD: GameLauncher " + Application.ProductVersion + "_" + InsiderInfo.BuildNumberOnly());

                Log.Checking("OS: Detecting");
                DiscordLauncherPresence.Status("Start Up", "Checking Operating System");
                try
                {
                    if (UnixOS.Detected())
                    {
                        InformationCache.OSName = UnixOS.FullName();
                        Log.System("SYSTEM: Detected OS: " + InformationCache.OSName);
                    }
                    else
                    {
                        InformationCache.OSName = WindowsProductVersion.ConvertWindowsNumberToName();
                        Log.System("SYSTEM: Detected OS: " + InformationCache.OSName);
                        Log.System("SYSTEM: Windows Build: " + WindowsProductVersion.GetWindowsBuildNumber());
                        Log.System("SYSTEM: NT Version: " + Environment.OSVersion);
                        Log.System("SYSTEM: Video Card: " + HardwareInfo.GPU.CardName());
                        Log.System("SYSTEM: Driver Version: " + HardwareInfo.GPU.DriverVersion());
                    }
                    Log.Completed("OS: Detected");
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("SYSTEM", null, Error, null, true);
                    FunctionStatus.LauncherForceCloseReason = "Operating System Detection Check Encountered an Error.\n" + Error.Message;
                    FunctionStatus.LauncherForceClose = true;
                }

                if (FunctionStatus.LauncherForceClose)
                {
                    FunctionStatus.ErrorCloseLauncher("Closing From Operating System Check", false);
                }
                else
                {
                    /* Set Launcher Directory */
                    Log.Checking("SETUP: Setting Launcher Folder Directory");
                    Directory.SetCurrentDirectory(Locations.LauncherFolder);
                    Log.Completed("SETUP: Current Directory now Set at -> " + Locations.LauncherFolder);

                    if (!UnixOS.Detected())
                    {
                        Log.Checking("FOLDER LOCATION: Checking Launcher Folder Directory");
                        DiscordLauncherPresence.Status("Start Up", "Checking Launcher Folder Locations");

                        switch (FunctionStatus.CheckFolder(Locations.LauncherFolder))
                        {
                            case FolderType.IsTempFolder:
                            case FolderType.IsUsersFolders:
                            case FolderType.IsProgramFilesFolder:
                            case FolderType.IsWindowsFolder:
                            case FolderType.IsRootFolder:
                                String constructMsg = String.Empty;

                                constructMsg += "Using this location for GameLauncher is not allowed.\n\n";
                                constructMsg += "The following locations are also NOT allowed:\n";
                                constructMsg += "• X:\\GameLauncher.exe (Root of Drive, such as C:\\ or D:\\, must be in a folder)\n";
                                constructMsg += "• C:\\Program Files\n";
                                constructMsg += "• C:\\Program Files (x86)\n";
                                constructMsg += "• C:\\Users (Includes 'Desktop', 'Documents', 'Downloads')\n";
                                constructMsg += "• C:\\Windows\n\n";
                                constructMsg += "Instead, move the Launcher folder to someplace like:\n";
                                constructMsg += "• 'C:\\Soapbox Race World' or 'C:\\SBRW'\n";
                                constructMsg += "(Or any other NTFS 'Local Disk' location such as 'D:')\n\n";

                                MessageBox.Show(null, constructMsg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                FunctionStatus.LauncherForceClose = true;
                                break;
                        }

                        Log.Completed("FOLDER LOCATION: Done");
                    }

                    if (FunctionStatus.LauncherForceClose)
                    {
                        FunctionStatus.ErrorCloseLauncher("Closing From Invalid Launcher Location", false);
                    }
                    else
                    {
                        Log.Checking("WRITE TEST: Launcher Folder Test");
                        if (!FunctionStatus.HasWriteAccessToFolder(Locations.LauncherFolder))
                        {
                            MessageBox.Show("Unable to do a Test Write to Launcher Folder\nPermission Issue");
                        }
                        Log.Completed("WRITE TEST: Passed");

                        Log.Checking("INI FILES: Doing Nullsafe");
                        DiscordLauncherPresence.Status("Start Up", "Doing NullSafe ini Files");
                        FileSettingsSave.NullSafeSettings();
                        FileAccountSave.NullSafeAccount();
                        Log.Checking("INI FILES: Done");

                        /* Windows 7 Fix */
                        if (WindowsProductVersion.GetWindowsNumber() == 6.1 && string.IsNullOrWhiteSpace(FileSettingsSave.Win7UpdatePatches))
                        {
                            Log.Checking("SSL/TLS: Windows 7 Detected");
                            DiscordLauncherPresence.Status("Start Up", "Checking Windows 7 TLS/SSL Update");

                            try
                            {
                                if (!ManagementSearcher.GetInstalledHotFix("KB3020369") || !ManagementSearcher.GetInstalledHotFix("KB3125574"))
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
                                        RegistryKey key = Registry.LocalMachine.CreateSubKey(
                                            @"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client");
                                        key.SetValue("DisabledByDefault", 0x0);

                                        MessageBox.Show(null, "Registry option set, Remember that the changes may require a system reboot to take effect",
                                            "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        FileSettingsSave.Win7UpdatePatches = "1";
                                    }
                                    else
                                    {
                                        MessageBox.Show(null, "Roger that, There may be some issues connecting to the servers.",
                                            "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        FileSettingsSave.Win7UpdatePatches = "0";
                                    }

                                    FileSettingsSave.SaveSettings();
                                }

                                Log.Completed("SSL/TLS: Done");
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("SSL/TLS", null, Error, null, true);
                            }
                        }
                    }

                    Log.Checking("JSON: Servers File");
                    try
                    {
                        if (File.Exists(Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameOldServersJSON))))
                        {
                            if (File.Exists(Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON))))
                            {
                                File.Delete(Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON)));
                            }

                            File.Move(
                                Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameOldServersJSON)),
                                Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON)));
                            Log.Completed("JSON: Renaming Servers File");
                        }
                        else if (!File.Exists(Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON))))
                        {
                            try
                            {
                                File.WriteAllText(
                                    Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON)), "[]");
                                Log.Completed("JSON: Created Servers File");
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("JSON SERVER FILE", null, Error, null, true);
                            }
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("JSON SERVER FILE", null, Error, null, true);
                    }
                    Log.Checking("JSON: Done");

                    if (!string.IsNullOrWhiteSpace(FileSettingsSave.GameInstallation))
                    {
                        Log.Checking("CLEANLINKS: Game Path");
                        if (File.Exists(Locations.GameLinksFile))
                        {
                            ModNetHandler.CleanLinks(Locations.GameLinksFile, FileSettingsSave.GameInstallation);
                            Log.Completed("CLEANLINKS: Done");
                        }
                        else
                        {
                            Log.Completed("CLEANLINKS: Not Present");
                        }                        
                    }

                    Log.Checking("PROXY: Checking if Proxy Is Disabled from User Settings! It's value is " + FileSettingsSave.Proxy);
                    if (FileSettingsSave.Proxy == "0")
                    {
                        Log.Core("PROXY: Starting Proxy (From Startup)");
                        ServerProxy.Instance.Start("Splash Screen [Program.cs]");
                        Log.Completed("PROXY: Started");
                    }
                    else
                    {
                        Log.Completed("PROXY: Disabled");
                    }

                    Log.Info("REDISTRIBUTABLE: Moved to Function");
                    /* (Starts Function Chain) Check if Redistributable Packages are Installed */
                    Redistributable.Check();
                }
            }
        }            
    }
}