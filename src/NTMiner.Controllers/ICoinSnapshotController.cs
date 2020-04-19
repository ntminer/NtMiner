using NTMiner.Core.MinerServer;

namespace NTMiner.Controllers {
    public interface ICoinSnapshotController {
        GetCoinSnapshotsResponse LatestSnapshots(GetCoinSnapshotsRequest request);
    }
}
