using System;

namespace NTMiner.User {
    public class UserUpdateInput : UserUpdateData {
        public UserUpdateInput() { }

        public Guid ActionCaptchaId { get; set; }
        /// <summary>
        /// 动作验证码，动作指的是SignUp和UpdateUser
        /// </summary>
        public string ActionCaptcha { get; set; }
        public string EmailCodeId { get; set; }
        /// <summary>
        /// 邮箱验证码，填邮箱收到的验证码。注册时如果填了邮箱则验证邮箱的验证码，更新用户信息时如果变动了邮箱则验证邮箱验证码。
        /// 验证验证码的存在性以及邮箱地址和验证码的对应关系的正确性而不是只验证验证码的存在性。
        /// </summary>
        public string EmailCode { get; set; }
        public string MobileCodeId { get; set; }
        /// <summary>
        /// 手机验证码，填手机收到的验证码。注册时如果填写了手机则验证手机验证码，更新用户信息时如果变动了手机则验证手机验证码。
        /// 验证验证码的存在性以及手机和验证码的对应关系的正确性而不是只验证验证码的存在性。
        /// </summary>
        public string MobileCode { get; set; }
    }
}
