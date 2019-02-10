using NTMiner.Bus;

namespace NTMiner {
    [MessageType(messageType: typeof(UserLoginedEvent), description: "用户登录成功后")]
    public class UserLoginedEvent : EventBase {
        public UserLoginedEvent(IUser user) {
            this.User = user;
        }

        public IUser User { get; private set; }
    }
}
