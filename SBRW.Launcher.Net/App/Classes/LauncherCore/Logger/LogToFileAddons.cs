using SBRW.Launcher.App.Classes.LauncherCore.Global;
using SBRW.Launcher.Core.Extension.Logging_;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SBRW.Launcher.App.Classes.LauncherCore.Logger
{
    class LogToFileAddons
    {
        public static string OpenLogMessage = "Would you Like to Open the Launcher Error Log and Send it to a Support Channel?" +
                    "\nThis would be Useful for Fixing Issues and Potential Solutions";

        public static void OpenLog(string From, string MessageDetails, Exception Error, string Icon, bool Suppress)
        {
            if (File.Exists(Path.Combine(Locations.LauncherFolder, "SBRW.Launcher.Core.dll")))
            {
                Log_Detail.OpenLog(From, MessageDetails, Error, Icon, Suppress, OpenLogMessage);
            }
            else
            {
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

                    string FormattedMessage = string.IsNullOrWhiteSpace(MessageDetails) ? string.Empty : MessageDetails + "\n" + ((Error != null) ? Error.Message + 
                        (Error.GetBaseException() != null && (Error.GetBaseException() != Error) ? "\n" + Error.GetBaseException().Message : string.Empty) : "Unknown Error [Null Exception]") + "\n\n";

                    DialogResult OpenLogFile = MessageBox.Show(null, FormattedMessage + OpenLogMessage, "GameLauncher Error Log",
                        MessageBoxButtons.YesNo, IconBox);

                    if (OpenLogFile == DialogResult.Yes)
                    {
                        try
                        {
                            if (Directory.Exists(Path.Combine(Locations.LauncherFolder, "Logs")))
                            {
                                Process.Start(Path.Combine(Locations.LauncherFolder, "Logs"));
                            }
                        }
                        finally
                        {
                            GC.Collect();
                        }
                    }
                }
            }
        }
    }
}
