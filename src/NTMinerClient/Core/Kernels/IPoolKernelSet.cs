using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IPoolKernelSet {
        int Count { get; }
        bool Contains(Guid poolKernelId);
        bool TryGetPoolKernel(Guid poolKernelId, out IPoolKernel poolKernel);
        IEnumerable<IPoolKernel> AsEnumerable();
    }
}
