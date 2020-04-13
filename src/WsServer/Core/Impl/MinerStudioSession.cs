using NTMiner.User;
using NTMiner.Ws;

namespace NTMiner.Core.Impl {
    public class MinerStudioSession : AbstractSession, IMinerStudioSession {
        public static MinerStudioSession Create(IUser user, WsUserName userName, string wsSessionID) {
            return new MinerStudioSession(user, userName, wsSessionID);
        }

        private MinerStudioSession(IUser user, WsUserName userName, string wsSessionID)
            : base(user, userName, wsSessionID) {
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
