using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.Logger;
using System;
using System.Net;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.APICheckers
{
    class APIChecker
    {
        public static APIStatus CheckStatus(string APIURI)
        {
            if (!string.IsNullOrEmpty(APIURI))
            {
                HttpWebResponse serverResponse = null;

                try
                {
                    FunctionStatus.TLS();
                    HttpWebRequest requestAPIStatus = (HttpWebRequest)HttpWebRequest.Create(APIURI);
                    requestAPIStatus.AllowAutoRedirect = false; /* Find out if this site is up and don't follow a redirector */
                    requestAPIStatus.Method = "GET";
                    requestAPIStatus.UserAgent = "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                    serverResponse = (HttpWebResponse)requestAPIStatus.GetResponse();
                    Log.Info("CORE: " + APIURI + " is Online!");
                    return APIStatus.Online;
                    /* Do something with response.Headers to find out information about the request */
                }
                catch (WebException Error)
                {
                    ErrorCode(APIURI, Error);
                }
                finally
                {
                    if (serverResponse != null)
                    {
                        serverResponse.Close();
                    }
                }
            }

            return APIStatus.Null;
        }

        public static APIStatus ErrorCode(string APIURI, WebException Error)
        {
            try
            {
                if (Error.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse serverResponse = (HttpWebResponse)Error.Response;

                    Console.Write("Errorcode: {0}\n", (int)serverResponse.StatusCode);
                    Log.Error("CORE: " + APIURI + " has an Error! Status Code: " + (int)serverResponse.StatusCode);

                    if ((int)serverResponse.StatusCode == 400)
                    {
                        return APIStatus.BadRequest;
                    }
                    else if ((int)serverResponse.StatusCode == 401)
                    {
                        return APIStatus.Unauthorized;
                    }
                    else if ((int)serverResponse.StatusCode == 403)
                    {
                        return APIStatus.Forbidden;
                    }
                    else if ((int)serverResponse.StatusCode == 404)
                    {
                        return APIStatus.NotFound;
                    }
                    else if ((int)serverResponse.StatusCode == 500)
                    {
                        return APIStatus.ServerError;
                    }
                    else if ((int)serverResponse.StatusCode == 501)
                    {
                        return APIStatus.NotImplmented;
                    }
                    else if ((int)serverResponse.StatusCode == 502)
                    {
                        return APIStatus.BadGateway;
                    }
                    else if ((int)serverResponse.StatusCode == 503)
                    {
                        return APIStatus.ServerUnavailable;
                    }
                    else if ((int)serverResponse.StatusCode == 504)
                    {
                        return APIStatus.GetWayTimeOut;
                    }
                    else if ((int)serverResponse.StatusCode == 520)
                    {
                        return APIStatus.OriginError;
                    }
                    else if ((int)serverResponse.StatusCode == 521)
                    {
                        return APIStatus.Offline;
                    }
                    else if ((int)serverResponse.StatusCode == 522)
                    {
                        return APIStatus.ConnectionTimeOut;
                    }
                    else if ((int)serverResponse.StatusCode == 523)
                    {
                        return APIStatus.OriginUnreachable;
                    }
                    else if ((int)serverResponse.StatusCode == 524)
                    {
                        return APIStatus.Timeout;
                    }
                    else if ((int)serverResponse.StatusCode == 525)
                    {
                        return APIStatus.SSLFailed;
                    }
                    else if ((int)serverResponse.StatusCode == 526)
                    {
                        return APIStatus.InvaildSSL;
                    }
                    else
                    {
                        return APIStatus.Unknown;
                    }
                    /* Set flag if there was a timeout or some other issues */
                }
                else
                {
                    Console.Write("------------");
                    Console.Write("Error: {0}", Error.Status);
                    Console.Write("------------\n");
                    Log.Error("CORE: " + APIURI + " is Offline!");
                    return APIStatus.Offline;
                }
            }
            catch
            {
                return APIStatus.Null;
            }
        }
    }

    class VisualsAPIChecker
    {
        public static bool UnitedAPI = true;

        public static bool CarbonAPI = true;

        public static bool CarbonAPITwo = true;

        public static bool WOPLAPI = true;

        public static void PingAPIStatus()
        {
            switch (APIChecker.CheckStatus(URLs.Main + "/serverlist.json"))
            {
                case APIStatus.Online:
                    break;
                default:
                    UnitedAPI = false;
                    break;
            }

            if (UnitedAPI == false)
            {
                switch (APIChecker.CheckStatus(URLs.Static + "/serverlist.json"))
                {
                    case APIStatus.Online:
                        break;
                    default:
                        CarbonAPI = false;
                        break;
                }
            }

            if (CarbonAPI == false)
            {
                switch (APIChecker.CheckStatus(URLs.Static_Alt + "/serverlist.json"))
                {
                    case APIStatus.Online:
                        break;
                    default:
                        CarbonAPITwo = false;
                        break;
                }
            }

            if (CarbonAPITwo == false)
            {
                switch (APIChecker.CheckStatus(URLs.WOPL + "/serverlist.json"))
                {
                    case APIStatus.Online:
                        break;
                    default:
                        WOPLAPI = false;
                        break;
                }
            }

            FunctionStatus.IsVisualAPIsChecked = true;
        }
    }
}
