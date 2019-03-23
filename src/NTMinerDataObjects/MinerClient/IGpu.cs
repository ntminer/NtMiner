namespace NTMiner.MinerClient {
    public interface IGpu {
        IOverClock OverClock { get; }
        int Index { get; }
        string Name { get; }
        uint Temperature { get; set; }
        uint FanSpeed { get; set; }
        uint PowerUsage { get; set; }

        // 以下是超频信息
        int CoreClockDelta { get; set; }
        int MemoryClockDelta { get; set; }
        int CoreClockDeltaMin { get; set; }
        int CoreClockDeltaMax { get; set; }
        int MemoryClockDeltaMin { get; set; }
        int MemoryClockDeltaMax { get; set; }
        int Cool { get; set; }
        int CoolMin { get; set; }
        int CoolMax { get; set; }
        double PowerMin { get; set; }
        double PowerMax { get; set; }
        double Power { get; set; }
    }
}
