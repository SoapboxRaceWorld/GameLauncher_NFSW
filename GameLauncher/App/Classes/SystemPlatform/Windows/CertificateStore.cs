using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Validator.VerifyTrust;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class CertificateStore
    {
        /* Used to either Download Certifcate or Remove the file from last installation */
        private static bool IsROOTCAInstalled = false;
        /* Beginning of File Name (No Extention) */
        public static string RootCAFileName = "CC-CA";
        /* URL of File Name (Extention) */
        public static string RootCAFileURL = "http://crl.carboncrew.org/CC-CA.cer";
        /* Certificate Fail Safe Details */
        public static string RootCACommonName = "Carbon Crew CA";
        public static string RootCASubjectName = "CN=Carbon Crew CA, OU=Certificate Authority, O=Carbon Crew Productions, C=US";
        public static string RootCASerial = "7449A8EB07C997A6";
        /* Serial Number of Exe */
        public static string LauncherSerial = "NOT-SIGNED";

        /* Retrive CA Information */
        public static void Latest()
        {
            if (!DetectLinux.LinuxDetected())
            {
                try
                {
                    FunctionStatus.TLS();
                    WebClient Client = new WebClient();
                    Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    /* Download Up to Date Certificate Status */
                    var json_data = Client.DownloadString("http://crl.carboncrew.org/RCA-Info.json");
                    JsonRootCA API = JsonConvert.DeserializeObject<JsonRootCA>(json_data);

                    if (API.CN != null)
                    {
                        Log.Info("CERTIFICATE STORE: Setting Common Name -> " + API.CN);
                        RootCACommonName = API.CN;
                    }

                    if (API.Subject != null)
                    {
                        Log.Info("CERTIFICATE STORE: Setting Subject Name -> " + API.Subject);
                        RootCASubjectName = API.Subject;
                    }

                    if (API.Ids != null)
                    {
                        foreach (IdsModel entries in API.Ids)
                        {
                            if (entries.Serial != null)
                            {
                                Log.Info("CERTIFICATE STORE: Setting Serial Number -> " + entries.Serial);
                                RootCASerial = entries.Serial;
                            }
                        }
                    }

                    if (API.File != null)
                    {
                        foreach (FileModel entries in API.File)
                        {
                            if (entries.Name != null)
                            {
                                Log.Info("CERTIFICATE STORE: Setting Root CA File Name -> " + entries.Name);
                                RootCAFileName = entries.Name;
                            }

                            if (entries.Cer != null)
                            {
                                Log.Info("CERTIFICATE STORE: Setting Root CA File URL -> " + entries.Cer);
                                RootCAFileURL = entries.Cer;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    Log.Error("LAUNCHER UPDATER: " + error.Message);
                }

                /* Install Custom Root Certificate (If Default Values aren't used) */
                if (RootCASerial != "7449A8EB07C997A6")
                {
                    try
                    {
                        X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                        store.Open(OpenFlags.ReadWrite);

                        var certificatesThumbPrint = store.Certificates.Find(X509FindType.FindByThumbprint,
                            RootCASerial, false);

                        for (int i = 0; i < store.Certificates.Count; i++)
                        {
                            if (store.Certificates[i].SerialNumber == RootCASerial)
                            {
                                Log.Info("CERTIFICATE STORE: Found Root CA [" + store.Certificates[i].Subject + "]");
                                Log.Info("CERTIFICATE STORE: Serial Number [" + store.Certificates[i].SerialNumber + "]");
                                IsROOTCAInstalled = true;
                            }
                            else if (store.Certificates[i].SerialNumber != RootCASerial && store.Certificates[i].Subject == RootCASubjectName)
                            {
                                Log.Info("CERTIFICATE STORE: Removing OLD Root CA [" + store.Certificates[i].Subject + "]");
                                Log.Info("CERTIFICATE STORE: Serial Number [" + store.Certificates[i].SerialNumber + "]");
                                store.Remove(store.Certificates[i]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("CERTIFICATE STORE: Failed to Run. " + ex.Message);
                    }

                    string CertSaveLocation = AppDomain.CurrentDomain.BaseDirectory + RootCAFileName + ".cer";

                    try
                    {
                        if (IsROOTCAInstalled == false)
                        {
                            FunctionStatus.TLS();
                            WebClient Client = new WebClient();
                            Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            Client.DownloadFile(RootCAFileURL, CertSaveLocation);

                            X509Store store = new X509Store(StoreName.Root,
                            StoreLocation.LocalMachine);
                            store.Open(OpenFlags.ReadWrite);
                            X509Certificate2Collection collection = new X509Certificate2Collection();
                            X509Certificate2 cert = new X509Certificate2(CertSaveLocation);
                            byte[] encodedCert = cert.GetRawCertData();
                            Log.Info("CERTIFICATE STORE: We are now installing [" + RootCACommonName + "] certificate into the Trusted Root Certificate store ...");
                            store.Add(cert);
                            Log.Info("CERTIFICATE STORE: Done! [" + RootCACommonName + "] certificate was installed successfully.");
                            store.Close();
                        }
                        else
                        {
                            if (File.Exists(CertSaveLocation))
                            {
                                Log.Info("CERTIFICATE STORE: Removed [" + RootCACommonName + "] certificate from launcher folder.");
                                File.Delete(CertSaveLocation);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("CERTIFICATE STORE: Failed to Install. " + ex.Message);
                    }
                }
                else
                {
                    Log.Warning("CERTIFICATE STORE: Default Information was detected. Not running additional Function Calls");
                }
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(Application.ExecutablePath);
                Module module = assembly.GetModules().First();
                X509Certificate certificate = module.GetSignerCertificate();
                if (certificate != null)
                {
                    LauncherSerial = certificate.GetSerialNumberString();
                }
            }
            catch (Exception error)
            {
                Log.Error("CERTIFICATE CHECK: " + error.Message);
            }

            /* (Start Process) Check if Launcher Is Signed or Not */
            IsExeVerified.Check();
        }
    }
}
