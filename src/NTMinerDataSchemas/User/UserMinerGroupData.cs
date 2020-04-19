using NTMiner.Core;

namespace NTMiner.User {
    public class UserMinerGroupData : MinerGroupData, IUserMinerGroup {
        public UserMinerGroupData() { }

        public string LoginName { get; set; }
    }
}
