using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IGpuNameSet {
        void Add(GpuName gpuName);
        IEnumerable<IGpuName> AsEnumerable();
    }
}
