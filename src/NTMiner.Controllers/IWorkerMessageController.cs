using NTMiner.Core;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IWorkerMessageController {
        DataResponse<List<WorkerMessageData>> WorkerMessages(WorkerMessagesRequest request);
        ResponseBase AddOrUpdateWorkerMessage(AddOrUpdateWorkerMessageRequest request);
    }
}
