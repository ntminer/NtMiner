namespace NTMiner.Core.Gpus.Impl {
    internal class Gpu : IGpu {
        public static readonly IGpu Total = new Gpu {
            Index = NTMinerRoot.GpuAllId,
            Name = "全部显卡",
            Temperature = 0,
            FanSpeed = 0,
            PowerUsage = 0,
            CoreClockDeltaMin = 0,
            CoreClockDeltaMax = 0,
            MemoryClockDeltaMin = 0,
            MemoryClockDeltaMax = 0,
            OverClock = new GpuAllOverClock()
        };

        public Gpu() {
        }

        public IOverClock OverClock { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }

        public uint Temperature { get; set; }

        public uint FanSpeed { get; set; }

        public uint PowerUsage { get; set; }

        public int CoreClockDeltaMin { get; set; }

        public int CoreClockDeltaMax { get; set; }

        public int MemoryClockDeltaMin { get; set; }

        public int MemoryClockDeltaMax { get; set; }
    }
}
