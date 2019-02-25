namespace NTMiner.Core.Gpus.Impl {
    public class AMDOverClock : IOverClock {
        public IGpuOverClockData Data { get; private set; }

        public AMDOverClock(IGpuOverClockData data) {
            this.Data = data;
        }

        public void SetCoreClock() {
            // 暂不支持A卡超频
        }

        public void SetMemoryClock() {
            // 暂不支持A卡超频
        }

        public void SetPowerCapacity() {
            // 暂不支持A卡超频
        }

        public void SetCool() {
            // 暂不支持A卡超频
        }
    }
}
