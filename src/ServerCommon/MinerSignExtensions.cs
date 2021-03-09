using NTMiner.Core.MinerServer;
using NTMiner.User;

namespace NTMiner {
    public static class MinerSignExtensions {
        public static bool IsOwnerBy(this IMinerSign minerSign, IUser user) {
            if (user == null || minerSign == null) {
                return false;
            }
            if (string.IsNullOrEmpty(minerSign.LoginName)) {
                return false;
            }
            return minerSign.LoginName == user.LoginName;
        }

        public static bool IsAdminOrOwnerBy(this IMinerSign minerSign, IUser user) {
            bool result = IsOwnerBy(minerSign, user);
            if (!result) {
                result = user.IsAdmin();
            }
            return result;
        }
    }
}
