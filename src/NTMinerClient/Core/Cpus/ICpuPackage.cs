using System;

namespace NTMiner.Core.Cpus {
    /// <summary>
    /// 指的是整个CPU包，而不是单个CPU核。
    /// </summary>
    public interface ICpuPackage {
        void Start();
        int Performance { get; }
        int Temperature { get; }

        int HighCpuPercent { get; }
        int HighCpuSeconds { get; }
        DateTime LowCpuOn { get; }
        void ResetCpu(int highCpuPercent, int highCpuSeconds);

        DateTime HighTemperatureOn { get; set; }
        DateTime LowTemperatureOn { get; set; }
    }
}
