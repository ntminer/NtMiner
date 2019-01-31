using LiteDB;

namespace NTMiner.User {
    public interface IUser {
        ObjectId Id { get; }

        string LoginName { get; }

        /// <summary>
        /// 签名用，并非RSA privateKey
        /// </summary>
        string Password { get; }

        string Description { get; }
    }
}
