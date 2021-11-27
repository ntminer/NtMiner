using NTMiner.User;
using NTMiner.Ws;
using System;
using System.Net;

namespace NTMiner.Core.Impl {
    public abstract class AbstractSession : ISession {
        private readonly IWsSessionsAdapter _wsSessions;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="wsSessionID"></param>
        public AbstractSession(
            IUser user, 
            WsUserName wsUserName, 
            IPEndPoint remoteEndPoint, 
            string wsSessionID, 
            IWsSessionsAdapter wsSessions) {

            _wsSessions = wsSessions;
            this.WsUserName = wsUserName;
            this.ClientId = wsUserName.ClientId;
            this.ClientVersion = Version.Parse(wsUserName.ClientVersion);// 因为前面的流程已经过验证所以可以直接Parse
            this.LoginName = user.LoginName;
            this.ActiveOn = DateTime.Now;
            this.RemoteEndPoint = remoteEndPoint;
            this.WsSessionId = wsSessionID;
        }

        public IWsUserName WsUserName { get; private set; }

        public Guid ClientId { get; private set; }

        public Version ClientVersion { get; private set; }

        public string LoginName { get; private set; }

        public DateTime ActiveOn { get; private set; }

        public IPEndPoint RemoteEndPoint { get; private set; }
        public string WsSessionId { get; private set; }

        public void CloseAsync(WsCloseCode code, string reason) {
            if (TryGetWsSession(out IWsSessionAdapter wsSession)) {
                wsSession.CloseAsync(code, reason);
            }
        }

        public void SendAsync(WsMessage message) {
            if (TryGetWsSession(out IWsSessionAdapter wsSession)) {
                if (WsUserName.IsBinarySupported) {
                    wsSession.SendAsync(message.SignToBytes(this.GetSignPassword()));
                }
                else {
                    wsSession.SendAsync(message.SignToJson(this.GetSignPassword()));
                }
            }
        }

        private bool TryGetWsSession(out IWsSessionAdapter wsSession) {
            return _wsSessions.TryGetSession(this.WsSessionId, out wsSession);
        }

        public void Active() {
            this.ActiveOn = DateTime.Now;
        }
    }
}
