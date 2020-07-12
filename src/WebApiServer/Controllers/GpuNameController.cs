using NTMiner.Gpus;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class GpuNameController : ApiControllerBase, IGpuNameController {
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
    }
}
