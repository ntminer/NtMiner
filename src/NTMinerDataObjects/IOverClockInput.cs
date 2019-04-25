namespace NTMiner {
    public interface IOverClockInput {
        int CoreClockDelta { get; set; }

        int MemoryClockDelta { get; set; }

        /// <summary>
        /// 又名PowerLimit
        /// </summary>
        int PowerCapacity { get; set; }

        int TempLimit { get; set; }

        int Cool { get; set; }
    }
}
