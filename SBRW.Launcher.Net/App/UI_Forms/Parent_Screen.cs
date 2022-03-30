using SBRW.Launcher.App.Classes.InsiderKit;
using SBRW.Launcher.App.Classes.LauncherCore.APICheckers;
using SBRW.Launcher.App.Classes.LauncherCore.FileReadWrite;
using SBRW.Launcher.App.Classes.LauncherCore.Global;
using SBRW.Launcher.App.Classes.LauncherCore.Languages.Visual_Forms;
using SBRW.Launcher.App.Classes.LauncherCore.LauncherUpdater;
using SBRW.Launcher.App.Classes.LauncherCore.Lists;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.App.Classes.LauncherCore.ModNet;
using SBRW.Launcher.App.Classes.LauncherCore.Support;
using SBRW.Launcher.App.Classes.LauncherCore.Visuals;
using SBRW.Launcher.App.Classes.SystemPlatform.Components;
using SBRW.Launcher.App.Classes.SystemPlatform.Unix;
using SBRW.Launcher.App.Classes.SystemPlatform.Windows;
using SBRW.Launcher.App.UI_Forms.Main_Screen;
using SBRW.Launcher.App.UI_Forms.Welcome_Screen;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.Registry_;
using SBRW.Launcher.Core.Extension.Time_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Extra.Ini_;
using SBRW.Launcher.Core.Extra.XML_;
using SBRW.Launcher.Core.Proxy.Nancy_;
using SBRW.Launcher.Core.Required.Certificate;
using SBRW.Launcher.Core.Required.System.Windows_;
using SBRW.Launcher.Core.Theme;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms
{
    public partial class Parent_Screen : Form
    {
        #region Screen Variables
        private Point Mouse_Down_Point { get; set; } = Point.Empty;
        public static Parent_Screen? Screen_Instance { get; set; }
        public static Panel? Screen_Panel_Forms { get; set; }
        public static TextBox? Screen_TextBox_LiveLog { get; set; }
        private static bool Clock_Tick_Theme_Update { get; set; }
        #endregion

        #region Screen Login Variables
        public static BackgroundWorker? BackgroundWorker_One { get; set; }
        public static bool Launcher_Restart { get; set; }
        #endregion

        #region Dragable Form Window & Functions
        public void Move_Window_Mouse_Down(object sender, MouseEventArgs e)
        {
            if (e.Y <= 90) Mouse_Down_Point = new Point(e.X, e.Y);
        }

        public void Move_Window_Mouse_Up(object sender, MouseEventArgs e)
        {
            Mouse_Down_Point = Point.Empty;
            Opacity = 1;
        }

        public void Move_Window_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (Mouse_Down_Point.IsEmpty) { return; }
            Form Main_Local_Window = this as Form;
            Main_Local_Window.Location = new Point(Main_Local_Window.Location.X + (e.X - Mouse_Down_Point.X), Main_Local_Window.Location.Y + (e.Y - Mouse_Down_Point.Y));
            InformationCache.ParentScreenLocation = new Point(Main_Local_Window.Location.X + (e.X - Mouse_Down_Point.X), Main_Local_Window.Location.Y + (e.Y - Mouse_Down_Point.Y));
            Opacity = 0.9;
        }

        public void Position_Window_Set()
        {
            FunctionStatus.CenterScreen(this);
        }
        #endregion

        #region Parent Screen Load and Shown
        private void Parent_Screen_Load(object sender, EventArgs e)
        {
            if (e != null)
            {
                Position_Window_Set();
            } 
        }

        #region Application Start Process
        private void Parent_Screen_Shown(object sender, EventArgs e)
        {
            if (e == null)
            {
                return;
            }

            LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "Set Parent Window location");

            Presence_Launcher.Start("Start Up", "576154452348633108");

            if (!UnixOS.Detected())
            {
                Presence_Launcher.Status("Start Up", "Checking .NET Framework");
                try
                {
                    /* Check if User has a compatible .NET Framework Installed */
                    if (int.TryParse(Registry_Core.Read("Release", @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"), out int NetFrame_Version))
                    {
                        /* For now, allow edge case of Windows 8.0 to run .NET 4.6.1 where upgrading to 8.1 is not possible */
                        if (Product_Version.GetWindowsNumber() == 6.2 && NetFrame_Version <= 394254)
                        {
                            if (MessageBox.Show(null, Translations.Database("Program_TextBox_NetFrame_P1") +
                            " .NETFramework, Version=v4.6.1 \n\n" + Translations.Database("Program_TextBox_NetFrame_P2"),
                            "GameLauncher.exe - " + Translations.Database("Program_TextBox_NetFrame_P3"),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                            {
                                Process.Start("https://dotnet.microsoft.com/download/dotnet-framework/net461");
                            }

                            FunctionStatus.LauncherForceClose = true;
                        }
                        /* Otherwise, all other OS Versions should have 4.6.2 as a Minimum Version */
                        else if (NetFrame_Version <= 394802)
                        {
                            if (MessageBox.Show(null, Translations.Database("Program_TextBox_NetFrame_P1") +
                            " .NETFramework, Version=v4.6.2 \n\n" + Translations.Database("Program_TextBox_NetFrame_P2"),
                            "GameLauncher.exe - " + Translations.Database("Program_TextBox_NetFrame_P3"),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                            {
                                Process.Start("https://dotnet.microsoft.com/download/dotnet-framework");
                            }

                            FunctionStatus.LauncherForceClose = true;
                        }
                        else
                        {
                            LogToFileAddons.Parent_Log_Screen(7, "NET-FRAMEWORK", "Supported Installed Version");
                        }
                    }
                    else
                    {
                        LogToFileAddons.Parent_Log_Screen(4, "NET-FRAMEWORK", "Failed to Parse Version");
                    }
                }
                catch
                {
                    FunctionStatus.LauncherForceClose = true;
                }
                finally
                {
                    GC.Collect();
                }
            }

            if (FunctionStatus.LauncherForceClose)
            {
                FunctionStatus.ErrorCloseLauncher("Closing From .NET Framework Check", false);
            }
            else
            {
                Log.Start();
                Log_Location.RemoveLegacyLogs();

                LogToFileAddons.Parent_Log_Screen(1, "CURRENT DATE", Time_Clock.GetTime("Date"));
                LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER MIGRATION", "Appdata and/or Roaming Folders");
                /* Deletes Folders that will Crash the Launcher (Cleanup Migration) */
                try
                {
                    if (!Directory.Exists(Locations.RoamingAppDataFolder_Launcher))
                    {
                        Directory.CreateDirectory(Locations.RoamingAppDataFolder_Launcher);
                    }
                    if (Directory.Exists(Path.Combine(Locations.LocalAppDataFolder, "Soapbox_Race_World")))
                    {
                        Directory.Delete(Path.Combine(Locations.LocalAppDataFolder, "Soapbox_Race_World"), true);
                    }
                    if (Directory.Exists(Path.Combine(Locations.RoamingAppDataFolder, "Soapbox_Race_World")))
                    {
                        Directory.Delete(Path.Combine(Locations.RoamingAppDataFolder, "Soapbox_Race_World"), true);
                    }
                    if (Directory.Exists(Path.Combine(Locations.LocalAppDataFolder, "SoapBoxRaceWorld")))
                    {
                        Directory.Delete(Path.Combine(Locations.LocalAppDataFolder, "SoapBoxRaceWorld"), true);
                    }
                    if (Directory.Exists(Path.Combine(Locations.RoamingAppDataFolder, "SoapBoxRaceWorld")))
                    {
                        Directory.Delete(Path.Combine(Locations.RoamingAppDataFolder, "SoapBoxRaceWorld"), true);
                    }
                    if (Directory.Exists(Path.Combine(Locations.LocalAppDataFolder, "WorldUnited.gg")))
                    {
                        Directory.Delete(Path.Combine(Locations.LocalAppDataFolder, "WorldUnited.gg"), true);
                    }
                    if (Directory.Exists(Path.Combine(Locations.RoamingAppDataFolder, "WorldUnited.gg")))
                    {
                        Directory.Delete(Path.Combine(Locations.RoamingAppDataFolder, "WorldUnited.gg"), true);
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER MIGRATION", string.Empty, Error, string.Empty, true);
                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER MIGRATION", Error.InnerException.Message, false, true);
                    }
                }
                finally
                {
                    LogToFileAddons.Parent_Log_Screen(3, "LAUNCHER MIGRATION", "Done");
                    GC.Collect();
                }

                LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER XML", "If File Exists or Not");
                Presence_Launcher.Status("Start Up", "Checking if UserSettings XML Exists");
                /* Create Default Configuration Files (if they don't already exist) */
                if (!File.Exists(Locations.UserSettingsXML))
                {
                    try
                    {
                        if (!Directory.Exists(Locations.UserSettingsFolder))
                        {
                            Directory.CreateDirectory(Locations.UserSettingsFolder);
                        }

                        File.WriteAllBytes(Locations.UserSettingsXML, ExtractResource.AsByte("GameLauncher.Resources.UserSettings.UserSettings.xml"));
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("LAUNCHER XML", string.Empty, Error, string.Empty, true);
                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER XML", Error.InnerException.Message, false, true);
                        }
                    }
                    finally
                    {
                        GC.Collect();
                    }
                }
                LogToFileAddons.Parent_Log_Screen(3, "LAUNCHER XML", "Done");

                LogToFileAddons.Parent_Log_Screen(8,
                    EnableInsiderDeveloper.Allowed() ? "DEV TEST " : (EnableInsiderBetaTester.Allowed() ? "BETA TEST " : ""),
                    "GameLauncher " + Application.ProductVersion + "_" + InsiderInfo.BuildNumberOnly());

                LogToFileAddons.Parent_Log_Screen(2, "OS", "Detecting");
                Presence_Launcher.Status("Start Up", "Checking Operating System");
                try
                {
                    if (UnixOS.Detected())
                    {
                        LogToFileAddons.Parent_Log_Screen(7, "Detected OS", Launcher_Value.System_OS_Name = UnixOS.FullName());
                    }
                    else
                    {
                        LogToFileAddons.Parent_Log_Screen(7, "Detected OS", Launcher_Value.System_OS_Name = Product_Version.ConvertWindowsNumberToName());
                        LogToFileAddons.Parent_Log_Screen(7, "Windows Build", Product_Version.GetWindowsBuildNumber().ToString());
                        LogToFileAddons.Parent_Log_Screen(7, "NT Version", Environment.OSVersion.VersionString);
                        LogToFileAddons.Parent_Log_Screen(7, "Video Card", HardwareInfo.GPU.CardName());
                        LogToFileAddons.Parent_Log_Screen(7, "Driver Version", HardwareInfo.GPU.DriverVersion());
                    }
                    LogToFileAddons.Parent_Log_Screen(3, "OS", "Detected");
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("SYSTEM", string.Empty, Error, string.Empty, true);
                    FunctionStatus.LauncherForceCloseReason = "Code: 0\n" + Translations.Database("Program_TextBox_System_Detection") + "\n" + Error.Message;
                    FunctionStatus.LauncherForceClose = true;
                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER XML", Error.InnerException.Message, false, true);
                    }
                }
                finally
                {
                    GC.Collect();
                }

                if (FunctionStatus.LauncherForceClose)
                {
                    FunctionStatus.ErrorCloseLauncher("Closing From Operating System Check", false);
                }
                else
                {
                    /* Set Launcher Directory */
                    LogToFileAddons.Parent_Log_Screen(2, "SETUP", "Setting Launcher Folder Directory");
                    Directory.SetCurrentDirectory(Locations.LauncherFolder);
                    LogToFileAddons.Parent_Log_Screen(3, "SETUP", "Current Directory now Set at -> " + Locations.LauncherFolder);

                    if (!UnixOS.Detected())
                    {
                        LogToFileAddons.Parent_Log_Screen(2, "FOLDER LOCATION", "Checking Launcher Folder Directory");
                        Presence_Launcher.Status("Start Up", "Checking Launcher Folder Locations");

                        switch (FunctionStatus.CheckFolder(Locations.LauncherFolder))
                        {
                            case FolderType.IsTempFolder:
                            case FolderType.IsUsersFolders:
                            case FolderType.IsProgramFilesFolder:
                            case FolderType.IsWindowsFolder:
                            case FolderType.IsRootFolder:
                                string Constructed_Msg = string.Empty;

                                Constructed_Msg += Translations.Database("Program_TextBox_Folder_Check_Launcher") + "\n\n";
                                Constructed_Msg += Translations.Database("Program_TextBox_Folder_Check_Launcher_P2") + "\n";
                                Constructed_Msg += "• X:\\GameLauncher.exe " + Translations.Database("Program_TextBox_Folder_Check_Launcher_P3") + "\n";
                                Constructed_Msg += "• C:\\Program Files\n";
                                Constructed_Msg += "• C:\\Program Files (x86)\n";
                                Constructed_Msg += "• C:\\Users " + Translations.Database("Program_TextBox_Folder_Check_Launcher_P4") + "\n";
                                Constructed_Msg += "• C:\\Windows\n\n";
                                Constructed_Msg += Translations.Database("Program_TextBox_Folder_Check_Launcher_P5") + "\n";
                                Constructed_Msg += "• 'C:\\Soapbox Race World' " + Translations.Database("Program_TextBox_Folder_Check_Launcher_P6") + " 'C:\\SBRW'\n";
                                Constructed_Msg += Translations.Database("Program_TextBox_Folder_Check_Launcher_P7") + "\n\n";

                                MessageBox.Show(null, Constructed_Msg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                FunctionStatus.LauncherForceClose = true;
                                break;
                        }

                        LogToFileAddons.Parent_Log_Screen(3, "FOLDER LOCATION", "Done");
                    }

                    if (FunctionStatus.LauncherForceClose)
                    {
                        FunctionStatus.ErrorCloseLauncher("Closing From Invalid Launcher Location", false);
                    }
                    else
                    {
                        if (!FunctionStatus.HasWriteAccessToFolder(Locations.LauncherFolder))
                        {
                            FunctionStatus.LauncherForceClose = true;
                            FunctionStatus.LauncherForceCloseReason = Translations.Database("Program_TextBox_Folder_Write_Test");
                            FunctionStatus.ErrorCloseLauncher("Closing From No Write Access", false);
                        }
                        else
                        {
                            LogToFileAddons.Parent_Log_Screen(3, "WRITE TEST", "Passed");
                            /* Location Migration */
                            if (!UnixOS.Detected())
                            {
                                LogToFileAddons.Parent_Log_Screen(2, "Account File Migration", "Doing Migration");
                                Presence_Launcher.Status("Start Up", "Doing Ini File Migration");
                                if (File.Exists(Ini_Location.Name_Account_Ini))
                                {
                                    try
                                    {
                                        if (File.Exists(Ini_Location.Launcher_Account))
                                        {
                                            File.Move(Ini_Location.Launcher_Account,
                                                Path.Combine(Locations.RoamingAppDataFolder_Launcher, Time_Folder.DateAndTime() + "_" + Ini_Location.Name_Account_Ini));
                                        }

                                        File.Move(Ini_Location.Name_Account_Ini, Ini_Location.Launcher_Account);
                                    }
                                    catch (Exception Error)
                                    {
                                        LogToFileAddons.OpenLog("Account File Migration", string.Empty, Error, string.Empty, true);
                                        FunctionStatus.LauncherForceClose = true;
                                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                        {
                                            LogToFileAddons.Parent_Log_Screen(5, "Account File Migration", Error.InnerException.Message, false, true);
                                        }
                                    }
                                    finally
                                    {
                                        GC.Collect();
                                    }
                                }
                                else
                                {
                                    LogToFileAddons.Parent_Log_Screen(3, "Account File Migration", "Already Migrated");
                                }

                                LogToFileAddons.Parent_Log_Screen(3, "Account File Migration", "Done");
                            }

                            if (FunctionStatus.LauncherForceClose)
                            {
                                ///@DavidCarbon or @Zacam - Remember to Translate This!
                                FunctionStatus.LauncherForceCloseReason = "Failed to Successfully Migrate Ini File(s)";
                                FunctionStatus.ErrorCloseLauncher("Closing Ini Migration", false);
                            }
                            else
                            {
                                LogToFileAddons.Parent_Log_Screen(2, "INI FILES", "Doing Nullsafe");
                                Presence_Launcher.Status("Start Up", "Doing NullSafe ini Files");
                                Save_Settings.NullSafe();
                                Save_Account.NullSafe();
                                LogToFileAddons.Parent_Log_Screen(3, "INI FILES", "Done");
                                /* Sets up Theming */
                                Theming.CheckIfThemeExists();

                                LogToFileAddons.Parent_Log_Screen(12, "APPLICATION", "Setting Language");
                                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(Translations.UI(Translations.Application_Language = Save_Settings.Live_Data.Launcher_Language.ToLower(), true));
                                LogToFileAddons.Parent_Log_Screen(3, "APPLICATION", "Done Setting Language '" + Translations.UI(Translations.Application_Language) + "'");

                                /* Windows 7 TLS Check */
                                if (Product_Version.GetWindowsNumber() == 6.1)
                                {
                                    LogToFileAddons.Parent_Log_Screen(2, "SSL/TLS", "Windows 7 Detected");
                                    Presence_Launcher.Status("Start Up", "Checking Windows 7 SSL/TLS");

                                    try
                                    {
                                        string MessageBoxPopupTLS = string.Empty;

                                        if (string.IsNullOrWhiteSpace(Registry_Core.Read("DisabledByDefault", @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client")))
                                        {
                                            MessageBoxPopupTLS = Translations.Database("Program_TextBox_W7_TLS_P1") + "\n\n";

                                            MessageBoxPopupTLS += "- HKLM/SYSTEM/CurrentControlSet/Control/SecurityProviders\n  /SCHANNEL/Protocols/TLS 1.2/Client\n";
                                            MessageBoxPopupTLS += "- Value: DisabledByDefault -> 0\n\n";

                                            MessageBoxPopupTLS += Translations.Database("Program_TextBox_W7_TLS_P2") + "\n\n";
                                            MessageBoxPopupTLS += Translations.Database("Program_TextBox_W7_TLS_P3");

                                            /* There is only 'OK' Available because this IS Required */
                                            if (MessageBox.Show(null, MessageBoxPopupTLS, "SBRW Launcher",
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                                            {
                                                Registry_Core.Write("DisabledByDefault", 0x0,
                                                    @"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client");
                                                MessageBox.Show(null, Translations.Database("Program_TextBox_W7_TLS_P4"),
                                                    "SBRW Launcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }

                                            LogToFileAddons.Parent_Log_Screen(3, "SSL/TLS", "Added Registry Key");
                                        }
                                        else
                                        {
                                            LogToFileAddons.Parent_Log_Screen(3, "SSL/TLS", "Done");
                                        }
                                    }
                                    catch (Exception Error)
                                    {
                                        LogToFileAddons.OpenLog("SSL/TLS", string.Empty, Error, string.Empty, true);
                                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                        {
                                            LogToFileAddons.Parent_Log_Screen(5, "SSL/TLS", Error.InnerException.Message, false, true);
                                        }
                                    }
                                    finally
                                    {
                                        GC.Collect();
                                    }
                                }

                                /* Windows 7 HotFix Check */
                                if (Product_Version.GetWindowsNumber() == 6.1 && string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Win_7_Patches))
                                {
                                    LogToFileAddons.Parent_Log_Screen(2, "HotFixes", "Windows 7 Detected");
                                    Presence_Launcher.Status("Start Up", "Checking Windows 7 HotFixes");

                                    try
                                    {
                                        if (!ManagementSearcher.GetInstalledHotFix("KB3020369") || !ManagementSearcher.GetInstalledHotFix("KB3125574"))
                                        {
                                            string MessageBoxPopupKB = string.Empty;
                                            MessageBoxPopupKB = Translations.Database("Program_TextBox_W7_KB_P1") + "\n";
                                            MessageBoxPopupKB += Translations.Database("Program_TextBox_W7_KB_P2") + "\n\n";

                                            if (!ManagementSearcher.GetInstalledHotFix("KB3020369"))
                                            {
                                                MessageBoxPopupKB += "- " + Translations.Database("Program_TextBox_W7_KB_P3") + " KB3020369\n";
                                            }

                                            if (!ManagementSearcher.GetInstalledHotFix("KB3125574"))
                                            {
                                                MessageBoxPopupKB += "- " + Translations.Database("Program_TextBox_W7_KB_P3") + " KB3125574\n";
                                            }
                                            MessageBoxPopupKB += "\n" + Translations.Database("Program_TextBox_W7_KB_P4") + "\n";

                                            if (MessageBox.Show(null, MessageBoxPopupKB, "SBRW Launcher",
                                                MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                                            {
                                                /* Since it's Informational we just need to know if they clicked 'OK' */
                                                Save_Settings.Live_Data.Win_7_Patches = "1";
                                            }
                                            else
                                            {
                                                /* or if they clicked 'Cancel' */
                                                Save_Settings.Live_Data.Win_7_Patches = "0";
                                            }

                                            Save_Settings.Save();
                                        }

                                        LogToFileAddons.Parent_Log_Screen(3, "HotFixes", "Done");
                                    }
                                    catch (Exception Error)
                                    {
                                        LogToFileAddons.OpenLog("HotFixes", string.Empty, Error, string.Empty, true);
                                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                        {
                                            LogToFileAddons.Parent_Log_Screen(5, "HotFixes", Error.InnerException.Message, false, true);
                                        }
                                    }
                                    finally
                                    {
                                        GC.Collect();
                                    }
                                }

                                try
                                {
                                    LogToFileAddons.Parent_Log_Screen(2, "FOLDER", "Launcher Data Folder");

                                    if (!Directory.Exists(Locations.LauncherDataFolder))
                                    {
                                        Directory.CreateDirectory(Locations.LauncherDataFolder);
                                    }
                                }
                                catch (Exception Error)
                                {
                                    LogToFileAddons.OpenLog("FOLDER Launcher Data", string.Empty, Error, string.Empty, true);
                                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                    {
                                        LogToFileAddons.Parent_Log_Screen(5, "FOLDER Launcher Data", Error.InnerException.Message, false, true);
                                    }
                                }
                                finally
                                {
                                    LogToFileAddons.Parent_Log_Screen(3, "FOLDER", "Launcher Data Done");
                                    GC.Collect();
                                }

                                try
                                {
                                    LogToFileAddons.Parent_Log_Screen(2, "JSON", "Servers File");

                                    if (File.Exists(Path.Combine(Locations.LauncherFolder, Locations.NameOldServersJSON)) || File.Exists(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON)))
                                    {
                                        if (File.Exists(Locations.LauncherCustomServers))
                                        {
                                            File.Delete(Locations.LauncherCustomServers);
                                        }

                                        File.Move(Path.Combine(Locations.LauncherFolder, Locations.NameOldServersJSON),
                                            Locations.LauncherCustomServers);

                                        LogToFileAddons.Parent_Log_Screen(3, "FOLDER", "Renaming Servers File");
                                    }
                                    else if (!UnixOS.Detected())
                                    {
                                        if (File.Exists(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON)))
                                        {
                                            File.Move(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON), Locations.LauncherCustomServers);
                                        }
                                    }
                                    else if (!File.Exists(Locations.LauncherCustomServers))
                                    {
                                        try
                                        {
                                            File.WriteAllText(Locations.LauncherCustomServers, "[]");
                                            LogToFileAddons.Parent_Log_Screen(3, "FOLDER", "Created Servers File");
                                        }
                                        catch (Exception Error)
                                        {
                                            LogToFileAddons.OpenLog("JSON SERVER FILE", string.Empty, Error, string.Empty, true);
                                        }
                                    }
                                }
                                catch (Exception Error)
                                {
                                    LogToFileAddons.OpenLog("JSON SERVER FILE", string.Empty, Error, string.Empty, true);
                                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                    {
                                        LogToFileAddons.Parent_Log_Screen(5, "JSON SERVER FILE", Error.InnerException.Message, false, true);
                                    }
                                }
                                finally
                                {
                                    LogToFileAddons.Parent_Log_Screen(3, "FOLDER", "Done");
                                    GC.Collect();
                                }

                                if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                                {
                                    LogToFileAddons.Parent_Log_Screen(2, "CLEANLINKS", "Game Path");

                                    if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)))
                                    {
                                        ModNetHandler.CleanLinks(Save_Settings.Live_Data.Game_Path);
                                        LogToFileAddons.Parent_Log_Screen(3, "CLEANLINKS", "Done");
                                    }
                                    else
                                    {
                                        LogToFileAddons.Parent_Log_Screen(3, "CLEANLINKS", "Not Present");
                                    }
                                }

                                LogToFileAddons.Parent_Log_Screen(2, "PROXY", "Checking if Proxy Is Disabled from User Settings! It's value is " + Save_Settings.Live_Data.Launcher_Proxy);
                                if (Save_Settings.Live_Data.Launcher_Proxy == "0")
                                {
                                    LogToFileAddons.Parent_Log_Screen(11, "PROXY", "Starting Proxy (From Startup)");
                                    Proxy_Server.Instance.Start("Splash Screen [Program.cs]");
                                    LogToFileAddons.Parent_Log_Screen(3, "PROXY", "Started");
                                }
                                else
                                {
                                    LogToFileAddons.Parent_Log_Screen(3, "PROXY", "Disabled");
                                }

                                LogToFileAddons.Parent_Log_Screen(2, "PRELOAD", "Headers");
                                Custom_Header.Headers_WHC();
                                LogToFileAddons.Parent_Log_Screen(3, "PRELOAD", "Headers");
                                Presence_Launcher.Status("Start Up", "Checking Root Certificate Authority");
                                Certificate_Store.Latest();

                                LogToFileAddons.Parent_Log_Screen(1, "REDISTRIBUTABLE", "Moved to Function");
                                /* (Starts Function Chain) Check if Redistributable Packages are Installed */
                                Redistributable.Check();
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region First Time Run
        public static void First_Time_Run()
        {
            if (!LauncherUpdateCheck.UpdatePopupStoppedSplashScreen)
            {
                FunctionStatus.LoadingComplete = true;
            }

            if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
            {
                LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "Checking Installation Directory at " + Save_Settings.Live_Data.Game_Path);
            }

            LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER", "Checking Game Installation");
            if (string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
            {
                Presence_Launcher.Status("Start Up", "Doing First Time Run");
                LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "First run!");

                try
                {
                    Form welcome = new Screen_Welcome();
                    DialogResult welcomereply = welcome.ShowDialog();

                    if (welcomereply != DialogResult.OK)
                    {
                        FunctionStatus.LauncherForceClose = true;
                    }
                    else
                    {
                        Save_Settings.Live_Data.Launcher_CDN = SelectedCDN.CDNUrl;
                        Save_Settings.Save();
                    }
                }
                catch
                {
                    LogToFileAddons.Parent_Log_Screen(4, "LAUNCHER", "CDN Source URL was Empty! Setting a Null Safe URL 'http://localhost'");
                    Save_Settings.Live_Data.Launcher_CDN = "http://localhost";
                    LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "Installation Directory was Empty! Creating and Setting Directory at " + Locations.GameFilesFailSafePath);
                    Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                    Save_Settings.Save();
                }

                if (FunctionStatus.LauncherForceClose)
                {
                    FunctionStatus.ErrorCloseLauncher("Closing From Welcome Dialog", false);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                    {
                        Presence_Launcher.Status("Start Up", "User Selecting/Inputting Game Files Folder");

                        try
                        {
                            if (!UnixOS.Detected())
                            {
                                string GameFolderPath = string.Empty;

                                OpenFileDialog FolderDialog = new OpenFileDialog
                                {
                                    InitialDirectory = "C:\\",
                                    ValidateNames = false,
                                    CheckFileExists = false,
                                    CheckPathExists = true,
                                    AutoUpgradeEnabled = false,
                                    Title = "Select the location to Find or Download nfsw.exe",
                                    FileName = "   Select Game Files Folder"
                                };

                                if (FolderDialog.ShowDialog() == DialogResult.OK)
                                {
                                    if (!string.IsNullOrWhiteSpace(FolderDialog.FileName))
                                    {
                                        GameFolderPath = Path.GetDirectoryName(FolderDialog.FileName) ?? string.Empty;
                                    }
                                }

                                FolderDialog.Dispose();

                                if (!string.IsNullOrWhiteSpace(GameFolderPath))
                                {
                                    Presence_Launcher.Status("Start Up", "Verifying Game Files Folder Location");

                                    if (!FunctionStatus.HasWriteAccessToFolder(GameFolderPath))
                                    {
                                        LogToFileAddons.Parent_Log_Screen(5, "FOLDER SELECT DIALOG", "Not enough permissions.");
                                        string ErrorMessage = "You don't have enough permission to select this path as the Installation folder. " +
                                            "Please select another directory by manually setting a new path.";
                                        MessageBox.Show(null, ErrorMessage, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        FunctionStatus.LauncherForceClose = true;
                                        FunctionStatus.LauncherForceCloseReason = ErrorMessage;
                                    }
                                    else
                                    {
                                        if (GameFolderPath.Length == 3)
                                        {
                                            Directory.CreateDirectory("Game Files");
                                            LogToFileAddons.Parent_Log_Screen(4, "FOLDER SELECT DIALOG", "Installing NFSW in root of the harddisk is not allowed.");
                                            MessageBox.Show(null, string.Format("Installing NFSW in root of the harddisk is not allowed. " +
                                                "Instead, we will install it on {0}.", Locations.GameFilesFailSafePath),
                                                "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                                            Save_Settings.Save();
                                            XML_File.Save(1);
                                        }
                                        else if (GameFolderPath == Locations.LauncherFolder)
                                        {
                                            Directory.CreateDirectory("Game Files");
                                            LogToFileAddons.Parent_Log_Screen(4, "FOLDER SELECT DIALOG", "Installing NFSW in same location where the GameLauncher resides is NOT allowed.");
                                            MessageBox.Show(null, string.Format("Installing NFSW in same location where the GameLauncher resides is NOT allowed.\n " +
                                                "Instead, we will install it on {0}.", Locations.GameFilesFailSafePath),
                                                "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                                            Save_Settings.Save();
                                            XML_File.Save(1);
                                        }
                                        else
                                        {
                                            LogToFileAddons.Parent_Log_Screen(11, "FOLDER SELECT DIALOG", "Directory Set at " + GameFolderPath);
                                            Save_Settings.Live_Data.Game_Path = GameFolderPath;
                                            Save_Settings.Save();
                                            XML_File.Save(1);
                                        }
                                    }
                                }
                                else
                                {
                                    FunctionStatus.LauncherForceClose = true;
                                }
                            }
                            else if (string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                            {
                                try
                                {
                                    Save_Settings.Live_Data.Game_Path = Path.GetFullPath("GameFiles");
                                }
                                catch
                                {
                                    Save_Settings.Live_Data.Game_Path = "GameFiles";
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                            {
                                if (!Directory.Exists(Save_Settings.Live_Data.Game_Path))
                                {
                                    LogToFileAddons.Parent_Log_Screen(11, "FOLDER SELECT DIALOG", "Created Game Files Directory at " + Save_Settings.Live_Data.Game_Path);
                                    Directory.CreateDirectory(Save_Settings.Live_Data.Game_Path);
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                            {
                                LogToFileAddons.Parent_Log_Screen(2, "CLEANLINKS", "Game Path");
                                if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)))
                                {
                                    ModNetHandler.CleanLinks(Save_Settings.Live_Data.Game_Path);
                                    LogToFileAddons.Parent_Log_Screen(3, "CLEANLINKS", "Done");
                                }
                                else
                                {
                                    LogToFileAddons.Parent_Log_Screen(3, "CLEANLINKS", "Not Present");
                                }
                            }
                        }
                        catch (Exception Error)
                        {
                            FunctionStatus.LauncherForceClose = true;
                            FunctionStatus.LauncherForceCloseReason = Error.Message;
                            LogToFileAddons.OpenLog("FOLDER SELECT DIALOG", string.Empty, Error, string.Empty, true);
                            if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                            {
                                LogToFileAddons.Parent_Log_Screen(5, "FOLDER SELECT DIALOG", Error.InnerException.Message, false, true);
                            }
                        }
                        finally
                        {
                            GC.Collect();
                        }
                    }
                    else if (!Directory.Exists(Save_Settings.Live_Data.Game_Path))
                    {
                        try
                        {
                            Directory.CreateDirectory(Save_Settings.Live_Data.Game_Path);
                            LogToFileAddons.Parent_Log_Screen(11, "FOLDER", "Created Game Files Directory at " + Save_Settings.Live_Data.Game_Path);
                        }
                        catch (Exception Error)
                        {
                            FunctionStatus.LauncherForceClose = true;
                            FunctionStatus.LauncherForceCloseReason = Error.Message;
                            LogToFileAddons.OpenLog("FOLDER CREATE", string.Empty, Error, string.Empty, true);
                            if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                            {
                                LogToFileAddons.Parent_Log_Screen(5, "FOLDER Create", Error.InnerException.Message, false, true);
                            }
                        }
                        finally
                        {
                            GC.Collect();
                        }
                    }
                }
            }
            LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "Game Installation Path Done");

            if (FunctionStatus.LauncherForceClose)
            {
                FunctionStatus.ErrorCloseLauncher("Closing From Folder Dialog", false);
            }
            else
            {
                if (!UnixOS.Detected())
                {
                    LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER", "Checking Game Path Location");
                    Presence_Launcher.Status("Start Up", "Checking Game Files Folder Location");

                    switch (FunctionStatus.CheckFolder(Save_Settings.Live_Data.Game_Path))
                    {
                        case FolderType.IsSameAsLauncherFolder:
                            try
                            {
                                if (!Directory.Exists(Locations.GameFilesFailSafePath))
                                {
                                    Directory.CreateDirectory(Locations.GameFilesFailSafePath);
                                    LogToFileAddons.Parent_Log_Screen(11, "FOLDER", "Created Game Files Directory at " + Locations.GameFilesFailSafePath);
                                }
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("FOLDER CREATE", string.Empty, Error, string.Empty, true);
                                if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                {
                                    LogToFileAddons.Parent_Log_Screen(5, "FOLDER Create", Error.InnerException.Message, false, true);
                                }
                            }
                            finally
                            {
                                GC.Collect();
                            }
                            LogToFileAddons.Parent_Log_Screen(4, "LAUNCHER", "Installing NFSW in same location where the GameLauncher resides is NOT allowed.", false, true);
                            MessageBox.Show(null, string.Format("Installing NFSW in same location where the GameLauncher resides is NOT allowed.\n" +
                                "Instead, we will install it at {0}.", Locations.GameFilesFailSafePath),
                                "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                            break;
                        case FolderType.IsTempFolder:
                        case FolderType.IsUsersFolders:
                        case FolderType.IsProgramFilesFolder:
                        case FolderType.IsWindowsFolder:
                        case FolderType.IsRootFolder:
                            string constructMsg = string.Empty;
                            constructMsg += "Using this location for Game Files is not allowed.\n\n";
                            constructMsg += "The following locations are also NOT allowed:\n";
                            constructMsg += "• X:\\nfsw.exe (Root of Drive, such as C:\\ or D:\\, must be in a folder)\n";
                            constructMsg += "• C:\\Program Files\n";
                            constructMsg += "• C:\\Program Files (x86)\n";
                            constructMsg += "• C:\\Users (Includes 'Desktop', 'Documents', 'Downloads')\n";
                            constructMsg += "• C:\\Windows\n\n";
                            constructMsg += "Instead, we will install the NFSW Game at " + Locations.GameFilesFailSafePath;
                            try
                            {
                                if (!Directory.Exists(Locations.GameFilesFailSafePath))
                                {
                                    Directory.CreateDirectory(Locations.GameFilesFailSafePath);
                                    LogToFileAddons.Parent_Log_Screen(11, "FOLDER", "Created Game Files Directory at " + Locations.GameFilesFailSafePath);
                                }
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("FOLDER CREATE", string.Empty, Error, string.Empty, true);
                                if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                {
                                    LogToFileAddons.Parent_Log_Screen(5, "FOLDER Create", Error.InnerException.Message, false, true);
                                }
                            }
                            finally
                            {
                                GC.Collect();
                            }
                            LogToFileAddons.Parent_Log_Screen(4, "LAUNCHER", "Installing NFSW in a Restricted Location is not allowed.", false, true);
                            MessageBox.Show(null, constructMsg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                            break;
                    }
                    Save_Settings.Save();
                    LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "Done Checking Game Path Location");
                }

                /* Check If Launcher Failed to Connect to any APIs */
                if (!VisualsAPIChecker.Local_Cached_API())
                {
                    Presence_Launcher.Status("Start Up", "Launcher Encountered API Errors");

                    DialogResult restartAppNoApis = MessageBox.Show(null, "There is no internet connection or local cache, Launcher might crash." +
                        "\n\nClick Yes to Close GameLauncher" +
                        "\nor" +
                        "\nClick No Continue", "GameLauncher has Stopped, Failed To Connect To API", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (restartAppNoApis == DialogResult.Yes)
                    {
                        FunctionStatus.LauncherForceClose = true;
                    }
                }

                if (FunctionStatus.LauncherForceClose)
                {
                    FunctionStatus.ErrorCloseLauncher("Closing From API Error", false);
                }
                else
                {
                    try
                    {
                      
                        if (Screen_Instance != null)
                        {
                            Screen_Instance.Text = "SBRW Launcher: v" + Application.ProductVersion;
                            Screen_Main Custom_Instance_Settings = new Screen_Main() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true, FormBorderStyle = FormBorderStyle.None };

                            if (Screen_Panel_Forms != null)
                            {
                                Screen_Panel_Forms.Visible = true;
                                Screen_Panel_Forms.Controls.Add(Custom_Instance_Settings);
                                Custom_Instance_Settings.Show();
                                Screen_Instance.Size = new Size(891, 529);
                                Screen_Instance.Position_Window_Set();
                                LogToFileAddons.Parent_Log_Screen(1, "MAINSCREEN", "Hello World!", true);
                            }
                        }
                    }
                    catch (COMException Error)
                    {
                        LogToFileAddons.OpenLog("Main Screen", "Launcher Encounterd an Error.", Error, "Error", false);
                        FunctionStatus.ErrorCloseLauncher("Main Screen [Application Run]", false);
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("Main Screen", "Launcher Encounterd an Error.", Error, "Error", false);
                        FunctionStatus.ErrorCloseLauncher("Main Screen [Application Run]", false);
                    }
                    finally
                    {
                        GC.Collect();
                    }
                }
            }
        }
        #endregion

        #region Splash Screen
        private void Clock_Tick(object sender, EventArgs e)
        {
            if (e != null)
            {
                if (FunctionStatus.LoadingComplete || FunctionStatus.LauncherForceClose || Clock_Tick_Theme_Update)
                {
                    if (Clock.Enabled)
                    {
                        Clock.Stop();
                    }
                }
                else if (BackgroundImage != Image_Other.Logo_Splash)
                {
                    Clock_Tick_Theme_Update = true;
                    this.SafeInvokeAction(() => PictureBox_Screen_Splash.BackgroundImage = Image_Other.Logo_Splash, this);
                }
                else
                {
                    GC.Collect();
                }
            }
        }
        #endregion

        #region App Close Functions
        private void ClosingTasks()
        {
            Save_Settings.Save();
            Save_Account.Save();

            try
            {
                if (Screen_Main.LZMA_Downloader != null)
                {
                    if (Screen_Main.LZMA_Downloader.Downloading)
                    {
                        Screen_Main.LZMA_Downloader.Stop();
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("CDN DOWNLOADER [LZMA]", string.Empty, Error, string.Empty, true);
            }

            try
            {
                if (Screen_Main.Pack_SBRW_Unpacker != null)
                {
                    Screen_Main.Pack_SBRW_Unpacker.Cancel = true;
                }

                if (Screen_Main.Pack_SBRW_Downloader != null)
                {
                    Screen_Main.Pack_SBRW_Downloader.Cancel = true;
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("CDN DOWNLOADER", string.Empty, Error, string.Empty, true);
            }

            try
            {
                if (BackgroundWorker_One != null)
                {
                    if (BackgroundWorker_One.IsBusy)
                    {
                        BackgroundWorker_One.CancelAsync();
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("BackgroundWorker_One", string.Empty, Error, string.Empty, true);
            }

            try
            {
                if (FunctionStatus.LauncherBattlePass)
                {
                    Process.GetProcessById(Screen_Main.NfswPid).Kill();
                }
                else
                {
                    Process[] allOfThem = Process.GetProcessesByName("nfsw");

                    if (allOfThem != null && allOfThem.Any())
                    {
                        foreach (var oneProcess in allOfThem)
                        {
                            Process.GetProcessById(oneProcess.Id).Kill();
                        }
                    }
                }
            }
            catch { }

            if (Presence_Launcher.Running())
            {
                Presence_Launcher.Stop("Close");
            }

            if (Proxy_Settings.Running())
            {
                Proxy_Server.Instance.Stop("Main Screen");
            }

            if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
            {
                if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)) && !FunctionStatus.LauncherBattlePass)
                {
                    ModNetHandler.CleanLinks(Save_Settings.Live_Data.Game_Path);
                }
            }
        }

        public void Button_Close_Click(object sender, EventArgs e)
        {
            if (FunctionStatus.LoadingComplete)
            {
                ClosingTasks();
            }
            else
            {
                FunctionStatus.LauncherForceClose = true;
            }

            /* Leave this here. Its to properly close the launcher from Visual Studio (And Close the Launcher a well) 
             * If the Boolen is true it will restart the Application
             */
            if (Launcher_Restart)
            {
                Application.Restart();
            }
            else if (Application.MessageLoop)
            {
                // WinForms Mode
                Application.Exit();
            }

            // If in Console Mode or if Form is Hidden and/or for Background Threads
            Environment.Exit(Environment.ExitCode);
        }
        #endregion

        public Parent_Screen()
        {
            InitializeComponent();
            #region Custom EventHandlers
            MouseMove += new MouseEventHandler(Move_Window_Mouse_Move);
            MouseUp += new MouseEventHandler(Move_Window_Mouse_Up);
            MouseDown += new MouseEventHandler(Move_Window_Mouse_Down);

            Panel_Splash_Screen.MouseMove += new MouseEventHandler(Move_Window_Mouse_Move);
            Panel_Splash_Screen.MouseUp += new MouseEventHandler(Move_Window_Mouse_Up);
            Panel_Splash_Screen.MouseDown += new MouseEventHandler(Move_Window_Mouse_Down);

            Panel_Form_Screens.MouseMove += new MouseEventHandler(Move_Window_Mouse_Move);
            Panel_Form_Screens.MouseUp += new MouseEventHandler(Move_Window_Mouse_Up);
            Panel_Form_Screens.MouseDown += new MouseEventHandler(Move_Window_Mouse_Down);

            Load += new EventHandler(Parent_Screen_Load);
            Shown += new EventHandler(Parent_Screen_Shown);
            Clock.Tick += new EventHandler(Clock_Tick);
            #endregion

            #region Custom Theme
            /*******************************/
            /* Set Font                     /
            /*******************************/
            float MainFontSize = UnixOS.Detected() ? 9f : 9f * 96f / CreateGraphics().DpiY;

            Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            TextBox_Live_Log.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            GroupBox_Launcherlog.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            TransparencyKey = Color_Screen.BG_Settings;
            BackgroundImage = Image_Background.Settings;

            PictureBox_Screen_Splash.BackgroundImage = Image_Other.Logo_Splash;

            ForeColor = Color_Winform.Text_Fore_Color;
            BackColor = Color_Winform.BG_Fore_Color;

            GroupBox_Launcherlog.ForeColor = Color_Winform.Text_Fore_Color;
            TextBox_Live_Log.ForeColor = Color_Winform.Secondary_Text_Fore_Color;
            TextBox_Live_Log.BackColor = Color_Winform.BG_Darker_Fore_Color;

            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Text = "SBRW Launcher: v" + Application.ProductVersion;

            this.Closing += (x, y) =>
            {
                if (FunctionStatus.LoadingComplete)
                {
                    ClosingTasks();
                }
                else
                {
                    FunctionStatus.LauncherForceClose = true;
                }

                Screen_Instance = null;
                Screen_Panel_Forms = null;
                Screen_TextBox_LiveLog = null;

                GC.Collect();
            };
            #endregion

            #region Update Variables
            Screen_Instance = this;
            Screen_Panel_Forms = Panel_Form_Screens;
            Screen_TextBox_LiveLog = TextBox_Live_Log;
            Size = new Size(559, 404);
            #endregion
        }
    }
}
