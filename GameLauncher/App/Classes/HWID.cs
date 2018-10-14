using System;
using System.Management;
using System.Security.Cryptography;
using System.Security;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace Security {
    public class FingerPrint {
        private static string fingerPrint = string.Empty;
        public static string Value() {
            if (string.IsNullOrEmpty(fingerPrint)) {
                fingerPrint = GetHash(cpuId() + baseId() + diskId() + videoId());
            }

            return fingerPrint;
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

        private static string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue) {
            string result = "";
            ManagementClass mc = new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {
                if (mo[wmiMustBeTrue].ToString() == "True") {
                    if (result == "") {
                        try {
                            result = mo[wmiProperty].ToString();
                            break;
                        } catch { }
                    }
                }
            }

            return result;
        }

        private static string identifier(string wmiClass, string wmiProperty) {
            string result = "";
            ManagementClass mc = new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {
                if (result == "") {
                    try {
                        result = mo[wmiProperty].ToString();
                        break;
                    } catch { }
                }
            }

            return result;
        }

        private static string cpuId() {
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "") {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "") {
                    retVal = identifier("Win32_Processor", "Name");

                    if (retVal == "") {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }

                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }

            return retVal;
        }

        private static string diskId() {
            return identifier("Win32_DiskDrive", "PNPDeviceId");
        }

        private static string baseId() {
            return identifier("Win32_BaseBoard", "Product") + " " + identifier("Win32_BaseBoard", "SerialNumber");
        }

        private static string videoId() {
            return identifier("Win32_VideoController", "PNPDeviceId");
        }

        private static string macId() {
            return identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }
    }
}