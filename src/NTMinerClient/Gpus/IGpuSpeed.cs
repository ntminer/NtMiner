using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.Gpus.Impl;
using System;

namespace NTMiner.Gpus {
    public interface IGpuSpeed {
        DateTime FoundShareOn { get; }
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
