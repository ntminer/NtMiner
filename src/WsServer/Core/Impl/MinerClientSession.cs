using NTMiner.Core.MinerServer;
using NTMiner.User;
using WebSocketSharp.Server;

namespace NTMiner.Core.Impl {
    public class MinerClientSession : AbstractSession, IMinerClientSession {
        /// <summary>
        /// 不会返回null
        /// </summary>
        /// <param name="user"></param>
        /// <param name="wsUserName"></param>
        /// <param name="wsSessionID"></param>
        /// <param name="wsSessionManager"></param>
        /// <returns></returns>
        public static MinerClientSession Create(IUser user, WsUserName wsUserName, string wsSessionID, WebSocketSessionManager wsSessionManager) {
            return new MinerClientSession(user, wsUserName, wsSessionID, wsSessionManager);
        }

        private MinerClientSession(IUser user, WsUserName wsUserName, string wsSessionID, WebSocketSessionManager wsSessionManager)
            : base(user, wsUserName, wsSessionID, wsSessionManager) {
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
