using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using GameLauncher.App.Classes;

namespace Security
{
    public class FingerPrint {
        private static string fingerPrint = string.Empty;
        public static string Value() {
            if (string.IsNullOrEmpty(fingerPrint)) {
                if (!DetectLinux.LinuxDetected()) {
                    fingerPrint = WindowsValue();
                } else if (DetectLinux.LinuxDetected()) {
                    fingerPrint = LinuxValue();
                } 
            }

            return fingerPrint;
        }

        private static string WindowsValue()
        {
            return GetHash(CpuId() + BaseId() + DiskId() + VideoId());
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

        private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            ManagementClass mc = new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    if (result == "")
                    {
                        try
                        {
                            var tmp = mo[wmiProperty];
                            if (tmp != null) {
                                result = tmp.ToString();
                            } else {
                                continue;
                            }
                            break;
                        }
                        catch { }
                    }
                }
            }

            return result;
        }

        private static string Identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            ManagementClass mc = new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (result == "")
                {
                    try {
                        var tmp = mo[wmiProperty];
                        if(tmp != null) {
                            result = tmp.ToString();
                        }
                        break;
                    }
                    catch { }
                }
            }

            return result;
        }

        private static string CpuId()
        {
            string retVal = Identifier("Win32_Processor", "UniqueId");
            if (retVal == "")
            {
                retVal = Identifier("Win32_Processor", "ProcessorId");
                if (retVal == "")
                {
                    retVal = Identifier("Win32_Processor", "Name");

                    if (retVal == "")
                    {
                        retVal = Identifier("Win32_Processor", "Manufacturer");
                    }

                    retVal += Identifier("Win32_Processor", "MaxClockSpeed");
                }
            }

            return retVal;
        }

        private static string DiskId()
        {
            return Identifier("Win32_DiskDrive", "PNPDeviceId");
        }

        private static string BaseId()
        {
            return Identifier("Win32_BaseBoard", "Product") + " " + Identifier("Win32_BaseBoard", "SerialNumber");
        }

        private static string VideoId()
        {
            return Identifier("Win32_VideoController", "PNPDeviceId");
        }

        /* Moved 1 Private function Code to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/dfefd74204f2675cc34a4614fb09a74a23ed6e0e/HWID.cs */

    }
}