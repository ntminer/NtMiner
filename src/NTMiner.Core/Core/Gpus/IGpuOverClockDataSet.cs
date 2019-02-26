using System;

namespace NTMiner.Core.Gpus {
    public interface IGpuOverClockDataSet {
        IGpuOverClockData GetGpuOverClockData(Guid coinId, int index);
    }
}
