namespace NTMiner.Core.Gpus {
    public interface IGpuSpeedData {
        int Index { get; }

        string Name { get; }
        ulong TotalMemory { get; }

        double MainCoinSpeed { get; }

        int FoundShare { get; }

        int AcceptShare { get; }

        int RejectShare { get; }

        int IncorrectShare { get; }

        double DualCoinSpeed { get; }

        int Temperature { get; }

        uint FanSpeed { get; }

        uint PowerUsage { get; }
        int CoreClockDelta { get; }
        int MemoryClockDelta { get; }
        int Cool { get; }
        double PowerCapacity { get; }
        int TempLimit { get; }
        int CoreVoltage { get; }
        int MemoryVoltage { get; }
    }
}
