using NTMiner.User;

namespace NTMiner.Role {
    public class ClientSignData {
        public ClientSignData(string loginName, string sign, long timestamp) {
            this.LoginName = loginName;
            this.Sign = sign;
            this.Timestamp = timestamp;
        }

        public string LoginName { get; private set; }
        public string Sign { get; private set; }
        public long Timestamp { get; private set; }

        private UserId _userId;
        public UserId UserId {
            get {
                if (_userId == null) {
                    _userId = UserId.Create(this.LoginName);
                }
                return _userId;
            }
        }
    }
}
