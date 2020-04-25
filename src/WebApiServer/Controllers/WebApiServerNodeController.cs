using NTMiner.ServerNode;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WebApiServerNodeController : ApiControllerBase, IWebApiServerNodeController {
        [HttpGet]
        [HttpPost]
        public DataResponse<WebApiServerState> GetServerState(SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<WebApiServerState>>("参数错误");
            }
            if (!IsValidAdmin(request, out DataResponse<WebApiServerState> response, out _)) {
                return response;
            }
            return new DataResponse<WebApiServerState> {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = WebApiRoot.GetServerState()
            };
        }
    }
}
