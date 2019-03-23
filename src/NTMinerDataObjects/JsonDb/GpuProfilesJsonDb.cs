using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.JsonDb {
    public class GpuProfilesJsonDb {
        public List<GpuProfileData> GpuProfiles { get; set; }
        public List<CoinOverClockData> CoinOverClocks { get; set; }

        public GpuProfilesJsonDb() {
            GpuProfiles = new List<GpuProfileData>();
            CoinOverClocks = new List<CoinOverClockData>();
        }
    }
}
