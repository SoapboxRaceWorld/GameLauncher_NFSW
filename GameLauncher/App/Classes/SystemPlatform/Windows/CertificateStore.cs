using GameLauncher.App.Classes.Logger;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

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

        public static void Check()
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
                        Log.Info("CERTIFICATE STORE: With Thumprint [" + store.Certificates[i].SerialNumber + "]");
                        IsROOTCAInstalled = true;
                    }
                    else if (store.Certificates[i].SerialNumber != RootCASerial && store.Certificates[i].Subject == RootCASubjectName)
                    {
                        Log.Info("CERTIFICATE STORE: Removing OLD Root CA [" + store.Certificates[i].Subject + "]");
                        Log.Info("CERTIFICATE STORE: With Thumprint [" + store.Certificates[i].SerialNumber + "]");
                        store.Remove(store.Certificates[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("CERTIFICATE STORE: Failed to Run. " + ex.Message);
            }

            InstallNewRootCA();
        }

        private static void InstallNewRootCA()
        {
            string CertSaveLocation = AppDomain.CurrentDomain.BaseDirectory + RootCAFileName + ".cer";

            try
            {
                if (IsROOTCAInstalled == false)
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(RootCAFileURL, CertSaveLocation);

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
    }
}
