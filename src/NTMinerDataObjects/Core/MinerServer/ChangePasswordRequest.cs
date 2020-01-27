namespace NTMiner.Core.MinerServer {
    public class ChangePasswordRequest : RequestBase {
        public ChangePasswordRequest() { }
        public string LoginName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string Description { get; set; }
    }
}
