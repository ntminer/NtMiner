using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.JsonDb {
    public interface IGpuProfilesJsonDb : IJsonDb {
        GpuData[] Gpus { get; }
        List<GpuProfileData> GpuProfiles { get; }
        List<CoinOverClockData> CoinOverClocks { get; }
    }
}
