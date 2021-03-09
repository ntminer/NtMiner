namespace NTMiner.Gpus {
    public interface IGpuHelper {
        OverClockRange GetClockRange(IGpu gpu);
        void SetFanSpeed(IGpu gpu, int value);
        void OverClock(IGpu gpu, OverClockValue value);
    }
}
