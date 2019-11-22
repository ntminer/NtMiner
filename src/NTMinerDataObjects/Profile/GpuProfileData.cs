using System;

namespace NTMiner.Profile {
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
        }

        public GpuProfileData(IGpuProfile data) {
            Update(data);
        }

        public void Update(IGpuProfile data) {
            this.CoinId = data.CoinId;
            this.Index = data.Index;
            this.IsAutoFanSpeed = data.IsAutoFanSpeed;
            Update((IOverClockInput)data);
        }

        private void Update(IOverClockInput input) {
            this.CoreClockDelta = input.CoreClockDelta;
            this.MemoryClockDelta = input.MemoryClockDelta;
            this.CoreVoltage = input.CoreVoltage;
            this.MemoryVoltage = input.MemoryVoltage;
            this.PowerCapacity = input.PowerCapacity;
            this.TempLimit = input.TempLimit;
            this.Cool = input.Cool;
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

        public int PowerCapacity { get; set; }

        public int TempLimit { get; set; }

        public bool IsAutoFanSpeed { get; set; }

        public int Cool { get; set; }

        public override string ToString() {
            return $"{CoinId.ToString()}{Index.ToString()}{CoreClockDelta.ToString()}{MemoryClockDelta.ToString()}{CoreVoltage.ToString()}{MemoryVoltage.ToString()}{PowerCapacity.ToString()}{TempLimit.ToString()}{IsAutoFanSpeed.ToString()}{Cool.ToString()}";
        }
    }
}
