using NTMiner.Bus;
using NTMiner.MinerClient;

namespace NTMiner {
    [MessageType(description: "发生了矿机事件")]
    public class WorkerEvent : DomainEvent<IWorkerMessage> {
        public WorkerEvent(IWorkerMessage source) : base(source) {
        }
    }
}
