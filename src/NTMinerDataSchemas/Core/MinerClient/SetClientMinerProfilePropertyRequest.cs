using System.Text;

namespace NTMiner.Core.MinerClient {
    public class SetClientMinerProfilePropertyRequest : RequestBase, IGetSignData {
        public SetClientMinerProfilePropertyRequest() { }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
