using SBRW.Launcher.RunTime.Auth;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Time_;
using System.Diagnostics;
using System.Threading;

namespace SBRW.Launcher.RunTime.LauncherCore.Client
{
    class NFSW
    {
        public static bool DetectByMutex()
        {
            Mutex? detectRunningNFSW = null;
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
            return Exit_Code switch
            {
                -1073741819 => "Game Crash: Access Violation (0x" + Exit_Code.ToString("X") + ")",
                -1073740940 => "Game Crash: Heap Corruption (0x" + Exit_Code.ToString("X") + ")",
                -1073740791 => "Game Crash: Stack buffer overflow (0x" + Exit_Code.ToString("X") + ")",
                -805306369 => "Game Crash: Application Hang (0x" + Exit_Code.ToString("X") + ")",
                -1073741515 => "Game Crash: Missing dependency files (0x" + Exit_Code.ToString("X") + ")",
                -1073740972 => "Game Crash: Debugger crash (0x" + Exit_Code.ToString("X") + ")",
                -1073741676 => "Game Crash: Division by Zero (0x" + Exit_Code.ToString("X") + ")",
                -1073741674 => "Game Crash: Cannot access file(s) (0x" + Exit_Code.ToString("X") + ")",
                1 => "The process nfsw.exe was killed via Task Manager",
                69 => "AllocationAssistant encountered an 'Out of Memory' condition",
                998 => "Invalid access to memory location.",
                2137 => "Launcher Forced Closed your Game. \nYou are Required to Restart the Game After " +
Time_Conversion.RelativeTime((Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer != 0) ? Launcher_Value.Launcher_Select_Server_JSON.Server_Session_Timer : 7200),
                2017 => "Server replied with Code: " + Tokens.UserId + " (0x" + Exit_Code.ToString("X") + ")",
                2020 => "Game Failed to Start. Launcher closed any leftover instances of the game.",
                -1 => "No DirectX resources. Please check if GPU has enough VRAM before playing",
                -2 => "Not enough contiguous memory. Please reboot game or system.",
                -3 => "The Server was unable to resolve the request",
                -4 => "Another instance is already executed",
                -5 => "DirectX Device was not found. Please install GPU Drivers before playing",
                -6 => "Server was unable to resolve your request",
                -7 => "Corrupted Memory. Please check for interrupting Programs or System Updates.",/* Known Affected Programs: MSI Dragon Center, Windows 10 1909 (Fix: Update to Latest)*/
                /* ModLoader */
                1450 => "ModNet: Unable to load ModLoader. Please Exclude Game Files with your Antivirus Software",
                193 => "ModNet: Unable to load ModLoader. Please Install Microsoft Visual C++ Runtimes 2015-2019",
                2 => "ModNet: Game was launched with invalid command line parameters.",
                3 => "ModNet: .links file should not exist upon startup!",
                4 => "ModNet: An Unhandled Error Appeared",
                /* Generic Error */
                _ => "Game Crash with exitcode: " + Exit_Code.ToString() + " (0x" + Exit_Code.ToString("X") + ")",
            };
        }
    }
}