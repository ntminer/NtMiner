using System.Collections.Generic;

namespace NTMiner.User {
    public interface IUserSet {
        IUser GetUser(string loginName);
        IEnumerable<IUser> AsEnumerable();
    }
}
