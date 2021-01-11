using NTMiner.User;
using NTMiner.Ws;

namespace NTMiner.Core.Impl {
    public class MinerStudioSession : AbstractSession, IMinerStudioSession {
        /// <summary>
        /// 不会返回null
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userName"></param>
        /// <param name="wsSessionID"></param>
        /// <param name="sessions"></param>
        /// <returns></returns>
        public static MinerStudioSession Create(IUser user, WsUserName userName, string wsSessionID, IWsSessionsAdapter sessions) {
            return new MinerStudioSession(user, userName, wsSessionID, sessions);
        }

        private MinerStudioSession(IUser user, WsUserName userName, string wsSessionID, IWsSessionsAdapter sessions)
            : base(user, userName, wsSessionID, sessions) {
        }

        public bool IsValid(WsMessage message) {
            if (message == null || string.IsNullOrEmpty(message.Sign)) {
                return false;
            }
            var userData = WsRoot.ReadOnlyUserSet.GetUser(UserId.CreateLoginNameUserId(this.LoginName));
            if (userData == null) {
                return false;
            }
            return message.Sign == message.CalcSign(userData.Password);
        }
    }
}
