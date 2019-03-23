using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.JsonDb {
    public interface IGpuProfilesJsonDb : IJsonDb {
        List<GpuProfileData> GpuProfiles { get; }
        List<CoinOverClockData> CoinOverClocks { get; }
    }
}
