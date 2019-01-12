using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputSet : IEnumerable<IKernelOutput> {
        bool TryGetKernelOutput(Guid id, out IKernelOutput kernelOutput);
    }
}
