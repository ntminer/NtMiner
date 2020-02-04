using NTMiner.Hub;
using System;
using WebSocketSharp.Server;

namespace WsCommands {
    public abstract class WsCommandBase : ICmd {
        private readonly Guid _messageId = Guid.NewGuid();

        public WsCommandBase(string sessionId, WebSocketSessionManager sessions) {
            this.SessionId = sessionId;
            this.Sessions = sessions;
        }

        public Guid MessageId {
            get { return _messageId; }
        }
        public string SessionId { get; private set; }
        public WebSocketSessionManager Sessions { get; private set; }
    }
}
