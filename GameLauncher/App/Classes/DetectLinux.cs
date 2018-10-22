using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

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
                _linuxDistro = LinuxDistroInternal();
            }

            return _linuxDistro;
        }

        private static string LinuxDistroInternal() {
            if (!File.Exists("/etc/os-release"))
            {
                return "Linux";
            }
            using (var stream = new StreamReader("/etc/os-release"))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    var splits = line.Split(new[] { '=' }, 2);
                    if (splits[0] == "PRETTY_NAME")
                    {
                        var val = splits[1];
                        if (val[0] == '"')
                        {
                            val = val.Substring(1);
                        }
                        if (val[val.Length - 1] == '"')
                        {
                            val = val.Substring(0, val.Length - 1);
                        }
                        if (val == "Arch Linux" && new Random().NextDouble() < 0.2) {
                            return "btw i use arch";
                        }
                        return val;
                    }
                }
            }
            return "Linux";
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
