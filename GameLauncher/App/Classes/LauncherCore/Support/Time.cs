using System;

namespace GameLauncher.App.Classes.LauncherCore.Support
{
    class Time
    {
        /* Current Time Stamp  */
        public static string GetTime(string Mode)
        {
            try
            {
                if (Mode == "Date")
                {
                    return DateTime.Now.ToString("MM/dd/yyyy");
                }
                else if (Mode == "Time")
                {
                    return DateTime.Now.ToString("HH:mm:ss.ff");
                }
                else if (Mode == "Log")
                {
                    return DateTime.Now.ToString("MM dd yyyy - HH mm.ss");
                }
                else if (Mode == "Now - Local Time")
                {
                    return DateTime.Now.ToString();
                }
                else if (Mode == "Now - UTC Time (Offset)")
                {
                    return DateTimeOffset.Now.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return "Unknown-Time-Date";
            }
        }

        /* Old Removed Code Moved to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/e26ab9bc5f9ce69da5d2294cc65aea69ab88c4fb/Time.cs */
    }
}
