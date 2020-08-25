using NTMiner.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IGpuNameSet {
        void AddCount(GpuType gpuType, string gpuName, ulong gpuTotalMemory);
        List<GpuNameCount> QueryGpuNameCounts(QueryGpuNameCountsRequest query, out int total);
    }
}
