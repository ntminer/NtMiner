using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class NTMinerWalletController : ApiControllerBase, INTMinerWalletController {
        #region AddOrUpdateNTMinerWallet
        [HttpPost]
        public ResponseBase AddOrUpdateNTMinerWallet([FromBody]DataRequest<NTMinerWalletData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.NTMinerWalletSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveNTMinerWallet
        [HttpPost]
        public ResponseBase RemoveNTMinerWallet([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.NTMinerWalletSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region NTMinerWallets
        [HttpPost]
        public DataResponse<List<NTMinerWalletData>> NTMinerWallets([FromBody]NTMinerWalletsRequest request) {
            try {
                var data = HostRoot.Instance.NTMinerWalletSet.GetAll();
                return DataResponse<List<NTMinerWalletData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<NTMinerWalletData>>>(e.Message);
            }
        }
        #endregion
    }
}
