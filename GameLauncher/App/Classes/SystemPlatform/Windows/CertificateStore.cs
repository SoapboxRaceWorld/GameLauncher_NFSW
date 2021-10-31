using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Validator.JSON;
using GameLauncher.App.Classes.LauncherCore.Validator.VerifyTrust;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
        public static string LauncherSerial;
        /* ROOT CA JSON Cache */
        private static string RootCAJson;

        /* Retrieve CA Information */
        /// <summary>
        /// Retrieves the Root CA JSON file with the latest details of the Certificate
        /// </summary>
        /// <remarks>Sets the Certificate Details For Launcher Comparison</remarks>
        public static void Latest()
        {
            if (!UnixOS.Detected())
            {
                Log.Checking("CERTIFICATE STORE: Is Installed or Not");
                DiscordLauncherPresence.Status("Start Up", "Checking Root Certificate Authority");

                try
                {
                    Uri URLCall = new Uri("http://crl.carboncrew.org/RCA-Info.json");
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

                    try
                    {
                        /* Download Up to Date Certificate Status */
                        RootCAJson = Client.DownloadString(URLCall);
                    }
                    catch (WebException Error)
                    {
                        APIChecker.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                            Error, (HttpWebResponse)Error.Response);
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("CERTIFICATE STORE", null, Error, null, true);
                    }
                    finally
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }

                    if (IsJSONValid.ValidJson(RootCAJson))
                    {
                        JsonRootCA API = JsonConvert.DeserializeObject<JsonRootCA>(RootCAJson);

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

                        if (API != null)
                        {
                            API = null;
                        }
                    }
                    else
                    {
                        Log.Warning("CERTIFICATE STORE: Received Invalid JSON Data");
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("CERTIFICATE STORE", null, Error, null, true);
                }
                finally
                {
                    if (RootCAJson != null)
                    {
                        RootCAJson = null;
                    }
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
                        store.Close();
                        store.Dispose();
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("CERTIFICATE STORE", null, Error, null, true);
                    }

                    string CertSaveLocation = Strings.Encode(Path.Combine(Locations.LauncherFolder, RootCAFileName + ".cer"));

                    try
                    {
                        if (!IsROOTCAInstalled)
                        {
                            Uri URLCall = new Uri(RootCAFileURL);
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

                            try
                            {
                                /* Download Up to Date Certificate Status */
                                Client.DownloadFile(URLCall, CertSaveLocation);
                            }
                            catch (WebException Error)
                            {
                                APIChecker.StatusCodes(RootCAFileURL, Error, (HttpWebResponse)Error.Response);
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("CERTIFICATE STORE", null, Error, null, true);
                            }
                            finally
                            {
                                if (Client != null)
                                {
                                    Client.Dispose();
                                }
                            }

                            if (File.Exists(CertSaveLocation))
                            {
                                X509Store Store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                                Store.Open(OpenFlags.ReadWrite);
                                X509Certificate2Collection collection = new X509Certificate2Collection();
                                X509Certificate2 cert = new X509Certificate2(CertSaveLocation);
                                byte[] encodedCert = cert.GetRawCertData();
                                Log.Info("CERTIFICATE STORE: We are now installing [" + RootCACommonName + "] certificate into the Trusted Root Certificate store ...");
                                Store.Add(cert);
                                Log.Info("CERTIFICATE STORE: Done! [" + RootCACommonName + "] certificate was installed successfully.");
                                Store.Close();
                                Store.Dispose();
                            }
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("CERTIFICATE STORE", null, Error, null, true);
                    }

                    try
                    {
                        if (File.Exists(CertSaveLocation))
                        {
                            Log.Info("CERTIFICATE STORE: Removed [" + RootCACommonName + "] certificate from launcher folder.");
                            File.Delete(CertSaveLocation);
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("CERTIFICATE STORE", null, Error, null, true);
                    }
                }
                else
                {
                    Log.Warning("CERTIFICATE STORE: Default Information was detected. Not running additional Function Calls");
                }

                Log.Completed("CERTIFICATE STORE: Done");
            }

            Log.Checking("CERTIFICATE CHECK: Is Signed or Not");
            try
            {
                X509Certificate certificate = null;

                try
                {
                    Assembly assembly = Assembly.LoadFrom(Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameLauncher)));
                    Module module = assembly.GetModules().First();
                    certificate = module.GetSignerCertificate();

                    if (certificate != null)
                    {
                        LauncherSerial = certificate.GetSerialNumberString();
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("CERTIFICATE CHECK", null, Error, null, true);
                }
                finally
                {
                    if (certificate != null)
                    {
                        certificate.Dispose();
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("CERTIFICATE CHECK", null, Error, null, true);
            }
            Log.Completed("CERTIFICATE CHECK: Done");

            Log.Info("VERIFIED: Moved to Function");
            /* (Start Process) Check if Launcher Is Signed or Not */
            IsExeVerified.Check();
        }
    }
}
