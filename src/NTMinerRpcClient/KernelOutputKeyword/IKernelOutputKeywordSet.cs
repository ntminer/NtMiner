using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.KernelOutputKeyword {
    public interface IKernelOutputKeywordSet : IEnumerable<IKernelOutputKeyword> {
        bool Contains(Guid kernelOutputId, string keyword);
        IEnumerable<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId);
        bool TryGetKernelOutputKeyword(Guid id, out IKernelOutputKeyword keyword);
    }
}
