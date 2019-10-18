using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IKernelOutputKeywordSet : IEnumerable<IKernelOutputKeyword> {
        bool Contains(string keyword);
        bool Contains(Guid id);
        bool TryGetKernelOutputKeyword(Guid id, out IKernelOutputKeyword keyword);
    }
}
