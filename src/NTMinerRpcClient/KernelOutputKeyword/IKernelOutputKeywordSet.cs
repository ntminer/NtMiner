using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.KernelOutputKeyword {
    public interface IKernelOutputKeywordSet {
        List<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId);
        IEnumerable<IKernelOutputKeyword> AsEnumerable();
    }
}
