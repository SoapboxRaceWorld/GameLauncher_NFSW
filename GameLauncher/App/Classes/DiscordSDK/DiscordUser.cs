using System;

namespace DiscordRPC
{
	[Serializable]
	public struct DiscordUser
	{
		public string userId;
		public string username;
		public string discriminator;
		public string avatar;
	}
}