namespace NTMiner.Core.Gpus.Impl {
    public class GpuAllOverClock : IOverClock {
        public GpuAllOverClock() {
        }

        public void SetCool(IGpuOverClockData data) {
            GpuOverClockData input = new GpuOverClockData(data);
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                if (gpu.Index == NTMinerRoot.GpuAllId) {
                    continue;
                }
                input.Index = gpu.Index;
                gpu.OverClock.SetCool(input);
            }
        }

        public void SetCoreClock(IGpuOverClockData data) {
            GpuOverClockData input = new GpuOverClockData(data);
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                if (gpu.Index == NTMinerRoot.GpuAllId) {
                    continue;
                }
                input.Index = gpu.Index;
                gpu.OverClock.SetCoreClock(input);
            }
        }

        public void SetMemoryClock(IGpuOverClockData data) {
            GpuOverClockData input = new GpuOverClockData(data);
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                if (gpu.Index == NTMinerRoot.GpuAllId) {
                    continue;
                }
                input.Index = gpu.Index;
                gpu.OverClock.SetMemoryClock(input);
            }
        }

        public void SetPowerCapacity(IGpuOverClockData data) {
            GpuOverClockData input = new GpuOverClockData(data);
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                if (gpu.Index == NTMinerRoot.GpuAllId) {
                    continue;
                }
                input.Index = gpu.Index;
                gpu.OverClock.SetPowerCapacity(input);
            }
        }
    }
}
