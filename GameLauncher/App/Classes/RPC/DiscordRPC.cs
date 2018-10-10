using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GameLauncher.App.Classes.RPC {
    class DiscordRPC {
        public static long RPCstartTimestamp = 0000000000;
        public static DiscordRpc.RichPresence _presence = new DiscordRpc.RichPresence();
        public static DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();

        //Some checks
        private static string serverName = ServerProxy.Instance.GetServerName();
        private static bool canUpdateProfileField = false;
        private static bool inSafehouse = false;
        private static int EventID;
        private static string carslotsXML = String.Empty;

        public DiscordRPC() {
            DiscordRpc.Initialize("427355155537723393", ref handlers, true, "");
            Console.WriteLine("INITIALIZED!");
        }

        //Some data related, can be touched.
        public static string PersonaId = String.Empty;
        public static string PersonaName = String.Empty;
        public static string PersonaLevel = String.Empty;
        public static string PersonaAvatarId = String.Empty;
        public static string PersonaCarId = String.Empty;
        public static string PersonaCarName = String.Empty;
        public static List<string> PersonaIds = new List<string>();

        public static void handleGameState(string uri, string serverreply = "", string POST = "", string GET = "") {
            var SBRW_XML = new XmlDocument();      

            Console.WriteLine("POST: " + POST);
            Console.WriteLine("GET: " + GET);
            Console.WriteLine("serverreply: " + serverreply);
            Console.WriteLine("------------------------------------");

            if (uri == "/User/SecureLoginPersona") {
                canUpdateProfileField = true;
            }

            //FIRST PERSONA EVER LOCALIZED IN CODE
            if (uri == "/User/GetPermanentSession") {
                SBRW_XML.LoadXml(serverreply);

                PersonaName = SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/Name").InnerText.Replace("¤", "[S]");
                PersonaLevel = SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/Level").InnerText;
                PersonaAvatarId = (SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/IconIndex").InnerText == "26") ? "nfsw" : "avatar_" + SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/IconIndex").InnerText;
                PersonaId = SBRW_XML.SelectSingleNode("UserInfo/personas/ProfileData/PersonaId").InnerText;

                //Let's get rest of PERSONAIDs
                XmlNode UserInfo = SBRW_XML.SelectSingleNode("UserInfo");
                XmlNodeList personas = UserInfo.SelectNodes("personas/ProfileData");
                foreach (XmlNode node in personas) {
                    PersonaIds.Add(node.SelectSingleNode("PersonaId").InnerText);
                }
            }

            //DRIVING CARNAME
            if (uri == "/DriverPersona/GetPersonaInfo" && canUpdateProfileField == true) {
                SBRW_XML.LoadXml(serverreply);
                PersonaName = SBRW_XML.SelectSingleNode("ProfileData/Name").InnerText.Replace("¤", "[S]");
                PersonaLevel = SBRW_XML.SelectSingleNode("ProfileData/Level").InnerText;
                PersonaAvatarId = (SBRW_XML.SelectSingleNode("ProfileData/IconIndex").InnerText == "26") ? "nfsw" : "avatar_" + SBRW_XML.SelectSingleNode("ProfileData/IconIndex").InnerText;
                PersonaId = SBRW_XML.SelectSingleNode("ProfileData/PersonaId").InnerText;

                Console.WriteLine(PersonaId);

                _presence.details = "Driving " + PersonaCarId;
                _presence.state = serverName;
                _presence.largeImageText = PersonaName + " - Level: " + PersonaLevel;
                _presence.largeImageKey = PersonaAvatarId;
                _presence.smallImageText = (inSafehouse == true) ? "Safehouse" : "In-Freeroam";
                _presence.smallImageKey = "gamemode_freeroam";
                _presence.startTimestamp = RPCstartTimestamp;
                _presence.instance = true;
                DiscordRpc.UpdatePresence(_presence);

                canUpdateProfileField = false;
            }
            if (uri == "/matchmaking/leavelobby" || uri == "/event/arbitration" || uri == "/DriverPersona/UpdatePersonaPresence") {
                _presence.details = "Driving " + PersonaCarId;
                _presence.state = serverName;
                _presence.largeImageText = PersonaName + " - Level: " + PersonaLevel;
                _presence.largeImageKey = PersonaAvatarId;
                _presence.smallImageText = "In-Freeroam";
                _presence.smallImageKey = "gamemode_freeroam";
                _presence.startTimestamp = RPCstartTimestamp;
                _presence.instance = true;
                DiscordRpc.UpdatePresence(_presence);
            }

            //IN LOBBY
            if (uri == "/matchmaking/acceptinvite") {
                SBRW_XML.LoadXml(serverreply);
                var EventID = SBRW_XML.SelectSingleNode("LobbyInfo/EventId").InnerText;

                _presence.details = "In Lobby: " + EventList.getEventName(Convert.ToInt32(EventID));
                _presence.state = serverName;
                _presence.largeImageText = PersonaName + " - Level: " + PersonaLevel;
                _presence.largeImageKey = PersonaAvatarId;
                _presence.smallImageText = EventList.getEventName(Convert.ToInt32(EventID));
                _presence.smallImageKey = EventList.getEventType(Convert.ToInt32(EventID));
                _presence.startTimestamp = RPCstartTimestamp;
                _presence.instance = true;
                DiscordRpc.UpdatePresence(_presence);
            }

            //IN EVENT
            if (Regex.Match(uri, "/matchmaking/launchevent").Success) {
                string[] splitted_uri = uri.Split('/');
                EventID = Convert.ToInt32(splitted_uri[3]);

                _presence.details = "In Event: " + EventList.getEventName(EventID);
                _presence.state = serverName;
                _presence.largeImageText = PersonaName + " - Level: " + PersonaLevel;
                _presence.largeImageKey = PersonaAvatarId;
                _presence.smallImageText = EventList.getEventName(EventID);
                _presence.smallImageKey = EventList.getEventType(EventID);
                _presence.startTimestamp = RPCstartTimestamp;
                _presence.instance = true;
                DiscordRpc.UpdatePresence(_presence);
            }
            if (uri == "/event/launched") {
                _presence.details = "In Event: " + EventList.getEventName(EventID);
                _presence.state = serverName;
                _presence.largeImageText = PersonaName + " - Level: " + PersonaLevel;
                _presence.largeImageKey = PersonaAvatarId;
                _presence.smallImageText = EventList.getEventName(EventID);
                _presence.smallImageKey = EventList.getEventType(EventID);
                _presence.startTimestamp = Self.getTimestamp(true);
                _presence.instance = true;
                DiscordRpc.UpdatePresence(_presence);
            }

            //CARS RELATED
            if (uri == "/personas/"+PersonaId+"/carslots") {
                carslotsXML = serverreply;
            }
            if (Regex.Match(uri, "/personas/" +PersonaId+"/defaultcar").Success) {
                string[] splitted_uri = uri.Split('/');
                PersonaCarId = splitted_uri[4];
            }

        }
    }
}