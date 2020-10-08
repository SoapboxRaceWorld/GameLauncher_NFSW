using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GameLauncher.App.Classes {
    public class Konami {
        List<Keys> Keys = new List<Keys>{System.Windows.Forms.Keys.M, System.Windows.Forms.Keys.A,
                                       System.Windows.Forms.Keys.D, System.Windows.Forms.Keys.H,
                                       System.Windows.Forms.Keys.O, System.Windows.Forms.Keys.U,
                                       System.Windows.Forms.Keys.S, System.Windows.Forms.Keys.E};
        private int mPosition = -1;

        public int Position
        {
            get { return mPosition; }
            private set { mPosition = value; }
        }

        public bool IsCompletedBy(Keys key)
        {

            if (Keys[Position + 1] == key)
            {
                // move to next
                Position++;
            }
            else if (Position == 1 && key == System.Windows.Forms.Keys.Up)
            {
                // stay where we are
            }
            else if (Keys[0] == key)
            {
                // restart at 1st
                Position = 0;
            }
            else
            {
                // no match in sequence
                Position = -1;
            }

            if (Position == Keys.Count - 1)
            {
                Position = -1;
                return true;
            }

            return false;
        }
    }
}