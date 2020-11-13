using Newtonsoft.Json;
using System;
using GameLauncher.App.Classes.Logger;

namespace GameLauncher.App.Classes.RPC
{
    class EventsList {
        public static String remoteEventsList = String.Empty;

        public static string GetEventName(int id) {
            // Let's load the "From Server" version first
            if (remoteEventsList != String.Empty) {
                dynamic dynJson = JsonConvert.DeserializeObject(remoteEventsList);

                foreach (var item in dynJson) {
                    if (item.id == id) {
                        return item.trackname;
                    }
                }
            }

            // If we don't have a Server version, load "default" version
            if (remoteEventsList == String.Empty) {
                dynamic dynJson = JsonConvert.DeserializeObject(ExtractResource.AsString("GameLauncher.App.Classes.RPC.JSON.events.json"));

                foreach (var item in dynJson) {
                    if (item.id == id) {
                        return item.trackname;
                    }
                }
            }

            // And if it's not found, do this instead
            return "EVENT:"+id;
        }

        public static string GetEventType(int id) {
            // Let's load the "From Server" version first
            if (remoteEventsList != String.Empty) {
                dynamic dynJson = JsonConvert.DeserializeObject(remoteEventsList);

                foreach (var item in dynJson) {
                    if (item.id == id) {
                        return item.type;
                    }
                }
            }

            // If we don't have a Server version, load "default" version
            if (remoteEventsList != String.Empty) {
                dynamic dynJson = JsonConvert.DeserializeObject(ExtractResource.AsString("GameLauncher.App.Classes.RPC.JSON.events.json"));

                foreach (var item in dynJson) {
                    if (item.id == id) {
                        return item.type;
                    }
                }
            }

            // And if it's not found, do this instead
            return "gamemode_freeroam";
        }
    }
}
