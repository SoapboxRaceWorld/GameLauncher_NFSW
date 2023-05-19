using SBRW.Launcher.RunTime.LauncherCore.Client;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Languages.Visual_Forms;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.App.UI_Forms;
using SBRW.Launcher.App.UI_Forms.Main_Screen;
using SBRW.Launcher.Core.Extension.Logging_;
using System;
#if !(RELEASE_UNIX || DEBUG_UNIX)
using System.Collections.Generic;
using SBRW.Launcher.RunTime.SystemPlatform.Components;
#endif
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
#if NETFRAMEWORK
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SBRW.Launcher.RunTime.SystemPlatform.Unix;

namespace SBRW.Launcher.Net
{
    internal static class Program
    {
        public static bool LauncherMustRestart { get; set; }
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs Error)
        {
            try
            {
                try
                {
                    if (Screen_Main.Screen_Instance != null)
                    {
                        if (Screen_Main.Screen_Instance.NotifyIcon_Notification.Visible)
                        {
                            Screen_Main.Screen_Instance.NotifyIcon_Notification.Visible = false;
                            Screen_Main.Screen_Instance.NotifyIcon_Notification.Dispose();
                        }
                    }
                }
                catch (Exception Error_Y)
                {
                    LogToFileAddons.OpenLog("Notification Disposal", string.Empty, Error_Y, string.Empty, true);
                }

                LogToFileAddons.OpenLog("Thread Exception", Translations.Database("Application_Exception_Thread") + ": ",
                    Error.Exception, "Error", false);

                try
                {
                    Process[] Its_The_Law = Process.GetProcessesByName("nfsw");
                    if (Its_The_Law != null)
                    {
                        if (Its_The_Law.Length > 0)
                        {
                            foreach (Process Papers_Please in Its_The_Law)
                            {
                                try
                                {
                                    if (!Process.GetProcessById(Papers_Please.Id).HasExited)
                                    {
                                        if (!Process.GetProcessById(Papers_Please.Id).CloseMainWindow())
                                        {
                                            Process.GetProcessById(Papers_Please.Id).Kill();
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch { }
            }
            finally
            {
                Application.Exit();
                // If in Console Mode or if Form is Hidden and/or for Background Threads
                Environment.Exit(Environment.ExitCode);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs Error)
        {
            try
            {
                try
                {
                    if (Screen_Main.Screen_Instance != null)
                    {
                        if (Screen_Main.Screen_Instance.NotifyIcon_Notification.Visible)
                        {
                            Screen_Main.Screen_Instance.NotifyIcon_Notification.Visible = false;
                            Screen_Main.Screen_Instance.NotifyIcon_Notification.Dispose();
                        }
                    }
                }
                catch (Exception Error_Y)
                {
                    LogToFileAddons.OpenLog("Notification Disposal", string.Empty, Error_Y, string.Empty, true);
                }

                LogToFileAddons.OpenLog("Unhandled Exception", Translations.Database("Application_Exception_Unhandled") + ": ",
                    (Exception)Error.ExceptionObject, "Error", false);

                try
                {
                    Process[] Its_The_Law = Process.GetProcessesByName("nfsw");
                    if (Its_The_Law != null)
                    {
                        if (Its_The_Law.Length > 0)
                        {
                            foreach (Process Papers_Please in Its_The_Law)
                            {
                                try
                                {
                                    if (!Process.GetProcessById(Papers_Please.Id).HasExited)
                                    {
                                        if (!Process.GetProcessById(Papers_Please.Id).CloseMainWindow())
                                        {
                                            Process.GetProcessById(Papers_Please.Id).Kill();
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch { }
            }
            finally
            {
                Application.Exit();
                /* If in Console Mode or if Form is Hidden and/or for Background Threads */
                Environment.Exit(Environment.ExitCode);
            }
        }

        static void Start(string[] args)
        {
            try
            {
                Log.Info("MAINSCREEN: Program Started");
                Application.Run(new Parent_Screen());
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("Main Screen [Application Run]", "Launcher Encounterd an Error.", Error, "Error", false);
                FunctionStatus.ErrorCloseLauncher("Main Screen [Application Run]", false);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Main Screen [Application Run]", "Launcher Encounterd an Error.", Error, "Error", false);
                FunctionStatus.ErrorCloseLauncher("Main Screen [Application Run]", false);
            }
        }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#region Core application Settings set By the Developer
            /* Application and Thread Language */
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(InformationCache.Lang.Name);
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(Translations.UI(Translations.Application_Language = InformationCache.Lang.Name));
            /* Custom Error Handling */
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
#if !NETFRAMEWORK
#if NET5_0_OR_GREATER
            AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);
            Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
#endif
            ApplicationConfiguration.Initialize();
#else
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /* We need to set these once and Forget about it (Unless there is a bug such as HttpWebClient) */
            AppContext.SetSwitch("Switch.System.Net.DontEnableSchUseStrongCrypto", false);
            AppContext.SetSwitch("Switch.System.Net.DontEnableSystemDefaultTlsVersions", false);
            ServicePointManager.DnsRefreshTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
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
#endif
            #endregion
            #region Application Library File Checks and Process
            if (Debugger.IsAttached && !NFSW.IsRunning())
            {
                Start(args);
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
                    string LZMAPath = Path.Combine(Locations.LauncherFolder, Locations.NameLZMA);

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
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                            ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            WebClient Client = new WebClient
                            {
                                Encoding = Encoding.UTF8,
                                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                            };
#pragma warning restore SYSLIB0014 // Type or member is obsolete
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
                                LauncherMustRestart = true;

                                MessageBox.Show(null, Translations.Database("Program_TextBox_LZMA_Redownloaded"),
                                    "GameLauncher Restart Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            catch (Exception Error)
                            {
                                FunctionStatus.LauncherForceCloseReason = Error.Message;
                            }
                            finally
                            {
                                Client?.Dispose();
                            }
                        }
                        catch { }
                    }

                    if (FunctionStatus.LauncherForceClose)
                    {
                        FunctionStatus.ErrorCloseLauncher("Closing From Downloaded Missing LZMA", LauncherMustRestart);
                    }
#if !(RELEASE_UNIX || DEBUG_UNIX)
                    else if (UnixOS.Detected())
                    {
                        FunctionStatus.LauncherForceCloseReason = "Running Windows Build on Unix is Not Allowed";
                        FunctionStatus.ErrorCloseLauncher(FunctionStatus.LauncherForceCloseReason, LauncherMustRestart, null, true, true);
                    }
#else
                    else if (!UnixOS.Detected())
                    {
                        FunctionStatus.LauncherForceCloseReason = "Running Unix Build on Windows is Not Allowed";
                        FunctionStatus.ErrorCloseLauncher(FunctionStatus.LauncherForceCloseReason, LauncherMustRestart, null, true, true);
                    }
#endif
                    else
                    {
                        Mutex No_Java = new Mutex(false, "GameLauncherNFSW-MeTonaTOR");
                        try
                        {
                            if (No_Java.WaitOne(0, false))
                            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                /* MONO Hates this... */
                                string[] File_List =
                                {
                                        "DiscordRPC.dll - 1.1.3.18",
                                        "Flurl.dll - 3.0.6",
                                        "Flurl.Http.dll - 3.2.4",
                                        "LZMA.dll - 9.10 beta",
                                        "Newtonsoft.Json.dll - 13.0.3",
                                        "System.ValueTuple.dll - 4.6.26515.06 @BuiltBy: dlab-DDVSOWINAGE059 " +
                                        "@Branch: release/2.1 @SrcCode: https://github.com/dotnet/corefx/tree/30ab651fcb4354552bd4891619a0bdd81e0ebdbf",
                                        "WindowsFirewallHelper.dll - 2.2.0.85",
                                        "SBRW.Ini.Parser.dll - 3.0.2",
                                        "SBRW.Nancy.dll - 2.0.13",
                                        "SBRW.Nancy.Hosting.Self.dll - 2.0.11",
                                        "SBRW.Launcher.Core.dll - 0.3.0",
                                        "SBRW.Launcher.Core.Extra.dll - 0.3.6",
                                        "SBRW.Launcher.Core.Discord.dll - 0.3.0",
                                        "SBRW.Launcher.Core.Proxy.dll - 0.3.0",
                                        "SBRW.Launcher.Core.Theme.dll - 0.2.0",
                                        "SBRW.Launcher.Core.Downloader.dll - 0.3.7",
                                        "SBRW.Launcher.Core.Downloader.LZMA.dll - 0.3.1"
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
                                            string[] Version_Split = (Version_Info.ProductVersion ?? string.Empty).Split('+');
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
#endif
                                if (FunctionStatus.LauncherForceClose)
                                {
                                    FunctionStatus.ErrorCloseLauncher("Closing From Missing .dll Files Check", LauncherMustRestart);
                                }
                                else
                                {
                                    Start(args);
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
#endregion
        }
    }
}