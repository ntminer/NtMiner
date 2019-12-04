using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ISysDicItemSet {
        bool ContainsKey(Guid dicItemId);
        bool ContainsKey(Guid dicId, string dicItemCode);
        bool ContainsKey(string dicCode, string dicItemCode);
        bool TryGetDicItem(Guid dicItemId, out ISysDicItem dicItem);
        bool TryGetDicItem(string dicCode, string dicItemCode, out ISysDicItem dicItem);
        bool TryGetDicItem(Guid dicId, string dicItemCode, out ISysDicItem dicItem);
        IEnumerable<ISysDicItem> GetSysDicItems(string dicCode);
        IEnumerable<ISysDicItem> AsEnumerable();
    }
}
