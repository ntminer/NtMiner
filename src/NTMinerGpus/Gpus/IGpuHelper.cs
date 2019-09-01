namespace NTMiner.Gpus {
    public interface IGpuHelper {
        OverClockRange GetClockRange(int busId);
        bool SetCoreClock(int busId, int mHz, int voltage);
        bool SetMemoryClock(int busId, int mHz, int voltage);
        bool SetPowerLimit(int busId, int powerValue);
        bool SetTempLimit(int busId, int value);
        bool SetFanSpeed(int busId, int value, bool isAutoMode);
    }
}
