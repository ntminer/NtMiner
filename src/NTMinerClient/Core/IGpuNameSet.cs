using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IGpuNameSet {
        IEnumerable<IGpuName> AsEnumerable();
    }
}
