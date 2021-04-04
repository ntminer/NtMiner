using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public class OperationResults {
        public OperationResults() {
            this.Data = new List<OperationResultData>();
        }

        public string LoginName { get; set; }
        public Guid ClientId { get; set; }
        public List<OperationResultData> Data { get; set; }
    }
}
