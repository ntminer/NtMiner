using NTMiner.Core.Gpus;
using System;

namespace NTMiner.Core.Profiles.Impl {
    public static class GpuProfileExtension {
        public static void OverClock(this IGpuProfile data, IOverClock overClock) {
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
