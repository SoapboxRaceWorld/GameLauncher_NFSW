using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.App
{
    public partial class SecurityCenterScreen : Form
    {
        public static Thread ExclusionRulesThread;

        public static string CacheOldLocation;

        public SecurityCenterScreen()
        {
            InitializeComponent();
        }

        public static void GameScanner(bool startScan)
        {
            if (startScan)
            {
                if (!ExclusionRulesThread.IsAlive)
                {
                    ExclusionRulesThread = new Thread(new ThreadStart(FunctionsOnAThread))
                    {
                        Name = "SecurityCenter"
                    };
                    Log.Info("SECURITY CENTER: Attempting to Start Thread");
                    ExclusionRulesThread.Start();
                    Log.Info("SECURITY CENTER: Started Thread");
                }
                else
                {
                    MessageBox.Show("Security Center is Currently Running. " +
                        "Please Wait a Minute before Trying Again.", "Security Center", MessageBoxButtons.OK);
                }
            }
            else if (!startScan)
            {
                Log.Info("SECURITY CENTER: Attempting to Stop the Thread");
                ExclusionRulesThread.Abort();
                Log.Info("SECURITY CENTER: Stopped Thread");
            }
        }

        public static void FunctionsOnAThread()
        {

        }

        public static bool GetSecurityCenterStatus(string Query)
        {
            ManagementObjectSearcher ObjectPath = null;
            ManagementObjectCollection ObjectCollection = null;

            try
            {
                ObjectPath = new ManagementObjectSearcher(Path.Combine("root", "Microsoft", "Windows", "Defender"),
                    "SELECT * FROM MSFT_MpComputerStatus");
                ObjectCollection = ObjectPath.Get();

                foreach (ManagementBaseObject SearchBase in ObjectCollection)
                {
                    if (ObjectCollection != null)
                    {
                        if (bool.TryParse(SearchBase.Properties[Query].Value.ToString(), out bool TrueOrFalse))
                        {
                            return (bool)SearchBase.Properties[Query].Value;
                        }
                    }
                }
            }
            catch (ManagementException Error)
            {
                LogToFileAddons.OpenLog("Windows Defender Status", null, Error, null, true);
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("Windows Defender Status", null, Error, null, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Windows Defender Status", null, Error, null, true);
            }
            finally
            {
                if (ObjectPath != null) { ObjectPath.Dispose(); }
                if (ObjectCollection != null) { ObjectCollection.Dispose(); }
            }

            return false;
        }

        private static string[] ExclusionCheck()
        {
            ManagementObjectSearcher ObjectPath = null;
            ManagementObjectCollection ObjectCollection = null;

            try
            {
                ObjectPath = new ManagementObjectSearcher(Path.Combine("root", "Microsoft", "Windows", "Defender"),
                    "SELECT * FROM MSFT_MpPreference");
                ObjectCollection = ObjectPath.Get();

                foreach (ManagementBaseObject SearchBase in ObjectCollection)
                {
                    if (ObjectCollection != null)
                    {
                        return (string[])SearchBase.Properties["ExclusionPath"].Value;
                    }
                }
            }
            catch (ManagementException Error)
            {
                LogToFileAddons.OpenLog("Windows Defender Exclusion Path Check", null, Error, null, true);
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("Windows Defender Exclusion Path Check", null, Error, null, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Windows Defender Exclusion Path Check", null, Error, null, true);
            }
            finally
            {
                if (ObjectPath != null) { ObjectPath.Dispose(); }
                if (ObjectCollection != null) { ObjectCollection.Dispose(); }
            }

            return Array.Empty<string>();
        }

        private static bool ExlusionBooleanCheck(string FolderPath)
        {
            if (!string.IsNullOrWhiteSpace(FolderPath))
            {
                try
                {
                    if (GetSecurityCenterStatus("AntivirusEnabled") && GetSecurityCenterStatus("AntispywareEnabled") &&
                        GetSecurityCenterStatus("RealTimeProtectionEnabled"))
                    {
                        if (ExclusionCheck() != null)
                        {
                            if (ExclusionCheck().Any())
                            {
                                return ExclusionCheck().Contains(FolderPath);
                            }
                        }
                    }
                }
                catch { return false; }
            }

            return false;
        }

        public static void DefenderCommands(string Type, string PathOfFolder, bool Supress)
        {
            Log.Checking("Windows Defender: ".ToUpper() + "Exclusion Checking");
            switch (Type)
            {
                case "Add":
                    if (!ExlusionBooleanCheck(PathOfFolder))
                    {
                        try
                        {
                            /* Remove current Exclusion and Add new location for Exclusion (Game Files Only!) */
                            using (PowerShell ps = PowerShell.Create())
                            {
                                ps.AddScript($"Add-MpPreference -ExclusionPath \"{Strings.Encode(PathOfFolder)}\"");
                                var result = ps.Invoke();
                            }

                            Log.Completed("Windows Defender: ".ToUpper() + "Folder is now Excluded. -> " + PathOfFolder);
                        }
                        catch (Exception Error)
                        {
                            string ErrorMessage = "Windows Defender Exclusion Encountered an Error:";
                            LogToFileAddons.OpenLog("Windows Defender Exclusion Add Path", ErrorMessage, Error, "Error", false);
                            FileSettingsSave.WindowsDefenderStatus = "Not Supported";
                            FileSettingsSave.SaveSettings();
                        }
                    }
                    else if (Supress)
                    {
                        Log.Completed("Windows Defender: ".ToUpper() + "This Folder is already Excluded. -> " + PathOfFolder);
                    }
                    else
                    {
                        Log.Completed("Windows Defender: ".ToUpper() + "This Folder is already Excluded. -> " + PathOfFolder);
                        MessageBox.Show("This Folder is already Excluded.", "Security Center", MessageBoxButtons.OK);
                    }
                    break;
                case "Remove":
                    if (ExlusionBooleanCheck(PathOfFolder))
                    {
                        try
                        {
                            /* Remove current Exclusion and Add new location for Exclusion (Game Files Only!) */
                            using (PowerShell ps = PowerShell.Create())
                            {
                                ps.AddScript($"Remove-MpPreference -ExclusionPath \"{Strings.Encode(PathOfFolder)}\"");
                                var result = ps.Invoke();
                            }

                            Log.Completed("Windows Defender: ".ToUpper() + "Folder is no longer Excluded. -> " + PathOfFolder);
                        }
                        catch (Exception Error)
                        {
                            string ErrorMessage = "Windows Defender Exclusion Encountered an Error:";
                            LogToFileAddons.OpenLog("Windows Defender Exclusion Remove Path", ErrorMessage, Error, "Error", false);
                            FileSettingsSave.WindowsDefenderStatus = "Not Supported";
                            FileSettingsSave.SaveSettings();
                        }
                    }
                    else if (Supress)
                    {
                        Log.Completed("Windows Defender: ".ToUpper() + "This Folder is already not Excluded. -> " + PathOfFolder);
                    }
                    else
                    {
                        Log.Completed("Windows Defender: ".ToUpper() + "This Folder is already not Excluded. -> " + PathOfFolder);
                        MessageBox.Show("This Folder is already not Excluded.", "Security Center", MessageBoxButtons.OK);
                    }
                    break;
                case "Check":
                    break;
                default:
                    Log.Warning("Windows Defender: ".ToUpper() + " Did you know, DavidCarbon made a mistake at a line of code?");
                    break;
            }
        }

        private static bool ButtonEnabler(string WhichAPI, string PathOfFolder)
        {
            switch (WhichAPI)
            {
                case "Defender":
                    try { return ExlusionBooleanCheck(PathOfFolder); }
                    catch { return false; }
                case "Firewall":
                    try { return true; }
                    catch { return false; }
                default:
                    return false;
            }
        }

        private void SetColors()
        {
            ResetFirewallRulesButton.ForeColor = Theming.RedForeColorButton;
            ResetFirewallRulesButton.BackColor = Theming.RedBackColorButton;
            ResetFirewallRulesButton.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
            ResetFirewallRulesButton.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
        }

        public static void WindowsDefender(string Type, string Mode, string Path, string SecondPath, string Notes)
        {
            try
            {
                if (SecurityCenter.Antivirus() && SecurityCenter.Antispyware() && SecurityCenter.RealTimeProtection())
                {
                    /* Create Windows Defender Exclusion */
                    try
                    {
                        if (Type == "Add-Launcher" || Type == "Add-Game")
                        {
                            if (Mode == "Complete Exclude")
                            {
                                /* Add Exclusion to Windows Defender */
                                using (PowerShell ps = PowerShell.Create())
                                {
                                    ps.AddScript($"Add-MpPreference -ExclusionPath \"{Strings.Encode(Path)}\"");
                                    if (Directory.Exists(SecondPath))
                                    {
                                        ps.AddScript($"Add-MpPreference -ExclusionPath \"{Strings.Encode(SecondPath)}\"");
                                    }
                                    var result = ps.Invoke();
                                }
                            }
                            else
                            {
                                Log.Warning("WINDOWS DEFENDER: Unknown Function Call in 'WindowsDefender' with Add Sub-Section. " +
                                    "Please Check Code in Visual Studio");
                            }

                            FileSettingsSave.WindowsDefenderStatus = "Excluded";

                            Log.Info("WINDOWS DEFENDER: Excluded " + Notes + " Folder");
                        }
                        else if (Type == "Reset-Launcher" || Type == "Reset-Game")
                        {
                            if (Mode == "Complete Reset")
                            {
                                /* Add Exclusion to Windows Defender */
                                using (PowerShell ps = PowerShell.Create())
                                {
                                    ps.AddScript($"Remove-MpPreference -ExclusionPath \"{Strings.Encode(Path)}\"");
                                    ps.AddScript($"Remove-MpPreference -ExclusionPath \"{Strings.Encode(SecondPath)}\"");
                                    var result = ps.Invoke();
                                }

                                FileSettingsSave.WindowsDefenderStatus = "Not Excluded";
                            }
                            else if (Mode == "Update Reset")
                            {
                                /* Remove current Exclusion and Add new location for Exclusion (Game Files Only!) */
                                using (PowerShell ps = PowerShell.Create())
                                {
                                    ps.AddScript($"Remove-MpPreference -ExclusionPath \"{Strings.Encode(Path)}\"");
                                    if (Directory.Exists(SecondPath))
                                    {
                                        ps.AddScript($"Add-MpPreference -ExclusionPath \"{Strings.Encode(SecondPath)}\"");
                                    }
                                    var result = ps.Invoke();
                                }
                            }
                            else
                            {
                                Log.Warning("WINDOWS DEFENDER: Unknown Function Call in 'WindowsDefender' with Reset Sub-Section. " +
                                    "Please Check Code in Visual Studio");
                            }

                            Log.Warning("WINDOWS DEFENDER: " + Notes + " Folders");
                        }
                        else
                        {
                            Log.Warning("WINDOWS DEFENDER: Unknown Function Call in 'WindowsDefender'. Please Check Code in Visual Studio");
                        }

                        FileSettingsSave.SaveSettings();
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("WINDOWS DEFENDER", null, Error, null, true);
                        FileSettingsSave.WindowsDefenderStatus = "Not Excluded";
                        FileSettingsSave.SaveSettings();
                    }
                }
                else
                {
                    FileSettingsSave.WindowsDefenderStatus = "Not Supported";
                    FileSettingsSave.SaveSettings();
                }
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS DEFENDER", null, Error, null, true);
                FileSettingsSave.WindowsDefenderStatus = "Not Supported";
                FileSettingsSave.SaveSettings();
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS DEFENDER", null, Error, null, true);
                FileSettingsSave.WindowsDefenderStatus = "Not Supported";
                FileSettingsSave.SaveSettings();
            }
        }
    }
}
