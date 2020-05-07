using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IGpuNameSet {
        void Add(GpuType gpuType, string gpuName, ulong gpuTotalMemory);
        IEnumerable<GpuNameCount> AsEnumerable();
    }
}
