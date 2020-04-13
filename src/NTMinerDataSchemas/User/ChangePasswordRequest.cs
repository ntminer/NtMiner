using System.Text;

namespace NTMiner.User {
    /// <summary>
    /// User修改密码都是使用这个模型
    /// 注意：基于Url Query中传递的LoginName、Timestamp、Sign验证用户，所以该模型只有NewPassword字段没有别的
    /// </summary>
    public class ChangePasswordRequest : IRequest, ISignableData {
        public ChangePasswordRequest() { }
        public string NewPassword { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
