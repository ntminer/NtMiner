using NTMiner.Bus;

namespace NTMiner {
    [MessageType(description: "发生了矿机事件")]
    public class WorkerEvent : EventBase {
        public WorkerEvent(WorkerEventChannel Channel, string content) {
            this.Channel = Channel;
            this.Content = content;
        }

        public WorkerEventChannel Channel { get; private set; }
        public string Content { get; private set; }
    }
}
