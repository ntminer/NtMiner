using NTMiner.Core.MinerServer;
using NTMiner.User;

namespace NTMiner.Core.Impl {
    public class MinerClientSession : AbstractSession, IMinerClientSession {
        public static MinerClientSession Create(IUser user, WsUserName wsUserName, string wsSessionID) {
            return new MinerClientSession(user, wsUserName, wsSessionID);
        }

        private MinerClientSession(IUser user, WsUserName wsUserName, string wsSessionID)
            : base(user, wsUserName, wsSessionID) {
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
