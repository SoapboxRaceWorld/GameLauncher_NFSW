using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using DiscordButton = DiscordRPC.Button;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.LauncherCore.Client;
using GameLauncher.App.Classes.Logger;
using System.Text;

namespace GameLauncher.App.Classes.LauncherCore.RPC
{
    class DiscordGamePresence
    {
        public static RichPresence _presence = new RichPresence();

        /* Some checks */
        private static readonly string serverName = ServerProxy.Instance.GetServerName();
        private static bool canUpdateProfileField = false;
        private static bool eventTerminatedManually = false;
        private static int EventID;
        private static string carslotsXML = String.Empty;
        private static bool inSafeHouse = false;

        /* Some data related, can be touched. */
        public static string PersonaId = String.Empty;
        public static string PersonaName = String.Empty;
        public static string PersonaLevel = String.Empty;
        public static string PersonaAvatarId = String.Empty;
        public static string PersonaCarId = String.Empty;
        public static string PersonaCarName = String.Empty;
        public static string LoggedPersonaId = String.Empty;
        public static string LauncherRPC = "SBRW Launcher: v" + Theming.PrivacyRPCBuild;
        public static int PersonaTreasure = 0;
        public static int TotalTreasure = 15;
        public static int THDay = 0;
        public static List<string> PersonaIds = new List<string>();

        public static void HandleGameState(string uri, string serverreply, string GET)
        {
            var SBRW_XML = new XmlDocument();
            string[] splitted_uri = uri.Split('/');

            String _serverPanelLink = InformationCache.SelectedServerJSON.webPanelUrl;
            String _serverWebsiteLink = InformationCache.SelectedServerJSON.homePageUrl;
            String _serverDiscordLink = InformationCache.SelectedServerJSON.discordUrl;
            if (!String.IsNullOrWhiteSpace(_serverWebsiteLink) || !String.IsNullOrWhiteSpace(_serverDiscordLink) || !String.IsNullOrWhiteSpace(_serverPanelLink))
            {
                DiscordLauncherPresense.ButtonsList.Clear();

                if (!String.IsNullOrWhiteSpace(_serverPanelLink))
                {
                    /* Let's format it now, if possible */
                    if (AntiCheat.persona_id == String.Empty || AntiCheat.persona_name == String.Empty)
                    {
                        DiscordLauncherPresense.ButtonsList.Add(new DiscordButton()
                        {
                            Label = "View Panel",
                            Url = _serverPanelLink.Split(new string[] { "{sep}" }, StringSplitOptions.None)[0]
                        });
                    }
                    else
                    {
                        _serverPanelLink = _serverPanelLink.Replace("{personaid}", AntiCheat.persona_id);
                        _serverPanelLink = _serverPanelLink.Replace("{personaname}", AntiCheat.persona_name);
                        _serverPanelLink = _serverPanelLink.Replace("{sep}", String.Empty);

                        DiscordLauncherPresense.ButtonsList.Add(new DiscordButton()
                        {
                            Label = "Check " + AntiCheat.persona_name + " on Panel",
                            Url = _serverPanelLink
                        });
                    }
                }
                else if (!String.IsNullOrWhiteSpace(_serverWebsiteLink) && _serverWebsiteLink != _serverDiscordLink)
                {
                    DiscordLauncherPresense.ButtonsList.Add(new DiscordButton()
                    {
                        Label = "Website",
                        Url = _serverWebsiteLink
                    });
                }

                if (!String.IsNullOrWhiteSpace(_serverDiscordLink))
                {
                    DiscordLauncherPresense.ButtonsList.Add(new DiscordButton()
                    {
                        Label = "Discord",
                        Url = _serverDiscordLink
                    });
                }
            }

            if (uri == "/User/SecureLoginPersona")
            {
                LoggedPersonaId = GET.Split(';').Last().Split('=').Last();
                canUpdateProfileField = true;
            }

            if (uri == "/User/SecureLogoutPersona")
            {
                PersonaId = String.Empty;
                PersonaName = String.Empty;
                PersonaLevel = String.Empty;
                PersonaAvatarId = String.Empty;
                PersonaCarId = String.Empty;
                PersonaCarName = String.Empty;
                LauncherRPC = String.Empty;
                PersonaTreasure = 0;
            }

            /* FIRST PERSONA EVER LOCALIZED IN CODE */
            if (uri == "/User/GetPermanentSession")
            {
                /* Moved Statuses.cs Code to Gist | Check RemovedClasses.cs for Link */
                //try { Statuses.getToken(); } catch { }

                try
                {
                    SBRW_XML.LoadXml(serverreply);

                    PersonaName = SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/Name").InnerText.Replace("¤", "[S]");
                    PersonaLevel = SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/Level").InnerText;
                    PersonaAvatarId = "avatar_" + SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/IconIndex").InnerText;
                    PersonaId = SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/PersonaId").InnerText;

                    /* Let's get rest of PERSONAIDs */
                    XmlNode UserInfo = SBRW_XML.SelectSingleNode("UserInfo");
                    XmlNodeList personas = UserInfo.SelectNodes("personas/ProfileData");
                    foreach (XmlNode node in personas)
                    {
                        PersonaIds.Add(node.SelectSingleNode("PersonaId").InnerText);
                    }
                }
                catch (Exception Error)
                {
                    Log.Error("DISCORD GAME PRESENSE: " + Error.Message);
                    Log.Error("DISCORD GAME PRESENSE [HResult]: " + Error.HResult);
                    Log.ErrorInner("DISCORD GAME PRESENSE [Full Report]: " + Error.ToString());
                }
            }

            /* CREATE/DELETE PERSONA Handler  */
            if (uri == "/DriverPersona/CreatePersona")
            {
                SBRW_XML.LoadXml(serverreply);
                PersonaIds.Add(SBRW_XML.SelectSingleNode("ProfileData/PersonaId").InnerText);
            }

            /* DRIVING CARNAME */
            if (uri == "/DriverPersona/GetPersonaInfo" && canUpdateProfileField == true)
            {
                if (LoggedPersonaId == GET.Split(';').Last().Split('=').Last())
                {
                    SBRW_XML.LoadXml(serverreply);
                    PersonaName = SBRW_XML.SelectSingleNode("ProfileData/Name").InnerText.Replace("¤", "[S]");
                    PersonaLevel = SBRW_XML.SelectSingleNode("ProfileData/Level").InnerText;
                    PersonaAvatarId = "avatar_" + SBRW_XML.SelectSingleNode("ProfileData/IconIndex").InnerText;
                    PersonaId = SBRW_XML.SelectSingleNode("ProfileData/PersonaId").InnerText;

                    AntiCheat.persona_id = SBRW_XML.SelectSingleNode("ProfileData/PersonaId").InnerText;
                    AntiCheat.persona_name = SBRW_XML.SelectSingleNode("ProfileData/Name").InnerText.Replace("¤", "[S]");
                }
            }

            if (uri == "/events/gettreasurehunteventsession")
            {
                /* Treasure Hunt Streak/Gems From Server */
                PersonaTreasure = 0;
                TotalTreasure = 15;
                THDay = 0;

                SBRW_XML.LoadXml(serverreply);
                var xPersonaTreasure = Convert.ToInt32(SBRW_XML.SelectSingleNode("TreasureHuntEventSession/CoinsCollected").InnerText);
                for (var i = 0; i < 15; i++)
                {
                    if ((xPersonaTreasure & (1 << (15 - i))) != 0) PersonaTreasure++;
                }

                TotalTreasure = Convert.ToInt32(SBRW_XML.SelectSingleNode("TreasureHuntEventSession/NumCoins").InnerText);
                THDay = Convert.ToInt32(SBRW_XML.SelectSingleNode("TreasureHuntEventSession/Streak").InnerText);
            }

            if (uri == "/events/notifycoincollected")
            {
                /* Actively Collection Treasure Hunt Gems */
                PersonaTreasure++;

                if (PersonaTreasure != TotalTreasure)
                {
                    _presence.Details = "Collecting Gems (" + PersonaTreasure + " of " + TotalTreasure + ")";
                }
                else if (PersonaTreasure == TotalTreasure)
                {
                    _presence.Details = "Finished Collecting Gems (" + PersonaTreasure + " of " + TotalTreasure + ")";
                }

                _presence.State = LauncherRPC;
                _presence.Assets = new Assets
                {
                    LargeImageText = PersonaName + " - Level: " + PersonaLevel,
                    LargeImageKey = PersonaAvatarId,
                    SmallImageText = "Treasure Hunt - Day: " + THDay,
                    SmallImageKey = "gamemode_treasure"
                };
                _presence.Buttons = DiscordLauncherPresense.ButtonsList.ToArray();

                if (DiscordLauncherPresense.Running()) DiscordLauncherPresense.Client.SetPresence(_presence);
            }

            /* IN SAFEHOUSE/FREEROAM */
            if (uri == "/DriverPersona/UpdatePersonaPresence")
            {
                string UpdatePersonaPresenceParam = GET.Split(';').Last().Split('=').Last();
                _presence.Assets = new Assets();
                if (UpdatePersonaPresenceParam == "1")
                {
                    _presence.Details = "Driving " + PersonaCarName;
                    _presence.Assets.SmallImageText = "In-Freeroam";
                    _presence.Assets.SmallImageKey = "gamemode_freeroam";
                    _presence.State = LauncherRPC;
                    FunctionStatus.CanCloseGame = true;
                    inSafeHouse = false;
                }
                else
                {
                    _presence.Details = "In Safehouse";
                    _presence.Assets.SmallImageText = "In-Safehouse";
                    _presence.Assets.SmallImageKey = "gamemode_safehouse";
                    _presence.State = serverName;
                    FunctionStatus.CanCloseGame = false;
                    inSafeHouse = true;
                }

                _presence.Assets.LargeImageText = PersonaName + " - Level: " + PersonaLevel;
                _presence.Assets.LargeImageKey = PersonaAvatarId;
                _presence.Buttons = DiscordLauncherPresense.ButtonsList.ToArray();

                if (DiscordLauncherPresense.Running()) DiscordLauncherPresense.Client.SetPresence(_presence);
            }

            if (uri == "/matchmaking/leavelobby" || uri == "/matchmaking/declineinvite")
            {
                /* Display Current Car in Freeroam */
                _presence.Details = "Driving " + PersonaCarName;
                _presence.State = LauncherRPC;
                _presence.Assets = new Assets
                {
                    LargeImageText = PersonaName + " - Level: " + PersonaLevel,
                    LargeImageKey = PersonaAvatarId,
                    SmallImageText = "In-Freeroam",
                    SmallImageKey = "gamemode_freeroam"
                };
                _presence.Buttons = DiscordLauncherPresense.ButtonsList.ToArray();

                if (DiscordLauncherPresense.Running()) DiscordLauncherPresense.Client.SetPresence(_presence);

                if (uri == "/matchmaking/leavelobby")
                {
                    AntiCheat.DisableChecks(false);
                }

                eventTerminatedManually = true;
                FunctionStatus.CanCloseGame = true;
            }
            /* IN LOBBY */
            else if (uri == "/matchmaking/acceptinvite")
            {
                /* Accept (Group/Search) Event Invite */
                FunctionStatus.CanCloseGame = false;

                SBRW_XML.LoadXml(serverreply);
                var eventIdNode = SBRW_XML.SelectSingleNode("LobbyInfo/EventId");

                if (eventIdNode != null)
                {
                    EventID = Convert.ToInt32(eventIdNode.InnerText);

                    _presence.Details = "In Lobby: " + EventsList.GetEventName(EventID);
                    _presence.State = serverName;
                    _presence.Assets = new Assets
                    {
                        LargeImageText = PersonaName + " - Level: " + PersonaLevel,
                        LargeImageKey = PersonaAvatarId,
                        SmallImageText = LauncherRPC,
                        SmallImageKey = EventsList.GetEventType(Convert.ToInt32(EventID))
                    };
                    _presence.Buttons = DiscordLauncherPresense.ButtonsList.ToArray();

                    if (DiscordLauncherPresense.Running()) DiscordLauncherPresense.Client.SetPresence(_presence);

                    eventTerminatedManually = false;
                }
            }
            else if (uri == "/matchmaking/joinqueueracenow")
            {
                /* Searching for Events */
                _presence.Details = "Searching for Event";
                _presence.State = LauncherRPC;
                _presence.Assets = new Assets
                {
                    LargeImageText = PersonaName + " - Level: " + PersonaLevel,
                    LargeImageKey = PersonaAvatarId,
                    SmallImageText = "In-Freeroam",
                    SmallImageKey = "gamemode_freeroam"
                };
                _presence.Buttons = DiscordLauncherPresense.ButtonsList.ToArray();

                if (DiscordLauncherPresense.Running()) DiscordLauncherPresense.Client.SetPresence(_presence);

                eventTerminatedManually = true;
            }

            /* IN EVENT */
            if (Regex.Match(uri, "/matchmaking/launchevent").Success)
            {
                /* Singleplayer Event (Launch) */
                FunctionStatus.CanCloseGame = false;

                EventID = Convert.ToInt32(splitted_uri[3]);

                _presence.Details = "Loading Event: " + EventsList.GetEventName(EventID);
                _presence.State = serverName;
                _presence.Assets = new Assets
                {
                    LargeImageText = PersonaName + " - Level: " + PersonaLevel,
                    LargeImageKey = PersonaAvatarId,
                    SmallImageText = LauncherRPC,
                    SmallImageKey = EventsList.GetEventType(EventID)
                };
                _presence.Buttons = DiscordLauncherPresense.ButtonsList.ToArray();

                if (DiscordLauncherPresense.Running()) DiscordLauncherPresense.Client.SetPresence(_presence);

                eventTerminatedManually = false;
            }
            else if (uri == "/event/launched" && eventTerminatedManually == false)
            {
                /* Once the Race Starts */
                _presence.Details = "In Event: " + EventsList.GetEventName(EventID);
                _presence.State = serverName;
                _presence.Assets = new Assets
                {
                    LargeImageText = PersonaName + " - Level: " + PersonaLevel,
                    LargeImageKey = PersonaAvatarId,
                    SmallImageText = LauncherRPC,
                    SmallImageKey = EventsList.GetEventType(EventID)
                };
                _presence.Buttons = DiscordLauncherPresense.ButtonsList.ToArray();

                AntiCheat.event_id = EventID;
                AntiCheat.EnableChecks();

                if (DiscordLauncherPresense.Running()) DiscordLauncherPresense.Client.SetPresence(_presence);
            }
            else if (uri == "/event/arbitration")
            {
                /* Once the Race Finishes */
                _presence.Details = "Finished Event: " + EventsList.GetEventName(EventID);
                _presence.State = serverName;
                _presence.Assets = new Assets
                {
                    LargeImageText = PersonaName + " - Level: " + PersonaLevel,
                    LargeImageKey = PersonaAvatarId,
                    SmallImageText = LauncherRPC,
                    SmallImageKey = EventsList.GetEventType(EventID)
                };
                _presence.Buttons = DiscordLauncherPresense.ButtonsList.ToArray();

                AntiCheat.DisableChecks(true);
                if (DiscordLauncherPresense.Running()) DiscordLauncherPresense.Client.SetPresence(_presence);

                eventTerminatedManually = false;
            }

            /* CARS RELATED */
            foreach (var single_personaId in PersonaIds)
            {
                if (Regex.Match(uri, "/personas/" + single_personaId + "/carslots", RegexOptions.IgnoreCase).Success)
                {
                    carslotsXML = serverreply;

                    SBRW_XML.LoadXml(carslotsXML);

                    int DefaultID = Convert.ToInt32(SBRW_XML.SelectSingleNode("CarSlotInfoTrans/DefaultOwnedCarIndex").InnerText);
                    int current = 0;

                    XmlNode CarsOwnedByPersona = SBRW_XML.SelectSingleNode("CarSlotInfoTrans/CarsOwnedByPersona");
                    XmlNodeList OwnedCarTrans = CarsOwnedByPersona.SelectNodes("OwnedCarTrans");

                    foreach (XmlNode node in OwnedCarTrans)
                    {
                        if (DefaultID == current)
                        {
                            PersonaCarName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes
                                (CarsList.GetCarName(node.SelectSingleNode("CustomCar/Name").InnerText)));
                        }
                        current++;
                    }
                }
                if (Regex.Match(uri, "/personas/" + single_personaId + "/defaultcar", RegexOptions.IgnoreCase).Success)
                {
                    if (splitted_uri.Last() != "defaultcar")
                    {
                        string receivedId = splitted_uri.Last();

                        SBRW_XML.LoadXml(carslotsXML);
                        XmlNode CarsOwnedByPersona = SBRW_XML.SelectSingleNode("CarSlotInfoTrans/CarsOwnedByPersona");
                        XmlNodeList OwnedCarTrans = CarsOwnedByPersona.SelectNodes("OwnedCarTrans");

                        foreach (XmlNode node in OwnedCarTrans)
                        {
                            if (receivedId == node.SelectSingleNode("Id").InnerText)
                            {
                                PersonaCarName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(
                                    CarsList.GetCarName(node.SelectSingleNode("CustomCar/Name").InnerText)));
                            }
                        }
                    }
                }
            }

            //Extending Safehouse
            if (uri.Contains("catalog") && inSafeHouse)
            {                   
                if (GET.Contains("categoryName=NFSW_NA_EP_VINYLS_Category"))    _presence.Details = "In Safehouse - Applying Vinyls";
                if (GET.Contains("clientProductType=PAINTS_BODY"))              _presence.Details = "In Safehouse - Applying Colors";
                if (GET.Contains("clientProductType=PERFORMANCEPART"))          _presence.Details = "In Safehouse - Applying Performance Parts";
                if (GET.Contains("clientProductType=VISUALPART"))               _presence.Details = "In Safehouse - Applying Visual Parts";
                if (GET.Contains("clientProductType=SKILLMODPART"))             _presence.Details = "In Safehouse - Applying Skillmods";
                if (GET.Contains("clientProductType=PRESETCAR"))                _presence.Details = "In Safehouse - Purchasing Car";
                if (GET.Contains("categoryName=BoosterPacks"))                  _presence.Details = "In Safehouse - Opening Cardpacks";

                _presence.Assets = new Assets();
                _presence.Assets.SmallImageText = "In-Safehouse";
                _presence.Assets.SmallImageKey = "gamemode_safehouse";
                _presence.State = serverName;
                _presence.Assets.LargeImageText = PersonaName + " - Level: " + PersonaLevel;
                _presence.Assets.LargeImageKey = PersonaAvatarId;
                _presence.Buttons = DiscordLauncherPresense.ButtonsList.ToArray();

                if (DiscordLauncherPresense.Running()) DiscordLauncherPresense.Client.SetPresence(_presence);
            }
        }
    }
}