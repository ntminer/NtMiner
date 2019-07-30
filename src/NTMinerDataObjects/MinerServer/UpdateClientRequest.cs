using System.Text;

namespace NTMiner.MinerServer {
    public class UpdateClientRequest : RequestBase, IGetSignData {
        public UpdateClientRequest() { }
        public string ObjectId { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
