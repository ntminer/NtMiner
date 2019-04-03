using NTMiner.Core.Gpus;
using NTMiner.MinerClient;
using System;

namespace NTMiner.Core.Profiles.Impl {
    public static class GpuProfileExtension {
        public static void OverClock(this IGpuProfile data, IOverClock overClock) {
            overClock.SetCoreClock(data.Index, data.CoreClockDelta);
            overClock.SetMemoryClock(data.Index, data.MemoryClockDelta);
            overClock.SetPowerCapacity(data.Index, data.PowerCapacity);
            overClock.SetThermCapacity(data.Index, data.TempLimit);
            overClock.SetCool(data.Index, data.Cool);
            TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                overClock.RefreshGpuState(data.Index);
            });
        }
    }
}
