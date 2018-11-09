using GameLauncher.App.Classes.Logger;
using System;
using System.IO;
using System.Reflection;

namespace GameLauncher.App.Classes {
    class ExtractResource {
        public static byte[] AsByte(String filename) {
            Log.Debug("Extracting " + filename + " as Byte");

            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream(filename)) {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        public static String AsString(String filename) {
            Log.Debug("Extracting " + filename + " as String");

            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(filename))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
