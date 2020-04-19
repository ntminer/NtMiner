namespace NTMiner.User {
    public interface ISignUser : ILoginName {
        /// <summary>
        /// 矿工（人）登录后台时的密码的sha1
        /// </summary>
        string Password { get; }
    }
}
