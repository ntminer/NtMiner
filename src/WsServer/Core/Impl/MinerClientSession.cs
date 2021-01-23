using NTMiner.Core.MinerServer;
using NTMiner.User;
using System.Net;

namespace NTMiner.Core.Impl {
    public class MinerClientSession : AbstractSession, IMinerClientSession {
        /// <summary>
        /// 不会返回null
        /// </summary>
        /// <param name="user"></param>
        /// <param name="wsUserName"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="wsSessionID"></param>
        /// <param name="wsSessions"></param>
        /// <returns></returns>
        public static MinerClientSession Create(
            IUser user, WsUserName wsUserName, IPEndPoint remoteEndPoint, string wsSessionID, IWsSessionsAdapter wsSessions) {
            return new MinerClientSession(user, wsUserName, remoteEndPoint, wsSessionID, wsSessions);
        }

        private MinerClientSession(
            IUser user, WsUserName wsUserName, IPEndPoint remoteEndPoint, string wsSessionID, IWsSessionsAdapter wsSessions)
            : base(user, wsUserName, remoteEndPoint, wsSessionID, wsSessions) {
        }

        public string GetSignPassword() {
            if (!AppRoot.MinerSignSet.TryGetByClientId(this.ClientId, out MinerSign minerSign)) {
                return string.Empty;
            }
            return minerSign.AESPassword;
        }
    }
}
