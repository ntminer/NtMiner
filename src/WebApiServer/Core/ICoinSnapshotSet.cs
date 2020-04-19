using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Core {
    /// <summary>
    /// 只为管理员提供面向全部矿机统计的服务，部位普通用户提供面向特定用户的服务。
    /// </summary>
    public interface ICoinSnapshotSet {
        List<CoinSnapshotData> GetLatestSnapshots(int limit, out int totalMiningCount, out int totalOnlineCount);
        List<CoinSnapshotData> GetLatestSnapshots(out int totalMiningCount, out int totalOnlineCount);
    }
}
