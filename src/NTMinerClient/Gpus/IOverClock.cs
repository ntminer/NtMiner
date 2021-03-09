namespace NTMiner.Gpus {
    public interface IOverClock {
        void SetFanSpeed(int gpuIndex, int value);
        void OverClock(int gpuIndex, OverClockValue value);
        void RefreshGpuState(int gpuIndex);
        void Restore();
    }
}
