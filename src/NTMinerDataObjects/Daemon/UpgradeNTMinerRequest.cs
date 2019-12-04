using System.Text;

namespace NTMiner.Daemon {
    public class UpgradeNTMinerRequest : RequestBase, IGetSignData {
        public string NTMinerFileName { get; set; }

        public UpgradeNTMinerRequest() { }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
