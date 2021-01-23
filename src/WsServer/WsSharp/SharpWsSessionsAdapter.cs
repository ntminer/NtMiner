using WebSocketSharp.Server;

namespace NTMiner.WsSharp {
    public class SharpWsSessionsAdapter : IWsSessionsAdapter {
        private readonly WebSocketSessionManager _wsSessions;
        public SharpWsSessionsAdapter(WebSocketSessionManager wsSessions) {
            _wsSessions = wsSessions;
        }

        public int Count {
            get {
                return _wsSessions.Count;
            }
        }

        public bool TryGetSession(string sessionId, out IWsSessionAdapter session) {
            if (_wsSessions.TryGetSession(sessionId, out IWebSocketSession item)) {
                session = (IWsSessionAdapter)item;
                return true;
            }
            session = null;
            return false;
        }
    }
}
