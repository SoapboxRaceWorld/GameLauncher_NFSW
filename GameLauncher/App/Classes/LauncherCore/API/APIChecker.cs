using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Languages.Visual_Forms;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Validator.JSON;
using SBRW.Launcher.Core.Classes.Cache;
using SBRW.Launcher.Core.Classes.Extension.Api_;
using SBRW.Launcher.Core.Classes.Extension.Logging_;
using SBRW.Launcher.Core.Classes.Extension.Web_;
using System;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.APICheckers
{
    class APIChecker
    {
        public static string StatusStrings(APIStatus Error)
        {
            switch (Error)
            {
                case APIStatus.CacheEntryNotFound:
                    return Translations.Database("APIChecker_CacheEntryNotFound");
                case APIStatus.ConnectFailure:
                    return Translations.Database("APIChecker_ConnectFailure");
                case APIStatus.ConnectionClosed:
                    return Translations.Database("APIChecker_ConnectionClosed");
                case APIStatus.KeepAliveFailure:
                    return Translations.Database("APIChecker_KeepAliveFailure");
                case APIStatus.MessageLengthLimitExceeded:
                    return Translations.Database("APIChecker_MessageLengthLimitExceeded");
                case APIStatus.NameResolutionFailure:
                    return Translations.Database("APIChecker_NameResolutionFailure");
                case APIStatus.Pending:
                    return Translations.Database("APIChecker_Pending");
                case APIStatus.PipelineFailure:
                    return Translations.Database("APIChecker_PipelineFailure");
                case APIStatus.NotFound:
                    return Translations.Database("APIChecker_NotFound");
                case APIStatus.ServerError:
                    return Translations.Database("APIChecker_ServerError");
                case APIStatus.ServerOverloaded:
                    return Translations.Database("APIChecker_ServerOverloaded");
                case APIStatus.ServerUnavailable:
                    return Translations.Database("APIChecker_ServerUnavailable");
                case APIStatus.GetWayTimeOut:
                    return Translations.Database("APIChecker_GetWayTimeOut");
                case APIStatus.ConnectionTimeOut:
                    return Translations.Database("APIChecker_ConnectionTimeOut");
                case APIStatus.OriginUnreachable:
                    return Translations.Database("APIChecker_OriginUnreachable");
                case APIStatus.Timeout:
                    return Translations.Database("APIChecker_Timeout");
                case APIStatus.SSLFailed:
                    return Translations.Database("APIChecker_SSLFailed");
                case APIStatus.InvaildSSL:
                    return Translations.Database("APIChecker_InvaildSSL");
                case APIStatus.UnknownStatusCode:
                    return Translations.Database("APIChecker_UnknownStatusCode");
                case APIStatus.ProxyNameResolutionFailure:
                    return Translations.Database("APIChecker_ProxyNameResolutionFailure");
                case APIStatus.ReceiveFailure:
                    return Translations.Database("APIChecker_ReceiveFailure");
                case APIStatus.RequestCanceled:
                    return Translations.Database("APIChecker_RequestCanceled");
                case APIStatus.RequestProhibitedByCachePolicy:
                    return Translations.Database("APIChecker_RequestProhibitedByCachePolicy");
                case APIStatus.SecureChannelFailure:
                    return Translations.Database("APIChecker_SecureChannelFailure");
                case APIStatus.SendFailure:
                    return Translations.Database("APIChecker_SendFailure");
                case APIStatus.ServerProtocolViolation:
                    return Translations.Database("APIChecker_ServerProtocolViolation");
                case APIStatus.Success:
                    return Translations.Database("APIChecker_Success");
                case APIStatus.TrustFailure:
                    return Translations.Database("APIChecker_TrustFailure");
                case APIStatus.Unknown:
                case APIStatus.UnknownError:
                    return Translations.Database("APIChecker_Unknown");
                case APIStatus.Null:
                    return Translations.Database("APIChecker_Null");
                case APIStatus.Online:
                    return Translations.Database("APIChecker_Online");
                default:
                    return Translations.Database("APIChecker_Offline");
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
            switch (UnitedSC = API_Core.StatusCheck(URLs.Main + "/serverlist.json", 15))
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
                switch (CarbonSC = API_Core.StatusCheck(URLs.Static + "/serverlist.json", 15))
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
                switch (CarbonTwoSC = API_Core.StatusCheck(URLs.Static_Alt + "/serverlist.json", 15))
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
                Uri URLCall = new Uri(JSONUrl);
                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                var Client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };

                if (!Launcher_Value.Launcher_Alternative_Webcalls()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
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
                    APIStatus API_Status = API_Core.StatusCodes(JSONUrl, Error, (HttpWebResponse)Error.Response);

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
