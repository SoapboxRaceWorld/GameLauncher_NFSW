using SBRW.Launcher.RunTime.LauncherCore.Languages.Visual_Forms;
using SBRW.Launcher.Core.Cache;

namespace SBRW.Launcher.RunTime.InsiderKit
{
    /* This sets Build Number Information */
    public class InsiderInfo
    {
        /* Current month, day, year (2 digits), and letter! Ex: 12-15-20-A */
        /* If a second build gets release within the same day bump letter version up (No R2 or D2)*/

        private static string InsiderBuildNumber { get; set; } = "05-18-23-C";

        public static string BuildNumberOnly()
        {
            return Launcher_Value.Launcher_Insider_Version = InsiderBuildNumber;
        }
        
        public static string BuildNumber()
        {
            if (EnableInsiderDeveloper.Allowed())
            {
                return Translations.Database("KitEnabler_Dev") + ": " + InsiderBuildNumber;
            }
            else if (EnableInsiderBetaTester.Allowed())
            {
                return Translations.Database("KitEnabler_Beta") + ": " + InsiderBuildNumber;
            }

            return Translations.Database("KitEnabler_Public") + ": " + InsiderBuildNumber;
        }
    }

    /* This is only used for Developers (Bypasses Most Checks) */
    public class EnableInsiderDeveloper
    {
        private static bool Enabled { get; set; } = false;

        public static bool Allowed()
        {
            return Enabled;
        }
        /// <summary>
        /// User had Opt-In to Use Beta Builds
        /// </summary>
        /// <param name="Opt_In">Takes in Boolean Values</param>
        /// <returns>New Conditional Status</returns>
        public static bool Allowed(bool Opt_In)
        {
            return Enabled = Opt_In;
        }
    }

    /* This is only used for Beta Testers (Treated like a Public Release) */
    public class EnableInsiderBetaTester
    {
        private static bool Enabled { get; set; } = false;

        public static bool Allowed()
        {
            return Enabled;
        }
        /// <summary>
        /// User had Opt-In to Use Beta Builds
        /// </summary>
        /// <param name="Opt_In">Takes in Boolean Values</param>
        /// <returns>New Conditional Status</returns>
        public static bool Allowed(bool Opt_In)
        {
            return Enabled = Opt_In;
        }
    }
}
