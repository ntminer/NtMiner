using NTMiner.ServerNode;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class AdminController : ApiControllerBase, IAdminController {
        [Role.Admin]
        [HttpPost]
        public ResponseBase SetClientTestId(DataRequest<ClientTestIdData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                AppRoot.ClientTestIdDataRedis.SetAsync(request.Data).ContinueWith(t => {
                    AppRoot.AdminMqSender.SendRefreshMinerTestId();
                });
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
