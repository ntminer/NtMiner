namespace NTMiner.MinerClient {
    public interface IGpuStaticData {
        int Index { get; }
        string Name { get; }

        int CoreClockDeltaMin { get; set; }

        int CoreClockDeltaMax { get; set; }

        int MemoryClockDeltaMin { get; set; }

        int MemoryClockDeltaMax { get; set; }
        int CoolMin { get; set; }
        int CoolMax { get; set; }
        double PowerMin { get; set; }
        double PowerMax { get; set; }
        int TempLimitMin { get; set; }
        int TempLimitDefault { get; set; }
        int TempLimitMax { get; set; }
    }
}
