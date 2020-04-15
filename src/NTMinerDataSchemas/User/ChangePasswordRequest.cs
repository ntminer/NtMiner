using System;
using System.Text;

namespace NTMiner.User {
    /// <summary>
    /// 注意：基于Url Query中传递的LoginName、Timestamp、Sign验证用户，所以该模型只有NewPassword字段没有别的
    /// </summary>
    public class ChangePasswordRequest : IRequest, ISignableData {
        public ChangePasswordRequest() { }
        public string NewPassword { get; set; }
        public Guid ActionCaptchaId { get; set; }
        public string ActionCaptcha { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
