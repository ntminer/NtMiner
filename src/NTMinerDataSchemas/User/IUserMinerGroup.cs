using NTMiner.Core;

namespace NTMiner.User {
    public interface IUserMinerGroup : IMinerGroup {
        string LoginName { get; }
    }
}
