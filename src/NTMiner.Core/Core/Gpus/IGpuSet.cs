using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public interface IGpuSet : IEnumerable<IGpu> {
        GpuType GpuType { get; }
        /// <summary>
        /// NVIDIA的驱动版本号形如399.24，AMD的驱动版本号形如23.20.15025.1004
        /// </summary>
        string DriverVersion { get; }
        IOverClock OverClock { get; }
        int Count { get; }

        bool TryGetGpu(int index, out IGpu gpu);

        List<GpuSetProperty> Properties { get; }

        string GetProperty(string key);

        void LoadGpuState();
    }
}
