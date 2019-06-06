namespace NTMiner.MinerClient {
    public interface IGpuStaticData {
        int Index { get; }
        string BusId { get; }
        string Name { get; }

        /// <summary>
        /// 单位Mb
        /// </summary>
        ulong TotalMemory { get; set; }

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
