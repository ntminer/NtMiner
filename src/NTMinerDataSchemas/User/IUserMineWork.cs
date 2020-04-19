using NTMiner.Core;

namespace NTMiner.User {
    public interface IUserMineWork : IMineWork {
        string LoginName { get; }
    }
}
