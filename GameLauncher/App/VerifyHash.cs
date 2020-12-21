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

            if (File.Exists("invalidfiles.dat"))
            {
                File.Delete("invalidfiles.dat");
            }

            if (startScan == true) {
                //StatusText.Text = "Validating files on background.".ToUpper();
                //Threaded CheckFiles
                StartScan.Start();
                Log.Debug("Started Scanner");
            }
            else {
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
            String[] getFilesToCheck = null;
            try
            {
                getFilesToCheck = new WebClient().DownloadString("http://localhost/checksums.dat").Split('\n');
                scannedHashes = new string[getFilesToCheck.Length][];
                for (var i = 0; i < getFilesToCheck.Length; i++)
                {
                    scannedHashes[i] = getFilesToCheck[i].Split(' ');
                }
                filesToScan = scannedHashes.Length;
                if (File.Exists("checksums.dat"))
                {
                    File.Delete("checksums.dat");
                }
                File.WriteAllLines("checksums.dat", getFilesToCheck);
                totalFilesScanned = 0;
                redownloadedCount = 0;

                Directory.EnumerateFiles(_settingFile.Read("InstallationDirectory"), "*.*", SearchOption.AllDirectories).AsParallel().ForAll((file) => {
                    for (var i = 0; i < scannedHashes.Length; i++)
                    {
                        //if (scannedHashes[i][1].Trim() == file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim())
                        //{
                            //Log.Debug("Scanned File: [GAMEDIR] " + file);
                            if (File.Exists(_settingFile.Read("InstallationDirectory") + scannedHashes[i][1]) && scannedHashes[i][1].Trim() == SHA.HashFile(file.Trim()))
                            {
                                Log.Debug("1 - Vaild file found: [GAMEDIR] " + file);
                            }
                            else if (!File.Exists(_settingFile.Read("InstallationDirectory") + scannedHashes[i][1]) && scannedHashes[i][1].Trim() == SHA.HashFile(file.Trim()))
                            {
                                invalidFileList.Add(file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim());
                                Log.Debug("6 - Invalid file found: [GAMEDIR] " + file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty));
                            }
                            else if (!File.Exists(_settingFile.Read("InstallationDirectory") + scannedHashes[i][1]))
                            {
                                invalidFileList.Add(file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim());
                                Log.Debug("7 - Invalid file found: [GAMEDIR] " + file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty));
                            }
                            else if (scannedHashes[i][1].Trim() == SHA.HashFile(file.Trim()))
                            {
                                Log.Debug("2 - Vaild file found: [GAMEDIR] " + file);
                            }
                            /*
                            else
                            {
                                invalidFileList.Add(file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim());
                                Log.Debug("5 - Invalid file found: [GAMEDIR] " + file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty));
                            }
                            /*
                            if (scannedHashes[i][0].Trim() != SHA.HashFile(file).Trim())
                            {
                                invalidFileList.Add(file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim());
                                Log.Debug("1 - Invalid file found: [GAMEDIR] " + file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty));
                            }
                            else if (scannedHashes[i][0] != SHA.HashFile(file))
                            {
                                invalidFileList.Add(file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim());
                                Log.Debug("2 - Invalid file found: [GAMEDIR] " + file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty));
                            }
                            else if (scannedHashes[i][0].Trim() != file)
                            {
                                invalidFileList.Add(file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim());
                                Log.Debug("3 - Invalid file found: [GAMEDIR] " + file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty));
                            }
                            else if (scannedHashes[i][0].Trim() != file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty))
                            {
                                invalidFileList.Add(file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim());
                                Log.Debug("4 - Invalid file found: [GAMEDIR] " + file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty));
                            }
                            */
                        //}
                    }
                    totalFilesScanned++;
                });

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
            }
            catch (Exception ex)
            { Log.Error(ex.Message); }
        }

        private void CorruptedFilesFound()
        {
            if (File.Exists("invalidfiles.dat") && File.ReadAllLines("invalidfiles.dat") != null)
            {
                //StatusText.Text = "RE-DOWNLOADING INVALID FILES";
                string[] files = File.ReadAllLines("invalidfiles.dat");
                foreach (string text in files)
                {
                    try
                    {
                        string text2 = _settingFile.Read("InstallationDirectory") + text;
                        string address = "http://mtntr.pl/unpacked" + text.Replace("\\", "/");
                        if (File.Exists(text2))
                        {
                            Log.Debug("Deleting " + text2);
                            File.Delete(text2);
                        }
                        new WebClient().DownloadFile(address, text2);
                        Log.Debug("Downloaded " + text2);
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
