using System;

namespace NTMiner.User {
    public interface ISignUpInput {
        string LoginName { get; }

        string Password { get; }

        string PasswordAgain { get; }
        Guid ActionCaptchaId { get; }
        /// <summary>
        /// 动作验证码
        /// </summary>
        string ActionCaptcha { get; }
    }
}
