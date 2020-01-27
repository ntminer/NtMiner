using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class OverClockDataController : ApiControllerBase, IOverClockDataController {
        #region AddOrUpdateOverClockData
        [HttpPost]
        public ResponseBase AddOrUpdateOverClockData([FromBody]DataRequest<OverClockData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.OverClockDataSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveOverClockData
        [HttpPost]
        public ResponseBase RemoveOverClockData([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.OverClockDataSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region OverClockDatas
        [HttpPost]
        public DataResponse<List<OverClockData>> OverClockDatas([FromBody]OverClockDatasRequest request) {
            try {
                var data = HostRoot.Instance.OverClockDataSet.GetAll();
                return DataResponse<List<OverClockData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<OverClockData>>>(e.Message);
            }
        }
        #endregion
    }
}
