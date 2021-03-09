using System.Text;

namespace NTMiner.ServerNode {
    public class QueryActionCountsRequest : IPagingRequest, ISignableData {
        public QueryActionCountsRequest() { }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }

        public StringBuilder GetSignData() {
            return this.GetActionIdSign("3902609F-063C-46BB-80B7-BDA059F1B53F");
        }
    }
}
