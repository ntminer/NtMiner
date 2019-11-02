using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public class WorkerMessageController : ApiControllerBase, IWorkerMessageController {
        public DataResponse<List<WorkerMessageData>> WorkerMessages(WorkerMessagesRequest request) {
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
