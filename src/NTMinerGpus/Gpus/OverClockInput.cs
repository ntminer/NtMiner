namespace NTMiner.Gpus {
    public class OverClockInput {
        public OverClockInput(IGpu gpu) {
            this.Gpu = gpu;
        }
        public IGpu Gpu { get; private set; } 
        public int CoreClockMHz { get; set; } 
        public int CoreClockVoltage { get; set; }
        public bool IsResetCoreClock {
            get {
                return CoreClockMHz == 0 && CoreClockVoltage == 0;
            }
        }
        public int MemoryClockMHz { get; set; }
        public int MemoryClockVoltage { get; set; }
        public bool IsResetMemoryClock {
            get {
                return MemoryClockMHz == 0 && MemoryClockVoltage == 0;
            }
        }
        public int PowerLimit { get; set; }
        public bool IsResetPowerLimit {
            get {
                return PowerLimit == 0;
            }
        }
        public int TempLimit { get; set; }
        public bool IsResetTempLimit {
            get {
                return TempLimit == 0;
            }
        }
        public int FanSpeed { get; set; }
        public bool FanIsAutoMode {
            get {
                return FanSpeed == 0;
            }
        }
    }
}
