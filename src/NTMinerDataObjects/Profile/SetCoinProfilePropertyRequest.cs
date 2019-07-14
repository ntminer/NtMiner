using System;
using System.Text;

namespace NTMiner.Profile {
    public class SetCoinProfilePropertyRequest : RequestBase, IGetSignData {
        public SetCoinProfilePropertyRequest() { }
        public Guid WorkId { get; set; }
        public Guid CoinId { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(WorkId)).Append(WorkId)
              .Append(nameof(CoinId)).Append(CoinId)
              .Append(nameof(PropertyName)).Append(PropertyName)
              .Append(nameof(Value)).Append(Value);
            return sb;
        }
    }
}
