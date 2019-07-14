using System.Text;

namespace NTMiner.Daemon {
    public class UpgradeNTMinerRequest : RequestBase, IGetSignData {
        public string NTMinerFileName { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(NTMinerFileName)).Append(NTMinerFileName);
            return sb;
        }
    }
}
