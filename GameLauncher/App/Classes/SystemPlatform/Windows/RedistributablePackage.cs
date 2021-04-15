using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.Logger;
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
        public static async Task CheckAsync()
        {
            if (!DetectLinux.LinuxDetected())
            {
                await Task.Run(() => Installed());
            }
        }

        public static void Installed()
        {
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

                using (WebClientWithTimeout Client = new WebClientWithTimeout())
                {
                    try
                    {
                        Client.DownloadFile("https://aka.ms/vs/16/release/VC_redist.x86.exe", "VC_redist.x86.exe");
                    }
                    catch (Exception error)
                    {
                        Log.Error("LAUNCHER UPDATER: " + error.Message);
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
                            MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to download package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                    }
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

                    using (WebClientWithTimeout Client = new WebClientWithTimeout())
                    {
                        try
                        {
                            Client.DownloadFile("https://aka.ms/vs/16/release/VC_redist.x64.exe", "VC_redist.x64.exe");
                        }
                        catch (Exception error)
                        {
                            Log.Error("LAUNCHER UPDATER: " + error.Message);
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
                                MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Failed to download package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    }
}
