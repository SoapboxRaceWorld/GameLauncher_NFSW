using DiscordRPC;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using System;
using System.Collections.Generic;
using DiscordButton = DiscordRPC.Button;

namespace GameLauncher.App.Classes.LauncherCore.RPC
{
    class DiscordLauncherPresence
    {
        /// <summary>
        /// Launcher's Discord RPC Client
        /// </summary>
        /// <remarks>Discord RPC Client</remarks>
        public static DiscordRpcClient Client;

        /// <summary>
        /// Boolean Value on If RPC is Running
        /// </summary>
        /// <returns>True or False</returns>
        public static bool Running() => Client != null;

        /// <summary>
        /// User's Discord ID Cache and is Set Once by Startuo or During Operation of Launcher's State
        /// </summary>
        /// <remarks>User's Discord ID</remarks>
        public static string UserID = String.Empty;

        /// <summary>
        /// Launcher's Discord Presence To Show Statuss
        /// </summary>
        /// <remarks>Instance of Discord Presence</remarks>
        public static RichPresence Presence = new RichPresence();

        /// <summary>
        /// Used to Set Discord Buttons on RPC Status
        /// </summary>
        /// <remarks>Instance of Discord Buttons</remarks>
        public static List<DiscordButton> ButtonsList = new List<DiscordButton>();

        /// <summary>
        /// Used to prevent Displaying RPC when there is an Error (Displays a Simple Error Message in RPC)
        /// </summary>
        /// <remarks>Displays Launcher Errors in RPC</remarks>
        public static bool Download = true;

        /// <summary>
        /// Sets the current Status of the Launcher's State
        /// </summary>
        /// <remarks>RPC Status</remarks>
        /// <param name="State">Which RPC Status Text to Set</param>
        /// <param name="Status">Additional RPC Status Details to Display</param>
        public static void Status(string State, string Status)
        {
            try
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
                        SmallImageKey = !string.IsNullOrWhiteSpace(CertificateStore.LauncherSerial) ? "official" : "unofficial"
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
                    if (State == "Security Center")
                    {
                        Presence.Details = "In-Launcher: " + Theming.PrivacyRPCBuild;
                        Presence.State = "On Security Center Screen";
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Launcher",
                            LargeImageKey = "nfsw",
                            SmallImageText = SecurityCenter.SecurityCenterRPC(1),
                            SmallImageKey = SecurityCenter.SecurityCenterRPC(0)
                        };
                    }
                    else if (State == "Register")
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
                    else if (State == "Settings")
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
                        Presence.State = ServerListUpdater.ServerName("RPC");
                        Presence.Details = "In-Game";
                        Presence.Assets = new Assets
                        {
                            LargeImageText = "Need for Speed: World",
                            LargeImageKey = "nfsw",
                            SmallImageText = string.Empty,
                            SmallImageKey = "ingame"
                        };

                        if (!String.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.homePageUrl) ||
                            !String.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.discordUrl) ||
                            !String.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.webPanelUrl))
                        {
                            ButtonsList.Clear();

                            if (!String.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.webPanelUrl))
                            {
                                /* Let's format it now, if possible */
                                ButtonsList.Add(new DiscordButton()
                                {
                                    Label = "View Panel",
                                    Url = InformationCache.SelectedServerJSON.webPanelUrl.Split(new string[] { "{sep}" }, StringSplitOptions.None)[0]
                                });
                            }
                            else if (!String.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.homePageUrl) &&
                                InformationCache.SelectedServerJSON.homePageUrl != InformationCache.SelectedServerJSON.discordUrl)
                            {
                                ButtonsList.Add(new DiscordButton()
                                {
                                    Label = "Website",
                                    Url = InformationCache.SelectedServerJSON.homePageUrl
                                });
                            }

                            if (!String.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.discordUrl))
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

                if (Running() && InformationCache.SelectedServerCategory != "DEV")
                    Client.SetPresence(Presence);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("DISCORD", null, Error, null, true);
            }
        }

        /// <summary>
        /// Starts Game Launcher's RPC Status. If a Discord Client is Running on the Machine, it will Display the status on the User's Profile.
        /// </summary>
        /// <remarks>Displays Launcher and Server Status</remarks>
        public static void Start(string State, string RPCID)
        {
            try
            {
                if (FileSettingsSave.RPC == "0")
                {
                    if (State == "Start Up")
                    {
                        Log.Core("DISCORD: Initializing Rich Presence Core");

                        Client = new DiscordRpcClient("576154452348633108");

                        Client.OnReady += (sender, e) =>
                        {
                            Log.Info("DISCORD: Discord ready. Detected user: " + e.User.Username + ". Discord version: " + e.Version);
                            UserID = e.User.ID.ToString();
                        };

                        Client.OnError += (sender, Error) =>
                        {
                            Log.Error("DISCORD: " + Error.Message);
                        };
                        Client.SkipIdenticalPresence = true;
                        Client.ShutdownOnly = true;

                        Client.Initialize();
                        Update();
                    }
                    else if (State == "New RPC")
                    {
                        if (InformationCache.SelectedServerCategory != "DEV")
                        {
                            Log.Core("DISCORD: Initializing Rich Presence Core For Server");

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
                            Stop("Close");
                        }
                    }
                    else
                    {
                        Log.Error("DISCORD: Unable to determine the RPC State");
                    }
                }
                else
                {
                    Log.Warning("DISCORD: User disabled Rich Presence Core from Launcher Settings");
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("DISCORD", null, Error, null, true);
            }
        }

        /// <summary>
        /// Invokes a new Instance of RPC
        /// </summary>
        /// <remarks>Starts a new RPC for Launcher</remarks>
        public static void Update()
        {
            if (Running())
                Client.Invoke();
        }

        /// <summary>
        /// Invokes a Stop Command to RPC by Clearing Current Status before Disposing RPC
        /// </summary>
        /// <remarks>Clears and Stops RPC</remarks>
        public static void Stop(string State)
        {
            if (Running())
            {
                try
                {
                    if (State == "Close")
                    {
                        Client.ClearPresence();
                        Log.Core("DISCORD: Client RPC has now been Cleared");
                    }
                }
                catch { }
                Log.Core("DISCORD: Client RPC Service has been " + State + "d.");
                Client.Dispose();
                Client = null;
            }
        }

        /// <summary>
        /// Retives Discord Application ID by first checking the Server JSON, with the Server List being Second, and the Fallback being the Launcher's ID
        /// </summary>
        /// <remarks>Server's Discord Application ID</remarks>
        public static string ApplicationID()
        {
            if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.discordApplicationID ?? string.Empty))
            {
                return InformationCache.SelectedServerJSON.discordApplicationID;
            }
            else if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerData.DiscordAppID ?? string.Empty))
            {
                return InformationCache.SelectedServerData.DiscordAppID;
            }
            else
            {
                return "540651192179752970";
            }
        }
    }
}
