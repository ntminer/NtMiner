using System;
using System.Text;

namespace NTMiner.Core.Daemon {
    public class WorkRequest : IRequest, ISignableData {
        public WorkRequest() { }

        public Guid WorkId { get; set; }
        public string LocalJson { get; set; }
        public string ServerJson { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
