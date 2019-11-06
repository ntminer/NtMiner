using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WalletController : ApiControllerBase, IWalletController {
        #region Wallets
        [HttpPost]
        public DataResponse<List<WalletData>> Wallets([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<WalletData>>>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<WalletData>> response)) {
                    return response;
                }
                var data = HostRoot.Instance.WalletSet.GetAll();
                return DataResponse<List<WalletData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<WalletData>>>(e.Message);
            }
        }
        #endregion

        #region AddOrUpdateWallet
        [HttpPost]
        public ResponseBase AddOrUpdateWallet([FromBody]DataRequest<WalletData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.WalletSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveWallet
        [HttpPost]
        public ResponseBase RemoveWallet([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.WalletSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion
    }
}
