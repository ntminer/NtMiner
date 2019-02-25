namespace NTMiner.Core.Gpus {
    public interface IGpuOverClockData {
        int Index { get; }
        string Name { get; }

        int CoreClockDelta { get; }

        int MemoryClockDelta { get; }

        int PowerCapacity { get; }

        int Cool { get; }
    }
}
