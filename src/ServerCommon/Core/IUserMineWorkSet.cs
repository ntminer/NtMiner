using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IUserMineWorkSet {
        void AddOrUpdate(UserMineWorkData data);
        void RemoveById(Guid id);
        UserMineWorkData GetById(Guid id);
        List<UserMineWorkData> GetsByLoginName(string loginName);
    }
}
