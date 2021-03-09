using System;

namespace NTMiner.User {
    public interface IUser : ILoginedUser, ISignUser {
        string PublicKey { get; }
        string PrivateKey { get; }
        DateTime LastLogin { get; }
    }
}
