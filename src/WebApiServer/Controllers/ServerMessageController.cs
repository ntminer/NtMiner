using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    // 注意该控制器不能重命名
    public class ServerMessageController : ApiControllerBase, IServerMessageController {
        [HttpPost]
        public DataResponse<List<ServerMessageData>> ServerMessages([FromBody]ServerMessagesRequest request) {
            try {
                DateTime timestamp = Timestamp.FromTimestamp(request.Timestamp + 1);
                var data = WebApiRoot.ServerMessageSet.GetServerMessages(timestamp);
                return DataResponse<List<ServerMessageData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<ServerMessageData>>>(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase AddOrUpdateServerMessage([FromBody]DataRequest<ServerMessageData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!IsValidAdmin(request, out ResponseBase response, out _)) {
                    return response;
                }
                VirtualRoot.Execute(new AddOrUpdateServerMessageCommand(request.Data));
                WebApiRoot.UpdateServerMessageTimestamp();
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase MarkDeleteServerMessage([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!IsValidAdmin(request, out ResponseBase response, out _)) {
                    return response;
                }
                VirtualRoot.Execute(new MarkDeleteServerMessageCommand(request.Data));
                WebApiRoot.UpdateServerMessageTimestamp();
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
