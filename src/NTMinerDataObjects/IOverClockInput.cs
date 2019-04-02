namespace NTMiner {
    public interface IOverClockInput {
        int CoreClockDelta { get; }

        int MemoryClockDelta { get; }

        int PowerCapacity { get; }

        int ThermCapacity { get; }

        int ThermGuard { get; }

        int Cool { get; }
    }
}
