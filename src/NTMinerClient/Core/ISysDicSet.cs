using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ISysDicSet {
        bool ContainsKey(Guid sysDicId);
        bool ContainsKey(string sysDicCode);
        bool TryGetSysDic(Guid sysDicId, out ISysDic sysDic);
        bool TryGetSysDic(string sysDicCode, out ISysDic sysDic);
        IEnumerable<ISysDic> AsEnumerable();
    }
}
