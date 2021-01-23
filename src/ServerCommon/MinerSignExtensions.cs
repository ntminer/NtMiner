using NTMiner.Core.MinerServer;
using NTMiner.User;

namespace NTMiner {
    public static class MinerSignExtensions {
        public static bool IsOwnerBy(this IMinerSign minerSign, IUser user) {
            if (user == null) {
                return false;
            }
            if (string.IsNullOrEmpty(minerSign.LoginName)) {
                return false;
            }
            return minerSign.LoginName == user.LoginName;
        }
    }
}
