using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.Client;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Languages.Visual_Forms;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Components;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.App.UI_Forms.Splash_Screen;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GameLauncher
{
    static class Program
    {
        public static bool LauncherMustRestart = false;

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs Error)
        {
            try
            {
                LogToFileAddons.OpenLog("Thread Exception", Translations.Database("Application_Exception_Thread") + ": ",
                    Error.Exception, "Error", false);

                try
                {
                    Process[] allOfThem = Process.GetProcessesByName("nfsw");
                    if (allOfThem != null && allOfThem.Any())
                    {
                        foreach (var oneProcess in allOfThem)
                        {
                            Process.GetProcessById(oneProcess.Id).Kill();
                        }
                    }
                }
                catch { }
            }
            finally
            {
                Application.Exit();
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs Error)
        {
            try
            {
                LogToFileAddons.OpenLog("Unhandled Exception", Translations.Database("Application_Exception_Unhandled") + ": ",
                    (Exception)Error.ExceptionObject, "Error", false);

                try
                {
                    Process[] allOfThem = Process.GetProcessesByName("nfsw");
                    if (allOfThem != null && allOfThem.Any())
                    {
                        foreach (var oneProcess in allOfThem)
                        {
                            Process.GetProcessById(oneProcess.Id).Kill();
                        }
                    }
                }
                catch { }
            }
            finally
            {
                Application.Exit();
            }
        }

        [STAThread]
        static void Main()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(InformationCache.Lang.Name);
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(Translations.UI(Translations.Application_Language = InformationCache.Lang.Name));
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            Application.EnableVisualStyles();
            /* We need to set these once and Forget about it (Unless there is a bug such as HttpWebClient) */
            AppContext.SetSwitch("Switch.System.Net.DontEnableSchUseStrongCrypto", false);
            AppContext.SetSwitch("Switch.System.Net.DontEnableSystemDefaultTlsVersions", false);
            ServicePointManager.DnsRefreshTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = (Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                bool isOk = true;
                if (sslPolicyErrors != SslPolicyErrors.None)
                {
                    for (int i = 0; i < chain.ChainStatus.Length; i++)
                    {
                        if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                        {
                            continue;
                        }
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 15);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                            break;
                        }
                    }
                }
                return isOk;
            };

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
                        MessageBox.Show(null, Translations.Database("Program_TextBox_GameIsRunning"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (NFSW.DetectGameLauncherSimplified())
                    {
                        MessageBox.Show(null, Translations.Database("Program_TextBox_SimplifiedIsRunning"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        MessageBox.Show(null, Translations.Database("Program_TextBox_SBRWIsRunning"), "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    FunctionStatus.LauncherForceClose = true;
                }

                if (FunctionStatus.LauncherForceClose)
                {
                    FunctionStatus.ErrorCloseLauncher("User Tried to Launch SBRW Launcher with one Running Already", false);
                }
                else
                {
                    /* Check if File needs to be Downloaded */
                    string LZMAPath = Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameLZMA));

                    if (File.Exists(LZMAPath))
                    {
                        try
                        {
                            if (new FileInfo(LZMAPath).Length == 0)
                            {
                                File.Delete(LZMAPath);
                            }
                        }
                        catch { }
                    }
                    /* INFO: this is here because this dll is necessary for downloading game files and I want to make it async.
                    Updated RedTheKitsune Code so it downloads the file if its missing.
                    It also restarts the launcher if the user click on yes on Prompt. - DavidCarbon */
                    if (!File.Exists("LZMA.dll"))
                    {
                        try
                        {
                            Uri URLCall = new Uri(URLs.File + "/LZMA.dll");
                            ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            WebClient Client = new WebClient
                            {
                                Encoding = Encoding.UTF8
                            };
                            Client.Headers.Add("user-agent", "SBRW Launcher " +
                                Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            Client.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                            {
                                if (File.Exists(LZMAPath))
                                {
                                    try
                                    {
                                        if (new FileInfo(LZMAPath).Length == 0)
                                        {
                                            File.Delete(LZMAPath);
                                        }
                                    }
                                    catch { }
                                }
                            };

                            FunctionStatus.LauncherForceClose = true;

                            try
                            {
                                Client.DownloadFile(URLCall, LZMAPath);

                                if (MessageBox.Show(null, Translations.Database("Program_TextBox_LZMA_Redownloaded"),
                                    "GameLauncher Restart Required",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                {
                                    LauncherMustRestart = true;
                                }
                            }
                            catch (Exception Error)
                            {
                                FunctionStatus.LauncherForceCloseReason = Error.Message;
                            }
                            finally
                            {
                                if (Client != null)
                                {
                                    Client.Dispose();
                                }
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
                        Mutex No_Java = new Mutex(false, "GameLauncherNFSW-MeTonaTOR");
                        try
                        {
                            if (No_Java.WaitOne(0, false))
                            {
                                if (!UnixOS.Detected())
                                {
                                    /* MONO Hates this... */
                                    string[] File_List =
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

                                    List<string> Missing_File_List = new List<string>();

                                    foreach (string File_String in File_List)
                                    {
                                        string[] Split_File_Version = File_String.Split(new string[] { " - " }, StringSplitOptions.None);

                                        if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), Split_File_Version[0])))
                                        {
                                            Missing_File_List.Add(Split_File_Version[0] + " - " + Translations.Database("Program_TextBox_File_NotFound"));
                                        }
                                        else
                                        {
                                            try
                                            {
                                                FileVersionInfo Version_Info = FileVersionInfo.GetVersionInfo(Split_File_Version[0]);
                                                string[] Version_Split = Version_Info.ProductVersion.Split('+');
                                                string File_Version = Version_Split[0];

                                                if (File_Version == "")
                                                {
                                                    Missing_File_List.Add(Split_File_Version[0] + " - " + Translations.Database("Program_TextBox_File_Invalid"));
                                                }
                                                else
                                                {
                                                    if (!HardwareInfo.CheckArchitectureFile(Split_File_Version[0]))
                                                    {
                                                        Missing_File_List.Add(Split_File_Version[0] + " - " + Translations.Database("Program_TextBox_File_Invalid_CPU"));
                                                    }
                                                    else
                                                    {
                                                        if (File_Version != Split_File_Version[1])
                                                        {
                                                            Missing_File_List.Add(Split_File_Version[0] + " - " + Translations.Database("Program_TextBox_File_Invalid_Version") +
                                                                "(" + Split_File_Version[1] + " != " + File_Version + ")");
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                Missing_File_List.Add(Split_File_Version[0] + " - " + Translations.Database("Program_TextBox_File_Invalid"));
                                            }
                                        }
                                    }

                                    if (Missing_File_List.Count != 0)
                                    {
                                        string Message_Display = Translations.Database("Program_TextBox_File_Invalid_Start");

                                        foreach (string File_String in Missing_File_List)
                                        {
                                            Message_Display += "• " + File_String + "\n";
                                        }

                                        FunctionStatus.LauncherForceClose = true;
                                        MessageBox.Show(null, Message_Display, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
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
                                MessageBox.Show(null, Translations.Database("Program_TextBox_SBRWIsRunning"),
                                    "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        finally
                        {
                            No_Java.Close();
                            No_Java.Dispose();
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
                    if (int.TryParse(RegistryCore.Read("Release", @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"), out int NetFrame_Version))
                    {
                        /* For now, allow edge case of Windows 8.0 to run .NET 4.6.1 where upgrading to 8.1 is not possible */
                        if (WindowsProductVersion.GetWindowsNumber() == 6.2 && NetFrame_Version <= 394254)
                        {
                            if (MessageBox.Show(null, Translations.Database("Program_TextBox_NetFrame_P1") +
                            " .NETFramework, Version=v4.6.1 \n\n" + Translations.Database("Program_TextBox_NetFrame_P2"),
                            "GameLauncher.exe - " + Translations.Database("Program_TextBox_NetFrame_P3"),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                            {
                                Process.Start("https://dotnet.microsoft.com/download/dotnet-framework/net461");
                            }

                            FunctionStatus.LauncherForceClose = true;
                        }
                        /* Otherwise, all other OS Versions should have 4.6.2 as a Minimum Version */
                        else if (NetFrame_Version <= 394802)
                        {
                            if (MessageBox.Show(null, Translations.Database("Program_TextBox_NetFrame_P1") +
                            " .NETFramework, Version=v4.6.2 \n\n" + Translations.Database("Program_TextBox_NetFrame_P2"),
                            "GameLauncher.exe - " + Translations.Database("Program_TextBox_NetFrame_P3"),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                            {
                                Process.Start("https://dotnet.microsoft.com/download/dotnet-framework");
                            }

                            FunctionStatus.LauncherForceClose = true;
                        }
                        else
                        {
                            Log.System("NET-FRAMEWORK: Supported Installed Version");
                        }
                    }
                    else
                    {
                        Log.Warning("NET-FRAMEWORK: Failed to Parse Version");
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
                /* Splash Screen */
                if (!Debugger.IsAttached)
                {
                    /* Starts Splash Screen */
                    SplashScreen.ThreadStatus("Start");
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
                        Log.System("SYSTEM: NT Version: " + Environment.OSVersion.VersionString);
                        Log.System("SYSTEM: Video Card: " + HardwareInfo.GPU.CardName());
                        Log.System("SYSTEM: Driver Version: " + HardwareInfo.GPU.DriverVersion());
                    }
                    Log.Completed("OS: Detected");
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("SYSTEM", null, Error, null, true);
                    FunctionStatus.LauncherForceCloseReason = "Code: 0\n" + Translations.Database("Program_TextBox_System_Detection") + "\n" + Error.Message;
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
                                String Constructed_Msg = String.Empty;

                                Constructed_Msg += Translations.Database("Program_TextBox_Folder_Check_Launcher") + "\n\n";
                                Constructed_Msg += Translations.Database("Program_TextBox_Folder_Check_Launcher_P2") + "\n";
                                Constructed_Msg += "• X:\\GameLauncher.exe " + Translations.Database("Program_TextBox_Folder_Check_Launcher_P3") + "\n";
                                Constructed_Msg += "• C:\\Program Files\n";
                                Constructed_Msg += "• C:\\Program Files (x86)\n";
                                Constructed_Msg += "• C:\\Users " + Translations.Database("Program_TextBox_Folder_Check_Launcher_P4") + "\n";
                                Constructed_Msg += "• C:\\Windows\n\n";
                                Constructed_Msg += Translations.Database("Program_TextBox_Folder_Check_Launcher_P5") + "\n";
                                Constructed_Msg += "• 'C:\\Soapbox Race World' " + Translations.Database("Program_TextBox_Folder_Check_Launcher_P6") + " 'C:\\SBRW'\n";
                                Constructed_Msg += Translations.Database("Program_TextBox_Folder_Check_Launcher_P7") + "\n\n";

                                MessageBox.Show(null, Constructed_Msg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            MessageBox.Show(Translations.Database("Program_TextBox_Folder_Write_Test"));
                        }
                        Log.Completed("WRITE TEST: Passed");

                        Log.Checking("INI FILES: Doing Nullsafe");
                        DiscordLauncherPresence.Status("Start Up", "Doing NullSafe ini Files");
                        FileSettingsSave.NullSafeSettings();
                        FileAccountSave.NullSafeAccount();
                        Log.Completed("INI FILES: Done");
                        /* Sets up Theming */
                        Theming.CheckIfThemeExists();

                        Log.Function("APPLICATION: Setting Language");
                        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(Translations.UI(Translations.Application_Language = FileSettingsSave.Lang.ToLower(), true));
                        Log.Completed("APPLICATION: Done Setting Language '" + Translations.UI(Translations.Application_Language) + "'");

                        /* Windows 7 TLS Check */
                        if (WindowsProductVersion.GetWindowsNumber() == 6.1)
                        {
                            Log.Checking("SSL/TLS: Windows 7 Detected");
                            DiscordLauncherPresence.Status("Start Up", "Checking Windows 7 SSL/TLS");

                            try
                            {
                                String MessageBoxPopupTLS = String.Empty;
                                string keyName = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client";
                                string subKey = "DisabledByDefault";

                                if (Registry.GetValue(keyName, subKey, null) == null)
                                {
                                    MessageBoxPopupTLS = Translations.Database("Program_TextBox_W7_TLS_P1") + "\n\n";

                                    MessageBoxPopupTLS += "- HKLM/SYSTEM/CurrentControlSet/Control/SecurityProviders\n  /SCHANNEL/Protocols/TLS 1.2/Client\n";
                                    MessageBoxPopupTLS += "- Value: DisabledByDefault -> 0\n\n";

                                    MessageBoxPopupTLS += Translations.Database("Program_TextBox_W7_TLS_P2") + "\n\n";
                                    MessageBoxPopupTLS += Translations.Database("Program_TextBox_W7_TLS_P3");

                                    /* There is only 'OK' Available because this IS Required */
                                    if (MessageBox.Show(null, MessageBoxPopupTLS, "SBRW Launcher",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                                    {
                                        RegistryCore.Write("DisabledByDefault", 0x0,
                                            @"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client");
                                        MessageBox.Show(null, Translations.Database("Program_TextBox_W7_TLS_P4"),
                                            "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    Log.Completed("SSL/TLS: Added Registry Key");
                                }
                                else
                                {
                                    Log.Completed("SSL/TLS: Done");
                                }
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("SSL/TLS", null, Error, null, true);
                            }
                        }

                        /* Windows 7 HotFix Check */
                        if (WindowsProductVersion.GetWindowsNumber() == 6.1 && string.IsNullOrWhiteSpace(FileSettingsSave.Win7UpdatePatches))
                        {
                            Log.Checking("HotFixes: Windows 7 Detected");
                            DiscordLauncherPresence.Status("Start Up", "Checking Windows 7 HotFixes");

                            try
                            {
                                if (!ManagementSearcher.GetInstalledHotFix("KB3020369") || !ManagementSearcher.GetInstalledHotFix("KB3125574"))
                                {
                                    String MessageBoxPopupKB = String.Empty;
                                    MessageBoxPopupKB = Translations.Database("Program_TextBox_W7_KB_P1") + "\n";
                                    MessageBoxPopupKB += Translations.Database("Program_TextBox_W7_KB_P2") + "\n\n";

                                    if (!ManagementSearcher.GetInstalledHotFix("KB3020369"))
                                    {
                                        MessageBoxPopupKB += "- " + Translations.Database("Program_TextBox_W7_KB_P3") + " KB3020369\n";
                                    }

                                    if (!ManagementSearcher.GetInstalledHotFix("KB3125574"))
                                    {
                                        MessageBoxPopupKB += "- " + Translations.Database("Program_TextBox_W7_KB_P3") + " KB3125574\n";
                                    }
                                    MessageBoxPopupKB += "\n" + Translations.Database("Program_TextBox_W7_KB_P4") + "\n";

                                    if (MessageBox.Show(null, MessageBoxPopupKB, "SBRW Launcher",
                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                                    {
                                        /* Since it's Informational we just need to know if they clicked 'OK' */
                                        FileSettingsSave.Win7UpdatePatches = "1";
                                    }
                                    else
                                    {
                                        /* or if they clicked 'Cancel' */
                                        FileSettingsSave.Win7UpdatePatches = "0";
                                    }

                                    FileSettingsSave.SaveSettings();
                                }

                                Log.Completed("HotFixes: Done");
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("HotFixes", null, Error, null, true);
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