using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.RPC {
    class EventList {
        public static string getEventName(int id) {
            dynamic dynJson = JsonConvert.DeserializeObject(ExtractResource.AsString("GameLauncher.App.Classes.RPC.JSON.events.json"));

            foreach (var item in dynJson) {
                if (item.id == id) {
                    return item.trackname;
                }
            }

            return "EVENT:"+id;
        }

        public static string getEventType(int id) {
            dynamic dynJson = JsonConvert.DeserializeObject(ExtractResource.AsString("GameLauncher.App.Classes.RPC.JSON.events.json"));

            foreach (var item in dynJson) {
                if (item.id == id) {
                    return item.type;
                }
            }

            return "gamemode_freeroam";
        }
    }
}
