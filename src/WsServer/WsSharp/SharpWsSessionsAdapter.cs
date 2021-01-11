using System.Collections.Generic;
using System.Linq;
using WebSocketSharp.Server;

namespace NTMiner.WsSharp {
    public class SharpWsSessionsAdapter : IWsSessionsAdapter {
        private readonly WebSocketSessionManager _sessions;
        public SharpWsSessionsAdapter(WebSocketSessionManager sessions) {
            _sessions = sessions;
        }

        public int Count {
            get {
                return _sessions.Count;
            }
        }

        public IEnumerable<IWsSessionAdapter> Sessions {
            get {
                return _sessions.Sessions.Cast<IWsSessionAdapter>();
            }
        }

        public bool TryGetSession(string sessionId, out IWsSessionAdapter session) {
            if (_sessions.TryGetSession(sessionId, out IWebSocketSession item)) {
                session = (IWsSessionAdapter)item;
                return true;
            }
            session = null;
            return false;
        }
    }
}
