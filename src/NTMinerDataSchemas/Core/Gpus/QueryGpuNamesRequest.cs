using System.Text;

namespace NTMiner.Core.Gpus {
    public class QueryGpuNamesRequest : IPagingRequest, ISignableData {
        public QueryGpuNamesRequest() { }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }

        public StringBuilder GetSignData() {
            return this.GetActionIdSign("4FBC19D4-55AB-4827-BEF4-A24D28D620CE");
        }
    }
}
