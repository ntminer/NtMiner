using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTMiner.Core;

namespace NTMiner.Controllers {
    public class WorkerMessageController : ApiControllerBase, IWorkerMessageController {
        public DataResponse<List<WorkerMessageData>> WorkerMessages(WorkerMessagesRequest request) {
            throw new NotImplementedException();
        }
    }
}
