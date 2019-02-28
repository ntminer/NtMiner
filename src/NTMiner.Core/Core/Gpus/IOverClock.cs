namespace NTMiner.Core.Gpus {
    public interface IOverClock {
        void SetCoreClock(IGpuOverClockData data);
        void SetMemoryClock(IGpuOverClockData data);
        void SetPowerCapacity(IGpuOverClockData data);
        void SetCool(IGpuOverClockData data);
        void RefreshGpuState(int gpuIndex);
    }
}
