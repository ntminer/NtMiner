using NTMiner.ServerNode;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WebApiServerNodeController : ApiControllerBase, IWebApiServerNodeController {
        [Role.Admin]
        [HttpGet]
        [HttpPost]
        public DataResponse<WebApiServerState> GetServerState(object request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<WebApiServerState>>("参数错误");
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
