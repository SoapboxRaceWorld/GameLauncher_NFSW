using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.Events
{
    public partial class About : Form {
        IniFile SettingFile = new IniFile("Settings.ini");

        public About() {
            InitializeComponent();
        }

        internal static void showAbout(object sender, EventArgs e) {
            //About frm = new About();
            //frm.Show();
            MessageBox.Show("About page is not yet ready");
        }

        private void About_Load(object sender, EventArgs e) {
            closeAbout.Click += new EventHandler(closeAbout_Click);

            //Replace cursor
            if (File.Exists(SettingFile.Read("InstallationDirectory") + "\\Media\\Cursors\\default.cur")) {
                Cursor mycursor = new Cursor(Cursor.Current.Handle);
                IntPtr colorcursorhandle = User32.LoadCursorFromFile(SettingFile.Read("InstallationDirectory") + "\\Media\\Cursors\\default.cur");
                mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, null, mycursor, new object[] { colorcursorhandle });
                this.Cursor = mycursor;
            }
        }

        private void closeAbout_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
