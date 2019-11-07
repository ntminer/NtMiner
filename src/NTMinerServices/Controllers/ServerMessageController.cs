using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public class ServerMessageController : ApiControllerBase, IServerMessageController {
        public DataResponse<List<ServerMessageData>> ServerMessages(ServerMessagesRequest request) {
            try {
                var data = HostRoot.Instance.ServerMessageSet.GetServerMessages(NTMiner.Timestamp.FromTimestamp(request.Timestamp));
                return DataResponse<List<ServerMessageData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<ServerMessageData>>>(e.Message);
            }
        }

        public ResponseBase AddOrUpdateServerMessage(DataRequest<ServerMessageData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                VirtualRoot.Execute(new AddOrUpdateServerMessageCommand(request.Data));
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        public ResponseBase MarkDeleteServerMessage(DataRequest<Guid> request) {
            if (request == null || request.Data == Guid.Empty) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                VirtualRoot.Execute(new MarkDeleteServerMessageCommand(request.Data));
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
