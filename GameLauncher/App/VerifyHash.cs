using GameLauncherReborn;
using DiscordRPC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncher.App.Classes.Logger;
using GameLauncher.HashPassword;
using GameLauncher.Resources;

namespace GameLauncher.App
{
    public partial class VerifyHash : Form
    {
        private readonly IniFile _settingFile = new IniFile("Settings.ini");
        private readonly RichPresence _presence = new RichPresence();

        //VerifyHash
        string[][] scannedHashes;
        public int filesToScan;
        public int badFiles;
        public int totalFilesScanned;
        public int redownloadedCount;
        public List<string> InvalidFileList = new List<string>();
        public List<string> ValidFileList = new List<string>();

        public VerifyHash()
        {
            InitializeComponent();
            ApplyEmbeddedFonts();
            VersionLabel.Text = "Version: v" + Application.ProductVersion;
            LogVerify.StartVerifyLogging();

            if (File.Exists("validfiles.dat"))
            {
                File.Delete("validfiles.dat");
                File.Delete("invalidfiles.dat");
                File.Delete("Verify.log");
                
            }
            
        }

        public void GameScanner(bool startScan)
        {
            Thread StartScan;
            StartScan = new Thread(new ThreadStart(StartGameScanner))
            {
                Name = "FileScanner"
            };
            /*
            if (File.Exists("invalidfiles.dat"))
            {
                File.Delete("invalidfiles.dat");
            }
            */
            if (startScan == true)
            {
                //StatusText.Text = "Validating files on background.".ToUpper();
                //Threaded CheckFiles
                StartScan.Start();
                Log.Debug("Started Scanner");
            }
            else if (startScan == false)
            {
                StartScan.Abort();
                //StatusText.Text = "Unkown Status.".ToUpper();
                 Process[] allOfThem = Process.GetProcessesByName("VerifyHash");
                foreach (var oneProcess in allOfThem)
                {
                    Process.GetProcessById(oneProcess.Id).Kill();
                }
                Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                
                Log.Debug("Stopped Scanner");
            }
           
        }

        private void StartGameScanner()
        {
            _presence.Details = "In-Launcher: " + Application.ProductVersion;
            _presence.State = "Validating Game Files!";
            _presence.Assets = new Assets
            {
                LargeImageText = "SBRW",
                LargeImageKey = "nfsw"
            };
            if (MainScreen.discordRpcClient != null) MainScreen.discordRpcClient.SetPresence(_presence);

            try
            {
                String[] getFilesToCheck = new WebClient().DownloadString("http://localhost/checksums.dat").Split('\n');
                //String[] getFilesToCheck = File.ReadAllLines("checksums.dat");
                scannedHashes = new string[getFilesToCheck.Length][];
                for (var i = 0; i < getFilesToCheck.Length; i++)
                {
                    scannedHashes[i] = getFilesToCheck[i].Split(' ');
                }
                filesToScan = scannedHashes.Length;

                totalFilesScanned = 0;


                foreach (string[] file in scannedHashes)
                {
                    String FileHash = file[0].Trim();
                    String FileName = file[1].Trim();
                    String RealPathToFile = _settingFile.Read("InstallationDirectory") + FileName;

                    if (!File.Exists(RealPathToFile))
                    {
                        InvalidFileList.Add(FileName);
                        LogVerify.Missing("File: " + RealPathToFile);
                    }
                    else
                    {
                        if (FileHash != SHA.HashFile(RealPathToFile).Trim())
                        {
                            InvalidFileList.Add(FileName);
                            LogVerify.Invalid("File: " + RealPathToFile);
                        }
                        else
                        {
                            ValidFileList.Add(RealPathToFile);
                            File.WriteAllLines("validfiles.dat", ValidFileList);
                            LogVerify.Valid("File: " + RealPathToFile);
                        }
                    }
                    ScanProgressText.Text = "Found Missing Files";
                    totalFilesScanned++;
                    ScanProgressBar.Value = (getFilesToCheck.Length / filesToScan) * 100;
                }

                if (InvalidFileList != null)
                {
                    ScanProgressText.Text = "Found Missing Files";
                    File.WriteAllLines("invalidfiles.dat", InvalidFileList);
                    Log.Info("Found Invalid Files and Will Start File Downloader");
                    CorruptedFilesFound();
                }
                else
                {
                    GameScanner(false);
                    StartScanner.Visible = true;
                    StopScanner.Visible = false;
                    ScanProgressText.Text = "Scan Complete. No Missing Files Where Found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            Log.Info("Scan Completed");
        }

        private void CorruptedFilesFound()
        {
            redownloadedCount = 0;

            if (File.Exists("invalidfiles.dat") && File.ReadAllLines("invalidfiles.dat") != null)
            {
                InvalidProgressText.Text = "RE-DOWNLOADING INVALID FILES";
                string[] files = File.ReadAllLines("invalidfiles.dat");

                foreach (string text in files) {
                    try {
                        string text2 = _settingFile.Read("InstallationDirectory") + text;
                        string address = "http://mtntr.pl/unpacked" + text.Replace("\\", "/");
                        if (File.Exists(text2))
                        {
                            LogVerify.Deleted("File: " + text2);
                            File.Delete(text2);
                        }
                        new WebClient().DownloadFile(address, text2);
                        LogVerify.Downloaded("File: " + text2);
                        Application.DoEvents();
                    }
                    catch { }
                    //InvalidProgressBar.Value = files.int() * 100;
                }
                GameScanner(false);
                StartScanner.Visible = true;
                StopScanner.Visible = false;
            }
            else
            {
                InvalidProgressText.Text = "All Files Validated";
                GameScanner(false);
                StartScanner.Visible = true;
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

        private void ApplyEmbeddedFonts() {
            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");
            ScanProgressText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            InvalidProgressText.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            StartScanner.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            StopScanner.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            VersionLabel.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
        }
    }
}
