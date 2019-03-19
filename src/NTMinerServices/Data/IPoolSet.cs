using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IPoolSet {
        void AddOrUpdate(PoolData data);
        void Remove(Guid id);
        List<PoolData> GetAll();
    }
}
