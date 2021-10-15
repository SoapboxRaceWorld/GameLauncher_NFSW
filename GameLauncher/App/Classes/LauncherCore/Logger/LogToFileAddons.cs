using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Support;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Logger
{
    class LogToFileAddons
    {
        private static string CacheDateAndTime = string.Empty;
        public static string DateAndTime()
        {
            if (string.IsNullOrWhiteSpace(CacheDateAndTime))
            {
                CacheDateAndTime = Time.GetTime("Log");
            }

            return CacheDateAndTime;
        }

        public static string OpenLogMessage = "Would you Like to Open the Launcher Error Log and Send it to a Support Channel?" +
                    "\nThis would be Useful for Fixing Issues and Potential Solutions";

        public static void OpenLog(string From, string MessageDetails, Exception Error, string Icon, bool Suppress)
        {
            try
            {
                if (Error != null)
                {
                    Log.Error(From.ToUpper() + ": " + Error.Message);
                    Log.ErrorIC(From.ToUpper() + ": " + Error.HResult);
                    Log.ErrorFR(From.ToUpper() + ": " + Error.ToString());
                }
                else
                {
                    Log.Error(From.ToUpper() + ": No Exception Provided");
                }
            }
            catch { Log.Error("Unable to Log [" + From.ToUpper() + "] Details"); }

            try
            {
                if (Error != null)
                {
                    if (Error.GetBaseException() != null && (Error.GetBaseException() != Error))
                    {
                        Log.Error(From.ToUpper() + " [Base]: " + Error.GetBaseException().Message);
                        Log.ErrorIC(From.ToUpper() + " [Base]: " + Error.GetBaseException().HResult);
                        Log.ErrorFR(From.ToUpper() + " [Base]: " + Error.GetBaseException().ToString());
                    }
                }
                else
                {
                    Log.Error(From.ToUpper() + ": No Exception Provided for Root [Base] Cause");
                }
            }
            catch { Log.Error("Unable to Log [" + From.ToUpper() + " {Base} ] Details"); }

            if (!Suppress)
            {
                MessageBoxIcon IconBox;

                switch (Icon)
                {
                    case "Error":
                        IconBox = MessageBoxIcon.Error;
                        break;
                    case "Exclamation":
                        IconBox = MessageBoxIcon.Exclamation;
                        break;
                    case "Information":
                        IconBox = MessageBoxIcon.Information;
                        break;
                    case "Warning":
                        IconBox = MessageBoxIcon.Warning;
                        break;
                    default:
                        IconBox = MessageBoxIcon.None;
                        break;
                }

                string FormattedMessage = string.IsNullOrWhiteSpace(MessageDetails) ? string.Empty : MessageDetails + "\n" + ((Error != null) ? Error.Message : "Unknown Error [Null Exception]") + "\n\n";

                DialogResult OpenLogFile = MessageBox.Show(null, FormattedMessage + OpenLogMessage, "GameLauncher Error Log",
                    MessageBoxButtons.YesNo, IconBox);

                if (OpenLogFile == DialogResult.Yes)
                {
                    try
                    {
                        if (File.Exists(Locations.LogLauncher))
                        {
                            Process.Start(Locations.LogLauncher);
                        }
                    }
                    catch
                    {
                        if (Directory.Exists(Locations.LogCurrentFolder))
                        {
                            Process.Start(Locations.LogCurrentFolder);
                        }
                    }
                }
            }
        }

        public static void RemoveLogs()
        {
            String[] FilesToRemove = new string[]
            {
                "Communication.log",
                "communication.log",
                /* Legacy Logs */
                "launcher.log",
                "Verify.log"
            };

            foreach (string Files in FilesToRemove)
            {
                if (File.Exists(Path.Combine(Locations.LauncherFolder, Files)))
                {
                    try
                    {
                        File.Delete(Path.Combine(Locations.LauncherFolder, Files));
                    }
                    catch (Exception Error)
                    {
                        OpenLog("LOG REMOVAL", null, Error, null, true);
                    }
                }
            }
        }
    }
}
