namespace NTMiner.MinerClient {
    public class GpuClockDelta : IGpuClockDelta {
        public static readonly GpuClockDelta Empty = new GpuClockDelta(0, 0, 0, 0);

        public GpuClockDelta(int coreClockDeltaMin, int coreClockDeltaMax, int memoryClockDeltaMin, int memoryClockDeltaMax) {
            this.CoreClockDeltaMin = coreClockDeltaMin;
            this.CoreClockDeltaMax = coreClockDeltaMax;
            this.MemoryClockDeltaMin = memoryClockDeltaMin;
            this.MemoryClockDeltaMax = memoryClockDeltaMax;
        }

        public int CoreClockDeltaMin { get; private set; }
        public int CoreClockDeltaMax { get; private set; }
        public int MemoryClockDeltaMin { get; private set; }
        public int MemoryClockDeltaMax { get; private set; }
    }
}
