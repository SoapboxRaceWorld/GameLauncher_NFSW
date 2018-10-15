using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace GameLauncher.App.Classes {
    class DetectLinux {
        private static string _kernelName = null;
        private static string _linuxDistro = null;

        public static bool WineDetected() {
            bool wine = false;
            try {
                RegistryKey regKey = Registry.CurrentUser;
                RegistryKey rkTest = regKey.OpenSubKey(@"Software\Wine");

                if (!String.IsNullOrEmpty(rkTest.ToString())) {
                    wine = true;
                }
            } catch {
                wine = false;
            }

            return wine;
        }

        public static bool UnixDetected() {
            // TODO: Check if wine gets detected by this
            int SysVersion = (int)Environment.OSVersion.Platform;
            return (SysVersion == 4 || SysVersion == 6 || SysVersion == 128);
        }

		public static bool LinuxDetected() {
            return UnixDetected() && KernelName() != "Darwin";
		}

        public static bool MacOSDetected() {
            return UnixDetected() && KernelName() == "Darwin";
        }

        public static string Distro() {
            if(_linuxDistro == null) {
                var ps = new ProcessStartInfo
                {
                    FileName = "lsb_release",
                    Arguments = "-d",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                var p = Process.Start(ps);
                _linuxDistro = p.StandardOutput.ReadLine().Replace("Description:", "").Trim();
                p.StandardOutput.ReadToEnd();
            }

            return _linuxDistro;
        }

        private static string KernelName() {
            if (_kernelName == null)
            {
                var ps = new ProcessStartInfo
                {
                    FileName = "/bin/uname",
                    Arguments = "-s",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                var p = Process.Start(ps);
                _kernelName = p.StandardOutput.ReadLine();
                p.StandardOutput.ReadToEnd();
            }
            return _kernelName;
        }
    }
}
