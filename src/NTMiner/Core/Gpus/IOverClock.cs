namespace NTMiner.Core.Gpus {
    public interface IOverClock {
        void SetCoreClock(int gpuIndex, int value, int voltage);
        void SetMemoryClock(int gpuIndex, int value, int voltage);
        void SetPowerLimit(int gpuIndex, int value);
        void SetTempLimit(int gpuIndex, int value);
        void SetFanSpeed(int gpuIndex, int value);
        void RefreshGpuState(int gpuIndex);
        void Restore();
    }
}
