using System;
using System.Diagnostics;
using System.Globalization;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class WindowsProductVersion
    {
        /* Cached Results from Program.cs To be Referenced Quickly */

        /* Final output: 18362.1016 */
        public static int CachedWindowsBuildNumber = 0;
        /* Final output: 10.0 */
        public static double CachedWindowsNumber;

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
            /* Get the Kernel32 DLL File Version */
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
            /* Get the Kernel32 DLL File Version */
            FileVersionInfo osVersionInfo = FileVersionInfo.GetVersionInfo(Environment.GetEnvironmentVariable("windir") + @"\System32\Kernel32.dll");

            return double.Parse(osVersionInfo.FileMajorPart + "." + osVersionInfo.FileMinorPart, CultureInfo.InvariantCulture);
        }

        /* This Converts the OS Kernal Number to a Name */
        public static string ConvertWindowsNumberToName(double osVersionInfo)
        {
            string BitType = " 32 bit";
            if (Environment.Is64BitOperatingSystem == true)
            {
                BitType = " 64 Bit";
            }

            if (osVersionInfo == 10) return "Windows 10" + BitType;
            else if (osVersionInfo == 6.3) return "Windows 8.1" + BitType;
            else if (osVersionInfo == 6.2) return "Windows 8" + BitType;
            else if (osVersionInfo == 6.1) return "Windows 7" + BitType;
            else if (osVersionInfo == 6.0) return "Windows Vista" + BitType;
            else if (osVersionInfo == 5.1) return "Windows XP" + BitType;
            return "Windows -∞" + BitType;
        }
    }
}
