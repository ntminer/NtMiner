namespace NTMiner.MinerClient {
    public class AddUserRequest : RequestBase, IUser {
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
    }
}
