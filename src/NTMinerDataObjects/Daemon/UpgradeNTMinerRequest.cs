using System.Text;

namespace NTMiner.Daemon {
    public class UpgradeNTMinerRequest : RequestBase, IGetSignData {
        public string NTMinerFileName { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
