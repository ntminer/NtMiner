using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public interface IGpuSet : IEnumerable<IGpu> {
        GpuType GpuType { get; }
        int Count { get; }
        IGpu this[int index] { get; }

        IGpuClockDeltaSet GpuClockDeltaSet { get; }

        bool TryGetGpu(int index, out IGpu gpu);

        List<GpuSetProperty> Properties { get; }
    }
}
