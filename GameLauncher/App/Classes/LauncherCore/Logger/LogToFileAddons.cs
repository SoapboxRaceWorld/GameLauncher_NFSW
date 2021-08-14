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
            Log.Error(From.ToUpper() + ": " + Error.Message);
            Log.ErrorIC(From.ToUpper() + ": " + Error.HResult);
            Log.ErrorFR(From.ToUpper() + ": " + Error.ToString());

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

                string FormattedMessage = string.IsNullOrWhiteSpace(MessageDetails) ? string.Empty : MessageDetails + "\n" + Error.Message + "\n\n";

                DialogResult OpenLogFile = MessageBox.Show(null, FormattedMessage + OpenLogMessage, "GameLauncher Error Log", 
                    MessageBoxButtons.YesNo, IconBox);

                if (OpenLogFile == DialogResult.Yes)
                {
                    Process.Start(Locations.LogLauncher);
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
