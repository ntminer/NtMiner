using WebSocketSharp.Server;

namespace WsCommands {
    public class GetSpeedWsCommand : WsCommandBase {
        public const string Ping = "getSpeed";
        public const string Pong = "speed";
        public GetSpeedWsCommand(string sessionId, WebSocketSessionManager sessions) : base(sessionId, sessions) {
        }
    }
}
