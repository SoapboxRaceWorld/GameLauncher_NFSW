using GameLauncher.App.Classes.Logger;
using System;
using System.IO;
using System.Xml;

namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    class FileGameSettingsData
    {
        /* Audio */
        public static string AudioMode = "0";
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
        public static string SpeedUnits = "1";
        /* Physics */
        public static string CameraPOV = "2";
        public static string TransmissionType = "1";
        /* VideoConfig */
        public static string AudioQuality = "0";
        public static string Brightness = "0";
        public static string EnableAero = "0";
        public static string FirstTime = "0";
        public static string ForcesM1x = "false";
        public static string PerformanceLevel = "2";
        public static string PixelAspectRatioOverride = "2";
        public static string ScreenWindowed = "0";
        public static string VSyncOn = "0";
    }

    class FileGameSettings
    {
        public static string UserSettingsLocation = Environment.GetEnvironmentVariable("AppData") + "/Need for Speed World/Settings/UserSettings.xml";

        public static XmlDocument UserSettingsFile = new XmlDocument();

        public static void Read()
        {
            if (File.Exists(UserSettingsLocation))
            {
                try
                {
                    UserSettingsFile.Load(UserSettingsLocation);

                    /* Audio */
                    FileGameSettingsData.AudioMode = UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["AudioMode"].Value;
                    FileGameSettingsData.SFXAudio = UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["SFXVol"].Value;
                    FileGameSettingsData.MasterAudio = UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["MasterVol"].Value;
                    FileGameSettingsData.CarAudio = UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["CarVol"].Value;
                    FileGameSettingsData.SpeechAudio = UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["SpeechVol"].Value;
                    FileGameSettingsData.MusicAudio = UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["GameMusicVol"].Value;
                    FileGameSettingsData.FreeroamAudio = UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["FEMusicVol"].Value;
                    /* Gameplay */
                    FileGameSettingsData.Camera = UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["camera"].Value;
                    FileGameSettingsData.Transmission = UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["transmission"].Value;
                    FileGameSettingsData.Damage = UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["damage"].Value;
                    FileGameSettingsData.SpeedUnits = UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["speedUnits"].Value;
                    /* Physics */
                    FileGameSettingsData.CameraPOV = UserSettingsFile.SelectSingleNode("Settings/Physics/CameraPOV").InnerText;
                    FileGameSettingsData.TransmissionType = UserSettingsFile.SelectSingleNode("Settings/Physics/TransmissionType").InnerText;
                    /* VideoConfig */
                    FileGameSettingsData.AudioQuality = UserSettingsFile.SelectSingleNode("Settings/VideoConfig/audioquality").InnerText;
                    FileGameSettingsData.Brightness = UserSettingsFile.SelectSingleNode("Settings/VideoConfig/brightness").InnerText;
                    FileGameSettingsData.EnableAero = UserSettingsFile.SelectSingleNode("Settings/VideoConfig/enableaero").InnerText;
                    FileGameSettingsData.FirstTime = UserSettingsFile.SelectSingleNode("Settings/VideoConfig/firsttime").InnerText;
                    FileGameSettingsData.ForcesM1x = UserSettingsFile.SelectSingleNode("Settings/VideoConfig/forcesm1x").InnerText;
                    FileGameSettingsData.PixelAspectRatioOverride = UserSettingsFile.SelectSingleNode("Settings/VideoConfig/pixelaspectratiooverride").InnerText;
                    FileGameSettingsData.ScreenWindowed = UserSettingsFile.SelectSingleNode("Settings/VideoConfig/screenwindowed").InnerText;
                    FileGameSettingsData.VSyncOn = UserSettingsFile.SelectSingleNode("Settings/VideoConfig/vsyncon").InnerText;
                }
                catch (Exception Error)
                {
                    Log.Error("USX File: " + Error.Message);
                }
            }
            else
            {
                Log.Error("USX File: How Could This Happen!? - Heavy");
            }
        }

        public static void Save()
        {
            try
            {
                /* Audio */
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["AudioMode"].Value != FileGameSettingsData.AudioMode)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["AudioMode"].Value = FileGameSettingsData.AudioMode;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["SFXVol"].Value != FileGameSettingsData.SFXAudio)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["SFXVol"].Value = FileGameSettingsData.SFXAudio;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["MasterVol"].Value != FileGameSettingsData.MasterAudio)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["MasterVol"].Value = FileGameSettingsData.MasterAudio;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["CarVol"].Value != FileGameSettingsData.CarAudio)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["CarVol"].Value = FileGameSettingsData.CarAudio;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["SpeechVol"].Value != FileGameSettingsData.SpeechAudio)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["SpeechVol"].Value = FileGameSettingsData.SpeechAudio;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["GameMusicVol"].Value != FileGameSettingsData.MusicAudio)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["GameMusicVol"].Value = FileGameSettingsData.MusicAudio;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["FEMusicVol"].Value != FileGameSettingsData.FreeroamAudio)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Audio/AudioOptions").Attributes["FEMusicVol"].Value = FileGameSettingsData.FreeroamAudio;
                }
                /* Gameplay */
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["camera"].Value != FileGameSettingsData.Camera)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["camera"].Value = FileGameSettingsData.Camera;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["transmission"].Value != FileGameSettingsData.Transmission)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["transmission"].Value = FileGameSettingsData.Transmission;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["damage"].Value != FileGameSettingsData.Damage)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["damage"].Value = FileGameSettingsData.Damage;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["speedUnits"].Value != FileGameSettingsData.SpeedUnits)
                {
                    UserSettingsFile.SelectSingleNode("Settings/UI/Gameplay/GamePlayOptions").Attributes["speedUnits"].Value = FileGameSettingsData.SpeedUnits;
                }
                /* VideoConfig */
                if (UserSettingsFile.SelectSingleNode("Settings/VideoConfig/audiomode").InnerText != FileGameSettingsData.AudioMode)
                {
                    UserSettingsFile.SelectSingleNode("Settings/VideoConfig/audiomode").InnerText = FileGameSettingsData.AudioMode;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/VideoConfig/audioquality").InnerText != FileGameSettingsData.AudioQuality)
                {
                    UserSettingsFile.SelectSingleNode("Settings/VideoConfig/audioquality").InnerText = FileGameSettingsData.AudioQuality;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/VideoConfig/brightness").InnerText != FileGameSettingsData.Brightness)
                {
                    UserSettingsFile.SelectSingleNode("Settings/VideoConfig/brightness").InnerText = FileGameSettingsData.Brightness;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/VideoConfig/enableaero").InnerText != FileGameSettingsData.EnableAero)
                {
                    UserSettingsFile.SelectSingleNode("Settings/VideoConfig/enableaero").InnerText = FileGameSettingsData.EnableAero;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/VideoConfig/vsyncon").InnerText != FileGameSettingsData.VSyncOn)
                {
                    UserSettingsFile.SelectSingleNode("Settings/VideoConfig/vsyncon").InnerText = FileGameSettingsData.VSyncOn;
                }
                if (UserSettingsFile.SelectSingleNode("Settings/VideoConfig/screenwindowed").InnerText != FileGameSettingsData.ScreenWindowed)
                {
                    UserSettingsFile.SelectSingleNode("Settings/VideoConfig/screenwindowed").InnerText = FileGameSettingsData.ScreenWindowed;
                }

                if (new FileInfo(UserSettingsLocation).IsReadOnly != true)
                {
                    UserSettingsFile.Save(UserSettingsLocation);
                }
                else
                {
                    Log.Error("USX File: User Game Settings File is ReadOnly. Sorry Chief");
                }
            }
            catch (Exception Error)
            {
                Log.Error("USX File: " + Error.Message);
            }
        }
    }
}
