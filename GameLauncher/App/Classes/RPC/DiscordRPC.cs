using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GameLauncher.App.Classes.RPC {
    class DiscordRPC {
        public static long RPCstartTimestamp = Self.getTimestamp(true);
        public static DiscordRpc.RichPresence _presence = new DiscordRpc.RichPresence();
        public static DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();

        public DiscordRPC() {
            DiscordRpc.Initialize("427355155537723393", ref handlers, true, "");
            Console.WriteLine("INITIALIZED!");
        }

        //Some data related, can be touched.
        public static string PersonaName = String.Empty;
        public static string PersonaLevel = String.Empty;
        public static string PersonaAvatarId = String.Empty;

        public static void sendInFreeroamRPC(string state, string largeImageText, string largeImageKey, string smallImageText, string smallImageKey) {
            _presence.state = state;
            _presence.largeImageText = largeImageText;
            _presence.largeImageKey = largeImageKey;
            _presence.smallImageText = smallImageText;
            _presence.smallImageKey = smallImageKey;
            _presence.startTimestamp = RPCstartTimestamp;
            _presence.instance = true;

            DiscordRpc.UpdatePresence(_presence);
        }

        public static void handleGameState(string uri, string serverreply = "", string POST = "") {
            var SBRW_XML = new XmlDocument();

            if (uri == "/DriverPersona/GetPersonaInfo") {
                SBRW_XML.LoadXml(serverreply);
                PersonaName = SBRW_XML.SelectSingleNode("ProfileData/Name").InnerText;
                PersonaLevel = SBRW_XML.SelectSingleNode("ProfileData/Level").InnerText;
                PersonaAvatarId = (SBRW_XML.SelectSingleNode("ProfileData/IconIndex").InnerText == "26") ? "nfsw" : "avatar_" + SBRW_XML.SelectSingleNode("ProfileData/IconIndex").InnerText;

                sendInFreeroamRPC($"Freeroaming as {PersonaName}", PersonaName + " - Level: " + PersonaLevel, PersonaAvatarId, "In-Freeroam", "gamemode_freeroam");
            }
        }

        public static string eventName(int eventId) {
            return String.Empty;
        }
    }
}
