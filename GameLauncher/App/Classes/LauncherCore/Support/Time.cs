using System;
using System.Windows.Forms;

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
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return "Unknown Time/Date";
            }
        }

        /* Count Down Timer */
        public static void SecondsRemaining(int sec)
        {
            if (sec < 1) return;
            DateTime _desired = DateTime.Now.AddSeconds(sec);
            while (DateTime.Now < _desired)
            {
                Application.DoEvents();
            }
        }

        /* Moved "AddMilliseconds" Code to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/15d5ef79b6c5e3dbca112a65b2a77623c5712411/Delay.cs */
    }
}
