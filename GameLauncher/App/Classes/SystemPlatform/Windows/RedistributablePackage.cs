using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Components;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using Microsoft.Win32;

// based on https://github.com/bitbeans/RedistributableChecker/blob/master/RedistributableChecker/RedistributablePackage.cs
namespace GameLauncher.App.Classes.SystemPlatform.Windows
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
        /// <summary>
        /// Check if a Microsoft Redistributable Package is installed.
        /// </summary>
        /// <param name="redistributableVersion">The package version to detect.</param>
        /// <returns><c>true</c> if the package is installed, otherwise <c>false</c></returns>
        public static bool IsInstalled(RedistributablePackageVersion redistributableVersion)
        {
            try
            {
                switch (redistributableVersion)
                {
                    case RedistributablePackageVersion.VC2015to2019x86:
                        var parametersVc2015to2019x86 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x86", false);
                        if (parametersVc2015to2019x86 == null) return false;
                        var vc2015to2019x86Version = parametersVc2015to2019x86.GetValue("Version");
                        if (((string)vc2015to2019x86Version).StartsWith("v14.2"))
                        {
                            return true;
                        }
                        break;
                    case RedistributablePackageVersion.VC2015to2019x64:
                        var parametersVc2015to2019x64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64", false);
                        if (parametersVc2015to2019x64 == null) return false;
                        var vc2015to2019x64Version = parametersVc2015to2019x64.GetValue("Version");
                        if (((string)vc2015to2019x64Version).StartsWith("v14.2"))
                        {
                            return true;
                        }
                        break;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    class Redistributable
    {
        public static bool ErrorFree = true;
        public static void Check()
        {
            if (!DetectLinux.LinuxDetected())
            {
                Log.Checking("REDISTRIBUTABLE: Is Installed or Not");
                DiscordLauncherPresense.Status("Start Up", "Checking Redistributable Package Visual Code 2015 to 2019");

                if (!RedistributablePackage.IsInstalled(RedistributablePackageVersion.VC2015to2019x86))
                {
                    var result = MessageBox.Show(
                        "You do not have the 32-bit 2015-2019 VC++ Redistributable Package installed.\n \nThis will install in the Background\n \nThis may restart your computer. \n \nClick OK to install it.",
                        "Compatibility",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.OK)
                    {
                        ErrorFree = false;
                        MessageBox.Show("The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    try
                    {
                        FunctionStatus.TLS();
                        Uri URLCall = new Uri("https://aka.ms/vs/16/release/VC_redist.x86.exe");
                        ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                        WebClient Client = new WebClient();
                        Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        Client.DownloadFile(URLCall, "VC_redist.x86.exe");
                    }
                    catch (Exception Error)
                    {
                        Log.Error("REDISTRIBUTABLE: " + Error.Message);
                        Log.ErrorIC("REDISTRIBUTABLE: " + Error.HResult);
                        Log.ErrorFR("REDISTRIBUTABLE: " + Error.ToString());
                    }

                    if (File.Exists("VC_redist.x86.exe"))
                    {
                        var proc = Process.Start(new ProcessStartInfo
                        {
                            Verb = "runas",
                            Arguments = "/quiet",
                            FileName = "VC_redist.x86.exe"
                        });

                        if (proc == null)
                        {
                            ErrorFree = false;
                            MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        ErrorFree = false;
                        MessageBox.Show("Failed to download package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                    }
                }

                if (Environment.Is64BitOperatingSystem)
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
                            ErrorFree = false;
                            MessageBox.Show("The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }

                        try
                        {
                            FunctionStatus.TLS();
                            Uri URLCall = new Uri("https://aka.ms/vs/16/release/VC_redist.x64.exe");
                            ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            WebClient Client = new WebClient();
                            Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            Client.DownloadFile(URLCall, "VC_redist.x64.exe");
                        }
                        catch (Exception Error)
                        {
                            Log.Error("REDISTRIBUTABLE x64: " + Error.Message);
                            Log.ErrorIC("REDISTRIBUTABLE x64: " + Error.HResult);
                            Log.ErrorFR("REDISTRIBUTABLE x64: " + Error.ToString());
                        }

                        if (File.Exists("VC_redist.x64.exe"))
                        {
                            var proc = Process.Start(new ProcessStartInfo
                            {
                                Verb = "runas",
                                Arguments = "/quiet",
                                FileName = "VC_redist.x64.exe"
                            });

                            if (proc == null)
                            {
                                ErrorFree = false;
                                MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            ErrorFree = false;
                            MessageBox.Show("Failed to download package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                        }
                    }
                }

                Log.Completed("REDISTRIBUTABLE: Done");
            }

            Log.Info("ID: Moved to Function");
            /* (Start Process) */
            HardwareID.FingerPrint.Get();
        }
    }
}
