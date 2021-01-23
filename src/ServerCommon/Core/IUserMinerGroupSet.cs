using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IUserMinerGroupSet {
        void AddOrUpdate(UserMinerGroupData data);
        void RemoveById(Guid id);
        UserMinerGroupData GetById(Guid id);
        List<UserMinerGroupData> GetsByLoginName(string loginName);
    }
}
