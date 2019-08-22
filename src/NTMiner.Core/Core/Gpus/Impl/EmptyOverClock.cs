namespace NTMiner.Core.Gpus.Impl {
    public class EmptyOverClock : IOverClock {
        public void RefreshGpuState(int gpuIndex) {
            // noting need todo
        }

        public void SetFanSpeed(int gpuIndex, int value) {
            // noting need todo
        }

        public void SetTempLimit(int gpuIndex, int value) {
            // noting need todo
        }

        public void SetCoreClock(int gpuIndex, int value, int voltage) {
            // noting need todo
        }

        public void SetMemoryClock(int gpuIndex, int value, int voltage) {
            // noting need todo
        }

        public void SetPowerLimit(int gpuIndex, int value) {
            // noting need todo
        }

        public void Restore() {
            // nothing need todo
        }
    }
}
