namespace NTMiner.Core {
    public interface IOverClockInput : ICanUpdateByReflection {
        /// <summary>
        /// 对于N卡来说是Delta，对于A卡来说是绝对值。
        /// </summary>
        int CoreClockDelta { get; set; }

        /// <summary>
        /// 对于N卡来说是Delta，对于A卡来说是绝对值。
        /// </summary>
        int MemoryClockDelta { get; set; }

        int CoreVoltage { get; set; }

        int MemoryVoltage { get; set; }

        /// <summary>
        /// 又名PowerLimit
        /// </summary>
        int PowerCapacity { get; set; }

        int TempLimit { get; set; }

        /// <summary>
        /// 百分比，FanSpeed
        /// </summary>
        int Cool { get; }
    }
}
