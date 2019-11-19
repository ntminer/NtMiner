using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IPoolSet {
        int Count { get; }
        bool Contains(Guid poolId);
        bool TryGetPool(Guid poolId, out IPool pool);
        string GetPoolDelayText(Guid poolId, bool isDual);
        void SetPoolDelayText(Guid poolId, bool isDual, string delayText);
        IEnumerable<IPool> AsEnumerable();
    }
}
