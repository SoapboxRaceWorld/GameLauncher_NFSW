using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.Logger {
    class Log {
        private static void _toFile(string text, string errorname = "DEBUG") {
            using (StreamWriter stream = new StreamWriter("log.txt", true)) {
                stream.WriteLine("["+errorname+"] " + text);
                Console.WriteLine("[" + errorname + "] " + text);
            }
        }

        public static void Debug(string text) {
            _toFile(text, "DEBUG");
        }

        public static void Info(string text) {
            _toFile(text, "INFO");
        }

        public static void Warning(string text) {
            _toFile(text, "WARN");
        }

        public static void Error(string text) {
            _toFile(text, "ERROR");
        }
    }
}
