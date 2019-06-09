using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IWorkerEventTypeSet : IEnumerable<IWorkerEventType> {
        bool Contains(string name);
        bool Contains(Guid id);
        bool TryGetEventType(Guid id, out IWorkerEventType eventType);
    }
}
