using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace GameLauncher.App.Classes {
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

        private static string LinuxDistroInternal() {
            if (!File.Exists("Z:/etc/os-release")) {
                return "UNIX-Like System";
            }

            using (var stream = new StreamReader("Z:/etc/os-release")) {
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

                        if (val == "Arch Linux" && new Random().NextDouble() < 0.2) {
                            return "btw i use arch";
                        }

                        return val;
                    }
                }
            }

            return "Linux";
        }
    }
}
