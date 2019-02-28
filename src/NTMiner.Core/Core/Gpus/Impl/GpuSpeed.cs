using NTMiner.Core.Impl;

namespace NTMiner.Core.Gpus.Impl {
    internal class GpuSpeed : IGpuSpeed {
        public static readonly GpuSpeed Empty = new GpuSpeed(new Gpu {
            Index = NTMinerRoot.GpuAllId,
            Name = "全部显卡",
            Temperature = 0,
            FanSpeed = 0,
            PowerUsage = 0,
            CoreClockDelta = 0,
            MemoryClockDelta = 0,
            GpuClockDelta = new GpuClockDelta(0, 0, 0, 0),
            OverClock = new EmptyOverClock()
        }) {
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
