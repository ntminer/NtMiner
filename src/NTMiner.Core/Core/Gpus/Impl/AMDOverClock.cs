namespace NTMiner.Core.Gpus.Impl {
    public class AMDOverClock : IOverClock {
        public AMDOverClock() {
        }

        public void SetCoreClock(int gpuIndex, int value) {
            // 暂不支持A卡超频
        }

        public void SetMemoryClock(int gpuIndex, int value) {
            // 暂不支持A卡超频
        }

        public void SetPowerCapacity(int gpuIndex, int value) {
            // 暂不支持A卡超频
        }

        public void SetThermCapacity(int gpuIndex, int value) {
            // 暂不支持A卡超频
        }

        public void SetCool(int gpuIndex, int value) {
            // 暂不支持A卡超频
        }

        public void RefreshGpuState(int gpuIndex) {
            // 暂不支持A卡超频
        }
    }
}
