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
    }
}