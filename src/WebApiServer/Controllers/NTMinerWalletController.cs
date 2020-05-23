using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class NTMinerWalletController : ApiControllerBase, INTMinerWalletController {
        #region AddOrUpdateNTMinerWallet
        [Role.Admin]
        [HttpPost]
        public ResponseBase AddOrUpdateNTMinerWallet([FromBody]DataRequest<NTMinerWalletData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                WebApiRoot.NTMinerWalletSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveNTMinerWallet
        [Role.Admin]
        [HttpPost]
        public ResponseBase RemoveNTMinerWallet([FromBody]DataRequest<Guid> request) { 
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                WebApiRoot.NTMinerWalletSet.RemoveById(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region NTMinerWallets
        [Role.Public]
        [HttpPost]
        [HttpGet]
        public DataResponse<List<NTMinerWalletData>> NTMinerWallets() {
            try {
                var data = WebApiRoot.NTMinerWalletSet.GetAll();
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
