using SBRW.Launcher.RunTime.InsiderKit;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.ModNet;
using SBRW.Launcher.RunTime.LauncherCore.Support;
using SBRW.Launcher.RunTime.SystemPlatform.Unix;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Hash_;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.Time_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extra.File_;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SBRW.Launcher.Core.Theme;
using System.Net.Cache;
using SBRW.Launcher.App.UI_Forms.Main_Screen;

namespace SBRW.Launcher.App.UI_Forms.VerifyHash_Screen
{
    public partial class Screen_Verify_Hash : Form
    {
        private static bool IsVerifyHashOpen { get; set; }
        /* VerifyHash */
        public string[][] ScannedHashes { get; set; } = new string[0][];
        public int FilesToScan { get; set; }
        public int BadFiles { get; set; }
        public int TotalFilesScanned { get; set; }
        public int CurrentCount { get; set; }
        public int RedownloadedCount { get; set; }
        public int RedownloadErrorCount { get; set; }
        public List<string> InvalidFileList { get; set; } = new List<string>();
        public List<string> ValidFileList { get; set; } = new List<string>();
        public string FinalCDNURL { get; set; } = string.Empty;
        public static Thread? StartScan { get; set; }
        public bool IsScanning { get; set; }
        public static bool ForceStopScan { get; set; }
        public static string CurrentDownloadingFile { get; set; } = string.Empty;
        public static int DeletionError { get; set; }
        public static bool DeletionErrorBypass { get; set; }
        public static bool StillDownloading { get; set; }
        public static bool DownloadErrorEncountered { get; set; }

        public static void OpenScreen()
        {
            if (IsVerifyHashOpen || Application.OpenForms["Screen_Verify_Hash"] != null)
            {
                if (!Application.OpenForms["Screen_Verify_Hash"].IsDisposed) { Application.OpenForms["Screen_Verify_Hash"].Activate(); }
            }
            else
            {
                try { new Screen_Verify_Hash().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Verify Hash Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Verify Hash Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public Screen_Verify_Hash()
        {
            IsVerifyHashOpen = true;
            Presence_Launcher.Status(24);
            InitializeComponent();
            Icon = FormsIcon.Retrive_Icon();
            SetVisuals();
            this.Closing += (x, CloseForm) =>
            {
                if (IsScanning)
                {
                    if (MessageBox.Show("Do you really want to exit the VerifyHash process?", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        CloseForm.Cancel = true;
                    }
                    else
                    {
                        Presence_Launcher.Status(22);
                        IsVerifyHashOpen = false;
                        GameScanner(false);
                        Log_Verify.Stop = true;
                    }
                }
                else
                {
                    IsVerifyHashOpen = false;
                    Presence_Launcher.Status(22);
                    Log_Verify.Stop = true;
                }

                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            };
        }

        private void VerifyHash_Load(object sender, EventArgs e)
        {
            VersionLabel.Text = "Version: " + Application.ProductVersion;
            Log.Core("VERIFY HASH: Opened");

            if (!FunctionStatus.IsVerifyHashDisabled)
            {
                Log_Verify.Start();

                /* Clean up previous logs and start logging */
                string[] filestocheck = new string[] { "checksums.dat", "validfiles.dat", "invalidfiles.dat", "Verify.log" };
                foreach (String file in filestocheck)
                {
                    if (File.Exists(file))
                    {
                        try { File.Delete(file); }
                        catch (Exception Error)
                        {
                            DeletionError++;
                            Log_Verify.Error("File: " + file + " Error: " + Error.Message);
                            Log_Verify.ErrorIC("File: " + file + " Error: " + Error.HResult);
                            Log_Verify.ErrorFR("File: " + file + " Error: " + Error.ToString());
                        }
                    }
                }

                Log_Verify.Info("VERIFYHASH: Checking Characters in URL");
                if (Save_Settings.Live_Data.Launcher_CDN.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    FinalCDNURL = Save_Settings.Live_Data.Launcher_CDN.TrimEnd(charsToTrim);
                    Log_Verify.Info("VERIFYHASH: Trimed end of CDN URL -> " + FinalCDNURL);
                }
                else
                {
                    FinalCDNURL = Save_Settings.Live_Data.Launcher_CDN;
                    Log_Verify.Info("VERIFYHASH: Choosen CDN URL -> " + FinalCDNURL);
                }
            }
            else
            {
                StartScanner.Enabled = false;
            }
        }

        public void GameScanner(bool startScan)
        {
            if (startScan)
            {
                if (!FunctionStatus.IsVerifyHashDisabled)
                {
                    StartScan = new Thread(new ThreadStart(StartGameScanner))
                    {
                        Name = "FileScanner",
                        IsBackground = true
                    };

                    IsScanning = true;
                    Log.Info("VERIFY HASH: Started Scanner");
                    StartScan.Start();
                }
                else
                {
                    MessageBox.Show("Verify Hash has already done a Full Scan this run.\n" +
                        "Please restart the GameLauncher to do a New Scan.", "VerifyHash", MessageBoxButtons.OK);
                }
            }
            else if (!startScan)
            {
                ForceStopScan = true;
                IsScanning = false;
                Log.Info("VERIFY HASH: Stopped Scanner");
                if (DownloadErrorEncountered)
                {
                    DownloadErrorEncountered = false;

                    if (MessageBox.Show("Verify Hash has encountered Download Errors.\n" +
                        "Would you like to Open Verify.Log", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (File.Exists(Log_Location.LogVerify))
                        {
                            Process.Start(Log_Location.LogCurrentFolder);
                            Process.Start(Log_Location.LogVerify);
                        }
                    }
                }
            }
        }

        private void StartGameScanner()
        {
            Presence_Launcher.Status(25);
            Log.Info("VERIFY HASH: Checking and Deleting '.orig' Files and Symbolic Folders");
            ScanProgressText.SafeInvokeAction(() => ScanProgressText.Text = "Removing any '.orig' Files in Game Directory");

            /* START Show Warning Text */
            VerifyHashText.SafeInvokeAction(() =>
            {
                VerifyHashText.ForeColor = Color_Winform.Warning_Text_Fore_Color;
                VerifyHashText.Text = "Warning:\nIf '.orig' Files Exist\nIt will be Removed Permanently";
            });
            /* END Show Warning Text */

            try
            {
                DirectoryInfo InstallationDirectory = new DirectoryInfo(Save_Settings.Live_Data.Game_Path);

                foreach (DirectoryInfo FoldersWeFound in InstallationDirectory.GetDirectories())
                {
                    if (!ForceStopScan)
                    {
                        foreach (FileInfo FoundFile in InstallationDirectory.EnumerateFiles("*.orig", SearchOption.AllDirectories))
                        {
                            if (!ForceStopScan)
                            {
                                try
                                {
                                    FoundFile.Delete();
                                    Log_Verify.Deleted("File: " + FoundFile.Name);
                                }
                                catch (Exception Error)
                                {
                                    DeletionError++;
                                    Log_Verify.Error("File: " + FoundFile.Name + " Error: " + Error.Message);
                                    Log_Verify.ErrorIC("File: " + FoundFile.Name + " Error: " + Error.HResult);
                                    Log_Verify.ErrorFR("File: " + FoundFile.Name + " Error: " + Error.ToString());
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        foreach (FileInfo FoundFile in FoldersWeFound.EnumerateFiles("*.orig", SearchOption.AllDirectories))
                        {
                            if (!ForceStopScan)
                            {
                                try
                                {
                                    FoundFile.Delete();
                                    Log_Verify.Deleted("File: " + FoundFile.Name);
                                }
                                catch (Exception Error)
                                {
                                    DeletionError++;
                                    Log_Verify.Error("File: " + FoundFile.Name + " Error: " + Error.Message);
                                    Log_Verify.ErrorIC("File: " + FoundFile.Name + " Error: " + Error.HResult);
                                    Log_Verify.ErrorFR("File: " + FoundFile.Name + " Error: " + Error.ToString());
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        foreach (DirectoryInfo FoundDirectory in InstallationDirectory.EnumerateDirectories())
                        {
                            if (!ForceStopScan)
                            {
                                if (ModNetHandler.IsSymbolic(FoundDirectory.FullName))
                                {
                                    if (Directory.Exists(FoundDirectory.FullName))
                                    {
                                        try
                                        {
                                            Directory.Delete(FoundDirectory.FullName, true);
                                            Log_Verify.Deleted("Folder: " + FoundDirectory.Name);
                                        }
                                        catch (Exception Error)
                                        {
                                            DeletionError++;
                                            Log_Verify.Error("Folder: " + FoundDirectory.Name + " Error: " + Error.Message);
                                            Log_Verify.ErrorIC("Folder: " + FoundDirectory.Name + " Error: " + Error.HResult);
                                            Log_Verify.ErrorFR("Folder: " + FoundDirectory.Name + " Error: " + Error.ToString());
                                        }
                                    }
                                    else if (File.Exists(FoundDirectory.FullName))
                                    {
                                        try
                                        {
                                            File.Delete(FoundDirectory.FullName);
                                            Log_Verify.Deleted("File: " + FoundDirectory.Name);
                                        }
                                        catch (Exception Error)
                                        {
                                            DeletionError++;
                                            Log_Verify.Error("File: " + FoundDirectory.Name + " Error: " + Error.Message);
                                            Log_Verify.ErrorIC("File: " + FoundDirectory.Name + " Error: " + Error.HResult);
                                            Log_Verify.ErrorFR("File: " + FoundDirectory.Name + " Error: " + Error.ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        foreach (DirectoryInfo FoundDirectory in FoldersWeFound.EnumerateDirectories())
                        {
                            if (!ForceStopScan)
                            {
                                if (ModNetHandler.IsSymbolic(FoundDirectory.FullName))
                                {
                                    if (Directory.Exists(FoundDirectory.FullName))
                                    {
                                        try
                                        {
                                            Directory.Delete(FoundDirectory.FullName, true);
                                            Log_Verify.Deleted("Folder: " + FoundDirectory.Name);
                                        }
                                        catch (Exception Error)
                                        {
                                            DeletionError++;
                                            Log_Verify.Error("Folder: " + FoundDirectory.Name + " Error: " + Error.Message);
                                            Log_Verify.ErrorIC("Folder: " + FoundDirectory.Name + " Error: " + Error.HResult);
                                            Log_Verify.ErrorFR("Folder: " + FoundDirectory.Name + " Error: " + Error.ToString());
                                        }
                                    }
                                    else if (File.Exists(FoundDirectory.FullName))
                                    {
                                        try
                                        {
                                            File.Delete(FoundDirectory.FullName);
                                            Log_Verify.Deleted("File: " + FoundDirectory.Name);
                                        }
                                        catch (Exception Error)
                                        {
                                            DeletionError++;
                                            Log_Verify.Error("File: " + FoundDirectory.Name + " Error: " + Error.Message);
                                            Log_Verify.ErrorIC("File: " + FoundDirectory.Name + " Error: " + Error.HResult);
                                            Log_Verify.ErrorFR("File: " + FoundDirectory.Name + " Error: " + Error.ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (Directory.Exists(Path.Combine(Save_Settings.Live_Data.Game_Path, "scripts")) && !ForceStopScan)
                {
                    DirectoryInfo ScriptsFolder = new DirectoryInfo(Path.Combine(Save_Settings.Live_Data.Game_Path, "scripts"));

                    if (ScriptsFolder.EnumerateFiles().Count() > 1)
                    {
                        if (MessageBox.Show("Verify Hash has found files in the Scripts folder.\n" +
                            "If you have installed custom Scripts or have not installed any Scripts" +
                            "\n\nClick Yes, to Allow Deletion of Files" +
                            "\nClick No, to Skip Deletion of Files", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            foreach (FileInfo FoundFile in ScriptsFolder.EnumerateFiles())
                            {
                                if (FoundFile.Name != "LangPicker.ini")
                                {
                                    try
                                    {
                                        File.Delete(FoundFile.FullName);
                                        Log_Verify.Deleted("File: " + FoundFile.Name);
                                    }
                                    catch (Exception Error)
                                    {
                                        DeletionError++;
                                        Log_Verify.Error("File: " + FoundFile.Name + " Error: " + Error.Message);
                                        Log_Verify.ErrorIC("File: " + FoundFile.Name + " Error: " + Error.HResult);
                                        Log_Verify.ErrorFR("File: " + FoundFile.Name + " Error: " + Error.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("VERIFY HASH", string.Empty, Error, string.Empty, true);
            }

            if (DeletionError != 0)
            {
                Log.Info("VERIFY HASH: Completed check for '.orig' Files and Symbolic Folders, BUT Encounterd a File or Folder Deletion Error. " +
                "Check Verify.log for More Details");

                if (MessageBox.Show("Verify Hash has encountered File or Folder Deletion Errors.\n" +
                "Would you like to Open Verify.Log and Stop the Scanner?", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (File.Exists(Log_Location.LogVerify))
                    {
                        Process.Start(Log_Location.LogCurrentFolder);
                        Process.Start(Log_Location.LogVerify);
                    }

                    StopScanner_Click(null, null);
                }
            }
            else
            {
                Log.Info("VERIFY HASH: Completed check for '.orig' Files and Symbolic Folders");
            }

            if (!ForceStopScan)
            {
                try
                {
                    FunctionStatus.IsVerifyHashDisabled = true;

                    String[] getFilesToCheck = { };

                    if (File.Exists("checksums.dat") && EnableInsiderDeveloper.Allowed())
                    {
                        /* Read Local checksums.dat */
                        getFilesToCheck = File.ReadAllLines("checksums.dat");
                    }
                    else
                    {
                        /* Fetch and Read Remote checksums.dat */
                        ScanProgressText.SafeInvokeAction(() => ScanProgressText.Text = "Downloading Checksums File");

                        Uri URLCall = new Uri(FinalCDNURL + "/unpacked/checksums.dat");
                        ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                    Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
                        var Client = new WebClient
                        {
                            Encoding = Encoding.UTF8,
                            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                        };
                        if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                        { 
                            Client = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) }; 
                        }
                        else
                        {
                            Client.Headers.Add("user-agent", "SBRW Launcher " +
                            Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        }

                        bool ErrorFree = true;

                        try
                        {
                            getFilesToCheck = Client.DownloadString(URLCall).Split('\n');
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("VERIFY HASH CHECKSUMS", "Downloading of the Checksums File has Encountered an Error", Error, "Error", false);
                            ErrorFree = false;
                        }
                        finally
                        {
                            Client?.Dispose();
                        }

                        if (ErrorFree)
                        {
                            File.WriteAllLines("checksums.dat", getFilesToCheck);
                        }
                        else
                        {
                            StopScanner_Click(null, null);
                        }
                    }

                    ScannedHashes = new string[getFilesToCheck.Length][];
                    for (var i = 0; i < getFilesToCheck.Length; i++)
                    {
                        if (!ForceStopScan)
                        {
                            ScannedHashes[i] = getFilesToCheck[i].Split(' ');
                        }
                        else
                        {
                            break;
                        }
                    }
                    FilesToScan = ScannedHashes.Length;
                    TotalFilesScanned = 0;

                    /* START Show Warning Text */
                    VerifyHashText.SafeInvokeAction(() =>
                    {
                        VerifyHashText.ForeColor = Color_Winform.Warning_Text_Fore_Color;
                        VerifyHashText.Text = "Warning:\n Stopping the Scan before it is complete\nWill result in needing to start over!";
                    });
                    /* END Show Warning Text */

                    foreach (string[] file in ScannedHashes)
                    {
                        if (!ForceStopScan)
                        {
                            string FileHash = file[0].Trim();
                            string FileName = file[1].Trim();
                            string RealPathToFile = Path.Combine(Save_Settings.Live_Data.Game_Path + FileName);

                            if (!File.Exists(RealPathToFile))
                            {
                                InvalidFileList.Add(FileName);
                                Log_Verify.Missing("File: " + FileName);
                            }
                            else
                            {
                                if (FileHash != Hashes.Hash_SHA(RealPathToFile).Trim())
                                {
                                    InvalidFileList.Add(FileName);
                                    Log_Verify.Invalid("File: " + FileName);
                                }
                                else
                                {
                                    Log_Verify.Valid("File: " + FileName);
                                }
                            }
                            TotalFilesScanned++;
                            ScanProgressText.SafeInvokeAction(() => ScanProgressText.Text = "Scanning Files: " + (TotalFilesScanned * 100 / getFilesToCheck.Length) + "%");
                            ScanProgressBar.SafeInvokeAction(() => ScanProgressBar.Value = TotalFilesScanned * 100 / getFilesToCheck.Length);
                        }
                        else
                        {
                            break;
                        }
                    }

                    Log.Info("VERIFY HASH: Scan Completed");
                    if (!InvalidFileList.Any() || ForceStopScan)
                    {
                        StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
                        StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);
                        ScanProgressText.SafeInvokeAction(() => ScanProgressText.Text = ForceStopScan ? "User Stopped Scan." : "Scan Complete. No Files Missing or Invalid!");

                        /* Hide the DownloadProgressBar as un-needed */
                        DownloadProgressBar.SafeInvokeAction(() => DownloadProgressBar.Visible = false);
                        DownloadProgressText.SafeInvokeAction(() => DownloadProgressText.Visible = false);
                        /* Update the player messaging that we're done */
                        VerifyHashText.SafeInvokeAction(() =>
                        {
                            if (!ForceStopScan) 
                            { 
                                VerifyHashText.ForeColor = Color_Winform.Success_Text_Fore_Color;
                            }
                            VerifyHashText.Text = ForceStopScan ? "Verify Hash Scan Process has been Terminated" : "Excellent News! There are ZERO\nmissing or invalid Gamefiles!";
                        });

                        Integrity();
                        GameScanner(false);
                    }
                    else
                    {
                        ScanProgressText.SafeInvokeAction(() => ScanProgressText.Text = "Found Invalid or Missing Files");

                        File.WriteAllLines("invalidfiles.dat", InvalidFileList);
                        Log.Info("VERIFY HASH: Found Invalid or Missing Files and will Start File Downloader");
                        CorruptedFilesFound();
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("VERIFY HASH", string.Empty, Error, string.Empty, true);
                }
            }
        }

        private void Integrity()
        {
            Presence_Launcher.Status(27);
            Save_Settings.Live_Data.Game_Integrity = "Good";
            Save_Settings.Save();
            /* @DavidCarbon OR @Zacam 
            * Ini File Save Error Happens Above
            */
            if (Screen_Main.Screen_Instance != null)
            {
                if (Screen_Main.Screen_Instance.Button_Settings.InvokeRequired)
                {
                    Screen_Main.Screen_Instance.Button_Settings.SafeInvokeAction(() => Screen_Main.Screen_Instance.Button_Settings.BackgroundImage = Image_Icon.Gear);
                }
                else
                {
                    Screen_Main.Screen_Instance.Button_Settings.BackgroundImage = Image_Icon.Gear;
                }
            }
        }

        private void CorruptedFilesFound()
        {
            Presence_Launcher.Status(26);
            /* START Show Redownloader Progress*/
            StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
            StopScanner.SafeInvokeAction(() => StopScanner.Visible = true);

            VerifyHashText.SafeInvokeAction(() => VerifyHashText.Text = "Currently (re)downloading files\nThis part may take awhile\ndepending on your connection.");

            if (File.Exists("invalidfiles.dat") && File.ReadAllLines("invalidfiles.dat") != null)
            {
                DownloadProgressText.SafeInvokeAction(() => DownloadProgressText.Text = "\nPreparing to Download Files");

                string[] files = File.ReadAllLines("invalidfiles.dat");

                foreach (string text in files)
                {
                    if (!ForceStopScan)
                    {
                        try
                        {
                            while (StillDownloading) { }

                            if (!ForceStopScan)
                            {
                                CurrentCount = files.Count();

                                string text2 = Save_Settings.Live_Data.Game_Path + text;
                                string address = FinalCDNURL + "/unpacked" + text.Replace("\\", "/");
                                if (File.Exists(text2))
                                {
                                    try
                                    {
                                        Log_Verify.Deleted("File: " + text2);
                                        File.Delete(text2);
                                    }
                                    catch (Exception Error)
                                    {
                                        Log_Verify.Error("File: " + text2 + " Error: " + Error.Message);
                                        Log_Verify.ErrorIC("File: " + text2 + " Error: " + Error.HResult);
                                        Log_Verify.ErrorFR("File: " + text2 + " Error: " + Error.ToString());
                                    }
                                }

                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(text2))
                                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                                        if (!new FileInfo(text2).Directory.Exists)
                                        {
                                            new FileInfo(text2).Directory.Create();
                                        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                                    }
                                }
                                catch (Exception Error) { LogToFileAddons.OpenLog("VERIFY HASH File Info", string.Empty, Error, string.Empty, true); }

                                Uri URLCall = new Uri(address);
                                int Timeout = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;

                                if (address.Contains("copspeechdat"))
                                {
                                    Timeout = (int)TimeSpan.FromMinutes(30).TotalMilliseconds;
                                }
                                else if (address.Contains("nfs09mx.mus"))
                                {
                                    Timeout = (int)TimeSpan.FromMinutes(15).TotalMilliseconds;
                                }

                                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = Timeout;

                                var Client = new WebClient() 
                                { 
                                    Encoding = Encoding.UTF8,
                                    CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                                };
                                if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                                { 
                                    Client = new WebClientWithTimeout()
                                    {
                                        Encoding = Encoding.UTF8,
                                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                                    }; 
                                }
                                else
                                {
                                    Client.Headers.Add("user-agent", "SBRW Launcher " +
                                    Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                }

                                Client.DownloadProgressChanged += (Systems, RecevingData) =>
                                {
                                    if (RecevingData.TotalBytesToReceive >= 1 && !ForceStopScan)
                                    {
                                        DownloadProgressText.SafeInvokeAction(() =>
                                        DownloadProgressText.Text = "Downloading File [ " + RedownloadedCount + " / " +
                                        CurrentCount + " ]:\n" + CurrentDownloadingFile + "\n" + Time_Conversion.FormatFileSize(RecevingData.BytesReceived) +
                                        " of " + Time_Conversion.FormatFileSize(RecevingData.TotalBytesToReceive));
                                    }
                                    else if (ForceStopScan)
                                    {
                                        Client.CancelAsync();
                                    }
                                };
                                Client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);

                                try
                                {
                                    Client.DownloadFileAsync(URLCall, text2);
                                    CurrentDownloadingFile = text;
                                    StillDownloading = true;
                                }
                                catch (Exception Error)
                                {
                                    if (!ForceStopScan) { RedownloadErrorCount++; }
                                    LogToFileAddons.OpenLog("VERIFY HASH", string.Empty, Error, string.Empty, true);
                                }
                                finally
                                {
                                    Client?.Dispose();
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception Error)
                        {
                            if (!ForceStopScan) { RedownloadErrorCount++; }
                            LogToFileAddons.OpenLog("VERIFY HASH", string.Empty, Error, string.Empty, true);
                        }
                        finally
                        {
                            if (IsVerifyHashOpen)
                            {
                                Application.DoEvents();
                                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                                GC.Collect(); 
                                #endif
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            StillDownloading = false;

            if (e.Error != null && IsVerifyHashOpen && !ForceStopScan)
            {
                RedownloadErrorCount++;
                Log_Verify.Downloaded("File: " + CurrentDownloadingFile);
                Presence_Launcher.Status(26, RedownloadedCount + RedownloadErrorCount + " out of " + CurrentCount);

                DownloadProgressText.SafeInvokeAction(() =>
                DownloadProgressText.Text = "Failed To Download File [ " + 
                RedownloadedCount + RedownloadErrorCount + " / " + CurrentCount + " ]:" + "\n" + CurrentDownloadingFile);
                
                DownloadProgressBar.SafeInvokeAction(() => DownloadProgressBar.Value = RedownloadedCount + RedownloadErrorCount * 100 / CurrentCount);

                Log_Verify.Error("Download for [" + CurrentDownloadingFile + "] - " +
                (e.Error != null ? (string.IsNullOrWhiteSpace(e.Error.Message) ? e.Error.ToString() : e.Error.Message) : "No Exception Error Provided"));

                if (RedownloadedCount + RedownloadErrorCount == CurrentCount)
                {
                    StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
                    StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);

                    DownloadProgressText.SafeInvokeAction(() =>
                         DownloadProgressText.Text = "\n" + RedownloadedCount + " Invalid/Missing File(s) were Redownloaded");

                    VerifyHashText.SafeInvokeAction(() =>
                    {
                        VerifyHashText.ForeColor = Color_Winform.Warning_Text_Fore_Color;
                        VerifyHashText.Text = RedownloadErrorCount + " Files Failed to Download. Check Log for Details";
                    }, this);

                    DownloadErrorEncountered = true;
                    GameScanner(false);
                }
            }
            else if (IsVerifyHashOpen && !ForceStopScan)
            {
                RedownloadedCount++;

                Presence_Launcher.Status(26, RedownloadedCount + " out of " + CurrentCount);
                Log_Verify.Downloaded("File: " + CurrentDownloadingFile);

                DownloadProgressText.SafeInvokeAction(() =>
                DownloadProgressText.Text = "Downloaded File [ " + RedownloadedCount + " / " + CurrentCount + " ]:\n" + CurrentDownloadingFile);
                DownloadProgressBar.SafeInvokeAction(() => DownloadProgressBar.Value = RedownloadedCount * 100 / CurrentCount);

                if (RedownloadedCount == CurrentCount)
                {
                    Integrity();
                    Log.Info("VERIFY HASH: Re-downloaded Count: " + RedownloadedCount + " Current File Count: " + CurrentCount);
                    DownloadProgressText.SafeInvokeAction(() => DownloadProgressText.Text = "\n" + RedownloadedCount + " Invalid/Missing File(s) were downloaded");

                    VerifyHashText.SafeInvokeAction(() =>
                    {
                        VerifyHashText.ForeColor = Color_Winform.Warning_Text_Fore_Color;
                        VerifyHashText.Text = "Yay! Scanning and Downloading\n is now completed on Gamefiles";
                    }, this);

                    StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
                    StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);

                    GameScanner(false);
                }
                else if (RedownloadedCount + RedownloadErrorCount == CurrentCount)
                {
                    DownloadProgressText.SafeInvokeAction(() => DownloadProgressText.Text = "\n" + RedownloadedCount + " Invalid/Missing File(s) were downloaded");

                    VerifyHashText.SafeInvokeAction(() =>
                    {
                        VerifyHashText.ForeColor = Color_Winform.Warning_Text_Fore_Color;
                        VerifyHashText.Text = RedownloadErrorCount + " Files Failed to Download. Check Log for Details";
                    }, this);

                    StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
                    StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);

                    DownloadErrorEncountered = true;
                    GameScanner(false);
                }
            }
            else if (IsVerifyHashOpen && ForceStopScan)
            {
                Log.Info("VERIFY HASH: Download Process has Stopped");
                Presence_Launcher.Status(26, RedownloadedCount + " out of " + CurrentCount);

                DownloadProgressText.SafeInvokeAction(() =>
                DownloadProgressText.Text = "Download Stopped on File [ " + 
                RedownloadedCount + " / " + CurrentCount + " ]:" + "\n" + CurrentDownloadingFile);

                DownloadProgressBar.SafeInvokeAction(() => DownloadProgressBar.Value = RedownloadedCount * 100 / CurrentCount);

                Log_Verify.Error("Download for [" + CurrentDownloadingFile + "] -  has been Cancelled");

                StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
                StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);
                VerifyHashText.SafeInvokeAction(() =>
                {
                    VerifyHashText.Text = "Verify Hash Download Process has been Terminated";
                }, this);
            }
            else if (ForceStopScan)
            {
                Log.Info("VERIFY HASH: Download Process has Stopped");
                Log_Verify.Error("Download for [" + CurrentDownloadingFile + "] -  has been Cancelled");
            }
        }

        private void StartScanner_Click(object sender, EventArgs e)
        {
            StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
            StopScanner.SafeInvokeAction(() => StopScanner.Visible = true);

            GameScanner(true);
        }

        private void StopScanner_Click(object? sender, EventArgs? e)
        {
            StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
            StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);

            GameScanner(false);
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/
#if !(RELEASE_UNIX || DEBUG_UNIX)
            float MainFontSize = 9f * 96f / CreateGraphics().DpiY;
#else
            float MainFontSize = 9f;
#endif
            Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);

            VerifyHashWelcome.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            ScanProgressText.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            DownloadProgressText.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            StartScanner.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            StopScanner.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            VerifyHashText.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            VersionLabel.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors              /
            /********************************/

            ForeColor = Color_Winform.Text_Fore_Color;
            BackColor = Color_Winform.BG_Fore_Color;

            DownloadProgressText.ForeColor = Color_Winform.Text_Fore_Color;
            ScanProgressText.ForeColor = Color_Winform.Text_Fore_Color;

            VerifyHashWelcome.ForeColor = Color_Winform.Secondary_Text_Fore_Color;
            VerifyHashText.ForeColor = Color_Winform.Success_Text_Fore_Color;

            VersionLabel.ForeColor = Color_Winform.Text_Fore_Color;

            StartScanner.ForeColor = Color_Winform.Success_Text_Fore_Color;
            StartScanner.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            StartScanner.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            StartScanner.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            StopScanner.ForeColor = Color_Winform.Warning_Text_Fore_Color;
            StopScanner.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            StopScanner.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            StopScanner.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            /********************************/
            /* Events Handlers               /
            /********************************/

            StartScanner.Click += new EventHandler(StartScanner_Click);
            StopScanner.Click += new EventHandler(StopScanner_Click);

            /********************************/
            /* Hardcoded Text [Linux Fix]    /
            /********************************/

            VerifyHashWelcome.Text = "Welcome!\n\nThe scanning process is pretty quick,\nbut may still take a while." +
                "\nDepending on your connection,\nre-downloading will take the longest\nPlease allow it to complete fully!";
            ScanProgressText.Text = "Scanning Progress:";
            DownloadProgressText.Text = "Download Progress:";
            VerifyHashText.Text = "Please select \"Start Scan\" \nTo begin Validating Gamefiles";

            Shown += (x, y) =>
            {
                Application.OpenForms[this.Name].Activate();
                this.BringToFront();
            };
        }
    }
}
