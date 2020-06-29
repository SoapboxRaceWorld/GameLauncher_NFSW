using System;
using System.Runtime.InteropServices;

namespace DiscordRPC {
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ReadyCallback(ref DiscordUser connectedUser);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void DisconnectedCallback(int errorCode, string message);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ErrorCallback(int errorCode, string message);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void JoinCallback(string secret);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SpectateCallback(string secret);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void RequestCallback(ref DiscordUser request);

	public class DiscordRpc {
		[DllImport("discord-rpc", EntryPoint = "Discord_Initialize", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Initialize(string applicationId, ref EventHandlers handlers, bool autoRegister, string optionalSteamId);

		[DllImport("discord-rpc", EntryPoint = "Discord_Shutdown", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Shutdown();

		[DllImport("discord-rpc", EntryPoint = "Discord_RunCallbacks", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunCallbacks();

		[DllImport("discord-rpc", EntryPoint = "Discord_UpdatePresence", CallingConvention = CallingConvention.Cdecl)]
		private static extern void UpdatePresenceNative(ref RichPresenceStruct presence);

		[DllImport("discord-rpc", EntryPoint = "Discord_ClearPresence", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ClearPresence();

		[DllImport("discord-rpc", EntryPoint = "Discord_Respond", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Respond(string userId, Reply reply);

		[DllImport("discord-rpc", EntryPoint = "Discord_UpdateHandlers", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UpdateHandlers(ref EventHandlers handlers);

        [DllImport("discord-rpc", EntryPoint = "Discord_Register", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Register(String applicationId, String command);

        [DllImport("discord-rpc", EntryPoint = "Discord_Register", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterSteamGame(String applicationId, String steamId);


        public static void UpdatePresence(RichPresence presence) {
			var presencestruct = presence.GetStruct();
			UpdatePresenceNative(ref presencestruct);
			presence.FreeMem();
		}

		public static void UpdatePresence(Action<RichPresence> callback) {
			var rp = new RichPresence();
			callback(rp);
			UpdatePresence(rp);
		}

		public static void UpdatePresence(Action<RichPresenceBuilder> callback) {
			var rpb = new RichPresenceBuilder();
			callback(rpb);
			UpdatePresence(rpb);
		}

		public static void UpdatePresence(RichPresenceBuilder builder) {
			UpdatePresence(builder.Build());
		}
	}
}