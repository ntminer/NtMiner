using NTMiner.ServerNode;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class RemoteIpController : ApiControllerBase, IRemoteIpController {
        [Role.Admin]
        [HttpPost]
        public TopNRemoteIpsResponse TopNRemoteIps([FromBody] DataRequest<int> request) {
            if (request == null || request.Data <= 0) {
                return ResponseBase.InvalidInput<TopNRemoteIpsResponse>("参数错误");
            }
            var data = ServerRoot.RemoteIpSet.GetTopNRemoteIpEntries(request.Data).Select(a => a.ToDto()).ToList();
            return TopNRemoteIpsResponse.Ok(data, ServerRoot.RemoteIpSet.Count);
        }
    }
}
