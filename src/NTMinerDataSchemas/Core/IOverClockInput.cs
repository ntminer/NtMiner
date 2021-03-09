namespace NTMiner.Core {
    // 注意：这个类型中的属性名是不能改名的，因为已经进入服务端和挖矿端的信息交换协议。
    public interface IOverClockInput : ICanUpdateByReflection {
        /// <summary>
        /// 对于N卡来说是Delta，对于A卡来说是绝对值。
        /// </summary>
        int CoreClockDelta { get; set; }

        /// <summary>
        /// 对于N卡来说是Delta，对于A卡来说是绝对值。
        /// </summary>
        int MemoryClockDelta { get; set; }

        /// <summary>
        /// 核心电压
        /// </summary>
        int CoreVoltage { get; set; }

        /// <summary>
        /// 显存电压
        /// </summary>
        int MemoryVoltage { get; set; }

        /// <summary>
        /// 又名PowerLimit
        /// </summary>
        int PowerCapacity { get; set; }

        /// <summary>
        /// 温度阈值
        /// </summary>
        int TempLimit { get; set; }

        /// <summary>
        /// 百分比，又名FanSpeed
        /// </summary>
        int Cool { get; }

        /// <summary>
        /// 显存时序
        /// </summary>
        int CurrentMemoryTimingLevel { get; set; }
    }
}
