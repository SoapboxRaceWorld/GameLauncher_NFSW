using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace GameLauncher.App.Classes.RPC {
    class DiscordRPC {
        public static long RPCstartTimestamp = Self.getTimestamp(true);
        public static DiscordRpc.RichPresence _presence = new DiscordRpc.RichPresence();
        public static DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();

        //Some checks
        private static string serverName = ServerProxy.Instance.GetServerName();
        private static bool canUpdateProfileField = false;
        private static bool inSafehouse = false;

        public DiscordRPC() {
            DiscordRpc.Initialize("427355155537723393", ref handlers, true, "");
            Console.WriteLine("INITIALIZED!");
        }

        //Some data related, can be touched.
        public static string PersonaName = String.Empty;
        public static string PersonaLevel = String.Empty;
        public static string PersonaAvatarId = String.Empty;

        public static void handleGameState(string uri, string serverreply = "", string POST = "") {
            var SBRW_XML = new XmlDocument();      

            Console.WriteLine("------------------------------------");
            Console.WriteLine("POST: " + POST);
            Console.WriteLine("serverreply: " + serverreply);

            if (uri == "/User/SecureLoginPersona") {
                canUpdateProfileField = true;
            }

            if ((uri == "/DriverPersona/GetPersonaInfo" && canUpdateProfileField == true)) {
                SBRW_XML.LoadXml(serverreply);
                PersonaName = SBRW_XML.SelectSingleNode("ProfileData/Name").InnerText.Replace("¤", "[S]");
                PersonaLevel = SBRW_XML.SelectSingleNode("ProfileData/Level").InnerText;
                PersonaAvatarId = (SBRW_XML.SelectSingleNode("ProfileData/IconIndex").InnerText == "26") ? "nfsw" : "avatar_" + SBRW_XML.SelectSingleNode("ProfileData/IconIndex").InnerText;

                _presence.details = (inSafehouse == true) ? "In-Safehouse as " + PersonaName : "Freeroaming as " + PersonaName;
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

            if (uri == "/matchmaking/leavelobby") {
                _presence.details = "Freeroaming as " + PersonaName;
                _presence.state = serverName;
                _presence.largeImageText = PersonaName + " - Level: " + PersonaLevel;
                _presence.largeImageKey = PersonaAvatarId;
                _presence.smallImageText = "In-Freeroam";
                _presence.smallImageKey = "gamemode_freeroam";
                _presence.startTimestamp = RPCstartTimestamp;
                _presence.instance = true;
                DiscordRpc.UpdatePresence(_presence);
            }

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

            if (Regex.Match(uri, "/matchmaking/launchevent").Success) {
                string[] splitted_uri = uri.Split('/');
                var EventID = splitted_uri[3];

                _presence.details = "In Event: " + EventList.getEventName(Convert.ToInt32(EventID));
                _presence.state = serverName;
                _presence.largeImageText = PersonaName + " - Level: " + PersonaLevel;
                _presence.largeImageKey = PersonaAvatarId;
                _presence.smallImageText = EventList.getEventName(Convert.ToInt32(EventID));
                _presence.smallImageKey = EventList.getEventType(Convert.ToInt32(EventID));
                _presence.startTimestamp = RPCstartTimestamp;
                _presence.instance = true;
                DiscordRpc.UpdatePresence(_presence);
            }
        }
    }
}
