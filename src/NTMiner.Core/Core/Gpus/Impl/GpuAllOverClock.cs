namespace NTMiner.Core.Gpus.Impl {
    public class GpuAllOverClock : IOverClock {
        public GpuAllOverClock() {
        }

        public void SetCool(IGpuOverClockData data) {
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                if (gpu.Index == NTMinerRoot.GpuAllId) {
                    continue;
                }
                gpu.OverClock.SetCool(data);
            }
        }

        public void SetCoreClock(IGpuOverClockData data) {
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                if (gpu.Index == NTMinerRoot.GpuAllId) {
                    continue;
                }
                gpu.OverClock.SetCoreClock(data);
            }
        }

        public void SetMemoryClock(IGpuOverClockData data) {
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                if (gpu.Index == NTMinerRoot.GpuAllId) {
                    continue;
                }
                gpu.OverClock.SetMemoryClock(data);
            }
        }

        public void SetPowerCapacity(IGpuOverClockData data) {
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                if (gpu.Index == NTMinerRoot.GpuAllId) {
                    continue;
                }
                gpu.OverClock.SetPowerCapacity(data);
            }
        }
    }
}
