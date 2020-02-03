using NTMiner.Hub;
using System;
using WebSocketSharp.Server;

namespace NTMiner {
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

    public class GetSpeedWsCommand : WsCommandBase {
        public const string Action = "getSpeed";
        public GetSpeedWsCommand(Guid messageId, string sessionId, WebSocketSessionManager sessions) : base(messageId, sessionId, sessions) {
        }
    }
}
