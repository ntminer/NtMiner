using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelSet  : ICountSet {
        bool Contains(Guid kernelId);
        bool TryGetKernel(Guid kernelId, out IKernel kernel);
        IEnumerable<IKernel> AsEnumerable();
    }
}
