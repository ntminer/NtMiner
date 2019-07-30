using System;
using System.Text;

namespace NTMiner.Daemon {
    public class WorkRequest : RequestBase, IGetSignData {
        public WorkRequest() { }

        public Guid WorkId { get; set; }
        public string LocalJson { get; set; }
        public string ServerJson { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
