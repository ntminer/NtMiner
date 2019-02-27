namespace NTMiner.Core.Gpus.Impl {
    public class AMDOverClock : IOverClock {
        public AMDOverClock() {
        }

        public void SetCoreClock(IGpuOverClockData data) {
            // 暂不支持A卡超频
        }

        public void SetMemoryClock(IGpuOverClockData data) {
            // 暂不支持A卡超频
        }

        public void SetPowerCapacity(IGpuOverClockData data) {
            int value = data.PowerCapacity;
            if (value == 0) {
                return;
            }
            // 暂不支持A卡超频
        }

        public void SetCool(IGpuOverClockData data) {
            int value = data.Cool;
            if (value == 0) {
                return;
            }
            // 暂不支持A卡超频
        }
    }
}
