using System;

namespace SBRW.Launcher.RunTime.LauncherCore.ModNet
{
    internal class Download_Information_ModNet
    {
        /// <summary>
        /// 
        /// </summary>
        public int Download_Percentage { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string File_Name { get; internal set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public long File_Size_Total { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public long File_Size_Current { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public long File_Size_Remaining { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Start_Time { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime End_Time { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Download_Complete { get; internal set; }
    }
}
