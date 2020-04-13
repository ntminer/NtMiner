using NTMiner.Core;

namespace NTMiner.User {
    public class UserMineWorkData : MineWorkData, IUserMineWork {
        public UserMineWorkData() {
        }

        public string LoginName { get; set; }
    }
}
