namespace NTMiner.MinerClient {
    public interface IGpu : IGpuStaticData {
        uint Temperature { get; set; }
        uint FanSpeed { get; set; }
        uint PowerUsage { get; set; }

        // 以下是超频信息
        int CoreClockDelta { get; set; }
        int MemoryClockDelta { get; set; }
        int Cool { get; set; }
        double Power { get; set; }
        int TempLimit { get; set; }
    }
}
