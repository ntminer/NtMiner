namespace NTMiner {
    public class ChangePasswordRequest : RequestBase {
        public string LoginName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string Description { get; set; }
    }
}
