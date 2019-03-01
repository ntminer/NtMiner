using System.Collections.Generic;

namespace NTMiner.User {
    public interface IUserSet : IEnumerable<IUser> {
        bool Contains(string loginName);
        bool TryGetKey(string loginName, out IUser user);
    }
}
