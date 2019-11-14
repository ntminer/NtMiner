using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.KernelOutputKeyword {
    public interface IKernelOutputKeywordSet : IEnumerable<IKernelOutputKeyword> {
        List<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId);
    }
}
