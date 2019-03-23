using NTMiner.MinerClient;

namespace NTMiner.MinerClient {
    public interface IOverClock {
        void SetCoreClock(IGpuProfile data);
        void SetMemoryClock(IGpuProfile data);
        void SetPowerCapacity(IGpuProfile data);
        void SetCool(IGpuProfile data);
        void RefreshGpuState(int gpuIndex);
    }
}
