using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GameLauncher.App.Classes
{
    class AntiCheat
    {
        public static int process_id = 0;

        public static string serverip = String.Empty; //
        public static string user_id = String.Empty; //
        public static string persona_name = String.Empty; //
        public static int event_id = 0; //
        public static int cheats_detected = 0;


        //INTERNAL//
        /* 0x3B01 */ public static bool detect_418534  = false;
        /* 0x807D */ public static bool detect_3788216 = false;
        /* 0x7639 */ public static bool detect_4552702 = false;
        /* 0x84C0 */ public static bool detect_4476396 = false;
        /* 0x7417 */ public static bool detect_4506534 = false;

        public static void enableChecks() {
            Process process = Process.GetProcessById(process_id);
            IntPtr processHandle = Kernel32.OpenProcess(0x0010, false, process.Id);
            int baseAddress = process.MainModule.BaseAddress.ToInt32();

            var thread = new Thread(() => {
                List<int> addresses = new List<int>();
                addresses.Add(418534);
                addresses.Add(3788216); //FAST_POWERUPS
                addresses.Add(4552702); //SPEEDHACK
                addresses.Add(4476396); //SMOOTH_WALLS
                addresses.Add(4506534);

                while (true) { 
                    foreach (var oneAddress in addresses) {
                        int bytesRead = 0;
                        byte[] buffer = new byte[4];
                        Kernel32.ReadProcessMemory((int)processHandle, baseAddress + oneAddress, buffer, buffer.Length, ref bytesRead);

                        String checkInt = "0x"+BitConverter.ToString(buffer).Replace("-", String.Empty);

                        if (oneAddress == 418534  && checkInt != "0x3B010F84" && detect_418534  == false) detect_418534  = true;
                        if (oneAddress == 3788216 && checkInt != "0x807DFB00" && detect_3788216 == false) detect_3788216 = true;
                        if (oneAddress == 4552702 && checkInt != "0x76390F2E" && detect_4552702 == false) detect_4552702 = true;
                        if (oneAddress == 4476396 && checkInt != "0x84C00F84" && detect_4476396 == false) detect_4476396 = true;
                        if (oneAddress == 4506534 && checkInt != "0x74170F57" && detect_4506534 == false) detect_4506534 = true;
                    }
                    Thread.Sleep(500);
                }
            }) { IsBackground = true };
            thread.Start();
        }

        public static void disableChecks() {
            if (detect_418534 == true)  AntiCheat.cheats_detected |= 1;
            if (detect_3788216 == true) AntiCheat.cheats_detected |= 2;
            if (detect_4552702 == true) AntiCheat.cheats_detected |= 4;
            if (detect_4506534 == true) AntiCheat.cheats_detected |= 8;
            if (detect_4476396 == true) AntiCheat.cheats_detected |= 16;

            if (AntiCheat.cheats_detected != 0) {
                //Not nice. Send to global registry of cheaters.

                String responseString;
                try {
                    Uri sendReport = new Uri("http://launcher.worldunited.gg/report");

                    var request = (HttpWebRequest)WebRequest.Create(sendReport);
                    var postData = "serverip=" + AntiCheat.serverip + "&user_id=" + AntiCheat.user_id + "&persona_name=" + AntiCheat.persona_name + "&event_session=" + AntiCheat.event_id + "&cheat_type=" + AntiCheat.cheats_detected;
                    var data = Encoding.ASCII.GetBytes(postData);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream()) {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    Console.WriteLine("Detected: " + AntiCheat.cheats_detected);
                } catch { }
            }

            detect_3788216 = detect_418534 = detect_4476396 = detect_4506534 = detect_4552702 = false;
            AntiCheat.cheats_detected = 0;
        }
    }
}
