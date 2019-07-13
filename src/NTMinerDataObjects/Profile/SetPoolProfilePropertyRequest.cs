using System;
using System.Text;

namespace NTMiner.Profile {
    public class SetPoolProfilePropertyRequest : RequestBase, IGetSignData {
        public SetPoolProfilePropertyRequest() { }
        public Guid WorkId { get; set; }
        public Guid PoolId { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(WorkId)).Append(WorkId)
                .Append(nameof(PoolId)).Append(PoolId)
                .Append(nameof(PropertyName)).Append(PropertyName)
                .Append(nameof(Value)).Append(Value)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
