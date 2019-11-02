using NTMiner.Bus;
using NTMiner.Core;
using System.Collections.Generic;

namespace NTMiner {
    [MessageType(description: "发生了矿机事件")]
    public class WorkerMessageAddedEvent : DomainEvent<IWorkerMessage> {
        public WorkerMessageAddedEvent(IWorkerMessage source, List<IWorkerMessage> removes) : base(source) {
            this.Removes = removes ?? new List<IWorkerMessage>();
        }

        public List<IWorkerMessage> Removes { get; private set; }
    }

    [MessageType(description: "挖矿消息集清空后")]
    public class WorkerMessageClearedEvent : EventBase {
        public WorkerMessageClearedEvent() { }
    }
}
