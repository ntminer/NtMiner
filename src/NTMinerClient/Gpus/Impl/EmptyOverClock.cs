namespace NTMiner.Gpus.Impl {
    public class EmptyOverClock : IOverClock {
        public void RefreshGpuState(int gpuIndex) {
            // nothing need todo
        }

        public void OverClock(
            int gpuIndex, int coreClockMHz, int coreClockVoltage, int memoryClockMHz, 
            int memoryClockVoltage, int powerLimit, int tempLimit, int fanSpeed) {
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
