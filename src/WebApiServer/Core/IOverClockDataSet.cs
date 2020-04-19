using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IOverClockDataSet {
        void AddOrUpdate(OverClockData data);
        void RemoveById(Guid id);
        List<OverClockData> GetAll();
    }
}
