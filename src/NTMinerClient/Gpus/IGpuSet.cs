using System;
using System.Collections.Generic;

namespace NTMiner.Gpus {
    public interface IGpuSet : ICountSet {
        GpuType GpuType { get; }
        /// <summary>
        /// NVIDIA的驱动版本号形如399.24，AMD的驱动版本号形如18.6.1
        /// 都是正常格式的版本号
        /// </summary>
        string DriverVersion { get; }
        bool IsLowDriverVersion { get; }
        IOverClock OverClock { get; }


        DateTime HighTemperatureOn { get; set; }
        DateTime LowTemperatureOn { get; set; }
        bool TryGetGpu(int index, out IGpu gpu);

        List<GpuSetProperty> Properties { get; }

        void LoadGpuState();
        void LoadGpuState(int gpuIndex);

        IEnumerable<IGpu> AsEnumerable();
    }
}
