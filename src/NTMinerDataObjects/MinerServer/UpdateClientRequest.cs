using System.Text;

namespace NTMiner.MinerServer {
    public class UpdateClientRequest : RequestBase, IGetSignData {
        public UpdateClientRequest() { }
        public string ObjectId { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(ObjectId)).Append(ObjectId)
              .Append(nameof(PropertyName)).Append(PropertyName)
              .Append(nameof(Value)).Append(Value);
            return sb;
        }
    }
}
