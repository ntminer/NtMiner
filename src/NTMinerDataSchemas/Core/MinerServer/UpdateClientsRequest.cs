using System.Collections.Generic;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class UpdateClientsRequest : RequestBase, IGetSignData {
        public UpdateClientsRequest() {
            this.Values = new Dictionary<string, object>();
        }
        public string PropertyName { get; set; }
        [ManualSign]
        public Dictionary<string, object> Values { get; set; }

        private string GetValuesString() {
            if (Values == null || Values.Count == 0) {
                return string.Empty;
            }
            return $"{string.Join(",", Values.Keys)}:{string.Join(",", Values.Values)}";
        }

        public StringBuilder GetSignData() {
            StringBuilder sb = this.BuildSign();
            sb.Append(nameof(Values)).Append(GetValuesString());
            return sb;
        }
    }
}
