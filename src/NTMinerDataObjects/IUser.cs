namespace NTMiner {
    public interface IUser {
        string LoginName { get; }

        /// <summary>
        /// 签名用，并非RSA privateKey
        /// </summary>
        string Password { get; }

        bool IsEnabled { get; }

        string Description { get; }
    }
}
