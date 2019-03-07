using NTMiner.OverClock;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class OverClockDataController : ApiController, IOverClockDataController {
        #region AddOrUpdateOverClockData
        [HttpPost]
        public ResponseBase AddOrUpdateOverClockData([FromBody]AddOrUpdateOverClockDataRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.OverClockDataSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region RemoveOverClockData
        [HttpPost]
        public ResponseBase RemoveOverClockData([FromBody]RemoveOverClockDataRequest request) {
            if (request == null || request.OverClockDataId == Guid.Empty) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.OverClockDataSet.Remove(request.OverClockDataId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }
        #endregion

        #region OverClockDatas
        [HttpPost]
        public GetOverClockDatasResponse OverClockDatas([FromBody]OverClockDatasRequest request) {
            try {
                var data = HostRoot.Current.OverClockDataSet.GetAll();
                return GetOverClockDatasResponse.Ok(request.MessageId, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetOverClockDatasResponse>(request.MessageId, e.Message);
            }
        }
        #endregion
    }
}
