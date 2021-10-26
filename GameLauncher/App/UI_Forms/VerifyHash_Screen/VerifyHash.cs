using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using SBRWCore.Classes.Extensions;
using SBRWCore.Classes.Launcher;
using SBRWCore.Classes.Required;
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

namespace GameLauncher.App.UI_Forms.VerifyHash_Screen
{
    public partial class VerifyHash : Form
    {
        private static bool IsVerifyHashOpen = false;
        /* VerifyHash */
        string[][] scannedHashes;
        public int filesToScan;
        public int badFiles;
        public int totalFilesScanned;
        public int currentCount;
        public int redownloadedCount;
        public int redownloadErrorCount;
        public List<string> InvalidFileList = new List<string>();
        public List<string> ValidFileList = new List<string>();
        public string FinalCDNURL;
        public static Thread StartScan;
        public bool isScanning = false;
        public static string CurrentDownloadingFile = String.Empty;
        public static int DeletionError = 0;
        public static bool DeletionErrorBypass = false;
        public static bool StillDownloading = false;
        public static bool DownloadErrorEncountered = false;

        public static void OpenScreen()
        {
            if (IsVerifyHashOpen || Application.OpenForms["VerifyHash"] != null)
            {
                if (!Application.OpenForms["VerifyHash"].IsDisposed) { Application.OpenForms["VerifyHash"].Activate(); }
            }
            else
            {
                try { new VerifyHash().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Verify Hash Screen Encountered an Error";
                    Classes.LauncherCore.Logger.LogToFileAddons.OpenLog("Verify Hash Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public VerifyHash()
        {
            IsVerifyHashOpen = true;
            DiscordLauncherPresence.Status("Verify", null);
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, CloseForm) =>
            {
                if (isScanning)
                {
                    if (MessageBox.Show("Do you really want to exit the VerifyHash process?", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        CloseForm.Cancel = true;
                    }
                    else
                    {
                        DiscordLauncherPresence.Status("Settings", null);
                        IsVerifyHashOpen = false;
                        GameScanner(false);
                    }
                }
                else
                {
                    IsVerifyHashOpen = false;
                    DiscordLauncherPresence.Status("Settings", null);
                }

                GC.Collect();
            };
        }

        private void VerifyHash_Load(object sender, EventArgs e)
        {
            VersionLabel.Text = "Version: v" + Application.ProductVersion;
            Log.Core("VERIFY HASH: Opened");

            if (!FunctionStatus.IsVerifyHashDisabled)
            {
                Log_Verify.StartVerifyLogging();

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
                if (FileSettingsSave.CDN.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    FinalCDNURL = FileSettingsSave.CDN.TrimEnd(charsToTrim);
                    Log_Verify.Info("VERIFYHASH: Trimed end of CDN URL -> " + FinalCDNURL);
                }
                else
                {
                    FinalCDNURL = FileSettingsSave.CDN;
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
                        Name = "FileScanner"
                    };

                    isScanning = true;
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
                isScanning = false;
                Log.Info("VERIFY HASH: Stopped Scanner");
                StartScan.Abort();

                if (DownloadErrorEncountered)
                {
                    DownloadErrorEncountered = false;

                    if (MessageBox.Show("Verify Hash has encountered Download Errors.\n" +
                        "Would you like to Open Verify.Log", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (File.Exists(LogToFile_Extensions.LogVerify))
                        {
                            Process.Start(LogToFile_Extensions.LogCurrentFolder);
                            Process.Start(LogToFile_Extensions.LogVerify);
                        }
                    }
                }
            }
        }

        private void StartGameScanner()
        {
            DiscordLauncherPresence.Status("Verify Scan", null);
            Log.Info("VERIFY HASH: Checking and Deleting '.orig' Files and Symbolic Folders");

            try
            {
                DirectoryInfo InstallationDirectory = new DirectoryInfo(FileSettingsSave.GameInstallation);

                foreach (DirectoryInfo FoldersWeFound in InstallationDirectory.GetDirectories())
                {
                    foreach (FileInfo FoundFile in InstallationDirectory.EnumerateFiles("*.orig", SearchOption.AllDirectories))
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

                    foreach (FileInfo FoundFile in FoldersWeFound.EnumerateFiles("*.orig", SearchOption.AllDirectories))
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

                    foreach (DirectoryInfo FoundDirectory in InstallationDirectory.EnumerateDirectories())
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

                    foreach (DirectoryInfo FoundDirectory in FoldersWeFound.EnumerateDirectories())
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
                }

                if (Directory.Exists(Path.Combine(FileSettingsSave.GameInstallation, "scripts")))
                {
                    DirectoryInfo ScriptsFolder = new DirectoryInfo(Path.Combine(FileSettingsSave.GameInstallation, "scripts"));

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
                LogToFileAddons.OpenLog("VERIFY HASH", null, Error, null, true);
            }

            if (DeletionError != 0)
            {
                Log.Info("VERIFY HASH: Completed check for '.orig' Files and Symbolic Folders, BUT Encounterd a File or Folder Deletion Error. " +
                "Check Verify.log for More Details");

                if (MessageBox.Show("Verify Hash has encountered File or Folder Deletion Errors.\n" +
                "Would you like to Open Verify.Log and Stop the Scanner?", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (File.Exists(LogToFile_Extensions.LogVerify))
                    {
                        Process.Start(LogToFile_Extensions.LogCurrentFolder);
                        Process.Start(LogToFile_Extensions.LogVerify);
                    }

                    StopScanner_Click(null, null);
                }
            }
            else
            {
                Log.Info("VERIFY HASH: Completed check for '.orig' Files and Symbolic Folders");
            }

            try
            {
                FunctionStatus.IsVerifyHashDisabled = true;

                String[] getFilesToCheck = { };

                if (File.Exists("checksums.dat"))
                {
                    /* Read Local checksums.dat */
                    getFilesToCheck = File.ReadAllLines("checksums.dat");
                }
                else
                {
                    /* Fetch and Read Remote checksums.dat */
                    ScanProgressText.SafeInvokeAction(() => ScanProgressText.Text = "Downloading Checksums File");

                    Uri URLCall = new Uri(FinalCDNURL + "/unpacked/checksums.dat");
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var Client = new WebClient
                    {
                        Encoding = Encoding.UTF8
                    };
                    if (!Live_Cache.Launcher_Alternative_Webcalls()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
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
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
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

                scannedHashes = new string[getFilesToCheck.Length][];
                for (var i = 0; i < getFilesToCheck.Length; i++)
                {
                    scannedHashes[i] = getFilesToCheck[i].Split(' ');
                }
                filesToScan = scannedHashes.Length;
                totalFilesScanned = 0;

                /* START Show Warning Text */
                VerifyHashText.SafeInvokeAction(() =>
                {
                    VerifyHashText.ForeColor = Theming.WinFormWarningTextForeColor;
                    VerifyHashText.Text = "Warning:\n Stopping the Scan before it is complete\nWill result in needing to start over!";
                });
                /* END Show Warning Text */

                foreach (string[] file in scannedHashes)
                {
                    String FileHash = file[0].Trim();
                    String FileName = file[1].Trim();
                    String RealPathToFile = FileSettingsSave.GameInstallation + FileName;

                    if (!File.Exists(RealPathToFile))
                    {
                        InvalidFileList.Add(FileName);
                        Log_Verify.Missing("File: " + FileName);
                    }
                    else
                    {
                        if (FileHash != Hash_Extention.HashesSHA(RealPathToFile).Trim())
                        {
                            InvalidFileList.Add(FileName);
                            Log_Verify.Invalid("File: " + FileName);
                        }
                        else
                        {
                            Log_Verify.Valid("File: " + FileName);
                        }
                    }
                    totalFilesScanned++;
                    ScanProgressText.SafeInvokeAction(() => ScanProgressText.Text = "Scanning Files: " + (totalFilesScanned * 100 / getFilesToCheck.Length) + "%");
                    ScanProgressBar.SafeInvokeAction(() => ScanProgressBar.Value = totalFilesScanned * 100 / getFilesToCheck.Length);
                }

                Log.Info("VERIFY HASH: Scan Completed");

                if (!InvalidFileList.Any())
                {
                    StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
                    StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);
                    StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
                    ScanProgressText.SafeInvokeAction(() => ScanProgressText.Text = "Scan Complete. No Files Missing or Invalid!");

                    /* Hide the DownloadProgressBar as un-needed */
                    DownloadProgressBar.SafeInvokeAction(() => DownloadProgressBar.Visible = false);
                    DownloadProgressText.SafeInvokeAction(() => DownloadProgressText.Visible = false);
                    /* Update the player messaging that we're done */
                    VerifyHashText.SafeInvokeAction(() =>
                    {
                        VerifyHashText.ForeColor = Theming.WinFormSuccessTextForeColor;
                        VerifyHashText.Text = "Excellent News! There are ZERO\nmissing or invalid Gamefiles!";
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
                LogToFileAddons.OpenLog("VERIFY HASH", null, Error, null, true);
            }
        }

        private void Integrity()
        {
            DiscordLauncherPresence.Status("Verify Good", null);
            FileSettingsSave.GameIntegrity = "Good";
            FileSettingsSave.SaveSettings();
        }

        private void CorruptedFilesFound()
        {
            DiscordLauncherPresence.Status("Verify Bad", null);
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
                    try
                    {
                        while (StillDownloading) { }

                        currentCount = files.Count();

                        string text2 = FileSettingsSave.GameInstallation + text;
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
                            if (!new FileInfo(text2).Directory.Exists)
                            {
                                new FileInfo(text2).Directory.Create();
                            }
                        }
                        catch (Exception Error) { LogToFileAddons.OpenLog("VERIFY HASH File Info", null, Error, null, true); }

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

                        var Client = new WebClient
                        {
                            Encoding = Encoding.UTF8
                        };
                        if (!Live_Cache.Launcher_Alternative_Webcalls()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                        else
                        {
                            Client.Headers.Add("user-agent", "SBRW Launcher " +
                            Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        }

                        Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
                        Client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);

                        try
                        {
                            Client.DownloadFileAsync(URLCall, text2);
                            CurrentDownloadingFile = text;
                            StillDownloading = true;
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("VERIFY HASH", null, Error, null, true);
                            redownloadErrorCount++;
                        }
                        finally
                        {
                            if (Client != null)
                            {
                                Client.Dispose();
                            }
                        }
                    }
                    catch (Exception Error)
                    {
                        redownloadErrorCount++;
                        LogToFileAddons.OpenLog("VERIFY HASH", null, Error, null, true);
                    }
                    finally
                    {
                        if (IsVerifyHashOpen)
                        {
                            Application.DoEvents();
                            GC.Collect();
                        }
                    }
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            StillDownloading = false;

            if ((e.Cancelled || e.Error != null) && IsVerifyHashOpen)
            {
                redownloadErrorCount++;
                DiscordLauncherPresence.Status("Verify Bad", redownloadedCount + redownloadErrorCount + " out of " + currentCount);
                Log_Verify.Downloaded("File: " + CurrentDownloadingFile);

                DownloadProgressText.SafeInvokeAction(() =>
                DownloadProgressText.Text = "Failed To Download File [ " + redownloadedCount + redownloadErrorCount + " / " + currentCount + " ]:" + "\n" + CurrentDownloadingFile);

                DownloadProgressBar.SafeInvokeAction(() => DownloadProgressBar.Value = redownloadedCount + redownloadErrorCount * 100 / currentCount);

                Log_Verify.Error("Download for [" + CurrentDownloadingFile + "] - " +
                (e.Cancelled ? "has been Cancelled" :
                (e.Error != null ? (string.IsNullOrWhiteSpace(e.Error.Message) ? e.Error.ToString() : e.Error.Message) : "No Exception Error Provided")));

                if (redownloadedCount + redownloadErrorCount == currentCount)
                {
                    DownloadProgressText.SafeInvokeAction(() =>
                    DownloadProgressText.Text = "\n" + redownloadedCount + " Invalid/Missing File(s) were Redownloaded");

                    VerifyHashText.SafeInvokeAction(() =>
                    {
                        VerifyHashText.ForeColor = Theming.WinFormWarningTextForeColor;
                        VerifyHashText.Text = redownloadErrorCount + " Files Failed to Download. Check Log for Details";
                    }, this);

                    StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);

                    StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);

                    DownloadErrorEncountered = true;

                    GameScanner(false);
                }
            }
            else if (IsVerifyHashOpen)
            {
                redownloadedCount++;

                DiscordLauncherPresence.Status("Verify Bad", redownloadedCount + " out of " + currentCount);
                Log_Verify.Downloaded("File: " + CurrentDownloadingFile);

                DownloadProgressText.SafeInvokeAction(() =>
                DownloadProgressText.Text = "Downloaded File [ " + redownloadedCount + " / " + currentCount + " ]:\n" + CurrentDownloadingFile);
                DownloadProgressBar.SafeInvokeAction(() => DownloadProgressBar.Value = redownloadedCount * 100 / currentCount);

                if (redownloadedCount == currentCount)
                {
                    Integrity();
                    Log.Info("VERIFY HASH: Re-downloaded Count: " + redownloadedCount + " Current File Count: " + currentCount);
                    DownloadProgressText.SafeInvokeAction(() => DownloadProgressText.Text = "\n" + redownloadedCount + " Invalid/Missing File(s) were downloaded");

                    VerifyHashText.SafeInvokeAction(() =>
                    {
                        VerifyHashText.ForeColor = Theming.WinFormWarningTextForeColor;
                        VerifyHashText.Text = "Yay! Scanning and Downloading\n is now completed on Gamefiles";
                    }, this);

                    StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
                    StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);

                    GameScanner(false);
                }
                else if (redownloadedCount + redownloadErrorCount == currentCount)
                {
                    DownloadProgressText.SafeInvokeAction(() => DownloadProgressText.Text = "\n" + redownloadedCount + " Invalid/Missing File(s) were downloaded");

                    VerifyHashText.SafeInvokeAction(() =>
                    {
                        VerifyHashText.ForeColor = Theming.WinFormWarningTextForeColor;
                        VerifyHashText.Text = redownloadErrorCount + " Files Failed to Download. Check Log for Details";
                    }, this);

                    StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
                    StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);

                    DownloadErrorEncountered = true;

                    GameScanner(false);
                }
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.TotalBytesToReceive >= 1)
            {
                DownloadProgressText.SafeInvokeAction(() =>
                DownloadProgressText.Text = "Downloading File [ " + redownloadedCount + " / " +
                currentCount + " ]:\n" + CurrentDownloadingFile + "\n" + TimeConversion.FormatFileSize(e.BytesReceived) +
                " of " + TimeConversion.FormatFileSize(e.TotalBytesToReceive));
            }
        }

        private void StartScanner_Click(object sender, EventArgs e)
        {
            StartScanner.SafeInvokeAction(() => StartScanner.Visible = false);
            StopScanner.SafeInvokeAction(() => StopScanner.Visible = true);

            GameScanner(true);
        }

        private void StopScanner_Click(object sender, EventArgs e)
        {
            StartScanner.SafeInvokeAction(() => StartScanner.Visible = true);
            StopScanner.SafeInvokeAction(() => StopScanner.Visible = false);

            GameScanner(false);
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            float MainFontSize = UnixOS.Detected() ? 9f : 9f * 96f / CreateGraphics().DpiY;
            Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);

            VerifyHashWelcome.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            ScanProgressText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            DownloadProgressText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            StartScanner.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            StopScanner.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            VerifyHashText.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            VersionLabel.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors              /
            /********************************/

            ForeColor = Theming.WinFormTextForeColor;
            BackColor = Theming.WinFormTBGForeColor;

            DownloadProgressText.ForeColor = Theming.WinFormTextForeColor;
            ScanProgressText.ForeColor = Theming.WinFormTextForeColor;

            VerifyHashWelcome.ForeColor = Theming.WinFormSecondaryTextForeColor;
            VerifyHashText.ForeColor = Theming.WinFormSuccessTextForeColor;

            VersionLabel.ForeColor = Theming.WinFormTextForeColor;

            StartScanner.ForeColor = Theming.WinFormSuccessTextForeColor;
            StartScanner.BackColor = Theming.BlueBackColorButton;
            StartScanner.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            StartScanner.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            StopScanner.ForeColor = Theming.WinFormWarningTextForeColor;
            StopScanner.BackColor = Theming.BlueBackColorButton;
            StopScanner.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            StopScanner.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

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
