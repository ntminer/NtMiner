namespace NTMiner.MinerClient {
    public class AddUserRequest : RequestBase, IUser {
        public AddUserRequest() { }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public string Description { get; set; }
    }
}
