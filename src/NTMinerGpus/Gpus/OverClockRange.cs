namespace NTMiner.Gpus {
    public class OverClockRange {
        public OverClockRange(int gpuIndexOrBusId) {
            this.GpuIndexOrBusId = gpuIndexOrBusId;
        }

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
        public int FanSpeedDefault { get; set; }

        public override string ToString() {
            return $@"GpuIndexOrBusId={GpuIndexOrBusId}
CoreClockMin={CoreClockMin},CoreClockMax={CoreClockMax},CoreClockDelta={CoreClockDelta},CoreVoltage={CoreVoltage}
MemoryClockMin={MemoryClockMin},MemoryClockMax={MemoryClockMax},MemoryClockDelta={MemoryClockDelta},MemoryVoltage={MemoryVoltage},
VoltMin={VoltMin},VoltMax={VoltMax},VoltDefault={VoltDefault}
PowerMin={PowerMin},PowerMax={PowerMax},PowerCurr={PowerCurr},PowerDefault={PowerDefault}
TempLimitMin={TempLimitMin},TempLimitMax={TempLimitMax},TempCurr={TempCurr},TempLimitDefault={TempLimitDefault}
FanSpeedMin={FanSpeedMin},FanSpeedMax={FanSpeedMax},FanSpeedCurr={FanSpeedCurr},FanSpeedDefault={FanSpeedDefault}";
        }
    }
}
