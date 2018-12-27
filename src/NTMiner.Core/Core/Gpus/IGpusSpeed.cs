using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public interface IGpusSpeed : IEnumerable<IGpuSpeed> {
        IGpuSpeed CurrentSpeed(int gpuIndex);
        List<IGpuSpeed> GetGpuSpeedHistory(int index);
    }
}
