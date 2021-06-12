using GameLauncher.App.Classes.LauncherCore.Global;
using System;

namespace GameLauncher.App.Classes.LauncherCore.Visuals
{
    class TimeConversions
    {
        public static string FormatFileSize(long byteCount, bool si = true)
        {
            int unit = si ? 1000 : 1024;
            if (byteCount < unit) return byteCount + " B";
            int exp = (int)(Math.Log(byteCount) / Math.Log(unit));
            String pre = (si ? "kMGTPE" : "KMGTPE")[exp - 1] + (si ? "" : "i");
            return String.Format("{0}{1}B", Convert.ToDecimal(byteCount / Math.Pow(unit, exp)).ToString("0.00"), pre);
        }

        public static string EstimateFinishTime(long current, long total, DateTime keyValue)
        {
            try
            {
                var num = current / (double)total;
                if (num < 0.00185484899838312)
                {
                    return "Calculating";
                }

                var now = DateTime.Now - keyValue;
                var timeSpan = TimeSpan.FromTicks((long)(now.Ticks / num)) - now;

                int rHours = Convert.ToInt32(timeSpan.Hours.ToString()) + 1;
                int rMinutes = Convert.ToInt32(timeSpan.Minutes.ToString()) + 1;
                int rSeconds = Convert.ToInt32(timeSpan.Seconds.ToString()) + 1;

                if (rHours > 1) return rHours.ToString() + " hours remaining";
                if (rMinutes > 1) return rMinutes.ToString() + " minutes remaining";
                if (rSeconds > 1) return rSeconds.ToString() + " seconds remaining";

                return "Just now";
            }
            catch
            {
                return "N/A";
            }
        }

        public static String RelativeTime(int seconds) 
        {
            int calcs;

            if (seconds >= 2592000) 
            {
                calcs = seconds / 2592000;
                return calcs == 1 ? "1 Month" : calcs + " Months";
            }
            else if (seconds >= 86400) 
            {
                calcs = seconds / 86400;
                return calcs == 1 ? "1 Day" : calcs + " Days";
            }
            else if (seconds >= 3600) 
            {
                calcs = seconds / 3600;
                return calcs == 1 ? "1 Hour" : calcs + " Hours";
            }
            else if (seconds >= 60) 
            {
                calcs = seconds / 60;
                return calcs == 1 ? "1 Minute" : calcs + " Minute";
            }
            else if (seconds >= 0) 
            {
                return seconds == 1 ? "1 Second" : seconds + " Seconds";
            }
            else
            {
                return "Unknown";
            }
        }

        public static void MUFRTime(string Mode)
        {
            if (Mode == "0")
            {
                if (AntiCheat.SpeedTicket == 0)
                {
                    AntiCheat.SpeedTicket++;
                    AntiCheat.IAmSpeed = 250;
                }
                else if (AntiCheat.SpeedTicket == 1)
                {
                    AntiCheat.SpeedTicket++;
                    AntiCheat.IAmSpeed = 125;
                }

                int seconds = InformationCache.RestartTimer;

                if (seconds >= 2592000)
                {
                    InformationCache.RestartTimer = seconds - 2592000;
                }
                else if (seconds >= 86400)
                {
                    InformationCache.RestartTimer = seconds - 86400;
                }
                else if (seconds >= 3600)
                {
                    InformationCache.RestartTimer = seconds - 3600;
                }
                else if (seconds >= 1800)
                {
                    InformationCache.RestartTimer = seconds - 1800;
                }
                else if (seconds >= 900)
                {
                    InformationCache.RestartTimer = seconds - 900;
                }
                else if (seconds >= 600)
                {
                    InformationCache.RestartTimer = seconds - 600;
                }
                else if (seconds >= 300)
                {
                    InformationCache.RestartTimer = seconds - 300;
                }
                else if (seconds >= 60)
                {
                    InformationCache.RestartTimer = seconds - 60;
                }
                else if (seconds >= 0)
                {
                    InformationCache.RestartTimer = seconds - 30;
                }
                else
                {
                    InformationCache.RestartTimer = 0;
                }
            }
            else
            {
                InformationCache.RestartTimer = 0;
            }
        }
    }
}