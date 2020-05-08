using NTMiner.Core.Gpus;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class GpuNameController : ApiControllerBase, IGpuNameController {
        [HttpGet]
        [HttpPost]
        public QueryGpuNamesResponse QueryGpuNames(QueryGpuNamesRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryGpuNamesResponse>("参数错误");
            }
            request.PagingTrim();
            var data = WebApiRoot.GpuNameSet.QueryGpuNames(request, out int total);

            return QueryGpuNamesResponse.Ok(data, total);
        }

        [Role.Admin]
        [HttpGet]
        [HttpPost]
        public QueryGpuNameCountsResponse QueryGpuNameCounts(QueryGpuNameCountsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryGpuNameCountsResponse>("参数错误");
            }
            request.PagingTrim();
            var data = WebApiRoot.GpuNameSet.QueryGpuNameCounts(request, out int total);

            return QueryGpuNameCountsResponse.Ok(data, total);
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase SetGpuName(DataRequest<GpuName> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            WebApiRoot.GpuNameSet.Set(request.Data);
            return ResponseBase.Ok("设置成功");
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase RemoveGpuName(DataRequest<GpuName> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            WebApiRoot.GpuNameSet.Remove(request.Data);
            return ResponseBase.Ok("移除成功");
        }
    }
}
