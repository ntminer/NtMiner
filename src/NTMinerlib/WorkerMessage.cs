using NTMiner.Bus;
using NTMiner.MinerClient;

namespace NTMiner {
    [MessageType(description: "发生了矿机事件")]
    public class WorkerMessage : DomainEvent<IWorkerMessage> {
        public WorkerMessage(IWorkerMessage source) : base(source) {
        }
    }
}
