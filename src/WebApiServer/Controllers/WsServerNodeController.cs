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
        public DataResponse<List<WsServerNodeState>> Nodes([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<List<WsServerNodeState>>>("参数错误");
            }
            return new DataResponse<List<WsServerNodeState>> {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = WebApiRoot.WsServerNodeSet.AsEnumerable().ToList()
            };
        }

        [Role.Admin]
        [HttpPost]
        public DataResponse<string[]> NodeAddresses([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput<DataResponse<string[]>>("参数错误");
            }
            return new DataResponse<string[]> {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = WebApiRoot.WsServerNodeSet.AsEnumerable().Select(a => a.Address).ToArray()
            };
        }

        // 这是矿机根据自己的ClientId获取Ws服务器地址的接口
        [HttpPost]
        public DataResponse<string> GetNodeAddress([FromBody]GetNodeAddressRequest request) {
            if (request == null || request.ClientId == Guid.Empty || string.IsNullOrEmpty(request.UserId)) {
                return ResponseBase.InvalidInput<DataResponse<string>>("参数错误");
            }
            try {
                var user = WebApiRoot.UserSet.GetUser(UserId.Create(request.UserId));
                if (user == null) {
                    return ResponseBase.InvalidInput<DataResponse<string>>("用戶不存在");
                }
                return DataResponse<string>.Ok(WebApiRoot.WsServerNodeSet.GetTargetNode(request.ClientId));
            }
            catch (Exception e) {
                return ResponseBase.ServerError<DataResponse<string>>(e.Message);
            }
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase ReportNodeState([FromBody]WsServerNodeState state) {
            if (state == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                WebApiRoot.WsServerNodeSet.SetNodeState(state);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                return ResponseBase.ServerError(e.Message);
            }
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase RemoveNode([FromBody]DataRequest<string> request) {
            if (request == null || string.IsNullOrEmpty(request.Data)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                WebApiRoot.WsServerNodeSet.RemoveNode(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
