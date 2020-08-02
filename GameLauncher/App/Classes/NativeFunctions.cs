using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameLauncher.App.Classes {
    internal static class Kernel32  {
        [DllImport("kernel32", CharSet = CharSet.Auto, ExactSpelling = false)]
        public static extern int GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);
    }

    internal static class User32 {
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursorFromFile(string filename);

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, string text);
    }
}
