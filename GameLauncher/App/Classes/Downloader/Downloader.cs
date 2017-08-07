using System;
using System.Windows.Forms;

namespace GameLauncher {
    class Downloader {
        public static string ServerFile;

        public enum Mirror {
            EACDN,
            Tiktalik
        }

        public static void selectMirror(Mirror mirror) {
            if(mirror.ToString() == "EACDN") {
                ServerFile = "http://static.cdn.ea.com/blackbox/u/f/NFSWO/1614b/client";
            } else if(mirror.ToString() == "Tiktalik") {
                ServerFile = "http://nfsw-soapbox.metonator.p4.tiktalik.io/NFSWO/"; //Go ahead, check how exactly EA CDN stores NFSW files
            } else {
                throw new Exception("Failed to select mirror");
            }
        }

        public static void ValidateFiles() {
            
        }

        public static void StartDownload() {
            
        }
    }
}
