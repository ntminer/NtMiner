using NTMiner.Hub;

namespace NTMiner {
    [MessageType(description: "打扫时间到，保持清洁")]
    public class CleanTimeArrivedEvent : EventBase {
        /// <param name="nodeAddresses">在线的节点</param>
        public CleanTimeArrivedEvent(string[] nodeAddresses) {
            this.NodeAddresses = nodeAddresses;
        }

        /// <summary>
        /// 在线的节点
        /// </summary>
        public string[] NodeAddresses { get; private set; }
    }

    [MessageType(description: "WebSocket服务已启动")]
    public class WebSocketServerStatedEvent : EventBase {
        public WebSocketServerStatedEvent() { }
    }
}
