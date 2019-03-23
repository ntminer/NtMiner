using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class OverClockDataController : ApiController, IOverClockDataController {
        private string ClientIp {
            get {
                return Request.GetWebClientIp();
            }
        }

        #region AddOrUpdateOverClockData
        [HttpPost]
        public ResponseBase AddOrUpdateOverClockData([FromBody]DataRequest<OverClockData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Current.OverClockDataSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
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
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Current.OverClockDataSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region OverClockDatas
        [HttpPost]
        public DataResponse<List<OverClockData>> OverClockDatas([FromBody]OverClockDatasRequest request) {
            try {
                var data = HostRoot.Current.OverClockDataSet.GetAll();
                return DataResponse<List<OverClockData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<DataResponse<List<OverClockData>>>(e.Message);
            }
        }
        #endregion
    }
}
