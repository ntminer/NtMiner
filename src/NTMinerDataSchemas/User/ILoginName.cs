namespace NTMiner.User {
    /// <summary>
    /// 因为IUser同时继承ISignUser和ILoginedUserData，引入ILoginName而不是让ISignUser和
    /// ILoginedUserData具有重名的LoginName属性是为了避免IUser访问LoginName时的强制类型转换。
    /// </summary>
    public interface ILoginName {
        /// <summary>
        /// 登录是必填的，注册时只需验证登录名的唯一性即可开始使用，为了防止非人类注册则需要用验证码验证人类。
        /// </summary>
        string LoginName { get; }
    }
}
