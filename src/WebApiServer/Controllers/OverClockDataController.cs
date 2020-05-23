using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    // 注意该控制器不能重命名
    public class OverClockDataController : ApiControllerBase, IOverClockDataController {
        #region AddOrUpdateOverClockData
        [Role.Admin]
        [HttpPost]
        public ResponseBase AddOrUpdateOverClockData([FromBody]DataRequest<OverClockData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                WebApiRoot.OverClockDataSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region RemoveOverClockData
        [Role.Admin]
        [HttpPost]
        public ResponseBase RemoveOverClockData([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                WebApiRoot.OverClockDataSet.RemoveById(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion

        #region OverClockDatas
        [Role.Public]
        [HttpPost]
        public DataResponse<List<OverClockData>> OverClockDatas([FromBody]object request) {
            try {
                var data = WebApiRoot.OverClockDataSet.GetAll();
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
