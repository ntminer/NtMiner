using NTMiner.Hub;
using System;
using WebSocketSharp.Server;

namespace WsCommands {
    public abstract class WsCommandBase : ICmd {
        public WsCommandBase(string sessionId, WebSocketSessionManager sessions) {
            this.SessionId = sessionId;
            this.Sessions = sessions;
        }

        public Guid MessageId { get; private set; } = Guid.NewGuid();
        public string SessionId { get; private set; }
        public WebSocketSessionManager Sessions { get; private set; }
    }
}
