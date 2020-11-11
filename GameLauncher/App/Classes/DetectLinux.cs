using System;
using System.IO;

namespace GameLauncher.App.Classes
{
    class DetectLinux {
        private static string _linuxDistro = null;

		public static bool LinuxDetected() {
            return Type.GetType("Mono.Runtime") != null;
        }

        public static string Distro() {
            if(_linuxDistro == null) {
                _linuxDistro = LinuxDistroInternal();
            }

            return _linuxDistro;
        }

        public static bool IsValidWineMonoInstalled() {
            return true;
        }

        private static string LinuxDistroInternal() {
            if (!File.Exists("/etc/os-release")) {
                return "UNIX-Like System";
            }

            using (var stream = new StreamReader("/etc/os-release")) {
                string line;
                while ((line = stream.ReadLine()) != null) {
                    var splits = line.Split(new[] { '=' }, 2);
                    if (splits[0] == "PRETTY_NAME") {
                        var val = splits[1];

                        if (val[0] == '"') {
                            val = val.Substring(1);
                        }

                        if (val[val.Length - 1] == '"') {
                            val = val.Substring(0, val.Length - 1);
                        }

                        return val;
                    }
                }
            }

            return "Linux";
        }
    }
}
