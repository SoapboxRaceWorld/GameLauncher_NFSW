using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.SystemPlatform;
using GameLauncher.App.Classes.SystemPlatform.Components;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Client
{
    class AntiCheat
    {
        public static int process_id = 0;
        public static string serverip = String.Empty;
        public static string user_id = String.Empty;
        public static string persona_name = String.Empty;
        public static string persona_id = String.Empty;
        public static int event_id = 0;
        public static int cheats_detected = 0;
        public static Thread Secret = new Thread(() => { });
        public static int Completed = 0;
        public static int IAmSpeed = 5000;
        public static int SpeedTicket = 0;

        /* INTERNAL */
        public static bool detect_MULTIHACK = false;
        public static bool detect_FAST_POWERUPS = false;
        public static bool detect_SPEEDHACK = false;
        public static bool detect_SMOOTH_WALLS = false;
        public static bool detect_TANK_MODE = false;
        public static bool detect_WALLHACK = false;
        public static bool detect_DRIFTMOD = false;
        public static bool detect_PURSUITBOT = false;
        public static bool detect_GHOSTING = false;

        public static List<int> addresses = new List<int>
        {
            418534,  /* GMZ_MULTIHACK */
            3788216, /* FAST_POWERUPS */
            4552702, /* SPEEDHACK */
            4476396, /* SMOOTH_WALLS */
            4506534, /* TANK */
            4587060, /* WALLHACK */
            4486168, /* DRIFTMOD/MULTIHACK */
            4820249, /* PURSUITBOT (NO COPS VARIATION) */
            4573882  /* GHOSTING */
        };

        public static void EnableChecks()
        {
            Process process = Process.GetProcessById(process_id);
            IntPtr processHandle = Kernel32.OpenProcess(0x0010, false, process.Id);
            int baseAddress = process.MainModule.BaseAddress.ToInt32();

            Secret = new Thread(() =>
            {
                while (true)
                {
                    foreach (int oneAddress in addresses)
                    {
                        int bytesRead = 0;
                        byte[] buffer = new byte[4];
                        Kernel32.ReadProcessMemory((int)processHandle, baseAddress + oneAddress, buffer, buffer.Length, ref bytesRead);

                        String checkInt = "0x" + BitConverter.ToString(buffer).Replace("-", String.Empty);

                        if (oneAddress == 418534 && checkInt != "0x3B010F84" && detect_MULTIHACK == false) { detect_MULTIHACK = true; }
                        if (oneAddress == 3788216 && checkInt != "0x807DFB00" && detect_FAST_POWERUPS == false) { detect_FAST_POWERUPS = true; }
                        if (oneAddress == 4552702 && checkInt != "0x76390F2E" && detect_SPEEDHACK == false) { detect_SPEEDHACK = true; }
                        if (oneAddress == 4476396 && checkInt != "0x84C00F84" && detect_SMOOTH_WALLS == false) { detect_SMOOTH_WALLS = true; }
                        if (oneAddress == 4506534 && checkInt != "0x74170F57" && detect_TANK_MODE == false) { detect_TANK_MODE = true; }
                        if (oneAddress == 4587060 && checkInt != "0x74228B16" && detect_WALLHACK == false) { detect_WALLHACK = true; }
                        if (oneAddress == 4820249 && checkInt != "0x0F845403" && detect_PURSUITBOT == false) { detect_PURSUITBOT = true; }
                        if (oneAddress == 4573882 && checkInt != "0x85FF0F84" && detect_GHOSTING == false) { detect_GHOSTING = true; }

                        if (oneAddress == 4486168 && checkInt != "0xF30F1086")
                        {
                            if (checkInt.Substring(0, 4) == "0xE8" && detect_MULTIHACK == false) { detect_MULTIHACK = true; }
                            if (checkInt.Substring(0, 4) == "0xE9" && detect_DRIFTMOD == false) { detect_DRIFTMOD = true; }
                        }

                        if (!ServerProxy.Running())
                        {
                            if (
                            detect_MULTIHACK == true || detect_FAST_POWERUPS == true || detect_SPEEDHACK == true ||
                            detect_SMOOTH_WALLS == true || detect_TANK_MODE == true || detect_WALLHACK == true ||
                            detect_DRIFTMOD == true || detect_PURSUITBOT == true || detect_GHOSTING == true)
                            {
                                FunctionStatus.ExternalToolsWasUsed = true;
                            }
                        }
                    }
                    Thread.Sleep(IAmSpeed);
                }
            })
            { IsBackground = true };
            Secret.Start();
        }

        public static int Get_Cheat_Status()
        {
            int _cheats_detected = 0;
            if (detect_MULTIHACK) _cheats_detected |= 1;
            if (detect_FAST_POWERUPS) _cheats_detected |= 2;
            if (detect_SPEEDHACK) _cheats_detected |= 4;
            if (detect_SMOOTH_WALLS) _cheats_detected |= 8;
            if (detect_TANK_MODE) _cheats_detected |= 16;
            if (detect_WALLHACK) _cheats_detected |= 32;
            if (detect_DRIFTMOD) _cheats_detected |= 64;
            if (detect_PURSUITBOT) _cheats_detected |= 128;
            if (detect_GHOSTING) _cheats_detected |= 512;
            return _cheats_detected;
        }

        public static void DisableChecks(bool CompletedEvent)
        {
            cheats_detected = Get_Cheat_Status();

            if (cheats_detected != 0)
            {
                if (cheats_detected == 64 && !CompletedEvent)
                {
                    /* You Know the Rules and So Do I */
                }
                else
                {
                    if (ServerProxy.Running())
                    {
                        foreach (string report_url in URLs.AntiCheatFD)
                        {
                            if (Completed == 0)
                            {
                                Completed++;
                                FunctionStatus.ExternalToolsWasUsed = true;
                            }

                            if (report_url.EndsWith("?"))
                            {
                                try
                                {
                                    Uri sendReport = new Uri(report_url + "serverip=" + serverip + "&user_id=" + user_id + "&persona_name=" + persona_name + "&event_session=" + event_id + "&cheat_type=" + cheats_detected + "&hwid=" + HardwareID.FingerPrint.Value() + "&persona_id=" + persona_id + "&launcher_hash=" + WebHelpers.Value() + "&launcher_certificate=" + CertificateStore.LauncherSerial + "&hwid_fallback=" + HardwareID.FingerPrint.ValueAlt() + "&car_used=" + DiscordGamePresence.PersonaCarName + "&os_platform=" + InformationCache.OSName + "&event_status=" + CompletedEvent);
                                    ServicePointManager.FindServicePoint(sendReport).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

                                    var Client = new WebClient
                                    {
                                        Encoding = Encoding.UTF8
                                    };

                                    if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                                    else
                                    {
                                        Client.Headers.Add("user-agent", "SBRW Launcher " + Application.ProductVersion + " - (" + InsiderInfo.BuildNumberOnly() + ")");
                                    }
                                    Client.DownloadStringCompleted += (Nice, Brock) => { Client.Dispose(); };

                                    try
                                    {
                                        string NTVersion = WindowsProductVersion.GetWindowsBuildNumber() != 0 ? WindowsProductVersion.GetWindowsBuildNumber().ToString() : "Wine";
                                        Client.Headers.Add("os-version", NTVersion);
                                        Client.DownloadStringAsync(sendReport);
                                    }
                                    catch { }
                                }
                                catch { }
                            }
                            else
                            {
                                try
                                {
                                    Uri sendReport = new Uri(report_url);
                                    ServicePointManager.FindServicePoint(sendReport).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

                                    var request = (HttpWebRequest)WebRequest.Create(sendReport);
                                    var postData = "serverip=" + serverip + "&user_id=" + user_id + "&persona_name=" + persona_name + "&event_session=" + event_id + "&cheat_type=" + cheats_detected + "&hwid=" + HardwareID.FingerPrint.Value() + "&persona_id=" + persona_id + "&launcher_hash=" + WebHelpers.Value() + "&launcher_certificate=" + CertificateStore.LauncherSerial + "&hwid_fallback=" + HardwareID.FingerPrint.ValueAlt() + "&car_used=" + DiscordGamePresence.PersonaCarName + "&os_platform=" + InformationCache.OSName + "&event_status=" + CompletedEvent;

                                    var data = Encoding.ASCII.GetBytes(postData);
                                    request.Method = "POST";
                                    request.ContentType = "application/x-www-form-urlencoded";
                                    request.ContentLength = data.Length;
                                    request.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

                                    using (var stream = request.GetRequestStream())
                                    {
                                        stream.Write(data, 0, data.Length);
                                    }

                                    var response = (HttpWebResponse)request.GetResponse();
                                    String responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                                }
                                catch { }
                            }
                        }
                    }
                    else
                    {
                        if (Completed != URLs.AntiCheatSD.Length)
                        {
                            foreach (string report_url in URLs.AntiCheatSD)
                            {
                                Completed++;
                                if (report_url.EndsWith("?"))
                                {
                                    try
                                    {
                                        Uri sendReport = new Uri(report_url + "serverip=" + serverip + "&user_id=" + user_id + "&cheat_type=" + cheats_detected + "&hwid=" + HardwareID.FingerPrint.Value() + "&launcher_hash=" + WebHelpers.Value() + "&launcher_certificate=" + CertificateStore.LauncherSerial + "&hwid_fallback=" + HardwareID.FingerPrint.ValueAlt() + "&os_platform=" + InformationCache.OSName);
                                        ServicePointManager.FindServicePoint(sendReport).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

                                        var Client = new WebClient
                                        {
                                            Encoding = Encoding.UTF8
                                        };

                                        if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                                        else
                                        {
                                            Client.Headers.Add("user-agent", "SBRW Launcher " + Application.ProductVersion + " - (" + InsiderInfo.BuildNumberOnly() + ")");
                                        }
                                        Client.DownloadStringCompleted += (Nice, Brock) => { Client.Dispose(); };

                                        try
                                        {
                                            string NTVersion = WindowsProductVersion.GetWindowsBuildNumber() != 0 ? WindowsProductVersion.GetWindowsBuildNumber().ToString() : "Wine";
                                            Client.Headers.Add("os-version", NTVersion);
                                            Client.DownloadStringAsync(sendReport);
                                        }
                                        catch { }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }

                    TimeConversions.MUFRTime();
                }
            }

            detect_MULTIHACK = detect_FAST_POWERUPS = detect_SPEEDHACK = detect_SMOOTH_WALLS = detect_TANK_MODE = detect_WALLHACK = detect_DRIFTMOD = detect_PURSUITBOT = false;
            cheats_detected = 0;
            Secret.Abort();
        }
    }
}