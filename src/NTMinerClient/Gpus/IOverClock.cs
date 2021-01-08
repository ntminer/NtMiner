namespace NTMiner.Gpus {
    public interface IOverClock {
        void SetFanSpeed(int gpuIndex, int value);
        void OverClock(int gpuIndex, int coreClockMHz, int coreClockVoltage, int memoryClockMHz, int memoryClockVoltage, int powerLimit, int tempLimit, int fanSpeed);
        void RefreshGpuState(int gpuIndex);
        void Restore();
    }
}
