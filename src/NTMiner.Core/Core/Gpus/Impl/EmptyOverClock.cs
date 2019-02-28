namespace NTMiner.Core.Gpus.Impl {
    public class EmptyOverClock : IOverClock {
        public void RefreshGpuState(int gpuIndex) {
            // noting need todo
        }

        public void SetCool(IGpuOverClockData data) {
            // noting need todo
        }

        public void SetCoreClock(IGpuOverClockData data) {
            // noting need todo
        }

        public void SetMemoryClock(IGpuOverClockData data) {
            // noting need todo
        }

        public void SetPowerCapacity(IGpuOverClockData data) {
            // noting need todo
        }
    }
}
