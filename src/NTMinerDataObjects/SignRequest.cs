using System.Text;

namespace NTMiner {
    public class SignRequest : RequestBase, IGetSignData {
        public SignRequest() { }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
