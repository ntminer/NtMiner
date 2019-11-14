using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public interface IOverClockDataSet : IEnumerable<IOverClockData> {
        bool TryGetOverClockData(Guid id, out IOverClockData data);
    }
}
