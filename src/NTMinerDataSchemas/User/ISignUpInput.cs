using System;

namespace NTMiner.User {
    public interface ISignUpInput {
        string LoginName { get; }

        string Email { get; }

        string Mobile { get; }

        string Password { get; }

        string PasswordAgain { get; }
        Guid ActionCaptchaId { get; }
        /// <summary>
        /// 动作验证码
        /// </summary>
        string ActionCaptcha { get; }
        /// <summary>
        /// 邮箱验证码，填邮箱收到的验证码。注册时如果填了邮箱则验证邮箱的验证码，更新用户信息时如果变动了邮箱则验证邮箱验证码。
        /// 验证验证码的存在性以及邮箱地址和验证码的对应关系的正确性而不是只验证验证码的存在性。
        /// </summary>
        string EmailCode { get; }
        /// <summary>
        /// 手机验证码，填手机收到的验证码。注册时如果填写了手机则验证手机验证码，更新用户信息时如果变动了手机则验证手机验证码。
        /// 验证验证码的存在性以及手机和验证码的对应关系的正确性而不是只验证验证码的存在性。
        /// </summary>
        string MobileCode { get; }
    }
}
