using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IGroupSet : ICountSet {
        bool Contains(Guid groupId);
        bool TryGetGroup(Guid groupId, out IGroup group);
        IEnumerable<IGroup> AsEnumerable();
    }
}
