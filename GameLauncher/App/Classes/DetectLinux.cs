using Microsoft.Win32;
using System;

namespace GameLauncher.App.Classes {
    class DetectLinux {
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

        public static bool MonoDetected() {
            int SysVersion = (int)Environment.OSVersion.Platform;
            return (SysVersion == 4 || SysVersion == 6 || SysVersion == 128);
        }

		[Obsolete("LinuxDetected is deprecated, please use WineDetected instead.")]
        public static bool LinuxDetected() {
			return WineDetected();
        }

		public static bool NativeLinuxDetected() {
			return MonoDetected() && !WineDetected();
		}
    }
}
