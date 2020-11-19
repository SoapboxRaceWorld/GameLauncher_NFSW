using System;
using System.Diagnostics;
using System.Globalization;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class WindowsProductVersion
    {
        /// <summary>
        /// Get the Build number of the installed copy of Windows
        /// </summary>
        /// <returns>Microsoft Windows Build number as integer</returns>
        /// Examples:
        /// - Windows 10: {OS Build} 18362.1016
        /// - Windows 7: {OS Build} 6.1.7601
        /// https://docs.microsoft.com/en-us/windows/release-information/
        public static int GetWindowsBuildNumber()
        {
            // Get the Kernel32 DLL File Version
            FileVersionInfo osVersionInfo = FileVersionInfo.GetVersionInfo(Environment.GetEnvironmentVariable("windir") + @"\System32\Kernel32.dll");

            return osVersionInfo.FileBuildPart;
        }

        /// <summary>
        /// Get the Platform Version of the installed copy of Windows
        /// </summary>
        /// <returns>Microsoft Windows Platform version as double</returns>
        /// Examples: 
        /// - Windows 10: 10.0
        /// - Windows 7: 6.1
        /// https://docs.microsoft.com/en-us/windows/win32/sysinfo/operating-system-version
        /// For Windows 7, 8 and 8.1 please use the following
        /// https://www.lifewire.com/windows-version-numbers-2625171
        public static double GetWindowsNumber()
        {
            // Get the Kernel32 DLL File Version
            FileVersionInfo osVersionInfo = FileVersionInfo.GetVersionInfo(Environment.GetEnvironmentVariable("windir") + @"\System32\Kernel32.dll");

            return double.Parse(osVersionInfo.FileMajorPart + "." + osVersionInfo.FileMinorPart, CultureInfo.InvariantCulture);
        }
    }
}
