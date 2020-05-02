using NTMiner.User;
using NTMiner.Ws;
using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace NTMiner.Core.Impl {
    public abstract class AbstractSession : ISession {
        private readonly WebSocketSessionManager _wsSessionManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="wsSessionID"></param>
        public AbstractSession(IUser user, WsUserName wsUserName, string wsSessionID, WebSocketSessionManager wsSessionManager) {
            _wsSessionManager = wsSessionManager;
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

        public void CloseAsync(CloseStatusCode code, string reason) {
            if (TryGetWsSession(out IWebSocketSession wsSession)) {
                wsSession.Context.WebSocket.CloseAsync(code, reason);
            }
        }

        public void SendAsync(WsMessage message, string password) {
            if (TryGetWsSession(out IWebSocketSession wsSession)) {
                if (WsUserName.IsBinarySupported) {
                    wsSession.Context.WebSocket.SendAsync(message.SignToBytes(password), completed: null);
                }
                else {
                    wsSession.Context.WebSocket.SendAsync(message.SignToJson(password), completed: null);
                }
            }
        }

        private bool TryGetWsSession(out IWebSocketSession wsSession) {
            return _wsSessionManager.TryGetSession(this.WsSessionId, out wsSession);
        }

        public void Active() {
            this.ActiveOn = DateTime.Now;
        }
    }
}
