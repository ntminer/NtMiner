using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IWorkerMessageSet {
        List<WorkerMessageData> GetWorkerMessages(DateTime timestamp);
    }
}
