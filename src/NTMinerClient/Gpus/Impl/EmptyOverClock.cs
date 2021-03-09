namespace NTMiner.Gpus.Impl {
    public class EmptyOverClock : IOverClock {
        public void RefreshGpuState(int gpuIndex) {
            // nothing need todo
        }

        public void OverClock(int gpuIndex, OverClockValue value) {
            // nothing need todo
        }

        public void SetFanSpeed(int gpuIndex, int value) {
            // nothing need todo
        }

        public void Restore() {
            // nothing need todo
        }
    }
}
