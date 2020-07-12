namespace NTMiner.Gpus {
    public class OverClockRange {
        public OverClockRange(int gpuIndexOrBusId) {
            this.GpuIndexOrBusId = gpuIndexOrBusId;
        }

        /// <summary>
        /// A卡是GpuIndex，N卡是BusId
        /// </summary>
        public int GpuIndexOrBusId { get; private set; }

        public int CoreClockMin { get; set; }
        public int CoreClockMax { get; set; }
        public int CoreClockDelta { get; set; }
        public int CoreVoltage { get; set; }

        public int MemoryClockMin { get; set; }
        public int MemoryClockMax { get; set; }
        public int MemoryClockDelta { get; set; }
        public int MemoryVoltage { get; set; }

        public int VoltMin { get; set; }
        public int VoltMax { get; set; }
        public int VoltDefault { get; set; }

        public int PowerMin { get; set; }
        public int PowerMax { get; set; }
        public int PowerCurr { get; set; }
        public int PowerDefault { get; set; }

        public int TempLimitMin { get; set; }
        public int TempLimitMax { get; set; }
        public int TempCurr { get; set; }
        public int TempLimitDefault { get; set; }

        public int FanSpeedMin { get; set; }
        public int FanSpeedMax { get; set; }
        public int FanSpeedCurr { get; set; }

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }
}
