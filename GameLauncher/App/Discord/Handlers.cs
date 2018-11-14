using GameLauncher.App.Classes.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameLauncher.App.Discord {
    class Handlers {
        public Handlers()  {

        }

        public static void ErrorCallback(int errorcode, string errormsg) {
            MessageBox.Show("Error " + errorcode + ": " + errormsg);
        }

        public static void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser) {
            Log.Debug(string.Format("Discord: connected to {0}#{1}: {2}", connectedUser.username, connectedUser.discriminator, connectedUser.userId));
        }
    }
}
