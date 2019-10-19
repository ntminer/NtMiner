using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IKernelOutputKeywordSet : IEnumerable<IKernelOutputKeyword> {
        bool Contains(Guid kernelOutputId, string keyword);
        IEnumerable<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId);
        bool TryGetKernelOutputKeyword(Guid id, out IKernelOutputKeyword keyword);
    }
}
