using WebSocketSharp.Server;

namespace WsCommands {
    public class GetSpeedWsCommand : WsCommandBase {
        public const string PingName = "getSpeed";
        public const string PongName = "speed";
        public GetSpeedWsCommand(string sessionId, WebSocketSessionManager sessions) : base(sessionId, sessions) {
        }
    }
}
