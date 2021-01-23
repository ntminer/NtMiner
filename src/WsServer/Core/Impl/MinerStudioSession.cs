using NTMiner.User;
using NTMiner.Ws;
using System.Net;

namespace NTMiner.Core.Impl {
    public class MinerStudioSession : AbstractSession, IMinerStudioSession {
        /// <summary>
        /// 不会返回null
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userName"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="wsSessionID"></param>
        /// <param name="wsSessions"></param>
        /// <returns></returns>
        public static MinerStudioSession Create(
            IUser user, WsUserName userName, IPEndPoint remoteEndPoint, string wsSessionID, IWsSessionsAdapter wsSessions) {
            return new MinerStudioSession(user, userName, remoteEndPoint, wsSessionID, wsSessions);
        }

        private MinerStudioSession(
            IUser user, WsUserName userName, IPEndPoint remoteEndPoint, string wsSessionID, IWsSessionsAdapter wsSessions)
            : base(user, userName, remoteEndPoint, wsSessionID, wsSessions) {
        }

        public bool IsValid(WsMessage message) {
            if (message == null || string.IsNullOrEmpty(message.Sign)) {
                return false;
            }
            var userData = AppRoot.UserSet.GetUser(UserId.CreateLoginNameUserId(this.LoginName));
            if (userData == null) {
                return false;
            }
            return message.Sign == message.CalcSign(userData.Password);
        }
    }
}
