using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.RPC {
    class EventList {
        public static String remoteEvent = String.Empty;

        public static string GetEventName(int id) {
            // Let's load the "Cached From Server" version first
            if (remoteEvent != String.Empty) {
                dynamic dynJson = JsonConvert.DeserializeObject(remoteEvent);

                foreach (var item in dynJson) {
                    if (item.id == id) {
                        return item.trackname;
                    }
                }
            }

            // If we don't have a Server version, load "default" version
            if (remoteEvent == String.Empty) {
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
            // Let's load the "Cached From Server" version first
            if (remoteEvent != String.Empty) {
                dynamic dynJson = JsonConvert.DeserializeObject(remoteEvent);

                foreach (var item in dynJson) {
                    if (item.id == id) {
                        return item.type;
                    }
                }
            }

            // If we don't have a Server version, load "default" version
            if (remoteEvent != String.Empty) {
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
