namespace GameLauncher.App.Classes.InsiderKit
{
    class EnableInsider
    {
        public static bool Insider = true;

        /* Current month, day, year (2 digits), and letter! Ex: 12-15-20-A */
        public static string InsiderBuildNumber = "04-12-21-F";

        public static bool ShouldIBeAnInsider()
        {
            return Insider;
        }

        public static string BuildNumber()
        {
            return InsiderBuildNumber;
        }

    }
}
