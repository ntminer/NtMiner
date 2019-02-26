namespace NTMiner.Core.Gpus.Impl {
    public class GpuAllOverClock : IOverClock {
        public GpuAllOverClock() {
        }

        public void SetCool(IGpuOverClockData data) {
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                gpu.OverClock.SetCool(data);
            }
        }

        public void SetCoreClock(IGpuOverClockData data) {
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                gpu.OverClock.SetCoreClock(data);
            }
        }

        public void SetMemoryClock(IGpuOverClockData data) {
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                gpu.OverClock.SetMemoryClock(data);
            }
        }

        public void SetPowerCapacity(IGpuOverClockData data) {
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                gpu.OverClock.SetPowerCapacity(data);
            }
        }
    }
}
