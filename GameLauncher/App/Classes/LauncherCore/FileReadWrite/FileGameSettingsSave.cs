using GameLauncher.App.Classes.LauncherCore.Downloader;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Support;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    class FileGameSettingsData
    {
        /* Language */
        public static string Language = "EN";
        /* Audio */
        public static string AudioMode = "1";
        public static string MasterAudio = "100";
        public static string SFXAudio = "52";
        public static string CarAudio = "52";
        public static string SpeechAudio = "52";
        public static string MusicAudio = "52";
        public static string FreeroamAudio = "52";
        /* Gameplay */
        public static string Camera = "2";
        public static string Transmission = "2";
        public static string Damage = "1";
        public static string Moments = "1";
        public static string SpeedUnits = "1";
        /* Physics */
        public static string CameraPOV = "2";
        public static string TransmissionType = "1";
        /* VideoConfig */
        public static string AudioM = "1";
        public static string AudioQuality = "0";
        public static string Brightness = "0";
        public static string EnableAero = "0";
        public static string FirstTime = "0";
        public static string ForcesM1x = "false";
        public static string PerformanceLevel = "2";
        public static string PixelAspectRatioOverride = "2";
        public static string ScreenHeight = "600";
        public static string ScreenWidth = "800";
        public static string ScreenWindowed = "0";
        public static string VSyncOn = "0";
        /* VideoConfig Addons */
        public static string BaseTextureFilter = "0";
        public static string BaseTextureLODBias = "0";
        public static string BaseTextureMaxAni = "0";
        public static string CarEnvironmentMapEnable = "0";
        public static string CarLODLevel = "0";
        public static string FSAALevel = "0";
        public static string GlobalDetailLevel = "0";
        public static string MaxSkidMarks = "0";
        public static string MotionBlurEnable = "0";
        public static string OverBrightEnable = "0";
        public static string ParticleSystemEnable = "0";
        public static string PostProcessingEnable = "0";
        public static string RainEnable = "0";
        public static string RoadReflectionEnable = "0";
        public static string RoadTextureFilter = "0";
        public static string RoadTextureLODBias = "0";
        public static string RoadTextureMaxAni = "0";
        public static string ShaderDetail = "0";
        public static string ShadowDetail = "0";
        public static string Size = "0";
        public static string VisualTreatment = "0";
        public static string WaterSimEnable = "0";
    }

    class FileGameSettings
    {
        public static XmlDocument UserSettingsFile = new XmlDocument();

        public static void Read(string FileReadStatus)
        {
            if (File.Exists(Locations.UserSettingsXML))
            {
                try
                {
                    UserSettingsFile.Load(Locations.UserSettingsXML);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("USX File", null, Error, null, true);
                }
            }
            else
            {
                Log.Error("USX File: How Could This Happen!? - No UserSettings.xml found!");
                return;
            }

            if (FileReadStatus == "Language Only")
            {
                /* Language */
                FileGameSettingsData.Language = (NodeReader("InnerText", "Settings/UI/Language", null) != "ERROR") ?
                                                 NodeReader("InnerText", "Settings/UI/Language", null) : DownloaderAddons.SpeechFiles(null).ToUpper();
            }
            else if (FileReadStatus == "Full File")
            {
                /* Audio */
                FileGameSettingsData.AudioMode = (NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "AudioMode") != "ERROR") ?
                                                  NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "AudioMode") : "0";
                FileGameSettingsData.SFXAudio = (NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "SFXVol") != "ERROR") ?
                                                 NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "SFXVol") : "100";
                FileGameSettingsData.MasterAudio = (NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "MasterVol") != "ERROR") ?
                                                    NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "MasterVol") : "100";
                FileGameSettingsData.CarAudio = (NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "CarVol") != "ERROR") ?
                                                 NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "CarVol") : "100";
                FileGameSettingsData.SpeechAudio = (NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "SpeechVol") != "ERROR") ?
                                                    NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "SpeechVol") : "100";
                FileGameSettingsData.MusicAudio = (NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "GameMusicVol") != "ERROR") ?
                                                   NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "GameMusicVol") : "100";
                FileGameSettingsData.FreeroamAudio = (NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "FEMusicVol") != "ERROR") ?
                                                      NodeReader("Attributes", "Settings/UI/Audio/AudioOptions", "FEMusicVol") : "100";
                /* Gameplay */
                FileGameSettingsData.Camera = (NodeReader("Attributes", "Settings/UI/Gameplay/GamePlayOptions", "camera") != "ERROR") ?
                                               NodeReader("Attributes", "Settings/UI/Gameplay/GamePlayOptions", "camera") : "2";
                FileGameSettingsData.Transmission = (NodeReader("Attributes", "Settings/UI/Gameplay/GamePlayOptions", "transmission") != "ERROR") ?
                                                     NodeReader("Attributes", "Settings/UI/Gameplay/GamePlayOptions", "transmission") : "2";
                FileGameSettingsData.Damage = (NodeReader("Attributes", "Settings/UI/Gameplay/GamePlayOptions", "damage") != "ERROR") ?
                                               NodeReader("Attributes", "Settings/UI/Gameplay/GamePlayOptions", "damage") : "1";
                FileGameSettingsData.Moments = (NodeReader("Attributes", "Settings/UI/Gameplay/GamePlayOptions", "moments") != "ERROR") ?
                                               NodeReader("Attributes", "Settings/UI/Gameplay/GamePlayOptions", "moments") : "1";
                FileGameSettingsData.SpeedUnits = (NodeReader("Attributes", "Settings/UI/Gameplay/GamePlayOptions", "speedUnits") != "ERROR") ?
                                                   NodeReader("Attributes", "Settings/UI/Gameplay/GamePlayOptions", "speedUnits") : "1";
                /* Physics */
                FileGameSettingsData.CameraPOV = (NodeReader("InnerText", "Settings/Physics/CameraPOV", null) != "ERROR") ?
                                                  NodeReader("InnerText", "Settings/Physics/CameraPOV", null) : "2";
                FileGameSettingsData.TransmissionType = (NodeReader("InnerText", "Settings/Physics/TransmissionType", null) != "ERROR") ?
                                                         NodeReader("InnerText", "Settings/Physics/TransmissionType", null) : "1";
                /* VideoConfig */
                FileGameSettingsData.AudioM = (NodeReader("InnerText", "Settings/VideoConfig/audiomode", null) != "ERROR") ?
                                                     NodeReader("InnerText", "Settings/VideoConfig/audiomode", null) : "0";
                FileGameSettingsData.AudioQuality = (NodeReader("InnerText", "Settings/VideoConfig/audioquality", null) != "ERROR") ?
                                                     NodeReader("InnerText", "Settings/VideoConfig/audioquality", null) : "0";
                FileGameSettingsData.Brightness = (NodeReader("InnerText", "Settings/VideoConfig/brightness", null) != "ERROR") ?
                                                   NodeReader("InnerText", "Settings/VideoConfig/brightness", null) : "100";
                FileGameSettingsData.EnableAero = (NodeReader("InnerText", "Settings/VideoConfig/enableaero", null) != "ERROR") ?
                                                   NodeReader("InnerText", "Settings/VideoConfig/enableaero", null) : "0";
                FileGameSettingsData.FirstTime = (NodeReader("InnerText", "Settings/VideoConfig/firsttime", null) != "ERROR") ?
                                                  NodeReader("InnerText", "Settings/VideoConfig/firsttime", null) : "0";
                FileGameSettingsData.ForcesM1x = (NodeReader("InnerText", "Settings/VideoConfig/forcesm1x", null) != "ERROR") ?
                                                  NodeReader("InnerText", "Settings/VideoConfig/forcesm1x", null) : "False";
                FileGameSettingsData.PixelAspectRatioOverride = (NodeReader("InnerText", "Settings/VideoConfig/pixelaspectratiooverride", null) != "ERROR") ?
                                                                 NodeReader("InnerText", "Settings/VideoConfig/pixelaspectratiooverride", null) : "2";
                FileGameSettingsData.PerformanceLevel = (NodeReader("InnerText", "Settings/VideoConfig/performancelevel", null) != "ERROR") ?
                                                         NodeReader("InnerText", "Settings/VideoConfig/performancelevel", null) : "2";
                FileGameSettingsData.ScreenHeight = (NodeReader("InnerText", "Settings/VideoConfig/screenheight", null) != "ERROR") ?
                                                     NodeReader("InnerText", "Settings/VideoConfig/screenheight", null) : "600";
                FileGameSettingsData.ScreenWidth = (NodeReader("InnerText", "Settings/VideoConfig/screenwidth", null) != "ERROR") ?
                                                    NodeReader("InnerText", "Settings/VideoConfig/screenwidth", null) : "800";
                FileGameSettingsData.ScreenWindowed = (NodeReader("InnerText", "Settings/VideoConfig/screenwindowed", null) != "ERROR") ?
                                                       NodeReader("InnerText", "Settings/VideoConfig/screenwindowed", null) : "0";
                FileGameSettingsData.VSyncOn = (NodeReader("InnerText", "Settings/VideoConfig/vsyncon", null) != "ERROR") ?
                                                NodeReader("InnerText", "Settings/VideoConfig/vsyncon", null) : "0";
                /* VideoConfig Addons */
                FileGameSettingsData.BaseTextureFilter = (NodeReader("InnerText", "Settings/VideoConfig/basetexturefilter", null) != "ERROR") ?
                                                          NodeReader("InnerText", "Settings/VideoConfig/basetexturefilter", null) : "0";
                FileGameSettingsData.BaseTextureLODBias = (NodeReader("InnerText", "Settings/VideoConfig/basetexturelodbias", null) != "ERROR") ?
                                                           NodeReader("InnerText", "Settings/VideoConfig/basetexturelodbias", null) : "0";
                FileGameSettingsData.BaseTextureMaxAni = (NodeReader("InnerText", "Settings/VideoConfig/basetexturemaxani", null) != "ERROR") ?
                                                          NodeReader("InnerText", "Settings/VideoConfig/basetexturemaxani", null) : "0";
                FileGameSettingsData.CarEnvironmentMapEnable = (NodeReader("InnerText", "Settings/VideoConfig/carenvironmentmapenable", null) != "ERROR") ?
                                                                NodeReader("InnerText", "Settings/VideoConfig/carenvironmentmapenable", null) : "0";
                FileGameSettingsData.CarLODLevel = (NodeReader("InnerText", "Settings/VideoConfig/carlodlevel", null) != "ERROR") ?
                                                    NodeReader("InnerText", "Settings/VideoConfig/carlodlevel", null) : "0";
                FileGameSettingsData.FSAALevel = (NodeReader("InnerText", "Settings/VideoConfig/fsaalevel", null) != "ERROR") ?
                                                  NodeReader("InnerText", "Settings/VideoConfig/fsaalevel", null) : "0";
                FileGameSettingsData.GlobalDetailLevel = (NodeReader("InnerText", "Settings/VideoConfig/globaldetaillevel", null) != "ERROR") ?
                                                          NodeReader("InnerText", "Settings/VideoConfig/globaldetaillevel", null) : "0";
                FileGameSettingsData.MaxSkidMarks = (NodeReader("InnerText", "Settings/VideoConfig/maxskidmarks", null) != "ERROR") ?
                                                     NodeReader("InnerText", "Settings/VideoConfig/maxskidmarks", null) : "0";
                FileGameSettingsData.MotionBlurEnable = (NodeReader("InnerText", "Settings/VideoConfig/motionblurenable", null) != "ERROR") ?
                                                         NodeReader("InnerText", "Settings/VideoConfig/motionblurenable", null) : "0";
                FileGameSettingsData.OverBrightEnable = (NodeReader("InnerText", "Settings/VideoConfig/overbrightenable", null) != "ERROR") ?
                                                         NodeReader("InnerText", "Settings/VideoConfig/overbrightenable", null) : "0";
                FileGameSettingsData.ParticleSystemEnable = (NodeReader("InnerText", "Settings/VideoConfig/particlesystemenable", null) != "ERROR") ?
                                                             NodeReader("InnerText", "Settings/VideoConfig/particlesystemenable", null) : "0";
                FileGameSettingsData.PostProcessingEnable = (NodeReader("InnerText", "Settings/VideoConfig/postprocessingenable", null) != "ERROR") ?
                                                             NodeReader("InnerText", "Settings/VideoConfig/postprocessingenable", null) : "0";
                FileGameSettingsData.RainEnable = (NodeReader("InnerText", "Settings/VideoConfig/rainenable", null) != "ERROR") ?
                                                   NodeReader("InnerText", "Settings/VideoConfig/rainenable", null) : "0";
                FileGameSettingsData.RoadReflectionEnable = (NodeReader("InnerText", "Settings/VideoConfig/roadreflectionenable", null) != "ERROR") ?
                                                             NodeReader("InnerText", "Settings/VideoConfig/roadreflectionenable", null) : "0";
                FileGameSettingsData.RoadTextureFilter = (NodeReader("InnerText", "Settings/VideoConfig/roadtexturefilter", null) != "ERROR") ?
                                                          NodeReader("InnerText", "Settings/VideoConfig/roadtexturefilter", null) : "0";
                FileGameSettingsData.RoadTextureLODBias = (NodeReader("InnerText", "Settings/VideoConfig/roadtexturelodbias", null) != "ERROR") ?
                                                           NodeReader("InnerText", "Settings/VideoConfig/roadtexturelodbias", null) : "0";
                FileGameSettingsData.RoadTextureMaxAni = (NodeReader("InnerText", "Settings/VideoConfig/roadtexturemaxani", null) != "ERROR") ?
                                                          NodeReader("InnerText", "Settings/VideoConfig/roadtexturemaxani", null) : "0";
                FileGameSettingsData.ShaderDetail = (NodeReader("InnerText", "Settings/VideoConfig/shaderdetail", null) != "ERROR") ?
                                                     NodeReader("InnerText", "Settings/VideoConfig/shaderdetail", null) : "0";
                FileGameSettingsData.ShadowDetail = (NodeReader("InnerText", "Settings/VideoConfig/shadowdetail", null) != "ERROR") ?
                                                     NodeReader("InnerText", "Settings/VideoConfig/shadowdetail", null) : "0";
                FileGameSettingsData.Size = (NodeReader("InnerText", "Settings/VideoConfig/size", null) != "ERROR") ?
                                                     NodeReader("InnerText", "Settings/VideoConfig/size", null) : "0";
                FileGameSettingsData.VisualTreatment = (NodeReader("InnerText", "Settings/VideoConfig/visualtreatment", null) != "ERROR") ?
                                                        NodeReader("InnerText", "Settings/VideoConfig/visualtreatment", null) : "0";
                FileGameSettingsData.WaterSimEnable = (NodeReader("InnerText", "Settings/VideoConfig/watersimenable", null) != "ERROR") ?
                                                       NodeReader("InnerText", "Settings/VideoConfig/watersimenable", null) : "0";
            }
            else
            {
                Log.Warning("USX File: Unknown File Read Type -> " + FileReadStatus);
            }
        }

        public static void Save(string MessageBoxAlert, string FileReadStatus)
        {
            try
            {
                if (FileReadStatus == "Language Only")
                {
                    /* Language */
                    NodeUpdater("InnerText", "Settings/PersistentValue/Chat", "DefaultChatGroup", "Type", "string", FileGameSettingsData.Language);
                    NodeUpdater("InnerText", "Settings/UI", "Language", "Type", "string", FileGameSettingsData.Language);
                    /* Tracks */
                    NodeUpdater("InnerText", "Settings/UI", "Tracks", "Type", "int", "1");
                }
                else if (FileReadStatus == "Full File")
                {
                    /* Audio */
                    NodeUpdater("Attributes", "Settings/UI/Audio", "AudioOptions", "AudioMode", FileGameSettingsData.AudioMode, FileGameSettingsData.AudioMode);
                    NodeUpdater("Attributes", "Settings/UI/Audio", "AudioOptions", "SFXVol", FileGameSettingsData.SFXAudio, FileGameSettingsData.SFXAudio);
                    NodeUpdater("Attributes", "Settings/UI/Audio", "AudioOptions", "MasterVol", FileGameSettingsData.MasterAudio, FileGameSettingsData.MasterAudio);
                    NodeUpdater("Attributes", "Settings/UI/Audio", "AudioOptions", "CarVol", FileGameSettingsData.CarAudio, FileGameSettingsData.CarAudio);
                    NodeUpdater("Attributes", "Settings/UI/Audio", "AudioOptions", "SpeechVol", FileGameSettingsData.SpeechAudio, FileGameSettingsData.SpeechAudio);
                    NodeUpdater("Attributes", "Settings/UI/Audio", "AudioOptions", "GameMusicVol", FileGameSettingsData.MusicAudio, FileGameSettingsData.MusicAudio);
                    NodeUpdater("Attributes", "Settings/UI/Audio", "AudioOptions", "FEMusicVol", FileGameSettingsData.FreeroamAudio, FileGameSettingsData.FreeroamAudio);
                    /* Gameplay */
                    NodeUpdater("Attributes", "Settings/UI/Gameplay", "GamePlayOptions", "camera", FileGameSettingsData.CameraPOV, FileGameSettingsData.CameraPOV);
                    NodeUpdater("Attributes", "Settings/UI/Gameplay", "GamePlayOptions", "transmission", FileGameSettingsData.Transmission, FileGameSettingsData.Transmission);
                    NodeUpdater("Attributes", "Settings/UI/Gameplay", "GamePlayOptions", "damage", FileGameSettingsData.Damage, FileGameSettingsData.Damage);
                    NodeUpdater("Attributes", "Settings/UI/Gameplay", "GamePlayOptions", "moments", FileGameSettingsData.Moments, FileGameSettingsData.Moments);
                    NodeUpdater("Attributes", "Settings/UI/Gameplay", "GamePlayOptions", "speedUnits", FileGameSettingsData.SpeedUnits, FileGameSettingsData.SpeedUnits);
                    /* Physics */
                    NodeUpdater("InnerText", "Settings/Physics", "CameraPOV", "Type", "int", FileGameSettingsData.CameraPOV);
                    NodeUpdater("InnerText", "Settings/Physics", "TransmissionType", "Type", "int", FileGameSettingsData.Transmission);
                    /* VideoConfig */
                    NodeUpdater("InnerText", "Settings/VideoConfig", "audiomode", "Type", "int", FileGameSettingsData.AudioMode);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "audioquality", "Type", "int", FileGameSettingsData.AudioQuality);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "brightness", "Type", "int", FileGameSettingsData.Brightness);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "enableaero", "Type", "int", FileGameSettingsData.EnableAero);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "pixelaspectratiooverride", "Type", "int", FileGameSettingsData.PixelAspectRatioOverride);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "performancelevel", "Type", "int", FileGameSettingsData.PerformanceLevel);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "screenheight", "Type", "int", FileGameSettingsData.ScreenHeight);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "screenwidth", "Type", "int", FileGameSettingsData.ScreenWidth);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "screenwindowed", "Type", "int", FileGameSettingsData.ScreenWindowed);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "vsyncon", "Type", "int", FileGameSettingsData.VSyncOn);
                    /* VideoConfig Addons */
                    NodeUpdater("InnerText", "Settings/VideoConfig", "basetexturefilter", "Type", "int", FileGameSettingsData.BaseTextureFilter);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "basetexturelodbias", "Type", "int", FileGameSettingsData.BaseTextureLODBias);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "basetexturemaxani", "Type", "int", FileGameSettingsData.BaseTextureMaxAni);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "carenvironmentmapenable", "Type", "int", FileGameSettingsData.CarEnvironmentMapEnable);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "carlodlevel", "Type", "int", FileGameSettingsData.CarLODLevel);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "fsaalevel", "Type", "int", FileGameSettingsData.FSAALevel);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "globaldetaillevel", "Type", "int", FileGameSettingsData.GlobalDetailLevel);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "maxskidmarks", "Type", "int", FileGameSettingsData.MaxSkidMarks);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "motionblurenable", "Type", "int", FileGameSettingsData.MotionBlurEnable);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "overbrightenable", "Type", "int", FileGameSettingsData.OverBrightEnable);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "particlesystemenable", "Type", "int", FileGameSettingsData.ParticleSystemEnable);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "postprocessingenable", "Type", "int", FileGameSettingsData.PostProcessingEnable);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "rainenable", "Type", "int", FileGameSettingsData.RainEnable);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "roadreflectionenable", "Type", "int", FileGameSettingsData.RoadReflectionEnable);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "roadtexturefilter", "Type", "int", FileGameSettingsData.RoadTextureFilter);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "roadtexturelodbias", "Type", "int", FileGameSettingsData.RoadTextureLODBias);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "roadtexturemaxani", "Type", "int", FileGameSettingsData.RoadTextureMaxAni);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "shaderdetail", "Type", "int", FileGameSettingsData.ShaderDetail);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "shadowdetail", "Type", "int", FileGameSettingsData.ShadowDetail);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "visualtreatment", "Type", "int", FileGameSettingsData.VisualTreatment);
                    NodeUpdater("InnerText", "Settings/VideoConfig", "watersimenable", "Type", "int", FileGameSettingsData.WaterSimEnable);
                }
                else
                {
                    Log.Warning("USX File: Unknown File Read Type -> " + FileReadStatus);
                }

                if (new FileInfo(Locations.UserSettingsXML).IsReadOnly != true)
                {
                    UserSettingsFile.Save(Locations.UserSettingsXML);
                    if (MessageBoxAlert == "Display")
                    {
                        MessageBox.Show(null, "XML Settings Saved", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    Log.Error("USX File: UserSettings File is Read-Only. Settings Not Saved!");
                    if (MessageBoxAlert == "Display")
                    {
                        MessageBox.Show(null, "XML Settings Not Saved: \n Connot write to a Read-Only File ", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("USX File", null, Error, null, true);
            }
        }

        public static bool NodeChecker(string Type, string NodeLocation, string AttributeName)
        {
            try
            {
                if (Type == "Attributes")
                {
                    if (UserSettingsFile.SelectSingleNode(NodeLocation).Attributes[AttributeName].Value == null ||
                        UserSettingsFile.SelectSingleNode(NodeLocation).Attributes[AttributeName].Value != null)
                    {
                        return true;
                    }

                    return false;
                }
                else
                {
                    if (UserSettingsFile.SelectSingleNode(NodeLocation).InnerText == null ||
                        UserSettingsFile.SelectSingleNode(NodeLocation).InnerText != null)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void NodeUpdater(string Type, string NodePath, string SingleNode, string AttributeName, string AttributeValue, string ValueComparison)
        {
            string FullNodePath = Strings.Encode(NodePath + "/" + SingleNode);

            if (NodeChecker(Type, FullNodePath, AttributeName) == false)
            {
                try
                {
                    XmlNode Root = UserSettingsFile.SelectSingleNode(Strings.Encode(NodePath));
                    XmlNode CustomNode = UserSettingsFile.CreateElement(Strings.Encode(SingleNode));
                    XmlAttribute CustomNodeAttribute = UserSettingsFile.CreateAttribute(Strings.Encode(AttributeName));
                    CustomNodeAttribute.Value = Strings.Encode(AttributeValue);
                    CustomNode.Attributes.Append(CustomNodeAttribute);
                    Root.AppendChild(CustomNode);
                    Log.Info("USX File: Created XML Node [Type: '" + Type + "' NodePath: '" + NodePath + "' SingleNode: '" +
                                SingleNode + "' AttributeName: '" + AttributeName + "' AttributeValue: '" + AttributeValue + "']");
                }
                catch (Exception Error)
                {
                    Log.Error("USX File: Unable to Create XML Node [Type: '" + Type + "' NodePath: '" + NodePath + "' SingleNode: '" +
                                SingleNode + "' AttributeName: '" + AttributeName + "' AttributeValue: '" + AttributeValue + "']" + Error.Message);
                    Log.ErrorIC("USX File: " + Error.HResult);
                    Log.ErrorFR("USX File: " + Error.ToString());
                    return;
                }
            }

            if (Type == "Attributes")
            {
                if (InsiderKit.EnableInsiderDeveloper.Allowed() || InsiderKit.EnableInsiderBetaTester.Allowed())
                {
                    Log.Debug("USX File: Comparing Values for '" + FullNodePath + "' CURRENT: '" + UserSettingsFile.SelectSingleNode(FullNodePath).Attributes[AttributeName].Value +
                              "' COMPARING NEW: '" + ValueComparison + "'");
                }

                if (UserSettingsFile.SelectSingleNode(FullNodePath).Attributes[Strings.Encode(AttributeName)].Value != Strings.Encode(ValueComparison))
                {
                    UserSettingsFile.SelectSingleNode(FullNodePath).Attributes[Strings.Encode(AttributeName)].Value = Strings.Encode(ValueComparison);
                }
            }
            else
            {
                if (InsiderKit.EnableInsiderDeveloper.Allowed() || InsiderKit.EnableInsiderBetaTester.Allowed())
                {
                    Log.Debug("USX File: Comparing Values for '" + FullNodePath + "' CURRENT: '" + UserSettingsFile.SelectSingleNode(FullNodePath).InnerText +
                              "' COMPARING NEW: '" + ValueComparison + "'");
                }

                if (UserSettingsFile.SelectSingleNode(FullNodePath).InnerText != Strings.Encode(ValueComparison))
                {
                    UserSettingsFile.SelectSingleNode(FullNodePath).InnerText = Strings.Encode(ValueComparison);
                }
            }
        }

        public static string NodeReader(string Type, string FullNodePath, string AttributeName)
        {
            try
            {
                if (Type == "Attributes")
                {
                    if (UserSettingsFile.SelectSingleNode(FullNodePath).Attributes[AttributeName].Value == null)
                    {
                        return "ERROR";
                    }

                    return UserSettingsFile.SelectSingleNode(FullNodePath).Attributes[AttributeName].Value;
                }
                else
                {
                    if (UserSettingsFile.SelectSingleNode(FullNodePath).InnerText == null)
                    {
                        return "ERROR";
                    }
                    return UserSettingsFile.SelectSingleNode(FullNodePath).InnerText;
                }
            }
            catch (Exception Error)
            {
                Log.Error("USX File: Unable to Read XML Node [NodePath: '" + FullNodePath + "' AttributeName: '" + AttributeName + "']" + Error.Message);
                Log.ErrorIC("USX File: " + Error.HResult);
                Log.ErrorFR("USX File: " + Error.ToString());
                return "ERROR";
            }
        }
    }
}
