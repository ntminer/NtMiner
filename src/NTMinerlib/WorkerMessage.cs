using NTMiner.Bus;
using NTMiner.MinerClient;

namespace NTMiner {
    [MessageType(description: "发生了矿机事件")]
    public class WorkerMessageAddedEvent : DomainEvent<IWorkerMessage> {
        public WorkerMessageAddedEvent(IWorkerMessage source) : base(source) {
        }
    }

    [MessageType(description: "挖矿消息集清空后")]
    public class WorkerMessageClearedEvent : EventBase {
        public WorkerMessageClearedEvent() { }
    }
}
