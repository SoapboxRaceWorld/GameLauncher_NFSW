using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncher.HashPassword;
using System.ComponentModel;
using GameLauncher.App.Classes.Logger;
using System.Net;
using System.Diagnostics;

namespace GameLauncher.App
{
    public partial class VerifyHash : Form
    {
        private readonly IniFile _settingFile = new IniFile("Settings.ini");

        //VerifyHash
        string[][] scannedHashes;
        public int filesToScan;
        public int badFiles;
        public int totalFilesScanned;
        public int redownloadedCount;
        public List<string> invalidFileList = new List<string>();

        public VerifyHash()
        {
            InitializeComponent();
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
                //Log.Debug("Stopped Scanner");
            }
           
        }

        private void StartGameScanner()
        {
            String[] getFilesToCheck = null;
            try
            {
                //getFilesToCheck = new WebClient().DownloadString("http://localhost/checksums.dat").Split('\n');
                getFilesToCheck = File.ReadAllLines("checksums.dat");
                scannedHashes = new string[getFilesToCheck.Length][];
                for (var i = 0; i < getFilesToCheck.Length; i++)
                {
                    scannedHashes[i] = getFilesToCheck[i].Split(' ');
                }
                filesToScan = scannedHashes.Length;
                /*if (File.Exists("checksums.dat"))
                {
                    File.Delete("checksums.dat");
                }
                File.WriteAllLines("checksums.dat", getFilesToCheck);*/
                totalFilesScanned = 0;
                redownloadedCount = 0;

                foreach (string[] file in scannedHashes)
                {
                    String FileHash = file[0].Trim();
                    String FileName = file[1].Trim();
                    String RealPathToFile = _settingFile.Read("InstallationDirectory") + FileName;

                    if (!File.Exists(RealPathToFile))
                    {
                        invalidFileList.Add(FileName);
                        Log.Debug("File Missing: " + RealPathToFile);
                    }
                    else
                    {
                        if (FileHash != SHA.HashFile(RealPathToFile).Trim())
                        {
                            invalidFileList.Add(FileName);
                            Log.Debug("Invalid file found: " + RealPathToFile);
                        }
                        else
                        {
                            Log.Debug("Vaild file found: " + RealPathToFile);
                        }
                    }
                    totalFilesScanned++;
                }

                if (invalidFileList != null)
                {
                    File.WriteAllLines("invalidfiles.dat", invalidFileList);
                    CorruptedFilesFound();
                }
                else
                {
                    GameScanner(false);
                    StartScanner.Visible = true;
                    StopScanner.Visible = false;
                }
                //StatusText.Text = "Scan Complete.";
                //Log.Info("Scan Completed");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        private void CorruptedFilesFound()
        {
            if (File.Exists("invalidfiles.dat") && File.ReadAllLines("invalidfiles.dat") != null)
            {
                //StatusText.Text = "RE-DOWNLOADING INVALID FILES";
                string[] files = File.ReadAllLines("invalidfiles.dat");
                foreach (string text in files) {
                    try {
                        string text2 = _settingFile.Read("InstallationDirectory") + text;
                        string address = "http://mtntr.pl/unpacked" + text.Replace("\\", "/");
                        if (File.Exists(text2))
                        {
                            Log.Debug("Deleting " + text2);
                            File.Delete(text2);
                        }
                        new WebClient().DownloadFile(address, text2);
                        //Log.Debug("Downloaded " + text2);
                        Application.DoEvents();
                    }
                    catch { }
                }
                GameScanner(false);
                StartScanner.Visible = true;
                StopScanner.Visible = false;
            }
            else
            {
                //StatusText.Text = "All Files Validated";
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
    }
}
