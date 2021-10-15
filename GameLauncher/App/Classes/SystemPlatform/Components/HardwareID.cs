using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

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
                License_IC = WebHelpers.Value();
                License_IC = string.Empty;

                Log.Info("LIST: Moved to Function");
                /* (Start Process) Sets Up Langauge List */
                LanguageListUpdater.GetList();
            }

            public static string Value()
            {
                if (string.IsNullOrWhiteSpace(License_IB))
                {
                    if (UnixOS.Detected())
                    {
                        License_IB = Level_Three_Value();
                    }
                    else if (!UnixOS.Detected())
                    {
                        if (string.IsNullOrWhiteSpace(RegistryCore.Read("License_IB")))
                        {
                            RegistryCore.Write("License_IB", Level_One_Value());

                            if (string.IsNullOrWhiteSpace(RegistryCore.Read("License_IB")))
                            {
                                License_IB = Level_One_Value();
                            }
                            else
                            {
                                License_IB = RegistryCore.Read("License_IB");
                            }
                        }
                        else
                        {
                            if (RegistryCore.Read("License_IB") == Level_One_Value())
                            {
                                License_IB = Level_One_Value();
                            }
                            else
                            {
                                License_IB = RegistryCore.Read("License_IB");
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
                    if (UnixOS.Detected())
                    {
                        License_IA = Level_Three_Value();
                    }
                    else if (!UnixOS.Detected())
                    {
                        if (string.IsNullOrWhiteSpace(RegistryCore.Read("License_IA")))
                        {
                            RegistryCore.Write("License_IA", Level_Two_Value());

                            if (string.IsNullOrWhiteSpace(RegistryCore.Read("License_IA")))
                            {
                                License_IA = Level_Two_Value();
                            }
                            else
                            {
                                License_IA = RegistryCore.Read("License_IA");
                            }
                        }
                        else
                        {
                            if (RegistryCore.Read("License_IA") == Level_Two_Value())
                            {
                                License_IA = Level_Two_Value();
                            }
                            else
                            {
                                License_IA = RegistryCore.Read("License_IA");
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

            private static string Level_Two_Value()
            {
                if (string.IsNullOrWhiteSpace(License_A))
                {
                    License_A = GetHash(CpuId() + BaseId() + BiosId() + VideoId());
                }

                return License_A;
            }

            private static string Level_Three_Value()
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

            private static string GetHash(string s)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    return string.Empty;
                }
                else
                {
                    SHA1 sec = new SHA1CryptoServiceProvider();
                    ASCIIEncoding enc = new ASCIIEncoding();
                    byte[] bt = enc.GetBytes(s);
                    return GetHexString(sec.ComputeHash(bt));
                }
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
                try
                {
                    ManagementClass mc = new ManagementClass(wmiClass);
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        try
                        {
                            string PropertyValue = mo[wmiProperty]?.ToString();
                            if (!string.IsNullOrEmpty(PropertyValue))
                            {
                                return Strings.Encode(PropertyValue);
                            }
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("ID", null, Error, null, true);
                        }
                    }

                    return string.Empty;
                }
                catch (ManagementException Error)
                {
                    LogToFileAddons.OpenLog("ID", null, Error, null, true);
                    return string.Empty;
                }
                catch (COMException Error)
                {
                    LogToFileAddons.OpenLog("ID", null, Error, null, true);
                    return string.Empty;
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("ID", null, Error, null, true);
                    return string.Empty;
                }
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
                return Identifier("Win32_BIOS", "IdentificationCode") + " " +
                   Identifier("Win32_BIOS", "Manufacturer") + " " +
                   Identifier("Win32_BIOS", "Name") + " " +
                   Identifier("Win32_BIOS", "ReleaseDate") + " " +
                   Identifier("Win32_BIOS", "SMBIOSBIOSVersion") + " " +
                   Identifier("Win32_BIOS", "SerialNumber") + " " +
                   Identifier("Win32_BIOS", "Version");
            }

            private static string DiskId()
            {
                return Identifier("Win32_DiskDrive", "Manufacturer") + " " + Identifier("Win32_DiskDrive", "Model") + " " +
                    Identifier("Win32_DiskDrive", "Name") + " " + Identifier("Win32_DiskDrive", "PNPDeviceId") + " " +
                    Identifier("Win32_DiskDrive", "SerialNumber") + " " + Identifier("Win32_DiskDrive", "Signature") + " " +
                    Identifier("Win32_DiskDrive", "TotalHeads");
            }

            private static string BaseId()
            {
                return Identifier("Win32_BaseBoard", "Manufacturer") + " " + Identifier("Win32_BaseBoard", "Model") + " " +
                    Identifier("Win32_BaseBoard", "Name") + " " + Identifier("Win32_BaseBoard", "Product") + " " +
                    Identifier("Win32_BaseBoard", "SerialNumber");
            }

            private static string VideoId()
            {
                return Identifier("Win32_VideoController", "PNPDeviceId") + " " + Identifier("Win32_VideoController", "Name");
            }

            /* Moved 1 Private function Code to Gist */
            /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/dfefd74204f2675cc34a4614fb09a74a23ed6e0e/HWID.cs */

        }
    }
}
