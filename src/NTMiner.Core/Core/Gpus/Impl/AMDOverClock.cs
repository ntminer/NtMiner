namespace NTMiner.Core.Gpus.Impl {
    public class AMDOverClock : IOverClock {
        private readonly int _gpuIndex;

        public AMDOverClock(int gpuIndex) {
            _gpuIndex = gpuIndex;
        }

        public void SetCoreClock(int deltaValue) {
            // 暂不支持A卡超频
        }

        public void SetMemoryClock(int deltaValue) {
            // 暂不支持A卡超频
        }

        public void SetPowerCapacity(int nn) {
            // 暂不支持A卡超频
        }

        public void SetCool(int nn) {
            // 暂不支持A卡超频
        }
    }
}
