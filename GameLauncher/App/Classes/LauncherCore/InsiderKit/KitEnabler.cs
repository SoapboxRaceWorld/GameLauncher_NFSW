using GameLauncher.App.Classes.LauncherCore.Languages.Visual_Forms;
using SBRWCore.Classes.Launcher;

namespace GameLauncher.App.Classes.InsiderKit
{
    /* This sets Build Number Information */
    class InsiderInfo
    {
        /* Current month, day, year (2 digits), and letter! Ex: 12-15-20-A */
        /* If a second build gets release within the same day bump letter version up (No R2 or D2)*/
        private static readonly string InsiderBuildNumber = "10-25-21-C";

        public static string BuildNumberOnly()
        {
            return Live_Cache.Launcher_Version_Insider = InsiderBuildNumber;
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

            return Translations.Database("KitEnabler_Stable") + ": " + InsiderBuildNumber;
        }
    }

    /* This is only used for Developers (Bypasses Most Checks) */
    class EnableInsiderDeveloper
    {
        private static readonly bool Enabled = false;

        public static bool Allowed()
        {
            return Enabled;
        }
    }

    /* This is only used for Beta Testers (Treated like a Public Release) */
    class EnableInsiderBetaTester
    {
        private static bool Enabled = true;

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
