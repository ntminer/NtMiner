using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public interface IGpuClockDeltaSet : IEnumerable<IGpuClockDelta> {
        bool TryGetValue(int gpuIndex, out IGpuClockDelta data);
    }
}
