using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class WorkerMessageController : ApiControllerBase, IWorkerMessageController {
        [HttpPost]
        public ResponseBase AddOrUpdateWorkerMessage([FromBody]AddOrUpdateWorkerMessageRequest request) {
            if (request == null || string.IsNullOrEmpty(request.MessageType) || string.IsNullOrEmpty(request.Content)) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.WorkerMessageSet.Add(WorkerMessageChannel.Server.GetName(), nameof(WorkerMessageController), request.MessageType, request.Content);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public DataResponse<List<WorkerMessageData>> WorkerMessages([FromBody]WorkerMessagesRequest request) {
            try {
                DateTime timestamp = NTMiner.Timestamp.FromTimestamp(request.Timestamp);
                var data = HostRoot.Instance.WorkerMessageSet.GetWorkerMessages(timestamp);
                return DataResponse<List<WorkerMessageData>>.Ok(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError<DataResponse<List<WorkerMessageData>>>(e.Message);
            }
        }
    }
}
