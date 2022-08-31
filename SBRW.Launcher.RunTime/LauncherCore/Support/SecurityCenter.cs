using SBRW.Launcher.RunTime.LauncherCore.Visuals;
using SBRW.Launcher.Core.Extension.Security_;
using SBRW.Launcher.Core.Extra.Conversion_;
using SBRW.Launcher.Core.Theme;
using System.Drawing;

namespace SBRW.Launcher.RunTime.LauncherCore.Support
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
                    if (ImageState == 1) { return Image_Icon.Shield_Checking_Click; }
                    else if (ImageState == 2) { return Image_Icon.Shield_Checking_Hover; }
                    else { return Image_Icon.Shield_Checking; }
                case SecurityCenterCodes.Firewall_Outdated:
                case SecurityCenterCodes.Defender_Outdated:
                case SecurityCenterCodes.Permissions_Outdated:
                    if (ImageState == 1) { return Image_Icon.Shield_Warning_Click; }
                    else if (ImageState == 2) { return Image_Icon.Shield_Warning_Hover; }
                    else { return Image_Icon.Shield_Warning; }
                case SecurityCenterCodes.Firewall_Error:
                case SecurityCenterCodes.Defender_Error:
                case SecurityCenterCodes.Permissions_Error:
                    if (ImageState == 1) { return Image_Icon.Shield_Error_Click; }
                    else if (ImageState == 2) { return Image_Icon.Shield_Error_Hover; }
                    else { return Image_Icon.Shield_Error; }
                case SecurityCenterCodes.Firewall_Updated:
                case SecurityCenterCodes.Defender_Updated:
                case SecurityCenterCodes.Permissions_Updated:
                    if (ImageState == 1) { return Image_Icon.Shield_Success_Click; }
                    else if (ImageState == 2) { return Image_Icon.Shield_Success_Hover; }
                    else { return Image_Icon.Shield_Success; }
                default:
                    if (ImageState == 1) { return Image_Icon.Shield_Unknown_Click; }
                    else if (ImageState == 2) { return Image_Icon.Shield_Unknown_Hover; }
                    else { return Image_Icon.Shield_Unknown; }
            }
        }
    }
}
