namespace NTMiner.Core.Gpus.Impl {
    public class EmptyOverClock : IOverClock {
        public void RefreshGpuState(int gpuIndex) {
            // noting need todo
        }

        public void SetCool(int gpuIndex, int value) {
            // noting need todo
        }

        public void SetThermCapacity(int gpuIndex, int value) {
            // noting need todo
        }

        public void SetCoreClock(int gpuIndex, int value) {
            // noting need todo
        }

        public void SetMemoryClock(int gpuIndex, int value) {
            // noting need todo
        }

        public void SetPowerCapacity(int gpuIndex, int value) {
            // noting need todo
        }

        public void Restore() {
            // nothing need todo
        }
    }
}
