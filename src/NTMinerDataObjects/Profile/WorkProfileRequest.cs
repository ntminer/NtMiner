using System;
using System.Text;

namespace NTMiner.Profile {
    public class WorkProfileRequest : RequestBase, IGetSignData {
        public WorkProfileRequest() { }

        public Guid WorkId { get; set; }

        public Guid DataId { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
