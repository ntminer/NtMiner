using System;

namespace NTMiner.Core.Profile {
    public class GpuProfileData : IGpuProfile {
        public GpuProfileData() {
        }

        public GpuProfileData(Guid coinId, int index) {
            this.CoinId = coinId;
            this.Index = index;
            this.CoreClockDelta = 0;
            this.MemoryClockDelta = 0;
            this.CoreVoltage = 0;
            this.MemoryVoltage = 0;
            this.PowerCapacity = 0;
            this.TempLimit = 0;
            this.IsAutoFanSpeed = false;
            this.Cool = 90;
            this.CurrentMemoryTimingLevel = -1;
        }

        public string GetId() {
            return $"{CoinId.ToString()}_{Index.ToString()}";
        }

        public Guid CoinId { get; set; }

        public int Index { get; set; }

        public int CoreClockDelta { get; set; }

        public int MemoryClockDelta { get; set; }

        public int CoreVoltage { get; set; }
        public int MemoryVoltage { get; set; }

        public int CurrentMemoryTimingLevel { get; set; }

        public int PowerCapacity { get; set; }

        public int TempLimit { get; set; }

        public bool IsAutoFanSpeed { get; set; }

        public int Cool { get; set; }

        public override string ToString() {
            return $"{CoinId.ToString()}{Index.ToString()}{CoreClockDelta.ToString()}{MemoryClockDelta.ToString()}{CoreVoltage.ToString()}{MemoryVoltage.ToString()}{CurrentMemoryTimingLevel.ToString()}{PowerCapacity.ToString()}{TempLimit.ToString()}{IsAutoFanSpeed.ToString()}{Cool.ToString()}";
        }
    }
}
