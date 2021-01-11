using NTMiner.Core.MinerServer;
using NTMiner.User;

namespace NTMiner.Core.Impl {
    public class MinerClientSession : AbstractSession, IMinerClientSession {
        /// <summary>
        /// 不会返回null
        /// </summary>
        /// <param name="user"></param>
        /// <param name="wsUserName"></param>
        /// <param name="wsSessionID"></param>
        /// <param name="sessions"></param>
        /// <returns></returns>
        public static MinerClientSession Create(IUser user, WsUserName wsUserName, string wsSessionID, IWsSessionsAdapter sessions) {
            return new MinerClientSession(user, wsUserName, wsSessionID, sessions);
        }

        private MinerClientSession(IUser user, WsUserName wsUserName, string wsSessionID, IWsSessionsAdapter sessions)
            : base(user, wsUserName, wsSessionID, sessions) {
        }

        public string GetSignPassword() {
            if (!WsRoot.MinerSignSet.TryGetByClientId(this.ClientId, out MinerSign minerSign)) {
                return string.Empty;
            }
            return minerSign.AESPassword;
        }
    }
}
