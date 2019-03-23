using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.JsonDb {
    public class GpuProfilesJson {
        public List<GpuProfileData> GpuProfiles { get; set; }
        public List<CoinOverClockData> CoinOverClocks { get; set; }

        public GpuProfilesJson() {
            GpuProfiles = new List<GpuProfileData>();
            CoinOverClocks = new List<CoinOverClockData>();
        }
    }
}
