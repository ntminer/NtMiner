namespace NTMiner.Core.Gpus {
    public interface IGpuClockDelta {
        int CoreClockDeltaMin { get; }
        int CoreClockDeltaMax { get; }
        int MemoryClockDeltaMin { get; }
        int MemoryClockDeltaMax { get; }
    }
}
