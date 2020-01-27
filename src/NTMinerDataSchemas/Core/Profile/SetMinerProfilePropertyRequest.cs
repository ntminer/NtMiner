using System;
using System.Text;

namespace NTMiner.Core.Profile {
    public class SetMinerProfilePropertyRequest : RequestBase, IGetSignData {
        public SetMinerProfilePropertyRequest() { }
        public Guid WorkId { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
