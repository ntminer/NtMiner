namespace NTMiner.Gpus {
    public interface IGpuHelper {
        OverClockRange GetClockRange(int overClockGpuId);
        bool SetCoreClock(int overClockGpuId, int mHz, int voltage);
        bool SetMemoryClock(int overClockGpuId, int mHz, int voltage);
        bool SetPowerLimit(int overClockGpuId, int powerValue);
        bool SetTempLimit(int overClockGpuId, int value);
        bool SetFanSpeed(int overClockGpuId, int value, bool isAutoMode);
    }
}
