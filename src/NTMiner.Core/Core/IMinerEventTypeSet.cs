using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IMinerEventTypeSet : IEnumerable<IMinerEventType> {
        bool Contains(string name);
        bool Contains(Guid id);
        bool TryGetEventType(Guid id, out IMinerEventType eventType);
    }
}
