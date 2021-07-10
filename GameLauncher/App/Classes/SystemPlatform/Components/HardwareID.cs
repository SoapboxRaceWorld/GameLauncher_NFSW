using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Linux;

namespace GameLauncher.App.Classes.SystemPlatform.Components
{
    class HardwareID
    {
        public class FingerPrint
        {
            private static string License_A = string.Empty;

            private static string License_B = string.Empty;

            private static string License_C = string.Empty;

            private static string License_IA = string.Empty;

            private static string License_IB = string.Empty;

            private static string License_IC = string.Empty;

            public static void Get()
            {
                License_IC = Value();
                License_IC = ValueAlt();
                License_IC = string.Empty;

                /* (Start Process) Sets Up Langauge List */
                LanguageListUpdater.GetList();
            }

            public static string Value()
            {
                if (string.IsNullOrWhiteSpace(License_IB))
                {
                    if (DetectLinux.LinuxDetected())
                    {
                        License_IB = Level_Three_Value();
                    }
                    else if (!DetectLinux.LinuxDetected())
                    {
                        if (string.IsNullOrWhiteSpace(FunctionStatus.RegistryRead("License_IB")))
                        {
                            FunctionStatus.RegistryWrite("License_IB", Level_One_Value());

                            if (string.IsNullOrWhiteSpace(FunctionStatus.RegistryRead("License_IB")))
                            {
                                License_IB = Level_One_Value();
                            }
                            else
                            {
                                License_IB = FunctionStatus.RegistryRead("License_IB");
                            }
                        }
                        else
                        {
                            if (FunctionStatus.RegistryRead("License_IB") == Level_One_Value())
                            {
                                License_IB = Level_One_Value();
                            }
                            else
                            {
                                License_IB = FunctionStatus.RegistryRead("License_IB");
                            }
                        }
                    }
                }

                return License_IB;
            }

            public static string ValueAlt()
            {
                if (string.IsNullOrWhiteSpace(License_IA))
                {
                    if (DetectLinux.LinuxDetected())
                    {
                        License_IA = Level_Three_Value();
                    }
                    else if (!DetectLinux.LinuxDetected())
                    {
                        if (string.IsNullOrWhiteSpace(FunctionStatus.RegistryRead("License_IA")))
                        {
                            FunctionStatus.RegistryWrite("License_IA", Level_Two_Value());

                            if (string.IsNullOrWhiteSpace(FunctionStatus.RegistryRead("License_IA")))
                            {
                                License_IA = Level_Two_Value();
                            }
                            else
                            {
                                License_IA = FunctionStatus.RegistryRead("License_IA");
                            }
                        }
                        else
                        {
                            if (FunctionStatus.RegistryRead("License_IA") == Level_Two_Value())
                            {
                                License_IA = Level_Two_Value();
                            }
                            else
                            {
                                License_IA = FunctionStatus.RegistryRead("License_IA");
                            }
                        }
                    }
                }

                return License_IA;
            }

            public static string Level_One_Value()
            {
                if (string.IsNullOrWhiteSpace(License_B))
                {
                    License_B = GetHash(CpuId() + BaseId() + DiskId() + VideoId());
                }

                return License_B;
            }

            public static string Level_Two_Value()
            {
                if (string.IsNullOrWhiteSpace(License_A))
                {
                    License_A = GetHash(CpuId() + BaseId() + BiosId() + VideoId());
                }

                return License_A;
            }

            public static string Level_Three_Value()
            {
                if (string.IsNullOrWhiteSpace(License_C))
                {
                    var machineId = File.ReadAllLines("/etc/machine-id")[0];
                    var idBytes = Encoding.ASCII.GetBytes(machineId);
                    var hmac = new HMACSHA1(Encoding.ASCII.GetBytes("GameLauncher_NFSW"));

                    License_C = GetHexString(hmac.ComputeHash(idBytes));
                }

                return License_C;
            }

            public static string GetHash(string s)
            {
                SHA1 sec = new SHA1CryptoServiceProvider();
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] bt = enc.GetBytes(s);
                return GetHexString(sec.ComputeHash(bt));
            }

            private static string GetHexString(byte[] bt)
            {
                string s = string.Empty;
                for (int i = 0; i < bt.Length; i++)
                {
                    byte b = bt[i];
                    int n, n1, n2;
                    n = (int)b;
                    n1 = n & 15;
                    n2 = (n >> 4) & 15;
                    if (n2 > 9)
                    {
                        s += ((char)(n2 - 10 + (int)'A')).ToString();
                    }
                    else
                    {
                        s += n2.ToString();
                    }

                    if (n1 > 9)
                    {
                        s += ((char)(n1 - 10 + (int)'A')).ToString();
                    }
                    else
                    {
                        s += n1.ToString();
                    }

                    if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "";
                }

                return s;
            }

            private static string Identifier(string wmiClass, string wmiProperty)
            {
                string result = string.Empty;

                try
                {
                    ManagementClass mc = new ManagementClass(wmiClass);
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        if (string.IsNullOrWhiteSpace(result))
                        {
                            try
                            {
                                var tmp = mo[wmiProperty];
                                if (tmp != null)
                                {
                                    result = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(tmp.ToString()));
                                }
                                break;
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception Error)
                {
                    Log.Error("ID: " + Error.Message);
                    Log.Error("ID [HResult]: " + Error.HResult);
                    Log.ErrorInner("ID [Full Report]: " + Error.ToString());
                }

                return result;
            }

            private static string CpuId()
            {
                string retVal = Identifier("Win32_Processor", "UniqueId");
                if (string.IsNullOrWhiteSpace(retVal))
                {
                    retVal = Identifier("Win32_Processor", "ProcessorId");
                    if (string.IsNullOrWhiteSpace(retVal))
                    {
                        retVal = Identifier("Win32_Processor", "Name");
                        if (string.IsNullOrWhiteSpace(retVal))
                        {
                            retVal = Identifier("Win32_Processor", "Manufacturer");
                        }

                        retVal += Identifier("Win32_Processor", "MaxClockSpeed");
                    }
                }

                return retVal;
            }

            private static string BiosId()
            {
                string retVal = Identifier("Win32_BIOS", "SerialNumber");
                if (string.IsNullOrWhiteSpace(retVal))
                {
                    retVal = Identifier("Win32_BIOS", "Name");
                }

                return retVal + " " + Identifier("Win32_BIOS", "Manufacturer");
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
}
