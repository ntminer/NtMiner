using System;
using WebSocketSharp.Server;

namespace WsCommands {
    public class GetSpeedWsCommand : WsCommandBase {
        public const string Action = "getSpeed";
        public const string Result = "speed";
        public GetSpeedWsCommand(Guid messageId, string sessionId, WebSocketSessionManager sessions) : base(messageId, sessionId, sessions) {
        }
    }
}
