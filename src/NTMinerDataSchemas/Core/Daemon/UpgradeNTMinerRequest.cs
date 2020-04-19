using System.Text;

namespace NTMiner.Core.Daemon {
    public class UpgradeNTMinerRequest : IRequest, ISignableData {
        public string NTMinerFileName { get; set; }

        public UpgradeNTMinerRequest() { }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
