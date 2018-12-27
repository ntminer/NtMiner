using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public interface IGpuSet : IEnumerable<IGpu> {
        GpuType GpuType { get; }
        int Count { get; }
        IGpu this[int index] { get; }

        List<GpuSetProperty> Properties { get; }
    }
}
