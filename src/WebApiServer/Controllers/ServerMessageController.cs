using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    // 注意该控制器不能重命名
    public class ServerMessageController : ApiControllerBase, IServerMessageController {
        [Role.Public]
        [HttpPost]
        public DataResponse<List<ServerMessageData>> ServerMessages([FromBody]ServerMessagesRequest request) {
            return DoServerMessages(request);
        }

        internal static DataResponse<List<ServerMessageData>> DoServerMessages(ServerMessagesRequest request) {
            if (request == null ||
                request.Timestamp < 0 /*早于1970年*/ ||
                request.Timestamp > Timestamp.GetTimestamp(DateTime.Now.AddDays(1))/*晚于明天*/) {
                return ResponseBase.InvalidInput<DataResponse<List<ServerMessageData>>>("参数错误");
            }
            try {
                DateTime timestamp = Timestamp.FromTimestamp(request.Timestamp + 1);
                var data = AppRoot.ServerMessageSet.GetServerMessages(timestamp);
                return DataResponse<List<ServerMessageData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<ServerMessageData>>>(e.Message);
            }
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase AddOrUpdateServerMessage([FromBody]DataRequest<ServerMessageData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                VirtualRoot.Execute(new AddOrUpdateServerMessageCommand(request.Data));
                AppRoot.UpdateServerMessageTimestamp();
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [Role.Admin]
        [HttpPost]
        public ResponseBase MarkDeleteServerMessage([FromBody]DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                VirtualRoot.Execute(new MarkDeleteServerMessageCommand(request.Data));
                AppRoot.UpdateServerMessageTimestamp();
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
