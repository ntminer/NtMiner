using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IWorkerEventSet {
        IEnumerable<IWorkerEvent> GetEvents(Guid typeId, string keyword);
    }
}
