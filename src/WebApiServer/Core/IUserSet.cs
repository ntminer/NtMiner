using NTMiner.User;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IUserSet : IReadOnlyUserSet {
        List<UserData> QueryUsers(QueryUsersRequest query, out int total);
        void Add(UserData input);
        void Update(UserUpdateData input);
        void Remove(string loginName);
        void Enable(string loginName);
        void Disable(string loginName);
        void AddAdminRole(string loginName);
        void RemoveAdminRole(string loginName);
        void ChangePassword(string loginName, string newPassword);
        bool Contains(string loginName);
    }
}
