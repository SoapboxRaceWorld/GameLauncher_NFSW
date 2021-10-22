using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;
using WindowsFirewallHelper;
using WindowsFirewallHelper.Exceptions;
using WindowsFirewallHelper.FirewallRules;

namespace GameLauncher.App.UI_Forms.SecurityCenter_Screen
{
    public partial class SecurityCenterScreen : Form
    {
        ///<summary>Prevents a Duplicate Window from Happening</summary>
        private static bool IsSecurityCenterOpen = false;
        ///<summary>Windows 10: Caches Old Game Path in the event of the user does Firewall First</summary>
        private static string CacheOldGameLocation = FileSettingsSave.GameInstallationOld;
        ///<summary>Disable Button: Firewall Rules API</summary>
        private static bool DisableButtonFRAPI = false;
        ///<summary>Disable Button: Firewall Rules Check</summary>
        private static bool DisableButtonFRC = true;
        ///<summary>Disable Button: Firewall Rules Add All</summary>
        private static bool DisableButtonFRAA = true;
        ///<summary>Disable Button: Firewall Rules Add Launcher</summary>
        private static bool DisableButtonFRAL = true;
        ///<summary>Disable Button: Firewall Rules Add Game</summary>
        private static bool DisableButtonFRAG = true;
        ///<summary>Disable Button: Firewall Rules Remove All</summary>
        private static bool DisableButtonFRRA = true;
        ///<summary>Disable Button: Firewall Rules Remove Launcher</summary>
        private static bool DisableButtonFRRL = true;
        ///<summary>Disable Button: Firewall Rules Remove Game</summary>
        private static bool DisableButtonFRRG = true;
        ///<summary>Disable Button: Defender Exclusion API</summary>
        private static bool DisableButtonDRAPI = false;
        ///<summary>Disable Button: Defender Exclusion Check</summary>
        private static bool DisableButtonDRC = true;
        ///<summary>Disable Button: Defender Exclusion Add All</summary>
        private static bool DisableButtonDRAA = true;
        ///<summary>Disable Button: Defender Exclusion Add Launcher</summary>
        private static bool DisableButtonDRAL = true;
        ///<summary>Disable Button: Defender Exclusion Add Game</summary>
        private static bool DisableButtonDRAG = true;
        ///<summary>Disable Button: Defender Exclusion Remove All</summary>
        private static bool DisableButtonDRRA = true;
        ///<summary>Disable Button: Defender Exclusion Remove Launcher</summary>
        private static bool DisableButtonDRRL = true;
        ///<summary>Disable Button: Defender Exclusion Remove Game</summary>
        private static bool DisableButtonDRRG = true;
        ///<summary>Disable Button: Permission Check</summary>
        private static bool DisableButtonPRC = false;
        ///<summary>Disable Button: Permission Set</summary>
        private static bool DisableButtonPRAA = true;
        ///<summary>RPC: Which State to do once Form Closes</summary>
        private static string RPCStateCache;

        public static void OpenScreen(string RPCState)
        {
            if (IsSecurityCenterOpen || Application.OpenForms["SecurityCenterScreen"] != null)
            {
                if (Application.OpenForms["SecurityCenterScreen"] != null) { Application.OpenForms["SecurityCenterScreen"].Activate(); }
            }
            else
            {
                RPCStateCache = RPCState;
                try { new SecurityCenterScreen().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Security Center Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Security Center Panel", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public SecurityCenterScreen()
        {
            IsSecurityCenterOpen = true;
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, y) =>
            {
                DiscordLauncherPresence.Status(RPCStateCache, null);
                IsSecurityCenterOpen = DisableButtonFRAPI = DisableButtonDRAPI = DisableButtonDRAPI = DisableButtonPRC = false;
                RPCStateCache = null;
                GC.Collect();
            };

            DiscordLauncherPresence.Status("Security Center", null);
        }
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
                    Elements.ForeColor = Theming.BlueForeColorButton;
                    Elements.BackColor = Theming.BlueBackColorButton;
                    Elements.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
                    Elements.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;
                    Elements.Enabled = EnabledORDisabled;
                    break;
                /* Success Green */
                case 1:
                    Elements.ForeColor = Theming.GreenForeColorButton;
                    Elements.BackColor = Theming.GreenBackColorButton;
                    Elements.FlatAppearance.BorderColor = Theming.GreenBorderColorButton;
                    Elements.FlatAppearance.MouseOverBackColor = Theming.GreenMouseOverBackColorButton;
                    Elements.Enabled = EnabledORDisabled;
                    break;
                /* Warning Orange */
                case 2:
                    Elements.ForeColor = Theming.YellowForeColorButton;
                    Elements.BackColor = Theming.YellowBackColorButton;
                    Elements.FlatAppearance.BorderColor = Theming.YellowBorderColorButton;
                    Elements.FlatAppearance.MouseOverBackColor = Theming.YellowMouseOverBackColorButton;
                    Elements.Enabled = EnabledORDisabled;
                    break;
                /* Error Red */
                case 3:
                    Elements.ForeColor = Theming.RedForeColorButton;
                    Elements.BackColor = Theming.RedBackColorButton;
                    Elements.FlatAppearance.BorderColor = Theming.RedBorderColorButton;
                    Elements.FlatAppearance.MouseOverBackColor = Theming.RedMouseOverBackColorButton;
                    Elements.Enabled = EnabledORDisabled;
                    break;
                /* Unknown Gray */
                default:
                    Elements.ForeColor = Theming.GrayForeColorButton;
                    Elements.BackColor = Theming.GrayBackColorButton;
                    Elements.FlatAppearance.BorderColor = Theming.GrayBorderColorButton;
                    Elements.FlatAppearance.MouseOverBackColor = Theming.GrayMouseOverBackColorButton;
                    Elements.Enabled = !EnabledORDisabled;
                    break;
            }
        }
        /// <summary>Checks WMI Query on if Windows Defender is Enabled</summary>
        /// <param name="Query">Query a Specific Collection</param>
        /// <returns><code>True or False</code></returns>
        private bool GetDefenderStatus(string Query)
        {
            if (!UnixOS.Detected() && WindowsProductVersion.GetWindowsNumber() >= 10)
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
                    LogToFileAddons.OpenLog("Windows Defender Status [M.E.]", null, Error, null, true);
                }
                catch (COMException Error)
                {
                    LogToFileAddons.OpenLog("Windows Defender Status [C.O.M.]", null, Error, null, true);
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
            }

            return false;
        }
        /// <summary>Checks Windows Defender on if it's Enabled or Disabled by the User or Third-Party Program</summary>
        /// <remarks>Doesn't checks If the Service is Disabled or a Third-Party Program reports an Incorrect Value</remarks>
        /// <returns><code>True or False</code></returns>
        public bool Defender()
        {
            if (!UnixOS.Detected())
            {
                return GetDefenderStatus("AntivirusEnabled") &&
                    GetDefenderStatus("AntispywareEnabled") &&
                    GetDefenderStatus("RealTimeProtectionEnabled");
            }

            return false;
        }
        /// <summary>Windows Defender: Checks Defender's Current Exclusion List</summary>
        /// <returns>String-Array of Exclusions</returns>
        private string[] ExclusionCheck()
        {
            if (!UnixOS.Detected())
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
                    LogToFileAddons.OpenLog("Windows Defender Exclusion Path Check [M.E.]", null, Error, null, true);
                }
                catch (COMException Error)
                {
                    LogToFileAddons.OpenLog("Windows Defender Exclusion Path Check [C.O.M.]", null, Error, null, true);
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
            }

            return Array.Empty<string>();
        }
        /// <summary>
        /// Finds a Defender Exclusion in Defenders "Database"
        /// </summary>
        /// <param name="FilePath">Enter file Path</param>
        /// <returns><code>True or False</code></returns>
        private bool ExclusionExist(string FilePath)
        {
            if (UnixOS.Detected())
            {
                return true;
            }
            else
            {
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
                else { return false; }
            }
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
                        AddScript.AddScript($"Add-MpPreference -ExclusionPath \"{Strings.Encode(AppPath)}\"");
                        AddScript.Invoke();
                    }

                    Completed = true;
                    Log.Completed("Windows Defender: ".ToUpper() + "Folder is now Excluded. -> " + AppPath);
                }
                else { Log.Completed("WINDOWS FIREWALL: " + AppName + " Rule is already Added"); Completed = true; }
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Add Script [C.O.M.]", null, Error, null, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Add Script", null, Error, null, true);
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
                        RemovalScript.AddScript($"Remove-MpPreference -ExclusionPath \"{Strings.Encode(AppPath)}\"");
                        RemovalScript.Invoke();
                    }

                    Completed = true;
                    Log.Completed("Windows Defender: ".ToUpper() + "Folder is no longer Excluded. -> " + AppPath);
                }
                else { Log.Completed("WINDOWS FIREWALL: " + AppName + " Rule is already Removed"); Completed = true; }
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Removal Script [C.O.M.]", null, Error, null, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Removal Script", null, Error, null, true);
            }

            return Completed;
        }
        /// <summary>
        /// Checks the Firewall API Version Dynamically
        /// </summary>
        /// <returns>Firewall API Version</returns>
        private FirewallAPIVersion FirewallAPI()
        {
            if (UnixOS.Detected())
            {
                return FirewallAPIVersion.None;
            }
            else
            {
                try { return FirewallManager.Version; }
                catch { return FirewallAPIVersion.None; }
            }
        }
        /// <summary>
        /// Checks the Firewall API Version against Versions that isn't supported
        /// </summary>
        /// <returns><code>True or False</code></returns>
        private bool FirewallSupported()
        {
            if (UnixOS.Detected() || FirewallAPI() == FirewallAPIVersion.None)
            {
                return false;
            }
            else
            {
                return true;
            }
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
                if (!UnixOS.Detected())
                {
                    if (bool.TryParse(FirewallManager.IsServiceRunning.ToString(), out bool Result) && Result)
                    {
                        Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", true);
                        INetFwMgr Mana = (INetFwMgr)Activator.CreateInstance(NetFwMgrType);

                        if (bool.TryParse(Mana.LocalPolicy.CurrentProfile.FirewallEnabled.ToString(), out bool Results))
                        {
                            return Mana.LocalPolicy.CurrentProfile.FirewallEnabled;
                        }
                    }
                }
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Check", null, Error, null, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL Check", null, Error, null, true);
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
                LogToFileAddons.OpenLog("WINDOWS FIREWALL [Not Supported]", null, Error, null, true);
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL [COM]", null, Error, null, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
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
                                LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
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
            if (UnixOS.Detected())
            {
                return true;
            }
            else
            {
                if (FindRules(Mode, AppName, AppPath) != null) { return FindRules(Mode, AppName, AppPath).Any(); }
                else { return false; }
            }
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
                LogToFileAddons.OpenLog("WINDOWS FIREWALL [F.WAS.N.S.E]", null, Error, null, true);
            }
            catch (COMException Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL [C.O.M]", null, Error, null, true);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WINDOWS FIREWALL", null, Error, null, true);
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
                if (UnixOS.Detected())
                {
                    Completed = true;
                }
                else if (ModeType >= 0 && ModeType <= 1 && !string.IsNullOrWhiteSpace(LocationPath))
                {
                    SecurityIdentifier Everyone_Question_Mark = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                    switch (ModeType)
                    {
                        /* Checks File Permissions */
                        case 0:
                            FileSecurity CoreFileSecurity = File.GetAccessControl(LocationPath);
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
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("PERMISSION Checker", null, Error, null, true);
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
                if (UnixOS.Detected())
                {
                    Completed = true;
                }
                else if (ModeType >= 0 && ModeType <= 1 && !string.IsNullOrWhiteSpace(LocationPath))
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
                                FileSecurity CoreFileSecurity = File.GetAccessControl(LocationPath);
                                CoreFileSecurity.AddAccessRule(accessRule);
                                File.SetAccessControl(LocationPath, CoreFileSecurity);
                                Completed = true;
                                break;
                            /* Sets Folder Permissions */
                            case 1:
                                DirectoryInfo Info = new DirectoryInfo(LocationPath);
                                DirectorySecurity CoreFolderSecurity = Info.GetAccessControl();
                                CoreFolderSecurity.AddAccessRule(new FileSystemAccessRule(Everyone_Question_Mark, FileSystemRights.FullControl,
                                    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit,
                                    AccessControlType.Allow));
                                Directory.SetAccessControl(LocationPath, CoreFolderSecurity);
                                Completed = true;
                                break;
                            default:
                                Completed = false;
                                break;
                        }
                    }
                    else { Completed = true; }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("PERMISSION Setter", null, Error, null, true);
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
                        AppPath = Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameLauncher));
                    }
                    else { AppPath = Locations.LauncherFolder; }
                    break;
                /* Updater */
                case 1:
                    AppName = "SBRW - Game Launcher Updater";
                    AppPath = Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameUpdater));
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
                            AppPath = Strings.Encode(Path.Combine(FileSettingsSave.GameInstallation, "nfsw.exe"));
                        }
                        else
                        {
                            AppPath = Strings.Encode(Path.Combine(FileSettingsSave.GameInstallationOld, "nfsw.exe"));
                        }
                    }
                    else
                    {
                        if (ModeType == 2)
                        {
                            AppPath = FileSettingsSave.GameInstallation;
                        }
                        else
                        {
                            AppPath = FileSettingsSave.GameInstallationOld;
                        }
                    }
                    break;
                case 4:
                    return Firewall();
                case 5:
                    return Defender();
                case 6:
                    return CheckPermissionAccess(1, Locations.LauncherFolder) &&
                            CheckPermissionAccess(1, FileSettingsSave.GameInstallation);
                default:
                    return false;
            }

            if (ModeType >= 0 && ModeType <= 3)
            {
                switch (ModeAPI)
                {
                    /* Firewall Rule Add */
                    case 0:
                        if (RuleExist("Path", AppName, AppPath) && !RuleExist("Name", AppName, AppPath))
                        {
                            /* Inbound & Outbound */
                            RemoveRules("Path", "Non-" + AppName, AppPath, "Path Match");
                        }

                        if (!RuleExist("Path", AppName, AppPath) && !RuleExist("Name", AppName, AppPath))
                        {
                            /* Inbound */
                            AddApplicationRule(AppName, AppPath, GroupKey, Description,
                                FirewallDirection.Inbound, FirewallProtocol.Any, "Inbound");
                            /* Outbound */
                            return AddApplicationRule(AppName, AppPath, GroupKey, Description,
                                FirewallDirection.Outbound, FirewallProtocol.Any, "Outbound");
                        }
                        else if (RuleExist("Path", AppName, AppPath) && RuleExist("Name", AppName, AppPath))
                        {
                            Log.Completed("WINDOWS FIREWALL: " + AppName + " Rule is already Added");
                            return true;
                        }
                        else { Log.Completed("WINDOWS FIREWALL: " + AppName + " Rule wasn't added due to a Unknown Issue"); return false; }
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
                        else { return false; }
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
                            GiveEveryoneReadWriteAccess(1, FileSettingsSave.GameInstallation);
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
            try { return DataBase(ModeType, ModeAPI); }
            catch { return false; }
        }
        ///<summary>Button: Firewall Rules API</summary>
        private void ButtonFirewallRulesAPI_Click(object sender, EventArgs e)
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
                else { ButtonsColorSet(ButtonFirewallRulesCheck, 3, false); DisableButtonFRC = true; }

                ButtonsColorSet(ButtonFirewallRulesAPI, 1, false);
            }
        }
        ///<summary>Button: Firewall Rules Check</summary>
        private void ButtonFirewallRulesCheck_Click(object sender, EventArgs e)
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
                    if (ButtonEnabler(2, 2) && !string.IsNullOrWhiteSpace(FileSettingsSave.GameInstallationOld) &&
                        FileSettingsSave.GameInstallationOld != FileSettingsSave.GameInstallation)
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
                            (!string.IsNullOrWhiteSpace(FileSettingsSave.GameInstallationOld) &&
                            (FileSettingsSave.GameInstallationOld != FileSettingsSave.GameInstallation) ? 4 : 2), true);
                        DisableButtonFRAG = false;
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                    }

                    if (Firewall())
                    { ButtonsColorSet(ButtonFirewallRulesCheck, 1, true); }
                    else
                    { ButtonsColorSet(ButtonFirewallRulesCheck, 3, true); }
                }
                else
                { ButtonsColorSet(ButtonFirewallRulesCheck, 3, true); }
            }
        }
        ///<summary>Button: Firewall Rules Add All</summary>
        private void ButtonFirewallRulesAddAll_Click(object sender, EventArgs e)
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
                        FileSettingsSave.FirewallLauncherStatus = "Excluded";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddLauncher, 3, false);
                        FileSettingsSave.FirewallLauncherStatus = "Error";
                    }
                    /* Game */
                    if (ButtonEnabler(2, 0))
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddGame, 1, true);
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 2, true);
                        DisableButtonFRRG = false;
                        FileSettingsSave.FirewallGameStatus = "Excluded";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddGame, 3, false);
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                        FileSettingsSave.FirewallGameStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();

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
        }
        ///<summary>Button: Firewall Rules Add Launcher</summary>
        private void ButtonFirewallRulesAddLauncher_Click(object sender, EventArgs e)
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
                        FileSettingsSave.FirewallLauncherStatus = "Excluded";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddLauncher, 3, false);
                        ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 3, false);
                        FileSettingsSave.FirewallLauncherStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();
                }
                else
                {
                    ButtonsColorSet(ButtonFirewallRulesAddLauncher, 3, false);
                }
            }
        }
        ///<summary>Button: Firewall Rules Add Game</summary>
        private void ButtonFirewallRulesAddGame_Click(object sender, EventArgs e)
        {
            if (!DisableButtonFRAG)
            {
                DisableButtonFRAG = true;

                if (ButtonEnabler(4, 20))
                {
                    ButtonsColorSet(ButtonFirewallRulesAddGame, 2, true);

                    /* Remove Old Game Path and Cache Location Just in Case for Windows Defender */
                    if (!string.IsNullOrWhiteSpace(FileSettingsSave.GameInstallationOld))
                    {
                        if (ButtonEnabler(3, 1))
                        {
                            if (string.IsNullOrWhiteSpace(CacheOldGameLocation))
                            {
                                CacheOldGameLocation = FileSettingsSave.GameInstallationOld;
                            }
                            FileSettingsSave.GameInstallationOld = string.Empty;
                        }
                    }

                    /* Game */
                    if (ButtonEnabler(2, 0))
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddGame, 1, true);
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 2, true);
                        DisableButtonFRRG = false;
                        FileSettingsSave.FirewallGameStatus = "Excluded";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddGame, 3, false);
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                        FileSettingsSave.FirewallGameStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();
                }
                else
                {
                    ButtonsColorSet(ButtonFirewallRulesAddGame, 3, false);
                }
            }
        }
        ///<summary>Button: Firewall Rules Remove All</summary>
        private void ButtonFirewallRulesRemoveAll_Click(object sender, EventArgs e)
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
                        FileSettingsSave.FirewallLauncherStatus = "Removed";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddLauncher, 3, false);
                        ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 3, false);
                        FileSettingsSave.FirewallLauncherStatus = "Error";
                    }
                    /* Game */
                    if (ButtonEnabler(2, 1))
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddGame, 2, true);
                        DisableButtonFRAG = true;
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 1, true);
                        FileSettingsSave.FirewallGameStatus = "Removed";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddGame, 3, false);
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                        FileSettingsSave.FirewallGameStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();

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
        }
        ///<summary>Button: Firewall Rules Remove Launcher</summary>
        private void ButtonFirewallRulesRemoveLauncher_Click(object sender, EventArgs e)
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
                        FileSettingsSave.FirewallLauncherStatus = "Removed";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddLauncher, 3, false);
                        ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 3, false);
                        FileSettingsSave.FirewallLauncherStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();
                }
                else
                {
                    ButtonsColorSet(ButtonFirewallRulesRemoveLauncher, 3, false);
                }
            }
        }
        ///<summary>Button: Firewall Rules Remove Game</summary>
        private void ButtonFirewallRulesRemoveGame_Click(object sender, EventArgs e)
        {
            if (!DisableButtonFRRG)
            {
                DisableButtonFRRG = true;

                if (ButtonEnabler(4, 20))
                {
                    ButtonsColorSet(ButtonFirewallRulesRemoveGame, 0, true);
                    /* Remove Old Game Path and Cache Location Just in Case for Windows Defender */
                    if (!string.IsNullOrWhiteSpace(FileSettingsSave.GameInstallationOld))
                    {
                        if (ButtonEnabler(3, 1))
                        {
                            if (string.IsNullOrWhiteSpace(CacheOldGameLocation))
                            {
                                CacheOldGameLocation = FileSettingsSave.GameInstallationOld;
                            }
                            FileSettingsSave.GameInstallationOld = string.Empty;
                        }
                    }

                    /* Game */
                    if (ButtonEnabler(2, 1))
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddGame, 2, true);
                        DisableButtonFRAG = false;
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 1, true);
                        FileSettingsSave.FirewallGameStatus = "Removed";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFirewallRulesAddGame, 3, false);
                        ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                        FileSettingsSave.FirewallGameStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();
                }
                else
                {
                    ButtonsColorSet(ButtonFirewallRulesRemoveGame, 3, false);
                }
            }
        }
        ///<summary>Button: Defender Exclusion API</summary>
        private void ButtonDefenderExclusionAPI_Click(object sender, EventArgs e)
        {
            if (!DisableButtonDRAPI)
            {
                if (EnableInsiderDeveloper.Allowed() || (WindowsProductVersion.GetWindowsNumber() >= 10 &&
                    (MessageBox.Show(null, "There has been reports that some users are not able to run Windows Defender Checks." +
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
        }
        ///<summary>Button: Defender Exclusion Check</summary>
        private void ButtonDefenderExclusionCheck_Click(object sender, EventArgs e)
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
                        CacheOldGameLocation != FileSettingsSave.GameInstallation)
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
                            (CacheOldGameLocation != FileSettingsSave.GameInstallation) ? 4 : 2), true);
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
        }
        ///<summary>Button: Defender Exclusion Add All</summary>
        private void ButtonDefenderExclusionAddAll_Click(object sender, EventArgs e)
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
                        FileSettingsSave.DefenderLauncherStatus = "Excluded";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 3, false);
                        FileSettingsSave.DefenderLauncherStatus = "Error";
                    }
                    /* Game */
                    if (ButtonEnabler(2, 3))
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddGame, 1, true);
                        ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 2, true);
                        DisableButtonDRRG = false;
                        FileSettingsSave.DefenderGameStatus = "Excluded";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddGame, 3, false);
                        ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                        FileSettingsSave.DefenderGameStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();

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
        }
        ///<summary>Button: Defender Exclusion Add Launcher</summary>
        private void ButtonDefenderExclusionAddLauncher_Click(object sender, EventArgs e)
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
                        FileSettingsSave.DefenderLauncherStatus = "Excluded";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 3, false);
                        ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 3, false);
                        FileSettingsSave.DefenderLauncherStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();
                }
                else
                {
                    ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 3, false);
                }
            }
        }
        ///<summary>Button: Defender Exclusion Add Game</summary>
        private void ButtonDefenderExclusionAddGame_Click(object sender, EventArgs e)
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
                        FileSettingsSave.DefenderGameStatus = "Excluded";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddGame, 3, false);
                        ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                        FileSettingsSave.DefenderGameStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();
                }
                else
                {
                    ButtonsColorSet(ButtonDefenderExclusionAddGame, 3, false);
                }
            }
        }
        ///<summary>Button: Defender Exclusion Remove All</summary>
        private void ButtonDefenderExclusionRemoveAll_Click(object sender, EventArgs e)
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
                        FileSettingsSave.DefenderLauncherStatus = "Removed";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 3, false);
                        ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 3, false);
                        FileSettingsSave.DefenderLauncherStatus = "Error";
                    }
                    /* Game */
                    if (ButtonEnabler(2, 4))
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddGame, 2, true);
                        DisableButtonDRAG = true;
                        ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 1, true);
                        FileSettingsSave.DefenderGameStatus = "Removed";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddGame, 3, false);
                        ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                        FileSettingsSave.DefenderGameStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();

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
        }
        ///<summary>Button: Defender Exclusion Remove Launcher</summary>
        private void ButtonDefenderExclusionRemoveLauncher_Click(object sender, EventArgs e)
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
                        FileSettingsSave.DefenderLauncherStatus = "Removed";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddLauncher, 3, false);
                        ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 3, false);
                        FileSettingsSave.DefenderLauncherStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();
                }
                else
                {
                    ButtonsColorSet(ButtonDefenderExclusionRemoveLauncher, 3, false);
                }
            }
        }
        ///<summary>Button: Defender Exclusion Remove Game</summary>
        private void ButtonDefenderExclusionRemoveGame_Click(object sender, EventArgs e)
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
                        FileSettingsSave.DefenderGameStatus = "Removed";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonDefenderExclusionAddGame, 3, false);
                        ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                        FileSettingsSave.DefenderGameStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();
                }
                else
                {
                    ButtonsColorSet(ButtonDefenderExclusionRemoveGame, 3, false);
                }
            }
        }
        ///<summary>Button: File or Folder Permisson Check</summary>
        private void ButtonFolderPermissonCheck_Click(object sender, EventArgs e)
        {
            if (!DisableButtonPRC)
            {
                if (!ButtonEnabler(6, 6))
                {
                    ButtonsColorSet(ButtonFolderPermissonSet, 2, true);
                    DisableButtonPRAA = false;
                }
                else { ButtonsColorSet(ButtonFolderPermissonSet, 1, false); }

                ButtonsColorSet(ButtonFolderPermissonCheck, 1, false);
            }
        }
        ///<summary>Button: Firewall Rules Add Launcher</summary>
        private void ButtonFolderPermissonSet_Click(object sender, EventArgs e)
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
                        FileSettingsSave.FilePermissionStatus = "Set";
                    }
                    else
                    {
                        ButtonsColorSet(ButtonFolderPermissonSet, 3, false);
                        FileSettingsSave.FilePermissionStatus = "Error";
                    }

                    FileSettingsSave.SaveSettings();
                }
                else
                {
                    ButtonsColorSet(ButtonFolderPermissonSet, 3, false);
                }
            }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonClose_MouseDown(object sender, EventArgs e)
        {
            ButtonClose.BackgroundImage = Theming.CloseButtonClick;
        }

        private void ButtonClose_MouseEnter(object sender, EventArgs e)
        {
            ButtonClose.BackgroundImage = Theming.CloseButtonHover;
        }

        private void ButtonClose_MouseLeaveANDMouseUp(object sender, EventArgs e)
        {
            ButtonClose.BackgroundImage = Theming.CloseButton;
        }
        ///<summary>Theming, Function, EventHandlers, Etc. Meant to load critial functions before the forms loads</summary>
        private void SetVisuals()
        {
            /*******************************/
            /* Set Initial position & Icon  /
            /*******************************/

            FunctionStatus.CenterParent(this);

            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Text = "Security Center - SBRW Launcher: v" + Application.ProductVersion;

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            VersionLabel.Text = "Version : v" + Application.ProductVersion;

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            BackgroundImage = Theming.SecurityCenterScreen;
            TransparencyKey = Theming.SecurityCenterScreenTransparencyKey;
            ButtonClose.BackgroundImage = Theming.CloseButton;

            TextWindowsFirewall.ForeColor = Theming.FivithTextForeColor;
            TextWindowsDefender.ForeColor = Theming.FivithTextForeColor;
            TextFolderPermissions.ForeColor = Theming.FivithTextForeColor;
            VersionLabel.ForeColor = Theming.FivithTextForeColor;

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

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            float MainFontSize = UnixOS.Detected() ? 9f : 9f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = UnixOS.Detected() ? 8f : 8f * 96f / CreateGraphics().DpiY;

            Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            /* Text */
            TextWindowsFirewall.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            TextWindowsDefender.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            TextFolderPermissions.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            VersionLabel.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Regular);
            /* Firewall */
            ButtonFirewallRulesAPI.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesCheck.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesAddAll.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesAddLauncher.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesAddGame.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesRemoveAll.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesRemoveLauncher.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesRemoveGame.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            /* Defender */
            ButtonFirewallRulesAPI.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesCheck.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonFirewallRulesAddAll.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonDefenderExclusionAddLauncher.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonDefenderExclusionAddGame.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonDefenderExclusionRemoveAll.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonDefenderExclusionRemoveLauncher.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonDefenderExclusionRemoveGame.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            /* File/Folder Permission */
            ButtonFolderPermissonCheck.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            ButtonFolderPermissonSet.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);

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
        }
    }
}
