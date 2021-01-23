using NTMiner.ServerNode;
using NTMiner.User;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WsServerNodeController : ApiControllerBase, IWsServerNodeController {
        // 这是矿机根据自己的ClientId获取Ws服务器地址的接口
        [Role.Public]
        [HttpPost]
        public DataResponse<string> GetNodeAddress([FromBody]GetWsServerNodeAddressRequest request) {
            if (request == null || request.ClientId == Guid.Empty || string.IsNullOrEmpty(request.UserId)) {
                return ResponseBase.InvalidInput<DataResponse<string>>("参数错误");
            }
            try {
                var user = AppRoot.UserSet.GetUser(UserId.Create(request.UserId));
                if (user == null) {
                    return ResponseBase.InvalidInput<DataResponse<string>>("该用戶不存在");
                }
                if (!user.IsEnabled) {
                    return ResponseBase.InvalidInput<DataResponse<string>>("该用户已被禁用");
                }
                return DataResponse<string>.Ok(AppRoot.WsServerNodeAddressSet.GetTargetNode(request.ClientId));
            }
            catch (Exception e) {
                return ResponseBase.ServerError<DataResponse<string>>(e.Message);
            }
        }
    }
}
