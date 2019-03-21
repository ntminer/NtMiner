using System.Collections.Generic;

namespace NTMiner.MinerClient {
    public class GpuProfilesJson {
        public List<GpuProfileData> GpuProfiles { get; set; }
        public List<CoinOverClockData> CoinOverClocks { get; set; }

        public GpuProfilesJson() {
            GpuProfiles = new List<GpuProfileData>();
            CoinOverClocks = new List<CoinOverClockData>();
        }
    }
}
