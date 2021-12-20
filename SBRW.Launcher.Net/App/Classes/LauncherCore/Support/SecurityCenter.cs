using SBRW.Launcher.App.Classes.LauncherCore.Visuals;
using SBRW.Launcher.Core.Extension.Security_;
using SBRW.Launcher.Core.Extra.Conversion_;
using System.Drawing;

namespace SBRW.Launcher.App.Classes.LauncherCore.Support
{
    class SecurityCenter
    {
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
            switch (Security_Codes_Reference.Check())
            {
                case SecurityCenterCodes.Unix:
                    if (ImageState == 1) { return Theming.ShieldButtonCheckingClick; }
                    else if (ImageState == 2) { return Theming.ShieldButtonCheckingHover; }
                    else { return Theming.ShieldButtonChecking; }
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
                case SecurityCenterCodes.Firewall_Updated:
                case SecurityCenterCodes.Defender_Updated:
                case SecurityCenterCodes.Permissions_Updated:
                    if (ImageState == 1) { return Theming.ShieldButtonSuccessClick; }
                    else if (ImageState == 2) { return Theming.ShieldButtonSuccessHover; }
                    else { return Theming.ShieldButtonSuccess; }
                default:
                    if (ImageState == 1) { return Theming.ShieldButtonUnknownClick; }
                    else if (ImageState == 2) { return Theming.ShieldButtonUnknownHover; }
                    else { return Theming.ShieldButtonUnknown; }
            }
        }
    }
}
