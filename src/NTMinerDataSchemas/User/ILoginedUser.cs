using System;

namespace NTMiner.User {
    /// <summary>
    /// 该类型的意义是去掉密码成员
    /// </summary>
    public interface ILoginedUser : ILoginName {
        /// <summary>
        /// 注册时可不填，如果填了或者补填了则支持Email登录，因为用于登录所以注册时要保证Email的唯一性且要验证Email的收件能力。
        /// </summary>
        string Email { get; }
        /// <summary>
        /// 注册时可不填，如果填了或者补填了则支持手机号码登录，因为用于登录所以注册时要保证Mobile的唯一性且要验证Mobile的收件能力。
        /// </summary>
        string Mobile { get; }
        bool IsEnabled { get; }
        string Roles { get; }
        string Description { get; }
        DateTime CreatedOn { get; }
    }
}