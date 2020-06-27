using System;
using System.Runtime.InteropServices;

namespace DiscordRPC { 
    [Serializable, StructLayout(LayoutKind.Sequential)]
	public struct RichPresenceStruct {
		public IntPtr state; /* max 128 bytes */
		public IntPtr details; /* max 128 bytes */
		public long startTimestamp;
		public long endTimestamp;
		public IntPtr largeImageKey; /* max 32 bytes */
		public IntPtr largeImageText; /* max 128 bytes */
		public IntPtr smallImageKey; /* max 32 bytes */
		public IntPtr smallImageText; /* max 128 bytes */
		public IntPtr partyId; /* max 128 bytes */
		public int partySize;
		public int partyMax;
		public IntPtr matchSecret; /* max 128 bytes */
		public IntPtr joinSecret; /* max 128 bytes */
		public IntPtr spectateSecret; /* max 128 bytes */
		public bool instance;
	}
}