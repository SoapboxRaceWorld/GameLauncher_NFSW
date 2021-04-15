using DiscordRPC;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.Logger;
using System;
using System.Collections.Generic;
using DiscordButton = DiscordRPC.Button;

namespace GameLauncher.App.Classes.LauncherCore.RPC
{
    class DiscordLauncherPresense
    {
        /* Discord RPC Client */
        public static DiscordRpcClient Client;

        /* User's Discord ID */
        public static string UserID = String.Empty;

        /* Discord Presense (To Show Status) */
        public static RichPresence Presence = new RichPresence();

        /* Used to Set Discord Buttons */
        public static List<DiscordButton> ButtonsList = new List<DiscordButton>();

        public static void Status(string State, string Status)
        {
            ButtonsList.Clear();
            ButtonsList.Add(new DiscordButton()
            {
                Label = "Project Site",
                Url = "https://soapboxrace.world"
            });
            ButtonsList.Add(new DiscordButton()
            {
                Label = "Launcher Patch Notes",
                Url = "https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases/tag/" + Theming.PrivacyRPCBuild
            });
            Presence.Buttons = ButtonsList.ToArray();

            if (State == "Start Up")
            {
                Presence.State = Status;
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "SBRW",
                    LargeImageKey = "nfsw"
                };
            }
            else if (State == "Unpack Game Files")
            {
                Presence.State = Status;
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "SBRW",
                    LargeImageKey = "nfsw"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Download Game Files")
            {
                Presence.State = Status;
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "SBRW",
                    LargeImageKey = "nfsw"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Download Game Files Error")
            {
                Presence.State = "Game Download Error";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "SBRW",
                    LargeImageKey = "nfsw"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Idle Ready")
            {
                Presence.State = "Ready To Race";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "SBRW",
                    LargeImageKey = "nfsw"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Download ModNet")
            {
                Presence.State = "Checking ModNet Files!";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "SBRW",
                    LargeImageKey = "nfsw"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Download Server Mods")
            {
                Presence.State = "Downloading Server Mods!";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "SBRW",
                    LargeImageKey = "nfsw"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Verify")
            {
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.State = "Validating Game Files!";
                Presence.Assets = new Assets
                {
                    LargeImageText = "SBRW",
                    LargeImageKey = "nfsw"
                };
            }
            else if (State == "In-Game")
            {
                Presence.State = InformationCache.SelectedServerData.Name;
                Presence.Details = "In-Game";
                Presence.Assets = new Assets
                {
                    LargeImageText = "Need for Speed: World",
                    LargeImageKey = "nfsw",
                    SmallImageText = InformationCache.SelectedServerData.Name,
                    SmallImageKey = Status
                };

                if (!String.IsNullOrEmpty(InformationCache.SelectedServerJSON.homePageUrl) || 
                    !String.IsNullOrEmpty(InformationCache.SelectedServerJSON.discordUrl) || 
                    !String.IsNullOrEmpty(InformationCache.SelectedServerJSON.webPanelUrl))
                {
                    ButtonsList.Clear();

                    if (!String.IsNullOrEmpty(InformationCache.SelectedServerJSON.webPanelUrl))
                    {
                        /* Let's format it now, if possible */
                        ButtonsList.Add(new DiscordButton()
                        {
                            Label = "View Panel",
                            Url = InformationCache.SelectedServerJSON.webPanelUrl.Split(new string[] { "{sep}" }, StringSplitOptions.None)[0]
                        });
                    }
                    else if (!String.IsNullOrEmpty(InformationCache.SelectedServerJSON.homePageUrl) && 
                        InformationCache.SelectedServerJSON.homePageUrl != InformationCache.SelectedServerJSON.discordUrl)
                    {
                        ButtonsList.Add(new DiscordButton()
                        {
                            Label = "Website",
                            Url = InformationCache.SelectedServerJSON.homePageUrl
                        });
                    }

                    if (!String.IsNullOrEmpty(InformationCache.SelectedServerJSON.discordUrl))
                    {
                        ButtonsList.Add(new DiscordButton()
                        {
                            Label = "Discord",
                            Url = InformationCache.SelectedServerJSON.discordUrl
                        });
                    }
                }

                Presence.Buttons = ButtonsList.ToArray();
            }

            if (Client != null && InformationCache.SelectedServerCategory != "DEV")
                Client.SetPresence(Presence);
        }

        public static void Start(string State, string RPCID)
        {
            if (FileSettingsSave.RPC == "0")
            {
                if (State == "Start Up")
                {
                    Log.Core("DISCORD: Initializing Rich Presense Core");

                    Client = new DiscordRpcClient(RPCID);

                    Client.OnReady += (sender, e) =>
                    {
                        Log.Info("DISCORD: Discord ready. Detected user: " + e.User.Username + ". Discord version: " + e.Version);
                        UserID = e.User.ID.ToString();
                    };

                    Client.OnError += (sender, e) =>
                    {
                        Log.Error($"DISCORD: Discord Error\n{e.Message}");
                    };

                    Client.Initialize();
                    Update();
                }
                else if (State == "New RPC")
                {
                    if (InformationCache.SelectedServerCategory != "DEV")
                    {
                        Log.Core("DISCORD: Initializing Rich Presense Core For Server");

                        Stop();
                        Client = new DiscordRpcClient(RPCID);
                        Client.Initialize();
                    }
                    else
                    {
                        Stop();
                    }
                }
                else
                {
                    Log.Error("DISCORD: Unable to Determine The RPC State");
                }
            }
            else
            {
                Log.Warning("DISCORD: User Disabled Rich Presense Core From Launcher Settings");
            }
        }

        public static void Update()
        {
            if (Client != null)
                Client.Invoke();
        }

        public static void Stop()
        {
            if (Client != null)
            {
                Client.Dispose();
                Client = null;
            }
        }
    }
}
