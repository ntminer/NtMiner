using NTMiner.ServiceContracts.ControlCenter.DataObjects;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IMinerGroupSet : IEnumerable<IMinerGroup> {
        int Count { get; }
        bool Contains(Guid id);
        bool TryGetCoin(Guid id, out IMinerGroup group);
    }
}
