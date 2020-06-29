namespace DiscordRPC { 
    public struct EventHandlers {
		public ReadyCallback readyCallback;
		public DisconnectedCallback disconnectedCallback;
		public ErrorCallback errorCallback;
		public JoinCallback joinCallback;
		public SpectateCallback spectateCallback;
		public RequestCallback requestCallback;
	}
}