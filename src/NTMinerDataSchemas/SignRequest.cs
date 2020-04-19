using System.Text;

namespace NTMiner {
    /// <summary>
    /// 这个类型虽然没有成员但也是有用的，用这个类型作为参数表示需要签名。
    /// </summary>
    public class SignRequest : IRequest, ISignableData {
        public SignRequest() { }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
