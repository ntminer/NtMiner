using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface ICoinSnapshotSet {
        List<CoinSnapshotData> GetLatestSnapshots(int limit, out int totalMiningCount, out int totalOnlineCount);
    }
}
