namespace NTMiner.Core {
    public interface IOverClockInput : ICanUpdateByReflection {
        int CoreClockDelta { get; set; }

        int MemoryClockDelta { get; set; }

        int CoreVoltage { get; set; }

        int MemoryVoltage { get; set; }

        /// <summary>
        /// 又名PowerLimit
        /// </summary>
        int PowerCapacity { get; set; }

        int TempLimit { get; set; }

        int Cool { get; }
    }
}
