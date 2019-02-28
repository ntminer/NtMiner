using System;

namespace NTMiner.Core.Gpus.Impl {
    public static class GpuOverClockDataExtension {
        public static void OverClock(this IGpuOverClockData data, IOverClock overClock) {
            overClock.SetCoreClock(data);
            overClock.SetMemoryClock(data);
            overClock.SetPowerCapacity(data);
            overClock.SetCool(data);
            TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                overClock.RefreshGpuState(data.Index);
            });
        }
    }
}
