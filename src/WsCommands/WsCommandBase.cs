using NTMiner.Hub;
using System;
using WebSocketSharp.Server;

namespace WsCommands {
    public abstract class WsCommandBase : ICmd {
        public WsCommandBase(Guid messageId, string sessionId, WebSocketSessionManager sessions) {
            this.MessageId = messageId;
            this.SessionId = sessionId;
            this.Sessions = sessions;
        }

        public Guid MessageId { get; private set; }
        public string SessionId { get; private set; }
        public WebSocketSessionManager Sessions { get; private set; }
    }
}
