namespace NTMiner.Core.Gpus {
    public interface IGpu {
        IOverClock OverClock { get; }
        int Index { get; }
        string Name { get; }
        uint Temperature { get; set; }
        uint FanSpeed { get; set; }
        uint PowerUsage { get; set; }     
        int CoreClockDeltaMin { get; set; }
        int CoreClockDeltaMax { get; set; }
        int MemoryClockDeltaMin { get; set; }
        int MemoryClockDeltaMax { get; set; }
    }
}
