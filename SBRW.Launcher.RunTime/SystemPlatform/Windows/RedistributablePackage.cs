using SBRW.Launcher.RunTime.LauncherCore.Lists;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using System;
using System.IO;
using SBRW.Launcher.Core.Extension.Registry_;
#if !(RELEASE_UNIX || DEBUG_UNIX)
using SBRW.Launcher.RunTime.SystemPlatform.Unix;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Api_;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.RunTime.LauncherCore.Languages.Visual_Forms;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Net.Cache;
using System.Threading.Tasks;
#endif


// based on https://github.com/bitbeans/RedistributableChecker/blob/master/RedistributableChecker/RedistributablePackage.cs
namespace SBRW.Launcher.RunTime.SystemPlatform.Windows
{
    /// <summary>
    /// Microsoft Visual C++ Redistributable Package Versions
    /// </summary>
    public enum RedistributablePackageVersion
    {
        VC2015to2019x86,
        VC2015to2019x64,
    };

    /// <summary>
    ///	Class to detect installed Microsoft Redistributable Packages.
    /// </summary>
    /// <see cref="//https://stackoverflow.com/questions/12206314/detect-if-visual-c-redistributable-for-visual-studio-2012-is-installed"/>
    public static class RedistributablePackage
    {
        private static string InstalledVersion { get; set; } = string.Empty;
        /// <summary>
        /// Check if a Microsoft Redistributable Package is installed.
        /// </summary>
        /// <param name="Redistributable_Version">The package version to detect.</param>
        /// <returns><c>true</c> if the package is installed, otherwise <c>false</c></returns>
        public static bool IsInstalled(RedistributablePackageVersion Redistributable_Version)
        {
            switch (Redistributable_Version)
            {
                case RedistributablePackageVersion.VC2015to2019x86:
                    InstalledVersion = Registry_Core.Read("Version",
                            Path.Combine("SOFTWARE", "Microsoft", "VisualStudio", "14.0", "VC", "Runtimes", "x86"));
                    goto case RedistributablePackageVersion.VC2015to2019x64;
                case RedistributablePackageVersion.VC2015to2019x64:
                    if (string.IsNullOrWhiteSpace(InstalledVersion))
                    {
                        InstalledVersion = Registry_Core.Read("Version",
                            Path.Combine("SOFTWARE", "Microsoft", "VisualStudio", "14.0", "VC", "Runtimes", "x64"));
                    }

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(InstalledVersion))
                        {
                            if (InstalledVersion.StartsWith("v"))
                            {
                                InstalledVersion = InstalledVersion.Trim('v');
                            }

                            if (InstalledVersion.CompareTo("14.20") >= 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("Redistributable Package", string.Empty, Error, string.Empty, true);
                        return false;
                    }
                    finally
                    {
                        if (!string.IsNullOrWhiteSpace(InstalledVersion))
                        {
                            InstalledVersion = string.Empty;
                        }
                    }
                default:
                    return false;
            }
        }
    }

    class Redistributable
    {
        public static bool Error_Free { get; set; } = true;
#if !(RELEASE_UNIX || DEBUG_UNIX)
        public static async void Check()

#else
        public static void Check()
#endif
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
            LogToFileAddons.Parent_Log_Screen(2, "REDISTRIBUTABLE", "Is Installed or Not");
            Presence_Launcher.Status(0, "Checking Redistributable Package Visual Code 2015 to 2019");

            if (!RedistributablePackage.IsInstalled(RedistributablePackageVersion.VC2015to2019x86))
            {
                if (MessageBox.Show(Translations.Database("Redistributable_VC_32") +
                    "\n\n" + Translations.Database("Redistributable_VC_P2") +
                    "\n\n" + Translations.Database("Redistributable_VC_P3") +
                    "\n\n" + Translations.Database("Redistributable_VC_P4"),
                    Translations.Database("Redistributable_VC_P5"),
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            Uri URLCall = new Uri("https://aka.ms/vs/17/release/VC_redist.x86.exe");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                            ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            var Client = new WebClient()
                            {
                                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                            };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

                            if (!Launcher_Value.Launcher_Alternative_Webcalls())
                            {
                                Client = new WebClientWithTimeout() { CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) };
                            }
                            else
                            {
                                Client.Headers.Add("user-agent", "SBRW Launcher " +
                                Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            }

                            try
                            {
                                Client.DownloadFile(URLCall, "VC_redist.x86.exe");
                            }
                            catch (WebException Error)
                            {
                                API_Core.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                                    Error, Error.Response as HttpWebResponse);
                                if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                {
                                    LogToFileAddons.Parent_Log_Screen(5, "REDISTRIBUTABLE", Error.InnerException.Message, false, true);
                                }
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("REDISTRIBUTABLE", string.Empty, Error, string.Empty, true);
                                if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                {
                                    LogToFileAddons.Parent_Log_Screen(5, "REDISTRIBUTABLE", Error.InnerException.Message, false, true);
                                }
                            }
                            finally
                            {
                                Client?.Dispose();

#if !(RELEASE_UNIX || DEBUG_UNIX)
                                GC.Collect(); 
#endif
                            }
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("REDISTRIBUTABLE", string.Empty, Error, string.Empty, true);
                            if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                            {
                                LogToFileAddons.Parent_Log_Screen(5, "REDISTRIBUTABLE", Error.InnerException.Message, false, true);
                            }
                        }
                        finally
                        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                            GC.Collect(); 
#endif
                        }
                    });

                    if (File.Exists("VC_redist.x86.exe"))
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                                Process proc = Process.Start(new ProcessStartInfo
                                {
                                    Verb = "runas",
                                    Arguments = "/quiet",
                                    FileName = "VC_redist.x86.exe"
                                });
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                                proc?.WaitForExit((int)TimeSpan.FromMinutes(10).TotalMilliseconds);

                                if (proc == null)
                                {
                                    Error_Free = false;
                                    MessageBox.Show(Translations.Database("Redistributable_VC_P6"),
                                        Translations.Database("Redistributable_VC_P5"), MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                }
                                else if (proc != null)
                                {
                                    if (!proc.HasExited)
                                    {
                                        if (proc.Responding) { proc.CloseMainWindow(); }
                                        else { proc.Kill(); Error_Free = false; }
                                    }

                                    if (proc.ExitCode != 0)
                                    {
                                        Error_Free = false;
                                        LogToFileAddons.Parent_Log_Screen(5, "REDISTRIBUTABLE INSTALLER [EXIT CODE]", proc.ExitCode.ToString() +
                                            " HEX: (0x" + proc.ExitCode.ToString("X") + ")", false, true);
                                        MessageBox.Show(Translations.Database("Redistributable_VC_P7") + " " + proc.ExitCode.ToString() +
                                            " (0x" + proc.ExitCode.ToString("X") + ")" +
                                            "\n" + Translations.Database("Redistributable_VC_P8"),
                                            Translations.Database("Redistributable_VC_P5"), MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                                    }

                                    proc.Close();
                                }
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("REDISTRIBUTABLE x86 Process", string.Empty, Error, string.Empty, true);
                                if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                {
                                    LogToFileAddons.Parent_Log_Screen(5, "REDISTRIBUTABLE x86 Process", Error.InnerException.Message, false, true);
                                }
                                Error_Free = false;
                                MessageBox.Show(Translations.Database("Redistributable_VC_P9"),
                                    Translations.Database("Redistributable_VC_P5"), MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                            finally
                            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                GC.Collect(); 
#endif
                            }
                        });
                    }
                    else
                    {
                        Error_Free = false;
                        LogToFileAddons.Parent_Log_Screen(5, Translations.Database("Redistributable_VC_P10"),
                            Translations.Database("Redistributable_VC_P5"));
                        MessageBox.Show(Translations.Database("Redistributable_VC_P10"),
                            Translations.Database("Redistributable_VC_P5"), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Error_Free = false;
                    LogToFileAddons.Parent_Log_Screen(5, Translations.Database("Redistributable_VC_P8"),
                            Translations.Database("Redistributable_VC_P5"));
                    MessageBox.Show(Translations.Database("Redistributable_VC_P8"), Translations.Database("Redistributable_VC_P5"), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                LogToFileAddons.Parent_Log_Screen(1, "REDISTRIBUTABLE", "32-bit VC++ Redistributable Package is Installed");
            }

            if (Environment.Is64BitOperatingSystem)
            {
                if (!RedistributablePackage.IsInstalled(RedistributablePackageVersion.VC2015to2019x64))
                {
                    if (MessageBox.Show(Translations.Database("Redistributable_VC_64") +
                        "\n\n" + Translations.Database("Redistributable_VC_P2") +
                        "\n\n" + Translations.Database("Redistributable_VC_P3") +
                        "\n\n" + Translations.Database("Redistributable_VC_P4"),
                        Translations.Database("Redistributable_VC_P5"),
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                Uri URLCall = new Uri("https://aka.ms/vs/17/release/VC_redist.x64.exe");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                var Client = new WebClient()
                                {
                                    CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                                };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

                                if (!Launcher_Value.Launcher_Alternative_Webcalls())
                                {
                                    Client = new WebClientWithTimeout() { CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) };
                                }
                                else
                                {
                                    Client.Headers.Add("user-agent", "SBRW Launcher " +
                                    Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                }

                                try
                                {
                                    Client.DownloadFile(URLCall, "VC_redist.x64.exe");
                                }
                                catch (WebException Error)
                                {
                                    API_Core.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                                        Error, Error.Response as HttpWebResponse);
                                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                    {
                                        LogToFileAddons.Parent_Log_Screen(5, "REDISTRIBUTABLE x64", Error.InnerException.Message, false, true);
                                    }
                                }
                                catch (Exception Error)
                                {
                                    LogToFileAddons.OpenLog("REDISTRIBUTABLE", string.Empty, Error, string.Empty, true);
                                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                    {
                                        LogToFileAddons.Parent_Log_Screen(5, "REDISTRIBUTABLE x64", Error.InnerException.Message, false, true);
                                    }
                                }
                                finally
                                {
                                    Client?.Dispose();

#if !(RELEASE_UNIX || DEBUG_UNIX)
                                    GC.Collect(); 
#endif
                                }
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("REDISTRIBUTABLE x64", string.Empty, Error, string.Empty, true);
                                if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                {
                                    LogToFileAddons.Parent_Log_Screen(5, "REDISTRIBUTABLE x64", Error.InnerException.Message, false, true);
                                }
                            }
                            finally
                            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                GC.Collect(); 
#endif
                            }
                        });

                        if (File.Exists("VC_redist.x64.exe"))
                        {
                            await Task.Run(() =>
                            {
                                try
                                {
                                    var proc = Process.Start(new ProcessStartInfo
                                    {
                                        Verb = "runas",
                                        Arguments = "/quiet",
                                        FileName = "VC_redist.x64.exe"
                                    });

                                    proc?.WaitForExit((int)TimeSpan.FromMinutes(10).TotalMilliseconds);

                                    if (proc == null)
                                    {
                                        Error_Free = false;
                                        LogToFileAddons.Parent_Log_Screen(5, Translations.Database("Redistributable_VC_P6"),
                                            Translations.Database("Redistributable_VC_P5"));
                                        MessageBox.Show(Translations.Database("Redistributable_VC_P6"),
                                            Translations.Database("Redistributable_VC_P5"), MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                                    }
                                    else if (proc != null)
                                    {
                                        if (!proc.HasExited)
                                        {
                                            if (proc.Responding) { proc.CloseMainWindow(); }
                                            else { proc.Kill(); Error_Free = false; }
                                        }

                                        if (proc.ExitCode != 0)
                                        {
                                            Error_Free = false;
                                            LogToFileAddons.Parent_Log_Screen(5, "REDISTRIBUTABLE INSTALLER [EXIT CODE]", proc.ExitCode.ToString() +
                                                " HEX: (0x" + proc.ExitCode.ToString("X") + ")");
                                            MessageBox.Show(Translations.Database("Redistributable_VC_P7") + " " + proc.ExitCode.ToString() +
                                                " (0x" + proc.ExitCode.ToString("X") + ")" +
                                                "\n" + Translations.Database("Redistributable_VC_P8"),
                                                Translations.Database("Redistributable_VC_P5"), MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                                        }

                                        proc.Close();
                                    }
                                }
                                catch (Exception Error)
                                {
                                    LogToFileAddons.OpenLog("REDISTRIBUTABLE x64 INSTALLER", string.Empty, Error, string.Empty, true);
                                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                    {
                                        LogToFileAddons.Parent_Log_Screen(5, "REDISTRIBUTABLE x64 INSTALLER", Error.InnerException.Message, false, true);
                                    }
                                    Error_Free = false;
                                    MessageBox.Show(Translations.Database("Redistributable_VC_P9"),
                                        Translations.Database("Redistributable_VC_P5"), MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                }
                                finally
                                {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                    GC.Collect(); 
#endif
                                }
                            });
                        }
                        else
                        {
                            Error_Free = false;
                            LogToFileAddons.Parent_Log_Screen(5, Translations.Database("Redistributable_VC_P10"),
                                Translations.Database("Redistributable_VC_P5"));
                            MessageBox.Show(Translations.Database("Redistributable_VC_P10"),
                                Translations.Database("Redistributable_VC_P5"), MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Error_Free = false;
                        LogToFileAddons.Parent_Log_Screen(5, Translations.Database("Redistributable_VC_P8"),
                                Translations.Database("Redistributable_VC_P5"));
                        MessageBox.Show(Translations.Database("Redistributable_VC_P8"),
                            Translations.Database("Redistributable_VC_P5"), MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                {
                    LogToFileAddons.Parent_Log_Screen(1, "REDISTRIBUTABLE", "64-bit VC++ Redistributable Package is Installed");
                }
            }

            LogToFileAddons.Parent_Log_Screen(3, "REDISTRIBUTABLE", "Done");
#endif

        LogToFileAddons.Parent_Log_Screen(1, "LIST", "Moved to Function");
            /* (Start Process) Sets Up Langauge List */
            LanguageListUpdater.GetList();
        }
    }
}
