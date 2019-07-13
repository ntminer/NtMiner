using System;
using System.Text;

namespace NTMiner.Profile {
    public class WorkProfileRequest : RequestBase, ISignatureRequest {
        public WorkProfileRequest() { }

        public Guid WorkId { get; set; }

        public Guid DataId { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(WorkId)).Append(WorkId)
                .Append(nameof(DataId)).Append(DataId)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());

            return sb;
        }
    }
}
