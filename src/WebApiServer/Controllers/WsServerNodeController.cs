using NTMiner.ServerNode;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WsServerNodeController : ApiControllerBase, IWsServerNodeController {
        [Role.Admin]
        [HttpPost]
        public DataResponse<List<WsServerNodeState>> Nodes([FromBody]object request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<WsServerNodeState>>>("参数错误");
            }
            var t = WebApiRoot.WsServerNodeRedis.GetAllAsync();
            t.Wait();
            return new DataResponse<List<WsServerNodeState>> {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = t.Result
            };
        }

        [Role.Admin]
        [HttpPost]
        public DataResponse<string[]> NodeAddresses([FromBody]object request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<string[]>>("参数错误");
            }
            return new DataResponse<string[]> {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = WebApiRoot.WsServerNodeAddressSet.AsEnumerable().ToArray()
            };
        }

        // 这是矿机根据自己的ClientId获取Ws服务器地址的接口
        [Role.Public]
        [HttpPost]
        public DataResponse<string> GetNodeAddress([FromBody]GetWsServerNodeAddressRequest request) {
            if (request == null || request.ClientId == Guid.Empty || string.IsNullOrEmpty(request.UserId)) {
                return ResponseBase.InvalidInput<DataResponse<string>>("参数错误");
            }
            try {
                var user = WebApiRoot.UserSet.GetUser(UserId.Create(request.UserId));
                if (user == null) {
                    return ResponseBase.InvalidInput<DataResponse<string>>("用戶不存在");
                }
                return DataResponse<string>.Ok(WebApiRoot.WsServerNodeAddressSet.GetTargetNode(request.ClientId));
            }
            catch (Exception e) {
                return ResponseBase.ServerError<DataResponse<string>>(e.Message);
            }
        }
    }
}
