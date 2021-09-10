using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.LauncherUpdater;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Support;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GameLauncher.App.Classes.LauncherCore.Validator.VerifyTrust
{
    class IsExeVerified
    {
        public static bool LauncherSigned = false;

        public static void Check()
        {
            LauncherSigned = CheckExeVerified.Signed(Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameLauncher)));
            Log.Info("SIGNED: " + LauncherSigned);

            Log.Info("LAUNCHER UPDATER: Moved to Function");
            /* (Start Process) Check If Updater Exists or Requires an Update */
            UpdaterExecutable.Check();
        }
    }

    class CheckExeVerified
    {
        [DllImport("wintrust.dll")]
        private static extern int WinVerifyTrust(IntPtr hwnd, [MarshalAs(UnmanagedType.LPStruct)] Guid pgActionID, ref WINTRUST_DATA pWVTData);

        private const int WTD_UI_NONE = 2;
        private const int WTD_REVOKE_NONE = 0;
        private const int WTD_CHOICE_FILE = 1;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        private static readonly Guid WINTRUST_ACTION_GENERIC_VERIFY_V2 = new Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");

        public static bool Signed(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Log.Error("SIGNED: File Path cannot be Null");
            }
            else
            {
                try
                {
                    var File = new WINTRUST_FILE_INFO
                    {
                        cbStruct = Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)),
                        pcwszFilePath = filePath
                    };

                    var Data = new WINTRUST_DATA
                    {
                        cbStruct = Marshal.SizeOf(typeof(WINTRUST_DATA)),
                        dwUIChoice = WTD_UI_NONE,
                        dwUnionChoice = WTD_CHOICE_FILE,
                        fdwRevocationChecks = WTD_REVOKE_NONE,
                        pFile = Marshal.AllocHGlobal(File.cbStruct)
                    };
                    Marshal.StructureToPtr(File, Data.pFile, false);

                    int hr;
                    try
                    {
                        hr = WinVerifyTrust(INVALID_HANDLE_VALUE, WINTRUST_ACTION_GENERIC_VERIFY_V2, ref Data);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(Data.pFile);
                    }

                    return hr == 0;
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("SIGNED", null, Error, null, true);
                    return false;
                }
            }

            return false;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WINTRUST_FILE_INFO
        {
            public int cbStruct;
            public string pcwszFilePath;
            public IntPtr hFile;
            public IntPtr pgKnownSubject;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        private struct WINTRUST_DATA
        {
            public int cbStruct;
            public IntPtr pPolicyCallbackData;
            public IntPtr pSIPClientData;
            public int dwUIChoice;
            public int fdwRevocationChecks;
            public int dwUnionChoice;
            public IntPtr pFile;
            public int dwStateAction;
            public IntPtr hWVTStateData;
            public IntPtr pwszURLReference;
            public int dwProvFlags;
            public int dwUIContext;
            public IntPtr pSignatureSettings;
        }
    }

    /* Based on the Following Concepts (Thanks to Those who Explored this Function)
     * https://web.archive.org/web/20130417053308/http://geekswithblogs.net/robp/archive/2007/05/04/112250.aspx
     * https://stackoverflow.com/questions/6596327/how-to-check-if-a-file-is-signed-in-c
     * https://stackoverflow.com/questions/667017/how-to-check-if-a-file-has-a-digital-signature
     * https://stackoverflow.com/questions/4345962/verify-whether-an-executable-is-signed-or-not-signtool-used-to-sign-that-exe
     */
}
