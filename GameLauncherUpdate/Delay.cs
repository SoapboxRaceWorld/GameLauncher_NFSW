using System;
using System.Windows.Forms;

namespace GameLauncherUpdater
{
    class Delay {
        public static void WaitSeconds(int sec) {
            if (sec < 1) return;
            DateTime _desired = DateTime.Now.AddSeconds(sec);
            while (DateTime.Now < _desired) {
                Application.DoEvents();
            }
        }

        public static void WaitMSeconds(int sec) {
            if (sec < 1) return;
            DateTime _desired = DateTime.Now.AddMilliseconds(sec);
            while (DateTime.Now < _desired) {
                Application.DoEvents();
            }
        }
    }
}
