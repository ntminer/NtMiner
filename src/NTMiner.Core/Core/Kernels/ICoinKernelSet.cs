using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface ICoinKernelSet : IEnumerable<ICoinKernel> {
        int Count { get; }
        bool Contains(Guid coinKernelId);
        bool TryGetKernel(Guid coinKernelId, out ICoinKernel coinKernel);
    }
}
