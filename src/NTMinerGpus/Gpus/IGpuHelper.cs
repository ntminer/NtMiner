namespace NTMiner.Gpus {
    public interface IGpuHelper {
        OverClockRange GetClockRange(IGpu gpu);
        void SetFanSpeed(IGpu gpu, int value);
        void OverClock(IGpu gpu, int coreClockMHz, int coreClockVoltage, int memoryClockMHz, int memoryClockVoltage, int powerLimit, int tempLimit, int fanSpeed);
    }
}
