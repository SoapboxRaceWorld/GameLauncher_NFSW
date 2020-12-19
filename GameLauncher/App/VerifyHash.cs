using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncher.HashPassword;

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
            if(startScan != false) {
                StatusText.Text = "Validating files on background.".ToUpper();
                    //Threaded CheckFiles
                    var thread = new Thread(() => {
                    String[] getFilesToCheck = null;
                    try { 
                        getFilesToCheck = new WebClientWithTimeout().DownloadString("http://cdn.mtntr.pl/gamefiles/checksums.dat").Split('\n');
                        scannedHashes = new string[getFilesToCheck.Length][];
                        for (var i = 0; i < getFilesToCheck.Length; i++) {
                            scannedHashes[i] = getFilesToCheck[i].Split(' ');
                        }
                        filesToScan = scannedHashes.Length;
                        totalFilesScanned = 0;
                        redownloadedCount = 0;
                        Directory.EnumerateFiles(_settingFile.Read("InstallationDirectory"), "*.*", SearchOption.AllDirectories).AsParallel().ForAll((file) => {
                            for (var i = 0; i < scannedHashes.Length; i++) {
                                if (scannedHashes[i][1].Trim() == file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim()) {
                                    if (scannedHashes[i][0].Trim() != SHA.HashFile(file).Trim()) {
                                        invalidFileList.Add(file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim());
                                        StatusText.Text = "Invalid file found: [GAMEDIR]" + file.Replace(_settingFile.Read("InstallationDirectory"), string.Empty).Trim();
                                    }
                                }
                            }
                            totalFilesScanned++;
                        });
                        StatusText.Text = "Download Completed.".ToUpper();
                    } catch (Exception) { }
                }){ IsBackground = true };
                thread.Start();
            } else {
                StatusText.Text = "Download Completed.".ToUpper();
            }
        }

        public void DownloadMissingFiles (bool downloadFiles)
        {
            if (downloadFiles != false)
            {
                if (File.Exists("invalidfiles.dat"))
                {
                    StatusText.Text = "RE-DOWNLOADING INVALID FILES".ToUpper();
                    string[] files = File.ReadAllLines("invalidfiles.dat");
                    foreach (string text in files)
                    {
                        try
                        {
                            string text2 = _settingFile.Read("InstallationDirectory") + text;
                            string address = "http://cdn.mtntr.pl/gamefiles/unpacked" + text.Replace("\\", "/");
                            if (File.Exists(text2 + ".vhbak"))
                            {
                                File.Delete(text2 + ".vhbak");
                            }
                            File.Move(text2, text2 + ".vhbak");
                            new WebClientWithTimeout().DownloadFile(address, text2);
                            Application.DoEvents();
                        }
                        catch { }
                    }
                }
            }
            else
            {
                StatusText.Text = "Download Completed".ToUpper();
            }
        }
    }
}
