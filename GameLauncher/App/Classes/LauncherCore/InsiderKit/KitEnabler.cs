using GameLauncher.App.Classes.LauncherCore.Languages.Visual_Forms;

namespace GameLauncher.App.Classes.InsiderKit
{
    /* This sets Build Number Information */
    class InsiderInfo
    {
        /* Current month, day, year (2 digits), and letter! Ex: 12-15-20-A */
        /* If a second build gets release within the same day bump letter version up (No R2 or D2)*/
        public static string InsiderBuildNumber = "11-06-21-E";

        public static string BuildNumberOnly()
        {
            return InsiderBuildNumber;
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
        private static bool Enabled = false;

        public static bool Allowed()
        {
            return Enabled;
        }
    }

    /* This is only used for Beta Testers (Treated like a Public Release) */
    class EnableInsiderBetaTester
    {
        private static bool Enabled = false;

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
