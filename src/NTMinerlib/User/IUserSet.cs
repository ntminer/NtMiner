using NTMiner;

namespace NTMiner.User {
    public interface IUserSet {
        bool Contains(string loginName);
        bool TryGetKey(string loginName, out IUser user);
    }
}
