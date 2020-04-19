using NTMiner.Core.MinerServer;
using NTMiner.User;

namespace NTMiner.Core.Impl {
    public class MinerClientSession : AbstractSession, IMinerClientSession {
        public static MinerClientSession Create(IUser user, WsUserName userName, string wsSessionID) {
            return new MinerClientSession(user, userName, wsSessionID);
        }

        private MinerClientSession(IUser user, WsUserName userName, string wsSessionID)
            : base(user, userName, wsSessionID) {
        }

        public string GetSignPassword() {
            if (!WsRoot.MinerSignSet.TryGetByClientId(this.ClientId, out MinerSign minerSign)) {
                return string.Empty;
            }
            string password = minerSign.AESPassword;
            return password;
        }
    }
}
