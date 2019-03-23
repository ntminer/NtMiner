using System;
using System.Text;

namespace NTMiner.Daemon {
    public class WorkRequest : RequestBase, IGetSignData {
        public WorkRequest() { }

        public Guid WorkId { get; set; }
        public string LocalJson { get; set; }
        public string ServerJson { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(WorkId)).Append(WorkId)
                .Append(nameof(LocalJson)).Append(LocalJson)
                .Append(nameof(ServerJson)).Append(ServerJson)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
