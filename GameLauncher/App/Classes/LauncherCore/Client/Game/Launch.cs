using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.RPC;
using GameLauncherReborn;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Client.Game
{
    class Launch
    {
        public static NotifyIcon Notification;
        public static Label PlayProgressText;
        public static ProgressBarEx PlayProgress;
        public static Form Screen;

        private static int _nfswPid;
        private static bool _gameKilledBySpeedBugCheck = false;

        private static int TimerDebug = 40;

        public Launch(NotifyIcon StatusNotification, Label StatusPlayProgressText, ProgressBarEx StatusPlayProgress)
        {
            Notification = StatusNotification;
            PlayProgressText = StatusPlayProgressText;
            PlayProgress = StatusPlayProgress;
        }

        public static void Game(string ServerName, string ServerID, string ServerIP, string UserID, string LoginToken, int ForceRestart, Form x)
        {
            var oldfilename = FileSettingsSave.GameInstallation + "\\nfsw.exe";

            var args = ServerID + " " + ServerIP + " " + LoginToken + " " + UserID;
            var psi = new ProcessStartInfo();

            if (DetectLinux.LinuxDetected())
            {
                psi.UseShellExecute = false;
            }

            psi.WorkingDirectory = FileSettingsSave.GameInstallation;
            psi.FileName = oldfilename;
            psi.Arguments = args;

            var nfswProcess = Process.Start(psi);
            nfswProcess.PriorityClass = ProcessPriorityClass.AboveNormal;

            var processorAffinity = 0;
            for (var i = 0; i < Math.Min(Math.Max(1, Environment.ProcessorCount), 8); i++)
            {
                processorAffinity |= 1 << i;
            }

            nfswProcess.ProcessorAffinity = (IntPtr)processorAffinity;

            AntiCheat.process_id = nfswProcess.Id;

            //TIMER HERE
            int secondsToShutDown = (TimerDebug != 0) ? TimerDebug : 2 * 60 * 60;
            System.Timers.Timer shutdowntimer = new System.Timers.Timer();
            shutdowntimer.Elapsed += (x2, y2) =>
            {
                Process[] allOfThem = Process.GetProcessesByName("nfsw");

                if (secondsToShutDown <= 0)
                {
                    if (FunctionStatus.CanCloseGame == true)
                    {
                        foreach (var oneProcess in allOfThem)
                        {
                            _gameKilledBySpeedBugCheck = true;
                            Process.GetProcessById(oneProcess.Id).Kill();
                        }
                    }
                    else
                    {
                        secondsToShutDown = 0;
                    }
                }

                //change title

                foreach (var oneProcess in allOfThem)
                {
                    long p = oneProcess.MainWindowHandle.ToInt64();
                    TimeSpan t = TimeSpan.FromSeconds(secondsToShutDown);
                    string secondsToShutDownNamed = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);

                    if (secondsToShutDown == 0)
                    {
                        secondsToShutDownNamed = "Waiting for event to finish.";
                    }

                    User32.SetWindowText((IntPtr)p, "NEED FOR SPEED™ WORLD | Server: " + ServerName + " | " + DiscordGamePresence.LauncherRPC + " | Force Restart In: " + secondsToShutDownNamed);
                }

                --secondsToShutDown;
            };

            shutdowntimer.Interval = 1000;
            shutdowntimer.Enabled = true;

            if (nfswProcess != null)
            {
                nfswProcess.EnableRaisingEvents = true;
                _nfswPid = nfswProcess.Id;

                nfswProcess.Exited += (sender2, e2) =>
                {
                    _nfswPid = 0;
                    var exitCode = nfswProcess.ExitCode;

                    if (_gameKilledBySpeedBugCheck == true) exitCode = 2137;

                    if (exitCode == 0)
                    {
                        Application.OpenForms["MainScreen"].Close();
                    }
                    else
                    {
                        x.BeginInvoke(new Action(() =>
                        {
                            x.WindowState = FormWindowState.Normal;
                            x.Opacity = 1;
                            x.ShowInTaskbar = true;
                            String errorMsg = "Game Crash with exitcode: " + exitCode.ToString() + " (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073741819) errorMsg = "Game Crash: Access Violation (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073740940) errorMsg = "Game Crash: Heap Corruption (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073740791) errorMsg = "Game Crash: Stack buffer overflow (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -805306369) errorMsg = "Game Crash: Application Hang (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073741515) errorMsg = "Game Crash: Missing dependency files (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073740972) errorMsg = "Game Crash: Debugger crash (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == -1073741676) errorMsg = "Game Crash: Division by Zero (0x" + exitCode.ToString("X") + ")";
                            if (exitCode == 1) errorMsg = "The process nfsw.exe was killed via Task Manager";
                            if (exitCode == 2137) errorMsg = "Launcher killed your game to prevent SpeedBugging.";
                            if (exitCode == -3) errorMsg = "The Server was unable to resolve the request";
                            if (exitCode == -4) errorMsg = "Another instance is already executed";
                            if (exitCode == -5) errorMsg = "DirectX Device was not found. Please install GPU Drivers before playing";
                            if (exitCode == -6) errorMsg = "Server was unable to resolve your request";
                            //ModLoader
                            if (exitCode == 2) errorMsg = "ModNet: Game was launched with invalid command line parameters.";
                            if (exitCode == 3) errorMsg = "ModNet: .links file should not exist upon startup!";
                            if (exitCode == 4) errorMsg = "ModNet: An Unhandled Error Appeared";
                            
                            if (_nfswPid != 0)
                            {
                                try
                                {
                                    Process.GetProcessById(_nfswPid).Kill();
                                }
                                catch { /* ignored */ }
                            }

                            MainScreen.NFSWStarted.Abort();
                            DialogResult restartApp = MessageBox.Show(null, errorMsg + "\nWould you like to restart the launcher?", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (restartApp == DialogResult.Yes)
                            {
                                Properties.Settings.Default.IsRestarting = true;
                                Properties.Settings.Default.Save();
                                Application.Restart();
                                Application.ExitThread();
                            }
                            else
                            {
                                Application.OpenForms["MainScreen"].Close();
                            }
                        }));
                    }
                };
            }
        }
    }
}
