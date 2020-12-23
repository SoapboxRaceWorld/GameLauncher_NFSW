using GameLauncher.App.Classes.Logger;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class CertificateStore
    {
        private static bool IsROOTCAInstalled = false;

        private static string RootCAName = "Carbon Crew CA";

        public static void Check()
        {
            try
            {
                X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);

                var certificates = store.Certificates.Find(
                    X509FindType.FindBySubjectName,
                    RootCAName,
                    false);

                if (certificates != null && certificates.Count > 0)
                {
                    Log.Info("CERTIFICATE STORE: Found Root CA [" + RootCAName + "]");
                    IsROOTCAInstalled = true;
                }
                else if (certificates == null && certificates.Count > 0)
                {
                    Log.Error("CERTIFICATE STORE: Hey! You don't have any Certificates Installed at All!");
                    IsROOTCAInstalled = false;
                }
                else
                {
                    Log.Warning("CERTIFICATE STORE: [" + RootCAName + "] Not Found");
                    IsROOTCAInstalled = false;
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
            string CertSaveLocation = AppDomain.CurrentDomain.BaseDirectory + "CC-CA.cer";

            try
            {
                if (IsROOTCAInstalled == false)
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile("http://crl.carboncrew.org/CC-CA.cer", CertSaveLocation);

                    X509Store store = new X509Store(StoreName.Root,
                    StoreLocation.LocalMachine);
                    store.Open(OpenFlags.ReadWrite);
                    X509Certificate2Collection collection = new X509Certificate2Collection();
                    X509Certificate2 cert = new X509Certificate2(CertSaveLocation);
                    byte[] encodedCert = cert.GetRawCertData();
                    Log.Info("CERTIFICATE STORE: We are now installing [" + RootCAName + "] certificate into the Trusted Root Certificate store ...");
                    store.Add(cert);
                    Log.Info("CERTIFICATE STORE: Done! [" + RootCAName + "] certificate was installed successfully.");
                    store.Close();
                }
                else
                {
                    if (File.Exists(CertSaveLocation))
                    {
                        Log.Info("CERTIFICATE STORE: Removed [" + RootCAName + "] certificate from launcher folder.");
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
