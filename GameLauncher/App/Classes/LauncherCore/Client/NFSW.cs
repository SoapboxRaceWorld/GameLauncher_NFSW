using GameLauncher.App.Classes.Auth;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Support;
using System.Diagnostics;
using System.Threading;

namespace GameLauncher.App.Classes.LauncherCore.Client
{
    class NFSW
    {
        public static bool DetectByMutex()
        {
            Mutex detectRunningNFSW = null;
            try
            {
                detectRunningNFSW = new Mutex(false, "Global\\{3E34CEFB-7B34-4e62-8034-33256B8BC2F7}");

                if (!detectRunningNFSW.WaitOne(0, false))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                if (detectRunningNFSW != null)
                {
                    detectRunningNFSW.Close();
                    detectRunningNFSW.Dispose();
                }
            }
        }

        public static bool DetectGameProcess()
        {
            Process[] Game = Process.GetProcessesByName("nfsw");

            if (Game.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool DetectGameLauncherSimplified()
        {
            Process[] Launcher = Process.GetProcessesByName("GameLauncherSimplified");

            if (Launcher.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsRunning()
        {
            return DetectByMutex() || DetectGameProcess() || DetectGameLauncherSimplified();
        }

        public static string ErrorTranslation(int Exit_Code)
        {
            switch (Exit_Code)
            {
                case -1073741819:
                    return "Game Crash: Access Violation (0x" + Exit_Code.ToString("X") + ")";
                case -1073740940:
                    return "Game Crash: Heap Corruption (0x" + Exit_Code.ToString("X") + ")";
                case -1073740791:
                    return "Game Crash: Stack buffer overflow (0x" + Exit_Code.ToString("X") + ")";
                case -805306369:
                    return "Game Crash: Application Hang (0x" + Exit_Code.ToString("X") + ")";
                case -1073741515:
                    return "Game Crash: Missing dependency files (0x" + Exit_Code.ToString("X") + ")";
                case -1073740972:
                    return "Game Crash: Debugger crash (0x" + Exit_Code.ToString("X") + ")";
                case -1073741676:
                    return "Game Crash: Division by Zero (0x" + Exit_Code.ToString("X") + ")";
                case 1:
                    return "The process nfsw.exe was killed via Task Manager";
                case 69:
                    return "AllocationAssistant encountered an 'Out of Memory' condition";
                case 2137:
                    return "Launcher Forced Closed your Game. \nYou are Required to Restart the Game After " +
                        TimeConversions.RelativeTime((InformationCache.SelectedServerJSON.secondsToShutDown != 0) ? InformationCache.SelectedServerJSON.secondsToShutDown : 7200);
                case 2017:
                    return "Server replied with Code: " + Tokens.UserId + " (0x" + Exit_Code.ToString("X") + ")";
                case -1:
                    return "No DirectX resources. Please check if GPU has enough VRAM before playing";
                case -3:
                    return "The Server was unable to resolve the request";
                case -4:
                    return "Another instance is already executed";
                case -5:
                    return "DirectX Device was not found. Please install GPU Drivers before playing";
                case -6:
                    return "Server was unable to resolve your request";
                case -7:
                    /* Known Affected Programs: MSI Dragon Center, Windows 10 1909 (Fix: Update to Latest)*/
                    return "Corrupted Memory. Please check for interrupting Programs or System Updates.";
                /* ModLoader */
                case 1450:
                    return "ModNet: Unable to load ModLoader. Please Exclude Game Files with your Antivirus Software";
                case 193:
                    return "ModNet: Unable to load ModLoader. Please Install Microsoft Visual C++ Runtimes 2015-2019";
                case 2:
                    return "ModNet: Game was launched with invalid command line parameters.";
                case 3:
                    return "ModNet: .links file should not exist upon startup!";
                case 4:
                    return "ModNet: An Unhandled Error Appeared";
                /* Generic Error */
                default:
                    return "Game Crash with exitcode: " + Exit_Code.ToString() + " (0x" + Exit_Code.ToString("X") + ")";
            }
        }
    }
}