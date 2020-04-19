using NTMiner.Hub;

namespace NTMiner {
    [MessageType(description: "打扫时间到，保持清洁")]
    public class CleanTimeArrivedEvent : EventBase {
        /// <summary>
        /// seconds传0表示不需要清理death session
        /// </summary>
        /// <param name="nodeAddresses"></param>
        public CleanTimeArrivedEvent(string[] nodeAddresses) {
            this.NodeAddresses = nodeAddresses;
        }

        public string[] NodeAddresses { get; private set; }
    }

    [MessageType(description: "WebSocket服务已启动")]
    public class WebSocketServerStatedEvent : EventBase {
        public WebSocketServerStatedEvent() { }
    }
}
