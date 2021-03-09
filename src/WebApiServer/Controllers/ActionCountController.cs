using NTMiner.ServerNode;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ActionCountController : ApiControllerBase, IActionCountController {
        [Role.Admin]
        [HttpPost]
        public QueryActionCountsResponse QueryActionCounts([FromBody] QueryActionCountsRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<QueryActionCountsResponse>("参数错误");
            }
            request.PagingTrim();
            var data = AppRoot.QueryActionCounts(request, out int total);

            return QueryActionCountsResponse.Ok(data, total);
        }
    }
}
