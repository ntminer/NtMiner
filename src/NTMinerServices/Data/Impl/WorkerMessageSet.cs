using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTMiner.Core;

namespace NTMiner.Data.Impl {
    public class WorkerMessageSet : IWorkerMessageSet {
        private readonly IHostRoot _root;
        public WorkerMessageSet(IHostRoot root) {
            _root = root;
        }

        public List<WorkerMessageData> GetWorkerMessages(DateTime timestamp) {
            throw new NotImplementedException();
        }
    }
}
