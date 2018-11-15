using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DiscordRPC {
    public class RichPresence {
		private RichPresenceStruct _presence;
		private readonly List<IntPtr> _buffers = new List<IntPtr>(10);

		public string state; /* max 128 bytes */
		public string details; /* max 128 bytes */
		public long? startTimestamp;
		public long? endTimestamp;
		public string largeImageKey; /* max 32 bytes */
		public string largeImageText; /* max 128 bytes */
		public string smallImageKey; /* max 32 bytes */
		public string smallImageText; /* max 128 bytes */
		public string partyId; /* max 128 bytes */
		public int? partySize;
		public int? partyMax;
		public string matchSecret; /* max 128 bytes */
		public string joinSecret; /* max 128 bytes */
		public string spectateSecret; /* max 128 bytes */
		public bool? instance;

		internal RichPresenceStruct GetStruct() {
			if (_buffers.Count > 0) {
				FreeMem();
			}

			_presence.state = StrToPtr(state);
			_presence.details = StrToPtr(details);

			_presence.startTimestamp = startTimestamp ?? default(long);
			_presence.endTimestamp = endTimestamp ?? default(long);

			_presence.largeImageKey = StrToPtr(largeImageKey);
			_presence.largeImageText = StrToPtr(largeImageText);
			_presence.smallImageKey = StrToPtr(smallImageKey);
			_presence.smallImageText = StrToPtr(smallImageText);
			_presence.partyId = StrToPtr(partyId);


			_presence.partySize = partySize ?? default(int);
			_presence.partyMax = partyMax ?? default(int);

			_presence.matchSecret = StrToPtr(matchSecret);
			_presence.joinSecret = StrToPtr(joinSecret);
			_presence.spectateSecret = StrToPtr(spectateSecret);

			_presence.instance = instance ?? default(bool);

			return _presence;
		}

		private IntPtr StrToPtr(string input) {
			if (string.IsNullOrEmpty(input)) return IntPtr.Zero;
			var convbytecnt = Encoding.UTF8.GetByteCount(input);
			var buffer = Marshal.AllocHGlobal(convbytecnt + 1);
			for (int i = 0; i < convbytecnt + 1; i++)
			{
				Marshal.WriteByte(buffer, i, 0);
			}
			_buffers.Add(buffer);
			Marshal.Copy(Encoding.UTF8.GetBytes(input), 0, buffer, convbytecnt);
			return buffer;
		}

		private static string StrToUtf8NullTerm(string toconv) {
			var str = toconv.Trim();
			var bytes = Encoding.Default.GetBytes(str);
			if (bytes.Length > 0 && bytes[ bytes.Length - 1 ] != 0) {
				str += "\0\0";
			}
			return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(str));
		}


		internal void FreeMem() {
			for (var i = _buffers.Count - 1; i >= 0; i--) {
				Marshal.FreeHGlobal(_buffers[ i ]);
				_buffers.RemoveAt(i);
			}
		}
	}
}