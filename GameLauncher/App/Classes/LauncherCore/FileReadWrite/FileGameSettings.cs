using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    class FileGameSettings
    {
        XmlDocument userSettingsXml = new XmlDocument();

        String file = Path.Combine(Environment.GetEnvironmentVariable("AppData"), "Need for Speed World", "Settings", "UserSettings.xml");

        XmlNode check = null;

        public FileGameSettings()
        {
            if (File.Exists(file))
            {
                try
                {
                    userSettingsXml.Load(file);
                    check = userSettingsXml.SelectSingleNode("Settings");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    this.UseDefault();
                    userSettingsXml.Load(file);
                    check = userSettingsXml.SelectSingleNode("Settings");
                }
            }
            else
            {
                this.UseDefault();
                userSettingsXml.Load(file);
                check = userSettingsXml.SelectSingleNode("Settings");
            }

            /*  */

            try
            {
                if (File.Exists(file))
                {
                    try
                    {
                        userSettingsXml.Load(file);
                        var language = userSettingsXml.SelectSingleNode("Settings/UI/Language");
                        language.InnerText = SettingsLanguage.SelectedValue.ToString();
                    }
                    catch
                    {
                        File.Delete(file);

                        var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
                        var ui = setting.AppendChild(userSettingsXml.CreateElement("UI"));

                        var persistentValue = setting.AppendChild(userSettingsXml.CreateElement("PersistentValue"));
                        var chat = persistentValue.AppendChild(userSettingsXml.CreateElement("Chat"));
                        chat.InnerXml = "<DefaultChatGroup Type=\"string\">" + Self.currentLanguage + "</DefaultChatGroup>";
                        ui.InnerXml = "<Language Type=\"string\">" + SettingsLanguage.SelectedValue + "</Language>";

                        var directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(file));
                    }
                }
                else
                {
                    try
                    {
                        var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
                        var ui = setting.AppendChild(userSettingsXml.CreateElement("UI"));

                        var persistentValue = setting.AppendChild(userSettingsXml.CreateElement("PersistentValue"));
                        var chat = persistentValue.AppendChild(userSettingsXml.CreateElement("Chat"));
                        chat.InnerXml = "<DefaultChatGroup Type=\"string\">" + Self.currentLanguage + "</DefaultChatGroup>";
                        ui.InnerXml = "<Language Type=\"string\">" + SettingsLanguage.SelectedValue + "</Language>";

                        var directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(file));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(null, "There was an error saving your settings to actual file. Restoring default.\n" + ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, "There was an error saving your settings to actual file. Restoring default.\n" + ex.Message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.Delete(file);
            }
        }

        public string AudioMode
        {
            get { return this.getNode("Settings/VideoConfig/audiomode"); }
            set { this.setNode("Settings/VideoConfig/audiomode", value, "int"); }
        }
        public string AudioQuality
        {
            get { return this.getNode("Settings/VideoConfig/audioquality"); }
            set { this.setNode("Settings/VideoConfig/audioquality", value, "int"); }
        }

        public string Brightness
        {
            get { return this.getNode("Settings/VideoConfig/brightness"); }
            set { this.setNode("Settings/VideoConfig/brightness", value, "int"); }
        }

        public string Enableaero
        {
            get { return this.getNode("Settings/VideoConfig/enableaero"); }
            set { this.setNode("Settings/VideoConfig/enableaero", value, "int"); }
        }

        public string FirstTime
        {
            get { return this.getNode("Settings/VideoConfig/firsttime"); }
            set { this.setNode("Settings/VideoConfig/firsttime", value, "int"); }
        }

        public string Forcesm1x
        {
            get { return this.getNode("Settings/VideoConfig/forcesm1x"); }
            set { this.setNode("Settings/VideoConfig/forcesm1x", value, "bool"); }
        }

        public string PerformanceLevel
        {
            get { return this.getNode("Settings/VideoConfig/performancelevel"); }
            set { this.setNode("Settings/VideoConfig/performancelevel", value, "int"); }
        }

        public string PixelAspectRatioOverride
        {
            get { return this.getNode("Settings/VideoConfig/pixelaspectratiooverride"); }
            set { this.setNode("Settings/VideoConfig/pixelaspectratiooverride", value, "int"); }
        }

        public string ScreenHeight
        {
            get { return this.getNode("Settings/VideoConfig/screenheight"); }
            set { this.setNode("Settings/VideoConfig/screenheight", value, "int"); }
        }

        public string ScreenLeft
        {
            get { return this.getNode("Settings/VideoConfig/screenleft"); }
            set { this.setNode("Settings/VideoConfig/screenleft", value, "int"); }
        }

        public string ScreenRefresh
        {
            get { return this.getNode("Settings/VideoConfig/screenrefresh"); }
            set { this.setNode("Settings/VideoConfig/screenrefresh", value, "int"); }
        }

        public string screentop
        {
            get { return this.getNode("Settings/VideoConfig/screentop"); }
            set { this.setNode("Settings/VideoConfig/screentop", value, "int"); }
        }

        public string ScreenWidth
        {
            get { return this.getNode("Settings/VideoConfig/screenwidth"); }
            set { this.setNode("Settings/VideoConfig/screenwidth", value, "int"); }
        }

        public string ScreenWindowed
        {
            get { return this.getNode("Settings/VideoConfig/screenwindowed"); }
            set { this.setNode("Settings/VideoConfig/screenwindowed", value, "int"); }
        }

        public string Size
        {
            get { return this.getNode("Settings/VideoConfig/size"); }
            set { this.setNode("Settings/VideoConfig/size", value, "int"); }
        }

        public string Version
        {
            get { return this.getNode("Settings/VideoConfig/version"); }
            set { this.setNode("Settings/VideoConfig/version", value, "int"); }
        }

        public string VSyncON
        {
            get { return this.getNode("Settings/VideoConfig/vsyncon"); }
            set { this.setNode("Settings/VideoConfig/vsyncon", value, "int"); }
        }

        private void UseDefault()
        {
            Console.WriteLine("Failed to parse {0}, deleting.", file);
            File.Delete(file);

            var setting = userSettingsXml.AppendChild(userSettingsXml.CreateElement("Settings"));
            userSettingsXml.Save(file);
        }

        public string getNode(string xpath)
        {
            Console.WriteLine("Getting {0} value.", xpath);
            XmlNode node = userSettingsXml.SelectSingleNode(xpath);
            return node.InnerText;
        }

        private void setNode(string xpath, string value, string typeOf)
        {
            Console.WriteLine("Setting {0} value to {1} as {2}.", xpath, value, typeOf);

            XmlElement contentElement = (XmlElement)this.makeXPath(userSettingsXml, xpath);
            contentElement.SetAttribute("Type", typeOf);
            contentElement.InnerText = value;
            userSettingsXml.Save(file);
        }

        private XmlNode makeXPath(XmlDocument doc, string xpath)
        {
            return makeXPath(doc, doc as XmlNode, xpath);
        }

        private XmlNode makeXPath(XmlDocument doc, XmlNode parent, string xpath)
        {
            string[] partsOfXPath = xpath.Trim('/').Split('/');
            string nextNodeInXPath = partsOfXPath.First();
            if (string.IsNullOrEmpty(nextNodeInXPath))
                return parent;

            XmlNode node = parent.SelectSingleNode(nextNodeInXPath);
            if (node == null)
                node = parent.AppendChild(doc.CreateElement(nextNodeInXPath));

            string rest = String.Join("/", partsOfXPath.Skip(1).ToArray());
            return makeXPath(doc, node, rest);
        }
    }
}
