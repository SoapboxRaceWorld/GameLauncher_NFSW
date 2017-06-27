namespace SoapBox.JsonScheme {
    public class GetServerInformation {
        public string messageSrv { get; set; }
        public string homePageUrl { get; set; }
        public string facebookUrl { get; set; }
        public string discordUrl { get; set; }
        public string serverName { get; set; }
        public string country { get; set; }
        public int timezone { get; set; }
        public string bannerUrl { get; set; }
        public string adminList { get; set; }
        public string ownerList { get; set; }
        public int numberOfRegistered { get; set; }
        public int onlineNumber { get; set; }
    }

    //WorldRevival
    public class Server {
        public string status { get; set; }
        public string slots { get; set; }
        public string maxslots { get; set; }
    }

    public class GetStatus {
        public Server server { get; set; }
    }
}
