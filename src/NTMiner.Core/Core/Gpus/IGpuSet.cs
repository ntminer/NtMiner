using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public interface IGpuSet : IEnumerable<IGpu> {
        GpuType GpuType { get; }
        IOverClock OverClock { get; }
        int Count { get; }

        bool TryGetGpu(int index, out IGpu gpu);

        List<GpuSetProperty> Properties { get; }

        string GetProperty(string key);

        void LoadGpuState();
    }
}
