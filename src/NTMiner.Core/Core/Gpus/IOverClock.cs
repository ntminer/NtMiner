namespace NTMiner.Core.Gpus {
    public interface IOverClock {
        void SetCoreClock(int gpuIndex, int value, int voltage);
        void SetMemoryClock(int gpuIndex, int value, int voltage);
        void SetPowerCapacity(int gpuIndex, int value);
        void SetThermCapacity(int gpuIndex, int value);
        void SetCool(int gpuIndex, int value);
        void RefreshGpuState(int gpuIndex);
        void Restore();
    }
}
