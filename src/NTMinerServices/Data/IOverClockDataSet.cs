using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IOverClockDataSet {
        void AddOrUpdate(OverClockData data);
        void Remove(Guid id);
        List<OverClockData> GetAll();
    }
}
