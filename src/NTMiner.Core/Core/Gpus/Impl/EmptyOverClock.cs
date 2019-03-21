using NTMiner.MinerClient;

namespace NTMiner.Core.Gpus.Impl {
    public class EmptyOverClock : IOverClock {
        public void RefreshGpuState(int gpuIndex) {
            // noting need todo
        }

        public void SetCool(IGpuProfile data) {
            // noting need todo
        }

        public void SetCoreClock(IGpuProfile data) {
            // noting need todo
        }

        public void SetMemoryClock(IGpuProfile data) {
            // noting need todo
        }

        public void SetPowerCapacity(IGpuProfile data) {
            // noting need todo
        }
    }
}
