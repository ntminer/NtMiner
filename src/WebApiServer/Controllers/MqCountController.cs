using NTMiner.ServerNode;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class MqCountController : ApiControllerBase, IMqCountController {
        [Role.Admin]
        [HttpPost]
        public DataResponse<MqCountData[]> MqCounts() {
            var data = AppRoot.MqCountSet.GetAll();
            return DataResponse<MqCountData[]>.Ok(data);
        }

        [Role.Admin]
        [HttpPost]
        public DataResponse<MqCountData> MqCount(DataRequest<string> request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput<DataResponse<MqCountData>>("参数错误");
            }
            MqCountData data = AppRoot.MqCountSet.GetByAppId(request.Data);
            return DataResponse<MqCountData>.Ok(data);
        }
    }
}
