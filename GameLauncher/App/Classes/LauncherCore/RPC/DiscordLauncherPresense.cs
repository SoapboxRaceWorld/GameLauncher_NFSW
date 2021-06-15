using DiscordRPC;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Windows;
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

        /* Used to prevent Displaying RPC when there is an Error (Displays The Simple Error in RPC) */
        public static bool Download = true;

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
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw"
                };
            }
            else if (State == "Unpack Game Files")
            {
                Download = true;
                Presence.State = Status;
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw",
                    SmallImageText = string.Empty,
                    SmallImageKey = "files_success"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Download Game Files")
            {
                Download = true;
                Presence.State = Status;
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw",
                    SmallImageText = string.Empty,
                    SmallImageKey = "files"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Download Game Files Error")
            {
                Download = true;
                Presence.State = "Game Download Error";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw",
                    SmallImageText = string.Empty,
                    SmallImageKey = "files_error"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Idle Ready")
            {
                Presence.State = "Ready To Race";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw",
                    SmallImageText = string.Empty,
                    SmallImageKey = !string.IsNullOrEmpty(CertificateStore.LauncherSerial) ? "official" : "unofficial"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Checking ModNet")
            {
                Presence.State = "Checking ModNet";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw",
                    SmallImageText = string.Empty,
                    SmallImageKey = "files_alert"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "ModNet File Check Passed")
            {
                Presence.State = "Has ModNet File: " + Status;
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw",
                    SmallImageText = string.Empty,
                    SmallImageKey = "files_success"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Download ModNet")
            {
                Presence.State = "Downloading ModNet Files";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw",
                    SmallImageText = string.Empty,
                    SmallImageKey = "files"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Download ModNet Error")
            {
                Presence.State = "ModNet Encounterd an Error";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw",
                    SmallImageText = string.Empty,
                    SmallImageKey = "files_error"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Download Server Mods")
            {
                Presence.State = "Downloading Server Mods";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw",
                    SmallImageText = string.Empty,
                    SmallImageKey = "files"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }
            else if (State == "Download Server Mods Error")
            {
                Presence.State = "Server Mod Download Error";
                Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                Presence.Assets = new Assets
                {
                    LargeImageText = "Launcher",
                    LargeImageKey = "nfsw",
                    SmallImageText = string.Empty,
                    SmallImageKey = "files_error"
                };
                Presence.Buttons = ButtonsList.ToArray();
            }

            if (Download == false)
            {
                if (State == "Register")
                {
                    Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                    Presence.State = "On Registration Screen";
                    Presence.Assets = new Assets
                    {
                        LargeImageText = "Launcher",
                        LargeImageKey = "nfsw",
                        SmallImageText = string.Empty,
                        SmallImageKey = "screen_register"
                    };
                }
                if (State == "Settings")
                {
                    Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                    Presence.State = "On Settings Screen";
                    Presence.Assets = new Assets
                    {
                        LargeImageText = "Launcher",
                        LargeImageKey = "nfsw",
                        SmallImageText = string.Empty,
                        SmallImageKey = "screen_settings"
                    };
                }
                else if (State == "User XML Editor")
                {
                    Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                    Presence.State = "On User XML Editor Screen";
                    Presence.Assets = new Assets
                    {
                        LargeImageText = "Launcher",
                        LargeImageKey = "nfsw",
                        SmallImageText = string.Empty,
                        SmallImageKey = "screen_uxe"
                    };
                }
                else if (State == "Verify")
                {
                    Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                    Presence.State = "On Verify Game Files Screen";
                    Presence.Assets = new Assets
                    {
                        LargeImageText = "Launcher",
                        LargeImageKey = "nfsw",
                        SmallImageText = string.Empty,
                        SmallImageKey = "screen_verify"
                    };
                }
                else if (State == "Verify Scan")
                {
                    Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                    Presence.State = "Verifying Game Files";
                    Presence.Assets = new Assets
                    {
                        LargeImageText = "Launcher",
                        LargeImageKey = "nfsw",
                        SmallImageText = string.Empty,
                        SmallImageKey = "verify_files_scan"
                    };
                }
                else if (State == "Verify Bad")
                {
                    Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                    Presence.State = "Downloaded " + Status + " Missing Game Files";
                    Presence.Assets = new Assets
                    {
                        LargeImageText = "Launcher",
                        LargeImageKey = "nfsw",
                        SmallImageText = string.Empty,
                        SmallImageKey = "verify_files_bad"
                    };
                }
                else if (State == "Verify Good")
                {
                    Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                    Presence.State = "Finished Validating Game Files";
                    Presence.Assets = new Assets
                    {
                        LargeImageText = "Launcher",
                        LargeImageKey = "nfsw",
                        SmallImageText = string.Empty,
                        SmallImageKey = "verify_files_good"
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
                        SmallImageText = string.Empty,
                        SmallImageKey = "ingame"
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
            }

            if (Client != null && InformationCache.SelectedServerCategory != "DEV")
                Client.SetPresence(Presence);
        }

        public static void Start(string State, string RPCID)
        {
            try
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

                        Client.OnError += (sender, Error) =>
                        {
                            Log.Error("DISCORD: " + Error.Message);
                        };

                        Client.Initialize();
                        Update();
                    }
                    else if (State == "New RPC")
                    {
                        if (InformationCache.SelectedServerCategory != "DEV")
                        {
                            Log.Core("DISCORD: Initializing Rich Presense Core For Server");

                            Stop("Update");
                            Client = new DiscordRpcClient(RPCID);

                            Client.OnError += (sender, Error) =>
                            {
                                Log.Error("DISCORD: " + Error.Message);
                            };

                            Client.Initialize();
                        }
                        else
                        {
                            Stop("Update");
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
            catch (Exception Error)
            {
                Log.Error("DISCORD: " + Error.Message);
            }
        }

        public static void Update()
        {
            if (Client != null)
                Client.Invoke();
        }

        public static void Stop(string State)
        {
            if (Client != null)
            {
                if (State == "Close")
                {
                    Client.ClearPresence();
                }

                Client.Dispose();
                Client = null;
            }
        }

        public static string ApplicationID()
        {
            if (!string.IsNullOrEmpty(InformationCache.SelectedServerJSON.discordApplicationID))
            {
                return InformationCache.SelectedServerJSON.discordApplicationID;
            }
            else if (!string.IsNullOrEmpty(InformationCache.SelectedServerData.DiscordAppId))
            {
                return InformationCache.SelectedServerData.DiscordAppId;
            }
            else
            {
                return "540651192179752970";
            }
        }
    }
}
