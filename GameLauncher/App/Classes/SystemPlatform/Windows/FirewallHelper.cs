using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsFirewallHelper;
using WindowsFirewallHelper.Exceptions;
using WindowsFirewallHelper.FirewallRules;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class FirewallHelper
    {
        public static void DoesRulesExist(string Type, string Mode, string AppName, string AppPath, string groupKey, string description, FirewallProtocol protocol)
        {
            if (!DetectLinux.LinuxDetected())
            {
                if (FirewallManager.IsServiceRunning)
                {
                    if (FirewallStatus())
                    {
                        CheckIfRuleExists(Type, Mode, AppName, AppPath, groupKey, description, protocol);
                    }
                    else
                    {
                        Log.Warning("WINDOWS FIREWALL: Turned Off [Not by Launcher]");
                    }
                }
                else if (!FirewallManager.IsServiceRunning)
                {
                    Log.Warning("WINDOWS FIREWALL: Service is Stopped [Not by Launcher]");
                }
            }
            else if (DetectLinux.LinuxDetected())
            {
                Log.Warning("WINDOWS FIREWALL: Not Supported On Linux");
            }
            else
            {
                Log.Error("WINDOWS FIREWALL: Unknown Error Had Occured -> Check System Software");
            }
        }

        public static void CheckIfRuleExists(string Type, string Mode, string AppName, string AppPath, string groupKey, string description, FirewallProtocol protocol)
        {
            if (RuleExist("Path", AppName, AppPath) == true && RuleExist("Name", AppName, AppPath) == false)
            {
                /* Inbound & Outbound */
                RemoveRules(Type, Mode, "Non-" + AppName, AppPath, "Path Match");
            }

            if (RuleExist(Mode, AppName, AppPath) == false)
            {
                /* Add Firewall Rules */
                if (Type == "Add-Game" || Type == "Add-Launcher")
                {
                    /* Inbound */
                    AddApplicationRule(Type, AppName, AppPath, groupKey, description, FirewallDirection.Inbound, protocol, FirewallDirection.Inbound.ToString());
                    /* Outbound */
                    AddApplicationRule(Type, AppName, AppPath, groupKey, description, FirewallDirection.Outbound, protocol, FirewallDirection.Outbound.ToString());
                }
                else
                {
                    Log.Info("WINDOWS FIREWALL: Unknown Mode State in 'CheckIfRuleExists' Function");
                }
            }
            else if (RuleExist(Mode, AppName, AppPath) == true)
            {
                /* Remove Firewall Rules */
                if (Type == "Reset")
                {
                    /* Inbound & Outbound */
                    RemoveRules(Type, Mode, AppName, AppPath, "Path Match");
                }
                else
                {
                    Log.Info("WINDOWS FIREWALL: Already Exlcuded " + AppName);
                }
            }
            else
            {
                Log.Error("WINDOWS FIREWALL: Firewall Error - Check With Visual Studio for Error Debugging and Code Review");
            }
        }

        public static void AddApplicationRule(string Type, string AppName, string AppPath, string groupKey, string description, FirewallDirection direction, FirewallProtocol protocol, string firewallLogNote)
        {
            bool ErrorFree = true;
            try
            {
                Log.Info("WINDOWS FIREWALL: Supported Firewall Found");
                var rule = new FirewallWASRuleWin8(AppPath, FirewallAction.Allow, direction, FirewallProfiles.Domain | FirewallProfiles.Private | FirewallProfiles.Public)
                {
                    ApplicationName = AppPath,
                    Name = AppName,
                    Grouping = groupKey,
                    Description = description,
                    NetworkInterfaceTypes = NetworkInterfaceTypes.Lan | NetworkInterfaceTypes.RemoteAccess | NetworkInterfaceTypes.Wireless,
                    Protocol = protocol
                };

                if (direction == FirewallDirection.Inbound)
                {
                    rule.EdgeTraversalOptions = EdgeTraversalAction.Allow;
                }

                FirewallManager.Instance.Rules.Add(rule);

                if (Type == "Add-Game")
                {
                    FileSettingsSave.FirewallGameStatus = "Excluded";
                }
                else if (Type == "Add-Launcher")
                {
                    FileSettingsSave.FirewallLauncherStatus = "Excluded";
                }

                FileSettingsSave.SaveSettings();

                Log.Info("WINDOWS FIREWALL: Finished Adding " + AppName + " to Firewall! {" + firewallLogNote + "}");
            }
            catch (FirewallWASNotSupportedException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
                AddDefaultApplicationRule(Type, AppName, AppPath, direction, protocol, firewallLogNote);
                ErrorFree = false;
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
                AddDefaultApplicationRule(Type, AppName, AppPath, direction, protocol, firewallLogNote);
                ErrorFree = false;
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
                AddDefaultApplicationRule(Type, AppName, AppPath, direction, protocol, firewallLogNote);
                ErrorFree = false;
            }

            if (ErrorFree)
            {
                if (Type == "Add-Game")
                {
                    FileSettingsSave.FirewallGameStatus = (ErrorFree == true) ? "Excluded" : "Error";
                }
                else if (Type == "Add-Launcher")
                {
                    FileSettingsSave.FirewallLauncherStatus = (ErrorFree == true) ? "Excluded" : "Error";
                }

                FileSettingsSave.SaveSettings();
            }
        }

        private static void AddDefaultApplicationRule(string Type, string AppName, string AppPath, FirewallDirection direction, FirewallProtocol protocol, string firewallLogNote)
        {
            bool ErrorFree = true;

            try
            {
                Log.Warning("WINDOWS FIREWALL: Falling back to 'LegacyStandard'");
                var defaultRule = FirewallManager.Instance.CreateApplicationRule(
                    FirewallProfiles.Domain | FirewallProfiles.Private | FirewallProfiles.Public,
                    AppName, FirewallAction.Allow, AppPath, protocol);

                defaultRule.Direction = direction;

                FirewallManager.Instance.Rules.Add(defaultRule);

                Log.Warning("WINDOWS FIREWALL: Finished Adding " + AppName + " to Firewall! {" + firewallLogNote + "}");
            }
            catch (FirewallWASNotSupportedException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
                ErrorFree = false;
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
                ErrorFree = false;
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL ", null, Error, null, true);
                ErrorFree = false;
            }

            if (Type == "Add-Game")
            {
                FileSettingsSave.FirewallGameStatus = (ErrorFree == true) ? "Excluded" : "Error";
            }
            else if (Type == "Add-Launcher")
            {
                FileSettingsSave.FirewallLauncherStatus = (ErrorFree == true) ? "Excluded" : "Error";
            }

            FileSettingsSave.SaveSettings();
        }

        public static void RemoveRules(string Type, string Mode, string AppName, string AppPath, string firewallLogNote)
        {
            bool ErrorFree = true;
            var myRule = FindRules(Mode, AppName, AppPath).ToArray();

            if (Enumerable.Any(myRule))
            {
                foreach (var rule in myRule)
                {
                    try
                    {
                        FirewallManager.Instance.Rules.Remove(rule);
                        Log.Warning("WINDOWS FIREWALL: Removed " + AppName + " {" + firewallLogNote + "} From Firewall!");
                    }
                    catch (Exception Error)
                    {
                        ErrorFree = false;
                        LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
                    }
                }
            }

            if (Type == "Add-Game")
            {
                FileSettingsSave.FirewallGameStatus = (ErrorFree == true) ? "Reset" : "Removal Error";
            }
            else if (Type == "Add-Launcher")
            {
                FileSettingsSave.FirewallLauncherStatus = (ErrorFree == true) ? "Reset" : "Removal Error";
            }

            FileSettingsSave.SaveSettings();
        }

        public static bool RuleExist(string Mode, string Name, string Path)
        {
            if (DetectLinux.LinuxDetected())
            {
                return true;
            }
            else
            {
                return FindRules(Mode, Name, Path).Any();
            }
        }

        public static IEnumerable<IFirewallRule> FindRules(string Mode, string AppName, string AppPath)
        {
            try
            {
                if (FirewallWAS.IsSupported && FirewallWASRuleWin7.IsSupported)
                {
                    if (Mode == "Name")
                    {
                        return FirewallManager.Instance.Rules.Where(r => string.Equals(r.Name, AppName, StringComparison.OrdinalIgnoreCase)).ToArray();
                    }
                    else if (Mode == "Path")
                    {
                        return FirewallManager.Instance.Rules.Where(r => string.Equals(r.ApplicationName, AppPath, StringComparison.OrdinalIgnoreCase)).ToArray();
                    }
                    else
                    {
                        return Enumerable.Empty<IFirewallRule>();
                    }
                }
                else
                {
                    return Enumerable.Empty<IFirewallRule>();
                }
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
                return Enumerable.Empty<IFirewallRule>();
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
                return Enumerable.Empty<IFirewallRule>();
            }
        }

        /* Checks if Windows Firewall is Enabled or not from a System Level */
        public static bool FirewallStatus()
        {
            if (DetectLinux.LinuxDetected())
            {
                return false;
            }
            else
            {
                try
                {
                    Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
                    INetFwMgr mgr = (INetFwMgr)Activator.CreateInstance(NetFwMgrType);

                    return mgr.LocalPolicy.CurrentProfile.FirewallEnabled;
                }
                catch (COMException Error)
                {
                    LogToFileAddons.OpenLog("WINDOWS FIREWALL Check", null, Error, null, true);
                    return false;
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("WINDOWS FIREWALL Check", null, Error, null, true);
                    return false;
                }
            }
        }
    }

    class FirewallFunctions
    {
        public static void GameFiles()
        {
            if (!DetectLinux.LinuxDetected())
            {
                try
                {
                    /* First Time Run */
                    if (FirewallManager.IsServiceRunning && FirewallHelper.FirewallStatus())
                    {
                        string GameName = "SBRW - Game";
                        string GamePath = Strings.Encode(Path.Combine(FileSettingsSave.GameInstallation, "nfsw.exe"));

                        string groupKeyGame = "Need for Speed: World";
                        string descriptionGame = groupKeyGame;

                        /* Inbound & Outbound */
                        FirewallHelper.DoesRulesExist("Add-Game", "Path", GameName, GamePath, groupKeyGame, descriptionGame, FirewallProtocol.Any);
                    }
                    else if (FirewallManager.IsServiceRunning && !FirewallHelper.FirewallStatus())
                    {
                        FileSettingsSave.FirewallGameStatus = "Turned Off";
                    }
                    else
                    {
                        FileSettingsSave.FirewallGameStatus = "Service Stopped";
                    }
                }
                catch (COMException Error)
                {
                    LogToFileAddons.OpenLog("FIREWALL", null, Error, null, true);
                    FileSettingsSave.FirewallGameStatus = "Error";
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("FIREWALL", null, Error, null, true);
                    FileSettingsSave.FirewallGameStatus = "Error";
                }
            }
            else
            {
                FileSettingsSave.FirewallGameStatus = "Not Supported";
            }

            FileSettingsSave.SaveSettings();
        }

        public static void Launcher()
        {
            if (!DetectLinux.LinuxDetected())
            {
                try
                {
                    DiscordLauncherPresense.Status("Start Up", "Checking Firewall Exclusions");

                    if (FirewallManager.IsServiceRunning && FirewallHelper.FirewallStatus())
                    {
                        string LauncherName = "SBRW - Game Launcher";
                        string LauncherPath = Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameLauncher));

                        string UpdaterName = "SBRW - Game Launcher Updater";
                        string UpdaterPath = Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameUpdater));

                        string groupKeyLauncher = "Game Launcher for Windows";
                        string descriptionLauncher = "Soapbox Race World";

                        /* Inbound & Outbound */
                        FirewallHelper.DoesRulesExist("Add-Launcher", "Path", LauncherName, LauncherPath, groupKeyLauncher, descriptionLauncher, FirewallProtocol.Any);
                        FirewallHelper.DoesRulesExist("Add-Launcher", "Path", UpdaterName, UpdaterPath, groupKeyLauncher, descriptionLauncher, FirewallProtocol.Any);
                    }
                    else if (FirewallManager.IsServiceRunning && !FirewallHelper.FirewallStatus())
                    {
                        FileSettingsSave.FirewallLauncherStatus = "Turned Off";
                    }
                    else
                    {
                        FileSettingsSave.FirewallLauncherStatus = "Service Stopped";
                    }
                }
                catch (COMException Error)
                {
                    LogToFileAddons.OpenLog("FIREWALL", null, Error, null, true);
                    FileSettingsSave.FirewallLauncherStatus = "Error";
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("FIREWALL", null, Error, null, true);
                    FileSettingsSave.FirewallLauncherStatus = "Error";
                }
            }
            else
            {
                FileSettingsSave.FirewallLauncherStatus = "Not Supported";
            }

            FileSettingsSave.SaveSettings();
        }
    }
}
