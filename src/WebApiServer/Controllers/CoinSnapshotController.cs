using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class CoinSnapshotController : ApiControllerBase, ICoinSnapshotController {
        #region LatestSnapshots
        [HttpPost]
        public GetCoinSnapshotsResponse LatestSnapshots([FromBody]GetCoinSnapshotsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<GetCoinSnapshotsResponse>("参数错误");
            }
            try {
                if (!IsValidUser(request, out GetCoinSnapshotsResponse response, out UserData user)) {
                    return response;
                }
                List<CoinSnapshotData> data = WebApiRoot.CoinSnapshotSet.GetLatestSnapshots(
                    request.Limit,
                    out int totalMiningCount,
                    out int totalOnlineCount) ?? new List<CoinSnapshotData>();
                return GetCoinSnapshotsResponse.Ok(data, totalMiningCount, totalOnlineCount);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<GetCoinSnapshotsResponse>(e.Message);
            }
        }
        #endregion
    }
}
