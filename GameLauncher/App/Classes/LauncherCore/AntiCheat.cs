using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.SystemPlatform;
using GameLauncher.App.Classes.SystemPlatform.Components;

namespace GameLauncher.App.Classes
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
        public static Thread thread = new Thread(() => { });
        public static int Completed = 0;

        /* INTERNAL */
        public static bool detect_MULTIHACK     = false;
        public static bool detect_FAST_POWERUPS = false;
        public static bool detect_SPEEDHACK     = false;
        public static bool detect_SMOOTH_WALLS  = false;
        public static bool detect_TANK_MODE     = false;
        public static bool detect_WALLHACK      = false;
        public static bool detect_DRIFTMOD      = false;
        public static bool detect_PURSUITBOT    = false;
        public static bool detect_PMASKER       = false;

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
            8972152 /* PROFILEMASKER! */
        };

        public static void EnableChecks()
        {
            if (FunctionStatus.DisableProxy == false)
            {
                Process process = Process.GetProcessById(process_id);
                IntPtr processHandle = Kernel32.OpenProcess(0x0010, false, process.Id);
                int baseAddress = process.MainModule.BaseAddress.ToInt32();

                thread = new Thread(() =>
                {
                    while (true)
                    {
                        foreach (var oneAddress in addresses)
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
                            if (oneAddress == 4486168 && checkInt != "0xF30F1086")
                            {
                                if (checkInt.Substring(0, 4) == "0xE8" && detect_MULTIHACK == false) { detect_MULTIHACK = true; }
                                if (checkInt.Substring(0, 4) == "0xE9" && detect_DRIFTMOD == false) { detect_DRIFTMOD = true; }
                            }

                            /* ProfileMasker */
                            if (oneAddress == 8972152)
                            {
                                byte[] buffer16 = new byte[16];

                                Kernel32.ReadProcessMemory((int)processHandle, (int)(BitConverter.ToUInt32(buffer, 0) + 0x89), buffer16, buffer16.Length, ref bytesRead);
                                String MemoryUsername = Encoding.UTF8.GetString(buffer16, 0, buffer16.Length);

                                Console.WriteLine(MemoryUsername.Substring(0, DiscordGamePresence.PersonaName.Length));
                                Console.WriteLine(DiscordGamePresence.PersonaName);

                                if (MemoryUsername.Substring(0, DiscordGamePresence.PersonaName.Length) != DiscordGamePresence.PersonaName && detect_PMASKER == false)
                                {
                                    detect_PMASKER = true;
                                }
                            }
                        }
                        Thread.Sleep(100);
                    }
                })
                { IsBackground = true };
                thread.Start();
            }
        }

        public static void LocalEnablechecks()
        {
            if (FunctionStatus.DisableProxy == true)
            {
                Process process = Process.GetProcessById(process_id);
                IntPtr processHandle = Kernel32.OpenProcess(0x0010, false, process.Id);
                int baseAddress = process.MainModule.BaseAddress.ToInt32();

                thread = new Thread(() =>
                {
                    while (true)
                    {
                        foreach (var oneAddress in addresses)
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
                            if (oneAddress == 4486168 && checkInt != "0xF30F1086")
                            {
                                if (checkInt.Substring(0, 4) == "0xE8" && detect_MULTIHACK == false) { detect_MULTIHACK = true; }
                                if (checkInt.Substring(0, 4) == "0xE9" && detect_DRIFTMOD == false) { detect_DRIFTMOD = true; }
                            }

                            if (
                            detect_MULTIHACK == true || detect_FAST_POWERUPS == true || detect_SPEEDHACK == true ||
                            detect_SMOOTH_WALLS == true || detect_TANK_MODE == true ||  detect_WALLHACK == true ||
                            detect_DRIFTMOD == true || detect_PURSUITBOT == true || detect_PMASKER == true)
                            {
                                FunctionStatus.ExternalToolsWasUsed = true;
                            }
                        }

                        Thread.Sleep(100);
                    }
                })
                { IsBackground = true };
                thread.Start();
            }
        }

        public static void DisableChecks()
        {
            if (detect_MULTIHACK == true)       cheats_detected |= 1;
            if (detect_FAST_POWERUPS == true)   cheats_detected |= 2;
            if (detect_SPEEDHACK == true)       cheats_detected |= 4;
            if (detect_SMOOTH_WALLS == true)    cheats_detected |= 8;
            if (detect_TANK_MODE == true)       cheats_detected |= 16;
            if (detect_WALLHACK == true)        cheats_detected |= 32;
            if (detect_DRIFTMOD == true)        cheats_detected |= 64;
            if (detect_PURSUITBOT == true)      cheats_detected |= 128;
            if (detect_PMASKER == true)         cheats_detected |= 256;

            if (cheats_detected != 0)
            {
                try
                {
                    if (FunctionStatus.DisableProxy == false)
                    {
                        foreach (string report_url in URLs.AntiCheatFD)
                        {
                            if (report_url.EndsWith("?"))
                            {
                                WebClient update_data = new WebClient();
                                update_data.CancelAsync();
                                update_data.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion);
                                update_data.DownloadStringAsync(new Uri(report_url + "serverip=" + serverip + "&user_id=" + user_id + "&persona_name=" + persona_name + "&event_session=" + event_id + "&cheat_type=" + cheats_detected + "&hwid=" + HardwareID.FingerPrint.Value() + "&persona_id=" + persona_id));
                            }
                            else
                            {
                                Uri sendReport = new Uri(report_url);

                                var request = (HttpWebRequest)WebRequest.Create(sendReport);
                                var postData = "serverip=" + serverip + "&user_id=" + user_id + "&persona_name=" + persona_name + "&event_session=" + event_id + "&cheat_type=" + cheats_detected + "&hwid=" + HardwareID.FingerPrint.Value() + "&persona_id=" + persona_id;

                                var data = Encoding.ASCII.GetBytes(postData);
                                request.Method = "POST";
                                request.ContentType = "application/x-www-form-urlencoded";
                                request.ContentLength = data.Length;

                                using (var stream = request.GetRequestStream())
                                {
                                    stream.Write(data, 0, data.Length);
                                }

                                var response = (HttpWebResponse)request.GetResponse();
                                String responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
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
                                    WebClient update_data = new WebClient();
                                    update_data.CancelAsync();
                                    update_data.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion);
                                    update_data.DownloadStringAsync(new Uri(report_url + "serverip=" + serverip + "&user_id=" + user_id + "&cheat_type=" + cheats_detected + "&hwid=" + HardwareID.FingerPrint.Value()));
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            if (FunctionStatus.DisableProxy == true)
            {
                InformationCache.RestartTimer = 0;
            }

            detect_MULTIHACK = detect_FAST_POWERUPS = detect_SPEEDHACK = detect_SMOOTH_WALLS = detect_TANK_MODE = detect_WALLHACK = detect_DRIFTMOD = detect_PURSUITBOT = detect_PMASKER = false;
            cheats_detected = 0;
            thread.Abort();
        }
    }
}
