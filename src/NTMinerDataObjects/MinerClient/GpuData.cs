namespace NTMiner.MinerClient {
    public class GpuData {
        public GpuData() { }

        public int Index { get; set; }
        public string Name { get; set; }

        public int CoreClockDeltaMin { get; set; }

        public int CoreClockDeltaMax { get; set; }

        public int MemoryClockDeltaMin { get; set; }

        public int MemoryClockDeltaMax { get; set; }
    }
}
