using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ColumnsShowController : ApiControllerBase, IColumnsShowController {
        #region ColumnsShows
        [HttpPost]
        public DataResponse<List<ColumnsShowData>> ColumnsShows([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<ColumnsShowData>>>("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out DataResponse<List<ColumnsShowData>> response)) {
                    return response;
                }
                var data = HostRoot.Instance.ColumnsShowSet.GetAll();
                return DataResponse<List<ColumnsShowData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<ColumnsShowData>>>(e.Message);
            }
        }
        #endregion

        #region AddOrUpdateColumnsShow
        [HttpPost]
        public ResponseBase AddOrUpdateColumnsShow([FromBody]DataRequest<ColumnsShowData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.ColumnsShowSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveColumnsShow
        [HttpPost]
        public ResponseBase RemoveColumnsShow([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, base.ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.ColumnsShowSet.Remove(request.Data);
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
