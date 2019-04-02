namespace NTMiner {
    public interface IOverClockInput {
        int CoreClockDelta { get; }

        int MemoryClockDelta { get; }

        int PowerCapacity { get; }

        int TempLimit { get; }

        int TempGuard { get; }

        int Cool { get; }
    }
}
