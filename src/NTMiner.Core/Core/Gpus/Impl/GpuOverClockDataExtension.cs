namespace NTMiner.Core.Gpus.Impl {
    public static class GpuOverClockDataExtension {
        public static void OverClock(this IGpuOverClockData data, IOverClock overClock) {
            overClock.SetCoreClock(data);
            overClock.SetMemoryClock(data);
            overClock.SetPowerCapacity(data);
            overClock.SetCool(data);
        }
    }
}
