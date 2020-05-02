using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Services.Official {
    public class CoinSnapshotService {
        private readonly string _controllerName = JsonRpcRoot.GetControllerName<ICoinSnapshotController>();

        public CoinSnapshotService() {
        }

        #region GetLatestSnapshotsAsync
        public void GetLatestSnapshotsAsync(int limit, Action<GetCoinSnapshotsResponse, Exception> callback) {
            GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                Limit = limit
            };
            JsonRpcRoot.SignPostAsync(JsonRpcRoot.OfficialServerHost, JsonRpcRoot.OfficialServerPort, _controllerName, nameof(ICoinSnapshotController.LatestSnapshots), data: request, callback);
        }
        #endregion
    }
}
