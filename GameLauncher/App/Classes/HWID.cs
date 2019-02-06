using System;
using System.Management;
using System.Security.Cryptography;
using System.Security;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GameLauncher.App.Classes;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Security {
    public class FingerPrint {
        private static string fingerPrint = string.Empty;
        public static string Value() {
            if (string.IsNullOrEmpty(fingerPrint)) {
                if (!DetectLinux.UnixDetected())
                {
                    fingerPrint = WindowsValue();
                }
                else if (DetectLinux.LinuxDetected())
                {
                    fingerPrint = LinuxValue();
                }
                else
                {
                    fingerPrint = "hwid-not-impl";
                }
            }

            return fingerPrint;
        }

        private static string WindowsValue() {
            string location = @"SOFTWARE\Microsoft\Cryptography";
            string name = "MachineGuid";
            object machineGuid;

            using (RegistryKey localMachineX64View =
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(
                            string.Format("Key Not Found: {0}", location));

                    machineGuid = rk.GetValue(name);
                    if (machineGuid == null)
                        throw new IndexOutOfRangeException(
                            string.Format("Index Not Found: {0}", name));
                }
            }
            var hmac = new HMACSHA1(Encoding.ASCII.GetBytes("GameLauncher_NFSW"));
            return GetHexString(hmac.ComputeHash(Encoding.ASCII.GetBytes(machineGuid.ToString())));
        }

        private static string LinuxValue() {
            var machineId = File.ReadAllLines("/etc/machine-id")[0];
            var idBytes = Encoding.ASCII.GetBytes(machineId);
            var hmac = new HMACSHA1(Encoding.ASCII.GetBytes("GameLauncher_NFSW"));
            return GetHexString(hmac.ComputeHash(idBytes));
        }

        public static string GetHash(string s) {
            SHA1 sec = new SHA1CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }

        private static string GetHexString(byte[] bt) {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++) {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "";
            }

            return s;
        }
    }
}