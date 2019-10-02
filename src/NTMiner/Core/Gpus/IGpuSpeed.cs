using NTMiner.Core.Gpus.Impl;
using NTMiner.Core.Impl;

namespace NTMiner.Core.Gpus {
    public interface IGpuSpeed {
        IGpu Gpu { get; }
        ISpeed MainCoinSpeed { get; }
        ISpeed DualCoinSpeed { get; }
    }

    public static class GpuSpeedExtensions {
        internal static IGpuSpeed Clone(this IGpuSpeed gpuSpeed) {
            return new GpuSpeed(gpuSpeed.Gpu,
                mainCoinSpeed: new Speed(gpuSpeed.MainCoinSpeed),
                dualCoinSpeed: new Speed(gpuSpeed.DualCoinSpeed));
        }
    }
}
