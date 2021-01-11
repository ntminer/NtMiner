using NTMiner.User;
using NTMiner.Ws;
using System;

namespace NTMiner.Core.Impl {
    public abstract class AbstractSession : ISession {
        private readonly IWsSessionsAdapter _sessions;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="wsSessionID"></param>
        public AbstractSession(IUser user, WsUserName wsUserName, string wsSessionID, IWsSessionsAdapter sessions) {
            _sessions = sessions;
            this.WsUserName = wsUserName;
            this.ClientId = wsUserName.ClientId;
            this.ClientVersion = Version.Parse(wsUserName.ClientVersion);// 因为前面的流程已经过验证所以可以直接Parse
            this.LoginName = user.LoginName;
            this.ActiveOn = DateTime.Now;
            this.WsSessionId = wsSessionID;
        }

        public IWsUserName WsUserName { get; private set; }

        public Guid ClientId { get; private set; }

        public Version ClientVersion { get; private set; }

        public string LoginName { get; private set; }

        public DateTime ActiveOn { get; private set; }

        public string WsSessionId { get; private set; }

        public void CloseAsync(WsCloseCode code, string reason) {
            if (TryGetWsSession(out IWsSessionAdapter wsSession)) {
                wsSession.CloseAsync(code, reason);
            }
        }

        public void SendAsync(WsMessage message, string password) {
            if (TryGetWsSession(out IWsSessionAdapter wsSession)) {
                if (WsUserName.IsBinarySupported) {
                    wsSession.SendAsync(message.SignToBytes(password));
                }
                else {
                    wsSession.SendAsync(message.SignToJson(password));
                }
            }
        }

        private bool TryGetWsSession(out IWsSessionAdapter wsSession) {
            return _sessions.TryGetSession(this.WsSessionId, out wsSession);
        }

        public void Active() {
            this.ActiveOn = DateTime.Now;
        }
    }
}
