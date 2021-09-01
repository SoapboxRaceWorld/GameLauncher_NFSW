using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using System.Drawing;

namespace GameLauncher.App.Classes.LauncherCore.Support
{
    class SecurityCenter
    {
        public static SecurityCenterCodes SecurityCenterSavedCodes()
        {
            if (UnixOS.Detected())
            {
                return SecurityCenterCodes.Unix;
            }
            else if (FileSettingsSave.FirewallLauncherStatus == "Excluded" && FileSettingsSave.FirewallGameStatus == "Excluded")
            {
                return SecurityCenterCodes.Firewall_Updated;
            }
            else if ((FileSettingsSave.FirewallLauncherStatus == "Excluded" && FileSettingsSave.FirewallGameStatus == "Not Excluded") ||
                (FileSettingsSave.FirewallLauncherStatus == "Unknown" && FileSettingsSave.FirewallGameStatus == "Unknown") ||
                (FileSettingsSave.FirewallLauncherStatus == "Removed" && FileSettingsSave.FirewallGameStatus == "Removed"))
            {
                return SecurityCenterCodes.Firewall_Outdated;
            }
            else if ((FileSettingsSave.FirewallLauncherStatus == "Error" && FileSettingsSave.FirewallGameStatus == "Error") ||
                (FileSettingsSave.FirewallLauncherStatus == "Not Supported" && FileSettingsSave.FirewallGameStatus == "Not Supported"))
            {
                return SecurityCenterCodes.Firewall_Error;
            }
            else if ((WindowsProductVersion.GetWindowsNumber() >= 10) && 
                FileSettingsSave.DefenderLauncherStatus == "Excluded" && FileSettingsSave.DefenderGameStatus == "Excluded")
            {
                return SecurityCenterCodes.Defender_Updated;
            }
            else if ((WindowsProductVersion.GetWindowsNumber() >= 10) &&
                ((FileSettingsSave.DefenderLauncherStatus == "Excluded" && FileSettingsSave.DefenderGameStatus == "Not Excluded") ||
                (FileSettingsSave.DefenderLauncherStatus == "Unknown" && FileSettingsSave.DefenderGameStatus == "Unknown") ||
                (FileSettingsSave.DefenderLauncherStatus == "Removed" && FileSettingsSave.DefenderGameStatus == "Removed")))
            {
                return SecurityCenterCodes.Defender_Outdated;
            }
            else if ((WindowsProductVersion.GetWindowsNumber() >= 10) &&
                ((FileSettingsSave.DefenderLauncherStatus == "Error" && FileSettingsSave.DefenderGameStatus == "Error") ||
                (FileSettingsSave.DefenderLauncherStatus == "Not Supported" && FileSettingsSave.DefenderGameStatus == "Not Supported")))
            {
                return SecurityCenterCodes.Defender_Error;
            }
            else if (FileSettingsSave.FilePermissionStatus == "Set")
            {
                return SecurityCenterCodes.Permissions_Updated;
            }
            else if (FileSettingsSave.FilePermissionStatus == "Error")
            {
                return SecurityCenterCodes.Permissions_Error;
            }
            else if (FileSettingsSave.FilePermissionStatus == "Not Set")
            {
                return SecurityCenterCodes.Permissions_Outdated;
            }
            else if ((FileSettingsSave.FirewallLauncherStatus == "Not Excluded" && FileSettingsSave.FirewallGameStatus == "Not Excluded") ||
                ((WindowsProductVersion.GetWindowsNumber() >= 10) &&
                FileSettingsSave.DefenderLauncherStatus == "Not Excluded" && FileSettingsSave.DefenderGameStatus == "Not Excluded"))
            {
                return SecurityCenterCodes.Unknown;
            }
            else
            {
                return SecurityCenterCodes.Unknown;
            }
        }
        /// <summary>Returns the Shield Image for Security Panel Button
        /// <code>"0" Regular Colored Image</code>
        /// <code>"1" Clickage Colored Image</code>
        /// <code>"2" Hover Colored Image</code>
        /// </summary>
        /// <param name="ImageState">
        /// <code>"0" Regular Colored Image</code>
        /// <code>"1" Clickage Colored Image</code>
        /// <code>"2" Hover Colored Image</code>
        /// </param>
        /// <returns>Button Image</returns>
        public static Image SecurityCenterIcon(int ImageState)
        {
            switch (SecurityCenterSavedCodes())
            {
                case SecurityCenterCodes.Firewall_Updated:
                case SecurityCenterCodes.Defender_Updated:
                case SecurityCenterCodes.Permissions_Updated:
                    if (ImageState == 1) { return Theming.ShieldButtonSuccessClick; }
                    else if (ImageState == 2) { return Theming.ShieldButtonSuccessHover; }
                    else { return Theming.ShieldButtonSuccess; }
                case SecurityCenterCodes.Firewall_Outdated:
                case SecurityCenterCodes.Defender_Outdated:
                case SecurityCenterCodes.Permissions_Outdated:
                    if (ImageState == 1) { return Theming.ShieldButtonWarningClick; }
                    else if (ImageState == 2) { return Theming.ShieldButtonWarningHover; }
                    else { return Theming.ShieldButtonWarning; }
                case SecurityCenterCodes.Firewall_Error:
                case SecurityCenterCodes.Defender_Error:
                case SecurityCenterCodes.Permissions_Error:
                    if (ImageState == 1) { return Theming.ShieldButtonErrorClick; }
                    else if (ImageState == 2) { return Theming.ShieldButtonErrorHover; }
                    else { return Theming.ShieldButtonError; }
                case SecurityCenterCodes.Unix:
                    if (ImageState == 1) { return Theming.ShieldButtonCheckingClick; }
                    else if (ImageState == 2) { return Theming.ShieldButtonCheckingHover; }
                    else { return Theming.ShieldButtonChecking; }
                default:
                    if (ImageState == 1) { return Theming.ShieldButtonUnknownClick; }
                    else if (ImageState == 2) { return Theming.ShieldButtonUnknownHover; }
                    else { return Theming.ShieldButtonUnknown; }
            }
        }
    }
}
