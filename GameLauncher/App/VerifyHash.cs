using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.Hash;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.LauncherCore.RPC;

namespace GameLauncher.App
{
    public partial class VerifyHash : Form
    {
        private readonly RichPresence _presence = new RichPresence();

        //VerifyHash
        string[][] scannedHashes;
        public int filesToScan;
        public int badFiles;
        public int totalFilesScanned;
        public int currentCount;
        public int redownloadedCount;
        public List<string> InvalidFileList = new List<string>();
        public List<string> ValidFileList = new List<string>();
        public string FinalCDNURL;
        public bool isScanning = false;

        public VerifyHash()
        {
            InitializeComponent();

            this.Closing += (x, y) =>
            {
                if (isScanning)
                {
                    if (MessageBox.Show("Do you really want to exit the VerifyHash process?", "VerifyHash", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        y.Cancel = true;
                    }
                    else
                    {
                        GameScanner(false);
                    }
                }
            };

            SetVisuals();
        }

        private void VerifyHash_Load(object sender, EventArgs e)
        {
            VersionLabel.Text = "Version: v" + Application.ProductVersion;
            Log.Core("VerifyHash Opened");

            if (FunctionStatus.IsVerifyHashDisabled == false)
            {
                /* Clean up previous logs and start logging */
                string[] filestocheck = new string[] { "checksums.dat", "validfiles.dat", "invalidfiles.dat", "Verify.log" };
                foreach (String file in filestocheck)
                {
                    if (File.Exists(file)) File.Delete(file);
                }
                LogVerify.StartVerifyLogging();

                LogVerify.Info("VERIFYHASH: Checking Characters in URL");
                string SavedCDN = FileSettingsSave.CDN;
                char[] charsToTrim = { '/' };
                FinalCDNURL = SavedCDN.TrimEnd(charsToTrim);
                LogVerify.Info("VERIFYHASH: Trimed end of URL -> " + FinalCDNURL);
            }
            else
            {
                StartScanner.Enabled = false;
            }
        }

        public void GameScanner(bool startScan)
        {
            Thread StartScan;
            StartScan = new Thread(new ThreadStart(StartGameScanner))
            {
                Name = "FileScanner"
            };

            if (startScan == true)
            {
                FunctionStatus.IsVerifyHashDisabled = true;
                StartScan.Start();
                Log.Info("VERIFY HASH: Started Scanner");
                isScanning = true;
            }
            else if (startScan == false)
            {
                StartScan.Abort();
                Log.Info("VERIFY HASH: Stopped Scanner");
            }
        }

        private void StartGameScanner()
        {
            DiscordLauncherPresense.Status("Verify", null);

            Log.Info("VERIFY HASH: Checking and Deleting '.orig' Files");

            DirectoryInfo InstallationDirectory = new DirectoryInfo(FileSettingsSave.GameInstallation);

            foreach (var foundFolders in InstallationDirectory.GetDirectories())
            {
                foreach (var file in InstallationDirectory.EnumerateFiles("*.orig"))
                {
                    LogVerify.Deleted("File: " + file);
                    file.Delete();
                }

                foreach (var file in foundFolders.EnumerateFiles("*.orig"))
                {
                    LogVerify.Deleted("File: " + file);
                    file.Delete();
                }
            }
            Log.Info("VERIFY HASH: Completed check for '.orig' Files");

            try
            {
                String[] getFilesToCheck;

                if (File.Exists("checksums.dat"))
                {
                    /* Read Local checksums.dat */
                    getFilesToCheck = File.ReadAllLines("checksums.dat");
                }
                else
                {
                    /* Fetch and Read Remote checksums.dat */
                    ScanProgressText.Text = "Downloading Checksums File";
                    getFilesToCheck = new WebClient().DownloadString(FinalCDNURL + "/unpacked/checksums.dat").Split('\n');
                    File.WriteAllLines("checksums.dat", getFilesToCheck);
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
                        if (FileHash != SHA.HashFile(RealPathToFile).Trim())
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

                if (InvalidFileList.Any() != true)
                {
                    GameScanner(false);
                    StartScanner.Visible = false;
                    StopScanner.Visible = false;
                    ScanProgressText.Text = "Scan Complete. No Files Missing or Invalid!";
                    /* Hide the DownloadProgressBar as un-needed */
                    DownloadProgressBar.Visible = false;
                    DownloadProgressText.Visible = false;
                    /* Update the player messaging that we're done */
                    VerifyHashText.ForeColor = Theming.WinFormSuccessTextForeColor;
                    VerifyHashText.Text = "Excellent News! There are ZERO\nmissing or invalid Gamefiles!";
                }
                else
                {
                    ScanProgressText.Text = "Found Invalid or Missing Files";
                    File.WriteAllLines("invalidfiles.dat", InvalidFileList);
                    Log.Info("Found Invalid or Missing Files and will Start File Downloader");
                    CorruptedFilesFound();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        private void CorruptedFilesFound()
        {
            /* START Show Redownloader Progress*/
            StartScanner.Visible = false;
            StopScanner.Visible = false;
            VerifyHashText.Text = "Currently (re)downloading files\nThis part may take awhile\ndepending on your connection.";
            redownloadedCount = 0;

            if (File.Exists("invalidfiles.dat") && File.ReadAllLines("invalidfiles.dat") != null)
            {
                DownloadProgressText.Text = "\nPreparing to Download Files";
                string[] files = File.ReadAllLines("invalidfiles.dat");

                foreach (string text in files)
                {
                    currentCount = files.Count();
                    try 
                    {
                        string text2 = FileSettingsSave.GameInstallation + text;
                        string address = FinalCDNURL + "/unpacked" + text.Replace("\\", "/");
                        if (File.Exists(text2))
                        {
                            LogVerify.Deleted("File: " + text2);
                            File.Delete(text2);
                        }
                        new WebClient().DownloadFile(address, text2);
                        LogVerify.Downloaded("File: " + text2);
                        redownloadedCount++;
                        Application.DoEvents();
                    }
                    catch { }
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        DownloadProgressText.Text = "Downloaded File [ " + redownloadedCount + " / " + currentCount + " ]:\n" + text;
                        DownloadProgressBar.Value = redownloadedCount * 100 / files.Length;
                    });
                }
                DownloadProgressText.Text = "\n" + redownloadedCount + " Invalid/Missing File(s) were Redownloaded";
                VerifyHashText.ForeColor = Theming.WinFormWarningTextForeColor;
                VerifyHashText.Text = "Yay! Scanning and Downloading \nis now completed on Gamefiles";
                GameScanner(false);
                StartScanner.Visible = false;
                StopScanner.Visible = false;
            }
        }

        private void StartScanner_Click(object sender, EventArgs e)
        {
            GameScanner(true);
            StartScanner.Visible = false;
            StopScanner.Visible = true;
        }

        private void StopScanner_Click(object sender, EventArgs e)
        {
            GameScanner(false);
            StartScanner.Visible = true;
            StopScanner.Visible = false;
        }

        private void SetVisuals() 
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            var MainFontSize = 9f * 100f / CreateGraphics().DpiY;
            //var SecondaryFontSize = 8f * 100f / CreateGraphics().DpiY;
            //var ThirdFontSize = 10f * 100f / CreateGraphics().DpiY;
            //var FourthFontSize = 14f * 100f / CreateGraphics().DpiY;

            if (DetectLinux.LinuxDetected())
            {
                MainFontSize = 9f;
                //SecondaryFontSize = 8f;
                //ThirdFontSize = 10f;
                //FourthFontSize = 14f;
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
