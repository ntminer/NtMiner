using System.Collections.Generic;
using System.Text;

namespace NTMiner.MinerServer {
    public class UpdateClientsRequest : RequestBase, ISignatureRequest {
        public UpdateClientsRequest() {
            this.Values = new Dictionary<string, object>();
        }
        public string PropertyName { get; set; }
        public Dictionary<string, object> Values { get; set; }

        private string GetValuesString() {
            if (Values == null || Values.Count == 0) {
                return string.Empty;
            }
            return $"{string.Join(",", Values.Keys)}:{string.Join(",", Values.Values)}";
        }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(PropertyName)).Append(PropertyName)
                .Append(nameof(Values)).Append(GetValuesString())
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
