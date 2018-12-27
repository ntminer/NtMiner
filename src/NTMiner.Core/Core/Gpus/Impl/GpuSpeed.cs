using NTMiner.Core.Impl;

namespace NTMiner.Core.Gpus.Impl {
    internal class GpuSpeed : IGpuSpeed {
        public static readonly GpuSpeed Empty = new GpuSpeed(Impl.Gpu.Total) {
            MainCoinSpeed = new Speed(),
            DualCoinSpeed = new Speed()
        };

        public GpuSpeed(IGpu gpu) {
            this.Gpu = gpu;
        }

        public IGpu Gpu { get; private set; }

        public ISpeed MainCoinSpeed { get; set; }

        public ISpeed DualCoinSpeed { get; set; }

        public void Update(IGpuSpeed data) {
            this.MainCoinSpeed = data.MainCoinSpeed;
            this.DualCoinSpeed = data.DualCoinSpeed;
        }
    }
}
