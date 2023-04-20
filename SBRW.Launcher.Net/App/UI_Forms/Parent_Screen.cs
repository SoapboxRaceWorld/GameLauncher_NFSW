using SBRW.Launcher.RunTime.InsiderKit;
using SBRW.Launcher.RunTime.LauncherCore.APICheckers;
using SBRW.Launcher.RunTime.LauncherCore.FileReadWrite;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Languages.Visual_Forms;
using SBRW.Launcher.RunTime.LauncherCore.LauncherUpdater;
using SBRW.Launcher.RunTime.LauncherCore.Lists;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.ModNet;
using SBRW.Launcher.RunTime.LauncherCore.Support;
using SBRW.Launcher.RunTime.LauncherCore.Visuals;
using SBRW.Launcher.RunTime.SystemPlatform.Components;
using SBRW.Launcher.RunTime.SystemPlatform.Unix;
using SBRW.Launcher.RunTime.SystemPlatform.Windows;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using SBRW.Launcher.Core.Required.DLL;
using SBRW.Launcher.Core.Extension.Hash_;

namespace SBRW.Launcher.App.UI_Forms
{
    public partial class Parent_Screen : Form
    {
        #region Screen Variables
        private Point Mouse_Down_Point { get; set; } = Point.Empty;
        public static Parent_Screen? Screen_Instance { get; set; }
        private static bool Clock_Tick_Theme_Update { get; set; }
        #endregion

        #region Screen Login Variables
        public static bool Launcher_Restart { get; set; }
        #endregion

        #region Dragable Form Window & Functions
        public void Move_Window_Mouse_Down(object sender, MouseEventArgs e)
        {
            if (e.Y <= 90) 
            { 
                Mouse_Down_Point = new Point(e.X, e.Y); 
            }
        }

        public void Move_Window_Mouse_Up(object sender, MouseEventArgs e)
        {
            Mouse_Down_Point = Point.Empty;
            Opacity = 1;
        }

        public void Move_Window_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (Mouse_Down_Point.IsEmpty) 
            { 
                return; 
            }
            else
            {
                Form Main_Local_Window = this as Form;
                Main_Local_Window.Location = new Point(Main_Local_Window.Location.X + (e.X - Mouse_Down_Point.X), Main_Local_Window.Location.Y + (e.Y - Mouse_Down_Point.Y));
                InformationCache.ParentScreenLocation = new Point(Main_Local_Window.Location.X + (e.X - Mouse_Down_Point.X), Main_Local_Window.Location.Y + (e.Y - Mouse_Down_Point.Y));
                Opacity = 0.9;
            }
        }

        public void Position_Window_Set()
        {
            FunctionStatus.CenterScreen(this);
        }

        private void ButtonClose_MouseDown(object sender, EventArgs e)
        {
            Button_Close.BackgroundImage = Image_Icon.Close_Click;
        }

        private void ButtonClose_MouseEnter(object sender, EventArgs e)
        {
            Button_Close.BackgroundImage = Image_Icon.Close_Hover;
        }

        private void ButtonClose_MouseLeaveANDMouseUp(object sender, EventArgs e)
        {
            Button_Close.BackgroundImage = Image_Icon.Close;
        }
        #endregion

        #region Parent Screen Load and Shown
        private void Parent_Screen_Load(object sender, EventArgs e)
        {
            if (e != null)
            {
                LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "Set Parent Window location");
                Position_Window_Set();
                LogToFileAddons.Parent_Log_Screen(3, "LAUNCHER", "Set Parent Window location");
            } 
        }

        #region Application Start Process
        private async void Parent_Screen_Shown(object sender, EventArgs e)
        {
            if (e == null)
            {
                return;
            }

            Presence_Launcher.Start();

#if !(RELEASE_UNIX || DEBUG_UNIX)
            Presence_Launcher.Status(0, "Checking .NET Framework");
            await Task.Run(() =>
            {
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
#if NETFRAMEWORK
                                Process.Start("https://dotnet.microsoft.com/download/dotnet-framework/net461");
#else
                                    Process.Start(new ProcessStartInfo { FileName = "https://dotnet.microsoft.com/download/dotnet-framework/net461", UseShellExecute = true });
#endif
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
#if NETFRAMEWORK
                                Process.Start("https://dotnet.microsoft.com/download/dotnet-framework");
#else
                                    Process.Start(new ProcessStartInfo { FileName = "https://dotnet.microsoft.com/download/dotnet-framework", UseShellExecute = true });
#endif
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
                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                    GC.Collect(); 
                    #endif
                }
            });
#endif

            if (FunctionStatus.LauncherForceClose)
            {
                FunctionStatus.ErrorCloseLauncher("Closing From .NET Framework Check", false, this);
            }
            else
            {
                /* Set Launcher Directory */
                LogToFileAddons.Parent_Log_Screen(2, "SETUP", "Setting Launcher Folder Directory");
                Directory.SetCurrentDirectory(Locations.LauncherFolder);
                LogToFileAddons.Parent_Log_Screen(3, "SETUP", "Current Directory now Set at -> " + Locations.LauncherFolder);

#if !(RELEASE_UNIX || DEBUG_UNIX)
                LogToFileAddons.Parent_Log_Screen(2, "FOLDER LOCATION", "Checking Launcher Folder Directory");
                Presence_Launcher.Status(0, "Checking Launcher Folder Locations");

                await Task.Run(() =>
                {
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

                            MessageBox.Show(this, Constructed_Msg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            FunctionStatus.LauncherForceClose = true;
                            break;
                    }
                });

                LogToFileAddons.Parent_Log_Screen(3, "FOLDER LOCATION", "Done");
#endif
                if (FunctionStatus.LauncherForceClose)
                {
                    FunctionStatus.ErrorCloseLauncher("Closing From Invalid Launcher Location", false);
                }
                else if (FunctionStatus.HasWriteAccessToFolder(Locations.LauncherFolder) == 0)
                {
                    FunctionStatus.LauncherForceClose = true;
                    FunctionStatus.LauncherForceCloseReason = Translations.Database("Program_TextBox_Folder_Write_Test");
                    FunctionStatus.ErrorCloseLauncher("Closing From No Write Access", false);
                }
                else
                {
                    if (Debugger.IsAttached)
                    {
                        LogToFileAddons.Parent_Log_Screen(1, "Debug Mode", "Enabled for Current Session");
                    }

                    Log.Start();
                    await Task.Run(() => Log_Location.RemoveLegacyLogs());

                    LogToFileAddons.Parent_Log_Screen(1, "CURRENT DATE", Time_Clock.GetTime(0));
                    LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER MIGRATION", "Appdata and/or Roaming Folders");
                    /* Deletes Folders that will Crash the Launcher (Cleanup Migration) */
                    await Task.Run(() =>
                    {
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
                            if (!Directory.Exists(Path.Combine(Locations.LauncherFolder, "Launcher_Data", "Archive", "Game Files")))
                            {
                                Directory.CreateDirectory(Path.Combine(Locations.LauncherFolder, "Launcher_Data", "Archive", "Game Files"));
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
#if !(RELEASE_UNIX || DEBUG_UNIX)
                            GC.Collect();
#endif
                        }
                    });

                    LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER XML", "If File Exists or Not");
                    Presence_Launcher.Status(0, "Checking if UserSettings XML Exists");
                    /* Create Default Configuration Files (if they don't already exist) */
                    await Task.Run(() =>
                    {
                        if (!File.Exists(Locations.UserSettingsXML))
                        {
                            try
                            {
                                if (!Directory.Exists(Locations.UserSettingsFolder))
                                {
                                    Directory.CreateDirectory(Locations.UserSettingsFolder);
                                }

                                File.WriteAllBytes(Locations.UserSettingsXML, Core.Extra.Conversion_.Embeded_Files.User_Settings_XML_Bytes());
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("LAUNCHER XML", string.Empty, Error, string.Empty, true);
                            }
                            finally
                            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                GC.Collect();
#endif
                            }
                        }

                        LogToFileAddons.Parent_Log_Screen(3, "LAUNCHER XML", "Done");
                    });

                    LogToFileAddons.Parent_Log_Screen(8,
                        EnableInsiderDeveloper.Allowed() ? "DEV TEST " : (EnableInsiderBetaTester.Allowed() ? "BETA TEST " : ""),
                        "SBRW.Launcher " + Application.ProductVersion + " - (" + InsiderInfo.BuildNumberOnly() + ")");

                    LogToFileAddons.Parent_Log_Screen(2, "OS", "Detecting");
                    Presence_Launcher.Status(0, "Checking Operating System");
                    await Task.Run(() =>
                    {
                        try
                        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                            LogToFileAddons.Parent_Log_Screen(7, "Detected OS", Launcher_Value.System_OS_Name = Product_Version.ConvertWindowsNumberToName());
                            LogToFileAddons.Parent_Log_Screen(7, "Windows Build", Product_Version.GetWindowsBuildNumber().ToString());
                            LogToFileAddons.Parent_Log_Screen(7, "NT Version", Environment.OSVersion.VersionString);
                            LogToFileAddons.Parent_Log_Screen(7, "Video Card", HardwareInfo.GPU.CardName());
                            LogToFileAddons.Parent_Log_Screen(7, "Driver Version", HardwareInfo.GPU.DriverVersion());
#else
                            LogToFileAddons.Parent_Log_Screen(7, "Detected OS", Launcher_Value.System_OS_Name = UnixOS.FullName());
                            LogToFileAddons.Parent_Log_Screen(7, "Wine Version", DLL_NTDLL.WineVersion());
                            LogToFileAddons.Parent_Log_Screen(7, "Wine Build ID", DLL_NTDLL.WineBuildId());
#endif
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
#if !(RELEASE_UNIX || DEBUG_UNIX)
                            GC.Collect();
#endif
                        }
                    });

                    if (FunctionStatus.LauncherForceClose)
                    {
                        FunctionStatus.ErrorCloseLauncher("Closing From Operating System Check", false);
                    }
                    else
                    {
                        if (FunctionStatus.HasWriteAccessToFolder(Locations.LauncherFolder) == 0)
                        {
                            FunctionStatus.LauncherForceClose = true;
                            FunctionStatus.LauncherForceCloseReason = Translations.Database("Program_TextBox_Folder_Write_Test");
                            FunctionStatus.ErrorCloseLauncher("Closing From No Write Access", false);
                        }
                        else
                        {
                            LogToFileAddons.Parent_Log_Screen(3, "WRITE TEST", "Passed");
#if !(RELEASE_UNIX || DEBUG_UNIX)
                            /* Location Migration */
                            LogToFileAddons.Parent_Log_Screen(2, "Account File Migration", "Doing Migration");
                            Presence_Launcher.Status(0, "Doing Ini File Migration");
                            await Task.Run(() =>
                            {
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
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                        GC.Collect();
#endif
                                    }
                                }
                                else
                                {
                                    LogToFileAddons.Parent_Log_Screen(3, "Account File Migration", "Already Migrated");
                                }
                            });

                            LogToFileAddons.Parent_Log_Screen(3, "Account File Migration", "Done");
#endif

                            if (FunctionStatus.LauncherForceClose)
                            {
                                ///@DavidCarbon or @Zacam - Remember to Translate This!
                                FunctionStatus.LauncherForceCloseReason = "Failed to Successfully Migrate Ini File(s)";
                                FunctionStatus.ErrorCloseLauncher("Closing Ini Migration", false, this);
                            }
                            else
                            {
                                LogToFileAddons.Parent_Log_Screen(1, "File Archive Path", "Checking Default Game Archive Locations");
                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        if (Hashes.Hash_SHA(InformationCache.Secondary_Game_Archive_Path()) == "88C886B6D131C052365C3D6D14E14F67A4E2C253")
                                        {
                                            Save_Settings.Live_Data.Game_Archive_Location = InformationCache.Secondary_Game_Archive_Path();
                                        }
                                        else if (Hashes.Hash_SHA(InformationCache.Secondary_Game_Archive_Path_Old()) == "88C886B6D131C052365C3D6D14E14F67A4E2C253")
                                        {
                                            Save_Settings.Live_Data.Game_Archive_Location = InformationCache.Secondary_Game_Archive_Path_Old();
                                        }
                                        else if (Hashes.Hash_SHA(InformationCache.Legacy_Game_Archive_Path()) == "88C886B6D131C052365C3D6D14E14F67A4E2C253")
                                        {
                                            Save_Settings.Live_Data.Game_Archive_Location = InformationCache.Legacy_Game_Archive_Path();
                                        }

                                        if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Archive_Location))
                                        {
                                            LogToFileAddons.Parent_Log_Screen(1, "File Archive Path", "Using Pre-downloaded File: " + Save_Settings.Live_Data.Game_Archive_Location);
                                        }
                                    }
                                    catch (Exception Error)
                                    {
                                        LogToFileAddons.OpenLog("File Archive Path", string.Empty, Error, string.Empty, true);
                                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                        {
                                            LogToFileAddons.Parent_Log_Screen(5, "File Archive Path", Error.InnerException.Message, false, true);
                                        }
                                    }
                                });
                                LogToFileAddons.Parent_Log_Screen(2, "INI FILES", "Doing Nullsafe");
                                Presence_Launcher.Status(0, "Doing NullSafe ini Files");
                                await Task.Run(() =>
                                {
                                    Save_Settings.NullSafe();
                                    Save_Account.NullSafe();
                                });
                                LogToFileAddons.Parent_Log_Screen(3, "INI FILES", "Done");
                                /* Sets up Theming */
                                LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER THEME", "Checking");
                                await Task.Run(() =>
                                {
                                    Theming.CheckIfThemeExists();
                                });
                                LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER THEME", "Done");

                                LogToFileAddons.Parent_Log_Screen(12, "APPLICATION", "Setting Language");
                                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(Translations.UI(Translations.Application_Language = Save_Settings.Live_Data.Launcher_Language.ToLower(), true));
                                LogToFileAddons.Parent_Log_Screen(3, "APPLICATION", "Done Setting Language '" + Translations.UI(Translations.Application_Language) + "'");

                                /* Windows 7 TLS Check */
                                if (Product_Version.GetWindowsNumber() == 6.1)
                                {
                                    LogToFileAddons.Parent_Log_Screen(2, "SSL/TLS", "Windows 7 Detected");
                                    Presence_Launcher.Status(0, "Checking Windows 7 SSL/TLS");

                                    try
                                    {
                                        string MessageBoxPopupTLS = string.Empty;

                                        await Task.Run(() =>
                                        {
                                            if (string.IsNullOrWhiteSpace(Registry_Core.Read("DisabledByDefault", @"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client")))
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
                                        });
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
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                        GC.Collect();
#endif
                                    }
                                }

                                /* Windows 7 HotFix Check */
                                if (Product_Version.GetWindowsNumber() == 6.1 && string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Win_7_Patches))
                                {
                                    LogToFileAddons.Parent_Log_Screen(2, "HotFixes", "Windows 7 Detected");
                                    Presence_Launcher.Status(0, "Checking Windows 7 HotFixes");

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
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                        GC.Collect();
#endif
                                    }
                                }

                                try
                                {
                                    LogToFileAddons.Parent_Log_Screen(2, "FOLDER", "Launcher Data Folder");

                                    await Task.Run(() =>
                                    {
                                        if (!Directory.Exists(Locations.LauncherDataFolder))
                                        {
                                            Directory.CreateDirectory(Locations.LauncherDataFolder);
                                        }
                                    });
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
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                    GC.Collect();
#endif
                                }

                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        LogToFileAddons.Parent_Log_Screen(2, "JSON", "Servers File");

                                        if (File.Exists(Path.Combine(Locations.LauncherFolder, Locations.NameOldServersJSON)))
                                        {
                                            if (File.Exists(Locations.LauncherCustomServers))
                                            {
                                                File.Delete(Locations.LauncherCustomServers);
                                            }

                                            File.Move(Path.Combine(Locations.LauncherFolder, Locations.NameOldServersJSON),
                                                Locations.LauncherCustomServers);

                                            LogToFileAddons.Parent_Log_Screen(3, "FOLDER", "Renaming Servers File");
                                        }
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                        else if (File.Exists(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON)))
                                        {
                                            File.Move(Path.Combine(Locations.LauncherFolder, Locations.NameNewServersJSON), Locations.LauncherCustomServers);
                                        }
#endif
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
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                        GC.Collect();
#endif
                                    }
                                });

                                if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                                {
                                    LogToFileAddons.Parent_Log_Screen(2, "CLEANLINKS", "Game Path");

                                    await Task.Run(() =>
                                    {
                                        if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)))
                                        {
                                            ModNetHandler.CleanLinks(Save_Settings.Live_Data.Game_Path);
                                            LogToFileAddons.Parent_Log_Screen(3, "CLEANLINKS", "Done");
                                        }
                                        else
                                        {
                                            LogToFileAddons.Parent_Log_Screen(3, "CLEANLINKS", "Not Present");
                                        }
                                    });
                                }

                                LogToFileAddons.Parent_Log_Screen(2, "PROXY", "Checking if Proxy Is Disabled from User Settings! It's value is " + Save_Settings.Live_Data.Launcher_Proxy);
                                LogToFileAddons.Parent_Log_Screen(2, "CLIENT", "Checking Alternative WebCalls, it's value is " + Save_Settings.Live_Data.Launcher_WebClient_Method);

                                LogToFileAddons.Parent_Log_Screen(2, "PRELOAD", "Headers");
                                await Task.Run(() => Custom_Header.Headers_WHC());
                                LogToFileAddons.Parent_Log_Screen(3, "PRELOAD", "Headers");
                                Presence_Launcher.Status(0, "Checking Root Certificate Authority");
                                await Task.Run(() => Certificate_Store.Latest());

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
        public static async void First_Time_Run()
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
            if (string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path) ||
                (Save_Settings.Live_Data.Launcher_CDN.Contains("http://localhost") && 
                !Save_Settings.Live_Data.Launcher_CDN.Contains(".")))
            {
                Presence_Launcher.Status(0, "Doing First Time Run");
                LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "First run!");

                try
                {
                    Form welcome = new Screen_Welcome();
                    DialogResult welcomereply = welcome.ShowDialog();

                    if (welcomereply != DialogResult.OK)
                    {
                        FunctionStatus.LauncherForceClose = true;
                    }
                }
                catch
                {
                    if (string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Launcher_CDN))
                    {
                        LogToFileAddons.Parent_Log_Screen(4, "LAUNCHER", "CDN Source URL was Empty! Setting a Null Safe URL 'http://localhost'");
                        Save_Settings.Live_Data.Launcher_CDN = "http://localhost";
                        Save_Settings.Save();
                    }

                    if (string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                    {
                        LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "Installation Directory was Empty! Creating and Setting Directory at " + Locations.GameFilesFailSafePath);
                        Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                        Save_Settings.Save();
                    }
                }

                if (FunctionStatus.LauncherForceClose)
                {
                    FunctionStatus.ErrorCloseLauncher("Closing From Welcome Dialog", false);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                    {
                        Presence_Launcher.Status(0, "User Selecting/Inputting Game Files Folder");

                        try
                        {
#if !(RELEASE_UNIX || DEBUG_UNIX)

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
                                    Save_Settings.Live_Data.Game_Path = Path.GetDirectoryName(FolderDialog.FileName) ?? string.Empty;
                                }
                            }

                            FolderDialog.Dispose();
#endif
                            if (string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                            {
                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        Save_Settings.Live_Data.Game_Path = Path.GetFullPath("GameFiles");
                                    }
                                    catch
                                    {
                                        Save_Settings.Live_Data.Game_Path = "GameFiles";
                                    }
                                });
                            }

                            await Task.Run(() =>
                            {
                                if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                                {
#pragma warning disable CS8604
                                    Save_Settings.Live_Data.Game_Path.IsRestrictedGameFolderLocation(0, Screen_Instance??null);
#pragma warning restore CS8604
                                }
                                else
                                {
                                    FunctionStatus.LauncherForceClose = true;
                                }
                            });

                            if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path))
                            {
                                LogToFileAddons.Parent_Log_Screen(2, "CLEANLINKS", "Game Path");
                                await Task.Run(() =>
                                {
                                    if (File.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, Locations.NameModLinks)))
                                    {
                                        ModNetHandler.CleanLinks(Save_Settings.Live_Data.Game_Path);
                                        LogToFileAddons.Parent_Log_Screen(3, "CLEANLINKS", "Done");
                                    }
                                    else
                                    {
                                        LogToFileAddons.Parent_Log_Screen(3, "CLEANLINKS", "Not Present");
                                    }
                                });
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
                            #if !(RELEASE_UNIX || DEBUG_UNIX) 
                            GC.Collect(); 
                            #endif
                        }
                    }
#pragma warning disable CS8604 // Possible null reference argument.
                    else if (Save_Settings.Live_Data.Game_Path.IsRestrictedGameFolderLocation(1, Screen_Instance??null))
                    {
                        LogToFileAddons.Parent_Log_Screen(12, "LAUNCHER", "Folder Check Trigger in 'FOLDER SELECT DIALOG'");
                    }
#pragma warning restore CS8604 // Possible null reference argument.
                }
            }
#pragma warning disable CS8604 // Possible null reference argument.
            else if (Save_Settings.Live_Data.Game_Path.IsRestrictedGameFolderLocation(1, Screen_Instance ?? null))
            {
                LogToFileAddons.Parent_Log_Screen(12, "LAUNCHER", "Folder Check Trigger");
            }
#pragma warning restore CS8604 // Possible null reference argument.

            LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "Game Installation Path Done");

            if (FunctionStatus.LauncherForceClose)
            {
                FunctionStatus.ErrorCloseLauncher("Closing From Folder Dialog", false);
            }
            else
            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER", "Checking Game Path Location");
                Presence_Launcher.Status(0, "Checking Game Files Folder Location");

                await Task.Run(() =>
                {
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
                                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                                GC.Collect(); 
                                #endif
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
                                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                                GC.Collect(); 
                                #endif
                            }
                            LogToFileAddons.Parent_Log_Screen(4, "LAUNCHER", "Installing NFSW in a Restricted Location is not allowed.", false, true);
                            MessageBox.Show(null, constructMsg, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Save_Settings.Live_Data.Game_Path = Locations.GameFilesFailSafePath;
                            break;
                    }
                    Save_Settings.Save();
                });

                LogToFileAddons.Parent_Log_Screen(11, "LAUNCHER", "Done Checking Game Path Location");
#endif

                /* Check If Launcher Failed to Connect to any APIs */
                if (!VisualsAPIChecker.Local_Cached_API())
                {
                    Presence_Launcher.Status(0, "Launcher Encountered API Errors");

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
                            Screen_Instance.Text = "SBRW Launcher: " + Application.ProductVersion;
                            Screen_Main Custom_Instance_Settings = new Screen_Main() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true, FormBorderStyle = FormBorderStyle.None };
                            Screen_Instance.Panel_Form_Screens.Visible = true;
                            Screen_Instance.Panel_Form_Screens.Controls.Add(Custom_Instance_Settings);
                            Custom_Instance_Settings.Show();
                            Screen_Instance.Panel_Splash_Screen.Visible = false;
                            Screen_Instance.Size = new Size(891, 529);
                            Screen_Instance.Position_Window_Set();
                            LogToFileAddons.Parent_Log_Screen(1, "MAINSCREEN", "Hello World!", true);
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
                        #if !(RELEASE_UNIX || DEBUG_UNIX) 
                        GC.Collect(); 
                        #endif
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
                if ((FunctionStatus.LoadingComplete || FunctionStatus.LauncherForceClose || Clock_Tick_Theme_Update) && (Screen_Instance != null))
                {
                    if (Clock.Enabled)
                    {
                        Screen_Instance.SafeInvokeAction(() => Screen_Instance.Clock.Stop(), this);
                    }
                }
                else if ((PictureBox_Screen_Splash.BackgroundImage != Image_Other.Logo_Splash) && (Screen_Instance != null))
                {
                    Clock_Tick_Theme_Update = true;
                    Button_Close.SafeInvokeAction(() => Button_Close.BackgroundImage = Image_Icon.Close, this);
                    PictureBox_Screen_Splash.SafeInvokeAction(() => PictureBox_Screen_Splash.BackgroundImage = Image_Other.Logo_Splash, this);
                }
                else
                {
                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                    GC.Collect(); 
                    #endif
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

            try
            {
                if (Screen_Main.Screen_Instance != null)
                {
                    if (Screen_Main.Screen_Instance.NotifyIcon_Notification.Visible)
                    {
                        Screen_Main.Screen_Instance.NotifyIcon_Notification.Visible = false;
                        Screen_Main.Screen_Instance.NotifyIcon_Notification.Dispose();
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Notification Disposal", string.Empty, Error, string.Empty, true);
            }

            Log_Verify.Stop = Log.Stop = true;
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

            PictureBox_Screen_Splash.MouseMove += new MouseEventHandler(Move_Window_Mouse_Move);
            PictureBox_Screen_Splash.MouseUp += new MouseEventHandler(Move_Window_Mouse_Up);
            PictureBox_Screen_Splash.MouseDown += new MouseEventHandler(Move_Window_Mouse_Down);

            Load += new EventHandler(Parent_Screen_Load);
            Shown += new EventHandler(Parent_Screen_Shown);
            Clock.Tick += new EventHandler(Clock_Tick);

            Button_Close.MouseEnter += new EventHandler(ButtonClose_MouseEnter);
            Button_Close.MouseLeave += new EventHandler(ButtonClose_MouseLeaveANDMouseUp);
            Button_Close.MouseUp += new MouseEventHandler(ButtonClose_MouseLeaveANDMouseUp);
            Button_Close.MouseDown += new MouseEventHandler(ButtonClose_MouseDown);
            Button_Close.Click += new EventHandler(Button_Close_Click);
            #endregion

            #region Custom Theme
            /*******************************/
            /* Set Font                     /
            /*******************************/
#if !(RELEASE_UNIX || DEBUG_UNIX)
            float MainFontSize = 9f * 96f / CreateGraphics().DpiY;
#else
            float MainFontSize = 9f;
#endif

            Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            TextBox_Live_Log.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            GroupBox_Launcherlog.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            TransparencyKey = Color_Screen.BG_Splash;
            BackgroundImage = Image_Background.Splash;

            Button_Close.BackgroundImage = Image_Icon.Close;
            PictureBox_Screen_Splash.BackgroundImage = Image_Other.Logo_Splash;

            ForeColor = Color_Winform.Text_Fore_Color;
            BackColor = Color_Winform.BG_Fore_Color;

            GroupBox_Launcherlog.ForeColor = Color_Winform.Text_Fore_Color;
            TextBox_Live_Log.ForeColor = Color_Winform.Secondary_Text_Fore_Color;
            TextBox_Live_Log.BackColor = Color_Winform.BG_Darker_Fore_Color;

            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Icon = FormsIcon.Retrive_Icon();
            Text = "SBRW Launcher: " + Application.ProductVersion;

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

                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            };
#endregion

#region Update Variables
            Screen_Instance = this;
#endregion
        }
    }
}
