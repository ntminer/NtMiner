using NTMiner.Core.Gpus;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class GpuNameController : ApiControllerBase, IGpuNameController {
        [HttpGet]
        [HttpPost]
        public DataResponse<List<GpuName>> GpuNames([FromBody]object request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<GpuName>>>("参数错误");
            }
            return DataResponse<List<GpuName>>.Ok(WebApiRoot.GpuNameSet.GetAllGpuNames());
        }

        [Role.Admin]
        [HttpGet]
        [HttpPost]
        public QueryGpuNameCountsResponse QueryGpuNameCounts([FromBody]QueryGpuNameCountsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryGpuNameCountsResponse>("参数错误");
            }
            request.PagingTrim();
            var data = WebApiRoot.GpuNameSet.QueryGpuNameCounts(request, out int total);

            return QueryGpuNameCountsResponse.Ok(data, total);
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase SetGpuName([FromBody]DataRequest<GpuName> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            WebApiRoot.GpuNameSet.Set(request.Data);
            return ResponseBase.Ok("设置成功");
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase RemoveGpuName([FromBody]DataRequest<GpuName> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            WebApiRoot.GpuNameSet.Remove(request.Data);
            return ResponseBase.Ok("移除成功");
        }
    }
}
