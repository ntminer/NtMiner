using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IEventTypeSet : IEnumerable<IEventType> {
        bool Contains(string name);
        bool Contains(Guid id);
        bool TryGetEventType(Guid id, out IEventType eventType);
    }
}
