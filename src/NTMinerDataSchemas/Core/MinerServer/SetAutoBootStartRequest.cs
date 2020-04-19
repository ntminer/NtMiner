using System.Text;

namespace NTMiner.Core.MinerServer {
    public class SetAutoBootStartRequest : IRequest, ISignableData {
        public bool AutoBoot { get; set; }
        public bool AutoStart { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
