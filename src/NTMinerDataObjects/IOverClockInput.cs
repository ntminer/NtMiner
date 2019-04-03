namespace NTMiner {
    public interface IOverClockInput {
        int CoreClockDelta { get; set; }

        int MemoryClockDelta { get; set; }

        int PowerCapacity { get; set; }

        int TempLimit { get; set; }

        bool IsGuardTemp { get; }

        int GuardTemp { get; }

        int Cool { get; set; }
    }
}
