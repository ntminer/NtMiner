using System.Text;

namespace NTMiner {
    public class SignatureRequest : RequestBase, ISignatureRequest {
        public SignatureRequest() { }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
