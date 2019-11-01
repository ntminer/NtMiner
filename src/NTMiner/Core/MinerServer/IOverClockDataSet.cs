using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    // TODO:因为OverClockDataSet的数据源异步来自于服务器，考虑是否应该有个IsReady属性
    public interface IOverClockDataSet : IEnumerable<IOverClockData> {
        bool TryGetOverClockData(Guid id, out IOverClockData data);
    }
}
