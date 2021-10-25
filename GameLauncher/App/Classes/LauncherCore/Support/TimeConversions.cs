using GameLauncher.App.Classes.LauncherCore.Logger;
using System;

namespace GameLauncher.App.Classes.LauncherCore.Support
{
    class TimeConversions
    {
        public static string FormatFileSize(long byteCount, bool si = true)
        {
            try
            {
                int unit = si ? 1000 : 1024;
                if (byteCount < unit) return byteCount + " B";
                int exp = (int)(Math.Log(byteCount) / Math.Log(unit));
                String pre = (si ? "kMGTPE" : "KMGTPE")[exp - 1] + (si ? "" : "i");
                return String.Format("{0}{1}B", Convert.ToDecimal(byteCount / Math.Pow(unit, exp)).ToString("0.00"), pre);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Format File Size", null, Error, null, true);
                return String.Empty;
            }
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

                int rDays = Convert.ToInt32(timeSpan.Days.ToString()) + 1;
                int rHours = Convert.ToInt32(timeSpan.Hours.ToString()) + 1;
                int rMinutes = Convert.ToInt32(timeSpan.Minutes.ToString()) + 1;
                int rSeconds = Convert.ToInt32(timeSpan.Seconds.ToString()) + 1;

                if (rDays > 1) return rDays.ToString() + " days remaining";
                if (rHours > 1) return rHours.ToString() + " hours remaining";
                if (rMinutes > 1) return rMinutes.ToString() + " minutes remaining";
                if (rSeconds > 1) return rSeconds.ToString() + " seconds remaining";

                return "Just now";
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Estimate Finish Time", null, Error, null, true);
                return "N/A";
            }
        }

        public static String RelativeTime(int TimeSeconds)
        {
            int NoCalculus;
            if (TimeSeconds >= 2592000)
            {
                NoCalculus = TimeSeconds / 2592000;
                return NoCalculus == 1 ? "1 Month" : NoCalculus + " Months";
            }
            else if (TimeSeconds >= 86400)
            {
                NoCalculus = TimeSeconds / 86400;
                return NoCalculus == 1 ? "1 Day" : NoCalculus + " Days";
            }
            else if (TimeSeconds >= 3600)
            {
                NoCalculus = TimeSeconds / 3600;
                return NoCalculus == 1 ? "1 Hour" : NoCalculus + " Hours";
            }
            else if (TimeSeconds >= 60)
            {
                NoCalculus = TimeSeconds / 60;
                return NoCalculus == 1 ? "1 Minute" : NoCalculus + " Minute";
            }
            else if (TimeSeconds >= 0)
            {
                return TimeSeconds == 1 ? "1 Second" : TimeSeconds + " Seconds";
            }
            else
            {
                return "Outta Time";
            }
        }
    }
}