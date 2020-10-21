using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.ModNetReloaded {
    public class ElectronIndex {
        public string file { get; set; }
        public string hash { get; set; } //base64(sha256(file))
    }

    class ElectronModNet {
        public static string calculateHash(string file) {
            if (!File.Exists(file)) return String.Empty;

            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] retVal = new byte[] { };

            using (var test = File.OpenRead(file)) {
                retVal = sha256.ComputeHash(test);
            }

            return Convert.ToBase64String(retVal);
        }
    }
}