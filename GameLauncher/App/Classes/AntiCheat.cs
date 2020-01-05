using GameLauncher.App.Classes.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;

namespace GameLauncher.App.Classes
{
    class HashChecker
    {
        public static string HAName = "SHA1";
        //private static byte[] HashDirectory(DirectoryInfo directoryInfo)
        //{
        //    using (var hashAlgorithm = HashAlgorithm.Create(HAName))
        //    {
        //        using (var cryptoStream = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write))
        //        using (var binaryWriter = new BinaryWriter(cryptoStream))
        //        {
        //            FileSystemInfo[] infos = directoryInfo.GetFileSystemInfos();
        //            Array.Sort(infos, (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
        //            foreach (FileSystemInfo info in infos)
        //            {
        //                if ((info.Attributes & FileAttributes.Directory) == 0 && (info.Name.ToLower().Contains("dll") || info.Name.ToLower().Contains("exe")))
        //                {
        //                    binaryWriter.Write(info.Name);
        //                    binaryWriter.Write((byte)'F');
        //                    binaryWriter.Write(HashFile((FileInfo)info));
        //                }
        //            }
        //        }
        //        return hashAlgorithm.Hash;
        //    }
        //}

        private static byte[] HashFile(FileInfo fileInfo)
        {
            using (var hashAlgorithm = HashAlgorithm.Create(HAName))
            using (var inputStream = fileInfo.OpenRead())
            {
                return hashAlgorithm.ComputeHash(inputStream);
            }
        }

        private static byte[] GetGameHash(string gamePath) => HashFile(new FileInfo(gamePath));
        private static byte[] GetLauncherHash(string launcherPath) => HashFile(new FileInfo(launcherPath));
        //private static byte[] GetLauncherFolderHash(string launcherFolderPath) => HashDirectory(new DirectoryInfo(launcherFolderPath));

        private static bool CheckHash(string target, byte[] localHash)
        {
            string remoteHash;
            using (var wc = new WebClient()) remoteHash = wc.DownloadString($"https://damp-forest-50246.herokuapp.com/sha_knower?target={target}");
            return (target.ToLower().Contains("folder") ? BitConverter.ToString(localHash).Replace(" - ", "") : Convert.ToBase64String(localHash)) == remoteHash.TrimEnd(Environment.NewLine.ToCharArray());
        }

        public static void CheckGame(string gamePath)
        {
            if (!CheckHash("game", GetGameHash(gamePath)))
            {
                throw new Exception("Game complexity exception");
            }
        }

        public static void CheckLauncher(string launcherPath)
        {
            if (!CheckHash("launcher", GetLauncherHash(launcherPath)))
            {
                throw new Exception("Launcher complexity exception");
            }
        }

        //public static void CheckLauncherFolder(string launcherFolderPath)
        //{
        //    if (!CheckHash("sbrw_folder", GetLauncherFolderHash(launcherFolderPath)))
        //    {
        //        throw new Exception("SBRW complexity exception");
        //    }
        //}
    }
    class AntiCheat
    {
        public static int process_id = 0;

        public static string serverip = String.Empty; //
        public static string user_id = String.Empty; //
        public static string persona_name = String.Empty; //
        public static int event_id = 0; //
        public static int cheats_detected = 0;


        //INTERNAL//
        public static bool detect_MULTIHACK     = false;
        public static bool detect_FAST_POWERUPS = false;
        public static bool detect_SPEEDHACK     = false;
        public static bool detect_SMOOTH_WALLS  = false;
        public static bool detect_TANK_MODE     = false;
        public static bool detect_WALLHACK      = false;
        public static bool detect_DRIFTMOD      = false;

        public static void enableChecks() {
            Process process = Process.GetProcessById(process_id);
            IntPtr processHandle = Kernel32.OpenProcess(0x0010, false, process.Id);
            int baseAddress = process.MainModule.BaseAddress.ToInt32();

            var thread = new Thread(() => {
                List<int> addresses = new List<int>();
                addresses.Add(418534);  // GMZ_MULTIHACK
                addresses.Add(3788216); // FAST_POWERUPS
                addresses.Add(4552702); // SPEEDHACK
                addresses.Add(4476396); // SMOOTH_WALLS
                addresses.Add(4506534); // TANK
                addresses.Add(4587060); // WALLHACK
                addresses.Add(4486168); // DRIFTMOD/MULTIHACK

                while (true)
                {
                    foreach (var oneAddress in addresses)
                    {
                        int bytesRead = 0;
                        byte[] buffer = new byte[4];
                        Kernel32.ReadProcessMemory((int)processHandle, baseAddress + oneAddress, buffer, buffer.Length, ref bytesRead);

                        String checkInt = "0x" + BitConverter.ToString(buffer).Replace("-", String.Empty);

                        if (oneAddress == 418534 && checkInt != "0x3B010F84" && detect_MULTIHACK == false) detect_MULTIHACK = true;
                        if (oneAddress == 3788216 && checkInt != "0x807DFB00" && detect_FAST_POWERUPS == false) detect_FAST_POWERUPS = true;
                        if (oneAddress == 4552702 && checkInt != "0x76390F2E" && detect_SPEEDHACK == false) detect_SPEEDHACK = true;
                        if (oneAddress == 4476396 && checkInt != "0x84C00F84" && detect_SMOOTH_WALLS == false) detect_SMOOTH_WALLS = true;
                        if (oneAddress == 4506534 && checkInt != "0x74170F57" && detect_TANK_MODE == false) detect_TANK_MODE = true;
                        if (oneAddress == 4587060 && checkInt != "0x74228B16" && detect_WALLHACK == false) detect_WALLHACK = true;
                        if (oneAddress == 4486168 && checkInt != "0xF30F1086")
                        {
                            if (checkInt.Substring(0, 4) == "0xE8" && detect_MULTIHACK == false) detect_MULTIHACK = true;
                            if (checkInt.Substring(0, 4) == "0xE9" && detect_DRIFTMOD == false) detect_DRIFTMOD = true;
                        }
                    }
                    Thread.Sleep(500);
                }
            }) { IsBackground = true };
            thread.Start();
        }

        public static void disableChecks() {
            if (detect_MULTIHACK == true)       AntiCheat.cheats_detected |= 1;
            if (detect_FAST_POWERUPS == true)   AntiCheat.cheats_detected |= 2;
            if (detect_SPEEDHACK == true)       AntiCheat.cheats_detected |= 4;
            if (detect_SMOOTH_WALLS == true)    AntiCheat.cheats_detected |= 8;
            if (detect_TANK_MODE == true)       AntiCheat.cheats_detected |= 16;
            if (detect_WALLHACK == true)        AntiCheat.cheats_detected |= 32;
            if (detect_DRIFTMOD == true)        AntiCheat.cheats_detected |= 64;

            if (AntiCheat.cheats_detected != 0) {
                //Not nice. Send to global registry of cheaters.

                String responseString;
                try {
                    Uri sendReport = new Uri("http://launcher.worldunited.gg/report");

                    var request = (HttpWebRequest)WebRequest.Create(sendReport);
                    var postData = "serverip=" + AntiCheat.serverip + "&user_id=" + AntiCheat.user_id + "&persona_name=" + AntiCheat.persona_name + "&event_session=" + AntiCheat.event_id + "&cheat_type=" + AntiCheat.cheats_detected + "&hwid=" + Security.FingerPrint.Value();
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

            detect_MULTIHACK = detect_FAST_POWERUPS = detect_SPEEDHACK = detect_SMOOTH_WALLS = detect_TANK_MODE = detect_WALLHACK = detect_DRIFTMOD = false;
            AntiCheat.cheats_detected = 0;
        }
    }
}
