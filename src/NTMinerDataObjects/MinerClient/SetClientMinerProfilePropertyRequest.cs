using System.Text;

namespace NTMiner.MinerClient {
    public class SetClientMinerProfilePropertyRequest : RequestBase, IGetSignData {
        public SetClientMinerProfilePropertyRequest() { }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(PropertyName)).Append(PropertyName)
              .Append(nameof(Value)).Append(Value);
            return sb;
        }
    }
}
