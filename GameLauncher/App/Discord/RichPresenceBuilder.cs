namespace DiscordRPC { 
    public class RichPresenceBuilder {
		public class PartyBuilder {
			public string Id {
				get;
				private set;
			}

			public int? Max {
				get;
				private set;
			}

			public int? Size {
				get;
				private set;
			}

			public PartyBuilder() {

			}

			public PartyBuilder WithId(string id = null) {
				Id = id;
				return this;
			}

			public PartyBuilder WithSize(int? size = null) {
				Size = size;
				return this;
			}

			public PartyBuilder WithMax(int? max = null) {
				Max = max;
				return this;
			}
		}

		private RichPresence rp;

		public RichPresenceBuilder() {
			this.rp = new RichPresence();
		}

		public RichPresenceBuilder WithState(string state = null) {
			rp.state = state;
			return this;
		}

		public RichPresenceBuilder WithDetails(string details = null) {
			rp.details = details;
			return this;
		}

		public RichPresenceBuilder WithStartTimestamp(long? st = null) {
			rp.startTimestamp = st;
			return this;
		}

		public RichPresenceBuilder WithEndTimestamp(long? et = null) {
			rp.endTimestamp = et;
			return this;
		}

		public RichPresenceBuilder WithLargeImage(string key = null) {
			rp.largeImageKey = key;
			return this;
		}

		public RichPresenceBuilder WithLargeText(string text = null) {
			rp.largeImageText = text;
			return this;
		}

		public RichPresenceBuilder WithSmallImage(string key = null) {
			rp.largeImageKey = key;
			return this;
		}

		public RichPresenceBuilder WithSmallText(string text = null) {
			rp.largeImageText = text;
			return this;
		}

		public RichPresenceBuilder WithParty(PartyBuilder party = null) {
			if(party == null) {
				rp.partyId = null;
				rp.partyMax = null;
				rp.partySize = null;
			} else {
				rp.partyId = party.Id;
				rp.partySize = party.Size;
				rp.partyMax = party.Max;
			}
			return this;
		}

		public RichPresenceBuilder WithMatchSecret(string secret = null) {
			rp.matchSecret = secret;
			return this;
		}

		public RichPresenceBuilder WithJoinSecret(string secret = null) {
			rp.joinSecret = secret;
			return this;
		}

		public RichPresenceBuilder WithSpectateSecret(string secret = null) {
			rp.spectateSecret = secret;
			return this;
		}

		public RichPresenceBuilder WithIntance(bool? instance = null) {
			rp.instance = instance;
			return this;
		}

		public RichPresence Build() {
			return rp;
		}
	}
}