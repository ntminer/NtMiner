using System;
using WebSocketSharp.Server;

namespace WsCommands {
    public class GetSpeedWsCommand : WsCommandBase {
        public const string RequestAction = "getSpeed";
        public const string ResponseAction = "responseSpeed";
        public GetSpeedWsCommand(Guid messageId, string sessionId, WebSocketSessionManager sessions) : base(messageId, sessionId, sessions) {
        }
    }
}
