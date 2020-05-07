namespace NTMiner.Core.Gpus {
    public interface IGpuStaticData : IGpuName {
        int Index { get; }
        /// <summary>
        /// 必须是数字格式
        /// </summary>
        string BusId { get; }
        int CoreClockDeltaMin { get; set; }
        int CoreClockDeltaMax { get; set; }

        int MemoryClockDeltaMin { get; set; }
        int MemoryClockDeltaMax { get; set; }

        int VoltMin { get; set; }
        int VoltMax { get; set; }
        int VoltDefault { get; set; }

        int CoolMin { get; set; }
        int CoolMax { get; set; }
        double PowerMin { get; set; }
        double PowerMax { get; set; }
        double PowerDefault { get; set; }
        int TempLimitMin { get; set; }
        int TempLimitDefault { get; set; }
        int TempLimitMax { get; set; }
    }
}
