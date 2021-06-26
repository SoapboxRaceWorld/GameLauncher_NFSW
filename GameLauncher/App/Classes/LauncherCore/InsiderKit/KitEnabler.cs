namespace GameLauncher.App.Classes.InsiderKit
{
    /* This sets Build Number Information */
    class InsiderInfo
    {
        /* Current month, day, year (2 digits), and letter! Ex: 12-15-20-A */
        public static string InsiderBuildNumber = "06-26-21-A";

        public static string BuildNumberOnly()
        {
            return InsiderBuildNumber;
        }

        public static string BuildNumber()
        {
            if (EnableInsiderDeveloper.Allowed())
            {
                return "DEV Build Date: " + InsiderBuildNumber;
            }
            else if (EnableInsiderBetaTester.Allowed())
            {
                return  "BETA Build Date: " + InsiderBuildNumber;
            }

            return "Build Date: " + InsiderBuildNumber;
        }
    }

    /* This is only used for Developers (Bypasses Most Checks) */
    class EnableInsiderDeveloper
    {
        public static bool Enabled = false;

        public static bool Allowed()
        {
            return Enabled;
        }
    }

    /* This is only used for Beta Testers (Treated like a Public Release) */
    class EnableInsiderBetaTester
    {
        public static bool Enabled = true;

        public static bool Allowed()
        {
            return Enabled;
        }
    }
}
