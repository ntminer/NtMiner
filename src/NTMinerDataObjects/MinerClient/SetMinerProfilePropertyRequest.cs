using System.Text;

namespace NTMiner.MinerClient {
    public class SetMinerProfilePropertyRequest : RequestBase, IGetSignData {
        public SetMinerProfilePropertyRequest() { }
        public string ClientIp { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
