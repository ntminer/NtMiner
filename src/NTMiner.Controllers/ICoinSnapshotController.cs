using NTMiner.Core.MinerServer;

namespace NTMiner.Controllers {
    public interface ICoinSnapshotController {
        /// <summary>
        /// 需签名
        /// </summary>
        GetCoinSnapshotsResponse LatestSnapshots(GetCoinSnapshotsRequest request);
    }
}
