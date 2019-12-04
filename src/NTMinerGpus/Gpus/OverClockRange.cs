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
            return $@"GpuIndexOrBusId={GpuIndexOrBusId.ToString()}
                    CoreClockMin={CoreClockMin.ToString()},CoreClockMax={CoreClockMax.ToString()},CoreClockDelta={CoreClockDelta.ToString()},CoreVoltage={CoreVoltage.ToString()}
                    MemoryClockMin={MemoryClockMin.ToString()},MemoryClockMax={MemoryClockMax.ToString()},MemoryClockDelta={MemoryClockDelta.ToString()},MemoryVoltage={MemoryVoltage.ToString()},
                    VoltMin={VoltMin.ToString()},VoltMax={VoltMax.ToString()},VoltDefault={VoltDefault.ToString()}
                    PowerMin={PowerMin.ToString()},PowerMax={PowerMax.ToString()},PowerCurr={PowerCurr.ToString()},PowerDefault={PowerDefault.ToString()}
                    TempLimitMin={TempLimitMin.ToString()},TempLimitMax={TempLimitMax.ToString()},TempCurr={TempCurr.ToString()},TempLimitDefault={TempLimitDefault.ToString()}
                    FanSpeedMin={FanSpeedMin.ToString()},FanSpeedMax={FanSpeedMax.ToString()},FanSpeedCurr={FanSpeedCurr.ToString()}";
        }
    }
}
