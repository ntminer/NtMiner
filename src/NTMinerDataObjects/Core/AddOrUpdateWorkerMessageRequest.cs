using System.Text;

namespace NTMiner.Core {
    public class AddOrUpdateWorkerMessageRequest : RequestBase, IGetSignData {
        public AddOrUpdateWorkerMessageRequest() {
        }

        public string MessageType { get; set; }
        public string Content { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
