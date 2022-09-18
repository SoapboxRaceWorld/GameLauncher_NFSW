using SBRW.Launcher.RunTime.InsiderKit;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.Support;
using SBRW.Launcher.RunTime.SystemPlatform.Unix;
using SBRW.Launcher.App.UI_Forms.Main_Screen;
using SBRW.Launcher.App.UI_Forms.Settings_Screen;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Required.System.Windows_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;
using WindowsFirewallHelper;
using WindowsFirewallHelper.Exceptions;
using WindowsFirewallHelper.FirewallRules;
using System.Threading.Tasks;

namespace SBRW.Launcher.App.UI_Forms.SecurityCenter_Screen
{
    public partial class Screen_Security_Center : Form
    {
        ///<summary>Windows 10: Caches Old Game Path in the event of the user does Firewall First</summary>
        private static string CacheOldGameLocation { get; set; } = Save_Settings.Live_Data.Game_Path_Old;
        ///<summary>Disable Button: Firewall Rules API</summary>
        private static bool DisableButtonFRAPI { get; set; }
        ///<summary>Disable Button: Firewall Rules Check</summary>
        private static bool DisableButtonFRC { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Add All</summary>
        private static bool DisableButtonFRAA { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Add Launcher</summary>
        private static bool DisableButtonFRAL { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Add Game</summary>
        private static bool DisableButtonFRAG { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Remove All</summary>
        private static bool DisableButtonFRRA { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Remove Launcher</summary>
        private static bool DisableButtonFRRL { get; set; } = true;
        ///<summary>Disable Button: Firewall Rules Remove Game</summary>
        private static bool DisableButtonFRRG { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion API</summary>
        private static bool DisableButtonDRAPI { get; set; }
        ///<summary>Disable Button: Defender Exclusion Check</summary>
        private static bool DisableButtonDRC { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Add All</summary>
        private static bool DisableButtonDRAA { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Add Launcher</summary>
        private static bool DisableButtonDRAL { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Add Game</summary>
        private static bool DisableButtonDRAG { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Remove All</summary>
        private static bool DisableButtonDRRA { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Remove Launcher</summary>
        private static bool DisableButtonDRRL { get; set; } = true;
        ///<summary>Disable Button: Defender Exclusion Remove Game</summary>
        private static bool DisableButtonDRRG { get; set; } = true;
        ///<summary>Disable Button: Permission Check</summary>
        private static bool DisableButtonPRC { get; set; }
        ///<summary>Disable Button: Permission Set</summary>
        private static bool DisableButtonPRAA { get; set; } = true;
        ///<summary>RPC: Which State to do once Form Closes</summary>
        public static string RPCStateCache { get; set; } = string.Empty;

        /// <summary>
        /// Sets the Color for Buttons
        /// </summary>
        /// <param name="Elements">Button Control Name</param>
        /// <param name="Color">Range 0-3 Sets Colored Button.
        /// <code>"0" Checking Blue</code><code>"1" Success Green</code><code>"2" Warning Orange</code><code>"3" Error Red</code></param>
        /// <param name="EnabledORDisabled">Enables or Disables the Button</param>
        /// <remarks>Range 0-3 Sets Colored Button.
        /// <code>"0" Checking Blue</code><code>"1" Success Green</code><code>"2" Warning Orange</code><code>"3" Error Red</code></remarks>
        private void ButtonsColorSet(Button Elements, int Color, bool EnabledORDisabled)
        {
            switch (Color)
            {
                /* Checking Blue */
                case 0:
                    Elements.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
                    Elements.BackColor = Color_Winform_Buttons.Blue_Back_Color;
                    Elements.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
                    Elements.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;
                    Elements.Enabled = EnabledORDisabled;
                    break;
                /* Success Green */
                case 1:
                    Elements.ForeColor = Color_Winform_Buttons.Green_Fore_Color;
                    Elements.BackColor = Color_Winform_Buttons.Green_Back_Color;
                    Elements.FlatAppearance.BorderColor = Color_Winform_Buttons.Green_Border_Color;
                    Elements.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Green_Mouse_Over_Back_Color;
                    Elements.Enabled = EnabledORDisabled;
                    break;
                /* Warning Orange */
                case 2:
                    Elements.ForeColor = Color_Winform_Buttons.Yellow_Fore_Color;
                    Elements.BackColor = Color_Winform_Buttons.Yellow_Back_Color;
                    Elements.FlatAppearance.BorderColor = Color_Winform_Buttons.Yellow_Border_Color;
                    Elements.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Yellow_Mouse_Over_Back_Color;
                    Elements.Enabled = EnabledORDisabled;
                    break;
                /* Error Red */
                case 3:
                    Elements.ForeColor = Color_Winform_Buttons.Red_Fore_Color;
                    Elements.BackColor = Color_Winform_Buttons.Red_Back_Color;
                    Elements.FlatAppearance.BorderColor = Color_Winform_Buttons.Red_Border_Color;
                    Elements.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Red_Mouse_Over_Back_Color;
                    Elements.Enabled = EnabledORDisabled;
                    break;
                /* Unknown Gray */
                default:
                    Elements.ForeColor = Color_Winform_Buttons.Gray_Fore_Color;
                    Elements.BackColor = Color_Winform_Buttons.Gray_Back_Color;
                    Elements.FlatAppearance.BorderColor = Color_Winform_Buttons.Gray_Border_Color;
                    Elements.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Gray_Mouse_Over_Back_Color;
                    Elements.Enabled = !EnabledORDisabled;
                    break;
            }
        }
        /// <summary>Checks WMI Query on if Windows Defender is Enabled</summary>
        /// <param name="Query">Query a Specific Collection</param>
        /// <returns><code>True or False</code></returns>
        private bool GetDefenderStatus(string Query)
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
            if (Product_Version.GetWindowsNumber() >= 10)
            {
                ManagementObjectSearcher? ObjectPath = null;
                ManagementObjectCollection? ObjectCollection = null;

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
                    LogToFileAddons.OpenLog("Windows Defender Status [M.E.]", string.Empty, Error, string.Empty, true);
                }
                catch (COMException Error)
                {
                    LogToFileAddons.OpenLog("Windows Defender Status [C.O.M.]", string.Empty, Error, string.Empty, true);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Windows Defender Status", string.Empty, Error, string.Empty, true);
                }
                finally
                {
                    if (ObjectPath != null) { ObjectPath.Dispose(); }
                    if (ObjectCollection != null) { ObjectCollection.Dispose(); }
                }
            }
#endif
            return false;
        }
        /// <summary>Checks Windows Defender on if it's Enabled or Disabled by the User or Third-Party Program</summary>
        /// <remarks>Doesn't checks If the Service is Disabled or a Third-Party Program reports an Incorrect Value</remarks>
        /// <returns><code>True or False</code></returns>
        public bool Defender()
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
            return GetDefenderStatus("AntivirusEnabled") &&
                    GetDefenderStatus("AntispywareEnabled") &&
                    GetDefenderStatus("RealTimeProtectionEnabled");
#else
            return false;
#endif
        }
        /// <summary>Windows Defender: Checks Defender's Current Exclusion List</summary>
        /// <returns>String-Array of Exclusions</returns>
        private string[] ExclusionCheck()
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                ManagementObjectSearcher? ObjectPath = null;
                ManagementObjectCollection? ObjectCollection = null;

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
                    LogToFileAddons.OpenLog("Windows Defender Exclusion Path Check [M.E.]", string.Empty, Error, string.Empty, true);
                }
                catch (COMException Error)
                {
                    LogToFileAddons.OpenLog("Windows Defender Exclusion Path Check [C.O.M.]", string.Empty, Error, string.Empty, true);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Windows Defender Exclusion Path Check", string.Empty, Error, string.Empty, true);
                }
                finally
                {
                    if (ObjectPath != null) { ObjectPath.Dispose(); }
                    if (ObjectCollection != null) { ObjectCollection.Dispose(); }
                }
#endif

            return Array.Empty<string>();
        }
        /// <summary>
        /// Finds a Defender Exclusion in Defenders "Database"
        /// </summary>
        /// <param name="FilePath">Enter file Path</param>
        /// <returns><code>True or False</code></returns>
        private bool ExclusionExist(string FilePath)
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
            if (ExclusionCheck() != null)
            {
                return ExclusionCheck().Any(FilePath.Contains);
                /*
                foreach (string ExistingPaths in ExclusionCheck())
                {
                    if (ExistingPaths == FilePath)
                    {
                        return true;
                    }
                }

                return false; */
            }
            else
            {
                return false;
            }
#else
            return true;
#endif
        }
        /// <summary>Windows Defender: Add an Exclusion</summary>
        /// <param name="AppName">Enter the name of the Application</param>
        /// <param name="AppPath">Enter the Application Folder</param>
        /// <returns><code>True or False</code></returns>
        private bool AddApplicationExclusion(string AppName, string AppPath)
        {
            bool Completed = false;
            try
            {
                if (!ExclusionExist(AppPath))
                {
                    /* Remove current Exclusion and Add new location for Exclusion (Game Files Only!) */
                    using (PowerShell AddScript = PowerShell.Create())
                    {
                        AddScript.AddScript($"Add-MpPreference -ExclusionPath \"{AppPath}\"");
                        AddScript.Invoke();
                    }

                    Completed = true;
                    Log.Completed("Windows Defender: ".ToUpper() + "Folder is now Excluded. -> " + AppPath);
                }
                else { Log.Completed("WINDOWS FIREWALL: " + AppName + " Rule is already Added"); Completed = true; }
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Add Script [C.O.M.]", string.Empty, Error, string.Empty, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Add Script", string.Empty, Error, string.Empty, true);
            }

            return Completed;
        }
        /// <summary>Windows Defender: Removes an Exclusion</summary>
        /// <param name="AppName">Enter the name of the Application</param>
        /// <param name="AppPath">Enter the Application Folder</param>
        /// <returns><code>True or False</code></returns>
        private bool RemoveExclusion(string AppName, string AppPath)
        {
            bool Completed = false;
            try
            {
                if (ExclusionExist(AppPath))
                {
                    /* Remove current Exclusion and Add new location for Exclusion (Game Files Only!) */
                    using (PowerShell RemovalScript = PowerShell.Create())
                    {
                        RemovalScript.AddScript($"Remove-MpPreference -ExclusionPath \"{AppPath}\"");
                        RemovalScript.Invoke();
                    }

                    Completed = true;
                    Log.Completed("Windows Defender: ".ToUpper() + "Folder is no longer Excluded. -> " + AppPath);
                }
                else { Log.Completed("WINDOWS FIREWALL: " + AppName + " Rule is already Removed"); Completed = true; }
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Removal Script [C.O.M.]", string.Empty, Error, string.Empty, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Removal Script", string.Empty, Error, string.Empty, true);
            }

            return Completed;
        }
        /// <summary>
        /// Checks the Firewall API Version Dynamically
        /// </summary>
        /// <returns>Firewall API Version</returns>
        private FirewallAPIVersion FirewallAPI()
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
            try 
            { 
                return FirewallManager.Version; 
            }
            catch
            { 
                return FirewallAPIVersion.None; 
            }
#else
            return FirewallAPIVersion.None;
#endif
        }
        /// <summary>
        /// Checks the Firewall API Version against Versions that isn't supported
        /// </summary>
        /// <returns><code>True or False</code></returns>
        private bool FirewallSupported()
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
            return FirewallAPI() != FirewallAPIVersion.None;
#else
            return false;
#endif
        }
        /// <summary>
        /// Checks Windows Firewall on if it's Enabled or Disabled by the User or Third-Party Program
        /// </summary>
        /// <remarks>Checks the Firewall Service at the same time</remarks>
        /// <returns><code>True or False</code></returns>
        private bool Firewall()
        {
            try
            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                if (bool.TryParse(FirewallManager.IsServiceRunning.ToString(), out bool Service_Result) && Service_Result)
                {
                    if (bool.TryParse(FirewallManager.Instance.GetActiveProfile().IsActive.ToString(), out bool Profile_Result))
                    {
                        return Profile_Result;
                    }
                    /* @DavidCarbon Remember to Debug This!
                    Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", true);
                    INetFwMgr Mana = (INetFwMgr)Activator.CreateInstance(NetFwMgrType);

                    if (bool.TryParse(Mana.LocalPolicy.CurrentProfile.FirewallEnabled.ToString(), out bool Results))
                    {
                        return Mana.LocalPolicy.CurrentProfile.FirewallEnabled;
                    }
                    */
                }
#endif
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Check", string.Empty, Error, string.Empty, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Check", string.Empty, Error, string.Empty, true);
            }

            return false;
        }
        /// <summary>
        /// Used to find a Application Rule on the system by searching Firewall "Database"
        /// </summary>
        /// <param name="Mode">Used to Specifiy how to find a Rule. Enter "Name" or "Path"</param>
        /// <param name="AppName">Used to Specifiy how to find a Rule. Provide the name of Application</param>
        /// <param name="AppPath">Used to Specifiy how to find a Rule. Provide Application Path</param>
        /// <returns>An Array of Rules</returns>
        private IEnumerable<IFirewallRule> FindRules(string Mode, string AppName, string AppPath)
        {
            try
            {
                if (Firewall() && FirewallSupported() && (FirewallAPI() != FirewallAPIVersion.None))
                {
                    if (FirewallManager.Instance.Rules.Count != 0)
                    {
                        if (Mode == "Name")
                        {
                            return FirewallManager.Instance.Rules.Where(r =>
                            string.Equals(r.Name, AppName, StringComparison.OrdinalIgnoreCase)).ToArray();
                        }
                        else if (Mode == "Path")
                        {
                            return FirewallManager.Instance.Rules.Where(r =>
                            string.Equals(r.ApplicationName, AppPath, StringComparison.OrdinalIgnoreCase)).ToArray();
                        }
                    }
                }
            }
            catch (NotSupportedException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL [Not Supported]", string.Empty, Error, string.Empty, true);
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL [COM]", string.Empty, Error, string.Empty, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", string.Empty, Error, string.Empty, true);
            }

            return Enumerable.Empty<IFirewallRule>();
        }
        /// <summary>
        /// Finds a Firewall Rule and Attempts to Remove it. 
        /// If the Rule List is empty or Encounters an Issue, it will be False.
        /// If the rule list succeeds to remove a Rule, it will be True
        /// </summary>
        /// <param name="Mode"> Used in Find Rules Helper Function. Choose and Type "Name" or "Path"</param>
        /// <param name="AppName">Used in Find Rules Helper Function. Enter name of Application</param>
        /// <param name="AppPath">Used in Find Rules Helper Function. Enter file Path</param>
        /// <param name="F_LogNote">Used to Log which additional Details</param>
        /// <returns><code>True or False</code></returns>
        private bool RemoveRules(string Mode, string AppName, string AppPath, string F_LogNote)
        {
            try
            {
                var myRule = FindRules(Mode, AppName, AppPath).ToArray();

                if (myRule != null)
                {
                    if (Enumerable.Any(myRule))
                    {
                        int ErrorsRate = 0;

                        foreach (var rule in myRule)
                        {
                            try
                            {
                                FirewallManager.Instance.Rules.Remove(rule);
                                Log.Warning("WINDOWS FIREWALL: Removed " + AppName + " {" + F_LogNote + "} From Firewall!");
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("WINDOWS FIREWALL", string.Empty, Error, string.Empty, true);
                                ErrorsRate++;
                            }
                        }

                        if (ErrorsRate == 0) { return true; }
                    }
                }
            }
            catch { }

            return false;
        }
        /// <summary>
        /// Finds a Firewall Rule in Firewall "Database"
        /// </summary>
        /// <param name="AppName">Used in Find Rules Helper Function. Enter name of Application</param>
        /// <param name="AppPath">Used in Find Rules Helper Function. Enter file Path</param>
        /// <returns><code>True or False</code></returns>
        private bool RuleExist(string Mode, string AppName, string AppPath)
        {
#if !(RELEASE_UNIX || DEBUG_UNIX)
            if (FindRules(Mode, AppName, AppPath) != null) 
            {
                foreach (IFirewallRule Single_Rule in FindRules(Mode, AppName, AppPath))
                {
                    if (Single_Rule != null)
                    {
                        if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                        {
                            Log.Ignore("---Firewall---");
                            Log.Debug("Name: " + Single_Rule.Name);
                            Log.Debug("Friendly Name: " + Single_Rule.FriendlyName);
                            Log.Debug("Application Name: " + Single_Rule.ApplicationName);
                            Log.Debug("Direction: " + Single_Rule.Direction);
                            Log.Debug("Scope: " + Single_Rule.Scope);
                            Log.Debug("Enabled: " + Single_Rule.IsEnable);
                            Log.Ignore("------End------");
                        }

                        if (Single_Rule.Name.Equals(AppName) && Single_Rule.ApplicationName.Equals(AppPath))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            else 
            { 
                return false; 
            }
#else
            return true;
#endif
        }
        /// <summary>Windows Firewall: Adds an Exclusion</summary>
        /// <param name="AppName">Enter the name of the Application</param>
        /// <param name="AppPath">Enter the Application Path (Must include exe)</param>
        /// <param name="GroupKey">Sets the rule grouping string</param>
        /// <param name="C_Description">Sets the description string of this rule</param>
        /// <param name="C_Direction">Data direction in which this rule applies to</param>
        /// <param name="C_Protocol">Sets the protocol that the rule applies to</param>
        /// <param name="F_LogNote">Notes for Logging</param>
        /// <returns><code>True or False</code></returns>
        private bool AddApplicationRule(string AppName, string AppPath, string GroupKey, string C_Description,
                            FirewallDirection C_Direction, FirewallProtocol C_Protocol, string F_LogNote)
        {
            bool Completed = false;
            try
            {
                FirewallAPIVersion CachedAPIVersion = FirewallAPI();
                if (CachedAPIVersion == FirewallAPIVersion.None)
                {
                    Log.Warning("WINDOWS FIREWALL: API Version not Supported");
                }
                else if (CachedAPIVersion != FirewallAPIVersion.FirewallLegacy)
                {
                    Log.Info("WINDOWS FIREWALL: Supported Firewall [WASRuleWin8]");
                    FirewallWASRuleWin8 Rule = new FirewallWASRuleWin8(AppPath, FirewallAction.Allow, C_Direction,
                        FirewallProfiles.Domain | FirewallProfiles.Private | FirewallProfiles.Public)
                    {
                        ApplicationName = AppPath,
                        Name = AppName,
                        Grouping = GroupKey,
                        Description = C_Description,
                        NetworkInterfaceTypes = NetworkInterfaceTypes.Lan | NetworkInterfaceTypes.RemoteAccess | NetworkInterfaceTypes.Wireless,
                        Protocol = C_Protocol
                    };

                    if (C_Direction == FirewallDirection.Inbound)
                    {
                        Rule.EdgeTraversalOptions = EdgeTraversalAction.Allow;
                    }

                    FirewallManager.Instance.Rules.Add(Rule);
                    Log.Completed("WINDOWS FIREWALL: Finished Adding " + AppName + " to Firewall! {" + F_LogNote + "}");
                    Completed = true;
                }
                else if (CachedAPIVersion == FirewallAPIVersion.FirewallLegacy)
                {
                    Log.Info("WINDOWS FIREWALL: Supported Firewall [LegacyStandard]");
                    IFirewallRule Rule = FirewallManager.Instance.CreateApplicationRule(
                        FirewallProfiles.Domain | FirewallProfiles.Private | FirewallProfiles.Public,
                        AppName, FirewallAction.Allow, AppPath, C_Protocol);
                    Rule.Direction = C_Direction;

                    FirewallManager.Instance.Rules.Add(Rule);
                    Log.Completed("WINDOWS FIREWALL: Finished Adding " + AppName + " to Firewall! {" + F_LogNote + "}");
                    Completed = true;
                }
                else
                {
                    Log.Completed("WINDOWS FIREWALL: " + AppName + " Rule was not added due to Firewall API Version {" + F_LogNote + "}");
                }
            }
            catch (FirewallWASNotSupportedException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL [F.WAS.N.S.E]", string.Empty, Error, string.Empty, true);
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL [C.O.M]", string.Empty, Error, string.Empty, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", string.Empty, Error, string.Empty, true);
            }

            return Completed;
        }
        /// <summary>
        /// Checks if Permissions is Set
        /// <code>"0" Checks File Permission Only</code>
        /// <code>"1" Checks Folder Permission Only</code>
        /// </summary>
        /// <param name="ModeType">
        /// <code>"0" Checks File Permission Only</code>
        /// <code>"1" Checks Folder Permission Only</code>
        /// </param>
        /// <param name="LocationPath">Enter File or Folder Path</param>
        /// <returns><code>True or False</code></returns>
        private bool CheckPermissionAccess(int ModeType, string LocationPath)
        {
            bool Completed = false;

            try
            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                if (ModeType >= 0 && ModeType <= 1 && !string.IsNullOrWhiteSpace(LocationPath))
                {
                    SecurityIdentifier Everyone_Question_Mark = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                    switch (ModeType)
                    {
                        /* Checks File Permissions */
                        case 0:
#if NETFRAMEWORK
                            FileSecurity CoreFileSecurity = File.GetAccessControl(LocationPath);
#else
                            FileSecurity CoreFileSecurity = new FileInfo(LocationPath).GetAccessControl();
#endif
                            AuthorizationRuleCollection AccessRuleCollection_File =
                                CoreFileSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

                            foreach (FileSystemAccessRule RuleThatIsSet in AccessRuleCollection_File)
                            {
                                if (RuleThatIsSet.IdentityReference.Value == Everyone_Question_Mark.Value &&
                                    RuleThatIsSet.AccessControlType == AccessControlType.Allow &&
                                    (RuleThatIsSet.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write)
                                {
                                    Completed = true;
                                    Log.Completed("FILE PERMISSION: [" + LocationPath + "] Is permission set? -> Yes");
                                }
                            }
                            break;
                        /* Checks Folder Permissions */
                        case 1:
                            DirectoryInfo FolderInfos = new DirectoryInfo(LocationPath);
                            DirectorySecurity CoreFolderSecurity = FolderInfos.GetAccessControl();
                            AuthorizationRuleCollection AccessRuleCollection_Folder =
                                CoreFolderSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

                            foreach (FileSystemAccessRule RuleThatIsSet in AccessRuleCollection_Folder)
                            {
                                if (RuleThatIsSet.IdentityReference.Value == Everyone_Question_Mark.Value &&
                                    RuleThatIsSet.AccessControlType == AccessControlType.Allow &&
                                    (RuleThatIsSet.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write)
                                {
                                    Completed = true;
                                    Log.Completed("FOLDER PERMISSION: [" + LocationPath + "] Is permission set? -> Yes");
                                }
                            }
                            break;
                        default:
                            Completed = false;
                            break;
                    }
                }
#else
                Completed = true;
#endif
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("PERMISSION Checker", string.Empty, Error, string.Empty, true);
            }

            return Completed;
        }
        /// <summary>
        /// Attempts to Set Read and Write Access Permissions for the User
        /// <code>"0" Sets File Permission Only</code>
        /// <code>"1" Sets Folder Permission Only</code>
        /// </summary>
        /// <param name="ModeType">
        /// <code>"0" Sets File Permission Only</code>
        /// <code>"1" Sets Folder Permission Only</code>
        /// </param>
        /// <param name="LocationPath">Enter File or Folder Path</param>
        /// <returns><code>True or False</code></returns>
        private bool GiveEveryoneReadWriteAccess(int ModeType, string LocationPath)
        {
            bool Completed = false;

            try
            {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                if (ModeType >= 0 && ModeType <= 1 && !string.IsNullOrWhiteSpace(LocationPath))
                {
                    bool IsPermissionAlreadySet = CheckPermissionAccess(ModeType, LocationPath);

                    if (!IsPermissionAlreadySet)
                    {
                        SecurityIdentifier Everyone_Question_Mark = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                        switch (ModeType)
                        {
                            /* Sets File Permissions */
                            case 0:
                                FileSystemAccessRule accessRule = new FileSystemAccessRule(Everyone_Question_Mark, FileSystemRights.FullControl,
                                    InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow);
#if NETFRAMEWORK
                                FileSecurity CoreFileSecurity = File.GetAccessControl(LocationPath);
#else
                                FileSecurity CoreFileSecurity = new FileInfo(LocationPath).GetAccessControl();
#endif
                                CoreFileSecurity.AddAccessRule(accessRule);
#if NETFRAMEWORK
                                File.SetAccessControl(LocationPath, CoreFileSecurity);
#else
                                new FileInfo(LocationPath).SetAccessControl(CoreFileSecurity);
#endif
                                Completed = true;
                                break;
                            /* Sets Folder Permissions */
                            case 1:
                                DirectoryInfo Info = new DirectoryInfo(LocationPath);
                                DirectorySecurity CoreFolderSecurity = Info.GetAccessControl();
                                CoreFolderSecurity.AddAccessRule(new FileSystemAccessRule(Everyone_Question_Mark, FileSystemRights.FullControl,
                                    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit,
                                    AccessControlType.Allow));
#if NETFRAMEWORK
                                Directory.SetAccessControl(LocationPath, CoreFolderSecurity);
#else
                                new DirectoryInfo(LocationPath).SetAccessControl(CoreFolderSecurity);
#endif
                                Completed = true;
                                break;
                            default:
                                Completed = false;
                                break;
                        }
                    }
                    else { Completed = true; }
                }
#else
                Completed = true;
#endif
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("PERMISSION Setter", string.Empty, Error, string.Empty, true);
            }

            return Completed;
        }
        /// <summary>Function Splitter For Firewall and Defender Checks</summary>
        /// <param name="ModeType">Range 0-5 Sets the Check Status.
        /// <code>"0" Sets Launcher Path</code>
        /// <code>"1" Sets Updater Path</code>
        /// <code>"2" Sets Current/New Game Path</code>
        /// <code>"3" Sets Old Game Path</code>
        /// <code>"4" Returns the Firewall Status in a form of a Boolean</code>
        /// <code>"5" Returns the Defender Status in a form of a Boolean</code>
        /// <code>"6" Returns the File/Folder Permission in a form of a Boolean</code>
        /// </param>
        /// <param name="ModeAPI">Range 0-2 Sets the Function Status. Each Function Returns a Boolean on if it was completed.
        /// <code>"0" Adds Rule</code>
        /// <code>"1" Removes Rule</code>
        /// <code>"2" Checks if Rule Exists</code>
        /// <code>"3" Adds Exclusion</code>
        /// <code>"4" Removes Exclusion</code>
        /// <code>"5" Checks if Exclusion Exists</code>
        /// <code>"6" Sets File/Folder Permission</code>
        /// </param>
        /// <returns><code>True or False</code></returns>
        private bool DataBase(int ModeType, int ModeAPI)
        {
            string AppName;
            string AppPath;
            string GroupKey;
            string Description;

            switch (ModeType)
            {
                /* Launcher */
                case 0:
                    AppName = "SBRW - Game Launcher";
                    GroupKey = "Game Launcher for Windows";
                    Description = "Soapbox Race World";
                    if (ModeAPI >= 0 && ModeAPI <= 2)
                    {
                        AppPath = Path.Combine(Locations.LauncherFolder, Locations.NameLauncher);
                    }
                    else { AppPath = Locations.LauncherFolder; }
                    break;
                /* Updater */
                case 1:
                    AppName = "SBRW - Game Launcher Updater";
                    AppPath = Path.Combine(Locations.LauncherFolder, Locations.NameUpdater);
                    GroupKey = "Game Launcher for Windows";
                    Description = "Soapbox Race World";
                    break;
                /* Current/New Game Files [2] Old Game Files [3]*/
                case 2:
                case 3:
                    AppName = "SBRW - Game";
                    GroupKey = "Need for Speed: World";
                    Description = GroupKey;
                    if (ModeAPI >= 0 && ModeAPI <= 2)
                    {
                        if (ModeType == 2)
                        {
                            AppPath = Path.Combine(Save_Settings.Live_Data.Game_Path, "nfsw.exe");
                        }
                        else
                        {
                            AppPath = Path.Combine(Save_Settings.Live_Data.Game_Path_Old, "nfsw.exe");
                        }
                    }
                    else
                    {
                        if (ModeType == 2)
                        {
                            AppPath = Save_Settings.Live_Data.Game_Path;
                        }
                        else
                        {
                            AppPath = Save_Settings.Live_Data.Game_Path_Old;
                        }
                    }
                    break;
                case 4:
                    return Firewall();
                case 5:
                    return Defender();
                case 6:
                    return CheckPermissionAccess(1, Locations.LauncherFolder) &&
                            CheckPermissionAccess(1, Save_Settings.Live_Data.Game_Path);
                default:
                    return false;
            }

            if (ModeType >= 0 && ModeType <= 3)
            {
                switch (ModeAPI)
                {
                    /* Firewall Rule Add */
                    case 0:
                        /* If File Path Exits, but not with our Set Name. Then Remove the rule within the set path. */
                        if (RuleExist("Path", AppName, AppPath) && !RuleExist("Name", AppName, AppPath))
                        {
                            /* Inbound & Outbound */
                            RemoveRules("Path", "Non-" + AppName, AppPath, "Path Match");
                        }
                        /* If both path and set name is not set in firewall, go ahead and add it normally */
                        if (!RuleExist("Path", AppName, AppPath) && !RuleExist("Name", AppName, AppPath))
                        {
                            /* Inbound */
                            AddApplicationRule(AppName, AppPath, GroupKey, Description,
                                FirewallDirection.Inbound, FirewallProtocol.Any, "Inbound");
                            /* Outbound */
                            return AddApplicationRule(AppName, AppPath, GroupKey, Description,
                                FirewallDirection.Outbound, FirewallProtocol.Any, "Outbound");
                        }
                        /* If both path and name are set and match, then its already set */
                        else if (RuleExist("Path", AppName, AppPath) && RuleExist("Name", AppName, AppPath))
                        {
                            Log.Completed("WINDOWS FIREWALL: " + AppName + " Rule is already Added");
                            return true;
                        }
                        /* Proabably a code conditional problem. Developer must check the code to verify its issue */
                        else 
                        { 
                            Log.Completed("WINDOWS FIREWALL: " + AppName + " Rule wasn't added due to a Unknown Issue"); return false; 
                        }
                    /* Firewall Rule Removal */
                    case 1:
                        if (RuleExist("Path", AppName, AppPath) && !RuleExist("Name", AppName, AppPath))
                        {
                            /* Inbound & Outbound */
                            RemoveRules("Path", "Non-" + AppName, AppPath, "Path Match");
                        }

                        if (RuleExist("Path", AppName, AppPath) && RuleExist("Name", AppName, AppPath))
                        {
                            return RemoveRules("Path", AppName, AppPath, "Path Match");
                        }
                        else 
                        { 
                            return false; 
                        }
                    /* Firewall Rule Check (Exists) */
                    case 2:
                        return RuleExist("Path", AppName, AppPath) && RuleExist("Name", AppName, AppPath);
                    /* Defender Exclusion Add */
                    case 3:
                        return AddApplicationExclusion(AppName, AppPath);
                    /* Defender Exclusion Removal */
                    case 4:
                        return RemoveExclusion(AppName, AppPath);
                    /* Defender Exclusion Check (Exists) */
                    case 5:
                        return ExclusionExist(AppPath);
                    case 6:
                        return GiveEveryoneReadWriteAccess(1, Locations.LauncherFolder) &&
                            GiveEveryoneReadWriteAccess(1, Save_Settings.Live_Data.Game_Path);
                    default:
                        return false;
                }
            }
            else { return false; }
        }
        /// <summary>
        /// Used to Enable Buttons with only Booleans
        /// </summary>
        /// <param name="ModeType">Range 0-5 Sets the Check Status.
        /// <code>"0" Sets Launcher Path</code>
        /// <code>"1" Sets Updater Path</code>
        /// <code>"2" Sets Current/New Game Path</code>
        /// <code>"3" Sets Old Game Path</code>
        /// <code>"4" Returns the Firewall Status in a form of a Boolean</code>
        /// <code>"5" Returns the Defender Status in a form of a Boolean</code>
        /// <code>"6" Returns the File/Folder Permission in a form of a Boolean</code>
        /// </param>
        /// <param name="ModeAPI">Range 0-2 Sets the Function Status. Each Function Returns a Boolean on if it was completed.
        /// <code>"0" Adds Rule</code>
        /// <code>"1" Removes Rule</code>
        /// <code>"2" Checks if Rule Exists</code>
        /// <code>"3" Adds Exclusion</code>
        /// <code>"4" Removes Exclusion</code>
        /// <code>"5" Checks if Exclusion Exists</code>
        /// <code>"6" Sets File/Folder Permission</code>
        /// </param>
        /// <remarks>
        /// <b>ModeType:</b>
        /// <code>"0" Sets Launcher Path</code>
        /// <code>"1" Sets Updater Path</code>
        /// <code>"2" Sets Current/New Game Path</code>
        /// <code>"3" Sets Old Game Path</code>
        /// <code>"4" Returns the Firewall Status in a form of a Boolean</code>
        /// <code>"5" Returns the Defender Status in a form of a Boolean</code>
        /// <code>"6" Returns the File/Folder Permission in a form of a Boolean</code>
        /// <b>ModeAPI:</b>
        /// <code>"0" Adds Rule</code>
        /// <code>"1" Removes Rule</code>
        /// <code>"2" Checks if Rule Exists</code>
        /// <code>"3" Adds Exclusion</code>
        /// <code>"4" Removes Exclusion</code>
        /// <code>"5" Checks if Exclusion Exists</code>
        /// <code>"6" Sets File/Folder Permission</code> 
        /// </remarks>
        /// <returns><code>True or False</code></returns>
        private bool ButtonEnabler(int ModeType, int ModeAPI)
        {
            try 
            {
                return DataBase(ModeType, ModeAPI); 
            }
            catch 
            { 
                return false; 
            }
        }
        ///<summary>Button: Firewall Rules API</summary>
        private async void ButtonFirewallRulesAPI_Click(object sender, EventArgs e)
        {
            await Task.Run(() => 
            {
                if (!DisableButtonFRAPI)
                {
                    Log.Info("Security Center Screen: ".ToUpper() + "[Check Firewall API] Button was clicked by user");

                    DisableButtonFRAPI = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonFirewallRulesCheck, 2, true);
                        DisableButtonFRC = false;
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesCheck, 3, false);
                        DisableButtonFRC = true;
                    }

                    ButtonsColorSet(ButtonFirewallRulesAPI, 1, false);
                }
            });
        }
        ///<summary>Button: Firewall Rules Check</summary>
        private async void ButtonFirewallRulesCheck_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonFRC)
                {
                    Log.Info("Security Center Screen: ".ToUpper() + "[Check All Rules] Button was clicked by user");

                    if (Firewall())
                    {
                        ButtonsColorSet(ButtonFirewallRulesCheck, 0, true);

                        /* Both */
                        if (ButtonEnabler(0, 2) && ButtonEnabler(1, 2) && ButtonEnabler(2, 2))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddAll, 1, true);
                            DisableButtonFRAA = DisableButtonFRRA = false;
                            ButtonsColorSet(ButtonFirewallRulesRemoveAll, 2, true);
                        }
                        else if (!ButtonEnabler(0, 2) && !ButtonEnabler(1, 2) && !ButtonEnabler(2, 2))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddAll, 2, true);
                            DisableButtonFRAA = false;
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddAll, 3, false);
                            DisableButtonFRAA = DisableButtonFRRA = true;
                            ButtonsColorSet(ButtonFirewallRulesRemoveAll, 3, false);
                        }
                        /* Launcher */
                        if (ButtonEnabler(0, 2) && ButtonEnabler(1, 2))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 1, true);
                            DisableButtonFRAL = DisableButtonFRRL = false;
                            ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 2, true);
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 2, true);
                            DisableButtonFRAL = false;
                            ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 3, false);
                        }
                        /* Game */
                        if (ButtonEnabler(2, 2) && !string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path_Old) &&
                            Save_Settings.Live_Data.Game_Path_Old != Save_Settings.Live_Data.Game_Path)
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame, 1, true);
                            DisableButtonFRAG = DisableButtonFRRG = false;
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 4, true);
                        }
                        else if (ButtonEnabler(2, 2))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame, 1, true);
                            DisableButtonFRAG = DisableButtonFRRG = false;
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 2, true);
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame,
                                (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path_Old) &&
                                (Save_Settings.Live_Data.Game_Path_Old != Save_Settings.Live_Data.Game_Path) ? 4 : 2), true);
                            DisableButtonFRAG = false;
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                        }

                        if (Firewall())
                        { 
                            ButtonsColorSet(ButtonFirewallRulesCheck, 1, true); 
                        }
                        else
                        { 
                            ButtonsColorSet(ButtonFirewallRulesCheck, 3, true); 
                        }
                    }
                    else
                    { 
                        ButtonsColorSet(ButtonFirewallRulesCheck, 3, true); 
                    }
                }
            });
        }
        ///<summary>Button: Firewall Rules Add All</summary>
        private async void ButtonFirewallRulesAddAll_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonFRAA)
                {
                    DisableButtonFRAA = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddAll, 2, true);

                        /* Launcher & Updater */
                        if (ButtonEnabler(0, 0) && ButtonEnabler(1, 0))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 1, true);
                            ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 2, true);
                            DisableButtonFRRL = false;
                            Save_Settings.Live_Data.Firewall_Launcher = "Excluded";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 3, false);
                            Save_Settings.Live_Data.Firewall_Launcher = "Error";
                        }
                        /* Game */
                        if (ButtonEnabler(2, 0))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame, 1, true);
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 2, true);
                            DisableButtonFRRG = false;
                            Save_Settings.Live_Data.Firewall_Game = "Excluded";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame, 3, false);
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                            Save_Settings.Live_Data.Firewall_Game = "Error";
                        }

                        Save_Settings.Save();

                        if (Firewall())
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddAll, 1, true);
                            DisableButtonFRRA = !(ButtonFirewallRulesRemoveLauncher.Enabled && ButtonFirewallRulesRemoveGame.Enabled);
                            ButtonsColorSet(ButtonFirewallRulesRemoveAll, 2,
                                ButtonFirewallRulesRemoveLauncher.Enabled && ButtonFirewallRulesRemoveGame.Enabled);
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddAll, 3, false);
                            ButtonsColorSet(ButtonFirewallRulesRemoveAll, 3, false);
                        }
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddAll, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Firewall Rules Add Launcher</summary>
        private async void ButtonFirewallRulesAddLauncher_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonFRAL)
                {
                    DisableButtonFRAL = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddLauncher, 2, true);

                        /* Game */
                        if (ButtonEnabler(0, 0) && ButtonEnabler(1, 0))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 1, true);
                            ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 2, true);
                            DisableButtonFRRL = false;
                            Save_Settings.Live_Data.Firewall_Launcher = "Excluded";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 3, false);
                            ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 3, false);
                            Save_Settings.Live_Data.Firewall_Launcher = "Error";
                        }

                        Save_Settings.Save();
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddLauncher, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Firewall Rules Add Game</summary>
        private async void ButtonFirewallRulesAddGame_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonFRAG)
                {
                    DisableButtonFRAG = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddGame, 2, true);

                        /* Remove Old Game Path and Cache Location Just in Case for Windows Defender */
                        if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path_Old))
                        {
                            if (ButtonEnabler(3, 1))
                            {
                                if (string.IsNullOrWhiteSpace(CacheOldGameLocation))
                                {
                                    CacheOldGameLocation = Save_Settings.Live_Data.Game_Path_Old;
                                }
                                Save_Settings.Live_Data.Game_Path_Old = string.Empty;
                            }
                        }

                        /* Game */
                        if (ButtonEnabler(2, 0))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame, 1, true);
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 2, true);
                            DisableButtonFRRG = false;
                            Save_Settings.Live_Data.Firewall_Game = "Excluded";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame, 3, false);
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                            Save_Settings.Live_Data.Firewall_Game = "Error";
                        }

                        Save_Settings.Save();
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddGame, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Firewall Rules Remove All</summary>
        private async void ButtonFirewallRulesRemoveAll_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonFRRA)
                {
                    DisableButtonFRRA = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonFirewallRulesRemoveAll, 2, true);

                        /* Launcher & Updater */
                        if (ButtonEnabler(0, 1) && ButtonEnabler(1, 1))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 2, true);
                            DisableButtonFRAL = true;
                            ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 1, true);
                            Save_Settings.Live_Data.Firewall_Launcher = "Removed";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 3, false);
                            ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 3, false);
                            Save_Settings.Live_Data.Firewall_Launcher = "Error";
                        }
                        /* Game */
                        if (ButtonEnabler(2, 1))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame, 2, true);
                            DisableButtonFRAG = true;
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 1, true);
                            Save_Settings.Live_Data.Firewall_Game = "Removed";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame, 3, false);
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                            Save_Settings.Live_Data.Firewall_Game = "Error";
                        }

                        Save_Settings.Save();

                        if (Firewall())
                        {
                            ButtonsColorSet(ButtonFirewallRulesRemoveAll, 1, true);
                            DisableButtonFRAA = !(ButtonFirewallRulesAddLauncher.Enabled && ButtonFirewallRulesAddGame.Enabled);
                            ButtonsColorSet(ButtonFirewallRulesAddAll, 2, ButtonFirewallRulesAddLauncher.Enabled && ButtonFirewallRulesAddGame.Enabled);
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesRemoveAll, 3, false);
                            ButtonsColorSet(ButtonFirewallRulesAddAll, 3, false);
                        }
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesRemoveAll, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Firewall Rules Remove Launcher</summary>
        private async void ButtonFirewallRulesRemoveLauncher_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonFRRL)
                {
                    DisableButtonFRRL = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 2, true);

                        /* Launcher & Updater */
                        if (ButtonEnabler(0, 1) && ButtonEnabler(1, 1))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 2, true);
                            DisableButtonFRAL = false;
                            ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 1, true);
                            Save_Settings.Live_Data.Firewall_Launcher = "Removed";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 3, false);
                            ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 3, false);
                            Save_Settings.Live_Data.Firewall_Launcher = "Error";
                        }

                        Save_Settings.Save();
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Firewall Rules Remove Game</summary>
        private async void ButtonFirewallRulesRemoveGame_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonFRRG)
                {
                    DisableButtonFRRG = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 0, true);
                        /* Remove Old Game Path and Cache Location Just in Case for Windows Defender */
                        if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path_Old))
                        {
                            if (ButtonEnabler(3, 1))
                            {
                                if (string.IsNullOrWhiteSpace(CacheOldGameLocation))
                                {
                                    CacheOldGameLocation = Save_Settings.Live_Data.Game_Path_Old;
                                }
                                Save_Settings.Live_Data.Game_Path_Old = string.Empty;
                            }
                        }

                        /* Game */
                        if (ButtonEnabler(2, 1))
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame, 2, true);
                            DisableButtonFRAG = false;
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 1, true);
                            Save_Settings.Live_Data.Firewall_Game = "Removed";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFirewallRulesAddGame, 3, false);
                            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                            Save_Settings.Live_Data.Firewall_Game = "Error";
                        }

                        Save_Settings.Save();
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Defender Exclusion API</summary>
        private async void ButtonDefenderExclusionAPI_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonDRAPI)
                {
                    if (EnableInsiderDeveloper.Allowed() || (Product_Version.GetWindowsNumber() >= 10 &&
                        (MessageBox.Show(this, "There has been reports that some users are not able to run Windows Defender Checks." +
                        "\nThis ranges from the Built-In to Third-Party Anti-Virus Software." +
                        "\n\nIf this Window Closes or the Launcher Crashes with an Error Message" +
                        "\n\nDo not run this Check, just simply ignore this section." +
                        "\n\n\nClick Yes to Agree to a potential Launcher Crash" +
                        "\nClick No to avoid a potential Launcher Crash",
                        "Windows Defender API Check - SBRW Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)))
                    {
                        DisableButtonDRAPI = true;

                        if (ButtonEnabler(5, 10))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionCheck, 2, true);
                            DisableButtonDRC = false;
                        }
                        else { ButtonsColorSet(ButtonDefenderExclusionCheck, 3, false); DisableButtonDRC = true; }

                        ButtonsColorSet(ButtonDefenderExclusionAPI, 1, false);
                    }
                }
            });
        }
        ///<summary>Button: Defender Exclusion Check</summary>
        private async void ButtonDefenderExclusionCheck_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonDRC)
                {
                    if (Defender())
                    {
                        ButtonsColorSet(ButtonDefenderExclusionCheck, 0, true);

                        /* Launcher, Updater, & All */
                        if (ButtonEnabler(0, 5))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddAll, 1, true);
                            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 1, true);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveAll, 2, true);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 2, true);
                            DisableButtonDRAA = DisableButtonDRAL = DisableButtonDRRA = DisableButtonDRRL = false;
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddAll, 2, true);
                            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 2, true);
                            DisableButtonDRAL = DisableButtonDRAA = false;
                            ButtonsColorSet(ButtonDefenderExclusionRemoveAll, 3, false);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 3, false);
                        }
                        /* Game */
                        if (ButtonEnabler(2, 5) && !string.IsNullOrWhiteSpace(CacheOldGameLocation) &&
                            CacheOldGameLocation != Save_Settings.Live_Data.Game_Path)
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame, 1, true);
                            DisableButtonDRAG = DisableButtonDRRG = false;
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 4, true);
                        }
                        else if (ButtonEnabler(2, 5))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame, 1, true);
                            DisableButtonDRAG = DisableButtonDRRG = false;
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 2, true);
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame,
                                (!string.IsNullOrWhiteSpace(CacheOldGameLocation) &&
                                (CacheOldGameLocation != Save_Settings.Live_Data.Game_Path) ? 4 : 2), true);
                            DisableButtonDRAG = false;
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                            DisableButtonDRRG = true;
                        }

                        if (Defender())
                        { ButtonsColorSet(ButtonDefenderExclusionCheck, 1, true); }
                        else
                        { ButtonsColorSet(ButtonDefenderExclusionCheck, 3, true); }
                    }
                    else
                    { ButtonsColorSet(ButtonDefenderExclusionCheck, 3, true); }
                }
            });
        }
        ///<summary>Button: Defender Exclusion Add All</summary>
        private async void ButtonDefenderExclusionAddAll_Click(object sender, EventArgs e)
        {
            await Task.Run(() => 
            {
                if (!DisableButtonDRAA)
                {
                    DisableButtonDRAA = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddAll, 2, true);

                        /* Launcher & Updater */
                        if (ButtonEnabler(0, 3))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 1, true);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 2, true);
                            DisableButtonDRRL = false;
                            Save_Settings.Live_Data.Defender_Launcher = "Excluded";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 3, false);
                            Save_Settings.Live_Data.Defender_Launcher = "Error";
                        }
                        /* Game */
                        if (ButtonEnabler(2, 3))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame, 1, true);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 2, true);
                            DisableButtonDRRG = false;
                            Save_Settings.Live_Data.Defender_Game = "Excluded";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame, 3, false);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                            Save_Settings.Live_Data.Defender_Game = "Error";
                        }

                        Save_Settings.Save();

                        if (Defender())
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddAll, 1, true);
                            DisableButtonDRRA = !(ButtonDefenderExclusionRemoveLauncher.Enabled && ButtonDefenderExclusionRemoveGame.Enabled);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveAll, 2,
                                ButtonDefenderExclusionRemoveLauncher.Enabled && ButtonDefenderExclusionRemoveGame.Enabled);
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddAll, 3, false);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveAll, 3, false);
                        }
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddAll, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Defender Exclusion Add Launcher</summary>
        private async void ButtonDefenderExclusionAddLauncher_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonDRAL)
                {
                    DisableButtonDRAL = true;

                    if (ButtonEnabler(5, 10))
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 2, true);

                        /* Launcher & Updater */
                        if (ButtonEnabler(0, 3))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 1, true);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 2, true);
                            DisableButtonDRRL = false;
                            Save_Settings.Live_Data.Defender_Launcher = "Excluded";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 3, false);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 3, false);
                            Save_Settings.Live_Data.Defender_Launcher = "Error";
                        }

                        Save_Settings.Save();
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Defender Exclusion Add Game</summary>
        private async void ButtonDefenderExclusionAddGame_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonDRAG)
                {
                    DisableButtonDRAG = true;

                    if (ButtonEnabler(5, 10))
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddGame, 2, true);
                        /* Remove Old Game Path */
                        if (!string.IsNullOrWhiteSpace(CacheOldGameLocation))
                        {
                            if (ButtonEnabler(3, 4))
                            {
                                CacheOldGameLocation = string.Empty;
                            }
                        }

                        /* Game */
                        if (ButtonEnabler(2, 3))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame, 1, true);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 2, true);
                            DisableButtonDRRG = false;
                            Save_Settings.Live_Data.Defender_Game = "Excluded";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame, 3, false);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                            Save_Settings.Live_Data.Defender_Game = "Error";
                        }

                        Save_Settings.Save();
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddGame, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Defender Exclusion Remove All</summary>
        private async void ButtonDefenderExclusionRemoveAll_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonDRRA)
                {
                    DisableButtonDRRA = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonDefenderExclusionRemoveAll, 2, true);

                        /* Launcher & Updater */
                        if (ButtonEnabler(0, 4))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 2, true);
                            DisableButtonDRAL = true;
                            ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 1, true);
                            Save_Settings.Live_Data.Defender_Launcher = "Removed";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 3, false);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 3, false);
                            Save_Settings.Live_Data.Defender_Launcher = "Error";
                        }
                        /* Game */
                        if (ButtonEnabler(2, 4))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame, 2, true);
                            DisableButtonDRAG = true;
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 1, true);
                            Save_Settings.Live_Data.Defender_Game = "Removed";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame, 3, false);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                            Save_Settings.Live_Data.Defender_Game = "Error";
                        }

                        Save_Settings.Save();

                        if (Defender())
                        {
                            ButtonsColorSet(ButtonDefenderExclusionRemoveAll, 1, true);
                            DisableButtonDRAA = !(ButtonDefenderExclusionAddLauncher.Enabled && ButtonDefenderExclusionAddGame.Enabled);
                            ButtonsColorSet(ButtonDefenderExclusionAddAll, 2,
                                ButtonDefenderExclusionAddLauncher.Enabled && ButtonDefenderExclusionAddGame.Enabled);
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionRemoveAll, 3, false);
                            ButtonsColorSet(ButtonDefenderExclusionAddAll, 3, false);
                        }
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionRemoveAll, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Defender Exclusion Remove Launcher</summary>
        private async void ButtonDefenderExclusionRemoveLauncher_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonDRRL)
                {
                    DisableButtonDRRL = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 2, true);

                        /* Launcher & Updater */
                        if (ButtonEnabler(0, 4))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 2, true);
                            DisableButtonDRAL = false;
                            ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 1, true);
                            Save_Settings.Live_Data.Defender_Launcher = "Removed";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 3, false);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 3, false);
                            Save_Settings.Live_Data.Defender_Launcher = "Error";
                        }

                        Save_Settings.Save();
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: Defender Exclusion Remove Game</summary>
        private async void ButtonDefenderExclusionRemoveGame_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonDRRG)
                {
                    DisableButtonDRRG = true;

                    if (ButtonEnabler(4, 20))
                    {
                        ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 0, true);
                        /* Remove Old Game Path */
                        if (!string.IsNullOrWhiteSpace(CacheOldGameLocation))
                        {
                            if (ButtonEnabler(3, 4))
                            {
                                CacheOldGameLocation = string.Empty;
                            }
                        }

                        /* Game */
                        if (ButtonEnabler(2, 4))
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame, 2, true);
                            DisableButtonDRAG = false;
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 1, true);
                            Save_Settings.Live_Data.Defender_Game = "Removed";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonDefenderExclusionAddGame, 3, false);
                            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                            Save_Settings.Live_Data.Defender_Game = "Error";
                        }

                        Save_Settings.Save();
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                    }
                }
            });
        }
        ///<summary>Button: File or Folder Permisson Check</summary>
        private async void ButtonFolderPermissonCheck_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonPRC)
                {
                    if (!ButtonEnabler(6, 6))
                    {
                        ButtonsColorSet(ButtonFolderPermissonSet, 2, true);
                        DisableButtonPRAA = false;
                    }
                    else 
                    { 
                        ButtonsColorSet(ButtonFolderPermissonSet, 1, false); 
                    }

                    ButtonsColorSet(ButtonFolderPermissonCheck, 1, false);
                }
            });
        }
        ///<summary>Button: Firewall Rules Add Launcher</summary>
        private async void ButtonFolderPermissonSet_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!DisableButtonPRAA)
                {
                    DisableButtonPRAA = true;

                    if (ButtonEnabler(6, 6))
                    {
                        ButtonsColorSet(ButtonFolderPermissonSet, 2, true);
                        DisableButtonPRC = true;

                        /* Game */
                        if (ButtonEnabler(0, 6))
                        {
                            ButtonsColorSet(ButtonFolderPermissonSet, 1, true);
                            Save_Settings.Live_Data.Write_Permissions = "Set";
                        }
                        else
                        {
                            ButtonsColorSet(ButtonFolderPermissonSet, 3, false);
                            Save_Settings.Live_Data.Write_Permissions = "Error";
                        }

                        Save_Settings.Save();
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFolderPermissonSet, 3, false);
                    }
                }
            });
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonClose_MouseDown(object sender, EventArgs e)
        {
            ButtonClose.BackgroundImage = Image_Icon.Close_Click;
        }

        private void ButtonClose_MouseEnter(object sender, EventArgs e)
        {
            ButtonClose.BackgroundImage = Image_Icon.Close_Hover;
        }

        private void ButtonClose_MouseLeaveANDMouseUp(object sender, EventArgs e)
        {
            ButtonClose.BackgroundImage = Image_Icon.Close;
        }
        ///<summary>Theming, Function, EventHandlers, Etc. Meant to load critial functions before the forms loads</summary>
        private void SetVisuals()
        {
            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Icon = FormsIcon.Retrive_Icon();
            Text = "Security Center - SBRW Launcher: v" + Application.ProductVersion;

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            VersionLabel.Text = "Version : v" + Application.ProductVersion;

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            BackgroundImage = Image_Background.Security_Center;
            TransparencyKey = Color_Screen.BG_Security_Center;
            ButtonClose.BackgroundImage = Image_Icon.Close;

            TextWindowsFirewall.ForeColor = Color_Text.L_Five;
            TextWindowsDefender.ForeColor = Color_Text.L_Five;
            TextFolderPermissions.ForeColor = Color_Text.L_Five;
            VersionLabel.ForeColor = Color_Text.L_Five;

            /*******************************/
            /* Set Colored Buttons          /
            /*******************************/

            ButtonsColorSet(ButtonFirewallRulesAPI, 2, true);
            ButtonsColorSet(ButtonFirewallRulesCheck, 2017, false);
            ButtonsColorSet(ButtonFirewallRulesAddAll, 2017, false);
            ButtonsColorSet(ButtonFirewallRulesAddLauncher, 2017, false);
            ButtonsColorSet(ButtonFirewallRulesAddGame, 2017, false);
            ButtonsColorSet(ButtonFirewallRulesRemoveAll, 2017, false);
            ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 2017, false);
            ButtonsColorSet(ButtonFirewallRulesRemoveGame, 2017, false);
            ButtonsColorSet(ButtonDefenderExclusionAPI, 2, true);
            ButtonsColorSet(ButtonDefenderExclusionCheck, 2017, false);
            ButtonsColorSet(ButtonDefenderExclusionAddAll, 2017, false);
            ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 2017, false);
            ButtonsColorSet(ButtonDefenderExclusionAddGame, 2017, false);
            ButtonsColorSet(ButtonDefenderExclusionRemoveAll, 2017, false);
            ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 2017, false);
            ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 2017, false);
            ButtonsColorSet(ButtonFolderPermissonCheck, 2, true);
            ButtonsColorSet(ButtonFolderPermissonSet, 2017, false);

            /*******************************/
            /* Set Font                     /
            /*******************************/
#if !(RELEASE_UNIX || DEBUG_UNIX)
            float MainFontSize = 9f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = 8f * 96f / CreateGraphics().DpiY;
#else
            float MainFontSize = 9f;
            float SecondaryFontSize = 8f;
#endif

            Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            /* Text */
            TextWindowsFirewall.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            TextWindowsDefender.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            TextFolderPermissions.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            VersionLabel.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Regular);
            /* Firewall */
            ButtonFirewallRulesAPI.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesCheck.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesAddAll.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesAddLauncher.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesAddGame.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesRemoveAll.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesRemoveLauncher.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesRemoveGame.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            /* Defender */
            ButtonFirewallRulesAPI.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesCheck.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesAddAll.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonDefenderExclusionAddLauncher.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonDefenderExclusionAddGame.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonDefenderExclusionRemoveAll.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonDefenderExclusionRemoveLauncher.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonDefenderExclusionRemoveGame.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            /* File/Folder Permission */
            ButtonFolderPermissonCheck.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);
            ButtonFolderPermissonSet.Font = new Font(FormsFont.Primary_Bold(), SecondaryFontSize, FontStyle.Bold);

            /*******************************/
            /* Set Event Handlers           /
            /*******************************/

            /* Firewall Checks */
            ButtonFirewallRulesAPI.Click += new EventHandler(ButtonFirewallRulesAPI_Click);
            ButtonFirewallRulesCheck.Click += new EventHandler(ButtonFirewallRulesCheck_Click);
            /* Firewall Add */
            ButtonFirewallRulesAddAll.Click += new EventHandler(ButtonFirewallRulesAddAll_Click);
            ButtonFirewallRulesAddLauncher.Click += new EventHandler(ButtonFirewallRulesAddLauncher_Click);
            ButtonFirewallRulesAddGame.Click += new EventHandler(ButtonFirewallRulesAddGame_Click);
            /* Firewall Remove */
            ButtonFirewallRulesRemoveAll.Click += new EventHandler(ButtonFirewallRulesRemoveAll_Click);
            ButtonFirewallRulesRemoveLauncher.Click += new EventHandler(ButtonFirewallRulesRemoveLauncher_Click);
            ButtonFirewallRulesRemoveGame.Click += new EventHandler(ButtonFirewallRulesRemoveGame_Click);
            /* Defender Checks */
            ButtonDefenderExclusionAPI.Click += new EventHandler(ButtonDefenderExclusionAPI_Click);
            ButtonDefenderExclusionCheck.Click += new EventHandler(ButtonDefenderExclusionCheck_Click);
            /* Defender Add */
            ButtonDefenderExclusionAddAll.Click += new EventHandler(ButtonDefenderExclusionAddAll_Click);
            ButtonDefenderExclusionAddLauncher.Click += new EventHandler(ButtonDefenderExclusionAddLauncher_Click);
            ButtonDefenderExclusionAddGame.Click += new EventHandler(ButtonDefenderExclusionAddGame_Click);
            /* Defender Remove */
            ButtonDefenderExclusionRemoveAll.Click += new EventHandler(ButtonDefenderExclusionRemoveAll_Click);
            ButtonDefenderExclusionRemoveLauncher.Click += new EventHandler(ButtonDefenderExclusionRemoveLauncher_Click);
            ButtonDefenderExclusionRemoveGame.Click += new EventHandler(ButtonDefenderExclusionRemoveGame_Click);
            /* Permission Checks */
            ButtonFolderPermissonCheck.Click += new EventHandler(ButtonFolderPermissonCheck_Click);
            ButtonFolderPermissonSet.Click += new EventHandler(ButtonFolderPermissonSet_Click);
            /* Close */
            ButtonClose.MouseEnter += new EventHandler(ButtonClose_MouseEnter);
            ButtonClose.MouseLeave += new EventHandler(ButtonClose_MouseLeaveANDMouseUp);
            ButtonClose.MouseUp += new MouseEventHandler(ButtonClose_MouseLeaveANDMouseUp);
            ButtonClose.MouseDown += new MouseEventHandler(ButtonClose_MouseDown);
            ButtonClose.Click += new EventHandler(ButtonClose_Click);
            /* Window */
            if (Parent_Screen.Screen_Instance != null)
            {
                MouseMove += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Move);
                MouseUp += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Up);
                MouseDown += new MouseEventHandler(Parent_Screen.Screen_Instance.Move_Window_Mouse_Down);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Screen_Security_Center()
        {
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, y) =>
            {
                
                DisableButtonFRAPI = DisableButtonDRAPI = DisableButtonDRAPI = DisableButtonPRC = false;
                if (!string.IsNullOrWhiteSpace(RPCStateCache) && Screen_Main.Screen_Instance != null)
                {
                    if (RPCStateCache.Contains("Settings"))
                    {
                        Presence_Launcher.Status(22);
                        Screen_Settings.Clear_Hide_Screen_Form_Panel();
                    }
                    else if (RPCStateCache.Contains("Ready"))
                    {
                        Presence_Launcher.Status(4);
                        Screen_Main.Clear_Hide_Screen_Form_Panel();
                    }
                }
                RPCStateCache = string.Empty;
                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            };

            Presence_Launcher.Status(20);
        }
    }
}
