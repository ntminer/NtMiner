namespace NTMiner.Core.Gpus.Impl {
    internal class Gpu : IGpu {
        public Gpu() {
        }

        public IOverClock OverClock { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }

        public uint Temperature { get; set; }

        public uint FanSpeed { get; set; }

        public uint PowerUsage { get; set; }
        public int CoreClockDelta { get; set; }
        public int MemoryClockDelta { get; set; }

        public GpuClockDelta GpuClockDelta { get; set; }
    }
}
