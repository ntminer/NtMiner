using NTMiner.Hub;

namespace NTMiner {
    [MessageType(description: "WebSocket服务已启动")]
    public class WebSocketServerStatedEvent : EventBase {
        public WebSocketServerStatedEvent() { }
    }
}
