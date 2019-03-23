using NTMiner.MinerClient;

namespace NTMiner.Core.Gpus.Impl {
    public class AMDOverClock : IOverClock {
        public AMDOverClock() {
        }

        public void SetCoreClock(IGpuProfile data) {
            // 暂不支持A卡超频
        }

        public void SetMemoryClock(IGpuProfile data) {
            // 暂不支持A卡超频
        }

        public void SetPowerCapacity(IGpuProfile data) {
            int value = data.PowerCapacity;
            if (value == 0) {
                return;
            }
            // 暂不支持A卡超频
        }

        public void SetCool(IGpuProfile data) {
            int value = data.Cool;
            if (value == 0) {
                return;
            }
            // 暂不支持A卡超频
        }

        public void RefreshGpuState(int gpuIndex) {
            
        }
    }
}
