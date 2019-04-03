using System;

namespace NTMiner.MinerClient {
    public class GpuProfileData : IGpuProfile {
        public GpuProfileData() {
        }

        public GpuProfileData(Guid coinId, int index) {
            this.CoinId = coinId;
            this.Index = index;
            this.CoreClockDelta = 0;
            this.MemoryClockDelta = 0;
            this.PowerCapacity = 100;
            this.TempLimit = 83;
            this.IsGuardTemp = false;
            this.TempGuard = 70;
            this.Cool = 90;
        }

        public GpuProfileData(IGpuProfile data) {
            Update(data);
        }

        public void Update(IGpuProfile data) {
            this.CoinId = data.CoinId;
            this.Index = data.Index;
            Update((IOverClockInput)data);
        }

        private void Update(IOverClockInput input) {
            this.CoreClockDelta = input.CoreClockDelta;
            this.MemoryClockDelta = input.MemoryClockDelta;
            this.PowerCapacity = input.PowerCapacity;
            this.TempLimit = input.TempLimit;
            this.IsGuardTemp = input.IsGuardTemp;
            this.TempGuard = input.TempGuard;
            this.Cool = input.Cool;
        }

        public string GetId() {
            return $"{CoinId}_{Index}";
        }

        public Guid CoinId { get; set; }

        public int Index { get; set; }

        public int CoreClockDelta { get; set; }

        public int MemoryClockDelta { get; set; }

        public int PowerCapacity { get; set; }

        public int TempLimit { get; set; }

        public bool IsGuardTemp { get; set; }

        public int TempGuard { get; set; }

        public int Cool { get; set; }

        public override string ToString() {
            return $"{CoinId}{Index}{CoreClockDelta}{MemoryClockDelta}{PowerCapacity}{TempLimit}{IsGuardTemp}{TempGuard}{Cool}";
        }
    }
}
