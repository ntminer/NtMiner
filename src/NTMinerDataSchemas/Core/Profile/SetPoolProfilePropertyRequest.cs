using System;
using System.Text;

namespace NTMiner.Core.Profile {
    public class SetPoolProfilePropertyRequest : RequestBase, IGetSignData {
        public SetPoolProfilePropertyRequest() { }
        public Guid WorkId { get; set; }
        public Guid PoolId { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
