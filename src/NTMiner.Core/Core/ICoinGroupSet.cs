using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ICoinGroupSet : IEnumerable<ICoinGroup> {
        List<Guid> GetGroupCoinIds(Guid groupId);
    }
}
