using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Languages.Visual_Forms;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Validator.JSON;
using System;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.APICheckers
{
    class APIChecker
    {
        public static APIStatus CheckStatus(string URI, int Timer)
        {
            if (!string.IsNullOrWhiteSpace(URI))
            {
                HttpWebResponse ServerResponse = null;
                HttpWebRequest URLConnection = null;

                try
                {
                    Log.Checking("Checking Status: ".ToUpper() + URI);
                    FunctionStatus.TLS();
                    Uri ConvertedAPIURI = new Uri(URI);
                    /* Releases Connection Socket after 30 seconds */
                    ServicePointManager.FindServicePoint(ConvertedAPIURI).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Timer + 1).TotalMilliseconds;
                    URLConnection = (HttpWebRequest)WebRequest.Create(ConvertedAPIURI);
                    URLConnection.AllowAutoRedirect = false; /* Find out if this site is up and don't follow a redirector */
                    URLConnection.Method = "GET";
                    URLConnection.UserAgent = "SBRW Launcher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                    URLConnection.Timeout = (int)TimeSpan.FromSeconds(Timer).TotalMilliseconds;
                    URLConnection.KeepAlive = false;

                    try
                    {
                        ServerResponse = (HttpWebResponse)URLConnection.GetResponse();
                        Log.Completed("Checking Status: ".ToUpper() + URI + " [Is Online]");
                        return APIStatus.Online;
                        /* Do something with response.Headers to find out information about the request */
                    }
                    catch (WebException Error)
                    {
                        Log.Completed("Checking Status: ".ToUpper() + URI + " [WebException]");
                        return StatusCodes(URI, Error, (HttpWebResponse)Error.Response);
                    }
                }
                catch (Exception Error)
                {
                    Log.Completed("Checking Status: ".ToUpper() + URI + " [Exception]");
                    LogToFileAddons.OpenLog("API Checker", null, Error, null, true);
                    return APIStatus.UnknownError;
                }
                finally
                {
                    if (ServerResponse != null)
                    {
                        ServerResponse.Close();
                        ServerResponse.Dispose();
                    }

                    if (URLConnection != null)
                    {
                        URLConnection.Abort();
                    }
                }
            }
            else
            {
                Log.Error("CORE: " + URI + " URI can not be Null!");
                return APIStatus.Null;
            }
        }

        public static APIStatus StatusCodes(string URI, WebException Error, HttpWebResponse Response)
        {
            if (!string.IsNullOrWhiteSpace(Error.GetBaseException().Message)) 
            { Log.Error("CORE: " + URI + " Additional Details -> " + Error.GetBaseException().Message); }

            if (!string.IsNullOrWhiteSpace(URI))
            {
                switch (Error.Status)
                {
                    case WebExceptionStatus.CacheEntryNotFound:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Cache Entry Not Found");
                        return APIStatus.CacheEntryNotFound;
                    case WebExceptionStatus.ConnectFailure:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Connect Failure");
                        return APIStatus.ConnectFailure;
                    case WebExceptionStatus.ConnectionClosed:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Connection Closed");
                        return APIStatus.ConnectionClosed;
                    case WebExceptionStatus.KeepAliveFailure:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Keep Alive Failure");
                        return APIStatus.KeepAliveFailure;
                    case WebExceptionStatus.MessageLengthLimitExceeded:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Message Length Limit Exceeded");
                        return APIStatus.MessageLengthLimitExceeded;
                    case WebExceptionStatus.NameResolutionFailure:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Name Resolution Failure");
                        return APIStatus.NameResolutionFailure;
                    case WebExceptionStatus.Pending:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Pending");
                        return APIStatus.Pending;
                    case WebExceptionStatus.PipelineFailure:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Pipeline Failure");
                        return APIStatus.PipelineFailure;
                    case WebExceptionStatus.ProtocolError:
                        Log.Error("CORE: " + URI + " has an Error! Status Code: " + (int)Response.StatusCode);

                        /* Set flag if there was a timeout or some other issues */
                        switch ((int)Response.StatusCode)
                        {
                            case 404:
                                return APIStatus.NotFound;
                            case 500:
                                return APIStatus.ServerError;
                            case 502:
                                return APIStatus.ServerOverloaded;
                            case 503:
                                return APIStatus.ServerUnavailable;
                            case 504:
                                return APIStatus.GetWayTimeOut;
                            case 520:
                                return APIStatus.Unknown;
                            case 521:
                                return APIStatus.Offline;
                            case 522:
                                return APIStatus.ConnectionTimeOut;
                            case 523:
                                return APIStatus.OriginUnreachable;
                            case 524:
                                return APIStatus.Timeout;
                            case 525:
                                return APIStatus.SSLFailed;
                            case 526:
                                return APIStatus.InvaildSSL;
                            default:
                                return APIStatus.UnknownStatusCode;
                        }
                    case WebExceptionStatus.ProxyNameResolutionFailure:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Proxy Name Resolution Failure");
                        return APIStatus.ProxyNameResolutionFailure;
                    case WebExceptionStatus.ReceiveFailure:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Receive Failure");
                        return APIStatus.ReceiveFailure;
                    case WebExceptionStatus.RequestCanceled:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Request Canceled");
                        return APIStatus.RequestCanceled;
                    case WebExceptionStatus.RequestProhibitedByCachePolicy:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Request Prohibited By Cache Policy");
                        return APIStatus.RequestProhibitedByCachePolicy;
                    case WebExceptionStatus.SecureChannelFailure:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Secure Channel Failure");
                        return APIStatus.SecureChannelFailure;
                    case WebExceptionStatus.SendFailure:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Send Failure");
                        return APIStatus.SendFailure;
                    case WebExceptionStatus.ServerProtocolViolation:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Server Protocol Violation");
                        return APIStatus.ServerProtocolViolation;
                    case WebExceptionStatus.Success:
                        Log.Warning("CORE: " + URI + " has Encounterd an Error -> Success");
                        return APIStatus.Success;
                    case WebExceptionStatus.Timeout:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Timeout");
                        return APIStatus.Timeout;
                    case WebExceptionStatus.TrustFailure:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Trust Failure");
                        return APIStatus.TrustFailure;
                    case WebExceptionStatus.UnknownError:
                        Log.Error("CORE: " + URI + " has Encounterd an Error -> Unknown Error");
                        return APIStatus.UnknownError;
                    default:
                        Log.Error("CORE: " + URI + " is Offline!");
                        return APIStatus.Offline;
                }
            }
            else
            {
                Log.Error("CORE: " + URI + " URI can not be Null!");
                return APIStatus.Null;
            }
        }

        public static string StatusStrings(APIStatus Error)
        {
            switch (Error)
            {
                case APIStatus.CacheEntryNotFound:
                    return "Cache Entry Not Found";
                case APIStatus.ConnectFailure:
                    return "Connect Failure";
                case APIStatus.ConnectionClosed:
                    return "Connection Closed";
                case APIStatus.KeepAliveFailure:
                    return "Keep Alive Failure";
                case APIStatus.MessageLengthLimitExceeded:
                    return "Message Length Limit Exceeded";
                case APIStatus.NameResolutionFailure:
                    return "Name Resolution Failure";
                case APIStatus.Pending:
                    return "Pending";
                case APIStatus.PipelineFailure:
                    return "Pipeline Failure";
                case APIStatus.NotFound:
                    return "Not Found";
                case APIStatus.ServerError:
                    return "Server Error";
                case APIStatus.ServerOverloaded:
                    return "Server Overloaded";
                case APIStatus.ServerUnavailable:
                    return "Server Unavailable";
                case APIStatus.GetWayTimeOut:
                    return "GetWay Time Out";
                case APIStatus.ConnectionTimeOut:
                    return "Connection Timed Out";
                case APIStatus.OriginUnreachable:
                    return "Origin Unreachable";
                case APIStatus.Timeout:
                    return "Timed Out";
                case APIStatus.SSLFailed:
                    return "SSL Failed";
                case APIStatus.InvaildSSL:
                    return "Invalid SSL";
                case APIStatus.UnknownStatusCode:
                    return "Unknown Status Code";
                case APIStatus.ProxyNameResolutionFailure:
                    return "Proxy Name Resolution Failure";
                case APIStatus.ReceiveFailure:
                    return "Receive Failure";
                case APIStatus.RequestCanceled:
                    return "Request Canceled";
                case APIStatus.RequestProhibitedByCachePolicy:
                    return "Request Prohibited By Cache Policy";
                case APIStatus.SecureChannelFailure:
                    return "Secure Channel Failure";
                case APIStatus.SendFailure:
                    return "Send Failure";
                case APIStatus.ServerProtocolViolation:
                    return "Server Protocol Violation";
                case APIStatus.Success:
                    return "Success";
                case APIStatus.TrustFailure:
                    return "Trust Failure";
                case APIStatus.Unknown:
                case APIStatus.UnknownError:
                    return "Unknown Error";
                case APIStatus.Null:
                    return "URL was Null";
                case APIStatus.Online:
                    return "Online";
                default:
                    return "Offline";
            }
        }
    }

    class VisualsAPIChecker
    {
        public static bool UnitedSL = false;
        public static bool UnitedCDNL = false;
        public static APIStatus UnitedSC = APIStatus.Unknown;
        public static bool UnitedAPI() => (UnitedSL && UnitedCDNL);

        public static bool CarbonSL = false;
        public static bool CarbonCDNL = false;
        public static APIStatus CarbonSC = APIStatus.Unknown;
        public static bool CarbonAPI() => (CarbonSL && CarbonCDNL);

        public static bool CarbonTwoSL = false;
        public static bool CarbonTwoCDNL = false;
        public static APIStatus CarbonTwoSC = APIStatus.Unknown;
        public static bool CarbonAPITwo() => (CarbonTwoSL && CarbonTwoCDNL);

        public static bool GitHubAPI = false;
        public static APIStatus GitHubAPISC = APIStatus.Unknown;

        public static void PingAPIStatus()
        {
            Log.Checking("API: Checking Status");
            Log.Checking("API Status: WorldUnited");
            switch (UnitedSC = APIChecker.CheckStatus(URLs.Main + "/serverlist.json", 15))
            {
                case APIStatus.Online:
                    UnitedSL = RetrieveJSON(URLs.Main + "/serverlist.json", "SL");
                    if (UnitedSL) { UnitedCDNL = RetrieveJSON(URLs.Main + "/cdn_list.json", "CDNL"); }
                    Log.Completed("API Status: WorldUnited");
                    break;
                default:
                    Log.Completed("API Status: WorldUnited");
                    break;
            }

            if (!UnitedAPI())
            {
                Log.Checking("API Status: DavidCarbon");
                switch (CarbonSC = APIChecker.CheckStatus(URLs.Static + "/serverlist.json", 15))
                {
                    case APIStatus.Online:
                        if (!UnitedSL) { CarbonSL = RetrieveJSON(URLs.Static + "/serverlist.json", "SL"); }
                        else { CarbonSL = true; }
                        if (!UnitedCDNL) { CarbonCDNL = RetrieveJSON(URLs.Static + "/cdn_list.json", "CDNL"); }
                        else { CarbonCDNL = true; }
                        Log.Completed("API Status: DavidCarbon");
                        break;
                    default:
                        Log.Completed("API Status: DavidCarbon");
                        break;
                }
            }
            else
            {
                CarbonSL = true;
                CarbonCDNL = true;
            }

            if (!CarbonAPI())
            {
                Log.Checking("API Status: DavidCarbon [Second]");
                switch (CarbonTwoSC = APIChecker.CheckStatus(URLs.Static_Alt + "/serverlist.json", 15))
                {
                    case APIStatus.Online:
                        if (!CarbonSL) { CarbonTwoSL = RetrieveJSON(URLs.Static_Alt + "/serverlist.json", "SL"); }
                        else { CarbonTwoSL = true; }
                        if (!CarbonCDNL) { CarbonTwoCDNL = RetrieveJSON(URLs.Static_Alt + "/cdn_list.json", "CDNL"); }
                        else { CarbonTwoCDNL = true; }
                        Log.Completed("API Status: DavidCarbon [Second]");
                        break;
                    default:
                        Log.Completed("API Status: DavidCarbon [Second]");
                        break;
                }
            }
            else
            {
                CarbonTwoSL = true;
                CarbonTwoCDNL = true;
            }

            Log.Checking("API: Test #2");

            /* Check If Launcher Failed to Connect to any APIs */
            if (!CarbonAPITwo())
            {
                DiscordLauncherPresence.Status("Start Up", "Launcher Encountered API Errors");

                if (MessageBox.Show(null, Translations.Database("VisualsAPIChecker_TextBox_No_API"),
                    Translations.Database("VisualsAPIChecker_TextBox_No_API_P2"), 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    FunctionStatus.LauncherForceClose = true;
                }
                else
                {
                    Log.Warning("PRE-CHECK: User has Bypassed 'No Internet Connection' Check and will Continue");
                    MessageBox.Show(Translations.Database("VisualsAPIChecker_TextBox_No_API_P3"),
                        Translations.Database("VisualsAPIChecker_TextBox_No_API_P4"));
                }
            }
            Log.Completed("API: Test #2 Done");

            if (FunctionStatus.LauncherForceClose)
            {
                FunctionStatus.ErrorCloseLauncher("Closing From API Check Error", false);
            }
            else
            {
                FunctionStatus.IsVisualAPIsChecked = true;

                Log.Info("LIST CORE: Moved to Function");
                /* (Start Process) Check ServerList Status */
                ServerListUpdater.GetList();
            }
        }

        private static string OnlineListJson;

        private static bool RetrieveJSON(string JSONUrl, string Function)
        {
            Log.Checking("JSON LIST: Retriving " + JSONUrl);
            try
            {
                

                FunctionStatus.TLS();
                Uri URLCall = new Uri(JSONUrl);
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
                    OnlineListJson = Client.DownloadString(URLCall);

                    if (URLCall.OriginalString.Contains(URLs.Main))
                    { UnitedSC = APIStatus.Online; }
                    else if (URLCall.OriginalString.Contains(URLs.Static))
                    { CarbonSC = APIStatus.Online; }
                    else if (URLCall.OriginalString.Contains(URLs.Static_Alt))
                    { CarbonTwoSC = APIStatus.Online; }

                    Log.UrlCall("JSON LIST: Retrieved " + JSONUrl);
                }
                catch (WebException Error)
                {
                    APIStatus API_Status = APIChecker.StatusCodes(JSONUrl, Error, (HttpWebResponse)Error.Response);

                    if (URLCall.OriginalString.Contains(URLs.Main))
                    { UnitedSC = API_Status; }
                    else if (URLCall.OriginalString.Contains(URLs.Static))
                    { CarbonSC = API_Status; }
                    else if (URLCall.OriginalString.Contains(URLs.Static_Alt))
                    { CarbonTwoSC = API_Status; }
                    
                    return false;
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("JSON LIST", null, Error, null, true);

                    if (URLCall.OriginalString.Contains(URLs.Main))
                    { UnitedSC = APIStatus.Unknown; }
                    else if (URLCall.OriginalString.Contains(URLs.Static))
                    { CarbonSC = APIStatus.Unknown; }
                    else if (URLCall.OriginalString.Contains(URLs.Static_Alt))
                    { CarbonTwoSC = APIStatus.Unknown; }

                    return false;
                }
                finally
                {
                    if (Client != null)
                    {
                        Client.Dispose();
                    }
                }

                if (IsJSONValid.ValidJson(OnlineListJson))
                {
                    switch (Function)
                    {
                        case "SL":
                            ServerListUpdater.CachedJSONList = OnlineListJson;
                            break;
                        case "CDNL":
                            CDNListUpdater.CachedJSONList = OnlineListJson;
                            break;
                        default:
                            break;
                    }
                    Log.Completed("JSON LIST: Valid " + JSONUrl);

                    return true;
                }
                else
                {
                    Log.Completed("JSON LIST: Invalid " + JSONUrl);
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                if (OnlineListJson != null)
                {
                    OnlineListJson = null;
                }
            }
        }
    }
}
