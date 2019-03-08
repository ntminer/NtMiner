using System.Collections.Generic;

namespace NTMiner.User {
    public interface IUserSet : IEnumerable<IUser> {
        bool TryGetUser(string loginName, out IUser user);
    }
}
