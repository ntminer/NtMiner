using System.Text;

namespace NTMiner.Daemon {
    public class UpgradeNTMinerRequest : RequestBase, ISignatureRequest {
        public string NTMinerFileName { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(NTMinerFileName)).Append(NTMinerFileName)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
