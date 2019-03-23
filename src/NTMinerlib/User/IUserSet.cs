using System.Collections.Generic;

namespace NTMiner.User {
    public interface IUserSet : IEnumerable<IUser> {
        IUser GetUser(string loginName);
    }
}
