using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IPoolKernelSet  : ICountSet {
        bool Contains(Guid poolKernelId);
        bool TryGetPoolKernel(Guid poolKernelId, out IPoolKernel poolKernel);
        IEnumerable<IPoolKernel> AsEnumerable();
    }
}
