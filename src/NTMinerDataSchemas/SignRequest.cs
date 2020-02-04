using System.Text;

namespace NTMiner {
    public class SignRequest : RequestBase, IGetSignData {
        public SignRequest() { }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
