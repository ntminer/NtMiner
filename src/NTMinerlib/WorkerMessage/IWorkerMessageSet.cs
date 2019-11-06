using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.WorkerMessage {
    public interface IWorkerMessageSet : IEnumerable<IWorkerMessage> {
        int Count { get; }
        void Add(string channel, string provider, string messageType, string content);
        void Clear();
    }
}
