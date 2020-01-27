using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public class ServerMessageController : ApiControllerBase, IServerMessageController {
        public DataResponse<List<ServerMessageData>> ServerMessages(ServerMessagesRequest request) {
            try {
                DateTime timestamp = NTMiner.Timestamp.FromTimestamp(request.Timestamp + 1);
                var data = HostRoot.Instance.ServerMessageSet.GetServerMessages(timestamp);
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
                HostRoot.Instance.UpdateServerMessageTimestamp();
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
                HostRoot.Instance.UpdateServerMessageTimestamp();
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
