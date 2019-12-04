using System;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public interface IGpuSet {
        GpuType GpuType { get; }
        /// <summary>
        /// NVIDIA的驱动版本号形如399.24，AMD的驱动版本号形如18.6.1
        /// 都是正常格式的版本号
        /// </summary>
        Version DriverVersion { get; }
        IOverClock OverClock { get; }
        int Count { get; }

        bool TryGetGpu(int index, out IGpu gpu);

        List<GpuSetProperty> Properties { get; }

        void LoadGpuState();
        void LoadGpuState(int gpuIndex);

        IEnumerable<IGpu> AsEnumerable();
    }
}
