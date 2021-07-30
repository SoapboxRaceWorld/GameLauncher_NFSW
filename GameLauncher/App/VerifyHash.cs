using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.Hash;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.LauncherCore.RPC;
using System.ComponentModel;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.LauncherCore.Support;
using System.Diagnostics;
using GameLauncher.App.Classes.LauncherCore.Logger;
using System.Text;
using GameLauncher.App.Classes.LauncherCore.Client.Web;

namespace GameLauncher.App
{
    public partial class VerifyHash : Form
    {
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

        public VerifyHash()
        {
            DiscordLauncherPresense.Status("Verify", null);
            InitializeComponent();

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
                        GameScanner(false);
                    }
                }

                DiscordLauncherPresense.Status("Settings", null);
            };

            SetVisuals();
        }

        private void VerifyHash_Load(object sender, EventArgs e)
        {
            VersionLabel.Text = "Version: v" + Application.ProductVersion;
            Log.Core("VERIFY HASH: Opened");

            if (!FunctionStatus.IsVerifyHashDisabled)
            {
                LogVerify.StartVerifyLogging();

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
                            LogVerify.Error("File: " + file + " Error: " + Error.Message);
                            LogVerify.ErrorIC("File: " + file + " Error: " + Error.HResult);
                            LogVerify.ErrorFR("File: " + file + " Error: " + Error.ToString());
                        }
                    }
                }

                LogVerify.Info("VERIFYHASH: Checking Characters in URL");
                if (FileSettingsSave.CDN.EndsWith("/"))
                {
                    char[] charsToTrim = { '/' };
                    FinalCDNURL = FileSettingsSave.CDN.TrimEnd(charsToTrim);
                    LogVerify.Info("VERIFYHASH: Trimed end of CDN URL -> " + FinalCDNURL);
                }
                else
                {
                    FinalCDNURL = FileSettingsSave.CDN;
                    LogVerify.Info("VERIFYHASH: Choosen CDN URL -> " + FinalCDNURL);
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
                    MessageBox.Show("Verify Hash had Started a Full Scan Before." +
                        "\nTo do another Scan, Please Restart Launcher to do a New Scan.", "VerifyHash", MessageBoxButtons.OK);
                }
            }
            else if (!startScan)
            {
                isScanning = false;
                Log.Info("VERIFY HASH: Stopped Scanner");
                StartScan.Abort();
            }
        }

        private void StartGameScanner()
        {
            DiscordLauncherPresense.Status("Verify Scan", null);

            Log.Info("VERIFY HASH: Checking and Deleting '.orig' Files and Symbolic Folders");

            DirectoryInfo InstallationDirectory = new DirectoryInfo(FileSettingsSave.GameInstallation);

            try
            {
                foreach (var foundFolders in InstallationDirectory.GetDirectories())
                {
                    foreach (var file in InstallationDirectory.EnumerateFiles("*.orig"))
                    {
                        try
                        {
                            LogVerify.Deleted("File: " + file);
                            file.Delete();
                        }
                        catch (Exception Error)
                        {
                            DeletionError++;
                            LogVerify.Error("File: " + file + " Error: " + Error.Message);
                            LogVerify.ErrorIC("File: " + file + " Error: " + Error.HResult);
                            LogVerify.ErrorFR("File: " + file + " Error: " + Error.ToString());
                        }
                    }

                    foreach (var file in foundFolders.EnumerateFiles("*.orig"))
                    {
                        try
                        {
                            LogVerify.Deleted("File: " + file);
                            file.Delete();
                        }
                        catch (Exception Error)
                        {
                            DeletionError++;
                            LogVerify.Error("File: " + file + " Error: " + Error.Message);
                            LogVerify.ErrorIC("File: " + file + " Error: " + Error.HResult);
                            LogVerify.ErrorFR("File: " + file + " Error: " + Error.ToString());
                        }
                    }

                    foreach (var file in InstallationDirectory.EnumerateDirectories())
                    {
                        if (ModNetHandler.IsSymbolic(file.FullName))
                        {
                            if (Directory.Exists(foundFolders.FullName))
                            {
                                try
                                {
                                    LogVerify.Deleted("Folder: " + file);
                                    Directory.Delete(file.FullName, true);
                                }
                                catch (Exception Error)
                                {
                                    DeletionError++;
                                    LogVerify.Error("Folder: " + file + " Error: " + Error.Message);
                                    LogVerify.ErrorIC("Folder: " + file + " Error: " + Error.HResult);
                                    LogVerify.ErrorFR("Folder: " + file + " Error: " + Error.ToString());
                                }
                            }
                            else if (File.Exists(foundFolders.FullName))
                            {
                                try
                                {
                                    LogVerify.Deleted("File: " + file);
                                    File.Delete(file.FullName);
                                }
                                catch (Exception Error)
                                {
                                    DeletionError++;
                                    LogVerify.Error("File: " + file + " Error: " + Error.Message);
                                    LogVerify.ErrorIC("File: " + file + " Error: " + Error.HResult);
                                    LogVerify.ErrorFR("File: " + file + " Error: " + Error.ToString());
                                }
                            }
                        }
                    }

                    foreach (var file in foundFolders.EnumerateDirectories())
                    {
                        if (ModNetHandler.IsSymbolic(file.FullName))
                        {
                            if (Directory.Exists(foundFolders.FullName))
                            {
                                try
                                {
                                    LogVerify.Deleted("Folder: " + file);
                                    Directory.Delete(file.FullName, true);
                                }
                                catch (Exception Error)
                                {
                                    DeletionError++;
                                    LogVerify.Error("Folder: " + file + " Error: " + Error.Message);
                                    LogVerify.ErrorIC("Folder: " + file + " Error: " + Error.HResult);
                                    LogVerify.ErrorFR("Folder: " + file + " Error: " + Error.ToString());
                                }
                            }
                            else if (File.Exists(foundFolders.FullName))
                            {
                                try
                                {
                                    LogVerify.Deleted("File: " + file);
                                    File.Delete(file.FullName);
                                }
                                catch (Exception Error)
                                {
                                    DeletionError++;
                                    LogVerify.Error("File: " + file + " Error: " + Error.Message);
                                    LogVerify.ErrorIC("File: " + file + " Error: " + Error.HResult);
                                    LogVerify.ErrorFR("File: " + file + " Error: " + Error.ToString());
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

                if (MessageBox.Show("Verify Hash had encountered File or Folder Deletion Errors." +
                "\nWould you like to Open Verify.Log and Stop the Scanner?", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string LogFile = Strings.Encode(Path.Combine(Locations.LogFolder, Locations.LogVerify));
                    if (File.Exists(LogFile))
                    {
                        Process.Start(LogFile);
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
                    ScanProgressText.Text = "Downloading Checksums File";
                    FunctionStatus.TLS();
                    Uri URLCall = new Uri(FinalCDNURL + "/unpacked/checksums.dat");
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var Client = new WebClient
                    {
                        Encoding = Encoding.UTF8
                    };
                    if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
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
                        LogToFileAddons.OpenLog("VERIFY HASH CHECKSums", "Downloading Checksums File had Encountered an Error", Error, "Error", false);
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
                VerifyHashText.ForeColor = Theming.WinFormWarningTextForeColor;
                VerifyHashText.Text = "Warning:\n Stopping the Scan before it is complete\nWill result in needing to start over!";
                /* END Show Warning Text */

                foreach (string[] file in scannedHashes)
                {
                    String FileHash = file[0].Trim();
                    String FileName = file[1].Trim();
                    String RealPathToFile = FileSettingsSave.GameInstallation + FileName;

                    if (!File.Exists(RealPathToFile))
                    {
                        InvalidFileList.Add(FileName);
                        LogVerify.Missing("File: " + FileName);
                    }
                    else
                    {
                        if (FileHash != SHA.Files(RealPathToFile).Trim())
                        {
                            InvalidFileList.Add(FileName);
                            LogVerify.Invalid("File: " + FileName);
                        }
                        else
                        {
                            LogVerify.Valid("File: " + FileName);
                        }
                    }
                    totalFilesScanned++;
                    ScanProgressText.Text = "Scanning Files: " + (totalFilesScanned * 100 / getFilesToCheck.Length) + "%";
                    ScanProgressBar.Value = totalFilesScanned * 100 / getFilesToCheck.Length;
                }

                Log.Info("VERIFY HASH: Scan Completed");

                if (!InvalidFileList.Any())
                {
                    StartScanner.Visible = false;
                    StopScanner.Visible = false;
                    ScanProgressText.Text = "Scan Complete. No Files Missing or Invalid!";
                    /* Hide the DownloadProgressBar as un-needed */
                    DownloadProgressBar.Visible = false;
                    DownloadProgressText.Visible = false;
                    /* Update the player messaging that we're done */
                    VerifyHashText.ForeColor = Theming.WinFormSuccessTextForeColor;
                    VerifyHashText.Text = "Excellent News! There are ZERO\nmissing or invalid Gamefiles!";
                    Integrity();
                    GameScanner(false);
                }
                else
                {
                    ScanProgressText.Text = "Found Invalid or Missing Files";
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
            DiscordLauncherPresense.Status("Verify Good", null);
            FileSettingsSave.GameIntegrity = "Good";
            FileSettingsSave.SaveSettings();
        }

        private void CorruptedFilesFound()
        {
            DiscordLauncherPresense.Status("Verify Bad", null);
            /* START Show Redownloader Progress*/
            StartScanner.Visible = false;
            StopScanner.Visible = false;
            VerifyHashText.Text = "Currently (re)downloading files\nThis part may take awhile\ndepending on your connection.";

            if (File.Exists("invalidfiles.dat") && File.ReadAllLines("invalidfiles.dat") != null)
            {
                DownloadProgressText.Text = "\nPreparing to Download Files";
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
                                LogVerify.Deleted("File: " + text2);
                                File.Delete(text2);
                            }
                            catch (Exception Error)
                            {
                                LogVerify.Error("File: " + text2 + " Error: " + Error.Message);
                                LogVerify.ErrorIC("File: " + text2 + " Error: " + Error.HResult);
                                LogVerify.ErrorFR("File: " + text2 + " Error: " + Error.ToString());
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

                        FunctionStatus.TLS();
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
                        if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
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
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (Application.OpenForms["VerifyHash"] != null)
            {
                if (!Application.OpenForms["VerifyHash"].Disposing)
                {
                    StillDownloading = false;

                    if (e.Cancelled || e.Error != null)
                    {
                        redownloadErrorCount++;

                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            DiscordLauncherPresense.Status("Verify Bad", redownloadedCount + redownloadErrorCount + " out of " + currentCount);
                            LogVerify.Downloaded("File: " + CurrentDownloadingFile);
                            DownloadProgressText.Text = "Failed To Download File [ " + redownloadedCount + redownloadErrorCount + " / " + currentCount + " ]:" +
                            "\n" + CurrentDownloadingFile;
                            DownloadProgressBar.Value = redownloadedCount + redownloadErrorCount * 100 / currentCount;
                        });

                        Application.DoEvents();

                        LogVerify.Error("Download for [" + CurrentDownloadingFile + "] - " +
                        (e.Cancelled ? "has been Cancelled" : (string.IsNullOrWhiteSpace(e.Error.Message) ? e.Error.ToString() : e.Error.Message)));

                        if (redownloadedCount + redownloadErrorCount == currentCount)
                        {
                            DownloadProgressText.Text = "\n" + redownloadedCount + " Invalid/Missing File(s) were Redownloaded";
                            VerifyHashText.ForeColor = Theming.WinFormWarningTextForeColor;
                            VerifyHashText.Text = redownloadErrorCount + " Files Failed to Download. Check Log for Details";
                            StartScanner.Visible = false;
                            StopScanner.Visible = false;
                            GameScanner(false);

                            if (MessageBox.Show("Verify Hash had encountered Redownload Errors." +
                            "\nWould you like to Open Verify.Log", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                string LogFile = Strings.Encode(Path.Combine(Locations.LogFolder, Locations.LogVerify));
                                if (File.Exists(LogFile))
                                {
                                    Process.Start(LogFile);
                                }
                            }
                        }
                    }
                    else
                    {
                        redownloadedCount++;

                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            DiscordLauncherPresense.Status("Verify Bad", redownloadedCount + " out of " + currentCount);
                            LogVerify.Downloaded("File: " + CurrentDownloadingFile);
                            DownloadProgressText.Text = "Downloaded File [ " + redownloadedCount + " / " + currentCount + " ]:\n" + CurrentDownloadingFile;
                            DownloadProgressBar.Value = redownloadedCount * 100 / currentCount;
                        });

                        Application.DoEvents();

                        if (redownloadedCount == currentCount)
                        {
                            Integrity();
                            Log.Info("VERIFY HASH: Re-downloaded Count: " + redownloadedCount + " Current File Count: " + currentCount);

                            DownloadProgressText.Text = "\n" + redownloadedCount + " Invalid/Missing File(s) were Redownloaded";
                            VerifyHashText.ForeColor = Theming.WinFormWarningTextForeColor;
                            VerifyHashText.Text = "Yay! Scanning and Downloading \nis now completed on Gamefiles";
                            StartScanner.Visible = false;
                            StopScanner.Visible = false;
                            GameScanner(false);
                        }
                        else if (redownloadedCount + redownloadErrorCount == currentCount)
                        {
                            DownloadProgressText.Text = "\n" + redownloadedCount + " Invalid/Missing File(s) were Redownloaded";
                            VerifyHashText.ForeColor = Theming.WinFormWarningTextForeColor;
                            VerifyHashText.Text = redownloadErrorCount + " Files Failed to Download. Check Log for Details";
                            StartScanner.Visible = false;
                            StopScanner.Visible = false;
                            GameScanner(false);

                            if (MessageBox.Show("Verify Hash had encountered Redownload Errors." +
                            "\nWould you like to Open Verify.Log", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                string LogFile = Strings.Encode(Path.Combine(Locations.LogFolder, Locations.LogVerify));
                                if (File.Exists(LogFile))
                                {
                                    Process.Start(LogFile);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (Application.OpenForms["VerifyHash"] != null)
            {
                if (!Application.OpenForms["VerifyHash"].Disposing)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        DownloadProgressText.Text = "Downloading File [ " + redownloadedCount + " / "
                        + currentCount + " ]:\n" + CurrentDownloadingFile + "\n" + TimeConversions.FormatFileSize(e.BytesReceived) + " of "
                        + TimeConversions.FormatFileSize(e.TotalBytesToReceive);
                    });

                    Application.DoEvents();
                }
            }
        }

        private void StartScanner_Click(object sender, EventArgs e)
        {
            StartScanner.Visible = false;
            StopScanner.Visible = true;
            GameScanner(true);
        }

        private void StopScanner_Click(object sender, EventArgs e)
        {
            StartScanner.Visible = true;
            StopScanner.Visible = false;
            GameScanner(false);
        }

        private void SetVisuals() 
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            var MainFontSize = 9f * 100f / CreateGraphics().DpiY;

            if (DetectLinux.LinuxDetected())
            {
                MainFontSize = 9f;
            }

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
        }
    }
}
