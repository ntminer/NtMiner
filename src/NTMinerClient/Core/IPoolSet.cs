using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IPoolSet {
        bool Contains(Guid poolId);
        bool TryGetPool(Guid poolId, out IPool pool);
        string GetPoolDelayText(Guid poolId, bool isDual);
        IEnumerable<IPool> AsEnumerable();
    }
}
